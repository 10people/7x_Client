﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using qxmobile.protobuf;
using UnityEngine;

/// <summary>
/// Bag left part handler.
/// </summary>
public class UIBagLeft : MonoBehaviour, SocketListener
{
    #region Public Fields

    //public enum ItemType
    //{
    //    Equip,
    //    Material,
    //    Item
    //};

    //public ItemType m_itemType;

    /// <summary>
    /// Panel that controls bag left.
    /// </summary>
    public GameObject m_LeftPanel;
    public GameObject m_MainUIParent;
    public GameObject m_BottomLeft;
    /// <summary>
    /// Stored BagItem data.
    /// </summary>
    [HideInInspector]
    public List<BagItem> m_ListBag = new List<BagItem>();

    /// <summary>
    /// Stored BagItem UI.
    /// </summary>
    public List<IconSampleManager> m_IconSampleManagers;

    /// <summary>
    /// Bag right part handler.
    /// </summary>
    public UIBagRight m_UIBagRight;

    /// <summary>
    /// Grid contains bag data items.
    /// </summary>
    public UIGrid m_UiGrid;

    public UIScrollView m_ScrollView;

    #endregion

    #region Private Fields

    private const float panelMoveDuration = 0.5f;

    [HideInInspector]
    public bool m_IsPanelToLeft;
    public int m_iIndex;
    private GameObject m_objCopyEquip;
    private Transform m_transform;

    private float panelMovePosX
    {
        get { return m_IsPanelToLeft ? 0 : 142.53f; }
    }

    private int m_temp_count
    {
        get { return m_ListBag.Count > 25 ? m_ListBag.Count : 25; }
    }

    #endregion

    #region Public Fields

    private bool isInited = false;

    /// <summary>
    /// Initialize bag grid and items.
    /// </summary>
    public void Refresh()
    {
        //delete old gameobject and iconsample manager.
        var tempObjectList = (from Transform tempTrs in m_transform select tempTrs.gameObject).ToList();
        foreach (var item in tempObjectList)
        {
            Destroy(item);
        }
        m_IconSampleManagers.Clear();

        //sort and add item which dbid ! = 0 to bag list.
        m_ListBag = new List<BagItem>();
        List<BagItem> showedBagItems = BagData.Instance().m_bagItemList.Where(item => BagData.Instance().getXmlType(item) != 0).ToList();

        //Add other item to bag list
        List<BagItem> tempItems = showedBagItems.Where(item => !(new int[] { 21, 20000, 6, 1, 2 }.Contains(BagData.Instance().getXmlType(item)))).OrderBy(item => item.itemId).ToList();
        m_ListBag.AddRange(tempItems);

        //Add YuJue to bag list
        tempItems = showedBagItems.Where(item => BagData.Instance().getXmlType(item) == 21).OrderBy(item => item.itemId).ToList();
        m_ListBag.AddRange(tempItems);

        //Add equipments to bag list.
        tempItems = showedBagItems.Where(item => BagData.Instance().getXmlType(item) == 20000).OrderBy(item => item.buWei).ToList();
        m_ListBag.AddRange(tempItems);

        //Add equipments lvl up to bag list.
        tempItems = showedBagItems.Where(item => BagData.Instance().getXmlType(item) == 6).OrderBy(item => item.itemId).ToList();
        m_ListBag.AddRange(tempItems);

        //Add equipments enforce to bag list.
        tempItems = showedBagItems.Where(item => BagData.Instance().getXmlType(item) == 1).OrderByDescending(item => item.itemId).ToList();
        m_ListBag.AddRange(tempItems);
        tempItems = showedBagItems.Where(item => BagData.Instance().getXmlType(item) == 2).OrderByDescending(item => item.itemId).ToList();
        m_ListBag.AddRange(tempItems);

        if (m_iIndex < m_ListBag.Count)
        {
          //  m_UIBagRight.setProp(m_ListBag[m_iIndex]);
            m_IsPanelToLeft = true;
        }
        else
        {
            if (m_ListBag.Count != 0)
            {
                m_iIndex = 0;
              //  m_UIBagRight.setProp(m_ListBag[0]);
                m_IsPanelToLeft = true;
            }
            else
            {
                m_UIBagRight.setProp(null);
                m_IsPanelToLeft = false;
            }
        }

        RefreshIcons();

        isInited = true;
    }

    private void RefreshHighLightState()
    {
        m_IconSampleManagers.ForEach(item =>
        {
            if (BagData.Instance().m_HighlightItemList.Contains(item.iconID))
            {
                SparkleEffectItem.OpenSparkle(item.FgSprite.gameObject, SparkleEffectItem.MenuItemStyle.Common_Icon);
            }
            else
            {
                SparkleEffectItem.CloseSparkle(item.FgSprite.gameObject);
            }
        });
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message == null)
        {
            return false;
        }

        switch (p_message.m_protocol_index)
        {
            case ProtoIndexes.S_BagInfo:
            case ProtoIndexes.S_BAG_CHANGE_INFO:
                {
                    isInited = false;

                    if (gameObject.activeInHierarchy)
                    {
                        Refresh();
                    }

                    return true;
                }
            case ProtoIndexes.S_GET_HighLight_item_ids:
                {
                    if (gameObject.activeInHierarchy)
                    {
                        RefreshHighLightState();
                    }

                    return true;
                }

            default:
                return false;
        }
    }

    #endregion

    #region Private Methods

    public GameObject IconSamplePrefab;

    private void RefreshIcons()
    {
        for (var i = 0; i < m_temp_count; i++)
        {
            if (m_objCopyEquip == null)
            {
                m_objCopyEquip = (GameObject)IconSamplePrefab;
            }
            GameObject tempObject = NGUITools.AddChild(gameObject, m_objCopyEquip);
            var tempManager = tempObject.GetComponent<IconSampleManager>();

            //Set label text, sprite name and event.
            if (i < m_ListBag.Count)
            {
                tempManager.SetIconByID(m_ListBag[i].itemId, m_ListBag[i].cnt.ToString(), 0, false, false);
                tempManager.SetIconBasicDelegate(false, true, CheckInfo);
                tempManager.RightButtomCornorLabel.effectStyle = UILabel.Effect.Outline;
                tempManager.RightButtomCornorLabel.effectColor = new Color(0, 0, 0, 1);

                m_IconSampleManagers.Add(tempManager);
            }
            else
            {
                tempManager.SetIconType(IconSampleManager.IconType.null_type);
                tempManager.SetIconBasic(0, "", "", "");
                tempManager.RightButtomCornorLabel.effectStyle = UILabel.Effect.Outline;
                tempManager.RightButtomCornorLabel.effectColor = new Color(0, 0, 0, 1);

            }

            //Set gameobject name and transform info.
            tempObject.name = i.ToString();
            tempObject.transform.localScale = Vector3.one;
        }

        //Reposition items, wait one frame for avoiding error.
        StartCoroutine(DoReposition());

     //   MovePanel();

        //Refresh highlight.
        RefreshHighLightState();
    }

    public void MovePanel()
    {
        iTween.MoveTo(m_LeftPanel.gameObject,
            iTween.Hash("position", new Vector3(panelMovePosX, 0, 0),
                "time", panelMoveDuration,
                "easeType", "linear",
                "islocal", true));
    }

    private IEnumerator DoReposition()
    {
        yield return null;
        m_UiGrid.Reposition();

        m_ScrollView.UpdatePosition();
    }

    private void CheckInfo(GameObject tempObject)
    {
        IconSampleManager tempManager = tempObject.GetComponent<IconSampleManager>();
        if (tempManager == null)
        {
            Debug.LogError("No iconsamplemanager found in object:" + tempObject.name);
            return;
        }

        GameObject obj = new GameObject();
        obj.transform.parent = tempObject.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.parent = m_MainUIParent.transform;
        if (obj.transform.localPosition.y > 244 )
        {
           obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 244,0);
        }
        else if (obj.transform.localPosition.y < 44)
        {
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, 44, 0);
        }
        m_BottomLeft.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
        Destroy(obj);

        //foreach (var item in m_IconSampleManagers)
        //{
        //    item.SelectFrameSprite.gameObject.SetActive(false);
        //}
     //   tempManager.SelectFrameSprite.gameObject.SetActive(true);

        m_iIndex = int.Parse(tempManager.gameObject.name);
        m_UIBagRight.setProp(m_ListBag[m_iIndex]);
        m_IsPanelToLeft = true;
       // MovePanel();
    }

    #endregion

    #region Mono

    void Awake()
    {
        m_transform = transform;

        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);
    }

    void OnEnable()
    {
        //Close guide.
        CityGlobalData.m_isRightGuide = true;

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_GET_HighLight_item_ids);

        if (!isInited)
        {
            Refresh();
        }
    }

    #endregion
}