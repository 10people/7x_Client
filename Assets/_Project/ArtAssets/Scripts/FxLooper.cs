using UnityEngine;
using System.Collections;

public class FxLooper : MonoBehaviour {

	public float m_time_scale = 1.0f;

	public float m_interval = 0.5f;

	private float m_life_time;
	
	protected bool m_auto_destroy = false;

	#region Mono

	void Start(){
		Time.timeScale = m_time_scale;

		CalcLifeTime();

		if( !m_auto_destroy ){
			StartCoroutine( LoopFx() );
		}
		else{
			StartCoroutine( AutoDestroy() );
		}
	}

	void Update(){
		if( Time.timeScale != m_time_scale ){
			Time.timeScale = m_time_scale;
		}
	}

	#endregion



	#region Looper

	IEnumerator AutoDestroy(){
		yield return new WaitForSeconds( m_life_time );

		Destroy( gameObject );
	}

	IEnumerator LoopFx(){
		yield return new WaitForSeconds( m_life_time );

		int t_count = 0;

		while( m_life_time > 0 ){
			GameObject t_fx = (GameObject)Instantiate( gameObject,transform.position,transform.rotation );

			{
				t_fx.name = gameObject.name + " - " + ( ++t_count );
				
				FxLooper t_looper = t_fx.GetComponent<FxLooper>();

				t_looper.m_auto_destroy = true;
			}

			yield return new WaitForSeconds( m_life_time );
		}
	}

	#endregion



	#region Utilities
	
	void CalcLifeTime(){
		CalcLifeTimeShuriken();

		{
			m_life_time += m_interval;
		}
	}

	
	void CalcLifeTimeShuriken()
	{
		#if UNITY_EDITOR 
		//get all shuriken systems we need to do scaling on
		ParticleSystem[] t_systems = GetComponentsInChildren<ParticleSystem>();
		
		for( int i = 0; i < t_systems.Length; i++ ){
			ParticleSystem t_system = t_systems[ i ];

			if( t_system.loop ){
				continue;
			}

			if( m_life_time < t_system.time ){
				m_life_time = t_system.time;
			}
			
			if( m_life_time < t_system.duration ){
				m_life_time = t_system.duration;
			}
			
		}
		#endif
	}

	#endregion
}