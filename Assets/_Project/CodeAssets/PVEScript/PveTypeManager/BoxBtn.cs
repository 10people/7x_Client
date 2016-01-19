using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BoxBtn : MonoBehaviour {
	public Level m_Lev;
	public int Star_Id;
	void Start () {
	
	}

	void Update () {
	
	}
	public void SendLingQu()
	{
		MemoryStream t_tream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		GetPveStarAward award = new GetPveStarAward();
		
		award.s_starNum = Star_Id;
		
		award.guanQiaId = m_Lev.guanQiaId;
		
		t_qx.Serialize(t_tream, award);
		
		byte[] t_protof;
		
		t_protof = t_tream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.PVE_STAR_REWARD_GET, ref t_protof);
		this.gameObject.SetActive (false);
	}
}
