using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EventManager : MonoBehaviour  ,SocketProcessor {

	public EventListResp m_EventListResp;

	public GameObject Eventitem;

	public List<Eventitem> m_EventitemList = new List<Eventitem>();

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}

	public void Init()
	{

		SocketTool.Instance().SendSocketMessage (ProtoIndexes.ALLINACE_EVENT_REQ);
	}
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ALLINACE_EVENT_RESP://联盟事件请求返回
			{
			//	Debug.Log ("ApplicateResp" + ProtoIndexes.ALLINACE_EVENT_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				EventListResp _EventListResp = new EventListResp();
				
				application_qx.Deserialize(application_stream, _EventListResp, _EventListResp.GetType());

				m_EventListResp = _EventListResp;

				SortEvents();

				return true;
			}
				
			default:return false;
			}
		}
		
		return false;
	}
	private float Dis = 67;
	void SortEvents()
	{
		//Debug.Log ("m_EventListResp.msg.Count = " +m_EventListResp.msg.Count);
		if(m_EventListResp.msg == null)
		{
			return;
		}
		foreach(Eventitem mEventitem in m_EventitemList)
		{
			Destroy(mEventitem.gameObject);
		}
		m_EventitemList.Clear ();

		for(int i = 0 ; i < m_EventListResp.msg.Count; i ++)
		{
			GameObject m_Eventitem = Instantiate(Eventitem) as GameObject;
			
			m_Eventitem.SetActive(true);
			
			m_Eventitem.transform.parent = Eventitem.transform.parent;
			
			m_Eventitem.transform.localPosition = new Vector3(2,133-i*Dis,0);
			
			m_Eventitem.transform.localScale = Vector3.one;
			
			Eventitem mm__Eventitem = m_Eventitem.GetComponent<Eventitem>();

			mm__Eventitem.EventData = m_EventListResp.msg[i];

			mm__Eventitem.init();
			
			m_EventitemList.Add(mm__Eventitem);
		}
	}
}
