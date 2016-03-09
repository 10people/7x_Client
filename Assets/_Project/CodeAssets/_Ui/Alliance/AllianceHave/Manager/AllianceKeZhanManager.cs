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

	public	AllianceHaveResp m_AllianceHaveRes; 

	public UILabel MaxPersonNum;

	public GameObject m_Memberobj;

	public GameObject m_Eventobj;

	public bool isfirst;

	public GameObject Arelt;
	void Start () {
	
	}
	

	void Update () {
	

	}
	public void Init()
	{
		MaxPersonNum.text = "最大容纳人数："+m_AllianceHaveRes.memberMax.ToString ();

		m_Memberobj.SetActive(true);
		m_Eventobj.SetActive (false);
	
		MembersManager mMembersManager = m_Memberobj.GetComponent<MembersManager>();
		
		mMembersManager.m_allianceHaveRes = m_AllianceHaveRes;
		
		mMembersManager.Init ();

		StartCoroutine (getvalue());

		isfirst = true;
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
	public UIToggle mUItoggle;
	IEnumerator getvalue()
	{
		yield return new WaitForSeconds (0.01f);

		mUItoggle.value = !mUItoggle.value;

	}
	public void ShowMember()
	{
		m_Memberobj.SetActive(true);
		m_Eventobj.SetActive (false);
		MembersManager mMembersManager = m_Memberobj.GetComponent<MembersManager>();
		
		mMembersManager.m_allianceHaveRes = m_AllianceHaveRes;
		
		mMembersManager.Init ();
	}
	public void ShowEvent()
	{
		if(isfirst)
		{
			isfirst = false;
			return;
		}
		m_Eventobj.SetActive(true);
		m_Memberobj.SetActive(false);
		EventManager mEventManager = m_Eventobj.GetComponent<EventManager>();
		Arelt.SetActive(false);
		PushAndNotificationHelper.SetRedSpotNotification (600505, false); //关闭联盟主界面的红点
		mEventManager.Init ();
		NewAlliancemanager.Instance().Refreshtification ();
	
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
		NewAlliancemanager.Instance().BackToThis (this.gameObject);
	}
}
