using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AllianceEffigyManagerment : MonoBehaviour
{
    public NpcObjectItem m_Itemtemplate;
    public UILabel m_LabelName;
    public static int m_EffigyRoleId = 0;
    public static bool m_EffigyInfoGet = false;
    public List<GameObject> m_listEffigy;
    void Start()
    {
        //        if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0)
        //        {
        //          
        //        }
        SetNpcInfo();
    }
    void Update()
    {
        if (m_EffigyInfoGet && m_listEffigy.Count == 4)
        {
            m_EffigyInfoGet = false;
            ShowEffigy(m_EffigyRoleId);
        }

    }

    void ShowEffigy(int index)
    {
        //switch (index - 1)
        //{
        //    case 0:
        //        {
        //            m_listEffigy[index - 1].SetActive(true);
        //        }
        //        break;
        //    case 1:
        //        {
        //            m_listEffigy[index - 1].SetActive(true);
        //        }
        //        break;
        //    case 2:
        //        {

        //        }
        //        break;
        //    case 3:
        //        {

        //        }
        //        break;
        //    default:
        //        break;
        //}
        //if (index == 2 || index == 1)
        //{
        //    m_listEffigy[1].SetActive(true);
        //    m_listEffigy[0].SetActive(false);
        //}
        //else
        //{
        //    m_listEffigy[1].SetActive(false);
        //    m_listEffigy[0].SetActive(true);
        //}
        //sDebug.Log("index - 1index - 1index - 1index - 1index - 1")
        Effigy(index - 1);
    }

    void Effigy(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (index == i)
            {
                m_listEffigy[i].SetActive(true);
            }
            else
            {
                m_listEffigy[i].SetActive(false);
            }
        }

    }
    void LateUpdate()
    { 
    
    
    }
    void SetNpcInfo()
    {
        m_Itemtemplate.m_template = NpcCityTemplate.GetNpcItemById(10000);

     //   m_LabelName.text =MyColorData.getColorString(4, NameIdTemplate.GetName_By_NameId(NpcCityTemplate.GetNpcItemById(10000).m_npcName));
        NpcManager.m_NpcManager.m_npcObjectItemDic.Add(10000, m_Itemtemplate);
    }
}
