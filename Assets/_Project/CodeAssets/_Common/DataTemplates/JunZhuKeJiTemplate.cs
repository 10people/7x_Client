using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class JunZhuKeJiTemplate : XmlLoadManager{

    public int m_id;

    public int m_nameId;

    public int m_describeId;

    public int m_requireLevel;

    public int m_costMoney;

	public string m_icon;

	public static List<JunZhuKeJiTemplate> m_templates = new List<JunZhuKeJiTemplate>();



	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
       	UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "JunZhuKeji.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
    }

	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			m_templates.Clear();
		}

		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "JunzhuKeji" );
			
			if( !t_has_items ){
				break;
			}
			
			JunZhuKeJiTemplate t_template = new JunZhuKeJiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.m_id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_nameId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_describeId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_icon = t_reader.Value;

				t_reader.MoveToNextAttribute();
				// actionID

				t_reader.MoveToNextAttribute();
				// dataType

				t_reader.MoveToNextAttribute();
				// value

				t_reader.MoveToNextAttribute();
				t_template.m_requireLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_costMoney = int.Parse( t_reader.Value );

			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static JunZhuKeJiTemplate GetTemplateById(int tempId)
    {
		foreach (JunZhuKeJiTemplate tempLate in m_templates)
        {
            if (tempLate.m_id == tempId)
            {
                return tempLate;
            }
        }
        return null;
    }
}
