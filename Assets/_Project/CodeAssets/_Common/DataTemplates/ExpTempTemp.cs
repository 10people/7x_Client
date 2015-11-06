using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class ExpTempTemp : XmlLoadManager
{
    public int id;
    public int expId;
    public int level;
    public int needExp;
    public static List<ExpTempTemp> templates = new List<ExpTempTemp>();
 
 

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ExpTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);

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
            t_has_items = t_reader.ReadToFollowing("ExpTemp");

            if (!t_has_items)
            {
                break;
            }

            ExpTempTemp t_template = new ExpTempTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.expId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.level = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.needExp = int.Parse(t_reader.Value);
 
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static List<ExpTempTemp> GetEquipUpgradeInfo(int expId)
    {
        List<ExpTempTemp> list = new List<ExpTempTemp>();
        for (int i = 0; i < templates.Count; i++)
        {
            ExpTempTemp t_item = templates[i];

            if (t_item.expId == expId)
            {
                list.Add(t_item);
            }
        }

       

        return list;
    }
}
