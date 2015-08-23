using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class SpriteLightKit : MonoBehaviour
{
	[Tooltip( "This should be the main camera that is used to render your scene which you want the lights to appear on" )]
	public Camera mainCamera;
	[Tooltip( "All lights should be placed in this layer. It is the layer that lights camera will render and blend on top of your main camera." )]
	public LayerMask lightLayer;

	float _previousCameraOrthoSize;
	[HideInInspector]
	[SerializeField]
	Camera _spriteLightCamera;
	Transform _quadTransform;

	RenderTexture _texture;
	// we need to keep track of these so that if they change we update the RT
	int _lastScreenWidth = -1;
	int _lastScreenHeight = -1;


	void OnEnable()
	{
		if( mainCamera == null )
			mainCamera = Camera.main;
			
		prepareCamera();
		updateTexture();
		transform.localPosition = Vector3.zero;
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
			return;

		_spriteLightCamera = GetComponent<Camera>();
		if( _spriteLightCamera == null )
		{
			_spriteLightCamera = gameObject.AddComponent<Camera>();
			_spriteLightCamera.backgroundColor = new Color( 0.25f, 0.25f, 0.25f );
		}

		_spriteLightCamera.CopyFrom( mainCamera );
		mainCamera.cullingMask ^= lightLayer;
		Debug.Log( "Be sure to remove the lightLayer set on the SpriteLightKit component from your main camera's culling mask!" );
		_previousCameraOrthoSize = mainCamera.orthographicSize;

		// set our custom settings here
		_spriteLightCamera.cullingMask = lightLayer;
		_spriteLightCamera.clearFlags = CameraClearFlags.Color;
		_spriteLightCamera.useOcclusionCulling = false;
		_spriteLightCamera.targetTexture = null;

		// we need to render first so we can render to the quad
		_spriteLightCamera.depth = mainCamera.depth - 10;
	}


	private void updateTexture( bool forceRefresh = true )
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
				
			_texture = new RenderTexture( _spriteLightCamera.pixelWidth, _spriteLightCamera.pixelHeight, 24, RenderTextureFormat.Default );
			_texture.name = "SpriteLightKit RT";
			_texture.Create();
			_texture.filterMode = FilterMode.Point;
			_spriteLightCamera.targetTexture = _texture;

			// ensure the SpriteLightKitPostProcessor is on the Camera
			var postProcessor = mainCamera.GetComponent<SpriteLightKitImageEffect>();
			if( postProcessor == null )
				postProcessor = mainCamera.gameObject.AddComponent<SpriteLightKitImageEffect>();

			postProcessor.spriteLightRT = _texture;
		}
	}

}
