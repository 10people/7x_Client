//#define DEBUG_FILTER

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class AssetsFilter : MonoBehaviour {

	private static List<DetailInfo> m_info_list = new List<DetailInfo>();

	#region Mono

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	#endregion



	#region Process

	public static void Filter(){
		#if DEBUG_FILTER
		Debug.Log( "AssetFilter.Filter()" );
		#endif

		{
			if( !LoadAssets() ){
				return;
			}
		}

		{
			FormatClassify();
		}
	}

	private static bool LoadAssets(){
		{
			m_info_list.Clear();
		}

		TextAsset t_text = (TextAsset)AssetDatabase.LoadAssetAtPath( ASSET_FILTER, typeof(TextAsset) );

		if( t_text == null ){
			Debug.LogError( "Error, Text = null." );

			return false;
		}

		StringReader t_reader = new StringReader( t_text.text );

		int t_line_count = 0;

		while( true ){
			string t_line = t_reader.ReadLine();

			if( t_line == null ){
				#if DEBUG_FILTER
				Debug.Log( "AssetFilter.File.End()" );
				#endif

				break;
			}

			t_line_count++;

			LoadLine( t_line );
		}

		Debug.Log( "AssetFilter.Detail.Count: " + m_info_list.Count );

		Debug.Log( "AssetFilter.Detail.Total.PCT: " + GetTotalPCT( m_info_list ) );

		Debug.Log( "AssetFilter.Detail.Total.Size: " + GetTotalSize( m_info_list ) );

		return true;
	}

	private static void LoadLine( string p_line ){
		DetailInfo t_info = new DetailInfo( p_line );

		m_info_list.Add( t_info );
	}

	#endregion



	#region Classify

	private static Dictionary< string, List<DetailInfo> > m_dict_classify = new Dictionary< string, List<DetailInfo> >();

	private static void FormatClassify(){
		{
			m_dict_classify.Clear();
		}

		string t_report_info = "";

		{
			LoadClassifyItems();
		}

		// detail file
		{
			OutputClassifyItems( CLASSIFY_TEX );

			OutputClassifyItems( CLASSIFY_FBX );

			OutputClassifyItems( CLASSIFY_MAT );

			OutputClassifyItems( CLASSIFY_CONTROLLER );

			OutputClassifyItems( CLASSIFY_ANIM );

//			OutputClassifyItems( CLASSIFY_SOUND );
//
//			OutputClassifyItems( CLASSIFY_SHADER );
//
//			OutputClassifyItems( CLASSIFY_SCENE );
//
//			OutputClassifyItems( CLASSIFY_SCRIPTS );
//
//			OutputClassifyItems( CLASSIFY_TEXT );

			OutputClassifyItems( CLASSIFY_OTHER );
		}

		// report
		{
			t_report_info += LogClassifyListInfo( CLASSIFY_TEX );

			t_report_info += LogClassifyListInfo( CLASSIFY_FBX );

			t_report_info += LogClassifyListInfo( CLASSIFY_MAT );

			t_report_info += LogClassifyListInfo( CLASSIFY_CONTROLLER );

			t_report_info += LogClassifyListInfo( CLASSIFY_ANIM );

			t_report_info += LogClassifyListInfo( CLASSIFY_SOUND );

//			t_report_info += LogClassifyListInfo( CLASSIFY_SHADER );

//			t_report_info += LogClassifyListInfo( CLASSIFY_SCENE );

			t_report_info += LogClassifyListInfo( CLASSIFY_SCRIPTS );

			t_report_info += LogClassifyListInfo( CLASSIFY_TEXT );

			t_report_info += LogClassifyListInfo( CLASSIFY_OTHER );
		}

		{
			t_report_info += "\n";

			t_report_info += ClassifyTexDetail();
		}

		{
			string t_path = Application.dataPath + OUTPUT_FOLDER;

			OutputFile( t_path + CLASSIFY_REPORT + ".txt", t_report_info );
		}

		{
			LogClassifyListDetail( CLASSIFY_OTHER );
		}

		{
			Debug.Log( "Filter Done." );
		}
	}

	private static void LoadClassifyItems(){
		for( int i = 0; i < m_info_list.Count; i++ ){
			DetailInfo t_info = m_info_list[ i ];

			if( t_info.IsTex() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_TEX );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsFbx() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_FBX );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsMat() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_MAT );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsController() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_CONTROLLER );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsAnim() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_ANIM );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsSound() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_SOUND );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsShader() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_SHADER );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsScene() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_SCENE );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsScripts() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_SCRIPTS );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsText() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_TEXT );

				t_list.Add( t_info );

				continue;
			}

			if( t_info.IsPrefab() ){
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_PREFAB );

				t_list.Add( t_info );

				continue;
			}

			{
				List<DetailInfo> t_list = GetClassifyList( CLASSIFY_OTHER );

				t_list.Add( t_info );
			}
		}
	}

	#endregion



	#region Tex Detail

	private static string ClassifyTexDetail(){
		string t_return_text = "";

		List<DetailInfo> t_tex_ui_list = ClassifyTexDetail( CLASSIFY_TEX_UI, TEX_UI );

		List<DetailInfo> t_tex_scene_list = ClassifyTexDetail( CLASSIFY_TEX_SCENE, TEX_SCENE );

		List<DetailInfo> t_tex_char_list = ClassifyTexDetail( CLASSIFY_TEX_CHAR, TEX_CHAR );

		List<DetailInfo> t_tex_fx_list = ClassifyTexDetail( CLASSIFY_TEX_FX, TEX_FX );

		{
			List<DetailInfo> t_all = new List<DetailInfo>();

			CombineDetail( t_all, t_tex_ui_list );

			CombineDetail( t_all, t_tex_scene_list );

			CombineDetail( t_all, t_tex_char_list );

			CombineDetail( t_all, t_tex_fx_list );
		}

		List<DetailInfo> t_other_list = new List<DetailInfo>();

		{
			List<DetailInfo> t_tex_list = GetClassifyList( CLASSIFY_TEX );

			for( int i = 0; i < t_tex_list.Count; i++ ){
				t_other_list.Add( t_tex_list[ i ] );
			}

			{
				RemoveTex( t_other_list, t_tex_ui_list );

				RemoveTex( t_other_list, t_tex_scene_list );

				RemoveTex( t_other_list, t_tex_char_list );

				RemoveTex( t_other_list, t_tex_fx_list );
			}

			{
				string t_path = Application.dataPath + OUTPUT_FOLDER;

				string t_text = GetDetailInfoListString( t_other_list );

				OutputFile( t_path + CLASSIFY_TEX_OTHER + ".txt", t_text );
			}
		}

		{
			t_return_text += LogClassifyTexDetail( CLASSIFY_TEX_UI, t_tex_ui_list );

			t_return_text += LogClassifyTexDetail( CLASSIFY_TEX_SCENE, t_tex_scene_list );

			t_return_text += LogClassifyTexDetail( CLASSIFY_TEX_CHAR, t_tex_char_list );

			t_return_text += LogClassifyTexDetail( CLASSIFY_TEX_FX, t_tex_fx_list );

			t_return_text += LogClassifyTexDetail( CLASSIFY_TEX_OTHER, t_other_list );
		}



		return t_return_text;
	}

	private static List<DetailInfo> ClassifyTexDetail( string p_tex_part, string[] p_part_tag ){
		List<DetailInfo> t_selected_list = new List<DetailInfo>();

		List<DetailInfo> t_list = GetClassifyList( CLASSIFY_TEX );

		string t_path = Application.dataPath + OUTPUT_FOLDER;

		string t_text = "";

		for( int i = 0; i < t_list.Count; i++ ){
			string t_detail_info = t_list[ i ].m_line_str;

			for( int j = 0; j < p_part_tag.Length; j++ ){
				if( t_detail_info.Contains( p_part_tag[ j ] ) ){
					t_text = t_text + t_list[ i ].GetDescString() + "\n";

					t_selected_list.Add( t_list[ i ] );
				}
			}
		}

		OutputFile( t_path + p_tex_part + ".txt", t_text );

		return t_selected_list;
	}

	private static void RemoveTex( List<DetailInfo> p_list, List<DetailInfo> p_remove ){
		for( int i = 0; i < p_remove.Count; i++ ){
			p_list.Remove( p_remove[ i ] );
		}
	}

	private static string LogClassifyTexDetail( string p_key, List<DetailInfo> p_list ){
		string t_info_str = p_key + 
			//				" Total.PCT: " + GetTotalPCT( p_list ) + " --- " + 
			"Total.Size: " + GetTotalSize( p_list ) + " M --- " + 
			"Count: " + p_list.Count + " --- ";

		#if DEBUG_FILTER
		Debug.Log( t_info_str );
		#endif

		return t_info_str + "\n";
	}

	#endregion



	#region Classify Utilities

	private static List<DetailInfo> GetClassifyList( string p_key ){
		if( m_dict_classify.ContainsKey( p_key ) ){
			return m_dict_classify[ p_key ];
		}

		{
			List<DetailInfo> t_info_list = new List<DetailInfo>();

			m_dict_classify[ p_key ] = t_info_list;

			return t_info_list;
		}
	}

	private static void OutputClassifyItems( string p_classify_key ){
		List<DetailInfo> t_list = GetClassifyList( p_classify_key );

		string t_path = Application.dataPath + OUTPUT_FOLDER;

		string t_text = GetDetailInfoListString( t_list );

		OutputFile( t_path + p_classify_key + ".txt", t_text );
	}

	private static string LogClassifyListInfo( string p_classify_key ){
		List<DetailInfo> t_list = GetClassifyList( p_classify_key );

		string t_info_str = "--- " + p_classify_key + " --- " + 
			//			"Total.PCT: " + GetTotalPCT( t_list ) + " --- " + 
			"Total.Size: " + GetTotalSize( t_list ) + " M --- " + 
			"Count: " + t_list.Count + " --- ";

		#if DEBUG_FILTER
		Debug.Log( t_info_str );
		#endif

		return t_info_str + "\n";
	}

	private static void LogClassifyListDetail( string p_classify_key ){
		List<DetailInfo> t_list = GetClassifyList( p_classify_key );

		Debug.Log( "" );

		Debug.Log( "------------ " + p_classify_key + " ------------" );

		for( int i = 0; i < t_list.Count; i++ ){
			Debug.Log( i + ": " + t_list[ i ].m_composed_path );
		}

		Debug.Log( "------------ " + p_classify_key + " ------------" );

		Debug.Log( "" );
	}

	#endregion



	#region Utilities

	private static void CombineDetail( List<DetailInfo> p_all ,List<DetailInfo> p_target ){
		for( int i = 0; i < p_target.Count; i++ ){
			if( p_all.Contains( p_target[ i ] ) ){
				Debug.LogError( "Error, Already contained: " + p_target[ i ].m_line_str );
			}
			else{
				p_all.Add( p_target[ i ] );
			}
		}
	}

	private static string GetDetailInfoListString( List<DetailInfo> p_info_list ){
		string t_text = "";

		for( int i = 0; i < p_info_list.Count; i++ ){
			DetailInfo t_info = p_info_list[ i ];

			t_text = t_text + t_info.GetDescString() + "\n";
		}

		return t_text;
	}

	private static float GetTotalPCT( List<DetailInfo> p_info_list ){
		float m_pct = 0.0f;

		for( int i = 0; i < p_info_list.Count; i++ ){
			DetailInfo t_info = p_info_list[ i ];

			m_pct = m_pct + t_info.m_percentage;
		}

		return m_pct;
	}

	private static float GetTotalSize( List<DetailInfo> p_info_list ){
		float m_size = 0.0f;

		for( int i = 0; i < p_info_list.Count; i++ ){
			DetailInfo t_info = p_info_list[ i ];

			m_size = m_size + t_info.GetSizeInMB();
		}

		return m_size;
	}

	private static void OutputFile( string p_path, string p_text ){
		FileHelper.OutputFile( p_path, p_text );
	}

	#endregion



	#region Detail

	private class DetailInfo{
		public float m_size = 0.0f;

		public string m_unit = "None";

		public float m_percentage = 0.0f;

		public string m_composed_path = "";

		public bool m_is_path_composed = false;

		public string m_path_lower = "";

		public string m_line_str = "";

		public DetailInfo( string p_line ){
			m_line_str = p_line;

			Init( p_line );
		}

		private void Init( string p_line ){
			string[] t_items = p_line.Split( ITEM_SPLITTER );

			m_size = float.Parse( t_items[ INDEX_DETAIL_SIZE ] );

			m_unit = t_items[ INDEX_SIZE_UNIT ].ToLowerInvariant().Trim();


			m_percentage = float.Parse( t_items[ INDEX_PCT ].Replace( "%", "" ) );

			{
				for( int i = INDEX_PATH; i < t_items.Length; i++ ){
					if( i == INDEX_PATH + 1 ){
						m_is_path_composed = true;
					}

					if( i == INDEX_PATH ){
						m_composed_path = t_items[ i ].Trim();
					}
					else{
						m_composed_path = m_composed_path + " " + t_items[ i ].Trim();
					}
				}
			}

			m_path_lower = m_composed_path.ToLowerInvariant();
		}

		public float GetSizeInMB(){
			if( string.Equals( m_unit, UNIT_MB ) ){
				return m_size;
			}

			if( string.Equals( m_unit, UNIT_KB ) ){
				return m_size / 1000.0f;
			}

			Debug.LogError( "Error In Unit: " + m_unit );

			return 0.0f;
		}

		public string GetSizeString(){
			if( string.Equals( m_unit, UNIT_MB ) ){
				return m_size + " M";
			}

			if( string.Equals( m_unit, UNIT_KB ) ){
				return m_size + " K";
			}

			Debug.LogError( "Error In Unit: " + m_unit );

			return "0.0 " + m_unit;
		}

		public string GetDescString(){
			return GetSizeString() + "   " + m_composed_path;
		}

		public bool IsTex(){
			for( int i = 0; i < TEX_FILES.Length; i++ ){
				if( m_path_lower.EndsWith( TEX_FILES[ i ] ) ){
					return true;
				}
			}

			return false;
		}

		public bool IsFbx(){
			for( int i = 0; i < D3_FILES.Length; i++ ){
				if( m_path_lower.EndsWith( D3_FILES[ i ] ) ){
					return true;
				}
			}

			return false;
		}

		public bool IsMat(){
			for( int i = 0; i < MAT_FILES.Length; i++ ){
				if( m_path_lower.EndsWith( MAT_FILES[ i ] ) ){
					return true;
				}
			}

			return false;
		}

		public bool IsController(){
			if( m_path_lower.EndsWith( ".controller" ) ){
				return true;
			}

			return false;
		}

		public bool IsAnim(){
			if( m_path_lower.EndsWith( ".anim" ) ){
				return true;
			}

			return false;
		}

		public bool IsSound(){
			for( int i = 0; i < SOUND_FILES.Length; i++ ){
				if( m_path_lower.EndsWith( SOUND_FILES[ i ] ) ){
					return true;
				}
			}

			return false;
		}

		public bool IsShader(){
			if( m_path_lower.EndsWith( ".shader" ) ){
				return true;
			}

			return false;
		}

		public bool IsScene(){
			if( m_path_lower.EndsWith( ".scene" ) ){
				return true;
			}

			return false;
		}

		public bool IsScripts(){
			if( m_path_lower.EndsWith( ".cs" ) ){
				return true;
			}

			if( m_path_lower.EndsWith( ".js" ) ){
				return true;
			}

			return false;
		}

		public bool IsText(){
			for( int i = 0; i < TEXT_FILES.Length; i++ ){
				if( m_path_lower.EndsWith( TEXT_FILES[ i ] ) ){
					return true;
				}
			}

			return false;
		}

		public bool IsPrefab(){
			if( m_path_lower.EndsWith( ".prefab" ) ){
				return true;
			}

			return false;
		}
	}

	#endregion



	#region CONST Common

	private const string ASSET_FILTER	= "Assets/_Temp/_ZhangYuGu/CustomResources/AssetsFilter/AssetsFilter.txt";

	private const string OUTPUT_FOLDER	= "/_Temp/_ZhangYuGu/CustomResources/AssetsFilter/";

	private const char ITEM_SPLITTER	= ' ';



	private const int INDEX_DETAIL_SIZE	= 1;

	private const int INDEX_SIZE_UNIT	= 2;

	private const int INDEX_PCT			= 3;

	private const int INDEX_PATH		= 4;



	private const string UNIT_MB		= "mb";

	private const string UNIT_KB		= "kb";

	#endregion



	#region Classify

	private const string CLASSIFY_REPORT	= "Report";

	private const string CLASSIFY_TEX		= "tex";

	private const string CLASSIFY_FBX		= "fbx";

	private const string CLASSIFY_MAT		= "mat";

	private const string CLASSIFY_CONTROLLER	= "controller";

	private const string CLASSIFY_ANIM		= "anim";

	private const string CLASSIFY_SOUND		= "sound";

	private const string CLASSIFY_SHADER	= "shader";

	private const string CLASSIFY_SCENE		= "scene";

	private const string CLASSIFY_SCRIPTS	= "scripts";

	private const string CLASSIFY_TEXT		= "text";

	private const string CLASSIFY_PREFAB	= "prefab";

	private const string CLASSIFY_OTHER		= "others";



	private const string CLASSIFY_TEX_UI	= "tex.ui";

	private const string CLASSIFY_TEX_SCENE	= "tex.scene";

	private const string CLASSIFY_TEX_CHAR	= "tex.char";

	private const string CLASSIFY_TEX_FX	= "tex.fx";

	private const string CLASSIFY_TEX_OTHER	= "tex.other";



	private static string[] TEX_FILES		= {
		".png",
		".jpg",
		".psd",
		".tga",
		".tif",
		".exr",
		".bmp",
		".dds",
	};

	private static string[] D3_FILES	= {
		".fbx",
		".obj",
	};

	private static string[] MAT_FILES	= {
		".mat",	
		".cubemap",
	};

	private static string[] SOUND_FILES	= {
		".mp3",
		".ogg",
		".wav",
	};

	private static string[] TEXT_FILES = {
		".txt",
		".xml",
	};


	private static string[] TEX_UI	= {
		"Assets/_Project/ArtAssets/UIs/",
		"Assets/Resources/_UIs/",
	};

	private static string[] TEX_SCENE	= {
		"Assets/_Project/ArtAssets/Models/Buildings_InMainCity/",
		"Assets/_Project/ArtAssets/Scenes/",
		"Assets/_Project/ArtAssets/Night Skyboxes/",
		"Assets/_Project/ArtAssets/Models/Objects/",
		"Assets/_Project/ArtAssets/Models/Terrains/",
		"Assets/_Project/ArtAssets/Models/Plants/",
		"Assets/T4MOBJ/Terrains/Texture/",
		"Assets/Standard Assets/Skyboxes/",
	};

	private static string[] TEX_CHAR	= {
		"Assets/_Project/ArtAssets/Models/Chars/",
	};

	private static string[] TEX_FX		= {
		"Assets/_Project/ArtAssets/Fx/",
		"Assets/Art/particle/",
		"Assets/FxTemporary/",
		"Assets/Effects/Textures/",
	};

	#endregion

}