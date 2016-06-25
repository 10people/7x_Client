using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using qxmobile;
using qxmobile.protobuf;
public class VipTemplate : XmlLoadManager
{
    public int lv;
    public int needNum;
    public int bugMoneyTime;
    public int bugTiliTime;
    public int bugBaizhanTime;

    public int saodangFree;
    public int xilianLimit;
    public int yujueDuihuan;
    public int legendPveRefresh;
    public int YBxilianLimit;

    public int dangpuRefreshLimit;
    public int desc;
    public float baizhanPara;
    public int houseFitmentNum;
	public int MiBaoLimit;

	public int YouxiaTimes;
	public int YunbiaoTimes;
	public int JiebiaoTimes;
	public int InviteAssistTimes;
	public int LveduoTimes;

	public int HuangyeTimes;
	public int CarriageBlood;
	public int CarriageRebirth;
	public int CartPinZhiMax;

    public int resOnSiteTimes;
    public int ABRebirth;
    public int ABBlood;
    public int buyEquipment;
    public int buyBaoshi;
    public int buyQianghua;
    public int buyJingqi;
    public int buyJianshezhi;
    public int buyHufu;
	public int ExpAdd;
    public static List<VipTemplate> templates = new List<VipTemplate>();


    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "VIP.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            templates.Clear();
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
            t_has_items = t_reader.ReadToFollowing("VIP");

            if (!t_has_items)
            {
                break;
            }

            VipTemplate t_template = new VipTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.lv = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.needNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.bugMoneyTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.bugTiliTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.bugBaizhanTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.saodangFree = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.xilianLimit = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.yujueDuihuan = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.legendPveRefresh = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.YBxilianLimit = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.dangpuRefreshLimit = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.desc = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.baizhanPara = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.houseFitmentNum = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.MiBaoLimit = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.YouxiaTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.YunbiaoTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.JiebiaoTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.InviteAssistTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.LveduoTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.HuangyeTimes = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.CarriageBlood = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.CarriageRebirth = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.CartPinZhiMax = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.resOnSiteTimes = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.ABRebirth = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.ABBlood = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyEquipment = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyBaoshi = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyQianghua = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyJingqi = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyJianshezhi = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.buyHufu = int.Parse(t_reader.Value);
				t_reader.MoveToNextAttribute();
				t_template.ExpAdd = int.Parse(t_reader.Value);
}

            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);
    }

    public static int GetVipWorshipInfoByLevel(int level)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].lv == level)
            {
                return templates[i].yujueDuihuan;
            }
        }
        return 0;

    }

    public static int GetVipABRebirthInfoByLevel(int level)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].lv == level)
            {
                return templates[i].ABRebirth;
            }
        }
        return 0;

    }

    public static int GetLevelOfABQuickRebirthStart()
    {
        var list = templates.Where(item => item.ABRebirth > 0).ToList();

        if (list.Any())
        {
            return list.First().lv;
        }
        else
        {
            return -1;
        }
    }

    public static VipTemplate GetVipInfoByLevel(int level)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].lv == level)
            {
                return templates[i];
            }
        }
        return null;

    }

	public static int GetNextBaiZhanBuyTimeVipLevel (int curVipLevel)
	{
		for (int i = 0;i < templates.Count;i ++)
		{
			if (templates[i].bugBaizhanTime > VipTemplate.GetVipInfoByLevel (curVipLevel).bugBaizhanTime)
			{
				return templates[i].lv;
			}
		}

		Debug.LogError ("Can not GetNextBaiZhanBuyTimeVipLevel :" + curVipLevel);

		return -1;
	}

	public static int GetNextLueDuoBuyTimeVipLevel (int curVipLevel)
	{
		for (int i = 0;i < templates.Count;i ++)
		{
			if (templates[i].LveduoTimes > VipTemplate.GetVipInfoByLevel (curVipLevel).LveduoTimes)
			{
				return templates[i].lv;
			}
		}
		
		Debug.LogError ("Can not GetNextBaiZhanBuyTimeVipLevel :" + curVipLevel);
		
		return -1;
	}
}
