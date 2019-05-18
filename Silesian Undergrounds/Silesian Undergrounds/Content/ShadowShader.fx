﻿#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
float2 lightSource;
Texture2D SpriteTexture;
sampler s0;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_Position;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float random(float3 position, float3 scale, int seed)
{
    return frac(sin(dot(position.xyz + seed, scale)) * 43758.5453 + seed);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(s0, input.TextureCoordinates);
    if (color.a < 0.4)
    {
        float4 right = tex2D(s0, float2(input.TextureCoordinates.x - 0.05, input.TextureCoordinates.y + 0.05));
        if (right.a > 0.4)
        {
            return float4(0, 0, 0, 0.4);
        }
        }
    return float4(0, 0, 0, 0);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};