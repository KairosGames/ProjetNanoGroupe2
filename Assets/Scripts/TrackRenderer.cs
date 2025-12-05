using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class TrackRenderer : MonoBehaviour
{
    [System.Serializable]
    struct NewSplineParameters
    {
        public int startingKnot;
        public int endingKnot;
    }

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

    void Start()    
    {
        mainSpline = this.GetComponent<SplineContainer>().Spline;

        GameObject farLeftSplineContainer = SplineContainerInit("farLeftSplineContainer");
        GameObject leftSplineContainer = SplineContainerInit("leftSplineContainer");
        GameObject middleSplineContainer = SplineContainerInit("middleSplineContainer");
        GameObject rightSplineContainer = SplineContainerInit("rightSplineContainer");
        GameObject farRightSplineContainer = SplineContainerInit("farRightSplineContainer");

        foreach (NewSplineParameters data in farLeftSplinesData)
            CreateSpline(farLeftSplineContainer, data, -2);
        foreach (NewSplineParameters data in leftSplinesData)
            CreateSpline(leftSplineContainer, data, -1);
        foreach (NewSplineParameters data in middleSplinesData)
            CreateSpline(middleSplineContainer, data, 0);
        foreach (NewSplineParameters data in rightSplinesData)
            CreateSpline(rightSplineContainer, data, 1);
        foreach (NewSplineParameters data in farRightSplinesData)
            CreateSpline(farRightSplineContainer, data, 2);

        AddColliderAndMaterial(farLeftSplineContainer);
        AddColliderAndMaterial(leftSplineContainer);
        AddColliderAndMaterial(middleSplineContainer);
        AddColliderAndMaterial(rightSplineContainer);
        AddColliderAndMaterial(farRightSplineContainer);
    }

    void AddColliderAndMaterial(GameObject track)
    {
        MeshCollider collider = track.AddComponent<MeshCollider>();
        collider.sharedMesh = track.GetComponent<MeshFilter>().sharedMesh;
        track.GetComponent<MeshRenderer>().material = material;
    }

    GameObject SplineContainerInit(string name)
    {
        GameObject newTrack = new GameObject();
        newTrack.layer = LayerMask.NameToLayer(layerName);
        SplineContainer splineContainer = newTrack.AddComponent<SplineContainer>();
        splineContainer.RemoveSpline(splineContainer.Spline);
        SplineExtrude extrude = newTrack.AddComponent<SplineExtrude>();
        extrude.Radius = radius;
        extrude.SegmentsPerUnit = segmentsPerUnit;
        extrude.Container = splineContainer;
        newTrack.name = name;
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

    void Update()
    {
        
    }
}
