//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_float(float3 PositionWS, out float3 Direction, out float3 Color) 

{
#if defined (SHADERGRAPH_PREVIEW)
    Direction = normalize(float3(1, 1, -1));
    Color = 1.0f;
    
    #else 
    float4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    Light MainLight = GetMainLight(shadowCoord);
    
    Direction = MainLight.direction;
    Color = MainLight.color;
    #endif

}