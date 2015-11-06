using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ShuXingTemp : XmlLoadManager
{
    public int id;
    public int shuxingName;
    public static List<ShuXingTemp> templates = new List<ShuXingTemp>();
 
    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Shuxing.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);

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
            t_has_items = t_reader.ReadToFollowing("Shuxing");

            if (!t_has_items)
            {
                break;
            }

            ShuXingTemp t_template = new ShuXingTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.shuxingName = int.Parse(t_reader.Value);
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static string GetShuXingName(int id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            ShuXingTemp t_item = templates[i];
            if (t_item.id == id)
            {
                return NameIdTemplate.GetName_By_NameId(t_item.shuxingName);
            }
        }
        return null;
    }
}
