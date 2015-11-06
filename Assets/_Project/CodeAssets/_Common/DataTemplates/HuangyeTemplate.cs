using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangyeTemplate : XmlLoadManager {

	public int id;
	
	public int fogId;
	
	public int type;
	
	public int positionX;
	
	public int positionY;

	public int nameId;

	public int descId;

	public int icon;

	public int needLv;
	
	public static List<HuangyeTemplate> templates = new List<HuangyeTemplate>();

	public static List<int> huangye_idList = new List<int>();

	public void Log()
	{
		Debug.Log("HuangyeTemplate( id " + id +
		          " fogId: " + fogId +
		          " needLv: " + needLv +
		          " type: " + type +
		          " positionX: " + positionX +
		          " positionY: " + positionY +
		          " nameId: " + nameId +
		          " descId: " + descId +
		          " icon: " + icon+
		          " needLv: " + needLv
		          );
	}
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangYe.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	public static void CurLoad( ref WWW p_www, string p_path, Object p_object ){
		{
			templates.Clear ();
		}

		XmlReader t_reader = null;
		
		if(p_object != null){
			TextAsset t_text_asset = p_object as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( p_www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "HuangYe" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangyeTemplate t_template = new HuangyeTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fogId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.positionX = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.positionY = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.nameId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.descId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needLv = int.Parse( t_reader.Value );
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );
	}
	
	
	public static HuangyeTemplate getHuangyeTemplate_byid(int id)
	{
		foreach(HuangyeTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangyeTemplate with id " + id);
		
		return null;
	}

	// TODO, For RongLing, 2015.3.12
	public static List<int> get_idList( int Fog_id ){
		Debug.LogError( "Please Check when to Clear huangye_idList." );
		huangye_idList.Clear ();
		foreach( HuangyeTemplate template in templates ){
			if( template.fogId == Fog_id ){
				huangye_idList.Add( template.id );
			}
		}

		return huangye_idList;
	}

}
