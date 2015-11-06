using UnityEngine;
using System.Collections;

public class SingletonClass<T> {
	
	private static T m_instance;
	
	#region Singleton
	
	public static T Instance(){
		if ( !HaveInstance() ) {
			Debug.LogError( typeof( T ) + " is null." );
		}
		
		return m_instance;
	}
	
	public static bool HaveInstance(){
		return ( m_instance != null );
	}
	
	public static void SetInstance( T p_instance ){
		m_instance = p_instance;
	}
	
	#endregion
}
