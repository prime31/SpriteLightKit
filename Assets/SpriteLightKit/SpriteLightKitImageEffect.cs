using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class SpriteLightKitImageEffect : MonoBehaviour
{
	public Shader shader;
	public RenderTexture spriteLightRT;
	protected Material _material;


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


	protected void Start()
	{
		if( !SystemInfo.supportsImageEffects )
		{
			enabled = false;
			return;
		}

		// Disable the image effect if the shader can't run on the users graphics card
		if( !shader || !shader.isSupported )
			enabled = false;
	}


	public void OnDisable()
	{
		if( _material )
			DestroyImmediate( _material );
	}


	void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		material.SetTexture( "_LightsTex", spriteLightRT );
		Graphics.Blit( source, destination, material );
	}
}
