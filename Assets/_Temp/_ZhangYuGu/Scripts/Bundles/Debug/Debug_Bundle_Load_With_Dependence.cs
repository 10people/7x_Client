using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

public class Debug_Bundle_Load_With_Dependence : MonoBehaviour {

	private const string DEPENDENCE_TREE_TAG	= "Dependence";


	private Dictionary<string, AssetBundle> m_bundle_dict = new Dictionary<string, AssetBundle> ();

	private int m_loading_count = 0;

	private static JSONNode m_bundle_node;

	#region WWW Loader
	
	IEnumerator DownloadAndCache ( string p_url, int p_version, string p_bundle_dict_key ){
		WWW t_www;
		
		if( Caching.enabled ) { 
			while ( !Caching.ready ){
				yield return null;
			}
			
			t_www = WWW.LoadFromCacheOrDownload( p_url, p_version );
		}
		else {
			t_www = new WWW( p_url );
		}
		
		yield return t_www;

		if ( t_www.error != null ) {
			Debug.LogError( t_www.error + " : " + 
			               p_bundle_dict_key + ", " +
			               p_version + ", " +
			               p_url );
			
			t_www.Dispose();

			{
				m_bundle_dict.Remove( p_bundle_dict_key );
				
				DecreaseLoadingCount();
			}
			
			yield break;
		}
		
//		Debug.Log ( "t_www isDone: " + 
//		           t_www.isDone + ", " +
//		           "progress: " + t_www.progress );
		
		if (t_www.assetBundle == null) {
			Debug.LogError ("t_www.assetBundle = null.");

			m_bundle_dict.Remove( p_bundle_dict_key );
		} else {
			AssetBundle t_bundle = t_www.assetBundle;
			
			//Debug.Log ( "Bundle Downloaded = null: " + ( t_bundle == null ) );
			
			//Debug.Log ( "Log Bundle: " + t_bundle );
			
			m_bundle_dict[ p_bundle_dict_key ] = t_bundle;
		}

		// decrease loading count
		{
			DecreaseLoadingCount();
		}
	}

	#endregion
	
	void OnGUI(){
		int t_button_index = 0;
		
		if( GUI.Button( GetButton( t_button_index++ ), "Load Config" ) ){
			string t_config_path_name = Application.streamingAssetsPath + 
				"/" + 
				"ui_atlas_prefabs.config";

			FileStream t_file_stream = new FileStream( t_config_path_name,
			                                          FileMode.Open );

			StreamReader t_stream_reader = new StreamReader(
				t_file_stream,
				Encoding.Default );

			string t_json_string = t_stream_reader.ReadToEnd();

			t_stream_reader.Close();
			
			t_file_stream.Close();

			// json node
			{
				m_bundle_node = JSONNode.Parse( t_json_string );
			}
		}

		if (GUI.Button (GetButton (t_button_index++), "Load & Create Prefab")) {
			string t_path = "_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";

			Object t_obj = Resources.Load( t_path );
			
			GameObject t_gb = (GameObject)Instantiate( t_obj );
		}

		if (GUI.Button (GetButton (t_button_index++), "Load All Bundle")) {
			LoadAllBundle();
		}

		if( GUI.Button( GetButton( t_button_index++ ), "Find & Load Bundle" ) ){
			string t_path = "_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";

			string t_bundle_path = FindBundleKey( t_path );

			LoadBundle_Local( t_bundle_path );
		}

		if( GUI.Button( GetButton( t_button_index++ ), "Load & Create From Bundle" ) ){
			string t_res_path = "_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";

			string t_bundle_path = FindBundleKey( t_res_path );

			Object t_obj = LoadBundleAsset( t_bundle_path, t_res_path );

			GameObject t_gui_gb = (GameObject)Instantiate( t_obj );
		}

		if (GUI.Button (GetButton (t_button_index++), "unload useless")) {
			Debug.Log ("unload useless.");
			
			Resources.UnloadUnusedAssets();
		}
		
		if (GUI.Button (GetButton (t_button_index++), "Clean Cache")) {
			Debug.Log ("Clean Cache.");
			
			Caching.CleanCache();
		}

//		int t_label_index = 0;
//		GUI.Label( GetLabelRect ( t_label_index++ ), "Dependencies" );
		
//		if (m_tex != null) {
//			GUI.DrawTexture (new Rect (0, 100, 100, 100), m_tex);
//		}
	}

	#region Utilities
	
	private static Rect GetButton( int p_index ){
		return new Rect( 100, p_index * 30, 300, 35 );	
	}
	
	private static Rect GetLabelRect( int p_index ){
		return new Rect (0, p_index * 40, 100, 50);
	}

	#endregion



	#region Bundle Loader

	/* Params:
	 * 1.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 */
	public static string FindBundleKey( string p_res_asset_path ){
		string t_relative_bundle_path = GetResourceRelativePath( p_res_asset_path );

		for( int i = 0; i < m_bundle_node.Count; i++ ){
			JSONNode t_node = m_bundle_node[ i ][ "Items" ];

			for( int j = 0; j < t_node.Count; j++ ){
				string t_res_path = t_node[ j ][ "path" ];

				if( t_res_path == t_relative_bundle_path ){
					string t_key = ( (JSONClass)m_bundle_node ).GetKey( i );

					return t_key;
				}
			}
		}

		Debug.LogError( "Error Bundle Not Found: " + t_relative_bundle_path );

		return "";
	}

	public void LoadAllBundle(){
		for( int i = 0; i < m_bundle_node.Count; i++ ){
			string t_key = ( (JSONClass)m_bundle_node ).GetKey( i );

			LoadBundle_Local( t_key );
		}
	}

	/* Params:
	 * 1.p_bundle_path:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 */
	public void LoadBundle_Local( string p_bundle_key ){
		string t_bundle_url = "file://" + Application.streamingAssetsPath + "/" + p_bundle_key;
		//string t_path = "file://" + Application.streamingAssetsPath + "/iOS/UIResources/MemoryTrace/Textures.unity3d";
		//string t_path = Application.streamingAssetsPath + "/Android/UIResources/MemoryTrace/Textures.unity3d";

		// check if exist
		{
			if( m_bundle_dict.ContainsKey( p_bundle_key ) ){
				Debug.Log( "------ Key Already Contained: " + p_bundle_key + " --- " );
				
				return;
			}
		}

		string t_dependence_relative_path = m_bundle_node[ p_bundle_key ][ DEPENDENCE_TREE_TAG ];

		if( t_dependence_relative_path != null ){
			if( t_dependence_relative_path != "" ){
				char[] t_splitter = { ';' };

				string[] t_depends = t_dependence_relative_path.Split( t_splitter );

				for( int i = 0; i < t_depends.Length; i++ ){
					if( t_depends[ i ] != null ){
						if( t_depends[ i ] != "" ){
							LoadBundle_Local( t_depends[ i ] );
						}
					}
				}
			}
		}

		// add to dict to reduce unnecessary load
		{
			m_bundle_dict.Add( p_bundle_key, null );

			IncreaseLoadingCount();
		}
		
		StartCoroutine( DownloadAndCache(
			t_bundle_url,
			0,
			p_bundle_key ) );
	}

	/* Params:
	 * 1.p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
	 * 2.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 */
	public Object LoadBundleAsset( string p_bundle_key, string p_res_asset_path ){
		string t_asset_name = p_res_asset_path.Substring( p_res_asset_path.LastIndexOf( '/' ) + 1 );

		return m_bundle_dict[ p_bundle_key ].LoadAsset( t_asset_name );
	}

	#endregion



	#region Build Bundle Utilities

	/* Params:
	 * 1.p_res_path:	"_UIs/_CommonAtlas/Network/NetworkReConnectProcessor";
	 */
	static string GetResourceRelativePath( string p_res_asset_path ){
		string t_relative_bundle_path = "Resources/" + p_res_asset_path;

		return t_relative_bundle_path;
	}

	void IncreaseLoadingCount(){
		m_loading_count++;
	}

	void DecreaseLoadingCount(){
		m_loading_count--;

		if( m_loading_count == 0 ){
			Debug.Log( "WWW Loading Done." );
		}
	}

	#endregion
}
