using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using qxmobile;
using qxmobile.protobuf;
public class HouseWeaponTemplate : XmlLoadManager
{
    public int modelID;
    public int itemID;



    public static List<HouseWeaponTemplate> templates = new List<HouseWeaponTemplate>();


    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HouseWeapon.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("HouseWeapon");

            if (!t_has_items)
            {
                break;
            }

            HouseWeaponTemplate t_template = new HouseWeaponTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.modelID = string.IsNullOrEmpty(t_reader.Value) ? 0 : int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.itemID = string.IsNullOrEmpty(t_reader.Value) ? 0 : int.Parse(t_reader.Value);
            }

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static int GetHouseWeaponItemIDByModelID(int modelID)
    {
        return (from t in templates where t.modelID == modelID select t.itemID).FirstOrDefault();
    }
}
