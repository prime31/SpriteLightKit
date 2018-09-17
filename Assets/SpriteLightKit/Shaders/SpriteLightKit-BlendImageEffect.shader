// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Blend Image Effect"
{
	Properties
	{
		_MainTex ( "Base (RGB)", 2D ) = "white" {}
		_LightsTex ( "Lights (RGB)", 2D ) = "white" {}
		_MultiplicativeFactor ( "Multiplier", float ) = 1.0
	}
	
	SubShader
	{
		ZWrite Off
		ZTest Always
		Cull Off

		Pass
		{
			// all emissives write 2 to the stencil buffer. We want to render everything except those pixels
			Stencil
			{
			    Ref 2
			    Comp NotEqual
			}
CGPROGRAM
#pragma fragmentoption ARB_precision_hint_fastest
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"


// uniforms
sampler2D _MainTex;
uniform float4 _MainTex_ST;
sampler2D _LightsTex;
float _MultiplicativeFactor;


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
	o.pos = UnityObjectToClipPos( i.vertex );
	o.uv = TRANSFORM_TEX( i.texcoord, _MainTex );
    
	return o;
}


half4 frag( fragmentInput i ) : COLOR
{
	half4 main = tex2D( _MainTex, i.uv );
	half4 lights = tex2D( _LightsTex, i.uv );

	return _MultiplicativeFactor * main * lights;
}

ENDCG
		} // end Pass
	} // end SubShader
	
	FallBack "Diffuse"
}
