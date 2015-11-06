using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class XianshiControlTemp : XmlLoadManager
{
    public int id;
    public string Name;
    public string Desc;
    public string jinduDesc;
    public int Rank;
    public int status;
    public int doneType;
    public int languageId;
    public int StartTime;
    public int CloseTime;
    public int DelayTime;
    public int PicId;

    public static List<XianshiControlTemp> templates = new List<XianshiControlTemp>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "XianshiControl.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("XianshiControl");

            if (!t_has_items)
            {
                break;
            }

            XianshiControlTemp t_template = new XianshiControlTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.jinduDesc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Rank = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.status = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.doneType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.languageId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.StartTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.CloseTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.DelayTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.PicId = int.Parse(t_reader.Value);
            }
            templates.Add(t_template);
        }
        while (t_has_items);
    }
}
