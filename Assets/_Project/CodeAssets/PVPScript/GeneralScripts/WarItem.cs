using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarItem : MonoBehaviour {

	private SimpleInfo simpleInfo;

	public UISprite icon;
	public UILabel numLabel;

	public GameObject lockObj;
	public GameObject redObj;

	private bool isOpen;
	private bool isShowRed;
	public bool GetShowRed
	{
		get{return isShowRed;}
	}

	public EventHandler itemHandler;

	/// <summary>
	/// Ins it war item.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	public void InItWarItem (SimpleInfo tempInfo)
	{
		simpleInfo = tempInfo;

		isOpen = FunctionOpenTemp.IsHaveID (tempInfo.functionId);

		lockObj.SetActive (!isOpen);

		icon.spriteName = "Function_" + tempInfo.functionId;
	
//		Debug.Log( "InitWarItem: " + simpleInfo.functionId + " - " + isOpen + " - " + icon.spriteName );

		icon.color = isOpen ? Color.white : Color.black;

		string name = FunctionOpenTemp.GetTemplateById (tempInfo.functionId).Des.Split ('-')[1];

		if (isOpen)
		{
			switch (tempInfo.functionId)
			{
			case 310:
				numLabel.text = tempInfo.num1 > 0 ? "剩余 " + MyColorData.getColorString (4,tempInfo.num1 + "/" + tempInfo.num2) : MyColorData.getColorString (4,"进入多人战场");
				break;
			case 211:
				numLabel.text = "剩余 "  + MyColorData.getColorString (tempInfo.num1 > 0 ? 4 : 5,tempInfo.num1 + "/" + tempInfo.num2);
				break;
			case 300100:
				numLabel.text = "剩余 " + MyColorData.getColorString (tempInfo.num1 > 0 ? 4 : 5,tempInfo.num1 + "/" + tempInfo.num2);
				break;
			default:
				break;
			}
		}
		else
		{
			numLabel.text = FunctionOpenTemp.GetTemplateById (tempInfo.functionId).Level > 0 ? FunctionOpenTemp.GetTemplateById (tempInfo.functionId).Level + "级开放" : "即将开放";//FunctionOpenTemp.GetTemplateById (tempInfo.functionId).m_sNotOpenTips;
		}

		CheckRedPoint ();
		itemHandler.m_click_handler -= WarItemHandlerClickBack;
		itemHandler.m_click_handler += WarItemHandlerClickBack;
	}

	void WarItemHandlerClickBack (GameObject obj)
	{
		if (!isOpen)
		{
			ClientMain.m_UITextManager.createText (FunctionOpenTemp.GetTemplateById (simpleInfo.functionId).m_sNotOpenTips);
			return;
		}

		switch (simpleInfo.functionId)
		{
		case 310://行镖

			if (ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION) || FunctionOpenTemp.GetWhetherContainID(310))
			{
				PlayerSceneSyncManager.Instance.EnterCarriage();
			}
			else
			{
				FunctionWindowsCreateManagerment.ShowUnopen(310);
			}

			break;
		case 211://掠夺

			PlunderData.Instance.OpenPlunder ();

			break;
		case 300100:

			SportData.Instance.OpenSport ();

			break;
		default:
			break;
		}

//		WarPage.warPage.WarCloseHandlerClickBack (gameObject);
	}

	public void CheckRedPoint ()
	{
		switch (simpleInfo.functionId)
		{
		case 310:
			redObj.SetActive (isOpen && (FunctionOpenTemp.IsShowRedSpotNotification (312) || FunctionOpenTemp.IsShowRedSpotNotification (313) || FunctionOpenTemp.IsShowRedSpotNotification (315)) ? true : false);
			break;
		case 211:
			redObj.SetActive (isOpen && (FunctionOpenTemp.IsShowRedSpotNotification (215) || FunctionOpenTemp.IsShowRedSpotNotification (220)) ? true : false);
			break;
		case 300100:
			redObj.SetActive (isOpen && (FunctionOpenTemp.IsShowRedSpotNotification (300103) || FunctionOpenTemp.IsShowRedSpotNotification (300100)) ? true : false);
			break;
		default:
			break;
		}
	}
}
