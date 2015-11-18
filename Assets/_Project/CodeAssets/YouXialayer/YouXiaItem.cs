using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YouXiaItem : MonoBehaviour {

	[HideInInspector]

	public int L_id;

	public int bigid;

	public GameObject ColiderBox;

	[HideInInspector]

	public int CountTime; // 次数

	[HideInInspector]

	public int YouXiadifficulty;

	[HideInInspector]
	
	public string m_item_name;


	public UILabel YouXia_difficulty;

	public UILabel YouXia_Limit;

	public GameObject YouXiaLimit;

	public GameObject mUIroot;

	public UISprite UIIcon;

	public YouXiaInfo mYou_XiaInfo;

	void Start () {
	
	}

	void Update () {
	
	}

	public void Init()
	{
	
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		string difficult = NameIdTemplate.GetName_By_NameId (myouxia.smaName);

		YouXia_difficulty.text = difficult;

		UIIcon.spriteName = "00"+YouXiadifficulty.ToString ();

		YouxiaPveTemplate mYouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		if(mYouxia.monarchLevel > JunZhuData.Instance().m_junzhuInfo.level)
		{
			ColiderBox.SetActive(true);

			YouXiaLimit.SetActive(true);

			YouXia_Limit.text = LanguageTemplate.GetText(LanguageTemplate.Text.JUNZHU_LV)+mYouxia.monarchLevel.ToString()+"后开启";// get from Table
		}
		else
		{
			ColiderBox.SetActive(false);

			YouXiaLimit.SetActive(false);
		}

	}
	public void EnterBattleBtn()
	{
		YouxiaPveTemplate mYouxiapve = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		if(JunZhuData.Instance().m_junzhuInfo.level < mYouxiapve.monarchLevel)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), JunZhuLevelLock);
			return;
		}
		foreach(SelectYouXiaEntertype myoux in EnterYouXiaBattle.GlobleEnterYouXiaBattle.SelectYouXiaEntertypeList)
		{
			if(myoux.mYouXiaInfo.id == mYou_XiaInfo.id)
			{
				CountTime = myoux.ReMain_Times;
			}
		}

		Debug.Log ("CountTime = " +CountTime);

		if(CountTime > 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YOUXIAENEMY_UI), LoadYouXiaEnemyUIBack);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
		}
	}

	private GameObject tempOjbect;

	void LoadYouXiaEnemyUIBack( ref WWW p_www, string p_path, Object p_object )
	{
		if(tempOjbect)
		{
			return;
		}
		tempOjbect = Instantiate(p_object )as GameObject;

		EnterYouXiaBattle.GlobleEnterYouXiaBattle.ThirdNeedCloseObg = mUIroot;

		tempOjbect.transform.parent = GameObject.Find ("FightTypeSelectLayer").transform;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = Vector3.zero;

		YouXiaEnemyUI mYouXiaEnemyUI = tempOjbect.GetComponent<YouXiaEnemyUI>();

		mYouXiaEnemyUI.l_id = L_id;

		mYouXiaEnemyUI.big_id = bigid;

		mYouXiaEnemyUI.StrDifficult = YouXia_difficulty.text;

		mYouXiaEnemyUI.Youxia_Name = m_item_name;

		mYouXiaEnemyUI.m_You_XiaInfo = mYou_XiaInfo;

		mYouXiaEnemyUI.Init ();
		EnterYouXiaBattle.GlobleEnterYouXiaBattle.ThirdShowOrClose ();
	}
	void LoadBack_2(ref WWW p_www, string p_path, Object p_object)
	{
		
		//string str1 = "元宝不足";
		string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YOU_XIA_13);
		
		string str1 = "对不起，您今日的活动次数已经用尽。";
		
		string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		uibox.setBox(titleStr, null, str1, null, strbtn,  null, null, null, null);
	}

	void JunZhuLevelLock(ref WWW p_www, string p_path, Object p_object)
	{
		
		//string str1 = "level ";
		string titleStr =  LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);

		YouxiaPveTemplate mYouxiapve = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_LIMIT)+mYouxiapve.monarchLevel.ToString()+"后开启！";

		string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		uibox.setBox(titleStr, null, str1, null, strbtn,  null, null, null, null);
	}
}
