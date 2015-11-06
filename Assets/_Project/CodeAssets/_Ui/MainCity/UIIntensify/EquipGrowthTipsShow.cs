using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EquipGrowthTipsShow : MonoBehaviour
{

    public UISprite m_icon;

    public UILabel m_dec;
    public bool isOpen;


    public void ShowData(int tempType) //君主属性tips
    {
        string[] des = { "增加君主的物理伤害和物理减免", "增加君主的元素伤害和元素减免", "增加君主的技能伤害和技能减免", "增加君主的总生命", "增加君主的防御力", "增加君主的攻击力", "装备的高级属性在洗练后出现" };

       string[] allName = { "wu_1", "you_1", "zhi_1", "xue_1", "fang_1", "gong_1", "unknow_1" };

       if (isOpen && tempType < 3)
       {
           m_icon.spriteName = allName[6];

           m_dec.text = des[6];
       }
       else
       {
           m_icon.spriteName = allName[tempType];

           m_dec.text = des[tempType];
       }
    }
}
