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

	public UILabel Money;

	public SkillInfo m_Skill;

	public List<miBaoskilltemp> mmiBaoskilltempList = new List<miBaoskilltemp> ();//尚未激活秘宝

	public static NewMiBaoSkill mMiBaoData;

	public UILabel CHoseRemaind;

	public bool COmeMiBaoUI = false;

	public GameObject SaveBtn;

	public static NewMiBaoSkill Instance ()
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
		SaveId = ski_id;
		S_Type = SKillType;
		if (!COmeMiBaoUI)
		{
			CHoseRemaind.gameObject.SetActive (true);

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
				Debug.Log ("mibao data back !");
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
					Debug.Log ("激活技能成功");
				}
				else{
					Debug.Log ("碎片不足");
				}
			//	UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillTemp,EffectIdTemplate.GetPathByeffectId(100178));
				return true;
			}

			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      秘宝保存返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoSelectResp Sava_MiBao = new MibaoSelectResp();
				
				t_qx.Deserialize(t_stream, Sava_MiBao, Sava_MiBao.GetType());
				
				Debug.Log ("Sava_MiBao.success = "+Sava_MiBao.success);
				if(Sava_MiBao.success == 1)//保存成功
				{
					skillZuHeId = Sava_MiBao.zuheSkill;
					Debug.Log ("Sava_MiBao.type:" + Sava_MiBao.type);
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
//						GameObject tiaozhanObj = GameObject.Find ("GeneralChallengePage");
//						Debug.Log ("GeneralChallengePage:" + tiaozhanObj);
//						if (tiaozhanObj != null)
//						{
//							GeneralTiaoZhan tiaozhan = tiaozhanObj.GetComponent<GeneralTiaoZhan> ();
//							
//							tiaozhan.RefreshMiBaoSkillInfo (skillZuHeId);
//							
//							YinDaoCol ();
//						}
						
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.HY_TreSend ):
					{
						HY_UIManager.Instance ().ShowOrClose ();
						
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
						PvpPage.pvpPage.pvpResp.pvpInfo.zuheId = Sava_MiBao.zuheSkill;
						PvpPage.pvpPage.PvpActiveState (true);
						PvpPage.pvpPage.DefensiveSetUp ();
						
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
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Cailiao) :
					{
						//EnterYouXiaBattle.GlobleEnterYouXiaBattle.SecondShowOrClose();
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Jingpo) :
					{
						//EnterYouXiaBattle.GlobleEnterYouXiaBattle.SecondShowOrClose();
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_WanbiGuizhao):
					{
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_ZongHengLiuHe):
					{
						NewYXUI.Instance().m_You_XiaInfo.zuheId = skillZuHeId;
						NewYXUI.Instance().ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.LueDuo_GongJi):
					{
						GameObject tiaozhanObj = GameObject.Find ("GeneralChallengePage");
						
						if (tiaozhanObj != null)
						{
							GeneralTiaoZhan tiaozhan = tiaozhanObj.GetComponent<GeneralTiaoZhan> ();
							tiaozhan.RefreshMiBaoSkillInfo (skillZuHeId);
						}
						
						break;
					}
					default:
						break;
					}
					
					Destroy(this.gameObject);
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
	public void InitData()
	{
		Money.text = JunZhuData.Instance().m_junzhuInfo.jinBi.ToString();

		if(m_Skill_MiBaoInfo.skillList != null)
		{
			Debug.Log ("111SaveId = "+SaveId);

			if(SaveId > 0)
			{
				Debug.Log ("1aaaaaa ");
				ShowBeChoosed_MiBao(SaveId,true);
			}
			else
			{   if(m_Skill_MiBaoInfo.skillList.Count < 7)
				{
					ShowBeChoosed_MiBao( m_Skill_Gbj[m_Skill_MiBaoInfo.skillList.Count].GetComponent<miBaoskilltemp>().SKill_id,false);
				}
				else
				{
					ShowBeChoosed_MiBao( m_Skill_Gbj[m_Skill_MiBaoInfo.skillList.Count-1].GetComponent<miBaoskilltemp>().SKill_id,true);
				}
				Debug.Log ("bbbbbbbbbb ");
				Debug.Log("activeZuheId = "+m_Skill_MiBaoInfo.skillList[m_Skill_MiBaoInfo.skillList.Count -1].activeZuheId);
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
							UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,m_Skill_Gbj[i],EffectIdTemplate.GetPathByeffectId(100166));
						}
					}
				}
			}
			isMiBaoSkillActive = false;
		}
		else
		{
			Debug.Log ("cccccccccc ");
			ShowBeChoosed_MiBao(1,false);
		}
	
	}

	public int SaveId;

	public int S_Type;

	public bool IsActived = false;
	int Acmibaonuber = 0;

	int NeedAcmibaonuber = 0;
	public void ShowBeChoosed_MiBao(int  m_Skllinfo_id,bool Isactive)
	{
		Debug.Log ("m_Skllinfo_id = "+m_Skllinfo_id);
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
		
		DescIdTemplate mDesc1 = DescIdTemplate.getDescIdTemplateByNameId(m_MiBaoSkillTemp.skillDesc);
		DescIdTemplate mDesc2 = DescIdTemplate.getDescIdTemplateByNameId(mMiBaoSkillTemp.skillDesc);
		DescIdTemplate mDesc3 = DescIdTemplate.getDescIdTemplateByNameId(m_MiBaoSkillTemp.skill2Desc);
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
			if (!COmeMiBaoUI)
			{
				SaveBtn.SetActive(true);
			}
			else
			{
				SaveBtn.SetActive(false);
			}
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
	public void ActiveSkillBtn()
	{
		if(Acmibaonuber >= NeedAcmibaonuber)
		{
			bool iscanactive = false;

			Debug.Log("SaveId = "+SaveId);

			int x_n = SaveId;

			if(x_n < 2)
			{
				x_n = 2;
			}

			miBaoskilltemp m_miBaoskilltemp = m_Skill_Gbj [x_n-2].GetComponent<miBaoskilltemp> ();
			if(m_Skill_MiBaoInfo.skillList != null)
			{
				for(int j = 0; j < m_Skill_MiBaoInfo.skillList.Count; j ++)
				{
					if(m_Skill_MiBaoInfo.skillList[j].activeZuheId == m_miBaoskilltemp.SKill_id)
					{
						iscanactive = true;
						break;
					}
				}
			}else
			{
				if(SaveId == 1)
				{
					iscanactive = true;
				}else
				{
					iscanactive = false;
				}
			}

			if(!iscanactive)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),DontActive);
				return;
			}
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),MiBaoUpStarLoadBack);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenLockLoadBack);
		}
	}

	void MiBaoUpStarLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str = "";

		titleStr = "激活";
		str ="\r\n"+"确定激活此技能吗？";

		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancel,confirm,SendStarUpInfo);
			
	}
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		string str1 = "";

		str1 = "\r\n"+"秘宝不足，无法激活！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
	
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null,null
		             );
	}
	void DontActive(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		string str1 = "";
		
		str1 = "\r\n"+"先激活前面的秘技！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null,null
		             );
	}
	void SendStarUpInfo(int i)
	{
		if( i == 2)
		{
			MiBaoDealSkillReq mMiBaoDealSkillReq = new MiBaoDealSkillReq ();
			
			MemoryStream miBaoStream = new MemoryStream ();
			
			QiXiongSerializer MiBaoSer = new QiXiongSerializer ();

			//Debug.Log("SaveId222 = "+SaveId);

			mMiBaoDealSkillReq.zuheId = SaveId;
			Debug.Log("SaveId222 = "+mMiBaoDealSkillReq.zuheId);
			MiBaoSer.Serialize (miBaoStream,mMiBaoDealSkillReq);
			byte[] t_protof;
			t_protof = miBaoStream.ToArray();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.MIBAO_DEAL_SKILL_REQ,ref t_protof);
		}
	}

	public void SendSaveMiBaoMasege( )//保存秘宝技能
	{
		MibaoSelect Mibaoid = new MibaoSelect ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
		
		Mibaoid.type = S_Type;
		
		Mibaoid.zuheSkill = SaveId;
		
		MiBaoSer.Serialize (miBaoStream, Mibaoid);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT, ref t_protof);
		
		//CityGlobalData.m_MiBaoSkillId = SaveId;
	}

	public void Buy_Money()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
	}
	public void CloseBtn()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
