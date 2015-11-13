using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXComData {

	#region ComVariable 

	public static string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);//确定按钮
	public static string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);//取消按钮
	public static string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);//提示

	#endregion

	#region SendProtoMessage And ReceiveProtoMessage
	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (object value, int protoA ,string protoB = null)
	{
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,value);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),ref t_protof,protoB);
	}

	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (int protoA ,string protoB = null)
	{	
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),protoB);
	}

	/// <summary>
	/// Receives the qx proto message.
	/// </summary>
	/// <returns>The qx proto message.</returns>
	/// <param name="p_message">P_message.</param>
	/// <param name="value">Value.</param>
	public static object ReceiveQxProtoMessage (QXBuffer p_message,object value)
	{
		MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Deserialize(t_stream, value, value.GetType());

		object o_value = value;

		return o_value;
	}
	#endregion

	#region ShowBtnEffect
	public static void ShowChangeSkillEffect (bool tempOpen,GameObject tempSkillBtn,int tempEffectId)
	{
		UI3DEffectTool.Instance ().ClearUIFx (tempSkillBtn);
		if (tempOpen)
		{
			if (MiBaoGlobleData.Instance ().GetMiBaoskillOpen ())
			{
				UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,tempSkillBtn,
				                                               EffectIdTemplate.GetPathByeffectId(tempEffectId));
			}
			
			BoxCollider btnBox = tempSkillBtn.GetComponent<BoxCollider> ();
			if (btnBox != null)
			{
				btnBox.enabled = MiBaoGlobleData.Instance ().GetMiBaoskillOpen ();
			}
			
			UISprite btnSprite = tempSkillBtn.GetComponent<UISprite> ();	 
			if (btnSprite != null)
			{
				btnSprite.color = MiBaoGlobleData.Instance ().GetMiBaoskillOpen () ? Color.white : Color.black;
			}
		}
	}
	#endregion

	#region YinDaoControl
	public enum YinDaoStateControl
	{
		UN_FINISHED_TASK_YINDAO,//任务未完成时的引导步骤
		FINISHED_TASK_YINDAO,//任务完成时的引导步骤
	}
	public static void YinDaoStateController (YinDaoStateControl tempYinDao,int tempTaskId,int tempState)
	{
		switch (tempYinDao)
		{
		case YinDaoStateControl.UN_FINISHED_TASK_YINDAO:

			if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress >= 0)
			{
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[tempTaskId];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempState]);
			}

			break;
		case YinDaoStateControl.FINISHED_TASK_YINDAO:

			if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress < 0)
			{
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[tempTaskId];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempState]);
			}

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Checks the state of the yin DAO open.
	/// </summary>
	/// <returns><c>true</c>, if yin DAO open state was checked, <c>false</c> otherwise.</returns>
	/// <param name="tempTaskId">Temp task identifier.</param>
	public static bool CheckYinDaoOpenState (int tempTaskId)
	{
		if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress >= 0)
		{
			return true;
		}
		return false;
	}
	
	#endregion
}
