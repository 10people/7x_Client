using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class BuyTongBiUI : MonoBehaviour {

	public UILabel m_Baoji;
	public UILabel Huafei;
	public UILabel Huode;
	public BuyTongbiResp M_BuyTongBiInfo;
	public BuyTimesInfo M_BuyTimespInfo;
	public GameObject UIroot;
	public GameObject mEffect;
	public GameObject BaojiRoot;

	public void  Init()
	{
		//Debug.Log ("M_BuyTongBiInfo.baoji = "+M_BuyTongBiInfo.baoji);
		if(M_BuyTongBiInfo.baoji >= 2)
		{
			BaojiRoot.SetActive(true);
			m_Baoji.text = M_BuyTongBiInfo.baoji.ToString()+"倍暴击";
		}
		Huafei.text = "使用了"+M_BuyTimespInfo.tongBiHuaFei.ToString()+"元宝";
		Huode.text = "获得了"+(M_BuyTimespInfo.tongBiHuoDe * M_BuyTongBiInfo.baoji).ToString ()+"铜币";
		GameObject Effect = (GameObject)Instantiate (mEffect);
		Effect.transform.parent = mEffect.transform.parent;
		Effect.transform.localPosition = mEffect.transform.localPosition;
		Effect.transform.localScale = mEffect.transform.localScale;
		TongBiMoveEffect mTongBiMoveEffect = Effect.GetComponent<TongBiMoveEffect> ();
		mTongBiMoveEffect.Init ();
	}
	public void DestroyBtn()
	{
		Destroy (UIroot);

	}
}
