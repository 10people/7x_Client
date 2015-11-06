using UnityEngine;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FxTool : MonoBehaviour {

	private ParticleSystem[] m_systems;

	#region Mono

	// Use this for initialization
	void Start () {
//		Debug.Log( m_pre_delay + " - " + m_total_delay );

		#if UNITY_EDITOR 
		m_pre_delay = m_total_delay;
		#endif

		{
			m_systems = GetComponentsInChildren<ParticleSystem>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR 
		UpdateDelay();
		#endif

		{
			UpdateRotation();
		}
	}

	#endregion



	#region Delay for Editor

	#if UNITY_EDITOR

	public float m_total_delay = 0.0f;
	
	private float m_pre_delay = 0.0f;

	private void UpdateDelay( ){
		float t_delta = m_total_delay - m_pre_delay;
		
		//scale shuriken particle systems
		DelayShurikenSystems( t_delta );
		
		m_pre_delay = m_total_delay;
	}

	void DelayShurikenSystems( float p_delay ){
		//get all shuriken systems we need to do scaling on

		for( int i = 0; i < m_systems.Length; i++ ){
			ParticleSystem t_system = m_systems[ i ];

			t_system.startDelay += p_delay;
		}
	}

	#endif

	#endregion



	#region Apply Rotation

	public bool m_apply_rotation = false;

	private void UpdateRotation(){
		if( !m_apply_rotation ){
			return;
		}

		float t_new_rotation = transform.eulerAngles.y;

		for( int i = 0; i < m_systems.Length; i++ ){
			ParticleSystem t_system = m_systems[ i ];

			t_system.startRotation = Mathf.Deg2Rad * t_new_rotation;
			
//			t_system.Clear();
		}
	}

	#endregion
}