using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class RenWuTemplate : XmlLoadManager
{
    public int id;

    public int m_name;
 
    public int funDesc;

    public int type;

    public int condition;

    public string jiangli;

    public int triggerType = 3;

    public string icon;

    public int progress;

    public string LmGongxia;

    public int frontId;

    public int LinkNpcId;

    public int FunctionId;
    public string Title;
    public string Script;

    public string huoyue;

    public string award;
    public int site;
    public int funcID;
    public string doneTitle;

    public static List<RenWuTemplate> templates = new List<RenWuTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "RenWu.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("RenWu");

            if (!t_has_items)
            {
                break;
            }

            RenWuTemplate t_template = new RenWuTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_name = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.funDesc = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.condition = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jiangli = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.LmGongxia = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.icon = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.frontId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.LinkNpcId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.FunctionId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Title = t_reader.Value;

				//t_reader.MoveToNextAttribute();
				//t_template.m_sSprite = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Script = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.huoyue =  t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.award = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.site = int.Parse(t_reader.Value);

          
                t_reader.MoveToNextAttribute();
                t_template.funcID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.doneTitle =  t_reader.Value;
}

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static RenWuTemplate GetRenWuById(int tempNum)
    {
        foreach (RenWuTemplate _template in templates)
        {
            if (_template.id == tempNum)
            {
                return _template;
            }
        }

        return null;
    }

    public static string GetAllianceRewardById(int id)
    {
        foreach (RenWuTemplate _template in templates)
        {
            if (_template.id == id)
            {
                return _template.LmGongxia;
            }
        }

        return null;
    }

    public static bool GetWetherContainId(int id)
    {
        foreach (RenWuTemplate _template in templates)
        {
            if (_template.id == id)
            {
                return true;
            }
        }
        return false;
    }
}
