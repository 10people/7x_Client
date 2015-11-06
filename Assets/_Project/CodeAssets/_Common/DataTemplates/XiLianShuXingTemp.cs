using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class XiLianShuXingTemp : XmlLoadManager
{
    public int ZhuangbeiID;

    public string Shuxing1;

    public string Shuxing2 ;

    public string Shuxing3;
    public string Shuxing4;

    public static List<XiLianShuXingTemp> tempXiLian = new List<XiLianShuXingTemp>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "XilianShuxing.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            tempXiLian.Clear();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));

            //			Debug.Log( "Text: " + t_text_asset.text );
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("XilianShuxing");

            if (!t_has_items)
            {
                break;
            }

            XiLianShuXingTemp t_template = new XiLianShuXingTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.ZhuangbeiID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Shuxing1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Shuxing2 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Shuxing3 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Shuxing4 = t_reader.Value;


                tempXiLian.Add(t_template);
            }
        }
        while (t_has_items);
    }

    public static string GetTemplateById(int id)
    {
        foreach (XiLianShuXingTemp template in tempXiLian)
        {
            if (template.ZhuangbeiID == id)
            {
                if (!string.IsNullOrEmpty(ShuXingInfo(template.Shuxing1)))
                {
                    return ShuXingInfo(template.Shuxing1) + ":" + ShuXingInfo(template.Shuxing2) + ":" + ShuXingInfo(template.Shuxing3) + ":" + ShuXingInfo(template.Shuxing4);
                }
                else
                {
                    return null;
                }
            }
        }
        return null;
    }

    static string ShuXingInfo(string shuxin)
    {
        string str = "";
        switch (shuxin)
        {
            case "A"://	武器伤害加深  
                {
                    str = "0";
                }
                break;
            case "B"://	武器伤害抵抗  
                {
                    str = "1";
                }
                break;
            case "C"://	武器暴击加深   
                {
                    str = "2";
                }
                break;
            case "D"://	武器暴击抵抗   
                {
                    str = "3";
                }
                break;
            case "E"://	技能伤害加深   
                {
                    str = "4";
                }
                break;
            case "F"://	技能伤害抵抗 
                {
                    str = "5";
                }
                break;
            case "G"://	技能暴击加深  
                {
                    str = "6";
                }
                break;
            case "H"://	技能暴击抵抗  
                {
                    str = "7";
                }
                break;
            case "O"://	武器暴击率   
                {
                    str = "8";
                }
                break;
            case "P":// 	技能暴击率  
                {
                    str = "9";
                }
                break;
            case "Q"://	 武器免暴率 
                {
                    str = "10";
                }
                break;
            case "R"://	技能免暴率  
                {
                    str = "11";
                }
                break;
            case "S"://	技能冷却缩减 
                {
                    str = "12";
                }
                break;
            default:
                break;
        }
        return str;
    }
}
