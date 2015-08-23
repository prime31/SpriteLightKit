using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class SpriteLightKitImageEffect : MonoBehaviour
{
	public Shader shader;
	public RenderTexture spriteLightRT;
	public bool use2xMultiplicationBlending = false;
	Material _material;


	protected Material material
	{
		get
		{
			if( _material == null )
			{
				_material = new Material( shader );
				_material.hideFlags = HideFlags.HideAndDontSave;
			}

			return _material;
		} 
	}


	public void OnDisable()
	{
		if( _material )
			DestroyImmediate( _material );
	}


	void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		material.SetTexture( "_LightsTex", spriteLightRT );
		material.SetFloat( "_MultiplicativeFactor", use2xMultiplicationBlending ? 2f : 1f );
		Graphics.Blit( source, destination, material );
	}
}
