using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class NpcCityTemplate : XmlLoadManager {

	public int m_Id;

    public int m_Type;
	public int m_npcId;

	public int m_npcShowId;
    public int m_npcIcon;

	public int m_npcName;

	public int m_functionId;

	public float m_Angles;

	public Vector3 m_position;

	public string m_dialog1;

	public string m_dialog2;

	public string m_dialog3;

    public float NameDirectX;
    public float NameDirectY;

	public static List<NpcCityTemplate> m_templates = new List<NpcCityTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
//		Debug.Log ("加载这文件 ");

		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "NpcCity.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "NpcCity" );
			
			if( !t_has_items ){
				break;
			}
			
			NpcCityTemplate t_template = new NpcCityTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.m_Id = int.Parse( t_reader.Value );

                t_reader.MoveToNextAttribute();
                t_template.m_Type = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.m_npcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.m_npcShowId = int.Parse( t_reader.Value );
				
                t_reader.MoveToNextAttribute();
                t_template.m_npcIcon = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.m_npcName = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.m_functionId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				float t_x = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				float t_y = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				float t_z = float.Parse( t_reader.Value );

				t_template.m_position = new Vector3( t_x, t_y, t_z );

				t_reader.MoveToNextAttribute();
				// radius1

				t_reader.MoveToNextAttribute();
				// radius2

				t_reader.MoveToNextAttribute();
				t_template.m_Angles = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				int tempDialog1 = int.Parse( t_reader.Value );
				
				t_template.m_dialog1 = ( tempDialog1 > 0 )?(DescIdTemplate.GetDescriptionById(tempDialog1)):(null);

				t_reader.MoveToNextAttribute();
				int tempDialog2 = int.Parse( t_reader.Value );
				
				t_template.m_dialog2 = ( tempDialog2 > 0 )?(DescIdTemplate.GetDescriptionById(tempDialog2)):(null);

				t_reader.MoveToNextAttribute();
				int tempDialog3 = int.Parse( t_reader.Value );
				
				t_template.m_dialog3 = ( tempDialog3 > 0 )?(DescIdTemplate.GetDescriptionById(tempDialog3)):(null);

                t_reader.MoveToNextAttribute();
                t_template.NameDirectX = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.NameDirectY = float.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			m_templates.Add( t_template );
		}
		while( t_has_items );
	}
    public static NpcCityTemplate GetNpcItemById(int npc_id)
    {
        foreach (NpcCityTemplate item in m_templates)
        {
            if (item.m_Id == npc_id)
            {
                return item;
            }
        }
        return null;
    }

}
