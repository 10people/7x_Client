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

    public ChatBaseDataHandler m_ChatBaseDataHandler;
}
