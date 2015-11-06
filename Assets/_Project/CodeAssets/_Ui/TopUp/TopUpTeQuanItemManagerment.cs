using UnityEngine;
using System.Collections;

public class TopUpTeQuanItemManagerment : MonoBehaviour 
{
    public UILabel m_LabContent;
    public UIScrollView m_ScrollView;
    public UIScrollBar m_ScrollBar;
	void Start () 
    {
	
	}
    public void ShowInfo(string content)
    {
        m_ScrollView.verticalScrollBar = m_ScrollBar;
        m_LabContent.text = content;
       m_ScrollBar.value = m_ScrollView.verticalScrollBar.value;
       m_ScrollView.UpdateScrollbars(true);

    }
	


}
