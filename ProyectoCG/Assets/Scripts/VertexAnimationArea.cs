using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    SerializeField private Material targetMaterial;

    SerializeField private float radius;

    void Update()
    {
        Vector3 pos = transform.position;
        Vector4 sphere = Vector4(pos.x, pos.y, pos.z, w: radius);
        targetMaterial.SetVector(name: "_Sphere_Descriptor", sphere);
    }
}