//#define UNIT_TEST

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using qxmobile.protobuf;
using Rank;

public class AllianceMemberWindowManager : Singleton<AllianceMemberWindowManager>, SocketListener
{
    public void OpenAllianceMemeberWindow(int allianceID, string selectedAllianceName)
    {
        AlliancePlayerReq temp = new AlliancePlayerReq() { mengId = allianceID };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.RANKING_ALLIANCE_MEMBER_REQ);

        SelectedAllianceName = selectedAllianceName;
        isOutterCall = true;
    }

    public void OpenAllianceMemberWindowInRank(int allianceID, string selectedAllianceName, RootController controller, string internalPassword)
    {
        if (internalPassword != "UseInRank")
        {
            return;
        }

        AlliancePlayerReq temp = new AlliancePlayerReq() { mengId = allianceID };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.RANKING_ALLIANCE_MEMBER_REQ);

        SelectedAllianceName = selectedAllianceName;
        isOutterCall = false;
        m_RootController = controller;
    }

    private bool isOutterCall = false;
    public string SelectedAllianceName;
    public RootController m_RootController;
    public AlliancePlayerResp m_AlliancePlayerResp;

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {

                //Alliance member info request.
                case ProtoIndexes.RANKING_ALLIANCE_MEMBER_RESP:
                    {
                        object playerRespObject = new AlliancePlayerResp();
                        if (SocketHelper.ReceiveQXMessage(ref playerRespObject, p_message, ProtoIndexes.RANKING_ALLIANCE_MEMBER_RESP))
                        {
                            m_AlliancePlayerResp = playerRespObject as AlliancePlayerResp;

                            if (m_AlliancePlayerResp != null && m_AlliancePlayerResp.player != null && m_AlliancePlayerResp.player.Any())
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_MEMBER_WINDOW), AllianceMemberWindowCallBack);
                            }
                            else
                            {
                                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), NoAllianceCallBack);
                            }

                            return true;
                        }
                        return false;
                    }
            }
        }
        return false;
    }

    public void AllianceMemberWindowCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var tempObject = Instantiate(p_object) as GameObject;
        var controller = tempObject.GetComponent<AllianceMemberController>();

        controller.isOutterCall = isOutterCall;
        controller.m_AllianceName = SelectedAllianceName;
        controller.m_AlliancePlayerResp = m_AlliancePlayerResp;
        if (isOutterCall)
        {
            MainCityUI.TryAddToObjectList(tempObject);
        }
        else
        {
            controller.m_RootController = m_RootController;
        }

        controller.SetThis();
    }

    public void NoAllianceCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox(LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO),
            null, "此联盟不存在",
            null,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null);
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    new void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        base.OnDestroy();
    }

#if UNIT_TEST

    private int allianceid;

    void OnGUI()
    {
        allianceid = int.Parse(GUILayout.TextArea(allianceid.ToString()));

        if (GUILayout.Button("Test alliance memeber"))
        {
            OpenAllianceMemeberWindow(allianceid, "test");
        }
    }

#endif

}
