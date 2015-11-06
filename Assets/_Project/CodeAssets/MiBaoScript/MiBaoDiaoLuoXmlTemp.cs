using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoDiaoLuoXmlTemp : XmlLoadManager {
	
	//<AwardTemp id="800001" awardId="1" itemId="111001" itemType="0" itemNum="1" weight="100" />
	
	public int suipianId;
	
	public string legendPveId;

	public static List<MiBaoDiaoLuoXmlTemp> templates = new List<MiBaoDiaoLuoXmlTemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoDiaoLuoXmlTemp.Log( id: " + suipianId +
		          " awardId : " );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoDiaoLuo.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
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
			t_has_items = t_reader.ReadToFollowing( "MibaoDiaoLuo" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoDiaoLuoXmlTemp t_template = new MiBaoDiaoLuoXmlTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.suipianId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.legendPveId = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MiBaoDiaoLuoXmlTemp getMiBaoDiaoLuoXmlTempBysuipian_id(int suipian_id)
	{
		foreach( MiBaoDiaoLuoXmlTemp template in templates )
		{
			if( template.suipianId == suipian_id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoDiaoLuoXmlTemp with id " + suipian_id);
		
		return null;
	}

}
