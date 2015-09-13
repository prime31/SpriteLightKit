using UnityEngine;
using System.Collections;


namespace Prime31
{
	public class SpriteLightKitShadow : MonoBehaviour
	{
		[Tooltip( "Maximum distance that a light can be from the sprite to still affect it" )]
		public float maxLightSqrDistance = 100f;
		[Tooltip( "The maximum offset from the sprite that the shadow can be" )]
		public float maxShadowTranslation = 0.5f;
		[Tooltip( "The averaged light distance is multiplied by this and it affects how far the shadow will offset from the sprite" )]
		public float shadowDistanceMultiplier = 1f;

		SpriteLightKitLightManager _slkLightManager;
		Transform _transform;
		Material _material;


		void Awake()
		{
			_slkLightManager = FindObjectOfType<SpriteLightKitLightManager>();
			_transform = gameObject.transform;
			_material = GetComponent<SpriteRenderer>().material;
		}


		void Update()
		{
			// we dont want the z component to influence anything
			var position = _transform.position;
			position.z = 0f;

			var nearestLightPosition = _slkLightManager.getAffectedAverageLightPos( position, maxLightSqrDistance );

			//Debug.DrawLine( position, nearestLightPosition, Color.red, 0.1f );

			var lightDistance = _slkLightManager.sqrDistanceBetweenVectors( position, nearestLightPosition );
			var lightDir = ( position - nearestLightPosition ).normalized * lightDistance * shadowDistanceMultiplier;
			lightDir /= maxLightSqrDistance;

			_material.SetFloat( "_HorizontalTranslation", Mathf.Clamp( lightDir.x, -maxShadowTranslation, maxShadowTranslation ) );
			_material.SetFloat( "_VerticalTranslation", Mathf.Clamp( lightDir.y, -maxShadowTranslation, maxShadowTranslation ) );
		}
	}
}