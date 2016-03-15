using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TechnologyManager : MonoBehaviour ,SocketProcessor {

	public UILabel Builds;

	public KeJiList m_mKeJiList;

	public static TechnologyManager m_TechnologyManager;
	public int CurKejiType;

	public List<Technologytemp> mTechnologytempList = new List<Technologytemp>();

	public UISprite StudyVctry;
	public int JianZhu_InDex;

	public List<GameObject> BtnList = new List<GameObject>();

	public List<GameObject> m_TechnologyList = new List<GameObject>();

 	public static TechnologyManager Instance()
	{
		if (!m_TechnologyManager)
		{
			m_TechnologyManager = (TechnologyManager)GameObject.FindObjectOfType (typeof(TechnologyManager));
		}
		return m_TechnologyManager;
	}

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}

	void Start () {

	}

	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor(this);

		m_TechnologyManager = null;
	}

	void Update () {

		Builds.text = NewAlliancemanager.Instance().m_allianceHaveRes.build.ToString ();
	}
	
	public void Init()
	{
		isfirst = false;
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKJ_INFO);
		CurKejiType = 0;

	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_LMKJ_INFO://lianmeng keji back
			{
				//Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();

				KeJiList mKeJiList = new KeJiList(); // 科技等级  按顺序发送
				
				t_qx.Deserialize(application_stream, mKeJiList, mKeJiList.GetType());
				
				m_mKeJiList = mKeJiList;

				//Debug.Log("请求科技返回");
//
//				m_Technology1.SetActive(true);
//				Technology1 mTechnology1 = m_Technology1.GetComponent<Technology1>();
//				mTechnology1.m_JianZhuKeji = m_mKeJiList;
//				mTechnology1.Card = 1;
//				mTechnology1.Init ();
				mCurCard = 0;
				m_TechnologyBtn1();
				InitData();

				return true;
			}
			case ProtoIndexes.S_LMKJ_UP://lianmeng keji back
			{
				//Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ErrorMessage mErrorMessage = new ErrorMessage(); // 科技等级  按顺序发送
				
				t_qx.Deserialize(application_stream, mErrorMessage, mErrorMessage.GetType());
		
				//Debug.Log("科技升级");
				
				foreach(Technologytemp temp in mTechnologytempList)
				{
					if(temp.Keji_type == CurKejiType)
					{
						temp.Keji_level +=1;
						temp.Init();
					}
				}
		
				m_mKeJiList.list[JianZhu_InDex].lv += 1;
				StudyVctry.gameObject.SetActive(true);
				StudyVctry.spriteName = "StudyV";
				int effectid = 100180;
				UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,StudyVctry.gameObject,EffectIdTemplate.GetPathByeffectId(effectid));
				StopCoroutine("closeEffect");
				StartCoroutine( "closeEffect");

				bool showAerlt = false; // 是否显示红点
				foreach(KeJiInfo temp in m_mKeJiList.list)
				{
					LianMengKeJiTemplate mLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (CurKejiType,temp.lv);
					int ShuYuanLve = NewAlliancemanager.Instance().KejiLev;
					if(ShuYuanLve > temp.lv && mLianMengKeJiTemplate.lvUpValue  <= NewAlliancemanager.Instance().m_allianceHaveRes.build)
					{
						showAerlt = true;
						break;
					}
				}
				if(!showAerlt)
				{
					int ActiveAerlt = 600600;
					PushAndNotificationHelper.SetRedSpotNotification(ActiveAerlt,false);
					NewAlliancemanager.Instance().Refreshtification ();
				}

				return true;
			}
			case ProtoIndexes.S_LMKEJI_JIHUO://lianmeng keji back Active
			{
				Debug.Log ("S_LMKEJI_JIHUO" + ProtoIndexes.S_LMKEJI_JIHUO);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				JiHuoLMKJResp mErrorMessage = new JiHuoLMKJResp(); // 科技等级  按顺序发送
				
				t_qx.Deserialize(application_stream, mErrorMessage, mErrorMessage.GetType());
				
				//Debug.Log("科技升级");
				
				foreach(Technologytemp temp in mTechnologytempList)
				{
					if(temp.Keji_type == CurKejiType)
					{
						temp.JiHuoLv +=1;
						temp.Init();
					}
				}
				m_mKeJiList.list[JianZhu_InDex].jiHuoLv += 1;
				StudyVctry.gameObject.SetActive(true);
				// 激活特效需要更改
				StudyVctry.spriteName = "Active";
				int effectid = 100180;
				UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,StudyVctry.gameObject,EffectIdTemplate.GetPathByeffectId(effectid));
				StopCoroutine("closeEffect");
				StartCoroutine( "closeEffect");
				
				bool showAerlt = false; // 是否显示红点
				foreach(KeJiInfo temp in m_mKeJiList.list)
				{

					if(temp.jiHuoLv < temp.lv)
					{
						showAerlt = true;
						break;
					}
				}
				if(!showAerlt)
				{
					int ActiveAerlt = 600600;
					PushAndNotificationHelper.SetRedSpotNotification(ActiveAerlt,false);
					NewAlliancemanager.Instance().Refreshtification ();
				}

				return true;
			}
			default:return false;
			}
		}
		
		return false;
	}

	IEnumerator closeEffect()
	{
		yield return new WaitForSeconds (1.3f);
		StudyVctry.gameObject.SetActive(false);
	}

	bool isfirst = false;
	public UIToggle mUItoggle;
	public void InitData()
	{
		isfirst = true;

		//StartCoroutine (getvalue());
		//m_TechnologyBtn1 ();
	}
//	IEnumerator getvalue()
//	{
//		yield return new WaitForSeconds (0.01f);
//		
//		mUItoggle.value = !mUItoggle.value;
//		
//	}
	private int mCurCard;
	public void m_TechnologyBtn1()
	{
		InitItems (1);
		SetBtnState (0);
	}
	public void m_TechnologyBtn2()
	{
		InitItems (2);
		SetBtnState (1);
	}
	public void m_TechnologyBtn3()
	{
		InitItems (3);
		SetBtnState (2);
	}

	void HidAllGameobj(int Index)
	{
		m_TechnologyList.ForEach (item => Setactive (item, false));
		m_TechnologyList [Index].SetActive (true);

	}
    public void InitItems(int mCard)
	{
		if(mCard == mCurCard)
		{
			return;
		} 
		mCurCard = mCard;
		foreach(Technologytemp mTe in mTechnologytempList)
		{
			Destroy(mTe.gameObject);
		}
		mTechnologytempList.Clear ();

		Debug.Log ("mCard =" +mCard);
		HidAllGameobj (mCard-1);
		Technology1 mTechnology1 = m_TechnologyList[mCard-1].GetComponent<Technology1>();
		mTechnology1.m_JianZhuKeji = m_mKeJiList;
		mTechnology1.Card = mCard;
		mTechnology1.Init ();
	}

	void SetBtnState(int Index)
	{
		BtnList.ForEach (item => Setactive (item, false));
		BtnList [Index].SetActive (true);
	}
	private void Setactive(GameObject go, bool a)
	{
		go.SetActive (a);
	}
	public void Close()
	{
		StudyVctry.gameObject.SetActive(false);

		foreach(Technologytemp mTe in mTechnologytempList)
		{
			Destroy(mTe.gameObject);
		}
		mTechnologytempList.Clear ();
		NewAlliancemanager.Instance().BackToThis (this.gameObject);
	}
}
