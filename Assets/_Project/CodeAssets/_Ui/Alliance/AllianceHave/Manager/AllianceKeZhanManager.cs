using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class AllianceKeZhanManager : MonoBehaviour {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public	AllianceHaveResp m_AllianceHaveRes; 

	public UILabel MaxPersonNum;

	public GameObject m_Memberobj;

	public GameObject m_Eventobj;

	public bool isfirst;

	public GameObject Arelt;

	public List<GameObject> MenBerAndEvet = new List<GameObject>();
	void Awake()
	{
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "联盟客栈", 0, 0);
	}
	public void Init()
	{
		MaxPersonNum.text = "最大容纳人数："+m_AllianceHaveRes.memberMax.ToString ();

		ShowMember ();
		int id = 600505;
		//Debug.Log ("PushAndNotificationHelper.IsShowRedSpotNotification(id) = "+PushAndNotificationHelper.IsShowRedSpotNotification(id));
		if(PushAndNotificationHelper.IsShowRedSpotNotification(id))
		{
			Arelt.SetActive(true);
		}
		else
		{
			Arelt.SetActive(false);
		}
	}
	void HidAllGameobj(int Index)
	{
		MenBerAndEvet.ForEach (item => Setactive (item, false));
		MenBerAndEvet [Index].SetActive (true);
	}
	private void Setactive(GameObject go, bool a)
	{
		go.SetActive (a);
	}
	public void ShowMember()
	{
		HidAllGameobj (0);
		m_Memberobj.SetActive (true);
		m_Eventobj.SetActive (false);
		MembersManager mMembersManager = m_Memberobj.GetComponent<MembersManager>();
		
		mMembersManager.m_allianceHaveRes = m_AllianceHaveRes;
		
		mMembersManager.Init ();
	}
	public void ShowEvent()
	{
		HidAllGameobj (1);
		m_Eventobj.SetActive (true);
		m_Memberobj.SetActive (false);
		Arelt.SetActive (false);
		EventManager mEventManager = m_Eventobj.GetComponent<EventManager>();
	
		PushAndNotificationHelper.SetRedSpotNotification (600505, false); //关闭联盟主界面的红点
		mEventManager.Init ();
		NewAlliancemanager.Instance().Refreshtification ();
	
	}

	public void Close()
	{
		NewAlliancemanager.Instance().BackToThis (this.gameObject);
	}
}
