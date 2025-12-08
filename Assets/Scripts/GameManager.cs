using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timer = 0.0f;
    public bool finished = false;

    void Update()
    {
        if (!finished)
        {
            timer += Time.deltaTime;
            timerText.text = TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");
        }
    }
}
