using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

public class RTSkillTemplate : XmlLoadManager
{
    public int SkillId;
    public int SkillTarget;
    public int EffectTarget;
    public int ET_P1;
    public int ET_P2;
    public int ST_TypeRejectU;
    public int CRRejectU;
    public float Range_Min;
    public float Range_Max;
    public float BaseCD;
    public int IsInGCD;
    public int Action1;
    public int Action2;
    public string CsOnShot;
    public int EsOnShot;
    public int EsTrack;
    public int EsOnTrack;
    public int EsSpeed;
    public int TowardType;
    public int SelfCast;

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
                t_template.ET_P1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.ET_P2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.ST_TypeRejectU = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.CRRejectU = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Range_Min = float.Parse(t_reader.Value) / 100;
                t_reader.MoveToNextAttribute();
                t_template.Range_Max = float.Parse(t_reader.Value) / 100;
                t_reader.MoveToNextAttribute();
                t_template.BaseCD = float.Parse(t_reader.Value) / 1000;
                t_reader.MoveToNextAttribute();
                t_template.IsInGCD = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Action1 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.Action2 = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.CsOnShot = t_reader.Value;
                t_reader.MoveToNextAttribute();
                t_template.EsOnShot = t_reader.Value == "" ? 0 : int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.EsTrack = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.EsOnTrack = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.EsSpeed = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.TowardType = int.Parse(t_reader.Value);
                t_reader.MoveToNextAttribute();
                t_template.SelfCast = int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static RTSkillTemplate GetTemplateByID(int p_ID)
    {
        var temp = templates.Where(item => item.SkillId == p_ID).ToList();

        if (temp != null && temp.Any())
        {
            return temp.First();
        }
        else
        {
            Debug.LogError("No RTSkillTemplate found by id: " + p_ID);

            return null;
        }
    }
}
