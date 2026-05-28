using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delay = 1f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}