using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

public class ProtoIndexStructureTransfer
{
    public const int DEBUG_PROTO_WITHOUT_CONTENT = 100;
    public const int DEBUG_PROTO_WITHOUT_CONTENT_RET = 101;

    public const int TEST_CONN = 10001;
    /**
     * 错误返回信息
     */
    public const int S_ERROR = 10010;
    public const int S_Message = 20000;
    /**
     * 客户端发送聊天
     */
    public const int C_Send_Chat = 20001;
    /**
     * 服务器通知客户端聊天消息
     */
    public const int S_Send_Chat = 20002;

    /**
     * 获取聊天记录。
     */
    public const int C_Get_Chat_Log = 20003;
    public const int S_Send_Chat_Log = 20004;


    /**
     * 国战 Socket C 获取 用户信息
     */
    public const int C_Call_User_Info = 21101;

    /**
     * 国战 Socket S 发送  用户信息
     */
    public const int S_Send_User_Info = 21001;

    /**
     * 国战 Socket C 请求 城池信息 
     */
    public const int C_Call_City_Info = 21102;

    /**
     * 国战 Socket C 请求 城池信息 
     */
    public const int C_Call_City_List_Info = 21103;

    /**
     * 国战 Socket C 玩家移动
     */
    public const int C_Call_User_Move = 21104;

    /**
     * 国战 Socket C 玩家发起攻占、修城动作
     */
    public const int C_Call_User_Action = 21105;

    /**
     * 国战 Socket C 玩家发起攻击
     */
    public const int C_Call_User_Attack = 21106;

    /**
     * 国战 Socket C 玩家请求查看战报或离开
     */
    public const int C_Call_Check_Report = 21107;

    /**
     * 国战 Socket C 玩家请求获取个人奖励
     */
    public const int C_Call_Fetch_Award = 21108;

    /**
     * 国战 Socket S 发送  城池信息
     */
    public const int S_Send_City_Info = 21003;

    /**
     * 国战 Socket S 发送  城池列表信息
     */
    public const int S_Send_City_List_Info = 21007;

    /**
     * 国战 Socket S 发送 城池中用户列表
     */
    public const int S_Send_City_UserList = 21006;

    /**
     * 国战 Socket S 发送 城池状态Map列表
     */
    public const int S_Send_City_State_Maps = 21008;

    /**
     * 国战 Socket S 发送 国家信息
     */
    public const int S_Send_Country = 21009;

    /**
     * 国战 Socket S 发送 国家信息列表
     */
    public const int S_Send_Country_List = 21010;

    /**
     * 国战 Socket S 发送 国战战斗结果
     */
    public const int S_Send_Combat_Result = 21011;

    /**
     * 国战 Socket S 发送 玩家动作执行结果
     */
    public const int S_Send_Action_Result = 21012;

    /**
     * 国战 Socket S 发送 战报记录
     */
    public const int S_Send_Report = 21013;

    /**
     * 国战 Socket S 发送 城池玩家变动信息
     */
    public const int S_Send_City_User_Change = 21014;

    /**
     * 国战 Socket S 发送 领取到的奖励
     */
    public const int S_Send_Personal_Award = 21015;
    /**
     * 国战 Socket S 发送 个人战报
     */
    public const int S_Send_Combat_Record = 21016;

    public const int NationalWarInfoListResId = 21002;
    public const int NationalScheduleResId = 21004;
    public const int NationalScheduleListResId = 21005;

    /**
     * 场景相关协议号
     */
    public const int Enter_Scene = 22000;
    public const int Enter_Scene_Confirm = 22001;
    public const int Spirite_Move = 22002;
    public const int Exit_Scene = 22003;
    public const int Enter_HouseScene = 22004;
    public const int Exit_HouseScene = 22005;

    /**
     * pve相关协议号
     */
    public const int Battle_Pve_Init_Req = 23000;
    public const int B_Hero = 23001;
    public const int B_Soldier = 23002;
    public const int B_Troop = 23003;
    public const int Battle_Init = 23004;

    public const int Battle_Pvp_Init_Req = 23005;
    public const int Battle_Pvp_Init = 23006;

    //账号协议
    public const int ACC_REG = 23101;
    public const int ACC_REG_RET = 23102;
    public const int ACC_LOGIN = 23103;
    public const int ACC_LOGIN_RET = 23104;
    public const int CREATE_ROLE_REQUEST = 23105;
    public const int CREATE_ROLE_RESPONSE = 23106;
    public const int ROLE_NAME_REQUEST = 23107;
    public const int ROLE_NAME_RESPONSE = 23108;
    public const int S_ACC_login_kick = 23110;

    //pve章节协议
    public const int PVE_PAGE_REQ = 23201;
    public const int PVE_PAGE_RET = 23202;

    public const int PVE_BATTLE_OVER_REPORT = 23203;
    public const int PVE_STAR_REWARD_INFO_REQ = 23204;
    public const int PVE_STAR_REWARD_INFO_RET = 23205;
    public const int PVE_STAR_REWARD_GET = 23206;
    public const int PVE_STAR_REWARD_GET_RET = 23207;

    public const int PVE_GuanQia_Request = 23210;
    public const int PVE_GuanQia_Info = 23211;
    public const int C_PVE_Reset_CQ = 23212;
    public const int S_PVE_Reset_CQ = 23213;

    public const int C_PVE_SAO_DANG = 23220;
    public const int S_PVE_SAO_DANG = 23222;

    public const int C_YuanJun_List_Req = 23230;
    public const int S_YuanJun_List = 23231;

    public const int C_BuZhen_Report = 23240;//客户端向服务器发送布阵信息
    public const int C_MIBAO_SELECT = 23241;//客户端向服务器发送秘宝选择信息
    public const int S_MIBAO_SELECT_RESP = 23242;//服务器发送秘宝选择信息

    /** pve战斗请求 **/
    public const int ZHANDOU_INIT_PVE_REQ = 24201;
    /** pvp战斗请求 **/
    public const int ZHANDOU_INIT_PVP_REQ = 24202;

    /** pve、pvp战斗请求返回数据 **/
    public const int ZHANDOU_INIT_RESP = 24151;

    //通关奖励
    public const int BattlePveResult_Req = 23300;
    public const int Award_Item = 23301;
    public const int BattlePve_Result = 23302;

    public const int C_Report_battle_replay = 23310;
    public const int C_Request_battle_replay = 23311;

    public const int PLAYER_STATE_REPORT = 23401;

    public const int C_APP_SLEEP = 23411;//客户端报告进入后台
    public const int C_APP_WAKEUP = 23413;//客户端报告程序进入前台

    public const int PLAYER_SOUND_REPORT = 23501;
    public const int C_get_sound = 23505;
    public const int S_get_sound = 23507;

    public const int JunZhuInfoReq = 23601;
    public const int JunZhuInfoRet = 23602;
    public const int JunZhuAttPoReq = 23603;
    public const int JunZhuAttPoRet = 23604;
    public const int JunZhuAddPoReq = 23605;

    public const int C_KeJiInfo = 23650;
    public const int S_KeJiInfo = 23651;
    public const int C_KeJiUp = 23652;

    public const int C_EquipAdd = 23701;
    public const int C_EquipRemove = 23702;
    public const int S_EquipInfo = 23703;
    public const int S_BagInfo = 23704;
    public const int C_BagInfo = 23705;
    public const int C_EquipInfo = 23706;//装备列表

    public const int C_EquipDetailReq = 23710;
    public const int S_EquipDetail = 23711;

    public const int C_EquipInfoOtherReq = 23712;// 别人装备详情
    public const int S_EquipInfoOther = 23713;

    ////Equip
    //装备列表
    public const int C_EQUIP_LIST = 24001;
    public const int S_EQUIP_LIST = 24002;
    //装备强化
    public const int C_EQUIP_UPGRADE = 24003;
    public const int S_EQUIP_UPGRADE = 24004;
    public const int C_EQUIP_XiLian = 24012;
    public const int S_EQUIP_XiLian = 24013;
    public const int C_EQUIP_JINJIE = 24015;
    public const int S_EQUIP_JINJIE = 24016;


    public const int C_JingMai_info = 24100;
    public const int C_JingMai_up = 24101;
    public const int S_JingMai_info = 24103;

    //mail sys protocol code
    public const int C_REQ_MAIL_LIST = 25003;
    public const int S_REQ_MAIL_LIST = 25004;
    //	public const int  C_DELETE_MAIL = 25005;
    public const int S_DELETE_MAIL = 25006;
    public const int C_MAIL_GET_REWARD = 25007;
    public const int S_MAIL_GET_REWARD = 25008;
    public const int S_MAIL_NEW = 25010;
    public const int C_SEND_EAMIL = 25011;
    public const int S_SEND_EAMIL = 25012;
    public const int C_READED_EAMIL = 25013;
    public const int S_READED_EAMIL = 25014;
    public const int C_EMAIL_RESPONSE = 25015;
    public const int S_EMAIL_RESPONSE = 25016;

    //武将
    public const int HERO_INFO_REQ = 26001;
    public const int HERO_DATA = 26002;
    public const int HERO_INFO = 26003;
    public const int WUJIANG_TECHINFO_REQ = 26004;
    public const int WUJIANG_TECHLEVELUP_REQ = 26005;
    public const int WUJIANG_LEVELUP_REQ = 26006;
    public const int HERO_ACTIVE_REQ = 26007;
    public const int JINGPO_REFRESH_REQ = 26008;			//已废弃
    public const int WUJIANG_TECH_SPEEDUP_REQ = 26009;


    public const int WUJIANG_TECHINFO_RESP = 26054;
    public const int WUJIANG_TECHLEVELUP_RESP = 26055;
    public const int WUJIANG_LEVELUP_RESP = 26056;
    public const int HERO_ACTIVE_RESP = 26057;
    public const int JINGPO_REFRESH_RESP = 26058;
    public const int WUJIANG_TECH_SPEEDUP_RESP = 26059;

    // 百战千军协议类型
    /**请求百战 **/
    public const int BAIZHAN_INFO_REQ = 27001;
    /** **/
    public const int BAIZHAN_INFO_RESP = 27002;
    /** 请求兑换奖励**/
    public const int EXCHAGE_AWARD_REQ = 27003;
    /** **/
    public const int EXCHAGE_AWARD_RESP = 27004;
    /** 请求增加次数**/
    public const int ADD_CHANCE_REQ = 27007;
    /** **/
    public const int ADD_CHANCE_RESP = 27008;
    /**请求 领取奖励（领取按钮） **/
    public const int RECEIVE_AWARD_REQ = 27009;
    /** **/
    public const int RECEIVE_AWARD_RESP = 27010;
    /** 请求 挑战 **/
    public const int CHALLENGE_REQ = 27011;
    /** **/
    public const int CHALLENGE_RESP = 27012;
    /**请求 百战千军中确定做某种事情**/
    public const int CONFIRM_EXECUTE_REQ = 27015;
    /** **/
    public const int CONFIRM_EXECUTE_RESP = 27016;
    /** 前台发送百战千军的结果**/
    public const int BAIZHAN_RESULT = 27017;
    /** 挑战者状态 **/
    public const int PLAYER_STATE_REQ = 27018;
    public const int PLAYER_STATE_RESP = 27019;
    /** 进入战斗 **/
    public const int ENTER_BATTLEFIELD_REQ = 27020;
    /** 27020的返回 **/
    public const int ENTER_BATTLEFIELD_RESP = 27021;
    /** 战斗记录请求 **/
    public const int ZHAN_DOU_RECORD_REQ = 27022;
    /** 战斗记录响应 **/
    public const int ZHAN_DOU_RECORD_RESP = 27023;
    /**是27017的响应页面发送**/
    public const int BAIZHAN_RESULT_RESP = 27024;

    // 刷新挑战对手列表请求
    public const int REFRESH_ENEMY_LIST_REQ = 27026;
    public const int REFRESH_ENEMY_LIST_RESP = 27027;


    public const int C_LM_HOUSE_INFO = 27301;
    public const int S_LM_HOUSE_INFO = 27302;
    public const int S_LM_UPHOUSE_INFO = 27303;//更新房屋信息
    public const int C_HOUSE_EXCHANGE_RQUEST = 27311;
    public const int S_HOUSE_EXCHANGE_RESULT = 27312;
    public const int C_HOUSE_APPLY_LIST = 27313;
    public const int S_HOUSE_APPLY_LIST = 27314;
    public const int C_AnswerExchange = 27321;

    public const int C_Set_House_state = 27303;
    public const int C_EnterOrExitHouse = 27305;
    public const int C_GetHouseVInfo = 27306;//请求访客列表
    public const int C_get_house_exp = 27307;//获取小房子经验
    public const int S_house_exp = 27308;
    public const int C_huan_wu_info = 27309;
    public const int S_huan_wu_info = 27310;
    public const int C_huan_wu_Oper = 27331;
    public const int C_huan_wu_list = 27333;
    public const int S_huan_wu_list = 27334;
    public const int C_huan_wu_exchange = 27337;
    public const int S_huan_wu_exchange = 27338;
    public const int C_ExCanJuanJiangLi = 27341;
    public const int S_ExCanJuanJiangLi = 27342;
    public const int C_up_house = 27343;
    public const int C_Pai_big_house = 27351;
    public const int S_Pai_big_house = 27352;
    public const int C_GET_BIGHOUSE_EXP = 27353;//获取大房子经验
    public const int S_HouseVInfo = 27354;//发送访客列表
    public const int C_ShotOffVisitor = 27355;//踢出访客
    public const int S_ShotOffVisitor = 27356;//访客被踢
    public const int C_EHOUSE_EXCHANGE_RQUEST = 27357;//请求交换空房屋
    public const int S_EHOUSE_EXCHANGE_RESULT = 27358;//请求交换空房屋返回结果
    public const int C_CHANGE_BIGHOUSE_WORTH = 27359;//请求衰减高级房屋价值
    public const int C_CANCEL_EXCHANGE = 27360;//请求撤回交换房屋申请
    public const int S_CANCEL_EXCHANGE = 27361;//请求撤回交换房屋申请返回结果
    public const int C_get_house_info = 27362;//获取自己房子信息
    public const int S_house_info = 27363;//推送房子信息
    //联盟 28001~28099(预计)
    public const int GET_UNION_INFO_REQ = 28001;
    public const int UNION_APPLY_JION_REQ = 28002;
    public const int GET_UNION_FRIEND_INFO_REQ = 28003;
    public const int UNION_EDIT_REQ = 28004;
    public const int UNION_INNER_EDIT_REQ = 28005;
    public const int UNION_OUTER_EDIT_REQ = 28006;
    public const int CREATE_UNION_REQ = 28007;
    public const int UNION_LEVELUP_REQ = 28008;
    public const int UNION_APPLY_REQ = 28009;
    public const int UNION_INVITE_REQ = 28010;
    public const int UNION_INVITED_AGREE_REQ = 28011;
    public const int UNION_INVITED_REFUSE_REQ = 28012;
    public const int UNION_QUIT_REQ = 28013;
    public const int UNION_DISMISS_REQ = 28014;
    public const int UNION_TRANSFER_REQ = 28015;
    public const int UNION_ADVANCE_REQ = 28016;
    public const int UNION_DEMOTION_REQ = 28017;
    public const int UNION_REMOVE_REQ = 28018;
    public const int UNION_DETAIL_INFO_REQ = 28019;

    public const int GET_UNION_INFO_RESP = 28051;
    public const int UNION_APPLY_JION_RESP = 28052;
    public const int GET_UNION_FRIEND_RESP = 28053;
    public const int UNION_EDIT_RESP = 28054;
    public const int UNION_INNER_EDIT_RESP = 28055;
    public const int UNION_OUTER_EDIT_RESP = 28056;
    public const int CREATE_UNION_RESP = 28057;
    public const int UNION_LEVELUP_RESP = 28058;
    public const int UNION_APPLY_RESP = 28059;
    public const int UNION_INVITE_RESP = 28060;
    public const int UNION_INVITED_AGREE_RESP = 28061;
    public const int UNION_INVITED_REFUSE_RESP = 28062;
    public const int UNION_QUIT_RESP = 28063;
    public const int UNION_DISMISS_RESP = 28064;
    public const int UNION_TRANSFER_RESP = 28065;
    public const int UNION_ADVANCE_RESP = 28066;
    public const int UNION_DEMOTION_RESP = 28067;
    public const int UNION_REMOVE_RESP = 28068;
    public const int UNION_DETAIL_INFO = 28069;

    public const int C_get_daily_award_info = 28100;
    public const int S_daily_award_info = 28110;
    public const int C_get_daily_award = 28120;

    // ************  定时请求操作指令 	*************
    /** 玩家定时请求任务 **/
    public const int C_TIMEWORKER_INTERVAL = 28301;
    /** 发送玩家定时请求结果 **/
    public const int S_TIMEWORKER_INTERVAL = 28302;

    //**************  商城指令 	*******************
    //抽卡预留28201~28299
    public const int BUY_CARDBAG_REQ = 28201;
    public const int OPEN_CARDBAG_REQ = 28202;

    public const int BUY_CARDBAG_RESP = 28251;
    public const int OPEN_CARDBAG_RESP = 28252;

    /** 请求宝箱购买信息 **/
    public const int BUY_TREASURE_INFOS_REQ = 28253;
    /** 返回宝箱购买信息 **/
    public const int BUY_TREASURE_INFOS_RESP = 28254;
    /** 购买宝箱 **/
    public const int BUY_TREASURE = 28255;
    /** 返回购买宝箱获得物品信息 **/
    public const int BUY_TREASURE_RESP = 28256;
    /** 请求资源购买信息 **/
    public const int BUY_RESOURCE_INFOS_REQ = 28257;
    /** 返回资源购买信息 **/
    public const int BUY_RESOURCE_INFOS_RESP = 28258;
    /** 返回商城购买失败信息 **/
    public const int PURCHASE_FAIL = 28260;


    /** 请求购买体力和铜币的次数 **/
    public const int C_BUY_TIMES_REQ = 28321;
    /** 返回购买体力个铜币的次数 **/
    public const int S_BUY_TIMES_INFO = 28322;
    /** 购买体力 **/
    public const int C_BUY_TiLi = 28323;
    /** 购买铜币 **/
    public const int C_BUY_TongBi = 28324;
    /** 购买铜币返回 **/
    public const int S_BUY_TongBi = 28325;
    /** 购买秘宝升级点数 **/
    public const int C_BUY_MIBAO_POINT = 28327;
    /** 购买秘宝升级点数返回 **/
    public const int S_BUY_MIBAO_POINT_RESP = 28328;

    //**************  成就指令  ***************
    /** 请求成就列表 **/
    public const int C_ACHE_LIST_REQ = 28331;
    /** 返回成就列表 **/
    public const int S_ACHE_LIST_RESP = 28332;
    /** 成就完成通知 **/
    public const int S_ACHE_FINISH_INFORM = 28334;
    /** 领取成就奖励 **/
    public const int C_ACHE_GET_REWARD_REQ = 28335;
    /** 领取成就奖励返回结果 **/
    public const int S_ACHE_GET_REWARD_RESP = 28336;

    //***************  每日任务指令  *******************
    /** 请求每日任务列表 **/
    public const int C_DAILY_TASK_LIST_REQ = 28341;
    /** 返回每日任务列表 **/
    public const int S_DAILY_TASK_LIST_RESP = 28342;
    /** 每日任务完成通知 **/
    public const int S_DAILY_TASK_FINISH_INFORM = 28344;
    /** 领取每日任务奖励 **/
    public const int C_DAILY_TASK_GET_REWARD_REQ = 28345;
    /** 领取每日任务奖励返回结果 **/
    public const int S_DAILY_TASK_GET_REWARD_RESP = 28346;


    public const int C_BuZhen_Hero_Req = 29401;
    public const int S_BuZhen_Hero_Info = 29402;

    public const int C_TaskReq = 29501;
    public const int S_TaskList = 29502;
    public const int S_TaskSync = 29503;//
    public const int C_GetTaskReward = 29504;//
    public const int S_GetTaskRwardResult = 29505;//
    public const int C_TaskProgress = 29506;//客户端汇报任务进度

    public const int C_YuJueHeChengRequest = 29509;
    public const int S_YuJueHeChengResult = 29510;

    //秘宝协议
    /** 秘宝激活请求 **/
    public const int C_MIBAO_ACTIVATE_REQ = 29601;
    /** 秘宝激活结果返回 **/
    public const int S_MIBAO_ACTIVATE_RESP = 29602;
    /** 秘宝信息请求 **/
    public const int C_MIBAO_INFO_REQ = 29603;
    /** 秘宝信息返回 **/
    public const int S_MIBAO_INFO_RESP = 29604;
    /** 秘宝升级请求 **/
    public const int C_MIBAO_LEVELUP_REQ = 29605;
    /** 秘宝升级结果返回 **/
    public const int S_MIBAO_LEVELUP_RESP = 29606;
    /** 秘宝升星请求 **/
    public const int C_MIBAO_STARUP_REQ = 29607;
    /** 秘宝升星结果返回 **/
    public const int S_MIBAO_STARUP_RESP = 29608;
    /** 别人秘宝信息请求 **/
    public const int C_MIBAO_INFO_OTHER_REQ = 29609;
    /** 别人秘宝信息返回 **/
    public const int S_MIBAO_INFO_OTHER_RESP = 29610;

    //***************  探宝协议  *******************
    /**请求是否显示感叹号**/
    public const int IF_EXPLORE_REQ = 30000;
    /**响应是否感叹号**/
    public const int IF_EXPLORE_RESP = 30001;
    /********探宝页面需要协议**********/
    /**请求矿区主界面**/
    public const int EXPLORE_INFO_REQ = 30002;
    /**响应矿区主界面**/
    public const int EXPLORE_INFO_RESP = 30003;
    /**请求采矿**/
    public const int EXPLORE_REQ = 30004;
    /**响应不可以采矿**/
    public const int EXPLORE_RESP = 30005;
    /**响应发送采矿奖励信息**/
    public const int EXPLORE_AWARDS_INFO = 30006;


    //当铺
    /** 卖出物品 **/
    public const int PAWN_SHOP_GOODS_SELL = 30021;
    /** 卖出物品成功 **/
    public const int PAWN_SHOP_GOODS_SELL_OK = 30022;
    /** 请求当铺物品列表 **/
    public const int PAWN_SHOP_GOODS_LIST_REQ = 30023;
    /** 返回当铺物品列表 **/
    public const int PAWN_SHOP_GOODS_LIST = 30024;
    /** 购买物品 **/
    public const int PAWN_SHOP_GOODS_BUY = 30025;
    /** 购买物品成功 **/
    public const int PAWN_SHOP_GOODS_BUY_RESP = 30026;
    /** 手动刷新当铺物品 **/
    public const int PAWN_SHOP_GOODS_REFRESH = 30027;
    /** 手动刷新当铺物品 **/
    public const int PAWN_SHOP_GOODS_REFRESH_RESP = 30028;

    //***************** 联盟协议  ******************
    /** 从npc处点击查看联盟 **/
    public const int ALLIANCE_INFO_REQ = 30100;
    /** 返回联盟信息， 给没有联盟的玩家返回此条信息 **/
    public const int ALLIANCE_NON_RESP = 30101;
    /** 返回联盟信息， 给有联盟的玩家返回此条信息 **/
    public const int ALLIANCE_HAVE_RESP = 30102;
    /** 验证联盟名字 **/
    public const int CHECK_ALLIANCE_NAME = 30103;
    /** 返回验证联盟结果 **/
    public const int CHECK_ALLIANCE_NAME_RESP = 30104;
    /** 创建联盟 **/
    public const int CREATE_ALLIANCE = 30105;
    /** 返回创建联盟结果 **/
    public const int CREATE_ALLIANCE_RESP = 30106;
    /** 查找联盟 **/
    public const int FIND_ALLIANCE = 30107;
    /** 返回查找联盟结果 **/
    public const int FIND_ALLIANCE_RESP = 30108;
    /** 申请联盟 **/
    public const int APPLY_ALLIANCE = 30109;
    /** 返回申请联盟结果 **/
    public const int APPLY_ALLIANCE_RESP = 30110;
    /** 取消加入联盟 **/
    public const int CANCEL_JOIN_ALLIANCE = 30111;
    /** 返回取消加入联盟结果 **/
    public const int CANCEL_JOIN_ALLIANCE_RESP = 30112;
    /** 退出联盟 **/
    public const int EXIT_ALLIANCE = 30113;
    /** 退出联盟成功 **/
    public const int EXIT_ALLIANCE_RESP = 30114;
    /** 查看联盟成员 **/
    public const int LOOK_MEMBERS = 30115;
    /** 返回联盟成员信息 **/
    public const int LOOK_MEMBERS_RESP = 30116;
    /** 开除成员**/
    public const int FIRE_MEMBER = 30117;
    /** 开除成员返回**/
    public const int FIRE_MEMBER_RESP = 30118;
    /** 升职成员**/
    public const int UP_TITLE = 30119;
    /** 升职成员返回**/
    public const int UP_TITLE_RESP = 30120;
    /** 降职成员**/
    public const int DOWN_TITLE = 30121;
    /** 降职成员返回**/
    public const int DOWN_TITLE_RESP = 30122;
    /** 查看申请联盟玩家**/
    public const int LOOK_APPLICANTS = 30123;
    /** 查看申请联盟玩家结果返回**/
    public const int LOOK_APPLICANTS_RESP = 30124;
    /** 拒绝申请**/
    public const int REFUSE_APPLY = 30125;
    /** 拒绝申请返回**/
    public const int REFUSE_APPLY_RESP = 30126;
    /** 同意申请**/
    public const int AGREE_APPLY = 30127;
    /** 同意申请返回**/
    public const int AGREE_APPLY_RESP = 30128;
    /** 修改公告**/
    public const int UPDATE_NOTICE = 30129;
    /** 修改公告返回**/
    public const int UPDATE_NOTICE_RESP = 30130;
    /** 解散联盟**/
    public const int DISMISS_ALLIANCE = 30131;
    /** 解散联盟返回**/
    public const int DISMISS_ALLIANCE_OK = 30132;
    /** 打开招募**/
    public const int OPEN_APPLY = 30133;
    /** 打开招募返回**/
    public const int OPEN_APPLY_RESP = 30134;
    /** 关闭招募**/
    public const int CLOSE_APPLY = 30135;
    /** 关闭招募返回成功**/
    public const int CLOSE_APPLY_OK = 30136;
    /** 转让联盟**/
    public const int TRANSFER_ALLIANCE = 30137;
    /** 转让联盟返回**/
    public const int TRANSFER_ALLIANCE_RESP = 30138;
    /** 盟主选举报名**/
    public const int MENGZHU_APPLY = 30139;
    /** 盟主选举报名结果返回**/
    public const int MENGZHU_APPLY_RESP = 30140;
    /** 盟主选举报名**/
    public const int MENGZHU_VOTE = 30141;
    /** 盟主选举报名结果返回**/
    public const int MENGZHU_VOTE_RESP = 30142;
    /** 放弃投票 **/
    public const int GIVEUP_VOTE = 30143;
    /** 放弃投票结果返回 **/
    public const int GIVEUP_VOTE_RESP = 30144;
    /** 立刻加入联盟 **/
    public const int IMMEDIATELY_JOIN = 30145;
    /** 立刻加入联盟返回 **/
    public const int IMMEDIATELY_JOIN_RESP = 30146;
    /** 被联盟开除通知 **/
    public const int ALLIANCE_FIRE_NOTIFY = 30148;
    /** 联盟虎符捐献 **/
    public const int ALLIANCE_HUFU_DONATE = 30149;
    public const int ALLIANCE_HUFU_DONATE_RESP = 30150;


    public const int C_SETTINGS_GET = 30201;//客户端获取设置
    public const int C_SETTINGS_SAVE = 30203;//客户端请求保存设置
    public const int S_SETTINGS = 30204;//服务器发给客户端设置
    public const int C_change_name = 30301;
    public const int S_change_name = 30302;


    public const int C_MoBai_Info = 4010;
    public const int S_MoBai_Info = 4011;
    public const int C_MoBai = 4012;
    /** 加入聊天黑名单 **/
    public const int C_JOIN_BLACKLIST = 30151;
    /** 加入聊天黑名单返回 **/
    public const int S_JOIN_BLACKLIST_RESP = 30152;
    /** 查看黑名单 **/
    public const int C_GET_BALCKLIST = 30153;
    /** 返回黑名单列表 **/
    public const int S_GET_BALCKLIST = 30154;
    /** 取消屏蔽 **/
    public const int C_CANCEL_BALCK = 30155;
    /** 取消屏蔽结果 **/
    public const int S_CANCEL_BALCK = 30156;


    //***************** 荒野求生协议  ******************
    /** 打开荒野 **/
    public const int C_OPEN_HUANGYE = 30401;
    public const int S_OPEN_HUANGYE = 30402;
    /** 驱散迷雾 **/
    public const int C_OPEN_FOG = 30403;
    public const int S_OPEN_FOG = 30404;
    /** 开启藏宝点 **/
    public const int C_OPEN_TREASURE = 30405;
    public const int S_OPEN_TREASURE = 30406;
    /** 请求奖励库 **/
    public const int C_REQ_REWARD_STORE = 30407;
    public const int S_REQ_REWARD_STORE = 30408;

    /** 申请奖励 **/
    public const int C_APPLY_REWARD = 30409;
    public const int S_APPLY_REWARD = 30410;
    /** 取消申请奖励 **/
    public const int C_CANCEL_APPLY_REWARD = 30411;
    public const int S_CANCEL_APPLY_REWARD = 30412;
    /** 盟主分配奖励 **/
    public const int C_GIVE_REWARD = 30413;
    public const int S_GIVE_REWARD = 30414;
    /** 荒野pve-藏宝点挑战 **/
    public const int C_HUANGYE_PVE = 30415;
    public const int S_HUANGYE_PVE_RESP = 30416;
    /** 荒野pve-查看藏宝点信息 **/
    public const int C_HYTREASURE_BATTLE = 30417;
    public const int S_HYTREASURE_BATTLE_RESP = 30418;
    /** 荒野pve-藏宝点战斗结束 **/
    public const int C_HUANGYE_PVE_OVER = 30419;
    public const int S_HUANGYE_PVE_OVER_RESP = 30420;
    /** 荒野pvp-资源点挑战 **/
    public const int C_HUANGYE_PVP = 30421;
    public const int S_HUANGYE_PVP_RESP = 30422;
    /** 荒野pvp-资源点战斗结束 **/
    public const int C_HUANGYE_PVP_OVER = 30423;
    public const int S_HUANGYE_PVP_OVER_RESP = 30424;
    /** 荒野pvp-查看资源点信息 **/
    public const int C_HYRESOURCE_BATTLE = 30425;
    public const int S_HYRESOURCE_BATTLE_RESP = 30426;
    /** 更换资源点 **/
    public const int C_HYRESOURCE_CHANGE = 30427;
    public const int S_HYRESOURCE_CHANGE_RESP = 30428;



    /**排行榜**/
    public const int RANKING_REP = 30430;
    public const int RANKING_RESP = 30431;

    /*
     * 充值
     */
    /**请求充值**/
    public const int C_RECHARGE_REQ = 30432;
    public const int S_RECHARGE_RESP = 30433;
    /**请求充值页面(vip信息)**/
    public const int C_VIPINFO_REQ = 30434;
    public const int S_VIPINFO_RESP = 30435;
    // 获取包含了pve秘宝的战力
    public const int C_PVE_MIBAO_ZHANLI = 30436;
    public const int S_PVE_MIBAO_ZHANLI = 30437;

    //好友协议
    /**获取好友列表**/
    public const int C_FRIEND_REQ = 31001;
    public const int S_FRIEND_RESP = 31002;
    /**请求添加好友**/
    public const int C_FRIEND_ADD_REQ = 31003;
    public const int S_FRIEND_ADD_RESP = 31004;
    /**请求删除好友**/
    public const int C_FRIEND_REMOVE_REQ = 31005;
    public const int S_FRIEND_REMOVE_RESP = 31006;

    // 活动协议
    /**请求签到**/
    public const int C_QIANDAO_REQ = 32001;
    public const int S_QIANDAO_RESP = 32002;
    /**请求签到情况**/
    public const int C_GET_QIANDAO_REQ = 32003;
    public const int S_GET_QIANDAO_RESP = 32004;
    /**获取所有的活动列表**/
    public const int C_GET_ACTIVITYLIST_REQ = 32101;
    public const int S_GET_ACTIVITYLIST_RESP = 32102;
    /**获取首冲详情**/
    public const int C_GET_SHOUCHONG_REQ = 32201;
    public const int S_GET_SHOUCHONG_RESP = 32202;
    /**领取首冲奖励**/
    public const int C_SHOUCHONG_AWARD_REQ = 32203;
    public const int S_SHOUCHONG_AWARD_RESP = 32204;
    /**转国**/
    public const int C_ZHUANGGUO_REQ = 33001;
    public const int S_ZHUANGGUO_RESP = 33002;




    public static readonly Dictionary<int, string> protoIndexClassNameDic = new Dictionary<int, string>()
    {
        {C_Pai_big_house, "ExchangeHouse"},
{C_ExCanJuanJiangLi, "ExCanJuanJiangLi"},
{S_huan_wu_exchange, "ExItemResult"},
{C_huan_wu_exchange, "ExchangeItem"},
{S_house_exp, "HouseExpInfo"},
{C_GetHouseVInfo, "HouseVisitorInfo"},
{S_HouseVInfo, "HouseVisitorInfo"},
{C_ShotOffVisitor, "OffVisitorInfo"},
{S_ShotOffVisitor, "OffVisitorInfo"},
{S_huan_wu_list, "LianMengBoxes"},
{C_huan_wu_Oper, "setHuanWu"},
{S_huan_wu_info, "HuanWuInfo"},
{C_EnterOrExitHouse, "EnterOrExitHouse"},
{C_Set_House_state, "SetHouseState"},
{C_AnswerExchange, "AnswerExchange"},
{S_HOUSE_APPLY_LIST, "ApplyInfos"},
{S_HOUSE_EXCHANGE_RESULT, "ExchangeResult"},
{C_HOUSE_EXCHANGE_RQUEST, "ExchangeHouse"},
{C_EHOUSE_EXCHANGE_RQUEST, "ExchangeEHouse"},
{S_LM_HOUSE_INFO, "BatchSimpleInfo"},
{S_change_name, "ChangeNameBack"},
{C_change_name, "ChangeName"},
{C_SETTINGS_SAVE, "ConfSave"},
{S_SETTINGS, "ConfGet"},
{C_MoBai, "MoBaiReq"},
{S_MoBai_Info, "MoBaiInfo"},
{S_YuJueHeChengResult, "YuJueHeChengResult"},
{C_TaskProgress, "TaskProgress"},
{S_GetTaskRwardResult, "GetTaskRwardResult"},
{C_GetTaskReward, "GetTaskReward"},
{S_TaskSync, "TaskSync"},
{S_TaskList, "TaskList"},
{C_BuZhen_Report, "BuZhenReport"},
{C_MIBAO_SELECT, "MibaoSelect"},
{S_MIBAO_SELECT_RESP, "MibaoSelectResp"},
{S_YuanJun_List, "YuanZhuListReturn"},
{PVE_GuanQia_Request, "GuanQiaInfoRequest"},
{PVE_GuanQia_Info, "GuanQiaInfo"},
{S_PVE_Reset_CQ, "ResetCQTimesBack"},
{C_PVE_Reset_CQ, "ResetCQTimesReq"},
{S_BuZhen_Hero_Info, "BuZhenHeroList"},
{S_BUY_TIMES_INFO, "BuyTimesInfo"},
{Battle_Pvp_Init_Req, "BattlePvpInitReq"},
{C_PVE_SAO_DANG, "PveSaoDangReq"},
{S_PVE_SAO_DANG, "PveSaoDangRet"},
{BAIZHAN_INFO_RESP, "BaiZhanInfoResp"},
{EXCHAGE_AWARD_REQ, "ExchageAwardReq"},
{EXCHAGE_AWARD_RESP, "ExchageAwardResp"},
{ADD_CHANCE_RESP, "AddChanceResp"},
{RECEIVE_AWARD_RESP, "ReceiveAwardResp"},
{CHALLENGE_REQ, "ChallengeReq"},
{CHALLENGE_RESP, "ChallengeResp"},
{CONFIRM_EXECUTE_REQ, "ConfirmExecuteReq"},
{CONFIRM_EXECUTE_RESP, "ConfirmExecuteResp"},
{BAIZHAN_RESULT, "BaiZhanResult"},
{PLAYER_STATE_REQ, "PlayerStateReq"},
{PLAYER_STATE_RESP, "PlayerStateResp"},
{ENTER_BATTLEFIELD_REQ, "EnterBattlefieldReq"},
{ZHAN_DOU_RECORD_RESP, "ZhandouRecordResp"},
{BAIZHAN_RESULT_RESP, "BaiZhanResultResp"},
{C_get_daily_award, "GetDailyAward"},
{S_JingMai_info, "JingMaiRet"},
{C_JingMai_up, "XueWeiUpReq"},
{C_JingMai_info, "JingMaiReq"},
{C_EQUIP_XiLian, "XiLianReq"},
{S_EQUIP_XiLian, "XiLianRes"},
{C_EQUIP_JINJIE, "EquipJinJie"},
{S_EQUIP_JINJIE, "EquipJinJieResp"},
{S_EquipDetail, "EquipDetail"},
{C_EquipDetailReq, "EquipDetailReq"},
{C_EquipInfoOtherReq, "EquipInfoOtherReq"},
{S_EquipInfo, "EquipInfo"},
{S_BagInfo, "BagInfo"},
{C_EquipAdd, "EquipAddReq"},
{C_EquipRemove, "EquipRemoveReq"},
{C_Report_battle_replay, "BattleReplayData"},
{C_Request_battle_replay, "BattleReplayReq"},
{C_KeJiUp, "KeJiShengJiReq"},
{C_KeJiInfo, "KeJiInfoReq"},
{S_KeJiInfo, "KeJiInfoRet"},
{JunZhuInfoReq, "JunZhuInfoReq"},
{JunZhuInfoRet, "JunZhuInfoRet"},
{JunZhuAttPoReq, "JunZhuAttPoReq"},
{JunZhuAttPoRet, "JunZhuAttPoRet"},
{JunZhuAddPoReq, "JunZhuAddPoReq"},
{C_get_sound, "CGetYuYing"},
{PLAYER_SOUND_REPORT, "PlayerSound"},
{PLAYER_STATE_REPORT, "PlayerState"},
{S_Message, "SMessage"},
{C_Send_Chat, "ChatPct"},
{S_Send_Chat, "ChatPct"},
{C_Get_Chat_Log, "CGetChat"},
{S_Send_Chat_Log, "SChatLogList"},
{C_Call_User_Info, "NAccount"},
{C_Call_User_Move, "NUserMove"},
{C_Call_User_Action, "NUserAction"},
{C_Call_User_Attack, "NUserAttack"},
{S_Send_User_Info, "NationalWarInfo"},
{NationalWarInfoListResId, "NationalWarInfoList"},
{NationalScheduleResId, "NationalSchedule"},
{NationalScheduleListResId, "NationalScheduleList"},
{C_Call_City_Info, "NCRCity"},
{C_Call_City_List_Info, "NCRCitys"},
{S_Send_City_Info, "NationalCity"},
{S_Send_City_List_Info, "NationalCityList"},
{S_Send_City_UserList, "NCityUserList"},
{S_Send_City_State_Maps, "NCityStateMapList"},
{S_Send_Country, "NCountryInfo"},
{S_Send_Country_List, "NCountryInfoList"},
{S_Send_Combat_Result, "NAfterCombat"},
{S_Send_Action_Result, "NActionResult"},
{S_Send_City_User_Change, "NCityUserChange"},
{S_Send_Report, "NReport"},
{S_Send_Personal_Award, "NPersonalAwardList"},
{S_Send_Combat_Record, "PkRecordList"},
{C_Call_Check_Report, "NCheckReport"},
{C_Call_Fetch_Award, "NRequestAward"},
{Enter_Scene, "EnterScene"},
{Enter_HouseScene, "EnterScene"},
{Exit_HouseScene, "ExitScene"},
{Enter_Scene_Confirm, "EnterSceneConfirm"},
{Spirite_Move, "SpriteMove"},
{Exit_Scene, "ExitScene"},
{Battle_Pve_Init_Req, "BattlePveInitReq"},
{B_Hero, "Hero"},
{B_Soldier, "Soldier"},
{B_Troop, "Troop"},
{Battle_Init, "BattleInit"},
{ACC_REG, "RegReq"},
{ACC_REG_RET, "RegRet"},
{ACC_LOGIN, "LoginReq"},
{ACC_LOGIN_RET, "LoginRet"},
{CREATE_ROLE_REQUEST, "CreateRoleRequest"},
{CREATE_ROLE_RESPONSE, "CreateRoleResponse"},
{ROLE_NAME_REQUEST, "RoleNameRequest"},
{ROLE_NAME_RESPONSE, "RoleNameResponse"},
{PVE_PAGE_REQ, "PvePageReq"},
{PVE_PAGE_RET, "Section"},
{BattlePveResult_Req, "BattlePveResultReq"},
{Award_Item, "AwardItem"},
{BattlePve_Result, "BattleResult"},
{PVE_BATTLE_OVER_REPORT, "PveBattleOver"},
{PVE_STAR_REWARD_INFO_REQ, "GetPveStarAward"},
{PVE_STAR_REWARD_INFO_RET, "PveStarAwards"},
{PVE_STAR_REWARD_GET, "GetPveStarAward"},
{PVE_STAR_REWARD_GET_RET, "PveStarGetSuccess"},
{S_ERROR, "ErrorMessage"},
{C_EQUIP_UPGRADE, "EquipStrengthReq"},
{S_EQUIP_UPGRADE, "EquipStrengthResp"},
{C_EQUIP_LIST, "UserEquipsReq"},
{S_EQUIP_LIST, "UserEquipResp"},
{S_REQ_MAIL_LIST, "EmailListResponse"},
{S_DELETE_MAIL, "DeleteEmailResp"},
{C_MAIL_GET_REWARD, "GetRewardRequest"},
{S_MAIL_GET_REWARD, "GetRewardResponse"},
{S_MAIL_NEW, "NewMailResponse"},
{C_READED_EAMIL, "ReadEmail"},
{S_READED_EAMIL, "ReadEmailResp"},
{C_EMAIL_RESPONSE, "EmailResponse"},
{S_EMAIL_RESPONSE, "EmailResponseResult"},
{C_SEND_EAMIL, "SendEmail"},
{S_SEND_EAMIL, "SendEmailResp"},
{HERO_INFO_REQ, "HeroInfoReq"},
{HERO_DATA, "HeroDate"},
{HERO_INFO, "HeroInfoResp"},
{GET_UNION_INFO_REQ, "UnionListInitReq"},
{GET_UNION_INFO_RESP, "UnionListInit"},
{GET_UNION_FRIEND_INFO_REQ, "FriendListInitReq"},
{GET_UNION_FRIEND_RESP, "FriendListInit"},
{UNION_EDIT_REQ, "UnionListEditReq"},
{UNION_EDIT_RESP, "UnionListEdit"},
{UNION_INNER_EDIT_REQ, "UnionInnerEditReq"},
{UNION_INNER_EDIT_RESP, "UnionInnerEdit"},
{UNION_OUTER_EDIT_REQ, "UnionOuterEditReq"},
{UNION_OUTER_EDIT_RESP, "UnionOuterEdit"},
{CREATE_UNION_REQ, "UnionListCreateReq"},
{CREATE_UNION_RESP, "UnionListCreate"},
{UNION_LEVELUP_REQ, "UnionLevelupReq"},
{UNION_LEVELUP_RESP, "UnionLevelup"},
{UNION_APPLY_REQ, "UnionListApplyReq"},
{UNION_APPLY_JION_RESP, "UnionListApply"},
{UNION_INVITE_REQ, "UnionListInviteReq"},
{UNION_INVITE_RESP, "UnionListInvite"},
{UNION_INVITED_AGREE_REQ, "UnionAgreeInviteReq"},
{UNION_INVITED_AGREE_RESP, "UnionAgreeInvite"},
{UNION_INVITED_REFUSE_REQ, "UnionRefuseInviteReq"},
{UNION_INVITED_REFUSE_RESP, "UnionRefuseInvite"},
{UNION_QUIT_REQ, "UnionQuitReq"},
{UNION_QUIT_RESP, "UnionQuit"},
{UNION_DISMISS_REQ, "UnionDismissReq"},
{UNION_DISMISS_RESP, "UnionDismiss"},
{UNION_TRANSFER_REQ, "UnionTransferReq"},
{UNION_TRANSFER_RESP, "UnionTransfer"},
{UNION_ADVANCE_REQ, "UnionAdvanceReq"},
{UNION_ADVANCE_RESP, "UnionAdvance"},
{UNION_DEMOTION_REQ, "UnionDemotionReq"},
{UNION_DEMOTION_RESP, "UnionDemotion"},
{UNION_REMOVE_REQ, "UnionRemoveReq"},
{UNION_REMOVE_RESP, "UnionRemove"},
{UNION_APPLY_JION_REQ, "UnionApllyJoinReq"},
//{UNION_APPLY_JION_RESP, "UnionApllyJoin"},
{UNION_DETAIL_INFO_REQ, "UnionDetailInitReq"},
{UNION_DETAIL_INFO, "UnionDetailtInit"},
{WUJIANG_TECHINFO_REQ, "WuJiangTechReq"},
{WUJIANG_TECHINFO_RESP, "WuJiangTech"},
{WUJIANG_TECHLEVELUP_REQ, "WuJiangTechLevelupReq"},
{WUJIANG_TECHLEVELUP_RESP, "WuJiangTechLevelup"},
{WUJIANG_LEVELUP_REQ, "HeroGrowReq"},
{WUJIANG_LEVELUP_RESP, "HeroGrowResp"},
{BUY_CARDBAG_REQ, "BuyCardBagReq"},
{BUY_CARDBAG_RESP, "BuyCardBagResp"},
{OPEN_CARDBAG_REQ, "OpenCardBagReq"},
{OPEN_CARDBAG_RESP, "OpenCardBagResp"},
{C_TIMEWORKER_INTERVAL, "TimeWorkerRequest"},
{S_TIMEWORKER_INTERVAL, "TimeWorkerResponse"},
{HERO_ACTIVE_REQ, "HeroActivatReq"},
{HERO_ACTIVE_RESP, "HeroActivatResp"},
{JINGPO_REFRESH_RESP, "JingPoRefreshResp"},
{WUJIANG_TECH_SPEEDUP_RESP, "WuJiangTechSpeedupResp"},
{ZHANDOU_INIT_PVE_REQ, "PveZhanDouInitReq"},
{ZHANDOU_INIT_PVP_REQ, "PvpZhanDouInitReq"},
{ZHANDOU_INIT_RESP, "ZhanDouInitResp"},
{S_ACHE_LIST_RESP, "AcheListResponse"},
{C_ACHE_GET_REWARD_REQ, "AcheGetRewardRequest"},
{S_ACHE_GET_REWARD_RESP, "AcheGetRewardResponse"},
{S_ACHE_FINISH_INFORM, "AcheFinishInform"},
{S_DAILY_TASK_LIST_RESP, "DailyTaskListResponse"},
{S_DAILY_TASK_FINISH_INFORM, "DailyTaskFinishInform"},
{C_DAILY_TASK_GET_REWARD_REQ, "DailyTaskRewardRequest"},
{S_DAILY_TASK_GET_REWARD_RESP, "DailyTaskRewardResponse"},
{BUY_TREASURE_INFOS_RESP, "BuyTreasureInfosResp"},
{BUY_TREASURE, "BuyTreasure"},
{BUY_TREASURE_RESP, "BuyTreasureResp"},
{BUY_RESOURCE_INFOS_RESP, "BuyResourceInfosResp"},
{PURCHASE_FAIL, "PurchaseFail"},
{S_BUY_TongBi, "BuyTongbiResp"},
{S_BUY_MIBAO_POINT_RESP, "BuyMibaoPoResp"},
{C_MIBAO_ACTIVATE_REQ, "MibaoActivate"},
{S_MIBAO_ACTIVATE_RESP, "MibaoActivateResp"},
{S_MIBAO_INFO_RESP, "MibaoInfoResp"},
{C_MIBAO_STARUP_REQ, "MibaoStarUpReq"},
{S_MIBAO_STARUP_RESP, "MibaoStarUpResp"},
{C_MIBAO_LEVELUP_REQ, "MibaoLevelupReq"},
{S_MIBAO_LEVELUP_RESP, "MibaoLevelupResp"},
{C_MIBAO_INFO_OTHER_REQ, "MibaoInfoOtherReq"},
{IF_EXPLORE_REQ, "IfExploreReq"},
{IF_EXPLORE_RESP, "IfExploreResp"},
{EXPLORE_INFO_REQ, "ExploreInfoReq"},
{EXPLORE_INFO_RESP, "ExploreInfoResp"},
{EXPLORE_REQ, "ExploreReq"},
{EXPLORE_RESP, "ExploreResp"},
{EXPLORE_AWARDS_INFO, "ExploreAwardsInfo"},
{PAWN_SHOP_GOODS_BUY, "PawnshopGoodsBuy"},
{PAWN_SHOP_GOODS_LIST, "PawnshopGoodsList"},
{PAWN_SHOP_GOODS_SELL, "PawnShopGoodsSell"},
{PAWN_SHOP_GOODS_REFRESH_RESP, "PawnshopRefeshResp"},
{PAWN_SHOP_GOODS_BUY_RESP, "PawnshopGoodsBuyResp"},
{ALLIANCE_NON_RESP, "AllianceNonResp"},
{ALLIANCE_HAVE_RESP, "AllianceHaveResp"},
{CHECK_ALLIANCE_NAME, "CheckAllianceName"},
{CHECK_ALLIANCE_NAME_RESP, "CheckAllianceNameResp"},
{CREATE_ALLIANCE, "CreateAlliance"},
{CREATE_ALLIANCE_RESP, "CreateAllianceResp"},
{FIND_ALLIANCE, "FindAlliance"},
{FIND_ALLIANCE_RESP, "FindAllianceResp"},
{APPLY_ALLIANCE, "ApplyAlliance"},
{APPLY_ALLIANCE_RESP, "ApplyAllianceResp"},
{CANCEL_JOIN_ALLIANCE, "CancelJoinAlliance"},
{CANCEL_JOIN_ALLIANCE_RESP, "CancelJoinAllianceResp"},
{EXIT_ALLIANCE, "ExitAlliance"},
{EXIT_ALLIANCE_RESP, "ExitAllianceResp"},
{LOOK_MEMBERS, "LookMembers"},
{LOOK_MEMBERS_RESP, "LookMembersResp"},
{FIRE_MEMBER, "FireMember"},
{FIRE_MEMBER_RESP, "FireMemberResp"},
{UP_TITLE, "UpTitle"},
{UP_TITLE_RESP, "UpTitleResp"},
{DOWN_TITLE, "DownTitle"},
{DOWN_TITLE_RESP, "DownTitleResp"},
{LOOK_APPLICANTS, "LookApplicants"},
{LOOK_APPLICANTS_RESP, "LookApplicantsResp"},
{REFUSE_APPLY, "RefuseApply"},
{REFUSE_APPLY_RESP, "RefuseApplyResp"},
{AGREE_APPLY, "AgreeApply"},
{AGREE_APPLY_RESP, "AgreeApplyResp"},
{UPDATE_NOTICE, "UpdateNotice"},
{UPDATE_NOTICE_RESP, "UpdateNoticeResp"},
{DISMISS_ALLIANCE, "DismissAlliance"},
{OPEN_APPLY, "OpenApply"},
{OPEN_APPLY_RESP, "OpenApplyResp"},
{CLOSE_APPLY, "CloseApply"},
{TRANSFER_ALLIANCE, "TransferAlliance"},
{TRANSFER_ALLIANCE_RESP, "TransferAllianceResp"},
{MENGZHU_APPLY_RESP, "MengZhuApplyResp"},
{MENGZHU_VOTE, "MengZhuVote"},
{MENGZHU_VOTE_RESP, "MengZhuVoteResp"},
{GIVEUP_VOTE_RESP, "GiveUpVoteResp"},
{IMMEDIATELY_JOIN, "immediatelyJoin"},
{IMMEDIATELY_JOIN_RESP, "immediatelyJoinResp"},
{C_JOIN_BLACKLIST, "JoinToBlacklist"},
{C_CANCEL_BALCK, "CancelBlack"},
{ALLIANCE_HUFU_DONATE, "DonateHuFu"},
{ALLIANCE_HUFU_DONATE_RESP, "DonateHuFuResp"},
{C_OPEN_HUANGYE, "OpenHuangYe"},
{S_OPEN_HUANGYE, "OpenHuangYeResp"},
{C_OPEN_FOG, "OpenFog"},
{S_OPEN_FOG, "OpenFogResp"},
{C_OPEN_TREASURE, "OpenHuangYeTreasure"},
{S_OPEN_TREASURE, "OpenHuangYeTreasureResp"},
{C_REQ_REWARD_STORE, "ReqRewardStore"},
{S_REQ_REWARD_STORE, "ReqRewardStoreResp"},
{C_APPLY_REWARD, "ApplyReward"},
{S_APPLY_REWARD, "ApplyRewardResp"},
{C_CANCEL_APPLY_REWARD, "CancelApplyReward"},
{S_CANCEL_APPLY_REWARD, "CancelApplyRewardResp"},
{C_GIVE_REWARD, "GiveReward"},
{S_GIVE_REWARD, "GiveRewardResp"},
{C_HUANGYE_PVE, "HuangYePveReq"},
{C_HYTREASURE_BATTLE, "HYTreasureBattle"},
{S_HYTREASURE_BATTLE_RESP, "HYTreasureBattleResp"},
{C_HUANGYE_PVE_OVER, "HuangYePveOver"},
{C_HYRESOURCE_BATTLE, "BattleResouceReq"},
{S_HYRESOURCE_BATTLE_RESP, "BattleResouceResp"},
{C_HUANGYE_PVP, "HuangYePvpReq"},
{C_HUANGYE_PVP_OVER, "HuangYePvpOver"},
{C_HYRESOURCE_CHANGE, "ResourceChange"},
{S_HYRESOURCE_CHANGE_RESP, "ResourceChangeResp"},
{RANKING_RESP, "RankingResp"},
{C_RECHARGE_REQ, "RechargeReq"},
{S_RECHARGE_RESP, "RechargeResp"},
{S_VIPINFO_RESP, "VipInfoResp"},
{S_PVE_MIBAO_ZHANLI, "PveMiBaoZhanLi"},
{C_FRIEND_ADD_REQ, "AddFriendReq"},
{C_FRIEND_REMOVE_REQ, "RemoveFriendReq"},
{C_FRIEND_REQ, "GetFriendListReq"},

    };

    public static string GetClassNameByIndex(int index)
    {
        if (protoIndexClassNameDic.ContainsKey(index))
        {
            return protoIndexClassNameDic[index];
        }
        else
        {
            return null;
        }
    }
}