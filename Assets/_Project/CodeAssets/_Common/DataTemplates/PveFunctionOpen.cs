using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PveFunctionOpen : XmlLoadManager
{
	//-1:不显示 
	//0: 通关一次后显示
	//1: 显示

	//<PveFunctionOpen id="100000" joystick="1" attack="1" skill_1="1" skill_2="1" 
	//changeWeapon="1" skill_miBao="1" autoFight="0" pause="0" />


	public int id;

	public int levelType;

	public bool joystick;
	
	public bool attack;
	
	public bool skill_light_1;
	
	public bool skill_light_2;
	
	public bool skill_heavy_1;
	
	public bool skill_heavy_2;
	
	public bool skill_ranged_1;
	
	public bool skill_ranged_2;
	
	public bool weaponHeavy;

	public bool weaponLight;

	public bool weaponRange;
	
	public bool skill_miBao;
	
	public bool autoFight;
	
	public bool pause;

	public bool dodge;

	public int defaultWeapon;

	public bool broadcastable;

	
	public int joystick_eff;
	
	public int attack_eff;
	
	public int skill_light_1_eff;
	
	public int skill_light_2_eff;
	
	public int skill_heavy_1_eff;
	
	public int skill_heavy_2_eff;
	
	public int skill_ranged_1_eff;
	
	public int skill_ranged_2_eff;
	
	public int weaponHeavy_eff;
	
	public int weaponLight_eff;
	
	public int weaponRange_eff;
	
	public int skill_miBao_eff;
	
	public int autoFight_eff;
	
	public int pause_eff;
	
	public int dodge_eff;


	private int t_joystick;
	
	private int t_attack;
	
	private int t_skill_light_1;
	
	private int t_skill_light_2;
	
	private int t_skill_heavy_1;
	
	private int t_skill_heavy_2;
	
	private int t_skill_ranged_1;
	
	private int t_skill_ranged_2;
	
	private int t_weapon_heavy;

	private int t_weapon_light;

	private int t_weapon_range;

	private int t_skill_miBao;
	
	private int t_autoFight;
	
	private int t_pause;

	private int t_dodge;


	public static List<PveFunctionOpen> templates = new List<PveFunctionOpen>();


	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PveFunctionOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
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
			t_has_items = t_reader.ReadToFollowing( "PveFunctionOpen" );
			
			if( !t_has_items ){
				break;
			}
			
			PveFunctionOpen t_template = new PveFunctionOpen();

			{
				t_reader.MoveToNextAttribute();
				t_template.id = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.levelType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				string strJoystick = t_reader.Value;
				string[] strsJoystick = strJoystick.Split(',');
				t_template.t_joystick = int.Parse( strsJoystick[0] );
				t_template.joystick_eff = int.Parse(strsJoystick[1]);

				t_reader.MoveToNextAttribute();
				string strAttack = t_reader.Value;
				string[] strsAttack = strAttack.Split(',');
				t_template.t_attack = int.Parse( strsAttack[0] );
				t_template.attack_eff = int.Parse(strsAttack[1]);

				t_reader.MoveToNextAttribute();
				string strSkill_light_1 = t_reader.Value;
				string[] strsSkill_light_1 = strSkill_light_1.Split(',');
				t_template.t_skill_light_1 = int.Parse( strsSkill_light_1[0] );
				t_template.skill_light_1_eff = int.Parse( strsSkill_light_1[1] );
				
				t_reader.MoveToNextAttribute();
				string strSkill_light_2 = t_reader.Value;
				string[] strsSkill_light_2 = strSkill_light_2.Split(',');
				t_template.t_skill_light_2 = int.Parse( strsSkill_light_2[0] );
				t_template.skill_light_2_eff = int.Parse( strsSkill_light_2[1] );
				
				t_reader.MoveToNextAttribute();
				string strSkill_heavy_1 = t_reader.Value;
				string[] strsSkill_heavy_1 = strSkill_heavy_1.Split(',');
				t_template.t_skill_heavy_1 = int.Parse( strsSkill_heavy_1[0] );
				t_template.skill_heavy_1_eff = int.Parse( strsSkill_heavy_1[1] );
				
				t_reader.MoveToNextAttribute();
				string strSkill_heavy_2 = t_reader.Value;
				string[] strsSkill_heavy_2 = strSkill_heavy_2.Split(',');
				t_template.t_skill_heavy_2 = int.Parse( strsSkill_heavy_2[0] );
				t_template.skill_heavy_2_eff = int.Parse( strsSkill_heavy_2[1] );

				t_reader.MoveToNextAttribute();
				string strSkill_ranged_1 = t_reader.Value;
				string[] strsSkill_ranged_1 = strSkill_ranged_1.Split(',');
				t_template.t_skill_ranged_1 = int.Parse( strsSkill_ranged_1[0] );
				t_template.skill_ranged_1_eff = int.Parse( strsSkill_ranged_1[1] );
				
				t_reader.MoveToNextAttribute();
				string strSkill_ranged_2 = t_reader.Value;
				string[] strsSkill_ranged_2 = strSkill_ranged_2.Split(',');
				t_template.t_skill_ranged_2 = int.Parse( strsSkill_ranged_2[0] );
				t_template.skill_ranged_2_eff = int.Parse( strsSkill_ranged_2[1] );
				
				t_reader.MoveToNextAttribute();
				string strWeapon_heavy = t_reader.Value;
				string[] strsWeapon_heavy = strWeapon_heavy.Split(',');
				t_template.t_weapon_heavy = int.Parse( strsWeapon_heavy[0] );
				t_template.weaponHeavy_eff = int.Parse( strsWeapon_heavy[1] );

				t_reader.MoveToNextAttribute();
				string strWeapon_light = t_reader.Value;
				string[] strsWeapon_light = strWeapon_light.Split(',');
				t_template.t_weapon_light = int.Parse( strsWeapon_light[0] );
				t_template.weaponLight_eff = int.Parse( strsWeapon_light[1] );

				t_reader.MoveToNextAttribute();
				string strWeapon_range = t_reader.Value;
				string[] strsWeapon_range = strWeapon_range.Split(',');
				t_template.t_weapon_range = int.Parse( strsWeapon_range[0] );
				t_template.weaponRange_eff = int.Parse( strsWeapon_range[1] );

				t_reader.MoveToNextAttribute();
				string strSkill_mibao = t_reader.Value;
				string[] strsSkill_mibao = strSkill_mibao.Split(',');
				t_template.t_skill_miBao = int.Parse( strsSkill_mibao[0] );
				t_template.skill_miBao_eff = int.Parse( strsSkill_mibao[1] );

				t_reader.MoveToNextAttribute();
				string strAutoFight = t_reader.Value;
				string[] strsAutoFight = strAutoFight.Split(',');
				t_template.t_autoFight = int.Parse( strsAutoFight[0] );
				t_template.autoFight_eff = int.Parse( strsAutoFight[1] );
				
				t_reader.MoveToNextAttribute();
				string strPause = t_reader.Value;
				string[] strsPause = strPause.Split(',');
				t_template.t_pause = int.Parse( strsPause[0] );
				t_template.pause_eff = int.Parse( strsPause[1] );

				t_reader.MoveToNextAttribute();
				string strDodge = t_reader.Value;
				string[] strsDodge = strDodge.Split(',');
				t_template.t_dodge = int.Parse( strsDodge[0] );
				t_template.dodge_eff = int.Parse( strsDodge[1] );

				t_reader.MoveToNextAttribute();
				t_template.defaultWeapon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.broadcastable = int.Parse( t_reader.Value ) != 0;
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}

	public static PveFunctionOpen getPveFunctionOpenByIdWithoutRefresh(int id)
	{
		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;
		
		foreach(PveFunctionOpen template in templates)
		{
			if(template.id == id && template.levelType == levelType)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get PveFunctionOpen with id " + id);
		
		return null;
	}

	public static PveFunctionOpen getPveFunctionOpenById(int id)
	{
		int levelType = CityGlobalData.m_levelType == qxmobile.protobuf.LevelType.LEVEL_TALE ? 1 : 0;

		foreach(PveFunctionOpen template in templates)
		{
			if(template.id == id && template.levelType == levelType)
			{
				template.refreshData();

				return template;
			}
		}

		Debug.LogError("XML ERROR: Can't get PveFunctionOpen with id " + id);

		return null;
	}

	private void refreshData()
	{
		bool dramable = CityGlobalData.getDramable ();

		joystick = refreshUnitData (t_joystick, dramable);
		
		attack = refreshUnitData (t_attack, dramable);
		
		skill_light_1 = refreshUnitData (t_skill_light_1, dramable);
		
		skill_light_2 = refreshUnitData (t_skill_light_2, dramable);

		skill_heavy_1 = refreshUnitData (t_skill_heavy_1, dramable);
		
		skill_heavy_2 = refreshUnitData (t_skill_heavy_2, dramable);

		skill_ranged_1 = refreshUnitData (t_skill_ranged_1, dramable);
		
		skill_ranged_2 = refreshUnitData (t_skill_ranged_2, dramable);

		skill_miBao = refreshUnitData (t_skill_miBao, dramable);

		weaponHeavy = refreshUnitData (t_weapon_heavy, dramable);

		weaponLight = refreshUnitData (t_weapon_light, dramable);

		weaponRange = refreshUnitData (t_weapon_range, dramable);

		autoFight = refreshUnitData (t_autoFight, dramable);
		
		pause = refreshUnitData (t_pause, dramable);

		dodge = refreshUnitData (t_dodge, dramable);
	}

	private bool refreshUnitData(int i, bool dramable)
	{
		if (i == -1) return false;
		
		else if (i == 1) return true;
		
		else if (i == 0) return !dramable;

		Debug.LogError("XML ERROR: Can't parse to bool by int " + i);

		return false;
	}

}
