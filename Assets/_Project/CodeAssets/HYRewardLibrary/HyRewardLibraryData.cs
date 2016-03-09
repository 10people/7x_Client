using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HyRewardLibraryData : MonoBehaviour,SocketProcessor {

	public static HyRewardLibraryData hyRewardData;

	public AllianceHaveResp allianceResp;//联盟返回信息
	public ReqRewardStoreResp rewardResp;//荒野返回信息

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
		hyRewardData = this;
	}

	void Start ()
	{
		allianceResp = AllianceData.Instance.g_UnionInfo;

		HYRewardLibraryReq ();
	}

	//荒野奖励库请求
	public void HYRewardLibraryReq ()
	{
		ReqRewardStore rewardLibraryReq = new ReqRewardStore();
		
		rewardLibraryReq.lianmengId = allianceResp.id;
		
		MemoryStream t_stream = new MemoryStream();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,rewardLibraryReq);
		
		byte[] t_protof = t_stream.ToArray ();
		Debug.Log ("allianceResp.id:" + allianceResp.id);
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_REQ_REWARD_STORE,ref t_protof,"30408");
		Debug.Log ("RewardLibraryReq:" + ProtoIndexes.C_REQ_REWARD_STORE);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_REQ_REWARD_STORE://荒野奖励库返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ReqRewardStoreResp rewardLibraryResp = new ReqRewardStoreResp();
				
				t_qx.Deserialize(t_stream, rewardLibraryResp, rewardLibraryResp.GetType());
				
				if (rewardLibraryResp != null)
				{
					foreach (RewardItemInfo tempRewardItem in rewardLibraryResp.itemInfo)
					{
						if (tempRewardItem.applyerInfo == null)
						{
							tempRewardItem.applyerInfo = new List<ApplyerInfo>();
						}
					}

					rewardResp = rewardLibraryResp;
					Debug.Log ("荒野奖励库返回！");

//					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_REWARD_LIBRARY), RewardLibraryLoadBack);
				}
				
				return true;
			}
				
			default:return false;
			}
		}
		
		return false;
	}

	void OnClick ()
	{
//		allianceResp = AllianceData.Instance.g_UnionInfo;
//		
//		HYRewardLibraryReq ();
	}

	void RewardLibraryLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		GameObject hy = Instantiate( p_object ) as GameObject;
		
		hy.name = "HYRewardLibrary";
		
		hy.transform.localPosition = new Vector3 (0,1800,0);
		
		hy.transform.localScale = Vector3.one; 
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
