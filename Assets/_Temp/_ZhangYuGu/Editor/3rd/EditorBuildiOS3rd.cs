//#define DEBUG_THIRD_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorBuildiOS3rd : MonoBehaviour {

	#region Common

	public const string BUILD_MENU_IOS_3RD_PREFIX		= "Build/Third Platform/iOS/";

	public const string BUILD_MENU_SETTING_IOS_PREFIX	= "Build/Settings/iOS/";

	#endregion



	#region Project Config

	public const string DataFolderName = "Data";
	
	public const string LibFolderName = "Libraries";

	public const string TempFolderName = "Temp";

	public const string UnityIPhoneFoloderName = "Unity-iPhone";

	public const string UnityIPhoneXCShareDataFolderName = "Unity-iPhone.xcodeproj/xcshareddata";

	public const string UnityIPhoneXCUserDataFolderName = "Unity-iPhone.xcodeproj/xcuserdata";



	public const string INFO_LIST_FOLDER_NAME = "Info.plist";

	public const string CONTROLLER_H_FOLDER_NAME = "Classes/UnityAppController.h";
	
	public const string CONTROLLER_FOLDER_NAME = "Classes/UnityAppController.mm";
	
	public const string PROJECT_CONFIG_FOLDER_NAME = "Unity-iPhone.xcodeproj/project.pbxproj";

	#endregion



	#region XG

	public const string XG_CLASS_FOLDER_NAME = "Classes/XG";

	public const string XG_LIB_FILE_NAME = "libXG-SDK.a";

	public static void ProcessXG( string p_project ){
		ProcessFile ( p_project, XG_LIB_FILE_NAME );

		ProcessFolder (p_project, XG_CLASS_FOLDER_NAME);

		AssetDatabase.Refresh();
	}

	#endregion



	#region GameGoo

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "GameGoo", false, 1)]
	public static void BuildSettingsGameGoo(){
		EditorBuild3rd.BuildSettings( "GameGoo" );
	}

	#endregion


	
	#region Build 3rd

	[PostProcessBuild(1000)]
	public static void OnThirdPlatformPostBuild( BuildTarget p_target, string p_path_to_built_project ){
//		if ( true ) {
//			Debug.Log( "3rd platform not working." );
//
//			return;
//		}

		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "OnThirdPlatformPostBuild( " + p_path_to_built_project + " )" );

		Debug.Log( "PlatformType: " + ThirdPlatform.GetPlatformType() );
		#endif



		#if UNITY_IOS
		switch ( ThirdPlatform.GetPlatformType() ) {
		case ThirdPlatform.PlatformType.I4_Platform:
			EditorI4Platform.OnPostBuildI4Platform( p_target, p_path_to_built_project );
			break;
			
		case ThirdPlatform.PlatformType.KuaiYong_Platform:
			EditorKuaiYongPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;
			
		case ThirdPlatform.PlatformType.PP_Platform:
			EditorPPPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;
			
		case ThirdPlatform.PlatformType.TongBu_Platform:
			EditorTongBuPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;
			
		case ThirdPlatform.PlatformType.XY_Platform:
			EditorXYPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;

		case ThirdPlatform.PlatformType.HaiMa_Platform:
			EditorHaiMaPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;

		case ThirdPlatform.PlatformType.I_Apple_Platform:
			EditorIApplePlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;

		case ThirdPlatform.PlatformType.ITools_Platform:
			EditorIToolsPlatform.OnPostBuildPlatform( p_target, p_path_to_built_project );
			break;
			
		default:
			OnProcessXcode7Platform( p_target, p_path_to_built_project );
//			Debug.Log ( "Default Platform Build." );
			break;
		}
		#endif
	}
	
	private static string GetDesDataFolderPath(){
		return Path.Combine( PathHelper.GetXCodeProjectFullPath(), DataFolderName );
	}
	
//	private static string GetSrcDataFolderPath(){
//		return Path.Combine (Configured_PP_XCodeProject, DataFolderName);
//	}
	
	private static string GetDesLibFolderPath(){
		return Path.Combine (PathHelper.GetXCodeProjectFullPath(), LibFolderName);
	}
	
//	private static string GetSrcLibFolderPath(){
//		return Path.Combine (Configured_PP_XCodeProject, LibFolderName);
//	}
	
	public static void CleanDataFolder(){
		Debug.Log( "CleanDataFolder" ) ;
		
		string t_data_path = GetDesDataFolderPath ();
		
		Debug.Log ( "Data Path: " + t_data_path );
		
		Directory.Delete( t_data_path, true );
	}

	public static void ProcessFile( string p_project_path, string p_relative_path ){
		string t_des = Path.Combine( PathHelper.GetXCodeProjectFullPath(), p_relative_path );
		
		string t_src = Path.Combine( p_project_path, p_relative_path );
		
		if (File.Exists (t_des)) {
			#if DEBUG_THIRD_PLATFORM
			Debug.Log ( "Delete: " + t_des );
			#endif
			
			File.Delete (t_des);
		}
		
		#if DEBUG_THIRD_PLATFORM
		Debug.Log ( "Copy: " + t_src );
		
		Debug.Log ( "To: " + t_des );
		#endif
		
		File.Copy( t_src, t_des );
	}
	
	public static void ProcessFolder( string p_project_path, string p_relative_path ){
		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "ProcessFolder( " + p_relative_path + " )" ) ;
		#endif
		
		string t_des = Path.Combine ( PathHelper.GetXCodeProjectFullPath(), p_relative_path );
		
		string t_src = Path.Combine ( p_project_path, p_relative_path );
		
		if( Directory.Exists( t_des ) ){
			#if DEBUG_THIRD_PLATFORM
			Debug.Log ( "Delete: " + t_des );
			
			Directory.Delete ( t_des, true );
			#endif
		}
		
		#if DEBUG_THIRD_PLATFORM
		Debug.Log ( "Copy: " + t_src );
		
		Debug.Log ( "To: " + t_des );
		#endif
		
		FileHelper.DirectoryCopy( t_src, t_des );
	}

	#endregion



	#region xcode 7

	private static void OnProcessXcode7Platform(BuildTarget p_target, string p_path_to_built_project ){
//		Debug.Log ( "Xcode7." );

		{
			string t_path = PathHelper.GetMacHome() + "/workspace/unity_workspace/Unity_Project_1_0/Assets/Build/iOS/xcode7";
			
			EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );

		}

		string t_pbx = EditorBuildiOS3rd.GetPBXContent( p_target, p_path_to_built_project );
		
		// backup
		{
			EditorBuildiOS3rd.BackUpPbx( p_path_to_built_project );
		}

		EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
		                         "FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n" +
		                         "				GCC_DYNAMIC_NO_PIC = NO;\n",
		                         
		                         "ENABLE_BITCODE = NO;\n" +
		                         "FRAMEWORK_SEARCH_PATHS = (\n" +
		                         "\"$(inherited)\",\n" +
		                         "\"$(PROJECT_DIR)\",\n" +
		                         ");\n" +
		                         "				GCC_DYNAMIC_NO_PIC = NO;\n"
		                         );

		EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
		                         "FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n" +
		                         "				GCC_ENABLE_CPP_EXCEPTIONS = YES;\n",
		                         
		                         "ENABLE_BITCODE = NO;\n" +
		                         "FRAMEWORK_SEARCH_PATHS = (\n" +
		                         "\"$(inherited)\",\n" +
		                         "\"$(PROJECT_DIR)\",\n" +
		                         ");\n" +
		                         "				GCC_ENABLE_CPP_EXCEPTIONS = YES;\n"
		                         );
		
//		EditorBuild3rd.UpdatePbx( ref t_pbx,
//		                         "COPY_PHASE_STRIP = YES;\n" +
//		                         "				FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n",
//		                         
//		                         "COPY_PHASE_STRIP = YES;\n" +
//		                         "ENABLE_BITCODE = NO;\n" +
//		                         "FRAMEWORK_SEARCH_PATHS = (\n" +
//		                         "\"$(inherited)\",\n" +
//		                         "\"$(PROJECT_DIR)\",\n" +
//		                         ");\n"
//		                         );

		{
			// save
			EditorBuildiOS3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}

	#endregion



	#region pbx

	public static void LogPbxCotent( BuildTarget p_target, string p_path_to_built_project ){
		string t_content = GetPBXContent( p_target, p_path_to_built_project );

		Debug.Log ( "pbx content: " + t_content );
	}

	public static string GetPBXContent( BuildTarget p_target, string p_path_to_built_project ){
		string p_project_pbx = EditorBuildiOS3rd.GetProjectPBXPath ( p_path_to_built_project );
		
		FileInfo projectFileInfo = new FileInfo( p_project_pbx );
		
		string contents = projectFileInfo.OpenText().ReadToEnd();

		return contents;
	}

	public static void BackUpPbx( string p_path_to_built_project ){
		string p_project_pbx = EditorBuildiOS3rd.GetProjectPBXPath ( p_path_to_built_project );

		string t_back_path = p_project_pbx + ".back";

		if( File.Exists( t_back_path ) ){
			File.Delete( t_back_path );
		}
		
		{
			#if DEBUG_THIRD_PLATFORM
			Debug.Log( "BackUp Exist." );
			#endif

			File.Copy( p_project_pbx, t_back_path );
		}
	}

	public static void SavePbx( string p_path_to_built_project, string p_content ){
		string p_project_pbx = EditorBuildiOS3rd.GetProjectPBXPath ( p_path_to_built_project );

		if( File.Exists( p_project_pbx )){
			{
//				Debug.Log( "Delete exist." );
				
				File.Delete( p_project_pbx );
			}
		}
			
		{
			#if DEBUG_THIRD_PLATFORM
			Debug.Log( "save pbx." );
			#endif

			StreamWriter saveFile = File.CreateText( p_project_pbx );
			saveFile.Write( p_content );
			saveFile.Close();
		}
	}
	
	public static string GetProjectPBXPath( string p_built_project_path ){
		if( !System.IO.Directory.Exists( p_built_project_path ) ) {
			Debug.LogWarning( "XCode project path does not exist: " + p_built_project_path );
			
			return "";
		}
		
		string projectRootPath = "";
		
		if( p_built_project_path.EndsWith( ".xcodeproj" ) ) {
			Debug.Log( "Opening project " + p_built_project_path );
			
			projectRootPath = Path.GetDirectoryName( p_built_project_path );
			
			p_built_project_path = p_built_project_path;
		} else {
			//			Debug.Log( "Looking for xcodeproj files in " + p_built_project_path );
			
			string[] projects = System.IO.Directory.GetDirectories( p_built_project_path, "*.xcodeproj" );
			
			if( projects.Length == 0 ) {
				Debug.LogWarning( "Error: missing xcodeproj file" );
				
				return "";
			}
			
			projectRootPath = p_built_project_path;
			
			//if the path is relative to the project, we need to make it absolute
			if (!System.IO.Path.IsPathRooted(projectRootPath))
				projectRootPath = Application.dataPath.Replace("Assets", "") + projectRootPath;
			
			//Debug.Log ("projectRootPath adjusted to " + projectRootPath);
			p_built_project_path = projects[ 0 ];
		}
		
		p_built_project_path = Path.Combine( p_built_project_path, "project.pbxproj" );
		
		return p_built_project_path;
	}

	/*	Check pbx files to make sure no existance before, and make 3rd patch.
	 * 
	 *  Notes:
	 * 	1.chek from index 1;
	 * 	2.error if ocurrence count > 1;
	 * 	3.error if line already contained;
	 */
	public static void UpdatePbxAndCheckKeys( ref string p_pbx, string p_origin, string p_new ){
		if( !CheckIfOccurredAndOnlyOnce( p_pbx, p_origin ) ){
			return;
		}

		// check if not exist
		{
			string[] t_items = p_new.Split( new char[]{ '\n' } );

			for( int i = 1; i < t_items.Length; i++ ){
				if( t_items[ i ].Length < 24 ){
					continue;
				}

				string t_key = t_items[ i ].Substring( 0, 24 );

				#if DEBUG_THIRD_PLATFORM
//				Debug.Log( "Key " + i + ": " + t_key );
				#endif

				if( p_pbx.Contains( t_key ) ){
					Debug.LogError( "Error, key already contained: " + t_key );
				}
			}
		}

		{
			UpdatePbx( ref p_pbx, p_origin, p_new );
		}
	}

	public static void UpdatePbx( ref string p_pbx, string p_origin, string p_new ){
		if( !CheckIfOccurredAndOnlyOnce( p_pbx, p_origin ) ){
			return;
		}

		p_pbx = p_pbx.Replace( p_origin, p_new );
	}

	/// true if only p_origin could be found in p_pbx, and only occurred once;
	/// 
	/// 
	private static bool CheckIfOccurredAndOnlyOnce( string p_pbx, string p_origin ){
		if( p_pbx.IndexOf( p_origin ) < 0 ){
			Debug.LogError( "Not exist." );

			Debug.LogError( "Error target: " + p_pbx );
			
			Debug.LogError( "Error key: " + p_origin );

			return false;
		}

		if( p_pbx.IndexOf( p_origin ) == p_pbx.LastIndexOf( p_origin ) ){
			return true;
		}

		Debug.LogWarning( "More than once." );
		
		Debug.LogWarning( "Error key: " + p_origin );


		return true;
	}
	
	#endregion

}
