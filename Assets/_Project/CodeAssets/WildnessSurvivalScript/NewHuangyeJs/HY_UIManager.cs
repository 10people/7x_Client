using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class HY_UIManager : MonoBehaviour,SocketProcessor {

	public static HY_UIManager  HuangYeData;

	public AllianceHaveResp M_UnionInfo;

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

//	public UILabel Builds;

	//public UILabel Hy_JinBi;

	public UILabel Hy_ReMainTimes; //剩余挑战次数

	public OpenHuangYeResp m_OpenHuangYeResp;

	public List<HY_LevelTepm> mHuangYe_Huangyetreasure_List = new List<HY_LevelTepm> ();

	public GameObject HyTreasureTemp;

	public GameObject MapTemp;

	public int Levelsnum  = 7; //每一章的关卡个数设置为7个  已经问过策划20150908

	public int CurMap  = 1;
	public int MaxMap  = 0;

	Vector3 vect0 = new Vector3(0,0,0);
	Vector3 vectRight = new Vector3(960,0,0);
	Vector3 vectLeft = new Vector3(-960,0,0);


	string CancleBtn;
	
	string confirmStr;

	public GameObject Leftbtn;

	public GameObject RightBtn;

	public int IsactiveID ;

	public List<int > HY_IdList = new List<int> ();

	public Transform mTrans;
	public static HY_UIManager Instance()
	{
		if (!HuangYeData)
		{
			HuangYeData = (HY_UIManager)GameObject.FindObjectOfType (typeof(HY_UIManager));
		}
		
		return HuangYeData;
	}
	void Awake()
	{ 
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "荒野求生", 0, 0);
		SocketTool.RegisterMessageProcessor(this);
		
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
		
	}
	void Start () {
	
		IsactiveID = 0;

		CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}

	//public GameObject NeedScoleUI;
	public void init()
	{
//		Debug.Log("0000000000请求荒野信息 。。。。");
//		UIRoot root = GameObject.FindObjectOfType<UIRoot>();
//		if (root != null) {
//			float s = (float)root.activeHeight / Screen.height;
//			int height = Mathf.CeilToInt (Screen.height * s);
//			int width = Mathf.CeilToInt (Screen.width * s);
//			float v1 = (float)height / (float)640;
//			float v2 = (float)width / (float)960;
//			NeedScoleUI.transform.localScale = new Vector3(v2,v1,1);
//			//Debug.Log ("NeedScoleUI.localScale = " + NeedScoleUI.transform.localScale);
//		} else {
//			//Debug.Log ("root == null");
//		}
		M_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
		
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		
		if(M_UnionInfo == null )
		{
			//Debug.Log("M_UnionInfo为空。。。。");
			
			return;
		}
//		canOpenShop = true; 
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_HUANGYE);

		HY_IdList = HuangYePveTemplate.getHuangYePveTemplateList ();
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_OPEN_HUANGYE:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				OpenHuangYeResp Huangye_resp = new OpenHuangYeResp();
				
				t_qx.Deserialize(t_stream, Huangye_resp, Huangye_resp.GetType());
				
				if(Huangye_resp != null)
				{
					m_OpenHuangYeResp = Huangye_resp;
					if(Huangye_resp.treasure == null)
					{
						Debug.Log("Huangye_resp.treasure  == null");
					}
					else
					{
//						Debug.Log("Huangye_resp.treasure.Count = "+Huangye_resp.treasure.Count);
						
						InitLevelsAndUI(Huangye_resp);
					}

				}
				
				return true;
			}
			case ProtoIndexes.ACTIVE_TREASURE_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActiveTreasureResp HuangyeFog_resp = new ActiveTreasureResp();
				
				t_qx.Deserialize(t_stream, HuangyeFog_resp, HuangyeFog_resp.GetType());

				//Debug.Log("激活反回来了 = "+HuangyeFog_resp.id);
				if(HuangyeFog_resp.result == 0 ) // 开乌成功
				{
					AllianceData.Instance.RequestData();

					for(int i = 0; i < mHuangYe_Huangyetreasure_List.Count; i ++)
					{
						//Debug.Log("mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId = "+mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId);

						if(mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.guanQiaId == IsactiveID)
						{
//							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.isActive = 2;

							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.id = HuangyeFog_resp.id;

							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.isOpen = 1;

							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.jindu = 100;

							mHuangYe_Huangyetreasure_List[i].Init();

							HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (IsactiveID);
							
							M_UnionInfo.build -= mhuangye.openCost;
							
							ShowRemainTime();
						}
					}
				}
				else
				{
					//Debug.Log("建设值不足了--- 开启失败");

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),IsOpenFailLoadBack);

					//Global.CreateBox(LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_1), null, LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_3), null, LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM), null, null, null);
				}
				return true;
				
			}	
	
			case ProtoIndexes.HY_BUY_BATTLE_TIMES_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				HyBuyBattleTimesResp mHyBuyBattleTimesResp = new HyBuyBattleTimesResp();
				
				t_qx.Deserialize(t_stream, mHyBuyBattleTimesResp, mHyBuyBattleTimesResp.GetType());
				
				//Debug.Log("购买次数返回了0 wei chenggong = "+mHyBuyBattleTimesResp.isSuccess);

				//Debug.Log("mHyBuyBattleTimesResp.buyCiShuInfo = "+mHyBuyBattleTimesResp.buyCiShuInfo);
				if(mHyBuyBattleTimesResp.isSuccess == 0)
				{
					if( mHyBuyBattleTimesResp.buyCiShuInfo == 1)
					{
						m_OpenHuangYeResp.buyNextMoney = mHyBuyBattleTimesResp.buyNextMoney;
						
						m_OpenHuangYeResp.buyNextCiShu = mHyBuyBattleTimesResp.buyNextCiShu;
						
						m_OpenHuangYeResp.leftBuyCiShu = mHyBuyBattleTimesResp.leftBuyCiShu;

						if(CityGlobalData.IsOPenHyLeveUI == true)
						{
							HYRetearceEnemy.Instance().M_Treas_info.buyNextMoney = mHyBuyBattleTimesResp.buyNextMoney;
							
							HYRetearceEnemy.Instance().M_Treas_info.buyNextCiShu = mHyBuyBattleTimesResp.buyNextCiShu;
							
							HYRetearceEnemy.Instance().M_Treas_info.leftBuyCiShu = mHyBuyBattleTimesResp.leftBuyCiShu;

						}
					}
					if(CityGlobalData.IsOPenHyLeveUI == true)
					{
						
						HYRetearceEnemy.Instance().M_Treas_info.timesOfDay += 1;
						
						HYRetearceEnemy.Instance().M_Treas_info.buyCiShuInfo = mHyBuyBattleTimesResp.buyCiShuInfo;
						
						HYRetearceEnemy.Instance().ShowRemainTime();
						
					}
					m_OpenHuangYeResp.remianTimes += 1;
					m_OpenHuangYeResp.buyCiShuInfo = mHyBuyBattleTimesResp.buyCiShuInfo;
					ShowRemainTime();
				}

				return true;
			}
			case ProtoIndexes.MAX_DAMAGE_RANK_RESP :
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MaxDamageRankResp mMaxDamageRankResp = new MaxDamageRankResp();
				
				t_qx.Deserialize(t_stream, mMaxDamageRankResp, mMaxDamageRankResp.GetType());
				
				Debug.Log("_____排行榜");
				InitRankUI(mMaxDamageRankResp);
				return true;
			}
			default: return false;
			}
			
		}
		return false;
	}
	public void ShowDamageRank()
	{
		// 显示伤害排名
		MaxDamageRankReq mMaxDamageRankReq = new MaxDamageRankReq ();
		
		MemoryStream HYTreasureBattleStream = new MemoryStream ();
		
		QiXiongSerializer HYTreasureBattleper = new QiXiongSerializer ();
		
		//Debug.Log ("mHuangYeTreasure.id = "+mHuangYeTreasure.id);
		
		mMaxDamageRankReq.id = m_Huangye_resp.treasure.guanQiaId;
		
		HYTreasureBattleper.Serialize (HYTreasureBattleStream,mMaxDamageRankReq);
		
		byte[] t_protof;
		
		t_protof = HYTreasureBattleStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.MAX_DAMAGE_RANK_REQ, 
			ref t_protof,
			true,
			ProtoIndexes.MAX_DAMAGE_RANK_RESP );
	}
	private GameObject RankUI;
	MaxDamageRankResp m_MaxDamageRank;
	
	void InitRankUI(MaxDamageRankResp m_MaxDamageRankResp)
	{
		if(RankUI == null)
		{
			m_MaxDamageRank = m_MaxDamageRankResp;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.DAMAGERANK), LoadRankUIBack); // 还没添加路劲，
		}
	}
	void LoadRankUIBack(ref WWW p_www, string p_path, Object p_object)
	{
		RankUI = Instantiate (p_object) as GameObject;
		
		//RankUI.SetActive (true);
	//	RankUI.transform.parent = mTrans;
		
		RankUI.transform.localPosition = new Vector3 (100,100,0);
		
		RankUI.transform.localScale = Vector3.one;
		
		My_DamageRank mMy_DamageRank = RankUI.GetComponent<My_DamageRank>();
		
		mMy_DamageRank.mRankList = m_MaxDamageRank;

		mMy_DamageRank.m_levelid = m_Huangye_resp.treasure.guanQiaId;

		mMy_DamageRank.Init ();
		
		MainCityUI.TryAddToObjectList (RankUI,false);
	}
	public void ShowRemainTime()
	{
//		Builds.text = M_UnionInfo.build.ToString();
		if(m_Huangye_resp.remianTimes <= 0)
		{
			int Hyid = 300200;
			PushAndNotificationHelper.SetRedSpotNotification(Hyid,false);
		}
		Hy_ReMainTimes.text = m_Huangye_resp.remianTimes.ToString () + "/" + m_Huangye_resp.allTimes.ToString ();
	}

	void IsOpenFailLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "激活失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "联盟建设值不足或者权限不足，开启失败！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	//显示荒野币
//	public void ShowHuangyeBi(int money)
//	{
//		m_Huangye_resp.hYMoney = money;
//		Hy_JinBi.text = m_Huangye_resp.hYMoney.ToString ();
//		AllianceData.Instance.Hy_Bi = m_Huangye_resp.hYMoney;
//	}

	int curMaxlevel = 0;

	OpenHuangYeResp m_Huangye_resp;
	public void InitLevelsAndUI(OpenHuangYeResp mHuangye_resp)
	{
		m_Huangye_resp = mHuangye_resp;
		int MaxLevel = 1;
//		Debug.Log ("mHuangye_resp.treasure.Count = " + mHuangye_resp.treasure.Count);
//
//		Debug.Log ("Levelsnum = " + Levelsnum);

		int a = HY_IdList.Count % Levelsnum;
	
		ShowRemainTime ();

//		ShowHuangyeBi (mHuangye_resp.hYMoney);

//		Debug.Log ("a = " + a);
		if(a % Levelsnum == 0)
		{
			MaxMap = (a / Levelsnum);//总的章节数

		}
		else
		{
			MaxMap = (a / Levelsnum)+1;//总的章节数
		}
		int count = 0; // 激活个数

		for(int i = 0; i < HY_IdList.Count; i++)
		{
			if( mHuangye_resp.treasure.guanQiaId == HY_IdList[i])
			{
			    count = 1+i;
			}
		}
//		Debug.Log ("MaxLevel = " + MaxLevel);
		if(HY_IdList.Count == count)
		{
			CurMap = MaxMap;

		}
		else
		{
			if(count < Levelsnum)
			{
				CurMap = 1;
			}
			else
			{
				if((count%Levelsnum) == 0)
				{
					CurMap = count/Levelsnum;
				}
				else
				{
					CurMap = count/Levelsnum + 1;
				}
				
			}
		}
//		Debug.Log ("CurMap1 = "+CurMap);

		if(CityGlobalData.CurrentHY_Capter > 0)
		{
			CurMap = CityGlobalData.CurrentHY_Capter;
		}
		LordMap(vect0,0,CurMap+100);


	}
	
	public GameObject mMap1;

	public GameObject mMap2;

	public void LordMap(Vector3 StartPos, int Dirc, int curr_map)
	{
		//Dirc 方向 -1 左边 0 中间 1 右边
		GameObject mmap = Instantiate(MapTemp)as GameObject;
		
		mmap.SetActive(true);
		
		mmap.transform.parent = MapTemp.transform.parent;
		
		mmap.transform.localPosition = StartPos;
		
		mmap.transform.localScale = Vector3.one;

		InitMap minitMap = mmap.GetComponent<InitMap>();
		
		minitMap.map_num = curr_map;
		
		minitMap.init ();

		foreach(HY_LevelTepm mtemp in mHuangYe_Huangyetreasure_List)
		{
			Destroy(mtemp.gameObject);
		}

		mHuangYe_Huangyetreasure_List.Clear ();

		if(CurMap*Levelsnum > HY_IdList.Count)
		{
			curMaxlevel =HY_IdList.Count;
		}
		else
		{
			curMaxlevel = CurMap*Levelsnum;
		}

//		Debug.Log ("CurMap2 = " +CurMap);

		if(CurMap <=1 )
		{
			Leftbtn.SetActive(false);
			RightBtn.SetActive(true);
		}
		else if(CurMap>= MaxMap)
		{
		   RightBtn.SetActive(false);
			Leftbtn.SetActive(true);
			
		}
		else
		{
			RightBtn.SetActive(true);

			Leftbtn.SetActive(true);
		}
		for(int i = (CurMap -1)*Levelsnum; i < curMaxlevel; i++)
		{
			GameObject mHyTreasureTemp = Instantiate(HyTreasureTemp)as GameObject;
			
			mHyTreasureTemp.SetActive(true);
			
			mHyTreasureTemp.transform.parent = mmap.transform;

			HuangYePveTemplate pvetemp = HuangYePveTemplate.getHuangYePveTemplatee_byid(HY_IdList[i]);

			float x =  (float)pvetemp.positionX;

			float y =  (float)pvetemp.positionY+30;

			mHyTreasureTemp.transform.localPosition = new Vector3(x,y,0);
			
			mHyTreasureTemp.transform.localScale = Vector3.one;
			
			HY_LevelTepm mHY_LevelTepm = mHyTreasureTemp.GetComponent<HY_LevelTepm>();

			if(HY_IdList[i] == m_OpenHuangYeResp.treasure.guanQiaId)
			{
				mHY_LevelTepm.mState = 1;
				mHY_LevelTepm.mHuangYeTreasure = m_OpenHuangYeResp.treasure;

			}else if(HY_IdList[i] < m_OpenHuangYeResp.treasure.guanQiaId)
			{
				mHY_LevelTepm.mState = 0;
			}
			else
			{
				mHY_LevelTepm.mState = 2;
			}
			mHY_LevelTepm.m_GuanQiaID = HY_IdList[i];
//			Debug.Log("m_OpenHuangYeResp.treasure[i],id = "+m_OpenHuangYeResp.treasure[i].id);
			mHY_LevelTepm.Init();
			
			mHuangYe_Huangyetreasure_List.Add(mHY_LevelTepm);
		}

		if(Dirc == 0)
		{
			mMap1 = mmap;
		}
		else
		{
			mMap2 = mmap;

			if(Dirc == 1)
			{
				TweenPosition.Begin(mMap1,0.4f,vectLeft);
				
				TweenPosition.Begin(mMap2,0.4f,vect0);
				
				Destroy(mMap1,0.5f);
				
				mMap1 = mMap2;
			}
			else
			{
				TweenPosition.Begin(mMap1,0.4f,vectRight);
				
				TweenPosition.Begin(mMap2,0.4f,vect0);
				
				Destroy(mMap1,0.5f);
				
				mMap1 = mMap2;
			}
		}
		CityGlobalData.CurrentHY_Capter = curr_map-100;
//		Debug.Log ("CityGlobalData.CurrentHY_Capter = "+CityGlobalData.CurrentHY_Capter);
	}
	public void RightMoveMap()
	{
	
		if((CurMap)*Levelsnum == HY_IdList.Count )
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			return;
		}
		for(int i = (CurMap -1)*Levelsnum; i < (CurMap)*Levelsnum; i++)
		{
			int MaxNuber = HY_IdList.Count;
			if(i < HY_IdList.Count -1)
			{
				if(m_OpenHuangYeResp.treasure.guanQiaId == HY_IdList[i])
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
					return;
				}
			}

		}
		if((CurMap+1) <= MaxMap)
		{
			CurMap +=1 ;

			LordMap(vectRight,1,CurMap +100); //map name call 101 102...CurMap+1+100

		}
		else
		{
			//Debug.Log("暂时没有新的关卡可开启");
		}

	}
	public void LiftMoveMap()
	{
		if(CurMap > 1)
		{
			CurMap -=1 ;

			LordMap(vectLeft,-1,CurMap +100);
		}
		else
		{
			//Debug.Log("暂时没有新的关卡可开启");
		}
		
	}

	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "需要通关所有关卡才能进入下一张荒野地图！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null,null);
	}

	List<string> InstrctionList = new List<string>();

	public void ShowPlayerInstrction()
	{
		InstrctionList.Clear ();

		string st1 = LanguageTemplate.GetText (LanguageTemplate.Text.HY_HELP_DESC);

		string [] s = st1.Split ('#');

		for(int i = 0 ; i<s.Length; i ++)
		{
			InstrctionList.Add (s[i]);
		}
		GeneralControl.Instance.LoadRulesPrefab (InstrctionList);
	}

	List<DuiHuanInfo> tempHuangyeList = new List<DuiHuanInfo>();

	/// <summary>
	/// 商店是否可开启
	/// </summary>
//	private bool canOpenShop = true; 
//	public bool CanOpenShop
//	{
//		set{canOpenShop = value;}
//	}

//	public void EnterHY_Shop()
//	{
//	
////			GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.HUANGYE,GeneralControl.StoreReqType.FREE);
//	   ShopData.Instance.OpenShop (ShopData.ShopType.HUANGYE);
//
//	}

//	public void AddTimes()
//	{
//		if(m_OpenHuangYeResp.buyCiShuInfo == 1)
//		{
//			if(m_OpenHuangYeResp.leftBuyCiShu <= 0)
//			{
//				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
//			}
//			else{
//				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenBuyUILoadBack);
//			}
//		}
//		if(m_OpenHuangYeResp.buyCiShuInfo == 2)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpVipLoadBack);
//		}
//		if(m_OpenHuangYeResp.buyCiShuInfo == 3)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
//		}
//	}

//
//	void UpVipLoadBack(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		
//		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
//		int vip = 3;
//		if (vip <= JunZhuData.Instance().m_junzhuInfo.vipLv) {
//			
//			vip = 7;
//		} 
//		string str = "V特权等级不足，V特权等级提升到"+(vip).ToString()+"级即可购买挑战次数。参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
//
//		//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
//		
//		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
//	}

//	void buyFailLoad(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		
//		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
//		int vip = 3;
//
//		if (vip <= JunZhuData.Instance().m_junzhuInfo.vipLv) {
//
//			vip = 7;
//		} 
//	
//		string str = "V特权等级不足，V特权等级提升到"+(vip).ToString()+"级即可购买挑战次数。参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
//		if (7 <= JunZhuData.Instance().m_junzhuInfo.vipLv) {
//			
//			str = "今日的购买次数已经用完了，请明日再来吧。";
//		} 
//		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
//	}

//	void OpenBuyUILoadBack(ref WWW p_www,string p_path, Object p_object)
//	{
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//		
//		string titleStr = "购买次数";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
//		
//		string str = "您是否要花费"+m_OpenHuangYeResp.buyNextMoney.ToString()+"元宝购买"+m_OpenHuangYeResp.buyNextCiShu.ToString()+"次挑战次数？\r\n 今日还可购买"+m_OpenHuangYeResp.leftBuyCiShu.ToString()+"次。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
//		
//		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,SureBuy,null,null);
//	}
//	void SureBuy(int i)
//	{
//		int mMoney  = JunZhuData.Instance().m_junzhuInfo.yuanBao; 
//
//
//		if(i == 2)
//		{
//			if(mMoney < m_OpenHuangYeResp.buyNextMoney)
//			{
//                EquipSuoData.TopUpLayerTip();
//                
//			}
//			else
//			{
//				SocketTool.Instance().SendSocketMessage(ProtoIndexes.HY_BUY_BATTLE_TIMES_REQ);
//			}
//		}
//	}
	 
	
	public void CloseUI()
	{
		CityGlobalData.CurrentHY_Capter = 0;
		GameObject mLianmeng = GameObject.Find ("New_My_Union(Clone)");
		if(mLianmeng == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_HAVE_ROOT),
			                        AllianceHaveLoadCallback);
		}
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Global.m_isOpenHuangYe = false;
		Destroy (this.gameObject);
	}
	public void AllianceHaveLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObject = Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObject);
		
	}
	public GameObject NeedCloseObg;

	public void ShowOrClose()
	{
		//Debug.Log ("NeedCloseObg = " +NeedCloseObg);
		if(NeedCloseObg == null)
		{
			return;
		}
		if(NeedCloseObg.activeInHierarchy)
		{
			NeedCloseObg.SetActive(false);
		}
		else
		{
			NeedCloseObg.SetActive(true);
		}
	}
}
