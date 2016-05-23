using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIHuodongPage3 : MYNGUIPanel , SocketListener 
{
	public List<UIHuodongPage3EnemtData> m_listEnemt;
	public ActivityGetStrengthResp m_Info;
	// Use this for initialization
	void Start () 
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void setData(ActivityGetStrengthResp data)
	{
		m_Info = data;
		for(int i = 0; i < m_Info.periodList.Count; i ++)
		{
			if(m_Info.periodList[i].status == 1)
			{
				m_listEnemt[i].m_objButton.gameObject.SetActive(true);
				m_listEnemt[i].m_labelLingqu.gameObject.SetActive(false);
			}
			else if(m_Info.periodList[i].status == 2)
			{
				m_listEnemt[i].m_objButton.gameObject.SetActive(false);
				m_listEnemt[i].m_labelLingqu.gameObject.SetActive(true);
				m_listEnemt[i].m_labelLingqu.text = "已 领";
			}
			else if(m_Info.periodList[i].status == 3)
			{
				m_listEnemt[i].m_objButton.gameObject.SetActive(false);
				m_listEnemt[i].m_labelLingqu.gameObject.SetActive(true);
				m_listEnemt[i].m_labelLingqu.text = "未领取";
			}

			m_listEnemt[i].m_labelNum.text = "x" + m_Info.periodList[i].number;
			m_listEnemt[i].m_labelTime.text = m_Info.periodList[i].time;
		}
	}
	
	public void EndTime()
	{
		
	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_ACTIVITY_STRENGTH_GET_RESP:
			{
				Debug.Log("=========1");
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ActivityGetRewardResp tempInfo = new ActivityGetRewardResp();

				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				Debug.Log(tempInfo.result);
				switch(tempInfo.result)
				{
				case 1:
					ClientMain.m_UITextManager.createText("不在时间内");
					break;
				}
				break;
			}
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("ButtonLingqu") != -1)
		{
			int index = int.Parse(ui.name.Substring(12, 1));
			switch(m_Info.periodList[index].status)
			{
			case 0:
				break;
			case 1:
				Global.ScendID(ProtoIndexes.C_ACTIVITY_STRENGTH_GET_REQ, m_Info.periodList[index].id);
				Global.ScendNull(ProtoIndexes.C_ACTIVITY_STRENGTH_INFO_REQ);
				break;
			case 2:
				break;
			}
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}
