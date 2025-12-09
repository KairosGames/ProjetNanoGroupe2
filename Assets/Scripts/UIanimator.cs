using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    public Image targetImage;
    public Sprite[] frames;
    public float frameRate = 0.1f;

    private int currentFrame;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            if (frames.Length == 0) return;
            currentFrame = (currentFrame + 1) % frames.Length;
            targetImage.sprite = frames[currentFrame];
            timer = 0f;
        }
    }
}