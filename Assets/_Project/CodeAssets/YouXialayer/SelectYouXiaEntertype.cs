using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

using System.Text.RegularExpressions;

public class SelectYouXiaEntertype : MonoBehaviour {


	public UILabel YouXiaName;

	public UILabel MoveMentTime;

	public GameObject Award_Temp;

	public UISprite AwardIcon;

	public UILabel CountTime; //倒计时

	public YouXiaInfo mYouXiaInfo;

	public UILabel HaveTimeS; //剩余次数

	public UISprite CilderBox; // cant hit btn

	private int T;

	public static SelectYouXiaEntertype mSelectYouXia;

	[HideInInspector]public int ReMain_Times;

	public UITexture BgIcon;

	public GameObject mAlert;
	void Awake()
	{
		mSelectYouXia = this;
	}

	public void ShowAlert()
	{
		mAlert.SetActive (true);
	
	}
	public void CloseAlert()
	{
		mAlert.SetActive (false);
		
	}
	public void Init()
	{
		int m_bigid = mYouXiaInfo.id;

		string open_Day = mYouXiaInfo.openDay; // 开放时间周期

		string open_Time = mYouXiaInfo.openTime; //开放时间点

		int ReMainTime = mYouXiaInfo.remainColdTime;// 冷却时间

	    ReMain_Times = mYouXiaInfo.remainTimes; // 剩余次数

		BgIcon.mainTexture = (Texture)Resources.Load (Res2DTemplate.GetResPath (Res2DTemplate.Res.YOUXIABIGBG )+ m_bigid.ToString());

		char [] apretion = {':'};

		string [] mtime = open_Time.Split (apretion);

		//Debug.Log ("mYouXiaInfo.open = " +mYouXiaInfo.open);

		if(mYouXiaInfo.open)
		{
			if(ReMainTime > 0 )
			{
				CountTime.gameObject.SetActive(true);
				
				HaveTimeS.gameObject.SetActive(false);
				
				T = ReMainTime;
				
				CilderBox.gameObject.SetActive(true);
				
				StartCoroutine("StartCountTime");
			}
			else
			{
				CilderBox.gameObject.SetActive(false);	
				
				HaveTimeS.gameObject.SetActive(true);
				
				HaveTimeS.text = "今日剩余次数："+ReMain_Times.ToString();
				
				CountTime.gameObject.SetActive(false);
				ShouTimes ();
			}

		}
		else
		{
			CilderBox.gameObject.SetActive(true);	
			
			HaveTimeS.gameObject.SetActive(false);

			CountTime.gameObject.SetActive(false);
		}
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (m_bigid);

		InitAward (myouxia);

		YouXiaName.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);

		char [] aprcation = {','};

		string[] s1 = open_Day.Split (aprcation);

		string mTime = "";

		List<string> WeeksDay = new List<string>();

		WeeksDay.Clear();

		for (int i = 0; i < s1.Length; i++) 
		{

			switch(int.Parse(s1[i]))
			{
			case 1:
				mTime += "周一, ";
				WeeksDay.Add("Monday");
				break;
			case 2:
				mTime += "周二, ";
				WeeksDay.Add("Tuesday");
				break;
			case 3:
				mTime += "周三, ";
				WeeksDay.Add("Wednesday");
				break;
			case 4:
				mTime += "周四, ";
				WeeksDay.Add("Thursday");
				break;
			case 5:
				mTime += "周五, ";
				WeeksDay.Add("Friday");
				break;
			case 6:
				mTime += "周六, ";
				WeeksDay.Add("Saturday");
				break;
			case 7:
				mTime += "周日, ";
				WeeksDay.Add("Sunday");
				break;
			default :
				break;
			}

		}
		MoveMentTime.text = "每" + mTime+"\r\n"+open_Time +"开启";

	}
	public void ShouTimes()
	{
	    ReMain_Times = mYouXiaInfo.remainTimes; // 剩余次数

		HaveTimeS.text = "今日剩余次数："+ReMain_Times.ToString();
		if(ReMain_Times > 0 && mYouXiaInfo.open)
		{
			ShowAlert();
		}
		else
		{
			CloseAlert();
		}
	}

	private void InitAward(YouxiaPveTemplate m_youxia)
	{
		List<int> t_items = new List<int>();

		string Award = m_youxia.awardId;

		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = Award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);

			if(t_finals[0] != "")
			{
				t_items.Add(int.Parse(t_finals[0]));
			}
			
		}
		for (int n = 0; n < t_items.Count; n++)
		{
			GameObject m_Award = Instantiate(Award_Temp) as GameObject;

			m_Award.SetActive(true);

			m_Award.transform.parent = Award_Temp.transform.parent;

			m_Award.transform.localPosition = new Vector3(n * 50f - (t_items.Count - 1) * 25f,0,0);

			m_Award.transform.localScale = Vector3.one;

			Transform ico = m_Award.transform.FindChild("Award");

			UISprite mUisprite = ico.GetComponent<UISprite>();

			AwardTemp mAwad = AwardTemp.getAwardTemp_By_AwardId(t_items[n]);

			CommonItemTemplate mComm = CommonItemTemplate.getCommonItemTemplateById(mAwad.itemId);

			mUisprite.spriteName = mComm.icon.ToString();
		}

	}


	IEnumerator StartCountTime()
	{
		PushAndNotificationHelper.AddCountDownRedSpot( PushAndNotificationHelper.CountDownType.SetLocalRedSpot,
		                                              305,
		                                              T );

		while(T > 0)
		{
			T -= 1;

			int M = (int)(T/60);

			int S = (int)(T % 60);
			string m_s = "";
			if(S < 10)
			{
				 m_s = "0"+S.ToString();
			}
			else
			{
				m_s = S.ToString();
			}
			CountTime.text = M.ToString()+":"+m_s+" 后可进入";

			yield return new WaitForSeconds(1f);
		}
		if(mYouXiaInfo.open)
		{
			CilderBox.gameObject.SetActive(false);
		}
		else{
			
			CilderBox.gameObject.SetActive(true);		
		}
		
		HaveTimeS.gameObject.SetActive(true);
		
		HaveTimeS.text = "今日剩余次数："+ReMain_Times.ToString();
		ShouTimes ();
		CountTime.gameObject.SetActive(false);
	
	}

	public void  EnterbattleBtn()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.CHOOSEYOUXIA),LoadResourceCallback);
	}

	private GameObject tempOjbect;

	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		if(tempOjbect)
		{
			return;
		}
	    tempOjbect = Instantiate(p_object )as GameObject;

		tempOjbect.transform.parent = GameObject.Find ("FightTypeSelectLayer").transform;

		tempOjbect.transform.localScale = Vector3.one;

		tempOjbect.transform.localPosition = Vector3.zero;

		ChooseYouXiaUIManager mChooseYouXiaUIManager = tempOjbect.GetComponent<ChooseYouXiaUIManager>();

		mChooseYouXiaUIManager.HavatTimes = mYouXiaInfo.remainTimes;

		mChooseYouXiaUIManager.BigId = mYouXiaInfo.id;

		mChooseYouXiaUIManager.mYouXia_Info = mYouXiaInfo;

		mChooseYouXiaUIManager.Init ();

		EnterYouXiaBattle.GlobleEnterYouXiaBattle.ShowOrClose ();
	}
}
