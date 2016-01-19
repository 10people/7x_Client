//#define DEBUG_SCENE_STATE

//#define DEBUG_LOADING_SCENE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/** 
 * @author:		Zhang YuGu
 * @Date: 		2014.10.1
 * @since:		Unity 4.5.3
 * Function:	Scene entrances.
 * 
 * Notes:
 * None.
 */ 
public class SceneManager{

    public static bool m_isSequencer = false;
    private static int m_MainSceneIndex = 0;
    private static int m_AllianceSceneIndex = 0;

	#region Clean In Loading

	/// Enter Loading Scene, clean everything here
	private static void EnterLoading(){
		#if DEBUG_LOADING_SCENE
		Debug.Log( "SceneManager.EnterLoading()" );
		#endif
		
		if ( EnterNextScene.Instance () != null ) {
			Debug.Log( "Loading loading." );
			
			EnterNextScene.Instance().ManualDestroyImmediate();
		}
		
		// clear all fx
		{
			// ui fx
			if( UI3DEffectTool.HaveInstance() ){
				UI3DEffectTool.Instance().ClearAllUI3DEffect();
			}

			// 3d fx
			{
				FxTool.CleanFxToLoad();
			}
		}
		
		// clear ui reference
		{
			UI2DTool.ClearAll();
		}
		
		string t_loading_level_name = ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE;
		
		Global.LoadLevel( t_loading_level_name, LoadLoadinglDone );
	}

	#endregion



    #region Scene Loader

    /// Error, Never Use This.
    ///	THIS FUNCTION IS FOR OLD CODE, AND CHECK THEM OUT.
    public static void EnterSomeScene( string p_scene_name ){
        // Keep This Error, and Please RT YuGu.
        Debug.LogError("Error, Never Use This.");

        EnterNextScene.SetSceneToLoad( p_scene_name );

        EnterLoading();
    }

	/// request enter login.
	/// 
	/// Notes:
	/// 1.int our package, make no difference;
	/// 2.in 3rd package, login will be invoke by 3rd callback.

	public static void RequestEnterLogin(){
		if ( ThirdPlatform.IsThirdPlatform () ){
			ThirdPlatform.LogOut();
		}

		{
			SceneManager.EnterLogin();
		}
	}

	/// direct enter login, only first scene and switch account scene should call this.
	public static void EnterLogin(){
		#if DEBUG_LOADING_SCENE
		Debug.Log( "EnterLogin()" );
		#endif

//		Debug.Log( "EnterLogin()" );

		// fix twice login
		{
			if( IsInLoadingScene() ){
				if( EnterNextScene.GetSceneToLoad() == ConstInGame.CONST_SCENE_NAME_LOGIN ){
					Debug.Log( "twice login, return." );

					return;
				}
			}
		}

		{
			OnLogin();
		}

        EnterNextScene.SetSceneToLoad( ConstInGame.CONST_SCENE_NAME_LOGIN );

        EnterLoading();

		if( ThirdPlatform.IsThirdPlatform() ){
			ThirdPlatform.CheckLoginToShowSDK();
		}
    }

    /// Load Create Role Scene.
    public static void EnterCreateRole(){
		#if DEBUG_LOADING_SCENE
		Debug.Log("EnterCreateRole()" );
		#endif

		EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CREATE_ROLE ) );

        EnterLoading();
    }

    /// Load House Scene.
    public static void EnterHouse(){
		#if DEBUG_LOADING_SCENE
		Debug.Log("EnterHouse()" );
		#endif

        //MainCityUI mainCityUI = Object.FindObjectOfType<MainCityUI>();

        //Object.DontDestroyOnLoad( mainCityUI.gameObject );
        CityGlobalData.m_isAllianceTenentsScene = false;

		EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.HOUSE ) );
		
		EnterLoading();
    }

    /// Load Carriage Scene.
    public static void EnterCarriage(){

		#if DEBUG_LOADING_SCENE
		Debug.Log("EnterCarriage()" );
        #endif

		EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CARRIAGE ) );
		
		EnterLoading();
    }

    /// <summary>
    /// Load Alliance Battle Scene.
    /// </summary>
    public static void EnterAllianceBattle(){
		#if DEBUG_LOADING_SCENE
		Debug.Log("EnterAllianceBattle()" );
        #endif

		EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_BATTLE ) );
		
		EnterLoading();
    }

    /// Request to Load Main City Scene.
    public static void EnterMainCity(){
#if DEBUG_LOADING_SCENE
		Debug.Log("EnterMainCity( " + m_MainSceneIndex + " )" );
#endif

//    	Debug.Log("EnterMainCity.m_MainSceneIndex: " + m_MainSceneIndex);
    
		FunctionWindowsCreateManagerment.SceneNumSet(0);
        
		MainCityUI.m_PlayerPlace=MainCityUI.PlayerPlace.MainCity;
        
		m_AllianceSceneIndex = 0;
        
		if (m_MainSceneIndex == 0)
        {
            m_MainSceneIndex++;
    
           CityGlobalData.m_isMainScene = true;

        }
        else if (!CityGlobalData.m_isMainScene)
        {
            m_isSequencer = true;
        }


        //if (CityGlobalData.m_SeverTime < 5 || CityGlobalData.m_SeverTime > 20)
        //{
        //    EnterNextScene.SetSceneToLoad(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.MAIN_CITY_YE_WAN), false);
        //    //  EnterNextScene.SetSceneToLoad(ConstInGame.CONST_SCENE_NAME_MAIN_CITY);
        //}
        //else
        {
			EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.MAIN_CITY), false );
           // EnterNextScene.SetSceneToLoad(ConstInGame.CONST_SCENE_NAME_MAIN_CITY_YEWAN);
        }

        EnterLoading();
    }

    /// Request to Load Battle Field "p_battle_field_scene_name".
    public static void EnterBattleField( string p_battle_field_scene_name ){
		#if DEBUG_LOADING_SCENE
		Debug.Log("EnterBattleField( " + p_battle_field_scene_name + " )" );
		#endif

        EnterNextScene.SetSceneToLoad( p_battle_field_scene_name );

        EnterLoading();
    }


    /// Request to Load Alliance City Scene.
 
    public static void EnterAllianceCity1111(){
#if DEBUG_LOADING_SCENE
		Debug.Log("EnterAllianceCity( " + " )" );
#endif
        m_MainSceneIndex = 0;
        FunctionWindowsCreateManagerment.SceneNumSet(1);
        MainCityUI.m_PlayerPlace = MainCityUI.PlayerPlace.MainCity;

		if (m_AllianceSceneIndex == 0)
        {
            m_AllianceSceneIndex++;
           CityGlobalData.m_isAllianceScene = true;
        }
        else if (!CityGlobalData.m_isAllianceScene)
        {
            m_isSequencer = true;
        }

   //     if (CityGlobalData.m_SeverTime < 5 || CityGlobalData.m_SeverTime > 20)
   //     {
			//EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY_YE_WAN ), false );
   //       //  EnterNextScene.SetSceneToLoad(ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_YE_WAN);
   //     }
   //     else
        {
			EnterNextScene.SetSceneToLoad( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY ), false );
          //  EnterNextScene.SetSceneToLoad( ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY );
        }
  
        EnterLoading();
    }

   
    public static void EnterAllianceCityTenentsCityOne111(){
		Debug.LogError( "Should Never Be Here." );

#if DEBUG_LOADING_SCENE
		Debug.Log("EnterAllianceCityTenentsCityOne( " + " )" );
#endif
        FunctionWindowsCreateManagerment.SceneNumSet(2);
        CityGlobalData.m_isAllianceTenentsScene = true;

        //Debug.Log("CityGlobalData.m_SeverTimeCityGlobalData.m_SeverTime :: " + CityGlobalData.m_SeverTime);
        if (CityGlobalData.m_SeverTime < 5 || CityGlobalData.m_SeverTime > 20)
        {
            EnterNextScene.SetSceneToLoad(ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN);
        }
        else
        {
            EnterNextScene.SetSceneToLoad( ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE );
        }

        EnterLoading();
    }
    #endregion



	#region Clean
	
	private static void OnLogin(){
		// clear
		{
			Global.m_isOpenPVP = false;

			ClientMain.m_listPopUpData = new List<ClientMain.PopUpData>();
			ClientMain.m_isNewOpenFunction = false;

			Global.m_isOpenBaiZhan = false;
			Global.m_isOpenHuangYe = false;
			Global.m_iOpenFunctionIndex = -1;
			
			Global.m_iScreenID = 0;
			
			Global.m_iScreenNoID = 0;
			
			Global.m_isOpenJiaoxue = true;
			Global.m_isZhanli = false;
			Global.m_iAddZhanli = 0;
			
			Global.m_sMainCityWantOpenPanel = -1;
			Global.m_sPanelWantRun = "";
			Global.m_NewChenghao = new List<int>();
			Global.m_iAddZhanli = 0;
			Global.m_isFuWen = false;
			Global.m_isOpenFuWen = false;
			Global.m_isOpenShop = false;
			Global.m_isOpenPlunder = false;

            //Clear highest ui and chat objects.
			if( ClientMain.Instance() != null ){
				ClientMain.Instance().ClearObjectsWhenReconnect();
			}

			GameObjectHelper.ClearDontDestroyGameObject();
			
			MainCityUI.m_PlayerPlace=MainCityUI.PlayerPlace.MainCity;

			if( UIYindao.m_UIYindao != null ){
//				Debug.Log(UIYindao.m_UIYindao.m_isOpenYindao);

				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}

			if (GeneralRewardManager.Instance () != null)
			{
				GeneralRewardManager.Instance ().ClearRewardData ();
			}
			
			SocketTool.CloseSocket();
		}

		{
			CleanGuideAndDialog();
		}

		// clear red spot
		{
			PushAndNotificationHelper.ClearAllRedSpotNotification();
		}

		// reset
		{
			Time.timeScale = 1.0f;
		}
	}

	public static void CleanGuideAndDialog(){
		if( UIYindao.m_UIYindao != null && UIYindao.m_UIYindao.m_isOpenYindao ){
			UIYindao.m_UIYindao.CloseUI();
		}
		
		if( ClientMain.Instance() != null && ClientMain.Instance().m_UIDialogSystem != null ){
			ClientMain.Instance().m_UIDialogSystem.CloseDialog();
		}
	}

	#endregion

	
	#region Cur Scene
	
	public static bool IsInLoadingScene(){
		return Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE;
	}
	
	public static bool IsInLoginScene(){
		return Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_LOGIN;
	}
	
	public static bool IsInCreateRoleScene(){
		return Application.loadedLevelName ==SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CREATE_ROLE );
	}
	
	public static bool IsInMainCityScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.MAIN_CITY );
	}

	public static bool IsInMainCityYeWanScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.MAIN_CITY_YE_WAN );
	}
	
	public static bool IsInAllianceCityScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY );
	}
	
	public static bool IsInAllianceCityYeWanScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY_YE_WAN );
	}
	
	public static bool IsInAllianceTenentsCityScene(){
		return Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE;
	}
	
	public static bool IsInAllianceTenentsCityYeWanScene(){
		return Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN;
	}
	
	public static bool IsInBattleFieldScene(){
		return Application.loadedLevelName.StartsWith(ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_PREFIX);
	}
	
	public static bool IsInHouseScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.HOUSE );
	}
	
	public static bool IsInCarriageScene(){
		return Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CARRIAGE );
	}
	
	
	#endregion



    #region Utilities

	public static void LoadLevel( string p_level_name, bool p_clean_anim = false ){
		#if DEBUG_LOADING_SCENE
		Debug.Log( "LoadLevel( " + p_level_name + " )" );
		#endif

		Application.LoadLevel( p_level_name );
		
		UtilityTool.UnloadUnusedAssets( p_clean_anim );
	}

	public static bool IsInBattleScene(){
		return LevelHelper.IsInBattleScene ();
	}

    public static void LoadLoadinglDone( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		#if DEBUG_LOADING_SCENE
		Debug.Log( "SceneManager.LoadLoadinglDone( " + p_path + " )" );
		#endif

		LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE, true );
    }

    #endregion



	#region SceneState

	public enum SceneState{
		Login,			
		Loading, 		
		CreateRole,		
		BattleField,	
		MainCity,		
		AllianceCity,	
		Carriage,		
		AllianceBattle,
	}

	private static SceneState m_scene_state = SceneState.Login;

	/// Get Current Scene State.
	public static SceneState GetSceneState(){
		return m_scene_state;
	}

	public static void SetSceneState( SceneState p_scene_state ){
		#if DEBUG_SCENE_STATE
		Debug.Log( "---------------------------------SetSceneState( " + p_scene_state + " )-------------------------" );
		#endif

		m_scene_state = p_scene_state;
	}

	public static void UpdateSceneStateByLevel(){
		string t_level = Application.loadedLevelName;

		if ( t_level == ConstInGame.CONST_SCENE_NAME_LOGIN ) {
			SetSceneState( SceneState.Login );

			return;
		}

		if( t_level == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CREATE_ROLE ) ){
			SetSceneState( SceneState.CreateRole );

			return;
		}

		if (t_level.StartsWith ( ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_PREFIX ) ) {
			SetSceneState( SceneState.BattleField );

			return;
		}

		if (t_level.StartsWith ( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.MAIN_CITY ) )) {
			SetSceneState( SceneState.MainCity );
			
			return;
		}

		if (t_level.StartsWith ( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_CITY ) )) {
			SetSceneState( SceneState.AllianceCity );
			
			return;
		}

		if (t_level.StartsWith ( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.CARRIAGE ) )) {
			SetSceneState( SceneState.Carriage );
			
			return;
		}

		if (t_level.StartsWith ( SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.ALLIANCE_BATTLE ) ) ) {
			SetSceneState( SceneState.AllianceBattle );
			
			return;
		}

	}

	#endregion



    #region Debug Scene

    /// Request to Load Battle Replay "BattleReplay_V4".
    //	public static void EnterBattleReplay(){
    //		EnterNextScene.SetSceneToLoad( ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_REPLAY, true );
    //		
    //		Application.LoadLevel( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );
    //	}

    #endregion
}
