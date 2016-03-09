using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EmailSelectFriend : MonoBehaviour {

	public static EmailSelectFriend selectFriend;

	public UIScrollView friendSc;
	public UIScrollBar friendSb;
	public UIGrid friendGrid;
	public GameObject friendItemObj;
	private List<GameObject> friendItemList = new List<GameObject> ();

	public EventHandler closeHandler;
	public EventHandler zheZhao;

	public UILabel desLabel;

	public ScaleEffectController sEffectController;

	void Awake ()
	{
		selectFriend = this;
	}

	/// <summary>
	/// Ins it select friend page.
	/// </summary>
	public void InItSelectFriendPage (string tempName)
	{
		sEffectController.OnOpenWindowClick ();

		List<FriendJunzhuInfo> friendList = FriendOperationData.Instance.m_FriendListInfo.friends;

		friendItemList = QXComData.CreateGameObjectList (friendItemObj,friendGrid,friendList.Count,friendItemList);

		for (int i = 0;i < friendList.Count;i ++)
		{
			friendGrid.Reposition ();
			friendSc.UpdateScrollbars (true);
			EmailFriendItem emailFriend = friendItemList[i].GetComponent<EmailFriendItem> ();
			emailFriend.InItFriendItem (friendList[i]);
			emailFriend.RefreshSelectState (tempName);
		}

		friendSc.enabled = friendList.Count > 4 ? true : false;
		friendSb.gameObject.SetActive (friendList.Count > 4 ? true : false);

		desLabel.text = friendList.Count > 0 ? "" : "还没有好友";

		closeHandler.m_click_handler -= CloseHandlerClickBack;
		closeHandler.m_click_handler += CloseHandlerClickBack;
		zheZhao.m_click_handler -= CloseHandlerClickBack;
		zheZhao.m_click_handler += CloseHandlerClickBack;
	}

	public void CloseHandlerClickBack (GameObject obj)
	{
		friendSb.value = 0;
		gameObject.SetActive (false);
	}
}
