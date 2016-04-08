using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PveTempTemplate : XmlLoadManager {

	//<PveTemp id="100101" bigName="30001" bigDesc="30001" bigId="1" smaName="40101" smaDesc="40101" smaId="1" chapType="0"
	//useHp="5" monarchLevel="1" frontPoint="0" openCondition="0" money="100" exp="50" awardId="" firstAwardId="" npcId="101010" 
		//bossId="0" landId="0" power="244" star1="0" star2="0" star3="0" icon="4" soundId="100101" sceneId="900" time="-100" />

	public int id;


	public int bigName;
	
	public int bigDesc;
	
	public int bigId;


	public int smaName;
	
	public int smaDesc;
	
	public int smaId;


	public int chapType;

	public int useHp;

	public int monarchLevel;

	public int frontPoint;

	public int openCondition;

	public int money;

	public int exp;


	public string awardId;

	public string firstAwardId;

	public int npcId;

	public int bossId;

	public int landId;

	public int power;

	public int star1;

	public int star2;

	public int star3;

	public int icon;


	public int soundId;

	public int sceneId;
    
	public int time;

	public int RenWuLimit;

	public int PowerLimit;

	public int configId; //juqingNPC="100101" wanfaType="ÆÕÍ¨" recZhanli="1" recMibaoSkill="250101" />

	public int	TeammateID;

	public int juqingNPC;

	public string wanfaType;

	public int recZhanli;

	public string recMibaoSkill;

	public string OPenSkillLabel;

	public int bossIcon; //0 为没有bossicon  1 wei you boss

	public string bubble; // 气泡

	public int BigIcon; // 1wei bigicon

	public string enddingRemind;

	private static List<PveTempTemplate> templates = new List<PveTempTemplate>();

	public void Log(){
		Debug.Log( "PveTempTemplate-  id: " + id +
		          " bigName: " + bigName + 
		          " smaName: " + smaName );
	}
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PveTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "PveTemp" );
			
			if( !t_has_items ){
				break;
			}
			
			PveTempTemplate t_template = new PveTempTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.bigName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.bigDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.bigId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.smaName = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.smaDesc = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.smaId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.chapType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.useHp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.monarchLevel = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.frontPoint = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.openCondition = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.money = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.exp = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.awardId = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.firstAwardId = t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.npcId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.bossId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				//landId

				t_reader.MoveToNextAttribute();
				t_template.power = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.star1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.star2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.star3 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.soundId = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.sceneId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.time = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.RenWuLimit = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.PowerLimit = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.configId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.TeammateID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.juqingNPC = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.wanfaType =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.recZhanli = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.recMibaoSkill =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.OPenSkillLabel =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.bossIcon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.bubble =  t_reader.Value ;

				t_reader.MoveToNextAttribute();
				t_template.BigIcon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.enddingRemind = t_reader.Value;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static PveTempTemplate GetPVETemplate( int p_section, int p_level ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){
			PveTempTemplate t_item = templates[ i ];

			if( t_item.bigId == p_section && t_item.smaId == p_level ){
				return t_item;
			}
		}

		//Debug.LogError( "PveTempTemplate not found: " + p_section + ", " + p_level );

		return null;
	}

	public static List<int> GetAutoAwardItemIds( string p_award_id ){
		{
			ProcessAsset();	
		}

		List<int> t_items = new List<int>();

		char[] t_items_delimiter = { ',' };

		char[] t_item_id_delimiter = { '=' };

		string[] t_item_strings = p_award_id.Split( t_items_delimiter );

		for( int i = 0; i < t_item_strings.Length; i++ ){
			string t_item = t_item_strings[ i ];

			string[] t_finals = t_item.Split( t_item_id_delimiter );

			t_items.Add( int.Parse( t_finals[ 0 ] ) );
		}

		return t_items;
	}

	public static bool IsEssenceLevel( int p_section_num, int p_level_num ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){
			PveTempTemplate t_item = templates[ i ];

			if( t_item.bigId == p_section_num && t_item.smaId == p_level_num ){
				return t_item.chapType == 1 ? true : false;
			}
		}

		Debug.LogError( p_section_num + " - " + p_level_num + " : not found." );

		return false;
	}

	public static int GetRequiredLevel( int p_section_num, int p_level_num ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){
			PveTempTemplate t_item = templates[ i ];
			
			if( t_item.bigId == p_section_num && t_item.smaId == p_level_num ){
				return t_item.monarchLevel;
			}
		}
		
		Debug.LogError( p_section_num + " - " + p_level_num + " : not found." );
		
		return -1;
	}

	public static List<PveTempTemplate> GetTemplates(){
		{
			ProcessAsset();	
		}

		return templates;
	}

	public static int GetTemplatesCount(){
		{
			ProcessAsset();	
		}

		return templates.Count;
	}

	public static PveTempTemplate GetPveTemplate_By_index( int p_index ){
		{
			ProcessAsset();	
		}

		return templates[ p_index ];
	}

	public static PveTempTemplate GetPveTemplate_By_id( int p_level_id ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){
			PveTempTemplate t_item = templates[ i ];
			
			if( t_item.id == p_level_id ){
				return t_item;
			}
		}
		
		Debug.LogError( p_level_id + " : not found." );
		
		return null;
	}

	
	public static string LevelName_By_id( int p_level_id ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){

			PveTempTemplate t_item = templates[ i ];
			
			if( t_item.id == p_level_id ){

				string m_Name = NameIdTemplate.GetName_By_NameId(t_item.smaName);

				return m_Name;
			}
		}
		
		Debug.LogError( p_level_id + " : not found." );
		
		return null;
	}

	public static PveTempTemplate GetPveTemplate_By_Chapter_Id( int Chapter_Id ){
		{
			ProcessAsset();	
		}

		for( int i = 0; i < templates.Count; i++ ){
			PveTempTemplate t_item = templates[ i ];
			
			if( t_item.bigId == Chapter_Id ){
				return t_item;
			}
		}
		
		Debug.LogError( Chapter_Id + " : not found." );
		
		return null;
	}
    public static List<int> GetPveStarsIdByLevelId(int level_id){
		{
			ProcessAsset();	
		}

        List<int> listId = new List<int>();
        for (int i = 0; i < templates.Count; i++)
        {
            PveTempTemplate t_item = templates[i];

            if (t_item.id == level_id)
            {
                listId.Add(t_item.star1);
                listId.Add(t_item.star2);
                return listId;
            }
        }

		Debug.LogError( level_id + " : not found." );

        return null;
    }

    public static PveTempTemplate GetPveTemplateGuanQiaId(int _Id){
		{
			ProcessAsset();	
		}

        for (int i = 0; i < templates.Count; i++)
        {
            PveTempTemplate t_item = templates[i];

            if (t_item.id ==  _Id)
            {
                return t_item;
            }
        }

        Debug.LogError(_Id + " : not found.");

        return null;
    }
}
