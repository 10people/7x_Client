using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

public class XianshiHuodongTemp : XmlLoadManager
{
    public int id;
    public int BigId;
    public string desc;
    public int doneType;
    public string doneCondition;
    public int LimitTime;
    public string Award;

    public static List<XianshiHuodongTemp> templates = new List<XianshiHuodongTemp>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "XianshiHuodong.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("XianshiHuodong");

            if (!t_has_items)
            {
                break;
            }

            XianshiHuodongTemp t_template = new XianshiHuodongTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.BigId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.doneType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.doneCondition = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.LimitTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Award = t_reader.Value;
            }
            templates.Add(t_template);
        }
        while (t_has_items);
    }

	public static XianshiHuodongTemp GetXianShiHuoDongById(int id)
    {
 
		foreach (XianshiHuodongTemp _template in templates)
        {
            if (_template.id == id)
            {
                return _template;
            }
        }
        return null;
    }

   
}
