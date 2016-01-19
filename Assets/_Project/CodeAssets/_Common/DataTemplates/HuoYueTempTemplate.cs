using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class HuoYueTempTemplate : XmlLoadManager
{
   // <HuoYueTemp id = "1" type="0" needNum="20" award="0:900001:1000#0:900002:10" />
    //<RenWu id="100001" name="710001" funDesc="710001" type="1" condition="1" jiangli="9:900001:1000#9:900006:400#10:9101:1" />

    public int id;

    public int type;
 

    public int needNum;
 

    public string award;
 
    public static List<HuoYueTempTemplate> templates = new List<HuoYueTempTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuoYueTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("HuoYueTemp");

            if (!t_has_items)
            {
                break;
            }

            HuoYueTempTemplate t_template = new HuoYueTempTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.needNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.award = t_reader.Value;
 
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static HuoYueTempTemplate GetHuoYueTempById(int id)
    {
        foreach (HuoYueTempTemplate _template in templates)
        {
            if (_template.id == id)
            {
                return _template;
            }
        }

        return null;
    }
}
