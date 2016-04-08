//#define LOG_LOADING

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LoadingHelper {

	#region Loading Item Info

	public class LoadingItemInfo: IComparable<LoadingItemInfo>{
		private int m_loading_asset_index = -1;
		
		private string m_loading_asset_path = "";
		
		private float m_loading_asset_time = 0.0f;

		private float m_delegate_time = 0.0f;
		
		public LoadingItemInfo( string p_asset_path, int p_asset_index, float p_loading_time, float p_delegate_time ){
			m_loading_asset_index = p_asset_index;
			
			m_loading_asset_path = p_asset_path;
			
			m_loading_asset_time = p_loading_time;

			m_delegate_time = p_delegate_time;
		}
		
		public bool IsInDir( string p_dir ){
			if( m_loading_asset_path.StartsWith( p_dir ) ){
				return true;
			}
			
			return false;
		}
		
		public string GetAssetPath(){
			return m_loading_asset_path;
		}
		
		public float GetLoadTime(){
			return m_loading_asset_time;
		}

		public float GetDelegateTime(){
			return m_delegate_time;
		}

		public float GetTotalTime(){
			return m_loading_asset_time + m_delegate_time;
		}

		public int CompareTo( LoadingItemInfo p_target ){
			if( p_target == null ){
				return -1;
			}

			return -GetTotalTime().CompareTo( p_target.GetTotalTime() );
		}
	}
	
	#endregion
	
	
	#region Loading Info
	
	private static string[] m_loading_assets_dir = {
		"_3D/Models/",
		"_3D/Chars/",
		"_3D/Fx/",
		"Sounds/",
		"_Data/",
		"_Uis/",
	};
	
	private static List<LoadingItemInfo> m_loading_items_list = new List<LoadingItemInfo>();

	public static void ClearLoadingItemInfo(){
		m_loading_items_list.Clear ();
	}

	// Record detail loading info, prepare to analyse.
	public static void AddLoadingItemInfo( string p_asset_path, int p_asset_index, float p_loading_time, float p_delegate_time  ){
		LoadingItemInfo t_info = new LoadingItemInfo( p_asset_path, p_asset_index, p_loading_time, p_delegate_time  );
		
		m_loading_items_list.Add( t_info );
	}

	private static int SortLoadingInfo( LoadingItemInfo p_info_0, LoadingItemInfo p_info_1 ){
		if( p_info_0 == null || p_info_1 == null ){
			return 0;
		}

		if( p_info_0.GetTotalTime() > p_info_1.GetTotalTime() ){
			return 1;
		}
		else{
			return -1;
		}
	}

	public static void ShowDetailLoadingInfo( bool p_log_raw_data = false ){
		Debug.Log( "---------------------------- Detail Loading Info ------------------------------" );

		List<LoadingItemInfo> t_loading_items_list = new List<LoadingItemInfo>();
		
		{
			m_loading_items_list.ForEach( i => t_loading_items_list.Add( i ) );
		}

		t_loading_items_list.Sort();

		// each item log
		if( p_log_raw_data ){
			float t_loading_time = 0.0f;
			
			float t_delegate_time = 0.0f;
			
			for( int j = 0; j < t_loading_items_list.Count; j++ ){
				LoadingItemInfo t_item = t_loading_items_list[ j ];
				
				t_loading_time = t_item.GetLoadTime();
				
				t_delegate_time = t_item.GetDelegateTime();

				Debug.Log( "--- " + j + " / " + t_loading_items_list.Count + 
				          " Total Time: " + MathHelper.FloatToString( t_loading_time + t_delegate_time, 7 ) + 
				          " " + t_item.GetAssetPath() + 
				          " Load Time: " + MathHelper.FloatToString( t_loading_time, 7 ) +
				          " Delegate Time: " + MathHelper.FloatToString( t_delegate_time, 7 ) );
			}
		}

		int t_unique_index = 0;

		List<LoadingItemInfo> t_unique_list = new List<LoadingItemInfo>();

		Debug.Log( "--------------- Detail Repeat Loading Info -----------" );

		// repeat log
		while( t_loading_items_list.Count > 0 ){
			List<LoadingItemInfo> t_item_list = new List<LoadingItemInfo>();

			LoadingItemInfo t_target_item = t_loading_items_list[ 0 ];

			t_unique_list.Add( t_target_item );

			float t_total_time = 0;

			for( int i = t_loading_items_list.Count - 1; i >= 0; i-- ){
				LoadingItemInfo t_item = t_loading_items_list[ i ];

				if( t_item.GetAssetPath() == t_target_item.GetAssetPath() ){
					t_item_list.Add( t_item );

					t_total_time = t_total_time + t_item.GetTotalTime();

					t_loading_items_list.RemoveAt( i );
				}
			}

			Debug.Log( "--- " + t_unique_index + " - " + t_total_time + " - " +
			          t_item_list.Count + "x " + t_target_item.GetAssetPath() );

			t_unique_index++;

			for( int i = 0; i < t_item_list.Count; i++ ){
				LoadingItemInfo t_item = t_item_list[ i ];

				Debug.Log( i + ": " +
				          " Total Time: " + MathHelper.FloatToString( t_item.GetLoadTime() + t_item.GetDelegateTime(), 7 ) + 
				          " Load Time: " + MathHelper.FloatToString( t_item.GetLoadTime(), 7 ) +
				          " Delegate Time: " + MathHelper.FloatToString( t_item.GetDelegateTime(), 7 ) );
			}
		}

		Debug.Log( "--------------- Detail Unique Dir Loading Info -----------" );

		// dir classified unique log
		for( int i = 0; i < m_loading_assets_dir.Length; i++ ){
			int t_load_count = 0;
			
			for( int j = t_unique_list.Count - 1; j >= 0; j-- ){
				LoadingItemInfo t_item = t_unique_list[ j ];
				
				if( t_item.IsInDir( m_loading_assets_dir[ i ] ) ){
					t_load_count++;
					
					t_unique_list.Remove( t_item );
				}
			}
			
			Debug.Log( m_loading_assets_dir[ i ] + 
			          " Unique Asset Loadded Count: " + t_load_count );
		}

		if( t_unique_list.Count > 0 ){
			Debug.Log( t_unique_list.Count + " Other Asset." );
			
			for( int i = t_unique_list.Count - 1; i >= 0; i-- ){
				LoadingItemInfo t_item = t_unique_list[ i ];
				
				Debug.Log( i + " - Not Classified: " + t_item.GetAssetPath() );
			}
		}
	}
	
	public static void ShowTotalLoadingInfo(){
		Debug.Log( "-------------------------- Total Loading Info ---------------------------" );

		List<LoadingItemInfo> t_loading_items_list = new List<LoadingItemInfo>();

		{
			m_loading_items_list.ForEach( i => t_loading_items_list.Add( i ) );
		}

		{
			float t_total_loading_time = 0.0f;

			float t_total_delegate_time = 0.0f;
			
			for( int j = t_loading_items_list.Count - 1; j >= 0; j-- ){
				LoadingItemInfo t_item = t_loading_items_list[ j ];
				
				t_total_loading_time = t_total_loading_time + t_item.GetLoadTime();

				t_total_delegate_time = t_total_delegate_time + t_item.GetDelegateTime();
			}
			
			Debug.Log( "--- " + t_loading_items_list.Count + 
			          " Total Time: " + MathHelper.FloatToString( t_total_loading_time + t_total_delegate_time, 5 ) + 
			          " Load Time: " + MathHelper.FloatToString( t_total_loading_time, 5 ) +
			          " Delegate Time: " + MathHelper.FloatToString( t_total_delegate_time, 5 ) );
		}
		
		for( int i = 0; i < m_loading_assets_dir.Length; i++ ){
			float t_load_time = 0.0f;

			float t_delegate_time = 0.0f;
			
			int t_load_count = 0;
			
			for( int j = t_loading_items_list.Count - 1; j >= 0; j-- ){
				LoadingItemInfo t_item = t_loading_items_list[ j ];
				
				if( t_item.IsInDir( m_loading_assets_dir[ i ] ) ){
					t_load_time = t_load_time + t_item.GetLoadTime();

					t_delegate_time = t_delegate_time + t_item.GetDelegateTime();
					
					t_load_count++;
					
					t_loading_items_list.Remove( t_item );
				}
			}
			
			Debug.Log( m_loading_assets_dir[ i ] + 
			          " Count: " + t_load_count + 
			          "   Total Time: " + ( t_load_time + t_delegate_time ) +
			          "   Load Time: " + t_load_time +
			          "   Delegate Time: " + t_delegate_time );
		}

		if( t_loading_items_list.Count > 0 ){
			float t_total_time = 0.0f;
			
			for( int i = t_loading_items_list.Count - 1; i >= 0; i-- ){
				LoadingItemInfo t_item = t_loading_items_list[ i ];
				
				t_total_time = t_total_time + t_item.GetTotalTime();
				
				Debug.Log( i + " - " + t_item.GetLoadTime() + " - Not Classified: " + t_item.GetAssetPath() );
			}
			
			Debug.Log( t_loading_items_list.Count + " Other Asset, Total Time: " + t_total_time );
		}
	}
	
	#endregion



	#region dont destroy cache when loading new scene

	private static List<GameObject> m_dont_destroy_on_load = new List<GameObject>();
	
	/// if the DontDestroy GameObject need to be removed when enter new level, call this.
	public static void RemoveWhenSceneDone( GameObject p_game_object ){
		m_dont_destroy_on_load.Add( p_game_object );
	}

	/// remove objects marked ToRemove.
	public static void ClearDontDestroyOnLoads(){
//		Debug.Log( "ClearDontDestroyOnLoads()" );
		
		for( int i = 0; i < m_dont_destroy_on_load.Count; i++ ){
			GameObject t_game_object = m_dont_destroy_on_load[ i ];
			
			if( t_game_object != null ){
				GameObject.Destroy( t_game_object );
			}
		}
		
		m_dont_destroy_on_load.Clear();
	}

	#endregion



	#region Loading Scene Checkers

	/// Is Loading Login Now?
	public static bool IsLoadingLogin(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_LOGIN;
	}
	
	/// Is Loading CreateRole Now?
	public static bool IsLoadingCreateRole(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CREATE_ROLE );
	}
	
	/// Is Loading Main City Now?
	public static bool IsLoadingMainCity(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.MAIN_CITY );
	}
	
	public static bool IsLoadingMainCityYeWan(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.MAIN_CITY_YE_WAN );
	}
	/// Is Loading Alliance City Now?
	public static bool IsLoadingAllianceCity(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY );
	}

	public static bool IsLoadingAllianceCityYeWan(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY_YE_WAN );
	}
	
	public static bool IsLoadingAllianceTenentsCity(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE;
	}
	public static bool IsInAllianceTenentsCityYeWanScene(){
		return Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN;
	}
	/// Determines whether is loading Battle Field now.
	public static bool IsLoadingBattleField(){
		return EnterNextScene.GetSceneToLoad().StartsWith( ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_PREFIX );
	}
	
	public static bool IsLoadingHouse(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.HOUSE );
	}
	
	public static bool IsLoadingCarriage(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CARRIAGE );
	}
	
	public static bool IsLoadingAllianceBattle(){
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_BATTLE );
	}

	public static bool IsLoadingTreasureCity ()
	{
		return EnterNextScene.GetSceneToLoad() == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.TREASURE_CITY );
	}

	#endregion



	#region Scene Quality

	public static void ConfigBloomAndLight(){
		{
			bool t_active_light = false;
			
			bool t_active_bloom = false;
			
			if( IsLoadingLogin() ){
				t_active_light = false;
				
				t_active_bloom = false;
			}
			else if( IsLoadingCreateRole() ){
				t_active_light = Quality_Shadow.BattleField_RealShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else if( IsLoadingBattleField() ){
				t_active_light = Quality_Shadow.BattleField_RealShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else if (IsLoadingMainCity() || IsLoadingMainCityYeWan() || IsLoadingAllianceCity() || IsLoadingAllianceTenentsCity() || IsLoadingHouse() || IsLoadingAllianceCityYeWan() || IsInAllianceTenentsCityYeWanScene() || IsLoadingCarriage()||IsLoadingAllianceBattle()||IsLoadingTreasureCity())
			{
				t_active_light = Quality_Shadow.InCity_RealShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else{
				Debug.LogError( "Error, Unknown Scene: " + EnterNextScene.GetSceneToLoad() );
				
				t_active_light = false;
				
				t_active_bloom = false;
			}
			
			{
				Quality_Shadow.ConfigLights( t_active_light );
			}
			
			{
				Quality_Common.ConfigBloom( t_active_bloom );
			}
		}
	}

	#endregion

	
	
	#region Init and Update
	
	/// Inits the section info, Must be initialized at the beginning, Never Called Twice.
	/// 
	/// Note:
	/// 1.If you want to update section's total count , use UpdateSectionInfo().
	public static LoadingSection InitSectionInfo( List<LoadingSection> p_list, string p_section_name, float p_weight = 1.0f, int p_item_count = -1 ){
		LoadingSection t_section = LoadingHelper.GetSection( p_list, p_section_name );
		
		if( t_section == null ){
			#if LOG_LOADING
			Debug.Log("InitSectionInfo: " + p_section_name + ", " + p_weight + ", " + p_item_count );
			#endif
			
			t_section = new LoadingSection( p_section_name, p_weight, p_item_count );
			
			p_list.Add( t_section );
		}
		else{
			Debug.LogError( "Error, Loading Section Already Exist." );
		}
		
		return t_section;
	}
	
	/// After init, you can update section's  Total Item Count here.
	public static void UpdateSectionInfo( List<LoadingSection> p_list, string p_section_name, int p_item_count ){
		LoadingSection t_section = LoadingHelper.GetSection( p_list, p_section_name );
		
		if( t_section == null ){
			Debug.LogError( "Error In Finding, section must be init first." );
			
			return;
		}
		
		t_section.SetTotalCount( p_item_count );
	}
	
	#endregion



	#region Update Loading
	
	public static void ItemLoaded( List<LoadingSection> p_list, string p_section_name, string p_item_name = "" ){
		#if LOG_LOADING
		Debug.Log( "ItemLoaded( " + LoadingHelper.GetLoadingPercentage( p_list ) +
			" - " + p_section_name + ": " + p_item_name + " )" );
		#endif
		
		LoadingSection t_section = LoadingHelper.GetSection( p_list, p_section_name );
		
		if( t_section == null ){
			t_section = InitSectionInfo( p_list, p_section_name );
			
//			Debug.Log( "LoadingHelper.ItemLoaded( Section Not Found )" );
		}
		
		t_section.ItemLoaded();

		StaticLoading.SetCurLoading( p_item_name );
	}
	
	public static void UpdatePercentage( List<LoadingSection> p_list, string p_section_name, float p_percentage ){
//		Debug.Log( "UpdatePercentage( " + p_section_name + " - " + p_percentage + " )" );
		
		LoadingSection t_section = LoadingHelper.GetSection( p_list, p_section_name );
		
		if( t_section == null ){
			t_section = InitSectionInfo( p_list, p_section_name );
			
//			Debug.Log( "StaticLoading.UpdatePercentage( Section Not Found )" );
		}
		
		t_section.UpdatePercentage( p_percentage );
	}

	#endregion



	#region Get Info
	
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
	
	#endregion


	
	#region Common Init
	
	private static float m_begin_loading_time	= 0.0f;
	
	private static float m_last_tagged_time		= 0.0f;
	
	private static int m_asset_load_count 		= 0;
	
	public static void ResetLoadingInfo(){
		// reset local info
		{
			m_begin_loading_time = Time.realtimeSinceStartup;
			
			m_last_tagged_time = Time.realtimeSinceStartup;
			
			m_asset_load_count = 0;

			ResetCoroutineInfo();
		}
		
		// reset detail info
		{
			LoadingHelper.ClearLoadingItemInfo();
		}
	}
	
	public static int GetAssetLoadedCount(){
		return m_asset_load_count;
	}
	
	public static void AssetLoaded(){
		m_asset_load_count++;
	}
	
	public static float GetTimeSinceLoading(){
		return Time.realtimeSinceStartup - m_begin_loading_time;
	}
	
	public static float GetTimesinceLastTimeTag( bool p_retag = true ){
		float t_delta = Time.realtimeSinceStartup - m_last_tagged_time;

		if( p_retag ){
			m_last_tagged_time = Time.realtimeSinceStartup;
		}
		
		return t_delta;
	}
	
	public static void LogTimeSinceLoading( string p_loading_tag ){
		Debug.Log( MathHelper.FloatToString( GetTimesinceLastTimeTag(), 5 ) + 
		          " / " + 
		          MathHelper.FloatToString( GetTimeSinceLoading(), 5 ) + " - " + 
		          p_loading_tag + " - " +
		          Time.realtimeSinceStartup );
	}
	
	#endregion



	#region Coroutine Use
	
	private static float m_coroutine_time_tag = 0.0f;
	
	private static float m_total_coroutine_time = 0.0f;
	
	private static float m_total_prepare_load_next_asset_time = 0.0f;
	
	public static void ResetCoroutineInfo(){
		ResetCoroutineTimeTag();
		
		m_total_coroutine_time = 0.0f;
		
		m_total_prepare_load_next_asset_time = 0.0f;
	}
	
	public static void ResetCoroutineTimeTag(){
		m_coroutine_time_tag = Time.realtimeSinceStartup;
	}
	
	public static void BeforeStartCoroutine(){
		ResetCoroutineTimeTag();
	}
	
	public static void BeforeLoadRes(){
		m_total_coroutine_time = m_total_coroutine_time + ( Time.realtimeSinceStartup - m_coroutine_time_tag );
	}
	
	public static void BeforeReadyToLoadNextAsset(){
		ResetCoroutineTimeTag();
	}
	
	public static void AfterReadyToLoadNextAsset(){
		m_total_prepare_load_next_asset_time = m_total_prepare_load_next_asset_time + ( Time.realtimeSinceStartup - m_coroutine_time_tag );
	}
	
	public static void LogCoroutineInfo(){
		Debug.Log( "Total Coroutine Time Cost: " + m_total_coroutine_time );
		
		Debug.Log( "Total Coroutine Wait Ready Time Cost: " + m_total_prepare_load_next_asset_time );
	}
	
	#endregion



	#region Utility
	
	public static void ClearLoadingInfo( List<LoadingSection> p_list ){
		if( p_list == null ){
			Debug.Log( "p_list should not be null." );

			return;
		}

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
}
