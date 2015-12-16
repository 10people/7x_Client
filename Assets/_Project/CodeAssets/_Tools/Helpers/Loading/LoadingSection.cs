using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingSection{
	public string m_section_name;

	private float m_weight = 1.0f;

	private float m_percentage = 0.0f;
	
	private int m_items_loaded = 0;
	
	// -1 means unknown.
	private int m_item_count = -1;
	
	public LoadingSection( string p_section_name, float p_weight = 1.0f, int p_item_count = -1 ){
		m_section_name = p_section_name;

		m_weight = p_weight;

		SetTotalCount( p_item_count );
	}

	public void UpdatePercentage( float p_percentage ){
		m_percentage = p_percentage;
	}

	public void ItemLoaded(){
		m_items_loaded++;
	}

	public float GetLoadedWeight(){
		float t_local_weight = 0;

		if( m_item_count < 0 ){
			t_local_weight = m_percentage * m_weight;
		}
		else if( m_item_count > 0 ){
			t_local_weight = m_items_loaded * 1.0f / m_item_count * m_weight;
		}
		else{
//			Debug.LogError( "Error In GetLoadedWeight." );
//
//			Debug.Log( "Loading Section: " + 
//					  m_items_loaded + " / " + m_item_count + " - " + 
//					  m_weight + "   - " +
//					  "   " + m_section_name );

			t_local_weight = 0.0f;
		}

		t_local_weight = Mathf.Clamp( t_local_weight, 0, m_weight );

		return t_local_weight;
	}

	public float GetTotalWeight(){
		return m_weight;
	}

	// reset total resources item count
	public void SetTotalCount( int p_item_count ){
//			Debug.Log( m_section_name + ".SetTotalCount( " + p_item_count + " )" );
	
		m_item_count = p_item_count;
	}

	public void Log(){
		Debug.Log( "Loading Section( items.percent: " + 
				  m_items_loaded + " / " + m_item_count + 
				  " -    weight: " + m_weight + "   -    weight.percent: " +
				  GetLoadedWeight() + " / " + GetTotalWeight() +
				  "   " + m_section_name );
	}
}