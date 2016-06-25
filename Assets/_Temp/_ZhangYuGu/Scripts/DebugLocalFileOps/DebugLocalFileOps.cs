using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;



public class DebugLocalFileOps : MonoBehaviour {

	public const string LOCAL_DATA_KEY_NEWHAND	= "newhand";
	
	public const string LOCAL_DATA_KEY_USERNAME	= "username";
	
	public const string LOCAL_DATA_KEY_PASSWORD	= "password";

	private Dictionary<string,string> m_dict = new Dictionary<string,string>();

	#region Mono

	void OnGUI(){
		int t_btn_index = 0;
		
		int t_btn_left_offset = 0;
		
		if( GUI.Button( GetRect( t_btn_left_offset, t_btn_index++ ), "Read" ) ){
			System.IO.FileStream t_stream = GetLocalFileStream();

			byte[] t_bytes = new byte[ t_stream.Length ];

			t_stream.Read( t_bytes, 0, (int)t_stream.Length );

			string t_str = Encoding.UTF8.GetString( t_bytes );

			// read info
			{
				m_dict.Clear();

				string[] t_lines = t_str.Split( '\n' );

				foreach( string t_line in t_lines ){
					string[] t_info = t_line.Split( '=' );

					if( t_info.Length < 2 ){
						//Debug.LogError( "info cons error: " + t_info );

						continue;
					}

					m_dict.Add( t_info[ 0 ], t_info[ 1 ] );
				}
			}

			t_stream.Close();

			#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag( GetLocalInfoFilePathName() );
			#endif
		}

		if( GUI.Button( GetRect( t_btn_left_offset, t_btn_index++ ), "Update" ) ){
			if( !m_dict.ContainsKey( "newhand" ) ){
				m_dict.Add( "newhand", "-1" );
			}

			int t_newhand_step = int.Parse( m_dict[ "newhand" ] );

			m_dict[ "newhand" ] = ( t_newhand_step + 1 ).ToString();

			System.IO.FileStream t_stream = GetLocalFileStream();

			string t_data = "";

			foreach( KeyValuePair<string,string> t_pair in m_dict ){
				t_data += ( t_pair.Key + "=" + t_pair.Value + "\n" );
			}

			byte[] t_bytes = Encoding.UTF8.GetBytes( t_data );

			t_stream.Write( t_bytes, 0, t_bytes.Length );
			
			t_stream.Close();

			#if UNITY_IPHONE
			UnityEngine.iOS.Device.SetNoBackupFlag( GetLocalInfoFilePathName() );
			#endif
		}

		if( GUI.Button( GetRect( t_btn_left_offset, t_btn_index++ ), "Tool Init" ) ){
			LocalDataTool.Instance().Load();

			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_NEWHAND ] = "0";

			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_USERNAME ] = "7xdouble";

			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_PASSWORD ] = "7XDOUBLE";
		}

		if( GUI.Button( GetRect( t_btn_left_offset, t_btn_index++ ), "Tool Update" ) ){
			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_NEWHAND ] = "2";
			
			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_USERNAME ] = "7xdouble_update";
			
			LocalDataTool.Instance().m_data_dict[ LOCAL_DATA_KEY_PASSWORD ] = "7XDOUBLE_update";

			LocalDataTool.Instance().Save();
		}


		int t_lb_index = 0;
		
		int t_lb_left_offset = ( int )( Screen.width * 0.6f );

		if( m_dict.ContainsKey( "newhand" ) ){
			GUI.Label( GetRect( t_lb_left_offset, t_lb_index++ ), m_dict[ "newhand" ] );
		}

		if( m_dict.ContainsKey( "username" ) ){
			GUI.Label( GetRect( t_lb_left_offset, t_lb_index++ ), m_dict[ "username" ] );
		}

		if( m_dict.ContainsKey( "password" ) ){
			GUI.Label( GetRect( t_lb_left_offset, t_lb_index++ ), m_dict[ "password" ] );
		}
	}
	
	#endregion
	
	
	
	#region Interaction

	
	#endregion



	#region Utilities

	private System.IO.FileStream GetLocalFileStream(){
		string t_local_file_name = GetLocalInfoFilePathName();
		
		System.IO.FileStream t_stream = new System.IO.FileStream( t_local_file_name, System.IO.FileMode.OpenOrCreate );

		#if UNITY_IPHONE
		UnityEngine.iOS.Device.SetNoBackupFlag( t_local_file_name );
		#endif

		return t_stream;
	}

	private string GetLocalInfoFilePathName(){
		return Application.persistentDataPath + "/Debug_Local_File.bin";
	}
	
	private Rect GetRect( int p_x, int p_index_y ){
		return new Rect( p_x, 50 * p_index_y, 100, 50 );
	}
	
	#endregion
}
