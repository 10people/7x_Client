using UnityEngine;
using System.Collections;

public class TaskAwardItem : MonoBehaviour {

	public UITexture m_awardIcon;

	public UILabel m_label;

	public void InitWithItem()
	{
        m_label.text = "";
	}
}
