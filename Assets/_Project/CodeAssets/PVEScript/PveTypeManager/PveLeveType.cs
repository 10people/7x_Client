using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PveLeveType : MonoBehaviour {

	public GameObject UIrootName;
	public MibaoInfoResp miBaoResp;

	public Level levelInfo;

	public GameObject puTongObj;
	public GameObject jingYingObj;
	public GameObject chuanQiObj;

	public List<int> MiBaoidList = new List<int>();

	public UILabel tiLiLabel;//体力label

	public GameObject levelNameObj;

	[HideInInspector]public bool MiBaoisNull;

	public GameObject enemyBgObj1;//普通关卡时显示的敌人背景
	public GameObject enemyBgObj2;//精英或传奇关卡时显示的敌人背景

	public GameObject ChangeMiBaobtn;//更换秘宝的按钮

	public GuanQiaInfo GuanQia;
	//初始化关卡

	public bool IsTest = false;
	public int Test_Section = 1;
	public int Test_Level = 1;
	public UILabel testLable;
	public GameObject inputbox;

	public GameObject TestBtn;

	public void InItCheckPoint ()
	{

		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			ChangeMiBaobtn.SetActive(false);
		}else
		{
			ChangeMiBaobtn.SetActive(true);
		}

		tiLiLabel.text = GuanQia.tili.ToString ();



		//关卡类型 0为普通关卡 1 为精英关卡 2为传奇关卡 
		switch (levelInfo.type)
		{
		case 0:

			enemyBgObj1.SetActive (true);

			enemyBgObj2.SetActive (false);

			puTongObj.SetActive (true);

			jingYingObj.SetActive (false);

			chuanQiObj.SetActive (false);

			PuTongType ptType = puTongObj.GetComponent<PuTongType> ();

			ptType.GetNeedInfo (levelInfo,GuanQia);

			break;

		case 1:

			enemyBgObj1.SetActive (false);

			enemyBgObj2.SetActive (true);

			puTongObj.SetActive (false);

			jingYingObj.SetActive (true);

			chuanQiObj.SetActive (false);

			JingYingType jyType = jingYingObj.GetComponent<JingYingType> ();

			jyType.GetNeedInfo (levelInfo,GuanQia);

			break;

		case 2:

			enemyBgObj1.SetActive (false);

			enemyBgObj2.SetActive (true);

			puTongObj.SetActive (false);

			jingYingObj.SetActive (false);

			chuanQiObj.SetActive (true);

			ChuanQiType cqType = chuanQiObj.GetComponent<ChuanQiType> ();

			cqType.GetNeedInfo (levelInfo,GuanQia);

			break;

		default:break;
		}
		if( ConfigTool.GetBool(ConfigTool.CONST_QUICK_CHOOSE_LEVEL))
		{
			TestBtn.SetActive(true);
		}
		else
		{
			TestBtn.SetActive(false);
		}
	}

	/// 领取星级奖励
	public void popLv_Star_UI()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_GRADE_REWARD),LoadResourceCallback2);

		PveLevelUImaneger.mPveLevelUImaneger.CloseEffect ();
		PveLevelUImaneger.mPveLevelUImaneger.ColsePVEGuid ();
	}

	public void LoadResourceCallback2(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
		
		tempOjbect.transform.parent = GameObject.Find ("Mapss").transform;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = Vector3.zero;
		
		PveStarAward mPveStarAward = tempOjbect.GetComponent<PveStarAward>();
		
		mPveStarAward.M_Level = levelInfo;

		mPveStarAward.Opentype = 2;

		mPveStarAward.Init ();
	}

	//攻打按钮
	public void ComfireTest_btn()
	{
		UI3DEffectTool.Instance ().ClearUIFx (ChangeMiBaobtn);
		IsTest = true;
		char[] aprcation = {','};
		//Debug.Log ("text =  "+testLable.text);
		string[] s = testLable.text.Split(aprcation);

		if(s.Length > 0)
		{
			Test_Section = int.Parse(s[0]);

			Test_Level = int.Parse(s[1]);
		}
		//Debug.Log ("Test_Section =  "+Test_Section);
		//Debug.Log ("Test_Level = "+Test_Level);
		inputbox.SetActive(false);
	}
	public void CancleTest_BTN()
	{
		IsTest = false;
	}

	public void ShowInputBox()
	{
		if(inputbox.activeInHierarchy)
		{
			inputbox.SetActive(false);
		}
		else{
			inputbox.SetActive(true);
		}
	}

	public void ChangeMiBaoBtn()
	{

		PveLevelUImaneger.mPveLevelUImaneger.CloseEffect ();

		PveLevelUImaneger.mPveLevelUImaneger.ColsePVEGuid ();

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadBack);

	}

	void LoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.SetActive(true);
		
		mChoose_MiBao.transform.parent = this.transform.parent;
		
		mChoose_MiBao.transform.localPosition = Vector3.zero;
		
		mChoose_MiBao.transform.localScale = Vector3.one;

		CityGlobalData.PveLevel_UI_is_OPen = false;

		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();

		mChangeMiBaoSkill.GetRootName (UIrootName.name);

		mChangeMiBaoSkill.Init(1, PveLevelUImaneger.GuanqiaReq.zuheId);


	}

	public void PVE_Fight()
	{

		if(IsTest)
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
				CityGlobalData.m_tempSection = Test_Section;

				CityGlobalData.m_tempLevel = Test_Level;

				EnterBattleField.EnterBattlePve(Test_Section, Test_Level, LevelType.LEVEL_NORMAL);
		
			return;
		}
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		if(JunZhuData.Instance().m_junzhuInfo.tili < PveLevelUImaneger.GuanqiaReq.tili)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTiLiLoadBack);

			return ;
		}
//		if( JunZhuData.Instance().m_junzhuInfo.level< 5 && levelInfo.s_pass)
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),RemainBocBack);
//			return ;
//		}
//		
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}

		if(levelInfo.type == 2)
		{
			if(GuanQia.cqPassTimes >= 3)
			{
				PveLevelUImaneger.mPveLevelUImaneger.ReSettingLv();

				return;
				//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),buyTimes);
			}
		}
		CityGlobalData.PveLevel_UI_is_OPen = false;
		int  my_tempSection = MapData.mapinstance.myMapinfo.s_section;

		int a = Pve_Level_Info.CurLev;
		
		int my_tempLevel = a%10;
		
		CityGlobalData.m_tempSection = my_tempSection;

		CityGlobalData.m_tempLevel = my_tempLevel;

		if(CityGlobalData.PT_Or_CQ )
		{
			if(MapData.mapinstance.Lv[a].type == 1)
			{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_ELITE);
			}
			else{
				EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_NORMAL);
			}
		}
		else{
			EnterBattleField.EnterBattlePve(my_tempSection, my_tempLevel, LevelType.LEVEL_TALE);
		}

	}

	void MiBaoShangZhenLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject mibaoSelectObj = GameObject.Instantiate(p_object) as GameObject;
		
		mibaoSelectObj.name = "MiBaoShangZhen";

		mibaoSelectObj.transform.parent = levelNameObj.transform.parent;

		mibaoSelectObj.transform.localPosition = levelNameObj.transform.localPosition;

		mibaoSelectObj.transform.localScale = Vector3.one;
		
		MiBaoShangZhen selectMibao = mibaoSelectObj.GetComponent<MiBaoShangZhen>();
		
		//selectMibao.m_MiBaoInfo = miBaoResp;
		selectMibao.GJunzu_Data = (JunZhuInfoRet)JunZhuData.Instance ().m_junzhuInfo.Public_MemberwiseClone();

		selectMibao.InitData ();
	}

	void RemainBocBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.LEVEL_NOT_ENOUGH);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.LEVEL_NOT_ENOUGH_DES);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,getTili);
	}
	void getTili(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true,false,false);
		}
	}
	void LockTiLiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.TITITLE);

		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTili);
	}
}
