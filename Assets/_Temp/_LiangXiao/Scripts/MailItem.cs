using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// One mail item controller.
/// </summary>
public class MailItem : MonoBehaviour
{
    #region Public Fields

    [HideInInspector]
    public MailWindowManager MailWindowManager;

    public UISprite IconTypeSprite;
    public UILabel SenderNameLabel;
    public UILabel MailDetailLabel;
    public GameObject AccessoryObject;
    public UIEventListener MailItemListener;

    /// <summary>
    /// Mail item data structure.
    /// </summary>
    [HideInInspector]
    public class CachedMailItem
    {
        public enum IconType
        {
            SysNoticeReaden,
            SysNoticeNotRead,
            PlayerIcon,
            BattleReward,
            HouseDeal
        }
        public IconType Type;

        public string SenderName;

        public string MailInfo;

        public bool IsContainAccessory;

        public List<int> ItemID;
    }

    /// <summary>
    /// Stored cached mail data.
    /// </summary>
    [HideInInspector]
    public CachedMailItem MailItemData = new CachedMailItem();

    #endregion

    #region Private Fields

    private const string IconTypePrefix = "IconType_";

    /// <summary>
    /// Shows after long sender name and mail info.
    /// </summary>
    private const string ShowingSubfix = "......";

    private const int ShowingSenderNameNum = 5;
    private const int ShowingMailDetailNum = 12;

    /// <summary>
    /// Sender name that shows.
    /// </summary>
    private string showingSenderName
    {
        get
        {
            return (MailItemData.SenderName.Length > ShowingSenderNameNum) ?
                MailItemData.SenderName.Substring(0, ShowingSenderNameNum) + ShowingSubfix :
                MailItemData.SenderName;
        }
    }

    /// <summary>
    /// Mail detail that shows.
    /// </summary>
    private string showingMailDetail
    {
        get
        {
            return (MailItemData.MailInfo.Length > ShowingMailDetailNum) ?
                MailItemData.MailInfo.Substring(0, ShowingMailDetailNum) + ShowingSubfix :
                MailItemData.MailInfo;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Refresh mail item with stored mail item data.
    /// </summary>
    public void Refresh()
    {
        IconTypeSprite.spriteName = IconTypePrefix + (int)(MailItemData.Type);
        SenderNameLabel.text = showingSenderName;
        MailDetailLabel.text = showingMailDetail;
        AccessoryObject.SetActive(MailItemData.IsContainAccessory);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Response to click mail item.
    /// </summary>
    /// <param name="go">clicked gameobject</param>
    private void MailItemClick(GameObject go)
    {
        MailWindowManager.ShowDetail();

        MailWindowManager.MailDetail.MailItemData = MailItemData;
        MailWindowManager.MailDetail.Refresh();
    }

    /// <summary>
    /// Install trigger handlers.
    /// </summary>
    private void InstallHandlers()
    {
        MailItemListener.onClick = MailItemClick;
    }

    /// <summary>
    /// Uninstall trigger handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        MailItemListener.onClick = null;
    }

    #endregion

    #region Mono

    void OnEnable()
    {
        InstallHandlers();
    }

    void OnDisable()
    {
        UnInstallHandlers();
    }

    #endregion
}
