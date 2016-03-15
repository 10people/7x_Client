using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityData : MonoBehaviour ,SocketProcessor {

	private static TreasureCityData treasureData;
	public static TreasureCityData Instance ()
	{
		if (treasureData == null)
		{
			GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();
			
			treasureData = t_GameObject.AddComponent<TreasureCityData>();
		}
		
		return treasureData;
	}

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.OPEN_ShiLian_FuBen://开启十连副本
			{
				//				Debug.Log ("开启十连副本:" + ProtoIndexes.OPEN_ShiLian_FuBen);
				ErrorMessage errorMsg = new ErrorMessage();
				errorMsg = QXComData.ReceiveQxProtoMessage (p_message,errorMsg) as ErrorMessage;
				
				Debug.Log ("errorMsg:" + errorMsg.errorDesc);

				if (MainCityUI.m_MainCityUI != null)
				{
					MainCityUI.m_MainCityUI.addButtonTime (141,int.Parse (errorMsg.errorDesc));
				}

				if(TreasureCityUI.m_instance != null)
				{
					TreasureCityUI.m_instance.TopUI (errorMsg);
				}

				return true;
			}
			}
		}
		return false;
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
