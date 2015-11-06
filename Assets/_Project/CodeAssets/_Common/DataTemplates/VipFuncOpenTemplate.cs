using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;


public class VipFuncOpenTemplate : XmlLoadManager 
{
    public int key;
    public string desc;
    public int needlv;



    public static List<VipFuncOpenTemplate> templates = new List<VipFuncOpenTemplate>();


    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "VipFuncOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("VipFuncOpen");

            if (!t_has_items)
            {
                break;
            }

            VipFuncOpenTemplate t_template = new VipFuncOpenTemplate();

            {

                t_reader.MoveToNextAttribute();
                t_template.key = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.desc =  t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.needlv = int.Parse(t_reader.Value);

              
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static int GetNeedLevelByKey(int key)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].key == key)
            {
                return templates[i].needlv;
            }
        }
        return 0;

    }
   
}
