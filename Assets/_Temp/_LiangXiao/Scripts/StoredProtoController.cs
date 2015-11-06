using UnityEngine;
using System.Collections;

public class StoredProtoController : MonoBehaviour
{
    public ProtoIndex m_ProtoIndex;
    public UILabel m_Label;

    void OnClick()
    {
        ProtoToolManager.Instance.SetViewMessage(m_ProtoIndex);
        ProtoToolManager.Instance.RefreshViewMessageGrid();
    }        
}
