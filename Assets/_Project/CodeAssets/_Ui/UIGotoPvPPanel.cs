using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIGotoPvPPanel : MYNGUIPanel 
{
	public List<UIGotoPvPData> m_listUIGotoPvPData = new List<UIGotoPvPData>();
	public UIGotoPvPData m_UIGotoPvPData;
	public UILabel m_labelTile;

	public void setData(string data, string tile)
	{
		gameObject.SetActive(true);
		m_labelTile.text = tile;
		int num = 0;
		//0普通1传奇
		for(int i = 0; i < m_listUIGotoPvPData.Count; i ++)
		{
			GameObject.Destroy(m_listUIGotoPvPData[i].gameObject);
		}
		m_listUIGotoPvPData = new List<UIGotoPvPData>();
		while(true)
		{
			string temp = Global.NextCutting(ref data, "#");
			GameObject tempObj = GameObject.Instantiate(m_UIGotoPvPData.gameObject);
			tempObj.SetActive(true);
			tempObj.name = "Enemt" + num;
			tempObj.transform.parent = m_UIGotoPvPData.gameObject.transform.parent;
			tempObj.transform.localScale = Vector3.one;
			tempObj.transform.localPosition = new Vector3(-108 + ((num % 2) * 214), 64 - (64 * (num / 2)), 0);
			UIGotoPvPData tempEnemtData = tempObj.GetComponent<UIGotoPvPData>();
			num ++;
			tempEnemtData.m_iGuankaType = int.Parse(Global.NextCutting(ref temp, ":"));
			tempEnemtData.m_iID = int.Parse(temp);
			string tempName;
			if(tempEnemtData.m_iGuankaType == 0)
			{
				tempEnemtData.m_spriteChuanqi.gameObject.SetActive(false);
				PveTempTemplate PveTempTemplate = PveTempTemplate.GetPveTemplateGuanQiaId(tempEnemtData.m_iID);
				tempName = NameIdTemplate.getNameIdTemplateByNameId(PveTempTemplate.smaName).Name;
				tempEnemtData.m_iBigID = PveTempTemplate.bigId;
				Debug.Log(MainCityUI.MapCurrentInfo.commonId);
				Debug.Log(MainCityUI.MapCurrentInfo.chuanQiId);
				if(tempEnemtData.m_iID > MainCityUI.MapCurrentInfo.commonId)
				{
					tempEnemtData.m_spriteBack.gameObject.SetActive(true);
					tempEnemtData.m_labelWeikaifang.gameObject.SetActive(true);
					tempEnemtData.m_BoxCollider.enabled = false;
				}
				else
				{
					tempEnemtData.m_spriteBack.gameObject.SetActive(false);
					tempEnemtData.m_labelWeikaifang.gameObject.SetActive(false);
					tempEnemtData.m_BoxCollider.enabled = true;
				}
			}
			else
			{
				tempEnemtData.m_spriteChuanqi.gameObject.SetActive(true);
				LegendPveTemplate mLg_PveTempTemplate = LegendPveTemplate.GetlegendPveTemplate_By_id(tempEnemtData.m_iID);
				tempName = NameIdTemplate.getNameIdTemplateByNameId(mLg_PveTempTemplate.smaName).Name;
				tempEnemtData.m_iBigID = mLg_PveTempTemplate.bigId;
				if(tempEnemtData.m_iID > MainCityUI.MapCurrentInfo.chuanQiId)
				{
					tempEnemtData.m_spriteBack.gameObject.SetActive(true);
					tempEnemtData.m_labelWeikaifang.gameObject.SetActive(true);
					tempEnemtData.m_BoxCollider.enabled = false;
				}
				else
				{
					tempEnemtData.m_spriteBack.gameObject.SetActive(false);
					tempEnemtData.m_labelWeikaifang.gameObject.SetActive(false);
					tempEnemtData.m_BoxCollider.enabled = true;
				}
			}
			tempEnemtData.m_labelName.text = tempName;
			m_listUIGotoPvPData.Add(tempEnemtData);
			if(data == "")
			{
				break;
			}
		}
//		0:900000#0:9000014
	}
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Enemt") != -1)
		{
			int index = int.Parse(ui.name.Substring(5, 1));
			if(m_listUIGotoPvPData[index].m_iGuankaType  == 0)
			{
				CityGlobalData.PT_Or_CQ = true;
			}
			else
			{
				CityGlobalData.PT_Or_CQ = false;
			}

			EnterGuoGuanmap.Instance().ShouldOpen_id = m_listUIGotoPvPData[index].m_iID;
			
			EnterGuoGuanmap.EnterPveUI (m_listUIGotoPvPData[index].m_iBigID);
			if (FreshGuide.Instance().IsActive(100404) && TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
		}
		else if(ui.name.IndexOf("Close") != -1)
		{
			gameObject.SetActive(false);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}
