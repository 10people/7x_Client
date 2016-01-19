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

	
	public GameObject m_Technology1;
	
	public GameObject m_Technology2;
	
	public GameObject m_Technology3;

	public UILabel mTiLi;
	
	public UILabel mTongBi;
	
	public UILabel mYuanBao;

	public UILabel Builds;
	public JianZhuList m_JianZhuList;
	public static TechnologyManager m_TechnologyManager;
	public int CurKejiType;

	public List<Technologytemp> mTechnologytempList = new List<Technologytemp>();

 	public static TechnologyManager Instance ()
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

		Builds.text = NewAlliancemanager.Instance ().m_allianceHaveRes.build.ToString ();
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

				JianZhuList mJianZhuList = new JianZhuList(); // 科技等级  按顺序发送
				
				t_qx.Deserialize(application_stream, mJianZhuList, mJianZhuList.GetType());
				
				m_JianZhuList = mJianZhuList;
				
				Debug.Log("请求科技返回");

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
		
				Debug.Log("科技升级");
				
				foreach(Technologytemp temp in mTechnologytempList)
				{
					if(temp.Keji_type == CurKejiType)
					{
						temp.Keji_level +=1;
						temp.Init();
					}
				}
				
				return true;
			}	
			default:return false;
			}
		}
		
		return false;
	}
	bool isfirst = false;
	public UIToggle mUItoggle;
	public void InitData()
	{
		isfirst = true;
		//m_TechnologyBtn1 ();
		StartCoroutine (getvalue());
	}
	IEnumerator getvalue()
	{
		yield return new WaitForSeconds (0.01f);
		
		mUItoggle.value = !mUItoggle.value;
		
	}
	public void m_TechnologyBtn1()
	{
		if(!isfirst||m_Technology1.activeInHierarchy)
		{
			return;
		}
		HidAllGameobj ();
		m_Technology1.SetActive(true);
		Technology1 mTechnology1 = m_Technology1.GetComponent<Technology1>();
		mTechnology1.m_JianZhuKeji = m_JianZhuList;
		mTechnology1.Card = 1;
		mTechnology1.Init ();
		Debug.Log ("this................111");

	}
	public void m_TechnologyBtn2()
	{
		if(!isfirst||m_Technology2.activeInHierarchy)
		{
			return;
		}
		HidAllGameobj ();
		m_Technology2.SetActive(true);
		Technology1 mTechnology1 = m_Technology2.GetComponent<Technology1>();
		mTechnology1.m_JianZhuKeji = m_JianZhuList;
		mTechnology1.Card = 2;
		mTechnology1.Init ();
	}
	public void m_TechnologyBtn3()
	{
		if(!isfirst||m_Technology3.activeInHierarchy)
		{
			return;
		}
		HidAllGameobj ();
		m_Technology3.SetActive(true);
		Technology1 mTechnology1 = m_Technology3.GetComponent<Technology1>();
		mTechnology1.m_JianZhuKeji = m_JianZhuList;
		mTechnology1.Card = 3;
		mTechnology1.Init ();
	}

	void HidAllGameobj()
	{
		m_Technology1.SetActive(false);
		
		m_Technology3.SetActive(false);
		
		m_Technology2.SetActive(false);

	}
	public void BuyTiLi()
	{
		
	}
	public void BuyTongBi()
	{
		
	}
	public void BuyYuanBao()
	{
		
	}
	public void Close()
	{
		NewAlliancemanager.Instance ().BackToThis (this.gameObject);
	}
}
