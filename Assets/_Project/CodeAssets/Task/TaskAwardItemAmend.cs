using UnityEngine;
using System.Collections;

public class TaskAwardItemAmend : MonoBehaviour
{
   // public UITexture  IconTexture;
    public UISprite m_AwardIcon;
    public UILabel LabCount;
 

//	private string PathTexture = "";
    
	public bool ShowX = false;

    Texture _tex;

    public UIAtlas m_Atlas;
    public UIAtlas m_Atlas_MiBao;
    public UIAtlas m_Atlas_FuShi;
    public void Show(string iconName,string Count,int type)
    {
       
        //0普通道具;2装备;3玉玦;4秘宝；5秘宝碎片；6进阶材料；9强化材料
        if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 5)
        {
            m_AwardIcon.atlas = m_Atlas;
            m_AwardIcon.spriteName = iconName;
        }
        else if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 4)
        {
            m_AwardIcon.atlas = m_Atlas_MiBao;
            m_AwardIcon.spriteName = iconName;
        }
        else if (CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 7 || CommonItemTemplate.getCommonItemTemplateById(int.Parse(iconName)).itemType == 8)
        {
            m_AwardIcon.atlas = m_Atlas_FuShi;
            m_AwardIcon.type = UISprite.Type.Simple;
            m_AwardIcon.spriteName = iconName;
        }
       else 
       {
         m_AwardIcon.spriteName = iconName;
       }
             
 

       if (!ShowX)
       {
           LabCount.text = "x" + Count;
       }
       else
       {
           LabCount.text =  Count;
       }
       
    }
}
