using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ChongLouNpcTemplate : XmlLoadManager {

//	<ChonglouNpcTemp id="400001" npcId="400010" enemyId="102117" name="4000101" desc="4000101" position="101" modelId="3001" 
//		gongjiType="21" profession="1" type="2" skills="" award="900001=100" icon="3001" level="101" ifTeammate="0" lifebarNum="1"
//			modelApID="10121080" armor="0" armorMax="0" armorRatio="0" />

	
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
	
	public string skills;
	
	public string award;
	
	public int icon;
	
	public int level;
	
	public int ifTeammate;
	
	public int  lifebarNum;
	
	public int modelApID;
	
	public int armor;
	
	public int armorMax;
	
	public int  armorRatio;

	private static List<ChongLouNpcTemplate> templates = new List<ChongLouNpcTemplate>();
	
	public void Log(){
		Debug.Log( "ChongLouNpcTemplate-  id: " + id +
		          " bigName: " + npcId + 
		          " smaName: " + npcId );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ChonglouNpcTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null ) {
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
			t_has_items = t_reader.ReadToFollowing( "ChonglouNpcTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			ChongLouNpcTemplate t_template = new ChongLouNpcTemplate();
			
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
				t_template.modelId =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongjiType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skills =  t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.award = t_reader.Value ;
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ifTeammate = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lifebarNum = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.modelApID = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.armor =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.armorMax =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.armorRatio = int.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
		
		{
			m_templates_text = null;
		}
	}
	public static List<ChongLouNpcTemplate> GeChongLouNpcTemplates_By_npcid(int npc_id){
		{
			ProcessAsset();
		}
		
		List<ChongLouNpcTemplate> temps = new List<ChongLouNpcTemplate> ();
		
		for( int i = 0; i < templates.Count; i++ )
		{
			ChongLouNpcTemplate t_item = templates[ i ];
			
			if( t_item.npcId == npc_id && t_item.ifTeammate != 1&&t_item.icon != 0 &&t_item.type < 6)
			{
				temps.Add(t_item);
			}
		}
		return temps;
	}
	public static ChongLouNpcTemplate Get_QCL_NpcTemplate_By_Layer( int id ){
		{
			ProcessAsset();	
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			ChongLouNpcTemplate t_item = templates[ i ];
			
			if( t_item.id == id ){
				return t_item;
			}
		}
		
		Debug.LogError( "ChongLouNpcTemplate not found: " + id  );
		
		return null;
	}
	public static int Get_QCL_NpcModel_By_npcid( int id ){
		{
			ProcessAsset();	
		}
		
		for( int i = 0; i < templates.Count; i++ ){
			ChongLouNpcTemplate t_item = templates[ i ];
			
			if( t_item.npcId == id &&t_item.type == 4 ){
				return t_item.modelId;
			}
		}
		
		Debug.LogError( "ChongLouNpcTemplate not found: " + id  );
		
		return 0;
	}
}
