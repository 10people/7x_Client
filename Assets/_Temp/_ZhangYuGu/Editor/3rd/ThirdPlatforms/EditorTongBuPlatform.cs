//#define DEBUG_PLATFORM

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class EditorTongBuPlatform : MonoBehaviour {

	
	#region TongBu
	
	public const string TONGBU_PLATFORM_KIT_FOLDER_NAME = "TBPlatform.framework";
	
	public const string TONGBU_PLATFORM_KIT_BUNDLE_FOLDER_NAME = "TBPlatformResource.bundle";

	private const string Configured_TongBu_XCodeProject = "/workspace/xcode_workspace/xcode_qixiong_TongBu";


	[MenuItem("Build/Third Platform/TongBu Platform/Third Platform", false, 1)]
	public static void BuildPlatform(){
		string t_path = PathHelper.GetMacHome() + Configured_TongBu_XCodeProject;
		
		EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.INFO_LIST_FOLDER_NAME );
		
		EditorBuild3rd.ProcessFolder ( t_path, TONGBU_PLATFORM_KIT_FOLDER_NAME );
		
		EditorBuild3rd.ProcessFolder ( t_path, TONGBU_PLATFORM_KIT_BUNDLE_FOLDER_NAME );
		
		EditorBuild3rd.ProcessFile ( t_path, EditorBuild3rd.CONTROLLER_FOLDER_NAME );



		{
			EditorBuild3rd.ProcessXG ( t_path );
		}
	}


	[MenuItem("Build/Third Platform/TongBu Platform/Third Platform Project", false, 100)]
	private static void BuildPlatformProject(){
//		{
//			BuildPlatform();
//		}
		
		string t_built_projectpath = PathHelper.GetXCodeProjectFullPath();
		
		OnProcessPbx( BuildTarget.iOS, t_built_projectpath );
	}
	
	[MenuItem("Build/Settings/TongBu", false, 1)]
	public static void BuildSettings(){
		EditorBuild3rd.BuildSettings( "TongBu" );

		AssetDatabase.Refresh();
	}

	/// Auto Build for final release.
	public static void OnPostBuildPlatform( BuildTarget p_target, string p_path_to_built_project ){
		#if DEBUG_PLATFORM
		Debug.Log ( "TongBu.OnPostBuildPlatform()" );
		#endif

		{
			BuildPlatform();
		}

//		if ( true ) {
//			#if DEBUG_PLATFORM
//			Debug.Log( "keep pbx the same." );
//			#endif
//
//			Debug.Log( "TODO, test TongBu pbx." );
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
			                                     "D8A8DBC11BB162AB00721ADC /* TBPlatform.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A4E1B8301CA00D287B2 /* TBPlatform.framework */; };\n" +
			                                     "D8F67A501B8301CA00D287B2 /* libXG-SDK.a in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A4D1B8301CA00D287B2 /* libXG-SDK.a */; };\n" +
			                                     "D8F67A521B8301CA00D287B2 /* TBPlatformResource.bundle in Resources */ = {isa = PBXBuildFile; fileRef = D8F67A4F1B8301CA00D287B2 /* TBPlatformResource.bundle */; };\n" +
			                                     "D8F67A571B8301FC00D287B2 /* CoreTelephony.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A561B8301FC00D287B2 /* CoreTelephony.framework */; };\n" +
			                                     "D8F67A591B83020300D287B2 /* libz.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A581B83020300D287B2 /* libz.dylib */; };\n" +
			                                     "D8F67A5B1B83020C00D287B2 /* Security.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A5A1B83020C00D287B2 /* Security.framework */; };\n" +
			                                     "D8F67A5D1B83021200D287B2 /* libsqlite3.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A5C1B83021200D287B2 /* libsqlite3.dylib */; };\n" +
			                                     "D8F67A5E1B83023F00D287B2 /* AdSupport.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 8A1B656D185FB650009F37BD /* AdSupport.framework */; settings = {ATTRIBUTES = (Weak, ); }; };\n" +
			                                     "D8F67A601B83025500D287B2 /* MessageUI.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A5F1B83025500D287B2 /* MessageUI.framework */; };\n" +
			                                     "D8F67A621B83038A00D287B2 /* libxml2.dylib in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A611B83038A00D287B2 /* libxml2.dylib */; };\n" +
			                                     "D8F67A641B83039600D287B2 /* MobileCoreServices.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A631B83039600D287B2 /* MobileCoreServices.framework */; };\n" +
			                                     "D8F67A661B83039F00D287B2 /* CoreData.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = D8F67A651B83039F00D287B2 /* CoreData.framework */; };\n" 
			                                     );

			// ref
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n",

									 "D8A1C7250E80637F000160D3 /* RegisterMonoModules.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; name = RegisterMonoModules.h; path = Libraries/RegisterMonoModules.h; sourceTree = SOURCE_ROOT; };\n" +
			                         "D8F67A4D1B8301CA00D287B2 /* libXG-SDK.a */ = {isa = PBXFileReference; lastKnownFileType = archive.ar; path = \"libXG-SDK.a\"; sourceTree = \"<group>\"; };\n" +
			                         "D8F67A4E1B8301CA00D287B2 /* TBPlatform.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; path = TBPlatform.framework; sourceTree = \"<group>\"; };\n" +
			                         "D8F67A4F1B8301CA00D287B2 /* TBPlatformResource.bundle */ = {isa = PBXFileReference; lastKnownFileType = \"wrapper.plug-in\"; path = TBPlatformResource.bundle; sourceTree = \"<group>\"; };\n" +
			                         "D8F67A541B8301DB00D287B2 /* XGPush.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGPush.h; sourceTree = \"<group>\"; };\n" +
			                         "D8F67A551B8301DB00D287B2 /* XGSetting.h */ = {isa = PBXFileReference; fileEncoding = 4; lastKnownFileType = sourcecode.c.h; path = XGSetting.h; sourceTree = \"<group>\"; };\n" +
			                         "D8F67A561B8301FC00D287B2 /* CoreTelephony.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreTelephony.framework; path = System/Library/Frameworks/CoreTelephony.framework; sourceTree = SDKROOT; };\n" +
			                         "D8F67A581B83020300D287B2 /* libz.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libz.dylib; path = usr/lib/libz.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8F67A5A1B83020C00D287B2 /* Security.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Security.framework; path = System/Library/Frameworks/Security.framework; sourceTree = SDKROOT; };\n" +
			                         "D8F67A5C1B83021200D287B2 /* libsqlite3.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libsqlite3.dylib; path = usr/lib/libsqlite3.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8F67A5F1B83025500D287B2 /* MessageUI.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = MessageUI.framework; path = System/Library/Frameworks/MessageUI.framework; sourceTree = SDKROOT; };\n" +
			                         "D8F67A611B83038A00D287B2 /* libxml2.dylib */ = {isa = PBXFileReference; lastKnownFileType = \"compiled.mach-o.dylib\"; name = libxml2.dylib; path = usr/lib/libxml2.dylib; sourceTree = SDKROOT; };\n" +
			                         "D8F67A631B83039600D287B2 /* MobileCoreServices.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = MobileCoreServices.framework; path = System/Library/Frameworks/MobileCoreServices.framework; sourceTree = SDKROOT; };\n" +
			                         "D8F67A651B83039F00D287B2 /* CoreData.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreData.framework; path = System/Library/Frameworks/CoreData.framework; sourceTree = SDKROOT; };\n"
			                         );

			// PBXContainerItemProxy

			// PBXTargetDependency

			// PBXFrameworksBuildPhase.frameworks
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n",

			                         "D8A8DBC11BB162AB00721ADC /* TBPlatform.framework in Frameworks */,\n" +
			                         "D8F67A661B83039F00D287B2 /* CoreData.framework in Frameworks */,\n" +
			                         "D8F67A641B83039600D287B2 /* MobileCoreServices.framework in Frameworks */,\n" +
			                         "D8F67A621B83038A00D287B2 /* libxml2.dylib in Frameworks */,\n" +
			                         "D8F67A601B83025500D287B2 /* MessageUI.framework in Frameworks */,\n" +
			                         "D8F67A5E1B83023F00D287B2 /* AdSupport.framework in Frameworks */,\n" +
			                         "D8F67A5D1B83021200D287B2 /* libsqlite3.dylib in Frameworks */,\n" +
			                         "D8F67A5B1B83020C00D287B2 /* Security.framework in Frameworks */,\n" +
			                         "D8F67A591B83020300D287B2 /* libz.dylib in Frameworks */,\n" +
			                         "D8F67A571B8301FC00D287B2 /* CoreTelephony.framework in Frameworks */,\n" +
			                         "AA5D99871AFAD3C800B27605 /* CoreText.framework in Frameworks */,\n" +
			                         "D8F67A501B8301CA00D287B2 /* libXG-SDK.a in Frameworks */,\n"
			                         );

			// custom template
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n",

			                         "83B2574E0E63025400468741 /* libiconv.2.dylib */,\n" +
			                         "D8F67A4D1B8301CA00D287B2 /* libXG-SDK.a */,\n" +
			                         "D8F67A4E1B8301CA00D287B2 /* TBPlatform.framework */,\n" +
			                         "D8F67A4F1B8301CA00D287B2 /* TBPlatformResource.bundle */,\n" 
			                         );

			// PBXGroup.frameworks
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n",

			                         "D8F67A651B83039F00D287B2 /* CoreData.framework */,\n" +
			                         "D8F67A631B83039600D287B2 /* MobileCoreServices.framework */,\n" +
			                         "D8F67A611B83038A00D287B2 /* libxml2.dylib */,\n" +
			                         "D8F67A5F1B83025500D287B2 /* MessageUI.framework */,\n" +
			                         "D8F67A5C1B83021200D287B2 /* libsqlite3.dylib */,\n" +
			                         "D8F67A5A1B83020C00D287B2 /* Security.framework */,\n" +
			                         "D8F67A581B83020300D287B2 /* libz.dylib */,\n" +
			                         "D8F67A561B8301FC00D287B2 /* CoreTelephony.framework */,\n" +
			                         "AA5D99861AFAD3C800B27605 /* CoreText.framework */,\n"
			                          );

			// classes
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "8A5C148F174E662D0006EB36 /* PluginBase */,\n",

			                         "8A5C148F174E662D0006EB36 /* PluginBase */,\n" +
			                         "D8F67A531B8301DB00D287B2 /* XG */,\n"
			                         );

			// resources
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n",

			                         "56C56C9817D6015200616839 /* Images.xcassets in Resources */,\n" +
			                         "D8F67A521B8301CA00D287B2 /* TBPlatformResource.bundle in Resources */,\n"
			                          );

			// xg
			EditorBuild3rd.UpdatePbx( ref t_pbx, 
			                         "/* End PBXGroup section */\n",

			                         "D8F67A531B8301DB00D287B2 /* XG */ = {\n" +
									 "isa = PBXGroup;\n" +
									 "children = (\n" +
									 "D8F67A541B8301DB00D287B2 /* XGPush.h */,\n" +
									 "D8F67A551B8301DB00D287B2 /* XGSetting.h */,\n" +
									 ");\n" +
									 "path = XG;\n" +
									 "sourceTree = \"<group>\";\n" +
									 "};\n" +
									 "/* End PBXGroup section */\n"
									);

			// source
//			EditorBuild3rd.UpdatePbx( ref t_pbx,
//
//
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
			                         
			                         "OTHER_LDFLAGS = (\n" +
			                         "\"-ObjC\",\n" +
			                         "\"-lz\",\n" +
			                         ");\n"
			                         );



			//Debug.Log( "Processed t_pbx: " + t_pbx );
		}



		{
			// save
			EditorBuild3rd.SavePbx( p_path_to_built_project, t_pbx );
		}
	}

		
	#endregion
}
