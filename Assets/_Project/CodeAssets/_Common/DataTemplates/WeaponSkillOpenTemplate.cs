using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class WeaponSkillOpenTemplate : XmlLoadManager
{
	//<WeaponSkillOpen skill_light_1="101" skill_light_2="203" skill_heavy_1="301" skill_heavy_2="303" 
	//skill_ranged_1="401" skill_ranged_2="401" />

	public bool b_skill_light_1;
	
	public bool b_skill_light_2;
	
	public bool b_skill_heavy_1;
	
	public bool b_skill_heavy_2;
	
	public bool b_skill_ranged_1;

	public bool b_skill_ranged_2;

	public bool b_dodge;


	private int skill_light_1;

	private int skill_light_2;

	private int skill_heavy_1;

	private int skill_heavy_2;

	private int skill_ranged_1;

	private int skill_ranged_2;

	private int dodge;


	public static List<WeaponSkillOpenTemplate> templates = new List<WeaponSkillOpenTemplate> ();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "WeaponSkillOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "WeaponSkillOpen" );
			
			if( !t_has_items ){
				break;
			}
			
			WeaponSkillOpenTemplate t_template = new WeaponSkillOpenTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.skill_light_1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill_light_2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill_heavy_1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill_heavy_2 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill_ranged_1 = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.skill_ranged_2 = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dodge = int.Parse( t_reader.Value );
			}
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static WeaponSkillOpenTemplate getWeaponSkillTemplate(int maxLevel)
	{
		WeaponSkillOpenTemplate template = templates [0];

		template.b_skill_light_1 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.jueyingxingguangzhan] > 0 && (maxLevel >= template.skill_light_1);

		template.b_skill_light_2 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.xuejilaoyin] > 0 && (maxLevel >= template.skill_light_2);

		template.b_skill_heavy_1 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.bahuanglieri] > 0 && (maxLevel >= template.skill_heavy_1);

		template.b_skill_heavy_2 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.qiankundouzhuan] > 0 && (maxLevel >= template.skill_heavy_2);

		template.b_skill_ranged_1 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.zhuixingjian] > 0 && (maxLevel >= template.skill_ranged_1);

		template.b_skill_ranged_2 = BattleControlor.Instance().getKing().skillLevel [(int)CityGlobalData.skillLevelId.hanbingjian] > 0 && (maxLevel >= template.skill_ranged_2);

		template.b_dodge = (maxLevel >= template.dodge);
		
		return template;
	}

}
