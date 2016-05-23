using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;

public class LianmengJuanxianTemplate : XmlLoadManager
{
    public int id;

    public int name;

    public int type;

    public int awardShow1;

    public int awardShow2;

    public string award;
 
    public int tuTenglvNeeded;
 
    public static List<LianmengJuanxianTemplate> templates = new List<LianmengJuanxianTemplate>();
 

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianmengJuanxian.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("LianmengJuanxian");

            if (!t_has_items)
            {
                break;
            }

            LianmengJuanxianTemplate t_template = new LianmengJuanxianTemplate();

            t_reader.MoveToNextAttribute();
            t_template.id = int.Parse(t_reader.Value);

            t_reader.MoveToNextAttribute();
            t_template.name = int.Parse(t_reader.Value);

            t_reader.MoveToNextAttribute();
            t_template.type = int.Parse(t_reader.Value);
            t_reader.MoveToNextAttribute();
            t_template.awardShow1 = int.Parse(t_reader.Value);
            t_reader.MoveToNextAttribute();
            t_template.awardShow2 = int.Parse(t_reader.Value);
            t_reader.MoveToNextAttribute();
            t_template.award = t_reader.Value;

            t_reader.MoveToNextAttribute();
            t_template.tuTenglvNeeded = int.Parse(t_reader.Value);

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static LianmengJuanxianTemplate getJuanxianById(int id)
    {
        foreach (LianmengJuanxianTemplate template in templates)
        {
            if (template.id == id)
            {
                return template;
            }
        }
        Debug.LogWarning("XML ERROR: Can't get ZhuangBei with id " + id);
        return null;
    }
}
