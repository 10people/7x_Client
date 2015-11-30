using UnityEngine;
using System.Collections;
using System.IO;
using qxmobile.protobuf;

public class PlayerSceneSyncManager : Singleton<PlayerSceneSyncManager>, SocketListener
{
    public int m_MyselfUid = -1;

    private delegate void VoidDelegate();

    private VoidDelegate m_enterSceneDelegate;

    #region Alliance Battle

    public void EnterAB()
    {
        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene
        {
            senderName = SystemInfo.deviceName,
            uid = 0,
            posX = 0,
            posY = 1,
            posZ = 0
        };

        MemoryStream tempStream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        t_qx.Serialize(tempStream, tempEnterScene);
        byte[] t_protof;
        t_protof = tempStream.ToArray();

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

        //GoToReturn.
        //if (AllianceData.Instance.IsAllianceNotExist)
        //{
        SceneManager.EnterMainCity();
        //}
        //else
        //{
        //   // SceneManager.EnterAllianceCity();
        //}
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
