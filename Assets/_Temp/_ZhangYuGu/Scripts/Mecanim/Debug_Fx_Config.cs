using UnityEngine;
using System.Collections;

public class Debug_Fx_Config : MonoBehaviour {

	public string m_fx_name;

	public GameObject m_fx_prefab;

	public float m_delay;

	public GameObject m_fx_pos;

	public Vector3 m_fx_max_random_offset;


	public float m_delayed_destroy = 4.0f;

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Fx

	public void PlayFx(){
		StartCoroutine( DelayedFx() );
	}

	IEnumerator DelayedFx(){
		yield return new WaitForSeconds( m_delay );

		GameObject t_fx = (GameObject)Instantiate( m_fx_prefab );
		
		t_fx.transform.parent = transform;
		
		GameObject t_tran_target = gameObject;
		
		if( m_fx_pos != null ){
			t_tran_target = m_fx_pos;
		}
		
		Vector3 t_random = new Vector3(
			UtilityTool.GetRandom( -m_fx_max_random_offset.x, m_fx_max_random_offset.x ),
			UtilityTool.GetRandom( -m_fx_max_random_offset.y, m_fx_max_random_offset.y ),
			UtilityTool.GetRandom( -m_fx_max_random_offset.z, m_fx_max_random_offset.z ) );
		
		t_fx.transform.position = t_tran_target.transform.position + t_random;
		
		t_fx.transform.rotation = t_tran_target.transform.rotation;

		Destroy( t_fx, m_delayed_destroy );

		yield return new WaitForSeconds( 0 );
	}

	#endregion
}
