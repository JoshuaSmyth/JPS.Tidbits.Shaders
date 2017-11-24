float4x4 matWorldViewProj;

struct OUT_DEPTH
{
     float4 Position : POSITION;
     float2 Distance : TEXCOORD0;
};

OUT_DEPTH RenderDepthMapVS(float4 vPos: POSITION)
{
    OUT_DEPTH Out;
    // Translate the vertex using matWorldViewProj.
    Out.Position = mul(vPos, matWorldViewProj);
   
    // Get the distance of the vertex between near and far clipping plane in matWorldViewProj.

    Out.Distance.xy = Out.Position.zw; //1-(Out.Position.z/Out.Position.w); 
 
    return Out;
}

float4 RenderDepthMapPS( OUT_DEPTH In ) : COLOR
{ 
    return float4(In.Distance.x / In.Distance.y, 0, 0, 1);
}

technique DepthMapShader
{
     pass P0
     {
        ZEnable = TRUE;
        ZWriteEnable = TRUE;
        AlphaBlendEnable = FALSE;
  
        VertexShader = compile vs_2_0 RenderDepthMapVS();
        PixelShader  = compile ps_2_0 RenderDepthMapPS();
 
     }
}