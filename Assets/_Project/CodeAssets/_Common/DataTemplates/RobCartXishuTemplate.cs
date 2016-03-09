using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Object = UnityEngine.Object;

public class RobCartXishuTemplate : XmlLoadManager
{
    public int m_Scale;
    public float m_Xishu;

    public static List<RobCartXishuTemplate> Templates = new List<RobCartXishuTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "RobCartXishuTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("RobCartXishu");

            if (!t_has_items)
            {
                break;
            }

            RobCartXishuTemplate t_template = new RobCartXishuTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.m_Scale = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_Xishu = float.Parse(t_reader.Value);
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static float GetXishuByScale(int scale)
    {
        var temp = Templates.Where(item => item.m_Scale == scale).ToList();
        return temp.Any() ? temp.First().m_Xishu : -1f;
    }
}
