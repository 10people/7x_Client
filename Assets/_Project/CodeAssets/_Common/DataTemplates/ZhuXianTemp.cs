using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class ZhuXianTemp : XmlLoadManager
{
    public int id;

    public int type;

    public int rank;
    public int orderidx;

    public string nextGroup;

    public string title;

    public string brief;

    public string desc;

    public int finishType;
    public int triggerType;

    public int triggerCond;

    public int doneType;

    public string doneCond;

    public int NpcId;

    public string yindaoId;

    public string award;

    public string awardDesc;

    public string icon;

    /// <summary>
    /// major or equal to 0: not finished, minor to 0: finished
    /// </summary>
    public int progress;

    public int m_iCurIndex = 0;

    public int LinkNpcId = 0;

    public int FunctionId = 0;

	public string m_sSprite;

    public string doneTitle;

    public List<int> m_listYindaoShuju = new List<int>();

    public static List<int> taskShowIdList = new List<int>();

    public static List<ZhuXianTemp> tempTasks = new List<ZhuXianTemp>();



    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "ZhuXian.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
    }

    public static void CurLoad(ref WWW www, string path, Object obj)
    {
        {
            tempTasks.Clear();
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
            t_has_items = t_reader.ReadToFollowing("ZhuXian");

            if (!t_has_items)
            {
                break;
            }

            ZhuXianTemp t_template = new ZhuXianTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.id = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.rank = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.orderidx = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.nextGroup = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.title = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.brief = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.desc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.finishType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.triggerType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.triggerCond = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.doneType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.doneCond = t_reader.Value;


                t_reader.MoveToNextAttribute();
                t_template.NpcId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.yindaoId = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.award = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.awardDesc = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.icon = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.LinkNpcId = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.FunctionId = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();
				t_template.m_sSprite = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.doneTitle = t_reader.Value;
                
                //			Debug.Log(_template.yindaoId);

                string[] temp = t_template.yindaoId.Split(';');
                for (int i = 0; i < temp.Length; i++)
                {
                    while (temp[i].IndexOf("+") != -1)
                    {
                        string tempInt = temp[i].Substring(0, temp[i].IndexOf("+"));
                        temp[i] = temp[i].Substring(temp[i].IndexOf("+") + 1, temp[i].Length - temp[i].IndexOf("+") - 1);
                        //                        if (tempInt != "0")
                        //                        {
                        t_template.m_listYindaoShuju.Add(int.Parse(tempInt));
                        //                        }

                    }
                    //                    if (temp[i] != "0")
                    //                    {
                    t_template.m_listYindaoShuju.Add(int.Parse(temp[i]));
                    //                    }
                }
                tempTasks.Add(t_template);
                taskShowIdList.Add(t_template.id);
            }
        }
        while (t_has_items);
    }

    public static ZhuXianTemp getTemplateById(int id)
    {
        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.id == id)
            {
                return template;
            }
        }

        Debug.LogWarning("XML ERROR: Can't get HeroStarTemplate with id " + id);

        return null;
    }

    public static int GetTypeById(int tempId)
    {

        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.id == tempId)
            {
                return template.doneType;
            }
        }
        return tempId;
    }

    public static string GetTaskIconById(int tempId)
    {

        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.id == tempId)
            {
                return template.icon;
            }
        }
        return "";
    }

    public static int GetNpcIdById(int tempId)
    {

        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.id == tempId)
            {
                return template.NpcId;
            }
        }
        return tempId;
    }
    public static string GeTaskTitleById(int tempId)
    {
        //        Debug.Log("tempIdtempIdtempId  ::::::" + tempId);

        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.id == tempId)
            {
                return template.title;
            }
        }
        return "";
    }

    public static string GeTaskTitleByIndexid(int tempId)
    {
//        Debug.Log("tempIdtempIdtempId  ::::::" + tempId);
        foreach (ZhuXianTemp template in tempTasks)
        {
            if (template.orderidx == tempId)
            {
                return template.title;
            }
        }
        return "";
    }

}
