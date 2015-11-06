using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class ZBChushiDiaoluoTemp : XmlLoadManager
{
    public int id;

    public string content;

    public static List<ZBChushiDiaoluoTemp> tempXiLian = new List<ZBChushiDiaoluoTemp>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ZBChushiDiaoluo.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            tempXiLian.Clear();
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
            t_has_items = t_reader.ReadToFollowing("ZBChushiDiaoluo");

            if (!t_has_items)
            {
                break;
            }

            ZBChushiDiaoluoTemp t_template = new ZBChushiDiaoluoTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.content = t_reader.Value;
                tempXiLian.Add(t_template);
            }
        }
        while (t_has_items);
    }

    public static string GetTemplateById(int id)
    {
        foreach (ZBChushiDiaoluoTemp template in tempXiLian)
        {
            if (template.id == id)
            {
              return template.content;
            }
        }
        return null;
    }

    
}
