using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class JiangLiTemplate : XmlLoadManager
{
    public int id;
    public string name;
    public string description;
    public int awardType;
    public string item;
    public int yuanBao;
    public int time;
    public int day;

    public static List<JiangLiTemplate> templates = new List<JiangLiTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Jiangli.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("Jiangli");

            if (!t_has_items)
            {
                break;
            }

            JiangLiTemplate t_template = new JiangLiTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.awardType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.item = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.yuanBao = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.time = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.day = int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }
}