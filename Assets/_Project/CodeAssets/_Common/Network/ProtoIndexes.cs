using UnityEngine;
using System.Collections;

public class ProtoIndexes
{

    /* ---------------------------------------------------------------------- */
    /* ----------------------- General Message Process ---------------------- */

    /// general error for socket message
    public const short S_BROAD_CAST 		= 10003;

    public const short GENERAL_ERROR 		= 10010;
    /// multi login error
    public const short MULTI_LOGIN_ERROR 	= 23110;



    /// game pause
    public const short GAME_PAUSE 		= 23411;	// sended when app enter background
    public const short GAME_CONTINUE 	= 23413;	// sended when app enter forground



	// check network condition
    public const short NETWORK_CHECK 		= 100;
    public const short NETWORK_CHECK_RET 	= 101;
	public const short C_DROP_CONN 			= 120;


	/// send and receive, ping
	public const short DELAY_REQ 		= 110;
	public const short DELAY_RET 		= 111;
	


    /// general message
    public const short GENERAL_MESSAGE 		= 20000;

	public const short C_XG_TOKEN 			= 10101;

	/* ----------------------- General Message Process ---------------------- */
	/* ---------------------------------------------------------------------- */



    /// 创建账号
    public const int CREATE_ACCOUNT = 23101;

    /// 创建账号返回
    public const int CREATE_ACCOUNT_RET = 23102;

    /// 登陆账号
    public const int LOGIN_ACCOUNT = 23103;

    /// 登陆账号返回
    public const int LOGIN_ACCOUNT_RET = 23104;

    /// 账号协议
    public const short CREATE_ROLE_REQUEST = 23105;
    public const short CREATE_ROLE_RESPONSE1 = 23106;
    public const short ROLE_NAME_REQUEST = 23107;
    public const short ROLE_NAME_RESPONSE = 23108;

    public const int Enter_Scene = 22000;               //玩家发起进去主城
    public const int Enter_Scene_Confirm = 22001;       //确认进入主城
    public const int Sprite_Move = 22002;               //玩家移动
    public const int ExitScene = 22003;                 //玩家退出主城
    public const short Enter_HouseScene = 22004;        //玩家进入房屋
    public const short Exit_HouseScene = 22005;         //玩家退出房屋
    public const short EXIT_FIGHT_SCENE = 22006;        //exit fight scene.
    public const short ENTER_FIGHT_SCENE = 22007;       //进入战斗场景
    public const short ENTER_FIGHT_SCENE_OK = 22008;       //进入战斗场景ok
    public const short EXIT_CARRIAGE_SCENE = 22010;        
    public const short ENTER_CARRIAGE_SCENE = 22009;      


    public const short S_HEAD_STRING = 22101;//更新称号ok

	/// <summary>
	/// The Alliance Data
	/// </summary>

	public const short C_JIAN_ZHU_INFO = 27401;//请求联盟建筑信息，只发协议号

	public const short S_JIAN_ZHU_INFO = 27402;//返回联盟建筑信息

	public const short C_JIAN_ZHU_UP = 27403;//请求升级建筑

	//消息体用ErrorMessage，errorCode值表示要升级什么，1客栈；2书院；3图腾；4商铺；5宗庙
	
	public const short S_JIAN_ZHU_UP = 27404;

	//升级结果，消息体用ErrorMessage，errorCode值为0表示成功，其他表示失败

	////////////////////////////////////////////////////////////////////////////////
	public const short C_LMKJ_UP = 27503;//请求升级联盟科技
	//消息体用ErrorMessage，errorCode值表示要升级什么，用表里面的type值。
	
	public const short S_LMKJ_UP = 27504;
	//升级结果，消息体用ErrorMessage，errorCode值为0表示成功，其他表示失败
	
	//
	public const short C_LMKJ_INFO = 27507;
	public const short S_LMKJ_INFO = 27508;
	//获取联盟科技信息，消息体是JianZhuList，按照配置表，每个type发一个等级

	public const short C_LM_CHOU_JIANG_1 = 27510;// 宗庙抽奖一次
	public const short C_LM_CHOU_JIANG_N = 27511;//宗庙连续抽奖
	public const short S_LM_CHOU_JIANG = 27512;// 宗庙抽奖返回。

	public const short C_LM_CHOU_JIANG_INFO = 27513; //请求宗庙信息
	public const short S_LM_CHOU_JIANG_INFO = 27514;//宗庙信息返回

	/// <summary>
	/// 联盟目标
	/// </summary>

	public const short C_ALLIANCE_TARGET_INFO_Resp  = 30173; // 联盟目标信息
	public const short S_ALLIANCE_TARGET_INFO_Resp  = 30174; // 联盟目标信息返回 
	public const short C_GET_ALLIANCEL_LEVEL_AWARD = 30175;// 领取联盟等级目标奖励

	public const short S_GET_ALLIANCEL_LEVEL_AWARD_RESP = 30176;//领取联盟等级目标奖励结果返回

    /// <summary>
    /// 请求联盟战信息
    /// </summary>
	public const short ALLIANCE_FIGHT_INFO_REQ = 4201;

    /// <summary>
    /// 联盟站信息返回
    /// </summary>
	public const short ALLIANCE_FIGHT_INFO_RESP = 4202;

    /// <summary>
    /// 联盟战报名
    /// </summary>
	public const short ALLIANCE_FIGHT_APPLY = 4203;

    /// <summary>
    /// 报名结果返回
    /// </summary>
    public const short ALLIANCE_FIGHT_APPLY_RESP = 4204;

    /// <summary>
    /// 请求联盟战战场信息
    /// </summary>
	public const short ALLIANCE_BATTLE_FIELD_REQ = 4205;
	
    /// <summary>
    /// 返回联盟战战场信息
    /// </summary>
    public const short ALLIANCE_BATTLE_FIELD_RESP = 4206;

    /// <summary>
    /// 联盟战有人死亡通知
    /// </summary>
	public const short ALLIANCE_FIGHT_PLAYER_DEAD = 4207;

	/// <summary>
    /// 联盟战有人复活通知
	/// </summary>
    public const short ALLIANCE_FIGHT_PLAYER_REVIVE = 4208;

	/** 联盟战历史战况请求 */
	public const short ALLIANCE_FIGHT_HISTORY_REQ = 4209;
	/** 联盟战历史战况结果返回 */
	public const short ALLIANCE_FIGHT_HISTORY_RESP = 4210;
	/** 联盟战上届排名请求 */
	public const short ALLIANCE_FIGTH_LASTTIME_RANK = 4211;
	/** 联盟战上届排名返回 */
	public const short ALLIANCE_FIGTH_LASTTIME_RANK_RESP = 4212;

    /// <summary>
    /// Refresh alliance battle data.
    /// </summary>
    public const short ALLIANCE_BATTLE_FIELD_NOTIFY = 4214;

    public const short ALLIANCE_BATTLE_RESULT = 4216;

    public const short BUFFER_INFO = 4217;

    public const short PLAYER_REVIVE_REQUEST = 4218;

    // red notice info
    public const short RED_NOTICE_INFO = 4220;

    public const short SAFE_AREA_BOOLD_RETURN_RESP = 4222;

    public const int PVE_PAGE_REQ = 23201;              //请求章节信息
    public const int PVE_PAGE_RET = 23202;              //服务器返回章节信息

    public const int PVE_GuanQia_Request = 23210;              //请求关卡掉落，遇敌
    public const int PVE_GuanQia_Info = 23211;              //返回关卡掉落，遇敌
    public const short C_PVE_Reset_CQ = 23212;              //重置关卡
    public const short S_PVE_Reset_CQ = 23213;            //重置关起返回

    public const short C_YuanJun_List_Req = 23230;//请求援助单位信息
    public const short S_YuanJun_List = 23231;//返回

    public const short C_BuZhen_Report = 23240;//客户端向服务器发送布阵信息
    public const short C_MIBAO_SELECT = 23241;//客户端向服务器发送秘宝选择信息
	public const  short S_MIBAO_SELECT_RESP = 23242;//服务器发送秘宝选择信息

    public const int PVE_BATTLE_OVER_REPORT = 23203;    //战斗结束汇报是否胜利。
    public const int PVE_STAR_REWARD_INFO_REQ = 23204;  //查看星级奖励
    public const int PVE_STAR_REWARD_INFO_RET = 23205;  //查看星级奖励 返回
    public const int PVE_STAR_REWARD_GET = 23206;       //领取星级奖励
    public const int PVE_STAR_REWARD_GET_RET = 23207;   //查看星级奖励 返回

    public const int PLAYER_STATE_REPORT = 23401;       //客户端汇报状态。
    public const int PLAYER_SOUND_REPORT = 23501;       //玩家声音

    public const short C_EquipAdd = 23701;              //穿装备
    public const short C_EquipRemove = 23702;           //脱装备
    public const short S_EquipInfo = 23703;             //身上装备信息
    public const short S_BagInfo = 23704;               //背包信息
    public const short C_BagInfo = 23705;               //只有协议号
    public const short C_EquipInfo = 23706;             //只有协议号


	public const short PVE_MAX_ID_REQ = 23007;//请求打到的最大章节关卡

	public const short PVE_MAX_ID_RESP = 23008; //请求关卡返回

	/** 请求未领取过通关奖励章节列表 */
	public const short C_NOT_GET_AWART_ZHANGJIE_REQ = 24156;
	/** 返回未领取过通关奖励章节列表 */
	public const short S_NOT_GET_AWART_ZHANGJIE_RESP = 24157;


	/** 请求通章奖励*/
	public const short has_get_zhangJie_award_req  = 24152;
	public const short has_get_passZhangJie_award_req  = 24153;

	/** 领取通章奖励*/
	public const short get_zhangJie_award_req  = 24154;
	public const short get_passZhangJie_award_req  = 24155;


    public const short C_YuJueHeChengRequest = 29509;
    public const short S_YuJueHeChengResult = 29510;

    /// 装备进阶请求
    public const short C_EQUIP_JINJIE = 24015;

    /// 装备进阶返回
    public const short S_EQUIP_JINJIE = 24016;


    public const short C_EquipDetailReq = 23710;        //请求装备详情
    public const short S_EquipDetail = 23711;           //返回装备详情

    public const short C_GET_OTHER_WEAPON = 23712;      //请求别人武器装备信息
    public const short S_GET_OTHER_WEAPON = 23713;      //请求别人武器装备信息

    public const short C_EQUIP_LIST = 24001;            //装备列表请求
    public const short S_EQUIP_LIST = 24002;            //装备列表返回

    public const short JunZhuInfoReq = 23601;           //请求君主信息
    public const short JunZhuInfoRet = 23602;
    public const short JunZhuAttPointReq = 23603;       //请求君主点数分配信息
    public const short JunZhuAttPointRet = 23604;
    public const short JunZhuAddPointReq = 23605;       //请求加点，成功则服务器推送JunZhuAttPointRet

    public const short C_KeJiInfo = 23650;              //请求君主科技信息。
    public const short S_KeJiInfo = 23651;
    public const short C_KeJiUp = 23652;                //君主科技升级。

    public const short C_EQUIP_UPGRADE = 24003;			// 强化的装备
    public const short S_EQUIP_UPGRADE = 24004;
    public const short C_EQUIP_XiLian = 24012;			// 需要强化的装备 0-查询属性和元宝  1-免费洗练  2-元宝洗练
    public const short S_EQUIP_XiLian = 24013;
    public const short S_EQUIP_XILIAN_ERROR = 24018;

    public const short C_JingMai_info = 24100;          //经脉信息
    public const short C_JingMai_up = 24101;
    public const short S_JingMai_info = 24103;          //经脉升级

    public const short C_Send_Chat = 20001; 			//客户端发送聊天
    public const short S_Send_Chat = 20002; 			//服务器通知客户端聊天消息
	public const short C_GET_CHAT_CONF = 20104;			//请求聊天剩余免费次数
	public const short S_GET_CHAT_CONF = 20105;			//剩余免费次数返回

	public const short C_CLOSE_TAN_BAO_UI = 29531;		//抽中秘宝发送广播

    public const short C_Join_BlackList = 30151; 		//客户端加入聊天黑名单
    public const short S_Join_BlackList_Resp = 30152;   //服务器返回加入聊天黑名单
    public const short C_get_sound = 23505;				//获取语音信息
    public const short S_get_sound = 23507;				//服务器返回语音信息

    public const short C_BUY_TIMES_REQ = 28321;			//请求购买体力和铜币的次数.
    public const short S_BUY_TIMES_INFO = 28322;
    public const short C_BUY_TiLi = 28323;				//购买体力

	//金币购买
	public const short C_BUY_TongBi_Data = 2831;//界面数据请求
	public const short S_BUY_TongBi_Data = 2830;//界面数据返回
    public const short C_BUY_TongBi = 28324;			//购买铜币
	public const short C_BUY_TongBi_LiXu = 28329;
	public const short S_BUY_TongBi_LiXu = 28330;

	public const short C_BUY_MiBaoPoint = 28327;			//购买铜币
	public const short S_BUY_MiBaoPoint = 28328;			//购买铜币fanhui


    //**********百战协议号*********
    public const short BAIZHAN_INFO_REQ = 27001;/**请求百战**/
    public const short BAIZHAN_INFO_RESP = 27002;/**百战返回**/

    public const short EXCHAGE_AWARD_REQ = 27003;/**请求兑换奖励**/
    public const short EXCHAGE_AWARD_RESP = 27004;/**兑换奖励返回**/

    public const short EXCHAGE_MIBAO_REQ = 27005;/**请求更换秘宝**/
    public const short EXCHAGE_MIBAO_RESP = 27006;/**更换秘宝返回**/

    public const short ADD_CHANCE_REQ = 27007;/**请求增加次数**/
    public const short ADD_CHANCE_RESP = 27008;/**次数增加返回**/

    public const short RECEIVE_AWARD_REQ = 27009;/**请求领取奖励（领取按钮）**/
    public const short RECEIVE_AWARD_RESP = 27010;/**领取奖励返回**/

    public const short CHALLENGE_REQ = 27011;/**请求挑战**/
    public const short CHALLENGE_RESP = 27012;/**挑战返回**/

    public const short SAVE_MIBAO_REQ = 27013;/**请求保存秘宝配置**/
    public const short SAVE_MIBAO_RESP = 27014;/**保存秘宝配置返回**/

    public const short CONFIRM_EXECUTE_REQ = 27015;/**请求百战千军中确定做某种事情**/
    public const short CONFIRM_EXECUTE_RESP = 27016;/**返回结果**/

    public const short PLAYER_STATE_REQ = 27018;//** 被挑战者当前状态请求 **/
    public const short PLAYER_STATE_RESP = 27019;//** 被挑战者当前状态返回 **/
    public const short RECORD_CHALLENGER = 27020;//进入挑战——战斗
    public const short ZhanDou_Notes_Req = 27022;//战斗记录请求
    public const short ZhanDou_Notes_Resq = 27023;//战斗记录响应
    public const short BAIZHAN_RESULT_RESP = 27024;//27017d 响应 面页发送


    public const short SHARE_VIEW_REQ = 27026;//战斗录像分享请求
    public const short SHARE_VIEW_RESP = 27027;//战斗录像分享请求返回


    //抽卡预留28201~28299
    public const short BUY_CARDBAG_REQ = 28201; 		//购买卡包
    public const short OPEN_CARDBAG_REQ = 28202;		//打开卡包
    public const short BUY_CARDBAG_RESP = 28251;		//
    public const short OPEN_CARDBAG_RESP = 28252;		//

    /***************商城指令协议号***********************/
    /** 请求宝箱购买信息 **/
    public const short BUY_TREASURE_INFOS_REQ = 28253;
    /** 返回宝箱购买信息 **/
    public const short BUY_TREASURE_INFOS_RESP = 28254;
    /** 购买宝箱 **/
    public const short BUY_TREASURE = 28255;
    /** 返回购买宝箱获得物品信息 **/
    public const short BUY_TREASURE_RESP = 28256;
    /** 请求资源购买信息 **/
    public const short BUY_RESOURCE_INFOS_REQ = 28257;
    /** 返回资源购买信息 **/
    public const short BUY_RESOURCE_INFOS_RESP = 28258;
    /** 返回商城购买失败信息 **/
    public const short PURCHASE_FAIL = 28260;


    public const short C_get_daily_award_info = 28100;   //客户端获取每日奖励信息
    public const short S_daily_award_info = 28110;      //服务器发送每日奖励信息
    public const short C_get_daily_award = 28120;      //客户端领取奖励

    public const short C_ADD_TILI_INTERVAL = 28301; 	//玩家定时请求加体力
    public const short S_ADD_TILI_INTERVAL = 28302; 	//发送玩家董事请求加体力结果

    public const short C_PVE_SAO_DANG = 23220;//扫荡请求
    public const short S_PVE_SAO_DANG = 23222;//扫荡返回

    /** 请求成就列表 **/
    public const short C_ACHE_LIST_REQ = 28331;

    /** 返回成就列表 **/
    public const short S_ACHE_LIST_RESP = 28332;

    /** 成就完成通知 **/
    public const short S_ACHE_FINISH_INFORM = 28334;

    /** 领取成就奖励 **/
    public const short C_ACHE_GET_REWARD_REQ = 28335;

    /** 领取成就奖励返回结果 **/
    public const short S_ACHE_GET_REWARD_RESP = 28336;



    /** 请求任务列表 **/
    public const short C_TaskReq = 29501;//只发协议号，请求自己的任务列表

    public const short S_TaskList = 29502;

    public const short S_TaskSync = 29503; //服务器向客户端发送任务进度

    public const short C_GetTaskReward = 29504; //客户端领取奖励

    public const short S_GetTaskRwardResult = 29505;//客户端领取奖励结果

    public const short C_TaskProgress = 29506;//客户端向服务器发送任务进度，用户对话任务，进度发1表示对话完毕（任务完成）


    // 邮件
    //请求邮件列表
    public const short C_REQ_MAIL_LIST = 25003;
    //返回邮件列表
    public const short S_REQ_MAIL_LIST = 25004;
    //通知删掉一封邮件
    public const short C_DELETE_MAIL = 25005;
    //删除邮件返回
    public const short S_DELETE_MAIL = 25006;
    //领取附件
    public const short C_MAIL_GET_REWARD = 25007;
    //返回领取附件结果
    public const short S_MAIL_GET_REWARD = 25008;
    //通知添加一封邮件
    public const short S_MAIL_NEW = 25010;
    //发送邮件请求
    public const short C_SEND_EAMIL = 25011;
    //发送邮件返回
    public const short S_SEND_EAMIL = 25012;
    //读取邮件请求
    public const short C_READED_EAMIL = 25013;
    //读取邮件返回
    public const short S_READED_EAMIL = 25014;
    //换房请求
    public const short C_EMAIL_RESPONSE = 25015;
    //换房请求返回
    public const short S_EMAIL_RESPONSE = 25016;
	// 有新的申请成员通知
	public const short ALLIANCE_HAVE_NEW_APPLYER = 30160;

    /* ---------------------------------------------------------- */
    /* --------------- 战斗proto< 23000-26003 > --------------- */
    public const int Battle_Pve_Init_Req = 23000;
    public const int B_Hero = 23001;
    public const int B_Soldier = 23002;
    public const int B_Troop = 23003;
    public const int Battle_Init = 23004;
    public const int Battle_Pvp_Init_Req = 23005;
    public const int Battle_Pvp_Init = 23006;

    public const int BattlePveResult_Req = 23300;
    public const int BAIZHAN_RESULT = 27017;
    public const int Award_Item = 23301;
    public const int BattlePve_Result = 23302;

    public const int C_Report_battle_replay = 23310;
    public const int C_Request_battle_replay = 23311;
/// <summary>
/// 游侠战斗请求
/// </summary>

	public const int C_YOUXIA_TYPE_INFO_REQ   = 615; //游侠战斗请求
	public const int S_YOUXIA_TYPE_INFO_RESP  = 616; //游侠战斗请求

	public const int C_YOUXIA_INIT_REQ = 601; //游侠战斗请求
	public const int C_YOUXIA_BATTLE_OVER_REQ = 603; //游侠战斗结果
	// 游侠扫荡协议
	public const int C_YOUXIA_SAO_DANG_REQ = 611;
	
	public const int S_YOUXIA_SAO_DANG_RESP = 612;

	public const int C_YOUXIA_GUANQIA_REQ = 613;
	
	public const int S_YOUXIA_GUANQIA_RESP = 614;

	public const int LVE_BATTLE_END_RESP = 26071;

	public const int C_YUAN_HU_END_REQ = 26077;

    /* --------------- 战斗proto< 23000-26003 > --------------- */
    /* ---------------------------------------------------------- */


    /* ---------------------------------------------------------- */
    /* --------------- ARPG战斗proto --------------- */
    public const int ZhanDou_Init_Pve_Req = 24201;
    public const int ZhanDou_Init_Pvp_Req = 24202;
	public const int C_ZHANDOU_INIT_YB_REQ = 3419;

    public const int ZhanDou_Init_Resp = 24151;
	public const int S_ZHANDOU_INIT_ERROR = 3421;

    //有新的押镖者进入场景
    public const int S_YABIAO_ENTER_RESP = 3422;

	public const short C_InitProc = 301;//开始密语
	public const short S_InitProc = 302;//服务器返回密语信息
	public const short C_zlgdlc = 303;//客户端终止加密过程，必须在发送战斗结果前发给服务器
    /* --------------- ARPG战斗proto --------------- */
    /* ---------------------------------------------------------- */


    /* ---------------------------------------------------------- */
    /* --------------- 联盟proto< 28001-28099 > --------------- */
    public const short GET_UNION_INFO_REQ = 28001;
    public const short UNION_APPLY_JION_REQ = 28002;
    public const short GET_UNION_FRIEND_INFO_REQ = 28003;
    public const short UNION_EDIT_REQ = 28004;
    public const short UNION_INNER_EDIT_REQ = 28005;
    public const short UNION_OUTER_EDIT_REQ = 28006;
    public const short CREATE_UNION_REQ = 28007;
    public const short UNION_LEVELUP_REQ = 28008;
    public const short UNION_APPLY_REQ = 28009;
    public const short UNION_INVITE_REQ = 28010;
    public const short UNION_INVITED_AGREE_REQ = 28011;
    public const short UNION_INVITED_REFUSE_REQ = 28012;
    public const short UNION_QUIT_REQ = 28013;
    public const short UNION_DISMISS_REQ = 28014;
    public const short UNION_TRANSFER_REQ = 28015;
    public const short UNION_ADVANCE_REQ = 28016;
    public const short UNION_DEMOTION_REQ = 28017;
    public const short UNION_REMOVE_REQ = 28018;
    public const short UNION_DETAIL_INFO_REQ = 28019;

    public const short GET_UNION_INFO_RESP = 28051;
    public const short UNION_APPLY_JION_RESP = 28052;
    public const short GET_UNION_FRIEND_RESP = 28053;
    public const short UNION_EDIT_RESP = 28054;
    public const short UNION_INNER_EDIT_RESP = 28055;
    public const short UNION_OUTER_EDIT_RESP = 28056;
    public const short CREATE_UNION_RESP = 28057;
    public const short UNION_LEVELUP_RESP = 28058;
    public const short UNION_APPLY_RESP = 28059;
    public const short UNION_INVITE_RESP = 28060;
    public const short UNION_INVITED_AGREE_RESP = 28061;
    public const short UNION_INVITED_REFUSE_RESP = 28062;
    public const short UNION_QUIT_RESP = 28063;
    public const short UNION_DISMISS_RESP = 28064;
    public const short UNION_TRANSFER_RESP = 28065;
    public const short UNION_ADVANCE_RESP = 28066;
    public const short UNION_DEMOTION_RESP = 28067;
    public const short UNION_REMOVE_RESP = 28068;
    public const short UNION_DETAIL_INFO = 28069;
    /* --------------- 联盟proto< 28001-28099 > --------------- */
    /* ---------------------------------------------------------- */


    /* ---------------------------------------------------------- */
    /* --------------- 武将proto< 29001-29099 > --------------- */
    public const short HERO_INFO_REQ = 26001;
    public const short HERO_DATA = 26002;
    public const short HERO_INFO = 26003;
    public const short WuJiangTech_Info_REQ = 26004;
    public const short WuJiangTech_Levelup_REQ = 26005;
    public const short HERO_GROW_REQ = 26006;
    public const short HERO_ACTIVE_REQ = 26007;
    public const short JINGPO_REFRESH_REQ = 26008;
    public const short WuJiangTech_Speedup_REQ = 26009;
    public const short WwJiangShined_REQ = 26010;

    public const short WuJiangTech_Info = 26054;
    public const short WuJiangTech_Levelup = 26055;
    public const short HERO_GROW_RESP = 26056;
    public const short HERO_ACTIVE_RESP = 26057;
    public const short JINGPO_REFRESH_RESP = 26058;
    public const short WuJiangTech_Speedup_RESP = 26059;
    /* --------------- 武将proto< 29001-29099 > --------------- */
    /* ---------------------------------------------------------- */

    //************ 秘宝协议 ************

    /** 秘宝激活请求 **/
    public const short C_MIBAO_ACTIVATE_REQ = 29601;
    /** 秘宝激活结果返回 **/
    public const short S_MIBAO_ACTIVATE_RESP = 29602;
    /** 秘宝信息请求 **/
    public const short C_MIBAO_INFO_REQ = 29603;
    /** 秘宝信息返回 **/
    public const short S_MIBAO_INFO_RESP = 29604;
    /** 秘宝升级请求 **/
    public const short C_MIBAO_LEVELUP_REQ = 29605;
    /** 秘宝升级结果返回 **/
    public const short S_MIBAO_LEVELUP_RESP = 29606;
    /** 秘宝升星请求 **/
    public const short C_MIBAO_STARUP_REQ = 29607;
    /** 秘宝升星结果返回 **/
    public const short s_Mibao_StarUp_Resp = 29608;

	
	////////秘宝收集足够星星进行宝箱领奖/////////////////////
	public const short GET_FULL_STAR_AWARD_REQ = 29613;
	public const short GET_FULL_STAR_AWARD_RESP = 29614;

    /// <summary>
    /// 请求别人密保协议号 
    /// </summary>
    public const short C_MIBAO_INFO_OTHER_REQ = 29609;

    /// <summary>
    /// 返回别人密保协议号, 消息体是MibaoInfoResp
    /// </summary>
    public const short S_MIBAO_INFO_OTHER_RESP = 29610;

    //***************  探宝协议  *******************
    /**请求是否显示感叹号**/
    public const short IF_EXPLORE_REQ = 30000;
    /**响应是否感叹号**/
    public const short IF_EXPLORE_RESP = 30001;
    /********探宝页面需要协议**********/
    /**请求矿区主界面**/
    public const short EXPLORE_INFO_REQ = 30002;
    /**响应矿区主界面**/
    public const short EXPLORE_INFO_RESP = 30003;
    /**请求采矿**/
    public const short EXPLORE_REQ = 30004;
    /**响应不可以采矿**/
    public const short EXPLORE_RESP = 30005;
    /**响应发送采矿奖励信息**/
    public const short EXPLORE_AWARDS_INFO = 30006;

    /* ---------------------------------------------------------- */
    /* -----------------------  当铺proto  ----------------------- */
    /** 卖出物品 **/
    public const short PAWN_SHOP_GOODS_SELL = 30021;
    /** 卖出物品成功 **/
    public const short PAWN_SHOP_GOODS_SELL_OK = 30022;
    /** 请求当铺物品列表 **/
    public const short PAWN_SHOP_GOODS_LIST_REQ = 30023;
    /** 返回当铺物品列表 **/
    public const short PAWN_SHOP_GOODS_LIST = 30024;
    /** 购买物品 **/
    public const short PAWN_SHOP_GOODS_BUY = 30025;
    /** 购买物品成功 **/
    public const short PAWN_SHOP_GOODS_BUY_OK = 30026;
    /** 刷新货物 **/
    public const short PAWN_SHOP_GOODS_REFRESH = 30027;
	/** 刷新失败 **/
	public const short PAWN_SHOP_GOODS_REFRESH_RESP = 30028;
    /* -----------------------  当铺proto  ----------------------- */
    /* ---------------------------------------------------------- */
    //***************** 联盟协议  ******************
    /** 从npc处点击查看联盟 **/
    public const short ALLIANCE_INFO_REQ = 30100;

    /** 返回联盟信息， 给没有联盟的玩家返回此条信息 **/
    public const short ALLIANCE_NON_RESP = 30101;

    /** 返回联盟信息， 给有联盟的玩家返回此条信息 **/
    public const short ALLIANCE_HAVE_RESP = 30102;

    /** 验证联盟名字 **/
    public const short CHECK_ALLIANCE_NAME = 30103;

    /** 返回验证联盟结果 **/
    public const short CHECK_ALLIANCE_NAME_RESP = 30104;

    /** 创建联盟 **/
    public const short CREATE_ALLIANCE = 30105;

    /** 返回创建联盟结果 **/
    public const short CREATE_ALLIANCE_RESP = 30106;

    /** 查找联盟 **/
    public const short FIND_ALLIANCE = 30107;

    /** 返回查找联盟结果 **/
    public const short FIND_ALLIANCE_RESP = 30108;

    /** 申请联盟 **/
    public const short APPLY_ALLIANCE = 30109;
    /** 取消加入联盟 **/
    public const short CANCEL_JOIN_ALLIANCE = 30111;
    /** 返回取消加入联盟结果 **/
    public const short CANCEL_JOIN_ALLIANCE_RESP = 30112;

    /** 返回申请联盟结果 **/
    public const short APPLY_ALLIANCE_RESP = 30110;

    /** 退出联盟 **/
    public const short EXIT_ALLIANCE = 30113;
    /** 退出联盟成功 **/
    public const short EXIT_ALLIANCE_RESP = 30114;
    /** 查看联盟成员 **/
    public const short LOOK_MEMBERS = 30115;
    /** 返回联盟成员信息 **/
    public const short LOOK_MEMBERS_RESP = 30116;
    /** 开除成员**/
    public const short FIRE_MEMBER = 30117;
    /** 开除成员返回**/
    public const short FIRE_MEMBER_RESP = 30118;
    /** 升职成员**/
    public const short UP_TITLE = 30119;
    /** 升职成员返回**/
    public const short UP_TITLE_RESP = 30120;
    /** 降职成员**/
    public const short DOWN_TITLE = 30121;
    /** 降职成员返回**/
    public const short DOWN_TITLE_RESP = 30122;
    /** 查看申请联盟玩家**/
    public const short LOOK_APPLICANTS = 30123;
    /** 查看申请联盟玩家结果返回**/
    public const short LOOK_APPLICANTS_RESP = 30124;
    /** 拒绝申请**/
    public const short REFUSE_APPLY = 30125;
    /** 拒绝申请返回**/
    public const short REFUSE_APPLY_RESP = 30126;
    /** 同意申请**/
    public const short AGREE_APPLY = 30127;
    /** 同意申请返回**/
    public const short AGREE_APPLY_RESP = 30128;
    /** 修改公告**/
    public const short UPDATE_NOTICE = 30129;
    /** 修改公告返回**/
    public const short UPDATE_NOTICE_RESP = 30130;
    /** 解散联盟**/
    public const short DISMISS_ALLIANCE = 30131;
    /**被开除返回**/
    public const short DISMISS_ALLIANCE_OK = 30132;
    /** 打开招募**/
    public const short OPEN_APPLY = 30133;
    /** 打开招募返回**/
    public const short OPEN_APPLY_RESP = 30134;
    /** 关闭招募**/
    public const short CLOSE_APPLY = 30135;
    /** 关闭招募返回成功**/
    public const short CLOSE_APPLY_OK = 30136;
    /** 转让联盟**/
    public const short TRANSFER_ALLIANCE = 30137;
    /** 转让联盟返回**/
    public const short TRANSFER_ALLIANCE_RESP = 30138;
    /** 盟主选举报名**/
    public const short MENGZHU_APPLY = 30139;
    /** 盟主选举报名结果返回**/
    public const short MENGZHU_APPLY_RESP = 30140;
    /** 盟主选举投票**/
    public const short MENGZHU_VOTE = 30141;
    /** 盟主选举投票结果返回**/
    public const short MENGZHU_VOTE_RESP = 30142;
    /** 放弃投票 **/
    public const short GIVEUP_VOTE = 30143;
    /** 放弃投票结果返回 **/
    public const short GIVEUP_VOTE_RESP = 30144;

    public const short C_MoBai_Info = 4010;//请求膜拜信息 
    public const short S_MoBai_Info = 4011; //返回膜拜信息
    public const short C_MoBai = 4012;//请求膜拜

    public const short IMMEDIATELY_JOIN = 30145;
    /** 立刻加入联盟返回 **/
    public const short IMMEDIATELY_JOIN_RESP = 30146;
    /** 联盟虎符捐献 **/
    public const short ALLIANCE_HUFU_DONATE = 30149;
    public const short ALLIANCE_HUFU_DONATE_RESP = 30150;

	/** 联盟事件请求 */
	public const short ALLINACE_EVENT_REQ = 30161;
	/** 联盟事件返回 */
	public const short ALLINACE_EVENT_RESP = 30162;

	public const short JUNZHU_INFO_SPECIFY_REQ = 23067; //请求君主信息
	public const short JUNZHU_INFO_SPECIFY_RESP = 23068;//请求君主信息返回

	/** 联盟升级通知 */
	public const short ALLIANCE_LEVEL_UP_NOTIFY = 30164;
	/** 联盟解散 */
	public const short ALLIANCE_DISMISS_NOTIFY = 30166;

    /// <summary>
    /// Alliance state change notify
    /// </summary>
    public const short ALLIANCE_STATE_NOTIFY = 30167;

    // 荒野求生
    /** 打开荒野 **/
    public const short C_OPEN_HUANGYE = 30401;
    public const short S_OPEN_HUANGYE = 30402;
    /** 驱散迷雾 **/
    public const short C_OPEN_FOG = 30403;
    public const short S_OPEN_FOG = 30404;
    /** 开启藏宝点 **/
    public const short C_OPEN_TREASURE = 30405;
    public const short S_OPEN_TREASURE = 30406;
    /** 请求奖励库 **/
    public const short C_REQ_REWARD_STORE = 30407;
    public const short S_REQ_REWARD_STORE = 30408;
    /** 申请奖励 **/
    public const short C_APPLY_REWARD = 30409;
    public const short S_APPLY_REWARD = 30410;
    /** 取消申请奖励 **/
    public const short C_CANCEL_APPLY_REWARD = 30411;
    public const short S_CANCEL_APPLY_REWARD = 30412;
    /** 盟主分配奖励 **/
    public const short C_GIVE_REWARD = 30413;
    public const short S_GIVE_REWARD = 30414;
    /** 荒野pve-藏宝点挑战 **/
    public const short C_HUANGYE_PVE = 30415;
    public const short S_HUANGYE_PVE_RESP = 30416;
    /** 荒野pve-查看藏宝点信息 **/
    public const short C_HYTREASURE_BATTLE = 30417;
    public const short S_HYTREASURE_BATTLE_RESP = 30418;
    /** 荒野pve-藏宝点战斗结束 **/
    public const short C_HUANGYE_PVE_OVER = 30419;
    public const short S_HUANGYE_PVE_OVER_RESP = 30420;
    /** 荒野pvp-资源点挑战 **/
    public const short C_HUANGYE_PVP = 30421;
    public const short S_HUANGYE_PVP_RESP = 30422;
    /** 荒野pvp-资源点战斗结束 **/
    public const short C_HUANGYE_PVP_OVER = 30423;
    public const short S_HUANGYE_PVP_OVER_RESP = 30424;
    /** 荒野pvp-查看资源点信息 **/
    public const short C_HYRESOURCE_BATTLE = 30425;
    public const short S_HYRESOURCE_BATTLE_RESP = 30426;
    /** 更换资源点 **/
    public const short C_HYRESOURCE_CHANGE = 30427;
    public const short S_HYRESOURCE_CHANGE_RESP = 30428;

	// 20150909 

	public const short HY_SHOP_REQ = 30390;
	public const short HY_SHOP_RESP = 30391;
	public const short HY_BUY_GOOD_REQ = 30392;
	public const short HY_BUY_GOOD_RESP = 30393;
	public const short ACTIVE_TREASURE_REQ = 30394;
	public const short ACTIVE_TREASURE_RESP = 30395;
	public const short MAX_DAMAGE_RANK_REQ = 30396;
	public const short MAX_DAMAGE_RANK_RESP = 30397;


	////////////购买挑战次数/////////////////////
	public const short HY_BUY_BATTLE_TIMES_REQ = 30398;
	public const short HY_BUY_BATTLE_TIMES_RESP = 30399;


	/////////////秘宝技能手动激活和手动进阶 20150914////////////////////////////
	public const short  MIBAO_DEAL_SKILL_REQ = 29611;
	public const short  MIBAO_DEAL_SKILL_RESP = 29612;



    // 每日任务
    /** 请求每日任务列表 **/
    public const short C_DAILY_TASK_LIST_REQ = 28341;
    /** 返回每日任务列表 **/
    public const short S_DAILY_TASK_LIST_RESP = 28342;
    /** 每日任务完成通知 **/
    public const short S_DAILY_TASK_FINISH_INFORM = 28344;
    /** 领取每日任务奖励 **/
    public const short C_DAILY_TASK_GET_REWARD_REQ = 28345;
    /** 领取每日任务奖励返回结果 **/
    public const short S_DAILY_TASK_GET_REWARD_RESP = 28346;

    public const short C_SETTINGS_GET = 30201;//客户端获取设置，只发协议号就行

    public const short S_SETTINGS = 30204;//服务器发给客户端设置，下面的消息体
    public const short C_SETTINGS_SAVE = 30203;//客户端请求保存设置，发给服务器下面的消息体
    public const short C_change_name = 30301;//客户端请求改名字
    public const short S_change_name = 30302;//改名字返回

    /// <summary>
    /// 查看黑名单
    /// </summary>
    public const short C_GET_BLACKLIST = 30153;

    /// <summary>
    /// 返回黑名单列表
    /// </summary>
    public const short S_GET_BLACKLIST = 30154;

    /// <summary>
    /// 取消屏蔽
    /// </summary>
    public const short C_CANCEL_BLACK = 30155;

    /// <summary>
    /// 取消屏蔽结果
    /// </summary>
    public const short S_CANCEL_BLACK = 30156;

    /// <summary>
    /// 被联盟开除通知
    /// </summary>
    public const short ALLIANCE_FIRE_NOTIFY = 30148;

    /// <summary>
    /// 请求本联盟所有房屋信息
    /// </summary>
    public const short C_LM_HOUSE_INFO = 27301;

    /// <summary>
    /// House info update.
    /// </summary>
    public const short S_LM_UPHOUSE_INFO = 27303;

    /// <summary>
    /// 请求本联盟所有房屋信息返回
    /// </summary>
    public const short S_LM_HOUSE_INFO = 27302;

    /// <summary>
    /// 请求设置房屋状态，需要设置哪个属性，就给哪个属性赋值
    /// </summary>
    public const short C_SET_HOUSE_STATE = 27303;

    /// <summary>
    /// 请求获取房屋经验，只要协议id就够了
    /// </summary>
    public const short C_GET_SMALLHOUSE_EXP = 27307;

    /// <summary>
    /// 获取大房子经验
    /// </summary>
    public const short C_GET_BIGHOUSE_EXP = 27353;

    /// <summary>
    /// 自身房屋经验信息
    /// </summary>
    public const short S_GET_HOUSE_EXP = 27308;

    /// <summary>
    /// 请求交换房屋
    /// </summary>
    public const short C_HOUSE_EXCHANGE_RQUEST = 27311;

    /// <summary>
    /// 请求交换房屋返回结果
    /// </summary>
    public const short S_HOUSE_EXCHANGE_RESULT = 27312;

    /// <summary>
    /// 请求交换empty房屋
    /// </summary>
    public const short C_EHOUSE_EXCHANGE_RQUEST = 27357;

    /// <summary>
    /// 请求交换empty房屋返回结果
    /// </summary>
    public const short S_EHOUSE_EXCHANGE_RESULT = 27358;

    /// <summary>
    /// 请求自己房屋的请求交换列表
    /// </summary>
    public const short C_HOUSE_APPLY_LIST = 27313;

    /// <summary>
    /// 房屋请求交换列表
    /// </summary>
    public const short S_HOUSE_APPLY_LIST = 27314;

    /// <summary>
    /// 响应交换请求
    /// </summary>
    public const short C_ANSWER_HOUSE_EXCHANGE = 27321;

    /// <summary>
    /// 通知服务器玩家进入了哪个房间
    /// </summary>
    public const short C_EnterOrExitHouse = 27305;

    /// <summary>
    /// 竞拍大房子，消息体用上面的ExchangeHouse
    /// </summary>
    public const short C_PAI_BIG_HOUSE = 27351;

    /// <summary>
    /// 竞拍结果，消息体使用上面的ExchangeResult
    /// </summary>
    public const short S_PAI_BIG_HOUSE = 27352;

    /// <summary>
    /// 获取自己的换物箱信息，只要协议号就够了
    /// </summary>
    public const short C_HUAN_WU_INFO = 27309;

    /// <summary>
    /// 服务器返回玩家自己的换物箱信息，玩家id和名字是瞎填的客户端不要用
    /// </summary>
    public const short S_HUAN_WU_INFO = 27310;

    /// <summary>
    /// 自己的换物箱操作，操作完成后服务器会下发上面的玩家的换物箱信息
    /// </summary>
    public const short C_HUAN_WU_OPER = 27331;

    /// <summary>
    /// 请求联盟好友的换物箱列表
    /// </summary>
    public const short C_HUAN_WU_LIST = 27333;

    /// <summary>
    /// 联盟好友的换物箱列表
    /// </summary>
    public const short S_HUAN_WU_LIST = 27334;

    /// <summary>
    /// 客户端请求交换换物箱物品
    /// </summary>
    public const short C_HUAN_WU_EXCHANGE = 27337;

    /// <summary>
    /// 交换结果
    /// </summary>
    public const short S_HUAN_WU_EXCHANGE = 27338;

    /// <summary>
    /// 用残卷兑换奖励
    /// </summary>
    public const short C_EX_CAN_JUAN_JIANG_LI = 27341;

    /// <summary>
    /// 残卷兑换返回结果
    /// </summary>
    public const short S_EX_CAN_JUAN_JIANG_LI = 27342;

    /// <summary>
    /// 请求装修（增加房屋经验）
    /// </summary>
    public const short C_UP_HOUSE = 27343;

    public const short C_GET_HOUSE_VISITOR = 27306;

    /// <summary>
    /// 发送访客列表
    /// </summary>
    public const short S_GET_HOUSE_VISITOR = 27354;

    public const short C_SHOT_OFF_HOUSE_VISITOR = 27355;

    /// <summary>
    /// 访客被踢
    /// </summary>
    public const short S_SHOT_OFF_HOUSE_VISITOR = 27356;

    /// <summary>
    /// 请求撤回交换房屋申请
    /// </summary>
    public const short C_CANCEL_EXCHANGE = 27360;

    /// <summary>
    /// 请求撤回交换房屋申请返回结果
    /// </summary>
    public const short S_CANCEL_EXCHANGE = 27361;

    public const short C_GET_MYSELF_HOUSE_EXP = 27362;
    public const short S_RETURN_MYSELF_HOUSE_EXP = 27364;

    /**排行榜**/
    public const short RANKING_REQ = 30430;
    public const short RANKING_RESP = 30431;
    public const short RANKING_ALLIANCE_MEMBER_REQ = 7001;
    public const short RANKING_ALLIANCE_MEMBER_RESP = 7002;
    public const short RANKING_MY_RANK_REQ = 7003;
    public const short RANKING_MY_RANK_RESP = 7004;

    /**请求充值页面(vip信息)**/
    public const short C_VIPINFO_REQ = 30434;
    public const short S_VIPINFO_RESP = 30435;

    /// <summary>
    /// Get zhan li
    /// </summary>
    public const short C_PVE_ZHANLI = 30436;

    /// <summary>
    /// Receive zhan li
    /// </summary>
    public const short S_PVE_ZHANLI = 30437;

    /**请求充值**/
    public const short C_RECHARGE_REQ = 30432;
    public const short S_RECHARGE_RESP = 30433;


    /**获取好友列表*/

    public const short C_FRIEND_REQ = 31001;  // 获取好友列表，请求协议号31001
    public const short S_FRIEND_RESP = 31002;  // 获取好友列表，响应协议号31002

    // 取消关注好友请求，请求协议号31005，响应协议号31006，响应消息体FriendResp
    public const short C_DELEFRIEND_REQ = 31005; //请求协议号31005，
    public const short S_DELEFRIEND_RESP = 31006;
    // 关注好友请求，请求协议号31003，响应协议号31004，响应消息体FriendResp

    /// <summary>
    /// Request all friends.
    /// </summary>
    public const short C_ADDFRIEND_REQ = 31003;  

    /// <summary>
    /// All friends response.
    /// </summary>
    public const short S_ADDRIEND_RESP = 31004;

    public const short C_GET_FRIEND_IDS = 31011;
    public const short S_GET_FRIEND_IDS = 31012;

    // 查询签到情况 请求32003，返回32004
    public const short C_SIGNALINLIST_REQ = 32003;
    public const short S_SIGNALINLIST_RESP = 32004;

    // 签到 请求32001，返回32002
    public const short C_SIGNALIN_REQ = 32001;
    public const short S_SIGNALIN_RESP = 32002;
    // 活动列表返回消息体 请求32101，返回32102
    public const short C_ACTIVITY_REQ = 32101;
    public const short S_ACTIVITY_RESP = 32102;
    // 获取首冲详情，请求32201，响应32202
    public const short C_TOPUP_REQ = 32201;
    public const short S_TOPUP_RESP = 32202;
    // 领取首冲奖励，请求32203，响应32204
    public const short C_TOPUPLQ_REQ = 32203;
    public const short S_TOPUPLQ_RESP = 32204;

    // 转国
    public const short C_ChangeCountry_REQ = 32205;
    public const short S_ChangeCountry_RESP = 32206;

	/// <summary>
	/// 请求运镖主界面
	/// </summary>
	public const short C_YABIAO_INFO_REQ	= 3401;
	/// <summary>
	/// 请求运镖主界面返回
	/// </summary>
	public const short S_YABIAO_INFO_RESP= 3402;
	/// <summary>
	/// 请求选马主界面
	/// </summary>
	public const short C_YABIAO_MENU_REQ = 3403;
	/// <summary>
	/// 请求选马界面返回
	/// </summary>
	public const short S_YABIAO_MENU_RESP = 3404;
	/// <summary>
	/// 请求设置马匹
	/// </summary>
	public const short C_SETHORSE_REQ = 3405;
	/// <summary>
	/// 请求设置马匹返回
	/// </summary>
	public const short S_SETHORSE_RESP = 3406;
	/// <summary>
	/// 请求运镖
	/// </summary>
	public const short C_YABIAO_REQ = 3407;
	/// <summary>
	/// 请求运镖返回
	/// </summary>
	public const short S_YABIAO_RESP = 3408;

	public const short C_BUY_XUEPING_REQ= 3409;
	public const short S_BUY_XUEPING_RESP = 3410;

	/// <summary>
	/// 请求进入押镖场景
	/// </summary>
	public const short C_ENTER_YABIAOSCENE = 3411;
	/// <summary>
	/// 请求进入押镖场景返回
	/// </summary>
	public const short C_ENTER_JBBATTLE_REQ = 3412;
	/// <summary>
	/// 请求镖车信息
	/// </summary>
	public const short C_BIAOCHE_INFO = 3418;
	/// <summary>
	/// 推送镖车信息
	/// </summary>
	public const short S_BIAOCHE_INFO_RESP = 3413;
	/// <summary>
	/// 测试用 停止运镖
	/// </summary>
	public const short C_ENDYABIAO_REQ = 3414;
	public const short S_ENDYABIAO_RESP = 3415;

    public const short S_BIAOCHE_STATE = 3417;

	public const short C_YABIAO_RESULT = 3420;

	/// <summary>
	/// 请求押镖仇人
	/// </summary>
	public const short C_YABIAO_ENEMY_RSQ = 3423;
	/// <summary>
	/// 请求押镖仇人返回
	/// </summary>
	public const short S_YABIAO_ENEMY_RESP = 3424;
	/// <summary>
	/// 请求够买押镖相关次数
	/// </summary>
	public const short C_YABIAO_BUY_RSQ = 3425;
	/// <summary>
	/// 请求够买押镖相关次数返回
	/// </summary>
	public const short S_YABIAO_BUY_RESP = 3426;
	/// <summary>
	/// 请求押镖历史
	/// </summary>
	public const short C_YABIAO_HISTORY_RSQ = 3427;

	public const short C_YABIAO_HELP_RSQ = 3429;//请求押镖协助
	public const short S_YABIAO_HELP_RESP = 3430;//请求押镖协助返回 
	public const short C_ANSWER_YBHELP_RSQ = 3431;//同意押镖协助
	public const short S_ANSWER_YBHELP_RESP = 3432;//同意押镖协助返回
    public const short C_TICHU_YBHELP_RSQ = 3433;//踢出押镖协助
    public const short S_TICHU_YBHELP_RESP = 3434;//踢出押镖协助返回
    public const short S_TICHU_YBHELPXZ_RESP = 3438;//踢出押镖协助者给协助者返回
	public const short S_ASK_YABIAO_HELP_RESP = 3435;//答复请求押镖协助返回 
	public const short C_YABIAO_MOREINFO_RSQ = 3436; 
	public const short S_YABIAO_MOREINFO_RESP = 3437;
	public const short C_YABIAO_XIEZHU_LIST_REQ = 3446;//请求协助君主列表
	public const short S_YABIAO_XIEZHU_LIST_RESP = 3447;//请求协助君主列表返回 

    /// <summary>
    /// Request I help others list.
    /// </summary>
	public const short C_CHECK_YABIAOHELP_RSQ = 3449;
	public const short S_CHECK_YABIAOHELP_RESP = 3450;


    //通知
    public const short C_MengYouKuaiBao_Req = 4240;//请求盟友快报
	public const short S_MengYouKuaiBao_Resq = 4241;//请求盟友快报返回
	public const short S_MengYouKuaiBao_PUSH = 3448;//盟友快报推送
	public const short C_Prompt_Action_Req = 4242; //快报中的行为请求
	public const short S_Prompt_Action_Resp = 4243; //快报中行为请求返回

    /// <summary>
    /// 请求押镖协助次数
    /// </summary>
    public const short C_YABIAO_XIEZHU_RSQ = 3436;

    /// <summary>
    /// 请求押镖协助次数返回
    /// </summary>
    public const short S_YABIAO_XIEZHU_RESP = 3437;

    /// <summary>
    /// 请求押镖历史返回
    /// </summary>
    public const short S_YABIAO_HISTORY_RESP = 3428;
	/// <summary>
	/// 推送押镖战斗记录
	/// </summary>
	public const short S_PUSH_YBRECORD_RESP = 3439;
	//请求购买马车道具
	public const short C_BUYHORSEPROP_REQ = 3440;
	//请求购买马车道具返回
	public const short S_BUYHORSEBUFF_RESP = 3441;

    public const short C_GETMABIANTYPE_REQ = 3442;//请求马鞭类型
    public const short S_GETMABIANTYPE_RESP = 3443;//请求马鞭类型返回

    public const short C_CARTJIASU_REQ = 3444;
    public const short S_CARTJIASU_RESP = 3445;

	//------------------------------天赋协议-----------------------------
	public const short C_TALENT_INFO_REQ = 30537;//发送获得初始化数据
	public const short S_TALENT_INFO_RESP = 30538;//获得初始化数据
	public const short C_TALENT_UP_LEVEL_REQ = 30539;//发送升级ID
	public const short S_TALENT_UP_LEVEL_RESP = 30540;//获得升级错误
	public const short S_TALENT_UP_CAN = 30541;//获得可以升级协议
	public const short S_NOTICE_TALENT_CAN_NOT_UP = 30542;

	//-------------------------------称号协议----------------------------
	public const short C_GET_CUR_CHENG_HAO = 5101;//客户端请求当前君主使用的称号
	public const short S_GET_CUR_CHENG_HAO = 5102;//服务器返回当前君主使用的称号，消息体ChengHaoData
	public const short C_LIST_CHENG_HAO = 5111;//获取称号列表
	public const short S_LIST_CHENG_HAO = 5112;//服务器返回称号列表，消息体ChengHaoList
	public const short C_USE_CHENG_HAO = 5121;//客户端选择称号
	public const short S_NEW_CHENGHAO = 29520;

    //Highlight item for bag sys.
    public const short C_GET_HighLight_item_ids = 29523;//客户端请求
    public const short S_GET_HighLight_item_ids = 29524;//服务器返回

    //-------------------------------变强协议----------------------------
    public const short C_GET_UPACTION_DATA = 5131;//发送我要变强请求
	public const short S_UPACTION_DATA_0 = 5132;//接收到第一页消息
	public const short S_UPACTION_DATA_1 = 5133;//接收到第二页消息
	public const short S_UPACTION_DATA_2 = 5134;//接收到第三页消息
	public const short S_UPACTION_DATA_3 = 5135;//接收到第四页消息

	/**
	 * 游戏玩法信息请求
	 */
	public const short C_YOUXIA_INFO_REQ = 605;
	
	/**
	 * 游戏玩法信息请求返回
	 */
	public const short S_YOUXIA_INFO_RESP = 606;

	/**
	 * 游侠购买次数信息请求
	 */
	public const short C_YOUXIA_TIMES_INFO_REQ = 607;
	/**
	 * 游侠购买次数信息返回
	 */
	public const short S_YOUXIA_TIMES_INFO_RESP = 608;
	/**
	 * 游侠确认购买次数
	 */
	public const short C_YOUXIA_TIMES_BUY_REQ = 609;
	/**
	 * 游侠购买次数购买返回
	 */
	public const short S_YOUXIA_TIMES_BUY_RESP = 610;

    public const short C_XINSHOU_XIANSHI_INFO_REQ = 4001;//请求新手限时活动界面
    public const short S_XINSHOU_XIANSHI_INFO_RESP = 4002;//请求新手限时活动界面返回
    public const short C_XINSHOU_XIANSHI_AWARD_REQ = 4003;//请求领取新手限时活动奖励
    public const short S_XINSHOU_XIANSHI_AWARD_RESP = 4004;//请求领取新手限时活动奖励返回
    public const short C_XIANSHI_INFO_REQ = 4005;//请求限时活动界面
    public const short S_XIANSHI_INFO_RESP = 4006;//请求限时活动界面返回
    public const short C_XIANSHI_AWARD_REQ = 4007;//请求领取限时活动奖励
    public const short S_XIANSHI_AWARD_RESP = 4008;//请求领取限时活动奖励返回
    public const short C_XIANSHI_REQ = 4009;//请求可开启的限时活动(首日/七日)
    public const short S_XIANSHI_RESP = 4010;//请求可开启的限时活动(首日/七日)返回

     public const short C_FIGHT_ATTACK_REQ = 4103;
     public const short S_FIGHT_ATTACK_RESP = 4104;

    /** 加入联盟被批准通知 **/
    public const short ALLIANCE_ALLOW_NOTIFY = 30147;
    // 请求版本公告，请求协议号5001，响应协议号5002
    public const short C_NOTICE_REQ = 5001;
    public const short S_NOTICE_RESP = 5002; 

	//**********掠夺**********//
	/** 掠夺战斗请求**/
	public const int ZHANDOU_INIT_LVE_DUO_REQ = 24203;

	public const int ZHANDOU_INIT_YUAN_ZHU_REQ = 24204;
	
	/** pve、pvp, 掠夺，战斗请求返回数据 **/
	public const int ZHANDOU_INIT_RESP = 24151;
	
	public const short LVE_DUO_INFO_REQ = 26060;//掠夺信息请求
	public const short LVE_DUO_INFO_RESP = 26061;//掠夺信息返回
	public const short LVE_CONFIRM_REQ = 26062;//清除冷却或增加次数请求
	public const short LVE_CONFIRM_RESP = 26063;//清除冷却或增加次数返回
	public const short LVE_BATTLE_RECORD_REQ = 26064;//掠夺记录请求
	public const short LVE_BATTLE_RECORD_RESP = 26065;//掠夺记录返回
	public const short LVE_GO_LVE_DUO_REQ = 26066;//掠夺对手请求
	public const short LVE_GO_LVE_DUO_RESP = 26067;//掠夺对手返回
	public const short LVE_BATTLE_END_REQ  = 26068;//掠夺战斗结束请求

	public const short LVE_NEXT_ITEM_REQ = 26069;//下个掠夺页面请求
	public const short LVE_NEXT_ITEM_RESP = 26070;//下个掠夺页面返回

	public const short LVE_NOTICE_CAN_LVE_DUO = 26072;//掠夺新纪录提示推送

	public const short LVE_HELP_REQ = 26073;//掠夺失败求助
	public const short LVE_HELP_RESP = 26074;//掠夺失败求助

	public const short alliance_junQing_req = 26075;
	public const short alliance_junQing_resq = 26076;

	public const short go_qu_zhu_req = 26078; // 驱逐
	public const short go_qu_zhu_resp = 26079; // 驱逐返回

   //国家信息
    public const short GUO_JIA_MAIN_INFO_REQ = 6003;
    public const short GUO_JIA_MAIN_INFO_RESP = 6004;
    public const short C_GET_JUANXIAN_GONGJIN_REQ = 6001;//请求捐献贡金 
    public const short S_GET_JUANXIAN_GONGJIN_RESP = 6002;//请求捐献贡金返回

    public const short C_GET_JUANXIAN_DAYAWARD_REQ = 6007;//请求捐献贡金 日奖励
    public const short S_GET_JUANXIAN_DAYAWARD_RESP = 6008;//请求捐献贡金日奖励返回

	//符文
	public const short C_FUWEN_MAINPAGE_REQ = 8001;//符文首页请求
	public const short S_FUWEN_MAINPAGE_RES = 8002;//符文首页返回
	public const short C_FUWEN_OPERAT_REQ = 8003;//符文操作请求
	public const short S_FUWEN_OPERAT_RES = 8004;//符文操作返回
	public const short S_FUWEN_TIPS = 4221;//符文红点提醒

	//**********************************君主技能进阶*****************************************
	public const short C_HEROSKILLUP_DATA_REQ = 4250;//技能数据申请
	public const short S_HEROSKILLUP_DATA_RES = 4251;//技能数据返回
	public const short C_HEROSKILLUP_UP_REQ = 4252;//升级申请
	public const short S_HEROSKILLUP_UP_RES = 4253;//升级返回

    public const short C_BUY_FULL_REBIRTH_TIME_REQ = 4254;
    public const short S_BUY_FULL_REBIRTH_TIME_RESP = 4255;

    //一键强化
    public const short C_EQUIP_UPALLGRADE = 24019;//一键强化
    public const short S_EQUIP_UPALLGRADE = 24020;//一键强化返回

    // 请求兑换cdkey，协议号4230
    // 相应兑换cskey，协议号4231
    public const short C_CDKEY_REQ = 4230;//一键强化
    public const short S_CDKEY_RES = 4231;//一键强化返回

    public const short C_GET_MOBAI_AWARD = 4022;
    //领取阶段奖励，消息体用MoBaiReq，cmd的值用1/2/3表示领取第1/2/3段奖励

	public const short C_MAIN_SIMPLE_INFO_REQ = 23069;//战争选择窗口信息请求
	public const short S_MAIN_SIMPLE_INFO_RESP = 23070;//战争选择窗口信息返回

	public const short S_USE_ITEM = 11001; //使用物品后返回的列表

    public const short dailyTask_get_huoYue_award_req = 28347;//活跃度奖励
    public const short dailyTask_get_huoYue_award_resp = 28348;


    public const short C_ALLIANCE_FENGSHAN_REQ = 30168;// 请求封禅信息
    public const short S_ALLIANCE_FENGSHAN_RESP = 30169; // 请求封禅信息 返回  

    public const short C_DO_ALLIANCE_FENGSHAN_REQ = 30170; //请求封禅
    public const short S_DO_ALLIANCE_FENGSHAN_RESP = 30171; //请求封禅 返回 
   ///////////套装///////////////////////
    public const short tao_zhuang_Req = 30500;
    public const short tao_zhuang_Resp = 30501;

    //激活
    public const short activate_tao_zhuang_req = 30502;
    public const short activate_tao_zhuang_resp = 30503;

    /////////////累计签到达到一定次数，领取vip礼包//////////////

    public const short qianDao_get_vip_present_req =  31999;
    public const short qianDao_get_vip_present_resp = 32000;

	public const short FUNCTION_OPEN_NOTICE = 4223;//通知前台开启或者关闭功能 -为关闭

	//**********************************福利*****************************************
	public const short C_FULIINFO_REQ = 4020;//请求福利信息
	public const short S_FULIINFO_RESP = 4021;//请求福利信息返回
	public const short C_FULIINFOAWARD_REQ = 4030;//请求福利奖励
	public const short S_FULIINFOAWARD_RESP = 4031;//请求福利奖励返回
}
