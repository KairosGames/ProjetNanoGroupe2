using UnityEngine;
using UnityEngine.Splines;

public class TrackRenderer : MonoBehaviour
{
    [System.Serializable]
    struct NewSplineParameters
    {
        public int startingKnot;
        public int endingKnot;
        public float offset;
    }

    [SerializeField] NewSplineParameters[] splinesData;
    [SerializeField] GameObject mainTrack;
    [SerializeField] float radius;
    [SerializeField] float segmentsPerUnit;
    [SerializeField] Material material;

    Spline mainSpline;

    void Start()    
    {
        mainSpline = mainTrack.GetComponent<SplineContainer>().Spline;

        for (int i = 0; i < splinesData.Length; i++)
        {
            SplineContainer newSplineContainer = CreateSpline(mainTrack, splinesData[i]);
            newSplineContainer.GetComponent<MeshRenderer>().material = material;
        }
    }

    SplineContainer CreateSpline(GameObject mainTrack, NewSplineParameters data)
    {
        GameObject newTrack = new GameObject();
        SplineContainer newSplineContainer = newTrack.AddComponent<SplineContainer>();
        SplineExtrude extrude = newTrack.AddComponent<SplineExtrude>();
        extrude.Radius = radius;
        extrude.SegmentsPerUnit = segmentsPerUnit;
        extrude.Container = newSplineContainer;
       
        newSplineContainer.AddSpline();

        for (int i = data.startingKnot; i < data.endingKnot; i++)
        {
            BezierKnot knot = mainSpline[i];
            float t = mainSpline.ConvertIndexUnit(i, PathIndexUnit.Knot, PathIndexUnit.Normalized);

            Vector3 tangent = mainSpline.EvaluateTangent(t);
            tangent.Normalize();

            Vector3 up = mainSpline.CalculateUpVector(t);
            up.Normalize();

            Vector3 right = Vector3.Cross(tangent, up).normalized;

            knot.Position.x += right[0] * data.offset;
            knot.Position.y += right[1] * data.offset;
            knot.Position.z += right[2] * data.offset;

            newSplineContainer.Splines[0].Add(knot);
        }

        return newSplineContainer;
    }

    void Update()
    {
        
    }
}
