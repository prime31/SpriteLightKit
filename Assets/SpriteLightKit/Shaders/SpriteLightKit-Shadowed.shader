// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// note that the sprite must have it's pivot in the center for this to work
Shader "prime[31]/Sprite Light Kit/Shadowed"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_ShadowColor ( "Shadow Color", Color ) = ( 0, 0, 0.2, 0.9 )
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		_HorizontalSkew ( "Horizontal Skew", Float ) = 0
		_VerticalScale ( "Vertical Scale", Float ) = 1
		_VerticalOffset ( "Vertical Offset", Float ) = 0

		_HorizontalTranslation ( "Horizontal Translation", Float ) = 0
		_VerticalTranslation ( "Vertical Translation", Float ) = 0
	}


	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DUMMY PIXELSNAP_ON
#include "UnityCG.cginc"

struct appdata_t
{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 vertex   : SV_POSITION;
	fixed4 color    : COLOR;
	half2 texcoord  : TEXCOORD0;
};

sampler2D _MainTex;
fixed4 _Color;
fixed4 _ShadowColor;
float _HorizontalSkew;
float _VerticalScale;
float _VerticalOffset;
float _HorizontalTranslation;
float _VerticalTranslation;


v2f vert( appdata_t IN )
{
	v2f OUT;
	OUT.texcoord = IN.texcoord;
	
	float4x4 transformMatrix = float4x4
	(
		1, _HorizontalSkew, 0, _HorizontalTranslation,
		0, _VerticalScale, 0, _VerticalTranslation,
		0, 0, 1, 0,
		0, _VerticalOffset, 0, 1
	);
	
	float4 skewedVertex = mul( transformMatrix, IN.vertex );
	OUT.vertex = UnityObjectToClipPos( skewedVertex );
	
	#ifdef PIXELSNAP_ON
	OUT.vertex = UnityPixelSnap( OUT.vertex );
	#endif

	return OUT;
}

fixed4 frag( v2f IN ) : SV_Target
{
	fixed4 c = tex2D( _MainTex, IN.texcoord );
	return c.a * _ShadowColor;
}
ENDCG
		} // end Pass



Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DUMMY PIXELSNAP_ON
#include "UnityCG.cginc"

struct appdata_t
{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 vertex   : SV_POSITION;
	fixed4 color    : COLOR;
	half2 texcoord  : TEXCOORD0;
};

fixed4 _Color;

v2f vert(appdata_t IN)
{
	v2f OUT;
	OUT.vertex = UnityObjectToClipPos( IN.vertex );
	OUT.texcoord = IN.texcoord;
	OUT.color = IN.color * _Color;
	#ifdef PIXELSNAP_ON
	OUT.vertex = UnityPixelSnap( OUT.vertex );
	#endif

	return OUT;
}

sampler2D _MainTex;

fixed4 frag(v2f IN) : SV_Target
{
	fixed4 c = tex2D( _MainTex, IN.texcoord ) * IN.color;
	c.rgb *= c.a;
	return c;
}
ENDCG
} // end pass




	} // end SubShader
}
