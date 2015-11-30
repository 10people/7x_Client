using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TanBaoManager : MonoBehaviour,SocketProcessor {

	public static TanBaoManager tbManager;
//
//    public ScaleEffectController m_ScaleEffectController;
//
//	private ExploreInfoResp m_tanBapResp;//探宝信息返回
//	private ExploreAwardsInfo tanBaoAwardResp;//奖励返回
//	private ExploreResp failResp;//失败类型返回
//
//	private List<ExploreMineInfo> tanBaoInfoList = new List<ExploreMineInfo>();
//
//	public GameObject kdObj; //矿洞
//	public GameObject kjObj; //矿井
//	public GameObject kmObj; //矿脉
//	public GameObject allianceKjObj;//联盟矿井
//	public GameObject allianceKmObj;//联盟矿脉
//	
//	public GameObject moneyInfoObj;//显示元宝贡献值等信息的obj
//
//	public GameObject turnBtnObj;
//
//	public GameObject singleRewardObj;//矿洞或矿井奖励页面
//	public GameObject multipRewardObj;//矿脉奖励页面
//
//	public GameObject flyPoint;
//	[HideInInspector]public List<GameObject> flyPointList = new List<GameObject>();
//
//	public GameObject publicZheZhao;
//
//	public GameObject tbObj;//camera 子物体
//
//	private string titleStr;
//	private string str;
//	private string confirmStr;
//	private string cancelStr;
//
//	/// <summary>
//	/// 探宝按钮同事点击判断
//	/// </summary>
//	private bool isClick = false;
//	public bool IsClick
//	{
//		set{isClick = value;}
//		get{return isClick;}
//	}
//
//	private bool isOpenChongZhi = false;//是否打开充值
//
	void Awake ()
	{
		tbManager = this;

		SocketTool.RegisterMessageProcessor (this);
	}
//
//	void Start ()
//	{
//		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
//		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
//
//		CreateFlyPointList ();
//
//		QXTanBaoData.Instance ().TBInfoReq ();
//
//		if (FreshGuide.Instance ().IsActive (100040) && TaskData.Instance.m_TaskInfoDic [100040].progress >= 0 && !FirstTanBao.Instance.GetTanBaoState2 ()) {
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic [100040];
//			UIYindao.m_UIYindao.setOpenYindao (tempTaskData.m_listYindaoShuju [2]);
//			Debug.Log ("探宝引导:" + tempTaskData.m_iCurIndex);
//		}
//		else
//		{
//			CityGlobalData.m_isRightGuide = true;
//		}
//
////		foreach (ExploreMineInfo tempMineInfo in QXTanBaoData.Instance ().tanBaoResp.mineRegionList)
////		{
////			switch (tempMineInfo.type)
////			{
////			case 0:
////
////				tempMineInfo.remainingTime = QXTanBaoData.Instance ().tb1CdTime;
////
////				break;
////
////			case 1:
////
////				tempMineInfo.remainingTime = QXTanBaoData.Instance ().tb2CdTime;
////
////				break;
////
////			default:break;
////			}
////		}
////		GetTanBaoInfo (QXTanBaoData.Instance ().tanBaoResp);
//	}
//
//	//获得探宝返回信息
//	public void GetTanBaoInfo (ExploreInfoResp tempInfo)
//	{
//		m_tanBapResp = tempInfo;
//		tanBaoInfoList = tempInfo.mineRegionList;
//
//		MoneyManager money = moneyInfoObj.GetComponent<MoneyManager> ();
//		money.GetMoneyInfo (tempInfo);
//
//		foreach (ExploreMineInfo tanBaoInfo in tempInfo.mineRegionList)
//		{
//			InItTanBaoInfo (tanBaoInfo);
//		}
//
//		TurnBtns turnBtn = turnBtnObj.GetComponent<TurnBtns> ();
//		turnBtn.GetTanBaoInfo (tempInfo);
//	}
//
//	//初始化探宝信息
//	void InItTanBaoInfo (ExploreMineInfo tempMineInfo)
//	{
//		switch (tempMineInfo.type)
//		{
//		case 0:
//			
//			KuangDongInfo kd = kdObj.GetComponent<KuangDongInfo> ();
//			kd.GetKdInfo (tempMineInfo);
//			
//			break;
//			
//		case 1:
//			
//			KuangJingInfo kj = kjObj.GetComponent<KuangJingInfo> ();
//			kj.GetKjInfo (tempMineInfo);
//			
//			break;
//			
//		case 10:
//			
//			KuangMaiInfo km = kmObj.GetComponent<KuangMaiInfo> ();
//			km.GetKmInfo (tempMineInfo);
//			
//			break;
//			
//		case 11:
//			
//			AllianceKuangJingInfo allianceKj = allianceKjObj.GetComponent<AllianceKuangJingInfo> ();
//			allianceKj.GetAllianceKjInfo (tempMineInfo);
//			
//			break;
//			
//		case 12:
//			
//			AllianceKuangMaiInfo allianceKm = allianceKmObj.GetComponent<AllianceKuangMaiInfo> ();
//			allianceKm.GetAllianceKmInfo (tempMineInfo);
//			
//			break;
//		}
//	}
//
//	//初始化MoneyInfo
//	void SendMoneyInfo (int type,ExploreInfoResp tempInfo)
//	{
//		MoneyManager money = moneyInfoObj.GetComponent<MoneyManager> ();
//		money.RefreshMoneyInfo (type,tempInfo);
//	}
//
	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null) {
			
			switch (p_message.m_protocol_index) {
				
			case ProtoIndexes.EXPLORE_AWARDS_INFO://领取奖励返回
			{   
////				Debug.Log ("tbBuyRes:" + ProtoIndexes.EXPLORE_AWARDS_INFO);
//				
//				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				ExploreAwardsInfo tempEAInfo = new ExploreAwardsInfo();
//				
//				t_qx.Deserialize(t_stream, tempEAInfo, tempEAInfo.GetType());
//				
//				if (tempEAInfo != null) 
//				{	
//					isClick = false;
//
////					Debug.Log ("IsClick:" + isClick);
////					Debug.Log ("奖励item数:" + tempEAInfo.awardsList.Count);
////					foreach (Award a in tempEAInfo.awardsList)
////					{
////						Debug.Log ("物品id:" + a.itemId);
////						Debug.Log ("是否必得:" + a.isQuality);
////						Debug.Log ("物品数量:" + a.itemNumber);
////						Debug.Log ("物品类型:" + a.itemType);
////						Debug.Log ("物品星级:" + a.itemStar);
////						Debug.Log ("碎片id:" + a.pieceId);
////						Debug.Log ("碎片类型:" + a.pieceType);
////						Debug.Log ("碎片数量:" + a.pieceNumber);
////					}
//
//					MiBaoGlobleData.SendMiBaoIfoMessage ();
//
//					foreach (ExploreMineInfo tempMineInfo in m_tanBapResp.mineRegionList)
//					{
//						if (tempMineInfo.type == tempEAInfo.type)
//						{
//							if (tempEAInfo.type == 0)
//							{
//								if (tempMineInfo.isCanGet)
//								{
//									tempMineInfo.isCanGet = false;
//									tempMineInfo.gotTimes += 1;
//									tempMineInfo.remainingTime = (int)CanshuTemplate.GetValueByKey (CanshuTemplate.DIJI_TANBAOR_REFRESHTIME);
//									InItTanBaoInfo (tempMineInfo);
//								}
//							}
//
//							else if (tempEAInfo.type == 1)
//							{
//								CityGlobalData.isTanBaoGet = true;
//
//								if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress >= 0 && FirstTanBao.Instance.GetTanBaoState2 ())
//								{
//									CityGlobalData.m_isRightGuide = true;
//								}
//
//								if (tempMineInfo.isCanGet)
//								{
//									tempMineInfo.isCanGet = false;
//									tempMineInfo.gotTimes -= 1;
//									tempMineInfo.remainingTime = (int)CanshuTemplate.GetValueByKey (CanshuTemplate.GAOJI_TANBAO_REFRESHTIME);
//									
//									InItTanBaoInfo (tempMineInfo);
//								}
//								
//								else
//								{
//									m_tanBapResp.yuanBao -= tempMineInfo.cost;
//									SendMoneyInfo (tempEAInfo.type,m_tanBapResp);
//								}
//							}
//
//							else if (tempEAInfo.type == 10)
//							{
//								m_tanBapResp.yuanBao -= tempMineInfo.cost;
//								SendMoneyInfo (tempEAInfo.type,m_tanBapResp);
//							}
//
//							else if (tempEAInfo.type == 11 || tempEAInfo.type == 12)
//							{
//								m_tanBapResp.gongXian -= tempMineInfo.cost;
//								SendMoneyInfo (tempEAInfo.type,m_tanBapResp);
//							}
//						}
//					}
//
//					QXTanBaoData.Instance ().tanBaoResp = m_tanBapResp;
//					QXTanBaoData.Instance ().TanBaoCd ();
//
//					if (tempEAInfo.type == 10 || tempEAInfo.type == 12)
//					{
//						CreateMultipReward (tempEAInfo);
//					}
//
//					else
//					{
//						CreateSingleReward (tempEAInfo);
//					}
//
////					List<Award> newMiBaoList = new List<Award>();
////					foreach (Award tempAward in tempEAInfo.awardsList)
////					{
////						if (tempAward.itemType == 4 && (tempAward.pieceNumber == null || tempAward.pieceNumber == 0))
////						{
////							FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (6);
////							MainCityUIRB.SetRedAlert (functionTemp.m_iID,true);
////						}
////					}
//
//					tanBaoAwardResp = tempEAInfo;
//				}
//				
				return true;
			} 
//			case ProtoIndexes.EXPLORE_RESP://探宝失败返回
//			{
//				Debug.Log ("tbBuyFailRes:" + ProtoIndexes.EXPLORE_RESP);
//				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				ExploreResp failInfo = new ExploreResp();
//				
//				t_qx.Deserialize (t_stream,failInfo,failInfo.GetType ());
//				
//				if (failInfo != null) 
//				{
//					isClick = false;
//					failResp = failInfo;
//					Debug.Log ("探宝失败类型:" + failInfo.type);
//					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
//					                        FailTypeBack);
//				}
//				
//				return true;
//			}
			default: return false;
			}
		}
		
		return false;
	}
//
//	//发送成功弹窗加载
//	void FailTypeBack (ref WWW p_www,string p_path, Object p_object)
//	{
//		SetActiveZheZhao (false);
//
//		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
//
//		titleStr = "提示";
//
//		if (failResp.type == 1)
//		{
//			Debug.Log ("元宝不足");
//			
//			str = "元宝不足，是否立刻去充值？";
//
//			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,cancelStr,confirmStr,
//			             LackYBLoadBack);
//		}
//		else
//		{
//			switch (failResp.type)
//			{
//			case 2:
//				
//				Debug.Log ("次数不够");
//				
//				str = "免费次数已用完！";
//				
//				break;
//				
//			case 3:
//				
//				Debug.Log ("时间不到");
//				
//				str = "探宝时间未到！";
//				
//				break;
//				
//			case 4:
//				
//				Debug.Log ("数据错误");
//				
//				break;
//				
//			case 5:
//				
//				Debug.Log ("贡献值不足");
//				
//				str = "贡献值不足！";
//				
//				break;
//				
//			default:break;
//			}
//
//			uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,
//			             null);
//		}
//	}
//
//	void LackYBLoadBack (int i)
//	{
//		if (i == 2)
//		{
//			isOpenChongZhi = true;
//			DestroyRoot ();
//		}
//	}
//
//	void CreateMultipReward (ExploreAwardsInfo tempEAInfo)
//	{
//		GameObject multipObj = (GameObject)Instantiate (multipRewardObj);
//		multipObj.SetActive (true);
//		multipObj.transform.parent = multipRewardObj.transform.parent;
//		multipObj.transform.localPosition = multipRewardObj.transform.localPosition;
//		multipObj.transform.localScale = multipRewardObj.transform.localScale;
//
//		MultipleReward multipReward = multipObj.GetComponent<MultipleReward> ();
//		multipReward.GetMultipleRewardInfo (tempEAInfo);
//
//		SetActiveZheZhao (false);
//	}
//
//	void CreateSingleReward (ExploreAwardsInfo tempEAInfo)
//	{
//		GameObject singleObj = (GameObject)Instantiate (singleRewardObj);
//		singleObj.SetActive (true);
//		singleObj.transform.parent = singleRewardObj.transform.parent;
//		singleObj.transform.localPosition = singleRewardObj.transform.localPosition;
//		singleObj.transform.localScale = singleRewardObj.transform.localScale;
//		
//		SingleReward singleReward = singleObj.GetComponent<SingleReward> ();
//		singleReward.GetRewardInfo (tempEAInfo);
//
//		SetActiveZheZhao (false);
//	}
//
//	public void DestroyRoot ()
//	{
//		if (!isClick)
//		{
//			if (FreshGuide.Instance().IsActive(100040) && TaskData.Instance.m_TaskInfoDic[100040].progress < 0 && FirstTanBao.Instance.GetTanBaoState2 ())
//			{
//				UIYindao.m_UIYindao.CloseUI();
//				//			Debug.Log ("CloseYinDao");
//			}
//			QXTanBaoData.Instance().CheckFreeTanBao();
//			m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
//			m_ScaleEffectController.OnCloseWindowClick();
//		}
//	}
//
//    void DoCloseWindow()
//    {
//		MainCityUI.TryRemoveFromObjectList (this.gameObject);
//
//		if (isOpenChongZhi)
//		{
//			Debug.Log ("跳转到充值！");
//			TopUpLoadManagerment.m_instance.LoadPrefab(true);
//		}
//	
//        Destroy(this.gameObject);
//    }
//
//	//创建卡片飞落点
//	void CreateFlyPointList ()
//	{
//		for (int i = 0;i < 10;i ++)
//		{
//			GameObject point = (GameObject)Instantiate (flyPoint);
//
//			point.SetActive (true);
//			point.transform.parent = flyPoint.transform.parent;
//
//			if (i < 5)
//			{
//				point.transform.localPosition = new Vector3(-300 + 150 * i,130,0);
//			}
//
//			else
//			{
//				point.transform.localPosition = new Vector3(-300 + 150 * (i - 5),-130,0);
//			}
//
//			point.transform.localScale = Vector3.one;
//
//			flyPointList.Add (point);
//		}
//	}
//
//	void Update ()
//	{
//		if (publicZheZhao.activeSelf)
//		{
//			publicZheZhao.GetComponent<UISprite> ().alpha = 0.05f;
//		}
//
////		if (FirstTanBao.Instance.GetTanBaoState2 ())
////		{
////			CityGlobalData.m_isRightGuide = true;
////		}
//	}
//
//	public bool SetActiveZheZhao (bool isActive)
//	{
//		publicZheZhao.SetActive (isActive);
//		return true;
//	}
//
//	public void CameraShake ()
//	{
////		Debug.Log ("Shake!");
//		Hashtable shake = new Hashtable ();
//		shake.Add ("position",new Vector3(10f,10f,0));
//		shake.Add ("time",0.05f);
//		shake.Add ("islocal",true);
//		shake.Add ("oncomplete","ShakeEnd1");
//		shake.Add ("oncompletetarget",this.gameObject);
//		shake.Add ("easetype",iTween.EaseType.linear);
//		iTween.MoveTo (tbObj,shake);
//	}
//
//	void ShakeEnd1 ()
//	{
////		Debug.Log ("ShakeEnd!");
//		Hashtable shake = new Hashtable ();
//		shake.Add ("position",new Vector3(-20f,-20f,0));
//		shake.Add ("time",0.12f);
//		shake.Add ("islocal",true);
//		shake.Add ("oncomplete","ShakeEnd2");
//		shake.Add ("oncompletetarget",this.gameObject);
//		shake.Add ("easetype",iTween.EaseType.linear);
//		iTween.MoveTo (tbObj,shake);
//	}
//
//	void ShakeEnd2 ()
//	{
////		Debug.Log ("ShakeEnd!");
//		Hashtable shake = new Hashtable ();
//		shake.Add ("position",Vector3.zero);
//		shake.Add ("time",0.05f);
//		shake.Add ("islocal",true);
//		shake.Add ("easetype",iTween.EaseType.linear);
//		iTween.MoveTo (tbObj,shake);
//	}
//
	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
