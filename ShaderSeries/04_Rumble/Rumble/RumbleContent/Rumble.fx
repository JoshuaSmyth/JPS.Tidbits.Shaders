float4x4 World;
float4x4 View;
float4x4 Projection;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

float2 RumbleVector;
float2 RumbleVectorR;
float2 RumbleVectorG;
float2 RumbleVectorB;

struct VertexShaderOutput
{
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};

float4 Rumble(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TextureCoordinate + RumbleVector);
	return color;
}

float4 RgbRumble(VertexShaderOutput input) : COLOR0
{
    float3 colorR = tex2D(TextureSampler, input.TextureCoordinate + RumbleVectorR);
	float3 colorG = tex2D(TextureSampler, input.TextureCoordinate + RumbleVectorG);
    float3 colorB = tex2D(TextureSampler, input.TextureCoordinate + RumbleVectorB);

	float4 returnColor;
	returnColor.r = colorR.r;
	returnColor.g = colorG.g;
	returnColor.b = colorB.b;
	returnColor.a = 1.0f;
	return returnColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 Rumble();
    }
}

technique Technique2
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 RgbRumble();
    }
}
