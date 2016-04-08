using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class HuangyePvpNpcTemplate : XmlLoadManager  {
//	id="10001" icon="1" zhiye="1" name="360001" description="360001" level="1" gongji="28" fangyu="14" shengming="1080" wqSH="0" wqJM="0"
//		wqBJ="0" wqRX="0" jnSH="0" jnJM="0" jnBJ="0" jnRX="0" skill1="0" skill2="0" skill3="0" skill4="0" skill5="0" power="345" mibao1="301021"
//			mibao2="301022" mibao3="301023" mibaoLv="1" mibaoStar="3" weapon1="101001" weapon2="0" weapon3="0" />
	public int id;
	
	public int icon;
	
	public int zhiye;
	
	public int name;
	
	public int description;
	
	public int level;
	
	public int gongji;
	
	public int fangyu;
	
	public int shengming;
	
	public int wqSH;
	
	public int wqJM;
	
	public int wqBJ;

	public int wqRX;

	public int jnSH;

	public int jnJM;

	public int jnBJ;

	public int jnRX;

	public int skill1;

	public int skill2;

	public int skill3;

	public int skill4;

	public int skill5;

	public int power;

	public int mibao1;

	public int mibao2;

	public int mibao3;

	public int mibaoLv;

	public int mibaoStar;

	public int weapon1;

	public int weapon2;

	public int weapon3;

	public int profession;

	public int type;

	public int model;

	
	private static List<HuangyePvpNpcTemplate> templates = new List<HuangyePvpNpcTemplate>();

	private static void WarnningShouldNeverUse(){
		Debug.LogError( "Error, Template will never be use from 2016.3.24." );
	}

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		{
			WarnningShouldNeverUse();
		}

		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "HuangyePvpNpc.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
		{
			WarnningShouldNeverUse();
		}

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
			t_has_items = t_reader.ReadToFollowing( "HuangyePvpNpc" );
			
			if( !t_has_items ){
				break;
			}
			
			HuangyePvpNpcTemplate t_template = new HuangyePvpNpcTemplate();
			
			{
				
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.zhiye = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.name = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.description = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.level = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.gongji = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.fangyu = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.shengming = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqSH = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqJM =  int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.wqBJ = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnRX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnSH = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnJM = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnBJ = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.jnRX = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill4 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.skill5 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibao1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibao2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibao3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibaoLv = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.mibaoStar = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.weapon1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.weapon2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.weapon3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.profession = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.model = int.Parse( t_reader.Value );
			}
			
			templates.Add(t_template);
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static HuangyePvpNpcTemplate GetHuangyePvpNpcTemplate_By_id(int id){
		{
			ProcessAsset();
		}

		for( int i = 0; i < templates.Count; i++ ){
			HuangyePvpNpcTemplate t_item = templates[ i ];
			
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
			HuangyePvpNpcTemplate t_item = templates[ i ];
			
//			if( t_item.enemyId == EnId ){
//				return t_item.name;
//			}
		}
		
		Debug.Log( EnId+"npc_id is not found");
		
		return 0;
	}
	public static List<int> getEnemyidlist_by_Npcid(int Npc_id){
		{
			ProcessAsset();
		}

		List<int> enemyidlist = new List<int>();
		enemyidlist.Clear ();
		foreach(HuangyePvpNpcTemplate template in templates)
		{
			if(template.id == Npc_id)
			{
				if(!enemyidlist.Contains(template.id))
				{
					enemyidlist.Add(template.id);
				}
				
			}
		}
		
		return enemyidlist;
	}
	public static HuangyePvpNpcTemplate getHuangyepvpNPCTemplate_by_Npcid(int Npc_id){
		{
			ProcessAsset();
		}

		foreach(HuangyePvpNpcTemplate template in templates)
		{
			if(template.id == Npc_id)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get HuangyePvpNpcTemplate with id " + Npc_id);
		
		return null;
	}
}
