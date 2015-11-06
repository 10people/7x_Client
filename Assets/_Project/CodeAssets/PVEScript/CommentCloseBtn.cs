using UnityEngine;
using System.Collections;

public class CommentCloseBtn : MonoBehaviour {

	//public GameObject supObj;
	public UILabel mVipLv;
	public void init()
	{
		int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(13);
		mVipLv.text = viplv.ToString();
	}

	public void destroyUI()
	{
		Destroy (this.gameObject);
	}
}
