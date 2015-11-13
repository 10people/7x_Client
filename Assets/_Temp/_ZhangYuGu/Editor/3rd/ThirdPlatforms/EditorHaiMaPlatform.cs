//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class EditorHaiMaPlatform : MonoBehaviour {

	
	
	
	#region XY
	
	public const string PLATFORM_KIT_FOLDER_NAME = "ZHPaySDK";
	
	private const string Configured_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_HaiMa";


	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "HaiMa Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_XCodeProject;
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFolder ( t_path, PLATFORM_KIT_FOLDER_NAME );
		
		EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME );

		{
			EditorBuildiOS3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "HaiMa Platform/Third Platform Project", false, 100)]
	private static void BuildHaiMaProject(){
//		{
		//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "HaiMa", false, 1)]
	public static void BuildSettingsHaiMa(){
		EditorBuildiOS3rd.BuildSettings( "HaiMa" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
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


			                                     "110E9FDE1BC75BB700BA819A /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 110E9FDD1BC75BB700BA819A /* libXG-SDK.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A11BC6745200D5EC9A /* ZHPayBundle.bundle in Resources */ = {isa = PBXBuildFile; fileRef = 115501931BC6745200D5EC9A /* ZHPayBundle.bundle */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A21BC6745200D5EC9A /* AlipaySDK.bundle in Resources */ = {isa = PBXBuildFile; fileRef = 115501961BC6745200D5EC9A /* AlipaySDK.bundle */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A31BC6745200D5EC9A /* AlipaySDK.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501971BC6745200D5EC9A /* AlipaySDK.framework */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A41BC6745200D5EC9A /* libUPPayPlugin.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501991BC6745200D5EC9A /* libUPPayPlugin.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A51BC6745200D5EC9A /* libWeChatSDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = 1155019D1BC6745200D5EC9A /* libWeChatSDK.a */; settings = {ASSET_TAGS = (); }; };\n" +
			                                     "115501A71BC6747D00D5EC9A /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; };\n" +
			                                     "115501A91BC6748F00D5EC9A /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501A81BC6748F00D5EC9A /* CoreTelephony.framework */; };\n" +
			                                     "115501AB1BC6749D00D5EC9A /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501AA1BC6749D00D5EC9A /* Security.framework */; };\n" +
			                                     "115501B21BC674F900D5EC9A /* libstdc++.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501AC1BC674A900D5EC9A /* libstdc++.tbd */; };\n" +
			                                     "115501B31BC6750000D5EC9A /* libsqlite3.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501AE1BC674BA00D5EC9A /* libsqlite3.tbd */; };\n" +
			                                     "115501B41BC6750900D5EC9A /* libz.tbd in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501B01BC674CB00D5EC9A /* libz.tbd */; };\n" +
			                                     "115501B51BC67A8E00D5EC9A /* ZHPayU3D.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501A01BC6745200D5EC9A /* ZHPayU3D.framework */; };\n" +
			                                     "115501B71BC67B7700D5EC9A /* IOKit.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 115501B61BC67B7700D5EC9A /* IOKit.framework */; settings = {ASSET_TAGS = (); }; };\n"
			                                     );

			// embed framework
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//
//								);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									 "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                         "110E9FDD1BC75BB700BA819A /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "115501901BC6744500D5EC9A /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "115501911BC6744500D5EC9A /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "115501931BC6745200D5EC9A /* ZHPayBundle.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = ZHPayBundle.bundle; sourceTree = \"<group>\"; };\n" +
			                         "115501961BC6745200D5EC9A /* AlipaySDK.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = AlipaySDK.bundle; sourceTree = \"<group>\"; };\n" +
			                         "115501971BC6745200D5EC9A /* AlipaySDK.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = AlipaySDK.framework; sourceTree = \"<group>\"; };\n" +
			                         "115501991BC6745200D5EC9A /* libUPPayPlugin.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = libUPPayPlugin.a; sourceTree = \"<group>\"; };\n" +
			                         "1155019A1BC6745200D5EC9A /* UPPayPlugin.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = UPPayPlugin.h; sourceTree = \"<group>\"; };\n" +
			                         "1155019B1BC6745200D5EC9A /* UPPayPluginDelegate.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = UPPayPluginDelegate.h; sourceTree = \"<group>\"; };\n" +
			                         "1155019D1BC6745200D5EC9A /* libWeChatSDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = libWeChatSDK.a; sourceTree = \"<group>\"; };\n" +
			                         "1155019E1BC6745200D5EC9A /* WXApi.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = WXApi.h; sourceTree = \"<group>\"; };\n" +
			                         "1155019F1BC6745200D5EC9A /* WXApiObject.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = WXApiObject.h; sourceTree = \"<group>\"; };\n" +
			                         "115501A01BC6745200D5EC9A /* ZHPayU3D.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = ZHPayU3D.framework; sourceTree = \"<group>\"; };\n" +
			                         "115501A81BC6748F00D5EC9A /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "115501AA1BC6749D00D5EC9A /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "115501AC1BC674A900D5EC9A /* libstdc++.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = \"libstdc++.tbd\"; path = \"usr/lib/libstdc++.tbd\"; sourceTree = SDKROOT; };\n" +
			                         "115501AE1BC674BA00D5EC9A /* libsqlite3.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libsqlite3.tbd; path = usr/lib/libsqlite3.tbd; sourceTree = SDKROOT; };\n" +
			                         "115501B01BC674CB00D5EC9A /* libz.tbd */ = {isa = PBXFileReference; lastKnownFileType = \"sourcecode.text-based-dylib-definition\"; name = libz.tbd; path = usr/lib/libz.tbd; sourceTree = SDKROOT; };\n" +
			                         "115501B61BC67B7700D5EC9A /* IOKit.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = IOKit.framework; path = Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/System/Library/Frameworks/IOKit.framework; sourceTree = DEVELOPER_DIR; };\n"

					);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "110E9FDE1BC75BB700BA819A /* libXG-SDK.a in Frameworks */,\n" +
			                         "115501B71BC67B7700D5EC9A /* IOKit.framework in Frameworks */,\n" +
			                         "115501B51BC67A8E00D5EC9A /* ZHPayU3D.framework in Frameworks */,\n" +
			                         "115501B41BC6750900D5EC9A /* libz.tbd in Frameworks */,\n" +
			                         "115501B31BC6750000D5EC9A /* libsqlite3.tbd in Frameworks */,\n" +
			                         "115501B21BC674F900D5EC9A /* libstdc++.tbd in Frameworks */,\n" +
			                         "115501AB1BC6749D00D5EC9A /* Security.framework in Frameworks */,\n" +
			                         "115501A91BC6748F00D5EC9A /* CoreTelephony.framework in Frameworks */,\n" +
			                         "115501A71BC6747D00D5EC9A /* AdSupport.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "115501A31BC6745200D5EC9A /* AlipaySDK.framework in Frameworks */,\n" +
			                         "115501A41BC6745200D5EC9A /* libUPPayPlugin.a in Frameworks */,\n" +
			                         "115501A51BC6745200D5EC9A /* libWeChatSDK.a in Frameworks */,\n"
			                         );

			
			// PBXGroup
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "/* Begin PBXGroup section */\n",
			                         
			                         "/* Begin PBXGroup section */\n" +
			                         "1155018F1BC6744500D5EC9A /* XG */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "115501901BC6744500D5EC9A /* XGPush.h */,\n" +
			                         "115501911BC6744500D5EC9A /* XGSetting.h */,\n" +
			                         ");\n" +
			                         "path = XG;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "115501921BC6745200D5EC9A /* ZHPaySDK */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "115501931BC6745200D5EC9A /* ZHPayBundle.bundle */,\n" +
			                         "115501941BC6745200D5EC9A /* ZHPayDependence */,\n" +
			                         "115501A01BC6745200D5EC9A /* ZHPayU3D.framework */,\n" +
			                         ");\n" +
			                         "path = ZHPaySDK;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "115501941BC6745200D5EC9A /* ZHPayDependence */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "115501951BC6745200D5EC9A /* AliPay */,\n" +
			                         "115501981BC6745200D5EC9A /* UnionPay */,\n" +
			                         "1155019C1BC6745200D5EC9A /* WeChat */,\n" +
			                         ");\n" +
			                         "path = ZHPayDependence;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "115501951BC6745200D5EC9A /* AliPay */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "115501961BC6745200D5EC9A /* AlipaySDK.bundle */,\n" +
			                         "115501971BC6745200D5EC9A /* AlipaySDK.framework */,\n" +
			                         ");\n" +
			                         "path = AliPay;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "115501981BC6745200D5EC9A /* UnionPay */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "115501991BC6745200D5EC9A /* libUPPayPlugin.a */,\n" +
			                         "1155019A1BC6745200D5EC9A /* UPPayPlugin.h */,\n" +
			                         "1155019B1BC6745200D5EC9A /* UPPayPluginDelegate.h */,\n" +
			                         ");\n" +
			                         "path = UnionPay;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n" +
			                         "1155019C1BC6745200D5EC9A /* WeChat */ = {\n" +
			                         "isa = PBXGroup;\n" +
			                         "children = (\n" +
			                         "1155019D1BC6745200D5EC9A /* libWeChatSDK.a */,\n" +
			                         "1155019E1BC6745200D5EC9A /* WXApi.h */,\n" +
			                         "1155019F1BC6745200D5EC9A /* WXApiObject.h */,\n" +
			                         ");\n" +
			                         "path = WeChat;\n" +
			                         "sourceTree = \"<group>\";\n" +
			                         "};\n"
			                         );

			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "110E9FDD1BC75BB700BA819A /* libXG-SDK.a */,\n" +
			                         "115501B61BC67B7700D5EC9A /* IOKit.framework */,\n" +
			                         "115501921BC6745200D5EC9A /* ZHPaySDK */,\n" +
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n"
			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "115501B01BC674CB00D5EC9A /* libz.tbd */,\n" +
			                         "115501AE1BC674BA00D5EC9A /* libsqlite3.tbd */,\n" +
			                         "115501AC1BC674A900D5EC9A /* libstdc++.tbd */,\n" +
			                         "115501AA1BC6749D00D5EC9A /* Security.framework */,\n" +
			                         "115501A81BC6748F00D5EC9A /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n"
			                         );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "8AF18FE316490981007B4420 /* Unity */,\n",

			                         "8AF18FE316490981007B4420 /* Unity */,\n" +
			                         "1155018F1BC6744500D5EC9A /* XG */,\n"
			                         );

			// PBXNativeTarget
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n",
//
//			                         "033966F41B18B03000ECD701 /* ShellScript */,\n" +
//			                         "D8E4A11C1B80631C0098D54F /* Embed Frameworks */,\n"
//									);


			
			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "115501A21BC6745200D5EC9A /* AlipaySDK.bundle in Resources */,\n" +
			                         "115501A11BC6745200D5EC9A /* ZHPayBundle.bundle in Resources */,\n" +
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n"
			                          );



			// source
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//
//			                         );

			// code sign path
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//			                         "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;\n",
//
//			                         "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;\n" +
//			                         "CODE_SIGN_RESOURCE_RULES_PATH = iphoneos/ResourceRules.plist;\n"
//			                         );

			// framework search
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n" +
			                         "				GCC_DYNAMIC_NO_PIC = NO;\n",
			                         
			                         "ENABLE_BITCODE = NO;\n" +
			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
			                         "\"$(inherited)\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK/ZHPayDependence/AliPay\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK\",\n" +
			                         ");\n" +
			                         "GCC_DYNAMIC_NO_PIC = NO;\n"
			                         );

			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n" +
			                         "				GCC_ENABLE_CPP_EXCEPTIONS = YES;\n",
			                         
			                         "ENABLE_BITCODE = NO;\n" +
			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
			                         "\"$(inherited)\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK/ZHPayDependence/AliPay\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK\",\n" +
			                         ");\n" +
			                         "GCC_ENABLE_CPP_EXCEPTIONS = YES;\n"
			                         );
			
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "				FRAMEWORK_SEARCH_PATHS = \"$(inherited)\";\n",
//			                         
//			                         "COPY_PHASE_STRIP = YES;\n" +
//			                         "ENABLE_BITCODE = NO;\n" +
//			                         "FRAMEWORK_SEARCH_PATHS = (\n" +
//			                         "\"$(inherited)\",\n" +
//			                         "\"$(PROJECT_DIR)/ZHPaySDK/ZHPayDependence/AliPay\",\n" +
//			                         "\"$(PROJECT_DIR)/ZHPaySDK\",\n" +
//									 ");\n"
//			                         );

			// lib search
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n",
			                         
			                         "\"\\\"$(SRCROOT)/Libraries\\\"\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK/ZHPayDependence/UnionPay\",\n" +
			                         "\"$(PROJECT_DIR)/ZHPaySDK/ZHPayDependence/WeChat\",\n" +
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
