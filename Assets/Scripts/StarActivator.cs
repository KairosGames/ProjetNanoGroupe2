using UnityEngine;

public class StarActivator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       Player player = other.transform.parent.GetComponent<Player>();

        if (player != null)
        {
            ParticleSystem stars = player.transform.parent.GetComponent<FollowingSpline>().starEffect;
            stars.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            stars.Play();
            gameObject.SetActive(false);
        }
    }
}
