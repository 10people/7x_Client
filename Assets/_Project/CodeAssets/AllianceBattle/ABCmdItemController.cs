using UnityEngine;
using System.Collections;
using System.Linq;
using qxmobile.protobuf;

namespace AllianceBattle
{
    public class ABCmdItemController : MonoBehaviour
    {
        public AllianceBattleMain m_AllianceBattleMain;

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
            if (ConfigTool.GetBool(ConfigTool.CONST_TEST_MODE) || m_AllianceBattleMain.battleStatState >= 0)
            {
                if (m_AllianceBattleMain.RemainingCmdCD > 0)
                {
                    ClientMain.m_UITextManager.createText("请在" + m_AllianceBattleMain.RemainingCmdCD + "秒后使用");
                    return;
                }

                if (m_CmdItemData.ID == 104 && m_AllianceBattleMain.m_RootManager.m_AbHoldPointManager.HoldPointDic.Values.Where(item => item.Side == 1 && item.Type == 3).All(item => !item.IsDestroyed))
                {
                    ClientMain.m_UITextManager.createText("攻陷一路前哨和行营后才能进攻帅旗");
                    m_AllianceBattleMain.StartCommandCDCalc();
                    return;
                }

                var str = "";

                if (!AllianceData.Instance.IsAllianceNotExist)
                {
                    str = str + ColorTool.Color_Blue_01edf0 + "[" + FunctionWindowsCreateManagerment.GetIdentityById(AllianceData.Instance.g_UnionInfo.identity) + "]" + "[-]";
                }

                str = str + JunZhuData.Instance().m_junzhuInfo.name + ":" + ColorTool.Color_Yellow_f4e002 + m_CmdItemData.Desc + "[-]";

                ErrorMessage temp = new ErrorMessage()
                {
                    errorCode = m_CmdItemData.ID,
                    errorDesc = str
                };

                SocketHelper.SendQXMessage(temp, ProtoIndexes.LMZ_CMD_ONE);

                RootManager.Instance.m_AllianceBattleMain.OnCommandClick();
            }
            else
            {
                ClientMain.m_UITextManager.createText("开战后可以发布战报指令");
            }
        }
    }
}