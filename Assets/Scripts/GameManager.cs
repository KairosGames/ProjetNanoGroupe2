using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timerText;

    public float timer = 0.0f;
    public bool finished = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!finished)
        {
            timer += Time.deltaTime;
            timerText.text = TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");
        }

    }
}
