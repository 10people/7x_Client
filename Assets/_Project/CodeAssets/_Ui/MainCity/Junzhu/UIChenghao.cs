using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class UIChenghao : MYNGUIPanel 
{
	public UIJunZhu m_UIJunZhu;
	public UISprite m_UISpriteChenghao;
	public UIChenghaoList m_UIChenghaoCopy;
	public UILabel m_UILbaelYulan;
	private List<UIChenghaoList> m_UIChenghaoList = new List<UIChenghaoList>();
	private ChengHaoList m_chenghaoData;
	private int m_iSelectIndex = -1;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_iSelectIndex != -1)
		{
			if(m_chenghaoData != null && m_chenghaoData.list.Count > m_iSelectIndex && JunZhuData.m_iChenghaoID != m_chenghaoData.list[m_iSelectIndex].id)
			{
				m_UILbaelYulan.gameObject.SetActive(true);
				m_UISpriteChenghao.spriteName = m_chenghaoData.list[m_iSelectIndex].id + "";
				m_UISpriteChenghao.gameObject.SetActive(true);
			}
			else
			{
				m_UILbaelYulan.gameObject.SetActive(false);
				if(JunZhuData.m_iChenghaoID != -1)
				{
					m_UISpriteChenghao.spriteName = JunZhuData.m_iChenghaoID + "";
					m_UISpriteChenghao.gameObject.SetActive(true);
				}
				else
				{
					m_UISpriteChenghao.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			m_UILbaelYulan.gameObject.SetActive(false);
		}
	}
	
	public override void MYClick(GameObject ui)
	{
//		Debug.Log(ui.name);

		if(ui.name.IndexOf("ChenghaoIndex") != -1)
		{
			if(m_iSelectIndex != -1)
			{
				m_UIChenghaoList[m_iSelectIndex].m_SpriteSelect.gameObject.SetActive(false);
			}
			m_iSelectIndex = int.Parse(ui.name.Substring(13, ui.name.Length - 13));
			m_UIChenghaoList[m_iSelectIndex].m_SpriteSelect.gameObject.SetActive(true);
		}
		else if(ui.name.IndexOf("OnChenghao") != -1)
		{
//			ErrorMessage
			int selectIndex = int.Parse(ui.name.Substring(10, ui.name.Length - 10));
			Debug.Log(selectIndex);
			if(m_chenghaoData.list[selectIndex].state == 0)
			{
				if(m_chenghaoData.myPoint >= m_chenghaoData.list[selectIndex].price)
				{
					Global.ScendID(ProtoIndexes.C_DuiHuan_CHENGHAO, m_chenghaoData.list[selectIndex].id);
				}
			}
			else
			{
				if(JunZhuData.m_iChenghaoID == m_chenghaoData.list[selectIndex].id)
				{
					JunZhuData.m_iChenghaoID = -1;
					m_UIChenghaoList[selectIndex].m_LabelButton.text = "装上";
					m_UIChenghaoList[selectIndex].m_SpriteNewOrIng.gameObject.SetActive(false);
					m_UISpriteChenghao.gameObject.SetActive(false);
				}
				else
				{
					if(JunZhuData.m_iChenghaoID != -1)
					{
						for(int i = 0; i < m_chenghaoData.list.Count; i ++)
						{
							if(m_chenghaoData.list[i].id == JunZhuData.m_iChenghaoID)
							{
								m_UIChenghaoList[i].m_SpriteNewOrIng.gameObject.SetActive(false);
							}
						}
					}
					JunZhuData.m_iChenghaoID = m_chenghaoData.list[selectIndex].id;
					m_UIChenghaoList[selectIndex].m_LabelButton.text = "卸下";
					m_UIChenghaoList[selectIndex].m_SpriteNewOrIng.gameObject.SetActive(true);
					m_UIChenghaoList[selectIndex].m_SpriteNewOrIng.spriteName = "ing";
					m_UISpriteChenghao.gameObject.SetActive(true);
					m_UISpriteChenghao.spriteName = JunZhuData.m_iChenghaoID + "";
				}
				PlayerNameManager.UpdateSelfName();
				Global.ScendID(ProtoIndexes.C_USE_CHENG_HAO, JunZhuData.m_iChenghaoID);
			}
//			m_iSelectIndex = int.Parse(ui.name.Substring(10, ui.name.Length - 10));
		}
		else if(ui.name.IndexOf("chenghaoback") != -1)
		{
			m_iSelectIndex = -1;

			if(JunZhuData.m_iChenghaoID != -1)
			{
				m_UISpriteChenghao.spriteName = JunZhuData.m_iChenghaoID + "";
				m_UISpriteChenghao.gameObject.SetActive(true);
			}
			else
			{
				m_UISpriteChenghao.gameObject.SetActive(false);
			}

			for(int i = 0; i < m_UIChenghaoList.Count; i ++)
			{
				GameObject.Destroy(m_UIChenghaoList[i].gameObject);
			}
			m_UIChenghaoList = new List<UIChenghaoList>();
			m_UIJunZhu.m_PlayerRight.SetActive(true);
			m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
			m_UIJunZhu.m_UILbaelHeroName.gameObject.SetActive(true);
			m_UILbaelYulan.gameObject.SetActive(false);
//			m_UIJunZhu.m_ZhuangBeiIcon.SetActive(true);
			gameObject.SetActive(false);
		}
		else if(ui.name.IndexOf("Select") != -1)
		{
			if(ui.name.IndexOf("Select0") == -1)
			{
				ClientMain.m_UITextManager.createText("暂未开启");
			}
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
	
	public void setData(ChengHaoList chenghaoData)
	{
		m_chenghaoData = chenghaoData;
		for(int i = 0; i < m_chenghaoData.list.Count; i ++)
		{
			//				Debug.Log(m_chenghaoData.list[i].id);
			UIChenghaoList tempChenghaoData;
			if(m_UIChenghaoList.Count <= i)
			{
				tempChenghaoData = GameObject.Instantiate(m_UIChenghaoCopy);
				tempChenghaoData.gameObject.SetActive(true);
				tempChenghaoData.transform.parent = m_UIChenghaoCopy.transform.parent;
				tempChenghaoData.transform.localScale = Vector3.one;
				tempChenghaoData.transform.localPosition = new Vector3(61f, 41f - (95f * i), 0);
				tempChenghaoData.gameObject.name = "ChenghaoIndex" + i;
				tempChenghaoData.m_LabelName.text = ChenghaoTemplate.GetChenghaoName(m_chenghaoData.list[i].id);
				tempChenghaoData.m_SpriteButton.name = "OnChenghao" + m_chenghaoData.list[i].id;
				string tempArr = "攻击+" + m_chenghaoData.list[i].gongJi + "\n" + "防御+" + m_chenghaoData.list[i].fangYu + "\n" + "生命+" + m_chenghaoData.list[i].shengMing;
				tempChenghaoData.m_LabelArr.text = tempArr;
				tempChenghaoData.m_UIButton.gameObject.name = "OnChenghao" + i;
			}
			else
			{
				tempChenghaoData = m_UIChenghaoList[i];
			}
			switch(m_chenghaoData.list[i].state)
			{
			case 0:
				tempChenghaoData.m_ObjExp.SetActive(true);
				tempChenghaoData.m_SpriteExp.SetDimensions(Global.getBili(100, m_chenghaoData.myPoint, m_chenghaoData.list[i].price) ,15);
				tempChenghaoData.m_LabelExp.text = m_chenghaoData.myPoint + "/" + m_chenghaoData.list[i].price;
				long day = m_chenghaoData.list[i].leftSec / 3600 / 24;
				tempChenghaoData.m_LabelTime.text = "有效期" + day + "天";
				tempChenghaoData.m_LabelButton.text = "兑 换";
				if(m_chenghaoData.myPoint >= m_chenghaoData.list[i].price)
				{
					tempChenghaoData.m_SpriteButton.color = Color.white;
					tempChenghaoData.m_UILabelType.setType(10);
				}
				else
				{
					tempChenghaoData.m_SpriteButton.color = Color.gray;
					tempChenghaoData.m_UILabelType.setType(101);
					tempChenghaoData.m_UIButton.enabled = false;
				}
				break;
			case 'U':
				tempChenghaoData.m_SpriteNewOrIng.spriteName = "ing";
				tempChenghaoData.m_SpriteNewOrIng.gameObject.SetActive(true);
				tempChenghaoData.m_ObjExp.SetActive(false);
				Debug.Log(m_chenghaoData.list[i].leftSec);
				TimeLabelHelper.Instance.setTimeLabel(tempChenghaoData.m_LabelTime, "00:00", (int)m_chenghaoData.list[i].leftSec, EndTime, "后过期");
				tempChenghaoData.m_LabelButton.text = "卸下";
				break;
			case 'G':
				Debug.Log(m_chenghaoData.list[i].leftSec);
				tempChenghaoData.m_ObjExp.SetActive(false);
				TimeLabelHelper.Instance.setTimeLabel(tempChenghaoData.m_LabelTime, "00:00", (int)m_chenghaoData.list[i].leftSec, EndTime, "后过期");
				tempChenghaoData.m_LabelButton.text = "装上";
				break;
			}
			if(m_UIChenghaoList.Count <= i)
			{
				m_UIChenghaoList.Add(tempChenghaoData);
			}
		}
		gameObject.SetActive(true);
	}

	public void EndTime()
	{
		Global.ScendNull(ProtoIndexes.C_LIST_CHENG_HAO);
	}

	public bool isHaveNew(int id)
	{
		for(int i = 0; i < Global.m_NewChenghao.Count; i ++)
		{
			if(id == Global.m_NewChenghao[i])
			{
				return true;
			}
		}
		return false;
	}
}