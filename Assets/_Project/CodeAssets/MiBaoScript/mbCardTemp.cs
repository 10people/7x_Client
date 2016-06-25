using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class mbCardTemp : MonoBehaviour {
[HideInInspector]public MibaoInfo  mibaoTemp ;  

	public UISprite sp;
	public UILabel mbName;
	//public UISprite mbIcon;
	public UISprite mbpinzhi;

	public UITexture mTexture;

	public void init()
	{
		MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(mibaoTemp.miBaoId);
		mbName.text = NameIdTemplate.GetName_By_NameId (mMiBaoXmlTemp.nameId); 
		//mbIcon.spriteName = mMiBaoXmlTemp.icon.ToString();          //暂时未有资源。。。。。。。。。。。。。。。。。。。。。。。

		switch(mibaoTemp.star)
		{
		case 1:
			
			mbpinzhi.spriteName = "pinzhi3";
			
			break;
		case 2:
			
			mbpinzhi.spriteName = "pinzhi6";
			
			break;
		case 3:
			
			mbpinzhi.spriteName = "pinzhi9";
			
			break;
		case 4:
			
			mbpinzhi.spriteName = "pinzhi9";
			
			break;
		case 5:
			
			mbpinzhi.spriteName = "pinzhi9";
			
			break;
		default:
			break;
		}

		mTexture.mainTexture = (Texture)Resources.Load( Res2DTemplate.GetResPath( Res2DTemplate.Res.MIBAO_BIGICON )+mMiBaoXmlTemp.icon.ToString());

		showStar ();

		Invoke ("ShowEffect",1f);

	}
	void ShowEffect()
	{
		UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.PopUI_2,this.gameObject,EffectIdTemplate.GetPathByeffectId(100157));
		//UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,this.gameObject,EffectIdTemplate.GetPathByeffectId(100148));
	}
	void showStar()
	{
		for(int i = 0; i < mibaoTemp.star; i++)
		{
			GameObject spriteObject = (GameObject)Instantiate(sp.gameObject);
			
			spriteObject.SetActive(true);
			
			spriteObject.transform.parent = sp.gameObject.transform.parent;
			
			spriteObject.transform.localScale = sp.gameObject.transform.localScale;
			
			spriteObject.transform.localPosition = new Vector3(i * 38 - (mibaoTemp.star - 1) * 19, 0, 0);
			
		}
	}
	public void CloseFun()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);	
		Destroy (this.gameObject);
	}
}
