using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    static public int scorePlayer1;
    static public int scorePlayer2;

    public float timer = 0.0f;
    static public bool finished = false;
    static public bool started = true;


    void Update()
    {
        if (!finished && started)
        {
            timer += Time.deltaTime;
            timerText.text = TimeSpan.FromSeconds(timer).ToString("mm\\:ss\\:ff");

            if (player1.isBoosting) scorePlayer1 += 1;
            if (player2.isBoosting) scorePlayer2 += 1;
        }
    }
}
