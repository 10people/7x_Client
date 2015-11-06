using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BattleReplayorReader : MonoBehaviour, SocketProcessor
{
	private static BattleReplayorReader _instance;
	
	private List<ReplayNode> replayNodes = new List<ReplayNode>();


	public static BattleReplayorReader Instance() { return _instance; }

	void Awake()
	{
		_instance = this; 

		SocketTool.RegisterMessageProcessor( this );
	}

	void Start()
	{
//		SocketTool.Instance().Connect();
		
		BattleReplayReq req = new BattleReplayReq();
		
		req.battleId = 1;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_Request_battle_replay, ref t_protof);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor( this );
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch (p_message.m_protocol_index)
		{
		case ProtoIndexes.C_Report_battle_replay:
		{
			MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			BattleReplayData brd = new BattleReplayData();
			
			t_qx.Deserialize(t_stream, brd, brd.GetType());

			StartCoroutine(initData(brd));
			
			return true;
		}
		}
		
		return false;
	}

	private IEnumerator initData(BattleReplayData replayData)
	{
		for(int i = 0; i < replayData.enemys.Count; i++)
		{
			Troop troop = replayData.enemys[i];
			
			BattleReplayControlor.Instance().createTroopEnemy(troop, i);
		}
		
		for(int i = 0; i < replayData.selfs.Count; i++)
		{
			Troop troop = replayData.selfs[i];
			
			BattleReplayControlor.Instance().createTroopSelf(troop, i);
		}

		replayNodes.Clear();

		yield return new WaitForSeconds(0.1f);

		for(int i = 0; i < replayData.nodes.Count; i++)
		{
			BattleNodeData nodeData = replayData.nodes[i];
			
			BattleReplayControlor.Instance().createNode(nodeData);
		}
	}

}
