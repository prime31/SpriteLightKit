Shader "prime[31]/Sprite Light Kit Blend Post Process"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_LightsTex ( "Lights (RGB)", 2D ) = "white" {}
	}
	
	SubShader
	{
		//Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		//Blend DstColor Zero           // multiplicative
		//Blend DstColor SrcColor       // 2x multiplicative

		ZWrite Off
		ZTest Always
		Cull Off

		Pass
		{
CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


// uniforms
sampler2D _MainTex;
uniform float4 _MainTex_ST;
sampler2D _LightsTex;


struct vertexInput
{
	float4 vertex : POSITION; // position (in object coordinates, i.e. local or model coordinates)
	float4 texcoord : TEXCOORD0;  // 0th set of texture coordinates (a.k.a. “UV”; between 0 and 1)
};


struct fragmentInput
{
	float4 pos : SV_POSITION;
    float4 color : COLOR0;
    half2 uv : TEXCOORD0;
};


fragmentInput vert( vertexInput i )
{
	fragmentInput o;
	o.pos = mul( UNITY_MATRIX_MVP, i.vertex );
	o.uv = TRANSFORM_TEX( i.texcoord, _MainTex );
    
	return o;
}


half4 frag( fragmentInput i ) : COLOR
{
	half4 main = tex2D( _MainTex, i.uv );
	half4 lights = tex2D( _LightsTex, i.uv );

	return main * lights;
}

ENDCG
		} // end Pass
	} // end SubShader
	
	FallBack "Diffuse"
}
