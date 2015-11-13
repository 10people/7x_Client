using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChangeMiBaoSkill : MonoBehaviour,SocketProcessor {

	public UILabel Zhanli;

	public UISprite SkillIcon1;

	public UILabel ZuheName;
	
	public UILabel SkillName;
	
	public UILabel Skillinstruction;
	
	public UILabel instruction;
	
	public UILabel SHUXIng1;
	
	public UILabel SHUXIng2;
	
	public UILabel SHUXIng3;
	
	public UILabel SHUXIng4;
	
	public UILabel SHUXIng_1;
	
	public UILabel SHUXIng_2;
	
	public UILabel SHUXIng_3;
	
	public UILabel SHUXIng_4;

	public MibaoInfoResp my_MiBaoInfo;

	public GameObject SkillGame;

	public GameObject Lock;

	public int newZuHe_id;
	
	public int SkillType; // 1 Pve  2 pvp  3 TRea  4  Res 6 运镖 7 劫镖 8-游侠金币 9-游侠材料 10-游侠精魄 11-掠夺防守 12-掠夺攻击

	public int yinDaoId;//百战引导技能判定id

	private int skillZuHeId;

	public GameObject MibaoSkillRoot;

	public List<MiBaoSkillTEm> MiBaoSkillTEmList = new List<MiBaoSkillTEm>();
	//public List<>

	public static ChangeMiBaoSkill mMiBaoSkillData;

	public string RootName;

	public static ChangeMiBaoSkill Instance ()
	{
		if (!mMiBaoSkillData)
		{
			mMiBaoSkillData = (ChangeMiBaoSkill)GameObject.FindObjectOfType (typeof(ChangeMiBaoSkill));
		}
		
		return mMiBaoSkillData;
	}


	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);	
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}

	void Start () 
	{

	}
	

	void Update () {
	
	}

	public int OldZuhe_id;

	public List<GameObject> skilltemp = new List<GameObject> ();

	public void GetRootName(string Root_Name)
	{
//		Debug.Log ("Root_Name = "+Root_Name);

		RootName = Root_Name;
	}

	public void Init(int MiBaoskillType, int ZH_ID)
	{
//		Debug.Log ("MiBaoskillType = "+MiBaoskillType);
//
//		Debug.Log ("ZH_ID = "+ZH_ID);

		yinDaoId = ZH_ID;

		if (ZH_ID < 1)
		{
			ZH_ID = 1;
		}

		skillZuHeId = ZH_ID;

		SkillIcon1.spriteName = "";

		SkillType = MiBaoskillType;

		if(FreshGuide.Instance().IsActive(100230)&& TaskData.Instance.m_TaskInfoDic[100230].progress >= 0 && SkillType == (int)(CityGlobalData.MibaoSkillType.PveSend))
		{
//			Debug.Log("选中一个秘宝技能");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100230];

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
		}

		Zhanli.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString ();

		newZuHe_id = ZH_ID;

		OldZuhe_id = ZH_ID;

//		foreach(MiBaoSkillTEm mib in MiBaoSkillTEmList)
//		{
//
//			Destroy(mib.gameObject);
//		}

		MiBaoSkillTEmList.Clear ();

		my_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;
	

		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
		{
			GameObject SKill = Instantiate( SkillGame ) as GameObject;

			SKill.SetActive(true);

			SKill.transform.parent = SkillGame.transform.parent;

			SKill.transform.localPosition = SkillGame.transform.localPosition;

			SKill.transform.localScale = Vector3.one;

			MiBaoSkillTEm mMiBaoSkillTEm = SKill.GetComponent<MiBaoSkillTEm>();

			mMiBaoSkillTEm.mMiBaoGroup =  my_MiBaoInfo.mibaoGroup[i];

			if(my_MiBaoInfo.mibaoGroup[i].zuheId == ZH_ID)
			{
				mMiBaoSkillTEm.isChoosed = true;
			}
			mMiBaoSkillTEm.SetChooseType = SkillType;
			mMiBaoSkillTEm.Init();

			MiBaoSkillTEmList.Add(mMiBaoSkillTEm);
		}
		 MibaoSkillRoot.GetComponent<UIGrid> ().repositionNow = true;

		 ReFreshLeftData (newZuHe_id);

	}

	public void ReFreshLeftData(int zh_id)
	{
		// SavaData
		if(zh_id < 1)
		{
			zh_id = 1;
			foreach(MiBaoSkillTEm mMiBaoSkillTEm in MiBaoSkillTEmList)
			{
				
				if(mMiBaoSkillTEm.ZUHE_id == 1)
				{
					mMiBaoSkillTEm.isChoosed = true;
					
					mMiBaoSkillTEm.BoolBeChoose();
				}
			}
		}
		 int Pinzhi = 0;
		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
		{
			if(my_MiBaoInfo.mibaoGroup[i].zuheId == zh_id)
			{
				if(my_MiBaoInfo.mibaoGroup[i].hasActive == 0)
				{
					Lock.SetActive(true);
				}
				if(my_MiBaoInfo.mibaoGroup[i].hasActive == 1)
				{
					Lock.SetActive(false);
				}

				for(int j = 0 ; j < my_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j++)
				{
					if(my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0 &&!my_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
					{
						Pinzhi += 1;
					}
				}
			}
		}

//		Debug.Log("zh_id =  " +zh_id);
//
//		Debug.Log("Pinzhi =  " +Pinzhi);
	
		if(Pinzhi < 2)
		{

			SkillIcon1.spriteName = "";

			MiBaoSkillTemp LockSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi ( zh_id,2 );

			NameIdTemplate m_Name = NameIdTemplate.getNameIdTemplateByNameId (LockSkill.nameId);

			SkillTemplate mskill_temp = SkillTemplate.getSkillTemplateById (LockSkill.skill);

			NameIdTemplate m_Skill_Name = NameIdTemplate.getNameIdTemplateByNameId (mskill_temp.skillName);

			DescIdTemplate Skill_Des = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.shuxingDesc);

			ZuheName.text = m_Name.Name;
			
			SkillName.text = m_Skill_Name.Name;

			DescIdTemplate SkillDes2 = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.shuxingDesc);
			
			Skillinstruction.text = SkillDes2.description;
			
			DescIdTemplate DeliSkillDes2 = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.SkillDetail);
			
			instruction.text = "激活两个及其以上秘宝开启技能";

			if(LockSkill.desc1 != 0)
			{
				DescIdTemplate SHUXIng1Des = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.desc1);
				
				SHUXIng1.text = SHUXIng1Des.description;
				
				SHUXIng_1.text = "0%";
			}
			
			if(LockSkill.desc2 != 0)
			{
				DescIdTemplate SHUXIng2Des = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.desc2);
				
				SHUXIng2.text = SHUXIng2Des.description;
				
				SHUXIng_2.text = "0%";
			}
			else{

				SHUXIng2.gameObject.SetActive(false);

			}
			if(LockSkill.desc3 != 0)
			{
				DescIdTemplate SHUXIng3Des = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.desc3);
				
				SHUXIng3.text = SHUXIng3Des.description;
				
				SHUXIng_3.text = "0%";
			}
			else{
				
				SHUXIng3.gameObject.SetActive(false);
				
			}
			if(LockSkill.desc4 != 0)
			{
				DescIdTemplate SHUXIng4Des = DescIdTemplate.getDescIdTemplateByNameId (LockSkill.desc4);
				
				SHUXIng4.text = SHUXIng4Des.description;
				
				SHUXIng_4.text = "0%";
			}
			else{
				
				SHUXIng4.gameObject.SetActive(false);
				
			}
			return;
		}

		SHUXIng1.gameObject.SetActive(true);
		
		SHUXIng_1.gameObject.SetActive(true);
		
		SHUXIng2.gameObject.SetActive(true);
		
		SHUXIng_2.gameObject.SetActive(true);
		
		SHUXIng3.gameObject.SetActive(true);
		
		SHUXIng_3.gameObject.SetActive(true);
		
		SHUXIng4.gameObject.SetActive(true);
		
		SHUXIng_4.gameObject.SetActive(true);

		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi ( zh_id,Pinzhi );
		
		NameIdTemplate mName = NameIdTemplate.getNameIdTemplateByNameId (mSkill.nameId);
		
		DescIdTemplate mDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.zuheDesc);

		SkillTemplate mskilltemp = SkillTemplate.getSkillTemplateById (mSkill.skill);

		NameIdTemplate Skill_Name = NameIdTemplate.getNameIdTemplateByNameId (mskilltemp.skillName);

		ZuheName.text = mName.Name;

		SkillName.text = Skill_Name.Name;

		SkillIcon1.spriteName = mSkill.icon.ToString();

		DescIdTemplate SkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.shuxingDesc);

		Skillinstruction.text = SkillDes.description;

		DescIdTemplate DeliSkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.SkillDetail);
		
		instruction.text = DeliSkillDes.description;


		if(mSkill.desc1 != 0)
		{
			DescIdTemplate SHUXIng1Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc1);
			
			SHUXIng1.text = SHUXIng1Des.description;
			
			SHUXIng_1.text = mSkill.value1;
		}
		
		if(mSkill.desc2 != 0)
		{
			DescIdTemplate SHUXIng2Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc2);
			
			SHUXIng2.text = SHUXIng2Des.description;
			
			SHUXIng_2.text = mSkill.value2;
		}
		if(mSkill.desc3 != 0)
		{
			DescIdTemplate SHUXIng3Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc3);
			
			SHUXIng3.text = SHUXIng3Des.description;
			
			SHUXIng_3.text = mSkill.value3;
		}
		if(mSkill.desc4 != 0)
		{
			DescIdTemplate SHUXIng4Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc4);
			
			SHUXIng4.text = SHUXIng4Des.description;
			
			SHUXIng_4.text = mSkill.value4;
		}

	}

	public void ResttingBtn() //重置秘宝技能
	{

		if(OldZuhe_id < 1 || OldZuhe_id > 4)
		{
			return;
		}

		foreach(MiBaoSkillTEm mMiBaoSkillTEm in MiBaoSkillTEmList)
		{
			
			if(mMiBaoSkillTEm.ZUHE_id == OldZuhe_id)
			{
				mMiBaoSkillTEm.isChoosed = true;

				mMiBaoSkillTEm.BoolBeChoose();
			}
		}

		ReFreshLeftData( OldZuhe_id);
	}


	public void SaveSkill()
	{
		Debug.Log ("SkillType = " +SkillType);

		Debug.Log ("newZuHe_id = " +newZuHe_id);

		SendSaveMiBaoMasege (SkillType,newZuHe_id);
	}


	public void SendSaveMiBaoMasege(int Type,int zuhe )//保存秘宝技能
	{
		for(int i = 0 ; i < my_MiBaoInfo.mibaoGroup.Count; i++)
		{
			if(my_MiBaoInfo.mibaoGroup[i].zuheId == zuhe)
			{
				if(my_MiBaoInfo.mibaoGroup[i].hasActive == 0)
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenLockLoadBack);
					return;
				}
			}
		}

		MibaoSelect Mibaoid = new MibaoSelect ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();

		Mibaoid.type = Type;

		Mibaoid.zuheSkill = zuhe;

		MiBaoSer.Serialize (miBaoStream,Mibaoid);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT,ref t_protof);

	}
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"技能未解锁，不能保存！";
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null,null);
	}

	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{

			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      318 还没修改协议 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoSelectResp Sava_MiBao = new MibaoSelectResp();
				
				t_qx.Deserialize(t_stream, Sava_MiBao, Sava_MiBao.GetType());
				

				if(Sava_MiBao.success == 1)//保存成功
				{
					Debug.Log ("Sava_MiBao.success");
					skillZuHeId = Sava_MiBao.zuheSkill;
					Debug.Log ("Sava_MiBao.type:" + Sava_MiBao.type);
					yinDaoId = Sava_MiBao.zuheSkill;
					switch(Sava_MiBao.type)
					{
					case (int)(CityGlobalData.MibaoSkillType.PveSend ):
					{
						PveLevelUImaneger.GuanqiaReq.zuheId = skillZuHeId ;

						PveLevelUImaneger.mPveLevelUImaneger.ShowMiBaoSkillIcon();

						PveLevelUImaneger.mPveLevelUImaneger.IsChooseMiBao = true;

						PveLevelUImaneger.mPveLevelUImaneger.ShowPVEGuid();

						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.PvpSend ):
					{
						GameObject tiaozhanObj = GameObject.Find ("GeneralChallengePage");
						Debug.Log ("GeneralChallengePage:" + tiaozhanObj);
						if (tiaozhanObj != null)
						{
							GeneralTiaoZhan tiaozhan = tiaozhanObj.GetComponent<GeneralTiaoZhan> ();

							tiaozhan.RefreshMiBaoSkillInfo (skillZuHeId);

							YinDaoCol ();
						}

						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.HY_TreSend ):
					{
						HYRetearceEnemy.Instance().M_Treas_info.zuheId =  skillZuHeId;

					    HYRetearceEnemy.Instance().ShowMiBaoSkill();

						break;
					}
//					case (int)(CityGlobalData.MibaoSkillType.HY_ResSend ):
//					{
//						HYResoureEnemy.Instance().m_HYResourseBattleResp.zuheId = Sava_MiBao.zuheSkill;
//
//						GameObject mNEW_ZHENGRONG = GameObject.Find ("NEW_ZHENGRONG");
//						if (mNEW_ZHENGRONG != null)
//						{
//							TiaoZhan mtiaozhan = mNEW_ZHENGRONG.GetComponent<TiaoZhan> ();
//							
//							mtiaozhan.Skill_ZUHE_Id = Sava_MiBao.zuheSkill;
//							
//							mtiaozhan.BattleType = 1;
//							
//							mtiaozhan.Init_HYUI (Sava_MiBao.zuheSkill,0);
//						}
//						break;
//					}
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
						GameObject jieBiaoPage = GameObject.Find ("JieHuoMainPage");
						if (jieBiaoPage)
						{
							JieBiaoMainPage jieBiaoMain = jieBiaoPage.GetComponent<JieBiaoMainPage> ();
							jieBiaoMain.jieHuoMainRes.fangyuZuHeId = skillZuHeId;
						}

						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YaBiao_Gongji) :
					{
						CarriageSceneManager.Instance.s_YabiaoJunZhuList.gongjiZuHeId = skillZuHeId;
						CarriageSceneManager.Instance.m_RootManager.m_CarriageUi.RefreshMibaoSkillEffect();

						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_JinBi) :
					{
						Debug.Log("(int)(CityGlobalData.MibaoSkillType.YX_JinBi) = "+(int)(CityGlobalData.MibaoSkillType.YX_JinBi));

						YouXiaEnemyUI.mYouXiaEnemyUI.m_You_XiaInfo.zuheId = skillZuHeId;
						YouXiaEnemyUI.mYouXiaEnemyUI.ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Cailiao) :
					{
						YouXiaEnemyUI.mYouXiaEnemyUI.m_You_XiaInfo.zuheId = skillZuHeId;
						YouXiaEnemyUI.mYouXiaEnemyUI.ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.YX_Jingpo) :
					{
						YouXiaEnemyUI.mYouXiaEnemyUI.m_You_XiaInfo.zuheId = skillZuHeId;
						YouXiaEnemyUI.mYouXiaEnemyUI.ShowMiBaoSkillIcon();
						break;
					}
					case (int)(CityGlobalData.MibaoSkillType.LueDuo_FangShou):
					{
						LueDuoData.Instance.IsStop = false;
						GameObject lueDuoObj = GameObject.Find ("LueDuo");
						if (lueDuoObj != null)
						{
							LueDuoManager ldManager = lueDuoObj.GetComponent<LueDuoManager> ();
							ldManager.lueDuoInfoRes.myFangShouId = skillZuHeId;
						}
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
			
		}
		return false;
	}

	/// <summary>
	/// 新手引导判定
	/// </summary>
	void YinDaoCol ()
	{
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0 && SkillType == (int)(CityGlobalData.MibaoSkillType.PvpSend))
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];

			if (yinDaoId > 0)
			{
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[6]);
			}
			else
			{
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
			}
		}
	}

	public void ClosBtn()
	{
		YinDaoCol ();
		LueDuoData.Instance.IsStop = false;
	    if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
	    {
            CarriageSceneManager.Instance.m_RootManager.m_CarriageUi.RefreshAllEffect();
        }
		if (SkillType == (int)CityGlobalData.MibaoSkillType.PVP_Fangshou)
		{
			PvpPage.pvpPage.DisActiveObj ();
		}
		else
		{
			if(RootName != null)
			{
				MainCityUI.TryRemoveFromObjectList (GameObject.Find(RootName));
				Destroy (GameObject.Find(RootName));
			}
		}

		Destroy (this.gameObject);
	}

	public void BackBtn()
	{
		YinDaoCol ();

		switch (SkillType) 
		{
		case (int)(CityGlobalData.MibaoSkillType.PVP_Fangshou ):
		{
//			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
//			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
			PvpPage.pvpPage.PvpActiveState (true);
			break;
		}
		case (int)(CityGlobalData.MibaoSkillType.YaBiao_Fangshou ):
		{
			JieBiaoMainPage.jieBiaoMain.IsOpenChangeMiBao = false;
			JieBiaoMainPage.jieBiaoMain.ShowChangeSkillEffect (true);
			break;
		}
		case (int)(CityGlobalData.MibaoSkillType.PvpSend ):
		{
			GeneralTiaoZhan.generalTiaoZhan.ShowChangeSkillEffect (true);
			break;
		}
		case (int)(CityGlobalData.MibaoSkillType.LueDuo_GongJi ):
		{
			GeneralTiaoZhan.generalTiaoZhan.ShowChangeSkillEffect (true);
			break;
		}
		case (int)(CityGlobalData.MibaoSkillType.LueDuo_FangShou ):
		{
			LueDuoData.Instance.IsStop = false;
			LueDuoManager.ldManager.ShowChangeSkillEffect (true);
			break;
		}
		default:
			break;
		}
        if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
        {
            CarriageSceneManager.Instance.m_RootManager.m_CarriageUi.RefreshAllEffect();
        }

		Destroy (this.gameObject);

	}
}
