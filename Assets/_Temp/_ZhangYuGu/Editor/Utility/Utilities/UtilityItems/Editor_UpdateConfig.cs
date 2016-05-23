//#define OPEN_XML_SYNC



//#define DEBUG_XML_SYNC



using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System;
using System.Xml.Linq;
using UnityEditor.Callbacks;
using System.Data;
using Mono.Data.Sqlite;
using System.Linq;  
using System.Text;  



public class Editor_UpdateConfig : AssetPostprocessor {

	#region Unity

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		#if DEBUG_XML_SYNC
		Debug.Log( "OnPostprocessAllAssets()" );
		#endif

		AssetsChanged();
	}


	[DidReloadScripts(100)]
	public static void ReloadedScripts(){
		AssetsChanged();
	}

	#endregion



	#region XML

	private static void UpdateClientXML(){
		string t_server_dir = Application.dataPath + "/_Temp/_ZhangYuGu/_Temp/_ServerXMLCache";

		string t_client_dir = Application.dataPath + "/Resources/_Data/Design";

		DirectoryInfo t_server_dir_info = new DirectoryInfo( t_server_dir );

		DirectoryInfo t_client_dir_info = new DirectoryInfo( t_client_dir );

		FileInfo[] t_server_files = t_server_dir_info.GetFiles();

		FileInfo[] t_client_files = t_client_dir_info.GetFiles();

		for( int i = 0; i < t_client_files.Length; i++ ){
			FileInfo t_client_file = t_client_files[ i ];

			if( t_client_file.Name.EndsWith( ".meta" ) ){
				continue;
			}

			bool t_file_updated = false;

			bool t_file_found = false;

			for( int j = 0; j < t_server_files.Length; j++ ){
				FileInfo t_server_file = t_server_files[ j ];

				if( t_server_file.Name != t_client_file.Name ){
					continue;
				}
				else{
					t_file_found = true;

					#if DEBUG_XML_SYNC
					Debug.Log( "File Found: " + t_client_file.FullName );
					#endif
				}

				{
					string t_server_md5 = FileHelper.GetMd5Hash( t_server_file.FullName );

					string t_client_md5 = FileHelper.GetMd5Hash( t_client_file.FullName );

					if( t_client_md5 == t_server_md5 ){
						if( t_file_found ){
							#if DEBUG_XML_SYNC
							Debug.Log( "File Same: " + t_client_file.FullName );

							Debug.Log( "t_server_md5: " + t_server_md5 );

							Debug.Log( "t_client_md5: " + t_client_md5 );
							#endif
						}

						continue;
					}
				}

				#if DEBUG_XML_SYNC
				Debug.Log( "Copy : " + t_server_file.Name + "   To   " + t_client_file.FullName );
				#endif

				{
					t_server_file.CopyTo( t_client_file.FullName, true );

					t_file_updated = true;
				}
			}

			if( !t_file_found ){
				#if DEBUG_XML_SYNC
				Debug.LogError( "Error, File Lost: " + t_client_file.Name );
				#endif	
			}
		}
	}

	#endregion



	#region Local Cache

	private static void UpdateLocalCache(){
		#if UNITY_IOS
		return;
		#endif

		string t_project_root = PathHelper.GetParentPath( Application.dataPath );

		string t_db_path = t_project_root + "/.svn/wc.db";

		{
			if( !File.Exists( t_db_path ) ){
				t_db_path = PathHelper.GetParentPath( t_project_root ) + "/.svn/wc.db";
			}

			if( !File.Exists( t_db_path ) ){
				t_db_path = PathHelper.GetParentPath( t_project_root ) + "/.svn/wc.db";
			}

			if( !File.Exists( t_db_path ) ){
				t_db_path = PathHelper.GetParentPath( t_project_root ) + "/.svn/wc.db";
			}

			if( !File.Exists( t_db_path ) ){
//				Debug.Log( "File Not Exist: " + t_db_path );

				return;
			}
		}

		string ConnectionString = "Data Source=" + 
			t_db_path +
			";Pooling=true;FailIfMissing=false";  

		SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();  

		builder.DataSource = t_db_path;  

		builder.Pooling = true;  

		SqliteConnection conn = null;

		try{
			conn = new SqliteConnection( ConnectionString );  

			conn.ConnectionString = builder.ToString();  

			conn.Open();  
		}
		catch( Exception e ){
			if( conn != null ){
				conn.Close();
			}

			return;
		}

		using ( SqliteCommand cmd = conn.CreateCommand() ){  
			cmd.CommandText = "SELECT Max(revision) FROM [NODES_CURRENT];";  

			using( SqliteDataReader t_reader = cmd.ExecuteReader() ){
				int t_index = 0;
				while( t_reader.Read() ){
					try{
						UpdateConfig( t_reader.GetInt64( 0 ) );
					}
					catch( System.Exception e ){
						Debug.LogError( "Exception: " + e );
					}

					t_index++;
				}
			}
		} 

		conn.Close(); 
	}

	private static void UpdateConfig( long p_version ){
		string t_path = Application.dataPath + "/Resources/_Data/Config/LocalCache.txt";

		string t_content = FileHelper.ReadString( t_path );

		Dictionary<string, string> t_dict = new Dictionary<string, string>();

		UtilityTool.LoadStringStringDict( t_dict, t_content, ConfigHelper.CONST_LINE_SPLITTER );

		UtilityTool.UpdateStringStringDictKeyValue( t_dict, LocalCacheTool.CONST_VERSION, TimeHelper.GetCurrentTime_String( "MMdd-HHmm" ) );

		UtilityTool.UpdateStringStringDictKeyValue( t_dict, LocalCacheTool.CONST_VERSION_CODE, p_version + "" );

		StringBuilder t_builder = new StringBuilder();

		foreach( KeyValuePair<string, string> t_kv in t_dict ){
			t_builder.Append( t_kv.Key + " : " + t_kv.Value + "\n" );
		}

		FileHelper.WriteFile( t_path, t_builder.ToString() );
	}

	#endregion



	#region Utilities

	private static void AssetsChanged(){
		{
			UpdateLocalCache();
		}

		#if OPEN_XML_SYNC
		{
			UpdateClientXML();
		}
		#endif
	}

	#endregion

}
