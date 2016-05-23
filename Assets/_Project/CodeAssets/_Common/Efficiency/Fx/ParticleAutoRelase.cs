//#define DEBUG_AUTO_RELEASE



using UnityEngine;
using System.Collections;



public class ParticleAutoRelease : MonoBehaviour {

	public float m_time_left = 0.0f;

	private ParticleSystem[] m_pss = null;

	#region Config Const

	// default max particle duration is 5s
	private const float PARTILCE_AUTO_RELEASE_TIME	= 5.0f;

	#endregion



	#region Mono

	void Awake(){

	}

	void Start(){
		{
			this.enabled = IsAutoReleaseEnabled( gameObject );

			m_pss = gameObject.GetComponentsInChildren<ParticleSystem>();
		}

		#if DEBUG_AUTO_RELEASE
		GameObjectHelper.LogGameObjectHierarchy( gameObject, "ParticleAutoRelease.Start() " + this.enabled );
		#endif

		m_time_left = PARTILCE_AUTO_RELEASE_TIME;
	}

	void Update(){
		m_time_left = m_time_left - Time.deltaTime;

		if( m_time_left <= 0 ){
			if( ShouldRelease() ){
				#if DEBUG_AUTO_RELEASE
				GameObjectHelper.LogGameObjectHierarchy( gameObject, "Release Fx: " );
				#endif

				GameObjectHelper.HideAndDestroy( gameObject );
			}
		}
	}

	#endregion



	#region Utilities

	// is fx should be auto released?
	public static bool IsAutoReleaseEnabled( GameObject p_gb ){
		if( p_gb == null ){
			Debug.LogError( "GameObject is null." );

			return true;
		}

		// ps
		{
			// self
			{
				ParticleSystem t_ps = p_gb.GetComponent<ParticleSystem>();

				if( t_ps != null ){
					if( t_ps.loop ){
						return false;
					}
				}
			}

			// child
			{
				ParticleSystem[] t_pss = p_gb.GetComponentsInChildren<ParticleSystem>();
				
				#if DEBUG_AUTO_RELEASE
				Debug.Log( "IsAutoReleaseEnabled.PS.Count: " + t_pss.Length );
				#endif
				
				if( t_pss.Length == 0 ){
					#if DEBUG_AUTO_RELEASE
					GameObjectHelper.LogGameObjectHierarchy( p_gb, "Fx not made of Particle: " );
					#endif
					
					return false;
				}
				
				for( int i = 0; i < t_pss.Length; i++ ){
					if( t_pss[ i ].loop ){
						ParticleSystemRenderer t_ps_renderer = t_pss[ i ].gameObject.GetComponent<ParticleSystemRenderer>();
						
						if( t_ps_renderer.enabled ){
							return false;
						}
					}
				}
			}
		}

//		// mesh
//		{
//			MeshRenderer t_mesh = p_gb.GetComponentInChildren<MeshRenderer>();
//
//			if( t_mesh != null ){
//				return false;
//			}
//		}

		return true;
	}

	// should fx be released now?
	bool ShouldRelease(){
		if( m_pss == null ){
			#if DEBUG_AUTO_RELEASE
			Debug.LogError( "Error, Should Not Be Here." );

			GameObjectHelper.LogGameObjectHierarchy( gameObject, "Error In ShouldRelease()" ); 
			#endif

			return false;
		}

		for( int i = 0; i < m_pss.Length; i++ ){
			if( m_pss[ i ].particleCount > 0 ){
				return false;
			}
		}

		return true;
	}

	#endregion
}
