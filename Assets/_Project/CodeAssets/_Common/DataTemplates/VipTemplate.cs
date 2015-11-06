using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

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

}
