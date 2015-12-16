#define DEBUG_MANIFEST

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

public class EditorBuildManifest {

	private static List<ManifestInfo> m_list_manifest = new List<ManifestInfo>();

	#region Build Manifest

	public static void BuildManifest( string p_relative_path ){
		// clean cached data
		{
			PrepareForBuild( p_relative_path );
		}

		// build manifest list
		{
			ExecuteBuildManifest( p_relative_path );
		}

		// build manifest
		{
			string t_manifest = ConstructManifest();

			#if DEBUG_MANIFEST
			Debug.Log( "----------------------- manifest -------------------" );

			Debug.Log( t_manifest );
			#endif

			{
				string t_path = GetManifestPath( p_relative_path );

				PathHelper.GetFullPath_WithRelativePath( t_path );

				FileHelper.OutputFile( t_path, t_manifest );
			}
		}

		{
			EditorHelper.Refresh();
		}

		// build for update
		{
			BuildBundle( p_relative_path );
		}

		{
			EditorHelper.Refresh();
		}

		// clean
		{
			Clean( p_relative_path );
		}

		{
			EditorHelper.Refresh();
		}
	}

	private static void PrepareForBuild( string p_relative_path ){
		{
			m_list_manifest.Clear();
		}

		{
			string t_path = GetManifestPath( p_relative_path );
			
			PathHelper.GetFullPath_WithRelativePath( t_path );
			
			FileHelper.FileDelete( t_path );
		}
	}

	#endregion



	#region Sub Build

	private static void BuildBundle( string p_relative_path ){
//		#if DEBUG_MANIFEST
//		Debug.Log( "BuildBundle( " + p_relative_path + " )" );
//		#endif

		{
			CleanBundle( GetBuildTargetPath( p_relative_path ) );
		}

		List<AssetBundleBuild> t_build_list = new List<AssetBundleBuild>();

		{
			AssetBundleBuild t_build = new AssetBundleBuild();
			
			t_build.assetBundleName = GetManifestBundleName();
			
			t_build.assetNames = new string[]{ GetManifestPath( p_relative_path ) };
			
			t_build_list.Add( t_build );
		}

		BuildPipeline.BuildAssetBundles( GetBuildTargetPath( p_relative_path ), 
		                                t_build_list.ToArray(),
		                                BuildAssetBundleOptions.None,
		                                EditorBundleHelper.GetBuildTarget() );
	}

	/// Constructs the manifest.
	private static string ConstructManifest(){
		StringBuilder t_string = new StringBuilder();
		for( int i = 0; i < m_list_manifest.Count; i++ ){
			ManifestInfo t_info = m_list_manifest[ i ];

			t_string.Append( ManifestHelper.CONST_BUNDLE_KEY + ManifestHelper.CONST_KV_SPLITTER + t_info.GetBundlePath() + "\n" );

			t_string.Append( ManifestHelper.CONST_SIZE_KEY + ManifestHelper.CONST_KV_SPLITTER + t_info.GetFileSize() + "\n" );

			t_string.Append( ManifestHelper.CONST_VERSION_KEY + ManifestHelper.CONST_KV_SPLITTER + t_info.GetVersion() + "\n" );

			{
				List<string> t_assets = t_info.GetAssets();

				if( t_assets.Count > 0 ){
					t_string.Append( ManifestHelper.CONST_ASSETS_KEY + ManifestHelper.CONST_KV_SPLITTER + "\n" );

					for( int j = 0; j < t_assets.Count; j++ ){
						t_string.Append( t_assets[ j ] + "\n" );
					}
				}
			}

			{
				List<string> t_scenes = t_info.GetScenes();

				if( t_scenes.Count > 0 ){
					t_string.Append( ManifestHelper.CONST_SCENES_KEY + ManifestHelper.CONST_KV_SPLITTER + "\n" );

					for( int j = 0; j < t_scenes.Count; j++ ){
						t_string.Append( StringHelper.GetWWWFileName( t_scenes[ j ] ) + "\n" );
					}
				}
			}
		}

		string t_manifest = t_string.ToString();

		return t_manifest;
	}

	/// Build assets and scenes manifest info.
	private static void ExecuteBuildManifest( string p_relative_path ){
		#if DEBUG_BUNDLE
		//		Debug.Log( "BuildManifest( " + p_relative_path + " )" );
		#endif
		
		string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
		
		DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
		
		// iterate sub folders
		{
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );
				
				string t_relative_process_path = t_sub_dir_path.Substring( Application.dataPath.Length + 1 );
				
//				Debug.Log( "Sub Folder " + i + " : " + t_relative_process_path );
				
				ExecuteBuildManifest( t_relative_process_path );
			}
		}
		
		{
			FileInfo[] t_files = t_dir.GetFiles();
			
			for( int i = 0; i < t_files.Length; i++ ){
				FileInfo t_info = t_files[ i ];
				
				if( StringHelper.IsEndWith( t_info.FullName, CONST_MANIFEST_EXTENSION ) ){
//					#if DEBUG_BUNDLE
//					Debug.Log( t_info.FullName );
//					#endif

					ManifestInfo t_manifest = new ManifestInfo( t_info.FullName );

					if( !t_manifest.IsMasterManifest() ){
						m_list_manifest.Add( t_manifest );
					}
				}
			}
		}
	}
	
	#endregion



	#region Clean

	private static void CleanBundle( string p_relative_path ){
//		#if DEBUG_MANIFEST
//		Debug.Log( "CleanBundle( " + p_relative_path + " )" );
//		#endif

		string t_full_target_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
		
		FileHelper.DeleteDirectoryAndCreate( t_full_target_path );
		
		EditorHelper.Refresh();
	}

	/// Clean temporary data.
	private static void Clean( string p_relative_path ){
		string t_full_path = PathHelper.GetFullPath_WithRelativePath( p_relative_path );
		
		DirectoryInfo t_dir = new DirectoryInfo( t_full_path );
		
		// iterate sub folders
		{
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				string t_sub_dir_path = t_sub_dir.FullName.Replace( "\\", "/" );
				
				string t_relative_process_path = t_sub_dir_path.Substring( Application.dataPath.Length + 1 );
				
//				Debug.Log( "Sub Folder " + i + " : " + t_relative_process_path );
				
				Clean( t_relative_process_path );
			}
		}
		
		{
			FileInfo[] t_files = t_dir.GetFiles();
			
			for( int i = 0; i < t_files.Length; i++ ){
				FileInfo t_info = t_files[ i ];
				
				if( StringHelper.IsEndWith( t_info.FullName, CONST_MANIFEST_EXTENSION ) ||
				   	StringHelper.IsEndWith( t_info.FullName, CONST_MANIFEST_MAP_EXCTENSION ) ){
					FileHelper.FileDelete( t_info.FullName );
				}
			}
		}
	}

	#endregion



	#region Utilities

	private static string GetManifestPath( string p_relative_path ){
		return p_relative_path + "/" + ManifestHelper.CONST_FILE_NAME + CONST_MANIFEST_MAP_EXCTENSION;
	}

	private static string GetManifestBundleName(){
		return ManifestHelper.CONST_FILE_NAME;
	}

	/// "Assets/StreamingArchived/Android/file"
	public static string GetBuildTargetPath( string p_relative_path ){
		return p_relative_path + "/" + ManifestHelper.CONST_MANIFEST_FOLDER_NAME;
	}

	#endregion



	#region Const

	private const string CONST_MANIFEST_EXTENSION		= ".manifest";

	private const string CONST_MANIFEST_MAP_EXCTENSION	= ".txt";

	#endregion

	public class ManifestInfo{
		/// E:\WorkSpace_External\DynastyMobile_2014_Unity_5\Assets\StreamingArchived\Android\assets\resources\_data\design\action.manifest
		private string m_full_path = "";

		private string m_file_content = "";



		private bool m_is_master_manifest = false;

		/// _Data/Design/Action
		private List<string> m_assets_list = new List<string>();

		/// _Temp/_ZhangYuGu/Scenes/_Empty/_Empty
		private List<string> m_scenes_list = new List<string>();

		/// -2G to 2G
		private int m_file_size = 0;

		/// retained for next version to use
		private int m_version = -1;

		public ManifestInfo( string p_full_path ){
			m_full_path = p_full_path;

//			#if DEBUG_MANIFEST
//			Debug.Log( "m_full_path: " + p_full_path );
//			#endif

			AnalyzeManifest();
		}


		#region Analyze

		private void AnalyzeManifest(){
			m_file_content = FileHelper.ReadString( m_full_path );

//			#if DEBUG_MANIFEST
//			Debug.Log( "File Path: " + m_full_path );
//
//			Debug.Log( "File Content: " + m_file_content );
//			#endif

			if( string.IsNullOrEmpty( m_file_content ) ){
				Debug.LogError( "File Format Error." );

				return;
			}

			string[] t_contents = m_file_content.Split( new string[]{ "\n" }, System.StringSplitOptions.None );

			{
				AnalyzeMaster( t_contents );
			}

			if( IsMasterManifest() ){
				return;
			}

			{
				string t_bundle_path = StringHelper.RemoveExtension( m_full_path );

				SetFileSize( FileHelper.GetFileSize( t_bundle_path ) );
			}

			{
				AnalyzeContents( t_contents );
			}
		}

		private void AnalyzeMaster( string[] p_contents ){
			if( p_contents[ 1 ].IndexOf( "AssetBundleManifest" ) >= 0 ){
				SetIsMasterManifest( true );
			}
			else{
				SetIsMasterManifest( false );
			}
		}

		private void AnalyzeContents( string[] p_contents ){
			bool t_is_analyzing_target = false;

			for( int i = 0; i < p_contents.Length; i++ ){
				string t_line = p_contents[ i ];

				if( t_line.IndexOf( "Assets:" ) == 0 ){
					t_is_analyzing_target = true;

					continue;
				}

				if( t_line.IndexOf( "Dependencies:" ) == 0 ){
					t_is_analyzing_target = false;
					
					continue;
				}

				// skip useless
				if( !t_is_analyzing_target ){
					continue;
				}

				// analyze content
				{
					string[] t_items = t_line.Split( new string[]{ " " }, System.StringSplitOptions.None );

					if( t_items.Length < 2 ){
						Debug.LogError( "Error in Length: " + t_items.Length + " " + t_line );

						continue;
					}

					if( t_items.Length > 2 ){
						Debug.LogError( "Error in line, to many spaces: " + t_items.Length + " " + t_line );

						continue;
					}

					string t_asset_path = t_items[ 1 ];

//					#if DEBUG_MANIFEST
//					Debug.Log( "Origin Asset: " + t_asset_path );
//					#endif

					t_asset_path = EditorHelper.RemoveAssetResourcesPrefix( t_asset_path );

					bool t_is_scene_asset = false;

					if( t_asset_path.EndsWith( ".unity" ) ){
						t_is_scene_asset = true;
					}

					t_asset_path = StringHelper.RemoveExtension( t_asset_path );

					if( t_is_scene_asset ){
//						#if DEBUG_MANIFEST
//						Debug.Log( "Final Scene: " + t_asset_path );
//						#endif

						m_scenes_list.Add( t_asset_path );
					}
					else{
//						#if DEBUG_MANIFEST
//						Debug.Log( "Final Asset: " + t_asset_path );
//						#endif

						m_assets_list.Add( t_asset_path );
					}
				}
			}
		}

		#endregion



		#region Info

		public List<string> GetAssets(){
			return m_assets_list;
		}

		public List<string> GetScenes(){
			return m_scenes_list;
		}

		/// assets/resources/_data/design/action
		public string GetBundlePath(){
			string t_path = m_full_path.Substring( Application.dataPath.Length + 1 + 
			                                      EditorHelper.GetStreamingArchivedPrefix().Length + 1 +
			                                      PlatformHelper.GetPlatformTag().Length + 1 );

			t_path = t_path.Replace( "\\", "/" );

			t_path = StringHelper.RemoveExtension( t_path );

			return t_path;
		}

		private void SetIsMasterManifest( bool p_is_master_manifest ){
			m_is_master_manifest = p_is_master_manifest;

//			#if DEBUG_MANIFEST
//			Debug.Log( "SetIsMasterManifest( " + m_is_master_manifest + " )" );
//			#endif
		}

		public bool IsMasterManifest(){
			return m_is_master_manifest;
		}

		private void SetFileSize( int p_file_size ){
			m_file_size = p_file_size;

//			#if DEBUG_MANIFEST
//			Debug.Log( "Set Bundle File Size( " + p_file_size + " )" );
//			#endif
		}

		public int GetFileSize(){
			return m_file_size;
		}

		private void SetVersion( int p_version ){
			m_version = p_version;
		}

		public int GetVersion(){
			return m_version;
		}

		#endregion
	}
}
