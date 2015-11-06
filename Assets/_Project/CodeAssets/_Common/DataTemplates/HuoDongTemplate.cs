using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuoDongTemplate : XmlLoadManager 
{

    public int id;
    public string title;
    public string desc;
    public string awardDesc;
    public int icon;
    public int type;
    public int buttonColor;
    public string buttonTitle;
    public string buttonTitleComplete;
    public int buttonCompleteTouch;
    public int state;

    public static List<HuoDongTemplate> templates = new List<HuoDongTemplate>();


    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(XmlLoadManager.m_LoadPath + "HuoDong.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("HuoDong");

            if (!t_has_items)
            {
                break;
            }

            HuoDongTemplate t_template = new HuoDongTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.title = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.awardDesc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.icon = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.buttonColor = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.buttonTitle = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.buttonTitleComplete = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.buttonCompleteTouch = int.Parse(t_reader.Value);

            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }
}
