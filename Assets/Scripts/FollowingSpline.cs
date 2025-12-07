using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class FollowingSpline : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Player player;

    [Header("Settings")]
    [SerializeField] float baseSpeed = 5.0f;
    [SerializeField] float topSpeed = 5.0f;
    [SerializeField] float acceleration = 0.05f;
    [SerializeField] float deceleration = 0.05f;

    [Header("Debugging")]
    [SerializeField] bool isDebugPosActive = false;
    [SerializeField] int startingNode = 0;
    [SerializeField] bool areFarsActives = false;

    private float speed;
    private float currentPos;
    private float currentLength;
    private bool accelerating = false;
    private float accelerationTime = 0.0f;
    private bool decelerating = true;
    private float decelerationTime = 1.0f;


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

        if (player.isBoosting)
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
        var normalizedPos = currentPos / currentLength;
        spline.Evaluate(normalizedPos, out var pos, out var dir, out var up);
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
            player.maxOffset++;
    }
}