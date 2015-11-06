using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TiaoZhan : MonoBehaviour
{//挑战阵容

    [HideInInspector]
    public int my_ZhanLi;

    [HideInInspector]
    public int Enemy_ZhanLi;

    [HideInInspector]
    ChallengeResp g_TiaoZhanInfo;

    public UILabel mZhanli;

    public UILabel eZhanLi;

	public int Skill_ZUHE_Id;

	public int Enemy_Skill_ZUHE_Id;

	public int Enemy_MiBao_Num;

    [HideInInspector]

    public PVPInfo myHeroInfo;//我的君主的信息-MiBao

    //[HideInInspector]public MiBaoBriefInfo my_MiBaoInfo;//我的君主的信息-MiBao

    [HideInInspector]
    public BaiZhanInfoResp my_HeroInfo;//我的君主的信息



    [HideInInspector]

//    public List<MiBaoBriefInfo> my_MiBaoInfo = new List<MiBaoBriefInfo>();
//
//    public List<MiBaoBriefInfo> Enemy_MiBaoIfon = new List<MiBaoBriefInfo>();

    public ChallengeResp TiaoZhanInfo;

    [HideInInspector]

    public GameObject IconSamplePrefab;

    public GameObject m_No_SkillColider;//我方组合技能不可用

    public GameObject e_No_SkillColider;//敌方组合技能不可用

    public UISprite m_Skill_Icon;//我方组合技能icon

    public UISprite e_Skill_Icon;//敌方组合技能icon
    //private int[] m_Zuhe_Id = new int[3];
    private int[] m_Zuhe_Id = new int[3];//定义组合技能的ID 如果数字的数字相同那就激活组技能

    private int[] e_Zuhe_Id = new int[3];//定义组合技能的ID 如果数字的数字相同那就激活组技能
    public static List<int> my_MiBao = new List<int>();

    public List<BaiZhanMiBaoInfo> my_BaiZhanMiBaoInfo = new List<BaiZhanMiBaoInfo>();

    public List<BaiZhanMiBaoInfo> e_BaiZhanMiBaoInfo = new List<BaiZhanMiBaoInfo>();

    public List<IconSampleManager> GuyongbingList = new List<IconSampleManager>();

    [HideInInspector]

    public bool isRefresh;

    public static TiaoZhan mTiaoZhan;

	//public ResourceNpcInfo m_ResInfo;

	//public BattleResouceResp my_HYResourseBattleResp;

	public  GameObject mBackBtn;

	public int yinDaoState;

    void Awake()
    {
        mTiaoZhan = this;

    }
    void Start()
    {
        isRefresh = false;
    }

    public void Init()//发送挑战请求
    {
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0 && 
		   yinDaoState == 1)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
		}
		
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0 && 
		   
		   yinDaoState == 2)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[6]);
		}

		mBackBtn.SetActive(false);
        if (TiaoZhanInfo == null)
        {
			Debug.Log("TiaoZhanInfo == null ");
			return;
        }

		Skill_ZUHE_Id = TiaoZhanInfo.myZuheId;
		Debug.Log ("TiaoZhanInfo.oppZuheId：" + TiaoZhanInfo.oppZuheId);
		Debug.Log ("TiaoZhanInfo.myZuheId：" + TiaoZhanInfo.myZuheId);
		Enemy_Skill_ZUHE_Id = TiaoZhanInfo.oppZuheId;
		Enemy_MiBao_Num = TiaoZhanInfo.oppActivateMiBaoCount;

		Debug.Log ("isRefresh  = "+isRefresh );

		init_Hero_GuYongBing();
        mZhanli.text = TiaoZhanInfo.myZhanli.ToString();//显示我的战力

        eZhanLi.text = TiaoZhanInfo.oppoZhanli.ToString();//显示敌方战力


    }

	public int m_Zl;

	public int E_Zl;

	public int ResType;//1为我资源点 2为敌方资源点

	//public HuangYeResource my_HuangYeResource;//

	//public GameObject EnemyUI;

	public GameObject m_Res_instruction;

	public UILabel m_Res_instructionLabel;
	//public UILabel Res_Name;
	public int NumberOfEnemyPosition;// 第几组 1 2 3

	public void Init_HYUI(int skillzuheid, int enemyskillzuheid)
	{
//		Skill_ZUHE_Id = skillzuheid;
//
//
//		Enemy_Skill_ZUHE_Id = enemyskillzuheid;
//
//		HuangyeTemplate mHuangyeTemplate = HuangyeTemplate.getHuangyeTemplate_byid (my_HuangYeResource.fileId);//m描述
//
//		string mDescIdTemplate = DescIdTemplate.GetDescriptionById (mHuangyeTemplate.descId);
//
//		string mName = NameIdTemplate.GetName_By_NameId (mHuangyeTemplate.nameId);
//		//Res_Name.text = mName;
//		if(ResType != 1  )
//		{
//
//			Save_RefreshMiBao();
//
//			init_Hero_GuYongBing();
//
//			mZhanli.text = m_Zl.ToString();//显示我的战力
//
//			eZhanLi.text = E_Zl.ToString();//显示敌方战力
//		}

	}
    void Save_RefreshMiBao()
    {
        Debug.Log("shshshshshhshshshsh");

        Show_mMiBaoInfo();//只刷新自己的秘宝 其他信息不变
    }

    /// 显示君主和雇佣兵信息
    void init_Hero_GuYongBing()
    {
        //Clear Mercenary object list.
        foreach (IconSampleManager GYB in GuyongbingList)
        {
            Destroy(GYB.gameObject);
        }

        GuyongbingList.Clear();

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),

            IconSampleLoadCallBack);
        }
        else
        {
            WWW tempWww = null;

            IconSampleLoadCallBack(ref tempWww, null, IconSamplePrefab);
        }
    }

    private readonly Vector3 HeroPos = new Vector3(-364, 0, 0);

	private readonly Vector3 e_HeroPos = new Vector3(355,160, 0);

	private readonly Vector3 m_HeroPos = new Vector3(-360,-240, 0);

    private readonly Vector3 MercenaryPos = new Vector3(-240, 0, 0);

	private readonly Vector3 E_MercenaryPos = new Vector3(235, 160, 0);

	private readonly Vector3 m_MercenaryPos = new Vector3(-240, -240, 0);

    private const int IconBasicDepth = 15;

    public GameObject MyHeroAndSoldierParent;

    public GameObject EnemyHeroAndSoldierParent;

  
    private void IconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        //store loaded prefab.
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        //Show my hero info.

		if(BattleType == 1)
//		{
//			//Show my mercenary info.
//
//				GameObject MyHero = Instantiate(IconSamplePrefab) as GameObject;
//
//				MyHero.transform.parent = MyHeroAndSoldierParent.transform;
//
//				MyHero.transform.localPosition = m_HeroPos;
//
//				MyHero.SetActive(true);
//				IconSampleManager myHeroIconSample = MyHero.GetComponent<IconSampleManager>();
//
//				myHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);
//
//				HuangyePvpNpcTemplate mHYPVP = HuangyePvpNpcTemplate.getHuangyepvpNPCTemplate_by_Npcid(m_ResInfo.bossId);
//
//				myHeroIconSample.SetIconBasic(IconBasicDepth, "PlayerIcon"+CityGlobalData.m_king_model_Id, JunZhuData.Instance().m_junzhuInfo.level.ToString());
//				
//				//Show enemy hero info.
//				GameObject EnemyHero = Instantiate(IconSamplePrefab) as GameObject;
//
//				EnemyHero.transform.parent = EnemyHeroAndSoldierParent.transform;
//
//				EnemyHero.transform.localPosition = e_HeroPos;
//
//				EnemyHero.SetActive(true);
//				//[FIX] fulfil label text, fgsprite name.
//				IconSampleManager enemyHeroIconSample = EnemyHero.GetComponent<IconSampleManager>();
//
//				enemyHeroIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//				//enemyHeroIconSample.SetIconDecoSprite(null, "boss");
//
//
//				enemyHeroIconSample.SetIconBasic(IconBasicDepth, mHYPVP.icon.ToString() , mHYPVP.level.ToString());
//
//				for (int i = 0; i < my_HYResourseBattleResp.yongBingList.Count; i++)//显示我的雇佣兵
//				{
//					GameObject MyMercenary = Instantiate(IconSamplePrefab) as GameObject;
//
//					MyMercenary.transform.parent = MyHeroAndSoldierParent.transform;
//
//					MyMercenary.transform.localPosition = m_MercenaryPos + new Vector3(120 * i, 0, 0);
//
//					MyMercenary.SetActive(true);
//
//				HY_GuYongBingTempTemplate mGuYongBingTempTemplate = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(my_HYResourseBattleResp.yongBingList[i]);
//					int yongbing_Lv = mGuYongBingTempTemplate.level;
//
//					int yongbing_Sprite = mGuYongBingTempTemplate.icon;
//
//					int yongbing_Pinzhi = mGuYongBingTempTemplate.quality;
//					
//					// 品质和icon暂时未作处理
//					string leftTopSpriteName;
//					if (mGuYongBingTempTemplate.profession == 1)
//					{
//						leftTopSpriteName = "dun";//盾
//					}
//					else if (mGuYongBingTempTemplate.profession == 4)
//					{
//						leftTopSpriteName = "piccar";//车
//					}
//					else if (mGuYongBingTempTemplate.profession == 3)
//					{
//						leftTopSpriteName = "gong";//弓箭
//					}
//					else if (mGuYongBingTempTemplate.profession == 5)
//					{
//						leftTopSpriteName = "hours";//Horse////马
//					}
//					else if (mGuYongBingTempTemplate.profession == 2)
//					{
//						leftTopSpriteName = "gang";//Horse////枪
//					}
//					else
//					{
//						
//						leftTopSpriteName = null;
//					}
//					var rightButtomSpriteName = "";
//
//
//					IconSampleManager myMercenaryIconSample = MyMercenary.GetComponent<IconSampleManager>();
//
//					myMercenaryIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//
//					myMercenaryIconSample.SetIconBasic(IconBasicDepth, yongbing_Sprite.ToString(), yongbing_Lv.ToString());
//
//					myMercenaryIconSample.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//
//					GuyongbingList.Add(myMercenaryIconSample);
//
//				}
//				
//				for (int i = 0; i < m_ResInfo.yongBingId.Count; i++)//显示敌方的雇佣兵
//				{
//					GameObject EnemyMercenary = Instantiate(IconSamplePrefab) as GameObject;
//
//					EnemyMercenary.transform.parent = EnemyHeroAndSoldierParent.transform;
//
//
//					EnemyMercenary.transform.localPosition = E_MercenaryPos + new Vector3(-120 * i, 0, 0);
//
//					EnemyMercenary.SetActive(true);
//
//				HY_GuYongBingTempTemplate mGuYongBingTempTemplate = HY_GuYongBingTempTemplate.GetHY_GuYongBingTempTemplate_By_id(m_ResInfo.yongBingId[i]);
//					int yongbing_Lv = mGuYongBingTempTemplate.level;
//
//					int yongbing_Sprite = mGuYongBingTempTemplate.icon;
//
//					int yongbing_Pinzhi = mGuYongBingTempTemplate.quality;
//		            
//					string leftTopSpriteName;
//					if (mGuYongBingTempTemplate.profession == 1)
//					{
//						leftTopSpriteName = "dun";//盾
//					}
//					else if (mGuYongBingTempTemplate.profession == 4)
//					{
//						leftTopSpriteName = "piccar";//车
//					}
//					else if (mGuYongBingTempTemplate.profession == 3)
//					{
//						leftTopSpriteName = "gong";//弓箭
//					}
//					else if (mGuYongBingTempTemplate.profession == 5)
//					{
//						leftTopSpriteName = "hours";//Horse////马
//					}
//					else if (mGuYongBingTempTemplate.profession == 2)
//					{
//						leftTopSpriteName = "gang";//Horse////枪
//					}
//					else
//					{
//						
//						leftTopSpriteName = null;
//					}
//					var rightButtomSpriteName = "";
//
//					IconSampleManager enemyMercenaryIconSample = EnemyMercenary.GetComponent<IconSampleManager>();
//
//					enemyMercenaryIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
//
//					enemyMercenaryIconSample.SetIconBasic(IconBasicDepth, yongbing_Sprite.ToString(), yongbing_Lv.ToString());
//
//					enemyMercenaryIconSample.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
//
//					GuyongbingList.Add(enemyMercenaryIconSample);
//
//				}
//
//		}
		if(BattleType == 2)
		{
			//Show my mercenary info.


			GameObject MyHero = Instantiate(IconSamplePrefab) as GameObject;

			MyHero.transform.parent = MyHeroAndSoldierParent.transform;

			MyHero.transform.localPosition = m_HeroPos;

			MyHero.SetActive(true);
			IconSampleManager myHeroIconSample = MyHero.GetComponent<IconSampleManager>();

			myHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);

			//HuangyePvpNpcTemplate mHYPVP = HuangyePvpNpcTemplate.getHuangyepvpNPCTemplate_by_Npcid(m_ResInfo.bossId);
			int junZhuIconId = CityGlobalData.m_king_model_Id;

			myHeroIconSample.SetIconBasic(IconBasicDepth, "PlayerIcon" + junZhuIconId, JunZhuData.Instance().m_junzhuInfo.level.ToString());
			//myHeroIconSample.SetIconDecoSprite(null, "boss");
			//Show enemy hero info.
			GameObject EnemyHero = Instantiate(IconSamplePrefab) as GameObject;

			EnemyHero.transform.parent = EnemyHeroAndSoldierParent.transform;

			EnemyHero.transform.localPosition = e_HeroPos;

			EnemyHero.SetActive(true);

			IconSampleManager enemyHeroIconSample = EnemyHero.GetComponent<IconSampleManager>();

			enemyHeroIconSample.SetIconType(IconSampleManager.IconType.mainCityAtlas);
			//enemyHeroIconSample.SetIconDecoSprite(null, "boss");
			enemyHeroIconSample.SetIconBasic(IconBasicDepth, "PlayerIcon" + TiaoZhanInfo.oppoRoleId, TiaoZhanInfo.oppoLevel.ToString());


			for (int i = 0; i < TiaoZhanInfo.mySoldiers.Count; i++)//显示我的雇佣兵
			{
				GameObject MyMercenary = Instantiate(IconSamplePrefab) as GameObject;

				MyMercenary.transform.parent = MyHeroAndSoldierParent.transform;

				MyMercenary.transform.localPosition = m_MercenaryPos + new Vector3(120 * i, 0, 0);

				MyMercenary.SetActive(true);

				IconSampleManager myMercenaryIconSample = MyMercenary.GetComponent<IconSampleManager>();

				myMercenaryIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			
				GuYongBingTempTemplate mGYB = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(TiaoZhanInfo.mySoldiers[i].id);

				string leftTopSpriteName;
				if (mGYB.profession == 1)
				{
					leftTopSpriteName = "dun";//盾
				}
				else if (mGYB.profession == 4)
				{
					leftTopSpriteName = "piccar";//车
				}
				else if (mGYB.profession == 3)
				{
					leftTopSpriteName = "gong";//弓箭
				}
				else if (mGYB.profession == 5)
				{
					leftTopSpriteName = "hours";//Horse////马
				}
				else if (mGYB.profession == 2)
				{
					leftTopSpriteName = "gang";//Horse////枪
				}
				else
				{
					
					leftTopSpriteName = null;
				}
				var rightButtomSpriteName = "";

				int icon_id = mGYB.icon;

				myMercenaryIconSample.SetIconBasic(IconBasicDepth, icon_id.ToString(), mGYB.needLv.ToString());

				myMercenaryIconSample.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);

				GuyongbingList.Add(myMercenaryIconSample);
			}
			
			//Show enemy mercenary info.
			for (int i = 0; i < TiaoZhanInfo.oppoSoldiers.Count; i++)//显示敌方的雇佣兵
			{
				GameObject EnemyMercenary = Instantiate(IconSamplePrefab) as GameObject;

				EnemyMercenary.transform.parent = EnemyHeroAndSoldierParent.transform;

				EnemyMercenary.transform.localPosition = E_MercenaryPos + new Vector3(-120 * i, 0, 0);

				EnemyMercenary.SetActive(true);
				
				IconSampleManager enemyMercenaryIconSample = EnemyMercenary.GetComponent<IconSampleManager>();

				enemyMercenaryIconSample.SetIconType(IconSampleManager.IconType.pveHeroAtlas);

				GuYongBingTempTemplate mGYB = GuYongBingTempTemplate.GetGuYongBingTempTemplate_By_id(TiaoZhanInfo.oppoSoldiers[i].id);


				string leftTopSpriteName;
				if (mGYB.profession == 1)
				{
					leftTopSpriteName = "dun";//盾
				}
				else if (mGYB.profession == 4)
				{
					leftTopSpriteName = "piccar";//车
				}
				else if (mGYB.profession == 3)
				{
					leftTopSpriteName = "gong";//弓箭
				}
				else if (mGYB.profession == 5)
				{
					leftTopSpriteName = "hours";//Horse////马
				}
				else if (mGYB.profession == 2)
				{
					leftTopSpriteName = "gang";//Horse////枪
				}
				else
				{
					
					leftTopSpriteName = null;
				}
				var rightButtomSpriteName = "";

				enemyMercenaryIconSample.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);

				enemyMercenaryIconSample.SetIconBasic(IconBasicDepth, mGYB.icon.ToString(), mGYB.needLv.ToString());
				
				GuyongbingList.Add(enemyMercenaryIconSample);
			}

		}

		Show_mMiBaoInfo();

		ShoweMiBaoInfo();
    }
	public int BattleType;//战斗类型 1 为荒野 2 为百战 3 为pve
	 
	public MibaoInfoResp my_MiBaoInfo;

    void Show_mMiBaoInfo()//显示我的秘宝信息
    {

		my_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;

		int Pinzhi = 0;
		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
		{
			if(my_MiBaoInfo.mibaoGroup[i].zuheId == Skill_ZUHE_Id)
			{
				for(int j = 0 ; j < my_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j++)
				{
					if(my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0 && !my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
					{
						Pinzhi += 1;
					}
				}
			}
		}
		if(Pinzhi > 3 || Pinzhi < 2)
		{
			m_No_SkillColider.SetActive(true);

			m_Skill_Icon.spriteName = "";

			return;
		}
		if(Skill_ZUHE_Id < 1 || Skill_ZUHE_Id > 4)
		{
			Debug.Log("return l ");

			m_No_SkillColider.SetActive(true);
			
			m_Skill_Icon.spriteName = "";

			return;
		}

		m_No_SkillColider.SetActive(false);

		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi ( Skill_ZUHE_Id,Pinzhi );

		m_Skill_Icon.spriteName = mSkill.icon.ToString();

    }
	void ShoweMiBaoInfo()//显示敌对的秘宝信息(不是自己资源点时候现在该秘宝函数)
	{
		
		my_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
		
		if (Enemy_Skill_ZUHE_Id > 0&&Enemy_Skill_ZUHE_Id < 5)
		{
			if (Enemy_MiBao_Num >= 2)
			{
				MiBaoSkillTemp miBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (Enemy_Skill_ZUHE_Id,
				                                                                               Enemy_MiBao_Num);
				e_Skill_Icon.spriteName = miBaoSkillTemp.skill.ToString ();
			}

			else
			{
				e_Skill_Icon.spriteName = "";
				e_No_SkillColider.SetActive(true);
			}
		}
		else
		{
			e_Skill_Icon.spriteName = "";
			e_No_SkillColider.SetActive(true);
		}
	}

    void Show_mJunZhuIfo()//我的君主信息
    {
        //myHeroInfo.junZhuId = 1;
    }
    void Show_eJunZhuIfo()//敌方君主信息
    {

    }
    void LoadBack(ref WWW p_www, string p_path, Object p_object)
    {


        GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;

        mChoose_MiBao.SetActive(true);

        mChoose_MiBao.transform.parent = this.transform.parent;

        mChoose_MiBao.transform.localPosition = Vector3.zero;

        mChoose_MiBao.transform.localScale = Vector3.one;

		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();

		mChangeMiBaoSkill.RootName = this.gameObject.name;

		if(BattleType == 1)
		{
			mChangeMiBaoSkill.GetRootName ("HuangYe_TiaoZhanZhengRong(Clone)");
		
			mChangeMiBaoSkill.Init((int)CityGlobalData.MibaoSkillType.HY_ResSend, Skill_ZUHE_Id);
		}

		if(BattleType == 2)
		{
			mChangeMiBaoSkill.Init((int)CityGlobalData.MibaoSkillType.PvpSend, Skill_ZUHE_Id);
		}
    }
    public void ChangerMiBao()//更换秘宝
    {
		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			return;
		}
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadBack);

//		if (BattleType == 2)
//		{
//			if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
//			{
//				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
//
//				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[6]);
//			}
//		}
    }
    public void EnterFight()//进入战斗
    {
//		if (BattleType == 1)
//		{
//		    //进入荒野资源点战斗
//			HuangYePVPTemplate mHuangYePvPTemplate = HuangYePVPTemplate.getHuangYePVPTemplate_byid (my_HuangYeResource.fileId);
//			EnterBattleField.EnterBattleHYPvp (my_HuangYeResource.id, m_ResInfo.bossId, mHuangYePvPTemplate);
//		}
		if(BattleType == 2)
		{
			BaiZhanUnExpected.unExpected.TiaoZhanStateReq (null,2,TiaoZhanInfo);
		}

    }
    public void DestroyBaiZhanRoot()
    {
		Global.m_isOpenBaiZhan = false;
		GameObject baizhan = GameObject.Find ("BaiZhan");
		if(baizhan)
		{
			Destroy(baizhan);
		}
		GameObject HuangYe = GameObject.Find ("HuangYe_TiaoZhanZhengRong(Clone)");
		if(HuangYe)
		{
			Destroy(HuangYe);
		}
		Destroy(this.gameObject);
	
    }
	public void BackBtn()
	{
		Global.m_isOpenBaiZhan = false;
	
		Destroy(this.gameObject);
		
	}

    void OnDestroy()
    {

    }
}
