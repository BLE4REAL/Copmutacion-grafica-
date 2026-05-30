using System.Collections;
using UnityEngine;

public class EnergyShield : MonoBehaviour
{
    [Tooltip("Duración de la animación de aparición/desaparición")]
    public float buildDuration = 0.6f;

    [Tooltip("Collider del escudo; se activa solo cuando el escudo está formado")]
    public Collider shieldCollider;

    [Tooltip("Duración del destello de impacto (segundos)")]
    public float hitDuration = 0.5f;

    private Coroutine hitAnimation;

    

    public bool IsActive => isActive;
    private Material shieldMaterial;
    private bool initialized = false;
    private bool isActive = false;
    private Coroutine activeAnimation;

    private Color currentColor = Color.cyan;

    private static readonly int HitPositionID = Shader.PropertyToID("_HitPosition");
    private static readonly int HitTimeID = Shader.PropertyToID("_HitTime");
    private static readonly int BuildProgressID = Shader.PropertyToID("_BuildProgress");
    private static readonly int ShieldColorID = Shader.PropertyToID("_ShieldColor");
    private static readonly int RingColorID = Shader.PropertyToID("_RingColor");
    private static readonly int HaloColorID = Shader.PropertyToID("_HaloColor");

    // Inicialización perezosa: cachea el material la primera vez que se necesita,
    // sin importar si el GameObject empezó activo o desactivado.
    private void EnsureInitialized()
    {
        if (initialized) return;
        Renderer rend = GetComponent<Renderer>();
        if (rend != null) shieldMaterial = rend.material;
        initialized = true;
    }

    public void ToggleShield()
    {
        if (!isActive) ActivateShield();
        else DeactivateShield();
    }

    private void ActivateShield()
    {
        isActive = true;

        // 1) Activar el GameObject ANTES de la corutina (no corren en objetos apagados).
        gameObject.SetActive(true);

        EnsureInitialized();

        // Reaplicar color por si el slider se movió mientras estaba apagado.
        if (shieldMaterial != null) shieldMaterial.SetColor(ShieldColorID, currentColor);

        if (activeAnimation != null) StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(AnimateBuildProgress(1f, false));
    }

    private void DeactivateShield()
    {
        isActive = false;

        // El collider se apaga apenas empieza a consumirse.
        if (shieldCollider != null) shieldCollider.enabled = false;

        if (activeAnimation != null) StopCoroutine(activeAnimation);
        // El "true" indica que al terminar debe apagar el GameObject.
        activeAnimation = StartCoroutine(AnimateBuildProgress(0f, true));
    }

    private IEnumerator AnimateBuildProgress(float target, bool disableOnFinish)
    {
        EnsureInitialized();
        if (shieldMaterial == null) yield break;

        float start = shieldMaterial.GetFloat(BuildProgressID);
        float elapsed = 0f;
        float distance = Mathf.Abs(target - start);
        float duration = Mathf.Max(0.0001f, buildDuration * distance);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            shieldMaterial.SetFloat(BuildProgressID, Mathf.Lerp(start, target, smoothT));
            yield return null;
        }

        shieldMaterial.SetFloat(BuildProgressID, target);
        activeAnimation = null;

        // Escudo formado: activamos su collider.
        if (!disableOnFinish && shieldCollider != null) shieldCollider.enabled = true;

        // Escudo consumido: ahora sí apagamos el GameObject.
        if (disableOnFinish) gameObject.SetActive(false);
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        EnsureInitialized();
        if (shieldMaterial != null) shieldMaterial.SetColor(ShieldColorID, color);
    }

    public void SetHaloColor(Color color)
    {
        EnsureInitialized();
        if (shieldMaterial != null) shieldMaterial.SetColor(HaloColorID, color);
    }

    public void SetRingColor(Color color)
    {
        EnsureInitialized();
        if (shieldMaterial != null) shieldMaterial.SetColor(RingColorID, color);
    }

    public void RegisterHit(Vector3 worldPosition)
    {
        EnsureInitialized();
        if (shieldMaterial == null) return;

        // Le decimos al shader DÓNDE pegó el proyectil.
        shieldMaterial.SetVector(HitPositionID, worldPosition);

        // Arrancamos la animación de la onda. Si había una corriendo,
        // la cancelamos para que este nuevo impacto empiece desde cero.
        if (hitAnimation != null) StopCoroutine(hitAnimation);
        hitAnimation = StartCoroutine(AnimateHit());
    }

    private IEnumerator AnimateHit()
    {
        float elapsed = 0f;
        // Empezamos con el tiempo en 0 (anillo en el punto de impacto).
        shieldMaterial.SetFloat(HitTimeID, 0f);

        while (elapsed < hitDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hitDuration;
            shieldMaterial.SetFloat(HitTimeID, t);
            yield return null;
        }

        // Al final dejamos en 1 (anillo invisible por el fade).
        shieldMaterial.SetFloat(HitTimeID, 1f);
        hitAnimation = null;
    }



}