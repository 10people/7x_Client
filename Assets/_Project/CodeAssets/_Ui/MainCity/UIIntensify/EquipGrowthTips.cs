using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipGrowthTips : MonoBehaviour
{

    private bool m_pressState;

    public GameObject m_tipsObject;
    public UISprite m_BackGround;

    public GameObject EquipInfo;
    public EventSuoHandle m_touchEvent;
    public UIFont titleFont;//标题字体
    public UIFont btn1Font;//按钮1字体
    public UIFont btn2Font;//按钮2字体
    public enum DataType
    {
        zero = 0,
        one,
        two,
        three
    };

    public DataType m_type;

    void Start()
    {
        if(m_touchEvent != null)
        m_touchEvent.m_Handle += TouchHide;
    }

    void TouchHide(int index,GameObject obj)
    {

        //Debug.Log("NOSUONOSUONOSUONOSUONOSUONOSUONOSUONOSUO");
            EquipSuoData.Instance().m_SuoAdd = true;
            obj.SetActive(false);
            transform.GetComponent<Collider>().enabled = true;
           // m_BackGround.GetComponent<UISprite>().enabled = true;
            m_BackGround.gameObject.SetActive(true);
            m_BackGround.gameObject.SetActive(true);
            EquipSuoData.Instance().es = EquipSuoData.Instance().m_EquipSuoInfo[EquipSuoData.Instance().m_EquipID];
            switch (index)
            {
                case 0:
                    {
                        EquipSuoData.Instance().es.oneSuo = false;
                    }
                    break;
                case 1:
                    {
                        EquipSuoData.Instance().es.twoSuo = false;
                    }
                    break;
                case 2:
                    {
                        EquipSuoData.Instance().es.threeSuo = false;
                    }
                    break;
                case 3:
                    {
                        EquipSuoData.Instance().es.fourSuo = false;
                    }
                    break;
                default:
                    break;
            }
            if (!EquipSuoData.Instance().es.oneSuo && !EquipSuoData.Instance().es.twoSuo && !EquipSuoData.Instance().es.threeSuo && !EquipSuoData.Instance().es.fourSuo)
            {
                EquipSuoData.Instance().m_EquipSuoInfo.Remove(EquipSuoData.Instance().m_EquipID);
            }
            else
            {
                EquipSuoData.Instance().m_EquipSuoInfo[EquipSuoData.Instance().m_EquipID] = EquipSuoData.Instance().es;
            }

           // EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().ShowExhibiteInfo();
    }
    public void UIBoxLoad(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;
        UIBox uibox = boxObj.GetComponent<UIBox>();
        string upLevelTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        string str = LanguageTemplate.GetText(LanguageTemplate.Text.VIP_SIGNAL_TAG) + NameIdTemplate.GetName_By_NameId(990019) + VipFuncOpenTemplate.GetNeedLevelByKey(5).ToString() + NameIdTemplate.GetName_By_NameId(990044);

        // string concelr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        uibox.setBox(upLevelTitleStr, MyColorData.getColorString(1, str), "", null, confirmStr, null, null, titleFont, btn1Font);

    }

    //void OnPress(bool tempPress) //按住0.5f后 显示tips
    //{
    //    m_pressState = tempPress;

    //    if (m_pressState)
    //    {
    //        StartCoroutine(ShowTips());
    //    }
    //    else
    //    {
    //      //  m_tipsObject.SetActive(false);
    //    }
    //}
    void OnClick()
    {
        ShowTips();
    }
    void ShowTips()
    {
        if (JunZhuData.Instance().m_junzhuInfo.vipLv >= VipFuncOpenTemplate.GetNeedLevelByKey(5))
        {
            if (EquipSuoData.Instance().m_WashIson && (int)m_type < 4)
            {
                transform.GetComponent<Collider>().enabled = false;
             //   m_BackGround.GetComponent<UISprite>().enabled = false;
                m_BackGround.gameObject.SetActive(false);
                m_touchEvent.gameObject.SetActive(true);
                EquipSuoData.Instance().es.oneSuo = false;
                EquipSuoData.Instance().es.twoSuo = false;
                EquipSuoData.Instance().es.threeSuo = false;
                EquipSuoData.Instance().es.fourSuo = false;

                if (!EquipSuoData.Instance().m_EquipSuoInfo.ContainsKey(EquipSuoData.Instance().m_EquipID))
                {
                    EquipSuoData.Instance().m_EquipSuoInfo.Add(EquipSuoData.Instance().m_EquipID, EquipSuoData.Instance().es);
                }
                else
                {
                    EquipSuoData.Instance().es = EquipSuoData.Instance().m_EquipSuoInfo[EquipSuoData.Instance().m_EquipID];
                }

                switch ((int)m_type)
                {
                    case 0:
                        {
                            EquipSuoData.Instance().es.oneSuo = true;
                        }
                        break;
                    case 1:
                        {
                            EquipSuoData.Instance().es.twoSuo = true;
                        }
                        break;
                    case 2:
                        {
                            EquipSuoData.Instance().es.threeSuo = true;
                        }
                        break;
                    case 3:
                        {
                            EquipSuoData.Instance().es.fourSuo = true;
                        }
                        break;
                    default:
                        break;
                }

                EquipSuoData.Instance().m_EquipSuoInfo[EquipSuoData.Instance().m_EquipID] = EquipSuoData.Instance().es;
                EquipSuoData.Instance().m_SuoAdd = true;
            }
          
            //else if(m_pressState)
            //{
            //    m_tipsObject.SetActive(true);

            //    m_tipsObject.GetComponent<EquipGrowthTipsShow>().ShowData((int)m_type);
            //}
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), UIBoxLoad);
        }
    //    EquipInfo.GetComponent<EquipGrowthEquipInfoManagerment>().ShowExhibiteInfo();
    }
}
