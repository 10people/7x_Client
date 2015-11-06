using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class BTNTest : MonoBehaviour, SocketListener
{
    public UILabel HouseIDs;
    public UILabel InputHouseID;
    public EventHandler EnterHouseBTN;
    public EventHandler RefreshHouseBTN;

    public string DicKeyStr
    {
        get { return dicKeyStr; }
        set
        {
            dicKeyStr = value;
            HouseIDs.text = value;
        }
    }

    private string dicKeyStr = "";

    void OnEnterHouse(GameObject go)
    {
        int tenementID;
        try
        {
            tenementID = int.Parse(InputHouseID.text);
        }
        catch (Exception ex)
        {
            Debug.LogError("ex: " + ex.Message);
            return;
        }

        TenementManagerment.Instance.NavgationToTenement(tenementID);
    }

    void OnRefresh(GameObject go)
    {
        TenementData.Instance.RequestData();
    }

    void Start()
    {
        DicKeyStr = string.Join(",", TenementData.Instance.m_AllianceCityTenementDic.Keys.Select(item => item.ToString()).ToArray());
    }

    void Awake()
    {
        EnterHouseBTN.m_handler += OnEnterHouse;
        RefreshHouseBTN.m_handler += OnRefresh;

        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        EnterHouseBTN.m_handler -= OnEnterHouse;
        RefreshHouseBTN.m_handler -= OnRefresh;

        SocketTool.UnRegisterSocketListener(this);
    }

    void OnClick()
    {
        //Send character sync message to server.
        EnterScene tempEnterScene = new EnterScene();

        tempEnterScene.senderName = SystemInfo.deviceName;

        tempEnterScene.uid = 0;

        tempEnterScene.posX = 0;

        tempEnterScene.posY = 1;

        tempEnterScene.posZ = 0;

        MemoryStream tempStream = new MemoryStream();

        QiXiongSerializer t_qx = new QiXiongSerializer();

        t_qx.Serialize(tempStream, tempEnterScene);

        byte[] t_protof;

        t_protof = tempStream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ENTER_FIGHT_SCENE, ref t_protof);

        SceneManager.EnterAllianceBattle();

        //Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_BATTLE_WINDOW), OnAllianceBattleLoadCallBack);
    }

    private void OnAllianceBattleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var temp = Instantiate(p_object) as GameObject;
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (Application.loadedLevelName != ConstInGame.CONST_SCENE_NAME_ALLIANCE_BATTLE) return false;

        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.Enter_Scene_Confirm:
                    {
                        Debug.LogWarning("Go to enter scene confirm.");

                        return true;
                    }
            }
        }

        return false;
    }
}
