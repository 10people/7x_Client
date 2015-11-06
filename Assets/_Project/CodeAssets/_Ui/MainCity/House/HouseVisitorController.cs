using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class HouseVisitorController : MonoBehaviour
{
    [HideInInspector]
    public VisitorInfo m_PlayerInfo;
    [HideInInspector]
    public SmallHouseSelfOperation m_SmallHouseSelfOperation;
    [HideInInspector]
    public BigHouseSelfOperation m_BigHouseSelfOperation;

    public UILabel VisitorLabel;
    public UISprite VisitorSprite;
    private UIEventListener m_eventListener;

    public void SetController()
    {
        VisitorLabel.text = string.IsNullOrEmpty(m_PlayerInfo.jzName) ? VisitorLabel.text : m_PlayerInfo.jzName;
        //VisitorSprite.spriteName = string.IsNullOrEmpty(m_PlayerInfo.PlayerIcon) ? VisitorSprite.spriteName : m_PlayerInfo.PlayerIcon;
    }

    /// <summary>
    /// on visitor click
    /// </summary>
    /// <param name="go"></param>
    private void OnThisClick(GameObject go)
    {
        if (m_SmallHouseSelfOperation != null)
        {
            m_SmallHouseSelfOperation.SelectedVisitorController = this;
            m_SmallHouseSelfOperation.ShowPopWindow();

            return;
        }

        if (m_BigHouseSelfOperation != null)
        {
            m_BigHouseSelfOperation.SelectedVisitorController = this;
            m_BigHouseSelfOperation.ShowPopWindow();

            return;
        }
    }

    void OnEnable()
    {
        m_eventListener.onClick = OnThisClick;
    }

    void OnDisable()
    {
        m_eventListener.onClick = null;
    }

    void Awake()
    {
        m_eventListener = UIEventListener.Get(gameObject);
    }
}
