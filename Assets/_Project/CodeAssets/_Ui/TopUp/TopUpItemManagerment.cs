using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopUpItemManagerment : MonoBehaviour 
{
    public List<UISprite> m_ListSprite = new List<UISprite>();
    public List<UILabel> m_ListLab = new List<UILabel>();
    private int saveID = 0;
    private int saveCost = 0; 
    public delegate void OnClick_Touch(int id,int consume);
    OnClick_Touch CallBackTouch;
	void Start () 
    {
	 
	}

    void OnClick()
    {
        CallBackTouch(saveID, saveCost);
    }

    public void ShowInfo(int id,int type,string des,string cost,string earn ,int times,OnClick_Touch callback)
    {
      saveCost = int.Parse(cost);
      saveID = id;
      CallBackTouch = callback;
      m_ListLab[1].text = MyColorData.getColorString(41,"[b]" + earn + NameIdTemplate.GetName_By_NameId(900002) + "[/b]");
      m_ListLab[2].text =MyColorData.getColorString(42 ,"[b]" +  cost + NameIdTemplate.GetName_By_NameId(990046) + "[/b]"); ;
      if (type == 1 && times == 0)
      {
          m_ListLab[0].text = MyColorData.getColorString(43, "[b]" + des + "[/b]");
          m_ListSprite[0].gameObject.SetActive(true);
      }
      else
      {
          m_ListLab[0].text = MyColorData.getColorString(43, "[b]" + des + "[/b]");
      }
      if (id > 6)
      {
          m_ListSprite[1].spriteName = "Top_Up_YuanBao_6";
      }
      else
      {
          m_ListSprite[1].spriteName = "Top_Up_YuanBao_" + id.ToString();
      }
    }
}
