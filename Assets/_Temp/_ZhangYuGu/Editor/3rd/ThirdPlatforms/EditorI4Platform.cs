//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class EditorI4Platform : MonoBehaviour {

	
	
	#region AiSi
	
	public const string AISI_PLATFORM_KIT_FOLDER_NAME = "AsSdkFMWK.framework";
	
	public const string AISI_PLATFORM_KIT_BUNDLE_FOLDER_NAME = "AlipaySDK.bundle";
	
	public const string AISI_PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME = "AsImage.bundle";

	private const string Configured_AiSi_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_AiSi";

	
	public const string AISI_PLATFORM_H_FOLDER_NAME = "Classes/AsSDKConnector.h";
	
	public const string AISI_PLATFORM_MM_FOLDER_NAME = "Classes/AsSDKConnector.mm";
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "AiSi Platform/Third Platform", false, 1)]
	public static void BuildAiSi(){
		#if DEBUG_PLATFORM
		Debug.Log ( "BuildAiSi()" );
		#endif

		string t_path = PathHelper.GetMacHome() + Configured_AiSi_XCodeProject;
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_H_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME );
		
		
		
		EditorBuildiOS3rd.ProcessFile ( t_path, AISI_PLATFORM_H_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, AISI_PLATFORM_MM_FOLDER_NAME );
		
		
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, AISI_PLATFORM_KIT_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, AISI_PLATFORM_KIT_BUNDLE_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, AISI_PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME );
		

		
		{
			EditorBuildiOS3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "AiSi Platform/Third Platform Project", false, 100)]
	private static void BuildAiSiProject(){
//		{
//			BuildAiSi();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "AiSi", false, 1)]
	public static void BuildSettingsAiSi(){
		EditorBuildiOS3rd.BuildSettings( "AiSi" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildI4Platform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnPostBuildI4Platform()" );
		#endif

		{
			BuildAiSi();
		}

//		if ( true ) {
//			#if DEBUG_I4_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
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
			// build file, order
			EditorBuildiOS3rd.UpdatePbxAndCheckKeys( ref t_pbx, 
			                            "D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n",

			                            "D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n" +
 				                        "D8C531F31B8076D000EE6F2A /* AlipaySDK.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8C531EF1B8076D000EE6F2A /* AlipaySDK.bundle */; };\n" +
 										"D8C531F41B8076D000EE6F2A /* AsImage.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8C531F01B8076D000EE6F2A /* AsImage.bundle */; };\n" +
 										"D8C531F51B8076D000EE6F2A /* AsSdkFMWK.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C531F11B8076D000EE6F2A /* AsSdkFMWK.framework */; };\n" +
 										"D8C531F61B8076D000EE6F2A /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C531F21B8076D000EE6F2A /* libXG-SDK.a */; };\n" +
				 						"D8C531FC1B8076E500EE6F2A /* AsSDKConnector.mm in Sources */ = {isa = PBXBuildFile; fileRef = D8C531F81B8076E500EE6F2A /* AsSDKConnector.mm */; };\n" +
 										"D8C531FE1B8076FE00EE6F2A /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C531FD1B8076FE00EE6F2A /* CoreTelephony.framework */; };\n" +
 				                        "D8C532001B80770500EE6F2A /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C531FF1B80770500EE6F2A /* libz.dylib */; };\n" +
 				                        "D8C532021B80770F00EE6F2A /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C532011B80770F00EE6F2A /* Security.framework */; };\n" +
 				                        "D8C532041B80771600EE6F2A /* libsqlite3.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C532031B80771600EE6F2A /* libsqlite3.dylib */; };\n" +
 				                        "D8C532051B80773200EE6F2A /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; settings = {ATTRIBUTES = (Weak, ); }; };\n" +
			                            "D8C532071B80778300EE6F2A /* libstdc++.6.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8C532061B80778300EE6F2A /* libstdc++.6.dylib */; settings = {ATTRIBUTES = (Weak, ); }; };\n" 
  			);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

			                         "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                         "D8C531EF1B8076D000EE6F2A /* AlipaySDK.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AlipaySDK.bundle; sourceTree = \"<group>\"; };\n" +
			                         "D8C531F01B8076D000EE6F2A /* AsImage.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AsImage.bundle; sourceTree = \"<group>\"; };\n" +
									 "D8C531F11B8076D000EE6F2A /* AsSdkFMWK.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = AsSdkFMWK.framework; sourceTree = \"<group>\"; };\n" +
			                         "D8C531F21B8076D000EE6F2A /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "D8C531F71B8076E500EE6F2A /* AsSDKConnector.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = AsSDKConnector.h; sourceTree = \"<group>\"; };\n" +
			                         "D8C531F81B8076E500EE6F2A /* AsSDKConnector.mm */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.cpp.objcpp; path = AsSDKConnector.mm; sourceTree = \"<group>\"; };\n" +
									 "D8C531FA1B8076E500EE6F2A /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "D8C531FB1B8076E500EE6F2A /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "D8C531FD1B8076FE00EE6F2A /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "D8C531FF1B80770500EE6F2A /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
									 "D8C532011B80770F00EE6F2A /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "D8C532031B80771600EE6F2A /* libsqlite3.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libsqlite3.dylib; path = usr/lib/libsqlite3.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8C532061B80778300EE6F2A /* libstdc++.6.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = \"libstdc++.6.dylib\"; path = \"usr/lib/libstdc++.6.dylib\"; sourceTree = SDKROOT; };\n"
			);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "D8C532071B80778300EE6F2A /* libstdc++.6.dylib in Frameworks */,\n" +
			                         "D8C532051B80773200EE6F2A /* AdSupport.framework in Frameworks */,\n" +
			                         "D8C532041B80771600EE6F2A /* libsqlite3.dylib in Frameworks */,\n" +
			                         "D8C532021B80770F00EE6F2A /* Security.framework in Frameworks */,\n" +
			                         "D8C532001B80770500EE6F2A /* libz.dylib in Frameworks */,\n" +
			                         "D8C531FE1B8076FE00EE6F2A /* CoreTelephony.framework in Frameworks */,\n" +
			                         "D8C531F51B8076D000EE6F2A /* AsSdkFMWK.framework in Frameworks */,\n" +
			                         "D8C531F61B8076D000EE6F2A /* libXG-SDK.a in Frameworks */,\n" 
			                         );

			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" +
			                         "D8C531EF1B8076D000EE6F2A /* AlipaySDK.bundle */,\n" +
			                         "D8C531F01B8076D000EE6F2A /* AsImage.bundle */,\n" +
			                         "D8C531F11B8076D000EE6F2A /* AsSdkFMWK.framework */,\n" +
			                         "D8C531F21B8076D000EE6F2A /* libXG-SDK.a */,\n" 
			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",
									 "D8C532061B80778300EE6F2A /* libstdc++.6.dylib */,\n" +
			                         "D8C532031B80771600EE6F2A /* libsqlite3.dylib */,\n" +
			                         "D8C532011B80770F00EE6F2A /* Security.framework */,\n" +
			                         "D8C531FF1B80770500EE6F2A /* libz.dylib */,\n" +
			                         "D8C531FD1B8076FE00EE6F2A /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n" 
			                          );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "8A5C148F174E662D0006EB36 /* PluginBase */,\n",
			                         "8A5C148F174E662D0006EB36 /* PluginBase */,\n" +
			                         "D8C531F71B8076E500EE6F2A /* AsSDKConnector.h */,\n" +
			                         "D8C531F81B8076E500EE6F2A /* AsSDKConnector.mm */,\n" +
			                         "D8C531F91B8076E500EE6F2A /* XG */,\n"
			                         );

			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",
			                         "D8C531F31B8076D000EE6F2A /* AlipaySDK.bundle in Resources */,\n" +
			                         "D8C531F41B8076D000EE6F2A /* AsImage.bundle in Resources */,\n" +
									 "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n"
			                          );

			// xg
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                        	"D8A1C7220E80637F000160D3 /* Libraries */ = {\n",

			                         	"D8C531F91B8076E500EE6F2A /* XG */ = {\n" +
										"isa = PBXGroup;\n" +
										"children = (\n" +
										"D8C531FA1B8076E500EE6F2A /* XGPush.h */,\n" +
										"D8C531FB1B8076E500EE6F2A /* XGSetting.h */,\n" +
										");\n" +
										"path = XG;\n" +
										"sourceTree = \"<group>\";\n" +
										"};\n" +
										"D8A1C7220E80637F000160D3 /* Libraries */ = {\n" 
										);

			// source
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "D82DCFC30E8000A5005D6AD8 /* main.mm in Sources */,\n",
			                         "D82DCFC30E8000A5005D6AD8 /* main.mm in Sources */,\n" +
			                         "D8C531FC1B8076E500EE6F2A /* AsSDKConnector.mm in Sources */,\n"
			                         );

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


			//Debug.Log( "Processed t_pbx: " + t_pbx );
		}


		
		{
			// save
			EditorBuildiOS3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}
	
	#endregion
}
