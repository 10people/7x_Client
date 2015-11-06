using UnityEngine;
using System.Collections;

public class NoticeContentInfo : MonoBehaviour 
{
    public UILabel m_LabelContent;
    public void ShowContent(string content)
    {
        m_LabelContent.text = content;
    }
 
}
