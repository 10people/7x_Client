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
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init()
	{
		switch(Type)
		{
		case 1:
			int XiaoWuid = 600800;
			if(PushAndNotificationHelper.IsShowRedSpotNotification(XiaoWuid))
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
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
			int ChouJiangid = 600900;
		//	Debug.Log("ChouJiangid) = "+PushAndNotificationHelper.IsShowRedSpotNotification(ChouJiangid));
			if(PushAndNotificationHelper.IsShowRedSpotNotification(ChouJiangid))
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
			break;
		case 4:
			int HYid = 300200;
			if(PushAndNotificationHelper.IsShowRedSpotNotification(HYid))
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
			//Debug.Log("PushAndNotificationHelper.IsShowRedSpotNotification(Evet) = "+PushAndNotificationHelper.IsShowRedSpotNotification(Evet));
			if(PushAndNotificationHelper.IsShowRedSpotNotification(Evet))
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
			break;
		case 6:
			int Readroom = 600600;
			Debug.Log("PushAndNotificationHelper.IsShowRedSpotNotification(600600) = "+PushAndNotificationHelper.IsShowRedSpotNotification(Readroom));
			if(PushAndNotificationHelper.IsShowRedSpotNotification(Readroom))
			{
				Arelt.SetActive(true);
			}
			else
			{
				Arelt.SetActive(false);
			}
			break;
		default:
			break;
		}
	
	}
}
