//#define DEBUG_VERSION

using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.12.4
 * @since:		Unity 5.l.3
 * Function:	Manage Versions to Help Build.
 * 
 * Notes:
 * 1.Data is the one building this package, never changed in update(Resources.Data).
 * 2.When SmallVersion == BigVersion, means nothing to Update.
 * 3.When SmallVersion != BigVersion, means small version update exist.
 */ 
public class VersionTool {

	public enum VersionType{
		None,
		SmallVersion,
		BigVersion,
	}

	/// Config txt dict.
	private static Dictionary<string, string> m_version_dict = new Dictionary<string, string>();
	
	public const char CONST_LINE_SPLITTER		= '=';



	private static VersionTool m_instance = null;

	public static VersionTool Instance(){
		if( m_instance == null ){
			m_instance = new VersionTool();
		}

		return m_instance;
	}

	public VersionTool(){

	}



	#region Init

	public void Init(){
		#if DEBUG_VERSION
		Debug.Log( "VersionTool.Init()" );
		#endif

		{
			Clear();
		}

		#if UNITY_EDITOR
		InitEditorAsset();
		#else
		InitBuildAsset();
		#endif
	}

	private void Clear(){
		#if DEBUG_VERSION
		Debug.Log( "Clear()" );
		#endif

		m_version_dict.Clear();
	}

	private void InitEditorAsset(){
		#if DEBUG_VERSION
		Debug.Log( "InitEditorAsset()" );
		#endif

		#if UNITY_EDITOR
		TextAsset t_text = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/Resources/_Data/Config/Version.txt", typeof(TextAsset) );

		LoadAsset( t_text );
		#endif
	}

	private void InitBuildAsset(){
		#if DEBUG_VERSION
		Debug.Log( "InitBuildAsset()" );
		#endif

		TextAsset t_text = (TextAsset)Resources.Load( "_Data/Config/Version" );

		LoadAsset( t_text );
	}

	private void LoadAsset( TextAsset p_text ){
		if (p_text == null) {
			Debug.LogError( "Error, p_text = null." );

			return;
		}

		#if DEBUG_VERSION
		Debug.Log( "LoadAsset( " + p_text.text + " )" );
		#endif

		{
			UtilityTool.LoadStringStringDict( m_version_dict, p_text, CONST_LINE_SPLITTER );
		}
	}

	#endregion

	
	
	#region Versions

	/// Small Version When Building The Package, never changed in update.
	public static string GetPackageSmallVersion(){
		return m_version_dict[ CONST_SMALL_VERSION ];
	}

	/// Big Version for this build when Building The Package., never changed in update.
	public static string GetPackageBigVersion(){
		return m_version_dict[ CONST_BIG_VERSION ];
	}
	
	#endregion



	#region Const Keys

	public const string CONST_SMALL_VERSION		= "SmallVersion";

	public const string CONST_BIG_VERSION		= "BigVersion";

	#endregion
}
