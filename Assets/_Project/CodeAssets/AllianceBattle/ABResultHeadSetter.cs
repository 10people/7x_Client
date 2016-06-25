using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AllianceBattle
{
    public class ABResultHeadSetter : MonoBehaviour
    {
        public UISprite HeadSprite;
        public UILabel NameLabel;
        public UILabel AllianceLabel;
        public UISprite RankBGSprite;

        private const string HeadPrefix = "PlayerIcon";
        private const string BGPrefix = "Top";

        public void SetThis(int roleID, string kingName, string allianceName, int rank)
        {
            kingName = kingName ?? "";
            allianceName = allianceName ?? "";

            HeadSprite.spriteName = HeadPrefix + roleID;
            if (AllianceData.Instance.g_UnionInfo == null)
            {
                NameLabel.text = ColorTool.Color_Red_c40000 + kingName + "[-]";
                AllianceLabel.text = ColorTool.Color_Red_c40000 + "<" + allianceName + ">[-]";
            }
            else
            {
                NameLabel.text = (((RootManager.Instance.MyPart == 1 && allianceName == AllianceData.Instance.g_UnionInfo.name) || (RootManager.Instance.MyPart == 2 && allianceName != AllianceData.Instance.g_UnionInfo.name)) ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_01edf0) + kingName + "[-]";
                AllianceLabel.text = (((RootManager.Instance.MyPart == 1 && allianceName == AllianceData.Instance.g_UnionInfo.name) || (RootManager.Instance.MyPart == 2 && allianceName != AllianceData.Instance.g_UnionInfo.name)) ? ColorTool.Color_Red_c40000 : ColorTool.Color_Blue_01edf0) + "<" + allianceName + ">[-]";
            }
            RankBGSprite.spriteName = BGPrefix + rank;
        }
    }
}