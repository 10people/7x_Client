using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mail window controller.
/// </summary>
public class MailWindowManager : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Stored cached mail items.
    /// </summary>
    [HideInInspector]
    public List<MailItem.CachedMailItem> CachedMailItems = new List<MailItem.CachedMailItem>();

    public MailDetail MailDetail;
    public TitleBTNControl TitleButtonControl;

    public UIGrid MailItemsGrid;
    public GameObject MailDetailObject;

    public GameObject MailItemPrefab;

    #endregion

    #region Public Methods

    /// <summary>
    /// Send request message to server.
    /// </summary>
    public void RequestMessage()
    {
//        Debug.Log("Sending mail request message to server.");

        //Used for testing.
        ReceiveMessage();
    }

    /// <summary>
    /// Show mail item list.
    /// </summary>
    public void ShowMailItems()
    {
        MailDetailObject.SetActive(false);
        NGUITools.SetActiveChildren(MailItemsGrid.gameObject, true);

        RequestMessage();
    }

    /// <summary>
    /// Show the detail of one mail.
    /// </summary>
    public void ShowDetail()
    {
        NGUITools.SetActiveChildren(MailItemsGrid.gameObject, false);
        MailDetailObject.SetActive(true);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Receive message from server and execute.
    /// </summary>
    private void ReceiveMessage()
    {
//        Debug.Log("Receive mail message from server.");

        //Used for testing.
        CachedMailItems.Clear();
        CachedMailItems.Add(new MailItem.CachedMailItem
        {
            Type = MailItem.CachedMailItem.IconType.SysNoticeNotRead,
            SenderName = "Adminstrator",
            MailInfo = "Ur mother calls you for dinner!",
            IsContainAccessory = false
        });
        CachedMailItems.Add(new MailItem.CachedMailItem
        {
            Type = MailItem.CachedMailItem.IconType.SysNoticeNotRead,
            SenderName = "AnotherAdminstrator",
            MailInfo = "U buy a 100 million dollar house, a equipment for rewarding.",
            IsContainAccessory = true,
            ItemID = new List<int> { 20202 }
        });

        Refresh();
    }

    /// <summary>
    /// Refresh mails with cached mail data.
    /// </summary>
    private void Refresh()
    {
        //Set grid child items num.
        Global.SetGridItems(MailItemsGrid, CachedMailItems.Count, MailItemPrefab);
        MailItemsGrid.Reposition();

        //Refresh each mail item with stored cached data.
        for (int i = 0; i < MailItemsGrid.transform.childCount; i++)
        {
            var child = MailItemsGrid.transform.GetChild(i);
            var mailItem = child.gameObject.GetComponent<MailItem>() ?? child.gameObject.AddComponent<MailItem>();
            mailItem.MailItemData = CachedMailItems[i];
            mailItem.MailWindowManager = this;

            mailItem.Refresh();
        }
    }

    /// <summary>
    /// Clear data and grid child.
    /// </summary>
    private void Clear()
    {
        CachedMailItems.Clear();

        while (MailItemsGrid.transform.childCount != 0)
        {
            var child = MailItemsGrid.transform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Response to backward button click.
    /// </summary>
    /// <param name="go">click gameobject</param>
    private void BackwardClick(GameObject go)
    {
        if (MailDetailObject.activeSelf)
        {
            ShowMailItems();

            return;
        }

        if (!MailDetailObject.activeSelf)
        {
            CloseClick(go);
        }
    }

    /// <summary>
    /// Response to close button click.
    /// </summary>
    /// <param name="go">click gameobject</param>
    private void CloseClick(GameObject go)
    {
        Clear();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Install trigger handlers.
    /// </summary>
    private void InstallHandlers()
    {
        TitleButtonControl.BtnLeftListener.onClick = BackwardClick;
        TitleButtonControl.BtnRightListener.onClick = CloseClick;
    }

    /// <summary>
    /// Uninstall trigger handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        TitleButtonControl.BtnLeftListener.onClick = null;
        TitleButtonControl.BtnRightListener.onClick = null;
    }

    #endregion

    #region Mono

    void OnEnable()
    {
        InstallHandlers();
        ShowMailItems();

        MailItemsGrid.gameObject.SetActive(true);
        MailDetailObject.SetActive(false);
    }

    void OnDisable()
    {
        UnInstallHandlers();
    }

#if UNITY_EDITOR

    /// <summary>
    /// Used for testing.
    /// </summary>
    void OnGUI()
    {
        if (GUILayout.Button("Send request to server."))
        {
            RequestMessage();
        }
    }

#endif

    #endregion
}