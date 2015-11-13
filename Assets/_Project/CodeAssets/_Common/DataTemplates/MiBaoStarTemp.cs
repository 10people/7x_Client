using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoStarTemp : XmlLoadManager {

	public int star;
	public int needNum;
	public int needMoney;
	public float chengzhang;

	public static List<MiBaoStarTemp> templates = new List<MiBaoStarTemp>();
	
	
	public void Log(){
		Debug.Log( "MiBaoStarTemp.Log( id: " +
		          " awardId : " );
	}

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoStar.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "MibaoStar" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoStarTemp t_template = new MiBaoStarTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.star = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needNum = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.needMoney = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.chengzhang = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static MiBaoStarTemp getMiBaoStarTempBystar(int star_id){
		foreach( MiBaoStarTemp template in templates ){
			if( template.star == star_id ){
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get MiBaoMiBaoStarTempDiaoLuoXmlTemp with id " + star_id);
		
		return null;
	}

}
