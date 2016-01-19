using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JiBaiAward : MonoBehaviour {
	public UISprite icon;

	public UISprite IsChoosed;

	public GameObject Box;

	public int Itemid;

	public Award mAwardinfo;
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void Init()
	{
		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardinfo.itemId);
		
		icon.spriteName = mItemTemp.icon.ToString();

		if (mAwardinfo.miBaoStar == 0) {
			Box.SetActive (false);
		} else
		{
			Box.SetActive (true);
		}
	}
	public void GetDataByChouJiang()
	{
		StartCoroutine (ShowEffect());
	}
	IEnumerator ShowEffect()
	{
		float time = 1f;
		IsChoosed.gameObject.SetActive (true);
		int i = 0;
		while(time>0)
		{
			time -= 0.25f;
			i ++;
			if(i%2 == 0)
			{
				IsChoosed.alpha = 1;
			}
			else
			{
				IsChoosed.alpha = 0;
			}
			yield return new WaitForSeconds(0.2f);
		}
		IsChoosed.alpha = 0;
		IsChoosed.gameObject.SetActive (false);
		Box.SetActive (true);
	}
	public void CHoseShowAnimation()
	{
		StartCoroutine (ShowAnimation());
	}
	IEnumerator ShowAnimation()
	{
		IsChoosed.gameObject.SetActive (true);
		yield return new WaitForSeconds(0.1f);
		IsChoosed.gameObject.SetActive (false);
	}
}
