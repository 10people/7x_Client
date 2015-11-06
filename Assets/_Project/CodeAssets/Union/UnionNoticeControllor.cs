using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnionNoticeControllor : MonoBehaviour
{
	public UnionUIControllor controllor;

	public GameObject unionAvatarTemple;


	private List<GameObject> avatars = new List<GameObject>();


	public void refreshDate()
	{
		if(avatars.Count > 0) return;

		foreach(GameObject avatar in avatars)
		{
			Destroy(avatar);
		}
		
		avatars.Clear();

		unionAvatarTemple.SetActive(false);

		for(int i = 0; i < controllor.unionList.Count; i++)
		{
			int page = i / 8;

			int index = i % 8;

			int row = index / 4;

			int col = index % 4;

			GameObject avatarObject = (GameObject)Instantiate(unionAvatarTemple);

			avatarObject.SetActive(true);

			avatarObject.transform.parent = unionAvatarTemple.transform.parent;

			avatarObject.transform.localScale = unionAvatarTemple.transform.localScale;

			avatarObject.transform.localPosition = unionAvatarTemple.transform.localPosition
				+ new Vector3(col * 200 + page * 800, - row * 210, 0);

			UnionAvatar avatar = (UnionAvatar)avatarObject.GetComponent("UnionAvatar");

			avatar.refreshDate(controllor.unionList[i]);

			avatars.Add(avatarObject);
		}
	}

}
