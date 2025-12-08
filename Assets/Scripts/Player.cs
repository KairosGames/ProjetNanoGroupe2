using PrimeTween;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.GraphicsBuffer;

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
    Vector3 checkBoxExtents = new Vector3(0.25f, 0.5f, 0.05f);
    Vector3 resetVisualScale;
    Vector3 targetTryBoostScale;
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


    FMOD.Studio.EventInstance movingSound;
    FMOD.Studio.EventInstance movingBoostSound;
    FMOD.Studio.EventInstance boostingSound;
    FMOD.Studio.EventInstance boostingBothSound;
    string boostParamName;

    private void Awake()
    {
        carParent = transform.parent.GetComponent<FollowingSpline>();
        resetVisualScale = childVisual.transform.localScale;
        targetTryBoostScale = resetVisualScale * 0.66f;
        boostLayerChecked = playerId == 0 ? boostLayer1 : boostLayer2;
        offsetLen = trackRenderer.offset;
        actualOffset = playerId == 0 ? -1 : 1;

        LoadAllSounds();
        StartCoroutine(LauchReadyTimer());
    }


    void Update()
    {
        SetActiveOnTrack();

        if (!isReady)
            return;

        isBoostActionPressed = playerId == 0 ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.Joystick2Button0);
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
            SetRotation();
            CheckGround();
        }

        if (isFalling)
            Fall();

        if (isBoostActionPressed)
        {
            CheckBooster();
            TryBoostEffect();
        }
        else
        {
            ResetTryBoostEffect();
            isBoosting = false;
        }

        if (!isJumping && !isFalling && isJumpActionReleased)
            LaunchJump(); 
    }

    void LoadAllSounds()
    {
        movingSound = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/moving");
        movingBoostSound = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Boost");
        string boostPath = playerId == 0 ? "event:/Avatar/Combot/Barre_combot_1" : "event:/Avatar/Combot/Barre_combot_2";
        boostParamName = playerId == 0 ? "is_boosting_1" : "is_boosting_2";
        boostingSound = FMODUnity.RuntimeManager.CreateInstance(boostPath);
        boostingBothSound = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Combot/Barre_combot_3");
    }


    void SetActiveOnTrack()
    {
        bool isActive = !isJumping && !isFalling && !isRespawning;
        float movParam = isActive ? 1.0f : 0.0f;

        foreach (GameObject go in activeOnTrack)
            go.SetActive(isActive);

        /*FMODUnity.RuntimeManager.StudioSystem.setParameterByName("is_moving", movParam);
        Debug.Log(movParam);
        if (!IsSoundPlaying(movingSound) && isActive)
        {
            movingSound.start();
            Debug.Log("OUUUI !");
        }*/

        bool isBothBoost = isBoosting && otherPlayer.isBoosting && otherPlayer.actualOffset == actualOffset;
        float boostParam = isBoosting ? (isBothBoost ? 0.0f : 1.0f) : 0.0f;
        float isBothBoostParam = isBothBoost ? 1.0f : 0.0f;

        /*FMODUnity.RuntimeManager.StudioSystem.setParameterByName(boostParamName, boostParam);
        if (!IsSoundPlaying(boostingSound) && isBoosting && !isBothBoost)
            boostingSound.start();*/

        /*FMODUnity.RuntimeManager.StudioSystem.setParameterByName("is_bothboosting", isBothBoostParam);
        Debug.Log(isBothBoostParam);
        if (!IsSoundPlaying(boostingBothSound) && isBothBoost)
        {
            boostingBothSound.start();
            Debug.Log("oui");
        }*/
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


    void SetRotation()
    {
        float targetZ = inputDir * -30.0f;
        Vector3 eul = transform.localEulerAngles;
        eul.z = Mathf.LerpAngle(eul.z, targetZ, 10f * Time.deltaTime);
        transform.localEulerAngles = eul;
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
        GoToResetRotation();
    }


    void GoToResetRotation()
    {
        Vector3 eul = transform.localEulerAngles;
        eul.z = Mathf.LerpAngle(eul.z, 0.0f, 10f * Time.deltaTime);
        transform.localEulerAngles = eul;
    }


    void LaunchJump()
    {
        if (inputDir != 0)
            LaunchSideStep();

        isJumping = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/Jump");

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


    void TryBoostEffect()
    {
        childVisual.transform.localScale = Vector3.Lerp(childVisual.transform.localScale, targetTryBoostScale, Time.deltaTime * 10.0f);
    }


    void ResetTryBoostEffect()
    {
        childVisual.transform.localScale = Vector3.Lerp(childVisual.transform.localScale, resetVisualScale, Time.deltaTime * 10.0f);
    }



    void DownFromJump()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Avatar/land");

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


    bool IsSoundPlaying(FMOD.Studio.EventInstance fmodEventInstance)
    {
        fmodEventInstance.getPlaybackState(out var state);
        return state == FMOD.Studio.PLAYBACK_STATE.PLAYING;
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
        transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);

        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            localYResetSpawn,
            0.5f,
            ease: Ease.OutQuint
            ).OnComplete(() => { canChoose = true; });
    }
}
