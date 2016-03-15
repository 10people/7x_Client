using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TCityPlayerManager : TreasureCitySingleton<TCityPlayerManager> , SocketProcessor {
	
	private Dictionary<int, EnterScene> boxExitList = new Dictionary<int, EnterScene> ();//销毁的箱子列表

	public GameObject m_NameParent;
	public GameObject m_boxParent;

	private bool isPlayer = false;

	private bool isOpenBox = false;//是否开启了宝箱
	public bool IsOpenBox { set{isOpenBox = value;} get{return isOpenBox;} }

	void Awake ()
	{
		base.Awake ();
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{

	}

	#region Open Treasure Box
	
	public void OpenTreasureBox (int boxUID)
	{
		Debug.Log ("boxUID:" + boxUID);
		ErrorMessage msg = new ErrorMessage ();
		msg.errorCode = boxUID;
		QXComData.SendQxProtoMessage (msg,ProtoIndexes.C_GET_BAO_XIANG);
		Debug.Log ("开启宝箱:" + ProtoIndexes.C_GET_BAO_XIANG);
	}
	
	#endregion

	#region Receive Socket Message
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.Enter_Scene: //有玩家进入主城
			{
				Debug.Log ("有玩家进入副本:" + ProtoIndexes.Enter_Scene);
				EnterScene enterScene = new EnterScene();
				enterScene = QXComData.ReceiveQxProtoMessage (p_message,enterScene) as EnterScene;

				if (enterScene != null)
				{
					Debug.Log ("enterScene.roleId:" + enterScene.roleId);
					isPlayer = roleIdLength.Contains (enterScene.roleId) ? true : false;
					CreatePlayer (enterScene);
				}

				return true;
			}

			case ProtoIndexes.Sprite_Move: //玩家移动
			{
//				Debug.Log ("玩家移动:" + ProtoIndexes.Sprite_Move);
				SpriteMove tempMove = new SpriteMove();
				tempMove = QXComData.ReceiveQxProtoMessage (p_message,tempMove) as SpriteMove;

				if (tempMove != null)
				{
					UpdatePlayerPosition (tempMove);
				}
				return true;
			}
				
			case ProtoIndexes.ExitScene: //玩家退出主城
			{
				Debug.Log ("玩家退出副本:" + ProtoIndexes.ExitScene);
				ExitScene tempScene = new ExitScene();
				tempScene = QXComData.ReceiveQxProtoMessage (p_message,tempScene) as ExitScene;

				if (tempScene != null)
				{
					DestoryPlayer (tempScene);
				}
				return true;
			}

			case ProtoIndexes.S_HEAD_STRING: //玩家称号、vip更新
			{
//				Debug.Log ("玩家称号、vip更新:" + ProtoIndexes.S_HEAD_STRING);
				ErrorMessage tempScene = new ErrorMessage();
				tempScene = QXComData.ReceiveQxProtoMessage (p_message,tempScene) as ErrorMessage;

				if (tempScene != null)
				{
					if (!PlayersManager.m_playrHeadInfo.ContainsKey (tempScene.errorCode))
					{
						PlayersManager.m_playrHeadInfo.Add (tempScene.errorCode, tempScene);
					}
					else
					{
						PlayersManager.m_playrHeadInfo[tempScene.errorCode] = tempScene;
					}
					PlayerNameManager.UpdateAllLabel(tempScene);
				}
				return true;
			}
			case ProtoIndexes.C_GET_BAO_XIANG:
			{
				Debug.Log ("开箱成功：" + ProtoIndexes.C_GET_BAO_XIANG);
				ErrorMessage msg = new ErrorMessage ();
				msg = QXComData.ReceiveQxProtoMessage (p_message,msg) as ErrorMessage;

				if (msg != null)
				{
					IsOpenBox = true;
					Debug.Log ("msg.errorCode:" + msg.errorCode + "IsOpenBox:" + IsOpenBox);
				}

				return true;
			}
			}
		}
		return false;
	}
	#endregion

	#region Player Or Box Enter City

	private readonly List<int> roleIdLength = new List<int>{1,2,3,4};
	private List<EnterScene> playerInfoList = new List<EnterScene>();
	private Dictionary<int, GameObject> playerDic = new Dictionary<int, GameObject>(); //玩家集合
	private Dictionary<int, GameObject> boxDic = new Dictionary<int, GameObject> ();//宝箱集合

	void CreatePlayer (EnterScene tempEnterScene) 
	{
		Debug.Log("Player:" + tempEnterScene.uid);
		Debug.Log("tempEnterScene.roleId:" + tempEnterScene.roleId);
		Debug.Log("tempEnterScene.senderName:" + tempEnterScene.senderName);

		int size = playerInfoList.Count;
		for (int i = 0; i < size; i++)
		{
			if (playerInfoList[i].uid == tempEnterScene.uid)
			{
				return;
			}
		}
		playerInfoList.Add (tempEnterScene);
	
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (isPlayer ? tempEnterScene.roleId : 6902), ResourceLoadCallback );
	}

	private void ResourceLoadCallback (ref WWW p_www, string p_path, Object p_object )
	{
		for (int i = playerInfoList.Count - 1;i >= 0;i--) 
		{
			EnterScene t_info = playerInfoList [i];

			if (PlayerInCityManager.GetModelResPathByRoleId (isPlayer ? t_info.roleId : 6902) == p_path)
			{
				LoadPlayer (t_info, p_object);
				
				playerInfoList.Remove(t_info);
			}
		}
	}

	private void LoadPlayer (EnterScene tempEnterScene,Object playerObject)
	{
		if (!playerDic.ContainsKey (tempEnterScene.uid))
		{
			Vector3 t_pos = new Vector3 (tempEnterScene.posX,
			                             0,
			                             tempEnterScene.posY);
			Debug.Log ("pos:" + t_pos);
			GameObject t_gb = Instantiate (playerObject, isPlayer ? new Vector3(-15,0,-30) : t_pos, Quaternion.Euler(Vector3.zero)) as GameObject;
//			GameObject t_gb = Instantiate (playerObject,new Vector3(-15,0,-30), Quaternion.Euler(Vector3.zero)) as GameObject;
			t_gb.name = isPlayer ? "PlayerObject:" + tempEnterScene.jzId : "TreasureBox";
			t_gb.transform.parent = isPlayer ? this.transform : m_boxParent.transform;
			t_gb.transform.localPosition = isPlayer ? new Vector3(-15,0,-30) : t_pos;
			t_gb.transform.localScale = isPlayer ? Vector3.one * 1.5f : Vector3.one * 1.5f;

			if (isPlayer)
			{
				PlayerInCity tempItem = t_gb.AddComponent<PlayerInCity>();
				tempItem.m_playerID = tempEnterScene.uid;

//				PlayerNameManager.m_PlayerNamesParent = m_NameParent;
//				PlayerNameManager.CreatePlayerName (tempEnterScene);

				playerDic.Add (tempEnterScene.uid, t_gb);
			}
			else
			{
				TreasureBox box = t_gb.AddComponent<TreasureBox> ();
				box.InItTreasureBox (tempEnterScene);
				boxDic.Add (tempEnterScene.uid, t_gb);
			}

			PlayerNameManager.m_PlayerNamesParent = m_NameParent;
			PlayerNameManager.CreatePlayerName (tempEnterScene);

			EffectTool.DisableCityOcclusion(t_gb);
		}
		else
		{
			playerObject = null;
		}
	}
	#endregion

	#region Player Exit City

	void DestoryPlayer (ExitScene tempPlayer) //删除玩家
	{
		//删除3d模型
		if (tempPlayer != null)
		{
			if (playerDic.ContainsKey (tempPlayer.uid))
			{
				PlayerInCity pInCity = playerDic[tempPlayer.uid].GetComponent<PlayerInCity> ();

				if (pInCity != null)
				{
					Destroy (playerDic[tempPlayer.uid]);
				}

				//及对应的名字
				PlayerNameManager.DestroyPlayerName(tempPlayer); 
				playerDic.Remove (tempPlayer.uid);
			}
			else if (boxDic.ContainsKey (tempPlayer.uid))
			{
				StartCoroutine (WaitForDestroy (tempPlayer));
			}
		}
	}

	IEnumerator WaitForDestroy (ExitScene tempPlayer)
	{
		yield return new WaitForSeconds (1);

		TreasureBox tBox = boxDic[tempPlayer.uid].GetComponent<TreasureBox> ();
		if (tBox != null)
		{
			Debug.Log ("destroy");
			tBox.DestroyBox ();
		}
		boxDic.Remove (tempPlayer.uid);
		PlayerNameManager.DestroyPlayerName(tempPlayer); 
	}

	#endregion

	#region Refresh Player's Pos And NamePos
	//更新玩家位置和玩家名字位置
	void LateUpdate()
	{
		UpdatePlayerNamePosition();
	}

	//更新玩家名字位置
	private void UpdatePlayerNamePosition() 
	{
		foreach (GameObject tempPlayer in playerDic.Values)
		{
			if (tempPlayer != null)
			{
				PlayerNameManager.UpdatePlayerNamePosition (tempPlayer.GetComponent<PlayerInCity>().m_playerID,tempPlayer);
			}
		}
		foreach (GameObject tempPlayer in boxDic.Values)
		{
			if (tempPlayer != null)
			{
				PlayerNameManager.UpdatePlayerNamePosition (tempPlayer.GetComponent<TreasureBox>().enterScene.uid,tempPlayer);
			}
		}
	}

	//更新玩家位置
	private void UpdatePlayerPosition (SpriteMove tempMove) 
	{
		if (!playerDic.ContainsKey (tempMove.uid))
		{
			return;
		}
		
		PlayerInCity tempPlayer = playerDic[tempMove.uid].GetComponent<PlayerInCity>();
		
		Vector3 targetPosition = new Vector3( tempMove.posX,tempMove.posY,tempMove.posZ );
		
		tempPlayer.PlayerRun( targetPosition );
	}

	#endregion

	void OnDestroy ()
	{
		base.OnDestroy ();
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
