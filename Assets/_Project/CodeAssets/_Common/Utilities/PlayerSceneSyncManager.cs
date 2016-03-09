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

    private byte[] GetEnterSceneStructure(float initYPosition)
    {
        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene
        {
            senderName = SystemInfo.deviceName,
            uid = 0,
            posX = 0,
            posY = initYPosition,
            posZ = 0
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
        byte[] t_protof = GetEnterSceneStructure(0.5f);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_FIGHT_SCENE, ref t_protof);

        m_enterSceneDelegate = DoEnterAB;
    }

    private void DoEnterAB()
    {
        SceneManager.EnterAllianceBattle();
    }

    public void ExitAB()
    {
        ExitScene temp = new ExitScene { uid = 0 };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.EXIT_FIGHT_SCENE);

        SceneManager.EnterMainCity();
    }

    #endregion

    #region Carriage

    public void EnterCarriage()
    {
        byte[] t_protof = GetEnterSceneStructure(-2.5f);
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_CARRIAGE_SCENE, ref t_protof);

        m_enterSceneDelegate = DoEnterCarriage;
    }

    private void DoEnterCarriage()
    {
        SceneManager.EnterCarriage();
    }

    public void ExitCarriage()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.EXIT_CARRIAGE_SCENE);

        SceneManager.EnterMainCity();
    }

    public DelegateUtil.VoidDelegate m_DelegateAfterCarriage;

    public Vector2 m_MovePosAfterCarriage;

    public void m_MoveToPosAfterCarriageInit()
    {
        Carriage.RootManager.Instance.m_CarriageMain.TPToPosition(m_MovePosAfterCarriage);
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
