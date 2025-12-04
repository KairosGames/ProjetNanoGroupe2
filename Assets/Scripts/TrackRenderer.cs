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

    [SerializeField] NewSplineParameters[] newSplines;
    [SerializeField] GameObject mainTrack;
    [SerializeField] float radius;
    [SerializeField] float segmentsPerUnit;
    [SerializeField] Material material;  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()    
    {
        for (int i = 0; i < newSplines.Length; i++)
        {
            NewSplineParameters newSpline = newSplines[i];
            CreateSpline(mainTrack, newSpline.startingKnot, newSpline.endingKnot, newSpline.offset);
            //MeshRenderer mesh = leftTrack.GetComponent<MeshRenderer>();
            //mesh.materials[0].color = Color.white;
        }

    }

    void CreateSpline(GameObject mainTrack, int start, int end, float offset)
    {
        Spline mainSpline = mainTrack.GetComponent<SplineContainer>().Spline;

        GameObject newTrack = new GameObject();
        SplineContainer newSpline = newTrack.AddComponent<SplineContainer>();
        SplineExtrude extrude = newTrack.AddComponent<SplineExtrude>();
        extrude.Radius = radius;
        extrude.SegmentsPerUnit = segmentsPerUnit;
        extrude.Container = newSpline;
       
        newSpline.AddSpline();

        for (int i = start; i < end; i++)
        {
            BezierKnot knot = mainSpline[i];
            float t = GetKnotRatio(mainSpline, i);

            Vector3 tangent = mainSpline.EvaluateTangent(t);
            tangent.Normalize();
            Debug.Log(tangent);

            Vector3 up = mainSpline.CalculateUpVector(t);
            up.Normalize();
            Debug.Log(up);

            Vector3 right = Vector3.Cross(tangent, up).normalized;

            knot.Position.x += right[0] * offset;
            knot.Position.y += right[1] * offset;
            knot.Position.z += right[2] * offset;

            newSpline.Splines[0].Add(knot);
        }
    }
    float GetKnotRatio(Spline spline, int knotIndex)
    {
        // Si c’est le premier knot -> t = 0
        if (knotIndex <= 0)
            return 0f;

        // Si c’est le dernier knot -> t = 1
        if (knotIndex >= spline.Count - 1)
            return 1f;

        // Longueur totale de la spline
        float totalLength = spline.GetLength();

        // Longueur cumulée jusqu’au knot
        float accumulated = 0f;

        // Additionner la longueur des courbes précédant le knot
        for (int curve = 0; curve < knotIndex; curve++)
        {
            accumulated += spline.GetCurveLength(curve);
        }

        // Ratio normalisé
        return accumulated / totalLength;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
