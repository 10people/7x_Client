using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class YouXiaNpcTemplate : XmlLoadManager {

	//<YouxiaNpcTemp id="300101" npcId="301010" enemyId="210101" modelId="4006" skills="238101,238102,238103,238104" 
	//	name="400402" desc="400402" position="101" gongjiType="1012" profession="4" type="3" award="" icon="1000002" 
		//	level="30" />

	public int id;
	
	public int NpcId;
	
	public int EnemyId;
	
	public int modelId;
	
	public string skills;

	public int Name;

	public int desc;
	
	public int position;
	
	public int gongjiType;
	
	public int profession;
	
	public int type;
	
	public string award;
	
	public string icon;

	public int level;

	public static List<YouXiaNpcTemplate> templates = new List<YouXiaNpcTemplate>();
	
	public void Log(){

	}
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( 
		                       PathManager.GetUrl( XmlLoadManager.m_LoadPath + "YouxiaNpcTemp.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "YouxiaNpcTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			YouXiaNpcTemplate t_template = new YouXiaNpcTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.NpcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.EnemyId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skills =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.Name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.position = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.icon =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static List<int> GetEnemyId_By_npcid(int npc_id)
	{
		List <int> Enemy_IdList = new List<int> ();

		Enemy_IdList.Clear ();

		for( int i = 0; i < templates.Count; i++ ){

			YouXiaNpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){
				
				if(!Enemy_IdList.Contains(t_item.EnemyId))
				{
					Enemy_IdList.Add(t_item.EnemyId);
				}
			}
		}
		return Enemy_IdList;
	}
	public static List<YouXiaNpcTemplate> GetYouXiaNpcTemplates_By_npcid(int npc_id)
	{
		List<YouXiaNpcTemplate> temps = new List<YouXiaNpcTemplate> ();
		
		for( int i = 0; i < templates.Count; i++ )
		{
			YouXiaNpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){
				
				temps.Add(t_item);
			}
		}
		
		return temps;
	}
	public static YouXiaNpcTemplate GetYouXiaNpcTemplate_By_id(int id ,int npcid)
	{
		for( int i = 0; i < templates.Count; i++ ){
			
			YouXiaNpcTemplate t_item = templates[ i ];
			
			if( t_item.id == id &&t_item.NpcId ==  npcid){
				
				return t_item;
			}
		}
		
		Debug.Log( id + " : not found." );
		
		return null;
	}
}
