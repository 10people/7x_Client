using UnityEngine;
using System.Collections;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityGlobalData  {

	public enum skillLevelId
	{
		zhongji = 0,
		bahuanglieri,
		qiankundouzhuan,
		tuci,
		jueyingxingguangzhan,
		xuejilaoyin,
		jishe,
		zhuixingjian,
		hanbingjian,
		fangun
	}

	public static int[] mPveStar = new int[3] ;

	public static bool QCLISOPen = false;//判断千重楼是UI是否打开

	public static bool IsFistGetMiBaoData = true;//判断是否是第一次获取秘宝数据

	public static bool autoFightDebug = false;//自动不停的战斗的测试

	public static bool  PveLevel_UI_is_OPen = false;

	public static bool  MibaoSatrUpCallback = false;//秘宝升星返回判断

	public static int  AllianceEventNotice ;

	public static int  AllianceApplyNotice ;

	public static int  CurrentHY_Capter = 0;

	public static bool IsOPenHyLeveUI = false;

	public static bool dramable = true;

	public static int m_save;

	public static EnterBattleField.BattleType m_battleType;

	public static int battleTemplateId;

	public static long m_tempPoint;//pvp战斗对手id
	public static long m_tempEnemy;//pvp战斗对手id

	public static int m_tempSection;//pve战斗章id

	public static int m_pve_max_level;//pve进度
	 
	public static int m_configId;//战斗配置文件id

	public static ZhanDouInitResp t_resp;//战斗数据

	public static ZhanDouInitError t_respError;//战斗错误数据

	public static string t_next_battle_field_scene;//战斗场景名

	public static bool m_debugPve = false;//测试全部pve模式

	public static int m_LastSection = 1;//pve战斗章的最后一章id

	public static string m_LastLoginName;

    public static int m_SeverTime;// 服务器时间
    public static int m_ShowSelectType = 0;//点击战力弹出 不同类型的界面 0 进阶 1 强化 2 洗练
	public static int m_temp_CQ_Section;//pve战斗章传奇id

    public static int m_Limite_Activity_Type;//显示活动类型
	public static bool PT_Or_CQ = true;//判断当前攻打的是普通关卡还是传奇关卡 True为普通 false为传奇
    public static int m_tempLevel;//pve战斗节id
	public static int m_king_icon;//战斗中君主头像id
    public static int m_king_model_Id;//君主展示中模型Id
	public static LevelType m_levelType;//关卡类型
    public static Vector3 m_EnterCityPosition;
    public static bool m_CreateRoleCurrent = false;
    public static bool m_isAllianceScene = false;
    public static bool m_isMainScene = false;
    public static bool m_isAllianceTenentsScene = false;
    public static int m_iAllianceTenentsSceneNum = 0;
    public static bool m_isBattleField_V4_2D = false;
    //在劫镖场景
    public static bool m_isJieBiaoScene  = false;

    public static bool m_isCreatedMoveLab= false;
    public static bool m_isWashMaxSignal = false;//xilianMaxTiShi
    public static bool m_isWashMaxSignalConfirm = false;//xilianMaxTiShi

    public static bool m_isNavToHome = false;
    public static int m_HomeIdSaved = 0;

    public static bool m_isNavToHouse = false;

    public static bool m_isNavToAllianCity = false;
    public static int m_AllianceCityIdSaved = 0;
    public static int m_TaskNavID = 0;

    public static bool m_isNavToAllianCityToTenement = false;

	public static bool m_showLevelupEnable = false;//当前是否可以显示升级(战斗用)
    public static bool m_isWhetherOpenLevelUp = true;
    public static bool TaskLingQu = false;//领取任务奖励
    public static bool TaskUpdata = false;//
    public static int m_TaskType = 0;

    public static bool m_AllianceIsHave = false;
    public static bool m_RefreshEquipInfo = false; 

    public static bool m_PlayerInCity = false;
    public static bool m_selfNavigation; //自动导航
	public static bool m_joystickControl;//遥感控制

	/** removed by ZhangYuGu, 2014.12.9
	 * 	make application easy to use,
	 *  make data sync.
	 */
//    public static string m_nextSceneName;//下一个scene名字 loading用到

    public static int m_getStarAward;

    public const int m_equipFalg = 20000; //装备标识
	//public const int m_
	public const int m_cardBag = 900008;

	public const int m_everyDaywardId = 1000;//个人奖励
	public const int m_countAwardID = 2000;//累计登陆奖励

    public static float m_ScreenWidth = Screen.width; //屏幕宽高
    public static float m_ScreenHeight = Screen.height;

    public static float m_HalfScreenWidth = m_ScreenWidth * 0.5f; 
    public static float m_HalfScreenHeight = m_ScreenHeight * 0.5f;

    public const string Color_White = "[c6c6c6]"; //16进制色值
    public const string Color_Green = "[49a71d]";
    public const string Color_Bule = "[0899ce]";
    public const string Color_Purple = "[aa31cc]";
    public const string Color_Orange = "[eb820a]";
  
	public static bool m_JunZhuCreate;//君主预设物体自动创建控制
	public static bool m_JunZhuAuto;

	public static bool m_JunZhuTouXiangGuide;//头像上的引导控制

//	public static bool m_LingQuJiangLi;//领取奖励引导控制

	public static bool m_JunZhuTouJinJieTag;//可以装备或进阶
	public static bool m_JunZhuEquipGuide;//君主穿装备引导
	public static bool m_UWDataRefresh = false;//跟踪判断所需数据的更新

    public static bool m_isRightGuide = false;
    public static bool m_isTouchLabToTask = false;
	//各品质颜色
	public const string Color_Equip_Green = "[10e12b]";		//RGB = (16,225,43)
	public const string Color_Equip_Purple = "[e85ef3]";	//RGB = (232,94,243)
	public const string Color_Equip_Orange = "[e16d10]"; 	//RGB = (255,109,16)
	public const string Color_Equip_Blue = "[10adff]";		//RGB = (16,173,255)
	public const string Color_Equip_White = "[eaecec]";		//RGB = (234,236,236)

	public static string RigisterURL; //注册
	public static string LoginURL; //登陆
   // public const string NoticeURL = "http://192.168.0.180:8080/qxrouter/sysNotice.jsp";//公告

    // 每日任务
    /** 请求每日任务列表 **/
	public const int m_npcBaseNum = 990000;

	//public static int m_currentEquipIndex;//装备在背包中的index；

	public const int m_maxFreq = 4000; //音频

    public static int  m_skillType; //君主技能类型

    public static Rect m_touchRect = new Rect(m_ScreenWidth * 0.02f,m_ScreenHeight * 0.02f,m_ScreenWidth * 0.2f,m_ScreenHeight*0.3f); //可点击区域

	public static int countryId;//国家id

	public  enum MibaoSkillType 
	{

		PveSend = 1,

		PVP_Fangshou = 2,

		HY_TreSend = 3,

		HY_ResSend = 4,

		PvpSend = 5,

		YaBiao_Fangshou = 6,

		YaBiao_Gongji = 7,

		YX_JinBi = 8,

		YX_Cailiao = 9,

		YX_Jingpo = 10,

		YX_WanbiGuizhao = 13,

		YX_ZongHengLiuHe = 14,

		LueDuo_GongJi = 12,

		QianChongLiu = 15,
	}

	/// <summary>
	/// 运劫镖次数购买类型 10-押镖，20-劫镖
	/// </summary>
	private static int yunBiaoBuyType;
	public static int SetYunBiaoBuyType
	{
		set{yunBiaoBuyType = value;}
	}
	public static int GetYunBiaoBuyType
	{
		get{return yunBiaoBuyType;}
	}

	public static int GetUpdrageData(int tempPinzhi,int tempLevel)
	{
		int tempFactor = 0;
		int tempBaseNum = 0;
		int tempNumCount = 0;
		switch(tempPinzhi)
		{
		case 1:
		{
			
			break;
		}
		case 2:
		{
			tempFactor = 1;
			tempBaseNum = 10;
			break;
		}
		case 3:
		{
			tempFactor = 2;
			tempBaseNum = 20;
			break;
		}
		case 4:
		{
			tempFactor = 4;
			tempBaseNum = 30;
			break;
		};
		case 5:
		{
			tempFactor = 8;
			tempBaseNum = 40;
			break;
		}
		}
		
		tempNumCount = tempFactor * tempLevel + tempBaseNum;

		return tempNumCount;
	}

	public static void setDramable(int maxLevelId)
	{
		int curLevel = 100000 + m_tempSection * 100 + m_tempLevel;

		if(maxLevelId < curLevel) // new level
		{
			m_save = 2;
		}
		else // old level
		{
			m_save = 1;

			//m_save = 2;
		}
	}

	public static bool getDramable()
	{
		if (m_save == 1) return false;

		if (m_save == 2) return true;

		Debug.LogError ("getDramable() should NEVER got here");

		return true;
	}

}
