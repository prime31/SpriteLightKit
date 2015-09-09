using UnityEngine;
using System.Collections;


namespace Prime31
{
	public enum SLKColorchannels
	{
		None,
		All,
		Red,
		Green,
		Blue
	}


	public enum SLKWaveFunctions
	{
		Sin,
		Triangle,
		Square,
		SawTooth,
		IntertedSawTooth,
		Random,
		PerlinNoise
	}


	public class SpriteLightColorCycler : MonoBehaviour
	{
		public SLKColorchannels colorChannel = SLKColorchannels.All;
		public SLKWaveFunctions waveFunction = SLKWaveFunctions.Sin;
		[Tooltip( "This value is added to the final result" )]
		[Range( 0f, 1f )]
		public float offset = 0.0f;
		[Range( 0.2f, 2f )]
		public float amplitude = 1.0f;
		[Tooltip( "start point in wave function" )]
		[Range( 0f, 1f )]
		public float phase = 0.0f;
		[Tooltip( "cycles per second" )]
		[Range( 0.01f, 10f )]
		public float frequency = 0.5f;
		[Tooltip( "should the alpha be changed as well as colors" )]
		public bool affectsIntensity = true;

		// cache original values
		SpriteRenderer _spriteRenderer;
		Color originalColor;
		float originalIntensity;


		void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			originalColor = _spriteRenderer.color;
			originalIntensity = originalColor.a;
		}



		void Update()
		{
			var color = _spriteRenderer.color;

			switch( colorChannel )
			{
				case SLKColorchannels.All:
					color = originalColor * evaluateWaveFunction();
					break;
				case SLKColorchannels.Red:
					color = new Color( originalColor.r * evaluateWaveFunction(), color.g, color.b, color.a );
					break;
				case SLKColorchannels.Green:
					color = new Color( color.r, originalColor.g * evaluateWaveFunction(), color.b, color.a );
					break;
				case SLKColorchannels.Blue:
					color = new Color( color.r, color.g, originalColor.b * evaluateWaveFunction(), color.a );
					break;
			}

			if( affectsIntensity )
				color.a = originalIntensity * evaluateWaveFunction();
			else
				color.a = originalColor.a;

			_spriteRenderer.color = color;
		}


		private float evaluateWaveFunction()
		{
			var t = ( Time.time + phase ) * frequency;
			var y = 1f;

			// only normalize x if we are not using perlin noise
			if( waveFunction != SLKWaveFunctions.PerlinNoise )
				t = t - Mathf.Floor( t ); // normalized value (0..1)

			switch( waveFunction )
			{
				case SLKWaveFunctions.Sin:
					y = Mathf.Sin( 1f * t * Mathf.PI );
					break;
				case SLKWaveFunctions.Triangle:
					if( t < 0.5f )
						y = 4.0f * t - 1.0f;
					else
						y = -4.0f * t + 3.0f; 
					break;
				case SLKWaveFunctions.Square:
					if( t < 0.5f )
						y = 1.0f;
					else
						y = -1.0f; 
					break;
				case SLKWaveFunctions.SawTooth:
					y = t;
					break;
				case SLKWaveFunctions.IntertedSawTooth:
					y = 1.0f - t;
					break;
				case SLKWaveFunctions.Random:
					y = 1f - ( Random.value * 2f );
					break;
				case SLKWaveFunctions.PerlinNoise:
					y = Mathf.PerlinNoise( phase, t );
					break;
			}

			return ( y * amplitude ) + offset;
		}
	}
}