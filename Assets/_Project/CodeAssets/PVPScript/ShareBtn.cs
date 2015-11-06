using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShareBtn : MonoBehaviour,SocketListener {

	public GameObject SharetoWorld;//分享到世界频道
	public GameObject SharetoAliance;//分享到联盟频道
	public GameObject Worldsuccessed1;//分享到联盟频道成功
	public GameObject Aliancesuccessed2;//分享到联盟频道成功
	private int isShareTo;//用来区别是分享到联盟还是分享到世界 1为世界 2为联盟

	public UILabel WorldText;//按钮上的字
	public UILabel AlianceText;//按钮上的字
	[HideInInspector]public int ZhanDou_id;
	[HideInInspector]public string tEnemyName;
	void Awake()
	{ 
		SocketTool.RegisterSocketListener(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	public void SharetoWorldbtn()
	{
		//Debug.Log ("分享到世界频道");
		isShareTo = 2;
		ChatPct msharereq = new ChatPct ();
		MemoryStream msharereqStream = new MemoryStream ();
		QiXiongSerializer msharereer = new QiXiongSerializer ();

		msharereq.content = JunZhuData.Instance().m_junzhuInfo.name+" VS " + tEnemyName;
		msharereq.isLink = true;
		msharereq.channel = ChatPct.Channel.SHIJIE;
		msharereq.type = 1;
		msharereq.param = ZhanDou_id.ToString ();
		msharereer.Serialize (msharereqStream,msharereq);
		byte[] t_protof;
		t_protof = msharereqStream.ToArray();
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.C_Send_Chat, 
			ref t_protof,
			true,
			ProtoIndexes.S_Send_Chat );

	}
	public void SharetoAliancebtn()
	{
		//Debug.Log ("分享到联盟频道");
		isShareTo = 1;
		ChatPct msharereq = new ChatPct ();
		MemoryStream msharereqStream = new MemoryStream ();
		QiXiongSerializer msharereer = new QiXiongSerializer ();
		
		msharereq.content = JunZhuData.Instance().m_junzhuInfo.name+" VS " + tEnemyName;;
		msharereq.isLink = true;
		msharereq.channel = ChatPct.Channel.GUOJIA;
		msharereq.type = 1;
		msharereq.param = ZhanDou_id.ToString ();
		msharereer.Serialize (msharereqStream,msharereq);
		byte[] t_protof;
		t_protof = msharereqStream.ToArray();
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.C_Send_Chat, 
			ref t_protof,
			true,
			ProtoIndexes.S_Send_Chat );
	}
	public void Init()
	{
		isShareTo = 0;
		UISprite m_sprite = this.GetComponent<UISprite>();
		if(AllianceData.Instance.IsAllianceNotExist)
		{
			m_sprite.SetDimensions(344,90);
			SharetoWorld.SetActive(true);
			SharetoWorld.transform.localPosition = Vector3.zero;
			SharetoAliance.SetActive(false);
		}
		else{
			m_sprite.SetDimensions(344,180);
			SharetoWorld.SetActive(true);
			SharetoWorld.transform.localPosition = new Vector3(0,40,0);
			SharetoAliance.SetActive(true);
			SharetoAliance.transform.localPosition = new Vector3(0,-40,0);
		}
		Worldsuccessed1.SetActive (false);
		Worldsuccessed1.SetActive (false);
		WorldText.gameObject.SetActive (true);
		AlianceText.gameObject.SetActive (true);
	}

	public   bool OnSocketEvent(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				//战斗记录返回
			case ProtoIndexes.S_Send_Chat: 
			{
				Debug.Log("分享信息返回了");
					if(isShareTo == 2)//分享到世界
					{
						WorldText.gameObject.SetActive (false);
						Worldsuccessed1.SetActive (true);
					}
					if(isShareTo == 1)//分享lianmeng
					{
						AlianceText.gameObject.SetActive (false);
						Aliancesuccessed2.SetActive (true);
					}
					StartCoroutine(HideUi());

				return true;
			}
			default: return false;
			}
		}
		return false;
	}
	IEnumerator HideUi()
	{
		yield return new WaitForSeconds (1.0f);
		this.gameObject.SetActive (false);
	}
	public void DeletUI()
	{
		this.gameObject.SetActive (false);
	}
}
