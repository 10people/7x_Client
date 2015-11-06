using UnityEngine;
using System.Collections;

public class WorshipItemInfoManagerment : MonoBehaviour
{
    public UISprite m_SpriteIcon;
    public UILabel m_LabIcon;
    public UIAtlas m_Atlas_Pieces;
    public UIAtlas m_Atlas_MiBao;
	void Start ()
    {
	
	}
    public void ShowInfo(string icon,string count,UISprite popback,UILabel poplab)
    {
        transform.GetComponent<TipsManagerment>().PopFrameSprite = popback;
        transform.GetComponent<TipsManagerment>().PopTextLabel = poplab;
        transform.GetComponent<TipsManagerment>().ShowType = 1;
        if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(icon)).itemType == 5)
        {
            m_SpriteIcon.atlas = m_Atlas_Pieces;
            m_SpriteIcon.spriteName = icon;
        }
        else if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(icon)).itemType == 4)
        {
            m_SpriteIcon.atlas = m_Atlas_MiBao;
            m_SpriteIcon.spriteName = icon;
        }
        else
        {
            m_SpriteIcon.spriteName = icon;
        }
        //m_SpriteIcon.spriteName = icon;
        m_LabIcon.text = "x" + count;
    }
   
	 
}
