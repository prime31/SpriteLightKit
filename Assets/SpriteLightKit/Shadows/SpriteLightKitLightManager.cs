using UnityEngine;
using System.Collections.Generic;


namespace Prime31
{
	public class SpriteLightKitLightManager : MonoBehaviour
	{
		List<Vector3> _spriteLightPositions = new List<Vector3>();


		void Awake()
		{
			var slk = FindObjectOfType<SpriteLightKit>();

			var allGOs = FindObjectsOfType<GameObject>();
			for( var i = 0; i < allGOs.Length; i++ )
			{
				if( ( slk.lightLayer.value & 1 << allGOs[i].layer ) != 0 )
				{
					var pos = allGOs[i].transform.position;
					pos.z = 0f;
					_spriteLightPositions.Add( pos );
				}
			}
		}


		/// <summary>
		/// returns the weighted average position of any lights within range or the original position passed in if everything
		/// is further than maxDistance
		/// </summary>
		/// <returns>The nearest light.</returns>
		/// <param name="position">Position.</param>
		/// <param name="maxDistance">Max distance.</param>
		public Vector3 getAffectedAverageLightPos( Vector3 position, float maxSqrDistance )
		{
			position.z = 0;

			// we want the weighted average position of any lights that are close enough
			var totalWeight = 0f;
			var accumulatedPosition = Vector3.zero;
			for( var i = 0; i < _spriteLightPositions.Count; i++ )
			{
				var sqrDistance = sqrDistanceBetweenVectors( position, _spriteLightPositions[i] );
				if( sqrDistance < maxSqrDistance )
				{
					// weight should be greater for closer lights and less for further away
					var weight = maxSqrDistance - sqrDistance;

					// tally the total weight
					totalWeight += weight;
					accumulatedPosition += ( weight * _spriteLightPositions[i] );
				}
			}
				
			// if we have a totalWeight we need to take into account
			if( totalWeight > 0 )
			{
				return accumulatedPosition * ( 1f / totalWeight );
			}

			return position;
		}


		public float sqrDistanceBetweenVectors( Vector3 a, Vector3 b )
		{
			return new Vector2( a.x - b.x, a.y - b.y ).sqrMagnitude;
		}
	}
}