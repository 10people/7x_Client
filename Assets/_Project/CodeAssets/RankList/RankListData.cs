using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class RankListData : MonoBehaviour,SocketProcessor {

	private static RankListData rankData;

	public static RankListData Instance()
	{
		if (!rankData)
		{
			rankData = (RankListData)GameObject.FindObjectOfType (typeof(RankListData));
		}
		
		return rankData;
	}

	public GameObject rankListObj;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		RankListReq ();
	}

	//请求排行榜
	public void RankListReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.RANKING_REQ,"30431");
//		Debug.Log ("RankListReq:" + ProtoIndexes.RANKING_REP);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.RANKING_RESP:

//				Debug.Log ("RankListResp:" + ProtoIndexes.RANKING_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				RankingResp rankingResp = new RankingResp();
				
				t_qx.Deserialize(t_stream, rankingResp, rankingResp.GetType());

				if (rankingResp != null)
				{
//					Debug.Log ("RankListBack!");

					if (rankingResp.junList == null)
					{
						rankingResp.junList = new List<JunZhuInfo>();
					}

					if (rankingResp.mengList == null)
					{
						rankingResp.mengList = new List<LianMengInfo>();
					}

					RankListManager rankListManObj = rankListObj.GetComponent<RankListManager> ();
					rankListManObj.GetRankResp (rankingResp);
				}

				return true;

			default:return false;
			}
		}

		return false;
	}

	public void DestroyRoot ()
	{
		Destroy (this.gameObject);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
