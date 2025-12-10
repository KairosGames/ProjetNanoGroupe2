using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public CanvasGroup TitleCanvas;
    public CanvasGroup HUDCanvas;
    public CanvasGroup TutoCanvas;
    public CanvasGroup EndScreenCanvas;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI finalTimerText;

    bool doneWithFade = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.started == 1)
        {
            TitleCanvas.alpha = Mathf.Lerp(TitleCanvas.alpha, 0.0f, Time.deltaTime * 10);
            if (TitleCanvas.alpha < 0.01) TutoCanvas.alpha = Mathf.Lerp(TutoCanvas.alpha, 1.0f, Time.deltaTime * 5);
        }
        if (GameManager.started == 2 && !doneWithFade)
        {
            TitleCanvas.alpha = 0.0f;
            TutoCanvas.alpha = Mathf.Lerp(TutoCanvas.alpha, 0.0f, Time.deltaTime * 10);
            if (TutoCanvas.alpha < 0.01) HUDCanvas.alpha = Mathf.Lerp(HUDCanvas.alpha, 1.0f, Time.deltaTime * 5);

            if (HUDCanvas.alpha > 0.99)
            {
                HUDCanvas.alpha = 1.0f;
                doneWithFade = true;
            }
        }

        timerText.text = TimeSpan.FromSeconds(GameManager.timer).ToString("mm\\:ss\\:ff");

        if (GameManager.finished)
        {
            finalTimerText.text = TimeSpan.FromSeconds(GameManager.timer).ToString("mm\\:ss\\:ff");
            EndScreenCanvas.alpha = Mathf.Lerp(EndScreenCanvas.alpha, 1.0f, Time.deltaTime);
            HUDCanvas.alpha = Mathf.Lerp(HUDCanvas.alpha, 0.0f, Time.deltaTime * 10);
            if (HUDCanvas.alpha < 0.1) HUDCanvas.alpha = 0.0f;
        }
    }
}
