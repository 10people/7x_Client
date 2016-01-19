using UnityEngine;
using System.Collections;

public class ObjectName : MonoBehaviour {

    public GameObject m_ObjController;
	public UILabel m_playerName;
    public UILabel m_LabAllianceName;
    public UISprite m_SpriteChengHao;
    public UILabel m_playerVip;
    public UISprite m_SpriteVip;

    protected Transform m_transform;
	
	void Start()
	{
		m_transform = this.GetComponent<Transform>();
	}
	
	public void Init(string tempPlayerName,string tempAllianceName,string tempChengHao,string vip_level,int zhiwei)//初始化玩家名字
	{
		
        if (!string.IsNullOrEmpty(tempAllianceName))
        {
            m_LabAllianceName.text = MyColorData.getColorString(12, "<" + tempAllianceName + ">") + FunctionWindowsCreateManagerment.GetIdentityById(zhiwei);
        }
        else
        {
            m_LabAllianceName.text = MyColorData.getColorString(12, LanguageTemplate.GetText(LanguageTemplate.Text.NO_ALLIANCE_TEXT));
        }

        if (!string.IsNullOrEmpty(tempChengHao) && !tempChengHao.Equals("-1"))
        {
            m_SpriteChengHao.gameObject.SetActive(true);
            m_SpriteChengHao.spriteName = tempChengHao;
        }
        else
        {
          m_SpriteChengHao.gameObject.SetActive(false);
        }

   
        if (!string.IsNullOrEmpty(vip_level))
        {
            //if (int.Parse(vip_level) > 0)
            //{
            //    m_playerName.text = MyColorData.getColorString(1,"V" + vip_level.ToString()) +" "+ MyColorData.getColorString(9, "[b]" + tempPlayerName + "[/b]");
            //}
            //else
            //{
            //    m_playerName.text =  MyColorData.getColorString(9, "[b]" + tempPlayerName + "[/b]");
            //}
            if (int.Parse(vip_level) > 0)
            {
                m_SpriteVip.gameObject.SetActive(true);
                m_SpriteVip.spriteName = "vip" + vip_level;
                m_playerName.text = MyColorData.getColorString(9, "[b]" + tempPlayerName + "[/b]");
            }
            else
            {
                m_SpriteVip.gameObject.SetActive(false);
                m_playerName.text = MyColorData.getColorString(9, "[b]" + tempPlayerName + "[/b]");
            }
        }
        else
        {
           // m_playerName.alignment = NGUIText.Alignment.Automatic; 
            m_playerName.text = MyColorData.getColorString(9, "[b]" + tempPlayerName + "[/b]");
        }
        ShowOrHide(true);
	}


	public void ShowOrHide(bool tempState) //显示或隐藏玩家名字
	{
		m_playerName.gameObject.SetActive(tempState);
	}

    public void WetherHide(bool is_show)
    {
     //   m_playerName.gameObject.SetActive(is_show);
        m_LabAllianceName.gameObject.SetActive(is_show);
        m_SpriteChengHao.gameObject.SetActive(is_show);
    }
}
