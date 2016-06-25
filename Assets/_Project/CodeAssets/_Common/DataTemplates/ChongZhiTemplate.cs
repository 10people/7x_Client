using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ChongZhiTemplate : XmlLoadManager
{
    public int id;

    public string name;

    public string desc;

    public int needNum;

    public int addNum;

    public int extraYuanbao;

    public int type;

    public int extraFirst;

	public int addVipExp;

	public int rank;

	public static List<ChongZhiTemplate> templates = new List<ChongZhiTemplate>();


    public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
        UnLoadManager.DownLoad(PathManager.GetUrl(XmlLoadManager.m_LoadPath + "ChongZhi.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
            t_has_items = t_reader.ReadToFollowing("ChongZhi");

            if (!t_has_items)
            {
                break;
            }

            ChongZhiTemplate t_template = new ChongZhiTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.name = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc =  t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.needNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.addNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.extraYuanbao =  int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);
                
                 t_reader.MoveToNextAttribute();
                 t_template.extraFirst = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.addVipExp = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.rank = int.Parse(t_reader.Value);
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

	public static ChongZhiTemplate GetChongZhiTempById (int id)
	{
		foreach (ChongZhiTemplate template in templates)
		{
			if (template.id == id)
			{
				return template;
			}
		}
		Debug.LogError ("Can not getChongZhiTemplate by id:" + id);
		return null;
	}
}
