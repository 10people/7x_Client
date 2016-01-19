﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewMiBaoManager : MonoBehaviour ,SocketListener {

	public GameObject First_MiBao_UI; //秘宝UI
	
	public GameObject MiBao_TempInfo; //秘宝信息
	
	public GameObject MiBao_ZhanLiInfo; //秘宝战力信息界面

	public MibaoInfoResp m_MiBaoInfo;

	public UILabel Money;

	public UILabel YuanBao;

	public UILabel ZhanLi;

	public UILabel SkillName;

	public UILabel SkillInstruction;

	public UILabel Num_of_ActiveMiBao;

	public UILabel Remaind;

	public UILabel DisactiveLabel;

	public UISprite SkillIcon;

	public UISprite ActiveMibaoBackGroud;

	public UISprite DisActiveMibaoBackGroud;

	public GameObject UIsilder;

	public GameObject AllMiBaoActive;

	public List<MibaoInfo> ActiveMiBaoList = new List<MibaoInfo> (); //已经激活秘宝

	public List<MibaoInfo> DisActiveMiBaoList = new List<MibaoInfo> ();//尚未激活秘宝

	public GameObject new_MiBaoTemp; //秘宝战力信息界面

	public GameObject Art; //可以合成技能的红点

	public static NewMiBaoManager mMiBaoData;

	public List<MBTemp> mMBTempList = new List<MBTemp>();

	public static NewMiBaoManager Instance ()
	{
		if (!mMiBaoData)
		{
			mMiBaoData = (NewMiBaoManager)GameObject.FindObjectOfType (typeof(NewMiBaoManager));
		}
		
		return mMiBaoData;
	}

	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		mMiBaoData = null;
	}

	void Start () {
	
		TaskData.Instance.m_DestroyMiBao = true;

	}

	void OnEnable()
	{
		Init ();
	}

	void Update () {
	
	}
	public void Init()
	{
		
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			if(Global.m_sPanelWantRun == "Mibaoskill")
			{
				ShowAllSkillS_Btn();
				
				Global.m_sPanelWantRun = "";
			}
		}
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
	}
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_INFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());

				m_MiBaoInfo = MiBaoInfo;

				mtime = m_MiBaoInfo.remainTime;

//				Debug.Log("秘宝信息返回！");

				InitData();
				StopCoroutine("showTime");
				StartCoroutine("showTime");

				return true;
			}
			case ProtoIndexes.S_BUY_MiBaoPoint: //请求tongbi购买信息 BuyTongbiResp
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				BuyMibaoPointResp mBuyMibaoPointResp = new BuyMibaoPointResp();
				
				t_qx.Deserialize(t_stream, mBuyMibaoPointResp, mBuyMibaoPointResp.GetType());
				if (mBuyMibaoPointResp.result == 0)
				{
					m_MiBaoInfo.levelPoint = 10;
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().Init();
				}
				return true;
			}

			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	int mtime;
	IEnumerator showTime()
	{
		VipTemplate mVip = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance().m_junzhuInfo.vipLv);
		
		int MaxPoint = mVip.MiBaoLimit;

		while (mtime > 0) {
			yield return new WaitForSeconds (1.0f);
			mtime -= 1;
			m_MiBaoInfo.remainTime = mtime;
		}
		if(MaxPoint <= m_MiBaoInfo.levelPoint)
		{
			StopCoroutine("showTime");
		}
		else
		{
			m_MiBaoInfo.levelPoint += 1;
			if(mtime <=0)
			{
				mtime = (int )CanshuTemplate.GetValueByKey("ADD_MIBAODIANSHU_INTERVAL_TIME");
			}

			StopCoroutine("showTime");
			
			StartCoroutine("showTime");
		}
	}
	public void InitUI()
	{
		Money.text = JunZhuData.Instance().m_junzhuInfo.jinBi.ToString();
		
		YuanBao.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
		
		ZhanLi.text = JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString();

		UISlider mSlider = UIsilder.GetComponent<UISlider>();

		if(m_MiBaoInfo.skillList == null|| m_MiBaoInfo.skillList.Count == 0 )
		{
			MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(1);

			string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);

			SkillName.text = mName;

			DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoskill.briefDesc);

			string []s = mDesc.description.Split('#');
			string mRul = "";
			for (int i = 0; i < s.Length; i++)
			{
				mRul += s[i]+"\r\n";	
			}

			SkillInstruction.text = mRul;

			Num_of_ActiveMiBao.text = ActiveMiBaoList.Count.ToString()+"/"+ mMiBaoskill.needNum.ToString();

			mSlider.value = (float)(ActiveMiBaoList.Count) / (float)(mMiBaoskill.needNum);

			SkillIcon.spriteName = mMiBaoskill.icon;

			AllMiBaoActive.SetActive(false);

			if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
			{
				Art.SetActive(true);
			}
			else
			{
				Art.SetActive(false);
			}
		}
		else
		{
			int MaxId = 0;
			if(m_MiBaoInfo.skillList.Count >= 7)
			{
				UIsilder.SetActive(false);
				MaxId = 6;
				AllMiBaoActive.SetActive(true);
			}
			else
			{
				UIsilder.SetActive(true);

				AllMiBaoActive.SetActive(false);

				for(int i = 0; i < m_MiBaoInfo.skillList.Count; i++)
				{
					//Debug.Log ("m_MiBaoInfo.skillList[i].activeZuheId = "+m_MiBaoInfo.skillList[i].activeZuheId);
					if(m_MiBaoInfo.skillList[i].activeZuheId > MaxId)
					{
						MaxId = m_MiBaoInfo.skillList[i].activeZuheId;//找到最大值
					}
				}
				MaxId  += 1  ;
			}

			MiBaoSkillTemp mMiBaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(MaxId);
			
			string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoskill.nameId);
			
			SkillName.text = mName;
			
			DescIdTemplate mDesc = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoskill.briefDesc);
			
			SkillIcon.spriteName = mMiBaoskill.icon;
			string []s = mDesc.description.Split('#');
			string mRul = "";
			for (int i = 0; i < s.Length; i++)
			{
				mRul += s[i]+"\r\n";	
			}
			
			SkillInstruction.text = mRul;

			Num_of_ActiveMiBao.text = ActiveMiBaoList.Count.ToString()+"/"+ mMiBaoskill.needNum.ToString();
			
			mSlider.value = (float)(ActiveMiBaoList.Count) / (float)(mMiBaoskill.needNum);
			if(ActiveMiBaoList.Count >= mMiBaoskill.needNum)
			{
				Art.SetActive(true);
			}
			else
			{
				Art.SetActive(false);
			}
		}
		ShowMiBaoYInDao ();
	}
	/// <summary>
	/// IShow yindao
	/// </summary>
	/// 
	void ShowMiBaoYInDao()
	{
		if(FreshGuide.Instance().IsActive(100170)&& TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
		{
			Debug.Log("choose one mibao ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100170];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			return;
		}
		if(FreshGuide.Instance().IsActive(100330)&& TaskData.Instance.m_TaskInfoDic[100330].progress >= 0)
		{
			Debug.Log("Make one mibao ");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100330];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			return;
		}
	}
	public void InitData()
	{
		ActiveMiBaoList.Clear ();
		DisActiveMiBaoList.Clear ();

		//Debug.Log ("m_MiBaoInfo.miBaoList.Count = "+m_MiBaoInfo.miBaoList.Count);

		for(int i = 0 ; i < m_MiBaoInfo.miBaoList.Count; i++)
		{
			if(m_MiBaoInfo.miBaoList[i].level > 0)
			{
				ActiveMiBaoList.Add(m_MiBaoInfo.miBaoList[i]);
			}
			else
			{
				DisActiveMiBaoList.Add(m_MiBaoInfo.miBaoList[i]);
			}
		}
		for(int i = 0 ; i < ActiveMiBaoList.Count; i++)
		{
			for(int j = i+1 ; j < ActiveMiBaoList.Count; j++)
			{
				if(ActiveMiBaoList[i].star < ActiveMiBaoList[j].star)
				{
					MibaoInfo mbTemp = ActiveMiBaoList[i];

					ActiveMiBaoList[i] = ActiveMiBaoList[j];
					
					ActiveMiBaoList[j] = mbTemp;
				}
			}
		}
		for(int i = 0 ; i < DisActiveMiBaoList.Count; i++)
		{
			MiBaoSuipianXMltemp mMiBaosuipian1 = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(DisActiveMiBaoList[i].tempId);

			for(int j = i+1 ; j < DisActiveMiBaoList.Count; j++)
			{
				MiBaoSuipianXMltemp mMiBaosuipian2 = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(DisActiveMiBaoList[j].tempId);

				if(DisActiveMiBaoList[i].suiPianNum < DisActiveMiBaoList[j].suiPianNum )
				{
					if(DisActiveMiBaoList[i].suiPianNum < mMiBaosuipian1.hechengNum)
					{
						MibaoInfo mbTemp = DisActiveMiBaoList[i];
						
						DisActiveMiBaoList[i] = DisActiveMiBaoList[j];
						
						DisActiveMiBaoList[j] = mbTemp;
					}
					else
					{
						if(DisActiveMiBaoList[j].suiPianNum >= mMiBaosuipian2.hechengNum)
						{
							MibaoInfo mbTemp = DisActiveMiBaoList[i];
							
							DisActiveMiBaoList[i] = DisActiveMiBaoList[j];
							
							DisActiveMiBaoList[j] = mbTemp;
						}
					}

				}
				else
				{
					if(DisActiveMiBaoList[i].suiPianNum >= DisActiveMiBaoList[j].suiPianNum )
					{
						if(DisActiveMiBaoList[i].suiPianNum < mMiBaosuipian1.hechengNum && DisActiveMiBaoList[j].suiPianNum >= mMiBaosuipian2.hechengNum)
						{
							MibaoInfo mbTemp = DisActiveMiBaoList[i];
							
							DisActiveMiBaoList[i] = DisActiveMiBaoList[j];
							
							DisActiveMiBaoList[j] = mbTemp;
						}
					}
				}
			}
		}
		InitUI ();

		InistanceMiBaoItem ();
		SaodangBack ();
	}
	void SaodangBack()
	{
		if(MiBao_TempInfo.activeInHierarchy)
		{
			foreach(MibaoInfo minfo in m_MiBaoInfo.miBaoList )
			{
				if(minfo.miBaoId == MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowmMiBaoinfo.miBaoId)
				{
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowmMiBaoinfo = minfo;
					MiBao_TempInfo.GetComponent<MiBaoDesInfo>().ShowSuipian();
					Debug.Log("RefreshMiBao");
				}
			}
		}
	}
	public float dis_x = 120f;

	public float dis_y = 120f;

	public void InistanceMiBaoItem()
	{
		//Debug.Log ("ActiveMiBaoList.Count = "+ActiveMiBaoList.Count);

		//Debug.Log ("ActiveMiBaoList.Count = "+DisActiveMiBaoList.Count);

		foreach(MBTemp m in mMBTempList)
		{
			Destroy(m.gameObject);
		}
		mMBTempList.Clear ();
		for(int i = 0 ; i < ActiveMiBaoList.Count; i++)
		{
			GameObject mMiBaotep = Instantiate(new_MiBaoTemp) as GameObject;
			
			mMiBaotep.SetActive(true);

			mMiBaotep.transform.parent = new_MiBaoTemp.transform.parent;

			int m_x = (i%4);

			int m_y = (int)(i/4);

			mMiBaotep.transform.localPosition = new Vector3(-180+m_x*dis_x,-m_y*dis_y+150,0);

			mMiBaotep.transform.localScale = Vector3.one;

			MBTemp mMBTemp = mMiBaotep.GetComponent<MBTemp>();

			mMBTemp.mMiBaoinfo = ActiveMiBaoList[i];
			mMBTempList.Add(mMBTemp);
			mMBTemp.Init();
		}
		if(ActiveMiBaoList.Count >= 21)
		{
			ActiveMibaoBackGroud.transform.localPosition = new Vector3(0,150-(6-1)*60,0);
			
			ActiveMibaoBackGroud.SetDimensions(486,120*6);

			DisactiveLabel.gameObject.SetActive(false);

			DisActiveMibaoBackGroud.gameObject.SetActive(false);
		}
		else
		{
			DisactiveLabel.gameObject.SetActive(true);

			DisActiveMibaoBackGroud.gameObject.SetActive(true);

			if(ActiveMiBaoList.Count == 0)
			{
				ActiveMibaoBackGroud.gameObject.SetActive(false);
				DisactiveLabel.gameObject.transform.localPosition = new Vector3(-150,-50,0);
			}
			else
			{
				int n = (int)((ActiveMiBaoList.Count-1)/4)+1;

				DisactiveLabel.gameObject.transform.localPosition = new Vector3(-150,-50 -dis_y*n,0);

				ActiveMibaoBackGroud.transform.localPosition = new Vector3(0,150-(n-1)*60,0);

				ActiveMibaoBackGroud.SetDimensions(486,120*n);
			}
		}

		float Sprite_y = 0;
		for(int i = 0 ; i < DisActiveMiBaoList.Count; i++)
		{
			GameObject mMiBaotep = Instantiate(new_MiBaoTemp) as GameObject;
			
			mMiBaotep.SetActive(true);
			
			mMiBaotep.transform.parent = new_MiBaoTemp.transform.parent;
			
			int m_x = (i%4);
			
			int m_y = (int)(i/4);
			int n = 0;
			if(ActiveMiBaoList.Count > 0)
			{
				n = (int)((ActiveMiBaoList.Count-1)/4)+1;
			}
			mMiBaotep.transform.localPosition = new Vector3(-180+m_x*dis_x,-m_y*(dis_y+10)+150 -(n*120+60),0);
			if( i == 0)
			{
				Sprite_y = -m_y*dis_y+150 -n*120+60;
			}
			mMiBaotep.transform.localScale = Vector3.one;
			
			MBTemp mMBTemp = mMiBaotep.GetComponent<MBTemp>();
			
			mMBTemp.mMiBaoinfo = DisActiveMiBaoList[i];
			mMBTempList.Add(mMBTemp);
			mMBTemp.Init();
		}
		if(DisActiveMiBaoList.Count > 0)
		{
			int n = (int)((DisActiveMiBaoList.Count-1)/4)+1;

			DisActiveMibaoBackGroud.transform.localPosition = new Vector3(0,Sprite_y-(n-1)*70 - 120,0);
			
			DisActiveMibaoBackGroud.SetDimensions(486,140*n);
		}
		else
		{
			DisactiveLabel.gameObject.SetActive(false);
			DisActiveMibaoBackGroud.gameObject.SetActive(false);
		}
	}
	public void Buy_Money()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
	}
	public void Buy_YuanBao()
	{
		MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
        //		QXTanBaoData.Instance().CheckFreeTanBao();
    }
	public void GoPropertyShow()
	{
		First_MiBao_UI.SetActive (false);
		MiBao_ZhanLiInfo.SetActive (true);
	}
	public void BackToFirstPage(GameObject Nextgme) // 返回首页
	{
		First_MiBao_UI.SetActive (true);
		Nextgme.SetActive (false);
	}
	public void ShowAllSkillS_Btn()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), ChangeMiBaoSkillLoadBack);

	}
	void ChangeMiBaoSkillLoadBack (ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;

		mChoose_MiBao.transform.localPosition = new Vector3(0, -100, 0);

		mChoose_MiBao.transform.localScale = Vector3.one;
		
		NewMiBaoSkill mNewMiBaoSkill = mChoose_MiBao.GetComponent<NewMiBaoSkill>();

		mNewMiBaoSkill.COmeMiBaoUI = true;

		mNewMiBaoSkill.Init ( 0,0 );
		MainCityUI.TryAddToObjectList(mChoose_MiBao);
	}

	public void ShowMiBaoDeilInf( MibaoInfo mMiBaoifon)
	{
		First_MiBao_UI.SetActive (false);
		MiBao_TempInfo.SetActive (true);
		MiBaoDesInfo mMiBaodes = MiBao_TempInfo.GetComponent<MiBaoDesInfo>();
		mMiBaodes.ShowmMiBaoinfo = mMiBaoifon;
		mMiBaodes.InitLevel ();
		mMiBaodes.Init();
	}

	public void HelpBtn()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.MIBAOINSTRCDUTION));
	}
	public void CloseBtn()
	{
		TaskData.Instance.m_DestroyMiBao = false;
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
