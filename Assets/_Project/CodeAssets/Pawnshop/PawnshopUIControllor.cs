using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PawnshopUIControllor : MonoBehaviour, SocketProcessor
{
    public ScaleEffectController m_ScaleEffectController;

	public PawnshopButtonChoose btnNormal;

	public PawnshopButtonChoose btnSecret;

	public PawnshopButtonChoose btnSell;

	public PawnshopLayerBuy layerNormal;

	public PawnshopLayerBuy layerSecret;

	public PawnshopLayerSell layerSell;

	public PawnshopBuyHint layerBuyHint;

	public PawnshopRefreshHint layerRefreshHint;

	public PawnshopErrorHint layerDkpErrorHint;

	public PawnshopErrorHint layerRmbErrorHint;

	public PawnshopErrorHint layerCoinErroHint;

	public UILabel labelText;

	public UILabel labelCoin;

	public UILabel labelRMB;

	public UILabel labelDKP;

    public EventHandler m_EventTouch;


	[HideInInspector] public GoodsInfo buyingItem = null;


	private List<GoodsInfo> normalInfo = new List<GoodsInfo>();

	private List<GoodsInfo> secretInfo = new List<GoodsInfo> ();

	private float lastTime;

	private int tempFrom;

	private int coin;

	private int RMB;

	private int DKP;

	private bool first;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	void Start ()
	{
		if (FreshGuide.Instance().IsActive(100290) && TaskData.Instance.m_TaskInfoDic[100290].progress >= 0)
		{
			CityGlobalData.m_isRightGuide = false;

			TaskData.Instance.m_iCurMissionIndex = 100290;

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

			tempTaskData.m_iCurIndex = 3;

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
		}
		else
		{
			CityGlobalData.m_isRightGuide = true;
		}

		lastTime = 0;

		coin = JunZhuData.Instance().m_junzhuInfo.jinBi;

		RMB = JunZhuData.Instance().m_junzhuInfo.yuanBao;

		DKP = AllianceData.Instance.g_UnionInfo == null ? 0 :
			AllianceData.Instance.g_UnionInfo.contribution;

		labelCoin.text = coin + "";

		labelRMB.text = RMB + "";

		labelDKP.text = DKP + "";

        m_EventTouch.m_click_handler += TopUp;

		first = true;

		closeAll ();

		sendGetData ();
	}

    void TopUp(GameObject obj)
    {
        EquipSuoData.TopUpLayerTip();
    }

	private void sendGetData()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PAWN_SHOP_GOODS_LIST_REQ);
	}

	public void sendRefreshData()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PAWN_SHOP_GOODS_REFRESH);
	}

	public bool sendSellItem()
	{
		PawnShopGoodsSell req = new PawnShopGoodsSell();
		
		req.sellGinfo = new List<SellGoodsInfo> ();

		bool temp = false;

		for(int i = 0; i < layerSell.sellBagId.Count; i++)
		{
			long id = layerSell.sellBagId[i];

			int num = layerSell.sellNum[i];

			if(num != 0)
			{
				temp = true;
			}

			SellGoodsInfo info = new SellGoodsInfo();

			info.bagId = id;

			info.count = num;

			req.sellGinfo.Add(info);

			layerSell.sellNum[i] = 0;

			continue;
		}

		if(!temp)
		{
			return false;
		}

		if (req.sellGinfo.Count == 0) return false;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PAWN_SHOP_GOODS_SELL, ref t_protof);

		return true;
	}

	public void sendBuyItem(GoodsInfo itemData, int from)
	{
		buyingItem = itemData;

		PawnshopGoodsBuy req = new PawnshopGoodsBuy();
		
		req.itemId = itemData.itemId;

		req.type = from;

		tempFrom = from;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PAWN_SHOP_GOODS_BUY, ref t_protof);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;

		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.PAWN_SHOP_GOODS_LIST:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			PawnshopGoodsList resp = new PawnshopGoodsList();

			t_qx.Deserialize(t_stream, resp, resp.GetType());

			initData(resp);

			return true;
		}
		case ProtoIndexes.PAWN_SHOP_GOODS_BUY_OK:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			PawnshopGoodsBuyResp resp = new PawnshopGoodsBuyResp();
			
			t_qx.Deserialize(t_stream, resp, resp.GetType());

			layerBuyHint.gameObject.SetActive(false);

			if(tempFrom == 1)//神秘货物
			{
				layerSecret.minusGood(buyingItem);

				labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)232);
			}
			else
			{
				labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)230);
			}

			buyingItem = null;
			
			if(AllianceData.Instance.g_UnionInfo != null)
			{
				AllianceData.Instance.g_UnionInfo.contribution = resp.contribution;
			}

			return true;
		}

		case ProtoIndexes.PAWN_SHOP_GOODS_SELL_OK:
		{
			showLayerSell();

			labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)227);

			return true;
		}
		case ProtoIndexes.PAWN_SHOP_GOODS_REFRESH_RESP:
		{
			MemoryStream t_stream1 = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx1 = new QiXiongSerializer();
			
			PawnshopRefeshResp resp1 = new PawnshopRefeshResp();
			
			t_qx1.Deserialize(t_stream1, resp1, resp1.GetType());

			switch(resp1.result)
			{
			case 1:
				Global.CreateBox("刷新货物",
				                 "vip等级不足",
				                 null, null, "确定", null, null);
				return true;
			case 2:
                            EquipSuoData.TopUpLayerTip();
				return true;
			case 3:
				int vipLevel = JunZhuData.Instance().m_junzhuInfo.vipLv;

				if(vipLevel >= 15)
				{
					Global.CreateBox("刷新货物",
					                 "您今日的刷新次数已用尽···",
					                 null, null, "确定", null, null);
				}
				else
				{
					Global.CreateBox("刷新货物",
					                 "您今日的刷新次数已用尽··· 提升到VIP等级可以刷新更多次",
					                 null, null, "确定", null, null);
				}
				return true;
			}

			break;
		}
		}
		
		return false;
	}

	private void initData(PawnshopGoodsList resp)
	{
		secretInfo = resp.secretInfo;

		normalInfo = resp.normalInfo;

		lastTime = resp.refreshTime;

		layerRefreshHint.refreshCost = resp.refreshCost;

		layerRefreshHint.gameObject.SetActive (false);

		if(first == true)
		{
			showLayerNormal ();

			first = false;
		}
		else
		{
			showLayerSecret();
		}

		layerSecret.refreshTime(lastTime);

		isUpdata = true;
	}

	bool isUpdata = false;

	bool isF = true;

	void FixedUpdate()
	{
		if(coin != JunZhuData.Instance().m_junzhuInfo.jinBi)
		{
			coin = JunZhuData.Instance().m_junzhuInfo.jinBi;

			labelCoin.text = coin + "";
		}

		if(RMB != JunZhuData.Instance().m_junzhuInfo.yuanBao)
		{
			RMB = JunZhuData.Instance().m_junzhuInfo.yuanBao;

			labelRMB.text = RMB + "";
		}

		if(AllianceData.Instance.g_UnionInfo != null && DKP != AllianceData.Instance.g_UnionInfo.contribution)
		{
			DKP = AllianceData.Instance.g_UnionInfo.contribution;

			labelDKP.text = DKP + "";
		}

		if (lastTime <= 0 && isUpdata)
		{
			sendGetData ();

			isUpdata = false;

			return;
		}
		else if(lastTime <= 0)
		{
			return;
		}
//		else if(isF && lastTime > 0)
//		{
//			isF = false;
//			lastTime = 0;
//		}

		lastTime -= Time.deltaTime;

		layerSecret.refreshTime(lastTime);
	}

	private void closeAll()
	{
		layerNormal.gameObject.SetActive (false);

		layerSecret.gameObject.SetActive (false);

		layerSell.gameObject.SetActive (false);

		layerBuyHint.gameObject.SetActive (false);

		layerRefreshHint.gameObject.SetActive (false);

		layerDkpErrorHint.gameObject.SetActive (false);

		layerRmbErrorHint.gameObject.SetActive (false);

		layerCoinErroHint.gameObject.SetActive (false);

		btnNormal.pawnshopNormal ();

		btnSecret.pawnshopNormal ();

		btnSell.pawnshopNormal ();
	}

	public void showLayerNormal()
	{
		closeAll();

		btnNormal.pawnshopFocus();

		layerNormal.gameObject.SetActive(true);

		layerNormal.refreshData(normalInfo, 0);

		labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)229);
	}

	public void showLayerSecret()
	{
		closeAll();
		
		btnSecret.pawnshopFocus();

		layerSecret.gameObject.SetActive (true);

		layerSecret.refreshData(secretInfo, 1);

		labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)231);

		if (FreshGuide.Instance().IsActive(100290) && TaskData.Instance.m_TaskInfoDic[100290].progress >= 0)
		{
			CityGlobalData.m_isRightGuide = false;
			
			TaskData.Instance.m_iCurMissionIndex = 100290;
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
			
			tempTaskData.m_iCurIndex = 4;
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
		}
	}

	public void showLayerSell()
	{
		closeAll ();

		btnSell.pawnshopFocus ();

		layerSell.gameObject.SetActive (true);

		layerSell.refreshData ();

		labelText.text = LanguageTemplate.GetText((LanguageTemplate.Text)226);
	}

	public void showbuyHint(GoodsInfo item, int from)
	{
		layerBuyHint.RefreshData (item, from);
		
		layerBuyHint.gameObject.SetActive (true);
	}

	public void showRefreshHint()
	{
		layerRefreshHint.gameObject.SetActive (true);

		layerRefreshHint.refreshData ();
	}

	public void showDkpErrorHint()
	{
		layerDkpErrorHint.gameObject.SetActive (true);
	}

	public void showRmbErrorHint()
	{
		layerRmbErrorHint.gameObject.SetActive (true);
	}

	public void showCoinErrorHint()
	{
		layerCoinErroHint.gameObject.SetActive (true);

		layerCoinErroHint.sendReq ();
	}

	public void closePawnshop()
	{
	    m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
        UIYindao.m_UIYindao.CloseUI();

        MainCityUI.TryRemoveFromObjectList(gameObject);

        CityGlobalData.m_isRightGuide = false;

        Destroy(gameObject);
    }
}
