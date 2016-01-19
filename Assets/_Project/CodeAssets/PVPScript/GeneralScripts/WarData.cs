using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class WarData : Singleton<WarData>,SocketProcessor {

	private MainSimpleInfoResp mainSimpleResp;

	private GameObject warPagePrefab;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// Wars the type req.
	/// </summary>
	/// <param name="tempType">Temp type.</param>
	public void OpenWarSelectWindow ()
	{
		MainSimpleInfoReq mainSimpleReq = new MainSimpleInfoReq ();
		mainSimpleReq.functionType = 8;
		QXComData.SendQxProtoMessage (mainSimpleReq,ProtoIndexes.C_MAIN_SIMPLE_INFO_REQ,ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP.ToString ());

		//		Debug.Log ("Fight窗口信息请求" + ProtoIndexes.C_MAIN_SIMPLE_INFO_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP:
			{
//				Debug.Log ("Fight窗口信息返回" + ProtoIndexes.S_MAIN_SIMPLE_INFO_RESP);

				MainSimpleInfoResp mainSimpleRes = new MainSimpleInfoResp();
				mainSimpleRes = QXComData.ReceiveQxProtoMessage (p_message,mainSimpleRes) as MainSimpleInfoResp;

				if (mainSimpleRes != null)
				{
					if (mainSimpleRes.info == null)
					{
						mainSimpleRes.info = new List<SimpleInfo>();
					}
					mainSimpleResp = mainSimpleRes;

					LoadWarPageObj ();
				}
				return true;
			}
			}
		}

		return false;
	}

	void LoadWarPageObj ()
	{
		if (warPagePrefab == null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.WAR_SELECT_PAGE ),
			                        WarPagePrefabLoadBack );
		}
		else
		{
			warPagePrefab.SetActive (true);
			InItWarPageInfo ();
		}
	}

	void WarPagePrefabLoadBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		warPagePrefab = GameObject.Instantiate( p_object ) as GameObject;
		InItWarPageInfo ();
	}

	void InItWarPageInfo ()
	{
		if (!MainCityUI.IsExitInObjectList (warPagePrefab))
		{
			MainCityUI.TryAddToObjectList (warPagePrefab);
		}

		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}

		WarPage.warPage.InItWarPage (mainSimpleResp);
	}

	void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
