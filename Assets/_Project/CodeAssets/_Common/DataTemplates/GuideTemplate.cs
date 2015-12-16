using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GuideTemplate : XmlLoadManager
{
	//<Guide id="100101" levelType="0" dungeonId="1" tiggerType="1" tp1="0" tp2="0" tp3="0" reTriggerAble="0,0" delay="0" actionType="0" 
	//ap1="10101" ap2="0" ap3="0" pause="1" delTarget="0" />


	public int id;

	public int levelType;

	public int dungeonId;
	
	public int triggerType;
	
	public int tp1;
	
	public int tp2;

	public int tp3;

	public bool retriggerable;

	public bool retriggerableCurBattle;

	public float delay;

	public int actionType;

	public int ap1;
	
	public int ap2;
	
	public string ap3;

	public int pause;

	public int delTarget;

	public List<int> flagId;
	
	private static List<GuideTemplate> templates = new List<GuideTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Guide.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}

	private static TextAsset m_templates_text = null;
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		if ( obj == null) {
			Debug.LogError ("Asset Not Exist: " + path );
			
			return;
		}
		
		m_templates_text = obj as TextAsset;
	}

	private static void ProcessAsset(){
		if(  templates.Count > 0 ) {
			return;
		}
		
		if( m_templates_text == null ) {
			//Debug.LogError( "Error, Asset Not Exist." );
			
			return;
		}
		
		XmlReader t_reader = XmlReader.Create( new StringReader( m_templates_text.text ) );

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "Guide" );
			
			if( !t_has_items ){
				break;
			}

			GuideTemplate t_template = new GuideTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.levelType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dungeonId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.triggerType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.tp1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.tp2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.tp3 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				string strRetrigger = t_reader.Value;
				string[] strsRetrigger = strRetrigger.Split(',');
				
				t_template.retriggerable = int.Parse( strsRetrigger[1] ) != 0;
				
				t_template.retriggerableCurBattle = int.Parse( strsRetrigger[0] ) != 0;

				t_reader.MoveToNextAttribute();
				t_template.delay = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.actionType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.ap1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ap2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ap3 = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.pause = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.delTarget = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.flagId = new List<int>();
				string flagIdStr = t_reader.Value;
				string[] flagIds = flagIdStr.Split(',');
				foreach(string flagIdS in flagIds)
				{
					int id = int.Parse(flagIdS);

					if(id == 0) continue;

					t_template.flagId.Add(id);
				}		
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );

		{
			m_templates_text = null;
		}
	}

	public static bool HaveId( int p_level_id, int p_event_id ){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		bool t_have = false;

		foreach(GuideTemplate template in templates){
			if( template.dungeonId == p_level_id && template.id == p_event_id && template.levelType == levelType){
				t_have = true;

				return t_have;
			}
		}

		return t_have;
	}

	public static bool HaveId_type( int p_level_id, int p_type ){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		bool t_have = false;
		
		foreach(GuideTemplate template in templates){
			if( template.dungeonId == p_level_id && template.triggerType == p_type && template.levelType == levelType)
			{
				t_have = true;
				
				return t_have;
			}
		}
		
		return t_have;
	}

	public static bool HaveId_type( int p_level_id, int p_type, int p_skillType ){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		bool t_have = false;
		
		foreach(GuideTemplate template in templates)
		{
			if( template.dungeonId == p_level_id && template.triggerType == p_type && template.tp1 == p_skillType && template.levelType == levelType)
			{
				t_have = true;

				return t_have;
			}
		}
		
		return t_have;
	}

	public static GuideTemplate getTemplateByLevelAndEvent(int levelId, int eventId){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		if (levelType == 1) return null;

		foreach(GuideTemplate template in templates)
		{
			if(template.dungeonId == levelId && template.id == eventId && template.levelType == levelType)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get GuideTemplate with level " + levelId + ", and eventId " + eventId);

		return null;
	}

	public static GuideTemplate getTemplateByLevelAndType(int levelId, int type){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		foreach(GuideTemplate template in templates)
		{
			if(template.dungeonId == levelId && template.triggerType == type && template.levelType == levelType)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get GuideTemplate with level " + levelId + ", and type " + type);
		
		return null;
	}

	public static GuideTemplate getTemplateByLevelAndType(int levelId, int type, int skillType){
		{
			ProcessAsset();
		}

		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		foreach(GuideTemplate template in templates)
		{
			if(template.dungeonId == levelId && template.triggerType == type && template.tp1 == skillType && template.levelType == levelType)
			{
				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get GuideTemplate with level " + levelId + ", and type " + type + ", and para2 " + skillType);

		return null;
	}

	public static List<GuideTemplate> GetTemplates(){
		{
			ProcessAsset();
		}

		return templates;
	} 
}
