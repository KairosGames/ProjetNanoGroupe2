using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackRenderer trackRenderer;
    [SerializeField] Player otherPlayer;

    [Header("InterReferences")]
    [SerializeField] GameObject[] activeOnTrack;
    [SerializeField] GameObject childVisual;

    [Header("Settings")]
    [Tooltip("Must be 0 or 1")]
    [SerializeField, Range(0, 1)] int playerId = 0;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask boostLayer1;
    [SerializeField] LayerMask boostLayer2;
    [SerializeField] LayerMask boostLayerBoth;
    [SerializeField] float fallingSpeed = 5.0f;
    [SerializeField] float jumpTime = 0.25f;
    [SerializeField] float respawnTime = 1.0f;

    [System.NonSerialized] public int maxOffset = 1;
    [HideInInspector] public bool isBoosting = false;
    [HideInInspector] public bool isBoostActionPressed = false;


    FollowingSpline carParent;
    LayerMask boostLayerChecked;
    Vector3 resettLocalPos;
    Vector3 checkBoxExtents = new Vector3(0.25f, 0.5f, 0.05f);
    float offsetLen;
    float inputDir = 0.0f;
    float localYResetSpawn = 2.0f;
    float lastT = 0.0f;
    float tVel = 0.0f;
    int actualOffset = 0;
    bool isReady = false;
    bool isJumping = false;
    bool isFalling = false;
    bool isRespawning = false;
    bool canChoose = false;


    private void Awake()
    {
        carParent = transform.parent.GetComponent<FollowingSpline>();
        boostLayerChecked = playerId == 0 ? boostLayer1 : boostLayer2;
        resettLocalPos = transform.localPosition;
        offsetLen = trackRenderer.offset;
        actualOffset = playerId == 0 ? -1 : 1;
        StartCoroutine(LauchReadyTimer());
    }


    void Update()
    {
        SetActiveOnTrack();

        if (!isReady)
            return;

        bool isBoostActionPressed = playerId == 0 ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.Joystick2Button0);
        bool isChooseActionJustPressed = playerId == 0 ? Input.GetKeyDown(KeyCode.Joystick1Button0) : Input.GetKeyDown(KeyCode.Joystick2Button0);
        bool isJumpActionReleased = playerId == 0 ? Input.GetKeyUp(KeyCode.Joystick1Button0) : Input.GetKeyUp(KeyCode.Joystick2Button0);
        inputDir = Input.GetAxis($"Horizontal_P{playerId + 1}");
        inputDir = Mathf.Abs(inputDir) >= 0.6f ? Mathf.Sign(inputDir) : 0.0f;

        if (isRespawning)
        {
            ChooseLane(isChooseActionJustPressed);
            return;
        }

        if (!isJumping && !isFalling)
        {
            SetPosition();
            CheckGround();
        }

        if (isFalling)
            Fall();

        if (isBoostActionPressed)
            CheckBooster();

        if (!isJumping && !isFalling && isJumpActionReleased)
            LaunchJump(); 
    }


    void SetActiveOnTrack()
    {
        foreach (GameObject go in activeOnTrack)
            go.SetActive(!isJumping && !isFalling && !isRespawning);
    }


    void SetPosition()
    {
        Vector3 carPos = transform.parent.position;
        SplineContainer actualSpline = trackRenderer.entireSplines[actualOffset + 2];
        Vector3 carLocalPos = actualSpline.transform.InverseTransformPoint(carPos);
        SplineUtility.GetNearestPoint(actualSpline.Spline, carLocalPos, out var nearestLocal, out float t);
        if (lastT == 0.0f) lastT = t;
        lastT = Mathf.SmoothDamp(lastT, t, ref tVel, carParent.tSmooth);
        actualSpline.Evaluate(lastT, out var localPosFromSpline, out var dir, out var up);
        Vector3 globalPosTarget = actualSpline.transform.TransformPoint(localPosFromSpline);
        Vector3 localPosTarget = transform.parent.InverseTransformPoint(globalPosTarget);
        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosTarget, 0.2f);
    }


    void CheckGround()
    {
        if (Physics.CheckBox(transform.position, checkBoxExtents, transform.rotation, groundLayer))
            return;

        isFalling = true;
        StartCoroutine(LaunchRespawnTimer());
    }


    void Fall()
    {
        transform.localPosition -= new Vector3(0.0f, fallingSpeed, 0.0f) * Time.deltaTime;
    }


    void LaunchJump()
    {
        if (inputDir != 0)
            LaunchSideStep();

        isJumping = true;

        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            transform.localPosition.y + 1.0f,
            jumpTime / 2.0f,
            ease: Ease.OutQuart).OnComplete(() => { DownFromJump(); });
    }


    void CheckBooster()
    {
        if (isFalling || isJumping)
            return;
            
        if (Physics.CheckBox(transform.position, checkBoxExtents, transform.rotation, boostLayerChecked))
        {
            isBoosting = true;
            return;
        }

        if (Physics.CheckBox(transform.position, checkBoxExtents, transform.rotation, boostLayerBoth) && otherPlayer.actualOffset == actualOffset && otherPlayer.isBoostActionPressed)
        {
            isBoosting = true;
            return;
        }

        isBoosting = false;
    }


    void DownFromJump()
    {
        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            transform.localPosition.y - 1.0f,
            jumpTime / 2.0f,
            ease: Ease.InQuart).OnComplete(() => { isJumping = false; });
    }


    void LaunchSideStep()
    {
        if (Mathf.Abs(actualOffset + inputDir) > maxOffset)
            return;

        actualOffset += (int)inputDir;
        lastT = 0.0f;

        Tween.LocalPositionX(
            transform,
            transform.localPosition.x,
            transform.localPosition.x + (offsetLen * inputDir),
            jumpTime,
            ease: Ease.OutQuart);
    }


    void ChooseLane(bool isChooseActionJustPressed)
    {
        if (isChooseActionJustPressed && canChoose)
        {
            canChoose = false;

            Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            0.0f,
            0.4f,
            ease: Ease.InQuart
            ).OnComplete(() => { isRespawning = false; });

            return;
        }

        if (canChoose && inputDir != 0.0f)
        {
            if (Mathf.Abs(actualOffset + inputDir) > maxOffset)
                return;

            actualOffset += (int)inputDir;
            canChoose = false;

            Tween.LocalPositionX(
            transform,
            transform.localPosition.x,
            transform.localPosition.x + (offsetLen * inputDir),
            jumpTime,
            ease: Ease.OutQuart).OnComplete(() => { canChoose = true; });
        }
    }


    IEnumerator LauchReadyTimer()
    {
        yield return new WaitForSeconds(0.1f);
        isReady = true;
    }


    IEnumerator LaunchRespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        isFalling = false;
        isBoosting = false;
        isRespawning = true;
        actualOffset = 0;
        lastT = 0.0f;
        transform.localPosition = new Vector3(0.0f, 6.0f, 0.0f);

        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            localYResetSpawn,
            0.5f,
            ease: Ease.OutQuint
            ).OnComplete(() => { canChoose = true; });
    }
}
