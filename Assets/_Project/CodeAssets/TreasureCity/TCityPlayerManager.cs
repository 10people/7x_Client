using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TCityPlayerManager : TreasureCitySingleton<TCityPlayerManager> , SocketProcessor {

	public GameObject m_NameParent;

	public List<UIPanel> namePanelList = new List<UIPanel> ();
	public GameObject m_boxParent;

	private int targetBoxUID;

	private int isOpeningId;

	void Awake ()
	{
		base.Awake ();
		SocketTool.RegisterMessageProcessor (this);
		LoadModel ();
	}

	void Start ()
	{

	}

	#region LoadPlayer Model

	private Dictionary<int,Object> playerModelDic = new Dictionary<int, Object>();
	private readonly Dictionary<string,int> playerObjIndexDic = new Dictionary<string, int>()
	{
		{PlayerInCityManager.GetModelResPathByRoleId (1),1},
		{PlayerInCityManager.GetModelResPathByRoleId (2),2},
		{PlayerInCityManager.GetModelResPathByRoleId (3),3},
		{PlayerInCityManager.GetModelResPathByRoleId (4),4},
		{PlayerInCityManager.GetModelResPathByRoleId (6902),600},
	};

	void LoadModel ()
	{
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (1), PlayerModelLoadCallback );
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (2), PlayerModelLoadCallback );
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (3), PlayerModelLoadCallback );
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (4), PlayerModelLoadCallback );
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (6902), PlayerModelLoadCallback );
	}

	private void PlayerModelLoadCallback (ref WWW p_www, string p_path, Object p_object )
	{
		playerModelDic.Add (playerObjIndexDic [p_path], p_object);
//		Debug.Log ("p_object:" + p_object);
	}

	#endregion

	#region Open Treasure Box

	public void OpenBox ()
	{
		OpenTreasureBox (targetBoxUID);
	}

	private void OpenTreasureBox (int openBoxId)
	{
		if (openBoxId == isOpeningId)
		{
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"正在开启宝箱！"));
			return;
		}
		else if (openBoxId == -1)
		{
			ClientMain.m_UITextManager.createText (MyColorData.getColorString (5,"请走到周围的宝箱附近！"));
			return;
		}

//		Debug.Log ("boxUID:" + boxUID);
		ErrorMessage msg = new ErrorMessage ();
		msg.errorCode = openBoxId;
		QXComData.SendQxProtoMessage (msg,ProtoIndexes.C_GET_BAO_XIANG);
		TreasureOpenBox box = new TreasureOpenBox ();
//		Debug.Log ("开启宝箱:" + ProtoIndexes.C_GET_BAO_XIANG);
		isOpeningId = openBoxId;
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
//				Debug.Log ("有玩家进入副本:" + ProtoIndexes.Enter_Scene);
				EnterScene enterScene = new EnterScene();
				enterScene = QXComData.ReceiveQxProtoMessage (p_message,enterScene) as EnterScene;

				if (enterScene != null)
				{
//					Debug.Log ("enterScene.roleId:" + enterScene.roleId + "||UID:" + enterScene.uid);
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
//					Debug.Log ("tempMove.uid:" + tempMove.uid);
//					Debug.Log ("tempMove.posX:" + tempMove.posX);
//					Debug.Log ("tempMove.posY:" + tempMove.posY);
//					Debug.Log ("tempMove.posZ:" + tempMove.posZ);
					UpdatePlayerPosition (tempMove);
				}
				return true;
			}
				
			case ProtoIndexes.ExitScene: //玩家退出主城
			{
//				Debug.Log ("玩家退出副本:" + ProtoIndexes.ExitScene);
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
//				Debug.Log ("开箱成功：" + ProtoIndexes.C_GET_BAO_XIANG);
				ErrorMessage msg = new ErrorMessage ();
				msg = QXComData.ReceiveQxProtoMessage (p_message,msg) as ErrorMessage;

				if (msg != null)
				{
//					Debug.Log ("msg.cmd:" + msg.errorDesc);//uid
					//弹出开箱人信息
//					ClientMain.m_UITextManager.createText(MyColorData.getColorString (4,msg.errorDesc));

					bool isOpenBySelf = msg.cmd == PlayerSceneSyncManager.Instance.m_MyselfUid ? true : false;//是否是自己开启
					string getBoxDes = (isOpenBySelf ? "[10ff2b]" : "[e5e205]") + msg.errorDesc + "[-]获得[e5e205]" + msg.errorCode + "元宝[-]";
					TreasureCityUI.m_instance.TopUIMsg (getBoxDes);

					tOpenBoxDic[tOpenBoxDic.Count - 1].num = msg.errorCode;
					tOpenBoxDic[tOpenBoxDic.Count - 1].isOpen = isOpenBySelf;

//					Debug.Log ("SetBoxEnd");
					DestoryTCityBox (tOpenBoxDic[tOpenBoxDic.Count - 1]);
//					Debug.Log ("DestoryEnd");
				}

				return true;
			}
			}
		}
		return false;
	}
	#endregion

	#region Player Or Box Enter City

	private Dictionary<int, GameObject> playerDic = new Dictionary<int, GameObject>(); //玩家集合key:uid
	private Dictionary<int, GameObject> boxDic = new Dictionary<int, GameObject>();//宝箱集合key:uid

	void CreatePlayer (EnterScene tempEnterScene) 
	{
//		Debug.Log("Player:" + tempEnterScene.uid);
//		Debug.Log("tempEnterScene.roleId:" + tempEnterScene.roleId);
//		Debug.Log("tempEnterScene.senderName:" + tempEnterScene.senderName);
		bool isPlayer = tempEnterScene.roleId == 600 ? false : true;

		if (isPlayer)
		{
			if (playerDic.ContainsKey (tempEnterScene.uid))
			{
				return;
			}
			GameObject tCityObj = (GameObject)Instantiate (playerModelDic [tempEnterScene.roleId]);
			tCityObj.name = "PlayerObject:" + tempEnterScene.jzId;
			tCityObj.transform.position = new Vector3 (tempEnterScene.posX,tempEnterScene.posY,tempEnterScene.posZ);
			tCityObj.transform.localScale = Vector3.one * 1.5f;

			playerDic.Add (tempEnterScene.uid,tCityObj);

			//ModelAutoActivator Controller
			{
				TCityPlayerAuto tempAuto = tCityObj.AddComponent<TCityPlayerAuto> ();
				GameObject autoObj = tCityObj.GetComponentInChildren<SkinnedMeshRenderer> ().gameObject;
				tempAuto.AddAutoObj (autoObj);
			}

			TCityPlayerMove playerMove = tCityObj.AddComponent<TCityPlayerMove> ();
			playerMove.InItTCityPlayer (tempEnterScene);
//			PlayerInCity tempItem = tCityObj.AddComponent<PlayerInCity>();
//			tempItem.m_playerID = tempEnterScene.uid;
//			Debug.Log ("NameParentIndex(playerDic.Count):" + NameParentIndex(playerDic.Count));
//			PlayerNameManager.m_PlayerNamesParent = namePanelList[NameParentIndex()].gameObject;
		}
		else
		{
			if (boxDic.ContainsKey (tempEnterScene.uid))
			{
				return;
			}
			GameObject tCityObj = (GameObject)Instantiate (playerModelDic [tempEnterScene.roleId]);
			tCityObj.name = "TreasureBox";
			tCityObj.transform.parent = m_boxParent.transform;
			tCityObj.transform.localPosition = new Vector3 (tempEnterScene.posX,0,tempEnterScene.posY);
			tCityObj.transform.localRotation = new Quaternion(0,0.47f,0,0);
			tCityObj.transform.localScale = Vector3.one * 2f;

			boxDic.Add (tempEnterScene.uid,tCityObj);

			TreasureBox box = tCityObj.GetComponent<TreasureBox> ();
			box.enterScene = tempEnterScene;
			box.InItEffect ();
			
			PlayerNameManager.m_PlayerNamesParent = m_NameParent;

			SerchBox (true);
		}

		PlayerNameManager.CreatePlayerName (tempEnterScene);
	}

	#endregion

	#region PlayerNameParent
	public GameObject NameParentObj ()
	{
		return namePanelList [NameParentIndex ()].gameObject;
	}

	private int NameParentIndex ()
	{
		for (int i = 0;i < namePanelList.Count;i ++)
		{
			int childCount = namePanelList[i].GetComponentsInChildren<PlayerNameInCity> ().Length;
//			Debug.Log ("childCount:" + childCount);
			if (childCount < 50)
			{
//				Debug.Log ("i:" + i);
				return i;
			}
		}

		return 0;
	}
	#endregion

	#region Player Or TreasuerBox Exit City

	private Dictionary<int,TreasureOpenBox> tOpenBoxDic = new Dictionary<int, TreasureOpenBox> ();
	
	void DestoryPlayer (ExitScene tempPlayer) //删除玩家
	{
//		Debug.Log ("Destroy");
		//删除3d模型
		if (tempPlayer != null)
		{
			if (playerDic.ContainsKey (tempPlayer.uid))
			{
				TCityPlayerAuto tempAuto = playerDic[tempPlayer.uid].GetComponent<TCityPlayerAuto> ();
				if (tempAuto != null)
				{
					tempAuto.RemoveAutoObj ();
				}

				TCityPlayerMove playerMove = playerDic[tempPlayer.uid].GetComponent<TCityPlayerMove> ();
				if (playerMove != null)
				{
					Destroy (playerDic[tempPlayer.uid]);
				}

//				PlayerInCity pInCity = playerDic[tempPlayer.uid].GetComponent<PlayerInCity> ();
//				if (pInCity != null)
//				{
//					Destroy (playerDic[tempPlayer.uid]);
//				}

				//及对应的名字
				PlayerNameManager.DestroyPlayerName(tempPlayer); 
				playerDic.Remove (tempPlayer.uid);
			}
			else if (boxDic.ContainsKey (tempPlayer.uid))
			{
				//new add
				TreasureOpenBox tBox = new TreasureOpenBox();
				tBox.exitScene = tempPlayer;
				tBox.boxObj = boxDic[tempPlayer.uid];
				tOpenBoxDic.Add (tOpenBoxDic.Count,tBox);

//				boxDic.Remove (tempPlayer.uid);
//				Debug.Log ("Add uid in tOpenBoxDic");
				//old code
//				StartCoroutine (WaitForDestroy (tempPlayer));
			}
		}
	}

	/// <summary>
	/// Destories the T city box.
	/// </summary>
	/// <param name="tempBoxUID">Temp box user interface.</param>
	void DestoryTCityBox (TreasureOpenBox tempOpenBox)
	{
		TreasureBox tBox = tempOpenBox.boxObj.GetComponent<TreasureBox> ();
		if (tBox != null)
		{
//			Debug.Log ("DestoryTCityBox");
			tBox.DestroyBox (tempOpenBox);
		}
	}

	/// <summary>
	/// Destroies the name of the box.
	/// </summary>
	/// <param name="tempScene">Temp scene.</param>
	public void DestroyBoxName (ExitScene tempScene)
	{
//		Debug.Log ("DestroyBoxName");
		if (boxDic.ContainsKey (tempScene.uid))
		{
			boxDic.Remove (tempScene.uid);
		}
//		Debug.Log ("boxDic.Count:" + boxDic.Count);
		PlayerNameManager.DestroyPlayerName (tempScene); 

		if (boxDic.Count <= 0)
		{
			ClearTOpenBoxDic ();
		}

//		Debug.Log ("tOpenBoxDic.count:" + tOpenBoxDic.Count);
	}

	/// <summary>
	/// Clears the open box dic.
	/// </summary>
	public void ClearTOpenBoxDic ()
	{
		tOpenBoxDic.Clear ();
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
				PlayerNameManager.UpdatePlayerNamePosition (tempPlayer.GetComponent<TCityPlayerMove> ().PlayerEnterScene ().uid,tempPlayer);
//				PlayerNameManager.UpdatePlayerNamePosition (tempPlayer.GetComponent<PlayerInCity>().m_playerID,tempPlayer);
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

		Vector3 targetPosition = new Vector3( tempMove.posX,tempMove.posY,tempMove.posZ );
//		PlayerInCity tempPlayer = playerDic[tempMove.uid].GetComponent<PlayerInCity>();
//		tempPlayer.PlayerRun (targetPosition);
		TCityPlayerMove playerMove = playerDic[tempMove.uid].GetComponent<TCityPlayerMove> ();
		playerMove.PlayerRun( targetPosition );
	}

	#endregion

	#region FindTarget

	private int serchTime = 0;
	public int SerchTime { set{serchTime = value;} get{return serchTime;} }

	/// <summary>
	/// Serchs the box.
	/// </summary>
	/// <param name="serch">If set to <c>true</c> serch.</param>
	/// <param name="tempTime">Temp time.</param>
	public void SerchBox (bool serch,int tempTime = 0)
	{
		SerchTime = tempTime;

		StopCoroutine ("FindBoxInTreasurdCity");

		if (serch)
		{
			StartCoroutine ("FindBoxInTreasurdCity");
		}
	}

	IEnumerator FindBoxInTreasurdCity ()
	{
		while (SerchTime < 1000)
		{
			SerchTime ++;
			yield return new WaitForSeconds (0.3f);
			FindTargetBox ();
			if (SerchTime >= 1000)
			{
				SerchTime = 0;
			}
		}
	}

	void FindTargetBox ()//////////////////
	{
		int uid = -1;
		bool isFind = false;
		foreach (GameObject obj in boxDic.Values)
		{
			TreasureBox box = obj.GetComponent<TreasureBox> ();
			if (Vector3.Distance (obj.transform.position, TreasureCityPlayer.m_instance.m_playerObj.transform.position) <= 4)
			{
				isFind = true;
				uid = box.enterScene.uid;
				break;
			}
		}
		targetBoxUID = uid;
		TreasureCityUI.m_instance.BottomUI (isFind);
	}
	#endregion

	void OnDestroy ()
	{
		PlayerNameManager.DicClear();
		base.OnDestroy ();
		SocketTool.UnRegisterMessageProcessor (this);
	}
}

public class TreasureOpenBox
{
	public ExitScene exitScene;
	public int num = 0;
	public bool isOpen = false;
	public GameObject boxObj;
}
