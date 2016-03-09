//#define LOG_LOADING



#define USE_LOADING_RES_CONFIG



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

	public GameObject m_fx_gb_parent;

	private static StaticLoading m_instance = null;

	public static StaticLoading Instance(){
		if( m_instance == null ){
			Debug.LogError( "Error, StaticLoadingBg = null." );
		}

		return m_instance;
	}

	public static bool HaveInstance(){
		return m_instance != null;
	}


	#region Mono

	void Awake(){
//		Debug.Log( "StaticLoadingBg.Awake()" );

		SceneManager.SetSceneState( SceneManager.SceneState.Loading );

		LoadingHelper.ClearDontDestroyOnLoads();

		m_instance = this;

		LoadingHelper.ClearLoadingInfo( m_loading_sections );

		SetTipsText();

		{
			InitBackgroundTexture();

			InitLoadingFx();
		}
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

		m_loading_bg = null;

		m_loading_bg_root = null;
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

		{
			// restore default loading for next loading
			LoadingTemplate.SetCurFunction( LoadingTemplate.LoadingFunctions.COMMON );
		}

//		LoadingHelper.LogLoadingInfo();

		LoadingHelper.ClearLoadingInfo( m_loading_sections );
	}

	#endregion



	#region Tips

	private LanguageTemplate.Text m_begin_id = LanguageTemplate.Text.LOADING_TIPS_1;

	private LanguageTemplate.Text m_end_id = LanguageTemplate.Text.LOADING_TIPS_49;

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

			if( t_delta > ConfigTool.GetFloat( ConfigTool.CONST_LOADING_INTERVAL, 1.0f ) ){
				return false;
			} 
			else{
				return true;
			}
		}
	}

	private static void InitLoadingFx(){
		string t_path = EffectIdTemplate.GetPathByeffectId( 620217 );

		Global.ResourcesDotLoad( t_path, LoadingFxLoadCallback );
	}

	public static void LoadingFxLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		if( p_object == null ){
			Debug.LogError( "Fx is null." );

			return;
		}

		if( Instance().m_fx_gb_parent == null ){
			Debug.LogError( "Error, gb parent is null." );

			return;
		}

		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );

		t_gb.transform.parent = Instance().m_fx_gb_parent.transform;

		GameObjectHelper.SetGameObjectLayerRecursive( t_gb, Instance().m_fx_gb_parent.layer );

		TransformHelper.ResetLocalPosAndLocalRotAndLocalScale( t_gb );
	}

	private static void InitBackgroundTexture(){
		string t_ui_path = "";

		#if USE_LOADING_RES_CONFIG
		t_ui_path = LoadingTemplate.GetResPath();
		#else
		if( LoadingHelper.IsLoadingMainCity() || LoadingHelper.IsLoadingMainCityYeWan() ){
			t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.LOADING_BG_FOR_MAINCITY );
		}
		else if( LoadingHelper.IsLoadingBattleField() || LoadingHelper.IsLoadingCarriage() || LoadingHelper.IsLoadingAllianceBattle() ){
			t_ui_path = Res2DTemplate.GetResPath( Res2DTemplate.Res.LOADING_BG_FOR_BATTLE_FIELD );
		}
		else{
			return;
		}
		#endif

		if( string.IsNullOrEmpty( t_ui_path ) ){
			Debug.Log( "Loading Res not configged." );

			return;
		}

		Global.ResourcesDotLoad( t_ui_path, BackgroundLoadCallback );
	}

	public static void BackgroundLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		Texture t_tex = (Texture)( p_object );
		
		if( t_tex == null ){
			Debug.LogError( "Texture to null." );
			
			return;
		}

		Instance().m_loading_bg.mainTexture = t_tex;
	}

	#endregion



	#region Update Loading

	public static void SetCurLoading( string p_cur_loading_name ){
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



	#region Loading Common

	public const string CONST_COMMON_LOADING_SCENE		= "Common_Scene";

	#endregion


	#region Loading MainCity

	public const string CONST_MAINCITY_NETWORK			= "MainCity_Network";

	#endregion
}
