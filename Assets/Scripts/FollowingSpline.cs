using UnityEngine;
using UnityEngine.Splines;

public class FollowingSpline : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    [Header("Settings")]
    [SerializeField] public float tSmooth = 0.3f;
    [SerializeField] public float baseSpeed = 10.0f;
    [SerializeField] public float topSpeed = 20.0f;
    [SerializeField] float acceleration = 0.05f;
    [SerializeField] float deceleration = 0.05f;

    [Header("Debugging")]
    [SerializeField] bool isDebugPosActive = false;
    [SerializeField] int startingNode = 0;
    [SerializeField] bool areFarsActives = false;

    [System.NonSerialized] public float speed;
    private float currentPos;
    private float currentLength;
    private bool accelerating = false;
    private float accelerationTime = 0.0f;
    private bool decelerating = true;
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
        UpdatePosOnSpline();

        if (player1.isBoosting || player2.isBoosting)
        {
            
            if (!accelerating)
            {
                if (decelerationTime < 1.0f)
                    accelerationTime = 1 - decelerationTime;
                else
                    accelerationTime = 0.0f;

                accelerating = true;
                decelerating = false;
            }
            speed = Mathf.Lerp(baseSpeed, topSpeed, accelerationTime);
            accelerationTime += Time.deltaTime * acceleration;
        }
        else
        {
            if (!decelerating)
            {
                if (accelerationTime < 1.0f)
                    decelerationTime = 1 - accelerationTime;
                else
                    decelerationTime = 0.0f;

                accelerating = false;
                decelerating = true;
            }
            speed = Mathf.Lerp(topSpeed, baseSpeed, decelerationTime);
            decelerationTime += Time.deltaTime * deceleration;
        }
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