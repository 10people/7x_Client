using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceFightMainPage : MonoBehaviour {

	public static AllianceFightMainPage fightMainPage;

	private RequestFightInfoResp fightResp; 

	public UILabel stateLabel;//联盟战状态

	public GameObject applyObj;
	public GameObject gameObj;

	public UILabel applyDesLabel;//报名描述
	public UILabel applyTimeLabel;//报名截止时间
	public GameObject applyBtnObj;//报名按钮
	private long leftTime;//报名剩余时间

	public UILabel gameStatusLabel;//比赛状态
	public GameObject gameGrid;
	public GameObject fightItemObj;
	private List<GameObject> fightItemList = new List<GameObject> ();
	private int m_allianceId;//我的联盟id

	public GameObject enterFightBtn;//加入战斗按钮

	private List<string> rulesList = new List<string>();

	public ScaleEffectController m_ScaleEffectController;

	void Awake ()
	{
		fightMainPage = this;
	}

	void Start ()
	{
		GetRulesList ();
		AllianceFightData.Instance.AllianceFightDataReq ();
	}

	//挑战规则
	void GetRulesList ()
	{
		string ruleStr = LanguageTemplate.GetText (LanguageTemplate.Text.UNIT_WAR_RULE).Replace (@"\n","#");
		string[] ruleStrLen = ruleStr.Split ('#');
		for (int i = 0;i < ruleStrLen.Length;i ++)
		{
			rulesList.Add (ruleStrLen[i]);
		}
	}

	//获得联盟战首页信息
	public void GetAllianceFightResp (RequestFightInfoResp tempResp)
	{
		fightResp = tempResp;
		leftTime = tempResp.applyRemaintime;

		stateLabel.text = ShowAllianceFightState (tempResp.state);

		StopCoroutine ("ShowLeftTime");

		if (tempResp.state == 8) 
		{
			applyObj.SetActive (true);
			gameObj.SetActive (false);

			string applyDesText = "";

			if (!tempResp.isCanApply)
			{
				//联盟不能报名
				applyDesText = "您的联盟未达到联盟战的报名要求，\n\n\n\n请提升联盟等级招募更多玩家。";

				applyBtnObj.SetActive (false);

				applyTimeLabel.transform.localPosition = new Vector3(0,-225,0);

				StartCoroutine ("ShowLeftTime");
			}
			else
			{
				//联盟能报名
				if (!tempResp.isApply)
				{
					//未报名
					applyDesText = "您的联盟已经达到联盟战的报名要求\n\n盟主和副盟主有权报名参加联盟战\n\n请提醒盟主和副盟主报名，以免错过联盟战报名";

					applyBtnObj.SetActive (AllianceData.Instance.g_UnionInfo.identity == 0 ? false : true);

					applyTimeLabel.transform.localPosition = AllianceData.Instance.g_UnionInfo.identity == 0 ? new Vector3 (0,-225,0) : new Vector3 (0,-190,0);

					StartCoroutine ("ShowLeftTime");
				}
				else
				{
					//已报名
					if (!tempResp.isCanFight)
					{
						//没资格参赛
						applyDesText = "根据当前联盟排行榜排行情况，您所在联盟目前\n\n没有资格参加本次联盟战\n\n请继续提升联盟排行，超越其他联盟争取参赛资格！";
					}
					else
					{
						//有资格参赛
						applyDesText = "根据当前联盟排行榜排行情况，您所在联盟目前\n\n有资格参加本次联盟战\n\n请继续提升联盟排行，以免被后面的联盟夺走参赛资格！";
					}
					applyBtnObj.SetActive (false);
					applyTimeLabel.transform.localPosition = new Vector3(0,-225,0);

					applyTimeLabel.text = MyColorData.getColorString (1,"已报名参赛");
				}
			}

			applyDesLabel.text = applyDesText;
		}
		else 
		{
			applyObj.SetActive (false);
			gameObj.SetActive (true);
			enterFightBtn.SetActive (false);

			m_allianceId = AllianceData.Instance.g_UnionInfo.id;

			//0-未开始，1-正在进行中，2-已经结束
			switch (tempResp.fightState)
			{
			case 0:
			{
				gameStatusLabel.text = "本轮比赛将于\n\n今日" + tempResp.startTime + "开始";
				enterFightBtn.SetActive (false);
				break;
			}
			case 1:
			{
				if (tempResp.isCanFight)
				{
					Debug.Log ("m_allianceId:" + m_allianceId);
					Debug.Log ("tempResp.matchInfos.Count:" + tempResp.matchInfos.Count);
					for (int i = 0;i < tempResp.matchInfos.Count;i ++)
					{
						List<int> lmIdList = new List<int>();
						lmIdList.Add (tempResp.matchInfos[i].lm1Id);
						lmIdList.Add (tempResp.matchInfos[i].lm2Id);
						Debug.Log ("lmIdList[0]:" + lmIdList[0]);
						Debug.Log ("lmIdList[1]:" + lmIdList[1]);
						if (lmIdList.Contains (m_allianceId))
						{
							if (lmIdList.Contains (-1))
							{
								gameStatusLabel.text = "本轮比赛轮空\n\n直接晋级下一轮";
							}
							else
							{
								gameStatusLabel.text = "";
								enterFightBtn.SetActive (true);
								Debug.Log ("dsaf");
							}
							break;
						}
						else
						{
							gameStatusLabel.text = "已淘汰";
						}
					}
				}
				else
				{
					gameStatusLabel.text = "本届比赛未达\n\n到参赛资格";
				}

				break;
			}
			case 2:
			{
				gameStatusLabel.text = "本轮比赛已结束";
				break;
			}
			default:
				break;
			}

			CreateFightItemList (tempResp.matchInfos);
		}
	}

	//创建fightItem
	void CreateFightItemList (List<FightMatchInfo> tempFightList)
	{
		Debug.Log ("tempFightList.Count:" + tempFightList.Count);
		if (tempFightList.Count > fightItemList.Count)
		{
			Debug.Log (">");
			int count  = tempFightList.Count - fightItemList.Count;
			for (int i = 0;i < count;i ++)
			{
				GameObject fightItem = (GameObject)Instantiate (fightItemObj);

				fightItem.SetActive (true);
				fightItem.transform.parent = gameGrid.transform;
				fightItem.transform.localPosition = Vector3.zero;
				fightItem.transform.localScale = Vector3.one;

				fightItemList.Add (fightItem);
			}
		}
		else if (tempFightList.Count < fightItemList.Count)
		{
			Debug.Log ("<");
			int count  = fightItemList.Count - tempFightList.Count;
			for (int i = 0;i < count;i ++)
			{
				Destroy (fightItemList[0]);
				fightItemList.RemoveAt (0);
			}
		}
		gameGrid.GetComponent<UIGrid> ().repositionNow = true;

		List<FightMatchInfo> fightMatchList = new List<FightMatchInfo> ();
		for (int i = 0;i < tempFightList.Count;i ++)
		{
			if (tempFightList[i].lm1Id == m_allianceId || tempFightList[i].lm2Id == m_allianceId)
			{
				fightMatchList.Add (tempFightList[i]);

				tempFightList.Remove (tempFightList[i]);
			}
		}

		for (int i = 0;i < tempFightList.Count;i ++)
		{
			fightMatchList.Add (tempFightList[i]);
		}

		for (int i = 0;i < fightMatchList.Count;i ++)
		{
			AllianceFightItem allianceFight = fightItemList[i].GetComponent<AllianceFightItem> ();
			allianceFight.GetAlianceFightItemInfo (fightMatchList[i],m_allianceId);
		}
	}

	// 联盟战状态：赛程，0-无，1-32强，2-16强，3-8强，4-4强，5-半决赛，6-三四名比赛，7-决赛，8-报名
	private string ShowAllianceFightState (int stateId)
	{
		string StateText = "当前处于";
		string gameText = "比赛阶段";
		switch (stateId)
		{
		case 0:
			StateText = "";
			break;
		case 1:
			StateText += gameText + "32强赛";
			break;
		case 2:
			StateText += gameText + "16强赛";
			break;
		case 3:
			StateText += gameText + "8强赛";
			break;
		case 4:
			StateText += gameText + "4强赛";
			break;
		case 5:
			StateText += gameText + "半决赛";
			break;
		case 6:
			StateText += gameText + "三四名决赛";
			break;
		case 7:
			StateText += gameText + "决赛";
			break;
		case 8:
			StateText += "报名阶段";
			break;
		default:
			break;
		}
		return StateText;
	}

	IEnumerator ShowLeftTime ()
	{
		if (leftTime == 0)
		{
			applyTimeLabel.text = MyColorData.getColorString (5,"报名截止");
		}
		Debug.Log ("leftTime:" + leftTime);
		//距离报名截止时间还有*天**小时**分**秒
		string hourStr = "";
		string minuteStr = "";
		string secondStr = "";

		while (leftTime > 0) 
		{
			leftTime --;

			long day = leftTime / (3600 * 24);
			long hour = (leftTime / 3600) % 24;
			long minute = (leftTime / 60) % 60;
			long second = leftTime % 60;

			if (hour < 10)
			{
				hourStr = "0" + hour;
			}
			else
			{
				hourStr = hour.ToString ();
			}
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

			applyTimeLabel.text = MyColorData.getColorString (5,"距离报名截止时间还有" + day + "天" + hourStr + "时" + minuteStr + "分" + secondStr + "秒");

			if (leftTime == 0) 
			{
				AllianceFightData.Instance.AllianceFightDataReq ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	//报名按钮
	public void ApplyBtn ()
	{
		AllianceFightData.Instance.AllianceFightApplyReq ();
	}

	//历史按钮
	public void HistoryBtn ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_BATTLE_HISTORY ),
		                        AllianceFightRecordLoadBack );
	}
	void AllianceFightRecordLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject allianceFightRecord = GameObject.Instantiate( p_object ) as GameObject;
	}

	//商店按钮
	public void StoreBtn ()
	{
		GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.ALLIANCE_FIGHT,GeneralControl.StoreReqType.FREE,"功勋商店");
	}

	//规则按钮
	public void RuleBtn ()
	{
		GeneralControl.Instance.LoadRulesPrefab (GeneralControl.RuleType.ALLIANCE_FIGHT,rulesList);
	}

	//加入战斗按钮
	public void EnterFightBtn ()
	{
		//Send character sync message to server.
		EnterScene tempEnterScene = new EnterScene();
		
		tempEnterScene.senderName = SystemInfo.deviceName;
		
		tempEnterScene.uid = 0;
		
		tempEnterScene.posX = 0;
		
		tempEnterScene.posY = 1;
		
		tempEnterScene.posZ = 0;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, tempEnterScene);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_FIGHT_SCENE, ref t_protof);
		
		SceneManager.EnterAllianceBattle();
	}

	//返回按钮
	public void BackBtn ()
	{
		Destroy (this.gameObject);
	}

	//关闭按钮
	public void CloseBtn ()
	{
		_MyAllianceManager.Instance().Closed();
		m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
		m_ScaleEffectController.OnCloseWindowClick();
	}
	void OnCloseWindow ()
	{
		Destroy (this.gameObject);
	}
}
