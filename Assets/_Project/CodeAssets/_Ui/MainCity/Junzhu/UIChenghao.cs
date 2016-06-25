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
	public UIChenghaoList m_UIChenghaoCopy;
	public GameObject m_objDuihuan;
	private List<UIChenghaoList> m_UIChenghaoList = new List<UIChenghaoList>();
	private ChengHaoList m_chenghaoData;
	private int m_iSelectIndex = -1;
	private int m_iSelectPage = 0;
	private bool m_isDuihuang = false;
	public List<UISprite> m_listSpritePage;
	public List<GameObject> m_listRed = new List<GameObject>();
	// Use this for initialization
	void Start () 
	{
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objDuihuan, EffectTemplate.getEffectTemplateByEffectId( 100180 ).path);
		Global.ScendNull(ProtoIndexes.C_GET_CUR_CHENG_HAO);
	}
	private int m_iNum = 0;
	// Update is called once per frame
	void Update ()
	{
		if(m_objDuihuan.activeSelf)
		{
			m_iNum ++;
			if(m_iNum == 66)
			{
				m_objDuihuan.SetActive(false);
				m_iNum = 0;
			}
		}
		if(m_iSelectIndex != -1)
		{
			if(m_chenghaoData != null && m_chenghaoData.list.Count > m_iSelectIndex && JunZhuData.m_iChenghaoID != m_chenghaoData.list[m_iSelectIndex].id)
			{
				m_UIJunZhu.m_UILabelYulan.gameObject.SetActive(true);
				m_UIJunZhu.m_UIChenghaoSprite.spriteName = m_chenghaoData.list[m_iSelectIndex].id + "";
				m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(true);
				m_UIJunZhu.m_UIChenghaoSprite.spriteName = "" + m_chenghaoData.list[m_iSelectIndex].id;
				for(int i = 0; i < 4; i ++)
				{
					m_UIJunZhu.m_listChenghaoEff[i].SetActive(i == ChenghaoTemplate.GetChenghaoColor(m_chenghaoData.list[m_iSelectIndex].id) - 272);
				}
			}
			else
			{
				m_UIJunZhu.m_UILabelYulan.gameObject.SetActive(false);
				if(JunZhuData.m_iChenghaoID != -1)
				{
					m_UIJunZhu.m_UIChenghaoSprite.spriteName = JunZhuData.m_iChenghaoID + "";
					m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(true);
					for(int i = 0; i < 4; i ++)
					{
						m_UIJunZhu.m_listChenghaoEff[i].SetActive(i == ChenghaoTemplate.GetChenghaoColor(JunZhuData.m_iChenghaoID) - 272);
					}
				}
				else
				{
					m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			m_UIJunZhu.m_UILabelYulan.gameObject.SetActive(false);
			if(JunZhuData.m_iChenghaoID == -1)
			{
				m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
			}
			else
			{
				for(int i = 0; i < 4; i ++)
				{
					m_UIJunZhu.m_listChenghaoEff[i].SetActive(i == ChenghaoTemplate.GetChenghaoColor(JunZhuData.m_iChenghaoID) - 272);
				}
			}
		}
		m_listRed[0].SetActive(FunctionOpenTemp.GetTemplateById(510015).m_show_red_alert);
		m_listRed[1].SetActive(FunctionOpenTemp.GetTemplateById(520015).m_show_red_alert);
	}
	
	public override void MYClick(GameObject ui)
	{
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
			if(m_chenghaoData.list[selectIndex].state == 0)
			{
				int tempCur = 0;
				if(m_iSelectPage == 0)
				{
					tempCur = m_chenghaoData.myPoint;
				}
				else
				{
					tempCur = m_chenghaoData.weiWang;
				}
				if(tempCur >= m_chenghaoData.list[selectIndex].price)
				{
					m_isDuihuang = true;
					Global.ScendID(ProtoIndexes.C_DuiHuan_CHENGHAO, m_chenghaoData.list[selectIndex].id);
				}
				else
				{
					if(m_iSelectPage == 0)
					{
						Global.CreateFunctionIcon(2201);
					}
					else
					{
						Global.CreateFunctionIcon(801);
					}
				}
			}
			else
			{
				for(int i = 0; i < m_chenghaoData.list.Count; i ++)
				{
					switch(m_chenghaoData.list[i].state)
					{
					case 0:
						m_UIChenghaoList[i].m_LabelButton.text = "兑 换";
						break;
					case 'U':
						m_UIChenghaoList[i].m_LabelButton.text = "装 上";
						break;
					case 'G':
						m_UIChenghaoList[i].m_LabelButton.text = "装 上";
						break;
					}
				}
				if(JunZhuData.m_iChenghaoID == m_chenghaoData.list[selectIndex].id)
				{
					JunZhuData.m_iChenghaoID = -1;
					m_UIChenghaoList[selectIndex].m_LabelButton.text = "装 上";
					m_UIChenghaoList[selectIndex].m_SpriteNewOrIng.gameObject.SetActive(false);
					m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
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
					m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(true);
					m_UIJunZhu.m_UIChenghaoSprite.spriteName = JunZhuData.m_iChenghaoID + "";
				}
				PlayerNameManager.UpdateSelfName();
				Global.ScendID(ProtoIndexes.C_USE_CHENG_HAO, JunZhuData.m_iChenghaoID);
			}
//			m_iSelectIndex = int.Parse(ui.name.Substring(10, ui.name.Length - 10));
		}
		else if(ui.name.IndexOf("chenghaoback") != -1)
		{
			m_iSelectPage = 0;
			m_iSelectIndex = -1;

			if(JunZhuData.m_iChenghaoID != -1)
			{
				m_UIJunZhu.m_UIChenghaoSprite.spriteName = JunZhuData.m_iChenghaoID + "";
				m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(true);
				for(int i = 0; i < 4; i ++)
				{
					m_UIJunZhu.m_listChenghaoEff[i].SetActive(i == ChenghaoTemplate.GetChenghaoColor(JunZhuData.m_iChenghaoID) - 272);
				}
			}
			else
			{
				m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
			}

			for(int i = 0; i < m_UIChenghaoList.Count; i ++)
			{
				GameObject.Destroy(m_UIChenghaoList[i].gameObject);
			}
			m_UIChenghaoList = new List<UIChenghaoList>();
			m_UIJunZhu.m_PlayerRight.SetActive(true);
//			m_UIJunZhu.m_UIChenghaoSprite.gameObject.SetActive(false);
			m_UIJunZhu.m_UILbaelHeroName.gameObject.SetActive(true);
			m_UIJunZhu.m_UILabelYulan.gameObject.SetActive(false);
//			m_UIJunZhu.m_ZhuangBeiIcon.SetActive(true);
			gameObject.SetActive(false);
		}
		else if(ui.name.IndexOf("Select") != -1)
		{
			if(ui.name.IndexOf("Select2") != -1)
			{
				ClientMain.m_UITextManager.createText("暂未开启");
			}
			else
			{
				m_iSelectPage = int.Parse(ui.name.Substring(6, 1));
           		Global.ScendID(ProtoIndexes.C_LIST_CHENG_HAO, m_iSelectPage);
				m_iSelectIndex = -1;
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
		if(m_isDuihuang)
		{
			m_objDuihuan.SetActive(true);
			m_isDuihuang = false;
		}
		for(int i = 0; i < m_UIChenghaoList.Count; i ++)
		{
			GameObject.Destroy(m_UIChenghaoList[i].gameObject);
		}
		m_UIChenghaoList = new List<UIChenghaoList>();
		m_chenghaoData = chenghaoData;
		for(int i = 0; i < 2; i ++)
		{
			if(i == m_iSelectPage)
			{
				m_listSpritePage[i].color = Color.white;
			}
			else
			{
				m_listSpritePage[i].color = Color.gray;
			}
		}
		int duihuanMax = 0;
		int meiduihuanMax = 0;
		for(int i = 0; i < m_chenghaoData.list.Count; i ++)
		{
			//				Debug.Log(m_chenghaoData.list[i].id);
			UIChenghaoList tempChenghaoData;

			tempChenghaoData = GameObject.Instantiate(m_UIChenghaoCopy);
			tempChenghaoData.gameObject.SetActive(true);
			tempChenghaoData.transform.parent = m_UIChenghaoCopy.transform.parent;
			tempChenghaoData.transform.localScale = Vector3.one;
			tempChenghaoData.transform.localPosition = new Vector3(61f, 41f - (95f * i), 0);
			tempChenghaoData.gameObject.name = "ChenghaoIndex" + i;
			tempChenghaoData.m_LabelName.text = ChenghaoTemplate.GetChenghaoName(m_chenghaoData.list[i].id);
			tempChenghaoData.m_SpriteButton.name = "OnChenghao" + m_chenghaoData.list[i].id;
			string tempArr = "玩家对战中，加伤+" + m_chenghaoData.list[i].gongJi + "%，" + "减伤+" + m_chenghaoData.list[i].fangYu + "%";
			tempChenghaoData.m_LabelArr.text = tempArr;
			tempChenghaoData.m_UIButton.gameObject.name = "OnChenghao" + i;

			switch(m_chenghaoData.list[i].state)
			{
			case 0:
				tempChenghaoData.m_ObjExp.SetActive(true);
				int tempCur = 0;
				if(m_iSelectPage == 0)
				{
					tempCur = m_chenghaoData.myPoint;
					tempChenghaoData.m_LabelDuihua.text = "功勋兑换：";
				}
				else
				{
					tempCur = m_chenghaoData.weiWang;
					tempChenghaoData.m_LabelDuihua.text = "威望兑换：";
				}
				tempChenghaoData.m_SpriteExp.SetDimensions(Global.getBili(154, tempCur, m_chenghaoData.list[i].price) ,15);

				tempChenghaoData.m_LabelExp.text = tempCur + "/" + m_chenghaoData.list[i].price;

				long day = m_chenghaoData.list[i].leftSec / 3600 / 24;
				tempChenghaoData.m_LabelTime.text = "[FFB12A]有效期" + day + "天[-]";
				tempChenghaoData.m_LabelButton.text = "兑 换";
				if(tempCur < m_chenghaoData.list[i].price)
				{
					tempChenghaoData.m_SpriteButton.transform.parent.gameObject.GetComponent<BoxCollider>().enabled = false;
					tempChenghaoData.m_SpriteButton.color = Color.gray;
					tempChenghaoData.m_UILabelType.setType(11);
				}
				else
				{
					tempChenghaoData.m_SpriteButton.transform.parent.gameObject.GetComponent<UIButton>().enabled = true;
					tempChenghaoData.m_SpriteButton.color = Color.white;
					tempChenghaoData.m_UILabelType.setType(10);
					if(meiduihuanMax < m_chenghaoData.list[i].price)
					{
						meiduihuanMax = m_chenghaoData.list[i].price;
					}
				}
				tempChenghaoData.m_LabelYiDuiHuan.gameObject.SetActive(false);
				break;
			case 'U':
				if(m_iSelectPage == 0)
				{
					tempCur = m_chenghaoData.myPoint;
					tempChenghaoData.m_LabelDuihua.text = "功勋兑换：";
				}
				else
				{
					tempCur = m_chenghaoData.weiWang;
					tempChenghaoData.m_LabelDuihua.text = "威望兑换：";
				}
				tempChenghaoData.m_SpriteNewOrIng.spriteName = "ing";
				tempChenghaoData.m_SpriteNewOrIng.gameObject.SetActive(true);
				tempChenghaoData.m_ObjExp.SetActive(false);
				TimeLabelHelper.Instance.setTimeLabel(tempChenghaoData.m_LabelTime, "00:00", (int)m_chenghaoData.list[i].leftSec, EndTime, "后过期");
				tempChenghaoData.m_LabelButton.text = "卸 下";
				tempChenghaoData.m_LabelYiDuiHuan.gameObject.SetActive(true);
				tempChenghaoData.m_SpriteButton.transform.parent.gameObject.GetComponent<UIButton>().enabled = true;
				if(duihuanMax < m_chenghaoData.list[i].price)
				{
					duihuanMax = m_chenghaoData.list[i].price;
				}
				break;
			case 'G':
				if(m_iSelectPage == 0)
				{
					tempCur = m_chenghaoData.myPoint;
					tempChenghaoData.m_LabelDuihua.text = "功勋兑换：";
				}
				else
				{
					tempCur = m_chenghaoData.weiWang;
					tempChenghaoData.m_LabelDuihua.text = "威望兑换：";
				}
				tempChenghaoData.m_ObjExp.SetActive(false);
				TimeLabelHelper.Instance.setTimeLabel(tempChenghaoData.m_LabelTime, "00:00", (int)m_chenghaoData.list[i].leftSec, EndTime, "后过期");
				tempChenghaoData.m_LabelButton.text = "装 上";
				tempChenghaoData.m_LabelYiDuiHuan.gameObject.SetActive(true);
				tempChenghaoData.m_SpriteButton.transform.parent.gameObject.GetComponent<UIButton>().enabled = true;
				if(duihuanMax < m_chenghaoData.list[i].price)
				{
					duihuanMax = m_chenghaoData.list[i].price;
				}
				break;
			}
			m_UIChenghaoList.Add(tempChenghaoData);
		}
		gameObject.SetActive(true);
		if(m_iSelectPage == 0)
		{
			MainCityUI.SetRedAlert(510015, duihuanMax < meiduihuanMax);
		}
		else
		{
			MainCityUI.SetRedAlert(520015, duihuanMax < meiduihuanMax);
		}
	}

	public void EndTime()
	{
		Debug.Log(m_iSelectPage);
		Global.ScendID(ProtoIndexes.C_LIST_CHENG_HAO, m_iSelectPage);
		Global.ScendNull(ProtoIndexes.C_GET_CUR_CHENG_HAO);
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