using PrimeTween;
using System.Collections;
using TreeEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackRenderer trackRenderer;

    [Header("Settings")]
    [SerializeField] int playerId = 0;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float fallingSpeed = 5.0f;

    float offsetLen;
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

        if (Input.GetKey(KeyCode.Joystick1Button0))
            Debug.Log("Player 1!");

        if (Input.GetKey(KeyCode.Joystick2Button0))
            Debug.Log("Player 2 !");

        if (Mathf.Abs(Input.GetAxis("Horizontal_P1")) >= 0.2f)
            Debug.Log(Input.GetAxis("Horizontal_P1"));

        if (Mathf.Abs(Input.GetAxis("Horizontal_P2")) >= 0.2f)
            Debug.Log(Input.GetAxis("Horizontal_P2"));

        if (!isJumping && !isFalling && Input.GetKeyDown(KeyCode.LeftArrow))
            LaunchJump(false);

        if (!isJumping && !isFalling && Input.GetKeyDown(KeyCode.RightArrow))
            LaunchJump(true);

        if (!isJumping && !isFalling && isDectingGround)
            CheckGround();

        if (isFalling)
            Fall();
    }

    void LaunchJump(bool isRight)
    {
        float dir = isRight ? 1.0f : -1.0f;

        if (Mathf.Abs(actualOffset + dir) > maxOffset)
            return;

        isJumping = true;
        actualOffset += (int)dir;

        Tween.LocalPositionX(
            transform,
            transform.localPosition.x,
            transform.localPosition.x + dir,
            0.25f,
            ease: Ease.OutQuart);

        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            transform.localPosition.y + 1.0f,
            0.125f,
            ease: Ease.OutQuart).OnComplete(() => { DownFromJump(); });
    }

    void DownFromJump()
    {
        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            transform.localPosition.y - 1.0f,
            0.125f,
            ease: Ease.InQuart).OnComplete(() => { isJumping = false; });
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
}
