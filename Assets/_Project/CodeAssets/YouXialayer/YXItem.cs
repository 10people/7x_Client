using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YXItem : MonoBehaviour  {

	public UILabel Re_AllTimes;

	public string m_Time;

	public UISprite Box;

	public UILabel OpenLevel;

	public YouXiaInfo mYouXiaInfo;

	public UISprite Art;

	public UISprite Icon;
	private int LimitLevel ;
	public bool isQianchonglou = false;
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void InitQianChongLouBtn()
	{
		YouXiaOpenTimeTemplate mYouxiaoOPen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (101);
		Re_AllTimes.gameObject.SetActive(false);
		Icon.spriteName = "Function_"+mYouxiaoOPen.functionID.ToString();
//		if()
//		{
//			Art.gameObject.SetActive(true);
//		}
//		else
//		{
		Art.gameObject.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(mYouxiaoOPen.functionID));
//		}300
//		int qianchonglouFunctionID = 300;
		if(!FunctionOpenTemp.IsHaveID(mYouxiaoOPen.functionID))
		{

			OpenLevel.gameObject.SetActive(true);
			
			mTips = FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).m_sNotOpenTips;

			if(FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).Level < 0)
			{
				OpenLevel.text = "即将开放";
				
			}
			else
			{
				OpenLevel.text = FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).m_sNotOpenTips;
			}
			Box.gameObject.SetActive(true);
			Icon.color = new Color(0,0,0,255);
			Art.gameObject.SetActive(false);
		}
		else
		{
			mTips = "";
			Icon.color = new Color(255,255,255,255);
			Box.gameObject.SetActive(false);
			OpenLevel.gameObject.SetActive(false);
		}
	
		isQianchonglou = true;
	}
	private string mTips;
	public void Init()
	{
		int m_bigid = mYouXiaInfo.id;

		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (m_bigid);

		YouXiaOpenTimeTemplate mYouxiaoOPen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (m_bigid);

		m_Time = "剩余 "+mYouXiaInfo.remainTimes.ToString () + "/" + mYouxiaoOPen.maxTimes.ToString ();
		mTips = "";
		if(!FunctionOpenTemp.IsHaveID(mYouxiaoOPen.functionID))
		{
			Re_AllTimes.gameObject.SetActive(false);
			OpenLevel.gameObject.SetActive(true);

			mTips = FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).m_sNotOpenTips;

			if(FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).Level < 0)
			{
				OpenLevel.text = "即将开放";

			}
			else
			{
				OpenLevel.text = FunctionOpenTemp.GetTemplateById(mYouxiaoOPen.functionID).m_sNotOpenTips;
			}

			Box.gameObject.SetActive(true);
			Icon.color = new Color(0,0,0,255);
			Art.gameObject.SetActive(false);
		}
		else{
			Box.gameObject.SetActive(false);
			Icon.color = new Color(255,255,255,255);
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
		Icon.spriteName = "Function_" + mYouxiaoOPen.functionID; // 有图标了就换
//		Icon.spriteName = "Function_250";
	}

	private int T;
	public UILabel CountTime;
	IEnumerator StartCountTime()
	{
		Re_AllTimes.gameObject.SetActive(false);
		T = mYouXiaInfo.remainColdTime;

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
		string str = "剩余 ";

		YouXiaOpenTimeTemplate mYouxiaoOPen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id (mYouXiaInfo.id);
		if(mYouXiaInfo.remainTimes >0 )
		{
			this.gameObject.GetComponent<UIButton>().enabled = true;
			Re_AllTimes.text = str +MyColorData.getColorString(4, mYouXiaInfo.remainTimes.ToString()+"/"+mYouxiaoOPen.maxTimes.ToString());
		}
		else
		{
			Art.gameObject.SetActive(false);
			this.gameObject.GetComponent<UIButton>().enabled = true;
			Re_AllTimes.text = str +MyColorData.getColorString(5, mYouXiaInfo.remainTimes.ToString()+"/"+mYouxiaoOPen.maxTimes.ToString());
		}
//		Debug.Log ("Re_AllTimes.text = "+Re_AllTimes.text);
	}
	public void Enter()
	{
		if(mTips != "")
		{
			ClientMain.m_UITextManager.createText(mTips);
			return;
		}
		if (!isQianchonglou)
		{
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.CHOOSEYOUXIA), LoadResourceCallback);

		} else
		{
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.QIANCHONGLOU), Load_QCL_ResourceCallback);
		}
	}
	private GameObject QCL_tempOjbect;
	void Load_QCL_ResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		if(QCL_tempOjbect == null)
		{
			QCL_tempOjbect = Instantiate(p_object )as GameObject;
			
			QCL_tempOjbect.transform.localScale = Vector3.one;
			
			QCL_tempOjbect.transform.localPosition = new Vector3 (100,100,0);

			MainCityUI.TryAddToObjectList(QCL_tempOjbect);
		}		
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
		Debug.Log(m_Time);
		mYxChooseDefcult.m_Times = m_Time;

		mYxChooseDefcult.Init ();
		MainCityUI.TryAddToObjectList(tempOjbect);
	}
}
