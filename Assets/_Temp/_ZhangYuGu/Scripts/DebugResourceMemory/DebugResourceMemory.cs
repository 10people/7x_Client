using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugResourceMemory : MonoBehaviour {

	public int m_max_res_count = 9;

	public static List<Texture2D> m_textures = new List<Texture2D>();

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		int t_btn_index = 0;

		int t_left_offset = 0;

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Load Res" ) ){
			bool t_found = false;

			for( int i = 0; i < m_max_res_count; i++ ){
				Texture2D t_tex = (Texture2D)Resources.Load( Res2DTemplate.GetResPath( Res2DTemplate.Res.DRILL_CARD_SMALL_PREFIX ) + 
					"10" + ( i + 1 ) );
				
				if( !m_textures.Contains( t_tex ) ){
					m_textures.Add( t_tex );

					t_found = true;

					break;
				}
			}
						
			if( !t_found ){
				Debug.Log( "All Added." );
			}
		}

		OnUITextures();

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Clear Unused" ) ){
			Resources.UnloadUnusedAssets();		
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Remove&Unload" ) ){
			if( m_textures.Count > 0 ){
				Texture2D t_tex = m_textures[ m_textures.Count - 1 ];

				m_textures.Remove( t_tex );

				Resources.UnloadAsset( t_tex );
			}
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Force Clear" ) ){
			for( int i = m_textures.Count - 1; i >= 0; i-- ){
				Resources.UnloadAsset( m_textures[ i ] );
			}
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Dont Destroy" ) ){
			for( int i = m_textures.Count - 1; i >= 0; i-- ){
				DontDestroyOnLoad( m_textures[ i ] );
			}
		}

		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Switch Scene" ) ){
			Application.LoadLevel( 0 );
		}
	}

	#endregion



	#region Interaction

	private void OnUITextures(){
		int t_max_ler_col = 8;

		for( int i = m_textures.Count - 1; i >= 0; i-- ){
			if( GUI.Button ( GetRect( Screen.width - 100 * ( 1 + i / t_max_ler_col ), i % t_max_ler_col ), m_textures[ i ] ) ){
				m_textures.Remove( m_textures[ i ] );
			}
		}
	}

	#endregion



	#region Utilities

	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 50 * p_index_y, 100, 50 );
	}

	#endregion
}