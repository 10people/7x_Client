using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using qxmobile.protobuf;

public class KingDetailMibaoInfo : MonoBehaviour
{
    public List<MishuData> m_MibaoData = new List<MishuData>();
    public List<MishuData> m_MishuData = new List<MishuData>();

    public bool m_isMibaoPanel = true;
    public GameObject m_MiBaoPanel;
    public GameObject m_MiShuPanel;

    public MishuData m_MibaoMishuData;
    public MibaoInfoResp m_MibaoInfoResp;

    public void Switch()
    {
        if (m_isMibaoPanel)
        {
            m_isMibaoPanel = false;
            m_MiBaoPanel.SetActive(false);
            m_MiShuPanel.SetActive(true);
            setDataMishu();
        }
        else
        {
            m_isMibaoPanel = true;
            m_MiBaoPanel.SetActive(true);
            m_MiShuPanel.SetActive(false);
            setDataMiBao();
        }
    }

    public void setDataMiBao()
    {
        for (int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i++)
        {
            //m_MibaoData[i].m_ObjRed.SetActive(false);
            if (m_MibaoInfoResp.miBaoList[i].star == 0)
            {
                m_MibaoData[i].m_SpriteIcon.color = Color.black;
                m_MibaoData[i].m_SpritePinZhi.gameObject.SetActive(false);
                if (m_MibaoInfoResp.miBaoList[i].suiPianNum >= m_MibaoInfoResp.miBaoList[i].needSuipianNum)
                {
                    //m_MibaoData[i].m_ObjRed.SetActive(true);
                }
            }
            else
            {
                m_MibaoData[i].m_SpriteIcon.color = Color.white;
                m_MibaoData[i].m_SpritePinZhi.gameObject.SetActive(true);
                m_MibaoData[i].m_SpritePinZhi.spriteName = "inlaRound" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[i].tempId).color;
            }
            m_MibaoData[i].m_SpriteIcon.spriteName = "" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[i].tempId).iconID;
        }

        if (m_MibaoInfoResp.levelPoint == 9)
        {
            m_MibaoMishuData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[8].iconID;
            m_MibaoMishuData.m_SpriteIcon.color = Color.white;
        }
        else
        {
            m_MibaoMishuData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[m_MibaoInfoResp.levelPoint].iconID;
            m_MibaoMishuData.m_SpriteIcon.color = Color.gray;
        }
    }

    public void setDataMishu()
    {
        for (int i = 0; i < 9; i++)
        {
            m_MishuData[i].m_SpriteIcon.spriteName = "" + MishuTemplate.templates[i].iconID;
            if (i < m_MibaoInfoResp.levelPoint)
            {
                m_MishuData[i].m_SpriteIcon.color = Color.white;
                m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(true);
                int tempIndex = m_MibaoInfoResp.levelPoint;
                if (tempIndex == 9)
                {
                    tempIndex = 8;
                }

                m_MishuData[i].m_SpritePinZhi.spriteName = "inlaRound" + MishuTemplate.templates[tempIndex].color;
                m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(true);
            }
            else
            {
                m_MishuData[i].m_SpriteIcon.color = Color.black;
                m_MishuData[i].m_SpritePinZhi.gameObject.SetActive(false);
            }
        }
    }

    //public void ShowMiBao(int index)
    //{
    //    if (m_isMibaoPanel)
    //    {
    //        if (m_MibaoInfoResp.miBaoList[index].star == 0)
    //        {
    //            setMiBaoUpData(index);
    //        }
    //    }
    //    else
    //    {
    //        if (index < m_MibaoInfoResp.levelPoint)
    //        {

    //        }
    //        else if (index == m_MibaoInfoResp.levelPoint)
    //        {
    //            setMiShuUpData(index);
    //        }
    //    }
    //}

    //public void setMiBaoUpData(int index)
    //{
    //    if (m_MibaoInfoResp.miBaoList[index].star == 0)
    //    {
    //        m_MibaoUpObj.SetActive(true);
    //        m_MibaoUpName.text = MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).pinzhiName + "\n" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).showName;
    //        m_MibaoUpNeedLabel.text = "x" + m_MibaoInfoResp.miBaoList[index].needSuipianNum + " (拥有：" + m_MibaoInfoResp.miBaoList[index].suiPianNum + ")";

    //        m_MibaoUpData.m_SpriteIcon.spriteName = "" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).iconID;
    //        m_MibaoUpSuipianIcon.spriteName = "" + MibaoNewSuipianTemplate.GetTemplateByID(MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).suipianID).iconID;
    //        string tempString = "";
    //        bool nextLink = false;
    //        if (m_MibaoInfoResp.miBaoList[index].gongJi > 0)
    //        {
    //            tempString = "攻击提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].gongJi);
    //            nextLink = true;
    //        }
    //        if (m_MibaoInfoResp.miBaoList[index].fangYu > 0)
    //        {
    //            if (nextLink)
    //            {
    //                tempString += "\n";
    //            }
    //            tempString += "防御提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].fangYu);
    //            nextLink = true;
    //        }
    //        if (m_MibaoInfoResp.miBaoList[index].shengMing > 0)
    //        {
    //            if (nextLink)
    //            {
    //                tempString += "\n";
    //            }
    //            tempString += "生命提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].shengMing);
    //            nextLink = true;
    //        }
    //        m_MibaoUpDes.text = tempString;
    //        if (m_MibaoInfoResp.miBaoList[index].suiPianNum >= m_MibaoInfoResp.miBaoList[index].needSuipianNum)
    //        {
    //            m_MibaoUpButtonObj.SetActive(true);
    //            m_MibaoUpJihuoLabel.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            m_MibaoUpButtonObj.SetActive(false);
    //            m_MibaoUpJihuoLabel.gameObject.SetActive(true);
    //        }
    //    }
    //    else if (m_MibaoInfoResp.miBaoList[index].star == 1)
    //    {
    //        m_MibaoTipObj.SetActive(true);
    //        m_MibaoTipName.text = MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).pinzhiName + "\n" + MibaoNewTemplate.GetTemplateByID(m_MibaoInfoResp.miBaoList[index].tempId).showName;

    //        string tempString = "";
    //        bool nextLink = false;
    //        if (m_MibaoInfoResp.miBaoList[index].gongJi > 0)
    //        {
    //            tempString = "攻击提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].gongJi);
    //            nextLink = true;
    //        }
    //        if (m_MibaoInfoResp.miBaoList[index].fangYu > 0)
    //        {
    //            if (nextLink)
    //            {
    //                tempString += "\n";
    //            }
    //            tempString += "防御提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].fangYu);
    //            nextLink = true;
    //        }
    //        if (m_MibaoInfoResp.miBaoList[index].shengMing > 0)
    //        {
    //            if (nextLink)
    //            {
    //                tempString += "\n";
    //            }
    //            tempString += "生命提升：" + MyColorData.getColorString(4, m_MibaoInfoResp.miBaoList[index].shengMing);
    //            nextLink = true;
    //        }
    //        m_MibaoTipDes.text = tempString;
    //    }
    //}

    //public void setMiShuUpData(int index)
    //{
    //    if (index == 9)
    //    {
    //        index = 8;
    //    }

    //    m_MishuUpObj.SetActive(true);
    //    m_MishuUpName.text = MishuTemplate.templates[index].pinzhiName + "\n" + MishuTemplate.templates[index].name;

    //    m_MishuUpData.m_SpriteIcon.spriteName = "" + MishuTemplate.templates[index].iconID;

    //    string tempString = "";
    //    bool nextLink = false;
    //    if (MishuTemplate.templates[index].wqSH > 0)
    //    {
    //        tempString = "武器伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqSH);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].wqJM > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "武器伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqJM);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].wqBJ > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "武器暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqBJ);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].wqRX > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "武器暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].wqRX);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].jnSH > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "技能伤害加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnSH);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].jnJM > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "技能伤害抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnJM);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].jnBJ > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "技能暴击加深：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnBJ);
    //        nextLink = true;
    //    }
    //    if (MishuTemplate.templates[index].jnRX > 0)
    //    {
    //        if (nextLink)
    //        {
    //            tempString += "\n";
    //        }
    //        tempString += "技能暴击抵抗：" + MyColorData.getColorString(4, MishuTemplate.templates[index].jnRX);
    //        nextLink = true;
    //    }
    //    bool isUpMishu = true;
    //    for (int i = 0; i < m_MibaoInfoResp.miBaoList.Count; i++)
    //    {
    //        if (m_MibaoInfoResp.miBaoList[i].star == 0 && m_MibaoInfoResp.miBaoList[i].suiPianNum < m_MibaoInfoResp.miBaoList[i].needSuipianNum)
    //        {
    //            isUpMishu = false;
    //        }
    //    }
    //    m_MishuUpDes.text = tempString;
    //    if (isUpMishu)
    //    {
    //        m_MishuUpButtonObj.SetActive(true);
    //        m_MishuUpJihuoLabel.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        m_MishuUpButtonObj.SetActive(false);
    //        m_MishuUpJihuoLabel.gameObject.SetActive(true);
    //    }
    //}
}
