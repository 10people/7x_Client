using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ExchangeManager : MonoBehaviour {

	public ExchageAwardResp exchangeResp;

	public UILabel weiWangLabel;

	public UILabel refreshTimeLabel;

	public UIGrid rewardGrid;

	public GameObject rewardObj;
	private List<GameObject> rewardObjList = new List<GameObject>();

	public GameObject chatObj;

	//初始化兑换页面
	public void InItExchangePage ()
	{
		weiWangLabel.text = exchangeResp.weiWang.ToString ();

		for (int i = 0;i < exchangeResp.duiHuanList.Count - 1;i ++)
		{
			for (int j = 0;j < exchangeResp.duiHuanList.Count - i - 1;j ++)
			{
				if (exchangeResp.duiHuanList[j].site > exchangeResp.duiHuanList[j + 1].site)
				{
					DuiHuanInfo temp = exchangeResp.duiHuanList[j];
					
					exchangeResp.duiHuanList[j] = exchangeResp.duiHuanList[j + 1];
					
					exchangeResp.duiHuanList[j + 1] = temp;
				}
			}
		}

		foreach (GameObject tempObj in rewardObjList)
		{
			Destroy (tempObj);
		}
		rewardObjList.Clear ();

		for (int i = 0;i < exchangeResp.duiHuanList.Count;i ++)
		{
			GameObject reward = (GameObject)Instantiate (rewardObj);

			reward.SetActive (true);
			reward.name = "Reward" + (i + 1);

			reward.transform.parent = rewardGrid.transform;

			reward.transform.localPosition = Vector3.zero;

			reward.transform.localScale = rewardObj.transform.localScale;

			rewardObjList.Add (reward);

			BaiZhanGoodsInfo goodsInfo = reward.GetComponent<BaiZhanGoodsInfo> ();
			goodsInfo.InItGoodsInfo (exchangeResp.duiHuanList[i]);
		}

		rewardGrid.repositionNow = true;
	}

	void Update ()
	{
		refreshTimeLabel.text = BaiZhanExchange.exchange.hour + ":" + BaiZhanExchange.exchange.minute 
			+ ":" + BaiZhanExchange.exchange.second;
	}

	//刷新按钮
	public void RefreshBtn ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        RefreshLoadCallback );
	}

	public void RefreshLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string askStr1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_REFRESH_AWARD_ASKSTR1);
		string weiWangStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_WEIWANG);
		string askStr2 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_REFRESH_AWARD_ASKSTR2);
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_REFRESH_AWARD_TITLE);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string str1 = askStr1 + exchangeResp.needYB + weiWangStr;
		string str2 = askStr2;
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1) ,MyColorData.getColorString (1,str2), null, cancelStr, confirmStr,ClickBtn);
	}

	void ClickBtn (int i)
	{
		if (i == 2)
		{
			Debug.Log ("确定刷新物品");
			BaiZhanExchange.exchange.DuiHuanReq (1);
		}
	}

	//刷新某个物品的兑换状态,修改威望
	public void GoodsRefresh (DuiHuanInfo tempInfo)
	{
		for (int i = 0;i < rewardObjList.Count;i ++)
		{
			if (exchangeResp.duiHuanList[i].id == tempInfo.id)
			{
				exchangeResp.duiHuanList[i].isChange = false;
				BaiZhanGoodsInfo goodsInfo = rewardObjList[i].GetComponent<BaiZhanGoodsInfo> ();
				goodsInfo.InItGoodsInfo (exchangeResp.duiHuanList[i]);

				DuiHuanTemplete duiHuanTemp = DuiHuanTemplete.getDuiHuanTemplateById (tempInfo.id);
				exchangeResp.weiWang -= duiHuanTemp.needNum;
				weiWangLabel.text = exchangeResp.weiWang.ToString ();
			}
		}

		BaiZhanExchange.exchange.exchangeInfo = exchangeResp;
	}

	public void SendChatMsg ()
	{
		chatObj.SetActive (true);

		ExchangeChat chat = chatObj.GetComponent<ExchangeChat> ();
		chat.ShowChat (1);
	}

	public void BackBtn ()
	{
		BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (true);
		Destroy (this.gameObject);
	}

	public void CloseBtn ()
	{
		BaiZhanData.Instance ().CloseBaiZhan ();
	}
}
