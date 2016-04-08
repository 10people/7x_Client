using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YXItem : MonoBehaviour  {

	public UILabel Itname;

	public UILabel Re_AllTimes;

	public string m_Time;

	public UISprite Box;

	public UILabel OpenLevel;

	public YouXiaInfo mYouXiaInfo;

	public UISprite Art;

	public UISprite Icon;
	public GameObject CDBox;
	private int LimitLevel ;

	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{
		int m_bigid = mYouXiaInfo.id;

		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (m_bigid);

		YouXiaOpenTimeTemplate mYouxiaoOPen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (m_bigid);

		Itname.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);
		m_Time = "剩余次数 "+mYouXiaInfo.remainTimes.ToString () + "/" + mYouxiaoOPen.maxTimes.ToString ();
		if(mYouxiaoOPen.openLevel > JunZhuData.Instance().m_junzhuInfo.level)
		{
			Re_AllTimes.gameObject.SetActive(false);
			OpenLevel.gameObject.SetActive(true);
			OpenLevel.text = mYouxiaoOPen.openLevel.ToString()+"级开放";
			this.gameObject.GetComponent<UIButton>().enabled = false;
			Box.gameObject.SetActive(true);
			CDBox.gameObject.SetActive(true);
			Art.gameObject.SetActive(false);
		}
		else{
			Box.gameObject.SetActive(false);
			if(mYouXiaInfo.remainColdTime > 0)
			{
				CountTime.gameObject.SetActive(true);
				StartCoroutine("StartCountTime");
			}
			else{
				ShowTime ();
			}
			if(mYouXiaInfo.remainTimes > 0 && mYouXiaInfo.remainColdTime <=0)
			{
				Art.gameObject.SetActive(true);
			}
			else
			{
				//this.gameObject.GetComponent<BoxCollider>().enabled = false;
				Art.gameObject.SetActive(false);
			}
		}

		//Debug.Log ("m_bigid.m_bigid = "+m_bigid);
		Icon.spriteName = m_bigid.ToString ();
	}

	private int T;
	public UILabel CountTime;
	IEnumerator StartCountTime()
	{
		Re_AllTimes.gameObject.SetActive(false);
		T = mYouXiaInfo.remainColdTime;
		CDBox.SetActive(false);

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
		//this.gameObject.GetComponent<BoxCollider>().enabled = true;

		CountTime.gameObject.SetActive(false);
		Art.gameObject.SetActive(true);
		ShowTime ();
	}
	private void ShowTime()
	{
		Re_AllTimes.gameObject.SetActive(true);
		OpenLevel.gameObject.SetActive(false);
		string str = "剩余次数 ";
		CDBox.SetActive(false);

		YouXiaOpenTimeTemplate mYouxiaoOPen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (mYouXiaInfo.id);
		if(mYouXiaInfo.remainTimes >0 )
		{
			this.gameObject.GetComponent<UIButton>().enabled = true;
			Re_AllTimes.text = MyColorData.getColorString(1, str) +MyColorData.getColorString(4, mYouXiaInfo.remainTimes.ToString()+"/"+mYouxiaoOPen.maxTimes.ToString());
		}
		else
		{
			Art.gameObject.SetActive(false);
			this.gameObject.GetComponent<UIButton>().enabled = true;
			Re_AllTimes.text = MyColorData.getColorString(1, str) +MyColorData.getColorString(5, mYouXiaInfo.remainTimes.ToString()+"/"+mYouxiaoOPen.maxTimes.ToString());
		}
//		Debug.Log ("Re_AllTimes.text = "+Re_AllTimes.text);
	}
	public void Enter()
	{
       Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.CHOOSEYOUXIA),LoadResourceCallback);
	}


	private GameObject tempOjbect;
	void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		if(tempOjbect)
		{
			return;
		}
		tempOjbect = Instantiate(p_object )as GameObject;
		
		tempOjbect.transform.parent = GameObject.Find ("YxChooseDefcult1").transform;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = Vector3.zero;
		
		YxChooseDefcult mYxChooseDefcult = tempOjbect.GetComponent<YxChooseDefcult>();
		
		//mYxChooseDefcult.HavatTimes = mYouXiaInfo.remainTimes;
		
		mYxChooseDefcult.BigId = mYouXiaInfo.id;
		
		mYxChooseDefcult.mYouXia_Info = mYouXiaInfo;

		mYxChooseDefcult.m_Times = m_Time;

		mYxChooseDefcult.Init ();
		MainCityUI.TryAddToObjectList(tempOjbect);
	}
}
