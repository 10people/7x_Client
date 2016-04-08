using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FangWuTemplate : XmlLoadManager
{
    public int id;
    public int type;
    public int level;
    public int exp;
    public int allianceLevel;
    public int produceSpeed;
    public int produceLimit;
    public int waiGuanID;
    public int buildNum;
    public int needNum;
    public int addNum;
    public int coolTime;
    public int value;

    public static List<FangWuTemplate> templates = new List<FangWuTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FangWu.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("FangWu");

            if (!t_has_items)
            {
                break;
            }

            FangWuTemplate t_template = new FangWuTemplate();

            {

                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.level = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.exp = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.allianceLevel = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.produceSpeed = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.produceLimit = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.waiGuanID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.buildNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.needNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.addNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.coolTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.value = int.Parse(t_reader.Value);

            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static FangWuTemplate GetFangWuTemplateByLevel(int level)
    {
        foreach (FangWuTemplate template in templates)
        {
            if (template.level == level)
            {
                return template;
            }
        }

        Debug.LogError("XML ERROR: Can't get FangWuTemplate with level " + level);

        return null;
    }
}