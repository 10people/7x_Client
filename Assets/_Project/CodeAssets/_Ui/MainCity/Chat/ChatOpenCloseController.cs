using UnityEngine;
using System.Collections;

public class ChatOpenCloseController : MonoBehaviour
{
    public UIRoot m_Root;
    public ChatWindow m_ChatWindow;
    //public ScaleEffectController m_ScaleEffectController;

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

    public void OnOpenWindowClick()
    {
        m_Root.gameObject.SetActive(true);

        isOpen = true;
        m_ChatWindow.isEnterToggleByOpeningWindow = true;

        MainCityUIL.SetRedAlert("chat", false);

        m_ChatWindow.transform.localPosition = new Vector3(-650, 0, 0);
        iTween.MoveTo(m_ChatWindow.gameObject, iTween.Hash("position", Vector3.zero, "time", 0.5f, "easetype", "easeOutBack", "islocal", true, "oncomplete", "OnOpenChatWindowComplete"));
    }

    void OnOpenChatWindowComplete()
    {
        //Disable error alert.
        if (m_ChatWindow.AllianceAlert.activeSelf && AllianceData.Instance.IsAllianceNotExist)
        {
            m_ChatWindow.AllianceAlert.SetActive(false);
        }
        if (m_ChatWindow.PrivateAlert.activeSelf &&
            Application.loadedLevelName != SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.HOUSE))
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
        else if (m_ChatWindow.PrivateAlert.activeSelf
                 && Application.loadedLevelName == SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.HOUSE))
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

    public void OnCloseWindowClick()
    {
        isOpen = false;
        MainCityUI.TryRemoveFromObjectList(gameObject);

        m_ChatWindow.transform.localPosition = Vector3.zero;
        iTween.MoveTo(m_ChatWindow.gameObject, iTween.Hash("position", new Vector3(-650, 0, 0), "time", 0.5f, "easetype", "easeInBack", "islocal", true, "oncomplete", "OnCloseChatWindowComplete"));
    }

    void OnCloseChatWindowComplete()
    {
        m_Root.gameObject.SetActive(false);

        m_ChatWindow.ClearObject();

        MainCityUI.TryRemoveFromObjectList(m_Root.gameObject);
    }

    void Start()
    {
        //m_ScaleEffectController.OpenCompleteDelegate = OnOpenChatWindowComplete;
        //m_ScaleEffectController.CloseCompleteDelegate = OnCloseChatWindowComplete;
    }

    void Awake()
    {
        m_Root = TransformHelper.GetComponentInParent<UIRoot>(transform);
    }
}