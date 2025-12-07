using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    [SerializeField] Transform Anchor;
    [SerializeField] float roughness = 100.0f;

    void Start()
    {
        transform.position = Anchor.position;
        transform.rotation = Anchor.rotation;
        transform.localScale = Anchor.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, Anchor.position, roughness*Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Anchor.rotation, roughness*Time.deltaTime);
    }
}
