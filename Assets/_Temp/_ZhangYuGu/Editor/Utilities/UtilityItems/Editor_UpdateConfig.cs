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

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		AssetsChanged();
	}


	[DidReloadScripts(100)]
	public static void ReloadedScripts(){
		AssetsChanged();
	}

	private static void AssetsChanged(){
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
				Debug.Log( "File Not Exist: " + t_db_path );

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
}
