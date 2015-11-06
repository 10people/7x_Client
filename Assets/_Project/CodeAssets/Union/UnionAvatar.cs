using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionAvatar : MonoBehaviour
{
	public UnionUIControllor uiControllor;

	public UISprite spriteIcon;

	public UILabel labelName;


	private UnionDate unionDate;


	public void refreshDate(UnionDate date)
	{
		unionDate = date;

		labelName.text = date.unionName;
	}

	public void enterUnion()
	{
		uiControllor.OnShowUnionDetail(unionDate);
	}

}
