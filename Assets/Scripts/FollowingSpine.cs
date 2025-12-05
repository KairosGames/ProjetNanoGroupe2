using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineMoving : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float maxSpeed = 3f;

    private float currentPos;

    private void Update()
    {
        UpdatePosOnSpline();
    }

    private void UpdatePosOnSpline()
    {
        var splineLength = spline.Spline.GetLength();
        var currentSpeed = maxSpeed;
        // make sure on spline
        currentPos = Mathf.Clamp(currentPos + currentSpeed * Time.deltaTime, 0f, splineLength);
        // most important part: evaluate currentPos on spline
        var normalizedPos = currentPos / splineLength;
        spline.Spline.Evaluate(normalizedPos, out var pos, out var dir, out var up);
        transform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
    }
}