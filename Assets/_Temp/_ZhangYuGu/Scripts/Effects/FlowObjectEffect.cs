using UnityEngine;
using System.Collections;

public class FlowObjectEffect : MonoBehaviour {

	#region Mono

	public Material m_mat = null;

	public Color m_main_color = Color.white;

	public Vector4 m_str_x_y_z_xyz = Vector4.zero;
	
	public Vector4 m_pos_center_xyz = Vector4.zero;
	
	

	// Use this for initialization
	void Start () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#endregion



	#region Set

	public void Init(){
		MeshRenderer t_render = GetComponent<MeshRenderer>();

		if( t_render == null ){
			return;
		}

		m_mat = t_render.material;

		if( m_mat == null ){
			Debug.LogError( "Mat = null, return." );

			return;
		}
		
		UpdateMat();
	}

	private void UpdateMat(){
		if( m_mat == null ){
			return;
		}

		m_mat.SetColor( "_Color", m_main_color );

		m_mat.SetVector( "_Str", m_str_x_y_z_xyz );

		m_mat.SetVector( "_Pos", m_pos_center_xyz );
	}

	#endregion
}
