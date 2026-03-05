using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float radius;

    void Update()
    {
        Vector3 pos = transform.position;
        Vector4 sphere = new Vector4(pos.x, pos.y, pos.z, radius);
        targetMaterial.SetVector("_Sphere_Descriptor", sphere);
    } 

    void OnDrawGizmos()
   {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}