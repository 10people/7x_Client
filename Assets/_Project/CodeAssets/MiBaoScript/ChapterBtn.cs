using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class ChapterBtn : MonoBehaviour {

	public UILabel Chapterssprite;

	public UILabel lv_namesprite;

	[HideInInspector]public MibaoInfo  m_mibao ;  

	public UISprite  m_mibaoicon ;  

	public UISprite  m_mibaoPinzhi ;  

	[HideInInspector]public int Chapters;

	[HideInInspector]public string lv_name;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void  Init()
	{
		Chapterssprite.text = Chapters.ToString ();
		lv_namesprite.text = lv_name;
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(m_mibao.miBaoId);

		MiBaoSuipianXMltemp mMiBaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (mMiBaoXmlTemp.suipianId);

		m_mibaoicon.spriteName = mMiBaosuipian.icon.ToString ();

		m_mibaoPinzhi.spriteName = "pinzhi" + (mMiBaoXmlTemp.pinzhi - 1).ToString ();
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TRANS_95);
		//string Contain2 = NeedJunZhuLv.ToString ();;
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		uibox.setBox(title,null, Contain1,null,Comfirm,null,null,null,null);
	}
	public void LoadReWuCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT);
		string Contain2 = ZhuXianTemp.GeTaskTitleById (FunctionOpenTemp.GetTemplateById(109).m_iMissionID);
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		uibox.setBox(title,Contain1, Contain2,null,Comfirm,null,null,null,null);
	}
	public void EnterChooseChapter()
	{
		//Debug.Log ("Chapters  = "+Chapters);
		//Debug.Log ("CityGlobalData.m_tempSection  = "+CityGlobalData.m_temp_CQ_Section);
		if(Chapters > CityGlobalData.m_temp_CQ_Section)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        LoadResourceCallback );
			return;
		}
		if(!FunctionOpenTemp.GetWhetherContainID( 109))
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        LoadReWuCallback );
			return;
		}

		CityGlobalData .PT_Or_CQ = false;
		EnterGuoGuanmap.EnterPveUI(Chapters);
		TaskData.Instance.m_DestroyMiBao = false;
		Destroy (GameObject.Find("Secret(Clone)"));
	}
}
