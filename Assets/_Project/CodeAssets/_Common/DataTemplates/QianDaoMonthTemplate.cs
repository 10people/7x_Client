using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class QianDaoMonthTemplate : XmlLoadManager
{
    public int month;

    public string desc;

    public string name;

    public int icon;


 

    public static List<QianDaoMonthTemplate> templates = new List<QianDaoMonthTemplate>();


    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(XmlLoadManager.m_LoadPath + "QianDaoMonth.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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

            //			Debug.Log( "Text: " + t_text_asset.text );
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("QianDaoMonth");

            if (!t_has_items)
            {
                break;
            }

            QianDaoMonthTemplate t_template = new QianDaoMonthTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.month = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.desc = t_reader.Value;


                t_reader.MoveToNextAttribute();
                t_template.name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.icon = int.Parse(t_reader.Value);

            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static QianDaoMonthTemplate getQianDaoMonthTemplateByMonth(int month)
    {
        foreach (QianDaoMonthTemplate template in templates)
        {
            if (template.month == month)
            {
                return template;
            }
        }

        Debug.LogError("XML ERROR: Can't get DescIdTemplate with descId " + month);

        return null;
    }
}
