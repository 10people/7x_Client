//#define DEBUG_MANIFEST



using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.12.7
 * @since:		Unity 5.1.3
 * Function:	Helper class for Custom Manifest.
 * 
 * Notes:
 * 1.Custom Manifest in Unity 5.
 */ 
public class ManifestHelper {

	#region Analyze

	public static void SetManifest( UnityEngine.Object p_object ){
		TextAsset t_manifest = (TextAsset)p_object;
		
		if( t_manifest == null ){
			Debug.LogError( "Error in SetManifest(), manifest is null." );
			
			return;
		}

		{
			Clean();
		}

		{
			#if DEBUG_MANIFEST
			Debug.Log( "-------------- Custom File ------------ \n" + t_manifest.text );
			#endif

			string[] t_contents = t_manifest.text.Split( new string[]{ "\n" }, System.StringSplitOptions.None );
			
			AnalyzeContents( t_contents );
		}

		{
			ConstructAssetDict();
		}
	}

	#endregion



	#region Asset Dict

	public static string GetBundleKey( string p_asset_path ){
		if( m_asset_dict.ContainsKey( p_asset_path ) ){
			return m_asset_dict[ p_asset_path ].m_bundle;
		}
		else{
			return "";
		}
	}

	public static bool IsSceneAsset( string p_asset_path ){
		if( !m_asset_dict.ContainsKey( p_asset_path ) ){
			return false;
		}

		return m_asset_dict[ p_asset_path ].ContainScene( p_asset_path );
	}

	/// Final Dict.
	private static Dictionary<string, CustomManifestInfo> m_asset_dict = new Dictionary<string, CustomManifestInfo>();

	private static void ConstructAssetDict(){
		for( int i = 0; i < m_manifest_list.Count; i++ ){
			CustomManifestInfo t_info = m_manifest_list[ i ];

			for( int j = 0; j < t_info.m_assets_list.Count; j++ ){
				AddInfo( t_info.m_assets_list[ j ], t_info );
			}

			for( int j = 0; j < t_info.m_scenes_list.Count; j++ ){
				AddInfo( t_info.m_scenes_list[ j ], t_info );
			}
		}
	}

	private static void AddInfo( string p_res, CustomManifestInfo p_ref ){
		if( m_asset_dict.ContainsKey( p_res ) ){
			Debug.LogError( "Error, Key Already Contained: " + p_res );
			return;
		}

		m_asset_dict.Add( p_res, p_ref );
	}

	#endregion



	#region File Cache

	/// File Cache.
	private static List<CustomManifestInfo> m_manifest_list = new List<CustomManifestInfo>();

	private static void AnalyzeContents( string[] p_contents ){
		int t_index = 0;
		
		while( t_index < p_contents.Length ){
//			#if DEBUG_MANIFEST
//			Debug.Log( "Analyze: " + t_index );
//			#endif

			t_index = CustomManifestInfo.AnalyzeManifest( t_index, p_contents );
		}

//		#if DEBUG_MANIFEST
//		for( int i = 0; i < m_manifest_list.Count; i++ ){
//			CustomManifestInfo t_info = m_manifest_list[ i ];
//
//			t_info.Log();
//		}
//		#endif
	}

	#endregion



	#region Clean

	private static void Clean(){
		{
			m_manifest_list.Clear();
		}

		{
			m_asset_dict.Clear();
		}
	}

	#endregion



	#region Const

	/// ------------------------------------ Root --------------------------------------
	public const string COSNT_UPDATE_FILE_NAME		= "Bundles.txt";

	public const string CONST_MANIFEST_FOLDER_NAME		= "platform";

	public const string CONST_FILE_NAME					= "file";

	public const string CONST_ROOT_BUNDLE_ASSET_NAME	= "assetbundlemanifest";
	/// ------------------------------------ Root --------------------------------------



	/// ------------------------------------ File --------------------------------------
	public const string CONST_KV_SPLITTER		= ":";

	public const string CONST_BUNDLE_KEY		= "bundle";

	public const string CONST_SIZE_KEY			= "size";

	public const string CONST_VERSION_KEY		= "version";

	public const string CONST_ASSETS_KEY		= "assets";

	public const string CONST_SCENES_KEY		= "scene";
	/// ------------------------------------ File --------------------------------------

	#endregion


	
	
	private class CustomManifestInfo{
		
		/// assets/resources/_data/config/config
		public string m_bundle	= "";
		
		/// -2G to 2G
		public int m_file_size = 0;
		
		/// retained for next version to use
		public int m_version = -1;
		
		
		
		/// _Data/Design/Action
		public List<string> m_assets_list = new List<string>();
		
		/// _Empty
		public List<string> m_scenes_list = new List<string>();
		
		/// Split line with ":"
		private static string[] SplitLineContent( string p_line ){
			return p_line.Split( new string[]{ ManifestHelper.CONST_KV_SPLITTER }, System.StringSplitOptions.None );
		}

		public bool ContainScene( string p_scene_name ){
			for( int i = 0; i < m_scenes_list.Count; i++ ){
				if( m_scenes_list[ i ] == p_scene_name ){
					return true;
				}
			}

			return false;
		}

		public void Log(){
			Debug.Log( "------ CustomManifestInfo: " + m_bundle );

			Debug.Log( "size: " + m_file_size );

			Debug.Log( "version: " + m_version );

			for( int i = 0; i < m_assets_list.Count; i++ ){
				Debug.Log( "asset " + i + " : " + m_assets_list[ i ] );
			}

			for( int i = 0; i < m_scenes_list.Count; i++ ){
				Debug.Log( "scene " + i + " : " + m_scenes_list[ i ] );
			}
		}
		
		/// Analyzes the manifest.
		/// 
		/// Params:
		/// 1.p_current_index: Current line to analyze;
		/// 2.p_contents: Content to analyze;
		public static int AnalyzeManifest( int p_current_index, string[] p_contents ){
			if( p_contents == null ){
				Debug.Log( "Content is null." );
				
				return p_current_index;
			}
			
			if( p_current_index >= p_contents.Length ){
				Debug.LogError( "Out of Bound." );
				
				return p_current_index;
			}
			
			CustomManifestInfo t_manifest = new CustomManifestInfo();
			
			// bundle
			{
				string[] t_line_info = SplitLineContent( p_contents[ p_current_index ] );
				
				if( t_line_info[ 0 ] == CONST_BUNDLE_KEY ){
					t_manifest.m_bundle = t_line_info[ 1 ];
					
					p_current_index++;
				}
				else{
					Debug.LogWarning( "Nothing in file." );

					return p_current_index + 1;
				}
			}

			{
				m_manifest_list.Add( t_manifest );
			}

			// size
			{
				string[] t_line_info = SplitLineContent( p_contents[ p_current_index ] );
				
				if( t_line_info[ 0 ] == CONST_SIZE_KEY ){
					t_manifest.m_file_size = int.Parse( t_line_info[ 1 ] );
					
					p_current_index++;
				}
				else{
					Debug.LogError( "Error in size." );
				}
			}
			
			// version
			{
				string[] t_line_info = SplitLineContent( p_contents[ p_current_index ] );
				
				if( t_line_info[ 0 ] == CONST_VERSION_KEY ){
					t_manifest.m_version = int.Parse( t_line_info[ 1 ] );
					
					p_current_index++;
				}
				else{
					Debug.LogError( "Error in version." );
				}
			}
			
			// assets
			if( p_current_index < p_contents.Length ){
				string[] t_line_info = SplitLineContent( p_contents[ p_current_index ] );
				
				if( t_line_info[ 0 ] == CONST_ASSETS_KEY ){
					p_current_index++;
					
					while( true ){
						if( p_current_index >= p_contents.Length ){
							break;
						}

						string[] t_items = SplitLineContent( p_contents[ p_current_index ] );
						
						if( t_items[ 0 ] == CONST_SCENES_KEY || t_items[ 0 ] == CONST_BUNDLE_KEY ){
							break;
						}
						
						{
							if( !string.IsNullOrEmpty( t_items[ 0 ] ) ){
//								#if DEBUG_MANIFEST
//								Debug.Log( "Add Asset: " + t_items[ 0 ] + " - " + t_manifest.m_bundle );
//								#endif

								t_manifest.m_assets_list.Add( t_items[ 0 ] );
							}
							
							p_current_index++;
						}
					}
				}
			}
			
			// scenes
			if( p_current_index < p_contents.Length ){
				string[] t_line_info = SplitLineContent( p_contents[ p_current_index ] );
				
				if( t_line_info[ 0 ] == CONST_SCENES_KEY ){
					p_current_index++;
					
					while( true ){
						if( p_current_index >= p_contents.Length ){
							break;
						}

						string[] t_items = SplitLineContent( p_contents[ p_current_index ] );
						
						if( t_items[ 0 ] == CONST_BUNDLE_KEY ){
							break;
						}
						
						{
							if( !string.IsNullOrEmpty( t_items[ 0 ] ) ){
								#if DEBUG_MANIFEST
								Debug.Log( "Add Scene: " + t_items[ 0 ] + " - " + t_manifest.m_bundle );
								#endif

								t_manifest.m_scenes_list.Add( t_items[ 0 ] );
							}

							p_current_index++;
						}
					}
				}
			}
			
			return p_current_index;
		}
	}
}
