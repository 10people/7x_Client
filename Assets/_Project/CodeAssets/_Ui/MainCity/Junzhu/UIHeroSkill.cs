using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class UIHeroSkill : MYNGUIPanel , SocketListener
{
	public List<UISprite> m_listButtonBg;
	public List<UISprite> m_listButtonAlert;
	public List<UILabel> m_listButtonLabel;
	public UILabel m_UILabelType;
	public UIJunZhu m_UIJunzhu;
	public int m_IndexType;
	public List<UISprite> m_listSkillIcon = new List<UISprite>();
	public List<GameObject> m_listSkillTupo = new List<GameObject>();
	public List<UILabel> m_listSkillUpNeedLv = new List<UILabel>();
	public List<GameObject> m_listSkillAlert = new List<GameObject>();
	public GetJiNengPeiYangQuality m_heroSkillData;

	public List<HeroData> m_listCurPageHeroData = new List<HeroData>();
	public List<HeroSkillUpTemplate> m_listCurPageHeroSkillUpTemplate = new List<HeroSkillUpTemplate>();

	public UILabel m_labelSkillName;
	public UILabel m_labelDes;

	public UILabel m_labelNeedMoney;
	public GameObject m_objNeedMoney;

	public int m_iSelectIndex;
	public int m_iSelectId;
	public GameObject m_objUp;
	public GameObject m_objUpHui;
	public GameObject m_eff;
	public GameObject m_effTupo;
	public GameObject m_effTupoWenzi;
	public GameObject m_selectSpirte;
	private bool m_isCreateEff;

	public GameObject m_objHeroSkillUpPanel;
	public UILabel m_labelHeroSkillUpDes;
	public UISprite m_spriteHeroSkillUp0Bg;
	public UISprite m_spriteHeroSkillUp0Icon;
	public UISprite m_spriteHeroSkillUp1Bg;
	public UISprite m_spriteHeroSkillUp1Icon;
	public int m_Num = 0;
	// Use this for initialization
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);	
	}

	// Update is called once per frame
	void Update () {
		if(m_effTupo.activeSelf)
		{
			m_Num ++;
			if(m_Num == 30)
			{
				m_effTupo.SetActive(false);
				m_Num = 0;
			}
		}
		if(m_labelNeedMoney.gameObject.activeSelf)
		{
			if(JunZhuData.Instance().m_junzhuInfo.jinBi >= m_listCurPageHeroSkillUpTemplate[m_iSelectIndex].m_iNeedMoney)
			{
				m_labelNeedMoney.color = Color.white;
			}
			else
			{
				m_labelNeedMoney.color = Color.red;
			}
		}
	}

	public void setData(GetJiNengPeiYangQuality data)
	{
		if (FreshGuide.Instance().IsActive(100230) && TaskData.Instance.m_TaskInfoDic[100230].progress >= 0)
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 100230;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 4;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
		}
		if(!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		m_heroSkillData = data;
		MainCityUI.SetRedAlert(500007, setAlert());
		setType(0);
		getCurPageData();
		upDateIcon();
		setDes(0);
		setEff();
	}

	public bool setAlert()
	{
		bool tempReturn = false;
		for(int i = 0; i < 3; i ++)
		{
			m_listButtonAlert[i].gameObject.SetActive(false);
		}
		for(int i = 0; i < m_heroSkillData.listHeroData.Count; i ++)
		{
			HeroSkillUpTemplate temp = HeroSkillUpTemplate.GetHeroSkillUpByID(m_heroSkillData.listHeroData[i].skillId);
			if(getIsTupo(temp) == 2 && JunZhuData.Instance().m_junzhuInfo.jinBi >= temp.m_iNeedMoney && EquipsOfBody.Instance().m_weapon[temp.m_iWuqiType])
			{
				m_listButtonAlert[temp.m_iWuqiType].gameObject.SetActive(true);
				tempReturn = true;
			}
		}
		return tempReturn;
	}

	public void setType(int index)
	{
		m_IndexType = index;
		for(int i = 0; i < m_listButtonBg.Count; i ++)
		{
			if(i == index)
			{
				m_listButtonBg[i].color = Color.white;
				m_listButtonLabel[i].GetComponent<UILabelType>().setType(2);
			}
			else
			{
				m_listButtonBg[i].color = Color.grey;
				m_listButtonLabel[i].GetComponent<UILabelType>().setType(101);
			}
		}
		m_UILabelType.text = DescIdTemplate.GetDescriptionById(311+index);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_HEROSKILLUP_UP_RES:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				UpgradeJiNengResp tempInfo = new UpgradeJiNengResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				if(tempInfo.result == 0)
				{
					setHeroSkillUp(HeroSkillUpTemplate.GetHeroSkillUpByID(m_iSelectId).m_iNeedParID);
					setEffUPTupo();
					for(int i = 0; i < m_heroSkillData.listHeroData.Count; i ++)
					{
						if(m_heroSkillData.listHeroData[i].skillId == m_iSelectId)
						{
							m_heroSkillData.listHeroData[i].isUp = true;
						}
					}
					upDateIcon();
					setDes(m_iSelectIndex);
					clear();
					MainCityUI.SetRedAlert(500007, setAlert());
				}
				else
				{
					TaskData.Instance.m_DestroyMiBao = false;
				}
				if (FreshGuide.Instance().IsActive(100230))
				{
					UIYindao.m_UIYindao.CloseUI();
				}
				break;
			}
			case ProtoIndexes.JunZhuInfoRet:
				setHeroSkillUp(HeroSkillUpTemplate.GetHeroSkillUpByID(m_iSelectId).m_iNeedParID);
				setEffUPTupo();
				for(int i = 0; i < m_heroSkillData.listHeroData.Count; i ++)
				{
					if(m_heroSkillData.listHeroData[i].skillId == m_iSelectId)
					{
						m_heroSkillData.listHeroData[i].isUp = true;
					}
				}
				upDateIcon();
				setDes(m_iSelectIndex);
				clear();
				MainCityUI.SetRedAlert(500007, setAlert());
				break;
			default: return false;
			}
			
		}
		else
		{
			Debug.Log ("p_message == null");
		}
		return false;
	}

	public void getCurPageData()
	{

		HeroSkillUpTemplate temp;
		m_listCurPageHeroData = new List<HeroData>();
		m_listCurPageHeroSkillUpTemplate = new List<HeroSkillUpTemplate>();
		for(int i = 0; i < m_heroSkillData.listHeroData.Count; i ++)
		{
			temp = HeroSkillUpTemplate.GetHeroSkillUpByID(m_heroSkillData.listHeroData[i].skillId);
			if(temp.m_iWuqiType == m_IndexType)
			{
				m_listCurPageHeroData.Add(m_heroSkillData.listHeroData[i]);

				m_listCurPageHeroSkillUpTemplate.Add(temp);
			}
		}
	}

	public void upDateIcon()
	{
		for(int i = 0; i < m_listCurPageHeroSkillUpTemplate.Count; i ++)
		{
			m_listSkillIcon[i].spriteName = m_listCurPageHeroSkillUpTemplate[i].m_sSpriteName;

			m_listSkillTupo[i].SetActive(false);
			m_listSkillAlert[i].SetActive(false);
			m_listSkillUpNeedLv[i].gameObject.SetActive(false);

			if(m_listCurPageHeroData[i].isUp)
			{
				m_listSkillIcon[i].color = Color.white;
			}
			else
			{
				m_listSkillIcon[i].color = Color.black;

				switch(getIsTupo(m_listCurPageHeroSkillUpTemplate[i]))
				{
				case 1:
					m_listSkillUpNeedLv[i].gameObject.SetActive(true);
					m_listSkillUpNeedLv[i].text = "Lv." + m_listCurPageHeroSkillUpTemplate[i].m_iNeedLV + "解锁";
					break;
				case 2:
					if(EquipsOfBody.Instance().m_weapon[m_IndexType])
					{
						m_listSkillTupo[i].SetActive(true);
						if(JunZhuData.Instance().m_junzhuInfo.jinBi >= m_listCurPageHeroSkillUpTemplate[i].m_iNeedMoney)
						{
							m_listSkillAlert[i].SetActive(true);
						}
					}
					break;
				}
			}
		}
	}

	public HeroData getHeroData(int id)
	{
		for(int i = 0; i < m_heroSkillData.listHeroData.Count; i ++)
		{
			if(m_heroSkillData.listHeroData[i].skillId == id)
			{
				return m_heroSkillData.listHeroData[i];
			}
		}
		return null;
	}

	public int getIsTupo(HeroSkillUpTemplate data)
	{
		//1解锁,2可升级,3已升级
//		if(1==1)
//		{
//			return Global.getRandom(3) + 1;
//		}
		HeroData temp = getHeroData(data.m_iID);
		if(temp.isUp)
		{
			return 3;
		}

		if(JunZhuData.Instance().m_CurrentLevel >= data.m_iNeedLV)
		{
			if(data.m_iNeedParID == 0)
			{
				return 2;
			}
			temp = getHeroData(data.m_iNeedParID);
			if(temp.isUp)
			{
				return 2;
			}
			else
			{
				return 3;
			}
		}
		else
		{
			return 1;
		}
	}

	public void setDes(int selectIndex)
	{
		m_iSelectIndex = selectIndex;
		m_labelSkillName.text = m_listCurPageHeroSkillUpTemplate[m_iSelectIndex].m_sName;
		m_labelDes.text = m_listCurPageHeroSkillUpTemplate[m_iSelectIndex].m_sDesc;
		if(getIsTupo(m_listCurPageHeroSkillUpTemplate[m_iSelectIndex]) == 1)
		{
			m_objUpHui.SetActive(true);
		}
		else if(EquipsOfBody.Instance().m_weapon[m_IndexType])
		{
			m_objUpHui.SetActive(false);
		}
		if(getIsTupo(m_listCurPageHeroSkillUpTemplate[m_iSelectIndex]) == 2)
		{
			if(EquipsOfBody.Instance().m_weapon[m_IndexType])
			{
				m_objUp.SetActive(true);
			}
			else
			{
				m_objUp.SetActive(false);
				m_objUpHui.SetActive(true);
			}
		}
		else
		{
			m_objUp.SetActive(false);
		}
		if(m_listCurPageHeroData[m_iSelectIndex].isUp)
		{
			m_objNeedMoney.SetActive(false);
		}
		else
		{
			m_objNeedMoney.SetActive(true);
			m_labelNeedMoney.text = m_listCurPageHeroSkillUpTemplate[m_iSelectIndex].m_iNeedMoney + "";
		}
	}

	public void setEff()
	{
//		if(m_isCreateEff)
//		{
//			clear();
//		}
		m_selectSpirte.transform.position = m_listSkillIcon[m_iSelectIndex].transform.position;
//		UI3DEffectTool.ShowMidLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_listSkillIcon[m_iSelectIndex].gameObject, EffectTemplate.getEffectTemplateByEffectId( 100113 ).path);
//		m_isCreateEff = true;
	}

	public void clear()
	{
//		m_isCreateEff = false;
//		UI3DEffectTool.ClearUIFx(m_listSkillIcon[m_iSelectIndex].gameObject);
	}

	public void setEffUP()
	{
		m_eff.transform.position = m_listSkillIcon[m_iSelectIndex].transform.position;
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_eff, EffectTemplate.getEffectTemplateByEffectId( 100009 ).path);

	}

	public void clearTupo()
	{
		m_effTupo.SetActive(false);
//		UI3DEffectTool.ClearUIFx(m_effTupo);
		UI3DEffectTool.ClearUIFx(m_effTupoWenzi);
		m_Num = 0;
	}
	
	public void setEffUPTupo()
	{
		m_Num = 0;
		m_effTupo.SetActive(true);
//		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_effTupo, EffectTemplate.getEffectTemplateByEffectId( 100009 ).path);
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_effTupoWenzi, EffectTemplate.getEffectTemplateByEffectId( 100180 ).path);
	}

	public void setHeroSkillUp(int id)
	{
		m_objHeroSkillUpPanel.SetActive(true);
		HeroSkillUpTemplate tempHeroSkillUpTemplate0 = HeroSkillUpTemplate.GetHeroSkillUpByID(id);
		switch(tempHeroSkillUpTemplate0.m_iQuality)
		{
		case 0:
			m_spriteHeroSkillUp0Bg.spriteName = "backGround_1";
			m_spriteHeroSkillUp0Bg.SetDimensions(100, 100);
			break;
		case 1:
			m_spriteHeroSkillUp0Bg.spriteName = "skillbg1";
			m_spriteHeroSkillUp0Bg.SetDimensions(102, 102);
			break;
		case 2:
			m_spriteHeroSkillUp0Bg.spriteName = "skillbg2";
			m_spriteHeroSkillUp0Bg.SetDimensions(118, 118);
			break;
		}
		m_spriteHeroSkillUp0Icon.spriteName = tempHeroSkillUpTemplate0.m_sSpriteName;

		HeroSkillUpTemplate tempHeroSkillUpTemplate1 = HeroSkillUpTemplate.GetHeroSkillUpByID(tempHeroSkillUpTemplate0.m_iNextID);
		switch(tempHeroSkillUpTemplate1.m_iQuality)
		{
		case 0:
			m_spriteHeroSkillUp1Bg.spriteName = "backGround_1";
			m_spriteHeroSkillUp1Bg.SetDimensions(100, 100);
			break;
		case 1:
			m_spriteHeroSkillUp1Bg.spriteName = "skillbg1";
			m_spriteHeroSkillUp1Bg.SetDimensions(102, 102);
			break;
		case 2:
			m_spriteHeroSkillUp1Bg.spriteName = "skillbg2";
			m_spriteHeroSkillUp1Bg.SetDimensions(118, 118);
			break;
		}

		m_spriteHeroSkillUp1Icon.spriteName = tempHeroSkillUpTemplate1.m_sSpriteName;

		m_labelHeroSkillUpDes.text = tempHeroSkillUpTemplate1.m_sTupoDesc;
	}
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Button") != -1)
		{
			clear();
			setType(int.Parse(ui.name.Substring(6,1)));
			getCurPageData();
			upDateIcon();
			setDes(0);
			setEff();
		}
		else if(ui.name.IndexOf("Skill") != -1)
		{
			clear();
			setDes(int.Parse(ui.name.Substring(5,1)));
			setEff();
			if (FreshGuide.Instance().IsActive(100230) && TaskData.Instance.m_TaskInfoDic[100230].progress >= 0)
			{
				//if(!UIYindao.m_UIYindao.m_isOpenYindao)
				{
					TaskData.Instance.m_iCurMissionIndex = 100230;
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
					tempTaskData.m_iCurIndex = 5;
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}
			}
		}
		else if(ui.name.IndexOf("UpBg") != -1)
		{
			if(JunZhuData.Instance().m_junzhuInfo.jinBi < m_listCurPageHeroSkillUpTemplate[m_iSelectIndex].m_iNeedMoney)
			{
				JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
			}
			else
			{
				TaskData.Instance.m_DestroyMiBao = true;
				m_iSelectId = m_listCurPageHeroData[m_iSelectIndex].skillId;
				Global.ScendID(ProtoIndexes.C_HEROSKILLUP_UP_REQ, m_iSelectId);
			}
		}
		else if(ui.name.IndexOf("CloseHeroUp") != -1)
		{
			m_objHeroSkillUpPanel.SetActive(false);
			setEff();
			setEffUP();
			clearTupo();
			TaskData.Instance.m_DestroyMiBao = false;
		}
		//		Debug.Log(ui.name);
		//		if(ui.name.IndexOf("tianfuback") != -1)
		//		{
		//			gameObject.SetActive(false);
		//			m_TianfuDisPanel.SetActive(false);
		//			m_UIJunzhu.m_PlayerRight.SetActive(true);
		//			m_UIJunzhu.m_PlayerLeft.SetActive(true);
		//			
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
		//		}
		//		else if(ui.name.IndexOf("tianfuicon") != -1)
		//		{
		//			setDis(int.Parse(ui.name.Substring(10, ui.name.Length - 10)));
		//		}
		//		else if(ui.name.IndexOf("TianfuUpButton") != -1)
		//		{
		//			if(m_isUp)
		//			{
		//				TalentUpLevelReq req = new TalentUpLevelReq();
		//				
		//				req.pointId = m_iCurId;
		//				
		//				MemoryStream tempStream = new MemoryStream();
		//				
		//				QiXiongSerializer t_qx = new QiXiongSerializer();
		//				
		//				t_qx.Serialize(tempStream, req);
		//				
		//				byte[] t_protof;
		//				
		//				t_protof = tempStream.ToArray();
		//				
		//				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_TALENT_UP_LEVEL_REQ, ref t_protof);
		//				if(!(EquipsOfBody.Instance().EquipUnWear() || EquipsOfBody.Instance().EquipReplace() || Global.m_isNewChenghao || Global.m_isFuWen || BagData.AllUpgrade()))
		//				{
		//					MainCityUIRB.SetRedAlert(200, false);
		//				}
		//				Debug.Log("=============1");
		//				Global.m_isTianfuUpCan = false;
		//			}
		//		}
		//		else if(ui.name.IndexOf("TianfuDisBG") != -1)
		//		{
		//			m_TianfuDisPanel.SetActive(false);
		//		}
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

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
}