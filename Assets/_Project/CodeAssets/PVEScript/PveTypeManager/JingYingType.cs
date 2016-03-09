using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JingYingType : MonoBehaviour {

	public GameObject notCrossIcon;//未通关时显示的icon
	public GameObject crossIcon;//通关时显示的icon

	public UISprite dun;//盾任务显示的icon
	public UISprite jian;//剑任务显示的icon

	public UISprite WinType;//剑任务显示的icon

	public List<UISprite> starSpriteList = new List<UISprite> ();//显示的得星list
	
	public GameObject notCrossDesObj;//未通关时显示的扫荡描述
	public GameObject crossDesObj;//通关时显示的扫荡描述
	
	public UILabel saoDangTime;//扫荡次数label
	public GameObject saoDangFinish;//扫荡次数用尽显示的label

	public GameObject saoDangBtn1;//扫荡一次按钮
	public GameObject saoDangBtn10;//连续扫荡按钮
	public GameObject sdBtnBg10;//连续扫荡按钮置灰背景
	
	public UILabel VIP_DesLabel;//vip扫荡描述label

	private int vipLevel ;//vip等级

	public UILabel desLabel;//描述label

	public UILabel Person_name;//描述label

	public GameObject enemyListObj;
	public GameObject awardListObj;


	//获得关卡以及秘宝信息
	public void GetNeedInfo (Level tempLevelInfo,GuanQiaInfo mGuanQia)
	{
		ShowInfo (tempLevelInfo,mGuanQia);

//		UICreateEnemy enemyCreate = enemyListObj.GetComponent<UICreateEnemy> ();
//		enemyCreate.InItEnemyList (tempLevelInfo.type);
//
//		UICreateDropthings awardCreate = awardListObj.GetComponent<UICreateDropthings> ();
//
//		awardCreate.mLevl = tempLevelInfo;
//
//		awardCreate.GetAward (tempLevelInfo.type);
	}

	void ShowInfo (Level levelInfo,GuanQiaInfo m_GuanQia) 
	{
		int guanqiaId = levelInfo.guanQiaId;

		int starnum = 0;//litter_Lv.starNum;

		if(levelInfo.s_pass)
		{
			notCrossIcon.SetActive (false);

			crossIcon.SetActive (true);

//			Debug.Log("levelInfo.win_Level = "+levelInfo.win_Level);

			if(levelInfo.win_Level == 1)
			{
				WinType.spriteName = "X_Victoyr";
			}
			if(levelInfo.win_Level == 2)
			{
				WinType.spriteName = "L_Victoyr";
			}
			if(levelInfo.win_Level == 3)
			{
				WinType.spriteName = "Prefect_Victor";
			}

			//Debug.Log ("starnum"+starnum);
			for(int j = 0 ; j < levelInfo.starInfo.Count; j++)
			{
				if(levelInfo.starInfo[j].finished)
				{
					starnum += 1;
				}
			}

//			Debug.Log ("starnum = "+starnum);

			for(int i = 0; i < starnum; i++)
			{
				starSpriteList[i].spriteName = "BigStar";
				
				starSpriteList[i].gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
			}
		}
		else
		{
			notCrossIcon.SetActive (true);

			crossIcon.SetActive (false);
		}

		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (guanqiaId);
		
		string descStr =  DescIdTemplate.GetDescriptionById (m_item.smaDesc);
		if(descStr == null )
		{
			descStr = "表里数据为空，#请Desc填表";
		}
		char[] separator = new char[] { '#' };

		string[] s = descStr.Split (separator);

		desLabel.text = s[0];

		if(s.Length > 1)
		{
			Person_name.text = s [1];
		}
		vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;

		if (levelInfo.s_pass)
		{
			//得星数为3时
			if (levelInfo.win_Level >= 3)
			{
				notCrossDesObj.SetActive (false);

				crossDesObj.SetActive (true);

				int sdTotleTime =  m_GuanQia.jySaoDangDayTimes;//扫荡总次数
                    
				int sdCountTime = sdTotleTime -m_GuanQia.jySaoDangUsedTimes;
				//Debug.Log ("tempLevelInfo.sdTotleTime  = "+sdTotleTime);
				//Debug.Log ("tempLevelInfo.sdCountTime  = "+sdCountTime);
				//扫荡剩余次数cqSaoDangUsedTimes=15;//
				
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
				
				if (vipLevel == 0)
				{
					//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,55,0);
					
					VIP_DesLabel.gameObject.SetActive (true);
					VIP_DesLabel.text = "VIP每日赠送更多次数";
					
					if (sdCountTime == 0)
					{
						saoDangBtn1.SetActive (false);
					}
				}
				
				else if (vipLevel > 0)
				{
					int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(13);
					if (vipLevel < viplv)
					{
						//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,55,0);
						
						VIP_DesLabel.gameObject.SetActive (true);
						VIP_DesLabel.text = "VIP"+viplv.ToString()+"以上可连续扫荡";
						
						if (sdCountTime == 0)
						{
							saoDangBtn1.SetActive (false);
						}
					}
					
					else
					{
						//saoDangTime.transform.parent.gameObject.transform.localPosition = new Vector3 (65,77,0);
						
						VIP_DesLabel.gameObject.SetActive (false);
						
						if (sdCountTime == 0)
						{
							saoDangBtn1.SetActive (false);
							saoDangBtn10.SetActive (false);
							sdBtnBg10.SetActive (true);
						}
						
						else if (sdCountTime > 0 && sdCountTime < 10)
						{
							saoDangBtn1.SetActive (true);
							saoDangBtn10.SetActive (false);
							sdBtnBg10.SetActive (true);
						}
						
						else if (sdCountTime >= 10)
						{
							saoDangBtn1.SetActive (true);
							saoDangBtn10.SetActive (true);
							sdBtnBg10.SetActive (false);
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
			notCrossDesObj.SetActive (true);
			crossDesObj.SetActive (false);
		}
	}
}
