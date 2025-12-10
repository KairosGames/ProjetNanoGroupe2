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

    static public float timer = 0.0f;
    static public bool finished = false;
    static public int started = 0; //started == 0 is titleScreen, stared == 1 is tuto, started == 2 is starting the game


    void Update()
    {
        Debug.Log(finished);

        if (started == 0)
        {
            if (Input.anyKeyDown) started = 1;
            return;
        }
        if (started == 1)
        {
            if (Input.anyKeyDown) started = 2;
        }

        if (!finished && started == 2)
        {
            timer += Time.deltaTime;

            if (player1.isBoosting) scorePlayer1 += 1;
            if (player2.isBoosting) scorePlayer2 += 1;
        }
    }
}
