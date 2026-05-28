using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectsDemoController : MonoBehaviour
{
    [Header("Referencias de efectos")]
    [Tooltip("Sistema de partículas del Botón 1 (aura)")]
    public ParticleSystem auraEffect;

    [Tooltip("Renderer del personaje que usa el shader de disolución (Botón 2)")]
    public Renderer dissolveRenderer;

    [Tooltip("Script del escudo de energía (Botón 3)")]
    public EnergyShield energyShield;

    [Header("Proyectiles (Botón 4)")]
    [Tooltip("Prefab del proyectil a instanciar")]
    public GameObject projectilePrefab;

    [Tooltip("El personaje, hacia donde apuntan los proyectiles")]
    public Transform target;

    [Tooltip("Cuántos proyectiles se lanzan por tanda")]
    public int projectilesPerVolley = 5;

    [Tooltip("Distancia desde el personaje a la que aparecen")]
    public float spawnRadius = 6f;

    [Tooltip("Velocidad inicial de los proyectiles")]
    public float projectileSpeed = 12f;

    [Tooltip("Tiempo entre cada proyectil de la tanda (segundos)")]
    public float timeBetweenProjectiles = 0.2f;


    [Header("UI - Sliders de color")]
    [Tooltip("Slider del matiz (recorre el círculo cromático)")]
    public Slider colorSlider;        // matiz (hue)

    [Tooltip("Slider de saturación (qué tan vivo es el color)")]
    public Slider saturationSlider;

    [Tooltip("Slider de brillo (qué tan claro u oscuro es)")]
    public Slider valueSlider;

    // Material instanciado del shader de disolución
    private Material dissolveMaterial;

    // Color actual elegido por los sliders
    private Color currentColor = Color.cyan;

    void Awake()
    {
        if (dissolveRenderer != null)
        {
            dissolveMaterial = dissolveRenderer.material;
        }
    }

    void Start()
    {
        // Valores iniciales (se asignan ANTES de los listeners para no disparar
        // el evento al asignarlos).
        if (saturationSlider != null) saturationSlider.value = 0.8f;
        if (valueSlider != null) valueSlider.value = 0.5f;

        // Los tres sliders disparan el mismo método.
        if (colorSlider != null)
            colorSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);
        if (saturationSlider != null)
            saturationSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);
        if (valueSlider != null)
            valueSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);

        // Aplicamos el color inicial una vez al arrancar.
        OnAnyColorSliderChanged(0f);
    }

    // ---------- BOTÓN 1: Partículas (aura) ----------
    public void OnButton1_Aura()
    {
        // TODO: disparar/alternar el aura.
        Debug.Log("Botón 1: aura de partículas");
    }

    // ---------- BOTÓN 2: Shader de disolución ----------
    public void OnButton2_Dissolve()
    {
        // TODO: animar la propiedad de disolución del material.
        Debug.Log("Botón 2: disolución");
    }

    // ---------- BOTÓN 3: Escudo de energía ----------
    public void OnButton3_Shield()
    {
        if (energyShield != null)
        {
            energyShield.ToggleShield();
        }
    }

    // ---------- BOTÓN 4: Lanzar proyectiles ----------
    public void OnButton4_LaunchProjectiles()
    {
        if (projectilePrefab == null || target == null)
        {
            Debug.LogWarning("Falta asignar projectilePrefab o target");
            return;
        }

        StartCoroutine(LaunchVolley());

        //for (int i = 0; i < projectilesPerVolley; i++)
        //{
        //    Vector2 circle = Random.insideUnitCircle.normalized;
        //    Vector3 spawnPos = target.position
        //        + new Vector3(circle.x, 0f, circle.y) * spawnRadius
        //        + Vector3.up * 1.2f;

        //    GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        //    Vector3 dir = (target.position + Vector3.up * 1.2f - spawnPos).normalized;

        //    Rigidbody rb = proj.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        rb.linearVelocity = dir * projectileSpeed;
        //    }
        //}
    }

    // ---------- SLIDERS de color (matiz, saturación, brillo) ----------
    public void OnAnyColorSliderChanged(float _)
    {
        float h = colorSlider != null ? colorSlider.value : 0f;
        float s = saturationSlider != null ? saturationSlider.value : 1f;
        float v = valueSlider != null ? valueSlider.value : 1f;

        currentColor = Color.HSVToRGB(h, s, v);

        Color ringColor = Color.HSVToRGB(h, 1f, 1f);

        if (energyShield != null)
        {
            energyShield.SetColor(currentColor);
            energyShield.SetRingColor(ringColor);
        }

        // TODO: aplicar también a partículas y material de disolución
        // cuando estén implementados.
    }

    private IEnumerator LaunchVolley()
    {
        for (int i = 0; i < projectilesPerVolley; i++)
        {
            SpawnOneProjectile();
            yield return new WaitForSeconds(timeBetweenProjectiles);
        }
    }

    private void SpawnOneProjectile()
    {
        Vector2 circle = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = target.position
            + new Vector3(circle.x, 0f, circle.y) * spawnRadius
            + Vector3.up * 1.2f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 dir = (target.position + Vector3.up * 1.2f - spawnPos).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }
    }
}