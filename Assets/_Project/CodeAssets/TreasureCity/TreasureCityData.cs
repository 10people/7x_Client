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

	private ErrorMessage tCityData = new ErrorMessage()
	{
		cmd = 0,
		errorCode = 0,
		errorDesc = "0",
	};

	private ErrorMessage tCityBoxData;

	public int staySeconds = 60;//可以待多久(秒)
	public int openState = 0;//可进，100 满了不可进
	public int nextBxSec = 60 * 5;//下次刷宝箱时间

	public void parseParam (string info)
	{
//		Debug.Log ("info:" + info);
		string[] arr = info.Split ('#');
		nextBxSec = int.Parse(arr [0].Split (':') [1]);
		openState = int.Parse(arr [1].Split (':') [1]);
		staySeconds = int.Parse(arr [2].Split (':') [1]);
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

				tCityData = errorMsg;
				parseParam (tCityData.errorDesc);

				if (MainCityUI.m_MainCityUI != null)
				{
//					Debug.Log ("1");
					if (tCityData.errorCode == 0 && tCityData.cmd == 0)
					{
//						Debug.Log ("2");
						MainCityUI.m_MainCityUI.deleteMaincityUIButton (141);
					}
					else
					{
//						Debug.Log ("4");
						if (FunctionOpenTemp.IsHaveID (1103))
						{
//							Debug.Log ("3");
							MainCityUI.m_MainCityUI.addButtonTime (141,nextBxSec);
						}
					}
				}

				if(TreasureCityUI.m_instance != null)
				{
					TreasureCityUI.m_instance.TopUI (errorMsg);
				}

				return true;
			}
			case ProtoIndexes.BAO_XIANG_PICKED_INFO:
			{
//				Debug.Log ("十连副本得宝箱数:" + ProtoIndexes.BAO_XIANG_PICKED_INFO);
				ErrorMessage errorMsg = new ErrorMessage();
				errorMsg = QXComData.ReceiveQxProtoMessage (p_message,errorMsg) as ErrorMessage;

				tCityBoxData = errorMsg;

//				Debug.Log ("可拾取箱子:" + tCityBoxData.cmd);
//				Debug.Log ("已捡元宝:" + tCityBoxData.errorCode);
//				Debug.Log ("tCityBoxData.errorDes:" + tCityBoxData.errorDesc);

				if (MainCityUI.m_MainCityUI != null)
				{
					MainCityUI.SetSuperRed (141,tCityBoxData.cmd > 0 && openState == 0 ? true : false);
				}

				if(TreasureCityUI.m_instance != null)
				{
					TreasureCityUI.m_instance.TopYBUI (errorMsg);
				}

				return true;
			}
			}
		}
		return false;
	}

	/// <summary>
	/// Gets the T city data.
	/// </summary>
	/// <returns>The T city data.</returns>
	public ErrorMessage GetTCityData ()
	{
		return tCityData;
	}

	/// <summary>
	/// Gets the T city box data.
	/// </summary>
	/// <returns>The T city box data.</returns>
	public ErrorMessage GetTCityBoxData ()
	{
		return tCityBoxData;
	}

	/// <summary>
	/// Determines whether this instance can get box count.
	/// </summary>
	/// <returns><c>true</c> if this instance can get box count; otherwise, <c>false</c>.</returns>
	public int CanGetBoxCount ()
	{
		return tCityBoxData.cmd;
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
