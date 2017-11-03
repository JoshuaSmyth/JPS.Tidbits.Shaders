float4x4 WorldViewProjection;
float3 CameraPosition;
float4 lightDirection = { 1, -0.7, 1, 0};
float textureScale = 0.04f;
float plateu = 0.5f;

texture gTex0;
sampler ColorMapSampler = sampler_state
{
   Texture = <gTex0>;
   MinFilter = ANISOTROPIC;
   MagFilter = ANISOTROPIC;
   MipFilter = Linear;   
   AddressU  = Wrap;
   AddressV  = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    float2 Sample1 : TEXCOORD1;
    float2 Sample2 : TEXCOORD2;
    float2 Sample3 : TEXCOORD3;
    float3 BlendWeights : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Normal = input.Normal;

// Calculate the UV position based on the model's world space
    float3 worldSample = output.Position + CameraPosition;

    // The sign part removes the symetry
    output.Sample1 = worldSample.yz * textureScale + sign(input.Position.x) * 0.25f;
    output.Sample2 = worldSample.zx * textureScale;
    output.Sample3 = worldSample.xy * textureScale + sign(input.Position.z) * 0.25f;

    // Scale the blend weights
    output.BlendWeights = max(abs(output.Normal) - plateu, 0);
    output.BlendWeights /= (output.BlendWeights.x + output.BlendWeights.y + output.BlendWeights.z).xxx;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
   float3 blend_weights = input.BlendWeights;
   
   float4 col1 = tex2D(ColorMapSampler, input.Sample1);    // Side A
   float4 col2 = tex2D(ColorMapSampler, input.Sample2);    // Ceiling
   float4 col3 = tex2D(ColorMapSampler, input.Sample3);    // Side B

   float4 blended_color = col1.xyzw * blend_weights.xxxx + col2.xyzw * blend_weights.yyyy + col3.xyzw * blend_weights.zzzz;

   // directional lighting
   float4 light = -normalize(lightDirection);
   float ldn = max(0, dot(light, input.Normal));
   float ambient = 0.95f;

   return float4(blended_color.xyz * (ambient + ldn), 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
