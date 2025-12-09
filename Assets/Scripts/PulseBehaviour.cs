using UnityEngine;

public class PulseBehaviour : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float speed = 2f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

    }
}
