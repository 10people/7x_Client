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

    public void EnterAB(int sceneID)
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterAB()" );
#endif

        ErrorMessage temp = new ErrorMessage()
        {
            errorCode = sceneID
        };

        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_ENTER_LMZ);

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

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ExitScene);

        SceneManager.EnterMainCity();
    }

    #endregion

    #region Carriage

    public void EnterCarriage(float initXPos = 0, float initZPos = 0)
    {
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterCarriage()" );
#endif
		if (!UIShoujiManager.m_UIShoujiManager.m_isPlayShouji)
        {
			UIShoujiManager.m_UIShoujiManager.close();
        }

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

    public void EnterTreasureCity(float initXPos = -15, float initZPos = -30)
    {
        if (TreasureCityData.Instance().CanGetBoxCount() <= 0)
        {
            ClientMain.m_UITextManager.createText(MyColorData.getColorString(4, "今日您可抢的宝箱数已为[d80202]0[-]，明天再来吧。"));
            return;
        }

        if (TreasureCityData.Instance().openState == 100)
        {
            ClientMain.m_UITextManager.createText(MyColorData.getColorString(4, "宝箱副本里的人数已满，请稍后再试。"));
            return;
        }

		if (!UIShoujiManager.m_UIShoujiManager.m_isPlayShouji)
        {
			UIShoujiManager.m_UIShoujiManager.close();
        }
#if DEBUG_SCENE_SYNC
		Debug.Log( "EnterTreasureCity()" );
#endif
        byte[] t_protof = GetEnterSceneStructure(new Vector3(initXPos, 85, initZPos));
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.Enter_TBBXScene, ref t_protof);

        //		Debug.Log ("ProtoIndexes.Enter_TBBXScene:" + ProtoIndexes.Enter_TBBXScene);

        m_enterSceneDelegate = DoEnterTreasureCity;
    }

    void DoEnterTreasureCity()
    {
        SceneManager.EnterTreasureCity();
    }

    public void ExitTreasureCity()
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
                        //						Debug.Log ("EndPos:" + m_MyselfPosition);
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
