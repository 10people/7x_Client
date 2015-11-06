using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class NpcTemplate : XmlLoadManager {
	public static List<int> Enemy_NameList = new List<int> ();
	public static List<int> Enemy_IdList = new List<int> ();

	//<NpcTemp id="100001" npcId="100000" enemyId="201000" name="410001" desc="410001" 
	//position="103" modelId="4002" gongjiType="32" profession="4" type="2" />


	public int id;
	
	public int NpcId;
	
	public int EnemyId;

	public int EnemyName;

	public int desc;

	public int e_position;

	public int modelId;

	public int gongjiType;

	public int profession;

	public int type;

	public string skills;

	public string award;

	public string icon;

	public int level;

	private static List<NpcTemplate> templates = new List<NpcTemplate>();

	public void Log(){
		Debug.Log( "NpcTemplate-  id: " + id +
		          " id: " + NpcId + 
		          " EnemyId: " + EnemyId + 
		          " e_position: " + e_position +
		          " modelId: " + modelId
		          );
	}

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad( 
			PathManager.GetUrl( XmlLoadManager.m_LoadPath + "NpcTemp.xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW www, string path, Object obj ){
		if (obj == null) {
			Debug.LogError ("Asset Not Exist: " + path);
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if( templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "NpcTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			NpcTemplate t_template = new NpcTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.NpcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.EnemyId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.EnemyName = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.e_position = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skills = t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.award =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.icon =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}
	
	public static bool HaveNpcTemplateByNpcId(int npcId){
		{
			ProcessAsset();
		}

		foreach(NpcTemplate t_item in templates)
		{
			if( t_item.NpcId == npcId )
			{
				return true;
			}
		}
		
		return false;
	}

	public static NpcTemplate GetNpcTemplate_By_npcid(int npc_id){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){

				return t_item;
			}
		}
		
		Debug.Log( npc_id + " : not found." );
		
		return null;
	}
	public static NpcTemplate GetNpcTemplate_By_id(int id){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.id == id ){
				
				return t_item;
			}
		}
		
		Debug.Log( id + " : not found." );
		
		return null;
	}
	public static List<NpcTemplate> GetNpcTemplates_By_npcid(int npc_id){
		{
			ProcessAsset();
		}

		List<NpcTemplate> temps = new List<NpcTemplate> ();

		for( int i = 0; i < templates.Count; i++ )
		{
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){
				
				temps.Add(t_item);
			}
		}
		
		return temps;
	}

	public static NpcTemplate GetNpcTemplate_By_npcid_position(int npc_id, int position){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id && t_item.e_position == position){
				
				return t_item;
			}
		}
		
		Debug.Log( npc_id + " + " + position + " : not found." );
		
		return null;
	}


	public static List<int> GetEnemyName_By_npcid(int npc_id){
		{
			ProcessAsset();
		}

		Enemy_NameList.Clear ();
		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){

				if(!Enemy_NameList.Contains(t_item.EnemyName))
				{
					Debug.Log( t_item.EnemyName + " : asdadadas." );
					Enemy_NameList.Add(t_item.EnemyName);
				}
			}
		}
		
		Debug.Log( "敌人被添加了的不同名字的个数"+Enemy_NameList.Count);
		
		return Enemy_NameList;
	}
	public static int GetEnemyNameId_By_EnemyId(int EnId){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.EnemyId == EnId ){
				return t_item.EnemyName;
			}
		}
		
		Debug.Log( EnId+"npc_id is not found");
		
		return 0;
	}

	public static NpcTemplate GetNpcTemplate_By_EnemyId(int E_Id){
		{
			ProcessAsset();
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.EnemyId == E_Id ){
				return t_item;
			}
		}
		
		Debug.Log( E_Id+"npc_id is not found");
		
		return null;
	}
	public static List<int> GetEnemyId_By_npcid(int npc_id){
		{
			ProcessAsset();
		}

		Enemy_IdList.Clear ();
		for( int i = 0; i < templates.Count; i++ ){
			NpcTemplate t_item = templates[ i ];
			
			if( t_item.NpcId == npc_id ){

				if(!Enemy_IdList.Contains(t_item.EnemyId))
				{

					Enemy_IdList.Add(t_item.EnemyId);
				}
			}
		}
		
//		Debug.Log( "敌人被添加了的不同名字的个数"+Enemy_IdList.Count);
		
		return Enemy_IdList;
	}
}
