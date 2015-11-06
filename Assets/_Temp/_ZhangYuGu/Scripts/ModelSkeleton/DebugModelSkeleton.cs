using UnityEngine;
using System.Collections;

public class DebugModelSkeleton : MonoBehaviour {


	public GameObject m_model_root;

	public GameObject m_model_prefab;

	public GameObject m_model_target;

	public float m_model_offset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddModel(){
		Debug.Log( "AddModel()" );

		GameObject t_model = (GameObject)Instantiate( m_model_prefab );

		t_model.transform.parent = m_model_root.transform;

		t_model.transform.position = m_model_target.transform.position + 
			new Vector3( ( Random.value - 0.5f ) * 2 * m_model_offset,
			            0,
			            ( Random.value - 0.5f ) * 2 * m_model_offset );
	}

	public void MinusModel(){
		Debug.Log( "MinusModel()" );

		if( m_model_root.transform.childCount <= 0 ){
			return;
		}

		Transform t_child_tran = m_model_root.transform.GetChild( 0 );

		Destroy( t_child_tran.gameObject );
	}
}
