//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;

public class EditorKuaiYongPlatform : MonoBehaviour {

	#region KuaiYong
	
	public const string KUAIYONG_PLATFORM_KIT_BUNDLE_FOLDER_NAME = "AlipaySDK.bundle";

	public const string KUAIYONG_PLATFORM_KIT_FRAMEWORK_FOLDER_NAME = "xsdkFramework.framework";

	public const string KUAIYONG_PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME = "XSDKResource.bundle";

	private const string Configured_KUAIYONG_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_KuaiYong";

	
	public const string KUAIYONG_CLASS_FOLDER_NAME = "Classes/XS";
	
	public const string KUAIYONG_LIB_FILE_NAME = "libXSDK.a";
	
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "KuaiYong Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_KUAIYONG_XCodeProject;
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_H_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME );
		
		
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, KUAIYONG_PLATFORM_KIT_BUNDLE_FOLDER_NAME );

		EditorBuildiOS3rd.ProcessFolder ( t_path, KUAIYONG_PLATFORM_KIT_FRAMEWORK_FOLDER_NAME );

		EditorBuildiOS3rd.ProcessFolder ( t_path, KUAIYONG_PLATFORM_KIT_IMAGE_BUNDLE_FOLDER_NAME );
		
		
		// removed by 2.2.3
//		EditorBuild3rd.ProcessFile ( t_path, KUAIYONG_LIB_FILE_NAME );

		// removed by 2.2.3
//		EditorBuild3rd.ProcessFolder (t_path, KUAIYONG_CLASS_FOLDER_NAME );
		

		
		{
			EditorBuildiOS3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "KuaiYong Platform/Third Platform Project", false, 100)]
	private static void BuildPlatformProject(){
//		{
//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "KuaiYong", false, 1)]
	public static void BuildSettings(){
		EditorBuildiOS3rd.BuildSettings( "KuaiYong" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "KuaiYong.OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
//
//			Debug.Log( "TODO, test KuaiYong pbx." );
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
			                     	"D8038DAA1BB28E3000C4CFC0 /* xsdkFramework.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8038DA81BB28BC500C4CFC0 /* xsdkFramework.framework */; };\n" +
									"D8038DAB1BB28E3000C4CFC0 /* xsdkFramework.framework in Embed Frameworks */ = {isa = PBXBuildFile; fileRef = D8038DA81BB28BC500C4CFC0 /* xsdkFramework.framework */; settings = {ATTRIBUTES = (CodeSignOnCopy, RemoveHeadersOnCopy, ); }; };\n" +
			                        "D8DDD4F91B8081B6003A7A6F /* AlipaySDK.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8DDD4F61B8081B5003A7A6F /* AlipaySDK.bundle */; };\n" +
									"D8DDD4FA1B8081B6003A7A6F /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DDD4F71B8081B6003A7A6F /* libXG-SDK.a */; };\n" +
									"D8DDD4FB1B8081B6003A7A6F /* XSDKResource.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8DDD4F81B8081B6003A7A6F /* XSDKResource.bundle */; };\n" +
									"D8DDD5061B8081DB003A7A6F /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DDD5051B8081DB003A7A6F /* CoreTelephony.framework */; };\n" +
									"D8DDD5081B8081E0003A7A6F /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DDD5071B8081E0003A7A6F /* libz.dylib */; };\n" +
									"D8DDD50A1B8081E7003A7A6F /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DDD5091B8081E7003A7A6F /* Security.framework */; };\n" +
									"D8DDD50C1B8081ED003A7A6F /* libsqlite3.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DDD50B1B8081ED003A7A6F /* libsqlite3.dylib */; };\n" +
//									"D8DDD50F1B808247003A7A6F /* libiPhone-lib.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 9C6D43B98E9F3A34E9DA484E /* libiPhone-lib.a */; };\n" +
									"D8DDD5101B80824C003A7A6F /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; settings = {ATTRIBUTES = (Weak, ); }; };\n"
  			);

			// embed framework
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "/* End PBXCopyFilesBuildPhase section */\n",

			                         "D8038DAC1BB28E3000C4CFC0 /* Embed Frameworks */ = {\n" +
			                         "isa = PBXCopyFilesBuildPhase;\n" +
			                         "buildActionMask = 2147483647;\n" +
			                         "dstPath = \"\";\n" +
			                         "dstSubfolderSpec = 10;\n" +
			                         "files = (\n" +
			                         "D8038DAB1BB28E3000C4CFC0 /* xsdkFramework.framework in Embed Frameworks */,\n" +
			                         ");\n" +
			                         "name = \"Embed Frameworks\";\n" +
			                         "runOnlyForDeploymentPostprocessing = 0;\n" +
			                         "};\n" +
			                         "/* End PBXCopyFilesBuildPhase section */\n"
									);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									"D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                        "D8038DA81BB28BC500C4CFC0 /* xsdkFramework.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = xsdkFramework.framework; sourceTree = \"<group>\"; };\n" +
			                        "D8DDD4F61B8081B5003A7A6F /* AlipaySDK.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AlipaySDK.bundle; sourceTree = \"<group>\"; };\n" +
									"D8DDD4F71B8081B6003A7A6F /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
									"D8DDD4F81B8081B6003A7A6F /* XSDKResource.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = XSDKResource.bundle; sourceTree = \"<group>\"; };\n" +
									"D8DDD4FD1B8081C6003A7A6F /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
									"D8DDD4FE1B8081C6003A7A6F /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
									"D8DDD5051B8081DB003A7A6F /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
									"D8DDD5071B8081E0003A7A6F /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
									"D8DDD5091B8081E7003A7A6F /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
									"D8DDD50B1B8081ED003A7A6F /* libsqlite3.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libsqlite3.dylib; path = usr/lib/libsqlite3.dylib; sourceTree = SDKROOT; };\n" 
			);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "D8038DAA1BB28E3000C4CFC0 /* xsdkFramework.framework in Frameworks */,\n" +
			                         "D8DDD5101B80824C003A7A6F /* AdSupport.framework in Frameworks */,\n" +
			                         "D8DDD50C1B8081ED003A7A6F /* libsqlite3.dylib in Frameworks */,\n" +
			                         "D8DDD50A1B8081E7003A7A6F /* Security.framework in Frameworks */,\n" +
			                         "D8DDD5081B8081E0003A7A6F /* libz.dylib in Frameworks */,\n" +
			                         "D8DDD5061B8081DB003A7A6F /* CoreTelephony.framework in Frameworks */,\n" +
//			                         "56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "D8DDD4FA1B8081B6003A7A6F /* libXG-SDK.a in Frameworks */,\n"
			                         );

			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" +
			                         "D8DDD4F61B8081B5003A7A6F /* AlipaySDK.bundle */,\n" +
			                         "D8DDD4F71B8081B6003A7A6F /* libXG-SDK.a */,\n" +
			                         "D8038DA81BB28BC500C4CFC0 /* xsdkFramework.framework */,\n" +
			                         "D8DDD4F81B8081B6003A7A6F /* XSDKResource.bundle */,\n"
			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "D8DDD50B1B8081ED003A7A6F /* libsqlite3.dylib */,\n" +
			                         "D8DDD5091B8081E7003A7A6F /* Security.framework */,\n" +
			                         "D8DDD5071B8081E0003A7A6F /* libz.dylib */,\n" +
			                         "D8DDD5051B8081DB003A7A6F /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n" 
			                          );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n",

			                         "FC3D7EBE16D2621600D1BD0D /* CrashReporter.h */,\n" +
			                         "D8DDD4FC1B8081C6003A7A6F /* XG */,\n" +
			                         "D8DDD4FF1B8081C6003A7A6F /* XS */,\n"
			                         );

			// PBXNativeTarget
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "033966F41B18B03000ECD701 /* ShellScript */,\n",

			                         "033966F41B18B03000ECD701 /* ShellScript */,\n" +
			                         "D8038DAC1BB28E3000C4CFC0 /* Embed Frameworks */,\n"
			                         );

			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "D8DDD4F91B8081B6003A7A6F /* AlipaySDK.bundle in Resources */,\n" +
			                         "D8DDD4FB1B8081B6003A7A6F /* XSDKResource.bundle in Resources */,\n" +
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n" 
			                          );

			// xg xs
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "/* End PBXGroup section */",

			                         "D8DDD4FC1B8081C6003A7A6F /* XG */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "D8DDD4FD1B8081C6003A7A6F /* XGPush.h */,\n" +
			                         "D8DDD4FE1B8081C6003A7A6F /* XGSetting.h */,\n" +
			                         ");\n" +
			                         "path = XG;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "/* End PBXGroup section */\n"
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

			// lib search
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "LIBRARY_SEARCH_PATHS = (\n" +
			                         "					\"$(inherited)\",\n" +
			                         "					\"\\\"$(SRCROOT)\\\"\",\n" +
			                         "					\"\\\"$(SRCROOT)/Libraries\\\"\",\n" +
			                         "				);\n",
			                         
			                         "LD_RUNPATH_SEARCH_PATHS = \"$(inherited) @executable_path/Frameworks\";\n" +
			                         "LIBRARY_SEARCH_PATHS = (\n" +
			                         "\"$(inherited)\",\n" +
			                         "\"$(SRCROOT)\",\n" +
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n" +
			                         "\"$(PROJECT_DIR)\",\n" +
			                         "\"$(PROJECT_DIR)/Libraries\",\n" +
			                         ");\n"
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



//	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_PREFIX + "KuaiYong Platform/ipa", false, 9999)]
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
