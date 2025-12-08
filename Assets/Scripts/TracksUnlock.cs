using UnityEngine;

public class TracksUnlock : MonoBehaviour
{
    [SerializeField] Player player1;
    [SerializeField] Player player2;

    private void OnTriggerEnter(Collider other)
    {
        player1.maxOffset = 2;
        player2.maxOffset = 2;
    }
}
