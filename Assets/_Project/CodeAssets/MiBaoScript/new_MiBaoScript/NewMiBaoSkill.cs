using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
using System.Linq;
public class NewMiBaoSkill : MonoBehaviour ,SocketListener {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public UILabel Remaind;

	public MibaoInfoResp m_Skill_MiBaoInfo;

	public UISprite Skill_Icon;

	public UISprite SKillName;

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

	public List<miBaoskilltemp> mmiBaoskilltempList = new List<miBaoskilltemp> ();//尚未激活将魂

	public static NewMiBaoSkill mMiBaoData;

	public UILabel CHoseRemaind;

	public bool COmeMiBaoUI = false;

	public GameObject SaveBtn;

	public GameObject NewAddMiBaoShangZhen;

	public List<GameObject> EffectList = new List<GameObject>();

	public delegate void mCloseMiBaoskillDo(int skillid = 0,bool issave = true);

	private mCloseMiBaoskillDo m_mCloseMiBaoskillDo;

	private int m_SaveID;

	public GameObject m_SkillGameObject;

	private List<miBaoskilltemp> miBaoskilltempList = new List<miBaoskilltemp>();
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
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "无双技", 0, 0);
		SocketTool.RegisterSocketListener(this);	
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		mMiBaoData = null;
	}

	void HidAllGameobj(int Index)
	{
		EffectList.ForEach (item => Setactive (item, false));
		EffectList [Index].SetActive (true);

	}
	private void Setactive(GameObject go, bool a)
	{
		go.SetActive (a);
	}

	public void Init(int SKillType, int ski_id, mCloseMiBaoskillDo mmCloseMiBaoskillDo = null)
	{
		if(mmCloseMiBaoskillDo != null )
		{
			m_mCloseMiBaoskillDo = mmCloseMiBaoskillDo ;
		}
		EffectList.ForEach (item=>Setactive(item,false));
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
		SaveId = ski_id;
		S_Type = SKillType;
		if (!COmeMiBaoUI)
		{
			CHoseRemaind.gameObject.SetActive (true);
		}
		else
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
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
			case ProtoIndexes.MIBAO_DEAL_SKILL_RESP://m将魂技能激活或者进阶返回
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

			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      将魂保存返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoSelectResp Sava_MiBao = new MibaoSelectResp();
				
				t_qx.Deserialize(t_stream, Sava_MiBao, Sava_MiBao.GetType());
				
//				Debug.Log ("Sava_MiBao.success = "+Sava_MiBao.success);
				if(Sava_MiBao.success == 1)//保存成功
				{
					m_SaveID = Sava_MiBao.zuheSkill;
					if(m_mCloseMiBaoskillDo != null)
					{
						m_mCloseMiBaoskillDo(m_SaveID,true);
						m_mCloseMiBaoskillDo = null;
					}

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
//						GeneralChallengePage.gcPage.RefreshMyMiBaoSkillInfo (skillZuHeId);
//						QXComData.YinDaoStateController (QXComData.YinDaoStateControl.UN_FINISHED_TASK_YINDAO,100200,5);
		
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
//						GeneralChallengePage.gcPage.RefreshMyMiBaoSkillInfo (skillZuHeId);
						
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
	private void ActiveMiBaoskills(GameObject kills)
	{
		kills.SetActive (true);
	}
	private void InitSkills()
	{
		miBaoskilltempList.Clear ();
		foreach(GameObject eff in EffectList)
		{
			Destroy(eff);
		}
		EffectList.Clear ();

//		Debug.Log ("m_Skill_MiBaoInfo.skillList.count = "+m_Skill_MiBaoInfo.skillList.Count);
		for(int i = 0; i < 7 ; i ++)
		{
			GameObject m_mb_skill = Instantiate(m_SkillGameObject) as GameObject;

			m_mb_skill.SetActive(true);

			m_mb_skill.transform.parent = m_SkillGameObject.transform.parent;

			m_mb_skill.transform.localScale = Vector3.one;

			MiBaoSkillTemp mMiBaoSkill = MiBaoSkillTemp.getMiBaoSkillTempBy_id(i+1);

			m_mb_skill.transform.localPosition = new Vector3(mMiBaoSkill.m_x,mMiBaoSkill.m_y,0);

			miBaoskilltemp m_miBaoskilltemp = m_mb_skill.GetComponent<miBaoskilltemp>();

			m_miBaoskilltemp.SKill_id = i+1;

			if(m_Skill_MiBaoInfo.skillList != null)
			{
				if(m_Skill_MiBaoInfo.skillList.Count >= i+1)
				{
					m_miBaoskilltemp.mSkillInfo = m_Skill_MiBaoInfo.skillList[i];

					m_miBaoskilltemp.Init();
				}else
				{
					m_miBaoskilltemp.IsUnLock();
				}

			}
			EffectList.Add(m_miBaoskilltemp.beChoosedSprite);
			miBaoskilltempList.Add(m_miBaoskilltemp);
		}
	}
	public void InitData()
	{
		InitSkills ();
		if(m_Skill_MiBaoInfo.skillList != null)
		{

			if(SaveId > 0)
			{
				ShowBeChoosed_MiBao(SaveId,true);
				if(!COmeMiBaoUI)
				{
					NewAddMiBaoShangZhen.SetActive(true);
					NewAddMiBaoShangZhen.transform.localPosition = miBaoskilltempList [SaveId-1].gameObject.transform.localPosition;
				}
			}
			else
			{   
				if(!COmeMiBaoUI)
				{
					NewAddMiBaoShangZhen.SetActive(true);
					NewAddMiBaoShangZhen.transform.localPosition = miBaoskilltempList [0].gameObject.transform.localPosition;
					ShowBeChoosed_MiBao( miBaoskilltempList[0].SKill_id,true);
				}
				else
				{
					if(m_Skill_MiBaoInfo.skillList.Count < 7)
					{
						ShowBeChoosed_MiBao( miBaoskilltempList[m_Skill_MiBaoInfo.skillList.Count].SKill_id,false);
					}
					else
					{
						ShowBeChoosed_MiBao( miBaoskilltempList[m_Skill_MiBaoInfo.skillList.Count-1].SKill_id,true);
					}
				}
			}
		}
		else
		{
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
		HidAllGameobj (m_Skllinfo_id -1);
		NeedAcmibaonuber = 0;
		Acmibaonuber = 0;
		int SKillLv = 1;
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
			foreach(SkillInfo mSkill in m_Skill_MiBaoInfo.skillList)
			{
				SKillLv = mSkill.level;
			}
			NewSaveId = m_Skllinfo_id;
		}
//		Debug.Log ("SKillLv= "+SKillLv);
//		Debug.Log ("ll_MiBaoInfo.skillList。count = "+m_Skill_MiBaoInfo.skillList.Count);
		IsActived = Isactive;
		for (int i = 0; i < miBaoskilltempList.Count; i ++)
		{
			if(miBaoskilltempList[i].SKill_id == m_Skllinfo_id  )
			{
				HidAllGameobj(m_Skllinfo_id -1);
			}
		}
		MiBaoSkillLvTempLate m_MiBaoSkillTemp = MiBaoSkillLvTempLate.GetMiBaoSkillLvTemplateByIdAndLevel (m_Skllinfo_id,SKillLv);
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (m_Skllinfo_id);
		string mName  = NameIdTemplate.GetName_By_NameId(mMiBaoSkillTemp.nameId);
		
		SKillName.spriteName = m_MiBaoSkillTemp.id.ToString();
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
			if(Acmibaonuber >=  mMiBaoSkillTemp.needNum)
			{
				Remaind.text = "该技能可以激活";
			}
			else{
				int mibaonuber = mMiBaoSkillTemp.needNum - Acmibaonuber;
				Remaind.text = "再激活"+MyColorData.getColorString(5,mibaonuber.ToString()) +"个将魂可以解锁该技能";
			}
		}
		else
		{
			Instroduction.gameObject.SetActive(true);

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

	public void SendSaveMiBaoMasege( )//保存将魂技能
	{
//		Debug.Log ("NewSaveId = "+NewSaveId);

		if (NewSaveId <= 0) {
			if(m_mCloseMiBaoskillDo != null)
			{
				m_mCloseMiBaoskillDo(m_SaveID,true);
				m_mCloseMiBaoskillDo = null;
			}
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

			SendSaveMiBaoMasege ();
		}
	}
}
