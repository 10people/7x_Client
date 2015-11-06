using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AllianceBattleWindow : MonoBehaviour
{
    public List<EventIndexHandle> m_EventIndexHandles = new List<EventIndexHandle>();
    public ScaleEffectController m_ScaleEffectController;

    public UILabel m_InfoLabelTop;
    public UILabel m_InfoLabelLeft;
    public UILabel m_FrameLabelMain;
    public UILabel m_FrameLabelSub;

    public void SetThis()
    {

    }

    void CloseWindow()
    {
        m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
    }

    void DoCloseWindow()
    {
        Destroy(gameObject);
    }

    void OnEventClick(int index)
    {
        switch (index)
        {
            //Back
            case 0:
                {
                    CloseWindow();
                    break;
                }
            //Close
            case 1:
                {
                    CloseWindow();
                    break;
                }
            //Histroy
            case 2:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_BATTLE_HISTORY), OnHistroyLoadCallBack);
                    break;
                }
            //Store
            case 3:
                {
                    break;
                }
            //Rule
            case 4:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_BATTLE_RULE), OnRuleLoadCallBack);
                    break;
                }
            //Submit
            case 5:
                {
                    break;
                }
        }
    }

    private void OnRuleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var temp = Instantiate(p_object) as GameObject;
    }

    private void OnHistroyLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        var temp = Instantiate(p_object) as GameObject;
    }

    void Awake()
    {
        m_EventIndexHandles.ForEach(item => item.m_Handle += OnEventClick);
    }

    void OnDestroy()
    {
        m_EventIndexHandles.ForEach(item => item.m_Handle -= OnEventClick);
    }
}
