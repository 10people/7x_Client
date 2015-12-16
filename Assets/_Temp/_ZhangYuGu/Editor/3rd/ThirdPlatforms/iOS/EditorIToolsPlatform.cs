//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class EditorIToolsPlatform : MonoBehaviour {

	#region I Apple
	
	public const string PLATFORM_KIT_BUNDLE_FOLDER_NAME = "AlipaySDK.bundle";

	public const string PLATFORM_KIT_FRAMEWORK_FOLDER_NAME = "AlipaySDK.framework";

	public const string PLATFORM_KIT_IMAGE_BUNDLE_FILE_NAME = "libHXAppPlatformKitPro.a";

	private const string Configured_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_iTools";

	private const string ITOOLS_H_FOLDER_NAME = "Classes/HXAppPlatformKitPro.h";


	
	public const string XG_CLASS_FOLDER_NAME = "Classes/XS";
	
	public const string XG_LIB_FILE_NAME = "libXSDK.a";



	
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "ITools Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_XCodeProject;
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_H_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME );

		EditorBuildiOS3rd.ProcessFile ( t_path, ITOOLS_H_FOLDER_NAME );

		
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, PLATFORM_KIT_BUNDLE_FOLDER_NAME );

		EditorBuildiOS3rd.ProcessFolder ( t_path, PLATFORM_KIT_FRAMEWORK_FOLDER_NAME );

		EditorBuildiOS3rd.ProcessFile ( t_path, PLATFORM_KIT_IMAGE_BUNDLE_FILE_NAME );
		

		
		{
			EditorBuildiOS3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "ITools Platform/Third Platform Project", false, 100)]
	private static void BuildPlatformProject(){
//		{
//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "ITools", false, 1)]
	public static void BuildSettings(){
		EditorBuild3rd.BuildSettings( "ITools" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "ITools.OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
//
//			Debug.Log( "TODO, test ITools pbx." );
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
			                        "11F2F9851BC90C1600956C09 /* libHXAppPlatformKitPro.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F9831BC90C1600956C09 /* libHXAppPlatformKitPro.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11F2F9861BC90C1600956C09 /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F9841BC90C1600956C09 /* libXG-SDK.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11F2F98C1BC90C5500956C09 /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F98B1BC90C5500956C09 /* CoreTelephony.framework */; };\n" +
			                        "11F2F98E1BC90C5E00956C09 /* libz.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F98D1BC90C5E00956C09 /* libz.tbd */; };\n" +
			                        "11F2F9901BC90C6900956C09 /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F98F1BC90C6900956C09 /* Security.framework */; };\n" +
			                        "11F2F9921BC90C7300956C09 /* libsqlite3.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F9911BC90C7300956C09 /* libsqlite3.tbd */; };\n" +
			                        "11F2F9951BC90D9B00956C09 /* AlipaySDK.bundle in Resources */ = {isa = PBXBuildFile; fileRef = 11F2F9931BC90D9B00956C09 /* AlipaySDK.bundle */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11F2F9961BC90D9B00956C09 /* AlipaySDK.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F9941BC90D9B00956C09 /* AlipaySDK.framework */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11F2F9981BC90DD200956C09 /* libstdc++.6.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F9971BC90DD200956C09 /* libstdc++.6.tbd */; };\n" +
			                        "11F2F9991BC90DE000956C09 /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; };\n" +
			                        "11F2F99B1BC90DF900956C09 /* MobileCoreServices.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11F2F99A1BC90DF900956C09 /* MobileCoreServices.framework */; };\n"

  			);

			// embed framework
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//			                         "/* End PBXCopyFilesBuildPhase section */\n",
//
//			                         "D8038DAC1BB28E3000C4CFC0 /* Embed Frameworks */ = {\n" +
//			                         "isa = PBXCopyFilesBuildPhase;\n" +
//			                         "buildActionMask = 2147483647;\n" +
//			                         "dstPath = \"\";\n" +
//			                         "dstSubfolderSpec = 10;\n" +
//			                         "files = (\n" +
//			                         "D8038DAB1BB28E3000C4CFC0 /* xsdkFramework.framework in Embed Frameworks */,\n" +
//			                         ");\n" +
//			                         "name = \"Embed Frameworks\";\n" +
//			                         "runOnlyForDeploymentPostprocessing = 0;\n" +
//			                         "};\n" +
//			                         "/* End PBXCopyFilesBuildPhase section */\n"
//									);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                        "11F2F9831BC90C1600956C09 /* libHXAppPlatformKitPro.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = libHXAppPlatformKitPro.a; sourceTree = \"<group>\"; };\n" +
			                         "11F2F9841BC90C1600956C09 /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "11F2F9871BC90C2500956C09 /* HXAppPlatformKitPro.h */ = {isa = PBXFileReference; lastKnownFileType = sourcecode.c.h; path = HXAppPlatformKitPro.h; sourceTree = \"<group>\"; };\n" +
			                         "11F2F9891BC90C2A00956C09 /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "11F2F98A1BC90C2A00956C09 /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "11F2F98B1BC90C5500956C09 /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "11F2F98D1BC90C5E00956C09 /* libz.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libz.tbd; path = usr/lib/libz.tbd; sourceTree = SDKROOT; };\n" +
			                         "11F2F98F1BC90C6900956C09 /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "11F2F9911BC90C7300956C09 /* libsqlite3.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libsqlite3.tbd; path = usr/lib/libsqlite3.tbd; sourceTree = SDKROOT; };\n" +
			                         "11F2F9931BC90D9B00956C09 /* AlipaySDK.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AlipaySDK.bundle; sourceTree = \"<group>\"; };\n" +
			                         "11F2F9941BC90D9B00956C09 /* AlipaySDK.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = AlipaySDK.framework; sourceTree = \"<group>\"; };\n" +
			                         "11F2F9971BC90DD200956C09 /* libstdc++.6.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = \"libstdc++.6.tbd\"; path = \"usr/lib/libstdc++.6.tbd\"; sourceTree = SDKROOT; };\n" +
			                         "11F2F99A1BC90DF900956C09 /* MobileCoreServices.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = MobileCoreServices.framework; path = System/Library/Frameworks/MobileCoreServices.framework; sourceTree = SDKROOT; };\n"

			);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "11F2F99B1BC90DF900956C09 /* MobileCoreServices.framework in Frameworks */,\n" +
			                         "11F2F9991BC90DE000956C09 /* AdSupport.framework in Frameworks */,\n" +
			                         "11F2F9981BC90DD200956C09 /* libstdc++.6.tbd in Frameworks */,\n" +
			                         "11F2F9961BC90D9B00956C09 /* AlipaySDK.framework in Frameworks */,\n" +
			                         "11F2F9921BC90C7300956C09 /* libsqlite3.tbd in Frameworks */,\n" +
			                         "11F2F9901BC90C6900956C09 /* Security.framework in Frameworks */,\n" +
			                         "11F2F98E1BC90C5E00956C09 /* libz.tbd in Frameworks */,\n" +
			                         "11F2F98C1BC90C5500956C09 /* CoreTelephony.framework in Frameworks */,\n" +
			                         "11F2F9861BC90C1600956C09 /* libXG-SDK.a in Frameworks */,\n" +
			                         "11F2F9851BC90C1600956C09 /* libHXAppPlatformKitPro.a in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" 

			                         );


			// PBXGroup
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "/* Begin PBXGroup section */\n",
			                         
			                         "/* Begin PBXGroup section */\n" +
			                         "11F2F9881BC90C2A00956C09 /* XG */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "11F2F9891BC90C2A00956C09 /* XGPush.h */,\n" +
			                         "11F2F98A1BC90C2A00956C09 /* XGSetting.h */,\n" +
			                         ");\n" +
			                         "path = XG;\n" +
									 "sourceTree = \"<group>\";\n" +
			                         "};\n"

			                         );


			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "11F2F9931BC90D9B00956C09 /* AlipaySDK.bundle */,\n" +
			                         "11F2F9941BC90D9B00956C09 /* AlipaySDK.framework */,\n" +
			                         "11F2F9831BC90C1600956C09 /* libHXAppPlatformKitPro.a */,\n" +
			                         "11F2F9841BC90C1600956C09 /* libXG-SDK.a */,\n" +
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" 

			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "11F2F99A1BC90DF900956C09 /* MobileCoreServices.framework */,\n" +
			                         "11F2F9971BC90DD200956C09 /* libstdc++.6.tbd */,\n" +
			                         "11F2F9911BC90C7300956C09 /* libsqlite3.tbd */,\n" +
			                         "11F2F98F1BC90C6900956C09 /* Security.framework */,\n" +
			                         "11F2F98D1BC90C5E00956C09 /* libz.tbd */,\n" +
			                         "11F2F98B1BC90C5500956C09 /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n" 

			                          );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n",

			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n" +
			                         "11F2F9871BC90C2500956C09 /* HXAppPlatformKitPro.h */,\n" +
			                         "11F2F9881BC90C2A00956C09 /* XG */,\n"

			                         );

			// PBXNativeTarget
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n",
//
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n" +
//			                         "D8038DAC1BB28E3000C4CFC0 /* Embed Frameworks */,\n"
//			                         );

			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "11F2F9951BC90D9B00956C09 /* AlipaySDK.bundle in Resources */,\n" +
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n" 
			                          );


			// source
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//			                         "D82DCFC30E8000A5005D6AD8 /* main.mm in Sources */,\n",
//
//			                         "D82DCFC30E8000A5005D6AD8 /* main.mm in Sources */,\n" +
//			                         "D8DDD5041B8081C6003A7A6F /* GTMBase64.m in Sources */,\n"
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

//			
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


//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "				FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n",
//			                         
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "ENABLE_BITCODE = NO;\n" +
//			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
//			                         "\"$(inherited)\",\n" +
//			                         "\"$(PROJECT_DIR)\",\n" +
//			                         ");\n"
//			                         );
//
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "				ENABLE_BITCODE = NO;\n" +
//			                         "				FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n",
//			                         
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "ENABLE_BITCODE = NO;\n" +
//			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
//			                         "\"$(inherited)\",\n" +
//			                         "\"$(PROJECT_DIR)\",\n" +
//			                         ");\n"
//			                         );


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



//	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_PREFIX + "IApple Platform/ipa", false, 9999)]
	private static void Build(){
		UnityEngine.Debug.Log( "Build ipa." );

//		Process.Start( "/Application/Utilities/Terminal.app", "/Users/dastan/Downloads/echo.sh" );

//		{
//			Process t_process = new Process();
//
//			ProcessStartInfo t_info = new ProcessStartInfo( "/bin/bash", "/Users/dastan/Downloads/echo.sh" );
//
//			t_info.WindowStyle = ProcessWindowStyle.Normal;
//
//			t_process.StartInfo = t_info;
//
//			t_process.Start();
//		}

//		{
//			
//			ProcessStartInfo t_proc = new ProcessStartInfo();
//			
//			t_proc.FileName = "open";
//			
//			t_proc.WorkingDirectory = "/Users/dastan/Downloads";
//			
//			t_proc.Arguments = "echo.sh";
//			
//			t_proc.UseShellExecute = false;
//			
//			t_proc.RedirectStandardOutput = true;
//			
//			t_proc.WindowStyle = ProcessWindowStyle.Normal;
//			
//			Process t_process = Process.Start( t_proc );
//			
//			t_process.WaitForExit();
//			
//		}

		
		UnityEngine.Debug.Log( "Build ipa Done." );


//		cd /Users/dastan/workspace/xcode_workspace/xcode_qixiong
//			
//		xcodebuild -scheme Unity-iPhone archive -archivePath $HOME/Desktop/x.xcarchive
//				
//		xcodebuild -exportArchive -exportFormat ipa -archivePath "$HOME/Desktop/x.xcarchive" -exportPath "$HOME/Desktop/x.ipa" -exportProvisioningProfile "iOSTeam Provisioning Profile: com.youxigu.doujianwushuang.kuaiyong"
	}
	
	#endregion
}
