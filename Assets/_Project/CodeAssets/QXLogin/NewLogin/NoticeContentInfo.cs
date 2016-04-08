using UnityEngine;
using System.Collections;

public class NoticeContentInfo : MonoBehaviour 
{
    public UILabel m_LabelContent;
    public void ShowContent(NoticeManager.AnnounceInfo info)
    {
        if (info.aligment == 0)
        {
            m_LabelContent.alignment = NGUIText.Alignment.Left;
        }
        else if (info.aligment == 1)
        {
            m_LabelContent.alignment = NGUIText.Alignment.Center;
        }
        m_LabelContent.text = info.content;
    }
 
}
