using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class Res2DTemplate : XmlLoadManager
{
    //<EffectTemplate  effectId="1" path="_3D/Fx/Prefabs/BattleEffect/GuangQiang" />

    public enum Res
    {
        NETWORK_RECONNECT_TIPS = 1,
        NETWORK_WAITING_TIPS,
        PUBLIC_ANNOUNCEMENT,
        SELECT_ROLE,
        LOGIN,
        LOGIN_SERVER_ITEM,
        LOGIN_SERVER_TWO_COL_LEFT_ITEM,
        LOGIN_SERVER_TWO_COL_RIGHT_ITEM,
        MAINCITY_MAINUI,
        YINDAO_DRAMA_AVATAR_PREFIX,
        GAME_ACTIVITY,
        JUN_ZHU_UPGRADE_LAYER,
        JUN_ZHU_DROP_ITEM,
        PVE_BUY_TI_LI,
        PVE_BUY_TI_LI_NO_CHANCE,
        PVE_NOT_ENOUGH_YUAN_BAO,
        PVE_TYPE_ICE,
        PVE_TYPE_FIRE,
        PVE_TYPE_LEI,
        PVE_TYPE_TIAN_DAO,
        PVE_TYPE_GUI,
        JUN_ZHU_TECH_ITEM,
        PVE_REMIND_2,
        PVE_REMIND_1,
        PVE_LEVEL_REMIND,
        PVE_UI,
        PVE_SAO_DANG_DONE,
        PVE_LEVEL_GRADE,
        PVE_GRADE_REWARD,
        PVE_SAO_DANG,
        PVE_CHOOSE_CHAPTER,
        PVE_ENEMY_1,
        PVE_ENEMY_NAME,
        PVE_SAODANG_LEVEL,
        PVE_NO_TI_LI,
        PVE_CANT_SAO_DANG_REMAIN,
        PVE_MAP_PREFIX,
        PVE_CLOUD,
        PVP_ZHENG_RONG,
        PVP_PAGE,
        TANBAO_ZHU_RONG_QING,
        TANBAO_ZHU_RONG_QI,
        TANBAO_ZHU_RONG_SHAN,
        TASK_SCROLL_VIEW_ITEM_AMEND,
        TASK_ITEM_AWARD_ITEM,
        INTENSIFY_EQUIP_GROWTH_AMEND,
        BAG_EQUIP_OF_BAG_ITEM,
        BAG_BAG_LAYER,
        PAWN_SHOP,
        TAN_BAO,
        TASK_LAYER_AMEND = 51,
        MI_BAO_SECRET,
        JUN_ZHU_LAYER_AMEND,
        SHOP_QX_SHOP,
        INTENSIFY_MATERIAL_ITEM = 55,
        MI_BAO_REMIND_MI_BAO,
        MI_BAO_CARD_TEMP,
        MI_BAO_ADD_STAR,
        MI_BAO_NOT_ENOUGH_PIECE,
        PVE_SECRET_CARD_TEMP,
        MI_BAO_START_MADE,
        MI_BAO_SKILL_GROUP,
        MI_BAO_MI_BAO,
        EQUIP_ICON_PREFIX,
        DRILL_CARD_PREFIX,
        BATTLE_FIELD_STORY_BOARD_PREFIX,
        DRILL_CARD_SMALL_PREFIX,
        JUN_ZHU_EQUIP_SCROLL_VIEW_ITEM,
        JUN_ZHU_JING_MAI_ITEM,
        GLOBAL_DIALOG_BOX,
        PVP_CHOOSE_MI_BAO,
        ALLIANCE_NO_SELF_ALLIANCE,
        ALLIANCE_NO_SELF_ALLIANCE_ITEM,
        ALLIANCE_NO_SELF_ALLIANCE_ICON_ITEM,
        TANBAO_NV_WA_PEI,
        TANBAO_XUAN_YUAN_TU,
        WORSHIP_MAIN_LAYER = 77,
        WORSHIP_YUJUE_ITEM,
        WORSHIP_EFFECT_ITEM,
        WORSHIP_REWARD_MOVE_ITEM,
        LOGIN_CREATE_ROLE,
        BATTLEFIELD_V4_2D_UI,
        BATTLEFIELD_V4_3D_ROOT,
        HAOJIE_CREATE_ROLE,
        RUYA_CREATE_ROLE,
        YUJIE_CREATE_ROLE,
        LUOLI_CREATE_ROLE,
        GLOBAL_DIALOG,
        GLOBAL_YINDAO,
        UI_PANEL_BAG,
        UI_PANEL_TANBAO,
        UI_PANEL_TASK,
        UI_PANEL_SECRET,
        UI_PANEL_EQUIPGROWTHAMEND,
        UI_PANEL_PAWNSHOP,
        PVP_DUI_HUAN,
        TASK_AWARD_ITEM_BIG,
        TASK_AWARD_ITEM_SMALL,
        ALLIANCE_CHECKMEMBERS,
        ALLIANCE_APPLY = 100,
        ALLIANCE_LEADER_SETTINGS,
        ALLIANCE_TRANS,
        ALLIANCE_RECRUIT,
        ALLIANCE_ELECTION,
        ALLIANCE_ELECTION_INFO,
        LOGIN_BACKGROUND,
        MAPPIC,
        ALLIANCE_NOT_HAVE_ROOT,
        ALLIANCE_HAVE_ROOT,
        FIGHT_TYPE_SELECT,
        DAILY_REWARD,
        DAILY_REWARD_ITEM,
        MAINCITY_PLAYER_NAME,
        UI_MAINCITY_NPC_TALK,
        TIAOZHAN_RECORD,
        UI_CHATITEM = 116,
        UI_CHAT_WINDOW,
        UI_CHATITEM_OTHER = 118,
        PVE_MIBAO_SHANGZHEN,
        SETTINGS_UP_LAYER,
        SETTINGS_UP_BLOCKED_ITEM,
        HUANGYE_ZHENGROONG,
        CHAT_MAIL_SENDER,
        MAIN_CITY_TIP = 124,
        UI_CHATITEM_SELF = 125,
        HOUSE_SECRET_CARD_SHOWER = 126,
        HOUSE_VISITOR,
        HOUSE_WEAPON_SHOWER,
        OLD_BOOK_ITEM_SELF,
        OLD_BOOK_ITEM_OTHER,
        OLD_BOOK_WINDOW,
        SMALL_HOUSE_SELF,
        SMALL_HOUSE_OTHER = 133,
        TOPUP_MAIN_LAYER,
        TOPUP_ITEM,
        TOPUP_TEQUAN_ITEM,
        UI_LOADING_ROOT,
        RANK_LIST,
        HYTREASURE_UI,//荒野藏宝点整容
        HY_REWARD_LIBRARY,
        BAIZHAN_MAIN_PAGE,
        BIG_HOUSE_SELF,
        BIG_HOUSE_OTHER,
        HY_MAP,
        NEW_ZHENGRONG,
        BUY_TONGBI_BACKUI,
        FRIEND_OPERATION = 147,
        FRIEND_OPERATION_ITEM = 148,
        ACTIVITY_LAYER = 149,
        ACTIVITY_ITEM = 150,
        SIGNAL_ITEM = 151,
        BATTLE_RESULT = 152,
        UI_3D_EFFECT = 153,
        MIBAO_BIGICON = 154,
        CLOUD = 155,
        EMAIL = 156,
        SCENE_GUIDE = 157,
        HIGHEST_UI = 158,
        SPOT_POINT = 159,
        CARRIAGE = 160,
        CARRIAGE_UI_ITEM = 161,
        CHOOSEYOUXIA = 162,
        POP_UP_LABEL_TOOL = 163,
        YOUXIA = 164,
        YOUXIAENEMY_UI = 165,
        LABEL_EFFECT = 166,
        YUNBIAO_MAIN_PAGE = 167,
        JIEBIAO_MAIN_PAGE = 168,
        SELECT_HORSE = 169,
        YUNBIAO_ENEMIES = 170,
        YUNBIAO_RECORD = 171,
        ONLINE_REWARD_ITEM = 172,
        ONLINE_REWARD_ROOT = 173,
        YOUXIABIGBG = 174,
        LUEDUO = 175,
        LUEDUO_RECORD = 176,
        FLOAT_BUTTON = 177,
        RANK_WINDOW = 178,
        NATION_ROOT = 179,
        NATION_RANK_ITEM = 180,
        GENERAL_CHALLENGE_PAGE = 181,
        GENERAL_RULES = 182,
        KING_DETAIL_WINDOW = 183,
        ALLIANCE_MEMBER_WINDOW = 184,
        Alliance_UP = 185,
        REWARD_EFFECT_ROOT = 186,
        EQUIP_MOVE_ITEM = 187,
        EQUIP_SPECIAL_ITEM = 188,
        ALLIANCE_BATTLE_HISTORY = 189,
        ALLIANCE_BATTLE_RULE = 190,
        ALLIANCE_BATTLE_WINDOW = 191,
        FUWEN = 192,
        TASK_EFFECT = 193,
        TIP = 194,
        DAMAGERANK = 195,
        ICON_SAMPLE = 196,
        AUTO_NAV = 197,
		GENERAL_STORE = 198,
        EQUIP_SPECIAL_ITEM2 = 199,
		UI_POP_REWARD_ROOT = 200,
		SHADOW_TEMPLE = 201,
        TAO_ZHUANG_ITEM = 202,
        EQUIPINFO_ITEM = 203,
        GLOBAL_Belongings = 204,
        OTHER_PLAYER_INFO = 205,
        SIGNAL_LAYER = 206,
		QXSHOP = 207,
        ONLINE_SKILL_ITEM = 208,
        ONLINE_REWARD_DAY = 209,
        ONLINE_REWARD_TIMES = 210,
        SWITCH_COUNTRY_ROOT = 211,
        SWITCH_COUNTRY_ITEM = 212,
		WAR_SELECT_PAGE = 213,
		UI_PANEL_JIESUO = 214,
		UI_PANEL_TONGZHI = 215,
		UI_PANEL_FULI = 216,
		UI_PANEL_BUYMONEY = 217,
        TASK_BUTTON_ITEM = 218,
		UI_ADDZHANLI = 219,
		LOADING_BG_FOR_MAINCITY = 220,
		GLOBAL_Titile = 221,
		LOADING_BG_FOR_BATTLE_FIELD = 222,
		UI_FX_SELF_ANIM = 223,
		UI_FX_MIRROR_ANIM = 224,
		UI_FX_RING = 225,


		BIAOJU_RECORD_PAGE = 231,
        EQUIP_ADVANCE = 232,
        EQUIP_TAO = 233,
		
		LOADING_BG_BAI_ZHAN = 234,
		LOADING_BG_MI_JI = 235,
		LOADING_BG_NEXT_DAY = 236,
		LOADING_BG_SHANG_YANG = 237,
		LOADING_BG_SUIT = 238,
		LOADING_BG_TAN_BAO = 239,
		LOADING_BG_WEAPON = 240,

		CHASE_ATTACK_NAV = 241,
		WARSITUATION = 242,
		PRE_LOAD_EQUIP_ATLAS = 243,
		PRE_LOAD_ACTIVITY_ATLAS = 244,
        ALLIANCE_YAOQING_ITEM = 246,
        TREASURE_CITY_UI = 247,
        ALLIANCE_SWITCH_COUNTRY = 248,
		SHOUJI = 250,
		MAXTILI = 251,
		MAIN_CITY_YUNYING = 252,
    }



    public int uiId;

    public string path;

    private static List<Res2DTemplate> m_templates = new List<Res2DTemplate>();


    private static Res2DTemplate m_instance = null;

    public static Res2DTemplate Instance()
    {
        if (m_instance == null)
        {
            m_instance = new Res2DTemplate();
        }

        return m_instance;
    }



    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        //		Debug.Log( "Res2DTemplate.LoadTemplates()" );

        UnLoadManager.DownLoad( PathManager.GetUrl(m_LoadPath + "Res2DTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false );
    }

    public static void CurLoad( ref WWW www, string path, Object obj ){
        if ( m_templates.Count > 0 ){
            return;
        }

        XmlReader t_reader = null;

        if ( obj != null ){
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("Res2DTemplate");

            if (!t_has_items)
            {
                break;
            }

            Res2DTemplate t_template = new Res2DTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.uiId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.path = t_reader.Value;

//				if( t_template.uiId == 4 ){
//					Debug.Log( t_template.uiId + " : " + t_template.path );
//				}
            }

            m_templates.Add( t_template );
        }
        while (t_has_items);

        {
            UtilityTool.LoadBox();
        }
    }

    public static string GetResPath( int p_ui_id ){
		int t_id = p_ui_id;

        for (int i = 0; i < m_templates.Count; i++)
        {
            Res2DTemplate t_template = m_templates[i];

            if (t_template.uiId == t_id)
            {
                return t_template.path;
            }
        }

        Debug.LogError("XML ERROR: Can't get UITemplate with UIs: " + t_id );

        return "";
    }

	public static string GetResPath( Res p_ui_enum ){
		int t_id = (int)p_ui_enum;

		return GetResPath( t_id );
	}
}