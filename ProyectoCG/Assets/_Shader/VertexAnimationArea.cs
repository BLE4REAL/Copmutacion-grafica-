using UnityEngine;

[ExecuteInEditMode]
public class VertexAnimationArea : MonoBehaviour
{
    [SerializeField] private Material targetMaterial;
    [SerializeField] private float radious;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector4 sphere = new Vector4(pos.x, pos.y, pos.z, w:radious);
        targetMaterial.SetVector(name: "_SphereDescription", sphere);
    }

   void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, radious);
    }


}
