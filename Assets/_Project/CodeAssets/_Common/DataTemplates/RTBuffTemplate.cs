using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class RTBuffTemplate : XmlLoadManager
{
    public int BuffId;
    public bool IsDebuff;
    public int Attr_1;
    public int Attr_1_P1;
    public int Attr_1_P2;
    public int Attr_2;
    public int Attr_2_P1;
    public int Attr_2_P2;
    public int Attr_3;
    public int Attr_3_P1;
    public int Attr_3_P2;
    public int BuffDisplay;
    public int BuffDuration;

    public static List<RTBuffTemplate> templates = new List<RTBuffTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Buff.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            templates.Clear();
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
            t_has_items = t_reader.ReadToFollowing("Buff");

            if (!t_has_items)
            {
                break;
            }

            RTBuffTemplate t_template = new RTBuffTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.BuffId = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.IsDebuff = (int.Parse(t_reader.Value) == 1);
                t_reader.MoveToNextAttribute();
                t_template.Attr_1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_1_P1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_1_P2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_2_P1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_2_P2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_3 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_3_P1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Attr_3_P2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.BuffDisplay = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.BuffDuration = int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }
}
