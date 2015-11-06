using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class LocalDataTool{

	public Dictionary<string,string> m_data_dict = new Dictionary<string,string>();


	private bool m_debug = true;

	private static LocalDataTool m_instance = null;

	public static LocalDataTool Instance(){
		if( m_instance == null ){
			m_instance = new LocalDataTool();
		}

		return m_instance;
	}


	#region File Ops

	public void Load(){
		System.IO.FileStream t_stream = GetLocalFileStream();
		
		byte[] t_bytes = new byte[ t_stream.Length ];
		
		t_stream.Read( t_bytes, 0, (int)t_stream.Length );
		
		string t_str = Encoding.UTF8.GetString( t_bytes );
		
		//Debug.Log( "data string: " + t_str );
		
		// read info
		{
			m_data_dict.Clear();
			
			string[] t_lines = t_str.Split( '\n' );
			
			foreach( string t_line in t_lines ){
				string[] t_info = t_line.Split( '=' );
				
				if( t_info.Length < 2 ){
					//Debug.LogError( "info cons error: " + t_info );
					
					continue;
				}
				
				m_data_dict.Add( t_info[ 0 ], t_info[ 1 ] );
			}

			if( m_debug ){
				Log();
			}
		}
		
		t_stream.Close();
	}

	public void Save(){
		System.IO.FileStream t_stream = GetLocalFileStream();
		
		string t_data = "";
		
		foreach( KeyValuePair<string,string> t_pair in m_data_dict ){
			t_data += ( t_pair.Key + "=" + t_pair.Value + "\n" );
		}
		
		//Debug.Log( "Write: " + t_data );
		
		byte[] t_bytes = Encoding.UTF8.GetBytes( t_data );
		
		t_stream.Write( t_bytes, 0, t_bytes.Length );
		
		t_stream.Close();
	}


	#endregion


	#region Utilities

	private string GetLocalInfoFilePathName(){
		return Application.persistentDataPath + "/Local_File.bin";
	}

	private System.IO.FileStream GetLocalFileStream(){
		string t_local_file_name = GetLocalInfoFilePathName();
		
		System.IO.FileStream t_stream = new System.IO.FileStream( t_local_file_name, System.IO.FileMode.OpenOrCreate );

#if UNITY_IPHONE
		UnityEngine.iOS.Device.SetNoBackupFlag( t_local_file_name );
#endif

		return t_stream;
	}

	public void Log(){
		Debug.Log( "--- LocalDataTool.Info Begin ---" );

		foreach( KeyValuePair<string,string> t_pair in m_data_dict ){
			Debug.Log( t_pair.Key + "=" + t_pair.Value );
		}

		Debug.Log( "--- LocalDataTool.Info End ---" );
	}

	#endregion
}
