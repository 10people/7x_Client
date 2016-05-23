using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Object = UnityEngine.Object;

public class LMZBuildingTemplate : XmlLoadManager
{
    public int Id;
    public int Type;
    public int Side;

    public Vector2 Position = new Vector2();
    public float Rotation;

    public int ZhanlingzhiMax;
    public int ZhanlingzhiAdd;
    public int ScoreAdd;
    public int Radius;
    public float Scale;

    public static List<LMZBuildingTemplate> Templates = new List<LMZBuildingTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LMZBuildingTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("LMZBuildingTemp");

            if (!t_has_items)
            {
                break;
            }

            LMZBuildingTemplate t_template = new LMZBuildingTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.Id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Side = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                var x = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Position = new Vector2(x, int.Parse(t_reader.Value));

                t_reader.MoveToNextAttribute();
                t_template.Rotation = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ZhanlingzhiMax = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ZhanlingzhiAdd = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Radius = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Scale = float.Parse(t_reader.Value);
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static List<LMZBuildingTemplate> GetTemplates(int type, int side)
    {
        return Templates.Where(item => item.Type == type && item.Side == side).ToList();
    }

    public static List<LMZBuildingTemplate> GetTemplatesByType(int type)
    {
        return Templates.Where(item => item.Type == type).ToList();
    }

    public static List<LMZBuildingTemplate> GetTemplatesBySide(int side)
    {
        return Templates.Where(item => item.Side == side).ToList();
    }
}
