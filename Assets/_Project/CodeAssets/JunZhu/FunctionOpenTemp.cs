//#define DEBUG_RED_ALERT

//#define DEBUG_USE_PUSH_DATA



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class FunctionOpenTemp : XmlLoadManager
{
    public int m_iID;
    public string key;
	public int m_iRedType;
    public string Des;
    public int Level;
    public int m_iMissionID;//开启新功能需要的完成的任务ID
    public int m_iDoneMissionID;//完成领奖的任务ID
    public int m_iMissionOpenID;//点击进入功能界面后完成的任务
    public int m_iNpcID;
    public int sceneType;

    public int type;
	public int m_iPlay;//是否播放添加功能动画
    public int rank;
	public string m_sNotOpenTips;
	public List<int> m_listNextID = new List<int>();


    /// parent menu id.
    public int m_parent_menu_id;

    /// use new red spot data or not
    public bool m_use_red_push_data;

	/// destroy ui if true
	public bool m_destroy_ui;

    /// custom data, show flag for red spot notification
    public bool m_show_red_alert;

    public static List<FunctionOpenTemp> templates = new List<FunctionOpenTemp>();

    public static List<int> m_EnableFuncIDList = new List<int>();

    public const int GoHomeID = 7;
    public static void LoadTemplates(EventDelegate.Callback p_callback = null)
    {
        UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "FunctionOpen.xml"), CurLoad, UtilityTool.GetEventDelegateList(p_callback), false);
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
            t_has_items = t_reader.ReadToFollowing("FunctionOpen");

            if (!t_has_items)
            {
                break;
            }

            FunctionOpenTemp t_template = new FunctionOpenTemp();

            {
                t_reader.MoveToNextAttribute();
                t_template.m_iID = int.Parse(t_reader.Value);

                t_template.m_parent_menu_id = ReadNextInt(t_reader);

                t_template.m_use_red_push_data = ReadNextBool(t_reader);

				t_template.m_destroy_ui = ReadNextBool(t_reader);

				t_template.m_iRedType = ReadNextInt(t_reader);

                t_reader.MoveToNextAttribute();
                t_template.key = t_reader.Value;

				t_template.m_iPlay = ReadNextInt(t_reader);

                t_reader.MoveToNextAttribute();
                t_template.type = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Des = t_reader.Value;

                t_reader.MoveToNextAttribute();
                t_template.rank = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_iNpcID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.sceneType = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.Level = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_iMissionID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_iDoneMissionID = int.Parse(t_reader.Value);

                t_reader.MoveToNextAttribute();
                t_template.m_iMissionOpenID = int.Parse(t_reader.Value);

				t_reader.MoveToNextAttribute();

				t_template.m_sNotOpenTips = ReadNextString(t_reader);

                // init custom data
                {
                    t_template.m_show_red_alert = false;
                }

                //				#if DEBUG_USE_PUSH_DATA
                //				Debug.Log( "Function open: " + t_template.m_iID + ", " + 
                //				          t_template.m_parent_menu_id + ", " + 
                //				          t_template.m_use_red_push_data );
                //				#endif
            }
            //			t_template.Log();

            templates.Add(t_template);
        }
        while (t_has_items);

		for(int i = 0; i < templates.Count; i ++)
		{
			if(templates[i].m_parent_menu_id != -1)
			{
				GetTemplateById(templates[i].m_parent_menu_id).m_listNextID.Add(templates[i].m_iID);
			}
		}
    }

    public static string GetDesByLevel(int level)
    {
        List<FunctionOpenTemp> listTemp = new List<FunctionOpenTemp>();
        string des = "";
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].Level == level)
            {
                listTemp.Add(templates[i]);
            }
        }

        for (int i = 0; i < listTemp.Count; i++)
        {
            if (i < listTemp.Count - 1)
            {
                des += "解锁" + listTemp[i].Des + "功能" + "、";
            }
            else
            {
                des += "解锁" + listTemp[i].Des + "功能";
            }
        }
        return des;
    }

    /// <summary>
    /// 升级时获得新添加的功能按钮,add directly for resname == -1
    /// </summary>
    /// <returns>The add open function.</returns>
    public static void GetLVAddOpenFunction(int LV)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            //If lv <0, then not controlled by lv
            if (templates[i].Level < 0)
            {
                continue;
            }

            //cannot add gohome btn
            if (templates[i].m_iID == GoHomeID)
            {
                continue;
            }

            if (JunZhuData.Instance().m_CurrentLevel >= templates[i].Level)
            {
                if (!IsHaveID(templates[i].m_iID))
                {
//                    if (templates[i].type > 0)
                    {
                        Global.m_iOpenFunctionIndex = templates[i].m_iID;
                    }
                    if (!m_EnableFuncIDList.Contains(templates[i].m_iID))
                    {
						FunctionOpenTemp.isAddFunction();
                        m_EnableFuncIDList.Add(templates[i].m_iID);

                        //Refresh comming soon button.
//                        MainCityUIRB.RefreshCommingSoonButton();
                    }
                    return;
                }
            }
        }
    }

    public static bool isAddFunction()
    {
		if(FunctionOpenTemp.GetTemplateById(Global.m_iOpenFunctionIndex).m_iPlay == 1)
		{
			ClientMain.addPopUP(40, 2, "" + Global.m_iOpenFunctionIndex, null);
			Global.m_iOpenFunctionIndex = -1;
		}
//		if(FunctionUnlock.getGroudById(Global.m_iOpenFunctionIndex) != null)
//		{
//
//		}
//        if (MainCityUIRB.ButtonSpriteNameTransferDic.ContainsKey(Global.m_iOpenFunctionIndex))
//        {
//            ClientMain.addPopUP(50, 2, "" + Global.m_iOpenFunctionIndex, null);
//        }
        return true;
    }

    /// <summary>
    /// called when task added
    /// </summary>
    /// <param name="missionID">Mission I.</param>
    public static void GetMissionAddOpenFunction(int missionID)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            //If m_iMissionID <0, then not controlled by m_iMissionID
            if (templates[i].m_iMissionID < 0)
            {
                continue;
            }

            //cannot add gohome btn
            if (templates[i].m_iID == GoHomeID)
            {
                continue;
            }

            if (templates[i].m_iMissionID == missionID && JunZhuData.Instance().m_CurrentLevel >= templates[i].Level)
            {
                if (!IsHaveID(templates[i].m_iID))
                {
//                    if (templates[i].type > 0)
                    {
                        Global.m_iOpenFunctionIndex = templates[i].m_iID;
                    }
                    if (!m_EnableFuncIDList.Contains(templates[i].m_iID))
                    {
						FunctionOpenTemp.isAddFunction();
                        m_EnableFuncIDList.Add(templates[i].m_iID);

                        //Refresh comming soon button.
//                        MainCityUIRB.RefreshCommingSoonButton();
                    }
                    return;
                }
            }
        }
    }

    /// <summary>
    /// called when task finished
    /// </summary>
    /// <param name="missionID">Mission I.</param>
    public static void GetMissionDoneOpenFunction(int missionID)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            //If m_iDoneMissionID <0, then not controlled by m_iDoneMissionID
            if (templates[i].m_iDoneMissionID < 0)
            {
                continue;
            }

            //cannot add gohome btn
            if (templates[i].m_iID == GoHomeID)
            {
                continue;
            }

            if (templates[i].m_iDoneMissionID == missionID && JunZhuData.Instance().m_CurrentLevel >= templates[i].Level)
            {
                if (!IsHaveID(templates[i].m_iID))
                {
//                    if (templates[i].type > 0)
                    {
                        Global.m_iOpenFunctionIndex = templates[i].m_iID;
                    }
                    if (!m_EnableFuncIDList.Contains(templates[i].m_iID))
                    {
						FunctionOpenTemp.isAddFunction();
                        m_EnableFuncIDList.Add(templates[i].m_iID);

                        //Refresh comming soon button.
//                        MainCityUIRB.RefreshCommingSoonButton();
                    }
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Check and add go home function.
    /// 判断要开启的ID是否已经开启
    /// </summary>
    /// <returns><c>true</c>,此ID已经开启, <c>false</c> 此ID没有开启.</returns>
    /// <param name="id">开启功能的ID</param>
    public static bool IsHaveID(int id)
    {
        for (int i = 0; i < m_EnableFuncIDList.Count; i++)
        {
            if (m_EnableFuncIDList[i] == id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获得要开启ID 的数据索引
    /// </summary>
    /// <returns>The function open id.</returns>
    /// <param name="id">要开启的ID</param>
    public static int GetFunctionOpenIndexByID(int id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iID == id)
            {
                return i;
            }
        }
        return -1;
    }

    public static int GetNpcIdByID(int id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iID.Equals(id))
            {
                return templates[i].m_iNpcID;
            }
        }
        return -1;
    }

    public static int GetTypeByNpcId(int id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iNpcID == id)
            {
                return templates[i].type;
            }
        }
        return -1;
    }

    public static FunctionOpenTemp GetTemplateById(int id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iID == id)
            {
                return templates[i];
            }
        }
        return null;
    }

	public static int GetParent(int id)
	{
		return GetParent(GetTemplateById(id));
	}

	public static int GetParent(FunctionOpenTemp temp)
	{
		if(temp.m_parent_menu_id == -1)
		{
			return -1;
		}
		else
		{
			while(temp.m_parent_menu_id != -1)
			{
				temp = GetTemplateById(temp.m_parent_menu_id);
			}
			return temp.m_iID;
		}
	}

    public static bool GetWhetherContainID(int id)
    {
        int size = m_EnableFuncIDList.Count;
        for (int i = 0; i < size; i++)
        {
            if (id == m_EnableFuncIDList[i])
            {
                return true;
            }

        }
        return false;
    }

    public static int GetMissionIdById(int id)
    {
        int size = templates.Count;
        for (int i = 0; i < size; i++)
        {
            if (id == templates[i].m_iID)
            {
                return templates[i].m_iDoneMissionID;
            }
        }
        return 0;
    }

    #region Red Spot Data

    /// Check if function_open_id is using 
    public static bool IsRedSpotDataOpen(int p_function_open_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iID == p_function_open_id)
            {
                return templates[i].m_use_red_push_data;
            }
        }

        Debug.LogError("Function id not eixst: " + p_function_open_id);

        return false;
    }

    /// check if red spot notification should be showed.
    ///
    /// Param:
    /// 1.p_funciton_open_id: target function open id.
    public static bool IsShowRedSpotNotification(int p_function_open_id)
    {
        for (int i = 0; i < templates.Count; i++)
        {
            if (templates[i].m_iID == p_function_open_id)
            {
                return templates[i].m_show_red_alert;
            }
        }

        Debug.LogError("IsShowRedSpotNotification.target function open id not exist: " + p_function_open_id);

        return false;
    }

    public static void SetRedSpotNotification(int p_function_open_id, bool p_is_show)
    {
#if DEBUG_RED_ALERT
		Debug.Log( "SetRedSpotNotification( " + p_function_open_id + ", " + p_is_show + " )" );
#endif

        FunctionOpenTemp t_template = GetTemplateById(p_function_open_id);

        if (t_template == null)
        {
            Debug.LogError("SetRedSpotNotification.target function open id not exist: " + p_function_open_id);

            return;
        }

        if (!t_template.m_use_red_push_data)
        {
            Debug.LogError("function should use old red spot workflow: " + p_function_open_id);

            return;
        }

        if (GetRedSpotChildCount(p_function_open_id) > 0)
        {
            Debug.LogError("Should not modify parent red spot status by hand: " + p_function_open_id);
        }
        else
        {
            t_template.m_show_red_alert = p_is_show;
        }
    }

    /// Manual update red spot data struct, make the previous change take effect for parents.
    /// 
    /// Note:
    /// 1.currently support 4 levels sub red spot.
    public static void UpdateRedSpotDataHierarchy()
    {
        UpdateIfSubRedSpotSetTrue();
        for (int i = 0; i < 4; i++)
        {
            UpdateIfSubRedSpotSetFalse();
        }
    }

    /// Set all parents's red spot data show to true.
    private static void UpdateIfSubRedSpotSetTrue()
    {
        for (int i = 0; i < templates.Count; i++)
        {
            int t_parent_id = templates[i].m_parent_menu_id;

            // skip old red spot
            if (!templates[i].m_use_red_push_data)
            {
                continue;
            }

            // skip new red spot not showing
            if (!templates[i].m_show_red_alert)
            {
                continue;
            }

            while (true)
            {
                // root menu
                if (t_parent_id == -1)
                {
                    break;
                }

                FunctionOpenTemp t_template = GetTemplateById(t_parent_id);

                if (t_template == null)
                {
                    Debug.LogError("Error, parent not exist: " + t_parent_id);

                    break;
                }

                t_template.m_show_red_alert = true;

                t_parent_id = t_template.m_parent_menu_id;
            }
        }
    }

    /// Must be invoked more than sub red spot levels.
    private static void UpdateIfSubRedSpotSetFalse()
    {
        for (int i = 0; i < templates.Count; i++)
        {
            FunctionOpenTemp t_target_template = templates[i];

            if (!t_target_template.m_use_red_push_data)
            {
                continue;
            }

            int t_child_count = 0;

            bool t_child_show = false;

            for (int j = 0; j < templates.Count; j++)
            {
                FunctionOpenTemp t_cur_template = templates[j];

                if (t_cur_template.m_parent_menu_id == t_target_template.m_iID)
                {
                    t_child_count++;

                    if (!t_cur_template.m_use_red_push_data)
                    {
//                        Debug.LogError("Child Red Spot not using push data: " + t_cur_template.m_iID);
                    }

                    if (t_cur_template.m_show_red_alert)
                    {
                        t_child_show = true;
                    }
                }
            }

            if (t_child_count == 0)
            {
                // nothing changes
            }
            else if (t_child_count > 0)
            {
                if (t_child_show)
                {
                    t_target_template.m_show_red_alert = true;
                }
                else
                {
                    t_target_template.m_show_red_alert = false;
                }
            }
        }
    }

    public static void ClearAllRedSpotNotification()
    {
        for (int i = 0; i < templates.Count; i++)
        {
            FunctionOpenTemp t_template = templates[i];

            t_template.m_show_red_alert = false;
        }
    }

    public static int GetRedSpotChildCount(int p_function_open_id)
    {
        FunctionOpenTemp t_template = GetTemplateById(p_function_open_id);

        if (t_template == null)
        {
            Debug.LogError("template not exist: " + p_function_open_id);

            return 0;
        }

        int t_child_count = 0;

        for (int i = 0; i < templates.Count; i++)
        {
            FunctionOpenTemp t_cur_template = templates[i];

            if (t_cur_template.m_parent_menu_id == p_function_open_id)
            {
                t_child_count++;

                if (!t_cur_template.m_use_red_push_data)
                {
                    Debug.LogError("Child Red Spot not using push data: " + t_cur_template.m_iID);
                }
            }
        }

        return t_child_count;
    }

    public static void LogUseRedSpotData()
    {
        Debug.Log("----------------------LogUseRedSpotData---------------------");

        for (int i = 0; i < templates.Count; i++)
        {
            FunctionOpenTemp t_target_template = templates[i];

            if (!t_target_template.m_use_red_push_data)
            {
                continue;
            }

            Debug.Log(t_target_template.m_iID + ", " + t_target_template.m_show_red_alert);
        }
    }

    #endregion



	#region Destroy UI

	/// Check if ui should be destroyed after close
	public static bool IsDestroyUI( int p_function_open_id ){
		for( int i = 0; i < templates.Count; i++ ){
			if( templates[i].m_iID == p_function_open_id ){
				return templates[i].m_destroy_ui;
			}
		}
		
		Debug.LogError( "Function id not eixst: " + p_function_open_id );
		
		return false;
	}

	#endregion
}
