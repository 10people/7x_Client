using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class ExpTempTemp : XmlLoadManager
{
    public int id;
    public int expId;
    public int level;
    public int needExp;

    private static List<ExpTempTemp> templates = new List<ExpTempTemp>();
 
    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ExpTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);

    }

	private static TextAsset m_templates_text = null;

    public static void CurLoad(ref WWW www, string path, Object obj){
		if ( obj == null) {
			Debug.LogError ("Asset Not Exist: " + path );

			return;
		}

		m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}

		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );

			return;
		}

		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;

		do{
            t_has_items = t_reader.ReadToFollowing("ExpTemp");

            if (!t_has_items)
            {
                break;
            }

            ExpTempTemp t_template = new ExpTempTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.expId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.level = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.needExp = int.Parse(t_reader.Value);
 
            }

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);

		{
			m_templates_text = null;
		}
    }

    public static List<ExpTempTemp> GetEquipUpgradeInfo(int expId){
		{
			ProcessAsset();
		}

        List<ExpTempTemp> list = new List<ExpTempTemp>();
        for (int i = 0; i < templates.Count; i++)
        {
            ExpTempTemp t_item = templates[i];

            if (t_item.expId == expId)
            {
                list.Add(t_item);
            }
        }

       

        return list;
    }

    public static int GetEquipUpgradeInfo(int expId,int leve){
		{
			ProcessAsset();
		}

        List<ExpTempTemp> list = new List<ExpTempTemp>();
        for (int i = 0; i < templates.Count; i++)
        {
            ExpTempTemp t_item = templates[i];

            if (t_item.expId == expId && t_item.level == leve)
            {
                list.Add(t_item);
            }
        }

        return 0;
    }



}
