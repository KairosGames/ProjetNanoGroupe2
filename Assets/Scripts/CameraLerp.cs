using FMOD.Studio;
using PrimeTween;
using Unity.VisualScripting;
using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    [Header("Extern References")]
    [SerializeField] GameManager manager;

    [Header("Intern References")]
    [SerializeField] Transform anchor;
    [SerializeField] public ParticleSystem windParticles;
    [SerializeField] public ParticleSystem starsParticles;

    [Header("Settings")]
    [SerializeField] float roughness = 1.0f;

    [System.NonSerialized] public bool isFollowingAnchor = true;
    FollowingSpline mainCar;
    Camera mainCam;


    void Awake()
    {
        mainCar = anchor.parent.GetComponent<FollowingSpline>();
        mainCam = GetComponent<Camera>();

        transform.SetParent(null, true);
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;

        starsParticles.gameObject.SetActive(false);
    }
    

    void Start()
    {
        LauchRoughnessTwean();
    }


    void Update()
    {
        isFollowingAnchor = !manager.finished;

        if (isFollowingAnchor)
        {
            SetSpeedEfect();
            transform.position = Vector3.Lerp(transform.position, anchor.position, roughness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, anchor.rotation, roughness * Time.deltaTime);
        }
        else
        {
            windParticles.Stop();
            starsParticles.gameObject.SetActive(true);
        }
    }


    void LauchRoughnessTwean()
    {
        Tween.Custom(roughness, 6.0f, duration : 5.0f, onValueChange: v => roughness = v);
    }


    void SetSpeedEfect()
    {
        float t = Mathf.InverseLerp(mainCar.baseSpeed, mainCar.topSpeed, mainCar.speed);
        mainCam.fieldOfView = Mathf.Lerp(80.0f, 110.0f, t);
        windParticles.startSpeed= Mathf.Lerp(30.0f, 60.0f, t);
        var main = windParticles.main;
        main.simulationSpeed = Mathf.Lerp(0.4f, 2.0f, t);
    }
}
