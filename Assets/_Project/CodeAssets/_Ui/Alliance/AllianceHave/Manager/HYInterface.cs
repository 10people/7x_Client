using UnityEngine;
using System.Collections;

public class HYInterface : MonoBehaviour {

	public GameObject Arelt;
	/// <summary>
	/// The type.1 小屋 2 图腾 3 宗庙 4 荒野 5 客栈 6 书院 科技
	/// </summary>
	public int Type;
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		bool IsShow = false;
		switch(Type)
		{
		case 1:
			int XiaoWuid = 600800;
		    IsShow = PushAndNotificationHelper.IsShowRedSpotNotification(XiaoWuid);
			Arelt.SetActive(IsShow);
			break;
		case 2:
			int Mobai = 400000;
			int fangcan = 400017;

			if(PushAndNotificationHelper.IsShowRedSpotNotification(Mobai)||PushAndNotificationHelper.IsShowRedSpotNotification(fangcan))
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
			break;
		case 3:
			int ChouJiangid1 = 600900;
			int ChouJiangid2 = 600905;

			if(!PushAndNotificationHelper.IsShowRedSpotNotification(ChouJiangid1) || !PushAndNotificationHelper.IsShowRedSpotNotification(ChouJiangid2))
			{
				Arelt.SetActive(false);
			}
			else
			{
				Arelt.SetActive(true);
			}
			break;
		case 4:
			int HYid = 300200;
			if(PushAndNotificationHelper.IsShowRedSpotNotification(HYid) && NewAlliancemanager.Instance().m_allianceHaveRes.level >= 2)
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
			break;
		case 5:
			int Evet = 600500;
		    IsShow = PushAndNotificationHelper.IsShowRedSpotNotification(Evet);
			Arelt.SetActive(IsShow);
			break;
		case 6:
			int Readroom = 600600;
			IsShow = PushAndNotificationHelper.IsShowRedSpotNotification(Readroom);
			Arelt.SetActive(IsShow);
			break;
		default:
			break;
		}
	
	}
}
