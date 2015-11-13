using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

public class TenementManagerment : Singleton<TenementManagerment>
{
    [HideInInspector]
    public GameObject SmallHouseSelfPrefab;
    [HideInInspector]
    public GameObject SmallHouseOtherPrefab;
    [HideInInspector]
    public GameObject BigHouseSelfPrefab;
    [HideInInspector]
    public GameObject BigHouseOtherPrefab;

    public List<EventIndexHandle> m_listTenementEvent = new List<EventIndexHandle>();

    private int houseIndexClicked;

    public static List<GameObject> m_HouseObjectList = new List<GameObject>();

    void Start()
    {
        m_listTenementEvent.ForEach(p => p.m_Handle += TenementLocalPosition);
    }

    void TenementLocalPosition(int index)
    {
        NpcManager.m_NpcManager.setGoToTenementNpc(index + 1000);
    }

    public void NavgationToTenement(int index)
    {

        if (m_HouseObjectList.Count > 0)
        {
            Debug.LogWarning("cancel tenement navigation cause a housebasic existed.");
            return;
        }

        //House exist.
        if (TenementData.Instance.m_AllianceCityTenementDic.ContainsKey(index))
        {
            UIYindao.m_UIYindao.CloseUI();
            houseIndexClicked = index;

            //Not my house.
            if (TenementData.Instance.m_AllianceCityTenementDic[index].jzId != JunZhuData.Instance().m_junzhuInfo.id)
            {
                if (index > 100)
                {
                    if (BigHouseOtherPrefab == null)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.BIG_HOUSE_OTHER), OnBigHouseOtherLoadCallBack);
                    }
                    else
                    {
                        WWW temp = null;
                        OnBigHouseOtherLoadCallBack(ref temp, null, BigHouseOtherPrefab);
                    }
                }
                else
                {
                    if (SmallHouseOtherPrefab == null)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SMALL_HOUSE_OTHER), OnSmallHouseOtherLoadCallBack);
                    }
                    else
                    {
                        WWW temp = null;
                        OnSmallHouseOtherLoadCallBack(ref temp, null, SmallHouseOtherPrefab);
                    }
                }
            }
            //My house.
            else
            {
//                Debug.Log("My houseMy houseMy houseMy houseMy houseMy houseMy houseMy house");
                
				if (index > 100)
                {
//                    Debug.Log("My houseMy houseMy houseMy houseMy houseMy houseMy houseMy house");
                    
					if (BigHouseSelfPrefab == null)
                    {
//                        Debug.Log("My houseMy houseMy houseMy houseMy houseMy houseMy houseMy house");
                        
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.BIG_HOUSE_SELF),
                            OnBigHouseSelfLoadCallBack);
                    }
                    else
                    {
//                        Debug.Log("My houseMy houseMy houseMy houseMy houseMy houseMy houseMy house");
                        
						WWW temp = null;
                   
                        OnBigHouseSelfLoadCallBack(ref temp, null, BigHouseSelfPrefab);
                    }
                }
                else
                {
                    if (SmallHouseSelfPrefab == null)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SMALL_HOUSE_SELF), OnSmallHouseSelfLoadCallBack);
                    }
                    else
                    {
                        WWW temp = null;
                        OnSmallHouseSelfLoadCallBack(ref temp, null, SmallHouseSelfPrefab);
                    }
                }
            }
        }
        //House not exist.
        else
        {
            if (index > 100)
            {
                Debug.LogWarning("big House with str:" + index + " has closed.");
            }
            else
            {
                Debug.LogWarning("small House with str:" + index + " has no ownner.");
                {
                    UIYindao.m_UIYindao.CloseUI();
                    houseIndexClicked = index;

                    if (SmallHouseOtherPrefab == null)
                    {
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SMALL_HOUSE_OTHER), OnSmallHouseNoOwnerLoadCallBack);
                    }
                    else
                    {
                        WWW temp = null;
                        OnSmallHouseNoOwnerLoadCallBack(ref temp, null, SmallHouseOtherPrefab);
                    }
                }
            }
        }
    }

    void OnSmallHouseSelfLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (SmallHouseSelfPrefab == null)
        {
            SmallHouseSelfPrefab = loadedObject as GameObject;
        }

        var smallHouseSelf = Instantiate(SmallHouseSelfPrefab) as GameObject;
        smallHouseSelf.transform.rotation = new Quaternion(0, 0, 0, 0);
        smallHouseSelf.transform.localScale = Vector3.one;
        DontDestroyOnLoad(smallHouseSelf);
        smallHouseSelf.gameObject.SetActive(true);

        var smallHouseSelfManager = smallHouseSelf.GetComponent<SmallHouseSelf>();
        smallHouseSelfManager.m_HouseSimpleInfo = TenementData.Instance.m_AllianceCityTenementDic[houseIndexClicked];

        //if big house exist
        if (TenementData.Instance.m_AllianceCityTenementDic.Count(item => item.Key > 100 && item.Value.jzId == JunZhuData.Instance().m_junzhuInfo.id) != 0)
        {
            smallHouseSelfManager.IsBigHouseExist = true;
            HouseRootManager.IsShow3D = false;
        }
        else
        {
            HouseRootManager.IsShow3D = true;
        }

        smallHouseSelfManager.m_SmallHouseSelfEnter.SetEnterInfo();
    }

    void OnSmallHouseOtherLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (SmallHouseOtherPrefab == null)
        {
            SmallHouseOtherPrefab = loadedObject as GameObject;
        }

        var smallHouseOther = Instantiate(SmallHouseOtherPrefab) as GameObject;
        smallHouseOther.transform.rotation = new Quaternion(0, 0, 0, 0);
        smallHouseOther.transform.localScale = Vector3.one;
        DontDestroyOnLoad(smallHouseOther);
        smallHouseOther.gameObject.SetActive(true);

        var smallHouseOtherManager = smallHouseOther.GetComponent<SmallHouseOther>();
        smallHouseOtherManager.m_HouseSimpleInfo = TenementData.Instance.m_AllianceCityTenementDic[houseIndexClicked];

        HouseRootManager.IsShow3D = true;
        smallHouseOtherManager.m_SmallHouseOtherEnter.SetEnterInfo();
    }

    void OnBigHouseSelfLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (BigHouseSelfPrefab == null)
        {
            BigHouseSelfPrefab = loadedObject as GameObject;
        }

        var bigHouseSelf = Instantiate(BigHouseSelfPrefab) as GameObject;
        bigHouseSelf.transform.rotation = new Quaternion(0, 0, 0, 0);
        bigHouseSelf.transform.localScale = Vector3.one;
        DontDestroyOnLoad(bigHouseSelf);
        bigHouseSelf.gameObject.SetActive(true);

        var bigHouseSelfManager = bigHouseSelf.GetComponent<BigHouseSelf>();
        bigHouseSelfManager.m_HouseSimpleInfo = TenementData.Instance.m_AllianceCityTenementDic[houseIndexClicked];

        HouseRootManager.IsShow3D = true;
        bigHouseSelfManager.m_BigHouseSelfEnter.SetEnterInfo();
    }

    void OnBigHouseOtherLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (BigHouseOtherPrefab == null)
        {
            BigHouseOtherPrefab = loadedObject as GameObject;
        }

        var bigHouseOther = Instantiate(BigHouseOtherPrefab) as GameObject;
        bigHouseOther.transform.rotation = new Quaternion(0, 0, 0, 0);
        bigHouseOther.transform.localScale = Vector3.one;
        DontDestroyOnLoad(bigHouseOther);
        bigHouseOther.gameObject.SetActive(true);

        var bigHouseOtherManager = bigHouseOther.GetComponent<BigHouseOther>();
        bigHouseOtherManager.m_HouseSimpleInfo = TenementData.Instance.m_AllianceCityTenementDic[houseIndexClicked];

        HouseRootManager.IsShow3D = true;
        bigHouseOtherManager.m_BigHouseOtherEnter.SetEnterInfo();
    }

    void OnSmallHouseNoOwnerLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        if (SmallHouseOtherPrefab == null)
        {
            SmallHouseOtherPrefab = loadedObject as GameObject;
        }

        var smallHouseOther = Instantiate(SmallHouseOtherPrefab) as GameObject;
        smallHouseOther.transform.rotation = new Quaternion(0, 0, 0, 0);
        smallHouseOther.transform.localScale = Vector3.one;
        DontDestroyOnLoad(smallHouseOther);
        smallHouseOther.gameObject.SetActive(true);

        var smallHouseOtherManager = smallHouseOther.GetComponent<SmallHouseOther>();
        smallHouseOtherManager.m_HouseSimpleInfo = new HouseSimpleInfo()
        {
            firstHoldTime = "",
            firstOwner = "",
            hworth = -1,
            jzName = "",
            jzId = -1,
            locationId = houseIndexClicked,
            open4my = false,
            state = -1
        };

        HouseRootManager.IsShow3D = true;
        smallHouseOtherManager.m_SmallHouseOtherEnter.SetEnterInfo();
    }

}
