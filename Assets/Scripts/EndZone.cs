using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EndZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.finished = true;
    }
}
