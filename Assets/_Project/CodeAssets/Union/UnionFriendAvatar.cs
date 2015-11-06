using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionFriendAvatar : MonoBehaviour
{
	public UnionFriendControllor controllor;

	public UILabel labelName;

	public UISprite spriteAvatar;

	public UISprite spriteSourse;


	private UnionFriendDate friend;


	public void refreshDate(UnionFriendDate _friend)
	{
		friend = _friend;

		labelName.text = friend.friendName;

		spriteSourse.spriteName = "union_friend_icon_" + friend.sourse;
	}

	public void selected()
	{
		controllor.selectedAvatar(friend);
	}

}
