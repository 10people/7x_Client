using UnityEngine;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FxDelayer : MonoBehaviour {
	
	public float m_total_delay = 0.0f;
	
	
	private float m_pre_delay = 0.0f;
	
	#region Mono
	
	// Use this for initialization
	void Start () {
		m_pre_delay = m_total_delay;
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR 
		//check if we need to update
		if ( m_pre_delay != m_total_delay && m_total_delay >= 0 )
		{
			float t_delta = m_total_delay - m_pre_delay;
			
			//scale shuriken particle systems
			DelayShurikenSystems( t_delta );
			
			m_pre_delay = m_total_delay;
		}
		#endif
	}
	
	#endregion
	
	void DelayShurikenSystems( float p_delay )
	{
		#if UNITY_EDITOR 
		//get all shuriken systems we need to do scaling on
		ParticleSystem[] t_systems = GetComponentsInChildren<ParticleSystem>();

		for( int i = 0; i < t_systems.Length; i++ ){
			ParticleSystem t_system = t_systems[ i ];

			t_system.startDelay += p_delay;
		}

		#endif
	}

}
