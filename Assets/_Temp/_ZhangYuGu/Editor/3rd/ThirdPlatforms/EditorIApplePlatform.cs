#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class EditorIApplePlatform : MonoBehaviour {

	#region I Apple
	
	public const string PLATFORM_KIT_BUNDLE_FOLDER_NAME = "AlipaySDK.bundle";

	public const string PLATFORM_KIT_FRAMEWORK_FOLDER_NAME = "AlipaySDK.framework";

	public const string PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME = "iiappleResource.bundle";

	private const string Configured_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_IApple";

	private const string IAPPLE_SDK_LIB_FILE_NAME = "iiappleSDK.a";

	private const string IAPPLE_SDK_LIB_WE_FILE_NAME = "libWeChatSDK.a";

	private const string IAPPLE_H_FOLDER_NAME = "Classes/IIApple.h";


	
	public const string XG_CLASS_FOLDER_NAME = "Classes/XS";
	
	public const string XG_LIB_FILE_NAME = "libXSDK.a";



	
	
	[MenuItem("Build/Third Platform/IApple Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_XCodeProject;
		
		EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.CONTROLLER_H_FOLDER_NAME );
		
		EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.CONTROLLER_FOLDER_NAME );

		EditorBuild3rd.ProcessFile ( t_path, IAPPLE_H_FOLDER_NAME );

		
		
		EditorBuild3rd.ProcessFolder ( t_path, PLATFORM_KIT_BUNDLE_FOLDER_NAME );

		EditorBuild3rd.ProcessFolder ( t_path, PLATFORM_KIT_FRAMEWORK_FOLDER_NAME );

		EditorBuild3rd.ProcessFolder ( t_path, PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME );
		
		

		EditorBuild3rd.ProcessFile ( t_path, IAPPLE_SDK_LIB_FILE_NAME );

		EditorBuild3rd.ProcessFile ( t_path, IAPPLE_SDK_LIB_WE_FILE_NAME );
		
		{
			EditorBuild3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem("Build/Third Platform/IApple Platform/Third Platform Project", false, 100)]
	private static void BuildPlatformProject(){
//		{
//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem("Build/Settings/IApple", false, 1)]
	public static void BuildSettings(){
		EditorBuild3rd.BuildSettings( "IApple" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "IApple.OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
//
//			Debug.Log( "TODO, test IApple pbx." );
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

		string t_pbx = EditorBuild3rd.GetPBXContent( p_target, p_path_to_built_project );

		// backup
		{
			EditorBuild3rd.BackUpPbx( p_path_to_built_project );
		}

		{
			// build file, rder
			EditorBuild3rd.UpdatePbxAndCheckKeys( ref t_pbx, 
									"D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n",

									"D8A1C7280E80637F000160D3 /* RegisterMonoModules.cpp in Sources */ = {isa = PBXBuildFile; fileRef = D8A1C7240E80637F000160D3 /* RegisterMonoModules.cpp */; };\n" +
			                        "11597A251BC7EA7A00B369E7 /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A241BC7EA7A00B369E7 /* CoreTelephony.framework */; };\n" +
			                        "11597A271BC7EA8200B369E7 /* libz.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A261BC7EA8200B369E7 /* libz.tbd */; };\n" +
			                        "11597A281BC7EA8C00B369E7 /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A221BC7EA3000B369E7 /* libXG-SDK.a */; };\n" +
			                        "11597A2A1BC7EA9500B369E7 /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A291BC7EA9500B369E7 /* Security.framework */; };\n" +
			                        "11597A2C1BC7EAAE00B369E7 /* libsqlite3.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A2B1BC7EAAE00B369E7 /* libsqlite3.tbd */; };\n" +
			                        "11597A331BC7EC4000B369E7 /* AlipaySDK.bundle in Resources */ = {isa = PBXBuildFile; fileRef = 11597A2E1BC7EC4000B369E7 /* AlipaySDK.bundle */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11597A351BC7EC4000B369E7 /* iiappleResource.bundle in Resources */ = {isa = PBXBuildFile; fileRef = 11597A301BC7EC4000B369E7 /* iiappleResource.bundle */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11597A361BC7EC4000B369E7 /* iiappleSDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A311BC7EC4000B369E7 /* iiappleSDK.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                        "11597A381BC7EC5300B369E7 /* libWeChatSDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A321BC7EC4000B369E7 /* libWeChatSDK.a */; };\n" +
			                        "11597A391BC7EC6800B369E7 /* AlipaySDK.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 11597A2F1BC7EC4000B369E7 /* AlipaySDK.framework */; };\n"

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
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                        "11597A201BC7EA2800B369E7 /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                        "11597A211BC7EA2800B369E7 /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                        "11597A221BC7EA3000B369E7 /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                        "11597A241BC7EA7A00B369E7 /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                        "11597A261BC7EA8200B369E7 /* libz.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libz.tbd; path = usr/lib/libz.tbd; sourceTree = SDKROOT; };\n" +
			                        "11597A291BC7EA9500B369E7 /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                        "11597A2B1BC7EAAE00B369E7 /* libsqlite3.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libsqlite3.tbd; path = usr/lib/libsqlite3.tbd; sourceTree = SDKROOT; };\n" +
			                        "11597A2D1BC7EC2E00B369E7 /* IIApple.h */ = {isa = PBXFileReference; lastKnownFileType = sourcecode.c.h; path = IIApple.h; sourceTree = \"<group>\"; };\n" +
			                        "11597A2E1BC7EC4000B369E7 /* AlipaySDK.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AlipaySDK.bundle; sourceTree = \"<group>\"; };\n" +
			                        "11597A2F1BC7EC4000B369E7 /* AlipaySDK.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = AlipaySDK.framework; sourceTree = \"<group>\"; };\n" +
			                        "11597A301BC7EC4000B369E7 /* iiappleResource.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = iiappleResource.bundle; sourceTree = \"<group>\"; };\n" +
			                        "11597A311BC7EC4000B369E7 /* iiappleSDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = iiappleSDK.a; sourceTree = \"<group>\"; };\n" +
			                        "11597A321BC7EC4000B369E7 /* libWeChatSDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = libWeChatSDK.a; sourceTree = \"<group>\"; };\n" 

			);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "11597A391BC7EC6800B369E7 /* AlipaySDK.framework in Frameworks */,\n" +
			                         "11597A381BC7EC5300B369E7 /* libWeChatSDK.a in Frameworks */,\n" +
			                         "11597A2C1BC7EAAE00B369E7 /* libsqlite3.tbd in Frameworks */,\n" +
			                         "11597A361BC7EC4000B369E7 /* iiappleSDK.a in Frameworks */,\n" +
			                         "11597A2A1BC7EA9500B369E7 /* Security.framework in Frameworks */,\n" +
			                         "11597A281BC7EA8C00B369E7 /* libXG-SDK.a in Frameworks */,\n" +
			                         "11597A271BC7EA8200B369E7 /* libz.tbd in Frameworks */,\n" +
			                         "11597A251BC7EA7A00B369E7 /* CoreTelephony.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" 

			                         );


			// PBXGroup
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "/* Begin PBXGroup section */\n",
			                         
			                         "/* Begin PBXGroup section */\n" +
			                         "11597A1F1BC7EA2800B369E7 /* XG */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "11597A201BC7EA2800B369E7 /* XGPush.h */,\n" +
			                         "11597A211BC7EA2800B369E7 /* XGSetting.h */,\n" +
			                         ");\n" +
			                         "path = XG;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n"

			                         );


			// custom template
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "11597A221BC7EA3000B369E7 /* libXG-SDK.a */,\n" +
			                         "11597A2E1BC7EC4000B369E7 /* AlipaySDK.bundle */,\n" +
			                         "11597A2F1BC7EC4000B369E7 /* AlipaySDK.framework */,\n" +
			                         "11597A301BC7EC4000B369E7 /* iiappleResource.bundle */,\n" +
			                         "11597A311BC7EC4000B369E7 /* iiappleSDK.a */,\n" +
			                         "11597A321BC7EC4000B369E7 /* libWeChatSDK.a */,\n" +
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" 

			                         );

			// PBXGroup.frameworks
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "11597A2B1BC7EAAE00B369E7 /* libsqlite3.tbd */,\n" +
			                         "11597A291BC7EA9500B369E7 /* Security.framework */,\n" +
			                         "11597A261BC7EA8200B369E7 /* libz.tbd */,\n" +
			                         "11597A241BC7EA7A00B369E7 /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n" 

			                          );

			// classes
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n",

			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n" +
			                         "11597A2D1BC7EC2E00B369E7 /* IIApple.h */,\n" +
			                         "11597A1F1BC7EA2800B369E7 /* XG */,\n" 

			                         );

			// PBXNativeTarget
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n",
//
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n" +
//			                         "D8038DAC1BB28E3000C4CFC0 /* Embed Frameworks */,\n"
//			                         );

			// resources
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "11597A351BC7EC4000B369E7 /* iiappleResource.bundle in Resources */,\n" +
			                         "11597A331BC7EC4000B369E7 /* AlipaySDK.bundle in Resources */,\n" +
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
			EditorBuild3rd.UpdatePbx( ref t_pbx,
			                         "FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n" +
			                         "				GCC_DYNAMIC_NO_PIC = NO;\n",

			                         "ENABLE_BITCODE = NO;\n" +
			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
			                         "\"$(inherited)\",\n" +
			                         "\"$(PROJECT_DIR)\",\n" +
			                         ");\n" +
			                         "				GCC_DYNAMIC_NO_PIC = NO;\n"
			                         );

			EditorBuild3rd.UpdatePbx( ref t_pbx,
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

			// lib search
			EditorBuild3rd.UpdatePbx( ref t_pbx,
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n",
			                         
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n" +
			                         "\"$(PROJECT_DIR)\",\n" +
			                         "\"$(PROJECT_DIR)/Libraries\",\n" 
			                         );

			// flag
			EditorBuild3rd.UpdatePbx( ref t_pbx,
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
			EditorBuild3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}



//	[MenuItem("Build/Third Platform/IApple Platform/ipa", false, 9999)]
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
