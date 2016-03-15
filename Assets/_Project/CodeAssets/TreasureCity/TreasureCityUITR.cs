using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureCityUITR : MonoBehaviour {

	public static TreasureCityUITR tCityUiTr;

	public List<EventHandler> topRightHandlerList = new List<EventHandler>();

	void Awake ()
	{
		tCityUiTr = this;
	}

	void Start ()
	{
		MainCityUI.setGlobalBelongings (this.transform.gameObject, -16, 0);

		foreach (EventHandler handler in topRightHandlerList)
		{
			handler.m_click_handler += TopRightHandlerListClickBack;
		}
	}

	void TopRightHandlerListClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "BtnSetting":



			break;
		case "BtnTanBao":

			TanBaoData.Instance.TanBaoInfoReq ();

			break;
		case "BtnBackToMainCity":

			PlayerSceneSyncManager.Instance.ExitTreasureCity ();

			break;
		default:
			break;
		}
	}

	void OnDestroy ()
	{
		tCityUiTr = null;
	}
}
