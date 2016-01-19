using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ProtoBuf.Meta;
using qxmobile.protobuf;

public class UIBagRight : MonoBehaviour, SocketProcessor
{
    public UIBagLeft m_UiBagLeft;
    public UILabel m_UILabelDis;

    public UILabel m_UILabelName;

    public float m_cellWidth;

    public float m_cellHeight;

    public BagItem m_BagItem;

    public int[] m_arrPosIndex = new int[5] { 1, 3, 4, 5, 7 };

    public List<GameObject> m_ListGameObject = new List<GameObject>();

    public List<UILabel> m_UILabelList = new List<UILabel>();

    public UIButton m_UseButton;
    public UIButton m_UseAllButton;
    public UIButton m_OKButton;

    public UIPanel m_ThisPanel;

    private float m_eAlpha = 1.0f;
    private int[] useItemID = { 101, 102, 103 };
    private int[] useAllItemID = { };

    void Start()
    {
        m_arrPosIndex = new int[5] { 1, 3, 4, 5, 7 };
        m_UseButton.gameObject.GetComponent<EventHandler>().m_handler += CheckUseBTNInfo;
        m_OKButton.gameObject.GetComponent<EventHandler>().m_handler += CheckOKBTNInfo;
    }

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Update()
    {
        if (m_eAlpha != m_ThisPanel.alpha)
        {
            if (m_ThisPanel.alpha > m_eAlpha)
            {
                m_ThisPanel.alpha -= 0.1f;
                if (m_ThisPanel.alpha < m_eAlpha)
                {
                    m_ThisPanel.alpha = m_eAlpha;
                }
            }
            else
            {
                m_ThisPanel.alpha += 0.1f;
                if (m_ThisPanel.alpha > m_eAlpha)
                {
                    m_ThisPanel.alpha = m_eAlpha;
                }
            }
        }
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message.m_protocol_index == ProtoIndexes.S_YuJueHeChengResult)
        {

            MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
            YuJueHeChengResult t_login_response = new YuJueHeChengResult();
            QiXiongSerializer t_qx = new QiXiongSerializer();

            t_qx.Deserialize(t_stream, t_login_response, t_login_response.GetType());

            Debug.Log(t_login_response.items.Count);

            Global.CreateBox("合成成功", "获得以下物品", "", t_login_response.items, "确定", "", null);
        }
        return false;
    }

    public void setProp(BagItem bagItem)
    {
        Debug.Log("========1");
        foreach (GameObject item in m_ListGameObject)
        {
            Destroy(item);
        }

        if (bagItem == null)
        {
            m_ThisPanel.gameObject.SetActive(false);
            m_ThisPanel.alpha = 0f;
            m_eAlpha = 0;
            return;
        }

        m_ThisPanel.gameObject.SetActive(true);
        m_eAlpha = 1;
        m_BagItem = bagItem;

        m_UILabelDis.text = DescIdTemplate.GetDescriptionById(BagData.Instance().getXmlFunDisID(bagItem));
        m_UILabelName.text = bagItem.name;

        m_ListGameObject = new List<GameObject>();

        CreateIcon(bagItem);
        Debug.Log(bagItem.itemType);
        if (useAllItemID.Contains(bagItem.itemType))
        {
            m_UseButton.gameObject.SetActive(true);
            m_UseAllButton.gameObject.SetActive(true);
            m_OKButton.gameObject.SetActive(false);
        }
        else if (useItemID.Contains(bagItem.itemType))
        {
            m_UseButton.gameObject.SetActive(true);
            m_UseAllButton.gameObject.SetActive(false);
            m_OKButton.gameObject.SetActive(false);
        }
        else
        {
            m_UseButton.gameObject.SetActive(false);
            m_UseAllButton.gameObject.SetActive(false);
            m_OKButton.gameObject.SetActive(true);
        }
    }

    private void CreateIcon(BagItem bagItem)
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
                                IconSampleLoadCallback);
    }

    public void IconSampleLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = NGUITools.AddChild(gameObject, p_object as GameObject);

        //Set label text and sprite name.
        if (tempObject == null)
        {
            Debug.LogError("Icon in UIBagRight is null");
            return;
        }
        IconSampleManager tempManager = tempObject.GetComponent<IconSampleManager>();

        IconSampleManager.IconType iconType = m_BagItem.itemType == 20000
            ? IconSampleManager.IconType.equipment
            : IconSampleManager.IconType.item;
        var m_wide = 0;
        var m_heiht = 0;

        tempManager.SetIconByID(m_BagItem.itemId, "x" + m_BagItem.cnt);
        tempManager.RightButtomCornorLabel.effectStyle = UILabel.Effect.Outline;
        tempManager.RightButtomCornorLabel.effectColor = new Color(1, 0, 0, 1);

        //Set gameobject name and transform info.
        tempObject.name = "icon";
        tempObject.transform.localScale = Vector3.one;
        tempObject.transform.localPosition = new Vector3(0, 160, 0);

        m_ListGameObject.Add(tempObject);
    }

    void CheckUseBTNInfo(GameObject tempObject)
    {
        Global.ScendID(ProtoIndexes.C_EquipAdd, m_UiBagLeft.m_iIndex);
    }

    void CheckOKBTNInfo(GameObject tempObject)
    {
        setProp(null);
        m_UiBagLeft.m_IsPanelToLeft = false;
        m_UiBagLeft.MovePanel();
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
