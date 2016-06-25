using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class UITianfu : MYNGUIPanel 
{
	public UIJunZhu m_UIJunzhu;
	public int m_JGJQ;
	public int m_JGDS;
	public int m_FYJQ;
	public int m_FYDS;
	public UILabel m_UILabelJGJQ;
	public UILabel m_UILabelJGDS;
	public UILabel m_UILabelFYJQ;
	public UILabel m_UILabelFYDS;
	public GameObject m_objClosePanel;
	
	public List<GameObject> m_listTianfuIconData;
	public List<TianfuData> m_listTianFuData;
	
	public struct TianfuData
	{
		public int m_iID;
		public int m_iType;
		public string m_sDir;
		public UIButton m_uiButton;
		public UILabel m_UILabel;
		public UISprite m_UISprite;
		public UISprite m_UISpriteCanUp;
		public int m_iMaxLV;
		public int m_iCurLV;
		public string name;
		public List<int> m_listFrontPoint;
		public int m_iFrontPointLv;
		public int m_iUPDianshu;//升级需要的点数
		public int m_iUPJingqi;
		public int m_iUPGiveDianshu;//升级给与点数
	}
	public GameObject m_TianfuDisPanel;
	public UILabel m_TianfuName;
	public UILabel m_TianfuLV;
	public UILabel m_TianfuWantLv;
	public UILabel m_TianfuDis;
	public UILabel m_TianfuDiShu;
	public UILabel m_TianfuWant;
	public GameObject m_UPButton;
	public UISprite m_TianfuUpLvSprite;
	public UILabel m_TianfuUpLvLabel;
	public BoxCollider m_TianfuUpBox;
	
	private int m_iCurId;
	private bool m_isUp = true;
	private int m_iNum = 0;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < m_listTianFuData.Count; i ++)
		{
			m_listTianFuData[i].m_UISpriteCanUp.spriteName = "tainfu_ui_" + m_iNum / 3;
		}
		m_iNum ++;
		if(m_iNum == 30)
		{
			m_iNum = 0;
		}
	}
	
	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("tianfuback") != -1)
		{
			gameObject.SetActive(false);
			m_TianfuDisPanel.SetActive(false);
			m_objClosePanel.SetActive(false);
			m_UIJunzhu.m_PlayerRight.SetActive(true);
			m_UIJunzhu.m_PlayerLeft.SetActive(true);
			m_UIJunzhu.m_LeftUI.SetActive(true);
			
//			if (FreshGuide.Instance().IsActive(100380) && TaskData.Instance.m_TaskInfoDic[100380].progress >= 0)
//			{
//				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
//				{
//					TaskData.Instance.m_iCurMissionIndex = 100380;
//					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//					tempTaskData.m_iCurIndex = 1;
//					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//				}
//			}
		}
		else if(ui.name.IndexOf("tianfuicon") != -1)
		{
			setDis(int.Parse(ui.name.Substring(10, ui.name.Length - 10)));
		}
		else if(ui.name.IndexOf("TianfuUpButton") != -1)
		{
			if(m_isUp)
			{
				for(int i = 0; i < m_UIJunzhu.m_TalentInfoResp.points.Count; i ++)
				{
					if(m_iCurId == m_UIJunzhu.m_TalentInfoResp.points[i].pointId)
					{
						m_UIJunzhu.m_TalentInfoResp.points[i].pointLev ++;
						TalentTemplate tempTalent = TalentTemplate.getSkillTemplateById(m_UIJunzhu.m_TalentInfoResp.points[i].pointId);
						if(tempTalent.type == 1)
						{
							m_UIJunzhu.m_TalentInfoResp.wuYiJingQi -= m_UIJunzhu.m_TalentInfoResp.points[i].needJingQi;
							if(m_UIJunzhu.m_TalentInfoResp.points[i].pointLev == 1)
							{
								if(m_UIJunzhu.m_TalentInfoResp.points[i].pointId != 101)
								{
									m_UIJunzhu.m_TalentInfoResp.jinGongDianShu --;
								}
							}
						}
						else if(tempTalent.type == 2)
						{
							m_UIJunzhu.m_TalentInfoResp.tiPoJingQi -= m_UIJunzhu.m_TalentInfoResp.points[i].needJingQi;
							if(m_UIJunzhu.m_TalentInfoResp.points[i].pointLev == 1)
							{
								if(m_UIJunzhu.m_TalentInfoResp.points[i].pointId != 201)
								{
									m_UIJunzhu.m_TalentInfoResp.fangShouDianShu --;
								}
							}
						}
						m_UIJunzhu.m_TalentInfoResp.points[i].desc = TalentArrTemplate.getByIDLV(tempTalent.arrID, m_UIJunzhu.m_TalentInfoResp.points[i].pointLev).des;
						m_UIJunzhu.m_TalentInfoResp.points[i].needJingQi = ExpTempTemp.GetExp(tempTalent.expID, m_UIJunzhu.m_TalentInfoResp.points[i].pointLev);
						setData(m_UIJunzhu.m_TalentInfoResp);
//						temp.m_iID = talentData.points[i].pointId;
//						temp.m_iCurLV = talentData.points[i].pointLev;
//						temp.m_sDir = talentData.points[i].desc;
//						temp.m_iUPJingqi = talentData.points[i].needJingQi;
//						temp.m_iUPGiveDianshu = talentData.points[i].difValueLev;
					}
				}
				TalentUpLevelReq req = new TalentUpLevelReq();
				
				req.pointId = m_iCurId;
				
				MemoryStream tempStream = new MemoryStream();
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				t_qx.Serialize(tempStream, req);
				
				byte[] t_protof;
				
				t_protof = tempStream.ToArray();
				
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TALENT_UP_LEVEL_REQ, ref t_protof);
				MainCityUI.SetRedAlert(500000, false);
			}
			
		}
		else if(ui.name.IndexOf("TianfuDisClose") != -1)
		{
			m_TianfuDisPanel.SetActive(false);
			m_objClosePanel.SetActive(false);
		}
		else if(ui.name.IndexOf("GetJingqi") != -1)
		{
			Global.CreateFunctionIcon(1001);
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
	
	public void setData(TalentInfoResp talentData)
	{
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		m_JGJQ = talentData.wuYiJingQi;
		m_JGDS = talentData.jinGongDianShu;
		m_FYJQ = talentData.tiPoJingQi;
		m_FYDS = talentData.fangShouDianShu;
		m_UILabelJGJQ.text = NameIdTemplate.GetName_By_NameId(900020) + talentData.wuYiJingQi;
		m_UILabelJGDS.text = NameIdTemplate.GetName_By_NameId(900022) + talentData.jinGongDianShu;
		m_UILabelFYJQ.text = NameIdTemplate.GetName_By_NameId(900021) + talentData.tiPoJingQi;
		m_UILabelFYDS.text = NameIdTemplate.GetName_By_NameId(900023) + talentData.fangShouDianShu;
		if(m_listTianFuData != null)
		{
			for(int i = 0; i < m_listTianFuData.Count; i ++)
			{
				m_listTianFuData[i].m_UISpriteCanUp.gameObject.SetActive(true);
			}
		}
		m_listTianFuData = new List<TianfuData>();
		for(int i = 0; i < talentData.points.Count; i ++)
		{
			TianfuData temp = new TianfuData();
			GameObject tempobj;
			m_listTianfuIconData[i].name = "tianfuicon" + talentData.points[i].pointId;
			tempobj = m_listTianfuIconData[i];
			temp.m_uiButton = tempobj.GetComponent<UIButton>();
			if(temp.m_uiButton != null)
			{
				GameObject.Destroy(temp.m_uiButton);
			}
			temp.m_UILabel = Global.GetObj(ref tempobj, "Label").GetComponent<UILabel>();
			temp.m_UISprite = Global.GetObj(ref tempobj, "skillicon").GetComponent<UISprite>();
			temp.m_UISpriteCanUp = Global.GetObj(ref tempobj, "skillCanUp").GetComponent<UISprite>();
			temp.m_iID = talentData.points[i].pointId;
			temp.m_iCurLV = talentData.points[i].pointLev;
			temp.m_sDir = talentData.points[i].desc;
			temp.m_iUPJingqi = talentData.points[i].needJingQi;
			temp.m_iUPGiveDianshu = talentData.points[i].difValueLev;
			TalentTemplate tempTalent = TalentTemplate.getSkillTemplateById(talentData.points[i].pointId);
			temp.m_iMaxLV = tempTalent.maxLv;
			temp.m_iType = tempTalent.type;
			temp.name = tempTalent.tianfuName;
			temp.m_listFrontPoint = tempTalent.listFrontPoint;
			temp.m_iFrontPointLv = tempTalent.FrontPointLv;
			temp.m_iUPDianshu = tempTalent.UPDianshu;
			
			
			
			temp.m_UILabel.text = temp.m_iCurLV + "/" + temp.m_iMaxLV;
			if(canUp(temp))
			{
				temp.m_uiButton = m_listTianfuIconData[i].gameObject.AddComponent<UIButton>();
				temp.m_uiButton.tweenTarget = temp.m_UISprite.gameObject;
				temp.m_UISprite.color = Color.white;
				if(!wantDataUp(temp))
				{
					temp.m_UISpriteCanUp.gameObject.SetActive(false);
				}
				
			}
			else
			{
				temp.m_UISprite.color = Color.black;
				temp.m_UISpriteCanUp.gameObject.SetActive(false);
			}
			m_listTianFuData.Add(temp);
		}
		if(m_TianfuDisPanel.activeSelf)
		{
			setDis(m_iCurId);
		}
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			GameObject tempObj = new GameObject();
			tempObj.name = "tianfuicon" + Global.NextCutting(ref Global.m_sPanelWantRun);
			MYClick(tempObj);
		}
	}
	
	public bool canUp(TianfuData  data)
	{
		if(data.m_iCurLV == 0)
		{
			for(int i = 0; i < data.m_listFrontPoint.Count; i ++)
			{
				if(data.m_listFrontPoint[i] == 0)
				{
					return true;
				}
			}
			if(data.m_iType == 1)
			{
				if(data.m_iUPDianshu <= m_JGDS)
				{
					if(data.m_iUPDianshu <= m_JGJQ)
					{
						for(int i = 0; i < data.m_listFrontPoint.Count; i ++)
						{
							if(getTianfuData(data.m_listFrontPoint[i]).m_iCurLV < data.m_iFrontPointLv)
							{
								return false;
							}
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			else if(data.m_iType == 2)
			{
				if(data.m_iUPDianshu <= m_FYDS)
				{
					if(data.m_iUPDianshu <= m_FYJQ)
					{
						for(int i = 0; i < data.m_listFrontPoint.Count; i ++)
						{
							if(getTianfuData(data.m_listFrontPoint[i]).m_iCurLV < data.m_iFrontPointLv)
							{
								return false;
							}
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			return true;
		}
		else
		{
			return true;
		}
	}
	
	public bool wantDataUp(TianfuData  temp)
	{
		if(temp.m_iCurLV >= JunZhuData.Instance().m_junzhuInfo.level)
		{
			return false;
		}
		if(temp.m_iCurLV == 0)
		{
			for(int i = 0; i < temp.m_listFrontPoint.Count; i ++)
			{
				if(temp.m_listFrontPoint[i] != 0)
				{
					if(getTianfuData(temp.m_listFrontPoint[i]).m_iCurLV < temp.m_iFrontPointLv)
					{
						m_TianfuWantLv.text += getTianfuData(temp.m_listFrontPoint[i]).name + MyColorData.getColorString(5, temp.m_iFrontPointLv + "级");
						return false;
					}
				}
			}
		}
		
		if(temp.m_iCurLV == temp.m_iMaxLV)
		{
			return false;
		}
		else
		{
			if(temp.m_iType == 1)
			{
				if(m_JGJQ >= temp.m_iUPJingqi)
				{
				}
				else
				{
					return false;
				}
			}
			else
			{
				if(m_FYJQ >= temp.m_iUPJingqi)
				{
				}
				else
				{
					return false;
				}
			}
			
			if(temp.m_iUPDianshu != 0 && temp.m_iCurLV == 0)
			{
				if(temp.m_iType == 1)
				{
					if(m_JGDS >= temp.m_iUPDianshu)
					{
					}
					else
					{
						return false;
					}
				}
				else
				{
					if(m_FYDS >= temp.m_iUPDianshu)
					{
					}
					else
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	
	public TianfuData getTianfuData(int id)
	{
		TianfuData temp = new TianfuData();
		for(int i = 0; i < m_listTianFuData.Count; i ++)
		{
			if(m_listTianFuData[i].m_iID == id)
			{
				return m_listTianFuData[i];
			}
		}
		return temp;
	}
	
	public void setDis(int id)
	{
		m_isUp = true;
		int tempWantLvNum = 0;
		m_iCurId = id;
		TianfuData temp = getTianfuData(id);
		m_TianfuWantLv.text = "需求";
		m_TianfuDisPanel.SetActive(true);
		m_objClosePanel.SetActive(true);
		if(temp.m_iCurLV >= JunZhuData.Instance().m_junzhuInfo.level)
		{
			m_TianfuWantLv.text += "君主等级" + MyColorData.getColorString(5, (temp.m_iCurLV + 1) + "级");
			m_isUp = false;
		}
		else
		{
			m_TianfuWantLv.text += "君主等级" + (temp.m_iCurLV + 1) + "级";
		}
		tempWantLvNum = 1;
		if(temp.m_iCurLV == 0)
		{
			for(int i = 0; i < temp.m_listFrontPoint.Count; i ++)
			{
				if(temp.m_listFrontPoint[i] != 0)
				{
					if(tempWantLvNum != 0)
					{
						m_TianfuWantLv.text += ",";
					}
					if(getTianfuData(temp.m_listFrontPoint[i]).m_iCurLV < temp.m_iFrontPointLv)
					{
						m_TianfuWantLv.text += getTianfuData(temp.m_listFrontPoint[i]).name + MyColorData.getColorString(5, temp.m_iFrontPointLv + "级");
						m_isUp = false;
					}
					else
					{
						m_TianfuWantLv.text += getTianfuData(temp.m_listFrontPoint[i]).name + MyColorData.getColorString(2, temp.m_iFrontPointLv + "级");
					}
					tempWantLvNum ++;
				}
			}
		}
		if(temp.m_iType == 1)
		{
			m_TianfuName.text = "[ec4534]" + temp.name + "[-]";
		}
		else
		{
			m_TianfuName.text = "[03abea]" + temp.name + "[-]";
		}

		m_TianfuLV.text = "等级:  " + temp.m_iCurLV + "/" + temp.m_iMaxLV;
		m_TianfuDis.text = temp.m_sDir;
		m_TianfuWant.text = "";
		m_TianfuDiShu.text = "";
		
		UIButton tempUpButton = m_UPButton.GetComponent<UIButton>();
		m_TianfuUpBox.enabled = false;
		if(tempUpButton != null)
		{
			GameObject.Destroy(tempUpButton);
		}
		
		if(temp.m_iCurLV == temp.m_iMaxLV)
		{
			tempWantLvNum = 0;
			m_UPButton.SetActive(false);
		}
		else
		{
			m_UPButton.SetActive(true);
			
			if(temp.m_iType == 1)
			{
				if(m_JGJQ >= temp.m_iUPJingqi)
				{
					m_TianfuWant.text = MyColorData.getColorString(2, "消耗") + MyColorData.getColorString(2, temp.m_iUPJingqi + "武艺精气");
				}
				else
				{
					m_TianfuWant.text = MyColorData.getColorString(2, "消耗") + MyColorData.getColorString(5, temp.m_iUPJingqi + "武艺精气");
					m_isUp = false;
				}
			}
			else
			{
				if(m_FYJQ >= temp.m_iUPJingqi)
				{
					m_TianfuWant.text = MyColorData.getColorString(2, "消耗") + MyColorData.getColorString(2, temp.m_iUPJingqi + "体魄精气");
				}
				else
				{
					m_TianfuWant.text = MyColorData.getColorString(2, "消耗") + MyColorData.getColorString(5, temp.m_iUPJingqi + "体魄精气");
					m_isUp = false;
				}
			}
			
			if(temp.m_iUPDianshu != 0 && temp.m_iCurLV == 0)
			{
				if(temp.m_iType == 1)
				{
					if(m_JGDS >= temp.m_iUPDianshu)
					{
						m_TianfuWant.text = m_TianfuWant.text + MyColorData.getColorString(2, temp.m_iUPDianshu + "进攻点数");
					}
					else
					{
						m_TianfuWant.text = m_TianfuWant.text + MyColorData.getColorString(5, temp.m_iUPDianshu + "进攻点数");
						m_isUp = false;
					}
				}
				else
				{
					if(m_FYDS >= temp.m_iUPDianshu)
					{
						m_TianfuWant.text = m_TianfuWant.text + MyColorData.getColorString(2, temp.m_iUPDianshu + "防守点数");
					}
					else
					{
						m_TianfuWant.text = m_TianfuWant.text + MyColorData.getColorString(5, temp.m_iUPDianshu + "防守点数");
						m_isUp = false;
					}
				}
			}
			
			if(temp.m_iUPGiveDianshu != 0)
			{
				if(temp.m_iType == 1)
				{
					m_TianfuDiShu.text = "破甲升级10级，可获得1点进攻点数";
				}
				else
				{
					m_TianfuDiShu.text = "铁壁升级10级，可获得1点防御点数";
				}
			}
		}
		
		if(tempWantLvNum == 0)
		{
			m_TianfuWantLv.gameObject.SetActive(false);
			m_TianfuDis.transform.localPosition = new Vector3(518, 18, 0);
		}
		else
		{
			m_TianfuWantLv.gameObject.SetActive(true);
			m_TianfuDis.transform.localPosition = new Vector3(518, -9, 0);
		}
		
		if(temp.m_iType == 1)
		{
			m_TianfuDisPanel.transform.localPosition = new Vector3(-476f, 3.7f, 0f);
		}
		else
		{
			m_TianfuDisPanel.transform.localPosition = new Vector3(-948f, 3.7f, 0f);
		}
		
		if(m_isUp)
		{
			m_TianfuUpBox.enabled = true;
			UIButton Temp = m_UPButton.gameObject.AddComponent<UIButton>();
			Temp.tweenTarget = m_TianfuUpLvSprite.gameObject;
			m_TianfuUpLvSprite.color = Color.white;
			m_TianfuUpLvLabel.color = Color.white;
		}
		else
		{
			m_TianfuUpLvSprite.color = Color.black;
			m_TianfuUpLvLabel.color = Color.grey;
		}
	}
}