//#define DEBUG_MATERIAL



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class MaterialHelper : MonoBehaviour {

	#region Materials

	public static Material[] CloneMaterials( Renderer p_renderer ){
		if( p_renderer == null ){
			return null;
		}

		Material[] t_mats = p_renderer.materials;

		return t_mats;
	}

	#endregion



	#region Renderers

	public static void UpdateColor( List<Renderer> p_renderers, string p_property_name, Color p_color ){
		if( p_renderers == null ){
			return;
		}

		if( string.IsNullOrEmpty( p_property_name ) ){
			return;
		}

		for( int i = 0; i < p_renderers.Count; i++ ){
			Renderer t_renderer = p_renderers[ i ];

			if( t_renderer == null ){
				continue;
			}

			if( t_renderer.material.HasProperty( p_property_name ) ){
				t_renderer.material.SetColor( p_property_name, p_color );
			}
			else{
				#if UNITY_EDITOR && DEBUG_MATERIAL
				Debug.LogError( "Error, no property exist: " + p_property_name + " - " + t_renderer.material );
				#endif
			}
		}
	}

	public static void UpdateFloat( List<Renderer> p_renderers, string p_property_name, float p_float ){
		if( p_renderers == null ){
			return;
		}

		if( string.IsNullOrEmpty( p_property_name ) ){
			return;
		}

		for( int i = 0; i < p_renderers.Count; i++ ){
			Renderer t_renderer = p_renderers[ i ];

			if( t_renderer == null ){
				continue;
			}

			if( t_renderer.material.HasProperty( p_property_name ) ){
				t_renderer.material.SetFloat( p_property_name, p_float );
			}
			else{
				#if UNITY_EDITOR && DEBUG_MATERIAL
				Debug.LogError( "Error, no property exist: " + p_property_name + " - " + t_renderer.material );
				#endif
			}
		}
	}

	#endregion
}