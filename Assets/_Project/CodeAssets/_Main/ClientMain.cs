//#define PROTO_TOOL

//#define PC_VIDEO

//#define SHOW_VIDEO_EVERY_TIME

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.Xml;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public enum EGameState{
	E_Init,
	E_ResDownLoad,  // download resource
	E_ReadRes, // reading resource
	E_Start, // game start when download and read resource completely 
	E_Loop,
	E_Over,
}

//public class ClientMain : MonoSingleton<ClientMain>
public class ClientMain : MonoBehaviour , SocketListener
{
	public struct RewardInfo
	{
		public int id;
		
		public int count;
	}
	
	public struct PopUpData
	{
		public int iLevel;
		public int iType;
		public string sData;
		public AddPopUpCallback Callback;
	}
	
	public static List<PopUpData> m_listPopUpData = new List<PopUpData>();
	public static bool m_isNewOpenFunction = false;
	//
	//	public static List<int> m_listPopUpLevel = new List<int>();
	//
	//	public static List<int> m_listPopUpType = new List<int>();
	//
	//	public static List<string> m_listPopUpData = new List<string>();
	//		 
	//	public static List<AddPopUpCallback> m_AddPopUpCallback = new List<AddPopUpCallback>();
	//
	public delegate bool AddPopUpCallback(string data);
	
	public static GameObject m_akkack;
	
	public static GameObject m_rect;
	
	public static GameObject m_line;
	
	public static ClientMain m_ClientMain = null;
	
	public static GameObject m_ClientMainObj = null;
	
	public static UITextManager m_UITextManager = null;
	
	public static UIAddZhanliManager m_UIAddZhanliManager = null;
	
	public static bool m_isOpenQIRI = false;
	
	public static bool m_isOpenQianDao = false;
	
	public SoundPlayYuyin m_SoundPlayEff;
	
	/** Get Instance, if null report error.
	 */
	public static ClientMain Instance(){
		if( m_ClientMain == null ){
			//			Debug.LogError( "ClientMain.instance = null." );
		}
		
		return m_ClientMain;
	}
	
	// Only Get.
	public static ClientMain GetInstance(){
		return m_ClientMain;
	}
	
	public static float m_fScale = 0f;
	public static int m_iMoveX = 0;
	public static int m_iMoveY = 0;
	
	public static float m_TotalWidthInCoordinate
	{
		get { return 960 + 2*m_iMoveX; }
	}
	
	public static float m_TotalHeightInCoordinate
	{
		get { return 640 + 2*m_iMoveY; }
	}
	
	public AudioSource m_sound1;
	public AudioSource m_sound2;
	public bool debugModel = false;
	
	public UIDialogSystem m_UIDialogSystem;
	
	private GameObject m_DialogSystemObj;
	
	public Font m_Font;
	
	private bool m_is_show_drama_switcher = false;
	private bool m_is_show_guide_switcher = false;
	
	public void loadover(){
		
		//UIYindao.m_UIYindao.setOpenYindao(110001);
	}
	
	#region Mono
	
	void Awake(){
		#if PROTO_TOOL
		var prefab = Resources.Load("_Remove_When_Build/ProtoToolRoot");
		var ins = Instantiate(prefab) as GameObject;
		DontDestroyOnLoad(ins);
		#endif
		
		float scalex = (float)(Screen.width / 960f);
		float scaley = (float)(Screen.height / 640f);
		m_fScale = scalex > scaley ? scaley : scalex;
		
		m_iMoveX = (int)((Screen.width - 960 * m_fScale) / 2 / m_fScale);
		
		m_iMoveY = (int)((Screen.height - 640 * m_fScale) / 2 / m_fScale);
		
		if( m_ClientMain != null ){
			if( m_ClientMain.m_DialogSystemObj != null ){
				m_ClientMain.m_DialogSystemObj.SetActive( false );
				
				Destroy( m_ClientMain.m_DialogSystemObj );
			}
			
			{
				m_ClientMain.gameObject.SetActive( false );
				
				Destroy( m_ClientMain.gameObject );
			}
			
			{
				m_ClientMain = null;
				
				m_ClientMainObj = null;
			}
			
			return;
		}
		
		m_akkack = Resources.Load("_3D/Fx/Prefabs/BattleEffect/yujingguangquan_yuanxing") as GameObject;
		m_rect = Resources.Load("_3D/Fx/Prefabs/BattleEffect/yujingguangquan_juxing") as GameObject;
		m_line = Resources.Load("_3D/Fx/Prefabs/BattleEffect/yujingguangquan_xian") as GameObject;
	}
	
	void Start(){
		m_ClientMain = this;
		
		m_ClientMainObj = gameObject;
		
		{
			PlatformHelper.ResetPlatformSettings();
		}
		
		if( m_UnLoadManager == null ){
			m_UnLoadManager = new UnLoadManager( this );
		}
		
		{
			m_sound_manager = new SoundManager( m_sound1, m_sound2 );

		    s_MSCPlayer = MSCPlayer.Instance;
		}
		
		if(m_UITextManager == null)
		{
			m_UITextManager = new UITextManager();
		}
		
		if(m_UIAddZhanliManager == null)
		{
			m_UIAddZhanliManager = new UIAddZhanliManager();
		}
		
		//		Debug.Log("==========================1");
		
		UnLoadManager.setLoad( loadover, UnLoadManager.ELoadState.E_Default );//????????? ???????????
		
		DontDestroyOnLoad( gameObject );
		
		float scalex = (float)(Screen.width / 960f);
		float scaley = (float)(Screen.height / 640f);
		
		if (scalex > scaley){
			m_fScale = scaley;
		}
		else{
			m_fScale = scalex;
		}
		
		m_iMoveX = (int)((Screen.width - 960 * m_fScale) / 2 / m_fScale);
		m_iMoveY = (int)((Screen.height - 640 * m_fScale) / 2 / m_fScale);
		
		//		Debug.Log(m_iMoveX);
		//		Debug.Log(m_iMoveY);
		//		Global.getStringColor("ffb12a");
		
		if(debugModel == false)
		{
			StartCoroutine( PlayVideo() );
		}
		SocketTool.RegisterSocketListener(this);
	}
	
	public void OnGUI(){
		if(Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_LOGIN ){
			if( Global.m_isOpenJiaoxue && ConfigTool.GetBool( ConfigTool.CONST_SHOW_GUIDE_SWITCHER ) ){
				if(GUI.Button(new Rect( 0,0,600,50 ), 
				              //				              "关闭引导(*慎点*，开发时快速调试用，遇异常请忽略)" ) ){
				              "Close Guide?" ) ){
					Global.m_isOpenJiaoxue = false;
					
					UIYindao.m_UIYindao.gameObject.SetActive( false );
				}
			}
			
			//			TaskData.Instance.m_TaskInfoDic[100001].progress >= 0
		}
		
		#if PC_VIDEO
		if ( m_stream_ready && m_movie_texture != null ){
			GUI.DrawTexture (new Rect( 0, 0, Screen.width, Screen.height ), m_movie_texture );
		}
		#endif
	}
	
	void Update(){
		if(m_UnLoadManager != null){
			m_UnLoadManager.UpData ();
		}
		
		if( m_sound_manager != null ){
			m_sound_manager.updata();
		}
		
		if( m_UITextManager != null)
		{
			m_UITextManager.Update();
		}
		
		if(m_UIAddZhanliManager != null)
		{
			m_UIAddZhanliManager.Update();
		}
		
		if(!m_isNewOpenFunction)
		{
			//			Debug.Log(m_listPopUpData.Count);
			for(int i = 0; i < m_listPopUpData.Count; i ++)
			{
				if(m_listPopUpData[i].iType == 0)
				{
					switch(m_listPopUpData[i].iLevel)
					{
					case 5:
						if(MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.setChange(m_listPopUpData[i].sData))
						{
							m_isNewOpenFunction = true;
							m_listPopUpData.RemoveAt(i);
							break;
						}
						else
						{
							Debug.Log("不能开启任务领奖");
						}
						break;
					case 10:
						if(MainCityUI.m_MainCityUI.m_MainCityUIL.m_MainCityTaskManager.setChange(m_listPopUpData[i].sData))
						{
							m_isNewOpenFunction = true;
							m_listPopUpData.RemoveAt(i);
							break;
						}
						break;
					case 20:
						if(JunZhuData.Instance().ShowLevelUp(m_listPopUpData[i].sData))
						{
							m_isNewOpenFunction = true;
							m_listPopUpData.RemoveAt(i);
							break;
						}
						break;
					case 30:
						
						break;
					case 40:
						if(MainCityUI.m_MainCityUI.openAddFunction(m_listPopUpData[i].sData))
						{
							m_isNewOpenFunction = true;
							m_listPopUpData.RemoveAt(i);
							break;
						}
						break;
					case 50:
						break;
						
					case 70:
						break;
					}
				}
			}
		}
		#if PC_VIDEO
		if( m_movie_texture != null ){
			if( Input.anyKeyDown ){
				m_stream_ready = false;
				
				m_movie_texture.Stop();
				
				m_movie_texture = null;
			}
			else if( !m_movie_texture.isPlaying ){
				m_stream_ready = false;
				
				m_movie_texture = null;
			}
		}
		#endif
	}
	
	public void ManualDestroy(){
		
	}
	
	void OnDestroy(){
		
	}
	
	void OnApplicationQuit(){
		//        m_eGameState = EGameState.E_Over;
		//        ResMgr.Inst().Destroy();
		//        LogWriter.Instance.FlushLog();
		//        MsgFactory.Inst().Destroy();
		//        Caching.CleanCache();
	}
	
	#endregion
	
	
	
	#region Play Video
	
	#if PC_VIDEO
	public MovieTexture m_movie_texture;
	
	private bool m_stream_ready = false;
	
	#endif
	
	IEnumerator PlayVideo(){
		//		int t_default = 0;
		//
		//		#if SHOW_VIDEO_EVERY_TIME
		//		PlayerPrefs.DeleteKey( ConstInGame.CONST_FIRST_TIME_TO_PLAY_VIDEO );
		//		#endif
		//
		//		if( PlayerPrefs.GetInt( ConstInGame.CONST_FIRST_TIME_TO_PLAY_VIDEO, t_default ) != t_default ){
		//			VideoPlayedDone();
		//
		//			yield break;
		//		}
		//		else{
		//			PlayerPrefs.SetInt( ConstInGame.CONST_FIRST_TIME_TO_PLAY_VIDEO, 1 );
		//			
		//			PlayerPrefs.Save();
		//		}
		//
		//		string t_path = "Video/x7.mp4";
		//
		//		#if PC_VIDEO
		//		{
		//			if( m_movie_texture == null ){
		//				Debug.LogError( "Error, No Available Movie Assigned, please Install \"QuickTimeInstaller.exe\", then restart your computer, and finally reimport all videos." );
		//
		//				VideoPlayedDone();
		//
		//				yield break;
		//			}
		//
		//			m_movie_texture.loop = false;
		//
		//			AudioSource t_audio = gameObject.AddComponent<AudioSource>();
		//
		//			t_audio.clip = m_movie_texture.audioClip;
		//
		//			t_audio.Play();
		//			
		//			m_movie_texture.Play();
		//			
		//			m_stream_ready = true;
		//
		//			while( m_stream_ready ){
		//				yield return new WaitForEndOfFrame();
		//			}
		//
		//			yield return new WaitForEndOfFrame();
		//		}
		//		#elif UNITY_IOS || UNITY_ANDROID
		//		{
		//			Handheld.PlayFullScreenMovie( t_path, Color.black );
		//
		//			yield return new WaitForEndOfFrame();
		//
		//			yield return new WaitForEndOfFrame();
		//
		//			yield return new WaitForEndOfFrame();
		//		}
		//		#endif
		
		VideoPlayedDone();
		
		yield return null;
	}
	
	private void VideoPlayedDone(){
		{
			AudioSource t_audio = gameObject.GetComponent<AudioSource>();
			
			if( t_audio != null ){
				t_audio.enabled = false;
				
				Destroy( t_audio );
			}
		}
		
		ManualLoadAssets();
	}
	
	//	private List<EventDelegate> m_movie_done_callback = new List<EventDelegate>();
	//
	//	public void AddMovieDoneCallback( EventDelegate.Callback p_callback ){
	//		EventDelegate.Add( m_movie_done_callback, p_callback );
	//	}
	
	#endregion
	
	
	
	#region Loading
	
	private int CONST_BASIC_ASSET_TO_LOAD_COUNT		= 6;
	
	private int m_basic_assets_loaded_count 		= 0;
	
	public static void ManualLoadAssets(){
		//		Debug.Log( Time.realtimeSinceStartup + "ClientMain.ManualLoadAssets()" );
		
		if( Instance() != null ){
			Instance().LoadBasicAssets();
		}
		else{
			Debug.Log( "ClientMain.Instance() = null." );
		}
	}
	
	private void LoadBasicAssets(){
		//		Debug.Log( Time.realtimeSinceStartup + "ClientMain.LoadBasicAssets()" );
		
		{
			m_templates_loaded = 0;
		}
		
		// load basic assets
		{
			m_basic_assets_loaded_count = 0;

			Res2DTemplate.LoadTemplates( BasicAssetsLoadCallback );

			LanguageTemplate.LoadTemplates( BasicAssetsLoadCallback );

			MyColorData.LoadTemplates( BasicAssetsLoadCallback );
			
			ConfigTool.Instance.LoadConfigs( BasicAssetsLoadCallback );

			PropertyTool.Instance.LoadConfigs( BasicAssetsLoadCallback );



			QualityTool.Instance.LoadQualities( BasicAssetsLoadCallback );
			
			
			
			LocalCacheTool.Instance().LoadConfig();
		}
	}
	
	public void BasicAssetsLoadCallback(){
		m_basic_assets_loaded_count++;
		
		//		Debug.Log( "ClientMain.AwakeLoadCallback( " + 
		//		          m_awake_loaded_asset_count + 
		//		          " / " + 
		//		          CONST_AWAKE_LOAD_ASSET_COUNT + " )" );
		
		if( m_basic_assets_loaded_count == CONST_BASIC_ASSET_TO_LOAD_COUNT ){
			//			Debug.Log( "ClientMain.Continue Loading -> Login, Network, DialogSystem, YinDao." );

			// load network
			{
				NetworkWaiting.Instance( true );
			}

			{
				// load login
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LOGIN ),
				                        LoginLoadCallback );
			}
			
			{
				UI3DEffectTool.Instance();
				
				SceneGuideManager.Instance();
				
				PopUpLabelTool.Instance();
				
				ComponentHelper.RegisterGlobalComponents();
				
				VersionTool.Instance().Init();
			}
			
			// load assets
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG ),
				                        DialogSystemLoadCallback );
				
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_YINDAO ),
				                        YinDaoLoadCallback );
				if(UIShouji.m_UIShouji == null)
				{
					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.SHOUJI ),
					                        YinDaoLoadCallback );
				}
			}
			
			// async load after announcement
			//			{
			//				LoadTemplates();
			//			}
			
			{
				SceneManager.SetSceneState( SceneManager.SceneState.Login );
			}
		}
	}
	
	public void LoginLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		//		Debug.Log( Time.realtimeSinceStartup + "LoginLoadCallback( " + p_path + " )" );
		
		Instantiate( p_object );
	}
	
	public void DialogSystemLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		m_DialogSystemObj = Instantiate( p_object ) as GameObject;
		
		m_UIDialogSystem = m_DialogSystemObj.GetComponentInChildren<UIDialogSystem>();
		
		m_UIDialogSystem.gameObject.SetActive( false );
		
		DontDestroyOnLoad( m_DialogSystemObj );
	}
	
	public void YinDaoLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		Instantiate( p_object );
	}
	
	#endregion
	
	
	
	#region Init
	
	private static float m_begin_loading_time 	= 0;
	
	private static int m_templates_loaded 		= 0;
	
	private const int CONST_TEMPLATES_COUNT		= 144;
	
	public static bool m_is_templates_loaded 	= false;
	
	public static void LoadSoundTemplate(){
		//		Debug.Log ( Time.realtimeSinceStartup + ".LoadSoundTemplate()" );
		
		m_sound_manager.LoadTemplates();
	}
	
	public static void LoadTemplates(){
		//		Debug.Log ( Time.realtimeSinceStartup + ".LoadTemplates()" );
		
		ClientMain.Instance().StartCoroutine ( ClientMain.Instance().ExeLoadTemplates() );
	}
	
	IEnumerator ExeLoadTemplates(){
		if ( m_ClientMain.debugModel == true ) {
			yield break;
		}
		
		{
			yield return new WaitForSeconds( 0.1f );
		}
		
		if( m_is_templates_loaded ){
			yield break;
		}
		
		m_begin_loading_time = Time.realtimeSinceStartup;
		
		// 1 - 10
		{
			SceneTemplate.LoadTemplates( TemplateLoadedCallback );
			
			CanshuTemplate.LoadTemplates( TemplateLoadedCallback );
			
			MyColorData.LoadTemplates( TemplateLoadedCallback );
			
			RenWuTemplate.LoadTemplates( TemplateLoadedCallback );
			
			CreateRoleTemplate.LoadTemplates( TemplateLoadedCallback );

			
			
			BattleMibaoEffTemplate.LoadTemplates( TemplateLoadedCallback );
			
			DangPuTemplate.LoadTemplates ( TemplateLoadedCallback );
			
			DangpuItemCommonTemplate.LoadTemplates( TemplateLoadedCallback );
			
			SkillTemplate.LoadTemplates( TemplateLoadedCallback );

			BattleConfigTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		
		// 11 - 20
		{
			NameIdTemplate.LoadTemplates( TemplateLoadedCallback );
			
			DescIdTemplate.LoadTemplates( TemplateLoadedCallback );
			
			FangWuInfoTemplate.LoadTemplates( TemplateLoadedCallback );
			
			JiangLiTemplate.LoadTemplates( TemplateLoadedCallback );
			
			ChongZhiTemplate.LoadTemplates( TemplateLoadedCallback );
			
			
			
			ModelTemplate.LoadTemplates( TemplateLoadedCallback );
			
			EmailTemp.LoadTemplates( TemplateLoadedCallback );
			
			PVEZuoBiaoTemplate.LoadTemplates( TemplateLoadedCallback );
			
			BaiZhanTemplate.LoadTemplates( TemplateLoadedCallback );
			
			JunzhuShengjiTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		
		// 21 - 30
		{
			AwardTemp.LoadTemplates( TemplateLoadedCallback );
			
			ItemTemp.LoadTemplates( TemplateLoadedCallback );
			
			HeroProtoTypeTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HeroGrowTemplate.LoadTemplates( TemplateLoadedCallback );
			
			EffectIdGroup.LoadTemplates( TemplateLoadedCallback );
			
			
			
			ShuXingTemp.LoadTemplates(TemplateLoadedCallback);
			
			HeroTypeTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LabelTemplate.LoadTemplates( TemplateLoadedCallback );
			
			NpcTemplate.LoadTemplates( TemplateLoadedCallback );
			
			NpcCityTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		
		// 31 - 40
		{
			JingPoTemplate.LoadTemplates( TemplateLoadedCallback );
			
			PurchaseTemplate.LoadTemplates( TemplateLoadedCallback );
			
			BaoJiTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HeroStarTemplate.LoadTemplates( TemplateLoadedCallback );
			
			TextIdTemplate.LoadTemplates( TemplateLoadedCallback );
			
			
			
			PlotChatTemplate.LoadTemplates( TemplateLoadedCallback );
			
			CameraTemplate.LoadTemplates( TemplateLoadedCallback );
			
			NameKuTemplate.LoadTemplates( TemplateLoadedCallback );
			
			DialogData.LoadTemplates( TemplateLoadedCallback );
			
			SoundIdGroup.LoadTemplates( TemplateLoadedCallback );
		}
		
		// 41 - 50
		{
			ControlOrderLvTemplate.LoadTemplates( TemplateLoadedCallback );
			
			YindaoTemp.LoadTemplates( TemplateLoadedCallback );
			
			AllianceIconTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LianmengMoBaiTemplate.LoadTemplates( TemplateLoadedCallback );
			
			GongJiTypeTemplate.LoadTemplates ( TemplateLoadedCallback );
			

			
			ExpXxmlTemp.LoadTemplates( TemplateLoadedCallback );
			
			VipTemplate.LoadTemplates( TemplateLoadedCallback );
			
			YouxiaPveTemplate.LoadTemplates(TemplateLoadedCallback);
			
			TaoZhuangTemplate.LoadTemplates( TemplateLoadedCallback );
			
			DiaoLuoTemplate.LoadTemplates( TemplateLoadedCallback );
		}

		// 51 - 60
		{
			MiBaoXmlTemp.LoadTemplates ( TemplateLoadedCallback );
			
			MiBaoSuipianXMltemp.LoadTemplates ( TemplateLoadedCallback );
			
			MiBaoDiaoLuoXmlTemp.LoadTemplates ( TemplateLoadedCallback );
			
			MiBaoStarTemp.LoadTemplates ( TemplateLoadedCallback );
			
			PveFunctionOpen.LoadTemplates ( TemplateLoadedCallback );
			
			
			
			MiBaoSkillTemp.LoadTemplates ( TemplateLoadedCallback );
			
			ZhuXianTemp.LoadTemplates( TemplateLoadedCallback );
			
			KingMovementTemplate.LoadTemplates( TemplateLoadedCallback );
			
			FunctionOpenTemp.LoadTemplates( TemplateLoadedCallback );
			
			DuiHuanTemplete.LoadTemplates ( TemplateLoadedCallback );
		}
		
		// 61 - 70
		{
			GuYongBingTempTemplate.LoadTemplates ( TemplateLoadedCallback );
			
			KingCrashTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HuangYeAwardTemplete.LoadTemplates ( TemplateLoadedCallback );
			
			HuangyeTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HuangYeFogTemplete.LoadTemplates( TemplateLoadedCallback );
			
			
			
			HuangYePVPTemplate.LoadTemplates(TemplateLoadedCallback);
			
			HuangYePveTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HuangyeNPCTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LegendNpcTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LegendPveTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		
		// 71 - 80
		{
			PveTempTemplate.LoadTemplates(TemplateLoadedCallback);
			
			PveStarTemplate.LoadTemplates(TemplateLoadedCallback);
			
			VipFuncOpenTemplate.LoadTemplates(TemplateLoadedCallback);
			
			HuoDongTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LMBubbleTemplate.LoadTemplates(TemplateLoadedCallback);


			
			QianDaoMonthTemplate.LoadTemplates(TemplateLoadedCallback);
			
			HouseWeaponTemplate.LoadTemplates(TemplateLoadedCallback);
			
			HouseTreasureTemplate.LoadTemplates(TemplateLoadedCallback);
			
			EffectIdTemplate.LoadTemplates(TemplateLoadedCallback);
			
			WeaponSkillOpenTemplate.LoadTemplates(TemplateLoadedCallback);
		}
		
		//81-90
		{
			CommonItemTemplate.LoadTemplates(TemplateLoadedCallback);
			
			CartTemplate.LoadTemplates(TemplateLoadedCallback);
			
			CartRouteTemplate.LoadTemplates(TemplateLoadedCallback);
			
			TalentTemplate.LoadTemplates(TemplateLoadedCallback);
			
			YouXiaNpcTemplate.LoadTemplates(TemplateLoadedCallback);
			


			LegendPveZuoBiaoTemplate.LoadTemplates(TemplateLoadedCallback);
			
			XianshiControlTemp.LoadTemplates(TemplateLoadedCallback);
			
			XianshiHuodongTemp.LoadTemplates(TemplateLoadedCallback);
			
			ZhuangBei.LoadTemplates( TemplateLoadedCallback );
			
			HuangYeDuiHuanTemplate.LoadTemplates ( TemplateLoadedCallback );
		}
		
		// 91-100
		{
			FuWenTemplate.LoadTemplates( TemplateLoadedCallback );
			
			FuWenOpenTemplate.LoadTemplates( TemplateLoadedCallback );
			
			FuWenJiaChengTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LianmengzhanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			ChenghaoTemplate.LoadTemplates(TemplateLoadedCallback);


			
			LMDuiHuanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LMFightDuiHuanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LMZBuildingTemplate.LoadTemplates(TemplateLoadedCallback);
			
			XiLianShuXingTemp.LoadTemplates(TemplateLoadedCallback);
			
			YouXiaOpenTimeTemplate.LoadTemplates(TemplateLoadedCallback);
		}
		
		//101-110
		{
			MiBaoJiXingTemplate.LoadTemplates(TemplateLoadedCallback);
			
			ZBChushiDiaoluoTemp.LoadTemplates(TemplateLoadedCallback);
			
			ExpTempTemp.LoadTemplates(TemplateLoadedCallback);
			
			EventTriggerTemplate.LoadTemplates( TemplateLoadedCallback );
			
			RTActionTemplate.LoadTemplates(TemplateLoadedCallback);
			
			
			
			RTBuffTemplate.LoadTemplates(TemplateLoadedCallback);
			
			RTSkillTemplate.LoadTemplates(TemplateLoadedCallback);
			
			YunBiaoSafeTemplate.LoadTemplates(TemplateLoadedCallback);
			
			QiriQiandaoTemplate.LoadTemplates(TemplateLoadedCallback);
			
			MiBaoExtrattributeTemplate.LoadTemplates(TemplateLoadedCallback);
		}
				
		//111-120
		{
			LianMengTuTengTemplate.LoadTemplates(TemplateLoadedCallback);
			
			HeroSkillUpTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianmengEventTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianMengKeJiTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianMengKeZhanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			
			
			LianMengShangPuTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianMengShuYuanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianMengZongMiaoTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LianMengTemplate.LoadTemplates(TemplateLoadedCallback);
			
			ReportTemplate.LoadTemplates(TemplateLoadedCallback);
		}
		
		//121-130
		{
			FunctionUnlock.LoadTemplates(TemplateLoadedCallback);
			
			PveAwardTemplate.LoadTemplates(TemplateLoadedCallback);
			
			MiBaoSkillLvTempLate.LoadTemplates(TemplateLoadedCallback);
			
			YunBiaoTemplate.LoadTemplates(TemplateLoadedCallback);
			
			MaJuTemplate.LoadTemplates( TemplateLoadedCallback );
			
			
			
			QiangHuaTemplate.LoadTemplates( TemplateLoadedCallback );
			
			GuideTemplate.LoadTemplates( TemplateLoadedCallback );
			
			HuoYueTempTemplate.LoadTemplates( TemplateLoadedCallback );
			
			BubblePopTemplate.LoadTemplates( TemplateLoadedCallback );
			
			BubbleTextTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		
		//131-140
		{
			AnnounceTemplate.LoadTemplates( TemplateLoadedCallback );
			
			LianmengFengshanTemplate.LoadTemplates(TemplateLoadedCallback);
			
			VIPQianDaoTemp.LoadTemplates(TemplateLoadedCallback);
			
			BaiZhanRankTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LueDuoPersonRankTemplate.LoadTemplates(TemplateLoadedCallback);
			


			LueDuoUnionRankTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LueDuoLianmengRankTemplate.LoadTemplates(TemplateLoadedCallback);
			
			RobCartXishuTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LoadingTemplate.LoadTemplates(TemplateLoadedCallback);
			
			LMTargetTemplate.LoadTemplates(TemplateLoadedCallback);
		}

		//141-150
		{
			ChuShiNuQiTemplate.LoadTemplates(TemplateLoadedCallback);

			BattleAppearanceTemplate.LoadTemplates( TemplateLoadedCallback );

			ChengJiuTemplate.LoadTemplates( TemplateLoadedCallback );
			
            FangWuTemplate.LoadTemplates(TemplateLoadedCallback);

			FuWenDuiHuanTemplate.LoadTemplates( TemplateLoadedCallback );



			MibaoNewTemplate.LoadTemplates(TemplateLoadedCallback);

			JCZCityTemplate.LoadTemplates (TemplateLoadedCallback);

			JCZTemplate.LoadTemplates (TemplateLoadedCallback);

			HYRankAwardTemplate.LoadTemplates (TemplateLoadedCallback);

			MibaoNewSuipianTemplate.LoadTemplates(TemplateLoadedCallback);
		}

		//151 -160
		{
			ChonglouPveTemplate.LoadTemplates( TemplateLoadedCallback );

			ChongLouNpcTemplate.LoadTemplates( TemplateLoadedCallback );

			FunctionLinkTemplate.LoadTemplates( TemplateLoadedCallback );

            BaoShiOpenTemplate.LoadTemplates(TemplateLoadedCallback);
       
			LieFuTemplate.LoadTemplates( TemplateLoadedCallback );



			MishuTemplate.LoadTemplates(TemplateLoadedCallback);		

			CountryZuoBiaoTemplate.LoadTemplates(TemplateLoadedCallback);	

			FuWenTabTemplate.LoadTemplates(TemplateLoadedCallback);	

			VipGiftTemplate.LoadTemplates( TemplateLoadedCallback );

			LianmengJuanxianTemplate.LoadTemplates(TemplateLoadedCallback);
		}

		// 161-170
		{
			GuideInfoTemplate.LoadTemplates( TemplateLoadedCallback );

			TalentArrTemplate.LoadTemplates( TemplateLoadedCallback );
		}
		  
		m_is_templates_loaded = true;
	}
	
	public static void TemplateLoadedCallback(){
		m_templates_loaded++;
		
		//		Debug.Log( "TemplateLoadedCallback( " + m_templates_loaded + " - " +
		//				( Time.realtimeSinceStartup - m_begin_loading_time ) + " )" );
		
		if( m_templates_loaded == CONST_TEMPLATES_COUNT ){
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_TOTAL_LOADING_TIME, true ) ){
				Debug.Log( "------ All " + CONST_TEMPLATES_COUNT + " Templates Loaded: " + 
				          ( Time.realtimeSinceStartup - m_begin_loading_time ) +
				          " ------" );
			}
		}
	}
	
	/// <summary>
	/// Clear all chat objects when connect again.
	/// </summary>
	public void ClearObjectsWhenReconnect()
	{
		if (ChatWindow.s_ChatWindow != null)
		{
			if (ChatWindow.s_ChatWindow.gameObject != null)
			{
				Destroy(ChatWindow.s_ChatWindow.ChatRoot.gameObject);
			}
			
			ChatWindow.s_ChatWindow = null;
		}
		
		if (EnterGame.s_HighestUI != null)
		{
			Destroy(EnterGame.s_HighestUI.gameObject);
			EnterGame.s_HighestUI = null;
		}
		
		if (QXChatPage.chatPage != null)
		{
			if (QXChatPage.chatPage.gameObject != null)
			{
				Destroy (QXChatPage.chatPage.gameObject);
			}
			
			QXChatPage.chatPage = null;
		}
	}
	
	#endregion
	
	
	
	#region Utility
	
	
	
	#endregion
	
	public void SetGameState(EGameState EgameState)
	{
		m_eGameState = EgameState;
	}
	
	public EGameState GameState
	{
		get { return m_eGameState; }
		set { m_eGameState = value; }
	}
	
	public static void at1(GameObject at1, GameObject at2)
	{
		at1.transform.parent = at2.transform;
	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_TALENT_UP_CAN:
			{
				MainCityUI.SetRedAlert(500000, true);
				return true;
			}
			case ProtoIndexes.S_NOTICE_TALENT_CAN_NOT_UP:
				MainCityUI.SetRedAlert(500000, false);
				return true;
			case ProtoIndexes.S_GET_CUR_CHENG_HAO:
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				ChengHaoData tempInfo1 = new ChengHaoData();
				t_qx.Deserialize(t_stream, tempInfo1, tempInfo1.GetType());
				JunZhuData.m_iChenghaoID = tempInfo1.id;
				return true;
			case ProtoIndexes.S_NEW_CHENGHAO:
				t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				t_qx = new QiXiongSerializer();
				tempInfo1 = new ChengHaoData();
				t_qx.Deserialize(t_stream, tempInfo1, tempInfo1.GetType());
				if(Global.m_NewChenghao == null)
				{
					Global.m_NewChenghao = new List<int>();
				}
				Global.m_NewChenghao.Add(tempInfo1.id);
				string saveString = "";
				for(int i = 0; i < Global.m_NewChenghao.Count; i ++)
				{
					saveString += Global.m_NewChenghao[i] + ",";
				}
				PlayerPrefs.SetString( ConstInGame.CONST_NEW_CHENGHAO + JunZhuData.Instance().m_junzhuInfo.id, saveString );
				PlayerPrefs.Save();
				
				MainCityUI.SetRedAlert(500015, true);
				return true;
			case ProtoIndexes.FUNCTION_OPEN_NOTICE:
				t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				t_qx = new QiXiongSerializer();
				ErrorMessage tempInfo2 = new ErrorMessage();
				t_qx.Deserialize(t_stream, tempInfo2, tempInfo2.GetType());
				
				if(tempInfo2.errorCode < 0)
				{
					int id = Math.Abs(tempInfo2.errorCode);
					FunctionOpenTemp.m_EnableFuncIDList.Remove(id);
					if(MainCityUI.m_MainCityUI != null)
					{
						MainCityUI.m_MainCityUI.deleteMaincityUIButton(id);
					}
				}
				else
				{
					if(!FunctionOpenTemp.IsHaveID(tempInfo2.errorCode) && FunctionOpenTemp.GetTemplateById(tempInfo2.errorCode) != null)
					{
						FunctionOpenTemp.EnableFunctionId( tempInfo2.errorCode );
						
						if(MainCityUI.m_MainCityUI != null)
						{
							MainCityUI.m_MainCityUI.AddButton(tempInfo2.errorCode);
						}
					}
				}
				return true;
			case ProtoIndexes.S_FUWEN_TIPS:
				MainCityUI.SetRedAlert(500010, true);
				return true;
			default: return false;
			}
		}
		return false;
	}
	private int m_iCurPopUpID = -1;
	public static void addPopUP(int Level, int type, string data, AddPopUpCallback callback)
	{
		//		Debug.Log("========================1");
		//		Debug.Log(Level);
		//		Debug.Log(data);
		for(int i = 0; i < m_listPopUpData.Count; i++)
		{
			if(m_listPopUpData[i].iLevel == Level && (Level != 40 && Level != 50))
			{
				//                Debug.LogWarning("============"+Level);
				
				return;
			}
		}
		PopUpData temp = new PopUpData();
		temp.iLevel = Level;
		temp.iType = type;
		temp.sData = data;
		temp.Callback = callback;
		if(m_listPopUpData.Count == 0)
		{
			m_listPopUpData.Add(temp);
			return;
		}
		for(int i = 0; i < m_listPopUpData.Count; i ++)
		{
			if(m_listPopUpData[i].iLevel > Level)
			{
				PopUpData other = new PopUpData();
				//				if(i == m_listPopUpData.Count - 1)
				//				{
				//					other = m_listPopUpData[i];
				//					m_listPopUpData[i] = temp;
				//					m_listPopUpData.Add(other);
				//					return;
				//				}
				other = m_listPopUpData[m_listPopUpData.Count - 1];
				
				for(int q = m_listPopUpData.Count - 1; q > i; q --)
				{
					m_listPopUpData[q] = m_listPopUpData[q - 1];
				}
				m_listPopUpData[i] = temp;
				m_listPopUpData.Add(other);
				return;
			}
		}
		m_listPopUpData.Add(temp);
	}
	
	public static void closePopUp()
	{
		//		Debug.Log("ClosePopUp");
		
		m_isNewOpenFunction = false;
		if(MainCityUI.m_MainCityUI != null)
		{
			MainCityUI.m_MainCityUI.setInit();
		}
	}
	private EGameState m_eGameState = EGameState.E_Init;
	
	public static UnLoadManager m_UnLoadManager;
	
	public static bool M_BRESLOAD = true;
	
	public static SoundManager m_sound_manager;

	public static MSCPlayer s_MSCPlayer;
}