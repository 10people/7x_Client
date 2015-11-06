using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionMemberDetailControllor : MonoBehaviour
{
	public UnionDetailUIControllor controllor;

	public UISprite spriteAvatar;

	public UILabel labelName;

	public UILabel labelLevel;

	public UILabel labelJunXian;


	private UnionMemberDate memberDate;


	public void refreshDate(UnionMemberDate _memberDate)
	{
		memberDate = _memberDate;

		labelName.text = memberDate.memberName;

		labelLevel.text = memberDate.level + "";

		labelJunXian.text = memberDate.junXian + "";
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

}
