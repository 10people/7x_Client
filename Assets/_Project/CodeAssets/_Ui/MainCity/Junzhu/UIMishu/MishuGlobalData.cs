using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;

public class MishuGlobalData : Singleton<MishuGlobalData> , SocketProcessor
{
	public MibaoInfoResp m_MibaoInfoResp;
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.NEW_MISHU_GLOBAL);
	}

	public void scend()
	{
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.NEW_MISHU_GLOBAL);
	}

	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				case ProtoIndexes.NEW_MISHU_GLOBAL://秘宝信息返回
				{
					MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					MibaoInfoResp info = new MibaoInfoResp();
					
					t_qx.Deserialize(t_tream, info, info.GetType());
					
					m_MibaoInfoResp = info;
					return true;
				}
			}
		}
		return false;
	}

	public int getMibaoSuipian(int id)
	{
		for(int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i ++)
		{
			if(m_MibaoInfoResp.miBaoList[i].tempId == id)
			{
				return m_MibaoInfoResp.miBaoList[i].suiPianNum;
			}
		}
		return 0;
	}

	void OnDestroy()
	{
		base.OnDestroy();
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
