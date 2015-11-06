using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debug_Load_JPG : MonoBehaviour {

	public List<Texture2D> m_textures = new List<Texture2D>();


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
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Load JPG" ) ){
			Debug.Log( "Load JPG." );
			
			TextAsset t_tex_text = (TextAsset)Resources.Load( "bytes/jpgs/512" );

			Texture2D t_tex = new Texture2D( 1, 1 );

			t_tex.LoadImage( t_tex_text.bytes );

			if( !m_textures.Contains( t_tex ) ){
				m_textures.Add( t_tex );
				
				Debug.Log( "Load Res." );
			}
		}

		OnUITextures();
		
		if( GUI.Button( GetRect( t_left_offset, t_btn_index++ ), "Switch Scene" ) ){
			Debug.Log( "Switch Scene" );
			
			Application.LoadLevel( 0 );
		}
	}
	
	#endregion
	

	
	#region Interaction
	
	private void OnUITextures(){
		int t_max_ler_col = 8;
		
		for( int i = m_textures.Count - 1; i >= 0; i-- ){
			if( GUI.Button ( GetTextureRect( Screen.width - 200 * ( 1 + i / t_max_ler_col ), i % t_max_ler_col ), m_textures[ i ] ) ){
				Debug.Log( "Remove Texture: " + i );
				
				m_textures.Remove( m_textures[ i ] );
			}
		}
	}
	
	#endregion
	
	
	
	#region Utilities
	
	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 75 * p_index_y, 200, 75 );
	}

	private Rect GetTextureRect( int p_x, int p_index_y ){
		return new Rect( p_x, 200 * p_index_y, 200, 200 );
	}

	#endregion
}
