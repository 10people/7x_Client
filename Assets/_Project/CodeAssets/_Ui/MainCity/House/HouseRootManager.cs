using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

/// <summary>
/// House 3d model root
/// </summary>
public class HouseRootManager : MonoBehaviour
{
    public static HouseRootManager m_HouseRootManager;

    [HideInInspector]
    public static bool IsShow3D = true;

    [HideInInspector]
    public MainCityUI m_mainCityUi;
    [HideInInspector]
    public HousePlayerController HousePlayerController;
    [HideInInspector]
    public HouseBasic m_houseBasic;

    private void SetHouseModel()
    {
        HouseModelController[] temp = FindObjectsOfType<HouseModelController>();
        if (temp == null || temp.Count() != 1)
        {
            Debug.LogError("Got error in finding house model controller.");
            return;
        }

        m_houseBasic.m_HouseModelController = temp[0];
        m_houseBasic.m_HouseModelController.m_House = m_houseBasic;

        var tempRoot = GameObject.Find("UI_Root_Temp");
        UtilityTool.ActiveWithStandardize(null, m_houseBasic.m_HouseModelController.transform);

        m_houseBasic.m_HouseModelController.gameObject.SetActive(true);
        m_houseBasic.m_HouseModelController.SetRoot3D();
    }

    private void OnPlayerLoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        var tempObject = Instantiate(loadedObject) as GameObject;
        UtilityTool.ActiveWithStandardize(null, tempObject.transform);

        HousePlayerController = tempObject.AddComponent<HousePlayerController>();

        HousePlayerController.IsRotateCamera = true;
        HousePlayerController.IsUploadPlayerPosition = true;
        HousePlayerController.BaseGroundPosY = 0;
        HousePlayerController.m_CharacterSyncDuration = 0.24f;

        HousePlayerController.TrackCamera = m_houseBasic.m_HouseModelController.m_HouseCamera;
        HousePlayerController.TrackCamera.gameObject.SetActive(true);

        HousePlayerController.m_Joystick = m_mainCityUi.m_MainCityUILB;
    }

    private void OnMainCityUILoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        var tempObject = Instantiate(loadedObject) as GameObject;
        m_mainCityUi = tempObject.GetComponent<MainCityUI>();
        UIYindao.m_UIYindao.CloseUI();

        //set house 3d model.
        SetHouseModel();

        //load and set player.
        Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(100 + CityGlobalData.m_king_model_Id),
            OnPlayerLoadCallBack);
    }

    private void Start()
    {
        m_houseBasic = m_houseBasic ?? FindObjectOfType<SmallHouseSelf>();
        m_houseBasic = m_houseBasic ?? FindObjectOfType<SmallHouseOther>();
        m_houseBasic = m_houseBasic ?? FindObjectOfType<BigHouseSelf>();
        m_houseBasic = m_houseBasic ?? FindObjectOfType<BigHouseOther>();
        if (m_houseBasic == null)
        {
            Debug.LogError("Got error in finding house basic object.");
            return;
        }

        m_houseBasic.m_HouseRootManager = this;
        TenementData.Instance.m_HouseBasic = m_houseBasic;

        //load and set main city ui, house model, house player.
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MAINCITY_MAINUI), OnMainCityUILoadCallBack);

        //Send character sync message to server.
        PlayerState t_state = new PlayerState();
        t_state.s_state = State.State_HOUSE;
        UtilityTool.SendQXMessage(t_state, ProtoIndexes.PLAYER_STATE_REPORT);

        //Show new scene title.
        if (m_houseBasic.m_HouseSimpleInfo.jzId == JunZhuData.Instance().m_junzhuInfo.id)
        {
            SceneGuideManager.Instance().ShowSceneGuide(1030001, "您已回到自己的房屋" + FangWuInfoTemplate.GetNameById(m_houseBasic.m_HouseSimpleInfo.locationId));
        }
        else
        {
            SceneGuideManager.Instance().ShowSceneGuide(1030001, "您已进入" + m_houseBasic.m_HouseSimpleInfo.jzName + "的房屋" + FangWuInfoTemplate.GetNameById(m_houseBasic.m_HouseSimpleInfo.locationId));
        }
    }

    private void Awake()
    {
        m_HouseRootManager = this;
    }
}
