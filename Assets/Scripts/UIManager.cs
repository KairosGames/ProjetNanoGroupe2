using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public CanvasGroup TitleCanvas;
    public CanvasGroup HUDCanvas;

    //public GameObject ColliderCanvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Disappear(CanvasGroup canvastoFade)
    {

        StartCoroutine(Fade(canvastoFade, false));
    }

    public void Appear(CanvasGroup canvastoPop)
    {
        StartCoroutine(Fade(canvastoPop, true));
    }

    private IEnumerator Fade(CanvasGroup canvastoFade, bool Fade)
    {
        float TimetoFade = 1f;

        canvastoFade.gameObject.SetActive(true);

        while (TimetoFade >= 0f)
        {
            TimetoFade -= Time.deltaTime;
            float alpha = TimetoFade; /// TimetoFade;
            canvastoFade.alpha = Fade ? 1 - alpha : alpha;
            yield return null;
        }

    }


}
