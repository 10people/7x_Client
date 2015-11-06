using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ItemTemp : XmlLoadManager
{
    //<ItemTemp id="900001" name="900001" funDesc="900001" icon="6" quality="0" repeatNum="-1" itemType="0" 
    //effectId="901,902,902,903,904" sellNum="0" />

    public int id;

    public int itemName;

    public int funDesc;

    public string icon;

    public int quality;

    public long repeatNum;

    public int itemType;

    public List<int> effectIds;
    public int effectId;

    public int sellNum;

    public int color;

	public static List<ItemTemp> templates = new List<ItemTemp>();


    public void Log()
    {
        Debug.Log("ItemTemp( id: " + id +
                  " itemName: " + itemName +
                  " funDesc: " + funDesc +
                  " icon: " + icon +
                  " quality: " + quality +
                  " repeatNum: " + repeatNum +
                  " itemType: " + itemType +
                  " color: " + color);
    }

    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ItemTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
    }

    public static void CurLoad( ref WWW www, string path, Object obj ){
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
            t_has_items = t_reader.ReadToFollowing("ItemTemp");

            if (!t_has_items)
            {
                break;
            }

            ItemTemp t_template = new ItemTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.itemName = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.funDesc = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.icon = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.quality = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.repeatNum = long.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.itemType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                string effectIdStr = t_reader.Value;
                t_template.effectId = int.Parse(t_reader.Value);

                string[] effectIds = effectIdStr.Split(',');

                t_template.effectIds = new List<int>();

                foreach (string st in effectIds)
                {
                    t_template.effectIds.Add(int.Parse(st));
                }

                t_reader.MoveToNextAttribute();
                t_template.sellNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.color = int.Parse(t_reader.Value);
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static ItemTemp getItemTempById(int id)
    {
        //        Debug.Log("templatestemplatestemplatestemplates" + templates.Count);
        foreach (ItemTemp template in templates)
        {
            if (template.id == id)
            {
                return template;
            }
        }

        Debug.LogError("XML ERROR: Can't get ItemTemp with id " + id);

        return null;
    }

    public static string getIconTempById(int id)
    {
        foreach (ItemTemp template in templates)
        {
            if (template.id == id)
            {
                return template.icon;
            }
        }
        return null;
    }




}
