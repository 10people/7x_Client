using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class UnionSignAvatar : MonoBehaviour
{
	public UnionUIControllor controllor;

	public UILabel labelName;

	public UISprite spriteAvatar;

	public UISprite spritePlus;

	public int position;


	[HideInInspector] public UnionFriendDate friendDate;


	public void setFriendDate(UnionFriendDate _date)
	{
		friendDate = _date;

		labelName.gameObject.SetActive(friendDate != null);

		spriteAvatar.gameObject.SetActive(friendDate != null);

		spritePlus.gameObject.SetActive(friendDate == null);

		if(friendDate == null) return;

		labelName.text = friendDate.friendName;
	}

	public void click()
	{
		if(friendDate != null)
		{
			setFriendDate(null);
		}
		else
		{
			controllor.OnShowUnionFriend(position);
		}
	}

}
