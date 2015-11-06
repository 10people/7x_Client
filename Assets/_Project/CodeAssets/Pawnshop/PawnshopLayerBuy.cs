using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class PawnshopLayerBuy : MonoBehaviour
{
	public PawnshopBuyItem ItemTemple;

    public UIScrollView ScrollView;

	public UILabel labelTime;


	private List<GoodsInfo> goods;

	private List<PawnshopBuyItem> m_buyGoods = new List<PawnshopBuyItem>();

	private int from;


	public void refreshData(List<GoodsInfo> t_goods, int _from)
	{
		from = _from;

		for(int i = 0; i < m_buyGoods.Count; i ++)
		{
			Destroy(m_buyGoods[i].gameObject);
		}

		m_buyGoods = new List<PawnshopBuyItem>();

		ItemTemple.gameObject.SetActive (false);

		goods = t_goods;

		if (goods == null) return;

		float width = 140f;

		float height = 165f;

		float left = ItemTemple.transform.localPosition.x;;

		float up = ItemTemple.transform.localPosition.y;

//		if(goods.Count >= 12)
//		{
//			left = ItemTemple.transform.localPosition.x;
//		}
//		else
//		{
//			int col = goods.Count / 2;
//
//			left = col * width / 2;
//		}

        //Clear exist itemTemple objects.
	    while (ScrollView.transform.childCount != 0)
	    {
	        var child = ScrollView.transform.GetChild(0);

			child.parent = null;

			Destroy(child.gameObject);
	    }

		for(int i = 0; i < goods.Count; i++)
		{
			int col = i / 2;

			int row = i % 2;

			GameObject itemObject = (GameObject)Instantiate(ItemTemple.gameObject);

			itemObject.SetActive(true);

			itemObject.transform.parent = ScrollView.transform;

			float x = left + width * col - ScrollView.transform.localPosition.x;

			itemObject.transform.localPosition = new Vector3(x, up - row * height, 0);

			itemObject.transform.localEulerAngles = Vector3.zero;

			itemObject.transform.localScale = new Vector3(1, 1, 1);

			PawnshopBuyItem item = (PawnshopBuyItem)itemObject.GetComponent("PawnshopBuyItem");

			if(from == 0)
			{
				DangpuItemCommonTemplate template = DangpuItemCommonTemplate.getDangpuItemCommonById(goods[i].itemId);
				
				item.RefreshData(goods[i], template, from);
			}
			else
			{
				DangPuTemplate template = DangPuTemplate.getDangpuTemplateById(goods[i].itemId);

				item.RefreshData(goods[i], template, from);
			}

			m_buyGoods.Add(item);
		}
	}

	public void refreshTime(float _lastTime)
	{
		if (labelTime == null) return;

		int lastTime = (int)_lastTime;

		int miao = lastTime % 60;
		
		int fen = (lastTime % 3600) / 60;
		
		int shi = lastTime / 3600;

		if(shi > 0)
		{
			labelTime.text = shi + LanguageTemplate.GetText(LanguageTemplate.Text.HOUR) + fen + LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE);
		}
		else if(fen > 0)
		{
			labelTime.text = fen + LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE) + miao + LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);
		}
		else
		{
			labelTime.text = miao + LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);
		}

//		labelTime.text = shi + LanguageTemplate.GetText(LanguageTemplate.Text.HOUR)
//			+ fen + LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE)
//			+ miao + LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);

	}

	public void minusGood(GoodsInfo tempGood)
	{
		GoodsInfo tempGoodInfo = null;

		foreach(GoodsInfo gi in goods)
		{
			if(gi.itemId == tempGood.itemId)
			{
			    gi.isSoldOut = true;

				tempGoodInfo = gi;

				//refreshData(goods, from);

				break;
			}
		}

		if (tempGoodInfo == null) return;

		foreach(PawnshopBuyItem item in m_buyGoods)
		{
			if(item.good.itemId == tempGood.itemId)
			{
				DangPuTemplate template = DangPuTemplate.getDangpuTemplateById(tempGoodInfo.itemId);

				item.RefreshData(tempGoodInfo, template, 1);
			}
		}
	}

}
