using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HeroTips : MonoBehaviour {

	public UILabel levelLabel;

	public UILabel heroNameLabel;

	public UILabel allianceNameLabel;

	public UILabel rankLabel;

	public UILabel victoryLabel;

	public UILabel zhanLiLabel;

	private UIPanel tipsPanel;
	
	[HideInInspector]public bool isAlpha;
	
	void Start ()
	{
		tipsPanel = this.GetComponent<UIPanel> ();
	}

	//获得玩家君主信息
	public void GetHeroInfo (JunZhuInfo tempInfo)
	{
		levelLabel.text = tempInfo.level.ToString ();

		heroNameLabel.text = tempInfo.name;

		allianceNameLabel.text = tempInfo.lianMeng;

		rankLabel.text = tempInfo.rank.ToString ();

		victoryLabel.text = tempInfo.winCount.ToString ();

		zhanLiLabel.text = tempInfo.zhanli.ToString ();
	}

	void Update ()
	{
		if (isAlpha)
		{
			tipsPanel.alpha = 1;
		}
		
		else
		{
			tipsPanel.alpha -= 0.05f;
		}
	}
}
