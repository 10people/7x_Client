using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MishuManager : MYNGUIPanel , SocketListener
{
	public bool m_isMibaoPanel = true;
	public List<MishuData> m_MibaoData = new List<MishuData>();
	public List<MishuData> m_MishuData = new List<MishuData>();
	public List<GameObject> m_listAnimationObj = new List<GameObject>();

	public GameObject m_MiBaoPanel;
	public GameObject m_MiShuPanel;

	public GameObject m_MibaoUpObj;
	public MishuData m_MibaoUpData;
	public UILabel m_MibaoUpName;
	public UILabel m_MibaoUpDes;
	public GameObject m_MishuUpObj;
	public MishuData m_MishuUpData;
	public UILabel m_MishuUpName;
	public UILabel m_MishuUpDes;

	public GameObject m_MibaoTipObj;
	public MishuData m_MibaoTipData;
	public UILabel m_MibaoTipName;
	public UILabel m_MibaoTipDes;
	public GameObject m_MishuTipObj;
	public MishuData m_MishuTipData;
	public UILabel m_MishuTipName;
	public UILabel m_MishuTipDes;

	public GameObject m_MibaoUpButtonObj;
	public UILabel m_MibaoUpNeedLabel;
	public UISprite m_MibaoUpSuipianIcon;
	public UILabel m_MibaoUpJihuoLabel;

	public GameObject m_MishuUpButtonObj;
	public UILabel m_MishuUpJihuoLabel;

	public MishuData m_MibaoMishuData;
	public MibaoInfoResp m_MibaoInfoResp;

	public UIGotoPvPPanel m_UIGotoPvPPanel;
	public GameObject m_objButtonAllUp;
	private int m_iClickIndex = -1;

	public bool m_isBAnimation = false;
	public bool m_isAnimationing = false;

	public List<float> m_listMoveX = new List<float>();
	public List<float> m_listMoveY = new List<float>();
	public List<int> m_listState = new List<int>();
	public List<int> m_listNum = new List<int>();

	// Use this for initialization
	void Start () {
		SocketTool.RegisterSocketListener(this);
		Global.ScendNull(ProtoIndexes.NEW_MIBAO_INFO);
		if (FreshGuide.Instance ().IsActive (100404) && TaskData.Instance.m_TaskInfoDic [100404].progress >= 0) 
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 100404;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic [TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 3;
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao (tempTaskData.m_listYindaoShuju [tempTaskData.m_iCurIndex++]);
			}
		}
		else if (FreshGuide.Instance ().IsActive (200010) && TaskData.Instance.m_TaskInfoDic [200010].progress >= 0) 
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 200010;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic [TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao (tempTaskData.m_listYindaoShuju [tempTaskData.m_iCurIndex++]);

			}
		}
		else if (FreshGuide.Instance ().IsActive (100057) && TaskData.Instance.m_TaskInfoDic [100057].progress >= 0) 
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 100057;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic [TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao (tempTaskData.m_listYindaoShuju [tempTaskData.m_iCurIndex++]);
			}
		}
		for(int i = 0; i < 9; i ++)
		{
			float movex = (m_MibaoMishuData.gameObject.transform.localPosition.x - m_MibaoData[i].gameObject.transform.localPosition.x) / 5;
			float movey = (m_MibaoMishuData.gameObject.transform.localPosition.y - m_MibaoData[i].gameObject.transform.localPosition.y) / 5;
			m_listMoveX.Add(movex);
			m_listMoveY.Add(movey);
		}
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}

	// Update is called once per frame
	void Update () {
		if (FreshGuide.Instance().IsActive(100404) && TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
		{
			if(m_UIGotoPvPPanel.gameObject.activeSelf)
			{
//				TaskData.Instance.m_iCurMissionIndex = 100404;
//				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//				tempTaskData.m_iCurIndex = 5;
//				if(UIYindao.m_UIYindao.m_iCurId != tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex])
//				{
//					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
//				}
			}
			else if(m_MibaoUpObj.activeSelf)
			{
				TaskData.Instance.m_iCurMissionIndex = 100404;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 4;
				if(UIYindao.m_UIYindao.m_iCurId != tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex])
				{
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex]);
				}
			}
			else
			{

			}
		}
		if(m_isAnimationing)
		{
			for(int i = 0; i < m_listState.Count; i ++)
			{
				switch(m_listState[i])
				{
				case 0:
					if(m_listNum[i] == 0)
					{
						UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_MibaoData[i].gameObject, EffectTemplate.getEffectTemplateByEffectId(620230).path);
					}
					if(i < 8 && m_listNum[i] == 4)
					{
						m_listState.Add(0);
						m_listNum.Add(0);
					}
					if(m_listNum[i] == 9)
					{
						m_listNum[i] = 0;
						m_listState[i] = 1;
					}
					m_listNum[i] ++;
					break;
				case 1:
					m_listNum[i] ++;
					if(m_listNum[i] == 10)
					{
						UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_listAnimationObj[i], EffectTemplate.getEffectTemplateByEffectId(620231).path);
					}
					else if(m_listNum[i] == 50)
					{
						m_listNum[i] = 0;
						m_listState[i] = 2;
					}
					break;
				case 2:
					Debug.Log(m_listAnimationObj.Count);
					Debug.Log(m_listMoveX.Count);
					m_listAnimationObj[i].transform.localPosition += new Vector3(m_listMoveX[i], m_listMoveY[i], 0);
					m_listNum[i] ++;
					if(m_listNum[i] == 5)
					{
						m_listNum[i] = 0;
						m_listState[i] = 3;
						if(i == 0)
						{
							m_MibaoMishuData.m_SpriteIcon.color = Color.white;
						}
					}
					break;
				case 3:
					m_listNum[i] ++;
					if(m_listNum[i] == 10)
					{
						UI3DEffectTool.ClearUIFx(m_listAnimationObj[i]);
						m_listAnimationObj[i].transform.localPosition = m_MibaoData[i].gameObject.transform.localPosition;
						m_listNum[i] = 0;
						m_listState[i] = 4;
						if(i == 8)
						{
							UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_MibaoMishuData.gameObject, EffectTemplate.getEffectTemplateByEffectId(620229).path);
						}
					}
					break;
				case 4:
					m_listNum[i] ++;
					if(m_listNum[i] == 10)
					{
						if(i == 8)
						{
							m_isBAnimation = false;
							m_isAnimationing = false;

							UI3DEffectTool.ClearUIFx(m_MibaoMishuData.gameObject);
							setDataMiBao();
							for(int q = 0; q < 9; q ++)
							{
								UI3DEffectTool.ClearUIFx(m_MibaoData[i].gameObject);
							}
							m_listState = new List<int>();
							m_listNum = new List<int>();
						}
					}
					break;
				}
			}
		}
	}

	public void setDataMiBao()
	{
		MainCityUI.SetRedAlert(701, false);
		m_objButtonAllUp.SetActive(false);
		bool upMishu = true;
		for(int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i ++)
		{
			UI3DEffectTool.ClearUIFx(m_MibaoData[i].gameObject);
			bool tempRed = m_MibaoData[i].m_ObjRed.activeSelf;
			m_MibaoData[i].m_ObjRed.SetActive(false);
			if(m_MibaoInfoResp.miBaoList[i].star == 0)
			{
				m_MibaoData[i].m_SpriteIcon.color = Color.black;
				m_MibaoData[i].m_SpritePinZhi.gameObject.SetActive(false);
				if(m_MibaoInfoResp.miBaoList[i].suiPianNum >= m_MibaoInfoResp.miBaoList[i].needSuipianNum)
				{
					m_MibaoData[i].m_ObjRed.SetActive(true);
					m_objButtonAllUp.SetActive(true);
					MainCityUI.SetRedAlert(701, true);
				}
				upMishu = false;
			}
			else
			{
				m_MibaoData[i].m_SpriteIcon.color = Color.white;
				m_MibaoData[i].m_SpritePinZhi.gameObject.SetActive(true);
				m_MibaoData[i].m_SpritePinZhi.spriteName = "inlaRound" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[i].tempId).color;
				if(tempRed)
				{
					UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_MibaoData[i].gameObject, EffectTemplate.getEffectTemplateByEffectId(620230).path);
				}
			}
			m_MibaoData[i].m_SpriteIcon.spriteName = "" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[i].tempId).iconID;
		}
		if(upMishu)
		{
			MainCityUI.SetRedAlert(701, true);
		}
		m_MibaoMishuData.m_ObjRed.SetActive(upMishu);
		if(m_MibaoInfoResp.levelPoint == 9)
		{
			m_MibaoMishuData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[8].iconID;
			m_MibaoMishuData.m_SpriteIcon.color = Color.white;
		}
		else
		{
			m_MibaoMishuData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[m_MibaoInfoResp.levelPoint].iconID;
			m_MibaoMishuData.m_SpriteIcon.color = Color.gray;
		}

		if(m_iClickIndex != -1)
		{
			setMiBaoUpData(m_iClickIndex);
		}
	}

	public void setDataMishu()
	{
		for(int i = 0; i < 9; i ++)
		{
			m_MishuData[i].m_SpriteIcon.spriteName = "" + MishuTemplate.templates[i].iconID;
			if(i < m_MibaoInfoResp.levelPoint)
			{
				m_MishuData[i].m_SpriteIcon.color = Color.white;
				m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(true);
				int tempIndex = m_MibaoInfoResp.levelPoint;
				if(tempIndex == 9)
				{
					tempIndex = 8;
				}

				m_MishuData[i].m_SpritePinZhi.spriteName = "inlaRound" + MishuTemplate.templates[i].color;
				m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(true);
			}
			else
			{
				m_MishuData[i].m_SpriteIcon.color = Color.black;
				m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(false);
			}
		}
	}

	void OnDisable()
	{
		for(int i = 0; i < 9; i ++)
		{
			UI3DEffectTool.ClearUIFx(m_MibaoData[i].gameObject);
		}
	}

	public void setMiBaoUpData(int index)
	{
		if(m_MibaoInfoResp.miBaoList[index].star == 0)
		{
			m_MibaoUpObj.SetActive(true);
			m_MibaoUpName.text = MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).pinzhiName + "\n" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).showName;
			m_MibaoUpNeedLabel.text = "x" + m_MibaoInfoResp.miBaoList[index].needSuipianNum + " (拥有：" + m_MibaoInfoResp.miBaoList[index].suiPianNum + ")";
			
			m_MibaoUpData.m_SpriteIcon.spriteName = "" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).iconID;
			m_MibaoUpSuipianIcon.spriteName = "" + MibaoNewSuipianTemplate.GetTemplateByID(MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).suipianID).iconID;
			string tempString = "";
			bool nextLink = false;
			if(m_MibaoInfoResp.miBaoList[index].gongJi > 0)
			{
				tempString = "攻击提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].gongJi);
				nextLink = true;
			}
			if(m_MibaoInfoResp.miBaoList[index].fangYu > 0)
			{
				if(nextLink)
				{
					tempString += "\n";
				}
				tempString += "防御提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].fangYu);
				nextLink = true;
			}
			if(m_MibaoInfoResp.miBaoList[index].shengMing > 0)
			{
				if(nextLink)
				{
					tempString += "\n";
				}
				tempString += "生命提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].shengMing);
				nextLink = true;
			}
			m_MibaoUpDes.text = tempString;
			if(m_MibaoInfoResp.miBaoList[index].suiPianNum >= m_MibaoInfoResp.miBaoList[index].needSuipianNum)
			{
				m_MibaoUpButtonObj.SetActive(true);
				m_MibaoUpJihuoLabel.gameObject.SetActive(false);
			}
			else
			{
				m_MibaoUpButtonObj.SetActive(false);
				m_MibaoUpJihuoLabel.gameObject.SetActive(true);
			}
		}
		else if(m_MibaoInfoResp.miBaoList[index].star == 1)
		{
			m_MibaoTipObj.SetActive(true);
			m_MibaoTipName.text = MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).pinzhiName + "\n" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).showName;

			m_MibaoTipData.m_SpriteIcon.spriteName = "" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).iconID;
			m_MibaoTipData.m_SpritePinZhi.spriteName = "inlaRound" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).color;
			string tempString = "";
			bool nextLink = false;
			if(m_MibaoInfoResp.miBaoList[index].gongJi > 0)
			{
				tempString = "攻击提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].gongJi);
				nextLink = true;
			}
			if(m_MibaoInfoResp.miBaoList[index].fangYu > 0)
			{
				if(nextLink)
				{
					tempString += "\n";
				}
				tempString += "防御提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].fangYu);
				nextLink = true;
			}
			if(m_MibaoInfoResp.miBaoList[index].shengMing > 0)
			{
				if(nextLink)
				{
					tempString += "\n";
				}
				tempString += "生命提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].shengMing);
				nextLink = true;
			}
			m_MibaoTipDes.text = tempString;
		}
	}


	public void setMiShuUpData(int index)
	{
		if(index == 9)
		{
			index = 8;
		}

		m_MishuUpObj.SetActive(true);
		m_MishuUpName.text = MishuTemplate.templates[index].pinzhiName + "\n" + MishuTemplate.templates[index].name;
		m_MishuUpData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[index].iconID;
		m_MishuUpData.m_SpritePinZhi.spriteName = "inlaRound" + MishuTemplate.templates[index].color;
		string tempString = "";
		bool nextLink = false;
		if(MishuTemplate.templates[index].wqSH > 0)
		{
			tempString = "武器伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqSH);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqJM > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqJM);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqBJ > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqBJ);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqRX > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqRX);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnSH > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnSH);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnJM > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnJM);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnBJ > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnBJ);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnRX > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnRX);
			nextLink = true;
		}
		bool isUpMishu = true;
		for(int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i ++)
		{
			if(m_MibaoInfoResp.miBaoList[i].star == 0)
			{
				isUpMishu = false;
			}
		}
		m_MishuUpDes.text = tempString;
		if(isUpMishu && index == m_MibaoInfoResp.levelPoint && m_MibaoInfoResp.levelPoint != 9)
		{
			m_MishuUpButtonObj.SetActive(true);
			m_MishuUpJihuoLabel.gameObject.SetActive(false);
		}
		else
		{
			m_MishuUpButtonObj.SetActive(false);
			m_MishuUpJihuoLabel.gameObject.SetActive(m_MibaoInfoResp.levelPoint < 9);
		}
	}

	public void setMiShuTipData(int index)
	{
		m_MishuTipObj.SetActive(true);
		m_MishuTipName.text = MishuTemplate.templates[index].pinzhiName + "\n" + MishuTemplate.templates[index].name;

		m_MishuTipData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[index].iconID;
		m_MishuTipData.m_SpritePinZhi.spriteName = "inlaRound" + MishuTemplate.templates[index].color;
		string tempString = "";
		bool nextLink = false;
		if(MishuTemplate.templates[index].wqSH > 0)
		{
			tempString = "武器伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqSH);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqJM > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqJM);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqBJ > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqBJ);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].wqRX > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "武器暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqRX);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnSH > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnSH);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnJM > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnJM);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnBJ > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnBJ);
			nextLink = true;
		}
		if(MishuTemplate.templates[index].jnRX > 0)
		{
			if(nextLink)
			{
				tempString += "\n";
			}
			tempString += "技能暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnRX);
			nextLink = true;
		}
		tempString += "\n";
		tempString += MishuTemplate.templates[index].tileLabel;
		tempString += "\n";
		tempString += MibaoNewTemplate.GetPinzhiAllArr(index);
		m_MishuTipDes.text = tempString;
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.NEW_MIBAO_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp tempInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
//				Debug.Log("============1");
				m_MibaoInfoResp = tempInfo;
//				m_isBAnimation = false;
//				m_isAnimationing = true;
//				m_listState.Add(0);
//				m_listNum.Add(0);
				if(m_isBAnimation)
				{
					m_isBAnimation = false;
					m_isAnimationing = true;
					m_listState.Add(0);
					m_listNum.Add(0);
				}
				else
				{
					setDataMiBao();
				}
				break;
			}
			}
		}
		else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Icon") != -1)
		{
			m_iClickIndex = int.Parse(ui.name.Substring(4,1));
			if(m_isMibaoPanel)
			{
				if(m_MibaoInfoResp.miBaoList[m_iClickIndex].star == 0)
				{
					setMiBaoUpData(m_iClickIndex);
				}
			}
			else
			{
				if(m_iClickIndex < m_MibaoInfoResp.levelPoint)
				{

				}
				else if(m_iClickIndex >= m_MibaoInfoResp.levelPoint)
				{
					setMiShuUpData(m_iClickIndex);
				}
			}
			if (FreshGuide.Instance().IsActive(100404) && TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					TaskData.Instance.m_iCurMissionIndex = 100404;
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
					tempTaskData.m_iCurIndex = 4;
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}
			}
		}
		else if(ui.name.IndexOf("jihuo") != -1)
		{
			ErrorMessage req = new ErrorMessage();
			
			req.errorDesc = m_MibaoInfoResp.miBaoList[m_iClickIndex].dbId + "";
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.NEW_MIBAO_JIHUO, ref t_protof);
			
			Global.ScendNull(ProtoIndexes.NEW_MIBAO_INFO);

			m_MibaoUpObj.SetActive(false);


		}
		else if(ui.name.IndexOf("jiesuo") != -1)
		{
			m_isBAnimation = true;
			m_MishuUpObj.SetActive(false);
			Global.ScendNull(ProtoIndexes.NEW_MISHU_JIHUO);
			Global.ScendNull(ProtoIndexes.NEW_MIBAO_INFO);
			if (FreshGuide.Instance().IsActive(200010) && TaskData.Instance.m_TaskInfoDic[200010].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
			m_MibaoMishuData.m_ObjRed.SetActive(false);
		}
		else if(ui.name.IndexOf("MishuButton") != -1)
		{
			setMiShuUpData(m_MibaoInfoResp.levelPoint);
			if (FreshGuide.Instance().IsActive(200010) && TaskData.Instance.m_TaskInfoDic[200010].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					TaskData.Instance.m_iCurMissionIndex = 200010;
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
					tempTaskData.m_iCurIndex = 3;
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}
			}
		}
		else if(ui.name.IndexOf("MibaoUpClose") != -1)
		{
			m_MibaoUpObj.SetActive(false);
			if (FreshGuide.Instance().IsActive(100404) && TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
			{
				TaskData.Instance.m_iCurMissionIndex = 100404;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
			if (FreshGuide.Instance().IsActive(100405) && TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
			{
				TaskData.Instance.m_iCurMissionIndex = 100405;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
			m_iClickIndex = -1;
			Global.ScendNull(ProtoIndexes.NEW_MIBAO_INFO);
		}
		else if(ui.name.IndexOf("MishuUpClose") != -1)
		{
			m_MishuUpObj.SetActive(false);
		}
		else if(ui.name.IndexOf("GoToMishu") != -1)
		{
			m_isMibaoPanel = false;
			m_MiBaoPanel.SetActive(false);
			m_MiShuPanel.SetActive(true);
			setDataMishu();
		}
		else if(ui.name.IndexOf("GoToMibao") != -1)
		{
			m_isMibaoPanel = true;
			m_MiBaoPanel.SetActive(true);
			m_MiShuPanel.SetActive(false);
			setDataMiBao();
		}
		else if(ui.name.IndexOf("GoToPve") != -1)
		{
			m_UIGotoPvPPanel.setData(MibaoNewSuipianTemplate.GetTemplateByID(MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[m_iClickIndex].tempId).suipianID).guanqiaID, "关卡掉落");
			if (FreshGuide.Instance().IsActive(100404) && TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					TaskData.Instance.m_iCurMissionIndex = 100404;
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
					tempTaskData.m_iCurIndex = 5;
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}
			}
		}
		else if(ui.name.IndexOf("AllUp") != -1)
		{
			if (FreshGuide.Instance().IsActive(100405) && TaskData.Instance.m_TaskInfoDic[100405].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
			else if (FreshGuide.Instance().IsActive(100057) && TaskData.Instance.m_TaskInfoDic[100057].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
			for(int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i ++)
			{
				if(m_MibaoInfoResp.miBaoList[i].star == 0)
				{
					if(m_MibaoInfoResp.miBaoList[i].suiPianNum >= m_MibaoInfoResp.miBaoList[i].needSuipianNum)
					{
						ErrorMessage req = new ErrorMessage();
						
						req.errorDesc = m_MibaoInfoResp.miBaoList[i].dbId + "";
						
						MemoryStream tempStream = new MemoryStream();
						
						QiXiongSerializer t_qx = new QiXiongSerializer();
						
						t_qx.Serialize(tempStream, req);
						
						byte[] t_protof;
						
						t_protof = tempStream.ToArray();
						
						SocketTool.Instance().SendSocketMessage(ProtoIndexes.NEW_MIBAO_JIHUO, ref t_protof);
					}
				}
			}
			Global.ScendNull(ProtoIndexes.NEW_MIBAO_INFO);

		}
	}
	public override void MYMouseOver(GameObject ui){}
	public override void MYMouseOut(GameObject ui){}
	public override void MYPress(bool isPress, GameObject ui)
	{
		if(ui.name.IndexOf("Icon") != -1)
		{
			if(m_isMibaoPanel)
			{
				if(isPress)
				{
					m_iClickIndex = int.Parse(ui.name.Substring(4,1));
					if(m_MibaoInfoResp.miBaoList[m_iClickIndex].star == 1)
					{
						setMiBaoUpData(m_iClickIndex);
					}
				}
				else
				{
					m_MibaoTipObj.SetActive(false);
				}
			}
			else
			{
				if(isPress)
				{
					m_iClickIndex = int.Parse(ui.name.Substring(4,1));
					if(m_iClickIndex < m_MibaoInfoResp.levelPoint)
					{
						setMiShuTipData(m_iClickIndex);
					}
				}
				else
				{
					m_MishuTipObj.SetActive(false);
				}
			}
		}
		else if(ui.name.IndexOf("MibaoTipClose") != -1)
		{
			m_MibaoTipObj.SetActive(false);
		}
		else if(ui.name.IndexOf("MishuTipClose") != -1)
		{
			m_MishuTipObj.SetActive(false);
		}
	}
	public override void MYelease(GameObject ui){}
	public override void MYoubleClick(GameObject ui){}
	public override void MYonInput(GameObject ui, string c){}
	public override void MYondrag(Vector2 delta){}
}
