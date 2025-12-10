using UnityEngine;
using UnityEngine.Splines;

public class FollowingSpline : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;

    [Header("InternReferences")]
    [SerializeField] public ParticleSystem starEffect;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Settings")]
    [SerializeField] public float tSmooth = 0.3f;
    [SerializeField] public float baseSpeed = 10.0f;
    [SerializeField] float oneSpeed = 12.5f;
    [SerializeField] float twoSpeed = 15.0f;
    [SerializeField] public float topSpeed = 17.5f;
    [SerializeField] float accelerationT = 1.5f;
    [SerializeField] float decelerationT = 0.5f;

    [Header("Debugging")]
    [SerializeField] bool isDebugPosActive = false;
    [SerializeField] int startingNode = 0;
    [SerializeField] bool areFarsActives = false;

    [System.NonSerialized] public float speed;
    private float currentPos;
    private float currentLength;
    private float accelerationTime = 0.0f;
    private float decelerationTime = 1.0f;

    [HideInInspector] static public float actualRatio = 0.0f;


    private void Awake()
    {
        speed = baseSpeed;
        currentLength = spline.Spline.GetLength();

        if (isDebugPosActive)
            SetDebugPos();
    }


    private void Update()
    {
        if (GameManager.started != 2) return;

        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);

        UpdateSpeed();
        UpdatePosOnSpline();
    }


    void UpdateSpeed()
    {
        float targetSpeed = baseSpeed;
        if (player1.isBoosting || player2.isBoosting)
            targetSpeed = oneSpeed;
        if (player1.isBoosting && player2.isBoosting)
            targetSpeed = (player1.actualOffset == player2.actualOffset) ? topSpeed : twoSpeed;

        speed = Mathf.Lerp(speed, targetSpeed, ((speed < targetSpeed) ? accelerationT : decelerationT) * Time.deltaTime);
    }


    private void UpdatePosOnSpline()
    {
        currentPos = currentPos + (speed * Time.deltaTime);
        actualRatio = currentPos / currentLength;
        spline.Evaluate(actualRatio, out var localPos, out var dir, out var up);
        Vector3 pos = spline.transform.TransformPoint(localPos);
        var rotation = Quaternion.LookRotation(dir, up);
        transform.SetPositionAndRotation(pos, rotation);
    }


    void SetDebugPos()
    {
        if (startingNode >= spline.Spline.Count)
            Debug.LogError("StartingNode is bigger than the count of spline nodes. Les GD vous puez la merde ! <3 ");

        float t = SplineUtility.ConvertIndexUnit(spline.Spline, startingNode, PathIndexUnit.Knot, PathIndexUnit.Normalized);
        currentPos = currentLength * t;

        if (areFarsActives)
        {
            player1.maxOffset = 2;
            player2.maxOffset = 2;
        }
    }
}