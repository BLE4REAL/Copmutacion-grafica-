#iifndef CUSTOM_LIGHTING_G1
#define CUSTOM_LIGHTING_G1
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_float(float3 PositionWS, out float3 Direction, out float3 Color, out float ShadowAttenuation) 

{
#if defined (SHADERGRAPH_PREVIEW)
    Direction = normalize(float3(1, 1, -1));
    Color = 1.0f;
    ShadowAttenuation = 1.0f;
    
    
    #else 
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Light MainLight = GetMainLight(shadowCoord);
    
    Direction = MainLight.direction;
    Color = MainLight.color;
    ShadowAttenuation = MainLight.shadowAttenuation;
    #endif
}
void AdditionalLightSimple_float(float2 uvss, float3 PositionWS, float3 viewDirectionWS, float3 NormalWS, out float3 Lit)
{
    #ifdef  SHADERGRAPH_PREVIEW
Lit = 0;
#else
    unit additionalLightCount = GetAdditionalLightCount();
    
    #ifdef USE_FOWARD_PLUS
    InputData inputData = (inputData)0;
    inputData.normalizedScreenSpaceUV = uvss;
    inputData.Position = PositionWS;
    #endif

    
    LIGHT_LOOP_BEGIN(additionalLightCount)
    Light currentLight = GetAdditionalLight(lightIndex, PositionWS);
    
    
    float lambert = dot(currentLight.direction, NormalWS);
    lambert = max(0, lambert * 0.5f + 0.5f);
    * currentLight.color
    * currentLight.shadowAttenuation
    * currentLight.distanceAttenuation;

    Float3 h = normalize(viewDirections + currentLight.direction);
    float blinnPhong = dot(h, NormalWS);
    blinnPhong = max(0, blinnPhong);
    blinnPhong = pow(blinnPhong, 60.0f);
    
    float specular = blinnPhong 
    * currentLight.color
    * currentLight.shadowAttenuation
    * currentLight.distanceAttenuation;
    
    Lit += diffuse + specular;
    
    float diffuse = lambert * currentLight.color * currentLight.shadowAttenuation * currentLight.distanceAttenuation;
    LIGHT_LOOP_END
    
    #endif

}
#endif