using PrimeTween;
using System.Collections;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackRenderer trackRenderer;

    [Header("Settings")]
    [Tooltip("Must be 0 or 1")]
    [SerializeField, Range(0, 1)] int playerId = 0;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask boostLayer1;
    [SerializeField] LayerMask boostLayer2;
    [SerializeField] float fallingSpeed = 5.0f;
    [SerializeField] float jumpTime = 0.25f;

    [HideInInspector] public bool isBoosting = false;
    [HideInInspector] public int maxOffset = 1;

    LayerMask boostLayerChecked;
    Vector3 resettLocalPos;
    Vector3 checkBoxCenter;
    Vector3 checkBoxExtents = new Vector3(0.25f, 0.5f, 0.05f);
    float offsetLen;
    float inputDir = 0.0f;
    int actualOffset = 0;
    bool isReady = false;
    bool isJumping = false;
    bool isFalling = false;


    private void Start()
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
        checkBoxCenter = transform.position - (transform.up * transform.localPosition.y);

        if (!isJumping && !isFalling)
            CheckGround();

        if (isFalling)
            Fall();

        if (isBoostActionPressed)
            CheckBooster();

        if (!isJumping && !isFalling && isJumpActionReleased)
            LaunchJump(); 
    }


    void CheckGround()
    {
        if (Physics.CheckBox(checkBoxCenter, checkBoxExtents, transform.rotation, groundLayer))
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
            
        if (Physics.CheckBox(checkBoxCenter, checkBoxExtents, transform.rotation, boostLayerChecked))
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

        Tween.LocalPositionX(
            transform,
            transform.localPosition.x,
            transform.localPosition.x + (offsetLen * inputDir),
            jumpTime,
            ease: Ease.OutQuart);
    }


    IEnumerator LauchReadyTimer()
    {
        yield return new WaitForSeconds(0.1f);
        isReady = true;
    }
}
