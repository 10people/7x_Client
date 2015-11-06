using UnityEngine;
using System.Collections;

public class ChatOpenCloseController : MonoBehaviour
{
    public UIRoot m_Root;
    public ChatWindow m_ChatWindow;
    public ScaleEffectController m_ScaleEffectController;

    public GameObject CloseButton;

    [HideInInspector]
    private UIEventListener CloseButtonLis;

    [HideInInspector]
    public bool isOpen
    {
        get { return IsChatWindowOpened; }
        private set
        {
            IsChatWindowOpened = value;

            if (value)
            {
                UIYindao.m_UIYindao.CloseUI();
            }
        }
    }

    /// <summary>
    /// Whether the chat window is opened or not.
    /// </summary>
    public static bool IsChatWindowOpened = false;

    public void OnOpenWindowClick(GameObject go)
    {
        m_Root.gameObject.SetActive(true);

        isOpen = true;
        m_ChatWindow.isEnterToggleByOpeningWindow = true;

        if (Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_CARRIAGE)
        {
            CarriageSceneManager.Instance.m_RootManager.m_CarriageUi.m_ChatRedAlert.SetActive(false);
        }
        else
        {
            MainCityUIL.SetRedAlert("chat", false);
        }

        m_ScaleEffectController.OnOpenWindowClick();
    }

    void OnOpenChatWindowComplete()
    {
        //Disable error alert.
        if (m_ChatWindow.AllianceAlert.activeSelf && AllianceData.Instance.IsAllianceNotExist)
        {
            m_ChatWindow.AllianceAlert.SetActive(false);
        }
        if (m_ChatWindow.PrivateAlert.activeSelf && Application.loadedLevelName != ConstInGame.CONST_SCENE_NAME_HOUSE)
        {
            m_ChatWindow.PrivateAlert.SetActive(false);
        }

        //Decide which channel to turn.
        if (m_ChatWindow.WorldAlert.activeSelf)
        {
            m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject,
                SendMessageOptions.DontRequireReceiver);
        }
        else if (m_ChatWindow.AllianceAlert.activeSelf && !AllianceData.Instance.IsAllianceNotExist)
        {
            m_ChatWindow.TogglesControl.TogglesEvents[1].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[1].gameObject,
                SendMessageOptions.DontRequireReceiver);
        }
        else if (m_ChatWindow.PrivateAlert.activeSelf && Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_HOUSE)
        {
            m_ChatWindow.TogglesControl.TogglesEvents[2].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[2].gameObject,
                SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject.SendMessage(
                "OnClick",
                m_ChatWindow.TogglesControl.TogglesEvents[0].gameObject,
                SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnCloseWindowClick(GameObject go)
    {
        isOpen = false;
        MainCityUI.TryRemoveFromObjectList(gameObject);

        m_ScaleEffectController.OnCloseWindowClick();
    }

    void OnCloseChatWindowComplete()
    {
        m_Root.gameObject.SetActive(false);

        m_ChatWindow.ClearObject();

        MainCityUI.TryRemoveFromObjectList(m_Root.gameObject);
    }

    void OnEnable()
    {
        CloseButtonLis.onClick = OnCloseWindowClick;
    }

    void OnDisable()
    {
        CloseButtonLis.onClick = null;
    }

    void Start()
    {
        m_ScaleEffectController.OpenCompleteDelegate = OnOpenChatWindowComplete;
        m_ScaleEffectController.CloseCompleteDelegate = OnCloseChatWindowComplete;
    }

    void Awake()
    {
        CloseButtonLis = UIEventListener.Get(CloseButton);

        m_Root = UtilityTool.GetComponentInParent<UIRoot>(transform);
    }
}