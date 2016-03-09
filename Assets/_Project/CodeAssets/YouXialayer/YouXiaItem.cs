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

	//public GameObject YouXiaLimit;

	// GameObject mUIroot;

	public UISprite UIIcon;

	public YouXiaInfo mYou_XiaInfo;

	[HideInInspector]
	public GameObject IconSamplePrefab;

	public GameObject AwardROot;

	public UISprite Diffcult;

	public UISprite DiffcultNameID;

	public GameObject Win;

	void Start () {
	
	}

	void Update () {
	
	}

	public void Init()
	{
		Win.SetActive (false);
		//Debug.Log ("YxChooseDefcult.Instance().PassedId.count = "+YxChooseDefcult.Instance().PassedId);
		foreach(int id in YxChooseDefcult.Instance().PassedId)
		{
			if(L_id == id)
			{
				Win.SetActive(true);
				break;
			}
		}
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		string difficult = NameIdTemplate.GetName_By_NameId (myouxia.smaName);

		m_item_name = NameIdTemplate.GetName_By_NameId (myouxia.bigName);

		YouXia_difficulty.text = difficult;

		Diffcult.spriteName = "D" + bigid.ToString ();

		DiffcultNameID.spriteName = bigid.ToString ()+"-"+YouXiadifficulty.ToString();
		//UIIcon.spriteName = "00"+YouXiadifficulty.ToString (); //暂时没有cion

		YouxiaPveTemplate mYouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		if(mYouxia.monarchLevel > JunZhuData.Instance().m_junzhuInfo.level)
		{
			ColiderBox.SetActive(true);

			//YouXiaLimit.SetActive(true);

			YouXia_Limit.text = "等级达到"+mYouxia.monarchLevel.ToString()+"可解锁";
		}
		else
		{
			ColiderBox.SetActive(false);

			//YouXiaLimit.SetActive(false);
		}
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}
	}

	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		List<int> t_items = new List<int>();

		t_items.Clear ();

		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateById (L_id);

		string Award = myouxia.awardId;

	//	Debug.Log ("myouxia.awardId = "+myouxia.awardId);

		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = Award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[0] != "" && !t_items.Contains(int.Parse(t_finals[0])))
			{
				t_items.Add(int.Parse(t_finals[0]));
			}
			
		}
		//Debug.Log ("t_items.Count = "+t_items.Count);
		int count = 0;
		for(int i = 0; i < t_items.Count ; i ++)
		{
			List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(t_items[i]);

			//Debug.Log ("mAwardTemp.Count = "+mAwardTemp.Count);
			for(int j = 0; j < mAwardTemp.Count ; j ++)
			{
				count += 1;
				if(count > 3)
				{
					break;
				}
				if (IconSamplePrefab == null)
				{
					IconSamplePrefab = p_object as GameObject;
				}
				//Debug.Log("mAwardTemp[j] = "+mAwardTemp[j]);
				GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
				
				iconSampleObject.SetActive(true);
				
				iconSampleObject.transform.parent = AwardROot.transform;
				
				//iconSampleObject.transform.localScale = new Vector3(0.7f,0.7f,1);
				
				var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
				
				var iconSpriteName = "";
				
				CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[j].itemId);
				
				iconSpriteName = mItemTemp.icon.ToString();
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.item);
				
				NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
				
				string mdesc = DescIdTemplate.GetDescriptionById(mItemTemp.descId);
				
				var popTitle = mNameIdTemplate.Name;
				
				var popDesc = mdesc;
				
				iconSampleManager.SetIconByID(mItemTemp.id, "", 7);
				iconSampleManager.SetIconPopText(mItemTemp.id, popTitle, popDesc, 1);
				iconSampleObject.transform.localScale = Vector3.one * 0.4f;
			}

			//iconSampleManager.SetAwardNumber(m_OneKeyAward[i].pieceNumber);

		}
		if(count == 1)
		{
			AwardROot.GetComponent<UIGrid>().m_x_offset = 0;
		}
		if(count == 2)
		{
			AwardROot.GetComponent<UIGrid>().m_x_offset = -20;
		}
		if(count == 3)
		{
			AwardROot.GetComponent<UIGrid>().m_x_offset = -40;
		}
		AwardROot.GetComponent<UIGrid>().repositionNow = true;
	}
	public void EnterBattleBtn()
	{

//		foreach(YouXiaItem myoux in YxChooseDefcult.Instance().YouXiaItemmList)
//		{
//			if(myoux.mYou_XiaInfo.id == mYou_XiaInfo.id)
//			{
//				CountTime = mYou_XiaInfo.remainTimes;
//			}
//		}
//
//		Debug.Log ("CountTime = " +CountTime);
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YOUXIAENEMY_UI), LoadYouXiaEnemyUIBack);
//		if(CountTime > 0)
//		{
//
//		}
//		else
//		{
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
//		}
	}

	private GameObject tempOjbect;

	void LoadYouXiaEnemyUIBack( ref WWW p_www, string p_path, Object p_object )
	{
		if(tempOjbect)
		{
			return;
		}
		tempOjbect = Instantiate(p_object )as GameObject;

		//EnterYouXiaBattle.GlobleEnterYouXiaBattle.ThirdNeedCloseObg = mUIroot;

		//tempOjbect.transform.parent = GameObject.Find ("YxChooseDefcult1").transform;
		
		tempOjbect.transform.localScale = new Vector3(0,-200,0);
		
		tempOjbect.transform.localPosition = Vector3.zero;

		NewYXUI mNewYXUI = tempOjbect.GetComponent<NewYXUI>();

		mNewYXUI.l_id = L_id;

		mNewYXUI.big_id = bigid;

		//ewYXUI.Difficult = YouXia_difficulty.text;

		//mNewYXUI.YouxiaName = m_item_name;

		mNewYXUI.m_You_XiaInfo = mYou_XiaInfo;

		mNewYXUI.Init ();
		MainCityUI.TryAddToObjectList (tempOjbect);
		//EnterYouXiaBattle.GlobleEnterYouXiaBattle.ThirdShowOrClose ();
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
