//#define DEBUG_SCENE_SYNC

using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

public class PlayerSceneSyncManager : Singleton<PlayerSceneSyncManager>, SocketListener
{
    public int m_MyselfUid = -1;
    public Vector3 m_MyselfPosition;

    private delegate void VoidDelegate();

    private VoidDelegate m_enterSceneDelegate;

    private byte[] GetEnterSceneStructure(Vector3 initPosition)
    {
        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene
        {
            senderName = SystemInfo.deviceName,
            uid = 0,
            posX = initPosition.x,
            posY = initPosition.y,
            posZ = initPosition.z
        };

        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        t_qx.Serialize(tempStream, tempEnterScene);
        byte[] t_protof;
        t_protof = tempStream.ToArray();

        return t_protof;
    }

    #region Alliance Battle

    public void EnterAB()
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterAB()" );
#endif

        byte[] t_protof = GetEnterSceneStructure(new Vector3(0, 0.5f, 0));
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_FIGHT_SCENE, ref t_protof);

        m_enterSceneDelegate = DoEnterAB;
    }

    private void DoEnterAB()
    {
        SceneManager.EnterAllianceBattle();
    }

    public void ExitAB()
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "ExitAB()" );
#endif

        ExitScene temp = new ExitScene { uid = 0 };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.EXIT_FIGHT_SCENE);

        SceneManager.EnterMainCity();
    }

    #endregion

    #region Carriage

    public void EnterCarriage(float initXPos = 0, float initZPos = 0)
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterCarriage()" );
#endif

        byte[] t_protof = GetEnterSceneStructure(new Vector3(initXPos, -2.5f, initZPos));
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_CARRIAGE_SCENE, ref t_protof);

        m_enterSceneDelegate = DoEnterCarriage;
    }

    private void DoEnterCarriage()
    {
        SceneManager.EnterCarriage();
    }

    public void ExitCarriage()
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterCarriage()" );
#endif

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.EXIT_CARRIAGE_SCENE);

        SceneManager.EnterMainCity();
    }

    #endregion

	#region Treasure City

	public void EnterTreasureCity (float initXPos = 0, float initZPos = 0)
	{
		#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterTreasureCity()" );
		#endif

		byte[] t_protof = GetEnterSceneStructure(new Vector3(initXPos, -2.5f, initZPos));
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.Enter_TBBXScene, ref t_protof);

//		Debug.Log ("ProtoIndexes.Enter_TBBXScene:" + ProtoIndexes.Enter_TBBXScene);

		m_enterSceneDelegate = DoEnterTreasureCity;
	}

	void DoEnterTreasureCity ()
	{
		SceneManager.EnterTreasureCity ();
	}

	public void ExitTreasureCity ()
	{
		#if DEBUG_SCENE_SYNC
		Debug.Log( "ExitTreasureCity()" );
		#endif

		ExitScene temp = new ExitScene { uid = 0 };
		SocketHelper.SendQXMessage(temp, ProtoIndexes.Exit_TBBXScene);
		
		SceneManager.EnterMainCity();
	}

	#endregion

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.Enter_Scene_Confirm:
                    {
#if DEBUG_SCENE_SYNC
						Debug.Log( "Enter_Scene_Confirm()" );
#endif

                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EnterSceneConfirm tempScene = new EnterSceneConfirm();
                        t_qx.Deserialize(t_stream, tempScene, tempScene.GetType());

                        m_MyselfUid = tempScene.uid;
                        m_MyselfPosition = new Vector3(tempScene.posX, tempScene.posY, tempScene.posZ);

                        if (m_enterSceneDelegate != null)
                        {
                            m_enterSceneDelegate();
                            m_enterSceneDelegate = null;
                        }

                        return true;
                    }
            }
        }
        return false;
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    new void OnDestroy()
    {
        base.OnDestroy();

        SocketTool.UnRegisterSocketListener(this);
    }
}
