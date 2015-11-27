﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class LanguageTemplate : XmlLoadManager
{
    // <LanTemp LID="1" str="测试文本" />

    public enum Text
    {
        LAN_SAMPLE_STRING = 1,
        TANBAO_PIECEWINDOW_DES1 = 2,
        TANBAO_PIECEWINDOW_DES2 = 3,
        TANBAO_PIECEWINDOW_DES3 = 4,
        TANBAO_FREE = 5,
        TANBAO_TODAY = 6,
        TANBAO_TODAY_FREENUM_USEEND = 7,
        TANBAO_WAITDES = 8,
        TANBAO_OPEL_NEED_LEVEL = 9,
        BAIZHAN_TIAOZHAN_DISCD = 10,
        CONFIRM = 11,
        CANCEL = 12,
        BAIZHAN_TIAOZHAN_TITLE_DISCD = 13,
        BAIZHAN_RANK = 14,
        DAY = 15,
        HOUR = 16,
        MINUTE = 17,
        SECOND = 18,
        BAIZHAN_NONE = 19,
        BAIZHAN_TIAOZHAN_FAIL = 20,
        BAIZHAN_BGING_CHALLENGED = 21,
        BAIZHAN_WAIT_TRY = 22,
        YUANBAO_LACK = 23,
        YUANBAO_LACK_TITLE = 24,
        BAIZHAN_TIAOZHAN_ADDNUM_DES = 25,
        BAIZHAN_TIAOZHAN_ADDNUM_TITLE = 26,
        BAIZHAN_DISCD_SUCCESS = 27,
        BAIZHAN_DISCD_TITLE = 28,
        BAIZHAN_DUIHUAN_SUCCESS = 29,
        BAIZHAN_DUIHUAN_TITLE = 30,
        BAIZHAN_REFRESH_SUCCESS = 31,
        BAIZHAN_REFRESH_SUCCESS_TITLE = 32,
        BAIZHAN_GET_AWARD_SUCCESS = 33,
        BAIZHAN_GET_AWARD_SUCCESS_TITLE = 34,
        VIP_LEVEL_NOT_ENOUGH = 35,
        BUY_FAIL = 36,
        WEIWANG_NOT_ENOUGU = 37,
        WEIWANG_NOT_ENOUGH_TITLE = 38,
        BAIZHAN_ADDNUM_ASKSTR1 = 39,
        BAIZHAN_YUANBAO_BUY = 40,
        BAIZHAN_ADDNUM_ASKSTR2 = 41,
        BAIZHAN_ADDNUM_ASKSTR3 = 42,
        BAIZHAN_ADDNUM_TITLE = 43,
        BAIZHAN_BUY_ADDNUM_VIP_NOT_ENOUGH = 44,
        BAIZHAN_LINGQU_AWARD_STR1 = 45,
        BAIZHAN_LINGQU_AWARD_STR2 = 46,
        BAIZHAN_LINGQU_AWARD_STR3 = 47,
        BAIZHAN_LINGQU_AWARD_STR4 = 48,
        BAIZHAN_LINGQU_AWARD_STR5 = 49,
        BAIZHAN_LINGQU_AWARD_STR6 = 50,
        BAIZHAN_LINGQU_AWARD_STR7 = 51,
        BAIZHAN_LINGQU_AWARD_TITLE = 52,
        BAIZHAN_LINGQU_NO_AWARD = 53,
        BAIZHAN_DISCD_USE_YUANBAO_ASKSTR1 = 54,
        BAIZHAN_DISCD_USE_YUANBAO_ASKSTR2 = 55,
        BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR1 = 56,
        BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR2 = 57,
        BAIZHAN_CONFIRM_DUIHUAN_TITLE = 58,
        BAIZHAN_REFRESH_AWARD_ASKSTR1 = 59,
        BAIZHAN_WEIWANG = 60,
        BAIZHAN_REFRESH_AWARD_ASKSTR2 = 61,
        BAIZHAN_REFRESH_AWARD_TITLE = 62,
        ALLIANCE_CONFIRM_JIESAN_ASKSTR1 = 63,
        ALLIANCE_CONFIRM_JIESAN_ASKSTR2 = 64,
        ALLIACNE_JIESAN_TITLE = 65,
        ALLIANCE_CONFIRM_JIESAN_ASKSTR3 = 66,
        ALLIANCE_CONFIRM_JIESAN_ASKSTR4 = 67,
        ALLIANCE_JIESAN_SANSI = 68,
        ALLIANCE_JIESAN = 69,
        ALLIANCE_CLOSE_RECRUIT_ASKSTR1 = 70,
        ALLIANCE_CLOSE_RECRUIT_ASKSTR2 = 71,
        ALLIANCE_CLOSE_RECRUIT_TITLE = 72,
        ALLIANCE_JIESAN_SUCCESS_STR1 = 73,
        ALLIANCE_JIESAN_SUCCESS_STR2 = 74,
        ALLIANCE_CLOSE_RECRUIT_SUCCESS = 75,
        ALLIANCE_UP_LEVEL_TIPSTR1 = 76,
        ALLIANCE_UP_LEVEL_TIPSTR2 = 77,
        ALLIANCE_UP_LEVEL = 78,
        ALLIANCE_UP_LEVEL_DES = 79,
        ALLIANCE_UP_LEVEL_TITLE = 80,
        ALLIANCE_EXIT_ASKSTR1 = 81,
        ALLIANCE_EXIT_ASKSTR2 = 82,
        ALLIANCE_EXIT_TITLE = 83,
        ALLIANCE_GONGGAO_CHANGE_SUCCESS = 84,
        ALLIANCE_GONGGAO_CHANGE_TITLE = 85,
        ALLIANCE_GONGGAO_STR_TOOMUCH = 86,
        ALLIANCE_GONGGAO_CHANGE_FAIL = 87,
        ALLIANCE_EXIT_DES1 = 88,
        ALLIANCE_EXIT_DES2 = 89,
        ALLIANCE_EXIT_FAIL = 90,
        ALLIANCE_EXIT_FAIL_REASON = 91,
        YES = 92,
        NO = 93,
        ALLIANCE_RUCRUIT_TIPS_SUCCESS = 94,
        ALLIANCE_RUCRUIT_TITLE = 95,
        ALLIANCE_MEMBER_CHENGYUAN = 96,
        ALLIANCE_MEMBER_LEADER = 97,
        ALLIANCE_MEMBER_FU_LEADER = 98,
        ALLIANCE_VOTE_GIVEUP_ASKSTR1 = 99,
        ALLIANCE_VOTE_GIVEUP_ASKSTR2 = 100,
        ALLIANCE_VOTE_CONFIRM_TITLE = 101,
        ALLIANCE_VOTE_TO_SOMEONE_DES1 = 102,
        ALLIANCE_VOTE_TO_SOMEONE_DES2 = 103,
        ALLIANCE_VOTE_SUCCESS_TITLE = 104,
        ALLIANCE_VOTE_GIVEUP_DES = 105,
        ALLIANCE_VOTE_GIVEUP_TITLE = 106,
        ALLIANCE_VOTE_GIVEUP_FAILDES = 107,
        ALLIANCE_VOTE_GIVEUP_FAIL = 108,
        ALLIANCE_KAICHU_WARRING_ASKSTR1 = 109,
        ALLIANCE_KAICHU_WARRING_ASKSTR2 = 110,
        ALLIANCE_KAICHU_WARRING_TITLE = 111,
        ALLIANCE_FU_LEADER_NUM = 112,
        ALLIANCE_SHENGZHI_FAIL = 113,
        ALLIANCE_NEW_LEADER_DES1 = 114,
        ALLIANCE_NEW_LEADER_DES2 = 115,
        ALLIANCE_NEW_LEADER_TITLE = 116,
        ALLIANCE_LEADER_SELECT_DES1 = 117,
        ALLIANCE_LEADER_SELECT_DES2 = 118,
        ALLIANCE_LEADER_SELECT_TITLE = 119,
        ALLIANCE_NOT_JOIN = 120,
        ALLIANCE_JOIN_CAMPAIGN = 121,
        ALLIANCE_LEADER_ELECT_DES = 122,
        ALLIANCE_VOTE_NOW_ASK = 123,
        ALLIANCE_LEADER_ELECT_TITLE = 124,
        ALLIANCE_NOT_VOTE = 125,
        ALLIANCE_VOTE_NOW = 126,
        ALLIANCE_JOIN_SUCCESS = 127,
        ALLIANCE_ELECT_TOMORROW = 128,
        ALLIANCE_JOIN_FAIL = 129,
        ALLIANCE_TRANS_DES1 = 130,
        ALLIANCE_TRANS_DES2 = 131,
        ALLIANCE_TRANS_TITLE = 132,
        LEVEL_NOT_ENOUGH = 133,
        LEVEL_NOT_ENOUGH_DES = 134,
        ALLIANCE_NO_APPLY_DES = 135,
        ALLIANCE_TRANS_ASK = 136,
        ALLIANCE_NO_FU_LEADER_DES = 137,
        SMALL_HOUSE_EXCHANGE_CONFIRM1 = 138,
        SMALL_HOUSE_NO_EXCHANGE_CARD1 = 139,
        SMALL_HOUSE_EXCHANGED = 140,
        SMALL_HOUSE_LEADER_CANT_SWITCH = 141,
        SMALL_HOUSE_LEADER_SWITCH_TO_SELL_CONFIRM = 142,
        SMALL_HOUSE_LEADER_SWITCH_TO_SELL = 143,
        SMALL_HOUSE_LEADER_SWITCH_TO_LIVING = 144,
        HOUSE_LOCK1 = 145,
        HOUSE_BEEN_KICKED1 = 146,
        HOUSE_LOCK2 = 147,
        HOUSE_KICKED = 148,
        BIG_HOUSE_EXCHANGE_COOL_DOWN = 149,
        BIG_HOUSE_EXCHANGE_CONFIRM = 150,
        BIG_HOUSE_NO_CONTRIBUTATION = 151,
        BIG_HOUSE_EXCHANGED = 152,
        HOUSE_OWNER_BOX_EMPTY1 = 153,
        BIG_HOUSE_INFO = 154,
        SMALL_HOUSE_FITMENT1 = 155,
        SMALL_HOUSE_NO_CONTRIBUTATION = 156,
        SMALL_HOUSE_CANT_SWITCH1 = 157,
        OLD_BOOK_NOT_ENOUGH1 = 158,
        OLD_BOOK_COMBINE_SUCCEED1 = 159,
        ALL_ALLIANCE_BOX_EMPTY = 160,
        HOUSE_LOCK3 = 161,
        HOUSE_BEEN_KICKED2 = 162,
        SMALL_HOUSE_EXCHANGE_LIVING = 163,
        SMALL_HOUSE_EXCHANGE_1_AT_1_TIME = 164,
        SMALL_HOUSE_EXCHANGE_LATER = 165,
        SMALL_HOUSE_EXCHANGE_CONFIRM2 = 166,
        SMALL_HOUSE_NO_EXCHANGE_CARD2 = 167,
        SMALL_HOUSE_EXCHANGE_SENT = 168,
        HOUSE_LOCK4 = 169,
        BIG_HOUSE_BEEN_KICKED = 170,
        HOUSE_OWNER_BOX_EMPTY2 = 171,
        SMALL_HOUSE_FITMENT2 = 172,
        SMALL_HOUSE_FITMENT_NO_CONTRIBUTATION = 173,
        SMALL_HOUSE_CANT_SWITCH2 = 174,
        OLD_BOOK_NOT_ENOUGH2 = 175,
        OLD_BOOK_COMBINE_SUCCEED2 = 176,
        ALL_ALLIANCE_BOX_EMPTY2 = 177,
        CHAT_UIBOX_INFO = 178,
        CHAT_UIBOX_TOO_FAST = 179,
        CHAT_UIBOX_IS_SHIELD = 180,
        CHAT_UIBOX_SHIELD = 181,
        CHAT_UIBOX_IS_SEND_1 = 182,
        CHAT_UIBOX_IS_SEND_2 = 183,
        CHAT_UIBOX_IS_RECHARGE = 184,
        CHAT_UIBOX_NO_ALLIANCE = 185,
        AllIANCE_APPLICATION_FAILURE_TAG_LEVEL_INSUFFICIENT = 186,
        AllIANCE_APPLICATION_FAILURE_TAG_MILLITARYRANK_INSUFFICIENT = 187,
        AllIANCE_APPLICATION_SUCCESS_TAG_1 = 188,
        AllIANCE_APPLICATION_SUCCESS_TAG_2 = 189,
        AllIANCE_APPLICATION_FAILURE_APPLICATIONED = 190,
        AllIANCE_APPLICATION_SUCCESS_TITLE = 191,
        AllIANCE_APPLICATION_FAILURE_TITLE = 192,
        MAIN_CITY_TIP_MAIN = 193,
        MAIN_CITY_TIP_LEVELUP_1 = 194,
        MAIN_CITY_TIP_LEVELUP_2 = 195,
        MAIN_CITY_TIP_LEVELUP_3 = 196,
        MAIN_CITY_TIP_EQUIP_1 = 197,
        MAIN_CITY_TIP_EQUIP_2 = 198,
        MAIN_CITY_TIP_EQUIP_3 = 199,
        MAIN_CITY_TIP_EQUIP_4 = 200,
        MAIN_CITY_TIP_TREASURE_1 = 201,
        MAIN_CITY_TIP_TREASURE_2 = 202,
        MAIN_CITY_TIP_TREASURE_3 = 203,
        MAIN_CITY_TIP_TREASURE_4 = 204,
        MAIN_CITY_TIP_STRATEGY_1 = 205,
        MAIN_CITY_TIP_STRATEGY_2 = 206,
        MAIN_CITY_TIP_STRATEGY_3 = 207,
        MAIN_CITY_TIP_STRATEGY_4 = 208,
        ALLIANCE_TRANS_80 = 209,
        PVE_RESET_BTN_BOX_TITLE = 210,
        PVE_RESET_BTN_BOX_DESC = 211,
        PVE_RESET_LACK_YUANBAO = 212,
        ALLIANCE_TRANS_81 = 213,
        ALLIANCE_APPLICATION_FAILURE_NO_OPEN = 214,
        ALLIANCE_APPLICATION_FAILURE_NEED_APPROVAL = 215,
        RESTTING_CQ_TITLE = 216,
        USE_YUANBAO = 217,
        YUANBAO_RESTTING = 218,
        TODAY_RESETTING = 219,
        RESETTING_FINSHED = 220,
        TOP_UP_FORBID = 221,
        TOP_UP_SUCCESS = 222,
        TASK_GET_FAIL = 223,
        TASK_ALREADY_GET = 224,
        TASK_HAS_GET = 225,
        ALLIANCE_TRANS_226 = 226,
        ALLIANCE_TRANS_82 = 227,
        ALLIANCE_TRANS_83 = 228,
        ALLIANCE_TRANS_84 = 229,
        ALLIANCE_TRANS_85 = 230,
        ALLIANCE_TRANS_86 = 231,
        ALLIANCE_TRANS_87 = 232,
        ALLIANCE_TRANS_88 = 233,
        ALLIANCE_TRANS_89 = 234,
        ALLIANCE_TRANS_90 = 235,
        ALLIANCE_TRANS_91 = 236,
        HOUSE_CANCEL_EXCHANGE_CONFIRM = 237,
        HOUSE_CANCELED_EXCHANGE = 238,
        HOUSE_LEADER_SWITCH_TO_LIVING_CONFIRM = 239,
        FRIEND_SIGNAL_TAG_0 = 240,
        FRIEND_SIGNAL_TAG_1 = 241,
        FRIEND_SIGNAL_TAG_2 = 242,
        FRIEND_SIGNAL_TAG_3 = 243,
        FRIEND_SIGNAL_TAG_4 = 244,
        FRIEND_SIGNAL_TAG_5 = 245,
        FRIEND_SIGNAL_TAG_6 = 246,
        FRIEND_SIGNAL_TAG_7 = 247,
        FRIEND_SIGNAL_TAG_8 = 248,
        FRIEND_SIGNAL_TAG_9 = 249,
        FRIEND_SIGNAL_TAG_10 = 250,
        FRIEND_SIGNAL_TAG_11 = 251,
        FRIEND_SIGNAL_TAG_12 = 252,
        FRIEND_SIGNAL_TAG_13 = 253,
        FRIEND_SIGNAL_TAG_14 = 254,
        FRIEND_SIGNAL_TAG_15 = 255,
        ALLIANCE_TRANS_92 = 256,
        ALLIANCE_ENHANCE_257 = 257,
        VIP_SIGNAL_TAG = 258,
        ALLIANCE_TRANS_93 = 259,
        TOPUP_MONTH_CARD_TAG_0 = 260,
        TOPUP_MONTH_CARD_TAG_1 = 261,
        ALLIANCE_TRANS_94 = 262,
        ALLIANCE_TRANS_95 = 263,
        ALLIANCE_TRANS_96 = 264,
        ALLIANCE_TRANS_97 = 265,
        TIME_OUT_1 = 266,
        TIME_OUT_2 = 267,
        BAI_ZHAN_1 = 268,
        BAI_ZHAN_2 = 269,
        BAI_ZHAN_3 = 270,
        BAI_ZHAN_4 = 271,
        BAI_ZHAN_5 = 272,
        BAI_ZHAN_6 = 273,
        BAI_ZHAN_7 = 274,
        BAI_ZHAN_8 = 275,
        BAI_ZHAN_9 = 276,
        BAI_ZHAN_10 = 277,
        BAI_ZHAN_11 = 278,
        BAI_ZHAN_12 = 279,
        GONG_DA_CONDITION = 280,
        LEVEL_LIMIT = 281,
        RENWU_LIMIT = 282,
        POWER_LIMIT = 283,
        COUNTRY_1 = 284,
        COUNTRY_2 = 285,
        COUNTRY_3 = 286,
        COUNTRY_4 = 287,
        COUNTRY_5 = 288,
        COUNTRY_6 = 289,
        COUNTRY_7 = 290,
        HOUSE_DECORATE_1 = 291,
        HOUSE_DECORATE_2 = 292,
        HOUSE_DECORATE_3 = 293,
        HOUSE_DECORATE_4 = 294,
        HOUSE_FITMENT_COOL_DOWN1 = 295,
        HOUSE_FITMENT_COOL_DOWN2 = 296,
        TIME_OUT_3 = 297,
        TIME_OUT_4 = 298,
        TIME_OUT_5 = 299,
        TIME_OUT_6 = 300,
        DISTANCE_LOGIN_1 = 301,
        DISTANCE_LOGIN_2 = 302,
        CHANGE_COUNTRY_TIP = 303,
        ACTIVITY_SIGH = 304,
        ACTIVITY_CHARGE_1 = 305,
        ACTIVITY_CHARGE_2 = 306,
        ACTIVITY_WAITING = 307,
        MIBAO_ENHANCE_1 = 308,
        MIBAO_ENHANCE_2 = 309,
        MIBAO_ENHANCE_3 = 310,
        MIBAO_ENHANCE_4 = 311,
        DAMAGE_CHANGE_1 = 312,
        DAMAGE_CHANGE_2 = 313,
        DAMAGE_CHANGE_3 = 314,
        DAMAGE_CHANGE_4 = 315,
        DAMAGE_CHANGE_5 = 316,
        DAMAGE_CHANGE_6 = 317,
        DAMAGE_CHANGE_7 = 318,
        DAMAGE_CHANGE_8 = 319,
        BUY_1 = 320,
        BUY_2 = 321,
        BUY_3 = 322,
        BUY_4 = 323,
        BUY_5 = 324,
        BUY_6 = 325,
        TITITLE = 326,
        JUNZHU_LV = 327,
        SAODANGGUANQIA = 328,
        LOST_CONNECTION_1 = 329,
        LOST_CONNECTION_2 = 330,
        LOST_CONNECTION_3 = 331,
        LOST_CONNECTION_4 = 332,
        WAIT_TIPS_1 = 333,
        WAIT_TIPS_2 = 334,
        WAIT_TIPS_3 = 335,
        WAIT_TIPS_4 = 336,
        WAIT_TIPS_5 = 337,
        WAIT_TIPS_6 = 338,
        WAIT_TIPS_7 = 339,
        WAIT_TIPS_8 = 340,
        WAIT_TIPS_9 = 341,
        WAIT_TIPS_10 = 342,
        WAIT_TIPS_11 = 343,
        WAIT_TIPS_12 = 344,
        WAIT_TIPS_13 = 345,
        WAIT_TIPS_14 = 346,
        WAIT_TIPS_15 = 347,
        WAIT_TIPS_16 = 348,
        BAIZHAN_RULE_1 = 349,
        BAIZHAN_RULE_2 = 350,
        BAIZHAN_RULE_3 = 351,
        BAIZHAN_RULE_4 = 352,
        BAIZHAN_RULE_5 = 353,
        BAIZHAN_RULE_6 = 354,
        BAIZHAN_RULE_7 = 355,
        BAIZHAN_RULE_8 = 356,
        BAIZHAN_RULE_9 = 357,
        HUANGYE_1 = 358,
        HUANGYE_2 = 359,
        HUANGYE_3 = 360,
        HUANGYE_4 = 361,
        HUANGYE_5 = 362,
        HUANGYE_6 = 363,
        HUANGYE_7 = 364,
        HUANGYE_8 = 365,
        HUANGYE_9 = 366,
        HUANGYE_10 = 367,
        HUANGYE_11 = 368,
        HUANGYE_12 = 369,
        HUANGYE_13 = 370,
        HUANGYE_14 = 371,
        HUANGYE_15 = 372,
        HUANGYE_16 = 373,
        HUANGYE_17 = 374,
        HUANGYE_18 = 375,
        HUANGYE_19 = 376,
        HUANGYE_20 = 377,
        HUANGYE_21 = 378,
        HUANGYE_22 = 379,
        HUANGYE_23 = 380,
        HUANGYE_24 = 381,
        HUANGYE_25 = 382,
        HUANGYE_26 = 383,
        HUANGYE_27 = 384,
        HUANGYE_28 = 385,
        HUANGYE_29 = 386,
        HUANGYE_30 = 387,
        HUANGYE_31 = 388,
        HUANGYE_32 = 389,
        HUANGYE_33 = 390,
        HUANGYE_34 = 391,
        HUANGYE_35 = 392,
        HUANGYE_36 = 393,
        HUANGYE_37 = 394,
        HUANGYE_38 = 395,
        HUANGYE_39 = 396,
        HUANGYE_40 = 397,
        HUANGYE_41 = 398,
        HUANGYE_42 = 399,
        HUANGYE_43 = 400,
        HUANGYE_44 = 401,
        HUANGYE_45 = 402,
        HUANGYE_46 = 403,
        HUANGYE_47 = 404,
        HUANGYE_48 = 405,
        HUANGYE_49 = 406,
        HUANGYE_50 = 407,
        HUANGYE_51 = 408,
        HUANGYE_52 = 409,
        HUANGYE_53 = 410,
        HUANGYE_54 = 411,
        HUANGYE_55 = 412,
        HUANGYE_56 = 413,
        HUANGYE_57 = 414,
        HUANGYE_58 = 415,
        HUANGYE_59 = 416,
        HUANGYE_60 = 417,
        HUANGYE_61 = 418,
        HUANGYE_62 = 419,
        HUANGYE_63 = 420,
        HUANGYE_64 = 421,
        HUANGYE_65 = 422,
        HUANGYE_66 = 423,
        HUANGYE_67 = 424,
        LEAVE_FIGHTING = 425,
        HOUSE_ALLIANCE_TIME_INSUFFICIENT = 426,
        BAI_ZHAN_13 = 427,
        MIBAO_LEVEL_UP_FAILE = 428,
        FINGHT_CONDITON = 429,
        CHAT_NOTENE = 430,
        HUANGYE_68 = 431,
        HUANGYE_69 = 432,
        YUN_BIAO_1 = 434,
        YUN_BIAO_2 = 435,
        YUN_BIAO_3 = 436,
        YUN_BIAO_4 = 437,
        YUN_BIAO_5 = 438,
        YUN_BIAO_6 = 439,
        YUN_BIAO_7 = 440,
        YUN_BIAO_8 = 441,
        YUN_BIAO_9 = 442,
        YUN_BIAO_10 = 443,
        YUN_BIAO_11 = 444,
        YUN_BIAO_12 = 445,
        YUN_BIAO_13 = 446,
        YUN_BIAO_14 = 447,
        YUN_BIAO_15 = 448,
        YUN_BIAO_16 = 449,
        YUN_BIAO_17 = 450,
        YUN_BIAO_18 = 451,
        YUN_BIAO_19 = 452,
        YUN_BIAO_20 = 453,
        YUN_BIAO_21 = 454,
        YUN_BIAO_22 = 455,
        YUN_BIAO_23 = 456,
        YUN_BIAO_24 = 457,
        YUN_BIAO_25 = 458,
        YUN_BIAO_26 = 459,
        YUN_BIAO_27 = 460,
        YUN_BIAO_28 = 461,
        YUN_BIAO_29 = 462,
        YUN_BIAO_30 = 463,
        YUN_BIAO_31 = 464,
        YUN_BIAO_32 = 465,
        YUN_BIAO_33 = 466,
        YUN_BIAO_34 = 467,
        YUN_BIAO_35 = 468,
        YUN_BIAO_36 = 469,
        YUN_BIAO_37 = 470,
        YUN_BIAO_38 = 471,
        YUN_BIAO_39 = 472,
        YUN_BIAO_40 = 473,
        YUN_BIAO_41 = 474,
        YUN_BIAO_42 = 475,
        YUN_BIAO_43 = 476,
        YUN_BIAO_44 = 477,
        YUN_BIAO_45 = 478,
        YUN_BIAO_46 = 479,
        YUN_BIAO_47 = 480,
        YUN_BIAO_48 = 481,
        YUN_BIAO_49 = 482,
        YUN_BIAO_50 = 483,
        YUN_BIAO_51 = 484,
        YUN_BIAO_52 = 485,
        YUN_BIAO_53 = 486,
        YUN_BIAO_54 = 487,
        YUN_BIAO_55 = 488,
        YUN_BIAO_56 = 489,
        YUN_BIAO_57 = 490,
        YUN_BIAO_58 = 491,
        YUN_BIAO_59 = 492,
        YUN_BIAO_60 = 493,
        YUN_BIAO_61 = 494,
        YUN_BIAO_62 = 495,
        YUN_BIAO_63 = 496,
        YUN_BIAO_64 = 497,
        YUN_BIAO_65 = 498,
        YUN_BIAO_66 = 499,
        YUN_BIAO_67 = 500,
        YUN_BIAO_68 = 501,
        YUN_BIAO_69 = 502,
        YUN_BIAO_70 = 503,
        YUN_BIAO_71 = 504,
        YUN_BIAO_72 = 505,
        YUN_BIAO_73 = 506,
        YUN_BIAO_74 = 507,
        YUN_BIAO_75 = 508,
        YUN_BIAO_76 = 509,
        YUN_BIAO_77 = 510,
        YOU_XIA_1 = 511,
        YOU_XIA_2 = 512,
        YOU_XIA_3 = 513,
        YOU_XIA_4 = 514,
        YOU_XIA_5 = 515,
        YOU_XIA_6 = 516,
        YOU_XIA_7 = 517,
        YOU_XIA_8 = 518,
        YOU_XIA_9 = 519,
        YOU_XIA_10 = 520,
        YOU_XIA_11 = 521,
        YOU_XIA_12 = 522,
        YOU_XIA_13 = 523,
        YOU_XIA_14 = 524,
        YOU_XIA_15 = 525,
        YOU_XIA_16 = 526,
        YOU_XIA_17 = 527,
        YOU_XIA_18 = 528,
        YOU_XIA_19 = 529,
        YOU_XIA_20 = 530,
        YOU_XIA_21 = 531,
        YOU_XIA_22 = 532,
        YUN_BIAO_78 = 533,
        YUN_BIAO_79 = 534,
        YUN_BIAO_80 = 535,
        YUN_BIAO_81 = 536,
        YUN_BIAO_82 = 537,
        YUN_BIAO_83 = 538,
        NO_TIME_1 = 539,
        NO_TIME_2 = 540,
        NO_FRIENDS_1 = 541,
        NO_PERSON_IN_LINE_1 = 542,
        QUIT_TIPS_1 = 543,
        MACHINE_TIPS_1 = 544,

        YUN_BIAO_84 = 545,
        YUN_BIAO_85 = 546,

        LIMIT_TIME_ACTIVITIES_1 = 546,
        LIMIT_TIME_ACTIVITIES_2 = 547,
        LIMIT_TIME_ACTIVITIES_3 = 548,
        LIMIT_TIME_ACTIVITIES_4 = 549,
        LIMIT_TIME_ACTIVITIES_5 = 550,
        LIMIT_TIME_ACTIVITIES_6 = 551,
        LIMIT_TIME_ACTIVITIES_7 = 552,
        LIMIT_TIME_ACTIVITIES_8 = 553,
        LIMIT_TIME_ACTIVITIES_9 = 554,
        LIMIT_TIME_ACTIVITIES_10 = 555,
        LIMIT_TIME_ACTIVITIES_11 = 556,
        LIMIT_TIME_ACTIVITIES_12 = 557,
        LIMIT_TIME_ACTIVITIES_13 = 558,
        LIMIT_TIME_ACTIVITIES_14 = 559,
        LIMIT_TIME_ACTIVITIES_15 = 560,
        LIMIT_TIME_ACTIVITIES_16 = 561,
        LIMIT_TIME_ACTIVITIES_17 = 562,
        LIMIT_TIME_ACTIVITIES_18 = 563,
        LIMIT_TIME_ACTIVITIES_19 = 564,
        LIMIT_TIME_ACTIVITIES_20 = 565,

        SET_UP_1 = 567,
        SET_UP_2 = 568,
        SET_UP_3 = 569,
        SET_UP_4 = 570,

        ZHU_BU_RULE_1 = 582,
        ZHU_BU_RULE_2 = 583,
        ZHU_BU_RULE_3 = 584,
        ZHU_BU_RULE_4 = 585,

        LOADING_TIPS_1 = 1000,
        LOADING_TIPS_2 = 1001,
        LOADING_TIPS_3 = 1002,
        LOADING_TIPS_4 = 1003,
        LOADING_TIPS_5 = 1004,
        LOADING_TIPS_6 = 1005,
        LOADING_TIPS_7 = 1006,
        LOADING_TIPS_8 = 1007,
        LOADING_TIPS_9 = 1008,
        LOADING_TIPS_10 = 1009,
        LOADING_TIPS_11 = 1010,
        LOADING_TIPS_12 = 1011,
        LOADING_TIPS_13 = 1012,
        LOADING_TIPS_14 = 1013,
        LOADING_TIPS_15 = 1014,
        LOADING_TIPS_16 = 1015,
        LOADING_TIPS_17 = 1016,
        LOADING_TIPS_18 = 1017,
        YUAN_BAO_XILIAN_TIP = 1018,
        WORSHIP_DINGLI_TIP = 1019,
        EQUIP_OVERSTEP_LEVEL = 1020,
        YIJIQIANGHUA_TITLE = 1021,
        YIJIQIANGHUA_CONTENT = 1022,
        EQUIPEXP_OVERSTEP = 1023,
        YIJIQIANGHUA_CONTENT2 = 1024,
        SETTINGUP_CHANGE_COUNTRY_ALLIANCE_1 = 1025,
        SETTINGUP_CHANGE_COUNTRY_ALLIANCE_2 = 1026,
        SETTINGUP_CHANGE_COUNTRY_ALLIANCE_3 = 1027,
        SETTINGUP_CHANGE_COUNTRY_CARD_1 = 1028,
        SETTINGUP_CHANGE_COUNTRY_CARD_USE = 1029,
        SETTINGUP_CHANGE_COUNTRY_SUCCESS = 1030,
        GET_EXP = 1031,
        UPGRADE_SUCCESS = 1032,
        INTENSIFY_SUCCESS = 1033,
        BUY_MATERIAL = 1034,
        INTENSIFY_MAX_LEVEL = 1035,
        JUNZHU_LEVEUP = 1036,
        WASH_TAG = 1037,
        NEW_EMAIL = 1038,
        COUNTRY_NOTICE_1 = 1039,
        COUNTRY_NOTICE_2 = 1040,
        COUNTRY_NOTICE_3 = 1041,
        COUNTRY_NOTICE_4 = 1042,
        COUNTRY_NOTICE_5 = 1043,
        COUNTRY_NOTICE_6 = 1044,
        COUNTRY_NOTICE_7 = 1045,
        NATION_LINGQUJIANGLI_TITLE = 1046,
        NATION_LINGQUJIANGLI_CONTENT1 = 1047,
        NATION_LINGQUJIANGLI_CONTENT2 = 1048,
        NATION_SHANGJIAO_TITLE = 1049,
        NATION_SHANGJIAO_Content1 = 1050,
        NATION_SHANGJIAO_Content2 = 1051,
        NATION_SHANGJIAO_Content3 = 1052,
        NATION_SHANGJIAO_Content4 = 1053,
        NATION_SHANGJIAO_Content5 = 1054,
        NATION_SHANGJIAO_Content6 = 1055,
        NATION_SHANGJIAO_Content7 = 1056,
        ALLIANCE_INPUT_SIGNAL = 1057,
        ALLIANCE_APPLY_HOUR = 1058,
        GEIVE_NOT_ENOUGH = 1059,
        GEIVE_NO_REWARD = 1060,
        FUNCTION_NO_OPEN = 1061,
        NO_ALLIANCE = 1062,
        GONGJIN_NO_ENOUGH = 1063,
        GONGJIN_TIME_FULL = 1064,
        GONGJIN_NO_START = 1065,
        ALLIANCE_NOTICE_DEFAULT = 1066,	

		SHIJIAN_CHUANGJIAN = 1067,
		SHIJIAN_JIARU = 1068,
		SHIJIAN_KAICHU = 1069,
		SHIJIAN_TUICHU = 1070,
		SHIJIAN_ZHUANRANG = 1071,
		SHIJIAN_SHENGZHI = 1072,
		SHIJIAN_JIANGZHI = 1073,
		SHIJIAN_GONGGAO = 1074,
		SHIJIAN_JUANXIAN = 1075,
		SHIJIAN_SHENGJI = 1076,
		SHIJIAN_CANGBAOKAIQI = 1077,
		SHIJIAN_CANGBAOGONGPO = 1078,
		SHIJIAN_ZIYUANZHANLING = 1079,
		SHIJIAN_LUODUO = 1080,
		SHIJIAN_SHANGJIAO = 1081,
		YOU_XIA_TIPS_1 = 1082,
		YOU_XIA_TIPS_2 = 1083,
		YOU_XIA_TIPS_3 = 1084,
		YOU_XIA_TIPS_4 = 1085,
		YOU_XIA_TIPS_5 = 1086,
		YOU_XIA_TIPS_6 = 1087,
		YOU_XIA_TIPS_7 = 1088,
		YOU_XIA_TIPS_8 = 1089,
		YOU_XIA_TIPS_9 = 1090,
		YOU_XIA_TIPS_10 = 1091,
		YOU_XIA_TIPS_11 = 1092,
		YOU_XIA_TIPS_12 = 1093,

        MAIN_CITY_TIP_1=1094,
        MAIN_CITY_TIP_2=1095,
        GUO_JIA_TIPS = 1096,

		YUN_BIAO_SCENE_1 = 1097,
		YUN_BIAO_SCENE_2 = 1098,
		YUN_BIAO_SCENE_3 = 1099,
		YUN_BIAO_SCENE_4 = 1100,
		YUN_BIAO_SCENE_5 = 1101,
		YUN_BIAO_SCENE_6 = 1102,
		YUN_BIAO_SCENE_7 = 1103,
		YUN_BIAO_SCENE_8 = 1104,
		YUN_BIAO_SCENE_9 = 1105,
		YUN_BIAO_SCENE_10 = 1106,
		YUN_BIAO_SCENE_11 = 1107,
		YUN_BIAO_SCENE_12 = 1108,
		YUN_BIAO_SCENE_13 = 1109,
		YUN_BIAO_SCENE_14 = 1110,
		YUN_BIAO_SCENE_15 = 1111,
		YUN_BIAO_SCENE_16 = 1112,
		YUN_BIAO_SCENE_17 = 1113,
		YUN_BIAO_SCENE_18 = 1114,
		YUN_BIAO_SCENE_19 = 1115,
		YUN_BIAO_SCENE_20 = 1116,
		YUN_BIAO_SCENE_21 = 1117,
		YUN_BIAO_SCENE_22 = 1118,
		YUN_BIAO_SCENE_23 = 1119,
		YUN_BIAO_SCENE_24 = 1120,
		YUN_BIAO_SCENE_25 = 1121,
		YUN_BIAO_SCENE_26 = 1122,
		YUN_BIAO_SCENE_27 = 1123,
		YUN_BIAO_SCENE_28 = 1124,
		YUN_BIAO_SCENE_29 = 1125,
		YUN_BIAO_SCENE_30 = 1126,
		YUN_BIAO_SCENE_31 = 1127,
		YUN_BIAO_SCENE_32 = 1128,
		YUN_BIAO_SCENE_33 = 1129,
		YUN_BIAO_SCENE_34 = 1130,
		YUN_BIAO_SCENE_35 = 1131,
		YUN_BIAO_SCENE_36 = 1132,
		YUN_BIAO_SCENE_37 = 1133,
		YUN_BIAO_SCENE_38 = 1134,
		YUN_BIAO_SCENE_39 = 1135,
		YUN_BIAO_SCENE_40 = 1136,
		YUN_BIAO_SCENE_41 = 1137,
		YUN_BIAO_SCENE_42 = 1138,
		YUN_BIAO_SCENE_43 = 1139,
		YUN_BIAO_SCENE_44 = 1140,
		YUN_BIAO_SCENE_45 = 1141,
		YUN_BIAO_SCENE_46 = 1142,
		YUN_BIAO_SCENE_47 = 1143,
		YUN_BIAO_SCENE_48 = 1144,
		YUN_BIAO_SCENE_49 = 1145,
		YUN_BIAO_SCENE_50 = 1146,
		YUN_BIAO_SCENE_51 = 1147,
		YUN_BIAO_SCENE_52 = 1148,
		YUN_BIAO_SCENE_53 = 1149,
		YUN_BIAO_SCENE_54 = 1150,
		YUN_BIAO_SCENE_55 = 1151,
		YUN_BIAO_SCENE_56 = 1152,
		YUN_BIAO_SCENE_57 = 1153,
		YUN_BIAO_SCENE_58 = 1154,
		YUN_BIAO_SCENE_59 = 1155,
		YUN_BIAO_SCENE_60 = 1156,
		YUN_BIAO_SCENE_61 = 1157,
		YUN_BIAO_SCENE_62 = 1158,
		PAI_HANG_BANG_01 = 1159,
		PAI_HANG_BANG_02 = 1160,
		PAI_HANG_BANG_03 = 1161,
		PAI_HANG_BANG_04 = 1162,
		BUILD_UNION_1 = 1163,
		BUILD_UNION_2 = 1164,
		ALLIANCE_TSG_TITLE = 1165,
		ALLIANCE_TSG_0 = 1166,
		ALLIANCE_TSG_1 = 1167,
		ALLIANCE_TSG_2 = 1168,
		ALLIANCE_TSG_3 = 1169,
		TOPUP_SIGNAL = 1170,
		LUEDUO_ZHUZHAN = 1171,
        NATION_REWARD = 1172,
        NATION_REWARD_1 = 1173,
        NATION_REWARD_2 = 1174,
		HUANG_YE_TIPS_1 = 1175,
		HUANG_YE_TIPS_2,
		HUANG_YE_TIPS_3,
		HUANG_YE_TIPS_4,
		HUANG_YE_TIPS_5,
		HUANG_YE_TIPS_6,
		HUANG_YE_TIPS_7,
		HUANG_YE_TIPS_8,
		HUANG_YE_TIPS_9,
		HUANG_YE_TIPS_10,
		HUANG_YE_TIPS_11 = 1185,
		HUANG_YE_TIPS_12,
		HUANG_YE_TIPS_13,
		HUANG_YE_TIPS_14,
		HUANG_YE_TIPS_15,
		HUANG_YE_TIPS_16,
		HUANG_YE_TIPS_17,
		HUANG_YE_TIPS_18,
		HUANG_YE_TIPS_19,
		HUANG_YE_TIPS_20,
		HUANG_YE_TIPS_21= 1195,
		I_WANT_POWER_1,
		I_WANT_POWER_2,
		I_WANT_POWER_3,
		I_WANT_POWER_4,
		I_WANT_POWER_5,
		I_WANT_POWER_6,
		I_WANT_POWER_7,
		I_WANT_POWER_8,
		I_WANT_POWER_9,
		I_WANT_POWER_10= 1205,
		I_WANT_POWER_11,
		I_WANT_POWER_12,
		I_WANT_POWER_13,
		I_WANT_POWER_14,
		I_WANT_POWER_15,
		I_WANT_POWER_16,
		I_WANT_POWER_17,
		I_WANT_POWER_18,
		I_WANT_POWER_19,
		I_WANT_POWER_20= 1215,
		I_WANT_POWER_21,
		I_WANT_POWER_22,
		I_WANT_POWER_23,
		I_WANT_POWER_24,
		I_WANT_POWER_25,
		I_WANT_POWER_26,
		I_WANT_POWER_27,
		I_WANT_POWER_28,
		I_WANT_POWER_29,

		TIP_1 = 1225,
		TIP_2 = 1226,
		TIP_3 = 1247,

		EQUIP_FUNCTION = 1227,
		EQUIP_STRENGTH = 1228,
		EQUIP_WASH = 1229,
		EQUIP_UPGRADE = 1230,
        EQUIP_WEAR = 1231,
        NO_ALLIANCE_TEXT = 1234,
        LEVEL_UP_SIGNAL = 1235,
        LEVEL_UP_SIGNAL_1 = 1236,
        WASH_STONE_MAX_TIME = 1237,
		UNIT_WAR_RULE = 1238,
        EXCHANG_FAIL_1=1239,
        EXCHANG_FAIL_2=1240,
		STRENGTH_SUFFICIENT = 1241,
        IDENTITY_0 = 1244,
        IDENTITY_1 = 1245,
        IDENTITY_2 = 1246,
        WASH_INFO = 1249,
        WASH_INFO1 = 1250,
        JUNZHU_EQUIP_SIGNAL = 1251,
        JUNZHU_EQUIP_SIGNAL1 = 1252,
        JUNZHU_EQUIP_SIGNAL2 = 1253,
        JUNZHU_EQUIP_SIGNAL3 = 1254,
        TOPUP_SS = 1255,
        TARGET_SIGNAL_GREEN = 1256,
        TARGET_SIGNAL_BLUE = 1257,
        TARGET_SIGNAL_PURPLE = 1258,
        TARGET_SIGNAL_ORANGE = 1259,
        TARGET_SIGNAL_BEST = 1260,
		Name_1 = 1261,
		Name_2 = 1262,

        TARGET_SIGNAL = 1263,
        XILIAN_DESC_1 = 1264,
        XILIAN_DESC_2 = 1265,
        XILIAN_DESC_3 = 1266,
        XILIAN_DESC_4 = 1267,
        XILIAN_DESC_5 = 1268,
        XILIAN_DESC_6 = 1269,
        XILIAN_DESC_7 = 1270,
        LACK_OF_QIANGHUACL = 1271,

        ALLIANCE_TAG_ADD = 1272,
        ALLIANCE_TAG_ADD_1 = 1273,
        ALLIANCE_TAG_ADD_2 = 1274,
        ALLIANCE_TAG_ADD_3 = 1275,
        ALLIANCE_TAG_ADD_4 = 1276,
        XILIAN_DESC_8 = 1277,
        XILIAN_DESC_9 = 1278,
        XILIAN_DESC_10 = 1279,
        XILIAN_DESC_11 = 1280,
    }

    public int lanId;

    public string m_text;

	private static List<LanguageTemplate> m_templates = new List<LanguageTemplate>();


    private static LanguageTemplate m_instance = null;

    public static LanguageTemplate Instance()
    {
        if (m_instance == null)
        {
            m_instance = new LanguageTemplate();
        }

        return m_instance;
    }



    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "LanguageTemp.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string text, Object obj)
    {
		if( m_templates.Count > 0 ) {
			return;
		}

        XmlReader t_reader = null;

        if (obj != null)
        {
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
            t_has_items = t_reader.ReadToFollowing("LanTemp");

            if (!t_has_items)
            {
                break;
            }

            LanguageTemplate t_template = new LanguageTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.lanId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_text = t_reader.Value;
            }

            m_templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static string GetText(Text p_text_enum)
    {
        int t_id = (int)p_text_enum;

        return GetText(t_id);
    }

    public static string GetText(int p_text_id)
    {
		if (m_templates == null) {
			return null;
		}

        int t_id = p_text_id;

        foreach (LanguageTemplate t_template in m_templates)
        {
            if (t_template.lanId == t_id)
            {
                return t_template.m_text;
            }
        }

        Debug.LogError("XML ERROR: Can't get LanguageTemplate with Id: " + t_id);

        return "";
    }

}
