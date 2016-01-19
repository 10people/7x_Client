using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using qxmobile.protobuf;

/// <summary>
/// House 3d model controller
/// </summary>
public class HouseModelController : MonoBehaviour, SocketListener
{
    /// <summary>
    /// if dimmer exist, stop trigger house item.
    /// </summary>
    public static List<GameObject> DimmerObjectList = new List<GameObject>();

    public static void TryAddToHouseDimmer(GameObject g)
    {
        if (MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.HouseSelf ||
            MainCityUI.m_PlayerPlace == MainCityUI.PlayerPlace.HouseOther)
        {
            DimmerObjectList.Add(g);
        }
    }

    public static void TryRemoveFromHouseDimmer(GameObject g)
    {
        if (DimmerObjectList.Contains(g))
        {
            DimmerObjectList.Remove(g);
        }
    }

    [HideInInspector]
    public HouseBasic m_House;
    public Camera m_HouseCamera;

    private List<BagItem> m_weaponList = new List<BagItem>();
    private List<MibaoInfo> m_treasureList = new List<MibaoInfo>();
    private readonly Dictionary<GameObject, int> m_weaponObjectDic = new Dictionary<GameObject, int>();
    private readonly Dictionary<GameObject, int> m_treasureObjectDic = new Dictionary<GameObject, int>();
    private GameObject m_bookObject;

    public Transform HouseDoorPosition;

    public void SetRoot3D()
    {
        //show 3d
        if (HouseRootManager.IsShow3D)
        {
            //self house
            if (m_House.m_HouseSimpleInfo.jzId == JunZhuData.Instance().m_junzhuInfo.id)
            {
                //get weapon list.
                m_weaponList =
                    BagData.Instance()
                        .m_playerEquipDic.Values.Concat(EquipsOfBody.Instance().m_equipsOfBodyDic.Values)
                        .Where(item => item.buWei == 3 || item.buWei == 4 || item.buWei == 5)
                        .ToList();

                //                //get treasure list.
                //                m_treasureList = MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoInfo.Where(
                //                   info =>
                //                   {
                //                       var miBaoXmlTemp = MiBaoXmlTemp.templates.FirstOrDefault(template => template.id == info.miBaoId);
                //                       return miBaoXmlTemp != null && miBaoXmlTemp.pinzhi >= 4;
                //                   })
                //                   .ToList();
            }
            //others' house
            else
            {
                //request weapon list and treasure list.
                EquipInfoOtherReq temp = new EquipInfoOtherReq
                {
                    ownerid = m_House.m_HouseSimpleInfo.jzId
                };
                SocketHelper.SendQXMessage(temp, ProtoIndexes.C_GET_OTHER_WEAPON);

                MibaoInfoOtherReq temp2 = new MibaoInfoOtherReq
                {
                    ownerId = m_House.m_HouseSimpleInfo.jzId
                };
                SocketHelper.SendQXMessage(temp2, ProtoIndexes.C_MIBAO_INFO_OTHER_REQ);
            }
        }
    }

    private void OnWeaponClick(int i)
    {
        if (m_weaponList.Count < i + 1)
        {
            Debug.LogWarning("weapon index: " + i + ", out of range: " + (m_weaponList.Count));
            return;
        }

        m_House.m_WeaponShowed = m_weaponList[i];
        HousePlayerController.s_HousePlayerController.m_CompleteNavDelegate = ShowWeaponWindow;
        HousePlayerController.s_HousePlayerController.StartNavigation(m_weaponObjectDic.FirstOrDefault(item => item.Value == i).Key.transform.position);
    }

    public void ShowWeaponWindow()
    {
        m_House.ShowWeapon();
    }

    private void OnTreasureClick(int i)
    {
        if (m_treasureList.Count < i + 1)
        {
            Debug.LogWarning("treasure index: " + i + ", out of range: " + (m_treasureList.Count));
            return;
        }

        m_House.m_TreasureShowed = m_treasureList[i];
        HousePlayerController.s_HousePlayerController.m_CompleteNavDelegate = ShowTreasureWindow;
        HousePlayerController.s_HousePlayerController.StartNavigation(m_treasureObjectDic.FirstOrDefault(item => item.Value == i).Key.transform.position);
    }

    public void ShowTreasureWindow()
    {
        m_House.ShowTreasure();
    }

    private void OnBookClick(int i)
    {
        HousePlayerController.s_HousePlayerController.m_CompleteNavDelegate = ShowBookWindow;
        HousePlayerController.s_HousePlayerController.StartNavigation(m_bookObject.transform.position);
    }

    public void ShowBookWindow()
    {
        m_House.OnBookClick();
    }

    public bool OnSocketEvent(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                //receive treasure list data.
                case ProtoIndexes.S_MIBAO_INFO_OTHER_RESP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        MibaoInfoResp tempInfo = new MibaoInfoResp();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        //Abort get data if null.
                        //                        if (tempInfo == null || tempInfo.mibaoInfo == null)
                        //                        {
                        //                            return true;
                        //                        }
                        //
                        //                        //Get treasure where pinzhi >= 4(purple).
                        //                        m_treasureList = tempInfo.mibaoInfo.Where(
                        //                               info =>
                        //                               {
                        //                                   var miBaoXmlTemp = MiBaoXmlTemp.templates.FirstOrDefault(template => template.id == info.miBaoId);
                        //                                   return miBaoXmlTemp != null && miBaoXmlTemp.pinzhi >= 4;
                        //                               })
                        //                               .ToList();

                        return true;
                    }
                //receive weapon list data.
                case ProtoIndexes.S_GET_OTHER_WEAPON:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                        QiXiongSerializer t_qx = new QiXiongSerializer();
                        EquipInfo tempInfo = new EquipInfo();
                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                        //Abort get data if null.
                        if (tempInfo == null || tempInfo.items == null || tempInfo.items.Count == 0)
                        {
                            return true;
                        }

                        //Get weapon equiped and not equiped.
                        m_weaponList = tempInfo.items.Where(item => item.buWei == 1 || item.buWei == 2 || item.buWei == 3).ToList();

                        return true;
                    }
                default:
                    return false;
            }
        }
        return false;
    }

    void Start()
    {
        m_weaponObjectDic.Clear();
        m_treasureObjectDic.Clear();
        m_bookObject = null;

		var bookParent = TransformHelper.FindChild(transform, "book_root");
        foreach (Transform child in bookParent)
        {
            m_bookObject = child.gameObject;
        }

		var weaponParent = TransformHelper.FindChild(transform, "weapon_root");
        foreach (Transform child in weaponParent)
        {
            m_weaponObjectDic.Add(child.gameObject, child.GetComponent<EventIndexHandle>().m_SendIndex);
        }

		var treasureParent = TransformHelper.FindChild(transform, "treasure_root");
        foreach (Transform child in treasureParent)
        {
            m_treasureObjectDic.Add(child.gameObject, child.GetComponent<EventIndexHandle>().m_SendIndex);
        }

        //Check setted.
        if (m_weaponObjectDic.Count == 0 || m_treasureObjectDic.Count == 0 || m_bookObject == null)
        {
            Debug.LogError("Error when start HouseModelController.");
            return;
        }

        foreach (var item in m_weaponObjectDic)
        {
            item.Key.GetComponent<EventIndexHandle>().m_Handle += OnWeaponClick;
        }
        foreach (var item in m_treasureObjectDic)
        {
            item.Key.GetComponent<EventIndexHandle>().m_Handle += OnTreasureClick;
        }
        m_bookObject.GetComponent<EventIndexHandle>().m_Handle += OnBookClick;
    }

    void Awake()
    {
        SocketTool.RegisterSocketListener(this);
    }

    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        foreach (var item in m_weaponObjectDic)
        {
            item.Key.GetComponent<EventIndexHandle>().m_Handle -= OnWeaponClick;
        }
        foreach (var item in m_treasureObjectDic)
        {
            item.Key.GetComponent<EventIndexHandle>().m_Handle -= OnTreasureClick;
        }
        m_bookObject.GetComponent<EventIndexHandle>().m_Handle -= OnBookClick;

		// added by YuGu, 2016.1.5
		Debug.LogError( "Is it clear DimmerObjectList ok?" );

		DimmerObjectList.Clear();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
        {
            if (DimmerObjectList.Count != 0)
            {
                return;
            }

            //House weapon, treasure, book click trigger.
            Ray ray = m_HouseCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] tempHits = Physics.RaycastAll(ray, Mathf.Infinity);

            RaycastHit tempHit = tempHits.Where(item => item.transform.tag == "HouseItemTrigger").FirstOrDefault();
            if (tempHit.transform != null)
            {
                tempHit.transform.gameObject.SendMessage("OnClick");
            }
        }
    }
}
