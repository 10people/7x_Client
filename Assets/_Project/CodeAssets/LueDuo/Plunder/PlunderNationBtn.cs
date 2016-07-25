using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PlunderNationBtn : MonoBehaviour {

	private GuoInfo nationInfo;

	public UISprite btnBg;
	public GameObject m_selectNation;
	public GameObject m_enemyMark;

	private int btnIndex;

	/// <summary>
	/// Ins it nation button.
	/// </summary>
	/// <param name="tempInfo">Temp info.</param>
	/// <param name="tempIndex">Temp index.</param>
	/// <param name="tempPos">Temp position.</param>
	public void InItNationBtn (GuoInfo tempInfo,int tempIndex)
	{
		nationInfo = tempInfo;
		btnIndex = tempIndex;
	}

	/// <summary>
	/// Buttons the animation.
	/// </summary>
	/// <param name="isAnimate">If set to <c>true</c> is animate.</param>
	public void BtnAnimation (bool isAnimate)
	{	
		btnBg.spriteName = "nation_" + (isAnimate ? nationInfo.guojiaId.ToString () : nationInfo.guojiaId + "1");
		m_selectNation.SetActive (isAnimate);
		m_enemyMark.SetActive (isAnimate ? (btnIndex == 0 || btnIndex == 1 ? true : false) : false);
	}
}
