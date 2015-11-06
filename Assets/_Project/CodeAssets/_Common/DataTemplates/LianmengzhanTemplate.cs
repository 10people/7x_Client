using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

public class LianmengzhanTemplate : XmlLoadManager
{
    public string StartTime;
    public string Deadline;
    public float Jianshezhi;
    public List<TimeHelper.ClockTime> AnnounceTime;
    public string Announcement;
    public int MemNumMin;
    public int LianmengLvMin;
    public int LianmengNumMin;
    public int LianmengNumMax;
    public TimeHelper.ClockTime MatchStart;
    public TimeHelper.ClockTime MatchEnd;
    public TimeHelper.ClockTime PublishTime;
    public TimeHelper.ClockTime PreStartTime;
    public List<TimeHelper.ClockTime> PreMathTipsTime;
    public string PreMathTips;
    public TimeHelper.ClockTime WarStart;
    public TimeHelper.ClockTime WarStop;
    public string JiesuanTime;
    public int PreCount;
    public int Countdown;
    public int ReviveTime;
    public int FightingDuration;
    public int ScreenAngle;
    public int ScreeningDistance;
    public int CampNum;
    public float ZhanlingzhiMax;
    public float CriticalValue;
    public float ZhanlingzhiAdd;
    public float ScoreAdd1;
    public float ScoreAdd2;
    public float ScoreAdd3;
    public float ScoreAdd4;
    public float ScoreAdd5;
    public float ScoreMax;
    public Vector2 BirthPoint1;
    public Vector2 BirthPoint2;
    public Vector2 Camp1;
    public Vector2 Camp2;
    public Vector2 Camp3;
    public Vector2 Camp4;
    public Vector2 Camp5;

    public static List<LianmengzhanTemplate> Templates = new List<LianmengzhanTemplate>();

    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Lianmengzhan.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            Templates.Clear();
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
            t_has_items = t_reader.ReadToFollowing("Lianmengzhan");

            if (!t_has_items)
            {
                break;
            }

            LianmengzhanTemplate t_template = new LianmengzhanTemplate();

            {
                t_reader.MoveToNextAttribute();
                t_template.StartTime = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Deadline = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.Jianshezhi = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.AnnounceTime = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(item => TimeHelper.ClockTime.Parse(item)).ToList();

                t_reader.MoveToNextAttribute();
                t_template.Announcement = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.MemNumMin = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.LianmengLvMin = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.LianmengNumMin = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.LianmengNumMax = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.MatchStart = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.MatchEnd = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.PublishTime = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.PreStartTime = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.PreMathTipsTime = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(item => TimeHelper.ClockTime.Parse(item)).ToList();

                t_reader.MoveToNextAttribute();
                t_template.PreMathTips = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.WarStart = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.WarStop = TimeHelper.ClockTime.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.JiesuanTime = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.PreCount = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Countdown = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ReviveTime = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.FightingDuration = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScreenAngle = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScreeningDistance = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.CampNum = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ZhanlingzhiMax = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.CriticalValue = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ZhanlingzhiAdd = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd1 = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd2 = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd3 = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd4 = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreAdd5 = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.ScoreMax = float.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                var temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.BirthPoint1 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.BirthPoint2 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.Camp1 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.Camp2 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.Camp3 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.Camp4 = new Vector2(temp[0], temp[1]);

                t_reader.MoveToNextAttribute();
                temp = t_reader.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                t_template.Camp5 = new Vector2(temp[0], temp[1]);
            }

            Templates.Add(t_template);
        }
        while (t_has_items);
    }
}
