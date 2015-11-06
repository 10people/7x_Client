using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class FangWuInfoTemplate : XmlLoadManager
{
    public int id;
    public int type;
    public string name;
    public string description1;
    public string description2;
    public string description3;
    public string description_noOwner1;
    public string description_noOwner2;
    public string description_noOwner3;

    public static List<FangWuInfoTemplate> templates = new List<FangWuInfoTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FangWuInformation.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("FangWuInformation");

            if (!t_has_items)
            {
                break;
            }

            FangWuInfoTemplate t_template = new FangWuInfoTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description2 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description3 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description_noOwner1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description_noOwner2 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.description_noOwner3 = t_reader.Value;
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static FangWuInfoTemplate GetFangWuInfoTemplateByNameId(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template;
            }
        }

        Debug.LogError("XML ERROR: Can't get FangWuInfoTemplate with FangWuInformation " + tempId);

        return null;
    }


    public static string GetDescriptionById(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template.description1 + template.description2 + template.description3;
            }
        }
        return null;
    }

    public static string GetNoConsDescriptionById(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template.description1 + template.description3;
            }
        }
        return null;
    }

    public static string GetNoOwnerDescriptionById(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template.description_noOwner1 + template.description_noOwner2 + template.description_noOwner3;
            }
        }
        return null;
    }

    public static string GetNameById(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template.name;
            }
        }
        return null;
    }

    public static int GetTypeById(int tempId)
    {
        foreach (FangWuInfoTemplate template in templates)
        {
            if (template.id == tempId)
            {
                return template.type;
            }
        }
        return -1;
    }
}

