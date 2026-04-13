
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"



void MainLight_float(float3 PositionWS, out float3 Direction, out float3 Color, out float ShadowAttenuation)
{
    #if defined(SHADERGRAPH_PREVIEW)
    Direction = normalize(float3(1, 1, -1));
    Color = 1.0f;
    ShadowAttenuation = 1.0f;
    
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Light mainLight = GetMainLight(shadowCoord);
    
    Direction = mainLight.direction;
    Color = mainLight.color;
    ShadowAttenuation = mainLight.shadowAttenuation;
    #endif
}

void AdditionalLightsSimple_float(float3 PositionWS, float3 ViewDirectonWS, float3 NormalWS, out float3 Lit)
{
    #if defined(SHADERGRAPH_PREVIEW)
    Lit = 0;
    #else
    uint additionalLightCount = GetAdditionalLightsCount();
    
    //TODO: Forward+
    
    LIGHT_LOOP_BEGIN(additionalLightCount)_
    Light currentLight = GetAdditionalLight(lightIndex, PositionWS);
    
    //Diffuse
    float lambert = dot(currentLight.direction, NormalWS);
    lambert = max(0, lambert * 0.5f + 0.5f); //half lambert
    
    float diffuse = lambert * currentLight.color * currentLight.shadowAttenuation * currentLight.distanceAttenuation;
    
    //Specular
    float3 h = normalize(ViewDirectonWS + currentLight.direction);
    float blinnPhong = dot(h, NormalWS);
    blinnPhong = max(0, blinnPhong);
    blinnPhong = pow(blinnPhong, 60.0f);
    float specular = blinnPhong * currentLight.color * currentLight.shadowAttenuation * currentLight.distanceAttenuation;
    
    Lit += difuse + specular;
    
    LIGHT_LOOP_END_
    
    #endif
}