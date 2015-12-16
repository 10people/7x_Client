using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;
public class QiriQiandaoTemplate :XmlLoadManager
{
    public int id;
    public string bigItem;
    public int bigItemType;
    public string bigTitle1;
    public string detail1;
    public string bigTitle2;
    public string desc3;
    public string award;
    public string tomorrowDesc;

    public static List<QiriQiandaoTemplate> templates = new List<QiriQiandaoTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "QiriQiandaoTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("QiriQiandaoTemp");

            if (!t_has_items)
            {
                break;
            }

            QiriQiandaoTemplate t_template = new QiriQiandaoTemplate();
            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.bigItem = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.bigItemType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.bigTitle1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.detail1 = t_reader.Value;
 
                t_reader.MoveToNextAttribute();
                t_template.bigTitle2 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc3 =  t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.award = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.tomorrowDesc = t_reader.Value;
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static QiriQiandaoTemplate getZhuangBeiById(int id)
    {
        foreach (QiriQiandaoTemplate template in templates)
        {
            if (template.id == id)
            {
                return template;
            }
        }

        Debug.LogWarning("XML ERROR: Can't get ZhuangBei with id " + id);

        return null;
    }
}