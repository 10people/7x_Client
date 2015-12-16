using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EffectTemplate : XmlLoadManager 
{
	//<EffectTemplate  effectId="1" path="_3D/Fx/Prefabs/BattleEffect/GuangQiang" />

	public enum Effects{
		GUANGQIANG_0 = 1,
		EFFECT_SHANPING = 2,
		EFFECT_ATTACKED_DAO = 3,
		EFFECT_ATTACKED_QIANG = 4,
		EFFECT_ATTACKED_GONG = 5,
		EFFECT_KING_ATTACKED_DAO = 6,
		EFFECT_KING_ATTACKED_QIANG = 7,
		EFFECT_KING_ATTACKED_GONG = 8,
		EFFECT_KING_ATTACK_DAO_0 = 9,
		EFFECT_KING_ATTACK_DAO_1 = 10,
		EFFECT_KING_ATTACK_DAO_2 = 11,
		EFFECT_KING_ATTACK_DAO_3 = 12,
		EFFECT_KING_SKILL_DAO_1 = 13,
		EFFECT_KING_SKILL_DAO_2 = 14,
		EFFECT_KING_ATTACK_LIGHT_0 = 15,
		EFFECT_KING_ATTACK_LIGHT_1 = 16,
		EFFECT_KING_ATTACK_LIGHT_2 = 17,
		EFFECT_KING_ATTACK_LIGHT_3 = 18,
		EFFECT_KING_ATTACK_LIGHT_4 = 19,
		EFFECT_KING_ATTACK_LIGHT_5 = 20,
		EFFECT_KING_ATTACK_LIGHT_6 = 21,
		EFFECT_KING_ATTACK_LIGHT_7 = 22,
		EFFECT_KING_ATTACK_LIGHT_8 = 23,
		EFFECT_KING_ATTACK_LIGHT_9 = 24,
		EFFECT_KING_CESHI_LIGHT_1 = 25,
		EFFECT_KING_CESHI_LIGHT_2 = 26,
		EFFECT_KING_CESHI_LIGHT_3 = 27,
		EFFECT_KING_CESHI_LIGHT_4 = 28,
		EFFECT_KING_CESHI_LIGHT_5 = 29,
		EFFECT_KING_CESHI_LIGHT_6 = 30,
		EFFECT_KING_CESHI_LIGHT_SHIZI = 31,
		EFFECT_KING_CESHI_LIGHT_TUCI = 32,
		EFFECT_KING_CESHI_LIGHT_XUANFENGZHAN = 33,
		EFFECT_KING_CESHI_LIGHT_SHANGTIAO_ZUO = 34,
		EFFECT_KING_CESHI_LIGHT_SHANGTIAO_YOU = 35,
		EFFECT_KING_CESHI_LIGHT_QIXUAN = 36,
		EFFECT_KING_CESHI_LIGHT_LUANWU_1 = 37,
		EFFECT_KING_CESHI_LIGHT_LUANWU_2 = 38,
		EFFECT_KING_CESHI_LIGHT_LUANWU_3 = 39,
		EFFECT_KING_CESHI_JUQI = 40,
		EFFECT_KING_CESHI_HEIPING = 41,
		EFFECT_KING_CESHI_SKILL_HUAHEN = 42,
		EFFECT_KING_CESHI_SKILL_CHONGJIBO = 43,
		EFFECT_KING_CESHI_SKILL_QIXUAN = 44,
		EFFECT_KING_CESHI_SKILL_HONGCHA = 45,
		EFFECT_KING_CESHI_SKILL_BAOQI = 46,
		EFFECT_KING_CESHI_SKILL_WUQICHANRAO_LEFT = 47,
		EFFECT_KING_CESHI_SKILL_WUQICHANRAO_RIGHT = 48,
		EFFECT_SKILL_JIANYU = 49,
		BATTLE_FIELD_TO_REPLACE_0 = 50,
		BATTLE_FIELD_TO_REPLACE_1 = 51,
		BATTLE_FIELD_TO_REPLACE_2 = 52,
		BATTLE_FIELD_TO_REPLACE_3 = 53,
		BATTLE_FIELD_TO_REPLACE_6 = 56,
		BATTLE_FIELD_TO_REPLACE_7 = 57,
		BATTLE_FIELD_TO_REPLACE_8 = 58,
		BATTLE_FIELD_TO_REPLACE_9 = 59,
		EFFECT_KING_YI_HUANG_QING_LONG_JUE = 60,
		EFFECT_KING_QIE_HUAN_WU_QI = 61,
		EFFECT_KING_QIAN_LONG_CHU_HAI = 62,
		EFFECT_KING_XU_LI = 63,
		EFFECT_KING_BA_HUANG_LIE_RI = 64,
		EFFECT_WU_QI = 65,
		EFFECT_TIANLEI = 66,
		EFFECT_BIAO_XUE = 67,
		EFFECT_KING_NU_SHE = 68,
		EFFECT_BING_JIAN = 6800,
		EFFECT_FANGYU = 69,
		EFFECT_GONGJI = 70,
		EFFECT_JIA_XUE = 71,
		EFFECT_CHI_YOU_QING_TONG_YIN = 72,
		BATTLE_FIELD_TO_REPLACE_10 = 73,
		BATTLE_FIELD_TO_REPLACE_11 = 74,
		EFFECT_KING_CESHI_HEIPING_2 = 75,
		BATTLE_FIELD_LIGHT_WALL_2 = 76,
		SLASH_NORMAL = 1000,
		SLASH_FIRE = 1010,
		SHUA_GUAI = 3000,
		EFFECT_KING_SHI_XUE = 10100,
		EFFECT_KING_QIAN_LONG_CHU_HAI_IMPACT = 10205,
		EFFECT_KING_HAN_BING_JIAN = 10310,
		EFFECT_KING_HAN_BING_JIAN_IMPACT = 10315,
		BOSS_SKILL_HAN_BING_JIAN_IMPACT = 50000,
		SKILL_LIE_DI_IMPACT = 50010,
		BOSS_SKILL_XUAN_YUN_IMPACT = 50020,
		BOSS_SKILL_SUMMON_FIRE1 = 50030,
		BOSS_SKILL_SUMMON_FIREBALL = 50040,
		UI_FUNCTION_OPEN = 100000,
		UI_RENWU_TISHI = 100010,
		UI_SHENGLI_MID = 100100,
		DOOR_CHUANSONG = 100200,
		DOOR_MYSELF = 100201,
		DOOR_OTHER = 100202,
		SKILL_TIAN_LEI = 260101,
		SKILL_DI_DANG_JI_NENG = 260102,
		BUFF_SUMMON_RED = 260103,
		SKILL_FIRE_100 = 260201,
		SKILL_JI_DAO = 260202,
		BUFF_SUMMON_FIRE_100 = 260203,
		BUFF_ZHEN_SHE = 260301,
		SKILL_ICE_100 = 260302,
		BUFF_SUMMON_GONG_JI_MIAN_YI = 260303,
		BUFF_ZHONG_DU = 260401,
		SKILL_DING_SHEN = 260402,
		SKILL_DI_DANG_FU_MIAN_JI_NENG = 260403,
		SKILL_JI_YUN_100 = 260501,
		SKILL_XI_XUE = 260502,
		BUFF_SUMMON_SUDU_100 = 260503,
		BUFF_SUMMON_GOLD = 260601,
		SKILL_SHAN_DIAN = 260602,
		SKILL_FIRE_200 = 260603,
		SKILL_GHOSTSPAWN = 260701,
		SKILL_SLEEP_100 = 260702,
		SKILL_DI_DANG_KONG_ZHI = 260703,
		SHIELD_LIGHT_1 = 360101,
		SHIELD_LIGHT_2 = 360102,
		SHIELD_LIGHT_3 = 360103,
		SKILL_FIRE_300 = 360202,
	}

	public int effectId;

	public string path;

	public string sound;


//	public static List<EffectTemplate> templates = new List<EffectTemplate>();


//	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
//		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "EffectId.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
//	}

//	public static void CurLoad( ref WWW www, string path, Object obj ){
//		{
//			templates.Clear();
//		}
//
//		XmlReader t_reader = null;
//		
//		if( obj != null ){
//			TextAsset t_text_asset = obj as TextAsset;
//			
//			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
//			
//			//			Debug.Log( "Text: " + t_text_asset.text );
//		}
//		else{
//			t_reader = XmlReader.Create( new StringReader( www.text ) );
//		}
//		
//		bool t_has_items = true;
//		
//		do{
//			t_has_items = t_reader.ReadToFollowing( "EffectTemplate" );
//			
//			if( !t_has_items ){
//				break;
//			}
//			
//			EffectTemplate t_template = new EffectTemplate();
//			
//			{
//				t_reader.MoveToNextAttribute();
//				t_template.effectId = int.Parse( t_reader.Value );
//				
//				t_reader.MoveToNextAttribute();
//				t_template.path = t_reader.Value;
//
//				t_reader.MoveToNextAttribute();
//				t_template.sound = t_reader.Value;
//			}
//			
//			//			t_template.Log();
//			
//			templates.Add( t_template );
//		}
//		while( t_has_items );
//	}

	public static EffectIdTemplate getEffectTemplateByEffectId( int effectId ){
		return EffectIdTemplate.getEffectTemplateByEffectId( effectId );
	}

    public static string GetEffectPathByID(int effectId)
    {
        return EffectIdTemplate.getEffectTemplateByEffectId(effectId).path;
    }

    //	public static EffectTemplate getEffectTemplateByEffectId( int effectId ){
    //		foreach( EffectTemplate template in templates ){
    //			if( template.effectId == effectId ){
    //				return template;
    //			}
    //		}
    //		
    //		Debug.LogError("XML ERROR: Can't get EffectTemplate with effectId " + effectId);
    //		
    //		return null;
    //	}

    public static string GetEffectPath( Effects p_effect_enum ){
		int t_id = (int)p_effect_enum;

		return EffectIdTemplate.GetPathByeffectId( t_id );
	}

}
