using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YunBiaoMainPage : MonoBehaviour,SocketProcessor
{
    public ScaleEffectController m_ScaleEffectController;

	public static YunBiaoMainPage yunBiaoMainData;
	/// <summary>
	/// The camera object.
	/// </summary>
	public GameObject cameraObj;
	/// <summary>
	/// 运镖按钮
	/// </summary>
	public GameObject yunBiaoBtnObj;
	/// <summary>
	/// 查看进度按钮
	/// </summary>
	public GameObject checkBtnObj;
	/// <summary>
	/// 运镖次数
	/// </summary>
	public UILabel yunBiaoNum;
	/// <summary>
	/// 劫镖次数
	/// </summary>
	public UILabel jieBiaoNum;
	/// <summary>
	/// 确定和取消按钮字符串
	/// </summary>
	[HideInInspector]public string confirmStr;
	[HideInInspector]public string cancelStr;
	/// <summary>
	/// 弹框提示
	/// </summary>
	private string titleStr;
	/// <summary>
	/// 弹框提示描述
	/// </summary>
	private string str;

	/// <summary>
	/// 劫镖记录返回
	/// </summary>
	public YBHistoryResp m_recordRes;
	/// <summary>
	/// 购买运镖次数需要的元宝数
	/// </summary>
	private int buyYBTimeNeedYb;
	/// <summary>
	/// 最大vip等级
	/// </summary>
	private int maxVipLevel;
	/// <summary>
	/// 购买运镖次数返回
	/// </summary>
	private BuyCountsResp ybBuyRes;
	/// <summary>
	/// 运镖规则
	/// </summary>
	public UILabel rule;

	public GameObject recordTip;
	public GameObject enemyTip;

	void Awake ()
	{
		yunBiaoMainData = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		titleStr = "提示";
		maxVipLevel = 10;
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		YunBiaoData.Instance.YunBiaoInfoReq ();
	}

	/// <summary>
	/// 劫镖记录请求
	/// </summary>
	public void JieBiaoRecordReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_HISTORY_RSQ,"3428");
		Debug.Log ("劫镖记录请求：" + ProtoIndexes.C_YABIAO_HISTORY_RSQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YABIAO_HISTORY_RESP://劫镖记录返回
			{
				Debug.Log ("劫镖记录返回：" + ProtoIndexes.S_YABIAO_HISTORY_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YBHistoryResp recordRes = new YBHistoryResp();
				
				t_qx.Deserialize(t_stream, recordRes, recordRes.GetType());

				if (recordRes != null)
				{
					Debug.Log ("recordRes:" + recordRes);
					if (recordRes.historyList == null)
					{
						recordRes.historyList = new List<YBHistory>();
					}

					m_recordRes = recordRes;

					GameObject jBRecordObj = GameObject.Find ("JieBiaoRecordObj");

					if (jBRecordObj != null)
					{
						JieBiaoRecordPage recordPage = jBRecordObj.GetComponent<JieBiaoRecordPage> ();
						recordPage.InItRecordItem (recordRes);
					}
					else
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YUNBIAO_RECORD ),
						                        JieBiaoRecordLoadBack );
					}
				}

				return true;
			}

			case ProtoIndexes.S_YABIAO_BUY_RESP://购买运镖次数返回
			{
				Debug.Log ("购买运镖次数返回：" + ProtoIndexes.S_YABIAO_BUY_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				BuyCountsResp ybBuyTimesRes = new BuyCountsResp();
				
				t_qx.Deserialize(t_stream, ybBuyTimesRes, ybBuyTimesRes.GetType());

				if (ybBuyTimesRes != null)
				{
					if (CityGlobalData.GetYunBiaoBuyType == 10)
					{
						ybBuyRes = ybBuyTimesRes;
						
						if (ybBuyTimesRes.result == 10)
						{
							Debug.Log ("剩余运镖次数：" + ybBuyTimesRes.leftYBTimes);
							Debug.Log ("已经购买运镖回数：" + ybBuyTimesRes.usedYBVip);
							
							YunBiaoData.Instance.yunBiaoRes.yaBiaoCiShu = ybBuyTimesRes.leftYBTimes;
							YunBiaoData.Instance.yunBiaoRes.buyCiShu = ybBuyTimesRes.usedYBVip;
							
							InItYunBiaoMainPage (YunBiaoData.Instance.yunBiaoRes);
						}
						
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        BuyYunBiaoTimesLoadBack );
					}
				}

				return true;
			}
			}
		}

		return false;
	}
	void JieBiaoRecordLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject jBRecordObj = GameObject.Instantiate( p_object ) as GameObject;
		
		jBRecordObj.name = "JieBiaoRecordObj";
		jBRecordObj.SetActive (true);
		jBRecordObj.transform.parent = cameraObj.transform;
		jBRecordObj.transform.localPosition = Vector3.zero;
		jBRecordObj.transform.localScale = Vector3.one;
		
		JieBiaoRecordPage recordPage = jBRecordObj.GetComponent<JieBiaoRecordPage> ();
		recordPage.GetRecordInfo (m_recordRes);
	}

	/// <summary>
	/// 购买运镖次数失败返回
	/// </summary>
	void BuyYunBiaoTimesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();
		Debug.Log ("caca");
		switch (ybBuyRes.result)
		{
		case 10://购买成功

			str = "购买成功！";
			uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
			             null, confirmStr, null,null);

			break;

		case 20://元宝不足

			str = "您的元宝不足...\n是否去充值？";
			uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
			             null, cancelStr, confirmStr,LockYuanBaoBack);

			break;

		case 30://今日购买次数已用完

			break;
		default:break;
		}
	}

	/// <summary>
	/// 初始化运镖首页
	/// </summary>
	/// <param name="tempInfoRes">Temp info res.</param>
	public void InItYunBiaoMainPage (YabiaoMainInfoResp tempInfoRes)
	{
		switch (tempInfoRes.state)
		{
		case 10:

			Debug.Log ("押镖");
			yunBiaoBtnObj.SetActive (false);
			checkBtnObj.SetActive (true);

			break;

		case 20:

			Debug.Log ("未押镖");
			yunBiaoBtnObj.SetActive (true);
			checkBtnObj.SetActive (false);

			break;
		}

		string countTimeStr = LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_42);
		string[] countTimeStrLength = countTimeStr.Split ('*');

		yunBiaoNum.text = countTimeStrLength[0] + "[dc0600]" + tempInfoRes.yaBiaoCiShu.ToString () + "[-]" + countTimeStrLength[1];
		jieBiaoNum.text = countTimeStrLength[0] + "[dc0600]" + tempInfoRes.jieBiaoCiShu.ToString () + "[-]" + countTimeStrLength[1];

		//LanguageTemp LID:533-538 LanguageTemplate:YUN_BIAO_78-YUN_BIAO_83
		rule.text = "";
		rule.fontSize = 30;
		List<string> ruleList = new List<string> ();
		string ybTimeStr = "";
		for (int i = 0;i < 6;i ++)
		{
			string s = "YUN_BIAO_" + (78 + i);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			if (i == 1)
			{
				//开服时间
				string openTime = CanshuTemplate.GetStrValueByKey (CanshuTemplate.OPENTIME_YUNBIAO);
				string closeTime = CanshuTemplate.GetStrValueByKey (CanshuTemplate.CLOSETIME_YUNBIAO);
				ruleStr += "[dc0600]" + openTime + "—" + closeTime + "[-]";
				ybTimeStr = ruleStr;
			}

			if (i == 3)
			{
				string[] strLen = ruleStr.Split ('*');

				ruleStr = strLen[0] + "[00ff00]" + 50 + "[-]" + strLen[1];
			}

			if (i != 1)
			{
				ruleList.Add (ruleStr);
			}
		}

		for (int i = 0;i < ruleList.Count;i ++)
		{
			ybTimeStr += "\n" + ruleList[i];
		}
		rule.text = ybTimeStr;
	}

	/// <summary>
	/// 运镖按钮(跳转到选马页面)
	/// </summary>
	public void YunHuoBtn ()
	{
		if (YunBiaoData.Instance.yunBiaoRes.isOpen)
		{
			if (YunBiaoData.Instance.yunBiaoRes.yaBiaoCiShu > 0)
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.SELECT_HORSE ),
				                        HorseMainPageLoadBack );
			}
			else
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        CantYunBiaoLoadBack );
			}
		}
		else
		{
			Debug.Log ("运镖活动未开启");
			YunBiaoNotOpen ();
		}
	}
	void HorseMainPageLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject horseMainPageObj = GameObject.Instantiate( p_object ) as GameObject;
		
		horseMainPageObj.transform.parent = cameraObj.transform;
		horseMainPageObj.name = "HorseMainPage";
		
		horseMainPageObj.transform.localPosition = Vector3.zero;
		horseMainPageObj.transform.localScale = Vector3.one;
	}

	void CantYunBiaoLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		if (JunZhuData.Instance ().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey (19))//购买运镖次数开启条件
		{
			int haveBuyTime = YunBiaoData.Instance.yunBiaoRes.buyCiShu;//已经购买几回
			
			VipTemplate vipTemp = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance ().m_junzhuInfo.vipLv);
			int canBuyYBTimes = vipTemp.YunbiaoTimes;//可以购买几回
			
			if (JunZhuData.Instance ().m_junzhuInfo.vipLv <= maxVipLevel)//最大vip等级 10 级
			{
				if (haveBuyTime < canBuyYBTimes)//允许购买运镖次数
				{
					PurchaseTemplate purchasTemp = PurchaseTemplate.GetPurchaseTempByTime (haveBuyTime + 1);
					buyYBTimeNeedYb = purchasTemp.price;
					
					str = "\n您今日的运镖次数已用尽！\n确定消耗" + purchasTemp.price + "元宝购买"
						+ purchasTemp.number + "次运镖次数？\n今天还可购买" + (canBuyYBTimes - haveBuyTime) + "次！";
					
					uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,  
					             null, cancelStr, confirmStr,
					             BuyYBTimesBack);
				}
				else
				{
					if (JunZhuData.Instance ().m_junzhuInfo.vipLv == maxVipLevel)
					{
						str = "\n\n您今日的购买运镖次数已用尽...";
					}
					else
					{
						int countVip = maxVipLevel - JunZhuData.Instance ().m_junzhuInfo.vipLv;

						int nextCanBuyVip = 0;

//						Debug.Log ("vipTemp.YunbiaoTimes：" + vipTemp.YunbiaoTimes);
//						Debug.Log ("countVip：" + countVip);
//						Debug.Log ("JunZhuVip：" + JunZhuData.Instance ().m_junzhuInfo.vipLv);

						for (int i = 0;i < countVip;i ++)
						{
							VipTemplate cVipTemp = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance ().m_junzhuInfo.vipLv + (i + 1));
//							Debug.Log ("i:" + i + "||" + "cVipTemp：" + cVipTemp.YunbiaoTimes);
							if (cVipTemp.YunbiaoTimes > canBuyYBTimes)
							{
//								Debug.Log ("cVipTemp.YunbiaoTimes：" + cVipTemp.YunbiaoTimes);
								nextCanBuyVip = JunZhuData.Instance ().m_junzhuInfo.vipLv + (i + 1);
								break;
							}
						}

						str = "\n您今日的购买运镖次数已用尽...\n提升到VIP" + nextCanBuyVip + "级时可以够买更多次数！";
					}
					uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
					             null, confirmStr, null,
					             null);
				}
			}
		}
		else
		{
			str = "\n升到VIP" + VipFuncOpenTemplate.GetNeedLevelByKey (19) + "级可购买运镖次数！";
			uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
			             null, confirmStr, null,
			             null);
		}
	}

	void BuyYBTimesBack (int i)
	{
		if (i == 2)
		{
			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao >= buyYBTimeNeedYb)
			{
				//发送购买次数请求
				BuyYunBiaoTimesReq ();
			}
			else
			{
				//元宝不足
				LackYbBoxLoad ();
			}
		}
	}

	/// <summary>
	/// 购买运镖次数请求
	/// </summary>
	void BuyYunBiaoTimesReq ()
	{
		BuyCountsReq yBTimesReq = new BuyCountsReq();
		
		yBTimesReq.type = 10;

		CityGlobalData.SetYunBiaoBuyType = yBTimesReq.type;

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,yBTimesReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_BUY_RSQ,ref t_protof,"3426");
	}

	/// <summary>
	/// 跳转到vip充值
	/// </summary>
	/// <param name="i">The index.</param>
	void TurnToVip (int i)
	{
		if (i == 2)
		{
			//跳转到vip充值
		}
	}

	/// <summary>
	/// 元宝不足弹框
	/// </summary>
	public void LackYbBoxLoad ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        LackYuanBaoLoadBack );
	}

	/// <summary>
	/// 元宝不足弹框loadback
	/// </summary>
	void LackYuanBaoLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		str = "您的元宝不足...\n是否去充值？";
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, cancelStr, confirmStr,LockYuanBaoBack);
	}
	void LockYuanBaoBack (int i)
	{
		if (i == 2)
		{
			//跳转到充值
			TopUpLoadManagerment.m_instance.LoadPrefab(true);
			DestroyRoot ();
		}
	}

	/// <summary>
	/// 截镖按钮(跳转到截货界面)
	/// </summary>
	public void JieHuoBtn ()
	{
		if (YunBiaoData.Instance.yunBiaoRes.isOpen)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.JIEBIAO_MAIN_PAGE ),
			                        JieBiaoMainPageLoadBack );
		}
		else
		{
			YunBiaoNotOpen ();
		}
	}
	void JieBiaoMainPageLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject jieHuoMainPageObj = GameObject.Instantiate( p_object ) as GameObject;

		jieHuoMainPageObj.transform.parent = cameraObj.transform;
		jieHuoMainPageObj.name = "JieHuoMainPage";
		
		jieHuoMainPageObj.transform.localPosition = Vector3.zero;
		jieHuoMainPageObj.transform.localScale = Vector3.one;
	}
	/// <summary>
	/// 设置进度条
	/// </summary>
	public void InItScrollBarValue (UIScrollBar scrollBar,int value)
	{
//		Debug.Log ("value:" + value);
		
		scrollBar.barSize = (float)value/100;
		
//		Debug.Log ("scrollBar.barSize:" + scrollBar.barSize);
	}

	/// <summary>
	/// scrollview 子对象回弹到顶部控制
	/// </summary>
	public void MoveBackToTop (GameObject obj,int curNum,int maxNum)
	{
		ItemTopCol itemTopCol = obj.GetComponent<ItemTopCol> ();

		if (curNum < maxNum)
		{
			itemTopCol.enabled = true;
		}
		else
		{
			itemTopCol.enabled = false;
		}
	}

	/// <summary>
	/// 仇人按钮
	/// </summary>
	public void EnemiesBtn ()
	{
		YunBiaoData.Instance.isNewEnemy = false;
		CheckNewEnemyOrRecord (YunBiaoData.Instance.isNewEnemy,
		                       YunBiaoData.Instance.isNewRecord);

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YUNBIAO_ENEMIES ),
		                        YunBiaoEnemiesLoadBack );
	}
	void YunBiaoEnemiesLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject enemiesObj = GameObject.Instantiate( p_object ) as GameObject;
		enemiesObj.transform.parent = cameraObj.transform;
		enemiesObj.name = "JieBiaoEnemies";
		
		enemiesObj.transform.localPosition = Vector3.zero;
		enemiesObj.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// 劫镖记录按钮
	/// </summary>
	public void RecordBtn ()
	{
		YunBiaoData.Instance.isNewRecord = false;
		CheckNewEnemyOrRecord (YunBiaoData.Instance.isNewEnemy,
		                       YunBiaoData.Instance.isNewRecord);

		JieBiaoRecordReq ();
	}

	/// <summary>
	/// 排行
	/// </summary>
	public void RankBtn ()
	{
		Debug.Log ("排行功能暂未开启");
		ClientMain.m_UITextManager.createText(MyColorData.getColorString (5,"排行功能暂未开启"));
	}

	/// <summary>
	/// 运镖未开启提示
	/// </summary>
	public void YunBiaoNotOpen ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        YunBiaoNotOpenLoadBack );
	}

	void YunBiaoNotOpenLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		str = "\n\n运镖活动还未到开启时间！";
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, confirmStr, null,null);
	}

	/// <summary>
	/// 获得收益数值
	/// </summary>
	public int GetHorseAwardNum (int type)
	{
		JunzhuShengjiTemplate junZhuUpLevelTemp = JunzhuShengjiTemplate.GetJunZhuShengJi (JunZhuData.Instance ().m_junzhuInfo.level);
		int xiShu = junZhuUpLevelTemp.xishu;
		
		CartTemplate cartemp = CartTemplate.GetCartTemplateByType (type);
		float award = cartemp.ProfitPara * xiShu;
		
		return (int)award;
	}

	/// <summary>
	/// 检测新的对战记录或新仇人
	/// </summary>
	public void CheckNewEnemyOrRecord (bool isNewEnemy,bool isNewRecord)
	{
		recordTip.SetActive (isNewRecord);
		enemyTip.SetActive (isNewEnemy);
	}

	/// <summary>
	/// 销毁镖局prefab
	/// </summary>
	public void DestroyRoot ()
	{
	    m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
        YunBiaoData.Instance.CheckNewRecordOrEnemy(YunBiaoData.Instance.isNewEnemy,
                                            YunBiaoData.Instance.isNewRecord);
		MainCityUI.TryRemoveFromObjectList (this.gameObject);


        MainCityUI.TryRemoveFromObjectList(gameObject);
        Destroy(this.gameObject);
    }

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
