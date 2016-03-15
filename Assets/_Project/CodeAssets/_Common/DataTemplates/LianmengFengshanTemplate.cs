using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class LianmengFengshanTemplate : XmlLoadManager
{
      
    public int id;
    public string name;
    public string desc1;
    public string desc2;
    public string desc3;
    public int huoyuedu;
    public string award;
    public int tuTenglvNeeded;

    public static List<LianmengFengshanTemplate> templates = new List<LianmengFengshanTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LianmengFengshan.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("LianmengFengshan");

            if (!t_has_items)
            {
                break;
            }

            LianmengFengshanTemplate t_template = new LianmengFengshanTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);


                t_reader.MoveToNextAttribute();
                t_template.name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc1 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc2 = t_reader.Value;


                t_reader.MoveToNextAttribute();
                t_template.desc3 = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.huoyuedu = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.award = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.tuTenglvNeeded = int.Parse(t_reader.Value);

            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }
    public static LianmengFengshanTemplate GetFengshanById(int id)
    {
        foreach (LianmengFengshanTemplate _template in templates)
        {
            if (_template.id == id)
            {
                return _template;
            }
        }

        return null;
    }
}
