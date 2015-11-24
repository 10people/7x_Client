using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

public class MainCityUIL : MonoBehaviour
{
    public EventIndexHandle TaskHandle;
    public EventIndexHandle EmailHandle;
    public EventIndexHandle ChatHandle;

    public GameObject TaskRedAlertObject;
    public GameObject EmailRedAlertObject;
    public GameObject ChatRedAlertObject;

    public UISprite TaskFinishSprite;
    public UILabel EmailDetailLabel;

    public GameObject TaskDescObject;
    public GameObject EmailDescObject;
    public MainCityTaskManager m_MainCityTaskManager;

    #region Task Detail/Finished Control

    public void ShowTaskDetail(bool isShow = true, string text = "")
    {
        if (isShow)
        {
            TaskFinishSprite.gameObject.SetActive(false);

            TaskDescObject.SetActive(true);
        }
        else
        {
            TaskDescObject.SetActive(false);
        }
    }

    public void ShowTaskFinished(bool isShow = true)
    {
        if (isShow)
        {
            TaskFinishSprite.gameObject.SetActive(true);

            TaskDescObject.SetActive(true);
        }
        else
        {
            TaskDescObject.SetActive(false);
        }
    }

    #endregion

    #region Email Detail Control

    private class StoredEmailDetail
    {
        public bool IsShow;
        public string text;
    }

    private static List<StoredEmailDetail> storedEmailDetailList = new List<StoredEmailDetail>();

    private static void ExecuteStoredMailDetail()
    {
        m_MainCityUILDelegate -= ExecuteStoredMailDetail;
        storedEmailDetailList.ForEach(item => ShowEmailDetail(item.IsShow, item.text));

        storedEmailDetailList.Clear();
    }

    public static void ShowEmailDetail(bool isShow, string text)
    {
        if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIL == null)
        {
            storedEmailDetailList.Add(new StoredEmailDetail
            {
                IsShow = isShow,
                text = text
            });
            m_MainCityUILDelegate += ExecuteStoredMailDetail;

            return;
        }

        MainCityUI.m_MainCityUI.m_MainCityUIL.DoShowEmailDetail(isShow, text);
    }

    private void DoShowEmailDetail(bool isShow, string text)
    {
        if (isShow)
        {
            EmailDetailLabel.text = text;
            EmailDescObject.SetActive(true);
            m_emailDetailLastShowTime = Time.realtimeSinceStartup;
        }
        else
        {
            EmailDescObject.SetActive(false);
        }
    }

    private float m_emailDetailLastShowTime;
    private const float m_EmailDetailShowDuration = 3.0f;

    #endregion

    void Update()
    {
        if (EmailDescObject.activeInHierarchy && (Time.realtimeSinceStartup - m_emailDetailLastShowTime > m_EmailDetailShowDuration))
        {
            ShowEmailDetail(false, "");
        }
    }

    public void OnIndexHandleClick(int i)
    {
        if (MainCityUI.IsWindowsExist())
        {
            return;
        }

        switch (i)
        {
            case 1:
                {
                    break;
                }
            case 2:
                {
                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.EMAIL),
                        EmailLoadCallback);
                    break;
                }
            case 3:
                {
                    OnChatClick();
                    break;
                }
            default:
                {
                    Debug.LogError("Error index clicked, index:" + i);
                    break;
                }
        }
    }

    public void EmailLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }
    }

    void Awake()
    {
        TaskHandle.m_Handle += OnIndexHandleClick;
        EmailHandle.m_Handle += OnIndexHandleClick;
        ChatHandle.m_Handle += OnIndexHandleClick;
    }

    void Start()
    {
        if (chatUI == null)
        {
            if (ChatWindow.s_ChatWindow != null && ChatWindow.s_ChatWindow.gameObject != null)
            {
                chatUI = ChatWindow.s_ChatWindow.ChatRoot.gameObject;
            }
            else
            {
                Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_CHAT_WINDOW),
                    ChatWindowLoadCallBack);
            }
        }

        if (m_MainCityUILDelegate != null)
        {
            m_MainCityUILDelegate();
        }
    }

    void OnDestroy()
    {
        TaskHandle.m_Handle -= OnIndexHandleClick;
        EmailHandle.m_Handle -= OnIndexHandleClick;
        ChatHandle.m_Handle -= OnIndexHandleClick;
    }

    public static bool IsDoDelegateAfterInit = false;
    public delegate void MainCityUILDelegate();
    public static MainCityUILDelegate m_MainCityUILDelegate;

    #region RedAlert Control

    public class RedAlertCommand
    {
        public string str;
        public bool IsShow;
    }
    public static List<RedAlertCommand> m_RedAlertCommandList = new List<RedAlertCommand>();

    private static void ExecuteStoredRedAlertCommand()
    {
        IsDoDelegateAfterInit = false;
        m_MainCityUILDelegate -= ExecuteStoredRedAlertCommand;

        foreach (var item in m_RedAlertCommandList)
        {
            SetRedAlert(item.str, item.IsShow);
        }
        m_RedAlertCommandList.Clear();
    }

    /// <summary>
    /// Set main city ui RB button's red alert active or deactive
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isShow">active or deactive, true: active, false: deactive</param>
    /// <returns>set successfully or not, true: succeed, false: fail</returns>
    public static bool SetRedAlert(string str, bool isShow)
    {
        try
        {
            //Add to command list if UI not inited, then return true.
            if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIL == null)
            {
                m_RedAlertCommandList.Add(new RedAlertCommand { IsShow = isShow, str = str });
                IsDoDelegateAfterInit = true;
                m_MainCityUILDelegate += ExecuteStoredRedAlertCommand;
                return true;
            }

            MainCityUIL temp = MainCityUI.m_MainCityUI.m_MainCityUIL;

            switch (str)
            {
                case "task":
                    {
                        temp.TaskRedAlertObject.gameObject.SetActive(isShow);
                        break;
                    }
                case "email":
                    {
                        temp.EmailRedAlertObject.gameObject.SetActive(isShow);
                        break;
                    }
                case "chat":
                    {
                        temp.ChatRedAlertObject.gameObject.SetActive(isShow);
                        break;
                    }
                default:
                    {
                        Debug.LogError("Error string in  red alert, string:" + str);
                        break;
                    }
            }


        }
        catch (Exception ex)
        {
            Debug.LogError("Got exception in setting alert, ex:" + ex.Message + ", \nstackTrace:" + ex.StackTrace);
            return false;
        }

        return false;
    }

    #endregion

    #region [Obsolete]Email Logo Control


    private static List<bool> m_SetMailBtnCommandList = new List<bool>();

    /// <summary>
    /// Set main button in left top UI active or deactive 
    /// </summary>
    /// <param name="isActive">true: active, false: deactive</param>
    /// <returns>true: set succeed, false: fail to set</returns>
    [Obsolete("Do not set anymore")]
    public static bool SetMailButton(bool isActive)
    {
        if (MainCityUI.m_MainCityUI == null || MainCityUI.m_MainCityUI.m_MainCityUIL == null)
        {
            m_SetMailBtnCommandList.Add(isActive);
            m_MainCityUILDelegate += SetMailButtonCommand;
            return true;
        }

        MainCityUI.m_MainCityUI.m_MainCityUIL.EmailHandle.gameObject.SetActive(isActive);
        return true;
    }

    private static void SetMailButtonCommand()
    {
        m_MainCityUILDelegate -= SetMailButtonCommand;

        foreach (var item in m_SetMailBtnCommandList)
        {
            SetMailButton(item);
        }

        m_SetMailBtnCommandList.Clear();
    }

    #endregion

    #region Chat Click Control

    private GameObject chatUI;
    /// <summary>
    /// Can only be called when chatUI is not null and actived.
    /// </summary>
    private ChatWindow chatWindow
    {
        get
        {
            if (chatWindow_Value != null)
            {
                return chatWindow_Value;
            }
            else
            {
                return chatWindow_Value = chatUI.GetComponentInChildren<ChatWindow>();
            }
        }
    }
    private ChatWindow chatWindow_Value;

    public void OnChatClick()
    {
        ChatWindow.s_ChatWindow.m_ChatChannelFrameList.ForEach(item => item.m_ChatBaseDataHandler.ClearUnUsedChatStructList());

        chatUI.SetActive(true);
        MainCityUI.TryAddToObjectList(chatUI);

        chatWindow.m_RootChatOpenObject = ChatHandle.gameObject;
        chatWindow.m_ChatOpenCloseController.OnOpenWindowClick(gameObject);
    }

    private void ChatWindowLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        chatUI = Instantiate(p_object) as GameObject;
        DontDestroyOnLoad(chatUI);
        chatWindow.m_RootChatOpenObject = ChatHandle.gameObject;

        chatUI.GetComponentInChildren<ChatWindow>().m_ChatChannelFrameList.ForEach(item =>
        {
            var temp = item.m_ChatBaseDataHandler.GetComponent<ChatDataHandler>();
            if (temp.m_Channel == ChatPct.Channel.SHIJIE || (temp.m_Channel == ChatPct.Channel.LIANMENG 
					&& !AllianceData.Instance.IsAllianceNotExist) || (temp.m_Channel == ChatPct.Channel.XiaoWu 
			      	&& Application.loadedLevelName == SceneTemplate.GetScenePath( SceneTemplate.SceneEnum.HOUSE ) ))
            {
                item.m_ChatBaseDataHandler.GetComponent<ChatDataHandler>().Awake();
            }
        });

        chatUI.SetActive(false);
    }

    private void ChatWindowOpenCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        chatUI = Instantiate(p_object) as GameObject;
        DontDestroyOnLoad(chatUI);
        MainCityUI.TryAddToObjectList(chatUI);

        chatWindow.m_RootChatOpenObject = ChatHandle.gameObject;
        chatWindow.m_ChatOpenCloseController.OnOpenWindowClick(gameObject);
    }

    public void changeTaskData(string data)
    {

    }
    #endregion
}
