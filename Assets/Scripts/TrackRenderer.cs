using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class TrackRenderer : MonoBehaviour
{
    [Header("Track Data")]
    [SerializeField] NewSplineParameters[] farLeftSplinesData;
    [SerializeField] NewSplineParameters[] leftSplinesData;
    [SerializeField] NewSplineParameters[] middleSplinesData;
    [SerializeField] NewSplineParameters[] rightSplinesData;
    [SerializeField] NewSplineParameters[] farRightSplinesData;

    [Header("Settings")]
    [SerializeField] string layerName;
    [SerializeField] public float offset;
    [SerializeField] float radius;
    [SerializeField] float segmentsPerUnit;

    [Header("Dependencies")]
    [SerializeField] Material material;

    List<GameObject> trackParents = new();
    Spline mainSpline;
    
    [HideInInspector] public List<SplineContainer> entireSplines;


    void Awake()
    {
        mainSpline = GetComponent<SplineContainer>().Spline;

        GenerateAllTracks();

        if (layerName == "Ground")
            GenerateEntireSplines();
    }


    void GenerateAllTracks()
    {
        NewSplineParameters[][] data = { farLeftSplinesData, leftSplinesData, middleSplinesData, rightSplinesData, farRightSplinesData };
        for (int iType = 0; iType <= 4; iType++)
        {
            GameObject trackParent = new GameObject();
            trackParent.transform.parent = transform;
            trackParent.name = $"{((TrackType)iType)}Track" + "_" + layerName + "Layer";
            trackParents.Add(trackParent);

            for (int i = 0; i < data[iType].Length; i++)
            {
                GameObject newTrack = SplineContainerInit(iType, i);
                CreateSpline(newTrack, data[iType][i], iType - 2);
                AddColliderAndMaterial(newTrack);
            }
        }
    }


    GameObject SplineContainerInit(int parentIndex, int indexOnTrack)
    {
        GameObject newTrack = new GameObject();
        newTrack.transform.parent = trackParents[parentIndex].transform;
        newTrack.name = $"{((TrackType)parentIndex)}Chunk{indexOnTrack}" + "_" + layerName;
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

            knot.Position.x -= right[0] * offsetMultiplier * offset;
            knot.Position.y -= right[1] * offsetMultiplier * offset;
            knot.Position.z -= right[2] * offsetMultiplier * offset;

            spline.Add(knot);
        }
    }


    void AddColliderAndMaterial(GameObject track)
    {
        MeshCollider collider = track.AddComponent<MeshCollider>();
        collider.sharedMesh = track.GetComponent<MeshFilter>().sharedMesh;
        track.GetComponent<MeshRenderer>().material = material;
    }


    void GenerateEntireSplines()
    {
        GameObject parent = new GameObject();
        parent.transform.parent = transform;
        parent.name = "EntireSplines";

        for(int i = 0; i <= 4; i++)
        {
            GameObject newTrack = new GameObject();
            newTrack.transform.parent = parent.transform;
            newTrack.name = $"{((TrackType)i)}TrackReference";
            SplineContainer splineContainer = newTrack.AddComponent<SplineContainer>();
            splineContainer.RemoveSpline(splineContainer.Spline);
            entireSplines.Add(splineContainer);

            NewSplineParameters data = new NewSplineParameters();
            data.startingKnot = 0;
            data.endingKnot = mainSpline.Count - 1;

            CreateSpline(newTrack, data, i - 2);
        }
    }


    [System.Serializable] struct NewSplineParameters
    {
        public int startingKnot;
        public int endingKnot;
    }


    public enum TrackType
    {
        FarLeft,
        Left,
        Middle,
        Right,
        FarRight
    }
}
