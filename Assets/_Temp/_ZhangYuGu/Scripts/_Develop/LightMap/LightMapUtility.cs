//#define DEBUG_LIGHTMAP

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LightMapUtility : MonoBehaviour {

	private GameObject m_target_light_root = null;

	public List<float> m_light_params_keys = new List<float>();
	
	public List<float> m_light_params_values = new List<float>();

	public Dictionary<float, float> m_light_params_dict;

	public void ComposeDict(){
		#if DEBUG_LIGHTMAP
		Debug.Log( "ComposeDict()" );
		#endif

		if( m_light_params_dict == null ){
			m_light_params_dict = new Dictionary<float, float>();
		}
		else{
			Debug.LogError( "Already Composed." );
		}

		#if DEBUG_LIGHTMAP
		Debug.Log( "m_light_params_keys: " + m_light_params_keys.Count );

		Debug.Log( "m_light_params_values: " + m_light_params_values.Count );
		#endif

		for( int i = 0; i < m_light_params_keys.Count; i++ ){
			m_light_params_dict.Add( m_light_params_keys[ i ],  m_light_params_values[ i ] );
		}
	}

	public void GenerateList(){
		if( m_light_params_dict == null ){
			Debug.LogError( "Dict Not Exist." );

			return;
		}

		#if DEBUG_LIGHTMAP
		Debug.Log( "GenerateList()" );
		#endif

		m_light_params_keys = m_light_params_dict.Keys.ToList();

		m_light_params_values = m_light_params_dict.Values.ToList();
	}


	public void OnProcess(){
		#if DEBUG_LIGHTMAP
		Debug.Log( "OnProcess()" );
		#endif

		GameObject t_gb = (GameObject)Instantiate( gameObject );

		gameObject.SetActive( false );

		t_gb.SetActive( true );

		Light[] t_lights = t_gb.GetComponentsInChildren<Light>();

		for( int i = 0; i < t_lights.Length; i++ ){
			Light t_light = t_lights[ i ];

			if( t_light.type == LightType.Point ){
				float t_new_int = GetProcessedSpotIntensity( t_light.intensity );

				if( t_new_int <= 0 ){
					Debug.Log( "Error, Light Intensity not covered ---> " + 
					          t_light.gameObject + 
					          " : " + t_light.intensity );

					t_gb.SetActive( false );

					Destroy( t_gb );

					break;
				}

				Debug.Log( "Change: " + t_light.intensity + " to " + t_new_int );

				t_light.intensity = t_new_int;
			}
		}
	}

	private float GetProcessedSpotIntensity( float p_origin_key ){
		if( m_light_params_dict.Count <= 1 ){
			return 0.0f;
		}

		List<float> t_keys = m_light_params_dict.Keys.ToList();

		t_keys.Sort();

		int t_min_index = 0;

		do{
			float t_key_min = t_keys[ t_min_index ];
			
			float t_key_max = t_keys[ t_min_index + 1 ];

			if( p_origin_key >= t_key_min && p_origin_key <= t_key_max ){
				float t_new_value = GetValue( p_origin_key, t_key_min, t_key_max );

				return t_new_value;
			}

			t_min_index++;

			if( t_min_index == t_keys.Count - 1 ){
				return 0.0f;
			}
		}
		while( true );

		return 0.0f;
	}

	private float GetValue( float p_origin_key, float p_min_key, float p_max_key ){
		float t_return = 0.0f;

		#if DEBUG_LIGHTMAP
		Debug.Log( "GetValue: " + p_origin_key + ", " + p_min_key + ", " + p_max_key );
		#endif

		t_return = m_light_params_dict[ p_min_key ] + 
			( p_origin_key - p_min_key ) / ( p_max_key - p_min_key ) * ( m_light_params_dict[ p_max_key ] - m_light_params_dict[ p_min_key ] );

		return t_return;
	}
}
