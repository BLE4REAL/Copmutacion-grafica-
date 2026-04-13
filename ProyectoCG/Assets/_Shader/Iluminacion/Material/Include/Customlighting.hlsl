//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_float (float3 PositionWS, out float3 Direction, out float3 Color, out float ShadowAttenuation)
{
    #if defined(SHADERGRAPH_PREVIEW)
    Direction = normalize(float3(1, 1, -1));
    Color = 1.0f;
    ShadowAttenuation = 1.0f;
    #else
    float4 ShadowCoord = TransformWorldToShadowCoord(PositionWS);
    Light mainLight = GetMainLight(ShadowCoord);
    
    Direction = mainLight.direction;
    Color = mainLight.color;
    ShadowAttenuation = mainLight.shadowAttenuation;
    #endif
}

void AdditionalLightsSimple_float(float3 PositionWS, float3 ViewDirectionWS,float3 NormalWS, out float3 Lit)
{
    #ifdef SHADERGRAPH_PREVIEW
    Lit = 0
    #else
    uint additionalLightCount = GetAdditionalLightsCount();

    //TODO: Forward+

    LIGHT_LOOP_BEGIN(additionalLightCount)
    Light currentLight = GetAdditionalLight(lightIndex, PositionWS);

    //diffuse
    float lambert = dot(currentLight.direction, NormalWS);
    lambert = max(0, lambert * 0.5 + 0.5f); //half-lambert
    
    float diffuse = lambert * currentLight.color * currentLight.shadowAttenuation * currentLight.distanceAttenuation;

    //Specular
    float3 h= normalize(ViewDirectionWS * currentLight.direction);
    float blinnphong = dot(h, NormalWS);
    blinnphong = max(0, blinnphong);
    blinnphong = pow(blinnphong, 60.0f);
    float3 specular = blinnphong * currentLight.color * currentLight.shadowAttenuation * currentLight.distanceAttenuation;

    Lit += diffuse + specular;
    LIGHT_LOOP_END
    
    #endif
}