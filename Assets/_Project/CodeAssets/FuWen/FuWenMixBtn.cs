using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenMixBtn : MonoBehaviour {

	public UISprite fuWenIcon;
	public UISprite addIcon;

	private int itemId;

	public enum FxType
	{
		OPEN,
		CLEAR,
	}

	public void GetFuWenItemId (int tempItemId,bool isShow)
	{
		addIcon.gameObject.SetActive (!isShow);

		itemId = tempItemId;

		ClearFx ();

		if (isShow)
		{
			fuWenIcon.spriteName = itemId.ToString ();
		}
		else
		{
			fuWenIcon.spriteName = "";
		}
	}

	public void OpenFx ()
	{
		FuWenTemplate fuWenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (itemId);
		int shuXingId = fuWenTemp.shuxing;//1-攻击(红色) 2-防御(橙色) 3-生命(绿色) 其它(蓝色)
		int effectId = 0;
		if (shuXingId == 1)
		{
			effectId = 100164;
		}
		else if (shuXingId == 2)
		{
			effectId = 100161;
		}
		else if (shuXingId == 3)
		{
			effectId = 100162;
		}
		else
		{
			effectId = 100163;
		}
		
		UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,fuWenIcon.gameObject,EffectIdTemplate.GetPathByeffectId(effectId));

		FuWenMainPage.fuWenMainPage.ShowEffectCount (1);
	}

	public void ClearFx ()
	{
		UI3DEffectTool.Instance ().ClearUIFx (fuWenIcon.gameObject);
	}

	void OnClick ()
	{
		if (!FuWenMainPage.fuWenMainPage.IsBtnClick)
		{
			FuWenMainPage.fuWenMainPage.IsBtnClick = true;
			
			FuWenMainPage.fuWenMainPage.FxController (FxType.CLEAR);
			FuWenMainPage.fuWenMainPage.SelectFuWen (FuWenSelect.SelectType.HECHENG,0,FuWenMainPage.FuShiType.OTHER,null);
		}
	}
}
