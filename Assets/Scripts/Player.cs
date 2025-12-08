using PrimeTween;
using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackRenderer trackRenderer;
    [SerializeField] GameObject childVisual;
    [SerializeField] Player otherPlayer;

    [Header("Settings")]
    [Tooltip("Must be 0 or 1")]
    [SerializeField, Range(0, 1)] int playerId = 0;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask boostLayer1;
    [SerializeField] LayerMask boostLayer2;
    [SerializeField] LayerMask boostLayerBoth;
    [SerializeField] float fallingSpeed = 5.0f;
    [SerializeField] float jumpTime = 0.25f;

    [System.NonSerialized] public int maxOffset = 2;
    [HideInInspector] public bool isBoosting = false;
    

    LayerMask boostLayerChecked;
    Vector3 resettLocalPos;
    Vector3 checkBoxExtents = new Vector3(0.25f, 0.5f, 0.05f);
    float offsetLen;
    float inputDir = 0.0f;
    int actualOffset = 0;
    bool isReady = false;
    bool isJumping = false;
    bool isFalling = false;


    [Header("MovmentSmoothness")]
    [SerializeField] float tWeight = 0.08f;
    float lastT = 0.0f;
    float tVel = 0.0f;

    private void Awake()
    {
        boostLayerChecked = playerId == 0 ? boostLayer1 : boostLayer2;
        resettLocalPos = transform.localPosition;
        offsetLen = trackRenderer.offset;
        StartCoroutine(LauchReadyTimer());
    }


    void Update()
    {
        if (!isReady)
            return;

        bool isBoostActionPressed = playerId == 0 ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.Joystick2Button0);
        bool isJumpActionReleased = playerId == 0 ? Input.GetKeyUp(KeyCode.Joystick1Button0) : Input.GetKeyUp(KeyCode.Joystick2Button0);
        inputDir = Input.GetAxis($"Horizontal_P{playerId + 1}");
        inputDir = Mathf.Abs(inputDir) >= 0.6f ? Mathf.Sign(inputDir) : 0.0f;

        if (!isFalling)
            SetPosition();

        if (!isJumping && !isFalling)
            CheckGround();

        if (isFalling)
            Fall();

        if (isBoostActionPressed)
            CheckBooster();

        if (!isJumping && !isFalling && isJumpActionReleased)
            LaunchJump(); 
    }


    void SetPosition()
    {
        Vector3 carPos = transform.parent.position;
        SplineContainer actualSpline = trackRenderer.entireSplines[actualOffset + 2];
        Vector3 carLocalPos = actualSpline.transform.InverseTransformPoint(carPos);
        SplineUtility.GetNearestPoint(actualSpline.Spline, carLocalPos, out var nearestLocal, out float t);
        if (lastT == 0.0f) lastT = t;
        lastT = Mathf.SmoothDamp(lastT, t, ref tVel, tWeight);
        actualSpline.Evaluate(lastT, out var localPosFromSpline, out var dir, out var up);
        Vector3 globalPosTarget = actualSpline.transform.TransformPoint(localPosFromSpline);
        Vector3 localPosTarget = transform.parent.InverseTransformPoint(globalPosTarget);
        transform.localPosition = Vector3.Lerp(transform.localPosition, localPosTarget, 0.05f);
    }


    void CheckGround()
    {
        if (Physics.CheckBox(transform.position, checkBoxExtents, transform.rotation, groundLayer))
            return;

        isFalling = true;
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
        //lastT = 0.0f;

        /*Tween.LocalPositionX(
            transform,
            transform.localPosition.x,
            transform.localPosition.x + (offsetLen * inputDir),
            jumpTime,
            ease: Ease.OutQuart);*/
    }


    IEnumerator LauchReadyTimer()
    {
        yield return new WaitForSeconds(0.1f);
        isReady = true;
    }
}
