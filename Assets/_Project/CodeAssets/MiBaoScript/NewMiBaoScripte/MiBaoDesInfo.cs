using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoDesInfo : MonoBehaviour , SocketProcessor{

	public UILabel MiBaoLevel;

	public UILabel MiBaoZl;

	public UILabel GongJi;

	public UILabel FangYu;

	public UILabel  Life;

	public UISprite MiBaoSuiPianIcon;

	public UITexture MiBao_Icon;

	public UILabel SuipianNum;

	public UILabel miBaoName;

	public UILabel miBaobig_Name;

	public UILabel MiBaoInstrution;

	public UILabel UP_Money;

	public UISprite Star;

	public List<UISprite> Stars = new List<UISprite>();

	public MibaoInfo ShowmMiBaoinfo;

	private float StarDis = 0;

	public UISlider mUISlider;

	public GameObject StarUpbtn;

	public GameObject Gre_StarUpbtn;

	public GameObject LevelUp;

	public GameObject Gre_dLevelUp;

	public UISprite MiBaoSuipianIcon;

	private int Cru_MiBao_Zhanli;

	private int m_Life;

	private int m_Gongji;

	private int m_Fangyu;

	public GameObject mLock;

	public GameObject mCollectBtn;

	public GameObject Art;
	private int mibaolevel;
	public bool IsCloseArt;

	MiBaoXmlTemp mmibaoxml;
	
	private bool Is_LevelUp = false;

	public bool IsOPenEffect = false;
	
	public MibaoStarUpResp m_iBaoActiveInfo;
	
	private bool MibaoUpCallback = true;
	
	private bool StartUsecountNunber = false;
	
	private bool MiBAostarStartUsecountNunber = false;
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
		IsCloseArt = false;

		MiBAostarStartUsecountNunber = false;

		MibaoUpCallback = true;
		
	    StartUsecountNunber = false;
	}
	void Update () {
	
		if(mibaolevel > 0)
		{
			ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mmibaoxml.expId, mibaolevel);
			
			if(mExpXxmlTemp.needExp > JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				UP_Money.text = MyColorData.getColorString(5, mExpXxmlTemp.needExp.ToString ());
			}
			else
			{
				UP_Money.text = MyColorData.getColorString(3, mExpXxmlTemp.needExp.ToString ());
			}
			MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);
		
			if(MiBaoManager.Instance().G_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
			   mibaolevel < JunZhuData.Instance().m_junzhuInfo.level&&mibaolevel < 100)
			{
				Art.SetActive(true);
				IsCloseArt =false ;
			}
			else
			{
				if(!IsCloseArt)
				{
					IsCloseArt = true;
					//Debug.Log("Close Art ----- ");
					//CantUpMiBao();
					Art.SetActive(false);
				}
			}
		}
	}

	void CreateLifeMove(GameObject move, int content)
	{
		GameObject clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
		clone.transform.localPosition = move.transform.localPosition;
		clone.transform.localRotation = move.transform.localRotation;
		clone.transform.localScale = move.transform.localScale;
		clone.GetComponent<UILabel>().text = "";
		
		clone.GetComponent<UILabel>().text = MyColorData.getColorString(4, "+"+content.ToString());
		
		
		clone.AddComponent< TweenPosition>();
		clone.AddComponent<TweenAlpha>();
		clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
		clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * 40;
		clone.GetComponent<TweenPosition>().duration = 0.5f;
		clone.GetComponent<TweenAlpha>().from = 1.0f;
		clone.GetComponent<TweenAlpha>().to = 0;
		clone.GetComponent<TweenPosition>().duration = 0.8f;
		StartCoroutine(WatiFor(clone));
	}
	IEnumerator WatiFor(GameObject obj)
	{
		yield return new WaitForSeconds(0.8f);
		Destroy(obj);
	}
	public void Init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);

		 mibaolevel = ShowmMiBaoinfo.level;

		if (mibaolevel <= 0)
		{
			mibaolevel = 1;
		}
	
		if(!Is_LevelUp)
		{
			Cru_MiBao_Zhanli = ShowmMiBaoinfo.zhanLi;
			
			MiBaoZl.text = ShowmMiBaoinfo.zhanLi.ToString();

			m_Life = ShowmMiBaoinfo.shengMing;

			m_Gongji = ShowmMiBaoinfo.gongJi;

		    m_Fangyu = ShowmMiBaoinfo.fangYu;
		}
		else
		{
			Is_LevelUp = false;

			if(m_Life < ShowmMiBaoinfo.shengMing)
			{
				int mContant = ShowmMiBaoinfo.shengMing - m_Life;

				m_Life = ShowmMiBaoinfo.shengMing;

				CreateLifeMove(Life.gameObject, mContant);
			}
			if(m_Gongji < ShowmMiBaoinfo.gongJi)
			{
				int mContant = ShowmMiBaoinfo.gongJi - m_Gongji;
				
				CreateLifeMove(GongJi.gameObject, mContant);

				m_Gongji = ShowmMiBaoinfo.gongJi;
			}
			if(m_Fangyu < ShowmMiBaoinfo.fangYu)
			{
				int mContant = ShowmMiBaoinfo.fangYu - m_Fangyu;
				
				CreateLifeMove(FangYu.gameObject, mContant);

				m_Fangyu = ShowmMiBaoinfo.fangYu;
			}
			StopCoroutine("Update_ZhanliAndLevel");
			
			StartCoroutine("Update_ZhanliAndLevel");
		}
		MiBaoSuipianXMltemp m_Mibaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (ShowmMiBaoinfo.tempId);
		if(ShowmMiBaoinfo.isLock)
		{
			mLock.SetActive(true);

			LevelUp.SetActive(false);
			
			Gre_dLevelUp.SetActive(true);

			Gre_dLevelUp.GetComponent<BoxCollider>().enabled = false;

			StarUpbtn.SetActive(false);

			mCollectBtn.SetActive(true);

			SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+m_Mibaosuipian.hechengNum.ToString();

			mUISlider.value = (float)( ShowmMiBaoinfo.suiPianNum )/ (float)(m_Mibaosuipian.hechengNum);
			
			
			if(mUISlider.value > 1f)mUISlider.value = 1f;
		}
		else
		{
			mLock.SetActive(false);
			if(ShowmMiBaoinfo.star >= 5 )
			{
				SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+ShowmMiBaoinfo.needSuipianNum.ToString();
				Gre_StarUpbtn.SetActive(true);
				mCollectBtn.SetActive(false);
				StarUpbtn.SetActive(false);
				mUISlider.value = 1.0f;
			}
			else
			{
				SuipianNum.text = ShowmMiBaoinfo.suiPianNum.ToString()+"/"+ShowmMiBaoinfo.needSuipianNum.ToString();
				
				mUISlider.value = (float)( ShowmMiBaoinfo.suiPianNum )/ (float)(ShowmMiBaoinfo.needSuipianNum);
			}
			if(mUISlider.value > 1f)mUISlider.value = 1f;

			if(ShowmMiBaoinfo.suiPianNum >= ShowmMiBaoinfo.needSuipianNum && ShowmMiBaoinfo.star < 5)
			{
				mCollectBtn.SetActive(false);
				StarUpbtn.SetActive(true);
				ShowEffect();
			}
			else
			{
				if(ShowmMiBaoinfo.star < 5)
				{
					mCollectBtn.SetActive(true);
					StarUpbtn.SetActive(false);
				}

			}
			if(mibaolevel >= JunZhuData.Instance().m_junzhuInfo.level)
			{
				LevelUp.SetActive(false);
				
				Gre_dLevelUp.SetActive(true);
			}
			else
			{
				LevelUp.SetActive(true);
				
				Gre_dLevelUp.SetActive(false);
			}
		}
		mmibaoxml = MiBaoXmlTemp.getMiBaoXmlTempById (ShowmMiBaoinfo.miBaoId);

		MiBaoSuipianIcon.spriteName = m_Mibaosuipian.icon.ToString ();

		DescIdTemplate mDescIdTemplate = DescIdTemplate.getDescIdTemplateByNameId (mmibaoxml.descId);

		MiBaoInstrution.text = mDescIdTemplate.description;
	
		MiBao_Icon.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.MIBAO_BIGICON) + mmibaoxml.icon.ToString ());

		miBaoName.text = NameIdTemplate.GetName_By_NameId (mmibaoxml.nameId);

		miBaobig_Name.text = miBaoName.text;

		MiBaoLevel.text = mibaolevel.ToString ();


		GongJi.text = MyColorData.getColorString(10, ShowmMiBaoinfo.gongJi.ToString ());

		FangYu.text = MyColorData.getColorString(10, ShowmMiBaoinfo.fangYu.ToString ());

		Life.text = MyColorData.getColorString(10, ShowmMiBaoinfo.shengMing.ToString ());

		ShowStar ();
		if(MiBaoScrollView.IsOPenPath)
		{
			
			if(MiBaoScrollView.OpenMiBaoId == ShowmMiBaoinfo.miBaoId)
			{
				
				if(!MiBaoScrollView.FirstOPenPath)
				{
					CloseEffect();
					ConllectMiBao();
				}
			}
		}
	}

    public void ShowEffect()
	{
		int effectid = 600153;
		IsOPenEffect = true;
		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,StarUpbtn,EffectIdTemplate.GetPathByeffectId(effectid));
	}
	public void CloseEffect()
	{
		IsOPenEffect = false;
		UI3DEffectTool.Instance ().ClearUIFx (StarUpbtn);
	}
	public void OpenLock()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenLockLoadBack);
	}

	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string str2 = "";

		string str1 = "\r\n"+"解锁此秘宝需要达成以下条件："+"\r\n"+"\r\n";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById ( ShowmMiBaoinfo.miBaoId );
		
		switch(mMiBaoXML.unlockType)
		{
		case 1:
			
			str2 = "君主等级达到"+mMiBaoXML.unlockValue.ToString();
			
			break;
		case 2:
			
			PveTempTemplate mPve = PveTempTemplate.GetPveTemplate_By_id(mMiBaoXML.unlockValue);
			
			string mName = NameIdTemplate.GetName_By_NameId(mPve.smaName);
			
			str2 = "完成普通关卡： "+mName;
			
			break;
		case 3:
			
			LegendPveTemplate mLegendPve = LegendPveTemplate.GetlegendPveTemplate_By_id(mMiBaoXML.unlockValue);
			
			string m_Name = NameIdTemplate.GetName_By_NameId(mLegendPve.smaName);
			
			str2 = "完成传奇关卡： "+m_Name;
			
			break;
			
		case 4:
			
			ZhuXianTemp mZHuxian = ZhuXianTemp.getTemplateById(mMiBaoXML.unlockValue);
			
			str2 = "完成主线任务： "+mZHuxian.title;
			break;
			
			
		default:
			break;
			
		}
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1+str2), null,null,confirmStr,null,null,null,null);
	}
	IEnumerator Update_ZhanliAndLevel()
	{
		if(ShowmMiBaoinfo.zhanLi > Cru_MiBao_Zhanli+1)
		{
			int m = (ShowmMiBaoinfo.zhanLi - Cru_MiBao_Zhanli);

			float mTime = 0.01f;

			while(m > 0)
			{

				if(m > 10000)
				{
					m -= 10000;
					Cru_MiBao_Zhanli += 10000;
				}
				if(m > 1000)
				{
					m -= 1000;
					Cru_MiBao_Zhanli += 1000;
				}
				if(m > 100)
				{
					m -= 100;
					Cru_MiBao_Zhanli += 100;
				}
				if(m > 10)
				{
					m -= 10;
					Cru_MiBao_Zhanli += 10;
				}
				else
				{
					m -= 1;
					Cru_MiBao_Zhanli += 1;
				}

			    MiBaoZl.text = (Cru_MiBao_Zhanli).ToString();

				MiBaoZl.gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.2f);

				yield return new WaitForSeconds(mTime);
			}

			MiBaoZl.gameObject.transform.localScale = Vector3.one;
		}
		else{
	
			MiBaoZl.text = ShowmMiBaoinfo.zhanLi.ToString();
		}
	}

	void ShowStar()
	{
		Debug.Log ("star = "+ShowmMiBaoinfo.star);

		foreach(UISprite s in Stars)
		{
			Destroy(s.gameObject);
		}
		
		Stars.Clear ();
		
		for(int i = 0 ; i < ShowmMiBaoinfo.star; i ++)
		{
			GameObject StarTemp = Instantiate(Star.gameObject) as GameObject;
			
			StarTemp.SetActive(true);
			
			StarTemp.transform.parent = Star.gameObject.transform.parent;
			
			StarTemp.transform.localPosition = new Vector3(0,-60+30*i,0);
			
			StarTemp.transform.localScale = Star.gameObject.transform.localScale;
			
			UISprite mUISprite = StarTemp.GetComponent<UISprite>();
			
			Stars.Add(mUISprite);
		}

	}

	IEnumerator countNunber()
	{
		yield return new WaitForSeconds (1.0f);
			
		MibaoUpCallback = true;

		StartUsecountNunber = false;
		Debug.Log("22222222222222222222");
	}

	public void MibaoUp() //秘宝升级
	{
		if(!MibaoUpCallback )
		{
			Debug.Log("99999999999");
			if(!StartUsecountNunber)
			{
				StartUsecountNunber = true;

				StartCoroutine("countNunber");
			}

			return;
		}
		MibaoUpCallback = false;
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowmMiBaoinfo.miBaoId);

		ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,mibaolevel);

		if(MiBaoManager.Instance().G_MiBaoInfo.levelPoint <= 0)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,false,true);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
			MibaoUpCallback = true;
			return;
		}
		if(JunZhuData.Instance().m_junzhuInfo.jinBi < mExpXxmlTemp.needExp)
		{
			CloseYInDao();
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
			return;
		}
		if(mibaolevel >= JunZhuData.Instance().m_junzhuInfo.level)
		{
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadMiBaolvBack);

			return;
		}
		if(mibaolevel>= 100)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),Load_NoLevelUpBack);
			if(IsOPenEffect)
			{
				CloseEffect ();
			}
			return;
		}
		JunZhuData.Instance().m_junzhuInfo.jinBi -= mExpXxmlTemp.needExp;

		MiBaoManager.Instance().G_MiBaoInfo.levelPoint  -= 1;

		MibaoLevelupReq MiBaoshengJ = new MibaoLevelupReq ();
		
		MemoryStream MiBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoer = new QiXiongSerializer ();
		
		MiBaoshengJ.mibaoId = ShowmMiBaoinfo.miBaoId;
		
		MiBaoer.Serialize (MiBaoStream,MiBaoshengJ);
	
		byte[] t_protof;
		t_protof = MiBaoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_LEVELUP_REQ,ref t_protof);

	}
    
	void CloseYInDao()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	void  CantUpMiBao()
	{
		// TODO
		// 秘宝升级全部完成后调用
		PushAndNotificationHelper.SetRedSpotNotification ( 600, false );

//
//		// 秘宝升星激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (602, true);
//
//		// 秘宝升星红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (602, false);



//
//		// 秘宝合成激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (605, true);
//		
//		// 秘宝合成红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (605, false);
//
//
//
//		// 秘宝技能激活，激活红点时
//		PushAndNotificationHelper.SetRedSpotNotification (610, true);
//		
//		// 秘宝技能激活，红点取消时
//		PushAndNotificationHelper.SetRedSpotNotification (610, false);
	}
	
    public bool OnProcessSocketMessage(QXBuffer p_message){
	
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_LEVELUP_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoLevelupResp Levelup = new MibaoLevelupResp();
				
				t_qx.Deserialize(t_stream, Levelup, Levelup.GetType());

				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress < 0)
					{
						
						ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];
						
						Debug.Log("密保升级");
						
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]); //密保升级
					}
				}
				if(Levelup.mibaoInfo != null)
				{
					Debug.Log("518  dataBack");

					ShowmMiBaoinfo = Levelup.mibaoInfo;

					SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);

					MibaoUpCallback = true;

					Is_LevelUp = true;

					Init();
					MiBaoManager.Instance().ShowZhanLiAnmition();

				}
				return true;
			 }
			case ProtoIndexes.s_Mibao_StarUp_Resp:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoStarUpResp MiBaoMibaoStarUpResp= new MibaoStarUpResp();
				
				t_qx.Deserialize(t_stream, MiBaoMibaoStarUpResp, MiBaoMibaoStarUpResp.GetType());

				m_iBaoActiveInfo = MiBaoMibaoStarUpResp;

				if(MiBaoMibaoStarUpResp.mibaoInfo != null)
				{
					ShowmMiBaoinfo = MiBaoMibaoStarUpResp.mibaoInfo;

					Is_LevelUp = true;

					PushAndNotificationHelper.SetRedSpotNotification (602, false);

					Init();
					StartCoroutine("countMiBaoStarUpNunber");

					UI3DEffectTool.Instance ().ShowBottomLayerEffect (UI3DEffectTool.UIType.PopUI_2,MiBaoeffect,EffectIdTemplate.GetPathByeffectId(100183));
					//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_CARD_TEMP ),LoadBck_2);

				}
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);

				MiBaoManager.Instance().ShowZhanLiAnmition();
				return true;
			}
			default: return false;
			}
			
		}
		return false;
	}
	public GameObject MiBaoeffect;

	void LoadBck_2(ref WWW p_www,string p_path, Object p_object) // 合成秘宝时候弹出的框 大秘宝
	{
		GameObject cardtemp = Instantiate(p_object) as GameObject;
		
		cardtemp.transform.parent = this.transform.parent;
		
		cardtemp.transform.localPosition = new Vector3(0,-46,0);
		
		cardtemp.transform.localScale = new Vector3(0.9f,0.9f,0.9f);

		mbCardTemp mmbCardTemp = cardtemp.GetComponent<mbCardTemp>();
		
		mmbCardTemp.mibaoTemp =  m_iBaoActiveInfo.mibaoInfo;
		
		mmbCardTemp.init();	
	}

	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTongBi,null,null,null);
	}

	void LoadMiBaolvBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.LEVEL_NOT_ENOUGH);

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_LEVEL_UP_FAILE);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null,str1,null,confirmStr,null,null,null,null);
	}

	void Load_NoLevelUpBack(ref WWW p_www,string p_path, Object p_object)//秘宝等级不足回调函数
	{
		MibaoUpCallback = true;
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null, "",null,confirmStr,null,null,null,null);
	}

	void getTongBi(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
		}
	}

	public void MibaoStarUp()
	{
		if(CityGlobalData.MibaoSatrUpCallback)
		{
			return;
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),MiBaoUpStarLoadBack);
	}

	private int StarNeedMoney;

	IEnumerator countMiBaoStarUpNunber()
	{
		yield return new WaitForSeconds (1.0f);
		
		CityGlobalData.MibaoSatrUpCallback = false;
	
	}

	void MiBaoUpStarLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		CloseEffect ();
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str = "";
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		if (ShowmMiBaoinfo.star >= 5)
		{
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_19);
			
			str = "\r\n"+"星级已满, 不能在进行升星了！";
		
			uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirm,null,null);
		}
		else
		{
			if (ShowmMiBaoinfo.suiPianNum < ShowmMiBaoinfo.needSuipianNum)
			{
				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_3);
				
				str = LanguageTemplate.GetText (LanguageTemplate.Text.MIBAO_ENHANCE_4);
				
				uibox.setBox(titleStr,null,MyColorData.getColorString (1,str),null,confirm,null,null);
			}

			else 
			{
				MiBaoStarTemp mMiBaoStarTemp = MiBaoStarTemp.getMiBaoStarTempBystar (ShowmMiBaoinfo.star);
				
				StarNeedMoney = mMiBaoStarTemp.needMoney;

				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
				
				str ="\r\n"+"此秘宝升星需要消耗" + StarNeedMoney.ToString() + "铜币"+"\r\n"+"\r\n"+"是否现在升星？";

				uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancel,confirm,SendStarUpInfo);

			}
		}
	}

	void SendStarUpInfo(int i)
	{
		if(i == 2)
		{
			//JunZhuData.Instance().BuyTiliAndTongBi(false,true);
			if(StarNeedMoney > JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
			}
			else
			{
				CityGlobalData.MibaoSatrUpCallback =  true;

				MibaoStarUpReq MiBaoinfo = new MibaoStarUpReq ();
				MemoryStream MiBaoinfoStream = new MemoryStream ();
				QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
				
				MiBaoinfo.mibaoId = ShowmMiBaoinfo.miBaoId;
				MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
				
				byte[] t_protof;
				t_protof = MiBaoinfoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_STARUP_REQ,ref t_protof,ProtoIndexes.s_Mibao_StarUp_Resp.ToString());
			}
		}
		else
		{
			ShowEffect();
		}
	}

	public void ConllectMiBao()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),ShowPicecPath);
	}

	void ShowPicecPath(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject mNoPiece = Instantiate ( p_object )as GameObject;
		
		mNoPiece.SetActive (true);
	
		mNoPiece.transform.parent = GameObject.Find ("Allpiece").transform;;
		
		mNoPiece.transform.localScale = new Vector3 (1,1,1);
		
		mNoPiece.transform.localPosition = new Vector3 (0,-46,0);

		LockPiece mLockPiece = mNoPiece.GetComponent<LockPiece>();
		
		mLockPiece.my_Diaoluomibao = ShowmMiBaoinfo; 
		
		mLockPiece.Init ();

		MiBaoScrollView.FirstOPenPath = false;
	}
}
