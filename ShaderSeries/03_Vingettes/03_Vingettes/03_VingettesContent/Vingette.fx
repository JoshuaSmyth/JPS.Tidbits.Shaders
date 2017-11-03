// Our texture sampler
texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

float AspectRatio;

float R = 1.5f;
bool EnableSepia;
bool PreserveAspectRatio;
float2 Centre;

// This data comes from the sprite batch vertex shader
struct VertexShaderOutput
{
    float2 Position : TEXCOORD0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};


float4 Sepia(VertexShaderOutput input)
{
    float4 color = tex2D(TextureSampler, input.TextureCoordinate);
	
	if (EnableSepia)
	{
		float3x3 sepiaMatrix ={0.393, 0.349, 0.272,
						0.769, 0.686, 0.534 ,
						0.189, 0.168, 0.131};

		float4 result;
		result.rgb = mul(color.rgb, sepiaMatrix);
		result.a = 1.0f;

		return result;
	}
	
	return color;
}

float4 PassThrough(VertexShaderOutput input) : COLOR0
{
	return Sepia(input);
}


float4 VingetteShader(VertexShaderOutput input) : COLOR0
{
	float4 color = Sepia(input);

	float dX = abs(input.TextureCoordinate.x - Centre.x)*R;
	float dY = abs(input.TextureCoordinate.y - Centre.y)*R;
	
	if (PreserveAspectRatio)
		dY *= (1/AspectRatio);

	float p = dX*dX + dY*dY;	// Squared distance
    float d = 1 - 0.2f*p - 0.2*p*p;

	color.rgb = saturate(color.rgb*d);

    return color;
}

float4 HorizontalVingetteShader(VertexShaderOutput input) : COLOR0
{
	float4 color = Sepia(input);

	float dY = abs(input.TextureCoordinate.y - Centre.y)*R;

	float p = dY*dY;	// Squared distance
    float d = 1 - 0.2f*p - 0.2*p*p;

	color.rgb = saturate(color.rgb*d);

    return color;
}

float4 VerticalVingetteShader(VertexShaderOutput input) : COLOR0
{
	float4 color = Sepia(input);

	float dX = abs(input.TextureCoordinate.x - Centre.x)*R;

	float p = dX*dX;	// Squared distance
    float d = 1 - 0.2f*p - 0.2*p*p;

	color.rgb = saturate(color.rgb*d);

    return color;
}

technique StandardTechnique
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PassThrough();
    }
}

technique Vingette
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 VingetteShader();
    }
}

technique HorizontalVingette
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 HorizontalVingetteShader();
    }
}

technique VerticalVingette
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 VerticalVingetteShader();
    }
}