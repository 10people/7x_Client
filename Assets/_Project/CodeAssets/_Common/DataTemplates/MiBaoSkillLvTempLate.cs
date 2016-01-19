using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class MiBaoSkillLvTempLate : XmlLoadManager {

	public int id;
	
	public int lv;
	
	public string skill;
	
	public int skillDesc;
	
	public int skill2;
	
	public int skill2Desc;
	
	public static List<MiBaoSkillLvTempLate> templates = new List<MiBaoSkillLvTempLate>();
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "MibaoSkillLv.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
		
		//		Debug.Log ("j加载了ibaoSuiPian.xml");
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
			t_has_items = t_reader.ReadToFollowing( "MibaoSkillLv" );
			
			if( !t_has_items ){
				break;
			}
			
			MiBaoSkillLvTempLate t_template = new MiBaoSkillLvTempLate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.lv = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.skillDesc = int.Parse(t_reader.Value);
				
				t_reader.MoveToNextAttribute();
				t_template.skill2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill2Desc = int.Parse(t_reader.Value);
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static MiBaoSkillLvTempLate GetMiBaoSkillLvTemplateByIdAndLevel (int skillId,int skillLevel)
	{
		foreach (MiBaoSkillLvTempLate template in templates)
		{
			if (template.id == skillId && template.lv == skillLevel)
			{
				return template;
			}
		}

		Debug.LogError ("Can not Get MiBaoSkillLvTemplate by skillId:" + skillId + "or skillLevel:" + skillLevel);

		return null;
	}
}
