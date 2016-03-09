using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpRankReward : MonoBehaviour {

	public static PvpRankReward rankReward;

	public UILabel yuanBaoLabel;
	
	public List<EventHandler> closeHandlerList = new List<EventHandler>();

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		rankReward = this;
	}

	void OnDestroy ()
	{
		rankReward = null;
	}

	public void InItRankReward (int rank)
	{
		sEffectController.OnOpenWindowClick ();

		yuanBaoLabel.text = (BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (1).yuanbao  - (rank > BaiZhanRankTemplate.templates.Count ? 0 : BaiZhanRankTemplate.getBaiZhanRankTemplateByRank (rank).yuanbao)).ToString ();

		foreach (EventHandler handler in closeHandlerList)
		{
			handler.m_click_handler -= CloseWindow;
			handler.m_click_handler += CloseWindow;
		}
	}

	void CloseWindow (GameObject obj)
	{
		gameObject.SetActive (false);
	}
}
