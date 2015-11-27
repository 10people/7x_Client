using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Object = UnityEngine.Object;

public class CanshuTemplate : XmlLoadManager
{
    public static string SHANGHAI_GONGSHI_XISHU1 = "SHANGHAI_GONGSHI_XISHU1";
    public static string SHANGHAI_GONGSHI_XISHU2 = "SHANGHAI_GONGSHI_XISHU2";
    public static string SHANGHAI_GONGSHI_XISHU3 = "SHANGHAI_GONGSHI_XISHU3";
    public static string ZHUCHENG_JINGTOU_X = "ZHUCHENG_JINGTOU_X";
    public static string ZHUCHENG_JINGTOU_Y = "ZHUCHENG_JINGTOU_Y";
    public static string ZHUCHENG_JINGTOU_Z = "ZHUCHENG_JINGTOU_Z";
    public static string ZHUCHENG_JINGTOU_R = "ZHUCHENG_JINGTOU_R";
    public static string ZHANDOU_JINGTOU_X = "ZHANDOU_JINGTOU_X";
    public static string ZHANDOU_JINGTOU_Y = "ZHANDOU_JINGTOU_Y";
    public static string ZHANDOU_JINGTOU_Z = "ZHANDOU_JINGTOU_Z";
    public static string ZHANDOU_JINGTOU_R = "ZHANDOU_JINGTOU_R";
    public static string ADD_TILI_INTERVAL_TIME = "ADD_TILI_INTERVAL_TIME";
    public static string ADD_TILI_INTERVAL_VALUE = "ADD_TILI_INTERVAL_VALUE";
    public static string ADD_XILIAN_VALUE = "ADD_XILIAN_VALUE";
    public static string ADD_XILIAN_INTERVAL_TIME = "ADD_XILIAN_INTERVAL_TIME";
    public static string FREE_XILIAN_TIMES_MAX = "FREE_XILIAN_TIMES_MAX";
    public static string CHOUKA_TIANGONGTU = "CHOUKA_TIANGONGTU";
    public static string JUNZHU_PUGONG_QUANZHONG = "JUNZHU_PUGONG_QUANZHONG";
    public static string JUNZHU_PUGONG_BEISHU = "JUNZHU_PUGONG_BEISHU";
    public static string JUNZHU_JINENG_QUANZHONG = "JUNZHU_JINENG_QUANZHONG";
    public static string JUNZHU_JINENG_BEISHU = "JUNZHU_JINENG_BEISHU";
	public static string JUNZHU_MIBAOJIACHENG_BD = "JUNZHU_MIBAOJIACHENG_BD";
	public static string JUNZHU_MIBAOJIACHENG_ZD = "JUNZHU_MIBAOJIACHENG_ZD";
    public static string WUJIANG_PUGONG_QUANZHONG = "WUJIANG_PUGONG_QUANZHONG";
    public static string WUJIANG_PUGONG_BEISHU = "WUJIANG_PUGONG_BEISHU";
    public static string WUJIANG_JINENG_QUANZHONG = "WUJIANG_JINENG_QUANZHONG";
    public static string WUJIANG_JINENG_BEISHU = "WUJIANG_JINENG_BEISHU";
    public static string SHIBING_PUGONG_QUANZHONG = "SHIBING_PUGONG_QUANZHONG";
    public static string SHIBING_PUGONG_BEISHU = "SHIBING_PUGONG_BEISHU";
    public static string SHIBING_JINENG_QUANZHONG = "SHIBING_JINENG_QUANZHONG";
    public static string SHIBING_JINENG_BEISHU = "SHIBING_JINENG_BEISHU";
    public static string ZHANLI_M = "ZHANLI_M";
    public static string ZHANLI_C = "ZHANLI_C";
    public static string ZHANLI_R = "ZHANLI_R";
    public static string ZHANLI_L = "ZHANLI_L";
    public static string YUANSUBEISHU_1 = "YUANSUBEISHU_1";
    public static string YUANSUBEISHU_2 = "YUANSUBEISHU_2";
    public static string YUANSUBEISHU_3 = "YUANSUBEISHU_3";
    public static string YUANSUBEISHU_4 = "YUANSUBEISHU_4";
    public static string YUANSUBEISHU_5 = "YUANSUBEISHU_5";
    public static string TIME_KEJI_LIMIT = "TIME_KEJI_LIMIT";
    public static string CHUSHIHUA_CHUANDAIZHUANGBEI_1 = "CHUSHIHUA_CHUANDAIZHUANGBEI_1";
    public static string CHUSHIHUA_CHUANDAIZHUANGBEI_2 = "CHUSHIHUA_CHUANDAIZHUANGBEI_2";
    public static string CHUSHIHUA_CHUANDAIZHUANGBEI_3 = "CHUSHIHUA_CHUANDAIZHUANGBEI_3";
    public static string CHUSHIHUA_WUJIANG = "CHUSHIHUA_WUJIANG";
    public static string MAXTIME_PVE = "MAXTIME_PVE";
    public static string MAXTIME_BAIZHAN = "MAXTIME_BAIZHAN";
	public static string MAXTIME_HUANGYE_PVE = "MAXTIME_HUANGYE_PVE";
	public static string MAXTIME_HUANGYE_PVP = "MAXTIME_HUANGYE_PVP";
	public static string MAXTIME_JIEBIAO = "MAXTIME_JIEBIAO";
	public static string MAXTIME_LUEDUO = "MAXTIME_LUEDUO";
    public static string LIANMENG_CREATE_COST = "LIANMENG_CREATE_COST";
	public static string BAIZHAN_LVEDUO_JIANSHEZHI = "BAIZHAN_LVEDUO_JIANSHEZHI";
	public static string BAIZHAN_LVEDUO_K = "BAIZHAN_LVEDUO_K";
	public static string BAIZHAN_NPC_WEIWANG = "BAIZHAN_NPC_WEIWANG";
	public static string BAIZHAN_WEIWANG_ADDLIMIT = "BAIZHAN_WEIWANG_ADDLIMIT";
	public static string YUEKA_TIME = "YUEKA_TIME";
	public static string YUEKA_YUANBAO = "YUEKA_YUANBAO";
	public static string CHAT_INTERVAL_TIME = "CHAT_INTERVAL_TIME";
	public static string CHAT_MAX_WORDS = "CHAT_MAX_WORDS"; 
    public static string HUANGYEPVE_AWARD_X = "HUANGYEPVE_AWARD_X";
	public static string HUANGYEPVP_PRODUCE_P = "HUANGYEPVP_PRODUCE_P";
	public static string HUANGYEPVP_KILL_K = "HUANGYEPVP_KILL_K";
	public static string HUANGYEPVP_AWARDTIME = "HUANGYEPVP_AWARDTIME";
	public static string REFRESHTIME_DANGPU = "REFRESHTIME_DANGPU"; 
	public static string REFRESHTIME_YUANBAOXILIAN = "REFRESHTIME_YUANBAOXILIAN";
	public static string REFRESHTIME_GAOJIFANGWU = "REFRESHTIME_GAOJIFANGWU";
    public static string ENDTIME_PVE_WIN = "ENDTIME_PVE_WIN";
    public static string ENDTIME_PVE_LOSE = "ENDTIME_PVE_LOSE";
    public static string ENDTIME_BAIZHAN_WIN = "ENDTIME_BAIZHAN_WIN";
    public static string ENDTIME_BAIZHAN_LOSE = "ENDTIME_BAIZHAN_LOSE";
	public static string ENDTIME_YOUXIA = "ENDTIME_YOUXIA";
    public static string LASTTIME_PVE = "LASTTIME_PVE";
	public static string PURCHASE_GONGBI = "PURCHASE_GONGBI";
	public static string HUANGYEPVE_FASTCLEAR_TIME = "HUANGYEPVE_FASTCLEAR_TIME";
	public static string FANGWUJINGPAI_1 = "FANGWUJINGPAI_1";
	public static string FANGWUJINGPAI_2 = "FANGWUJINGPAI_2";
	public static string FANGWUJINGPAI_3 = "FANGWUJINGPAI_3";
	public static string FANGWUJINGPAI_4 = "FANGWUJINGPAI_4";
	public static string MEIRI_DINGSHIZENGSONG_TILI = "MEIRI_DINGSHIZENGSONG_TILI";
	public static string TILI_JILEI_SHANGXIAN = "TILI_JILEI_SHANGXIAN";
	public static string DIJI_TANBAO_REFRESHTIME = "DIJI_TANBAO_REFRESHTIME";
	public static string GAOJI_TANBAO_REFRESHTIME = "GAOJI_TANBAO_REFRESHTIME";
	public static string MOVESPEED_CHENGNEI_JUNZHU = "MOVESPEED_CHENGNEI_JUNZHU";
	public static string BATTLE_SLOWDOWN_VALUE = "BATTLE_SLOWDOWN_VALUE";
	public static string BATTLE_SLOWDOWN_TIME = "BATTLE_SLOWDOWN_TIME";
	public static string XILIANADD_MIN = "XILIANADD_MIN";
	public static string XILIANADD_MAX = "XILIANADD_MAX";
	public static string YUNBIAO_MAXNUM = "YUNBIAO_MAXNUM";
	public static string JIEBIAO_MAXNUM = "JIEBIAO_MAXNUM";
	public static string JIEBIAO_CD = "JIEBIAO_CD";
	public static string CART_MAXNUM = "CART_MAXNUM";
	public static string REFRESHTIME_YUNBIAO = "REFRESHTIME_YUNBIAO";
	public static string OPENTIME_YUNBIAO = "OPENTIME_YUNBIAO";
	public static string CLOSETIME_YUNBIAO = "CLOSETIME_YUNBIAO";
	public static string OPENTIME_LUEDUO = "OPENTIME_LUEDUO";
	public static string CLOSETIME_LUEDUO = "CLOSETIME_LUEDUO";
	public static string LUEDUO_MAXNUM = "LUEDUO_MAXNUM";
	public static string LUEDUO_CD = "LUEDUO_CD";
	public static string TIANFULV_DIANSHUADD1 = "TIANFULV_DIANSHUADD1";
	public static string TIANFULV_DIANSHUADD2 = "TIANFULV_DIANSHUADD2";
	public static string TIANFULV_DIANSHUADD3 = "TIANFULV_DIANSHUADD3";
	public static string TIANFULV_DIANSHUADD4 = "TIANFULV_DIANSHUADD4";
	public static string TIANFULV_DIANSHUADD5 = "TIANFULV_DIANSHUADD5";    
    public static string VIPLV_ININT = "VIPLV_ININT";
    public static string IS_YUEKA_INIT = "IS_YUEKA_INIT";
    public static string YUNBIAOASSISTANCE_INVITEDMAXNUM = "YUNBIAOASSISTANCE_INVITEDMAXNUM";
    public static string YUNBIAOASSISTANCE_MAXNUM = "YUNBIAOASSISTANCE_MAXNUM";
    public static string YUNBIAOASSISTANCE_GAIN_SUCCEED = "YUNBIAOASSISTANCE_GAIN_SUCCEED";
    public static string YUNBIAOASSISTANCE_GAIN_FAIL = "YUNBIAOASSISTANCE_GAIN_FAIL";
    public static string YUNBIAOASSISTANCE_HPBONUS = "YUNBIAOASSISTANCE_HPBONUS";
	public static string LUEDUO_PROTECTTIME = "LUEDUO_PROTECTTIME";
	public static string LUEDUO_RECOVER_INTERVAL_TIME = "LUEDUO_RECOVER_INTERVAL_TIME";
	public static string LUEDUO_RECOVER_PERCENT = "LUEDUO_RECOVER_PERCENT";
	public static string LUEDUO_RESOURCE_PERCENT = "LUEDUO_RESOURCE_PERCENT";
	public static string LUEDUO_JIANSHE_REDUCE = "LUEDUO_JIANSHE_REDUCE";
	public static string LUEDUO_RESOURCE_MINNUM = "LUEDUO_RESOURCE_MINNUM";
	public static string LUEDUO_HAND_DAYMINNUM = "LUEDUO_HAND_DAYMINNUM";
	public static string LUEDUO_HAND_WEEKMINNUM = "LUEDUO_HAND_WEEKMINNUM";
	public static string LUEDUO_DAYAWARD_GIVETIME = "LUEDUO_DAYAWARD_GIVETIME";
	public static string LUEDUO_WEEKWARD_GIVETIME = "LUEDUO_WEEKWARD_GIVETIME";
	public static string REFRESHTIME = "REFRESHTIME";
	public static string RANK_MAXNUM = "RANK_MAXNUM";
	public static string RANK_MINLEVEL = "RANK_MINLEVEL";
    public static string XILIANSHI_MAXTIMES = "XILIANSHI_MAXTIMES";
	public static string GONGJICHOUHEN_ADD = "GONGJICHOUHEN_ADD";
	public static string GONGJICHOUHEN_TIME = "GONGJICHOUHEN_TIME";


    /// <summary>
    /// 以下参数表里未找到
    /// </summary>
    public static string HOUSE_JINGPAI_PREFIX = "FANGWUJINGPAI_";
	public static string DIJI_TANBAOR_REFRESHTIME = "DIJI_TANBAO_REFRESHTIME";
	public static string JIEBIAO_RESULTBACK_MAXTIME = "JIEBIAO_RESULTBACK_MAXTIME";

    public static Dictionary<string, double> m_TaskInfoDic = new Dictionary<string, double>();

	private static Dictionary<string, string> m_TimeInfoDic = new Dictionary<string, string> ();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        //		if (templates == null) 
        //		{
        //			templates = new List<RenWuTemplate> ();
        //		}
        //		else 
        //		{
        //			templates.Clear ();
        //		}

        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Canshu.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            m_TaskInfoDic.Clear ();
			m_TimeInfoDic.Clear ();
        }

        XmlReader t_reader = null;

        if (obj != null)
        {
            TextAsset t_text_asset = obj as TextAsset;

            t_reader = XmlReader.Create(new StringReader(t_text_asset.text));

            //			Debug.Log( "Text: " + t_text_asset.text );
        }
        else
        {
            t_reader = XmlReader.Create(new StringReader(www.text));
        }

        bool t_has_items = true;

        do
        {
            t_has_items = t_reader.ReadToFollowing("CanShu");

            if (!t_has_items)
            {
                break;
            }

            {
                t_reader.MoveToNextAttribute();
                string t_key = t_reader.Value;

                t_reader.MoveToNextAttribute();
                string t_value = t_reader.Value;

                t_reader.MoveToNextAttribute();
                // desc

                if (t_value.IndexOf(",") == -1 && t_value.IndexOf(":") == -1)
                {
                    try
                    {
                        double t_double_value = double.Parse(t_value);
                        m_TaskInfoDic.Add(t_key, t_double_value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("ex:" + e.Message + ", \n" + e.StackTrace + ", \nkey:" + t_key + ", value:" + t_value);
                    }
                }

				if (!m_TimeInfoDic.ContainsKey (t_key))
				{
					try
					{
						m_TimeInfoDic.Add (t_key,t_value);
					}
					catch (Exception e)
					{
						Debug.LogError("m_timeDic_ex:" + e.Message + ", \n" + e.StackTrace + ", \nkey:" + t_key + ", value:" + t_value);
					}
				}
            }
        }
        while (t_has_items);
    }

    public static double GetValueByKey (string key)
    {
        if (m_TaskInfoDic.ContainsKey(key))
        {
            return m_TaskInfoDic[key];
        }
        else
        {
            Debug.LogError("Can't find key:" + key + " in canshu.xml");
            return -1;
        }
    }

	public static string GetStrValueByKey (string key)
	{
		if (m_TimeInfoDic.ContainsKey (key))
		{
			return m_TimeInfoDic[key];
		}

		else
		{
			Debug.LogError("Can't find key:" + key + " in canshu.xml");
			return null;
		}
	}
}
