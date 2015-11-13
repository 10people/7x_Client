using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingHelper{

	#region Loading Item Info

	public class LoadingItemInfo{
		private int m_loading_asset_index = -1;
		
		private string m_loading_asset_path = "";
		
		private float m_loading_asset_time = 0.0f;
		
		public LoadingItemInfo( string p_asset_path, int p_asset_index, float p_loading_time ){
			m_loading_asset_index = p_asset_index;
			
			m_loading_asset_path = p_asset_path;
			
			m_loading_asset_time = p_loading_time;
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
	public static void AddLoadingItemInfo( string p_asset_path, int p_asset_index, float p_loading_time ){
		LoadingItemInfo t_info = new LoadingItemInfo( p_asset_path, p_asset_index, p_loading_time );
		
		m_loading_items_list.Add( t_info );
	}
	
	public static void ShowTotalLoadingInfo(){
		Debug.Log( m_loading_items_list.Count + " Assets Loaded." );
		
		for( int i = 0; i < m_loading_assets_dir.Length; i++ ){
			float t_load_time = 0.0f;
			
			int t_load_count = 0;
			
			for( int j = m_loading_items_list.Count - 1; j >= 0; j-- ){
				LoadingItemInfo t_item = m_loading_items_list[ j ];
				
				if( t_item.IsInDir( m_loading_assets_dir[ i ] ) ){
					t_load_time = t_load_time + t_item.GetLoadTime();
					
					t_load_count++;
					
					m_loading_items_list.Remove( t_item );
				}
			}
			
			Debug.Log( m_loading_assets_dir[ i ] + 
			          " Count: " + t_load_count + 
			          "   Load Time: " + t_load_time );
		}
		
		{
			float t_load_time = 0.0f;
			
			for( int i = m_loading_items_list.Count - 1; i >= 0; i-- ){
				LoadingItemInfo t_item = m_loading_items_list[ i ];
				
				t_load_time += t_item.GetLoadTime();
				
				Debug.Log( t_item.GetLoadTime() + " - Not Classified: " + t_item.GetAssetPath() );
			}
			
			Debug.Log( m_loading_items_list.Count + " Other Asset, Loading Time: " + t_load_time );
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
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_CREATE_ROLE;
	}
	
	/// Is Loading Main City Now?
	public static bool IsLoadingMainCity(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_MAIN_CITY ;
	}
	
	public static bool IsLoadingMainCityYeWan(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_MAIN_CITY_YEWAN;
	}
	/// Is Loading Alliance City Now?
	public static bool IsLoadingAllianceCity(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY;
	}

	public static bool IsLoadingAllianceCityYeWan(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_YE_WAN;
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
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_HOUSE;
	}
	
	public static bool IsLoadingCarriage(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_CARRIAGE;
	}
	
	public static bool IsLoadingAllianceBattle(){
		return EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_ALLIANCE_BATTLE;
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
				t_active_light = !QualityTool.Instance.BattleField_ShowSimpleShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else if( IsLoadingBattleField() ){
				t_active_light = !QualityTool.Instance.BattleField_ShowSimpleShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else if (IsLoadingMainCity() || IsLoadingMainCityYeWan() || IsLoadingAllianceCity() || IsLoadingAllianceTenentsCity() || IsLoadingHouse() || IsLoadingAllianceCityYeWan() || IsInAllianceTenentsCityYeWanScene() || IsLoadingCarriage()||IsLoadingAllianceBattle())
			{
				t_active_light = !QualityTool.Instance.InCity_ShowSimpleShadow();
				
				t_active_bloom = QualityTool.GetBool( QualityTool.CONST_BLOOM );
			}
			else{
				Debug.LogError( "Error, Unknown Scene: " + EnterNextScene.GetSceneToLoad() );
				
				t_active_light = false;
				
				t_active_bloom = false;
			}
			
			{
				QualityTool.ConfigLights( t_active_light );
			}
			
			{
				QualityTool.ConfigBloom( t_active_bloom );
			}
		}
	}

	#endregion

}
