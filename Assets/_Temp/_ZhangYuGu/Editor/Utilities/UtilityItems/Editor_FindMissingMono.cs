using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using UnityEditor.Callbacks;

public class Editor_FindMissingMono : MonoBehaviour {

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Menu Item

	[MenuItem("Utility/Utilities/Find Missing", false, (int)EditorUtilities.MenuItemPriority.UTILITIES___FIND_MISSING_MONO)]
	static void FindMissingMono(){
//		Debug.Log( "FindMissingMono()" );

		string[] t_search_foloders = { 
//			"Assets",
//			"Assets/_Project",
			"Assets/Resources",
			"Assets/ResourcesCache",
		};

		string[] t_assets = AssetDatabase.FindAssets( "t:GameObject", t_search_foloders );

		int t_index = 0;

		for( int i = 0; i < t_assets.Length; i++ ){
			string t_path = AssetDatabase.GUIDToAssetPath( t_assets[ i ] );

			GameObject t_gb_prefab = (GameObject)AssetDatabase.LoadAssetAtPath( t_path, typeof(GameObject) );

			if( t_gb_prefab == null ){
				Debug.Log( "GameObject.Prefab is null, " + t_path );

				continue;
			}

//			Debug.Log( t_index + ": " + t_path );

			{
				GameObject t_gb = (GameObject)GameObject.Instantiate( t_gb_prefab );

				MonoBehaviour[] t_monos = t_gb.GetComponentsInChildren<MonoBehaviour>();

				bool t_found = false;

				for( int j = 0; j < t_monos.Length; j++ ){
					MonoBehaviour t_mono = t_monos[ j ];

					if( t_mono == null ){
						Debug.Log( t_index + " missing " + " : " + t_path );

						t_found = true;
					}

					if( t_found ){
						LogMissing( t_gb );

						t_index++;
					}
				}

				DestroyImmediate( t_gb );
			}


		}
	}

	[MenuItem("Utility/Utilities/Find Tex In Res", false, (int)EditorUtilities.MenuItemPriority.UTILITIES___FIND_TEX_IN_RES)]
	static void FindTexInRes(){
//		Debug.Log( "FindLargetTex()" );
		
		string[] t_search_foloders = { 
			"Assets/Resources",
		};
		
		string[] t_assets = AssetDatabase.FindAssets( "t:Texture2D", t_search_foloders );
		
		int t_index = 0;
		
		for( int i = 0; i < t_assets.Length; i++ ){
			string t_path = AssetDatabase.GUIDToAssetPath( t_assets[ i ] );
			
			Texture2D t_tex = (Texture2D)AssetDatabase.LoadAssetAtPath( t_path, typeof(Texture2D) );
			
			if( t_tex == null ){
				Debug.Log( "tex is null, " + t_path );
				
				continue;
			}
			
			if( StringHelper.IsContain( t_path, ".exr" ) ){
				continue;
			}
				
			Debug.Log( t_index++ + ": " + t_path );
				
			if( t_tex.format == TextureFormat.DXT1 ||
				t_tex.format == TextureFormat.DXT1Crunched ||
				t_tex.format == TextureFormat.DXT5 ||
				t_tex.format == TextureFormat.DXT5Crunched ){
				ComponentHelper.LogTexture2D( t_tex );
			}
		}
	}

	#endregion



	#region Utilities

	private static void LogMissing( GameObject p_gb ){
		if( GameObjectHelper.HaveMissingMono( p_gb ) ){
			GameObjectHelper.LogGameObjectHierarchy( p_gb, " Missing Mono: " );
		}

		int t_c_count = p_gb.transform.childCount;

		for( int i = 0; i < t_c_count; i++ ){
			Transform t_child = p_gb.transform.GetChild( i );

			LogMissing( t_child.gameObject );
		}
	}

	#endregion
}
