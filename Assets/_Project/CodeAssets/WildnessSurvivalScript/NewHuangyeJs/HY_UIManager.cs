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

	public UILabel Builds;

	public UILabel Hy_JinBi;

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
	public static HY_UIManager Instance ()
	{
		if (!HuangYeData)
		{
			HuangYeData = (HY_UIManager)GameObject.FindObjectOfType (typeof(HY_UIManager));
		}
		
		return HuangYeData;
	}
	void Awake()
	{ 
		
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

	void Update () {
	
	}
	public void init()
	{
//		Debug.Log("0000000000请求荒野信息 。。。。");

		M_UnionInfo = AllianceData.Instance.g_UnionInfo;//获取联盟的信息
		

		
		if(M_UnionInfo == null )
		{
//			Debug.Log("M_UnionInfo为空。。。。");
			
			return;
		}
		canOpenShop = true; 
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_HUANGYE);
		
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
//						Debug.Log("Huangye_resp.treasure  == null");
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

				Debug.Log("激活反回来了 = "+HuangyeFog_resp.id);
				if(HuangyeFog_resp.result == 0 ) // 开乌成功
				{
					AllianceData.Instance.RequestData();

					for(int i = 0; i < mHuangYe_Huangyetreasure_List.Count; i ++)
					{
						//Debug.Log("mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId = "+mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId);

						if(mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId == IsactiveID)
						{
							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.isActive = 2;

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
					Debug.Log("建设值不足了--- 开启失败");

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),IsOpenFailLoadBack);

					//Global.CreateBox(LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_1), null, LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_3), null, LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM), null, null, null);
				}
				return true;
				
			}	
			case ProtoIndexes.S_OPEN_TREASURE:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				OpenHuangYeTreasureResp mOpenHuangYeTreasureResp = new OpenHuangYeTreasureResp();
				
				t_qx.Deserialize(t_stream, mOpenHuangYeTreasureResp, mOpenHuangYeTreasureResp.GetType());
				
				Debug.Log("重置反回来了 = "+mOpenHuangYeTreasureResp.id);
				if(mOpenHuangYeTreasureResp.result == 0 ) // 重置成功
				{


					
					for(int i = 0; i < mHuangYe_Huangyetreasure_List.Count; i ++)
					{
						//Debug.Log("mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId = "+mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId);
						
						if(mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.fileId == IsactiveID)
						{
							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.isActive = 2;

							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.id = mOpenHuangYeTreasureResp.id;

							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.isOpen = 1;
							
							mHuangYe_Huangyetreasure_List[i].mHuangYeTreasure.jindu = 100;
							
							mHuangYe_Huangyetreasure_List[i].Init();

							HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (IsactiveID);

							M_UnionInfo.build -= mhuangye.openCost;

							ShowRemainTime();
						}
					}

					AllianceData.Instance.RequestData();
				}
				else
				{
					Debug.Log("建设值不足了--- 开启失败");
					
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
				
				Debug.Log("购买次数返回了0 wei chenggong = "+mHyBuyBattleTimesResp.isSuccess);

				Debug.Log("mHyBuyBattleTimesResp.buyCiShuInfo = "+mHyBuyBattleTimesResp.buyCiShuInfo);
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
			
			default: return false;
			}
			
		}
		return false;
	}

	public void ShowRemainTime()
	{
		Builds.text = M_UnionInfo.build.ToString();
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
	public void ShowHuangyeBi(int money)
	{
		m_Huangye_resp.hYMoney = money;
		Hy_JinBi.text = m_Huangye_resp.hYMoney.ToString ();
	}

	int curMaxlevel = 0;

	OpenHuangYeResp m_Huangye_resp;
	public void InitLevelsAndUI(OpenHuangYeResp mHuangye_resp)
	{
		m_Huangye_resp = mHuangye_resp;
		int MaxLevel = 1;
//		Debug.Log ("mHuangye_resp.treasure.Count = " + mHuangye_resp.treasure.Count);
//
//		Debug.Log ("Levelsnum = " + Levelsnum);

		int a = mHuangye_resp.treasure.Count % Levelsnum;
	
		ShowRemainTime ();

		ShowHuangyeBi (mHuangye_resp.hYMoney);

//		Debug.Log ("a = " + a);
		if(mHuangye_resp.treasure.Count % Levelsnum == 0)
		{
			MaxMap = (mHuangye_resp.treasure.Count / Levelsnum);//总的章节数

		}
		else
		{
			MaxMap = (mHuangye_resp.treasure.Count / Levelsnum)+1;//总的章节数
		}




		int count = 0;

		for(int i = 0; i < mHuangye_resp.treasure.Count; i++)
		{

			if( mHuangye_resp.treasure[i].isActive == 2) //后面换成isactive
			{
			    count += 1;
				MaxLevel  = i+1;
				if( mHuangye_resp.treasure[i+1].isActive == 1) //后面换成isactive
				{
					MaxLevel  = i+2;
				}
			}
		}
//		Debug.Log ("MaxLevel = " + MaxLevel);
		if(mHuangye_resp.treasure.Count == count)
		{
			CurMap = MaxMap;

		}
		else
		{
			if(MaxLevel < Levelsnum)
			{
				CurMap = 1;
			}
			else
			{
				if((MaxLevel%Levelsnum) == 0)
				{
					CurMap = MaxLevel/Levelsnum;
				}
				else
				{
					CurMap = MaxLevel/Levelsnum + 1;
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
		if(CurMap*Levelsnum > m_Huangye_resp.treasure.Count)
		{
			curMaxlevel = m_Huangye_resp.treasure.Count;
		}
		else
		{
			curMaxlevel = CurMap*Levelsnum;
		}
		Debug.Log ("CurMap2 = " +CurMap);
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

			HuangYePveTemplate pvetemp = HuangYePveTemplate.getHuangYePveTemplatee_byid(m_OpenHuangYeResp.treasure[i].fileId);

			float x =  (float)pvetemp.positionX;

			float y =  (float)pvetemp.positionY+30;

			mHyTreasureTemp.transform.localPosition = new Vector3(x,y,0);
			
			mHyTreasureTemp.transform.localScale = Vector3.one;
			
			HY_LevelTepm mHY_LevelTepm = mHyTreasureTemp.GetComponent<HY_LevelTepm>();
			
			mHY_LevelTepm.mHuangYeTreasure = m_OpenHuangYeResp.treasure[i];

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
	
		for(int i = (CurMap -1)*Levelsnum; i < (CurMap)*Levelsnum; i++)
		{
			if(m_OpenHuangYeResp.treasure[i].isActive == 0||m_OpenHuangYeResp.treasure[i].isActive ==1)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
				return;
			}
		}
		if((CurMap+1) <= MaxMap)
		{
			CurMap +=1 ;

			LordMap(vectRight,1,CurMap +100); //map name call 101 102...CurMap+1+100

		}
		else
		{
			Debug.Log("暂时没有新的关卡可开启");
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
			Debug.Log("暂时没有新的关卡可开启");
		}
		
	}

	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "需要用关所有关卡才能进入下一张荒野地图！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null,null);
	}

	List<string> InstrctionList = new List<string>();

	public void ShowPlayerInstrction()
	{
		InstrctionList.Clear ();

		string st1 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_11);
		string st2 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_12);
		string st3 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_13);
		string st4 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_14);
		string st5 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_15);
		string st6 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_16);
		string st7 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_17);
		string st8 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_18);
		string st9 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANG_YE_TIPS_19);

		InstrctionList.Add (st1);
		InstrctionList.Add (st2);
		InstrctionList.Add (st3);
		InstrctionList.Add (st4);
		InstrctionList.Add (st5);
		InstrctionList.Add (st6);
		InstrctionList.Add (st7);
		InstrctionList.Add (st8);
		InstrctionList.Add (st9);
	
		GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.HUANGYE,InstrctionList);
	}

	List<DuiHuanInfo> tempHuangyeList = new List<DuiHuanInfo>();

	/// <summary>
	/// 商店是否可开启
	/// </summary>
	private bool canOpenShop = true; 
	public bool CanOpenShop
	{
		set{canOpenShop = value;}
	}

	public void EnterHY_Shop()
	{
		if (canOpenShop)
		{
			canOpenShop = false;
			string mtitleStr = "荒野货栈";
			GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.HUANGYE,GeneralControl.StoreReqType.FREE,mtitleStr);
		}
	}

	public void AddTimes()
	{
		if(m_OpenHuangYeResp.buyCiShuInfo == 1)
		{
			if(m_OpenHuangYeResp.leftBuyCiShu <= 0)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
			}
			else{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenBuyUILoadBack);
			}
		}
		if(m_OpenHuangYeResp.buyCiShuInfo == 2)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpVipLoadBack);
		}
		if(m_OpenHuangYeResp.buyCiShuInfo == 3)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyFailLoad);
		}
	}


	void UpVipLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "Vip等级不足，提升Vip等级可增加购买次数！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void buyFailLoad(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "对不起，您今日购买次数已用尽！改日再来吧";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void OpenBuyUILoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "购买次数";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "您是否要花费"+m_OpenHuangYeResp.buyNextMoney.ToString()+"元宝购买"+m_OpenHuangYeResp.buyNextCiShu.ToString()+"次挑战次数？\r\n 今日还可购买"+m_OpenHuangYeResp.leftBuyCiShu.ToString()+"次";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,SureBuy,null,null);
	}
	void SureBuy(int i)
	{
		int mMoney  = JunZhuData.Instance().m_junzhuInfo.yuanBao; 


		if(i == 2)
		{
			if(mMoney < m_OpenHuangYeResp.buyNextMoney)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),NoMoney);
			}
			else
			{
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.HY_BUY_BATTLE_TIMES_REQ);
			}
		}
	}
	void NoMoney(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "元宝不足";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "您的元宝不足了！是否充值？";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,SureChongZhi,null,null);
	}
	void SureChongZhi(int i)
	{
		if(i == 2)
		{
			Debug.Log("跳转到充值！");
			
			MainCityUI.ClearObjectList();
			TopUpLoadManagerment.m_instance.LoadPrefab(true);
			QXTanBaoData.Instance().CheckFreeTanBao();
		}
	}
	
	public void CloseUI()
	{
		CityGlobalData.CurrentHY_Capter = 0;
		MainCityUI.TryRemoveFromObjectList (this.gameObject);
		Global.m_isOpenHuangYe = false;
		Destroy (this.gameObject);
	}
}
