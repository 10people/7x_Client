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

public class Editor_FindLargeTex : MonoBehaviour {

	private const int m_min_width	= 1024;

	private const int m_min_height	= 1024;

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Menu Item

	[MenuItem("Utility/Utilities/Find Large Tex", false, (int)EditorUtilities.MenuItemPriority.UTILITIES___FIND_LARGE_TEX)]
	static void FindLargetTex(){
//		Debug.Log( "FindLargetTex()" );

		string[] t_search_foloders = { 
			"Assets",
//			"Assets/_Project",
//			"Assets/Resources",
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

			if( t_tex.width < m_min_width && t_tex.height < m_min_height ){
				continue;
			}

			Debug.Log( t_index++ + ": " + t_path );

			if( t_tex.format == TextureFormat.DXT1 ||
			    t_tex.format == TextureFormat.DXT1Crunched ||
				t_tex.format == TextureFormat.DXT5 ||
			    t_tex.format == TextureFormat.DXT5Crunched ||
			    t_tex.format == TextureFormat.ARGB4444 ||
			    t_tex.format == TextureFormat.RGB24 ||
			    t_tex.format == TextureFormat.RGB565 ||
			    t_tex.format == TextureFormat.RGBA32 ||
			    t_tex.format == TextureFormat.ARGB32 ){
				ComponentHelper.LogTexture2D( t_tex );
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
}
