using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class CountryZuoBiaoTemplate : XmlLoadManager {

	public int id;

	public string des;

	public float x;
	
	public float y;
	
	public float z;

    public int color;
	
	public static List<CountryZuoBiaoTemplate> templates = new List<CountryZuoBiaoTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "NationZuobiao.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			templates.Clear();
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
			t_has_items = t_reader.ReadToFollowing( "NationZuobiao" );
			
			if( !t_has_items ){
				break;
			}
			
			CountryZuoBiaoTemplate t_template = new CountryZuoBiaoTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.des =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.x = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.y = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.z = float.Parse( t_reader.Value );

                t_reader.MoveToNextAttribute();
                t_template.color = int.Parse(t_reader.Value);
            }
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	public static Vector3 GetZuobiao_by_id( int m_id ){

		Vector3 mv = new Vector3 (0,0,0);
		for( int i = 0 ; i < templates.Count; i++ ){
			
			CountryZuoBiaoTemplate t_item = templates[ i ];
			
			if( t_item.id ==m_id ){
				mv.x = t_item.x;
				mv.y = t_item.y;
				mv.z = t_item.z;
			}
		}
		return mv;
	}
	public static string GetDesco_by_id( int m_id ){


		Vector3 mv = new Vector3 (0,0,0);
		for( int i = 0 ; i < templates.Count; i++ ){
			
			CountryZuoBiaoTemplate t_item = templates[ i ];
			
			if( t_item.id ==m_id ){
				return t_item.des;
			}
		}
		return "";
	}

    public static CountryZuoBiaoTemplate GeTempByid(int m_id)
    {
        Vector3 mv = new Vector3(0, 0, 0);
        for (int i = 0; i < templates.Count; i++)
        {

            CountryZuoBiaoTemplate t_item = templates[i];

            if (t_item.id == m_id)
            {
                return t_item;
            }
        }
        return null;
    }
}
