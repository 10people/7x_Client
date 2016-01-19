using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIJunZhu :  MYNGUIPanel , SocketListener
{
	public enum E_JUNZHUSTATE
	{
		E_DEF,
		E_SKILL,
		E_XIANGXI,
		E_TIANFU,
		E_FUWEN,
		E_CHENGHAO,
	}
	public E_JUNZHUSTATE m_JunzhuState = E_JUNZHUSTATE.E_DEF;
	public ItemTopCol m_ItemTopCol;

 
	public static UIJunZhu m_UIJunzhu;
	
	[HideInInspector] public GameObject IconSamplePrefab;
    public ScaleEffectController m_ScaleEffectController;

	public UILabel m_UILbaelLV;
	public UILabel m_UILbaelJunxian;
	public UILabel m_UILbaelZhanli;

	public UILabel m_UILbaelXLV;
	public UILabel m_UILbaelXJunxian;
	public UILabel m_UILbaelXZhanli;
	public UILabel m_UILbaelXExp;
	public UILabel m_UILbaelXATC;
	public UILabel m_UILbaelXDEF;
	public UILabel m_UILbaelXHP;
	public UILabel m_UILbaelHeroName;
	public UILabel m_UILabelArrHeroName;
	public GameObject m_PlayerParent;
	public GameObject m_PlayerModel;
	public GameObject m_PlayerLeft;
	public GameObject m_PlayerRight;
    public GameObject m_Equip;
	public GameObject m_MonetParentObj;

	public List<UISprite> m_SkillIcon = new List<UISprite>();
	public List<UISprite> m_SkillLock = new List<UISprite>();
	public List<UILabel> m_SkillLabel = new List<UILabel>();
	
	public GameObject m_XiangxiPanelLeft;
	public UILabel m_UILbaelXiangxiWQSHJS;
	public UILabel m_UILbaelXiangxiWQBJL;
	public UILabel m_UILbaelXiangxiWQBJJS;
	public UILabel m_UILbaelXiangxiJNSHJS;
	public UILabel m_UILbaelXiangxiJNBJL;
	public UILabel m_UILbaelXiangxiJNBJJS;
	public UILabel m_UILbaelXiangxiWQSHDK;
	public UILabel m_UILbaelXiangxiWQBJDK;
	public UILabel m_UILbaelXiangxiJNSHDK;
	public UILabel m_UILbaelXiangxiJNBJDK;
	
	private MibaoInfoResp m_MiBaoInfo;
	public GameObject m_MibaoIcon;
	public List<UISprite> m_MibaoPinzhi = new List<UISprite>();
	public List<UISprite> m_MibaoIconS = new List<UISprite>();
	public List<GameObject> m_Mibao = new List<GameObject>();
	
	public List<GameObject> m_MibaoRectIcon = new List<GameObject>();
	public List<int> m_iMiBaoRectData = new List<int>();
	
	private List<IconSampleManager> iconSampleManagerList = new List<IconSampleManager>(); 

	private int[] m_iAddData = new int[14];
	
	private JunZhuInfoRet m_JunZhuInfoCopy;
	private JunZhuInfoRet m_JunZhuInfoCopy3;
	
	public UIScrollView m_UIScrollView;
	public GameObject m_TierGear;
	
	public UITianfu m_UITianfu;
	public UIChenghao m_UIChenghao;
	public UIHeroSkill m_UIHeroSkill;
	public GameObject m_ZhuangBeiIcon;
	public UISprite m_UIAlert;
	public UISprite m_UIHeroAlert;
	public UISprite m_UIFuwenAlert;
	public UISprite m_UIChenghaoAlert;
	public UISprite m_UIChenghaoSprite;
	
	void Awake()
	{
		m_UIJunzhu = this;
		SocketTool.RegisterSocketListener(this);	
	}


    void OnEnable()
    {
        m_UIJunzhu = this;
    }
    void OnDisable()
    {
        m_UIJunzhu = null; 
    }
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	
	void Start ()
	{
		MainCityUI.setGlobalBelongings(m_MonetParentObj, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		m_ScaleEffectController.OpenCompleteDelegate = EndDelegate;
		if (FreshGuide.Instance().IsActive(100280) && TaskData.Instance.m_TaskInfoDic[100280].progress >= 0)
		{
			{
				TaskData.Instance.m_iCurMissionIndex = 100280;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 3;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
		}
		else if (FreshGuide.Instance().IsActive(100230) && TaskData.Instance.m_TaskInfoDic[100230].progress >= 0)
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 100230;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
		}
		else if (FreshGuide.Instance().IsActive(100470) && TaskData.Instance.m_TaskInfoDic[100470].progress >= 0)
		{
			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
			{
				TaskData.Instance.m_iCurMissionIndex = 100470;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
				tempTaskData.m_iCurIndex = 2;
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			}
		}
//		else if (FreshGuide.Instance().IsActive(100300) && TaskData.Instance.m_TaskInfoDic[100300].progress >= 0)
//		{
//			//if(!UIYindao.m_UIYindao.m_isOpenYindao)
//			{
//				TaskData.Instance.m_iCurMissionIndex = 100300;
//				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//				tempTaskData.m_iCurIndex = 1;
//				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//			}
//		}
		else
		{
			CityGlobalData.m_isRightGuide = true;
		}
		m_JunZhuInfoCopy = (JunZhuInfoRet)JunZhuData.Instance().m_junzhuInfo.Public_MemberwiseClone();

		m_UILbaelLV.text = m_JunZhuInfoCopy.level.ToString();
		m_UILbaelZhanli.text = m_JunZhuInfoCopy.zhanLi.ToString();
		m_UILbaelJunxian.text =NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(m_JunZhuInfoCopy.junXian).templateName);


		m_UILbaelXLV.text = m_JunZhuInfoCopy.level.ToString();
		m_UILbaelXZhanli.text = m_JunZhuInfoCopy.zhanLi.ToString();
		m_UILbaelXJunxian.text =NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(m_JunZhuInfoCopy.junXian).templateName);

		m_UILbaelXATC.text = m_JunZhuInfoCopy.gongJi.ToString();
		m_UILbaelXDEF.text = m_JunZhuInfoCopy.fangYu.ToString();
		m_UILbaelXHP.text = m_JunZhuInfoCopy.shengMing.ToString();
		m_UILbaelXExp.text = m_JunZhuInfoCopy.exp + "/" + m_JunZhuInfoCopy.expMax;

		if(AllianceData.Instance.IsAllianceNotExist)
		{
			m_UILbaelHeroName.text =  m_JunZhuInfoCopy.name;
			m_UILabelArrHeroName.text =  m_JunZhuInfoCopy.name;

		}
		else
		{
            m_UILbaelHeroName.text = m_JunZhuInfoCopy.name + "\n<" + AllianceData.Instance.g_UnionInfo.name + ">";
			m_UILabelArrHeroName.text = m_JunZhuInfoCopy.name + "\n<" + AllianceData.Instance.g_UnionInfo.name + ">";

		}
		
		for(int i = 0; i < 3; i ++)
		{
//			m_SkillLock[i].gameObject.SetActive(false);
			//			if(EquipsOfBody.Instance().m_weapon[i])
			//			{
			//
			//			}
			//			else
			//			{
			//				m_SkillIcon[i].gameObject.SetActive(false);
			//				m_SkillLabel[i].gameObject.SetActive(false);
			//			}
		}
		UIButton tempButton;
		if(!FunctionOpenTemp.IsHaveID(500007))
		{
			m_SkillLabel[1].GetComponent<UILabelType>().setType(100);
			m_SkillIcon[1].color = Color.black;
		}
		else
		{
			tempButton = m_SkillIcon[1].gameObject.transform.parent.gameObject.AddComponent<UIButton>();
			tempButton.tweenTarget = m_SkillIcon[1].gameObject;
		}
		if(!FunctionOpenTemp.IsHaveID(500000))
		{
			m_SkillLabel[2].GetComponent<UILabelType>().setType(100);
			m_SkillIcon[2].color = Color.black;
		}
		else
		{
			tempButton = m_SkillIcon[2].gameObject.transform.parent.gameObject.AddComponent<UIButton>();
			tempButton.tweenTarget = m_SkillIcon[2].gameObject;
		}
		if(!FunctionOpenTemp.IsHaveID(500010))
		{
			m_SkillLabel[3].GetComponent<UILabelType>().setType(100);
			m_SkillIcon[3].color = Color.black;
		}
		else
		{
			tempButton = m_SkillIcon[3].gameObject.transform.parent.gameObject.AddComponent<UIButton>();
			tempButton.tweenTarget = m_SkillIcon[3].gameObject;
		}
        ShowPlayer();
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			string temp = Global.NextCutting(ref Global.m_sPanelWantRun);
			if(temp == "jinjie")
			{
				Debug.Log("===============1");
			}
			else
			{
				GameObject tempObj = new GameObject();
				tempObj.name = temp;
				MYClick(tempObj);
			}
		}
//		GameObject tempObj1 = new GameObject();
//		tempObj1.name = "Chenghao";
//		MYClick(tempObj1);
//		tempObj1.name = "Skill3";
//		MYClick(tempObj1);

//		if(JunZhuData.m_iChenghaoID != -1)
//		{
//			m_UIChenghaoSprite.gameObject.SetActive(true);
//			m_UIChenghaoSprite.spriteName = "" + JunZhuData.m_iChenghaoID;
//		}
//		else
//		{
//			m_UIChenghaoSprite.gameObject.SetActive(false);
//		}
	}
	
	//    void OnInitIconRectCallBack(ref WWW www, string path, Object loadedObject)
	//    {
	//        if (IconSamplePrefab == null)
	//        {
	//            IconSamplePrefab = loadedObject as GameObject;
	//        }
	//
	//        iconSampleManagerList.Clear();
	//        for (int i=0; i<m_MibaoRectIcon.Count;i++)
	//        {
	//            var tempIcon = Instantiate(loadedObject) as GameObject ;
	//            tempIcon.name = "mibaoRect" + i;
	//            TransformHelper.ActiveWithStandardize(m_MibaoRectIcon[i].transform, tempIcon.transform);
	//            IconSampleManager m_IconSampleManager = tempIcon.GetComponent<IconSampleManager>();
	//
	//            m_IconSampleManager.SetIconType(IconSampleManager.IconType.MiBao);
	//            m_IconSampleManager.SetIconBasic(2);
	//            m_IconSampleManager.SetIconBasicDelegate(false, true, OnMiBaoRectIconClick);
	//
	//            iconSampleManagerList.Add(m_IconSampleManager);
	//        }
	//    }
	
	// Update is called once per frame
	void Update () 
	{
		if(FunctionOpenTemp.IsHaveID(500000) && FunctionOpenTemp.GetTemplateById(500000).m_show_red_alert)
		{
			m_UIAlert.gameObject.SetActive(true);
		}
		else
		{
			m_UIAlert.gameObject.SetActive(false);
		}
		if(Global.m_isFuWen)
		{
			m_UIFuwenAlert.gameObject.SetActive(true);
		}
		else
		{
			m_UIFuwenAlert.gameObject.SetActive(false);
		}

		if(FunctionOpenTemp.IsHaveID(500015) && FunctionOpenTemp.GetTemplateById(500015).m_show_red_alert)
		{
			m_UIChenghaoAlert.gameObject.SetActive(true);
		}
		else
		{
			m_UIChenghaoAlert.gameObject.SetActive(false);
		}
		if(FunctionOpenTemp.IsHaveID(500007) && FunctionOpenTemp.GetTemplateById(500007).m_show_red_alert)
		{
			m_UIHeroAlert.gameObject.SetActive(true);
		}
		else
		{
			m_UIHeroAlert.gameObject.SetActive(false);
		}
	}
	
	private void OnIconSampleLoadCallBack(ref WWW www, string temp, Object loadedObject)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = loadedObject as GameObject;
		}
		else
		{
			m_ItemTopCol.enabled = true;
		}
	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.JunZhuInfoRet:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				JunZhuInfoRet tempInfo = new JunZhuInfoRet();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				m_JunZhuInfoCopy = (JunZhuInfoRet)tempInfo.Public_MemberwiseClone();

				m_UILbaelLV.text = m_JunZhuInfoCopy.level.ToString();
				m_UILbaelZhanli.text = m_JunZhuInfoCopy.zhanLi.ToString();
				m_UILbaelJunxian.text =NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(m_JunZhuInfoCopy.junXian).templateName);

				m_UILbaelXLV.text = m_JunZhuInfoCopy.level.ToString();
				m_UILbaelXZhanli.text = m_JunZhuInfoCopy.zhanLi.ToString();
				m_UILbaelXJunxian.text =NameIdTemplate.GetName_By_NameId(BaiZhanTemplate.getBaiZhanTemplateById(m_JunZhuInfoCopy.junXian).templateName);
				
				m_UILbaelXATC.text = m_JunZhuInfoCopy.gongJi.ToString();
				m_UILbaelXDEF.text = m_JunZhuInfoCopy.fangYu.ToString();
				m_UILbaelXHP.text = m_JunZhuInfoCopy.shengMing.ToString();
				m_UILbaelXExp.text = m_JunZhuInfoCopy.exp + "/" + m_JunZhuInfoCopy.expMax;
				break;
			}
			case ProtoIndexes.S_TALENT_INFO_RESP:
			{
				if(m_UIChenghao.gameObject.activeSelf)
				{
					break;
				}
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				TalentInfoResp tempInfo = new TalentInfoResp();
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				m_PlayerLeft.SetActive(false);
				m_PlayerRight.SetActive(false);
				m_UITianfu.setData(tempInfo);
				m_JunzhuState = E_JUNZHUSTATE.E_TIANFU;
				break;
			}
			case ProtoIndexes.S_TALENT_UP_LEVEL_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				TalentUpLevelResp tempInfo = new TalentUpLevelResp();
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				Debug.Log(tempInfo.errorNumber);
				//				m_UITianfu.setData(tempInfo);
				//				m_PlayerLeft.SetActive(false);
				//				m_PlayerRight.SetActive(false);
				break;
			}
			case ProtoIndexes.S_LIST_CHENG_HAO:
			{
				if(m_UITianfu.gameObject.activeSelf)
				{
					break;
				}
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				ChengHaoList tempInfo = new ChengHaoList();
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				m_PlayerRight.SetActive(false);
//				m_ZhuangBeiIcon.SetActive(false);
				m_UIChenghao.setData(tempInfo);

				if(m_XiangxiPanelLeft.gameObject.activeSelf)
				{
					m_XiangxiPanelLeft.SetActive(false);
					m_PlayerLeft.SetActive(true);
				}

				if(JunZhuData.m_iChenghaoID != -1)
				{
					m_UIChenghaoSprite.gameObject.SetActive(true);
					m_UIChenghaoSprite.spriteName = "" + JunZhuData.m_iChenghaoID;
				}
				else
				{
					m_UIChenghaoSprite.gameObject.SetActive(false);
				}
				m_UILbaelHeroName.gameObject.SetActive(false);
				m_PlayerLeft.gameObject.transform.localPosition = new Vector3(40,0,0);
				m_JunzhuState = E_JUNZHUSTATE.E_CHENGHAO;
				break;
			}
			case ProtoIndexes.S_HEROSKILLUP_DATA_RES:
				m_XiangxiPanelLeft.SetActive(false);

				MemoryStream t_stream1 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx1 = new QiXiongSerializer();
				GetJiNengPeiYangQuality tempInfo1 = new GetJiNengPeiYangQuality();
				t_qx1.Deserialize(t_stream1, tempInfo1, tempInfo1.GetType());
				m_UIHeroSkill.setData(tempInfo1);
				m_PlayerLeft.SetActive(false);
				m_PlayerRight.SetActive(false);
				m_JunzhuState = E_JUNZHUSTATE.E_SKILL;
				break;
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	
	private IEnumerator ShowSkillPopCoroutine(GameObject pop)
	{
		pop.SetActive(true);
		yield return new WaitForSeconds(2.0f);
		
		pop.SetActive(false);
	}

    void OnCloseWindow()
    {
        MainCityUI.TryRemoveFromObjectList(gameObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.ShowId];

			if ((TaskData.Instance.m_iCurMissionIndex != 100115 && TaskData.Instance.m_iCurMissionIndex != 100002 && TaskData.Instance.m_iCurMissionIndex != 100015) || (TaskData.Instance.m_iCurMissionIndex == 100002 || TaskData.Instance.m_iCurMissionIndex == 100015 || TaskData.Instance.m_iCurMissionIndex == 100245) && TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex].progress >= 0)
            {
                CityGlobalData.m_isRightGuide = true;
            }
            else if (TaskData.Instance.m_iCurMissionIndex == 100115)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
            else
            {
                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
            }

        }
        Destroy(gameObject);
    }

	public override void MYClick(GameObject ui)
	{
        if (FreshGuide.Instance().IsActive(100100) && TaskData.Instance.m_TaskInfoDic[100100].progress < 0)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        if (ui.name.IndexOf("Close") != -1 || ui.name.IndexOf ("FuWen(Clone)") != -1)
		{
			GameObject temoObjClickName = new GameObject();

//			Debug.Log(m_JunzhuState);

			switch(m_JunzhuState)
			{
			case E_JUNZHUSTATE.E_DEF:
				m_ScaleEffectController.CloseCompleteDelegate = OnCloseWindow;
				m_ScaleEffectController.OnCloseWindowClick();
				break;
			case E_JUNZHUSTATE.E_SKILL:
				temoObjClickName.name = "skillback";
				MYClick(temoObjClickName);
				m_JunzhuState = E_JUNZHUSTATE.E_DEF;
				break;
			case E_JUNZHUSTATE.E_XIANGXI:
				temoObjClickName.name = "ArrBack";
				MYClick(temoObjClickName);
				m_JunzhuState = E_JUNZHUSTATE.E_DEF;
				break;
			case E_JUNZHUSTATE.E_FUWEN:
				break;
			case E_JUNZHUSTATE.E_TIANFU:
				temoObjClickName.name = "tianfuback";
				m_UITianfu.MYClick(temoObjClickName);
				m_JunzhuState = E_JUNZHUSTATE.E_DEF;
				break;
			case E_JUNZHUSTATE.E_CHENGHAO:
				temoObjClickName.name = "chenghaoback";
				m_UIChenghao.MYClick(temoObjClickName);
				m_JunzhuState = E_JUNZHUSTATE.E_DEF;
				break;
			}
		}
		else if(ui.name.IndexOf("Skill") != -1)
		{
			int index = int.Parse(ui.name.Substring(5,1));
//			if(m_XiangxiPanelLeft.activeSelf)
//			{
//				m_XiangxiPanelLeft.SetActive(false);
//				m_PlayerLeft.SetActive(true);
//			}
			switch(index)
			{
			case 0:
				m_TierGear.SetActive(true);
				m_PlayerLeft.SetActive(false);
				m_PlayerRight.SetActive(false);
				break;
			case 1:
				if(FunctionOpenTemp.IsHaveID(500007))
				{
					Global.ScendNull(ProtoIndexes.C_HEROSKILLUP_DATA_REQ);
				}
				else
				{
					ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(500007).m_sNotOpenTips);
				}
				break;
			case 2:
				if(FunctionOpenTemp.IsHaveID(500000))
				{
					SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_TALENT_INFO_REQ);
				}
				else
				{
					ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(500000).m_sNotOpenTips);
				}
				if (FreshGuide.Instance().IsActive(100280) && TaskData.Instance.m_TaskInfoDic[100280].progress >= 0)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
				break;
			case 3:
				if(FunctionOpenTemp.IsHaveID(500010))
				{
					FuWenData.Instance.OpenFuWen();
					m_PlayerLeft.SetActive(false);
					m_PlayerRight.SetActive(false);
				}
				else
				{
					ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(500010).m_sNotOpenTips);
				}
				break;
			}
		}
		else if(ui.name.IndexOf("R_Xiangxi") != -1)
		{
			if(m_UIChenghao != null && m_UIChenghao.gameObject.activeSelf)
			{
				GameObject tempObj = new GameObject();
				tempObj.name = "chenghaoback";
				m_UIChenghao.MYClick(tempObj);
			}
			m_UILbaelXiangxiWQSHJS.text = MyColorData.getColorString(6, m_JunZhuInfoCopy.wqSH);
			m_UILbaelXiangxiWQBJL.text = MyColorData.getColorString(6, m_JunZhuInfoCopy.wqBJL + "%");
			m_UILbaelXiangxiWQBJJS.text = MyColorData.getColorString(6, m_JunZhuInfoCopy.wqBJ);
			m_UILbaelXiangxiJNSHJS.text = MyColorData.getColorString(7, m_JunZhuInfoCopy.jnSH);
			m_UILbaelXiangxiJNBJL.text = MyColorData.getColorString(7, m_JunZhuInfoCopy.jnBJL + "%");
			m_UILbaelXiangxiJNBJJS.text = MyColorData.getColorString(7, m_JunZhuInfoCopy.jnBJ);
			m_UILbaelXiangxiWQSHDK.text = MyColorData.getColorString(6, m_JunZhuInfoCopy.wqJM);
			m_UILbaelXiangxiWQBJDK.text = MyColorData.getColorString(6, m_JunZhuInfoCopy.wqRX);
			m_UILbaelXiangxiJNSHDK.text = MyColorData.getColorString(7, m_JunZhuInfoCopy.jnJM);
			m_UILbaelXiangxiJNBJDK.text = MyColorData.getColorString(7, m_JunZhuInfoCopy.jnRX);
			m_XiangxiPanelLeft.SetActive(true);
			m_PlayerLeft.SetActive(false);
			m_PlayerRight.SetActive(false);
			m_JunzhuState = E_JUNZHUSTATE.E_XIANGXI;
			//			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
		}
		else if(ui.name.IndexOf("HeroArrBack") != -1)
		{
			m_XiangxiPanelLeft.SetActive(false);
			m_PlayerLeft.SetActive(true);
			m_PlayerRight.SetActive(true);
		}
		else if(ui.name.IndexOf("skillback") != -1)
		{
			m_UIHeroSkill.clear();
			m_UIHeroSkill.gameObject.SetActive(false);
			m_PlayerLeft.SetActive(true);
			m_PlayerRight.SetActive(true);
		}
		else if(ui.name.IndexOf("EquipTZBack") != -1)
		{
			m_TierGear.SetActive(false);
			m_PlayerLeft.SetActive(true);
			m_PlayerRight.SetActive(true);
		}
		else if(ui.name.IndexOf("ArrBack") != -1)
		{
			m_XiangxiPanelLeft.SetActive(false);
			m_PlayerLeft.SetActive(true);
			m_PlayerRight.SetActive(true);
		}
		else if(ui.name.IndexOf("Chenghao") != -1)
		{
//			public const short C_GET_CUR_CHENG_HAO = 5101;//客户端请求当前君主使用的称号
//			public const short S_GET_CUR_CHENG_HAO = 5102;//服务器返回当前君主使用的称号，消息体ChengHaoData
//			public const short C_LIST_CHENG_HAO = 5111;//获取称号列表
//			public const short S_LIST_CHENG_HAO = 5112;//服务器返回称号列表，消息体ChengHaoList
//			public const short C_USE_CHENG_HAO = 5121;//客户端选择称号
			Global.ScendNull(ProtoIndexes.C_LIST_CHENG_HAO);
		}
	}
	
	private void OnMiBaoRectIconClick(GameObject go)
	{
		int tempIndex = int.Parse(go.name.Substring(9, go.name.Length - 9));
		if (m_iMiBaoRectData[tempIndex] != -1)
		{
			m_MibaoPinzhi[m_iMiBaoRectData[tempIndex]].color = Color.white;
			m_MibaoIconS[m_iMiBaoRectData[tempIndex]].color = Color.white;
			m_iMiBaoRectData[tempIndex] = -1;
			iconSampleManagerList[tempIndex].FgSprite.gameObject.SetActive(false);
			iconSampleManagerList[tempIndex].QualityFrameSprite.gameObject.SetActive(false);
			upAddData();
		}
	}
	
	private void OnMiBaoIconClick(GameObject go)
	{
		IconSampleManager tempManager = TransformHelper.GetComponentInParent<IconSampleManager>(go.transform);
		int tempIndex = int.Parse(tempManager.gameObject.name.Substring(9, tempManager.gameObject.name.Length - 9));
		for (int i = 0; i < 3; i++)
		{
			if (m_iMiBaoRectData[i] == tempIndex)
			{
				return;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_iMiBaoRectData[i] == -1)
			{
				//					public List<UISprite> m_MibaoPinzhi = new List<UISprite>();
				//                //					public List<UISprite> m_MibaoIconS = new List<UISprite>();
				//                m_MibaoPinzhi[tempIndex].color = Color.grey;
				//                m_MibaoIconS[tempIndex].color = Color.grey;
				//                m_iMiBaoRectData[i] = tempIndex;
				//                iconSampleManagerList[i].FgSprite.spriteName = m_MiBaoInfo.mibaoInfo[tempIndex].miBaoId.ToString();
				//                iconSampleManagerList[i].FgSprite.gameObject.SetActive(true);
				//                iconSampleManagerList[i].QualityFrameSprite.spriteName = "pinzhi" + MiBaoXmlTemp.getMiBaoXmlTempById(m_MiBaoInfo.mibaoInfo[tempIndex].miBaoId).pinzhi;
				//                iconSampleManagerList[i].QualityFrameSprite.gameObject.SetActive(true);
				//                upAddData();
				break;
			}
		}
	}
	
	void OnGUI()
	{
		
	}
	
	public void upAddData()
	{
		for(int i = 0; i < m_iAddData.Length; i ++)
		{
			m_iAddData[i] = 0;
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
 
	List<string> m_role_model_path = new List<string>();
 
	void ShowPlayer()
	{
		m_role_model_path.Clear();
		
		//// TODO: replace with new res.
		//{
		//    // HaoJie
		//    m_role_model_path.Add( ModelTemplate.GetResPathByModelId( -4 ) );
		
		//    // RuYa
		//    m_role_model_path.Add( ModelTemplate.GetResPathByModelId( -5 ) );
		
		//    // YuJie
		//    m_role_model_path.Add( ModelTemplate.GetResPathByModelId( -6 ) );
		
		//    // LuoLi
		//    m_role_model_path.Add( ModelTemplate.GetResPathByModelId( -7 ) );
		//}
		
 
	}
	public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		m_PlayerModel = Instantiate(p_object) as GameObject;
		
		m_PlayerModel.SetActive( true );
		
		m_PlayerModel.transform.parent = m_PlayerParent.transform;
		m_PlayerModel.name = p_object.name;

		GameObjectHelper.SetGameObjectLayer(m_PlayerModel.transform.parent.gameObject, m_PlayerModel);
     
		m_PlayerModel.transform.localPosition = new Vector3(0, -223, -100);
		m_PlayerModel.GetComponent<NavMeshAgent>().enabled = false;
		
		m_PlayerModel.GetComponent<Animator>().Play("zhuchengidle");
		
		m_PlayerModel.transform.localScale = new Vector3(190,190,190);
		
		m_PlayerModel.transform.rotation = new Quaternion(0,163,0,0);

//		m_PlayerModel.GetComponent<PlayerWeaponManagerment>().ShowWeapon(3);
	}
	public void setOpenWeapon(int index)
	{
		m_SkillLock[index].gameObject.SetActive(false);
		m_SkillIcon[index].gameObject.SetActive(true);
		m_SkillLabel[index].gameObject.SetActive(true);
	}

	public void closeFuwen()
	{
		if(!m_UITianfu.gameObject.activeSelf)
		{
			if(!m_UIChenghao.gameObject.activeSelf)
			{
				m_PlayerRight.SetActive(true);
			}
			m_PlayerLeft.SetActive(true);
		}
	}

	public void EndDelegate()
	{
        m_Equip.SetActive(true);
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
		                        LoadCallback );
	}
}
