using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class TongzhiData
{
    public SuBaoMSG m_SuBaoMSG;
    public ReportTemplate m_ReportTemplate;

    public List<int> m_ButtonIndexList = new List<int>();
	public List<int> m_ButtonIndexShowDesList = new List<int>();

    public int m_ReceiveSceneType = 0;
    public int m_PopupableType = 0;
    public int m_ShowType = 0;

    public bool m_isLooked = false;

    public bool IsInReceiveScene()
    {
        if ((Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN) && ((m_PopupableType & 2) != 0))
        {
            return true;
        }
        else if ((Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE) && ((m_PopupableType & 1) != 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsReportShowType()
    {
        return ((m_ShowType & 1) != 0);
    }

    public bool IsEffectShowType()
    {
        return ((m_ShowType & 2) != 0);
    }

    public TongzhiData(SuBaoMSG data)
    {
        m_SuBaoMSG = data;
        m_ReportTemplate = ReportTemplate.GetHeroSkillUpByID(m_SuBaoMSG.configId);
        string tempString = m_ReportTemplate.m_sButton;
        while (!string.IsNullOrEmpty(tempString) && tempString.Length != 0)
        {
            m_ButtonIndexList.Add(int.Parse(Global.NextCutting(ref tempString)));
        }

        tempString = m_ReportTemplate.m_sRecveiveScence;
        while (!string.IsNullOrEmpty(tempString) && tempString.Length != 0)
        {
            m_ReceiveSceneType ^= int.Parse(Global.NextCutting(ref tempString));
        }

        tempString = m_ReportTemplate.m_sPopupable;
        while (!string.IsNullOrEmpty(tempString) && tempString.Length != 0)
        {
            m_PopupableType ^= int.Parse(Global.NextCutting(ref tempString));
        }

        tempString = m_ReportTemplate.m_ShowType;
        while (!string.IsNullOrEmpty(tempString) && tempString.Length != 0)
        {
            m_ShowType ^= int.Parse(Global.NextCutting(ref tempString));
        }

		tempString = m_ReportTemplate.m_sButtonfeedback;
		while (!string.IsNullOrEmpty(tempString) && tempString.Length != 0)
		{
			m_ButtonIndexShowDesList.Add(int.Parse(Global.NextCutting(ref tempString)));
		}
	}
}
