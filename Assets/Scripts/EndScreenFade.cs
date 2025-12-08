using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndScreenFade : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI textFinal;
    [SerializeField] Canvas canvas;
    [SerializeField] Canvas timerCanvas;
    [SerializeField] float fadeSpeed = 2f;

    CanvasGroup canvasGroup;
    CanvasGroup timerCanvasGroup;
    void Start()
    {
        canvasGroup = canvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        timerCanvasGroup = timerCanvas.GetComponent<CanvasGroup>();
        timerCanvasGroup.alpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.finished)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.0f, (fadeSpeed * Time.deltaTime)/2);
            timerCanvasGroup.alpha = Mathf.Lerp(timerCanvasGroup.alpha, 0.0f, fadeSpeed * Time.deltaTime*2);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        textFinal.text = TimeSpan.FromSeconds(gameManager.timer).ToString("mm\\:ss\\:ff");
        gameManager.finished = true;
    }
}
