//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class EditorXYPlatform : MonoBehaviour {

	
	
	
	#region XY
	
	public const string XY_PLATFORM_KIT_FOLDER_NAME = "XYPlatform.framework";
	
	public const string XY_PLATFORM_KIT_BUNDLE_FOLDER_NAME = "XYPlatformResources.bundle";

	private const string Configured_XY_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_XY";


	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "XY Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_XY_XCodeProject;
		
		{
			EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.INFO_LIST_FOLDER_NAME );
			
			EditorBuildiOS3rd.ProcessFolder ( t_path, XY_PLATFORM_KIT_FOLDER_NAME );
			
			EditorBuildiOS3rd.ProcessFolder ( t_path, XY_PLATFORM_KIT_BUNDLE_FOLDER_NAME );
			
			EditorBuildiOS3rd.ProcessFile ( t_path, EditorBuildiOS3rd.CONTROLLER_FOLDER_NAME );
		}

		{
			EditorBuildiOS3rd.ProcessXG ( t_path );
		}
	}

	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_IOS_3RD_PREFIX + "XY Platform/Third Platform Project", false, 100)]
	private static void BuildXYProject(){
//		{
		//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem( EditorBuildiOS3rd.BUILD_MENU_SETTING_IOS_PREFIX + "XY", false, 1)]
	public static void BuildSettingsXY(){
		EditorBuild3rd.BuildSettings( "XY" );

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
			                                     "D8DD38561B8059FD00AFB632 /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD38531B8059FC00AFB632 /* libXG-SDK.a */; };\n" +
			                                     "D8DD385E1B805A2800AFB632 /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD385D1B805A2800AFB632 /* CoreTelephony.framework */; };\n" +
			                                     "D8DD38601B805A2E00AFB632 /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD385F1B805A2E00AFB632 /* libz.dylib */; };\n" +
			                                     "D8DD38621B805A3900AFB632 /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD38611B805A3900AFB632 /* Security.framework */; };\n" +
			                                     "D8DD38641B805A4100AFB632 /* libsqlite3.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD38631B805A4100AFB632 /* libsqlite3.dylib */; };\n" +
			                                     "D8DD38661B805A8D00AFB632 /* MobileCoreServices.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8DD38651B805A8D00AFB632 /* MobileCoreServices.framework */; };\n" +
			                                     "D8DD38671B805AA400AFB632 /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; settings = {ATTRIBUTES = (Weak, ); }; };\n" +
			                                     "D8E4A1171B8061E90098D54F /* XYPlatform.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8E4A1151B8061E90098D54F /* XYPlatform.framework */; };\n" +
			                                     "D8E4A1181B8061E90098D54F /* XYPlatformResources.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8E4A1161B8061E90098D54F /* XYPlatformResources.bundle */; };\n" +
			                                     "D8E4A1191B80624A0098D54F /* libiPhone-lib.a in Frameworks */ = {isa = PBXBuildFile; fileRef = C6184FECB0473389A7DE33E0 /* libiPhone-lib.a */; };\n" +
			                                     "D8E4A11A1B80631C0098D54F /* XYPlatform.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8E4A1151B8061E90098D54F /* XYPlatform.framework */; };\n" +
			                                     "D8E4A11B1B80631C0098D54F /* XYPlatform.framework in Embed Frameworks */ = {isa = PBXBuildFile; fileRef = D8E4A1151B8061E90098D54F /* XYPlatform.framework */; settings = {ATTRIBUTES = (CodeSignOnCopy, RemoveHeadersOnCopy, ); }; };\n" +
			                                     "D8F94E041BB241BF0056DE2D /* libstdc++.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F94E031BB241BF0056DE2D /* libstdc++.dylib */; };\n"

			                                     );

			// embed framework
//			EditorBuild3rd.UpdatePbx( ref t_pbx, 
//
//								);

			// ref
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									 "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                         "D8DD38531B8059FC00AFB632 /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "D8DD385B1B805A1300AFB632 /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "D8DD385C1B805A1300AFB632 /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "D8DD385D1B805A2800AFB632 /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "D8DD385F1B805A2E00AFB632 /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8DD38611B805A3900AFB632 /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "D8DD38631B805A4100AFB632 /* libsqlite3.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libsqlite3.dylib; path = usr/lib/libsqlite3.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8DD38651B805A8D00AFB632 /* MobileCoreServices.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = MobileCoreServices.framework; path = System/Library/Frameworks/MobileCoreServices.framework; sourceTree = SDKROOT; };\n" +
			                         "D8E4A1151B8061E90098D54F /* XYPlatform.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = XYPlatform.framework; sourceTree = \"<group>\"; };\n" +
			                         "D8E4A1161B8061E90098D54F /* XYPlatformResources.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = XYPlatformResources.bundle; sourceTree = \"<group>\"; };\n" +
			                         "D8F94E031BB241BF0056DE2D /* libstdc++.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = \"libstdc++.dylib\"; path = \"usr/lib/libstdc++.dylib\"; sourceTree = SDKROOT; };\n"

					);

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "D8F94E041BB241BF0056DE2D /* libstdc++.dylib in Frameworks */,\n" +
			                         "D8E4A1171B8061E90098D54F /* XYPlatform.framework in Frameworks */,\n" +
			                         "D8DD38671B805AA400AFB632 /* AdSupport.framework in Frameworks */,\n" +
			                         "D8DD38661B805A8D00AFB632 /* MobileCoreServices.framework in Frameworks */,\n" +
			                         "D8DD38641B805A4100AFB632 /* libsqlite3.dylib in Frameworks */,\n" +
			                         "D8DD38621B805A3900AFB632 /* Security.framework in Frameworks */,\n" +
			                         "D8DD38601B805A2E00AFB632 /* libz.dylib in Frameworks */,\n" +
			                         "D8DD385E1B805A2800AFB632 /* CoreTelephony.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "D8E4A11A1B80631C0098D54F /* XYPlatform.framework in Frameworks */,\n" +
			                         "D8DD38561B8059FD00AFB632 /* libXG-SDK.a in Frameworks */,\n"
			                         );

			// custom template
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" +
			                         "D8DD38531B8059FC00AFB632 /* libXG-SDK.a */,\n" +
			                         "D8E4A1151B8061E90098D54F /* XYPlatform.framework */,\n" +
			                         "D8E4A1161B8061E90098D54F /* XYPlatformResources.bundle */,\n"
			                         );

			// PBXGroup.frameworks
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "D8F94E031BB241BF0056DE2D /* libstdc++.dylib */,\n" +
			                         "D8DD38651B805A8D00AFB632 /* MobileCoreServices.framework */,\n" +
			                         "D8DD38631B805A4100AFB632 /* libsqlite3.dylib */,\n" +
			                         "D8DD38611B805A3900AFB632 /* Security.framework */,\n" +
			                         "D8DD385F1B805A2E00AFB632 /* libz.dylib */,\n" +
			                         "D8DD385D1B805A2800AFB632 /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n"
			                         );

			// classes
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "8AF18FE316490981007B4420 /* Unity */,\n",

			                         "8AF18FE316490981007B4420 /* Unity */,\n" +
			                         "D8DD385A1B805A1300AFB632 /* XG */,\n"
			                         );

			// PBXNativeTarget
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "033966F41B18B03000ECD701 /* ShellScript */,\n",

			                         "033966F41B18B03000ECD701 /* ShellScript */,\n" +
			                         "D8E4A11C1B80631C0098D54F /* Embed Frameworks */,\n"
									);


			
			// resources
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n" +
			                         "D8E4A1181B8061E90098D54F /* XYPlatformResources.bundle in Resources */,\n"
			                          );

			// xg
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx, 
			                         "/* End PBXGroup section */\n",

			                         "D8DD385A1B805A1300AFB632 /* XG */ = {\n" +
									 "isa = PBXGroup;\n" +
									 "children = (\n" +
									 "D8DD385B1B805A1300AFB632 /* XGPush.h */,\n" +
									 "D8DD385C1B805A1300AFB632 /* XGSetting.h */,\n" +
									 ");\n" +
									 "path = XG;\n" +
									 "sourceTree = \"<group>\";\n" +
									 "};\n" +
									 "/* End PBXGroup section */\n"
				);

			// source
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//
//			                         );

			// code sign path
			EditorBuildiOS3rd.UpdatePbx( ref t_pbx,
			                         "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;\n",

			                         "CLANG_WARN_DEPRECATED_OBJC_IMPLEMENTATIONS = YES;\n" +
			                         "CODE_SIGN_RESOURCE_RULES_PATH = iphoneos/ResourceRules.plist;\n"
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
			                         
			                         "OTHER_LDFLAGS = (\n" +
			                         "\"-ObjC\",\n" +
			                         "\"-lz\",\n" +
			                         ");"
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
