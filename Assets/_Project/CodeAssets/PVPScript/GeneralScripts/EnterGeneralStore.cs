using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterGeneralStore : MonoBehaviour {

	public List<EventHandler> btnList = new List<EventHandler> ();

	void Start ()
	{
		btnList [0].m_handler += TempObj1;
		btnList [1].m_handler += TempObj2;
		btnList [2].m_handler += TempObj3;
	}

	void TempObj1 (GameObject obj)
	{
		GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.HUANGYE,GeneralControl.StoreReqType.FREE);
	}
	void TempObj2 (GameObject obj)
	{
		GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.ALLANCE,GeneralControl.StoreReqType.FREE);
	}
	void TempObj3 (GameObject obj)
	{
		GeneralControl.Instance.GeneralStoreReq (GeneralControl.StoreType.ALLIANCE_FIGHT,GeneralControl.StoreReqType.FREE);
	}
}
