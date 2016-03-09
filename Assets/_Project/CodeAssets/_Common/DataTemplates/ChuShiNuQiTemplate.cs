using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ChuShiNuQiTemplate : XmlLoadManager {


	public int num;
	
	public int nuqiValue;
	
	public float nuqiRatioc;
	
	public static List<ChuShiNuQiTemplate> templates = new List<ChuShiNuQiTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ChuShiNuQi.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "ChuShiNuQi" );
			
			if( !t_has_items ){
				break;
			}
			
			ChuShiNuQiTemplate t_template = new ChuShiNuQiTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.num = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nuqiValue =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.nuqiRatioc = float.Parse( t_reader.Value );
				
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	public static ChuShiNuQiTemplate getChuShiNuQiTemplate_by_Num(int Num)
	{
		foreach(ChuShiNuQiTemplate template in templates)
		{
			if(template.num == Num)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get ChuShiNuQiTemplate with Num " + Num);
		
		return null;
	}
}
