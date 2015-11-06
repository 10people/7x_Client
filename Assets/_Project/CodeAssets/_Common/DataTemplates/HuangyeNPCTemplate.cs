using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class HuangyeNPCTemplate : XmlLoadManager {
	//<HuangyeNpc id="1" npcId="200001" enemyId="201000" name="410001" desc="400001" 
	//position="103" modelId="4002" gongjiType="11" profession="4" type="2" award="200001" />

	public int id;
	
	public int npcId;
	
	public int enemyId;
	
	public int name;
	
	public int desc;
	
	public int position;
	
	public int modelId;
	
	public int gongjiType;

	public int profession;

	public int type;

	public string skill;

	public string award;

	public string icon;

	public int level;

	private static List<HuangyeNPCTemplate> templates = new List<HuangyeNPCTemplate>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyeNpc.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;

	public static void CurLoad( ref WWW p_www, string p_path, Object p_object ){
		if (p_object == null) {
			Debug.LogError ("Asset Not Exist: " + p_path);
			
			return;
		}
		
		m_templates_text = p_object as TextAsset;
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
			t_has_items = t_reader.ReadToFollowing( "HuangyeNpc" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangyeNPCTemplate t_template = new HuangyeNPCTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.enemyId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.desc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.position = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.modelId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.icon =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
			
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static HuangyeNPCTemplate GetHuangyeNPCTemplate_By_id(int id){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			HuangyeNPCTemplate t_item = templates[ i ];
			
			if( t_item.id == id ){
				
				return t_item;
			}
		}
		
		Debug.Log( id + " : not found." );
		
		return null;
	}



	public static int GetEnemyNameId_By_EnemyId(int EnId){
		{
			ProcessAsset();
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			HuangyeNPCTemplate t_item = templates[ i ];
			
			if( t_item.enemyId == EnId ){
				return t_item.name;
			}
		}
		
		Debug.Log( EnId+"npc_id is not found");
		
		return 0;
	}

	public static List<HuangyeNPCTemplate> GetHuangyeNPCTemplates_By_npcid(int npc_id){
		{
			ProcessAsset();
		}

		List<HuangyeNPCTemplate> temps = new List<HuangyeNPCTemplate> ();
		
		for( int i = 0; i < templates.Count; i++ )
		{
			HuangyeNPCTemplate t_item = templates[ i ];
			
			if( t_item.npcId == npc_id ){
				
				temps.Add(t_item);
			}
		}
		
		return temps;
	}

	public static List<int> getEnemyidlist_by_Npcid(int Npc_id){
		{
			ProcessAsset();
		}

		 List<int> enemyidlist = new List<int>();
		enemyidlist.Clear ();
		foreach(HuangyeNPCTemplate template in templates)
		{
			if(template.npcId == Npc_id)
			{
				if(!enemyidlist.Contains(template.enemyId))
				{
					enemyidlist.Add(template.enemyId);
				}

			}
		}

		return enemyidlist;
	}

	public static HuangyeNPCTemplate GetHuangyeNPCTemplatee_By_EnemyId(int E_Id){
		{
			ProcessAsset();
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			HuangyeNPCTemplate t_item = templates[ i ];
			
			if( t_item.enemyId == E_Id ){
				return t_item;
			}
		}
		
		Debug.Log( E_Id+"npc_id is not found");
		
		return null;
	}

	public static HuangyeNPCTemplate getHuangyeNPCTemplatee_by_Npcid(int Npc_id){
		{
			ProcessAsset();
		}

		foreach(HuangyeNPCTemplate template in templates)
		{
			if(template.npcId == Npc_id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangyeNPCTemplate with id " + Npc_id);
		
		return null;
	}
}
