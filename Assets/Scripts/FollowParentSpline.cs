using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class FollowParentSpline : MonoBehaviour
{
    public SplineContainer parent;
    public float offset = 2f;

    private SplineContainer child;

    void OnEnable()
    {
        child = GetComponent<SplineContainer>();
        UpdateChildSpline();
    }

    void Update()
    {
        UpdateChildSpline();
    }

    void UpdateChildSpline()
    {
        if (parent == null) return;

        if (child == null)
            child = GetComponent<SplineContainer>();

        var parentSpline = parent.Spline;
        var childSpline = child.Spline;

        childSpline.Clear();
        int knotCount = parentSpline.Count;
        if (knotCount < 2) return;

        // --- 1) Pré-calcul des tangentes "T" pour tous les knots ---
        Vector3[] knotTangents = new Vector3[knotCount];

        for (int i = 0; i < knotCount; i++)
        {
            var k = parentSpline[i];
            Vector3 forward = k.TangentOut;
            Vector3 backward = -k.TangentIn;

            Vector3 t = (forward + backward) * 0.5f;
            knotTangents[i] = t.sqrMagnitude > 0.0001f ? t.normalized : Vector3.forward;
        }

        // --- 2) Génération de la spline enfant ---
        for (int i = 0; i < knotCount; i++)
        {
            var parentKnot = parentSpline[i];

            Vector3 pos = parentKnot.Position;
            Vector3 tin = parentKnot.TangentIn;
            Vector3 tout = parentKnot.TangentOut;

            // Tangente locale
            Vector3 T = knotTangents[i];

            // Tangente précédente = sert à calculer le banking
            Vector3 T_prev = knotTangents[(i - 1 + knotCount) % knotCount];

            // --- Construction du repère de Frenet pour banking ---
            Vector3 B = Vector3.Cross(T_prev, T); // binormale
            Vector3 N;

            if (B.sqrMagnitude < 0.0001f)
            {
                // Ligne droite → on ne peut pas déterminer un banking
                N = Vector3.Cross(Vector3.up, T).normalized;
            }
            else
            {
                B.Normalize();
                N = Vector3.Cross(B, T).normalized; // normale latérale avec banking
            }

            // Offset latéral
            Vector3 childPos = pos + N * offset;

            // Tangentes enfant (même longueur, même orientation relative)
            Vector3 childTanInAbs = childPos + tin;
            Vector3 childTanOutAbs = childPos + tout;

            var childKnot = new BezierKnot(childPos)
            {
                TangentIn = childTanInAbs - childPos,
                TangentOut = childTanOutAbs - childPos
            };

            childSpline.Add(childKnot);
        }

        childSpline.Closed = parentSpline.Closed;

#if UNITY_EDITOR
        EditorUtility.SetDirty(child);
#endif
    }
}
