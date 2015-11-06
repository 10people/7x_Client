using System;
using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

/// <summary>
/// House basic class, must be implemented, can't be instanced.
/// </summary>
public abstract class HouseBasic : MonoBehaviour
{
    /// <summary>
    /// Get house state display string.
    /// </summary>
    /// <param name="state">house state</param>
    /// <param name="isSmallHouse">is get state in small house or not</param>
    /// <returns>display string</returns>
    public static string GetStateStr(int state, bool isSmallHouse)
    {
        switch (state)
        {
            case 10:
                return isSmallHouse ? "待售" : "无主";
            case 20:
                return "自住";
            case 404:
                return isSmallHouse ? "荒废" : "关闭";
            case 505:
                return isSmallHouse ? "强制待售" : "";
            default:
                return "";
        }
    }

    /// <summary>
    /// Get official post display string.
    /// </summary>
    /// <param name="post">official post num</param>
    /// <returns>display string</returns>
    public static string GetAllianceOfficialPostStr(int post)
    {
        switch (post)
        {
            case 0:
                return "成员";
            case 1:
                return "副盟主";
            case 2:
                return "盟主";
            default:
                return "";
        }
    }

    /// <summary>
    /// Get military rank display string.
    /// </summary>
    /// <param name="rank">military rank num</param>
    /// <returns>display string</returns>
    public static string GetMilitaryRankStr(int rank)
    {
        return NameIdTemplate.GetName_By_NameId(rank);
    }

    /// <summary>
    /// store house info.
    /// </summary>
    [HideInInspector]
    public HouseSimpleInfo m_HouseSimpleInfo;

    /// <summary>
    /// Secert card to showed.
    /// </summary>
    public MibaoInfo m_TreasureShowed;
    /// <summary>
    /// weapon to showed
    /// </summary>
    public BagItem m_WeaponShowed;

    /// <summary>
    /// house 3d model root manager
    /// </summary>
    public HouseRootManager m_HouseRootManager;

    public OldBookWindow m_OldBookWindow;
    public HouseWeaponShower m_HouseWeaponShower;
    public HouseSecretCardShower m_HouseSecretCardShower;

    /// <summary>
    /// house 3d model
    /// </summary>
    public HouseModelController m_HouseModelController;

    public abstract void ShowTreasure();

    public abstract void ShowWeapon();

    public abstract void OnBookClick();

    /// <summary>
    /// Enter house click
    /// </summary>
    public abstract void OnEnterClick();

    /// <summary>
    /// exit house click
    /// </summary>
    public virtual void OnExitClick()
    {
        HousePlayerController.s_HousePlayerController.StartNavigation(m_HouseModelController.HouseDoorPosition.position);
        TenementData.Instance.RequestData();

        if (!AllianceData.Instance.IsAllianceNotExist)
        {
            CityGlobalData.m_isAllianceScene = true;
        }
    }

    public delegate void Refresh();
    public event Refresh RefreshEvent;

    /// <summary>
    /// Call when house data refresh, event cleaned when execute once.
    /// </summary>
    public void RefreshData()
    {
        if (RefreshEvent != null)
        {
            RefreshEvent();
            RefreshEvent = null;
        }
    }

    public void OnEnable()
    {
        //add this to main city gameobject list.
        MainCityUI.TryAddToObjectList(gameObject);
    }

    public void OnDisable()
    {
        //remove this from main city gameobject list.
        MainCityUI.TryRemoveFromObjectList(gameObject);
    }

    public void Start()
    {
        TenementManagerment.m_HouseObjectList.Add(gameObject);
    }

    public void OnDestroy()
    {
        TenementManagerment.m_HouseObjectList.Remove(gameObject);
    }

    public void Awake()
    {

    }
}

/// <summary>
/// Self house interface
/// </summary>
public interface IHouseSelf
{
    /// <summary>
    /// fitment house or donate to alliance
    /// </summary>
    void OnFitmentOrDonateClick();

    /// <summary>
    /// switch door open or lock.
    /// </summary>
    void OnSwitchDoorClick();

    /// <summary>
    /// switch sell or living state.
    /// </summary>
    void OnSwitchSellClick();

    /// <summary>
    /// kick out player.
    /// </summary>
    void OnGetOutClick();

    /// <summary>
    /// receive player exp.
    /// </summary>
    void OnReceiveClick();

    /// <summary>
    /// self house operation
    /// </summary>
    void OnOperationClick();
}