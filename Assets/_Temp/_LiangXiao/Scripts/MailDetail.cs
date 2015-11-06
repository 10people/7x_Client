using UnityEngine;
using System.Collections;

/// <summary>
/// Mail detail controller.
/// </summary>
public class MailDetail : MonoBehaviour
{
    #region Public Fields

    public UILabel DetailLabel;
    public UIGrid AccessoryGrid;
    public GameObject ReceiveBTNObject;

    public GameObject AccessoryPrefab;

    public UIEventListener ReceiveBtnListener;

    /// <summary>
    /// Stored cache mail data.
    /// </summary>
    [HideInInspector]
    public MailItem.CachedMailItem MailItemData = new MailItem.CachedMailItem();

    #endregion

    #region Public Methods

    /// <summary>
    /// Refresh mail detail with stored mail data.
    /// </summary>
    public void Refresh()
    {
        //Show mail info and accessory.
        DetailLabel.text = MailItemData.MailInfo;
        NGUITools.SetActiveChildren(AccessoryGrid.gameObject, MailItemData.IsContainAccessory);
        ReceiveBTNObject.SetActive(MailItemData.IsContainAccessory);

        //Set each accessory icon image.
        if (MailItemData.IsContainAccessory)
        {
            Global.SetGridItems(AccessoryGrid, MailItemData.ItemID.Count, AccessoryPrefab);
            AccessoryGrid.Reposition();

            for (int i = 0; i < AccessoryGrid.transform.childCount; i++)
            {
                var child = AccessoryGrid.transform.GetChild(i);
                var imgSprite = child.FindChild("ItemImg").GetComponent<UISprite>();
                imgSprite.spriteName = MailItemData.ItemID[i].ToString();
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Response to receive button click.
    /// </summary>
    /// <param name="go"></param>
    private void OnReceiveBTNClick(GameObject go)
    {
        //Send accessory received message to server.

        NGUITools.SetActiveChildren(AccessoryGrid.gameObject, false);
    }

    /// <summary>
    /// Install trigger handlers.
    /// </summary>
    private void InstallHandlers()
    {
        ReceiveBtnListener.onClick = OnReceiveBTNClick;
    }

    /// <summary>
    /// Uninstall trigger handlers.
    /// </summary>
    private void UnInstallHandlers()
    {
        ReceiveBtnListener.onClick = null;
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
