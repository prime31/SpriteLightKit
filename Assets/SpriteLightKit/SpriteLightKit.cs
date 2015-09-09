using UnityEngine;
using System.Collections;


namespace Prime31
{
	[ExecuteInEditMode]
	public class SpriteLightKit : MonoBehaviour
	{
		public enum RenderTextureDepth
		{
			None = 0,
			_16Bit = 16,
			_24Bit = 24
		}


		public enum SLKRenderTextureFormat
		{
			ARGB32 = 0,
			ARGBHalf = 2,
			RGB565 = 4,
			ARGB4444 = 5,
			ARGB1555 = 6,
			Default = 7,
			ARGB2101010 = 8,
			DefaultHDR = 9,
			ARGBFloat = 11
		}

		[Header( "Required Fields" )]
		[Tooltip( "This should be the main camera that is used to render your scene which you want the lights to appear on" )]
		public Camera mainCamera;
		[Tooltip( "All lights should be placed in this layer. It is the layer that lights camera will render and blend on top of your main camera." )]
		public LayerMask lightLayer;

		[Header( "Render Texture Setup" )]
		[SerializeField]
		SLKRenderTextureFormat rtFormat = SLKRenderTextureFormat.Default;
		[SerializeField]
		RenderTextureDepth rtTextureDepth = RenderTextureDepth.None;
		[SerializeField]
		[Tooltip( "This must be set before the component is enabled to take effect" )]
		FilterMode rtFilterMode = FilterMode.Point;
		[SerializeField]
		[Range( 0.1f, 1f )]
		[Tooltip( "This must be set before the component is enabled to take effect" )]
		float rtSizeMultiplier = 1f;

		float _previousCameraOrthoSize;
		[HideInInspector]
		[SerializeField]
		Camera _spriteLightCamera;
		Transform _quadTransform;

		RenderTexture _texture;
		// we need to keep track of these so that if they change we update the RT
		int _lastScreenWidth = -1;
		int _lastScreenHeight = -1;

		SpriteLightKitImageEffect _slkImageEffect;


		void OnEnable()
		{
			if( mainCamera == null )
				mainCamera = Camera.main;
			
			// ensure the SpriteLightKitPostProcessor is on the Camera
			_slkImageEffect = mainCamera.GetComponent<SpriteLightKitImageEffect>();
			if( _slkImageEffect == null )
				_slkImageEffect = mainCamera.gameObject.AddComponent<SpriteLightKitImageEffect>();
		
			prepareCamera();
			updateTexture();
			transform.localPosition = Vector3.zero;
		}


		void OnDisable()
		{
			if( _spriteLightCamera != null )
				_spriteLightCamera.targetTexture = null;

			if( _texture != null )
			{
				_texture.Release();
				UnityEngine.Object.DestroyImmediate( _texture );
			}

			_slkImageEffect.spriteLightRT = null;
		}


		void OnPreRender()
		{
			// if our camera orthoSize changes we need to change our orthoSize
			if( mainCamera.orthographicSize != _previousCameraOrthoSize || _lastScreenWidth != Screen.width || _lastScreenHeight != Screen.height )
			{
				_spriteLightCamera.orthographicSize = mainCamera.orthographicSize;
				_previousCameraOrthoSize = mainCamera.orthographicSize;

				updateTexture();
			}
		}


		void prepareCamera()
		{
			if( _spriteLightCamera != null )
			{
				_previousCameraOrthoSize = mainCamera.orthographicSize;
				return;
			}

			_spriteLightCamera = GetComponent<Camera>();
			if( _spriteLightCamera == null )
			{
				_spriteLightCamera = gameObject.AddComponent<Camera>();
				_spriteLightCamera.backgroundColor = new Color( 0.25f, 0.25f, 0.25f );
			}

			_spriteLightCamera.CopyFrom( mainCamera );
			mainCamera.cullingMask ^= lightLayer;
			Debug.Log( "Be sure to remove the lightLayer set on the SpriteLightKit component from your main camera's culling mask!" );

			// set our custom settings here
			_spriteLightCamera.cullingMask = lightLayer;
			_spriteLightCamera.clearFlags = CameraClearFlags.Color;
			_spriteLightCamera.useOcclusionCulling = false;
			_spriteLightCamera.targetTexture = null;

			// we need to render before the main camera
			_spriteLightCamera.depth = mainCamera.depth - 10;
		}


		void updateTexture( bool forceRefresh = true )
		{
			if( _spriteLightCamera == null )
				return;

			if( forceRefresh || _texture == null )
			{
				if( _texture != null )
				{
					_spriteLightCamera.targetTexture = null;
					_texture.Release();
					UnityEngine.Object.DestroyImmediate( _texture );
				}

				// keep track of these so we know when the resolution changes
				_lastScreenWidth = Screen.width;
				_lastScreenHeight = Screen.height;

				// setup the width/height based on our multiplier
				var rtWidth = Mathf.RoundToInt( _spriteLightCamera.pixelWidth * rtSizeMultiplier );
				var rtHeight = Mathf.RoundToInt( _spriteLightCamera.pixelHeight * rtSizeMultiplier );

				if( rtWidth == 0 || rtHeight == 0 )
				{
					Debug.LogWarning( "RT height or width rounded to 0. Defaulting to the camera pixelWidth" );
					rtWidth = _spriteLightCamera.pixelWidth;
					rtHeight = _spriteLightCamera.pixelWidth;
				}

				// convert then check format
				var format = (RenderTextureFormat)( (int)rtFormat );
				if( !SystemInfo.SupportsRenderTextureFormat( format ) )
				{
					Debug.LogWarning( "Invalid texture format for this system. Defaulting to RenderTextureFormat.Default" );
					format = RenderTextureFormat.Default;
				}

				_texture = new RenderTexture( rtWidth, rtHeight, (int)rtTextureDepth, format );
				_texture.name = "SpriteLightKit RT";
				_texture.Create();
				_texture.filterMode = rtFilterMode;
				_spriteLightCamera.targetTexture = _texture;

				// set the RenderTexture on our image effect
				_slkImageEffect.spriteLightRT = _texture;
			}
		}

	}
}