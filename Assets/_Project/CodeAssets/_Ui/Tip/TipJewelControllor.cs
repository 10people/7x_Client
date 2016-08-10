using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TipJewelControllor : MonoBehaviour 
{
	public UISprite spriteFrame;
	
	public UISprite spriteIcon;
	
	public UILabel labelName;
	
	public UILabel labelNumNormal;

	public UILabel labelNumNormalText;

	public UISlider bar;
	
	public UILabel labelNumBar;
	
	public GameObject spriteTab;

	public GameObject layerBar;

	public UILabel labelAttr;

	public UILabel labelDesc;
	
	public UILabel labelFrom;


	private int commonItemId;
	
	private CommonItemTemplate template;
	
	private long num;
	
	private GameObject itemTemple;

	private BagItem bagItem;
	
	
	public void refreshData(int _commonItemId)
	{
		if(UICamera.lastTouchPosition.x < Screen.width / 2)
		{
			gameObject.transform.localPosition = new Vector3(240, 0, 0);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(-240, 0, 0);
		}
		
		commonItemId = _commonItemId;
		
		template = CommonItemTemplate.getCommonItemTemplateById (commonItemId);
		
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
		
		labelName.text = NameIdTemplate.GetName_By_NameId(template.nameId);
		
		refreshNum ();
		
		//0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；7基础属性符文；8高级属性符文；9强化材料;201联盟科技;202联盟科技;203联盟科技

		string textNum = num + "";

		if (num == 0)
		{
			textNum = ColorTool.Color_Red_FF0000 + num + "[-]";
		}
		else 
		{
			Color col = labelNumNormalText.color;

			string strCol = "["  + "]";

			textNum = strCol + num + "[-]";
		}

		string text_2 = LanguageTemplate.GetText(LanguageTemplate.Text.TIP_2);

		labelNumNormal.text = textNum;

		labelDesc.text = DescIdTemplate.GetDescriptionById (template.descId);
		
		labelFrom.text = DescIdTemplate.GetDescriptionById (template.dropDesc);

		if(template.itemType == 2)//装备
		{
			ZhuangBei zhuangbei = ZhuangBei.GetItemByID(commonItemId);

			string attrText = "";

			if(!zhuangbei.gongji.Equals("0"))
			{
				if(attrText.Length > 0)	attrText += "\n";

				attrText += NameIdTemplate.GetName_By_NameId(2101) + "：" + zhuangbei.gongji;
			}

			if(!zhuangbei.fangyu.Equals("0"))
			{
				if(attrText.Length > 0)	attrText += "\n";

				attrText += NameIdTemplate.GetName_By_NameId(2102) + "：" + zhuangbei.fangyu;
			}

			if(!zhuangbei.shengming.Equals("0"))
			{
				if(attrText.Length > 0)	attrText += "\n";
				
				attrText += NameIdTemplate.GetName_By_NameId(2103) + "：" + zhuangbei.shengming;
			}

			labelAttr.text = attrText;

			if(zhuangbei.lvlupExp > 0)
			{
				layerBar.SetActive(true);
				
				spriteTab.SetActive(false);
				
				int max = zhuangbei.lvlupExp;
				
				int exp = ShowTip.tipItemData == null ? 0 : ShowTip.tipItemData.exp;
				
				labelNumBar.text = exp + "/" + max;

				if(exp == 0) labelNumBar.text = ColorTool.Color_Red_FF0000 + exp + "[-]/" + max;

				bar.value = exp * 1.0f / max;
			}
			else
			{
				layerBar.SetActive(false);
				
				spriteTab.SetActive(true);
			}
		}
		else
		{
			FuWenTemplate fuwenTemp = FuWenTemplate.GetFuWenTemplateByFuWenId (commonItemId);

			labelAttr.text = NameIdTemplate.GetName_By_NameId (fuwenTemp.shuXingName) + "：" + fuwenTemp.shuxingValue;

			if(fuwenTemp.lvlupExp > 0)
			{
				layerBar.SetActive(true);

				spriteTab.SetActive(false);

				int max = fuwenTemp.lvlupExp;

				int exp = ShowTip.tipItemData == null ? 0 : ShowTip.tipItemData.exp;

				labelNumBar.text = exp + "/" + max;

				if(exp == 0) labelNumBar.text = ColorTool.Color_Red_FF0000 + exp + "[-]/" + max;

				bar.value = exp * 1.0f / max;
			}
			else
			{
				layerBar.SetActive(false);
				
				spriteTab.SetActive(true);
			}
		}
	}
	
	private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		itemTemple = (GameObject)p_object;
		
		itemTemple.SetActive (false);
		
		GameObject gc = (GameObject)Instantiate(itemTemple.gameObject);
		
		gc.SetActive(true);
		
		gc.transform.parent = transform;
		
		gc.transform.localPosition = spriteIcon.transform.parent.localPosition;
		
		gc.transform.eulerAngles = itemTemple.transform.eulerAngles;
		
		IconSampleManager ism = gc.GetComponent<IconSampleManager>();
		
		ism.SetIconByID(commonItemId, "", 500);
		
		gc.transform.localScale = new Vector3(.75f, .75f, 1);
	}
	
	private void refreshNum()
	{
		num = 0;
		
		//0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；7基础属性符文；8高级属性符文；9强化材料

		foreach(BagItem item in BagData.Instance().m_bagItemList)
		{
			if(item.itemId == template.id)
			{
				num += item.cnt;

				bagItem = item;
			}
		}
	}
}
