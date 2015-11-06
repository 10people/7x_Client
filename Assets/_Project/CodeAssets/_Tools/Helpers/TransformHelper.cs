#define DEBUG_TRANSFORM_HELPER

using UnityEngine;
using System.Collections;

public class TransformHelper : MonoBehaviour {

	#region Rotation

	public static void SetLocalRotation( GameObject p_gb, Vector3 p_local_rot ){
		if ( p_gb == null ) {
			Debug.Log( "p_gb = null." );

			return;
		}

		p_gb.transform.localRotation = Quaternion.Euler( p_local_rot );
	}

	#endregion



	#region Save&Load

	private static Quaternion m_quaternion;
	
	private static Vector3 m_position;
	
	private static Vector3 m_local_scale;
	
	
	
	public static void StoreTransform(GameObject p_gb)
	{
		m_position = p_gb.transform.position;
		
		m_local_scale = p_gb.transform.localScale;
		
		m_quaternion = p_gb.transform.rotation;
	}
	
	public static void RestoreTransform(GameObject p_gb)
	{
		p_gb.transform.position = m_position;
		
		p_gb.transform.localScale = m_local_scale;
		
		p_gb.transform.rotation = m_quaternion;
	}

	#endregion
}
