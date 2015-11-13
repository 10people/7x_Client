//#define LOG_LOADING


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.1
 * @since:		Unity 4.5.3
 * Function:	Cache reference for preload images.
 * 
 * Notes:
 * 1.Exist in 3 scenes: Bundle_Loader, Loading, Login.
 * 2.Built-in Resources, Never Updated until big version update.
 */ 
public class StaticLoading : MonoBehaviour {

	public GameObject m_loading_bg_root;

	public UITexture m_loading_bg;

	public UILabel m_lb_tips;

	private static StaticLoading m_instance = null;

	public static StaticLoading Instance(){
		if( m_instance == null ){
			Debug.LogError( "Error, StaticLoadingBg = null." );
		}

		return m_instance;
	}


	#region Mono

	void Awake(){
//		Debug.Log( "StaticLoadingBg.Awake()" );

		SceneManager.SetSceneState( SceneManager.SceneState.Loading );

		LoadingHelper.ClearDontDestroyOnLoads();

		m_instance = this;

		ClearLoadingInfo( m_loading_sections );

		SetTipsText();
	}

	void OnGUI(){
		m_last_frame_time = Time.realtimeSinceStartup;
	}

	/** Notes:
	 * bug fixed, NEVER destroy here.
	 * destroy in ManualDestroy.
	 */
	void OnDestroy(){
//		Debug.Log( "StaticLoadingBg.OnDestroy()" );

	}

	#endregion



	#region Destroy

	public void ManualDestroy(){
		if ( m_loading_bg_root != null ) {
			m_loading_bg_root.SetActive (false);
		}
		else {
			Debug.Log( "Should not be here." );
		}


		Destroy( m_loading_bg_root );

		m_instance = null;
		
//		LogLoadingInfo();
		
		ClearLoadingInfo( m_loading_sections );
	}

	#endregion



	#region Tips

	private LanguageTemplate.Text m_begin_id = LanguageTemplate.Text.LOADING_TIPS_1;

	private LanguageTemplate.Text m_end_id = LanguageTemplate.Text.LOADING_TIPS_18;

	private void SetTipsText(){
		int t_begin_id = (int)m_begin_id;

		int t_end_id = (int)m_end_id;

		int t_target_id = (int)UtilityTool.GetRandom( t_begin_id, t_end_id );

		string t_text = LanguageTemplate.GetText( t_target_id );

		m_lb_tips.text = t_text;

//		Debug.Log( "new tips text: " + t_text );
	}


	#endregion



	#region Loading

	public class LoadingSection{
		public string m_section_name;

		private float m_weight = 1.0f;

		private float m_percentage = 0.0f;
		
		private int m_items_loaded = 0;
		
		// -1 means unknown.
		private int m_item_count = -1;
		
		public LoadingSection( string p_section_name, float p_weight = 1.0f, int p_item_count = -1 ){
			m_section_name = p_section_name;

			m_weight = p_weight;

			SetTotalCount( p_item_count );
		}

		public void UpdatePercentage( float p_percentage ){
			m_percentage = p_percentage;
		}

		public void ItemLoaded(){
			m_items_loaded++;
		}

		public float GetLoadedWeight(){
			float t_local_weight = 0;

			if( m_item_count < 0 ){
				t_local_weight = m_percentage * m_weight;
			}
			else if( m_item_count > 0 ){
				t_local_weight = m_items_loaded * 1.0f / m_item_count * m_weight;
			}
			else{
				Debug.LogError( "Error In GetLoadedWeight." );

				Debug.Log( "Loading Section: " + 
				          m_items_loaded + " / " + m_item_count + " - " + 
				          m_weight + "   - " +
				          "   " + m_section_name );

				t_local_weight = 0.0f;
			}

			t_local_weight = Mathf.Clamp( t_local_weight, 0, m_weight );

			return t_local_weight;
		}

		public float GetTotalWeight(){
			return m_weight;
		}

        // reset total resources item count
		public void SetTotalCount( int p_item_count ){
//			Debug.Log( m_section_name + ".SetTotalCount( " + p_item_count + " )" );
		
			m_item_count = p_item_count;
		}

		public void Log(){
			Debug.Log( "Loading Section( items.percent: " + 
			          m_items_loaded + " / " + m_item_count + 
			          " -    weight: " + m_weight + "   -    weight.percent: " +
			          GetLoadedWeight() + " / " + GetTotalWeight() +
			          "   " + m_section_name );
		}
	}

	public static List<LoadingSection> m_loading_sections = new List<LoadingSection>();

	private static string m_cur_loading_asset = "";

	private static float m_last_frame_time = 0.0f;

	public static bool IsReadyToLoadNextAsset(){
		if( m_instance == null ){
//			Debug.Log( "Not In Loading Scene." );

			return true;
		}
		else{
			float t_delta = Time.realtimeSinceStartup - m_last_frame_time;

			if( t_delta > ConfigTool.GetFloat( ConfigTool.CONST_LOADING_INTERVAL ) ){
				return false;
			} 
			else{
				return true;
			}
		}
	}

	#endregion



	#region Update Loading

	public static void ItemLoaded( List<LoadingSection> p_list, string p_section_name, string p_item_name = "" ){
		#if LOG_LOADING
	//	Debug.Log( "ItemLoaded( " + p_section_name + ": " + p_item_name + " )" );
		#endif

		LoadingSection t_section = GetSection( p_list, p_section_name );

		if( t_section == null ){
			t_section = InitSectionInfo( p_list, p_section_name );
			
//			Debug.Log( "StaticLoading.ItemLoaded( Section Not Found )" );
		}

		t_section.ItemLoaded();

		SetCurLoading( p_item_name );
	}

	public static void UpdatePercentag( List<LoadingSection> p_list, string p_section_name, float p_percentage ){
//		Debug.Log( "UpdatePercentag( " + p_section_name + " - " + p_percentage + " )" );

		LoadingSection t_section = GetSection( p_list, p_section_name );
		
		if( t_section == null ){
			t_section = InitSectionInfo( p_list, p_section_name );
			
//			Debug.Log( "StaticLoading.UpdatePercentag( Section Not Found )" );
		}
		
		t_section.UpdatePercentage( p_percentage );
	}

	private static void SetCurLoading( string p_cur_loading_name ){
		#if LOG_LOADING
		//Debug.Log( "SetCurLoading( " + p_cur_loading_name + " )" );
		#endif

		m_cur_loading_asset = p_cur_loading_name;

		EnterNextScene.SetLoadingAssetChanged( true );
	}

	public static string GetCurLoading(){
		return m_cur_loading_asset;
	}

	#endregion



	#region Utility

	public static LoadingSection InitSectionInfo( List<LoadingSection> p_list, string p_section_name, float p_weight = 1.0f, int p_item_count = -1 ){
		LoadingSection t_section = GetSection( p_list, p_section_name );
		
		if( t_section == null ){
            #if LOG_LOADING
          //  Debug.Log("InitSectionInfo: " + p_section_name + ", " + p_weight + ", " + p_item_count );
            #endif

            t_section = new LoadingSection( p_section_name, p_weight, p_item_count );
			
			p_list.Add( t_section );
		}
		else{
			Debug.LogError( "Error, Loading Section Already Exist." );
		}
		
		return t_section;
	}

	public static LoadingSection GetSection( List<LoadingSection> p_list, string p_section_name ){
		if( string.IsNullOrEmpty( p_section_name ) ){
			return null;
		}
		
		if( p_list.Count == 0 ){
			return null;
		}
		
		for( int i = 0; i < p_list.Count; i++ ){
			LoadingSection t_item = p_list[ i ];
			
			if( t_item.m_section_name == p_section_name ){
				return t_item;
			}
		}
		
		return null;
	}

	public static float GetLoadingPercentage( List<LoadingSection> p_list ){
		float t_loaded = 0.0f;
		
		float t_total = 0.0f;
		
		for( int i = 0; i < p_list.Count; i++ ){
			LoadingSection t_section = p_list[ i ];
			
			t_loaded += t_section.GetLoadedWeight();

			t_total += t_section.GetTotalWeight();

//			t_section.Log();
		}
		
		if( t_total <= 0.0f ){
//			Debug.LogError( "Error In Total: " + t_total );
			
			t_total = 1.0f;
		}
		
		return t_loaded / t_total;
	}
	
	public static void ClearLoadingInfo( List<LoadingSection> p_list ){
		p_list.Clear();
	}

	public static void LogLoadingInfo( List<LoadingSection> p_list ){
		Debug.Log( "--------- StaticLoading.LogLoadingInfo --------" );

		if( p_list == null ){
			return;
		}

		for( int i = 0; i < p_list.Count; i++ ){
			LoadingSection t_section = p_list[ i ];

			t_section.Log();
		}
	}

	#endregion



	#region Loading Common

	public const string CONST_COMMON_LOADING_SCENE		= "Common_Scene";

	#endregion


	#region Loading MainCity

	public const string CONST_MAINCITY_NETWORK			= "MainCity_Network";

	#endregion
}
