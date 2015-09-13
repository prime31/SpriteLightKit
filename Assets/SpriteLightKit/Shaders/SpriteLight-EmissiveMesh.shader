Shader "prime[31]/Sprite Light Kit/Emissive Mesh"
{
	Subshader
	{
		Pass
		{
			// dont write anything but the stencil buffer
			ZWrite Off
			ColorMask 0


			Stencil
			{
			    Ref 2
			    Comp Always
			    Pass Replace
			}
		}
	}
}
