using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class TrackRenderer : MonoBehaviour
{
    [SerializeField] NewSplineParameters[] farLeftSplinesData;
    [SerializeField] NewSplineParameters[] leftSplinesData;
    [SerializeField] NewSplineParameters[] middleSplinesData;
    [SerializeField] NewSplineParameters[] rightSplinesData;
    [SerializeField] NewSplineParameters[] farRightSplinesData;
    [SerializeField] string layerName;
    [SerializeField] public float offset;
    [SerializeField] float radius;
    [SerializeField] float segmentsPerUnit;
    [SerializeField] Material material;

    Spline mainSpline;
    
    [HideInInspector] public List<SplineContainer> entireSplines;


    void Awake()
    {
        mainSpline = GetComponent<SplineContainer>().Spline;

        foreach (NewSplineParameters data in farLeftSplinesData)
        {
            GameObject newTrack = SplineContainerInit();
            CreateSpline(newTrack, data, -2);
            AddColliderAndMaterial(newTrack);
        }
        foreach (NewSplineParameters data in leftSplinesData)
        {
            GameObject newTrack = SplineContainerInit();
            CreateSpline(newTrack, data, -1);
            AddColliderAndMaterial(newTrack);
        }
        foreach (NewSplineParameters data in middleSplinesData)
        {
            GameObject newTrack = SplineContainerInit();
            CreateSpline(newTrack, data, -0);
            AddColliderAndMaterial(newTrack);
        }
        foreach (NewSplineParameters data in rightSplinesData)
        {
            GameObject newTrack = SplineContainerInit();
            CreateSpline(newTrack, data, 1);
            AddColliderAndMaterial(newTrack);
        }
        foreach (NewSplineParameters data in farRightSplinesData)
        {
            GameObject newTrack = SplineContainerInit();
            CreateSpline(newTrack, data, 2);
            AddColliderAndMaterial(newTrack);
        }
    }

    void AddColliderAndMaterial(GameObject track)
    {
        MeshCollider collider = track.AddComponent<MeshCollider>();
        collider.sharedMesh = track.GetComponent<MeshFilter>().sharedMesh;
        track.GetComponent<MeshRenderer>().material = material;
    }

    GameObject SplineContainerInit()
    {
        GameObject newTrack = new GameObject();
        newTrack.transform.parent = transform;
        newTrack.layer = LayerMask.NameToLayer(layerName);
        SplineContainer splineContainer = newTrack.AddComponent<SplineContainer>();
        splineContainer.RemoveSpline(splineContainer.Spline);
        SplineExtrude extrude = newTrack.AddComponent<SplineExtrude>();
        extrude.Radius = radius;
        extrude.SegmentsPerUnit = segmentsPerUnit;
        extrude.Container = splineContainer;
        return newTrack;
    }

    void CreateSpline(GameObject track, NewSplineParameters data, int offsetMultiplier)
    {

        SplineContainer splineContainer = track.GetComponent<SplineContainer>();
        Spline spline = splineContainer.AddSpline();

        for (int i = data.startingKnot; i < data.endingKnot + 1; i++)
        {
            BezierKnot knot = mainSpline[i];
            float t = mainSpline.ConvertIndexUnit(i, PathIndexUnit.Knot, PathIndexUnit.Normalized);

            Vector3 tangent = mainSpline.EvaluateTangent(t);
            tangent.Normalize();

            Vector3 up = mainSpline.CalculateUpVector(t);
            up.Normalize();

            Vector3 right = Vector3.Cross(tangent, up).normalized;

            knot.Position.x += right[0] * offsetMultiplier * offset;
            knot.Position.y += right[1] * offsetMultiplier * offset;
            knot.Position.z += right[2] * offsetMultiplier * offset;

            spline.Add(knot);
        }
    }


    [System.Serializable] struct NewSplineParameters
    {
        public int startingKnot;
        public int endingKnot;
    }
}
