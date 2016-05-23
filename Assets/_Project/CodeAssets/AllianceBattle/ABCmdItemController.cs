using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABCmdItemController : MonoBehaviour
    {
        public struct CmdItemData
        {
            public int ID;
            public string Desc;
        }

        public CmdItemData m_CmdItemData;

        public UILabel DescLabel;

        public void SetThis()
        {
            DescLabel.text = m_CmdItemData.Desc;
        }

        public void OnClick()
        {
            ErrorMessage temp = new ErrorMessage()
            {
                errorCode = m_CmdItemData.ID,
                errorDesc = m_CmdItemData.Desc
            };

            SocketHelper.SendQXMessage(temp, ProtoIndexes.LMZ_CMD_ONE);

            RootManager.Instance.m_AllianceBattleMain.OnCommandClick();
        }
    }
}