using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TipItemControllor : MonoBehaviour 
{
	public UISprite spriteFrame;

	public UISprite spriteIcon;
	
	public UISprite spriteQuality;
	
	public UILabel labelName;
	
	public GameObject layerNum;
	
	public GameObject layerProgress;
	
	public UILabel labelNumNormal;
	
	public UISlider bar;
	
	public UILabel labelNumBar;
	
	public UISprite spriteTab;
	
	public UILabel labelDesc;
	
	public UILabel labelFrom;

	public GameObject layerDesc_2;

	public UIAtlas atlasEquip;

	public UIAtlas atlasMibao;

	public UIAtlas atlasFuwen;

	
	private int commonItemId;
	
	private CommonItemTemplate template;
	
	private int num;
	
	private BagItem bagItem;

	private GameObject itemTemple;

	
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

		if(template.itemType == 4)//秘宝
		{
			spriteIcon.atlas = atlasMibao;
		}
		else if(template.itemType == 7 || template.itemType == 8)//符文
		{
			spriteIcon.atlas = atlasFuwen;
		}
		else
		{
			spriteIcon.atlas = atlasEquip;
		}

		spriteIcon.spriteName = template.icon + "";

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);

		spriteQuality.spriteName = "pinzhi" + template.color;
		
		labelName.text = NameIdTemplate.GetName_By_NameId(template.nameId);

		if(template.id / 10000 == 94)//物品是古卷残简
		{
			layerDesc_2.SetActive (true);

			spriteFrame.SetDimensions(450, 512);
		}
		else
		{
			layerDesc_2.SetActive (false);

			spriteFrame.SetDimensions(450, 405);
		}

		layerNum.SetActive (false);
		
		layerProgress.SetActive (false);

		refreshNum ();

		bool showNormalNum = true;
		
		//0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；7基础属性符文；8高级属性符文；9强化材料
		if(template.itemType == 5)
		{
			showNormalNum = !refreshProgress_MibaoSuiPian();
		}
		else if(template.itemType == 6)
		{
			showNormalNum = !refreshProgress_JinJieCaiLiao();
		}
		else if(template.itemType == 211)
		{
			showNormalNum = false;
		}
		
		if(showNormalNum == true)
		{
			layerNum.SetActive(true);
			
			layerProgress.SetActive (false);
			
			string color = num == 0 ? "[FF0000]" : "[029528]";

			string textNum = num + "";

			string text_2 = LanguageTemplate.GetText(LanguageTemplate.Text.TIP_2);

			if(commonItemId == 900003)//体力
			{
				textNum = num + "/" + JunZhuData.Instance().m_junzhuInfo.tiLiMax;

				text_2 = LanguageTemplate.GetText(LanguageTemplate.Text.TIP_3);
			}

			string text = LanguageTemplate.GetText(LanguageTemplate.Text.TIP_1) 
				+ color + textNum + "[-]";

			labelNumNormal.text = text;
		}
		
		labelDesc.text = DescIdTemplate.GetDescriptionById (template.descId);

		if(commonItemId == 900003)//体力
		{
			if(JunZhuData.Instance().m_junzhuInfo.tili < JunZhuData.Instance().m_junzhuInfo.tiLiMax)
			{
				int miao = JunZhuData.Instance().m_remainTime % 60;
				
				int fen = JunZhuData.Instance().m_remainTime / 60;
				
				labelFrom.text = fen + ":" + miao + DescIdTemplate.GetDescriptionById(840029)  + DescIdTemplate.GetDescriptionById (template.dropDesc);
			}
			else
			{
				labelFrom.text = DescIdTemplate.GetDescriptionById(840030)  + DescIdTemplate.GetDescriptionById (template.dropDesc);
			}
		}
		else
		{
			labelFrom.text = DescIdTemplate.GetDescriptionById (template.dropDesc);
		}
		
		if(template.id / 10000 == 94 && template.id % 10 == 0)//物品是古卷
		{
			layerNum.SetActive(false);
		}

		if(template.id >= 902001 && template.id <= 902005)//运镖的马匹
		{
			string desc = DescIdTemplate.GetDescriptionById (template.descId);

			int horseType = template.id % 10;

			int awardNum = GetHorseAwardNum(horseType) - GetHorseAwardNum(1);

			desc = desc.Replace("$", awardNum + "");

			labelDesc.text = desc;

			layerNum.SetActive(false);
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
		
		gc.transform.localScale = itemTemple.transform.localScale;
		
		IconSampleManager ism = gc.GetComponent<IconSampleManager>();

		ism.SetIconByID(commonItemId, "", 500);
	}

	private void refreshNum()
	{
		num = 0;
		
		bagItem = null;

		bool gotoBag = true;

		if(template.id == 900001)// 铜币
		{
			num = JunZhuData.Instance().m_junzhuInfo.jinBi;
		}
		else if(template.id == 900002) // 元宝
		{
			num = JunZhuData.Instance().m_junzhuInfo.yuanBao;
		}
		else if(template.id == 900003)//体力
		{
			num = JunZhuData.Instance().m_junzhuInfo.tili;
		}
		else if(template.id == 900015)//联盟贡献
		{
			num = AllianceData.Instance.g_UnionInfo == null ? 0 :
				AllianceData.Instance.g_UnionInfo.contribution;
		}
		else if(template.id == 900026)//荒野币
		{
			num = AllianceData.Instance.Hy_Bi;
		}

		//0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；7基础属性符文；8高级属性符文；9强化材料

		else if(template.itemType == 2 || template.itemType == 6)//装备数量在君主装备上
		{
			foreach(BagItem item in EquipsOfBody.Instance().m_equipsOfBodyDic.Values)
			{
				if(item.itemId == template.id)
				{
					num = item.cnt;
				}
				
				if(item.itemId == template.synItemID)
				{
					bagItem = item;
				}
			}
		}
		else if(template.itemType == 4)//秘宝数量在秘宝列表中
		{
			foreach(MibaoInfo mibao in MiBaoGlobleData.Instance().G_MiBaoInfo.miBaoList)
			{
				if(mibao.miBaoId == template.id)
				{
					if(mibao.level != 0)
					{
						num = 1;
					}
				}
			}

			gotoBag = false;
		}
		else if(template.itemType == 5)//秘宝碎片数量在秘宝列表中
		{
			foreach(MibaoInfo mibao in MiBaoGlobleData.Instance().G_MiBaoInfo.miBaoList)
			{
				if(mibao.miBaoId == template.synItemID)
				{
					num = mibao.suiPianNum;
				}
			}
		}

		if(gotoBag == true)
		{
			foreach(BagItem item in BagData.Instance().m_bagItemList)
			{
				if(item.itemId == template.id)
				{
					num += item.cnt;
				}
				
				if(item.itemId == template.synItemID)
				{
					bagItem = item;
				}
			}
		}
	}

	//秘宝碎片
	private bool refreshProgress_MibaoSuiPian()
	{
		//if (bagItem != null) return false;

		layerNum.SetActive(false);
		
		layerProgress.SetActive (true);

		bool isLock = false;

		bool isHave = false;

		int numMax = 0;
		
		foreach(MibaoInfo mibao in MiBaoGlobleData.Instance().G_MiBaoInfo.miBaoList)
		{
			MiBaoXmlTemp mibaoTemplate = MiBaoXmlTemp.getMiBaoXmlTempById(mibao.miBaoId);

			if(mibaoTemplate.suipianId == template.id)
			{
				if(mibao.level == 0)
				{
					isHave = false;

					MiBaoSuipianXMltemp suipianTemplate = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempById(mibaoTemplate.suipianId);
					
//					isLock = mibao.isLock;
					
					numMax = suipianTemplate.hechengNum;
				}
				else
				{
					isHave = true;

					numMax = mibao.needSuipianNum;
				}

			}
		}

		bar.value = (num * 1f) / (numMax * 1f);
		
		bar.value = bar.value > 1 ? 1 : bar.value;
		
		labelNumBar.text = num + "/" + numMax;
		
		if(isLock == true)
		{
			spriteTab.gameObject.SetActive(true);
			
			spriteTab.spriteName = "tips_4";
			
			return true;
		}
		
		if(num < numMax)
		{
			spriteTab.gameObject.SetActive(false);
		}
		else
		{
			spriteTab.gameObject.SetActive(true);
			
			if(isHave == false)//没有秘宝，显示"可合成"
			{
				spriteTab.spriteName = "tips_1";
			}
			else //有秘宝，显示"可升星"
			{
				spriteTab.spriteName = "tips_3";
			}
		}
		
		return true;
	}
	
	//进阶材料
	private bool refreshProgress_JinJieCaiLiao()
	{
		if (bagItem == null) return false;

		layerNum.SetActive(false);
		
		layerProgress.SetActive (true);

		ZhuangBei templ = ZhuangBei.getZhuangBeiById (template.synItemID);
		
		int numMax = int.Parse(templ.jinjieNum);
		
		bar.value = (num * 1f) / (numMax * 1f);
		
		bar.value = bar.value > 1 ? 1 : bar.value;
		
		labelNumBar.text = num + "/" + numMax;
		
		if(num < numMax)
		{
			spriteTab.gameObject.SetActive(false);
		}
		else 
		{
			spriteTab.gameObject.SetActive(true);
			
			spriteTab.spriteName = "tips_2";
		}
		
		return true;
	}

	private int GetHorseAwardNum (int horseType)
	{
		JunzhuShengjiTemplate junZhuUpLevelTemp = JunzhuShengjiTemplate.GetJunZhuShengJi (JunZhuData.Instance().m_junzhuInfo.level);

		int xiShu = junZhuUpLevelTemp.xishu;
		
		CartTemplate cartemp = CartTemplate.GetCartTemplateByType (horseType);

		float award = cartemp.ProfitPara * xiShu;
		
		return (int)award;
	}

	void Update()
	{
		if(commonItemId == 900003)//体力
		{
			if(JunZhuData.Instance().m_junzhuInfo.tili < JunZhuData.Instance().m_junzhuInfo.tiLiMax)
			{
				int miao = JunZhuData.Instance().m_remainTime % 60;
				
				int fen = JunZhuData.Instance().m_remainTime / 60;
				
				labelFrom.text = fen + ":" + miao + DescIdTemplate.GetDescriptionById(840029)  + DescIdTemplate.GetDescriptionById (template.dropDesc);
			}
			else
			{
				labelFrom.text = DescIdTemplate.GetDescriptionById(840030)  + DescIdTemplate.GetDescriptionById (template.dropDesc);
			}
		}
	}


}
