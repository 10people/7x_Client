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
				m_UIChenghaoList[m_iSelectIndex].m_SpriteButton.SetActive(false);
				m_UIChenghaoList[m_iSelectIndex].m_SpriteSelect.gameObject.SetActive(false);

			}
			m_iSelectIndex = int.Parse(ui.name.Substring(13, ui.name.Length - 13));
			if(m_chenghaoData.list[m_iSelectIndex].state == 0)
			{
				return;
			}
			m_UIChenghaoList[m_iSelectIndex].m_SpriteButton.SetActive(true);
			m_UIChenghaoList[m_iSelectIndex].m_SpriteSelect.gameObject.SetActive(true);
			if(JunZhuData.m_iChenghaoID == m_chenghaoData.list[m_iSelectIndex].id)
			{
				m_UIChenghaoList[m_iSelectIndex].m_LabelButton.text = "卸下";
			}
			else
			{
				m_UIChenghaoList[m_iSelectIndex].m_LabelButton.text = "装上";
			}

			for(int i = 0; i < Global.m_NewChenghao.Count; i ++)
			{
				if(Global.m_NewChenghao[i] == m_chenghaoData.list[m_iSelectIndex].id)
				{
					m_UIChenghaoList[m_iSelectIndex].m_SpriteNewOrIng.gameObject.SetActive(false);
					Global.m_NewChenghao.RemoveAt(i);
					string saveString = "";
					for(int q = 0; q < Global.m_NewChenghao.Count; q ++)
					{
						saveString += Global.m_NewChenghao[q] + ",";
					}
					PlayerPrefs.SetString( ConstInGame.CONST_NEW_CHENGHAO + JunZhuData.Instance().m_junzhuInfo.id, saveString );
					PlayerPrefs.Save();
					if(Global.m_NewChenghao.Count == 0)
					{
						MainCityUI.SetRedAlert(500015, false);
					}
					break;
				}
			}
		}
		else if(ui.name.IndexOf("OnChenghao") != -1)
		{
//			ErrorMessage
			if(JunZhuData.m_iChenghaoID == m_chenghaoData.list[m_iSelectIndex].id)
			{
				JunZhuData.m_iChenghaoID = -1;
				m_UIChenghaoList[m_iSelectIndex].m_LabelButton.text = "装上";
				m_UIChenghaoList[m_iSelectIndex].m_SpriteNewOrIng.gameObject.SetActive(false);
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
				JunZhuData.m_iChenghaoID = m_chenghaoData.list[m_iSelectIndex].id;
				m_UIChenghaoList[m_iSelectIndex].m_LabelButton.text = "卸下";
				m_UIChenghaoList[m_iSelectIndex].m_SpriteNewOrIng.gameObject.SetActive(true);
				m_UIChenghaoList[m_iSelectIndex].m_SpriteNewOrIng.spriteName = "ing";
				m_UISpriteChenghao.gameObject.SetActive(true);
				m_UISpriteChenghao.spriteName = JunZhuData.m_iChenghaoID + "";
			}
			PlayerNameManager.UpdateSelfName();
			Global.ScendID(ProtoIndexes.C_USE_CHENG_HAO, JunZhuData.m_iChenghaoID);
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
			m_UIJunZhu.m_PlayerLeft.gameObject.transform.localPosition = new Vector3(-1,0,0);
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
		if(!gameObject.activeSelf)
		{
			for(int i = 0; i < m_chenghaoData.list.Count; i ++)
			{
//				Debug.Log(m_chenghaoData.list[i].id);

				UIChenghaoList tempChenghaoData = GameObject.Instantiate(m_UIChenghaoCopy);
				tempChenghaoData.gameObject.SetActive(true);
				tempChenghaoData.transform.parent = m_UIChenghaoCopy.transform.parent;
				tempChenghaoData.transform.localScale = Vector3.one;
				tempChenghaoData.transform.localPosition = new Vector3(61f, 41f - (95f * i), 0);
				tempChenghaoData.gameObject.name = "ChenghaoIndex" + i;
				tempChenghaoData.m_LabelDis.text = ChenghaoTemplate.GetChenghaoDis(m_chenghaoData.list[i].id);
				tempChenghaoData.m_LabelName.text = ChenghaoTemplate.GetChenghaoName(m_chenghaoData.list[i].id);
				tempChenghaoData.m_SpriteButton.name = "OnChenghao" + m_chenghaoData.list[i].id;

				switch(m_chenghaoData.list[i].state)
				{
				case 0:
					tempChenghaoData.m_SpriteBG.color = Color.grey;
					break;
				case 'U':
					tempChenghaoData.m_SpriteNewOrIng.spriteName = "ing";
					tempChenghaoData.m_SpriteNewOrIng.gameObject.SetActive(true);
					break;
				case 'G':
					break;
				}
				if(isHaveNew(m_chenghaoData.list[i].id))
				{
					tempChenghaoData.m_SpriteNewOrIng.spriteName = "newchenghao";
					tempChenghaoData.m_SpriteNewOrIng.gameObject.SetActive(true);
				}
				m_UIChenghaoList.Add(tempChenghaoData);
			}
			gameObject.SetActive(true);
		}
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