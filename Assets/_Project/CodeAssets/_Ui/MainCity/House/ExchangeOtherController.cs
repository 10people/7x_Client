using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// others' exchange box, used in old book sys in house sys.
/// </summary>
public class ExchangeOtherController : MonoBehaviour
{
    public OldBookWindow m_OldBookWindow;

    public List<GameObject> IconSampleParentList;

    [HideInInspector]
    public List<IconSampleManager> IconSampleManagerList = new List<IconSampleManager>();
    [HideInInspector]
    public GameObject IconSamplePrefab;

    [HideInInspector]
    public List<OldBookWindow.ExchangeInfo> ExchangeOtherInfoList = new List<OldBookWindow.ExchangeInfo>();

    [HideInInspector]
    public int PlayerId;

    public UILabel PlayerInfoLabel;

    public void Init()
    {
        if (IconSamplePrefab != null)
        {
            WWW temp = null;
            OnIconSampleLoadCallBack(ref temp, "", IconSamplePrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }

    private void OnIconSampleLoadCallBack(ref WWW www, string temp, Object loadedObject)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = loadedObject as GameObject;
        }

        IconSampleManagerList.Clear();

        //ergodic each exchange box item.
        for (int i = 0; i < IconSampleParentList.Count; i++)
        {
            GameObject iconSampleObject;
            if (IconSampleParentList[i].transform.childCount != 0)
            {
                iconSampleObject = IconSampleParentList[i].transform.GetChild(0).gameObject;
            }
            else
            {
                iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
            }

            //set item's manager
            UtilityTool.ActiveWithStandardize(IconSampleParentList[i].transform, iconSampleObject.transform);
            var itemManager = iconSampleObject.GetComponent<OldBookItemManager>() ??
                  iconSampleObject.AddComponent<OldBookItemManager>();
            itemManager.BoxId = ExchangeOtherInfoList[i].boxId;
            itemManager.ItemId = ExchangeOtherInfoList[i].itemId;

            //instance icon and set
            var m_IconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            if (ExchangeOtherInfoList[i].itemId != 0)
            {
                m_IconSampleManager.SetIconByID(IconSampleManager.IconType.exchangeBox, ExchangeOtherInfoList[i].itemId, "", 5);
                m_IconSampleManager.SetIconBasicDelegate(false, true, OnSelectClick);
            }
            else
            {
                m_IconSampleManager.SetIconType(IconSampleManager.IconType.null_type);
                m_IconSampleManager.SetIconBasic(5);
                m_IconSampleManager.SetIconBasicDelegate();
            }

            IconSampleManagerList.Add(m_IconSampleManager);
        }
    }

    /// <summary>
    /// on box item selected
    /// </summary>
    /// <param name="go"></param>
    private void OnSelectClick(GameObject go)
    {
        var iconSampleManager = go.GetComponent<IconSampleManager>();
        var itemManager = go.GetComponent<OldBookItemManager>();

        //set active state.
        bool isActiveBefore = iconSampleManager.SelectFrameSprite.gameObject.activeSelf;

        m_OldBookWindow.ExchangeOtherControllerList.ForEach(
            controller =>
                controller.IconSampleManagerList.ForEach(
                    manager => manager.SelectFrameSprite.gameObject.SetActive(false)));

        iconSampleManager.SelectFrameSprite.gameObject.SetActive(!isActiveBefore);

        //add to exchange or not.
        if (iconSampleManager.SelectFrameSprite.gameObject.activeSelf)
        {
            m_OldBookWindow.OtherToExchange = new OldBookWindow.ToExchange
            {
                itemId = itemManager.ItemId,
                boxId = itemManager.BoxId,
                playerId = PlayerId
            };
        }
        else
        {
            m_OldBookWindow.OtherToExchange = null;
        }

        //exchange check
        m_OldBookWindow.CheckExchange();
    }
}
