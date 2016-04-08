using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RankItem : MonoBehaviour {

	public UILabel JunZhuName;
	public UILabel mRank;
	public UILabel mDamage;
	public DamageInfo mDamageInfo;

	public UISprite myself;

	public bool iSMyself = false;

	void Start () {
	
	}
	

	void Update () {
	
	}
	public void Init()
	{
		JunZhuName.text = mDamageInfo.junZhuName;

		mRank.text = mDamageInfo.rank.ToString();

		mDamage.text = mDamageInfo.damage.ToString();
		if(iSMyself)
		{
			myself.spriteName = "Cmplete";
		}
	}
}
