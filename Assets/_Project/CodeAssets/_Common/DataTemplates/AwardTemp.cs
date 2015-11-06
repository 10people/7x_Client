using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class AwardTemp : XmlLoadManager
{
	private enum AwardItemType{
		NormalItems = 0,
		Equips = 2,
		Heros = 7,
	}

	//<AwardTemp id="800001" awardId="1" itemId="111001" itemType="0" itemNum="1" weight="100" />

	public int id;

	public int awardId;

	public int itemId;

	public int itemType;

	public int itemNum;

	public int weight;

	public static List<AwardTemp>templateList = new List<AwardTemp> ();

	public static List<AwardTemp> templates = new List<AwardTemp>();


	public void Log(){
		Debug.Log( "AwardTemp.Log( id: " + id +
		          " awardId : " + awardId +
		          " itemId : " + itemId +
		          " itemType : " + itemType +
		          " itemNum : " + itemNum +
		          " weight : " + weight );
	}


	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "AwardTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "AwardTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			AwardTemp t_template = new AwardTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.awardId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.itemType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.itemNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.weight = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static AwardTemp getAwardTempById(int id)
	{
		foreach( AwardTemp template in templates )
		{
			if( template.id == id )
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get AwardTemp with id " + id);
		
		return null;
	}


	public static List<AwardTemp> getAwardTempList_By_AwardId( int p_award_id )
	{
		templateList.Clear ();

//		Debug.Log ("p_award_id:"+p_award_id);

		foreach( AwardTemp template in templates )
		{
			if( template.awardId == p_award_id )
			{
				templateList.Add(template);;
			}
		}

		return templateList;
	}
	public static AwardTemp getAwardTemp_By_AwardId( int p_award_id )
	{
		foreach( AwardTemp template in templates )
		{
			if( template.awardId == p_award_id )
			{
				return template;
			}
		}
		
		Debug.LogError( "AwardTemp not found with award id: " + p_award_id );
		
		return null;
	}

}
