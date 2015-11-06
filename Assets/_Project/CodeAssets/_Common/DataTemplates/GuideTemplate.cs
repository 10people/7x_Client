using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class GuideTemplate : XmlLoadManager
{
	//<Guide id="100101" levelType="0" eventId="1" type="1" para1="0" para2="0" content="0" 
	//delay="0" cameraTarget="xxxxx" cameraPx="-0.000001636374" cameraPy="5.826938" cameraPz="7.066081" 
	//cameraRx="30.01426" cameraRy="180" pName="xxxxx" pText="这就到顶层了？[00B0F0]千重楼[-]也不过如此!" 
	//flagId="0" icon="0" pause="1" differentiate="0" position="1" desc="触发对话" />


	public int id;

	public int levelType;

	public int eventId;
	
	public int type;
	
	public int para1;
	
	public int para2;

	public int content;

	public float delay;

	public string cameraTarget;

	public float cameraPx;

	public float cameraPy;

	public float cameraPz;

	public float cameraRx;

	public float cameraRy;

	public string pName;

	public string pText;

	public List<int> flagId;
	
	public int icon;
	
	public int pause;

	public int differentiate;

	public int position;

	public bool retriggerable;

	public bool retriggerableCurBattle;

	public string desc;

	public int forwardFlagId;

	
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
			Debug.LogError( "Error, Asset Not Exist." );
			
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
				t_template.eventId = int.Parse( t_reader.Value );

				//Debug.Log("GGGGGGGGGGGuide " + t_template.id + ", " + t_template.eventId);

				t_reader.MoveToNextAttribute();
				t_template.type = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.para1 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.para2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.content = int.Parse( t_reader.Value );
		
				t_reader.MoveToNextAttribute();
				t_template.delay = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraTarget = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.cameraPx = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraPy = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraPz = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraRx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.cameraRy = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.pName = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.pText = t_reader.Value;

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

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.pause = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.differentiate = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.position = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				string strRetrigger = t_reader.Value;
				string[] strsRetrigger = strRetrigger.Split(',');

				t_template.retriggerable = int.Parse( strsRetrigger[1] ) != 0;

				t_template.retriggerableCurBattle = int.Parse( strsRetrigger[0] ) != 0;

				t_reader.MoveToNextAttribute();
				t_template.desc = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.forwardFlagId = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
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
			if( template.id == p_level_id && template.eventId == p_event_id && template.levelType == levelType){
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
			if( template.id == p_level_id && template.type == p_type && template.levelType == levelType)
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
			if( template.id == p_level_id && template.type == p_type && template.para2 == p_skillType && template.levelType == levelType)
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
			if(template.id == levelId && template.eventId == eventId && template.levelType == levelType)
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
			if(template.id == levelId && template.type == type && template.levelType == levelType)
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
			if(template.id == levelId && template.type == type && template.para2 == skillType && template.levelType == levelType)
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
