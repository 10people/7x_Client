using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionFriendDetailControllor : MonoBehaviour
{
	public UnionUIControllor controllor;

	public UISprite spriteAvatar;

	public UILabel labelName;

	public UILabel labelLevel;

	public UILabel labelJunXian;


	private UnionFriendDate friendDate;

	private int position;


	public void refreshDate(UnionFriendDate _friendDate, int _position)
	{
		position = _position;

		friendDate = _friendDate;

		labelName.text = friendDate.friendName;

		labelLevel.text = friendDate.level + "";

		labelJunXian.text = friendDate.junXian + "";
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}

	public void selectedAvatar()
	{
		controllor.OnShowNoticeLayer();
		
		controllor.OnShowUnionSign();
		
		controllor.refreshSignControllor(friendDate, position, "", 0);
	}
}
