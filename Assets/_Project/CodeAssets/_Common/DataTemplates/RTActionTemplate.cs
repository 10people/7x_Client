using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public class RTActionTemplate : XmlLoadManager
{
    public int Id;
    public int TypeKey;
    public int Param1;
    public int Param2;
    public int Param3;
    public int Param4;
    public int Param5;
    public int TSR;
    public int TTR;
    public string CeOnHit;
    public int CsOnHit;
    public int Prob;

    public static List<RTActionTemplate> templates = new List<RTActionTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Action.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("Action");

            if (!t_has_items)
            {
                break;
            }

            RTActionTemplate t_template = new RTActionTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.Id = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.TypeKey = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Param1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Param2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Param3 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Param4 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Param5 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.TSR = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.TTR = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.CeOnHit = t_reader.Value == "0" ? "" : t_reader.Value;
                t_reader.MoveToNextAttribute();
                t_template.CsOnHit = t_reader.Value == "" ? 0 : int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Prob = int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static RTActionTemplate GetTemplateByID(int id)
    {
        var temp = templates.Where(item => item.Id == id).ToList();

        if (temp.Any())
        {
            return temp.First();
        }
        else
        {
            return null;
        }
    }
}
