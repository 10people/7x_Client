using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanGoodsInfo : MonoBehaviour {

	private DuiHuanInfo duiHuanInfo;

	public UISprite icon;
	public UISprite suiPianIcon;
	
	public UILabel weiWangLabel;
	
	public UILabel numLabel;
	
	public GameObject saleEnd;
	
	private int needWeiWang;//需要威望
	
	private string itemName;//物品名字
	
	public void InItGoodsInfo (DuiHuanInfo tempInfo)
	{
		duiHuanInfo = tempInfo;

		DuiHuanTemplete duiHuanTemp = DuiHuanTemplete.getDuiHuanTemplateById (tempInfo.id);
		
		int tempType = duiHuanTemp.itemType;//物品类型
		
		int itemId = duiHuanTemp.itemId;//物品id
		
		if (tempType == 3 || tempType == 6 || tempType == 9)//当铺材料,装备进阶材料,强化材料 ItemTemp表
		{
			ItemTemp item = ItemTemp.getItemTempById (itemId);
			
			itemName = NameIdTemplate.GetName_By_NameId (item.itemName);

			icon.gameObject.SetActive (true);
			suiPianIcon.gameObject.SetActive (false);

			icon.spriteName = itemId.ToString ();
		}
		
		else if (tempType == 5)//秘宝碎片 MibaoSuiPian表
		{
			Debug.Log ("name:::" + itemId);
			MiBaoSuipianXMltemp miBaoSuiPian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById (itemId);

			itemName = NameIdTemplate.GetName_By_NameId (miBaoSuiPian.m_name);

			icon.gameObject.SetActive (false);
			suiPianIcon.gameObject.SetActive (true);
			
			suiPianIcon.spriteName = itemId.ToString ();
		}
		
		needWeiWang = duiHuanTemp.needNum;
		
		numLabel.text = "x" + duiHuanTemp.itemNum.ToString ();
		
		weiWangLabel.text = duiHuanTemp.needNum.ToString ();
		
		if (tempInfo.isChange)
		{
			saleEnd.SetActive (false);
			this.gameObject.GetComponent<BoxCollider>().enabled = true;
		}
		
		else
		{
			saleEnd.SetActive (true);
			this.gameObject.GetComponent<BoxCollider>().enabled = false;
		}
	}

	void OnClick ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        DuiHuanCallback );
	}
	
	public void DuiHuanCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR2);
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_TITLE);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string str = str1 + needWeiWang + str2 + itemName + "？";
		uibox.setBox(titleStr, null, MyColorData.getColorString (1,str), 
		             null, cancelStr, confirmStr,TipWindowBtn);
	}
	
	void TipWindowBtn (int i)
	{
		if (i == 2)
		{
			Debug.Log ("确定兑换");

			ConfirmManager.confirm.ConfirmReq (2,duiHuanInfo,0);
		}
	}
}
