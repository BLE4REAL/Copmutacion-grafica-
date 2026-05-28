using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Tiempo máximo de vida antes de autodestruirse (segundos)")]
    public float maxLifetime = 5f;

    [Tooltip("Sistema de partículas opcional para la explosión al impactar")]
    public GameObject impactEffectPrefab;

    void Start()
    {
        // Salvavidas: si por algún motivo no choca con nada, se borra solo
        // a los 5 segundos para no acumular basura en la escena.
        Destroy(gameObject, maxLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Buscamos el componente EnergyShield en el objeto con el que chocamos.
        EnergyShield shield = collision.gameObject.GetComponent<EnergyShield>();

        if (shield != null)
        {
            // El proyectil chocó con el escudo: le avisamos el punto exacto del impacto.
            Vector3 hitPoint = collision.contacts[0].point;
            shield.RegisterHit(hitPoint);
        }

        // Spawneamos el efecto de explosión si está configurado.
        if (impactEffectPrefab != null)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 hitNormal = collision.contacts[0].normal;
            
            // Orientamos el efecto para que su "frente" apunte hacia afuera del escudo.
            Quaternion rotation = Quaternion.LookRotation(hitNormal);
            
            Instantiate(impactEffectPrefab, hitPoint, rotation);

            //Vector3 spawnPos = collision.contacts[0].point;
            //Instantiate(impactEffectPrefab, spawnPos, Quaternion.identity);
        }

        // El proyectil desaparece tras chocar (con cualquier cosa, no solo el escudo).
        Destroy(gameObject);
    }
}