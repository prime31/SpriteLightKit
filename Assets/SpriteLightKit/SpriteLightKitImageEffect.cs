using UnityEngine;
using System.Collections;


namespace Prime31
{
	[ExecuteInEditMode]
	public class SpriteLightKitImageEffect : MonoBehaviour
	{
		public Shader shader;
		public RenderTexture spriteLightRT;
		[Range(0.0f, 2.0f)] public float intensity = 1.0f;
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
			{
				DestroyImmediate( _material );
				_material = null;
			}
		}


		void OnRenderImage( RenderTexture source, RenderTexture destination )
		{
			// if SpriteLightKit is disabled this RT will no longer be valid
			if( spriteLightRT == null )
				return;
		
			material.SetTexture( "_LightsTex", spriteLightRT );
			material.SetFloat( "_MultiplicativeFactor", intensity );
			Graphics.Blit( source, destination, material );
		}
	}
}