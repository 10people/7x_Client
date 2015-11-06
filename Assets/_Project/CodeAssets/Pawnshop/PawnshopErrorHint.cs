using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PawnshopErrorHint : MonoBehaviour, SocketProcessor
{
	public PawnshopUIControllor controllor;

	public UILabel labelRMB;

	public UILabel labelCoin;

	public UILabel labelCount;


	void Awake()
	{
		SocketTool.RegisterMessageProcessor( this );
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public void sendReq()
	{
		labelRMB.text = "";
		
		labelCoin.text = "";
		
		labelCount.text = "";

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TIMES_REQ);
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.S_BUY_TIMES_INFO:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			BuyTimesInfo resp = new BuyTimesInfo();
			
			t_qx.Deserialize(t_stream, resp, resp.GetType());
			
			initData(resp);
			
			return true;
		}
		}
		
		return false;
	}

	private void initData(BuyTimesInfo resp)
	{
		labelRMB.text = resp.tiLiHuaFei + "";

		labelCoin.text = resp.tiLiHuoDe + "";

		labelCount.text = resp.tongBi + "";
	}

	public void buyCoin()
	{
		int needRmb = int.Parse(labelRMB.text);

		if(needRmb > JunZhuData.Instance().m_junzhuInfo.yuanBao)
		{
			controllor.showRmbErrorHint();

			return;
		}

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TongBi);
	}

	public void CloseLayer()
	{
		gameObject.SetActive (false);
	}

}
