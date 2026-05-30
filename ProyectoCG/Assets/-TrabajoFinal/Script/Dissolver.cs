using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Dissolver : MonoBehaviour
{
    public bool IsDissolved => Dissolved;
    public bool Dissolved = false;
    public Material dissolveMaterial;
    public float dissolveDuration = 2f; // Duración de la animación de disolución
    public float dissolveValue = 0f; // Valor actual de disolución (0 = intacto, 1 = completamente disuelto)

    private static readonly int DissolveValueID = Shader.PropertyToID("_DissolveValue");
    private static readonly int DissolveColorID = Shader.PropertyToID("_EdgeColor");
    private Color currentColor = Color.cyan;




    [Header("Post-procesado")]
    [Tooltip("Global Volume de la escena para los efectos de post-procesado")]
    public Volume postProcessVolume;

    [Tooltip("Intensidad máxima de la aberración cromática cuando el dissolve está al 100%")]
    public float chromaticAberrationMax = 1f;

    [Tooltip("Cuánto se SUMA al vignette base cuando el dissolve está al 100%")]
    [Range(0f, 1f)]
    public float vignetteIncrease = 0.3f;

    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    private float vignetteBase = 0f; // Para almacenar el valor base del vignette y sumarle el aumento


    void Awake()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out vignette);

            if (vignette != null)
            {
                vignetteBase = vignette.intensity.value;
            }
        }
    }



    public void StartDissolverEffect()
    {
        StartCoroutine(StartDissolve());
    }
    public IEnumerator StartDissolve()
    {
        if(dissolveMaterial == null) dissolveMaterial.SetColor(DissolveColorID, currentColor);

        float elapsed = 0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            dissolveValue = Mathf.Lerp(0f, 1f, elapsed / dissolveDuration);
            UpdateDissolveShader(dissolveValue);
            yield return null;
        }

        // Aseguramos que el valor final sea exactamente 1 al terminar.
        dissolveValue = 1f;
        Dissolved = true;
        UpdateDissolveShader(dissolveValue);
    }

    public void StartAppearingEffect()
    {
        StartCoroutine(StartAppearing());
    }

    public IEnumerator StartAppearing()
    {

        float elapsed = 0f;
        while (elapsed < dissolveDuration)
        {
            elapsed += Time.deltaTime;
            dissolveValue = Mathf.Lerp(1f, 0f, elapsed / dissolveDuration);
            UpdateDissolveShader(dissolveValue);
            yield return null;
        }

        // Aseguramos que el valor final sea exactamente 0 al terminar.
        dissolveValue = 0f;
        Dissolved = false;
        UpdateDissolveShader(dissolveValue);
    }

    private void UpdateDissolveShader(float value)
    {
        // Aquí deberías actualizar el material del objeto para reflejar el valor de disolución.
        // Esto depende de cómo hayas configurado tu shader. Por ejemplo:
        if (dissolveMaterial != null)
        {
            dissolveMaterial.SetColor(DissolveColorID, currentColor);
            dissolveMaterial.SetFloat(DissolveValueID, value);
        }




        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = value * chromaticAberrationMax;
        }
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Clamp01(vignetteBase + value * vignetteIncrease);
        }
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        if (dissolveMaterial != null) dissolveMaterial.SetColor(DissolveColorID, color);
    }

}
