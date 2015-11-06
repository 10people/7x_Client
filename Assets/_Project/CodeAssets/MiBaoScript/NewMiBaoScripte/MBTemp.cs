using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MBTemp : MonoBehaviour {
//	required int64 dbId = 1;			// 秘宝DBid，不在db中发-1
//	required int32 tempId = 2;			// 秘宝类型id，配置文件中的tempId
//	required int32 miBaoId = 3;			// 配置文件mibao中id字段
//	required int32 star = 4;			// 星级
//	required int32 level = 5;			// 等级 , <= 0 表示没有激活
//	required int32 suiPianNum = 6;		// 拥有的碎片数量
//	required int32 needSuipianNum = 7;	// 升星 需要的碎片数量
//	required int32 gongJi = 8;			// 攻击
//	required int32 fangYu = 9;			// 防御
//	required int32 shengMing = 10;		// 生命
//	required bool lock = 11;

	public GameObject Lock;

	public GameObject MiBaoActive;

	public GameObject MiBaoDisActive;

	public UILabel suipianNum;

	public UILabel HechengNum;

	public UISprite Tips;

	public MibaoInfo mMiBaoinfo;

	public UISprite Star;

	public UISprite Gree_Star;

	public UISprite MiBaoIcon;

	public UISprite MiBaoSuipianIcon;

	private List<UISprite> Stars = new List<UISprite>();

	public UISprite MiBaopinZi;

	public GameObject m_MIBaoScorllview;
	public GameObject Proess;
	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{
		//Debug.Log ("==================5  = "+mMiBaoinfo.miBaoId);
		if(mMiBaoinfo.isLock)
		{
			Lock.SetActive(true);
			Proess.SetActive(false);
		}
		else
		{
			if(mMiBaoinfo.level <= 0)
			{
				Proess.SetActive(true);

			}
			else
			{
				Proess.SetActive(false);
			}
		}

		if(mMiBaoinfo.level <= 0)
		{
			MiBaoActive.SetActive(false);

			MiBaoDisActive.SetActive(true);

			MiBaoSuipianIcon.gameObject.SetActive(true);

			MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (mMiBaoinfo.tempId);

			if(mMiBaoinfo.suiPianNum <= 0)
			{
				HechengNum.text = MyColorData.getColorString(5, mMiBaoinfo.suiPianNum.ToString())+"/"+mMiBaoSuipianXMltemp.hechengNum.ToString();
			}
			else if(mMiBaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum)
			{
				HechengNum.text = MyColorData.getColorString(6, mMiBaoinfo.suiPianNum.ToString())+"/"+mMiBaoSuipianXMltemp.hechengNum.ToString();
			}
			else
			{
				HechengNum.text = mMiBaoinfo.suiPianNum.ToString()+"/"+mMiBaoSuipianXMltemp.hechengNum.ToString();
			}
			MiBaoSuipianIcon.spriteName = mMiBaoSuipianXMltemp.icon.ToString();

			MiBaoSuipianIcon.color = new Color(0,0,0,255);

			UISlider mSlider = Proess.GetComponent<UISlider>();
			
			mSlider.value = (float)(mMiBaoinfo.suiPianNum)/(float)(mMiBaoSuipianXMltemp.hechengNum);

			if(mMiBaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum && !mMiBaoinfo.isLock )
			{
				Tips.gameObject.SetActive(true);
			}
			CreateStar (Gree_Star);
			
		}
		else
		{
			MiBaoXmlTemp mmibao = MiBaoXmlTemp.getMiBaoXmlTempById(mMiBaoinfo.miBaoId);

			suipianNum.text = mMiBaoinfo.level.ToString();

			MiBaoActive.SetActive(true);
			
			MiBaoDisActive.SetActive(false);

			MiBaoIcon.spriteName = mmibao.icon.ToString();

			MiBaopinZi.spriteName = "";
			ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mmibao.expId,mMiBaoinfo.level);

			if(MiBaoManager.Instance().G_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
			   mMiBaoinfo.level < JunZhuData.Instance().m_junzhuInfo.level&&mMiBaoinfo.level < 100 && !mMiBaoinfo.isLock)
			{
				//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LOCkPoint);
				Tips.gameObject.SetActive(true);
			}
		

			switch(mMiBaoinfo.star)
			{
			case 1:

				MiBaopinZi.spriteName = "pinzhi3";

				break;
			case 2:
				
				MiBaopinZi.spriteName = "pinzhi6";
				
				break;
			case 3:
				
				MiBaopinZi.spriteName = "pinzhi9";
				
				break;
			case 4:
				
				MiBaopinZi.spriteName = "pinzhi9";
				
				break;
			case 5:
				
				MiBaopinZi.spriteName = "pinzhi9";
				
				break;
			default:
				break;

			}
			if(mMiBaoinfo.suiPianNum >= mMiBaoinfo.needSuipianNum && mMiBaoinfo.star < 5&& !mMiBaoinfo.isLock)
			{
				Tips.gameObject.SetActive(true);
			}
			CreateStar (Star);
		}


		if(Global.m_sPanelWantRun != null&&Global.m_sPanelWantRun != "")
		{
			if(int.Parse(Global.m_sPanelWantRun)  == mMiBaoinfo.miBaoId)
			{
				ShowActiveInfo();
				Global.m_sPanelWantRun = "";
			}
		}

		if(MiBaoScrollView.IsOPenPath)
		{

			if(MiBaoScrollView.OpenMiBaoId == mMiBaoinfo.miBaoId)
			{

				if(MiBaoScrollView.FirstOPenPath)
				{
					ShowDisActiveInfo();
				}
				else
				{
					ShowActiveInfo();
				}

			}
		}
	}
	void CreateStar(UISprite star)
	{
		foreach(UISprite s in Stars)
		{
			Destroy(s.gameObject);
		}

		Stars.Clear ();
		int stars = 0;
		if(mMiBaoinfo.level > 0)
		{
			stars = mMiBaoinfo.star;
		}
		else
		{
			MiBaoXmlTemp mmibao = MiBaoXmlTemp.getMiBaoXmlTempById(mMiBaoinfo.miBaoId);

			stars = mmibao.initialStar;


		}
		for(int i = 0 ; i < stars; i ++)
		{
			GameObject StarTemp = Instantiate(star.gameObject) as GameObject;

			StarTemp.SetActive(true);

			StarTemp.transform.parent = star.gameObject.transform.parent;

			StarTemp.transform.localPosition = Star.transform.localPosition + new Vector3(i * 20 - (stars - 1) * 10, 0, 0);

			StarTemp.transform.localScale = star.transform.localScale;

			UISprite mUISprite = StarTemp.GetComponent<UISprite>();

			Stars.Add(mUISprite);
		}
	}

	public void ShowActiveInfo()
	{
//		Debug.Log ("mMiBaoinfo.mibaoid = " +mMiBaoinfo.miBaoId);
//		
//		Debug.Log ("mMiBaoinfo.gongJi = " +mMiBaoinfo.gongJi);
//		
//		Debug.Log ("mMiBaoinfo.fangYu = " +mMiBaoinfo.fangYu);
//		
//		Debug.Log ("mMiBaoinfo.shengMing = " +mMiBaoinfo.shengMing);

		if(FreshGuide.Instance().IsActive(100050)&& TaskData.Instance.m_TaskInfoDic[100050].progress>= 0)
		{
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100050];

			Debug.Log("秘宝升级引导2");

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]); //选择秘宝
		}

		MiBaoManager.Instance ().MiBaoManager_mMiBaotempinfo = mMiBaoinfo;

		MiBaoManager.Instance ().SortUI ("MiBaoTempInfo");
	}

	private int Mony;

	public void ShowDisActiveInfo()
	{
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (mMiBaoinfo.tempId);

		Mony = mMiBaoSuipianXMltemp.money;

		if(mMiBaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum)
		{

			if(FreshGuide.Instance().IsActive(100210)&& TaskData.Instance.m_TaskInfoDic[100210].progress>= 0)
			{
				
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100210];
				
				Debug.Log("秘宝合成引导2");

				MiBaoScrollView mMiBaoScrollView = m_MIBaoScorllview.GetComponent<MiBaoScrollView>();

				mMiBaoScrollView.ActiveScrollViewPanlemove();

				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]); //选择秘宝
			}
			if(FreshGuide.Instance().IsActive(100350)&& TaskData.Instance.m_TaskInfoDic[100350].progress>= 0)
			{
				
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100350];
				
				Debug.Log("秘宝第二次合成2");

				MiBaoScrollView mMiBaoScrollView = m_MIBaoScorllview.GetComponent<MiBaoScrollView>();

				mMiBaoScrollView.ActiveScrollViewPanlemove();

				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]); //选择秘宝

			}
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),ShowCHAT_UIBOX_INFO);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),ShowPicecPath);
		}
	}


	void ShowPicecPath(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject mNoPiece = Instantiate ( p_object )as GameObject;
		
		mNoPiece.SetActive (true);

		mNoPiece.transform.parent = GameObject.Find ("Allpiece").transform;;
		
		mNoPiece.transform.localScale = new Vector3 (1,1,1);
		
		mNoPiece.transform.localPosition = new Vector3 (0,-46,0);

		LockPiece mLockPiece = mNoPiece.GetComponent<LockPiece>();

		mLockPiece.my_Diaoluomibao = mMiBaoinfo; 

		mLockPiece.Init ();

		MiBaoScrollView.FirstOPenPath = true;
	}

	void ShowCHAT_UIBOX_INFO(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"合成此秘宝需要消耗" +Mony.ToString()+"铜币"+"\r\n"+"\r\n"+"是否合成？";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
	
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,CancleBtn,confirmStr,StarmakeMiBao,null,null);
	}

	void StarmakeMiBao(int i)
	{
		if(i == 2)
		{
			if(Mony <= JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				MibaoActivate MiBaoinfo = new MibaoActivate ();

				MemoryStream MiBaoinfoStream = new MemoryStream ();

				QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();

				Debug.Log ("mMiBaoinfo.tempId"+mMiBaoinfo.tempId);

				MiBaoinfo.tempId = mMiBaoinfo.tempId;

				MiBaoinfoer.Serialize (MiBaoinfoStream,MiBaoinfo);
				
				byte[] t_protof;
				t_protof = MiBaoinfoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_ACTIVATE_REQ,ref t_protof,ProtoIndexes.S_MIBAO_ACTIVATE_RESP.ToString());//秘宝激活
				PushAndNotificationHelper.SetRedSpotNotification (605, false);
			}
			else
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTongBiLoadBack);
			}
		}
	}


	void LockTongBiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTongBi,null,null,null);
	}

	void getTongBi(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false,true,false);
		}
	}

	public void OpenLock()
	{

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenLockLoadBack);
	
	}

	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"解锁此秘宝需要达成以下条件：";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

		string str2 = "";

		MiBaoXmlTemp mMiBaoXML = MiBaoXmlTemp.getMiBaoXmlTempById ( mMiBaoinfo.miBaoId );

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
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,null,null,null);
	}

	public void ShowGetPath()
	{
		
	}
}
