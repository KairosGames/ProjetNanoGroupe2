using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    private float timer = 0.0f;
    private float alpha = 0.0f;
    private CanvasGroup selfCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfCanvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        selfCanvas.alpha = alpha;

        if(GameManager.finished == true)
        {
            timer += Time.deltaTime;
            if(timer > 2f)
            {
                alpha += Time.deltaTime;
            }
        }
    }
}
