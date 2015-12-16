using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LueDuoManager : MonoBehaviour {

	public static LueDuoManager ldManager;

	public LveDuoInfoResp lueDuoInfoRes;

	public GameObject leftObj;
	public GameObject rightObj;

	public GameObject nationBtnObj;

	private List<GameObject> btnsList = new List<GameObject>();

	public GameObject btnsObj;//包含规则按钮、切换技能按钮、对战记录按钮的obj
	public GameObject labelsObj;
	public UILabel numLabel;//显示剩余次数
	public UILabel timeLabel;
	public UILabel gongJinLabel;
	public GameObject addNumBtn;
	public GameObject resetBtn;

	public GameObject changeSkillBtn;//切换秘技按钮

	public GameObject tipObj;//红点提示

	public ScaleEffectController sEffectControl;

	private int nextPageReqCdTime = 5;

	private int cdTime;

	private List<string> ruleList = new List<string> ();

	string titleStr;
	string str;
	string cancelStr = "";
	string confirmStr = "";
	
	void Awake ()
	{
		ldManager = this;
	}

	void Start ()
	{
		titleStr = "提示";

		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		LueDuoData.Instance.LueDuoInfoReq ();

		//LanguageTemp.xml:582-585  ZHU_BU_RULE_(1,2,3,4)
		string ruleTitle = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_RULE_1);
		ruleList.Add (ruleTitle);
		
		for (int i = 0;i < 4;i ++)
		{
			string s = "ZHU_BU_RULE_" + (i + 1);
			LanguageTemplate.Text t = (LanguageTemplate.Text)System.Enum.Parse(typeof(LanguageTemplate.Text), s);
			string ruleStr = LanguageTemplate.GetText (t);
			
			ruleList.Add (ruleStr);
//			Debug.Log ("rule:" + ruleList[0] + ruleList[i + 1]);
		}
	}

	/// <summary>
	/// 获得掠夺首页信息
	/// </summary>
	/// <param name="tempRes">Temp res.</param>
	public void GetLueDuoData (LveDuoInfoResp tempRes)
	{
		CreateNationBtns (tempRes);

		if (tempRes.mengInfos.Count > 0)
		{
			LueDuoData.Instance.SetAllianceId = tempRes.mengInfos[0].mengId;
		}

		LDAllianceManager ldAlliance = leftObj.GetComponent<LDAllianceManager> ();
		ldAlliance.GetAllianceList (tempRes.mengInfos);

		LDOpponentManager ldOpponent = rightObj.GetComponent<LDOpponentManager> ();
		ldOpponent.GetJunZhuInfoList (tempRes.junInfos);

		SetLueDuoInfoState (tempRes);

		CheckNewRecord (tempRes.hasRecord);
	}

	/// <summary>
	/// 创建国家按钮
	/// </summary>
	/// <param name="tempRes">Temp res.</param>
	void CreateNationBtns (LveDuoInfoResp tempRes)
	{
		List<GuoInfo> nationList = new List<GuoInfo> ();

		GuoInfo myGuoInfo = new GuoInfo ();

//		for (int i = 0;i < tempRes.guoLianInfos.Count;i ++)
//		{
//			if (tempRes.guoLianInfos[i].hate == tempRes.showGuoId)
//			{
//				nationList.Add (tempRes.guoLianInfos[i]);
//				tempRes.guoLianInfos.RemoveAt (i);
//			}
//			if (JunZhuData.Instance ().m_junzhuInfo.guoJiaId == tempRes.guoLianInfos[i].guojiaId)
//			{
//				myGuoInfo = tempRes.guoLianInfos[i];
//				tempRes.guoLianInfos.RemoveAt (i);
//			}
//		}
//
//		for (int i = 0;i < tempRes.guoLianInfos.Count - 1;i ++)
//		{
//			for (int j = 0;j < tempRes.guoLianInfos.Count - i - 1;j ++)
//			{
//				if (tempRes.guoLianInfos[j].hate < tempRes.guoLianInfos[j + 1].hate)
//				{
//					GuoInfo tempInfo = tempRes.guoLianInfos[j];
//
//					tempRes.guoLianInfos[j] = tempRes.guoLianInfos[j + 1];
//
//					tempRes.guoLianInfos[j + 1] = tempInfo;
//				}
//			}
//		}

		for (int i = 0;i < tempRes.guoLianInfos.Count;i ++)
		{
			nationList.Add (tempRes.guoLianInfos[i]);
		}

//		nationList.Add (myGuoInfo);

		for (int i = 0;i < nationList.Count;i ++)
		{
			GameObject nationBtn = (GameObject)Instantiate (nationBtnObj);

			nationBtn.SetActive (true);

			nationBtn.transform.parent = nationBtnObj.transform.parent;
			nationBtn.transform.localPosition = new Vector3(0,-60 * i,0);
			nationBtn.transform.localScale = Vector3.one;

			btnsList.Add (nationBtn);

			LDNationBtn ldNationBtn = nationBtn.GetComponent<LDNationBtn> ();
			ldNationBtn.GetNationBtnInfo (nationList[i],i,new Vector3(0,-60 * i,0));
		}

		LueDuoData.Instance.SetNationId = nationList[0].guojiaId;
		LueDuoData.Instance.JunNationId = nationList[0].guojiaId;
		BtnScaleAnimation (0);
	}

	/// <summary>
	/// 国家按钮点击缩放
	/// </summary>
	/// <param name="index">国家id.</param>
	public void BtnScaleAnimation (int index)
	{
		for (int i = 0;i < btnsList.Count;i ++)
		{
			LDNationBtn ldNationBtn = btnsList[i].GetComponent<LDNationBtn> ();
			if (i == index)
			{
				ldNationBtn.BtnAnimation (true);
			}
			else
			{
				ldNationBtn.BtnAnimation (false);
			}
		}
	}

	/// <summary>
	/// 刷新掠夺页面信息
	/// </summary>
	/// <param name="reqType">Req type.</param>
	/// <param name="tempAllianceList">Temp alliance list.</param>
	/// <param name="tempJunZhuList">Temp jun zhu list.</param>
	public void RefreshLueDuoInfo (LueDuoData.ReqType reqType,List<LianMengInfo> tempAllianceList,List<JunZhuInfo> tempJunZhuList)
	{
		switch (reqType)
		{
		case LueDuoData.ReqType.Alliance:
		{
			LDAllianceManager ldAlliance = leftObj.GetComponent<LDAllianceManager> ();
			ldAlliance.GetAllianceList (tempAllianceList);

			LDOpponentManager ldOpponent = rightObj.GetComponent<LDOpponentManager> ();
			ldOpponent.GetJunZhuInfoList (tempJunZhuList);

			break;
		}
		case LueDuoData.ReqType.JunZhu:
		{
			LDOpponentManager ldOpponent = rightObj.GetComponent<LDOpponentManager> ();
			ldOpponent.GetJunZhuInfoList (tempJunZhuList);

			break;
		}
		default:
			break;
		}
	}

	/// <summary>
	/// 显示掠夺状态
	/// </summary>
	public void SetLueDuoInfoState (LveDuoInfoResp tempRes)
	{
		lueDuoInfoRes = tempRes;
		btnsObj.SetActive (true);

		StopCoroutine ("LueDuoCdTime");
		if (tempRes.all >= tempRes.nowMaxBattleCount)
		{
			addNumBtn.SetActive (false);

			if (tempRes.used >= tempRes.all)
			{
				btnsObj.transform.localPosition = new Vector3(10,-270,0);
				labelsObj.transform.localPosition = new Vector3(345,0,0);
				
				numLabel.text = "今日掠夺次数已用尽";
				resetBtn.SetActive (false);

				gongJinLabel.text = "贡金：" + tempRes.gongJin;
				timeLabel.text = "";
			}
			else
			{
				numLabel.text = "今日剩余次数：" + (tempRes.all - tempRes.used) + "/" + tempRes.all;

				if (tempRes.CdTime > 0)//掠夺冷却期
				{
					btnsObj.transform.localPosition = new Vector3(0,-270,0);
					labelsObj.transform.localPosition = new Vector3(345,0,0);
					gongJinLabel.text = "";
					resetBtn.SetActive (true);

					cdTime = tempRes.CdTime;
					StartCoroutine ("LueDuoCdTime");
				}
				else
				{
					btnsObj.transform.localPosition = new Vector3(10,-270,0);
					labelsObj.transform.localPosition = new Vector3(345,0,0);
					resetBtn.SetActive (false);

					gongJinLabel.text = "贡金：" + tempRes.gongJin;
					timeLabel.text = "";
				}
			}
		}
		else
		{
			numLabel.text = "今日剩余次数：" + (tempRes.all - tempRes.used) + "/" + tempRes.all;

			if (tempRes.used >= tempRes.all)
			{
				btnsObj.transform.localPosition = new Vector3(0,-270,0);
				labelsObj.transform.localPosition = new Vector3(335,0,0);

				resetBtn.SetActive (false);
				addNumBtn.SetActive (true);

				gongJinLabel.text = "贡金：" + tempRes.gongJin;
				timeLabel.text = "";
			}
			else
			{
				addNumBtn.SetActive (false);

				if (tempRes.CdTime > 0)//掠夺冷却期
				{
					btnsObj.transform.localPosition = new Vector3(0,-270,0);
					labelsObj.transform.localPosition = new Vector3(335,0,0);
					gongJinLabel.text = "";
					resetBtn.SetActive (true);

					cdTime = tempRes.CdTime;
					StartCoroutine ("LueDuoCdTime");
				}
				else
				{
					btnsObj.transform.localPosition = new Vector3(10,-270,0);
					labelsObj.transform.localPosition = new Vector3(345,0,0);
					resetBtn.SetActive (false);
					
					gongJinLabel.text = "贡金：" + tempRes.gongJin;
					timeLabel.text = "";
				}
			}
		}
	}

	IEnumerator LueDuoCdTime ()
	{
		string minuteStr = "";
		string secondStr = "";

		while (cdTime > 0) 
		{
			cdTime --;
			
			int minute = (cdTime / 60) % 60;
			int second = cdTime % 60;

			if (minute < 10)
			{
				minuteStr = "0" + minute;
			}
			else
			{
				minuteStr = minuteStr.ToString ();
			}

			if (second < 10) 
			{
				secondStr = "0" + second;
			} 
			else 
			{
				secondStr = second.ToString ();
			}

			timeLabel.text = "掠夺冷却：" + minuteStr + "：" + secondStr;
			
			if (cdTime == 0) 
			{
				lueDuoInfoRes.CdTime = 0;
				SetLueDuoInfoState (lueDuoInfoRes);
			}
			
			yield return new WaitForSeconds(1);
		}
	}



	/// <summary>
	/// 掠夺规则按钮
	/// </summary>
	public void RulesBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.LUE_DUO,ruleList);
		}
	}

	/// <summary>
	/// 掠夺记录按钮
	/// </summary>
	public void RecordBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			CheckNewRecord (false);
			LueDuoData.Instance.CheckNewRecord (false);
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LUEDUO_RECORD ),
			                        LueDuoRecordLoadBack );
		}
	}
	void LueDuoRecordLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject lueDuoRecord = GameObject.Instantiate( p_object ) as GameObject;
		lueDuoRecord.name = "LueDuoRecord";

		lueDuoRecord.transform.parent = sEffectControl.gameObject.transform;
		lueDuoRecord.transform.localPosition = Vector3.zero;
		lueDuoRecord.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// 掠夺记录按钮提示
	/// </summary>
	/// <param name="isNew">If set to <c>true</c> is new.</param>
	public void CheckNewRecord (bool isNew)
	{
		tipObj.SetActive (isNew);
	}

	/// <summary>
	/// 切换密保技能按钮
	/// </summary>
	public void ChangeSkillBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
			{
				return;
			}
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);
		}
	}

	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();
		mNewMiBaoSkill.Init ( (int)(CityGlobalData.MibaoSkillType.LueDuo_FangShou),lueDuoInfoRes.myFangShouId );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}
	/// <summary>
	/// 清除冷却cd按钮
	/// </summary>
	public void ClearCdBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			LueDuoData.Instance.SetBtnType = LueDuoData.BtnMakeType.ClearCd;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
			                        BoxLoadCallBack);
		}
	}

	/// <summary>
	/// 增加次数按钮
	/// </summary>
	public void AddNumBtn ()
	{
		if (!LueDuoData.Instance.IsStop)
		{
			LueDuoData.Instance.IsStop = true;

			LueDuoData.Instance.SetBtnType = LueDuoData.BtnMakeType.AddNum;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
			                        BoxLoadCallBack);
		}
	}

	private void BoxLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		var junZhuInfo = JunZhuData.Instance ().m_junzhuInfo;

		switch (LueDuoData.Instance.GetBtnType)
		{
		case LueDuoData.BtnMakeType.ClearCd:
		{
			if (junZhuInfo.vipLv < lueDuoInfoRes.canClearCdVIP)
			{
				str = "\n\n达到VIP" + lueDuoInfoRes.canClearCdVIP + "级可清除冷却！";

				uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,CantClearCd);
			}
			else
			{
				str = "\n\n是否使用" + lueDuoInfoRes.clearCdYB + "元宝清除冷却时间？";

				uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,cancelStr,confirmStr,LueDuoData.Instance.ClearCdCallBack);
			}

			break;
		}
		case LueDuoData.BtnMakeType.AddNum:
		{
			str = "\n掠夺次数用尽！\n" + "是否使用" + lueDuoInfoRes.buyNextBattleYB + "元宝购买" +
				lueDuoInfoRes.buyNextBattleCount + "次掠夺机会？\n今日还可购买" + lueDuoInfoRes.remainBuyHuiShi + "次";

			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,cancelStr,confirmStr,LueDuoData.Instance.AddNumCallBack);

			break;
		}
		default:
			break;
		}
	}

	//不能清除冷却cd
	void CantClearCd (int i)
	{
		LueDuoData.Instance.IsStop = false;
	}

	/// <summary>
	/// 设置联盟信息可刷新
	/// </summary>
	public void SetLdAllianceCanTap ()
	{
		LDAllianceManager ldAlliance = leftObj.GetComponent<LDAllianceManager> ();
		ldAlliance.SetCanTap = true;
	}

	private bool isOpenChongZhi = false;//是否打开充值
	public bool IsOpenChongZhi
	{
		set{isOpenChongZhi = value;}
	}
	public void DestroyRoot ()
	{
		MainCityUI.TryRemoveFromObjectList (this.gameObject);

		sEffectControl.CloseCompleteDelegate = DoCloseWindow;
		sEffectControl.OnCloseWindowClick();
	}
	void DoCloseWindow()
	{
		if (isOpenChongZhi)
		{
			//跳转到充值
			TopUpLoadManagerment.m_instance.LoadPrefab(true);
		}
		LueDuoData.Instance.IsStop = false;
		Destroy(this.gameObject);
	}
}
