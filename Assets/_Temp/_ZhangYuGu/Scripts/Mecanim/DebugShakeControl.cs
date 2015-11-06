using UnityEngine;
using System.Collections;

public class DebugShakeControl : MonoBehaviour {

	public DebugScreenShake m_shake;

	public UILabel m_lb_delay;

	public UILabel m_lb_duration;

	public UILabel m_lb_offset;


	public float m_delay_unit = 0.25f;

	public float m_duration_unit = 0.05f;

	public Vector3 m_offset_unit = new Vector3( 0.1f, 0, 0 );

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnDelayAdd(){
		m_shake.m_delay += m_delay_unit;

		m_lb_delay.text = m_shake.m_delay.ToString( "0.00" );
	}

	public void OnDelayDec(){
		m_shake.m_delay -= m_delay_unit;

		if( m_shake.m_delay < 0 ){
			m_shake.m_delay = 0.0f;
		}

		m_lb_delay.text = m_shake.m_delay.ToString( "0.00" );
	}

	public void OnDurationAdd(){
		m_shake.m_duration += m_duration_unit;

		m_lb_duration.text = m_shake.m_duration.ToString( "0.00" );
	}
	
	public void OnDeurationDec(){
		m_shake.m_duration -= m_duration_unit;

		if( m_shake.m_duration < 0 ){
			m_shake.m_duration = m_duration_unit;
		}

		m_lb_duration.text = m_shake.m_duration.ToString( "0.00" );
	}

	public void OnOffsetAdd(){
		m_shake.m_max_offset += m_offset_unit;

		m_lb_offset.text = m_shake.m_max_offset.x.ToString( "0.00" );
	}
	
	public void OnOffsetDec(){
		m_shake.m_max_offset -= m_offset_unit;

		if( m_shake.m_max_offset.x <= 0 ||
				m_shake.m_max_offset.y <= 0 ){
			m_shake.m_max_offset = Vector3.zero;
		}

		m_lb_offset.text = m_shake.m_max_offset.x.ToString( "0.00" );
	}


}
