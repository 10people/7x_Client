using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChuanQiType : MonoBehaviour,SocketProcessor {

	private Level levelInfo;

	public GameObject notCrossIcon;//未通关时显示的icon
	public GameObject crossIcon;//通关时显示的icon

	public GameObject notCrossDesObj;//未通关时显示的ui
	public GameObject crossDesObj;//通关时显示的ui

	public UILabel saoDangTime;//扫荡次数label
	public UILabel saoDangFinish;//扫荡次数用尽显示的label

	public GameObject saoDangBtn1;//扫荡一次按钮
	public GameObject saoDangBtn3;//连续扫荡按钮
	public GameObject sdBtnBg3;//连续扫荡按钮置灰背景

	public UILabel VIP_DesLabel;//vip扫荡描述label

	public UILabel tiaoZhanTimes;//今日已挑战次数
	private int tiaoZhanNum;

	public  UILabel ResstingCost;

	public UISprite WinType;

	private int vipLevel;//vip等级

	public GameObject fightBtn;//攻击按钮

	public GameObject enemyListObj;

	public GameObject awardListObj;

	public GameObject ResettingBtn;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	//获得关卡以及秘宝信息
	public void GetNeedInfo (Level tempLevelInfo,GuanQiaInfo m_GuanQia)
	{
		levelInfo = tempLevelInfo;

		//Debug.Log ("tempLevelInfo.cqSaoDangDayTimes  = "+m_GuanQia.cqSaoDangDayTimes);
		//Debug.Log ("tempLevelInfo.cqSaoDangUsedTimes  = "+m_GuanQia.cqSaoDangUsedTimes);
		tiaoZhanNum = 3 - m_GuanQia.cqPassTimes;
		sdTotleTime = m_GuanQia.cqSaoDangDayTimes;
		sdCountTime = sdTotleTime - m_GuanQia.cqSaoDangUsedTimes;
		tiaoZhanTimes.text = (3-m_GuanQia.cqPassTimes).ToString ()+"/3";

		ResstingCost.text = PveLevelUImaneger.GuanqiaReq.cqResetPay.ToString();

		ShowInfo (false);

		if(m_GuanQia.cqPassTimes < 3)
		{
			ResettingBtn.SetActive(false);
		}
		else{
			ResettingBtn.SetActive(true);
		}
		UICreateEnemy enemyCreate = enemyListObj.GetComponent<UICreateEnemy> ();

		enemyCreate.InItEnemyList (tempLevelInfo.type);

		UICreateDropthings awardCreate = awardListObj.GetComponent<UICreateDropthings> ();

		awardCreate.mLevl = levelInfo;

		awardCreate.GetAward (tempLevelInfo.type);
	}
	int sdTotleTime = 0;
	int sdCountTime = 0;
	void ShowInfo (bool isFresh)
	{
		if(isFresh)
		{
			tiaoZhanTimes.text = "3"+"/3";
		}
		vipLevel =  JunZhuData.Instance ().m_junzhuInfo.vipLv;
		if (levelInfo.chuanQiPass)
		{
			notCrossIcon.SetActive (false);
			crossIcon.SetActive (true);
			
			int starsAward = levelInfo.pingJia;

			int my_star = starsAward;

			Debug.Log("levelInfo.pingJia = "+levelInfo.pingJia);

			if(levelInfo.pingJia == 1)
			{
				WinType.spriteName = "X_Victoyr";
			}
			if(levelInfo.pingJia == 2)
			{
				WinType.spriteName = "L_Victoyr";
			}
			if(levelInfo.pingJia == 3)
			{
				WinType.spriteName = "Prefect_Victor";
			}


			if (levelInfo.pingJia == 3)
			{
				notCrossDesObj.SetActive (false);
				crossDesObj.SetActive (true);

			  
                    //MapData.mapinstance.myMapinfo.saoDangDayTimes;//扫荡总次数
			   
                    //sdTotleTime - MapData.mapinstance.myMapinfo.saoDangUsedTimes;//扫荡剩余次数
				
				if (sdCountTime > 0)
				{
					saoDangFinish.gameObject.SetActive (false);
					
					saoDangTime.text = sdCountTime.ToString () + "/" + sdTotleTime.ToString ();
				}
				
				else
				{	
					saoDangFinish.gameObject.SetActive (true);
					saoDangTime.transform.parent.gameObject.SetActive (false);
				}
				
				if (tiaoZhanNum == 0)
				{
					fightBtn.SetActive (false);
				}
				
				if (vipLevel == 0)
				{
					//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,55,0);
					
					VIP_DesLabel.gameObject.SetActive (true);
					VIP_DesLabel.text = "VIP每日赠送更多次数";
					
					if (tiaoZhanNum == 0)
					{
						saoDangBtn1.SetActive (false);
					}
					else 
					{
						if (sdCountTime == 0)
						{
							saoDangBtn1.SetActive (false);
						}
					}
				}
				
				else if (vipLevel > 0)
				{
					if (vipLevel < 4)
					{
						//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,55,0);
						
						VIP_DesLabel.gameObject.SetActive (true);
						VIP_DesLabel.text = "VIP4以上可连续扫荡";
						
						if (tiaoZhanNum == 0)
						{
							saoDangBtn1.SetActive (false);
						}
						else 
						{
							if (sdCountTime == 0)
							{
								saoDangBtn1.SetActive (false);
							}
						}
					}
					
					else
					{
						//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,77,0);
						
						VIP_DesLabel.gameObject.SetActive (false);
						
						if (tiaoZhanNum == 0)
						{
							saoDangBtn1.SetActive (false);
							saoDangBtn3.SetActive (false);
							sdBtnBg3.SetActive (true);
						}
						
						else
						{
							if (sdCountTime == 0)
							{
								saoDangBtn1.SetActive (false);
								saoDangBtn3.SetActive (false);
								sdBtnBg3.SetActive (true);
							}
							
							else if (sdCountTime > 0 && sdCountTime < 3)
							{
								saoDangBtn1.SetActive (true);
								saoDangBtn3.SetActive (false);
								sdBtnBg3.SetActive (true);
							}
							
							else if (sdCountTime >= 3)
							{
								if (tiaoZhanNum > 0 && tiaoZhanNum < 3)
								{
									saoDangBtn1.SetActive (true);
									saoDangBtn3.SetActive (false);
									sdBtnBg3.SetActive (true);
								}
								else if (tiaoZhanNum == 3)
								{
									saoDangBtn1.SetActive (true);
									saoDangBtn3.SetActive (true);
									sdBtnBg3.SetActive (false);
								}
							}
						}
					}
				}
			}

			else 
			{
				notCrossDesObj.SetActive (true);
				crossDesObj.SetActive (false);
			}
		}

		else
		{
			notCrossIcon.SetActive (true);
			crossIcon.SetActive (false);
			
			notCrossDesObj.SetActive (true);
			crossDesObj.SetActive (false);
		}
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			if (p_message.m_protocol_index == ProtoIndexes.S_PVE_Reset_CQ)//重置请求返回
			{
				//tiaoZhanNum = 3;
				//Debug.Log("重置请求返回了 ");
				//ShowInfo (true);
				fightBtn.SetActive (true);
				PveLevelUImaneger.mPveLevelUImaneger.sendLevelDrop(levelInfo.guanQiaId);
				return true;
			}
		}

		return false;
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
