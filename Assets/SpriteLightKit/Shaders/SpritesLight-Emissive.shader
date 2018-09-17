// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "prime[31]/Sprite Light Kit/Emissive Sprite"
{
	Properties
	{
		[PerRendererData] _MainTex ( "Sprite Texture", 2D ) = "white" {}
		[MaterialToggle] PixelSnap ( "Pixel snap", Float ) = 0
		_AlphaCutoff ( "Alpha Cutoff", float ) = 0.5
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
		Blend One OneMinusSrcAlpha

		Pass
		{
			Stencil
			{
			    Ref 2
			    Comp Always
			    Pass Replace
			}

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				half2 texcoord  : TEXCOORD0;
			};

			half _AlphaCutoff;


			v2f vert( appdata_t IN )
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos( IN.vertex );
				OUT.texcoord = IN.texcoord;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap( OUT.vertex );
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag( v2f IN ) : SV_Target
			{
				fixed4 c = tex2D( _MainTex, IN.texcoord );

				if( c.a < _AlphaCutoff )
					discard;

				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}
