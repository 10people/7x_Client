using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class FanRange : MonoBehaviour {

	[RangeAttribute( 0, 359 )]
	public int m_angle = 180;

	public float m_scale = 1.0f;

	public Material m_mat;

	#region Mono

	void Awake(){
		m_mat = GetComponent<Renderer>().material;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR
		SetAngle( m_angle );

		SetScale( m_scale );
#endif
	}

	void OnDestroy() {
		Destroy( GetComponent<Renderer>().material );
	}

	#endregion



	#region FanRange

	public void SetAngle( int p_angle ){
//		Debug.Log( "SetAngle: " + p_angle );

		while( p_angle < 0 ){
			p_angle += 360;
		}

		while( p_angle > 360 ){
			p_angle -= 360;
		}

		if( p_angle == 360 ){
			p_angle = 359;
		}

		m_angle = p_angle;

		if( m_mat != null ){
			m_mat.SetFloat( "_Angle", m_angle );
		}
		else{
			Debug.LogError( "Error, Mat = null." );
		}
	}

	public void SetScale( float p_scale ){
//		Debug.Log( "SetScale: " + p_scale );

		if( p_scale <= 0 ){
			return;
		}

		m_scale = p_scale;

		gameObject.transform.localScale = new Vector3( p_scale, 1, p_scale );

		if( m_mat != null ){
			m_mat.SetFloat( "_Factor", m_scale );
		}
		else{
			Debug.LogError( "Error, Mat = null." );
		}
	}

	#endregion
}
