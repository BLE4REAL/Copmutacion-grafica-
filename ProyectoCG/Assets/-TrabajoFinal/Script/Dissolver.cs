using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    }

    public void SetColor(Color color)
    {
        currentColor = color;
        if (dissolveMaterial != null) dissolveMaterial.SetColor(DissolveColorID, color);
    }

}
