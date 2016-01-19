using UnityEngine;
using System.Collections;

/// <summary>
/// Base class in chat sys.
/// </summary>
public class ChatChannelFrame : MonoBehaviour
{
    public UIButton SendMessageButton;
    public UIEventListener SendMessageListener;
    public UIInput EditInput;
    public UILabel EditLabel;

    public GameObject CostInfoObject;
    public GameObject IngotObject;
    public UILabel IngotCostLabel;
    public UILabel FreeLabel;

    public ChatBaseDataHandler m_ChatBaseDataHandler;
}
