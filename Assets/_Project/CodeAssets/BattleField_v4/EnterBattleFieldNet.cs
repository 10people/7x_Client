using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnterBattleFieldNet : MonoBehaviour, SocketProcessor 
{
	public static bool sending = false;


	void Awake() { SocketTool.RegisterMessageProcessor( this ); }
	
	void OnDestroy() 
	{
		sending = false;

		SocketTool.UnRegisterMessageProcessor( this ); 
	}

	public void sendBattle()
	{
		if (sending == true) 
		{
			Debug.Log( "Already.Sending, return now." );

			DestroyObject (gameObject);

			return;
		}

		sending = true;

		if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan)
		{
			OnSendPve();
	
			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_BaiZhan)
		{
			OnSendPvp();
		
			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pve)
		{
			OnSendHYPve();
			
			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_HuangYe_Pvp)
		{
			OnSendHYPvp();
			
			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YaBiao)
		{
			OnSendYaBiao();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YouXia)
		{
			OnSendYouXia();

			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_LueDuo)
		{
			OnSendLueDuo();

			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_YuanZhu)
		{
			OnSendYuanZhu();
			
			OnSendEnterBattle();
		}
		else if(CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_ChongLou)
		{
			CityGlobalData.QCLISOPen = true;

			OnSendChongLou();

			OnSendEnterBattle();
		}
	}

	private void OnSendPvp()
	{
		PvpZhanDouInitReq req = new PvpZhanDouInitReq();
		
		req.userId = CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZhanDou_Init_Pvp_Req, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}
	
	private void OnSendPve()
	{
		PveZhanDouInitReq req = new PveZhanDouInitReq();
		
		req.chapterId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
		
		req.levelType = CityGlobalData.m_levelType;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZhanDou_Init_Pve_Req, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}
	
	private void OnSendHYPve()
	{
		HuangYePveReq req = new HuangYePveReq ();
		
		req.id = (int)CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage( ProtoIndexes.C_HUANGYE_PVE, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR );
	}
	
	private void OnSendHYPvp()
	{
		HuangYePvpReq req = new HuangYePvpReq ();
		
		req.id = CityGlobalData.m_tempPoint;
		
		req.bossId = (int)CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage( ProtoIndexes.C_HUANGYE_PVP, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR );
	}
	
	private void OnSendYaBiao()
	{
		PvpZhanDouInitReq req = new PvpZhanDouInitReq();
		
		req.userId = CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ZHANDOU_INIT_YB_REQ, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}
	
	private void OnSendYouXia()
	{
		YouXiaZhanDouInitReq req = new YouXiaZhanDouInitReq ();
		
		req.chapterId = 300000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INIT_REQ, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}
	
	private void OnSendLueDuo()
	{
		PvpZhanDouInitReq req = new PvpZhanDouInitReq();
		
		req.userId = CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZHANDOU_INIT_LVE_DUO_REQ, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}

	private void OnSendYuanZhu()
	{
		PvpZhanDouInitReq req = new PvpZhanDouInitReq();
		
		req.userId = CityGlobalData.m_tempEnemy;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ZHANDOU_INIT_YUAN_ZHU_REQ, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}

	private void OnSendChongLou()
	{
		ChongLouBattleInit req = new ChongLouBattleInit ();

		req.layer = CityGlobalData.m_tempLevel;

		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.CHONG_LOU_BATTLE_INIT, ref t_protof, ProtoIndexes.ZhanDou_Init_Resp + "|" + ProtoIndexes.S_ZHANDOU_INIT_ERROR);
	}

	private void OnSendEnterBattle()
	{
		LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
		                         PrepareForBattleField.CONST_BATTLE_LOADING_NETWORK, "SendEnterBattle" );
		
		PlayerState t_state = new PlayerState();
		
		t_state.s_state = State.State_PVEOFBATTLE;
		
		SocketHelper.SendQXMessage( t_state, ProtoIndexes.PLAYER_STATE_REPORT );
	}

	public bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message == null) return false;
		
		switch( p_message.m_protocol_index )
		{
		case ProtoIndexes.ZhanDou_Init_Resp:
		{
			Debug.Log( "Receive Zhandou Init Resp." );

			LoadingHelper.ItemLoaded( StaticLoading.m_loading_sections,
			                         PrepareForBattleField.CONST_BATTLE_LOADING_NETWORK, "ZhanDou_Init_Resp" );
			
			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			ZhanDouInitResp resp = new ZhanDouInitResp();

			t_qx.Deserialize(t_stream, resp, resp.GetType());

//			resp.selfTroop.nodes[0].modleId = 4;

			CityGlobalData.t_resp = resp;

			CityGlobalData.m_isBattleField_V4_2D = true;

//			if(!UIShouji.m_isPlayShouji)
//			{
//				if(UIShouji.m_UIShouji.m_isPlay)
//				{
//					UIShouji.m_UIShouji.close();
//					UIShouji.m_UIShouji.gameObject.SetActive(false);
//				}
//			}
			SceneManager.EnterBattleField( CityGlobalData.t_next_battle_field_scene );

			StartCoroutine(des ());

			return true;
		}

		case ProtoIndexes.S_ZHANDOU_INIT_ERROR:
		{
			Debug.Log( "Receive Zhandou Init Error." );

			sending = false;

			MemoryStream t_stream = new MemoryStream( p_message.m_protocol_message, 0, p_message.position );
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			ZhanDouInitError resp = new ZhanDouInitError();
			
			t_qx.Deserialize(t_stream, resp, resp.GetType());
			
			//initError(resp);

			Global.CreateBox(LanguageTemplate.GetText( (LanguageTemplate.Text) 526), resp.result, "", null, LanguageTemplate.GetText( (LanguageTemplate.Text) 11), null, null);

			StartCoroutine(des ());

			return true;
		}
		}
		
		return false;
	}

	private IEnumerator des()
	{
		yield return new WaitForEndOfFrame ();

		DestroyObject (gameObject);
	}

}
