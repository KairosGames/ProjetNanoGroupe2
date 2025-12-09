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

    FMOD.Studio.EventInstance speedSound;
    string speedSoundParam;
    bool isDelerpLaunched = false;

    void Awake()
    {
        mainCar = anchor.parent.GetComponent<FollowingSpline>();
        mainCam = GetComponent<Camera>();

        speedSound = FMODUnity.RuntimeManager.CreateInstance("event:/Avatar/Speed_Sound");
        speedSoundParam = "Speed_player";
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(speedSoundParam, 0.0f);
        speedSound.start();

        transform.SetParent(null, true);
        transform.position = anchor.position;
        transform.rotation = anchor.rotation;

        starsParticles.gameObject.SetActive(false);
    }
    

    void Start()
    {
        LauchRoughnessTween();
    }


    void Update()
    {
        isFollowingAnchor = !GameManager.finished;

        
        SetSpeedEfect();
        transform.position = Vector3.Lerp(transform.position, anchor.position, roughness * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, anchor.rotation, roughness * Time.deltaTime);

        if (!isFollowingAnchor)
        {
            windParticles.Stop();
            starsParticles.gameObject.SetActive(true);

            if (!isDelerpLaunched)
            {
                isDelerpLaunched = true;
                DeLaunchRoughnessTween();
            }
        }
    }


    void LauchRoughnessTween()
    {
        Tween.Custom(roughness, 6.0f, duration : 5.0f, onValueChange: v => roughness = v);
    }


    void DeLaunchRoughnessTween()
    {
        Tween.Custom(roughness, 0.001f, duration: 4.0f, onValueChange: v => roughness = v);
    }


    void SetSpeedEfect()
    {
        float t = Mathf.InverseLerp(mainCar.baseSpeed, mainCar.topSpeed, mainCar.speed);
        mainCam.fieldOfView = Mathf.Lerp(80.0f, 120.0f, t);
        windParticles.startSpeed= Mathf.Lerp(30.0f, 60.0f, t);
        var main = windParticles.main;
        main.simulationSpeed = Mathf.Lerp(0.4f, 2.0f, t);
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(speedSoundParam, t + 0.4f);
    }
}
