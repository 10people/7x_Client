//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorPPPlatform : MonoBehaviour {

	
	
	#region PP
	
	public const string PP_PLATFORM_KIT_FOLDER_NAME = "PPAppPlatformKit.framework";

	private const string Configured_PP_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_PP";

	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "/PP Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_PP_XCodeProject;
		
		EditorBuildiOS3rd.ProcessFile (t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME);
		
		EditorBuildiOS3rd.ProcessFolder (t_path, PP_PLATFORM_KIT_FOLDER_NAME);
		
		EditorBuildiOS3rd.ProcessFile (t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME);



		{
			EditorBuildiOS3rd.ProcessXG (t_path);
		}
	}


	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "/PP Platform/Third Platform Project", false, 100)]
	private static void BuildPlatformProject(){
//		{
//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}

	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "PP", false, 1)]
	public static void BuildSettings(){
		EditorBuildiOS3rd.BuildSettings( "PP" );
	
		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "PP.OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
//
//			Debug.Log( "TODO, test PP pbx." );
//
//			return;
//		}

		{
//			EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.PROJECT_CONFIG_FOLDER_NAME );

			OnProcessPbx( p_target, p_path_to_built_project );
		}
	}

	private static void OnProcessPbx( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnProcessPbx()" );
		#endif

		string t_pbx = EditorBuildiOS3rd.GetPBXContent( p_target, p_path_to_built_project );

		// backup
		{
			EditorBuildiOS3rd.BackUpPbx( p_path_to_built_project );
		}

		{
			// build file, rder
			EditorBuildiOS3rd.UpdatePbxAndCheckKeys( ref t_pbx, 
			                                     "D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n",

			                                     "D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n" +
			                                     "D8939B6C1B7F464900C4ACCC /* PPAppPlatformKit.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B6B1B7F464900C4ACCC /* PPAppPlatformKit.framework */; };\n" +
			                                     "D8939B6E1B7F466300C4ACCC /* PPAppPlatformKit.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8939B6D1B7F466300C4ACCC /* PPAppPlatformKit.bundle */; };\n" +
			                                     "D8939B6F1B7F46B200C4ACCC /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; settings = {ATTRIBUTES = (Weak, ); }; };\n" +
			                                     "D8939B711B7F46D000C4ACCC /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B701B7F46D000C4ACCC /* libXG-SDK.a */; };\n" +
			                                     "D8939B721B7F46E900C4ACCC /* libiPhone-lib.a in Frameworks */ = {isa = PBXBuildFile; fileRef = A7FF4561B84759B56ECC870F /* libiPhone-lib.a */; };\n" +
			                                     "D8939B771B7F472200C4ACCC /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B761B7F472200C4ACCC /* CoreTelephony.framework */; };\n" +
			                                     "D8939B791B7F472F00C4ACCC /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B781B7F472F00C4ACCC /* libz.dylib */; };\n" +
			                                     "D8939B7B1B7F473700C4ACCC /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B7A1B7F473700C4ACCC /* Security.framework */; };\n" +
			                                     "D8939B7D1B7F475000C4ACCC /* libsqlite3.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8939B7C1B7F475000C4ACCC /* libsqlite3.dylib */; };\n"
  			);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.cpp; name = RegisterMonoModules.cpp; path = Libraries/RegisterMonoModules.cpp; sourceTree = SOURCE_ROOT; };\n",

									 "D8939B6B1B7F464900C4ACCC /* PPAppPlatformKit.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = PPAppPlatformKit.framework; sourceTree = \"<group>\"; };\n" +
			                         "D8939B6D1B7F466300C4ACCC /* PPAppPlatformKit.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; name = PPAppPlatformKit.bundle; path = ../../../software/3rd/2015_0922/PPSDK_U3D_S153D1551/PPSDK_U3D_S153D1551/PPAppPlatformKit.bundle; sourceTree = \"<group>\"; };\n" +
			                         "D8939B701B7F46D000C4ACCC /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "D8939B741B7F46FC00C4ACCC /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "D8939B751B7F46FC00C4ACCC /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "D8939B761B7F472200C4ACCC /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "D8939B781B7F472F00C4ACCC /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8939B7A1B7F473700C4ACCC /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "D8939B7C1B7F475000C4ACCC /* libsqlite3.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libsqlite3.dylib; path = usr/lib/libsqlite3.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.cpp; name = RegisterMonoModules.cpp; path = Libraries/RegisterMonoModules.cpp; sourceTree = SOURCE_ROOT; };\n" 
			);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "D8939B7D1B7F475000C4ACCC /* libsqlite3.dylib in Frameworks */,\n" +
			                         "D8939B7B1B7F473700C4ACCC /* Security.framework in Frameworks */,\n" +
			                         "D8939B791B7F472F00C4ACCC /* libz.dylib in Frameworks */,\n" +
			                         "D8939B771B7F472200C4ACCC /* CoreTelephony.framework in Frameworks */,\n" +
			                         "D8939B6F1B7F46B200C4ACCC /* AdSupport.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "D8939B6C1B7F464900C4ACCC /* PPAppPlatformKit.framework in Frameworks */,\n" +
			                         "D8939B711B7F46D000C4ACCC /* libXG-SDK.a in Frameworks */,\n"
			                         );

			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "D8939B6D1B7F466300C4ACCC /* PPAppPlatformKit.bundle */,\n" +
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" +
			                         "D8939B701B7F46D000C4ACCC /* libXG-SDK.a */,\n" +
			                         "D8939B6B1B7F464900C4ACCC /* PPAppPlatformKit.framework */,\n"
			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "D8939B7C1B7F475000C4ACCC /* libsqlite3.dylib */,\n" +
			                         "D8939B7A1B7F473700C4ACCC /* Security.framework */,\n" +
			                         "D8939B781B7F472F00C4ACCC /* libz.dylib */,\n" +
			                         "D8939B761B7F472200C4ACCC /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n"
			                          );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "8AF18FE316490981007B4420 /* Unity */,\n",

			                         "8AF18FE316490981007B4420 /* Unity */,\n" +
			                         "D8939B731B7F46FC00C4ACCC /* XG */,\n"
			                         );

			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "D8939B6E1B7F466300C4ACCC /* PPAppPlatformKit.bundle in Resources */,\n" +
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n"
			                          );

			// xg
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7220E80637F000160D3 /* Libraries */ = {\n",

									 "D8939B731B7F46FC00C4ACCC /* XG */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "D8939B741B7F46FC00C4ACCC /* XGPush.h */,\n" +
			                         "D8939B751B7F46FC00C4ACCC /* XGSetting.h */,\n" +
			                         ");\n" +
			                         "name = XG;\n" +
			                         "path = Classes/XG;\n" +
			                         "sourceTree = SOURCE_ROOT;\n" +
			                         "};\n" +
			                         "D8A1C7220E80637F000160D3 /* Libraries */ = {\n" 
									);

			// source
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//
//
//			                         );

			// framework search
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
			                         "COPY_PHASE_STRIP = YES;\n" +
			                         "				FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n",
			                         
			                         "COPY_PHASE_STRIP = YES;\n" +
			                         "ENABLE_BITCODE = NO;\n" +
			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
			                         "\"$(inherited)\",\n" +
			                         "\"$(PROJECT_DIR)\",\n" +
			                         ");\n"
			                         );

			// lib search
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n",
			                         
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n" +
			                         "\"$(PROJECT_DIR)\",\n" +
			                         "\"$(PROJECT_DIR)/Libraries\",\n" 
			                         );

			// flag
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "OTHER_LDFLAGS = (\n" +
			                         "					\"-weak_framework\",\n" +
			                         "					CoreMotion,\n" +
			                         "					\"-weak-lSystem\",\n" + 
			                         "				);",
			                         
			                         "OTHER_LDFLAGS = \"-ObjC\";\n"
			                         );



			//Debug.Log( "Processed t_pbx: " + t_pbx );
		}



		{
			// save
			EditorBuildiOS3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}

		
	#endregion
}
