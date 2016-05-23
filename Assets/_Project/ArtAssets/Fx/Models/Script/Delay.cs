using UnityEngine;
using System.Collections;

public class Delay : MonoBehaviour {
	
	public float delayTime = 1.0f;
	
	// Use this for initialization
	void Start () {		
		
	}

	private bool m_in_life = false;

	void OnEnable(){
		if( !m_in_life ){
			m_in_life = true;

			ParticleSystem[] t_pss = GetComponentsInChildren<ParticleSystem>();

			for( int i = 0; i < t_pss.Length; i++ ){
				t_pss[ i ].Play();
			}

			gameObject.SetActiveRecursively( false );

			Invoke("DelayFunc", delayTime);	
		}
	}
	
	void DelayFunc(){
		gameObject.SetActiveRecursively(true);

		m_in_life = false;
	}
	
}
