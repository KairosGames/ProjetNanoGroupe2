using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class FollowingSpline : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private float speed = 3.0f;

    private float currentPos;
    private float currentLength;

    private void Start()
    {
        currentLength = spline.Spline.GetLength();
    }

    private void Update()
    {
        UpdatePosOnSpline();
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