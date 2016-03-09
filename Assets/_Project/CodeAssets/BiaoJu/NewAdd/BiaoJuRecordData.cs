using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

namespace Carriage
{
	public class BiaoJuRecordData : Singleton<BiaoJuRecordData>,SocketProcessor {

		public enum RecordType
		{
			HISTORY,
			ENEMY,
		}
		private RecordType recordType = RecordType.HISTORY;

		private YBHistoryResp historyResp;
		private EnemiesResp enemyResp;

		private GameObject biaoJuRecordPrefab;

		void Awake ()
		{
			SocketTool.RegisterMessageProcessor (this);
		}

		/// <summary>
		/// Biaos the ju record req.
		/// </summary>
		/// <param name="tempType">Temp type.</param>
		public void BiaoJuRecordReq (RecordType tempType)
		{
			recordType = tempType;
			switch (tempType)
			{
			case RecordType.HISTORY:
				QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_HISTORY_RSQ,ProtoIndexes.S_YABIAO_HISTORY_RESP.ToString ());
				Debug.Log ("劫镖记录请求：" + ProtoIndexes.C_YABIAO_HISTORY_RSQ);
				break;
			case RecordType.ENEMY:
				QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_ENEMY_RSQ);
				Debug.Log ("仇人列表请求：" + ProtoIndexes.C_YABIAO_ENEMY_RSQ);
				break;
			default:
				break;
			}
		}

		public bool OnProcessSocketMessage (QXBuffer p_message)
		{
			if (p_message != null)
			{
				switch (p_message.m_protocol_index)
				{
				case ProtoIndexes.S_YABIAO_HISTORY_RESP:
				{
					Debug.Log ("劫镖记录返回：" + ProtoIndexes.S_YABIAO_HISTORY_RESP );
					YBHistoryResp recordRes = new YBHistoryResp();
					recordRes = QXComData.ReceiveQxProtoMessage (p_message,recordRes) as YBHistoryResp;
					
					if (recordRes != null)
					{
						if (recordRes.historyList == null)
						{
							recordRes.historyList = new List<YBHistory>();
						}

						historyResp = recordRes;

						recordType = RecordType.HISTORY;

						if (biaoJuRecordPrefab == null)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BIAOJU_RECORD_PAGE ),
							                        BiaoJuRecordLoadBack );
						}
						else
						{
							InItRecordPage ();
						}
					}
					
					return true;
				}
				case ProtoIndexes.S_YABIAO_ENEMY_RESP:
				{
					Debug.Log ("仇人列表返回：" + ProtoIndexes.S_YABIAO_ENEMY_RESP);
					EnemiesResp enemyPageResp = new EnemiesResp();
					enemyPageResp = QXComData.ReceiveQxProtoMessage (p_message,enemyPageResp) as EnemiesResp;
					
					if (enemyPageResp != null)
					{
						if (enemyPageResp.enemyList == null)
						{
							enemyPageResp.enemyList = new List<EnemiesInfo>();
						}
						
						enemyResp = enemyPageResp;

						recordType = RecordType.ENEMY;

						if (biaoJuRecordPrefab == null)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BIAOJU_RECORD_PAGE ),
							                        BiaoJuRecordLoadBack );
						}
						else
						{
							InItRecordPage ();
						}
					}
					
					return true;
				}
				}
			}

			return false;
		}

		void BiaoJuRecordLoadBack ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
		{
			biaoJuRecordPrefab = GameObject.Instantiate( p_object ) as GameObject;
			
			InItRecordPage ();
		}

		void InItRecordPage ()
		{
			biaoJuRecordPrefab.SetActive (true);
			BiaoJuRecordPage.bjRecordPage.SetRecordPage (recordType);
			switch (recordType)
			{
			case RecordType.HISTORY:

				BiaoJuRecordPage.bjRecordPage.InItHistoryPage (historyResp);

				break;
			case RecordType.ENEMY:

				BiaoJuRecordPage.bjRecordPage.InItEnemyPage (enemyResp);

				break;
			default:
				break;
			}
		}

		void OnDestroy ()
		{
			SocketTool.UnRegisterMessageProcessor (this);
			base.OnDestroy ();
		}
	}
}
