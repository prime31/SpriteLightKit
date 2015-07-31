using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class SpriteLightKit : MonoBehaviour
{
	[Tooltip( "This should be the main camera that is used to render your scene which you want the lights to appear on" )]
	public Camera mainCamera;
	[Tooltip( "This material will be used to blend the rendered lights with the standard scene" )]
	public Material blendMaterial;
	[Tooltip( "All lights should be placed in this layer. It is the layer that lights camera will render and blend on top of your main camera." )]
	public LayerMask lightLayer;

	float _previousCameraOrthoSize;
	[HideInInspector]
	[SerializeField]
	Camera _camera;
	Transform _quadTransform;


	void OnEnable()
	{
		if( mainCamera == null )
			mainCamera = Camera.main;
		
		if( blendMaterial == null )
		{
			enabled = false;
			Debug.LogWarning( "Disabling SpriteLightKit due to a null material. Set the material in the inspector and reenable the script." );
			return;
		}

		prepareQuad();
		prepareCamera();
		transform.localPosition = Vector3.zero;
	}


	void OnPreRender()
	{
		// if our camera orthoSize changes we need to change our orthoSize and the quads scale
		if( mainCamera.orthographicSize != _previousCameraOrthoSize )
		{
			_camera.orthographicSize = mainCamera.orthographicSize;
			_quadTransform.localScale = new Vector3( (float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight, 1f, 1f ) * mainCamera.orthographicSize * 2f;
			_previousCameraOrthoSize = mainCamera.orthographicSize;
		}
	}


	void OnRenderImage( RenderTexture source, RenderTexture destination )
	{
		blendMaterial.mainTexture = source;
	}


	void prepareQuad()
	{
		if( _quadTransform == null )
		{
			var componentsInChildren = transform.GetComponentsInChildren<Transform>();
			for( var i = 0; i < componentsInChildren.Length; i++ )
			{
				var childTransform = componentsInChildren[i];
				if( childTransform != transform )
					UnityEngine.Object.DestroyImmediate( childTransform.gameObject );
			}

			var go = GameObject.CreatePrimitive( PrimitiveType.Quad );
			go.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
			_quadTransform = go.transform;
			UnityEngine.Object.DestroyImmediate( _quadTransform.GetComponent<Collider>() );
		}

		var aspect = (float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight;
		if( float.IsNaN( aspect ) )
		{
			Debug.LogWarning( "main camera pixelWidth is 0. This can happen when Unity launches or loads a new scene. Nothing much to worry about." );
			aspect = 1.8f;
		}
		_quadTransform.parent = transform;
		_quadTransform.localPosition = Vector3.forward;
		_quadTransform.localScale = new Vector3( (float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight, 1f, 1f ) * mainCamera.orthographicSize * 2f;
		_quadTransform.GetComponent<MeshRenderer>().sharedMaterial = blendMaterial;
	}


	void prepareCamera()
	{
		_camera = GetComponent<Camera>();
		if( _camera == null )
		{
			_camera = gameObject.AddComponent<Camera>();
			_camera.backgroundColor = new Color( 0.25f, 0.25f, 0.25f );
		}

		_camera.CopyFrom( mainCamera );
		mainCamera.cullingMask ^= lightLayer;
		_previousCameraOrthoSize = mainCamera.orthographicSize;

		// set our custom settings here
		_camera.cullingMask = lightLayer;
		_camera.clearFlags = CameraClearFlags.Color;
		_camera.useOcclusionCulling = false;
		_camera.targetTexture = null;

		// we need to render first so we can render to the quad
		_camera.depth = mainCamera.depth - 10;
	}

}
