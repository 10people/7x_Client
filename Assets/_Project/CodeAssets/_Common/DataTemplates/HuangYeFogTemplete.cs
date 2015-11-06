using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangYeFogTemplete : XmlLoadManager {
	public int fogId;
	
	public int openCost;
	
	public int needLv;
	
	public float positionX;
	
	public float positionY;

	public int length;

	public int height;

	public int needMaxLv;
	
	public static List<HuangYeFogTemplete> templates = new List<HuangYeFogTemplete>();
	
	public void Log()
	{
		Debug.Log("HuangYeFogTemplete( idfogId " + fogId +
		          " openCost: " + openCost +
		          " needLv: " + needLv +
		          " positionX: " + positionX +
		          " positionY: " + positionY 
		          );
	}
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
//		Debug.Log ("加载荒野迷雾xml");

		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyeFog.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HuangyeFog" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangYeFogTemplete t_template = new HuangYeFogTemplete();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.fogId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.openCost = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.positionX = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.positionY = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.length = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.height = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needLv = int.Parse( t_reader.Value );
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );
	}


	public static HuangYeFogTemplete getHuangYeFogTemplete(int Fog_id)
	{
		foreach(HuangYeFogTemplete template in templates)
		{
			if(template.fogId == Fog_id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangYeFogTemplete with id " + Fog_id);
		
		return null;
	}

}
















