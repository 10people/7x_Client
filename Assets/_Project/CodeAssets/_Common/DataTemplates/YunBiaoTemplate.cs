using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Object = UnityEngine.Object;

public class YunBiaoTemplate : XmlLoadManager
{
    public static string incomeAdd_startTime2 = "incomeAdd_startTime2";
    public static string incomeAdd_endTime2 = "incomeAdd_endTime2";
    public static string enemyCartBonus = "foeCart_incomeAdd_pro";

    public string m_Key;
    public string m_Desc;
    public string m_Value;

    public static List<YunBiaoTemplate> Templates = new List<YunBiaoTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "YunbiaoTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            Templates.Clear();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("YunbiaoTemp");

            if (!t_has_items)
            {
                break;
            }

            YunBiaoTemplate t_template = new YunBiaoTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.m_Key = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.m_Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.m_Value = t_reader.Value;
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static string GetValueByKey(string key)
    {
        var temp = Templates.Where(item => item.m_Key == key).ToList();
        return temp.Any() ? temp.First().m_Value : null;
    }
}
