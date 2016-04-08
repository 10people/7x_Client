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

	public bool m_isDrag = false;

	public List<GameObject> BtnList = new List<GameObject>();

	public List<GameObject> AlertList = new List<GameObject>();

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
//				Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();

				KeJiList mKeJiList = new KeJiList(); // 科技等级  按顺序发送
				
				t_qx.Deserialize(application_stream, mKeJiList, mKeJiList.GetType());
				
				m_mKeJiList = mKeJiList;
				isfirst = true;
				mCurCard = 0;
				m_TechnologyBtn1();
				InitData();

				return true;
			}
			case ProtoIndexes.S_LMKJ_UP://lianmeng keji back
			{
				Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
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
					}
					temp.Init();
				}
		
				m_mKeJiList.list[JianZhu_InDex].lv += 1;
				StudyVctry.gameObject.SetActive(true);
				StudyVctry.spriteName = "StudyV";
				int effectid = 100180;
				UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,StudyVctry.gameObject,EffectIdTemplate.GetPathByeffectId(effectid));
				StopCoroutine("closeEffect");
				StartCoroutine( "closeEffect");

				bool showAerlt = false; // 是否显示红点
				for(int i = 0; i < m_mKeJiList.list.Count; i ++)
				{
					LianMengKeJiTemplate mLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Zu_And_Level (i,m_mKeJiList.list[i].lv);
					int ShuYuanLve = NewAlliancemanager.Instance().KejiLev;



					if(mLianMengKeJiTemplate.shuYuanlvNeeded <= ShuYuanLve && ShuYuanLve > m_mKeJiList.list[i].lv && mLianMengKeJiTemplate.lvUpValue  <= NewAlliancemanager.Instance().m_allianceHaveRes.build)
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
				ReFreshData();
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
					Debug.Log(temp.jiHuoLv);
					Debug.Log(temp.lv);
					if(temp.jiHuoLv < temp.lv)
					{
						showAerlt = true;
						break;
					}
				}
				if(!showAerlt)
				{
					Debug.Log("======================123");
					int ActiveAerlt = 600600;
					PushAndNotificationHelper.SetRedSpotNotification(ActiveAerlt,false);
					NewAlliancemanager.Instance().Refreshtification ();
				}
				ReFreshData();
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

	void ReFreshData()
	{
		AlertList.ForEach (item => Setactive (item, false));
		int Identy = NewAlliancemanager.Instance().m_allianceHaveRes.identity;
		int ShuYuanLevel = NewAlliancemanager.Instance().KejiLev;
		bool Btn1Alert = false;
		bool Btn2Alert = false;
		bool Btn3Alert = false;
		int Alliance_Builds = NewAlliancemanager.Instance().m_allianceHaveRes.build;
		List<LianMengKeJiTemplate> LianMengKeJiTemplateList = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_type ( );
		for(int i = 0; i< m_mKeJiList.list.Count ; i ++)
		{
			if(Identy == 0)
			{
				LianMengKeJiTemplate m_LianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (LianMengKeJiTemplateList[i].type,0);
				int OPenLv = m_LianMengKeJiTemplate.shuYuanlvNeeded;
				if(m_mKeJiList.list[i].jiHuoLv < m_mKeJiList.list[i].lv && OPenLv <= ShuYuanLevel)
				{
					if(i < 11)
					{
						Btn1Alert = true;
					}
					if(i >= 11 && i < 13)
					{
						Btn2Alert = true;
					}
					if(i >= 13)
					{
						Btn3Alert = true;
					}
				}
			}
			else
			{
				LianMengKeJiTemplate m_LianMengKeJiTemplat = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (LianMengKeJiTemplateList[i].type,0);
				int OPenLv = m_LianMengKeJiTemplat.shuYuanlvNeeded;
				LianMengKeJiTemplate m_LianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (LianMengKeJiTemplateList[i].type,m_mKeJiList.list[i].lv);
				if(m_mKeJiList.list[i].lv < ShuYuanLevel && m_LianMengKeJiTemplate.lvUpValue <= Alliance_Builds && OPenLv <= ShuYuanLevel)
				{
					if(i < 11)
					{
						Btn1Alert = true;
					}
					if(i >= 11 && i < 13)
					{
						Btn2Alert = true;
					}
					if(i >= 13)
					{
//						Debug.Log("m_LianMengKeJiTemplate.lvUpValue = "+m_LianMengKeJiTemplate.lvUpValue);
//						Debug.Log("mAlliance_Builds = "+Alliance_Builds);
//						Debug.Log("m_mKeJiList.list[i].lv = "+m_mKeJiList.list[i].lv);
						Btn3Alert = true;
					}
				}
			}
		}
		BoolShowAlert (Btn1Alert,Btn2Alert,Btn3Alert);
	}
	
	public void InitData()
	{

		ReFreshData ();
	}
	void BoolShowAlert(bool a,bool b ,bool c)
	{

		if(a)
		{
			ShowAlert(0);
		}
		if(b)
		{
			ShowAlert(1);
		}
		if(c)
		{
			ShowAlert(2);
		}

	}
	public void CloseAlert(int  Index)
	{
		AlertList [Index].SetActive (false);
	}
	void ShowAlert(int  Index)
	{
		AlertList [Index].SetActive (true);
	}
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
	void OnDrag(Vector2 delta)
	{
		Debug.Log ("m_isDrag = "+m_isDrag);
		m_isDrag = true;
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

//		Debug.Log ("mCard =" +mCard);
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
