// Parameters
float2 Center = float2(0.5, 0.5);	///center of the screen (could be any place)
float BlurStart = 0.5f;				/// blur offset
float BlurWidth = -0.05;			///how big it should be
int nsamples = 5;

// Non-Texture distortion
float Time; // effect elapsed time
float3 ShockParams = float3(10,0.1,0.1); // 10.0, 0.8, 0.1

// Texture Distortion

// Our texture sampler
texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

// Distortion Texture
texture InputTexture; // The distortion map.
sampler inputTexture = sampler_state
{
    texture = <InputTexture>;
};

// This data comes from the sprite batch vertex shader
struct VertexShaderOutput
{
    float2 Position : TEXCOORD0;
	float4 Color : COLOR0;
	float2 TextureCoordinate : TEXCOORD0;
};



float4 PS_RadialBlur(float2 UV	: TEXCOORD0 ) : COLOR
{
    UV -= Center;
    float4 c = 0;
    for(int i=0; i <5; i++) {
    	float scale = BlurStart + BlurWidth*(i/(float) (5-1));
    	c += tex2D(TextureSampler, UV * scale + Center );
   	}
   	c /= nsamples;
    return c;
}


float4 Distortion(float2 texCoord: TEXCOORD0) : COLOR 
{ 
	// TODO Try rendering to a backbuffer

  float2 uv = texCoord;


  float distance = length(texCoord - Center);
  float t = Time * 1.5f;
  float z = ShockParams.z;


	if ((distance <= (t + z)) && (distance >= (t - z)) ) 
	{
		float diff = (distance - t); 
		float powDiff = 1.0 - pow(diff*ShockParams.x, ShockParams.y); 
			

		float diffTime = diff  * powDiff; 
		float2 diffUV = normalize(uv - Center); 
			
		texCoord = uv + (diffUV * diffTime);
		
		//return float4(diffUV.x, diffUV.y, powDiff, 1);
	} 
  
 
	return tex2D(TextureSampler, texCoord);    
};

float4 DistortionFade(float2 texCoord: TEXCOORD0) : COLOR 
{ 
 float2 uv = texCoord;
  float distance = length(texCoord - Center);
  if ( (distance <= (Time + ShockParams.z)) && (distance >= (Time - ShockParams.z)) ) 
  {
    float diff = (distance - Time); 
    float powDiff = 1.0 - pow(abs(diff*ShockParams.x), ShockParams.y); 
    float diffTime = diff  * powDiff; 
    float2 diffUV = normalize(uv - Center); 
    texCoord = uv + (diffUV * diffTime);

	float4(diffUV, 1, 1);
  } 

  return tex2D(TextureSampler, texCoord);    
};


float4 PS_TextureDistort (float2 texCoord: TEXCOORD0) : COLOR0
{
	float2 coords = texCoord;
    float4 color1 = tex2D(inputTexture, texCoord);

    // 0.1 seems to work nicely.
    float mul = (color1.b * 0.2);

    coords.x += (color1.r * mul) - mul / 2;
    coords.y += (color1.g * mul) - mul / 2;

    return tex2D(TextureSampler, coords);
}


technique TextureDistort
{
    pass P0
    {
        PixelShader = compile ps_2_0 PS_TextureDistort();
    }
}

technique Distort
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 Distortion();
    }
}
technique StandardTechnique
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PS_RadialBlur();
    }
}

