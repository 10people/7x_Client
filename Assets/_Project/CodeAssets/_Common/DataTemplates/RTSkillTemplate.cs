using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class RTSkillTemplate : XmlLoadManager
{
    public int SkillId;
    public int SkillTarget;
    public int EffectTarget;
    public int ST_TypeRejectU;
    public int CRRejectU;
    public int Range_Min;
    public int Range_Max;
    public int BaseCD;
    public int IsInGCD;
    public int Action1;

    public static List<RTSkillTemplate> templates = new List<RTSkillTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Skill.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("Skill");

            if (!t_has_items)
            {
                break;
            }

            RTSkillTemplate t_template = new RTSkillTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.SkillId = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.SkillTarget = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.EffectTarget = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.ST_TypeRejectU = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.CRRejectU = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Range_Min = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Range_Max = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.BaseCD = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.IsInGCD = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Action1 = int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }
}
