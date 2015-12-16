using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using qxmobile;
using qxmobile.protobuf;
public class LianMengTuTengTemplate :XmlLoadManager
{ 
    public int id;
    public int tuTengLevel;
    public int alliance_lv_needed;
    public int tuTeng_lvUp_value;
    public int moBaiTimes1;
    public string award1;
    public int moBaiTimes2;
    public string award2;
    public int moBaiTimes3;
    public string award3;
 

    public static List<LianMengTuTengTemplate> templates = new List<LianMengTuTengTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianMengTuTeng.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("LianMengTuTeng");

            if (!t_has_items)
            {
                break;
            }

            LianMengTuTengTemplate t_template = new LianMengTuTengTemplate();
            {
 
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.tuTengLevel = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.alliance_lv_needed = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.tuTeng_lvUp_value = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.moBaiTimes1 = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.award1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.moBaiTimes2 = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.award2 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.moBaiTimes3 = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.award3 = t_reader.Value;
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static LianMengTuTengTemplate getTuTengAwardByLevel(int tuTengLevel)
    {
        Debug.Log("templatestemplatestemplatestemplates ::" + templates.Count);
        foreach (LianMengTuTengTemplate template in templates)
        {
            Debug.Log("template.tuTengLeveltemplate.tuTengLevel ::" + template.tuTengLevel);
            Debug.Log("tuTengLeveltuTengLeveltuTengLeveltuTengLevel ::" + template.tuTengLevel);
            if (template.tuTengLevel == tuTengLevel)
            {
                Debug.Log("templatetemplatetemplatetemplate ::" + template.award1);
                return template;
            }
        }

       // Debug.LogWarning("XML ERROR: Can't get ZhuangBei with id " + tuTengLevel);

        return null;
    }
}
