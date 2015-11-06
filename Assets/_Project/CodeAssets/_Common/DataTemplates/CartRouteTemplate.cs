using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Object = UnityEngine.Object;

public class CartRouteTemplate : XmlLoadManager
{
    public int Id;

    public List<Vector2> Position = new List<Vector2>();

    public static List<CartRouteTemplate> Templates = new List<CartRouteTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "CartRoute.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            Templates.Clear();
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
            t_has_items = t_reader.ReadToFollowing("CartRoute");

            if (!t_has_items)
            {
                break;
            }

            CartRouteTemplate t_template = new CartRouteTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.Id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                string[] splited = t_reader.Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splited.Count(); i++)
                {
                    string[] splited2 = splited[i].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    t_template.Position.Add(new Vector2(float.Parse(splited2[0]), float.Parse(splited2[1])));
                }
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }
}
