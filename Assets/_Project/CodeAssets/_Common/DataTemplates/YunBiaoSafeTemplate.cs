using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Object = UnityEngine.Object;

public class YunBiaoSafeTemplate : XmlLoadManager
{
    public struct SafeArea
    {
        public int ID;
        public Vector2 AreaPos;
        public float AreaRadius;
        public float RotateY;
    }

    public SafeArea m_SafeArea;

    public static List<YunBiaoSafeTemplate> Templates = new List<YunBiaoSafeTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "YunBiaoSafe.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            Templates.Clear();
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
            t_has_items = t_reader.ReadToFollowing("YunBiaoSafe");

            if (!t_has_items)
            {
                break;
            }

            YunBiaoSafeTemplate t_template = new YunBiaoSafeTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.m_SafeArea.ID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_SafeArea.AreaPos.x = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_SafeArea.AreaPos.y = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_SafeArea.AreaRadius = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_SafeArea.RotateY = float.Parse(t_reader.Value);
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }
}
