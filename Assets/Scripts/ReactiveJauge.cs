using UnityEngine;
using UnityEngine.UI;

public class ReactiveJauge : MonoBehaviour
{
    [SerializeField] FollowingSpline maincar;
    private float speed;

    private Material _material;
    private Image _renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Image>();
        _material = _renderer.material;  
    }

    // Update is called once per frame
    void Update()
    {
        speed = Mathf.Lerp(speed, maincar.speed, Time.deltaTime*2);
        //Debug.Log("speed : " + speed);

        _material.SetFloat("_FillJauge", speed);

    }
}
