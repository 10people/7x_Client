using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class VIPQianDaoTemp : XmlLoadManager
{
    //<VIPQianDao VIP = "1" desc="解锁【购买秘宝升级点数】功能。每日赠送免费扫荡100次。每日可购买体力2次。最高可购买蓝色镖车。每日可购买试炼2次。" day="1" jifen="0:900029:10" />
 

    public int VIP;
    public string desc;
    public int day;
    public string jifen;

     

    public static List<VIPQianDaoTemp> templates = new List<VIPQianDaoTemp>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "VIPQianDao.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("VIPQianDao");

            if (!t_has_items)
            {
                break;
            }

            VIPQianDaoTemp t_template = new VIPQianDaoTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.VIP = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.desc =  t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.day = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.jifen = t_reader.Value;

            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static VIPQianDaoTemp GetVipByVipLevel(int vip)
    {
        foreach (VIPQianDaoTemp _template in templates)
        {
            if (_template.VIP == vip)
            {
                return _template;
            }
        }

        return null;
    }
}
