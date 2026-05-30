using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsDemoController : MonoBehaviour
{
    
    [Header("Animator del personaje")]
    public Animator characterAnimator;

    [Header("Referencias de efectos")]

    [Header("Efecto 1: Aura")]
    [Tooltip("Sistema de partículas del Botón 1 (aura)")]
    public ParticleSystem auraEffect;

    [Header("Efecto 2: Disolución")]
    [Tooltip("Renderer del personaje que usa el shader de disolución (Botón 2)")]
    public Dissolver dissolver;

    [Header("Efecto 3: Escudo de energía")]
    [Tooltip("Script del escudo de energía (Botón 3)")]
    public EnergyShield energyShield;

    [Header("Boton 4: Proyectiles")]
    [Tooltip("Prefab del proyectil a instanciar")]
    public GameObject projectilePrefab;

    [Space(20)]
    [Header("Configuración de efectos")]

    [SerializeField] private AuraSettings auraSettings;

    [SerializeField] private DissolverSettings dissolverSettings;

    [SerializeField] private EscudoSettings escudoSettings;

    [SerializeField] private ProjectileSettings projectileSettings;

    [SerializeField] private UISettings uiSettings;

    private static readonly int ColorID = Shader.PropertyToID("_Color");
    



    void Awake()
    {
        if (dissolver != null)
        {
            dissolverSettings.dissolveMaterial = dissolver.dissolveMaterial;
        }

        if (auraSettings.auraMaterial1 != null)
            auraSettings.originalColor1 = auraSettings.auraMaterial1.GetColor(ColorID);
        if (auraSettings.auraMaterial2 != null)
            auraSettings.originalColor2 = auraSettings.auraMaterial2.GetColor(ColorID);
    }

    void Start()
    {
        // Valores iniciales (se asignan ANTES de los listeners para no disparar
        // el evento al asignarlos).
        dissolverSettings.dissolveMaterial.SetFloat("_DissolveValue", 0f);

        if (uiSettings.saturationSlider != null) uiSettings.saturationSlider.value = 0.8f;
        if (uiSettings.valueSlider != null) uiSettings.valueSlider.value = 0.5f;

        // Los tres sliders disparan el mismo método.
        if (uiSettings.colorSlider != null)
            uiSettings.colorSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);
        if (uiSettings.saturationSlider != null)
            uiSettings.saturationSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);
        if (uiSettings.valueSlider != null)
            uiSettings.valueSlider.onValueChanged.AddListener(OnAnyColorSliderChanged);

        // Aplicamos el color inicial una vez al arrancar.
        OnAnyColorSliderChanged(0f);
    }

    // ========== BOTÓN 1: Partículas (aura) ============================================================
    public void OnButton1_Aura()
    {
        if (auraEffect == null || auraSettings == null || auraSettings.auraController == null) return;

        bool willActivate = !auraSettings.isActive;

        if (willActivate)
        {
            // ACTIVAR: animación primero, efecto después.
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("Fight");
            }
            StartCoroutine(ActivateAuraAfterDelay());
        }
        else
        {
            // DESACTIVAR: fade primero, animación después.
            StartCoroutine(DeactivateAuraSequence());
        }
    }

    private IEnumerator ActivateAuraAfterDelay()
    {
        yield return new WaitForSeconds(auraSettings.auraActivationDelay);

        SetAuraBrightness(1f); // restaurar brillo antes de mostrar
        auraSettings.auraController.SetActive(true);
        auraSettings.isActive = true;
        auraEffect.Play();
    }

    private IEnumerator DeactivateAuraSequence()
    {
        // Fase 1: fade out gradual.
        float elapsed = 0f;
        while (elapsed < auraSettings.auraFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float brightness = Mathf.Lerp(1f, 0f, elapsed / auraSettings.auraFadeOutDuration);
            SetAuraBrightness(brightness);
            yield return null;
        }

        // Fase 2: apagar el efecto y el GameObject.
        auraEffect.Stop();
        auraSettings.auraController.SetActive(false);
        auraSettings.isActive = false;

        // Fase 3: restaurar el color original ya que el GameObject está apagado
        // (no se nota visualmente porque está invisible, pero deja el material
        // en buen estado para la próxima activación o si se corta el Play).
        SetAuraBrightness(1f);

        // Fase 4: animación del personaje.
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("StopFight");
        }
    }

    private void SetAuraBrightness(float brightness)
    {
        SetMaterialBrightness(auraSettings.auraMaterial1, auraSettings.originalColor1, brightness);
        SetMaterialBrightness(auraSettings.auraMaterial2, auraSettings.originalColor2, brightness);
    }

    private void SetMaterialBrightness(Material mat, Color originalColor, float brightness)
    {
        if (mat == null) return;

        Color faded = originalColor * brightness;
        faded.a = originalColor.a; // alpha intacto

        mat.SetColor(ColorID, faded);
    }

    // ========== BOTÓN 2: Shader de disolución ================================================================
    public void OnButton2_Dissolve()
    {
        if (dissolver == null) return;

        bool willDissolved = !dissolver.IsDissolved;
        
        if (willDissolved)
        {
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("Dissolve");
            }
            StartCoroutine(StartDissolving());
        }
        else
        {

            dissolver.StartAppearingEffect();
            StartCoroutine(StartAnimationAppearing());

        }
        
    }

    private IEnumerator StartDissolving()
    {
        
        yield return new WaitForSeconds(dissolverSettings.dissolveDelay);
        
        dissolver.StartDissolverEffect();
    }

    private IEnumerator StartAnimationAppearing()
    {
        
        yield return new WaitForSeconds(dissolverSettings.appearAnimationDelay);
    
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Appear");
        }

    }

    // ========== BOTÓN 3: Escudo de energía ============================================================
    public void OnButton3_Shield()
    {
        if (energyShield == null) return;

        bool willActivate = !energyShield.IsActive;

        if (willActivate)
        {
            // ACTIVACIÓN: arrancamos la animación primero, el escudo se activa
            // después del retraso configurado.
            if (characterAnimator != null)
            {
                characterAnimator.SetTrigger("Cast");
            }
            StartCoroutine(ActivateShieldAfterDelay());
        }
        else
        {
            // DESACTIVACIÓN: inmediata, sin animación del personaje.
            energyShield.ToggleShield();
        }
    }

    private IEnumerator ActivateShieldAfterDelay()
    {
        yield return new WaitForSeconds(escudoSettings.castAnimationDelay);
        energyShield.ToggleShield();
    }



    // ========== BOTÓN 4: Lanzar proyectiles =====================================================
    public void OnButton4_LaunchProjectiles()
    {
        if (projectilePrefab == null || projectileSettings.target == null)
        {
            Debug.LogWarning("Falta asignar projectilePrefab o target");
            return;
        }

        StartCoroutine(LaunchVolley());

    }
    private IEnumerator LaunchVolley()
    {
        for (int i = 0; i < projectileSettings.projectilesPerVolley; i++)
        {
            SpawnOneProjectile();
            yield return new WaitForSeconds(projectileSettings.timeBetweenProjectiles);
        }
    }

    private void SpawnOneProjectile()
    {
        Vector2 circle = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = projectileSettings.target.position
            + new Vector3(circle.x, 0f, circle.y) * projectileSettings.spawnRadius
            + Vector3.up * 1.2f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector3 dir = (projectileSettings.target.position + Vector3.up * 1.2f - spawnPos).normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSettings.projectileSpeed;
        }
    }

    // ========== BOTÓN 5: Play/Pause ============================================================

    public void OnButton5_PlayPause()
    {
        uiSettings.isPaused = !uiSettings.isPaused;

        Time.timeScale = uiSettings.isPaused ? 0f : 1f;

        if (uiSettings.pauseButtonText != null)
        {
            uiSettings.pauseButtonText.text = uiSettings.isPaused ? "Play" : "Pause";
        }
    }

    // ========== SLIDERS de color (matiz, saturación, brillo) ===========================================================
    public void OnAnyColorSliderChanged(float _)
    {
        float h = uiSettings.colorSlider != null ? uiSettings.colorSlider.value : 0f;
        float s = uiSettings.saturationSlider != null ? uiSettings.saturationSlider.value : 1f;
        float v = uiSettings.valueSlider != null ? uiSettings.valueSlider.value : 1f;

        uiSettings.currentColor = Color.HSVToRGB(h, s, v);
        uiSettings.saturationFillImage.color = Color.HSVToRGB(h, 1f, 1f);
        uiSettings.valueFillImage.color = Color.HSVToRGB(h, 1f, 1f);

        float t = h - 0.3f;
        if (t < 0f) t += 1f; else if (t > 1f) t -= 1f;

        Color ringColor = Color.HSVToRGB(h, 1f, 1f);
        Color haloColor = Color.HSVToRGB(t, 1f, 2f);
        if (v<= 0.1f) haloColor = Color.HSVToRGB(t, 1f, 2f * (10 * v));
        Color DissolveColor = Color.HSVToRGB(h, 1f, 3f);


        if (energyShield != null)
        {
            energyShield.SetColor(uiSettings.currentColor);
            energyShield.SetRingColor(ringColor);
            energyShield.SetHaloColor(haloColor);
        }

        if (escudoSettings.shieldAmbientParticles != null)
        {
            var main = escudoSettings.shieldAmbientParticles.main;
            main.startColor = ringColor;
        }

        if (dissolver != null)
        {
            dissolver.SetColor(DissolveColor);
        }

        // TODO: aplicar también a partículas y material de disolución
        // cuando estén implementados.
    }

}

[System.Serializable]
public class AuraSettings
{
    [Tooltip("El efecto de aura a activar")]
    public GameObject auraController;

    [Tooltip("Retraso antes de activar el aura, sincronizar con animación de Fight")]
    public float auraActivationDelay = 1f;

    [Tooltip("Duración del fade out del aura al desactivarse")]
    public float auraFadeOutDuration = 1f;

    public bool isActive = false;

    public Material auraMaterial1;
    public Material auraMaterial2;

    public Color originalColor1;
    public Color originalColor2;

}

[System.Serializable]
public class DissolverSettings
{
    [Tooltip("Retraso antes de empezar la animación de disolución, para sincronizar con la animación de dissolve")]
    public float dissolveDelay = 0.5f; // Retraso antes de empezar a disolver

    [Tooltip("Retraso después de iniciar el shader appear, antes de disparar la animación de Appear")]
    public float appearAnimationDelay = 1.0f;

    [Tooltip("Material que usa el shader de disolucion")]
    public Material dissolveMaterial;
}

[System.Serializable]
public class EscudoSettings
{
    [Header("Decoración del escudo")]
    public ParticleSystem shieldAmbientParticles;

    [Tooltip("Retraso antes de activar el escudo, para sincronizar con la animación de cast")]
    public float castAnimationDelay = 0.5f;
}

[System.Serializable]
public class ProjectileSettings
{
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

}

[System.Serializable]
public class UISettings
{
    [Header("Color Actual")]
    public Color currentColor = Color.cyan;


    [Header("UI - Sliders de color")]
    [Tooltip("Slider del matiz (recorre el círculo cromático)")]
    public Slider colorSlider;

    [Tooltip("Slider de saturación (qué tan vivo es el color)")]
    public Slider saturationSlider;
    public Image saturationFillImage; // Imagen del relleno del slider para cambiar su color

    [Tooltip("Slider de brillo (qué tan claro u oscuro es)")]
    public Slider valueSlider;
    public Image valueFillImage; // Imagen del relleno del slider para cambiar su color

    [Header("Pause")]
    [Tooltip("Texto del botón de pausa, opcional, para alternar entre Pause y Play")]
    public TMP_Text pauseButtonText;

    public bool isPaused = false;

}