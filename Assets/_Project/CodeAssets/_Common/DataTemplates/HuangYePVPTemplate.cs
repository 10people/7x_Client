using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangYePVPTemplate : XmlLoadManager {

	//<HuangyePvp id="201001" needLv="1" npc="5" refreshCost="2000" produceSpeed="10" killAward="100" 
	//match="0" sceneId="900" configId="900" />

	public int id;
	
	public int needLv;
	
	public int npc1;
	
	public int npc2;
	
	public int npc3;
	
	public int refreshCost;
	
	public int produceSpeed;
	
	public int killAward;
	
	public int match;

	public int sceneId;

	public int configId;

	
	public static List<HuangYePVPTemplate> templates = new List<HuangYePVPTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyePvp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "HuangyePvp" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangYePVPTemplate t_template = new HuangYePVPTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.needLv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.npc1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.npc2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.npc3 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.refreshCost = int.Parse(t_reader.Value) ;
				
				t_reader.MoveToNextAttribute();
				t_template.produceSpeed = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.killAward = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.match = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse( t_reader.Value );
			
				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse( t_reader.Value );
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );
	}
	
	
	public static HuangYePVPTemplate getHuangYePVPTemplate_byid(int id)
	{
		foreach(HuangYePVPTemplate template in templates)
		{
			if(template.id == id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangYePVPTemplate with id " + id);
		
		return null;
	}
}
