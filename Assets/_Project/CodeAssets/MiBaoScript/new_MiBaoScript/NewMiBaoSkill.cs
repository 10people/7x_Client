using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewMiBaoSkill : MonoBehaviour ,SocketListener {

	public MibaoInfoResp m_Skill_MiBaoInfo;

	public UISprite Skill_Icon;

	public UILabel SKillName;

	public UILabel NeedMiBaonum;

	public UISlider mSilder;

	public GameObject[] m_Skill_Gbj;

	public UILabel Instroduction; //激活的技能简介

	public UILabel BeChoose_Lv; //激活的技能简介

	public GameObject DisActiveMiBaoUISkill;

	public UILabel SKill_Instroduction; //主动效果

	public UILabel Be_SKill_Instroduction;//被动效果

	//public UILabel Money;

	public SkillInfo m_Skill;

	public List<miBaoskilltemp> mmiBaoskilltempList = new List<miBaoskilltemp> ();//尚未激活秘宝

	public static NewMiBaoSkill mMiBaoData;

	public UILabel CHoseRemaind;

	public bool COmeMiBaoUI = false;

	public GameObject SaveBtn;

	public GameObject NewAddMiBaoShangZhen;

	public static NewMiBaoSkill Instance()
	{
		if (!mMiBaoData)
		{
			mMiBaoData = (NewMiBaoSkill)GameObject.FindObjectOfType (typeof(NewMiBaoSkill));
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
	

	}

	void Update () {
	
	}
	public void Init(int SKillType, int ski_id)
	{
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		SaveId = ski_id;
		S_Type = SKillType;
		if (!COmeMiBaoUI)
		{
			CHoseRemaind.gameObject.SetActive (true);
			if(FreshGuide.Instance().IsActive(100260)&& TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
			{
				//	Debug.Log("切换秘技)
				
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100260];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			
			}
		}
		else
		{
			SaveBtn.SetActive(false);
			CHoseRemaind.gameObject.SetActive (false);
		}
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
	
	}
	private bool isMiBaoSkillActive = false;
	private int skillZuHeId;
	public int yinDaoId;//百战引导技能判定id
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
				
				m_Skill_MiBaoInfo = MiBaoInfo;
				
				InitData();
//				Debug.Log ("mibao data back !");
				return true;
			}
			case ProtoIndexes.MIBAO_DEAL_SKILL_RESP://m秘宝技能激活或者进阶返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MiBaoDealSkillResp mMiBaoDealSkillResp = new MiBaoDealSkillResp();
				
				t_qx.Deserialize(t_stream, mMiBaoDealSkillResp, mMiBaoDealSkillResp.GetType());
				
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
//				if(UIYindao.m_UIYindao.m_isOpenYindao)
//				{
//					UIYindao.m_UIYindao.CloseUI();
//				}
				if(mMiBaoDealSkillResp.message == 0)
				{
					isMiBaoSkillActive = true;
//					Debug.Log ("激活技能成功");
				}
				else{
//					Debug.Log ("碎片不足");
				}
			//	UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillTemp,EffectIdTemplate.GetPathByeffectId(100178));
				return true;
			}

			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      秘宝保存返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoSelectResp Sava_MiBao = new MibaoSelectResp();
				
				t_qx.Deserialize(t_stream, Sava_MiBao, Sava_MiBao.GetType());
				
//				Debug.Log ("Sava_MiBao.success = "+Sava_MiBao.success);
				if(Sava_MiBao.success == 1)//保存成功
				{
					skillZuHeId = Sava_MiBao.zuheSkill;
//					Debug.Log ("Sava_MiBao.type:" + Sava_MiBao.type);
					yinDaoId = Sava_MiBao.zuheSkill;
					switch(Sava_MiBao.type)
					{
					case (int)(CityGlobalData.MibaoSkillType.PveSend ):
					{
						NewPVEUIManager.Instance().GuanqiaReq.zuheId = skillZuHeId ;
						
						NewPVEUIManager.Instance().ShowMiBaoIcon();

						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.PvpSend ):
					{					
						GeneralChallengePage.gcPage.RefreshMyMiBaoSkillInfo (skillZuHeId);
						QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);
		
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.HY_TreSend ):
					{
						HY_UIManager.Instance().ShowOrClose ();
						
						HYRetearceEnemy.Instance().M_Treas_info.zuheId =  skillZuHeId;
						
						HYRetearceEnemy.Instance().ShowMiBaoSkill();
						
						
						break;
					}

					case (int)(CityGlobalData.MibaoSkillType.PVP_Fangshou ):
					{
//						GameObject baizhanObj = GameObject.Find ("BaiZhanMain");
//						if (baizhanObj != null)
//						{
//							BaiZhanMainPage baizhanMain = baizhanObj.GetComponent<BaiZhanMainPage> ();
//							baizhanMain.baiZhanResp.pvpInfo.zuheId = Sava_MiBao.zuheSkill;
//							baizhanMain.DefensiveSetUp ();
//							baizhanMain.IsOpenOpponent = false;
//						}
//						PvpPage.pvpPage.pvpResp.pvpInfo.zuheId = Sava_MiBao.zuheSkill;
//						PvpPage.pvpPage.PvpActiveState (true);
//						PvpPage.pvpPage.DefensiveSetUp ();
						
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YaBiao_Fangshou ):
					{
						
						
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YaBiao_Gongji) :
					{
						//CarriageMsgManager.Instance.s_YabiaoJunZhuList.gongjiZuHeId = skillZuHeId;
						//CarriageMsgManager.Instance.m_RootManager.m_CarriageUi.RefreshMibaoSkillEffect();
						
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_JinBi) :
					{
						//EnterYouXiaBattle.GlobleEnterYouXiaBattle.SecondShowOrClose();
						foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
						{
							if(m_YXItem.mYouXiaInfo.id == NewYXUI.Instance().big_id)
							{
								m_YXItem.mYouXiaInfo.zuheId = skillZuHeId;
							}
						}
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Cailiao) :
					{
						foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
						{
							if(m_YXItem.mYouXiaInfo.id == NewYXUI.Instance().big_id)
							{
								m_YXItem.mYouXiaInfo.zuheId = skillZuHeId;
							}
						}
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Jingpo) :
					{
						foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
						{
							if(m_YXItem.mYouXiaInfo.id == NewYXUI.Instance().big_id)
							{
								m_YXItem.mYouXiaInfo.zuheId = skillZuHeId;
							}
						}
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_WanbiGuizhao):
					{
						foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
						{
							if(m_YXItem.mYouXiaInfo.id == NewYXUI.Instance().big_id)
							{
								m_YXItem.mYouXiaInfo.zuheId = skillZuHeId;
							}
						}
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_ZongHengLiuHe):
					{
						foreach(YXItem m_YXItem in XYItemManager.initance().YXItemList)
						{
							if(m_YXItem.mYouXiaInfo.id == NewYXUI.Instance().big_id)
							{
								m_YXItem.mYouXiaInfo.zuheId = skillZuHeId;
							}
						}
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.LueDuo_GongJi):
					{
						GeneralChallengePage.gcPage.RefreshMyMiBaoSkillInfo (skillZuHeId);
						
						break;
					}
					default:
						break;
					}
					

				}
				Destroy(this.gameObject);
				return true;
			}
			default: return false;
			}
			
		}else
		{
//			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	public void InitData()
	{
		//Money.text = JunZhuData.Instance().m_junzhuInfo.jinBi.ToString();
		//Debug.Log ("m_Skill_MiBaoInfo.skillList.count  = "+m_Skill_MiBaoInfo.skillList.Count);
		if(m_Skill_MiBaoInfo.skillList != null)
		{
		//Debug.Log ("111SaveId = "+SaveId);

			if(SaveId > 0)
			{
//				Debug.Log ("1aaaaaa ");
				ShowBeChoosed_MiBao(SaveId,true);
				if(!COmeMiBaoUI)
				{
					NewAddMiBaoShangZhen.SetActive(true);
					NewAddMiBaoShangZhen.transform.localPosition = m_Skill_Gbj [SaveId-1].transform.localPosition;
				}
			}
			else
			{   
				if(!COmeMiBaoUI)
				{
					NewAddMiBaoShangZhen.SetActive(true);
					NewAddMiBaoShangZhen.transform.localPosition = m_Skill_Gbj [0].transform.localPosition;
					ShowBeChoosed_MiBao( m_Skill_Gbj[0].GetComponent<miBaoskilltemp>().SKill_id,true);
				}
				else
				{
					if(m_Skill_MiBaoInfo.skillList.Count < 7)
					{
						ShowBeChoosed_MiBao( m_Skill_Gbj[m_Skill_MiBaoInfo.skillList.Count].GetComponent<miBaoskilltemp>().SKill_id,false);
					}
					else
					{
						ShowBeChoosed_MiBao( m_Skill_Gbj[m_Skill_MiBaoInfo.skillList.Count-1].GetComponent<miBaoskilltemp>().SKill_id,true);
					}
				}
			}
			for(int i = 0; i < m_Skill_Gbj.Length; i ++)
			{
				miBaoskilltemp m_miBaoskilltemp = m_Skill_Gbj[i].GetComponent<miBaoskilltemp>();
				
				for(int j = 0; j < m_Skill_MiBaoInfo.skillList.Count; j ++)
				{
					if(m_Skill_MiBaoInfo.skillList[j].activeZuheId == m_miBaoskilltemp.SKill_id)
					{
						m_miBaoskilltemp.mSkillInfo = m_Skill_MiBaoInfo.skillList[j];
						
						m_miBaoskilltemp.IsActive = true;
						
						m_miBaoskilltemp.Init();

						if(isMiBaoSkillActive)
						{
							UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,m_Skill_Gbj[i],EffectIdTemplate.GetPathByeffectId(100166));
						}
					}
				}
			}
			isMiBaoSkillActive = false;
		}
		else
		{
//			Debug.Log ("cccccccccc ");
			ShowBeChoosed_MiBao(1,false);
		}
	
	}

	public int SaveId;
	public int NewSaveId;
	public int S_Type;

	public bool IsActived = false;
	int Acmibaonuber = 0;

	int NeedAcmibaonuber = 0;
	public void ShowBeChoosed_MiBao(int  m_Skllinfo_id,bool Isactive)
	{
//		Debug.Log ("m_Skllinfo_id = "+m_Skllinfo_id);
		NeedAcmibaonuber = 0;
		Acmibaonuber = 0;
		foreach(MibaoInfo minfo in m_Skill_MiBaoInfo.miBaoList)
		{
			if(minfo.level > 0 )
			{
				Acmibaonuber += 1;
			}
		}
		SaveId = m_Skllinfo_id;
		if(Isactive)
		{
			NewSaveId = m_Skllinfo_id;
		}
		IsActived = Isactive;
		for (int i = 0; i < m_Skill_Gbj.Length; i ++)
		{
			miBaoskilltemp m_miBaoskilltemp = m_Skill_Gbj [i].GetComponent<miBaoskilltemp> ();

			if(m_miBaoskilltemp.SKill_id == m_Skllinfo_id  )
			{
				m_miBaoskilltemp.beChoosed = true;

				m_miBaoskilltemp.Be_CHoosed();

			}
		}
		MiBaoSkillLvTempLate m_MiBaoSkillTemp = MiBaoSkillLvTempLate.GetMiBaoSkillLvTemplateByIdAndLevel (m_Skllinfo_id,1);
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (m_Skllinfo_id);
		string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoSkillTemp.nameId);
		
		SKillName.text = mName;
		if(Isactive&&!COmeMiBaoUI)
		{
			string MiBaoSkillName = mName;
			string MiBaoNameShangZhen = " 已上阵！";
			string data = MyColorData.getColorString(5, MiBaoSkillName)+MiBaoNameShangZhen;
			ClientMain.m_UITextManager.createText( data);
		}
		DescIdTemplate mDesc1 = DescIdTemplate.getDescIdTemplateByNameId(m_MiBaoSkillTemp.ZhuDongskillDesc);
		DescIdTemplate mDesc2 = DescIdTemplate.getDescIdTemplateByNameId(m_MiBaoSkillTemp.Desc);
		DescIdTemplate mDesc3 = DescIdTemplate.getDescIdTemplateByNameId(m_MiBaoSkillTemp.BeDongskillDesc);
		Skill_Icon.spriteName = mMiBaoSkillTemp.icon;
		
		SKill_Instroduction.text = mDesc1.description;

		Instroduction.text = mDesc2.description;

		Be_SKill_Instroduction.text = mDesc3.description;

		NeedAcmibaonuber = mMiBaoSkillTemp.needNum;

		if(!Isactive)
		{
			Instroduction.gameObject.SetActive(false);
			DisActiveMiBaoUISkill.SetActive(true);
			mSilder.value = (float)(Acmibaonuber) / (float)(mMiBaoSkillTemp.needNum);
			NeedMiBaonum.text = Acmibaonuber.ToString()+"/"+mMiBaoSkillTemp.needNum.ToString();
			SaveBtn.SetActive(false);
		}
		else
		{
			Instroduction.gameObject.SetActive(true);
//			if (!COmeMiBaoUI)
//			{
//				//SaveBtn.SetActive(true);
//			}
//			else
//			{
//				SaveBtn.SetActive(false);
//			}
			int level = 0;
			for(int j = 0; j < m_Skill_MiBaoInfo.skillList.Count; j ++)
			{
				if(m_Skill_MiBaoInfo.skillList[j].activeZuheId == m_Skllinfo_id)
				{
					level = m_Skill_MiBaoInfo.skillList[j].level;
					break;
				}
			}
			BeChoose_Lv.text = "Lv."+level.ToString();

			DisActiveMiBaoUISkill.SetActive(false);
		}
	}

	public void SendSaveMiBaoMasege( )//保存秘宝技能
	{
//		Debug.Log ("NewSaveId = "+NewSaveId);

		if (NewSaveId <= 0) {
			Destroy (this.gameObject);

		} else {
			MibaoSelect Mibaoid = new MibaoSelect ();
			
			MemoryStream miBaoStream = new MemoryStream ();
			
			QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
			
			Mibaoid.type = S_Type;
			
			Mibaoid.zuheSkill = NewSaveId;
			
			MiBaoSer.Serialize (miBaoStream, Mibaoid);
			byte[] t_protof;
			t_protof = miBaoStream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT, ref t_protof);
		}

	}

	public void CloseBtn()
	{
		if (COmeMiBaoUI) {

			Destroy (this.gameObject);
		} else {
			QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,4);

			MainCityUI.TryRemoveFromObjectList(this.gameObject);
			if(FreshGuide.Instance().IsActive(100260)&& TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
			{
				//	Debug.Log("切换秘技)
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100260];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);

			}
			SendSaveMiBaoMasege ();
		}
	}
}
