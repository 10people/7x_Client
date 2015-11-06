using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class _MyAllianceManager : MonoBehaviour  , SocketListener{

    public	AllianceHaveResp m_allianceHaveRes; 

	public static _MyAllianceManager m_MyAllianceManager;

	public GameObject m_Infomationobj;

	public GameObject m_Memberobj;

	public GameObject m_Eventobj;

	public GameObject m_Applyobj;

	public GameObject m_ApplyBtn;

	public ScaleEffectController m_ScaleEffectController;

	public List<GameObject> m_BtnList = new List<GameObject>();

	public GameObject m_ApplyAlert;

	public GameObject m_EvenrAlert;

	public static _MyAllianceManager Instance ()
	{
		if (!m_MyAllianceManager)
		{
			m_MyAllianceManager = (_MyAllianceManager)GameObject.FindObjectOfType (typeof(_MyAllianceManager));
		}
		return m_MyAllianceManager;
	}
	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
	}
	void Start()
	{
		Init ();
	}
	void Update()
	{
		Shownotice ();
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				
			case ProtoIndexes.ALLIANCE_HAVE_RESP://返回联盟信息， 给有联盟的玩家返回此条信息
			{
				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				AllianceHaveResp allianceHaveRes = new AllianceHaveResp();
				
				t_qx.Deserialize(t_tream, allianceHaveRes, allianceHaveRes.GetType());

				Debug.Log ("监听到联盟信息返回了");

				m_allianceHaveRes = allianceHaveRes;

				InitUI();
				if(m_Infomationobj.activeInHierarchy)
				{
					ShowInfomation();
				}
				return true;
			}
			case ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY://主界面联盟按钮提示
			{
				Debug.Log ("联盟升级：" + ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY);

				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.Alliance_UP ),
				                        ResLoaded );

				return true;
			}
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}

	public void Init()
	{
		AllianceData.Instance.RequestData ();

		if(AllianceData.Instance.IsAllianceUP)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.Alliance_UP ),
			                        ResLoaded );

		}
	}
	public static void ResLoaded( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		
		GameObject tempObject = ( GameObject )Instantiate( p_object );
		
		tempObject.transform.position = new Vector3( 0,500,0 );
		
		tempObject.transform.localScale = Vector3.one;

		AllianceData.Instance.IsAllianceUP = false;
	}
	void InitUI()
	{
		if(m_allianceHaveRes.identity == 2 || m_allianceHaveRes.identity == 1)
		{
			m_ApplyBtn.SetActive(true);
		}
		else
		{
			m_ApplyBtn.SetActive(false);
		}
	}
	public void Shownotice()
	{
	    if(CityGlobalData.AllianceApplyNotice > 0)
		{
			m_ApplyAlert.SetActive(true);

		}else
		{
			m_ApplyAlert.SetActive(false);
		}
		if(CityGlobalData.AllianceEventNotice > 0)
		{
			m_EvenrAlert.SetActive(true);
		}
		else
		{
			m_EvenrAlert.SetActive(false);
		}
		if(CityGlobalData.AllianceEventNotice <= 0 && CityGlobalData.AllianceApplyNotice <= 0)
		{
			PushAndNotificationHelper.SetRedSpotNotification (410000, false); //关闭联盟主界面的红点
		}
	}
	public void ShowInfomation()
	{
		if(m_allianceHaveRes == null)
		{
			return;
		}
		HidAllGameobj ();

		Debug.Log ("0000000000000000000000000000000");

		m_Infomationobj.SetActive(true);

		MyAllianceInfo mMyAllianceInfo = m_Infomationobj.GetComponent<MyAllianceInfo>();

		mMyAllianceInfo.m_Alliance = m_allianceHaveRes;

		mMyAllianceInfo.Init ();
	}
	public void ShowEvent()
	{
		if(m_allianceHaveRes == null)
		{
			return;
		}
		HidAllGameobj();

		m_Eventobj.SetActive(true);

		EventManager mEventManager = m_Eventobj.GetComponent<EventManager>();
		
		mEventManager.Init ();
		CityGlobalData.AllianceEventNotice = 0;

	}

	public void ShowMember()
	{
		if(m_allianceHaveRes == null)
		{
			return;
		}
		HidAllGameobj ();

		m_Memberobj.SetActive(true);
		
		MembersManager mMembersManager = m_Memberobj.GetComponent<MembersManager>();
		
		mMembersManager.m_allianceHaveRes = m_allianceHaveRes;
		
		mMembersManager.Init ();
	}
	public void ShowApply()
	{
		if(m_allianceHaveRes == null)
		{
			return;
		}
		HidAllGameobj ();

		m_Applyobj.SetActive(true);

		ApplyManager mApplyManager = m_Applyobj.GetComponent<ApplyManager>();
		
		mApplyManager.m_tempInfo = m_allianceHaveRes;
		
		mApplyManager.Init ();

	}
	public void Closed()
	{
		m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
		m_ScaleEffectController.OnCloseWindowClick();
	}
	void DoCloseWindow()
	{
		MainCityUI.TryRemoveFromObjectList(gameObject);
		Destroy (this.gameObject);
	}
	void HidAllGameobj()
	{
		m_Infomationobj.SetActive(false);
		
		m_Memberobj.SetActive(false);
		
		m_Eventobj.SetActive(false);
		
		m_Applyobj.SetActive(false);
	}
}
