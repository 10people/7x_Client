using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionDetailNoticeControllor : MonoBehaviour
{
	public UnionDetailUIControllor controllor;

	public UILabel labelUnionName;

	public UILabel labelLeaderName;

	public UILabel labelMember;

	public UILabel labelShengWang;

	public UILabel labelRenKou;

	public UILabel labelLevel;

	public UILabel labelSign;

	public GameObject btnSign;

	public GameObject btnDismis;

	public GameObject btnQuit;


	private UnionDate unionDate;


	public void refreshDate(UnionDate _date)
	{
		unionDate = _date;

		labelUnionName.text = unionDate.unionName;

		labelLeaderName.text = unionDate.leaderName;

		labelMember.text = unionDate.member + "";

		labelShengWang.text = unionDate.shengwang + "";

		labelRenKou.text = unionDate.renkou + "";

		labelLevel.text = unionDate.level + "";

		labelSign.text = unionDate.unionSignInner;

		int id = (int)JunZhuData.Instance().m_junzhuInfo.id;

		btnSign.SetActive(unionDate.leaderId == id);
		
		btnDismis.SetActive(unionDate.leaderId == id);
		
		btnQuit.SetActive(unionDate.leaderId != id);
	}

	public void OnDismis()
	{
		controllor.OnShowHint(UnionHintControllor.HintType.Dismis, null);
	}

	public void OnQuit()
	{
		controllor.OnShowHint(UnionHintControllor.HintType.Quit, null);
	}

}
