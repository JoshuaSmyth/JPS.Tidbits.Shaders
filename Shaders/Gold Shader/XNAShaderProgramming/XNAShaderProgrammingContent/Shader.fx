// XNA 4.0 Shader Programming #2 - Diffuse light

// Matrix
float4x4 World;
float4x4 View;
float4x4 Projection;

// Light related
float4 AmbientColor;
float AmbientIntensity;

float3 LightDirection;
float4 DiffuseColor;
float DiffuseIntensity;

float4 SpecularColor;
float3 EyePosition;


// The input for the VertexShader
struct VertexShaderInput
{
    float4 Position : POSITION0;
		float2 TexCoord : TEXCOORD0;
};

// The output from the vertex shader, used for later processing
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD2;
	float3 View : TEXCOORD1;
		float2 TexCoord : TEXCOORD0;
};

texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
	Texture = <ColorMap>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
};

// The VertexShader.
VertexShaderOutput VertexShaderFunction(VertexShaderInput input,float3 Normal : NORMAL)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	float3 normal = normalize(mul(Normal, World));
	output.Normal = normal;
	output.View = normalize(float4(EyePosition,1.0) - worldPosition);
		output.TexCoord = input.TexCoord;
    return output;
}

// The Pixel Shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);

	float4 normal = float4(input.Normal, 1.0);
	float4 diffuse = saturate(dot(-LightDirection,normal));
	float4 reflect = normalize(2*diffuse*normal-float4(LightDirection,1.0));
	float4 specular = pow(saturate(dot(reflect,input.View)),32);
	
	float f0 = 50.5f;

	// Schlick Fresnel
	float4 rim = 1.2 - dot(input.View, input.Normal); // Could be viewing direction or lighting direction
	rim.a = 1.0f;

	rim = saturate(rim);
	
	//return rim*AmbientColor;

	float exponential = pow( rim, 5.0);
	float fresnel = exponential + f0 * (1.0 - exponential);
	float4 spec2 = specular * fresnel;



	rim.a = 1.0f;
	spec2.a = 1.0f;
	return color*0.1 + (AmbientColor*(spec2 + rim) + specular*5);//, float4(0.3,0.3,0.0,1.0));
}

// Our Techinique
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
