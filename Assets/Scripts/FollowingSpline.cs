using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class FollowingSpline : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Player player;
    [SerializeField] float baseSpeed = 5.0f;
    [SerializeField] float topSpeed = 5.0f;
    [SerializeField] float acceleration = 0.05f;
    [SerializeField] float deceleration = 0.05f;

    private float speed;
    private float currentPos;
    private float currentLength;
    private bool accelerating = false;
    private float accelerationTime = 0.0f;
    private bool decelerating = true;
    private float decelerationTime = 1.0f;

    private void Start()
    {
        speed = baseSpeed;
        currentLength = spline.Spline.GetLength();
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
}