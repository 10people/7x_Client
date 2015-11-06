using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CheckEquipInfo : MonoBehaviour {

    public GameObject m_layer;

    void OnClick()
    {
        m_layer.SetActive(true);
    }
}
