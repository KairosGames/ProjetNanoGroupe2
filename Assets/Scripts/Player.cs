using PrimeTween;
using UnityEngine;

public class Player : MonoBehaviour
{
    bool isMoving = false;

    void Update()
    {
        if (!isMoving && Input.GetKeyDown(KeyCode.LeftArrow))
            LaunchJump(false);

        if (!isMoving && Input.GetKeyDown(KeyCode.RightArrow))
            LaunchJump(true);
    }

    void LaunchJump(bool isRight)
    {
        isMoving = true;
        float dir = isRight ? 1.0f : -1.0f;

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
            ease: Ease.OutQuart).OnComplete(() => { Fall(); });
    }

    void Fall()
    {
        Tween.LocalPositionY(
            transform,
            transform.localPosition.y,
            transform.localPosition.y - 1.0f,
            0.125f,
            ease: Ease.InQuart).OnComplete(() => { isMoving = false; });
    }
}
