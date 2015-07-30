Shader "prime[31]/Sprite Light"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off

		Blend SrcAlpha One // additive blending

		// TODO: expose these as a Property
		//Blend SrcAlpha OneMinusSrcAlpha // alpha blending
		//Blend One OneMinusSrcAlpha 	// premultiplied alpha blending
		//Blend One One					// additive
		//Blend OneMinusDstColor One    // soft additive


		Pass
		{
CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
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
sampler2D _MainTex;


v2f vert( appdata_t IN )
{
	v2f OUT;
	OUT.vertex = mul( UNITY_MATRIX_MVP, IN.vertex );
	OUT.texcoord = IN.texcoord;
	OUT.color = IN.color * _Color;
	#ifdef PIXELSNAP_ON
	OUT.vertex = UnityPixelSnap( OUT.vertex );
	#endif

	return OUT;
}


fixed4 frag( v2f IN ) : SV_Target
{
	fixed4 c = tex2D( _MainTex, IN.texcoord ) * IN.color;
	c.rgb *= c.a;
	return c;
}

ENDCG
		}
	}
}
