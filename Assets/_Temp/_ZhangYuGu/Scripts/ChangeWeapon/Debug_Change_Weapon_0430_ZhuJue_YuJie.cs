using UnityEngine;
using System.Collections;

public class Debug_Change_Weapon_0430_ZhuJue_YuJie : MonoBehaviour {

	public GameObject m_weapon_dao;
	
	//public GameObject m_weapon_dun;
	
	public GameObject m_weapon_chang_mao;
	
	public GameObject m_weapon_gong;
	
	
	public Transform m_pos_dao;
	
	//public Transform m_pos_dun;
	
	public Transform m_pos_chang_mao;
	
	public Transform m_pos_gong;
	
	
	private GameObject[] m_cur_weapons = new GameObject[ 2 ];
	
	
	private Animation m_animation;
	
	private int m_animation_index = 0;
	
	// Use this for initialization
	void Start () {
		m_cur_weapons[ 0 ] = null;
		
		m_cur_weapons[ 1 ] = null;
		
		m_animation = GetComponent<Animation>();
	}

	private AnimationClip GetAnimation( int p_index ){
		int t_i = 0;
		
		AnimationClip t_clip_0 = null;
		
		foreach( AnimationState t_clip in m_animation ){
			if( t_clip_0 == null ){
				t_clip_0 = t_clip.clip;
			}
			
			if( t_i == p_index ){
				return t_clip.clip;   
			}
			
			t_i++;   
		}
		
		m_animation_index = 0;
		
		return t_clip_0;
	} 
	
	public void ChangeAnimation(){
		m_animation_index++;
		
		AnimationClip t_clip = GetAnimation( m_animation_index );
		
		m_animation.Play( t_clip.name );
		
		Debug.Log( "ChangeAnimation: " + m_animation_index + ", " + t_clip.name );

		NGUIDebug.ClearLogs();

		NGUIDebug.Log( m_animation_index + ": " + t_clip.name );
	}
	
	public void ClearWeapon(){
		if( m_cur_weapons[ 0 ] != null ){
			Destroy( m_cur_weapons[ 0 ] );
			
			m_cur_weapons[ 0 ] = null;
		}
		
		if( m_cur_weapons[ 1 ] != null ){
			Destroy( m_cur_weapons[ 1 ] );
			
			m_cur_weapons[ 1 ] = null;
		}
	}
	
	public void ChangeWeapon_DaoDun(){
		Debug.Log( "ChangeWeapon_DaoDun()" );
		
		ClearWeapon();
		
		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_dao, m_pos_dao.transform.position, m_pos_dao.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_dao.parent;
		
		//m_cur_weapons[ 1 ] = (GameObject)Instantiate( m_weapon_dun, m_pos_dun.transform.position, m_pos_dun.transform.rotation  );
		//m_cur_weapons[ 1 ].transform.parent = m_pos_dun.parent;
	}
	
	public void ChangeWeapon_ChangMao(){
		Debug.Log( "ChangeWeapon_ChangMao()" );
		
		ClearWeapon();
		
		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_chang_mao, m_pos_chang_mao.transform.position, m_pos_chang_mao.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_chang_mao.parent;
		
	}
	
	public void ChangeWeapon_GongJian(){
		Debug.Log( "ChangeWeapon_GongJian()" );
		
		ClearWeapon();
		
		m_cur_weapons[ 0 ] = (GameObject)Instantiate( m_weapon_gong, m_pos_gong.transform.position, m_pos_gong.transform.rotation );
		m_cur_weapons[ 0 ].transform.parent = m_pos_gong.parent;
	}
}
