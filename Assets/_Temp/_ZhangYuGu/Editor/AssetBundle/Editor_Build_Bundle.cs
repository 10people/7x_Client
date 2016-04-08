// Default Open
#define BUNDLE_ITEMS
#define OPTIMIZE_D_O
#define CHECK_HASH
#define REMOVE_LEVELS
#define REMOVE_MD5
#define REMOVE_FILE_SIZE
#define REMOVE_CONFIG_FILES



#define DEBUG_BUILD


// Default Close
//#define DEVELOPMENT_BUILD
//#define DEPENDENCE_TREE
//#define ITEM_NAMES
//#define ITEM_EXTS
//#define DEPENDENCE_ASSETS
//#define CONTAINED_ASSETS
//#define CUSTOM_7ZIP_LOAD

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using SimpleJSON;


/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.3
 * @since:		Unity 4.5.3
 * Function:	Build Bundle files to maintain updates.
 * 
 * Notes:
 * None.
 */ 
public class Editor_Build_Bundle{

	
	/** Application.dataPath 		= "E:/WorkSpace_External/DynastyMobile_2014/Assets";
	 * 
	 * Application.streamingAssetsPath	= "E:/WorkSpace_External/DynastyMobile_2014/Assets/StreamingAssets";
	 * 
	 * AssetDatabase.GetAssetPath 	= "Assets/MyTextures/hello.png";
	 * 
	 */
	


	#region Build All Bundles

	public static void Build_All( BuildTarget p_build_target ){
		{
			ClearAllBundleJSON();
		}

//		{
//			Build_UI_Atlas_Prefab_Bundles( p_build_target );
//			
//			Build_UI_Image_Bundles( p_build_target );
//		}

		Build_Data( p_build_target );

//		Build_Sounds( p_build_target );

//		Build_3D_Models( p_build_target );

//		Build_Fx( p_build_target );
//
//		Build_Temp_Folders( p_build_target );
//
//		Build_Scenes( p_build_target );

		{
			Build_Configs( p_build_target );
			
			Rebuild_Bundle_List( p_build_target );
		}
	}

	#endregion



	#region Build Bundle List

	private static JSONClass m_all_bundle_json = new JSONClass();

	public static void ClearAllBundleJSON(){
		m_all_bundle_json = new JSONClass();
	}

	/// Manual ReBuild Bundles' Update List, could execute singly.
	public static void Rebuild_Bundle_List( BuildTarget p_build_target ){
		Debug.Log( "Rebuild_Bundle_List()" );

		int t_count = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

		JSONClass t_origin_json = m_all_bundle_json;

		{
			Debug.Log( "Origin Item Count: " + t_count );

			Debug.Log( "Origin Item: " + m_all_bundle_json.ToString( "" ) );
		}

		{
			ClearAllBundleJSON();
		}

		Generate_Bundle_List( p_build_target, "StreamingAssets/" + GetPlatformTag( p_build_target ) );

		{
//			Debug.Log( "BundleJSON: " + m_all_bundle_json.ToString( "" ) );

			int t_final_count = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

			Debug.Log( "Item Listed: " + t_final_count );
			
			if( t_count != t_final_count && t_count > 0 ){
//				Debug.LogError( "Error In Children Count." );

				{
					int t_cur_count = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

					for( int i = 0; i < t_cur_count; i++ ){
						string t_key_i = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];

						bool t_found = false;

						int t_origin_count = t_origin_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

						for( int j = 0; j < t_origin_count; j++ ){
							string t_key_j = t_origin_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];

							if( t_key_i == t_key_j ){
								t_found = true;

								break;
							}
						}

						if( !t_found ){
							Debug.Log( "Bundle Only In New List: " + t_key_i );
						}
					}
				}

				{
					int t_origin_count = t_origin_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;
					
					for( int i = 0; i < t_origin_count; i++ ){
						string t_key_i = t_origin_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];
						
						bool t_found = false;
						
						int t_cur_count = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;
						
						for( int j = 0; j < t_cur_count; j++ ){
							string t_key_j = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];
							
							if( t_key_i == t_key_j ){
								t_found = true;
								
								break;
							}
						}
						
						if( !t_found ){
							Debug.Log( "Bundle Only In Origin List: " + t_key_i );
						}
					}
				}
			}
		}

		Update_Bundle_List( p_build_target );
	}

	/** Desc:
	 * 1.p_relative_path: 	"_Project/ArtAssets/UIs", "Resources/_UIs";
	 * 
	 */
	public static void Generate_Bundle_List( BuildTarget p_build_target, string p_relative_path ){
//		Debug.Log( "Generate_Bundle_List( " + p_relative_path + " )" );

		string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
	
		{
			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );

			// files
			{
				FileInfo[] t_files = t_dir.GetFiles();

				for( int i = 0; i < t_files.Length; i++ ){
					FileInfo t_file_info = t_files[ i ];

					if( StringHelper.IsEndWith( t_file_info.FullName, ".meta#.txt" ) ){
						continue;
					}

					{
						int t_index = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

						string t_file_path = t_file_info.FullName.Replace( "\\", "/" );
						
						t_file_path = t_file_path.Substring( 
									Application.dataPath.Length + 1 + 
									"StreamingAssets/".Length +
									GetPlatformTag( p_build_target ).Length + 1 );

//						Debug.Log( "Process File: " + t_file_path );

						string t_key = t_file_path;
						
						m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_index ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ] = t_key;
						
						m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_index ][ CONFIG_BUNDLE_LIST_ITEM_SIZE_TAG ] = FileHelper.GetFileSize( t_file_info ).ToString();
						
						m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_index ][ CONFIG_BUNDLE_LIST_ITEM_MD5_TAG ] = GetMd5Hash( t_file_info );
						
						m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_index ][ CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG ] = 0 + "";
					}
				}
			}

			// dirs
			{
				DirectoryInfo[] t_dirs = t_dir.GetDirectories();
				
				for( int i = 0; i < t_dirs.Length; i++ ){
					DirectoryInfo t_sub_dir = t_dirs[ i ];
					
					string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );
					
					t_sub_dir_path = t_sub_dir_path.Substring( 
								Application.dataPath.Length + 1 );

//					Debug.Log( "Sub Dir: " + t_sub_dir_path );

					Generate_Bundle_List( p_build_target, t_sub_dir_path );
				}
			}
		}
	}

	public static void Update_Bundle_List( BuildTarget p_build_target ){
		Debug.Log( "Update_Bundle_List()" );

		string t_config_relative_path_name = All_Bundle_List_Path_Name + Bundle_Loader.Config_Path_Ext;

		if( string.IsNullOrEmpty( t_config_relative_path_name ) ){
			return;
		}

		// update info
		{
			VersionTool_4.Instance().Init();

			m_all_bundle_json[ CONFIG_BUNDLE_SMALL_VERSION_TAG ] = VersionTool_4.GetSmallVersion();

			m_all_bundle_json[ CONFIG_BUNDLE_BASE_VERSION_TAG ] = VersionTool_4.GetBaseVersion();

			m_all_bundle_json[ CONFIG_BUNDLE_BIG_VERSION_TAG ] = VersionTool_4.GetBigVersion();

			m_all_bundle_json[ CONFIG_BUNDLE_LIST_BUILD_TIME_TAG ] = DateTime.Now.ToLocalTime().ToString();
		}

		// check
		{
			for( int i = 0; i < m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count - 1; i++ ){
				string t_key_i = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];
				
				for( int j = i + 1; j < m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count; j++ ){
					string t_key_j = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];

					if( t_key_i == t_key_j ){
						Debug.LogError( "Error: " + t_key_i + "   - " + i + ", " + j );
					}
				}
			}
		}

		// version
		{
			UpdateBundleListVersions( p_build_target );
		}

		// write file
		{
			string t_config_path_name = GetStreamingFullPath_WithRelativePath( t_config_relative_path_name, p_build_target );
			
			FileStream t_file_stream = new FileStream( t_config_path_name,
			                                          FileMode.Create );
			
			StreamWriter t_stream_writer = new StreamWriter(
				t_file_stream,
				Encoding.Default );
			
			t_stream_writer.Write( m_all_bundle_json.ToString( "" ) );
			
			t_stream_writer.Close();
			
			t_file_stream.Close();
		}

		// refresh
		{
			AssetDatabase.Refresh();
		}
	}

	public static JSONNode GetArchivedJsonNode( BuildTarget p_build_target ){
		string t_full_path = PathHelper.GetFullPath_WithRelativePath( CONFIG_ARCHIVED_ASSETS_PATH );

		JSONNode t_pre_json = null;

		{
			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
			
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			DirectoryInfo t_target = null;
			
			long t_target_version = -1;
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_cur_str = t_sub_dir.Name.Replace( "_", "" );
				
				long t_cur = long.Parse( t_cur_str );
				
				//				Debug.Log( "Found Version: " + t_cur );
				
				if( t_cur > t_target_version ){
					t_target_version = t_cur;
					
					t_target = t_sub_dir;
				}
			}
			
			if( t_target != null ){
				string t_path = CONFIG_ARCHIVED_ASSETS_PATH + "/" + t_target.Name + "/" + GetPlatformTag( p_build_target ) + "/" + All_Bundle_List_Path_Name + Bundle_Loader.Config_Path_Ext;
				
				string t_config_path_name = GetFullPath_WithRelativePath( t_path );
				
				if( File.Exists( t_config_path_name ) ){
					FileStream t_file_stream = new FileStream( t_config_path_name,
					                                          FileMode.Open );
					
					StreamReader t_stream_reader = new StreamReader(
						t_file_stream,
						Encoding.Default );
					
					string t_content = t_stream_reader.ReadToEnd();
					
					t_pre_json = JSONNode.Parse( t_content );
					
					t_stream_reader.Close();
					
					t_file_stream.Close();
				}
				else{
					Debug.LogError( "Might Cause An Error: " + t_config_path_name );
				}
			}
		}

		return t_pre_json;
	}

	private static void UpdateBundleListVersions( BuildTarget p_build_target ){
		JSONNode t_pre_json = GetArchivedJsonNode( p_build_target );
		
		for( int i = 0; i < m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count; i++ ){
			string t_key_i = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];

			int t_new = 0;

			m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG ] = t_new + "";

			if( t_pre_json == null ){
				continue;
			}

			for( int j = 0; j < t_pre_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count; j++ ){
				string t_key_j = t_pre_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ];
				
				if( t_key_i == t_key_j ){
					int t_pre = int.Parse( t_pre_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG ] );

					string t_cur_md5 = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_MD5_TAG ];

					string t_pre_md5 = t_pre_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ j ][ CONFIG_BUNDLE_LIST_ITEM_MD5_TAG ];

					if( t_cur_md5 != t_pre_md5 ){
						t_new = t_pre + 1;
					}
					else{
						t_new = t_pre;
					}

					m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ i ][ CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG ] = t_new + "";

					break;
				}
			}
		}
	}

	#endregion



	#region Build Configs Bundle

	/// Build Assets Bundle's Configs.
	public static void Build_Configs( BuildTarget p_build_target ){
		Debug.Log( "Build_Configs()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_Config_Bundle.Begin ------",
		                     false,
		                     null,
		                     p_build_target );

		string t_cache_streaming_path = "StreamingAssetsCache/" + GetPlatformTag( p_build_target );

		string t_process_path = "StreamingAssets/" + GetPlatformTag ( p_build_target );

		// cache
		{
			string t_full_target_path = GetFullPath_WithRelativePath( t_cache_streaming_path );
			
			string t_full_src_path = GetFullPath_WithRelativePath( t_process_path );

			// clean
			FileHelper.DirectoryDelete( t_full_target_path, true );

			// copy
			{
				FileHelper.DirectoryCopy( t_full_src_path, t_full_target_path );
			}

			AssetDatabase.Refresh();
		}

		// Build Config
		{
			Build_Dir_To_One_Bundle( t_process_path,
			                        t_cache_streaming_path,
			                        ".txt",
			                        p_build_target,
			                        false,
			                        Bundle_Loader.Config_Bundle_Surfix );
		}

		// remove cache
		{
			string t_full_target_path = GetFullPath_WithRelativePath( "StreamingAssetsCache" );

			FileHelper.DirectoryDelete( t_full_target_path, true );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     false,
		                     null,
		                     p_build_target );

		#if REMOVE_CONFIG_FILES
		{
			string t_clean_path = t_process_path;

			Delete_Files_Under_Relative_Path( t_clean_path, ".txt" );
		}
		#endif

		{
			AssetDatabase.Refresh();
		}
	}
	
	#endregion



	#region Build New
	
	/// Prefabs Under "Resources/New".
	/// Prefabs Under "Resources/Global".
	public static void Build_Temp_Folders( BuildTarget p_build_target ){
		Debug.Log( "Build_Temp_Folders()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_New.Begin ------",
		                     false,
		                     null,
		                     p_build_target );
		
		string[] t_process_path = {
			"Assets/Resources/New",
			"Assets/Resources/Global",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                                 ".prefab",
			                                 p_build_target,
			                                 true );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.New_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}
	
	#endregion



	#region Build Fx
	
	// under Fx folder.
	public static void Build_Fx( BuildTarget p_build_target ){
		Debug.Log( "Build_Fx()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_Fx.Begin ------",
		                     false,
		                     null,
		                     p_build_target );
		
		string[] t_process_path = {
			"Assets/Resources/_3D/Fx",
		};

		// Base
		{
			Build_Fx_Base( p_build_target );
		}
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                               ".prefab",
			                               p_build_target,
			                               true );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Fx_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	public static void Build_Fx_Base( BuildTarget p_build_target ){
		for( int i = 0; i < CONFIG_FX_BUNDLE_BASE.Length; i++ ){
			string t_path = "Assets/" + CONFIG_FX_BUNDLE_BASE[ i ];
			
			Build_Dir_To_Multi_Bundle_Deeply( t_path,
			                                 "",
			                                 p_build_target,
			                                 true );
		}
	}
	
	#endregion



	#region Build 3D Models

	// under _3D folder.
	public static void Build_3D_Models( BuildTarget p_build_target ){
		Debug.Log( "Build_3D_Models()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_3D_Models.Begin ------",
		                     false,
		                     null,
		                     p_build_target );
		
		string[] t_process_path = {
			"Assets/Resources/_3D",

//			"Assets/Resources/_3D/Models/_Debug",
		};

		string[] t_exc_path = {
			"Resources/_3D/Fx",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                               	t_exc_path,
			                 				".prefab",
			                     			p_build_target,
			                        		true );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.D3D_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	#endregion



	#region Build Scenes

	/// Build All Scenes.
	/// 
	/// TODO
	/// NOTICE:
	/// 1.Only Bundle_Loader & Loading Exist In Build Settings.
	public static void Build_Scenes( BuildTarget p_build_target ){
		Debug.Log( "Build_Scenes()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_Scenes.Begin ------",
		                     false,
		                     null,
		                     p_build_target );

		string[] t_process_path = {
			"Assets/_Project/ArtAssets/Scenes/Login",
			"Assets/_Project/ArtAssets/Scenes/Loading",
			"Assets/_Project/ArtAssets/Scenes/CreateRole",

			"Assets/_Project/ArtAssets/Scenes/House",

			"Assets/_Project/ArtAssets/Scenes/MainCity",
			"Assets/_Project/ArtAssets/Scenes/AllianceCity",

			"Assets/_Project/ArtAssets/Scenes/PVE",
			"Assets/_Project/ArtAssets/Scenes/PVP",

			"Assets/_Project/ArtAssets/Scenes/Carriage",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Scenes_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                               null,
			                               ".unity",
			                               p_build_target,
			                               true,
			                               false );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Scenes_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );

		{
			AssetDatabase.Refresh();
		}
	}
	
	#endregion



	#region Build Data

	/// xml&txt Under "Resources/_Data".
	public static void Build_Data( BuildTarget p_build_target ){
		Debug.Log( "Build_Data()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_Data.Begin ------",
		                     false,
		                     null,
		                     p_build_target );
		
		string[] t_process_path = {
			"Assets/Resources/_Data",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_One_Bundle_Deeply( t_process_path[ i ],
											null,
			                    			".xml#.txt",
											p_build_target,
											false );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Data_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	#endregion



	#region Build Sound

	/// ogg&mp3&wav Under "Resources/Sounds".
	public static void Build_Sounds( BuildTarget p_build_target ){
		Debug.Log( "Build_Sounds()" );

		// reset for dependence
		ResetDependenceLevel( "------ Build_Sound.Begin ------",
		                     false,
		                     null,
		                     p_build_target );
		
		string[] t_process_path = {
			"Assets/Resources/Sounds",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_One_Bundle_Deeply( t_process_path[ i ],
			                               null,
			                               ".ogg#.mp3#.wav",
			                               p_build_target,
			                               false );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Sound_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}
	
	#endregion



	#region Build UI's Image bundles

	/// Images Under "Resources/_UIs".
	public static void Build_UI_Image_Bundles( BuildTarget p_build_target ){
		Debug.Log( "Build_UI_Image_Bundles()" );

		// reset for dependence
		ResetDependenceLevel( "------ Debug_Build_UI_Image_Bundles.Begin ------",
		                     false,
		                     null,
		                     p_build_target );

		string[] t_process_path = {
			"Assets/Resources/_UIs",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                          ".png#.jpg#.jpeg",
			                          p_build_target,
			                          false );
		}

		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Ui_Images_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	// images not under _UIs.
	public static void Build_Unfiled_Images( BuildTarget p_build_target ){
		Debug.Log( "Build_Unfiled_Images()" );

		// reset for dependence
		ResetDependenceLevel( "------ Debug_Editor_Build_Unfiled_Images.Begin ------",
		                     false, 
		                     null,
		                     p_build_target );

		string[] t_process_path = {
			"Assets/Resources/Equip_Icon",
			"Assets/Resources/Equips",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                          ".png#.jpg#.jpeg",
			                          p_build_target,
			                          false );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Ui_Unfiled_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	#endregion



	#region Build UIs Prefab Bundle

	/// Prefabs Under "Resources/_UIs".
	public static void Build_UI_Atlas_Prefab_Bundles( BuildTarget p_build_target ){
		Debug.Log( "Build_UI_Atlas_Prefab_Bundles()" );

		// reset for dependence
		ResetDependenceLevel( "------ Debug_Build_UI_Atlas_Prefab_Bundles.Begin ------",
		                     false,
		                     null,
		                     p_build_target );

		// Bundle Base
		{
			Build_UI_Bundle_Base( p_build_target );
		}

		{
			Build_UI_Common_Atlases( p_build_target );
			
			Build_UI_Prefabs( p_build_target );
		}
		
		ResetDependenceLevel( "Pop Dependencies Total Level: " + m_dependence_level,
		                     true,
		                     Bundle_Loader.Ui_Common_Config_Path_Name + Bundle_Loader.Config_Path_Ext,
		                     p_build_target );
	}

	// Build Bundle Base
	public static void Build_UI_Bundle_Base( BuildTarget p_build_target ){
		for( int i = 0; i < CONFIG_UI_BUNDLE_BASE.Length; i++ ){
			string t_path = "Assets/" + CONFIG_UI_BUNDLE_BASE[ i ];
			
			Build_Dir_To_Multi_Bundle_Deeply( t_path,
			                                 ".prefab",
			                                 p_build_target,
			                                 true );
		}
	}

	// Build Common Atlas
	public static void Build_UI_Common_Atlases( BuildTarget p_build_target ){
		string[] t_exc_path = {
			"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict"
		};
		
		// combine all dicts to one
		{
			Build_Dir_To_One_Bundle_Deeply( t_exc_path[ 0 ], 
			                               ".prefab",
			                               p_build_target,
			                               true );
		}

		string[] t_process_path = {
			"Assets/_Project/ArtAssets/UIs/_CommonAtlas/Atlases",
			"Assets/_Project/ArtAssets/Fonts",
		};
		
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_Multi_Bundle_Deeply( t_process_path[ i ],
			                                 t_exc_path,
			                          		".prefab",
			                          		p_build_target,
			                          		true );
		}
	}

	// Build UI Prefabs
	public static void Build_UI_Prefabs( BuildTarget p_build_target ){
		string[] t_exc_path = {
			"Resources/_UIs/_CommonAtlas",
			"Resources/Global",
		};

		// process common UI
		{
			for( int i = 0; i < t_exc_path.Length; i++ ){
				Build_Dir_To_One_Bundle_Deeply( t_exc_path[ i ], 
					null,
					".prefab",
					p_build_target,
					true );
			}
		}

		string[] t_process_path = {
			"Resources/_UIs",
		};

		// process other folders
		for( int i = 0; i < t_process_path.Length; i++ ){
			Build_Dir_To_One_Bundle_Deeply( t_process_path[ i ], 
			                               t_exc_path, 
			                               ".prefab",
			                               p_build_target,
			                               true );
		}
	}

	#endregion


	
	#region Build Config
		
	/*
	 * Params:
	 * 1.p_streaming_relative_path:		"UIResources/MemoryTrace/MemoryTrace";
	 */
	private static void Build_Bundle_Dependence_Config( string p_streaming_relative_path, UnityEngine.Object[] p_assets, string[] p_levels ){
		// update stack
		{
			string t_peek = (string)m_bundle_stack.Peek();

			if( string.IsNullOrEmpty( t_peek ) ){
				t_peek = p_streaming_relative_path;
			}
			else{
				t_peek = t_peek + Bundle_Loader.CONFIG_DEPENDENCE_SPLITTER_TAG + p_streaming_relative_path;
			}

			m_bundle_stack.Pop();
			
			// update json.dependence
			if( m_bundle_stack.Count >= 1 ){
				string t_peek_peek = (string)m_bundle_stack.Peek();
				
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_TREE_TAG ] = t_peek_peek;
			}
			else{
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_TREE_TAG ] = "";
			}
			
			// update dependence level
			{
				m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_LEVEL_TAG ] = m_dependence_level + "";
			}
			
			m_bundle_stack.Push( t_peek );
		}
		
		// update json.items
		{
#if BUNDLE_ITEMS
			Build_Bundle_Items_Config( p_streaming_relative_path, p_assets, p_levels );
#endif

			Build_Assets_Dependence_Config( p_streaming_relative_path, p_assets, p_levels );
		}
	}

	/*
	 * Params:
	 * 1.p_streaming_relative_path:		"UIResources/MemoryTrace/MemoryTrace";
	 */
	private static void Build_Bundle_Items_Config( string p_streaming_relative_path, UnityEngine.Object[] p_assets, string[] p_levels ){
		if( p_assets == null ){
			for( int i = 0; i< p_levels.Length; i++ ){
				string t_asset_path_u3d = p_levels[ i ];
				
				string t_asset_path = StringHelper.RemovePrefix( t_asset_path_u3d, "Assets/" );
				
				#if ITEM_NAMES
				string t_name = t_asset_path.Substring( t_asset_path.LastIndexOf( "/" ) + 1 );
				
				t_name = t_name.Substring( 0, t_name.LastIndexOf( '.' ) );
				
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ CONFIG_RES_NAME_TAG ] = t_name;
				#endif			
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ Bundle_Loader.CONFIG_ASSET_PATH_TAG ] = t_asset_path.Substring( 0, t_asset_path.LastIndexOf( '.' ) );
				#if ITEM_EXTS		
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ CONFIG_RES_EXT_TAG ] = t_asset_path.Substring( t_asset_path.LastIndexOf( '.' ) + 1 );
				#endif
			}

			return;
		}
		else{
			for( int i = 0; i< p_assets.Length; i++ ){
				string t_asset_path_u3d = AssetDatabase.GetAssetPath( p_assets[ i ] );
				
				string t_asset_path = StringHelper.RemovePrefix( t_asset_path_u3d, "Assets/" );
				
				#if ITEM_NAMES
				string t_name = t_asset_path.Substring( t_asset_path.LastIndexOf( "/" ) + 1 );
				
				t_name = t_name.Substring( 0, t_name.LastIndexOf( '.' ) );
				
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ CONFIG_RES_NAME_TAG ] = t_name;
				#endif			
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ Bundle_Loader.CONFIG_ASSET_PATH_TAG ] = t_asset_path.Substring( 0, t_asset_path.LastIndexOf( '.' ) );
				#if ITEM_EXTS		
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_ITEMS_TAG ][ i ][ CONFIG_RES_EXT_TAG ] = t_asset_path.Substring( t_asset_path.LastIndexOf( '.' ) + 1 );
				#endif
			}
		}
	}

	/**
	 * Params:
	 * 1.p_streaming_relative_path:		"UIResources/MemoryTrace/MemoryTrace";
	 * 2.p_assets[ 0 ]:					"Assets/Resources/MemoryTrace/GUI.prefab";
	 */
	private static void Build_Assets_Dependence_Config( string p_streaming_relative_path, UnityEngine.Object[] p_assets, string[] p_levels ){
		if( p_assets == null ){
			// building levels
			Reset_Dependence_Asset_Count();
			
			List<string> t_contained_list = new List<string>();
			
			for( int i = 0; i< p_levels.Length; i++ ){
				string t_asset_path_u3d = p_levels[ i ];
				
				// update json.dependencies
				{
					string[] t_assets_pathes = new string[ 1 ];
					
					t_assets_pathes[ 0 ] = t_asset_path_u3d;
					
					string[] t_dependence_path = AssetDatabase.GetDependencies( t_assets_pathes );
					
					for( int j = 0; j < t_dependence_path.Length; j++ ){
						UnityEngine.Object t_dependence_obj = AssetDatabase.LoadMainAssetAtPath( t_dependence_path[ j ] );
						
						if( Is_Recordable_Object( t_dependence_obj, t_dependence_path[ j ] ) ){
							string t_dependence_asset_path = StringHelper.RemovePrefix( t_dependence_path[ j ], "Assets/" );
							
							Build_D_O_Detail_Config( p_streaming_relative_path, 
							                        t_asset_path_u3d,
							                        t_dependence_asset_path, 
							                        t_contained_list );
						}
					}
				}
			}
		}
		else{
			// building assets
			Reset_Dependence_Asset_Count();
			
			List<string> t_contained_list = new List<string>();
			
			for( int i = 0; i< p_assets.Length; i++ ){
				string t_asset_path_u3d = AssetDatabase.GetAssetPath( p_assets[ i ] );
				
				// update json.dependencies
				{
					string[] t_assets_pathes = new string[ 1 ];
					
					t_assets_pathes[ 0 ] = t_asset_path_u3d;
					
					string[] t_dependence_path = AssetDatabase.GetDependencies( t_assets_pathes );
					
					for( int j = 0; j < t_dependence_path.Length; j++ ){
						UnityEngine.Object t_dependence_obj = AssetDatabase.LoadMainAssetAtPath( t_dependence_path[ j ] );
						
						if( Is_Recordable_Object( t_dependence_obj, t_dependence_path[ j ] ) ){
							string t_dependence_asset_path = StringHelper.RemovePrefix( t_dependence_path[ j ], "Assets/" );
							
							Build_D_O_Detail_Config( p_streaming_relative_path, 
							                        t_asset_path_u3d,
							                        t_dependence_asset_path, 
							                        t_contained_list );
						}
					}
				}
			}
		}
	}
	
	/**	Desc:
	 * Dependence Optimized.
	 * CONFIG_DEPENDENCE_ASSET_TAG will cotnain repeated asset.
	 * CONFIG_DEPENDENCE_OPTIMIZED_TAG will not.
	 * 
	 * Params:
	 * 1.p_streaming_relative_path:		"UIResources/MemoryTrace/MemoryTrace";
	 * 2.p_dependence_asset_path:		"Resources/MemoryTrace/GUI.prefab";
	 */
	private static void Build_D_O_Detail_Config( string p_streaming_relative_path, string p_root_asset_path, string p_dependence_asset_path, List<string> p_contained_list ){
		string t_ancestor_bundle = Get_Ancestor_Pos( p_streaming_relative_path, p_dependence_asset_path );
		
		if( t_ancestor_bundle != "" && t_ancestor_bundle != p_streaming_relative_path ){
			bool t_will_add = true;

			// assets in ancestor
			{
				#if DEPENDENCE_ASSETS
				string t_key = CONFIG_ASSET_PATH_EXT_TAG;

				for( int i = 0; i < m_dependence_asset_count; i++ ){
					JSONNode p_json = m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_ASSET_TAG ][ i ];

					string p_key = t_key;

					string p_value = p_dependence_asset_path;

					string p_json_value = p_json[ p_key ];

					if( p_json_value == p_value ){
						t_will_add = false;

						break;
					}

//					if( JSONContainsPair( 
//							m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_ASSET_TAG ][ i ],
//							t_key,
//							p_dependence_asset_path ) ){
//						t_will_add = false;
//
//						break;
//					}
				}

				if( t_will_add ){
					m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_ASSET_TAG ][ m_dependence_asset_count ][ CONFIG_ASSET_PATH_EXT_TAG ] = p_dependence_asset_path;
					
					m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_ASSET_TAG ][ m_dependence_asset_count ][ CONFIG_DEPENDENCE_ROOT_ASSET_TAG ] = p_root_asset_path;

					m_cur_bundle_json[ p_streaming_relative_path ][ CONFIG_DEPENDENCE_ASSET_TAG ][ m_dependence_asset_count ][ CONFIG_DEPENDENCE_IN_ASSET_TAG ] = t_ancestor_bundle;

					m_dependence_asset_count++;
				}

				#endif

			}
			
			// build dependence-optimized
			if( t_will_add ){
				string t_cur_d_o = m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_OPTIMIZED_TAG ];
				
				// append or create
				if( string.IsNullOrEmpty( t_cur_d_o ) ){
					m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_OPTIMIZED_TAG ] = t_ancestor_bundle;
				}
				else if( !t_cur_d_o.Contains( t_ancestor_bundle ) ){
					m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_OPTIMIZED_TAG ] = t_cur_d_o + Bundle_Loader.CONFIG_DEPENDENCE_SPLITTER_TAG + t_ancestor_bundle;
				}
			}
		}
		else{
			// assets in self
			if( !p_contained_list.Contains( p_dependence_asset_path ) ){
				m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_CONTAINED_ASSET_TAG ][ m_contained_asset_count ][ CONFIG_ASSET_PATH_EXT_TAG ] = p_dependence_asset_path;
				
				m_contained_asset_count++;
				
				p_contained_list.Add( p_dependence_asset_path );
			}
		}
	}

	private static void Update_Necessary_Keys( BuildTarget p_build_target ){
		{
			AssetDatabase.Refresh();
		}

#if CONTAINED_ASSETS

#else
		{
			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				m_cur_bundle_json[ t_key ].Remove( Bundle_Loader.CONFIG_CONTAINED_ASSET_TAG );
			}
		}
#endif

#if DEPENDENCE_TREE

#else
		{
			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				m_cur_bundle_json[ t_key ].Remove( Bundle_Loader.CONFIG_DEPENDENCE_TREE_TAG );
			}
		}
#endif

#if OPTIMIZE_D_O
		{
			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				string t_d_o = m_cur_bundle_json[ t_key ][ Bundle_Loader.CONFIG_DEPENDENCE_OPTIMIZED_TAG ];
				
				// skip if not exist
				if( string.IsNullOrEmpty( t_d_o ) ){
					continue;
				}
				
				char[] t_splitter = { Bundle_Loader.CONFIG_DEPENDENCE_SPLITTER_TAG };
				
				string[] t_d_o_items = t_d_o.Split(  t_splitter );
				
				string t_d_o_short = "";
				
				for( int j = 0; j < t_d_o_items.Length; j++ ){
					t_d_o_items[ j ] = t_d_o_items[ j ].Substring( t_d_o_items[ j ].LastIndexOf( '/' ) + 1 );
					
					if( string.IsNullOrEmpty( t_d_o_short ) ){
						t_d_o_short = t_d_o_items[ j ];
					}
					else{
						t_d_o_short = t_d_o_short + Bundle_Loader.CONFIG_DEPENDENCE_SPLITTER_TAG + t_d_o_items[ j ];
					}
				}
				
				m_cur_bundle_json[ t_key ][ Bundle_Loader.CONFIG_DEPENDENCE_OPTIMIZED_TAG ] = t_d_o_short;
			}
		}
#endif

#if CHECK_HASH
		{
			List<string> t_hashes = new List<string>();

			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				string t_hash = t_key.Substring( t_key.LastIndexOf( CONFIG_HASH_SPLITTER_TAG ) +
				                CONFIG_HASH_SPLITTER_TAG.Length );

				if( t_hashes.Contains( t_hash ) ){
					Debug.LogError( "Error, hash already contained: " + t_hash + " - " + t_key );
				}
				else{
					t_hashes.Add( t_hash );
				}
			}
		}
#endif

		// file info
		{
			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				string t_full_path = GetStreamingFullPath_WithRelativePath( t_key, p_build_target );

				// md5
				{
					string t_hash = GetMd5Hash( t_full_path );
					
					m_cur_bundle_json[ t_key ][ CONFIG_MD5_TAG ] = t_hash;
				}

				// len
				{
					int t_len = FileHelper.GetFileSize( t_full_path );

					m_cur_bundle_json[ t_key ][ CONFIG_FILE_LEN_TAG ] = t_len.ToString();
				}
			}
		}

		// all bundles
		{
			int t_count = m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ].Count;

			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );
				
				m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_count + i ][ CONFIG_BUNDLE_LIST_ITEM_KEY_TAG ] = t_key;

				m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_count + i ][ CONFIG_BUNDLE_LIST_ITEM_SIZE_TAG ] = m_cur_bundle_json[ t_key ][ CONFIG_FILE_LEN_TAG ];

				m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_count + i ][ CONFIG_BUNDLE_LIST_ITEM_MD5_TAG ] = m_cur_bundle_json[ t_key ][ CONFIG_MD5_TAG ];

				m_all_bundle_json[ CONFIG_BUNDLE_LIST_ITEMS_TAG ][ t_count + i ][ CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG ] = 0 + "";
			}
		}

		{
			for( int i = 0; i < m_cur_bundle_json.Count; i++ ){
				string t_key = m_cur_bundle_json.GetKey( i );

#if REMOVE_LEVELS
				m_cur_bundle_json[ t_key ].Remove( CONFIG_DEPENDENCE_LEVEL_TAG );
#endif

#if REMOVE_MD5
				m_cur_bundle_json[ t_key ].Remove( CONFIG_MD5_TAG );
#endif

#if REMOVE_FILE_SIZE
				m_cur_bundle_json[ t_key ].Remove( CONFIG_FILE_LEN_TAG );
#endif
			}
		}
	}
	
	private static void Build_Config_File( string p_config_relative_path_name, BuildTarget p_build_target ){
		if( string.IsNullOrEmpty( p_config_relative_path_name ) ){
			return;
		}

		string t_config_path_name = GetStreamingFullPath_WithRelativePath( p_config_relative_path_name, p_build_target );

		FileStream t_file_stream = new FileStream( t_config_path_name,
		                                          FileMode.Create );
		
		StreamWriter t_stream_writer = new StreamWriter(
			t_file_stream,
			Encoding.Default );
		
		t_stream_writer.Write( m_cur_bundle_json.ToString( "" ) );
		
		t_stream_writer.Close();
		
		t_file_stream.Close();
	}

	#endregion



	#region Build Config Utility

	private static int m_dependence_asset_count = 0;

	private static int m_contained_asset_count = 0;

	private static void Reset_Dependence_Asset_Count(){
		m_dependence_asset_count = 0;

		m_contained_asset_count = 0;
	}

	/** Desc:
	 * filter out all essential asset.
	 */
	private static bool Is_Recordable_Object( UnityEngine.Object p_object, string p_asset_path ){
//		Type t_type = p_object.GetType();

//		if( p_object.GetType() == typeof( UnityEditor.MonoScript ) ){
//			Debug.Log( "Filter Out: " + p_object.GetType() + " - " + p_asset_path );
//
//			return false;
//		}

//		if( p_object.GetType() == typeof( Shader ) ){
//			Debug.Log( "Filter Out: " + p_object.GetType() + " - " + p_asset_path );
//		
//			return false;
//		}

		return true;
	}

	/** Desc:
	 * Get Ancestor pos.
	 * 
	 * Params:
	 * 1.p_streaming_relative_path:		"UIResources/MemoryTrace/MemoryTrace";
	 * 2.p_asset_relative_path:			"Resources/MemoryTrace/GUI.prefab";
	 * 
	 * return:							"Android/Resources/_UIs/MainCity/UIBag/Prefabs/Prefabs_-26";
	 */
	private static string Get_Ancestor_Pos( string p_streaming_relative_path, string p_asset_path ){
		string t_dependence_bundle_path = m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_DEPENDENCE_TREE_TAG ];

		char[] t_splitter = { Bundle_Loader.CONFIG_DEPENDENCE_SPLITTER_TAG };
		
		string[] t_depends = t_dependence_bundle_path.Split( t_splitter );
		
		string t_ancestor_pos = "";

		// in ancestor
		if( t_depends != null && t_depends.Length > 0 ){
			string t_dependence_0 = t_depends[ 0 ];

			if( t_depends.Length > 1 ){
				Debug.LogError( "Error, Ancestor > 1." );
			}

			// just 0
			if( !string.IsNullOrEmpty( t_dependence_0 ) ){
				t_ancestor_pos = Get_Ancestor_Pos( t_dependence_0, p_asset_path );
			}
		}

		string t_self = "";

		// in self
		{
			if( Is_In_Bundle( p_streaming_relative_path, p_asset_path ) ){
				t_self = p_streaming_relative_path;
			}
		}

		if( !string.IsNullOrEmpty( t_ancestor_pos ) ){
			return t_ancestor_pos;
		}
		else{
			return t_self;
		}
	}

	private static bool Is_In_Bundle( string p_streaming_relative_path, string p_asset_path ){
		JSONNode t_node = m_cur_bundle_json[ p_streaming_relative_path ][ Bundle_Loader.CONFIG_CONTAINED_ASSET_TAG ];

		for( int i = 0; i < t_node.Count; i++ ){
			string t_res_path_with_ext = t_node[ i ][ CONFIG_ASSET_PATH_EXT_TAG ];

			if( p_asset_path == t_res_path_with_ext ){
				return true;
			}
		}

		return false;
	}

	#endregion



	#region Path

	/** Return:	iOS/Android/Windows
	 */
	public static string GetPlatformTag( BuildTarget p_build_target ){
		string t_platform = "";
		
		if( p_build_target == BuildTarget.Android ){
			t_platform = t_platform + PlatformHelper.GetAndroidTag();
		}
		else if( p_build_target == BuildTarget.iOS ){
			t_platform = t_platform + PlatformHelper.GetiOSTag();
		}
		else if( p_build_target == BuildTarget.StandaloneWindows ){
			t_platform = t_platform + PlatformHelper.GetWindowsTag();
		}
		else{
			Debug.LogError( "TargetPlatform Error: " + p_build_target );
		}
		
		return t_platform;
	}

	/** Params:
	 * 1.p_res_relative_path: 	"Assets/StreamingAssets/PlatformX/UIResources/_MemoryTrace";
	 * 
	 * Return:			"Assets/StreamingAssets/PlatformX/UIResources/MemoryTrace";
	 */
	public static string UpdateBundleRelativePath( string p_relative_path ){
		string t_path = p_relative_path;

		t_path = StringHelper.RemovePrefix( p_relative_path, "/" );

		t_path = StringHelper.AddPrefix( p_relative_path, "/" );

		t_path = t_path.Replace( "/_", "/" );

		t_path = StringHelper.RemovePrefix( t_path, "/" );

		return t_path;
	}

	/** Params:
	 * 1.p_res_relative_path: "UIResources/MemoryTrace";
	 * 
	 * 
	 * Return: "Assets/StreamingAssets/PlatformX/UIResources/MemoryTrace";
	 */
	public static string GetBundleRelativePath_WithRelativePath( string p_res_relative_path, BuildTarget p_build_target ){
		string t_bundle_relative_path = p_res_relative_path;
		
		// Remove Prefix Assets
		{
			if( !t_bundle_relative_path.StartsWith( "/" ) ){
				t_bundle_relative_path = "/" + t_bundle_relative_path;
			}

			t_bundle_relative_path = StringHelper.RemovePrefix( t_bundle_relative_path, "/Assets" );

			t_bundle_relative_path = StringHelper.RemovePrefix( t_bundle_relative_path, "/StreamingAssets" );

			t_bundle_relative_path = StringHelper.RemovePrefix( t_bundle_relative_path, "/" + PlatformHelper.GetAndroidTag() );

			t_bundle_relative_path = StringHelper.RemovePrefix( t_bundle_relative_path, "/" + PlatformHelper.GetiOSTag() );

			t_bundle_relative_path = StringHelper.RemovePrefix( t_bundle_relative_path, "/" + PlatformHelper.GetWindowsTag() );
		}
		
		string t_prefix = "Assets/StreamingAssets";
		
		// Add platform prefix
		{
			string t_platform = "/" + GetPlatformTag( p_build_target );
			
			t_prefix = t_prefix + t_platform;
		}
		
		// add bundle prefix
		{
			
			t_bundle_relative_path = t_prefix + t_bundle_relative_path;
		}

		{
			t_bundle_relative_path = UpdateBundleRelativePath( t_bundle_relative_path );
		}
		
		return t_bundle_relative_path;
	}

	/** Params:
	 * 1.p_res_relative_path: 	"Resources/_UIs/_CommonAtlas/Common";
	 * 
	 * Return:			"E:/WorkSpace_External/DynastyMobile_2014/Assets/StreamingAssets/Android/Resources/_UIs/_CommonAtlas/Common";
	 */
	public static string GetStreamingFullPath_WithRelativePath( string p_res_relative_path, BuildTarget p_build_target ){
		string t_bundle_relative_path = p_res_relative_path;
		
		// Remove Prefix Assets
		{
			if( !t_bundle_relative_path.StartsWith( "/" ) ){
				t_bundle_relative_path = "/" + t_bundle_relative_path;
			}
		}
		
		string t_prefix = Application.streamingAssetsPath;
		
		// Add platform prefix
		{
			t_prefix = t_prefix + "/";

			t_prefix = t_prefix + GetPlatformTag( p_build_target ) + "/";
		}

		string t_full_path = t_prefix + t_bundle_relative_path;
		
		return t_full_path;
	}

	/** Params:
	 * 1.p_res_relative_path: 	"Resources/_UIs/_CommonAtlas/Common";
	 * 
	 * Return:			"E:/WorkSpace_External/DynastyMobile_2014/Assets/Resources/_UIs/_CommonAtlas/Common";
	 */
	public static string GetFullPath_WithRelativePath( string p_res_relative_path ){
		string t_bundle_relative_path = p_res_relative_path;

		// Remove Prefix Assets
		{
			if( !t_bundle_relative_path.StartsWith( "/" ) ){
				t_bundle_relative_path = "/" + t_bundle_relative_path;
			}
		}
		
		string t_prefix = Application.dataPath;
		
		// Add platform prefix
		{
			t_prefix = t_prefix + "/";
		}
		
		string t_full_path = t_prefix + t_bundle_relative_path;
		
		return t_full_path;
	}

	#endregion



	#region Build Asset Bundles
	            
	/** Desc: Build res direct under relative path into a bundle. 
	 * 
	 * Params:
	 * 1.relative path: 	"_Project/ArtAssets/UIs", "Resources/_UIs"锛嬌	 
	 * 2.p_assets_path:		"_Temp/_Project/ArtAssets/UIs", get assets from this path.
	 * 3.p_surfixes:		".jpg#.prefab#.png",
	 * 4.p_build_target:	BuildTarget.iPhone,
	 */
	public static void Build_Dir_To_One_Bundle( string p_relative_path, string p_assets_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, string p_bundle_surfix ){
		string t_bundle_relative_path = p_relative_path;
		
		// add bundle prefix: "Assets/StreamingAssets/Platform"
		{
			t_bundle_relative_path = GetBundleRelativePath_WithRelativePath( t_bundle_relative_path,
			                                                                p_build_target );
		}
		
		List<string> t_files_relative_path = Get_All_Assets_Relative_Path ( p_assets_path, p_surfixes );
		
		//		Debug.Log ( "Assets.Relative.Path.Count: " + t_files_relative_path.Count + " - " + p_relative_path );
		
		List<UnityEngine.Object> t_list = Get_All_Assets_Objects ( t_files_relative_path );
		
		//		Debug.Log ( "UnityEngine.Object.Count: " + t_list.Count + " - " + p_relative_path );
		
		if( t_list.Count == 0 ){
			//			return;
		}
		
		// check folder
		{
			string t_bundle_full_path = PathHelper.GetFullPath_WithRelativePath( t_bundle_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_bundle_full_path );
			
			if( !t_dir.Exists ){
				t_dir.Create();
			}
		}
		
		string t_bundle_file_relative_path_name = t_bundle_relative_path + 
			( t_bundle_relative_path.EndsWith( "/" ) ? "" : "/" ) + 
				t_bundle_relative_path.Substring( t_bundle_relative_path.LastIndexOf( '/' ) + 1 );
		
		//		Debug.Log ( "Bundle File Path: " + t_bundle_file_relative_path_name );
		
		Build_PushAssetDependencies();
		
		Build_AssetBundle( null,
		                  t_list.ToArray(),
		                  t_bundle_file_relative_path_name,
		                  p_bundle_surfix,
		                  p_build_target,
		                  m_build_asset_bundle_options );
		
		if( !p_push_dependence ){
			Build_PopAssetDependencies();
		}
	}
		
	/** Desc: Build res direct under relative path into a bundle. 
	 * 
	 * Params:
	 * 1.relative path: 	"_Project/ArtAssets/UIs", "Resources/_UIs"锛嬌	 * 2.p_surfixes:		".jpg#.prefab#.png",
	 * 3.p_build_target:	BuildTarget.iPhone,
	 */
	public static void Build_Dir_To_One_Bundle( string p_relative_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, string p_bundle_surfix ){
		Build_Dir_To_One_Bundle( p_relative_path, p_relative_path, p_surfixes, p_build_target, p_push_dependence, p_bundle_surfix );
	}

	public static void Build_Dir_To_One_Bundle_Deeply( string p_relative_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, bool p_check_sub_folders = true ){
		Build_Dir_To_One_Bundle_Deeply( p_relative_path, null, p_surfixes, p_build_target, p_push_dependence, p_check_sub_folders );
	}
		
	/** Desc:
	 * Build assets into one bundle.
	 * 
	 * Params:
	 * 1.p_relative_path: 	"_Project/ArtAssets/UIs", "Resources/_UIs";
	 * 2.p_exc_path.item: 	"_Project/ArtAssets/UIs/_CommonAtlas";
	 * 3.2.p_surfixes:		".jpg#.prefab#.png";
	 */
	public static void Build_Dir_To_One_Bundle_Deeply( string p_relative_path, string[] p_exc_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, bool p_check_sub_folders = true ){
		string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );

		if( p_check_sub_folders ){
			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
			
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );
				
				if( p_exc_path != null ){
					bool t_will_process = true;
					
					for( int j = 0; j < p_exc_path.Length; j++ ){
						if( t_sub_dir_path.Contains( p_exc_path[ j ] ) ){
							t_will_process = false;
							
							break;
						}
					}
					
					if( !t_will_process ){
						continue;
					}
				}
				
				string p_relative_process_path = t_sub_dir_path.Substring( Application.dataPath.Length + 1 );
				
				Build_Dir_To_One_Bundle_Deeply( p_relative_process_path, 
				                               p_exc_path,
				                               p_surfixes,
				                               p_build_target,
				                               p_push_dependence,
				                               p_check_sub_folders );
			}
		}
		
		// turn it to "/_Project/ArtAssets/UIs/_CommonAtlas"
		{
			// add first '/'
			p_relative_path = StringHelper.AddPrefix( p_relative_path, "/" );
			
			// remove '/'
			while( p_relative_path.EndsWith( "/" ) ){
				p_relative_path = p_relative_path.Substring( 0, p_relative_path.Length - 1 );
			}
			
			// Build Prefabs
			Build_Dir_To_One_Bundle( p_relative_path, 
			                        p_surfixes,
			                        p_build_target,
			                        p_push_dependence,
			                        null );
		}
	}

	/** Desc: Build Asset into a bundle. 
	 * 
	 * Params:
	 * 1.relative path: 	"UIResources/MemoryTrace",
	   2.p_file_name:		"Com_Atlas_0.prefab",
	   3.p_build_target:	BuildTarget.iPhone,
	*/
	public static void Build_One_Asset_To_One_Bundle( string p_relative_path, string p_file_name, BuildTarget p_build_target, bool p_push_dependence ){
		string t_bundle_relative_path = p_relative_path;
		
		// add source prefix: "/Assets"
		{
			while( p_relative_path.StartsWith( "/" ) ){
				p_relative_path = p_relative_path.Substring( 1 );
			}
			
			if( !p_relative_path.StartsWith( "Assets" ) ){
				p_relative_path = "Assets/" + p_relative_path;
			}
		}
		
		// add bundle prefix: "Assets/StreamingAssets/Platform"
		{
			t_bundle_relative_path = GetBundleRelativePath_WithRelativePath( t_bundle_relative_path,
			                                                                p_build_target );
		}

		// check folder
		{
			string t_dir_path = PathHelper.GetFullPath_WithRelativePath( t_bundle_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_dir_path );
			
			if( !t_dir.Exists ){
				t_dir.Create();
			}
		}
		
		string t_file_relative_path = p_relative_path + "/" + p_file_name ;
		
		UnityEngine.Object t_obj = AssetDatabase.LoadMainAssetAtPath( t_file_relative_path );
		
		List<UnityEngine.Object> t_assets_list = new List<UnityEngine.Object>();
		
		t_assets_list.Add( t_obj );

		string t_bundle_file_relative_path_name = t_bundle_relative_path + ( t_bundle_relative_path.EndsWith( "/" ) ? "" : "/" );
		
		if( p_file_name.LastIndexOf( '.' ) > 0 ){
			t_bundle_file_relative_path_name = t_bundle_file_relative_path_name + 
				p_file_name.Substring( 0, p_file_name.LastIndexOf( '.' ) ) +
					"_" + p_file_name.Substring( p_file_name.LastIndexOf( '.' ) + 1 );
		}
		else{
			t_bundle_file_relative_path_name = t_bundle_file_relative_path_name + p_file_name;
		}
		
		//Debug.Log ( "bundle file path: " + t_bundle_file_relative_path_name );
		
		Build_PushAssetDependencies();
		
		Build_AssetBundle( null,
		              		t_assets_list.ToArray(),
		             		t_bundle_file_relative_path_name,
		           			null,
		          			p_build_target,
		             		m_build_asset_bundle_options );
		
		if( !p_push_dependence ){
			Build_PopAssetDependencies();
		}
	}

	public static void Build_Dir_To_Multi_Bundle_Deeply( string p_relative_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, bool p_check_sub_folders = true ){
		Build_Dir_To_Multi_Bundle_Deeply( p_relative_path, null, p_surfixes, p_build_target, p_push_dependence, p_check_sub_folders );
	}

	/** Desc: 
	 * Build each asset int one bundle.
	 * 
	 * Params:
	 * 1.p_relative_path: 	"Assets/_Project/ArtAssets/UIs/_CommonAtlas/Atlases",
	   2.p_surfixes:		".jpg#.prefab#.png",
	   3.p_build_target:	BuildTarget.iPhone,
	*/
	public static void Build_Dir_To_Multi_Bundle_Deeply( string p_relative_path, string[] p_exc_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, bool p_check_sub_folders = true ){
		if( p_check_sub_folders ){
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
			
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );

				if( p_exc_path != null ){
					bool t_will_process = true;
					
					for( int j = 0; j < p_exc_path.Length; j++ ){
						if( t_sub_dir_path.Contains( p_exc_path[ j ] ) ){
							t_will_process = false;
							
							break;
						}
					}
					
					if( !t_will_process ){
						continue;
					}
				}

				string t_relative_process_path = t_sub_dir_path.Substring( Application.dataPath.Length + 1 );
				
				Build_Dir_To_Multi_Bundle_Deeply( t_relative_process_path, 
												p_exc_path,
				                        		p_surfixes,
				                        		p_build_target,
				                 				p_push_dependence,
				                                p_check_sub_folders );
			}
		}

		// remove relative prefix: "Assets". "/Assets"
		{
			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "/" );

			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "Assets" );

			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "/" );
		}

		List<string> t_files_path_name = Get_All_Assets_Relative_Path ( p_relative_path, p_surfixes );
		
		List<UnityEngine.Object> t_list = Get_All_Assets_Objects ( t_files_path_name );

		if( t_list.Count == 0 ){
			return;
		}

		// bundle path
		string t_bundle_relative_path = p_relative_path;

		// check folder
		{
			t_bundle_relative_path = GetBundleRelativePath_WithRelativePath( t_bundle_relative_path,
																				p_build_target );

			string t_bundle_full_path = PathHelper.GetFullPath_WithRelativePath( t_bundle_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_bundle_full_path );
			
			if( !t_dir.Exists ){
				t_dir.Create();
			}
		}

//		string t_bundle_file_path = t_bundle_path + 
//			( t_bundle_path.EndsWith( "/" ) ? "" : "/" ) + 
//				t_bundle_path.Substring( t_bundle_path.LastIndexOf( '/' ) + 1 );
		
		//Debug.Log ( "Multi File Path: " + p_relative_path );

		for( int i = 0; i < t_files_path_name.Count; i++ ){
			string t_file_name = t_files_path_name[ i ];

			if( !string.IsNullOrEmpty( p_surfixes ) ){
				if( !StringHelper.IsEndWith( t_file_name, p_surfixes ) ){
					continue;
				}
			}

			Build_One_Asset_To_One_Bundle( p_relative_path, 
				                      t_file_name.Substring( ( t_file_name.LastIndexOf( '/' ) + 1 ) ),
				                      p_build_target,
				                      p_push_dependence );
		}
	}

	#endregion



	#region Build Scene Bundles

	/** Desc: 
	 * Build each Scene int one bundle.
	 * 
	 * Params:
	 * 1.p_relative_path: 	"Assets/_Project/ArtAssets/Scenes/Login",
	   2.p_surfixes:		".unity",
	   3.p_build_target:	BuildTarget.iPhone,
	*/
	public static void Build_Scenes_Dir_To_Multi_Bundle_Deeply( string p_relative_path, string[] p_exc_path, string p_surfixes, BuildTarget p_build_target, bool p_push_dependence, bool p_check_sub_folders = true ){
		if( p_check_sub_folders ){
			string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
			
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );
				
				if( p_exc_path != null ){
					bool t_will_process = true;
					
					for( int j = 0; j < p_exc_path.Length; j++ ){
						if( t_sub_dir_path.Contains( p_exc_path[ j ] ) ){
							t_will_process = false;
							
							break;
						}
					}
					
					if( !t_will_process ){
						continue;
					}
				}
				
				string t_relative_process_path = t_sub_dir_path.Substring( Application.dataPath.Length + 1 );
				
				Build_Scenes_Dir_To_Multi_Bundle_Deeply( t_relative_process_path, 
				                                 p_exc_path,
				                                 p_surfixes,
				                                 p_build_target,
				                                 p_push_dependence,
				                                 p_check_sub_folders );
			}
		}
		
		// remove relative prefix: "Assets". "/Assets"
		{
			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "/" );
			
			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "Assets" );
			
			p_relative_path = StringHelper.RemovePrefix( p_relative_path, "/" );
		}
		
		List<string> t_files_path_name = Get_All_Assets_Relative_Path ( p_relative_path, p_surfixes );
		
		List<UnityEngine.Object> t_list = Get_All_Assets_Objects ( t_files_path_name );
		
		if( t_list.Count == 0 ){
			return;
		}
		
		// bundle path
		string t_bundle_relative_path = p_relative_path;
		
		// check folder
		{
			t_bundle_relative_path = GetBundleRelativePath_WithRelativePath( t_bundle_relative_path,
			                                                                p_build_target );

			string t_bundle_full_path = PathHelper.GetFullPath_WithRelativePath( t_bundle_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_bundle_full_path );
			
			if( !t_dir.Exists ){
				t_dir.Create();
			}
		}

		//		string t_bundle_file_path = t_bundle_path + 
		//			( t_bundle_path.EndsWith( "/" ) ? "" : "/" ) + 
		//				t_bundle_path.Substring( t_bundle_path.LastIndexOf( '/' ) + 1 );
		
		//Debug.Log ( "Multi File Path: " + p_relative_path );
		
		for( int i = 0; i < t_files_path_name.Count; i++ ){
			string t_file_name = t_files_path_name[ i ];

			if( !string.IsNullOrEmpty( p_surfixes ) ){
				if( !StringHelper.IsEndWith( t_file_name, p_surfixes ) ){
					continue;
				}
			}
			
			Build_One_Scene_To_One_Bundle( p_relative_path, 
				                              t_file_name.Substring( ( t_file_name.LastIndexOf( '/' ) + 1 ) ),
				                              p_build_target,
				                              p_push_dependence );
		}
	}

	/** Desc: Build Scene into a bundle. 
	 * 
	 * Params:
	 * 1.relative path: 	"UIResources/MemoryTrace",
	   2.p_file_name:		"Com_Atlas_0.prefab",
	   3.p_build_target:	BuildTarget.iPhone,
	*/
	public static void Build_One_Scene_To_One_Bundle( string p_relative_path, string p_file_name, BuildTarget p_build_target, bool p_push_dependence ){
		string t_bundle_relative_path = p_relative_path;
		
		// add source prefix: "/Assets"
		{
			while( p_relative_path.StartsWith( "/" ) ){
				p_relative_path = p_relative_path.Substring( 1 );
			}
			
			if( !p_relative_path.StartsWith( "Assets" ) ){
				p_relative_path = "Assets/" + p_relative_path;
			}
		}
		
		// add bundle prefix: "Assets/StreamingAssets/Platform"
		{
			t_bundle_relative_path = GetBundleRelativePath_WithRelativePath( t_bundle_relative_path,
			                                                                p_build_target );
		}
		
		// check folder
		{
			string t_dir_path = PathHelper.GetFullPath_WithRelativePath( t_bundle_relative_path );
			
			DirectoryInfo t_dir = new DirectoryInfo( t_dir_path );
			
			if( !t_dir.Exists ){
				t_dir.Create();
			}
		}
		
		string t_file_relative_path = p_relative_path + "/" + p_file_name ;
		
		string[] t_levels = new string[ 1 ];

		t_levels[ 0 ] = t_file_relative_path;

		string t_bundle_file_relative_path_name = t_bundle_relative_path + ( t_bundle_relative_path.EndsWith( "/" ) ? "" : "/" );
		
		if( p_file_name.LastIndexOf( '.' ) > 0 ){
			t_bundle_file_relative_path_name = t_bundle_file_relative_path_name + 
				p_file_name.Substring( 0, p_file_name.LastIndexOf( '.' ) ) +
					"_" + p_file_name.Substring( p_file_name.LastIndexOf( '.' ) + 1 );
		}
		else{
			t_bundle_file_relative_path_name = t_bundle_file_relative_path_name + p_file_name;
		}
		
		//Debug.Log ( "bundle file path: " + t_bundle_file_relative_path_name );
		
		Build_PushAssetDependencies();
		
		Build_One_Scene_Bundle_Final( t_levels,
		                       t_bundle_file_relative_path_name,
		                       null,
		                       p_build_target,
		                       m_build_scene_options );
		
		if( !p_push_dependence ){
			Build_PopAssetDependencies();
		}
	}

	#endregion



	#region Build Utilities

	private static BuildAssetBundleOptions m_build_asset_bundle_options = BuildAssetBundleOptions.CollectDependencies | 
//						BuildAssetBundleOptions.CompleteAssets |
#if CUSTOM_7ZIP_LOAD
						BuildAssetBundleOptions.UncompressedAssetBundle |
#endif
#if DEVELOPMENT_BUILD
						BuildAssetBundleOptions.UncompressedAssetBundle |
#endif
						BuildAssetBundleOptions.DeterministicAssetBundle;

	private static BuildOptions m_build_scene_options = 
#if DEVELOPMENT_BUILD
						BuildOptions.UncompressedAssetBundle |
#endif
			BuildOptions.BuildAdditionalStreamedScenes ;
	
	// 1 - N
	private static int m_dependence_level = 0;

	public static int GetDependenceLevel(){
		return m_dependence_level;
	}

	private static JSONClass m_cur_bundle_json = new JSONClass();

	private static Stack m_bundle_stack = new Stack();


	/** Desc:
	 * Build Asset Bundle, log Dependence&Items.
	 * 
	 * Params:
	 * 1.p_path_name:	"Assets/StreamingAssets/UIResources/MemoryTrace/MemoryTrace";
	 * 
	 */
	private static void Build_AssetBundle( UnityEngine.Object p_main_asset, 
										UnityEngine.Object[] p_assets, 
										string p_path_name, 
										string p_bundle_surfix,
										BuildTarget p_target_platform,
										BuildAssetBundleOptions p_bundle_options = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets ){
		if( string.IsNullOrEmpty( p_bundle_surfix ) ){
			p_path_name = AppendBundlePathName( p_path_name );
		}
		else{
			p_path_name = p_path_name + p_bundle_surfix;
		}

		if (p_assets.Length == 0) {
//			Debug.Log( "No Item Exists." );

			return;
		}

		#if DEBUG_BUILD
//		Debug.Log( "Build_AssetBundle: " + p_path_name + " - " + p_assets.Length );
//
//		for( int i = 0; i < p_assets.Length; i++ ){
//			Debug.Log( "Item " + i + ": " + AssetDatabase.GetAssetPath( p_assets[ i ] ) + " - " + p_assets[ i ].GetType() );
//		}
		#endif

		if( BuildPipeline.BuildAssetBundle( p_main_asset,
		                                   p_assets,
		                                   p_path_name,
		                                   p_bundle_options,
		                                   p_target_platform ) ){
#if CUSTOM_7ZIP_LOAD
			{
				SevenZip.BundleEncoder.EncodeBundleFile( p_path_name );
			}
#endif

			if( m_bundle_stack.Count <= 0 ){
				Debug.LogError( "Stack Error: " + m_bundle_stack.Count );

				return;
			}

			// update bundle config
			{
				string t_streaming_relative_path = p_path_name.Substring( 
						"Assets/StreamingAssets/".Length +
						GetPlatformTag( p_target_platform ).Length +
						1 );

				// check if exist
				{
					IEnumerator<string> t_keys = m_cur_bundle_json.ChildKeys.GetEnumerator();
					
					while( t_keys.MoveNext() ){
						if( t_keys.Current == t_streaming_relative_path ){
							Debug.LogError( "Error, Key Already Exist: " + t_streaming_relative_path );
							
							return;
						}
					}
				}
				
				// update Config
				{
					Build_Bundle_Dependence_Config( t_streaming_relative_path, p_assets, null );
				}
			}
		}
		else{
			Debug.LogError( "Build Fail: " + p_path_name );
		}
	}

	/** Desc:
	 * Build Scene Bundle, log Dependence&Items.
	 * 
	 * Params:
	 * 1.p_path_name:	"Assets/StreamingAssets/UIResources/MemoryTrace/MemoryTrace";
	 * 
	 */
	private static void Build_One_Scene_Bundle_Final( string[] p_scenes, 
	                                  		string p_path_name, 
	                                   		string p_bundle_surfix,
											BuildTarget p_target_platform,
											BuildOptions p_bundle_options = BuildOptions.None ){
		if( string.IsNullOrEmpty( p_bundle_surfix ) ){
			p_path_name = AppendBundlePathName( p_path_name );
		}
		else{
			p_path_name = p_path_name + p_bundle_surfix;
		}

		for( int i = 0; i < p_scenes.Length; i++ ){
			Debug.Log( "Build Scene Bundle: " + p_scenes[ i ] );
		}

		// TODO, lzma
		string t_ret = BuildPipeline.BuildStreamedSceneAssetBundle( p_scenes,
											p_path_name,
											p_target_platform,
		                        			p_bundle_options );

		if( string.IsNullOrEmpty( t_ret ) ){
			if( m_bundle_stack.Count <= 0 ){
				Debug.LogError( "Stack Error: " + m_bundle_stack.Count );
				
				return;
			}
			
			// update bundle config
			{
				string t_streaming_relative_path = p_path_name.Substring( 
				                                                         "Assets/StreamingAssets/".Length +
				                                                         GetPlatformTag( p_target_platform ).Length +
				                                                         1 );
				
				// check if exist
				{
					IEnumerator<string> t_keys = m_cur_bundle_json.ChildKeys.GetEnumerator();
					
					while( t_keys.MoveNext() ){
						if( t_keys.Current == t_streaming_relative_path ){
							Debug.LogError( "Error, Key Already Exist: " + t_streaming_relative_path );
							
							return;
						}
					}
				}
				
				// update Config
				{
					Build_Bundle_Dependence_Config( t_streaming_relative_path, null, p_scenes );
				}
			}
		}
		else{
			Debug.LogError( "Build Fail: " + p_path_name );
		}
	}

	/** Desc:
	 * Append bundle path name with hash.
	 * 
	 * Params:
	 * 1.p_path_name:	"Assets/StreamingAssets/UIResources/MemoryTrace/MemoryTrace";
	 * 
	 * 
	 * Return:
	 * p_path_name_-16BaseHash.
	 * 
	 */
	public static string AppendBundlePathName( string p_path_name ){
		int t_name_hash = p_path_name.GetHashCode();

		string t_name_ext = Convert.ToString( t_name_hash, 16 );

		string t_bundle_path = p_path_name + CONFIG_HASH_SPLITTER_TAG + t_name_ext;

		return t_bundle_path;
	}

	/// Reset Dependence Tree.
	public static void ResetDependenceLevel( string p_log, bool p_build_json, string p_config_relative_path_name, BuildTarget p_build_target ){
		{
			AssetDatabase.Refresh();
		}

//		if( p_log != null ){
//			Debug.Log( "ResetDependenceLevel: " + p_log );
//		}

		// build dependence stack
		if( p_build_json ){
			Update_Necessary_Keys( p_build_target );

			Build_Config_File( p_config_relative_path_name, p_build_target );
		}

		if( m_dependence_level == 0 ){
			// init log
			{
				m_cur_bundle_json = new JSONClass();

				m_bundle_stack.Clear();
			}
		}

		// pop all
		while( m_dependence_level > 0 ){
			Build_PopAssetDependencies();
		}

		m_dependence_level = 0;
	}
	
	private static void Build_PushAssetDependencies(){
		// push dependence
		BuildPipeline.PushAssetDependencies();

		// update level
		{
			m_dependence_level++;
		}

		// update dependence log
		{
			if( m_bundle_stack.Count > 0 ){
				string t_peek = (string)m_bundle_stack.Peek();

				if( t_peek == "" ){
					Debug.LogError( "Dependence Break! " + m_dependence_level );
				}
			}

			m_bundle_stack.Push( "" );
		}
	}
	
	private static void Build_PopAssetDependencies(){
		// pop dependence
		BuildPipeline.PopAssetDependencies();

		// update level
		{
			m_dependence_level--;
		}

		// update dependence log
		{
			if( m_bundle_stack.Count <= 0 ){
				Debug.LogError( "Dependence Empty! " + m_dependence_level );
			}
			else{
//				string t_peek = (string)m_bundle_stack.Peek();
//
//				Debug.Log( "Peek: " + t_peek );
			}

			m_bundle_stack.Pop();
		}
	}

	#endregion
	
	
	
	#region Common Utilities

	/** Desc: Get assets relative path under param dir.
	 * 
	 * Params:
	 * 1.e.g: 		"UIResources/MemoryTrace";
	 * 2.p_surfixes:	".jpg#.prefab#.png";
	 * 
	 * Return:
	 * list.item: Assets/UIResources/MemoryTrace/*.png
	 */
	private static List<string> Get_All_Assets_Relative_Path( string p_relative_dir, string p_surfixes ){
		if( !p_relative_dir.StartsWith( "/" ) ){
			p_relative_dir = "/" + p_relative_dir;
		}

		List<string> t_path_list = new List<string>();

		string t_asset_prefix = "Assets";

		string t_path = PathHelper.GetFullPath_WithRelativePath( p_relative_dir );
		
		//Debug.Log ( "Assets Parent Path: " + t_path );

		DirectoryInfo t_dir = new DirectoryInfo ( t_path );
		
		FileInfo[] t_files = t_dir.GetFiles ();
		
		for( int i = 0; i < t_files.Length; i++ ){
			if( !string.IsNullOrEmpty( p_surfixes ) ){
				if( !StringHelper.IsEndWith( t_files[ i ].Name, p_surfixes ) ){
					continue;
				}
			}

			string t_file_path = t_asset_prefix + p_relative_dir + "/" + t_files[ i ].Name;
		
			UnityEngine.Object t_obj = AssetDatabase.LoadMainAssetAtPath( t_file_path );
			
			if( t_obj != null ){
				t_path_list.Add( AssetDatabase.GetAssetPath( t_obj ) );
			}
		}

		return t_path_list;
	}

	/** Desc: Get UnityEngineObject list.
	 * list.item: Assets/UIResources/MemoryTrace/*.png
	 */
	private static List<UnityEngine.Object> Get_All_Assets_Objects( List<string> p_files ){
		List<UnityEngine.Object> t_assets_list = new List<UnityEngine.Object>();

		for (int i = 0; i < p_files.Count; i++) {
			UnityEngine.Object t_obj = AssetDatabase.LoadMainAssetAtPath( p_files[ i ] );

			t_assets_list.Add( t_obj );
		}

		return t_assets_list;
	}

	/** Desc: Get assets relative path under param dir.
	 * 
	 * Params:
	 * 1.e.g: 		"UIResources/MemoryTrace";
	 * 2.p_surfixes:	".jpg#.prefab#.png";
	 * 
	 * Return:
	 * list.item: Assets/UIResources/MemoryTrace/*.png
	 */
	private static void Delete_Files_Under_Relative_Path( string p_relative_dir, string p_surfixes ){
		if( string.IsNullOrEmpty( p_surfixes ) ){
			Debug.Log( "Error, No Surfix: " + p_surfixes );

			return;
		}

		if( !p_relative_dir.StartsWith( "/" ) ){
			p_relative_dir = "/" + p_relative_dir;
		}

		string t_path = PathHelper.GetFullPath_WithRelativePath( p_relative_dir );
		
		//Debug.Log ( "Assets Parent Path: " + t_path );
		
		DirectoryInfo t_dir = new DirectoryInfo ( t_path );
		
		FileInfo[] t_files = t_dir.GetFiles ();
		
		for( int i = 0; i < t_files.Length; i++ ){
			if( !StringHelper.IsEndWith( t_files[ i ].Name, p_surfixes ) ){
				continue;
			}

			t_files[ i ].Delete();
		}
	}

	private static bool JSONContainsPair( JSONNode p_json, string p_key, string p_value ){
		if( p_json[ p_key ] == p_value ){
			return true;
		}

		return false;
	}

	private static MD5 m_md5 = null;

	private static MD5 GetMd5(){
		if( m_md5 == null ){
			m_md5 = MD5.Create();
		}

		return m_md5;
	}

	/** Desc:
	 * Return MD5 about the bundle.
	 * 
	 * Params:
	 * 1.p_full_path:			"E:/WorkSpace_External/DynastyMobile_2014/Assets/StreamingAssets/Android/Resources/_UIs/_CommonAtlas/Common";
	 */
	private static string GetMd5Hash( string p_full_path ){
		FileInfo t_file_info = new FileInfo( p_full_path );
		
		return GetMd5Hash( t_file_info );
	}

	private static string GetMd5Hash( FileInfo p_file_info ){
		FileStream t_f_s = p_file_info.Open( FileMode.Open );
		
		t_f_s.Position = 0;
		
		byte[] t_data = GetMd5().ComputeHash( t_f_s );
		
		StringBuilder t_builder = new StringBuilder();
		
		// Loop through each byte of the hashed data  
		// and format each one as a hexadecimal string. 
		for (int i = 0; i < t_data.Length; i++)
		{
			t_builder.Append( t_data[i].ToString( "x2" ) );
		}
		
		t_f_s.Close();
		
		return t_builder.ToString();
	}

	#endregion



	#region config Keys
	
	public const string CONFIG_ITEMS_TAG				= "Items";
	
	public const string CONFIG_RES_NAME_TAG				= "name";
	
	public const string CONFIG_ASSET_PATH_EXT_TAG		= "asset_ext";
	
	public const string CONFIG_RES_EXT_TAG				= "ext";
	
	public const string CONFIG_DEPENDENCE_LEVEL_TAG		= "Level";
	
	public const string CONFIG_DEPENDENCE_ASSET_TAG		= "DependenceAsset";

	public const string CONFIG_DEPENDENCE_ROOT_ASSET_TAG	= "Root";

	public const string CONFIG_DEPENDENCE_IN_ASSET_TAG		= "In";
	
	public const string CONFIG_MD5_TAG					= "MD5";

	public const string CONFIG_FILE_LEN_TAG				= "size";
	
	private const string CONFIG_HASH_SPLITTER_TAG		= "_-";
	
	private static string[] CONFIG_UI_BUNDLE_BASE			= {
		"_Project/ArtAssets/_Bundle_Base/NGUI_Base",
	};

	private static string[] CONFIG_FX_BUNDLE_BASE			= {
		"_Project/ArtAssets/Fx/Models/Materials",
	};
	
	#endregion

	
	
	#region All Bundle List

	/// All Folders under this must be named as 2015_0617_1635 format.
	/// 
	/// Each Version, server will receive a rar file named "2015_0617_1635" with 2015_0617_1635 folder and All Platform subfoders under it.
	public const string CONFIG_ARCHIVED_ASSETS_PATH				= "StreamingArchived";

	public const string All_Bundle_List_Path_Name				= "Bundles";

	
	public const string CONFIG_BUNDLE_SMALL_VERSION_TAG			= "Version";

	public const string CONFIG_BUNDLE_BASE_VERSION_TAG			= "baseVersion";

	public const string CONFIG_BUNDLE_BIG_VERSION_TAG			= "BigVersion";

	public const string CONFIG_BUNDLE_LIST_BUILD_TIME_TAG		= "BuildTime";

	public const string CONFIG_BUNDLE_LIST_ITEMS_TAG			= "items";



	public const string CONFIG_BUNDLE_LIST_ITEM_KEY_TAG			= "key";

	public const string CONFIG_BUNDLE_LIST_ITEM_SIZE_TAG		= "size";

	public const string CONFIG_BUNDLE_LIST_ITEM_MD5_TAG				= "MD5";

	public const string CONFIG_BUNDLE_LIST_ITEM_VERSION_TAG			= "version";


	#endregion



//	#region Debug Functions

//	#endregion
}