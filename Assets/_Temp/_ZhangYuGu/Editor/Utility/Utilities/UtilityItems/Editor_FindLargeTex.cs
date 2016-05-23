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

	private const int MAX_TEX_WIDTH		= 512;

	private const int MAX_TEX_HEIGHT	= 512;

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Menu Item

	[MenuItem("Utility/Utilities/Find Large Tex", false, (int)EditorUtilityUtilities.MenuItemPriority.UTILITIES___FIND_LARGE_TEX)]
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

			if( t_tex.width < MAX_TEX_WIDTH && t_tex.height < MAX_TEX_HEIGHT ){
				continue;
			}

			{
				t_index++;

				Debug.Log( t_index + " - " + t_tex.format + " - " + t_tex.width + ", " + t_tex.height + " : " + t_path );
			}

			if( t_tex.format == TextureFormat.RGBA32 ||
				t_tex.format == TextureFormat.ARGB32 ||
				t_tex.format == TextureFormat.ARGB4444 ||
				t_tex.format == TextureFormat.BGRA32 ||
				t_tex.format == TextureFormat.RGB565 ||
				t_tex.format == TextureFormat.RGBA32 ||
				t_tex.format == TextureFormat.RGBA4444 ){
				Debug.LogWarning( "Warnning Text Format: " + t_tex.format + " - " + t_tex.width + ", " + t_tex.height + " : " + t_path );
			}

			if( t_tex.format == TextureFormat.DXT1 ||
			    t_tex.format == TextureFormat.DXT1Crunched ||
				t_tex.format == TextureFormat.DXT5 ||
			    t_tex.format == TextureFormat.DXT5Crunched ){
				ComponentHelper.LogTexture2D( t_tex, "-----Warnning Text Format: " );
			}
		}

		{
			UtilityTool.UnloadUnusedAssets();
		}
	}

	[MenuItem("Utility/Utilities/Find Tex In Res", false, (int)EditorUtilityUtilities.MenuItemPriority.UTILITIES___FIND_TEX_IN_RES)]
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
				
			{
				t_index++;

				Debug.Log( t_index + " - " + t_tex.format + " - " + t_tex.width + ", " + t_tex.height + " : " + t_path );
			}

			if( t_tex.format == TextureFormat.DXT1 ||
				t_tex.format == TextureFormat.DXT1Crunched ||
				t_tex.format == TextureFormat.DXT5 ||
				t_tex.format == TextureFormat.DXT5Crunched ){
				ComponentHelper.LogTexture2D( t_tex, "-------Warnning Tex Format: " );
			}
		}

		{
			UtilityTool.UnloadUnusedAssets();
		}
	}

	[MenuItem("Utility/Utilities/Find Warnning Tex", false, (int)EditorUtilityUtilities.MenuItemPriority.UTILITIES___FIND_TEX_IN_RES)]
	static void FindWarnningTex(){
//		Debug.Log( "FindLargetTex()" );

		string[] t_search_foloders = { 
			//			"Assets/Resources",
			"Assets",
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

//			Debug.Log( t_index++ + ": " + t_path );

//			if( t_tex.format == TextureFormat.DXT1 ||
//				t_tex.format == TextureFormat.DXT1Crunched ||
//				t_tex.format == TextureFormat.DXT5 ||
//				t_tex.format == TextureFormat.DXT5Crunched ){
//
//				ComponentHelper.LogTexture2D( t_tex, t_index + " DXT : " );
//
//				t_index++;
//			}

			if( t_tex.format != TextureFormat.ARGB32 &&
				t_tex.format != TextureFormat.ARGB4444 &&
				t_tex.format != TextureFormat.BGRA32 &&
				t_tex.format != TextureFormat.ETC2_RGB &&
				t_tex.format != TextureFormat.ETC2_RGBA1 &&
				t_tex.format != TextureFormat.ETC2_RGBA8 &&
				t_tex.format != TextureFormat.ETC_RGB4 &&
				t_tex.format != TextureFormat.RGB24 &&
				t_tex.format != TextureFormat.RGB565 &&
				t_tex.format != TextureFormat.RGBA32 &&
				t_tex.format != TextureFormat.RGBA4444 &&
				t_tex.format != TextureFormat.RGBAFloat &&
				t_tex.format != TextureFormat.RGBAHalf ){
				ComponentHelper.LogTexture2D( t_tex, t_index + " ------Warnning Tex Format: " );

				t_index++;
			}
		}

		{
			UtilityTool.UnloadUnusedAssets();
		}
	}

	#endregion
}
