using PrimeTween;
using System.Collections;
using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackRenderer trackRenderer;

    [Header("Settings")]
    [Tooltip("Must be 0 or 1")]
    [SerializeField, Range(0, 1)] int playerId = 0;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask boostLayer;
    [SerializeField] float fallingSpeed = 5.0f;
    [SerializeField] float jumpTime = 0.25f;


    bool isInBoost = false;
    [HideInInspector] public bool isBoosting = false;

    float offsetLen;
    float inputDir = 0.0f;
    int maxOffset = 1;
    int actualOffset = 0;
    bool isDectingGround = false;
    bool isJumping = false;
    bool isFalling = false;

    private void Start()
    {
        offsetLen = trackRenderer.offset;
        StartCoroutine(LauchGroundDetectionTimer());

        foreach (var name in Input.GetJoystickNames())
            Debug.Log("Joystick détecté : " + name);
    }

    void Update()
    {
        bool isJumpActionPressed = playerId == 0 ? Input.GetKey(KeyCode.Joystick1Button0) : Input.GetKey(KeyCode.Joystick2Button0);
        bool isJumpActionReleased = playerId == 0 ? Input.GetKeyUp(KeyCode.Joystick1Button0) : Input.GetKeyUp(KeyCode.Joystick2Button0);
        inputDir = Input.GetAxis($"Horizontal_P{playerId + 1}");
        inputDir = Mathf.Abs(inputDir) >= 0.6f ? Mathf.Sign(inputDir) : 0.0f;

        if (!isJumping && !isFalling && isJumpActionReleased)
            LaunchJump();

        if (!isJumping && !isFalling && isDectingGround)
        {
            CheckGround();

            CheckInBoost();
            CheckIsBoosting();
        }

        if (isFalling)
            Fall();
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

    void CheckGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1.0f, groundLayer))
            return;

        isFalling = true;
    }

    void Fall()
    {
        transform.localPosition -= new Vector3(0.0f, fallingSpeed, 0.0f) * Time.deltaTime;
    }

    IEnumerator LauchGroundDetectionTimer()
    {
        yield return new WaitForSeconds(0.1f);
        isDectingGround = true;
    }




    void CheckInBoost()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1.0f, boostLayer))
            isInBoost = true;
        else
        {
            isInBoost = false;
            isBoosting = false;
        }
    }

    void CheckIsBoosting()
    {
        if (isInBoost && Input.GetKey(KeyCode.Space))
            isBoosting = true;
        else
            isBoosting = false;

    }
}
