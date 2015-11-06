using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FightTypeSelectManagerment : MonoBehaviour
{
 
    public List<EventIndexHandle> listEvent = new List<EventIndexHandle>();
    public List<UILabel> ListDes = new List<UILabel>();
    public List<UILabel> ListLevel = new List<UILabel>();
    public List<GameObject> listGameObjectLevel = new List<GameObject>();

    public GameObject m_DestroyTarget;
    public GameObject m_BaiZhanQianJunTanHao;
    void Start()
    {
       // m_Texture = new Texture2D(30, 30);
    
        listEvent.ForEach(p => p.m_Handle += TouChEvent);
        ShowInfo();
    }

	public void EnterPveTest()
	{
		CityGlobalData.m_tempSection = 5;

		CityGlobalData.m_tempLevel = 9;

		EnterBattleField.EnterBattlePveDebug ();
	}

    void ShowInfo()
    {

        if (FreshGuide.Instance().IsActive(100020) && TaskData.Instance.m_iCurMissionIndex == 100020 && TaskData.Instance.m_TaskInfoDic[100020].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100020;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100030) && TaskData.Instance.m_iCurMissionIndex == 100030 && TaskData.Instance.m_TaskInfoDic[100030].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100030;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100060) && TaskData.Instance.m_iCurMissionIndex == 100060 && TaskData.Instance.m_TaskInfoDic[100060].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100060;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100070) && TaskData.Instance.m_iCurMissionIndex == 100070 && TaskData.Instance.m_TaskInfoDic[100070].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100070;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100090) && TaskData.Instance.m_iCurMissionIndex == 100090 && TaskData.Instance.m_TaskInfoDic[100090].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100090;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100120) && TaskData.Instance.m_iCurMissionIndex == 100120 && TaskData.Instance.m_TaskInfoDic[100120].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100120;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100130) && TaskData.Instance.m_iCurMissionIndex == 100130 && TaskData.Instance.m_TaskInfoDic[100130].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100130;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100170) && TaskData.Instance.m_iCurMissionIndex == 100170 && TaskData.Instance.m_TaskInfoDic[100170].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100170;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_iCurMissionIndex == 100180 && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100180;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 2;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100190) && TaskData.Instance.m_iCurMissionIndex == 100190 && TaskData.Instance.m_TaskInfoDic[100190].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100190;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100200) && TaskData.Instance.m_iCurMissionIndex == 100200 && TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100200;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100230) && TaskData.Instance.m_iCurMissionIndex == 100230 && TaskData.Instance.m_TaskInfoDic[100230].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100230;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100240) && TaskData.Instance.m_iCurMissionIndex == 100240 && TaskData.Instance.m_TaskInfoDic[100240].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100240;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100280) && TaskData.Instance.m_iCurMissionIndex == 100280 && TaskData.Instance.m_TaskInfoDic[100280].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100280;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else if (FreshGuide.Instance().IsActive(100340) && TaskData.Instance.m_iCurMissionIndex == 100340 && TaskData.Instance.m_TaskInfoDic[100340].progress >= 0)
        {
            TaskData.Instance.m_iCurMissionIndex = 100340;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
            tempTaskData.m_iCurIndex = 1;
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
        }
        else
        {
            CityGlobalData.m_isRightGuide = true;
        }

        ListDes[0].text = DescIdTemplate.GetDescriptionById(501);
        ListDes[1].text = DescIdTemplate.GetDescriptionById(502);
        ListDes[2].text = DescIdTemplate.GetDescriptionById(503);
        m_BaiZhanQianJunTanHao.SetActive(PushAndNotificationHelper.IsShowRedSpotNotification(300100));
        // if (FunctionOpenTemp.GetWhetherContainID(300000))
        {
            listGameObjectLevel[0].SetActive(false);
        }
        //else
        //{
        //    if (FunctionOpenTemp.GetMissionIdById(300000) > 0)
        //    {
        //        ListLevel[0].text = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(300000));
        //    }

        //    listEvent[0].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        //}

        //Debug.Log("FunctionOpenTemp.GetTemplateById(300100).Level ::" + FunctionOpenTemp.GetTemplateById(300100).Level);
        if (FunctionOpenTemp.GetWhetherContainID(300100))
        {
            listGameObjectLevel[1].SetActive(false);
        }
        else
        {
            //Debug.Log("(FunctionOpenTemp.GetMissionIdById(300100)(FunctionOpenTemp.GetMissionIdById(300100)" + FunctionOpenTemp.GetMissionIdById(300100));
            //Debug.Log("ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(300100)) " + ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(300100)));
            ListLevel[1].text = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(300100));//*FunctionOpenTemp.GetTemplateById(300100).Level.ToString()*/ "10" + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
            listEvent[1].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
        }

        //Debug.Log("FunctionOpenTemp.GetTemplateById(300100).Level ::" + FunctionOpenTemp.GetTemplateById(300200).Level);
        if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300200).Level && JunZhuData.Instance().m_junzhuInfo.lianMengId > 0 && AllianceData.Instance.g_UnionInfo.level >= 2)
        {
            listGameObjectLevel[2].SetActive(false);
        }
        else
        {
            if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
            {
                listEvent[2].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                ListLevel[2].text = NameIdTemplate.GetName_By_NameId(990050) + "2" + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
            }
            else
            {
                if (JunZhuData.Instance().m_junzhuInfo.level < FunctionOpenTemp.GetTemplateById(300200).Level)
                {
                    listEvent[2].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                    ListLevel[2].text = FunctionOpenTemp.GetTemplateById(300200).Level.ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
                }
                else
                {
                    if (JunZhuData.Instance().m_junzhuInfo.lianMengId > 0 && AllianceData.Instance.g_UnionInfo.level >= 2)
                    {

                    }
                    else
                    {
                        listEvent[2].GetComponent<ButtonColorManagerment>().ButtonsControl(false);
                        ListLevel[2].text = NameIdTemplate.GetName_By_NameId(990050) + "2" + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
                    }
                }
            }
        }
    }
    private GameObject SendObj;
   

    public void TouChEvent(int index)
    {
        switch (index)
        {
            case 0:
                {
                   
                    if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300000).Level)
                    {
				        CityGlobalData .PT_Or_CQ = true;
                        EnterGuoGuanmap.EnterPveUI(-1);
				        MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
                        Destroy(m_DestroyTarget);
                    }


                }
                break;
            case 1:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300100).Level)
                    {
                        if (UIYindao.m_UIYindao.m_isOpenYindao)
                        {
                            if (TaskData.Instance.m_iCurMissionIndex == 100180 && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
                            {
                                TaskData.Instance.m_iCurMissionIndex = 100180;
                                ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
                                UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
                            }
                            else
                            {
                                CityGlobalData.m_isRightGuide = true;
                            }


                        }
                        BaiZhanWindow();
				        MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
                        Destroy(m_DestroyTarget);
                    }
                    else
                    {
                        //m_TagShow.SetActive(true);
                        //m_TagInfoShow.text = NameIdTemplate.GetName_By_NameId(990004) + FunctionOpenTemp.GetTemplateById(102).Level.ToString() + NameIdTemplate.GetName_By_NameId(990019) + NameIdTemplate.GetName_By_NameId(990044);
                        //StartCoroutine(WaitFor());
                    }
                }
                break;
            case 2:
                {
                    if (JunZhuData.Instance().m_junzhuInfo.level >= FunctionOpenTemp.GetTemplateById(300200).Level && AllianceData.Instance.g_UnionInfo.level >= 2)
                    {
				      
                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HY_MAP), LoadHY_Map);
				        MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
						Destroy(m_DestroyTarget);
                    }
                    else
                    {
                        //   m_TagShow.SetActive(true);
                        //  StartCoroutine(WaitFor());
                    }
                }
                break;
            case 3:
                {
                    MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
                    Destroy(m_DestroyTarget);
                }
                break;
            default:
                break;
        }

    }

    //IEnumerator WaitFor()
    //{
    //    yield return new WaitForSeconds(0.8f);
    //    m_TagShow.SetActive(false);
    //}
    private int m_chosen_chapter = -1;

    void EnterPveUI(int p_chapter)
    {
        m_chosen_chapter = p_chapter;

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVE_MAP_PREFIX),
                ResLoaded);
    }

    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);

        //		Debug.Log ("返回次数。。。。");

        tempObject.transform.position = new Vector3(0, 500, 0);

        MapData mMapinfo = tempObject.GetComponent<MapData>();

        //		Debug.Log("========1");

        mMapinfo.startsendinfo(m_chosen_chapter);
		MainCityUI.TryAddToObjectList (tempObject);
		MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
        Destroy(m_DestroyTarget);
    }

    void BaiZhanWindow()
    {
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_BAI_ZHAN),
                                BaiZhanLoadCallback);
    }

    public void BaiZhanLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject baizhanRoot = Instantiate(p_object) as GameObject;

        baizhanRoot.SetActive(true);

        baizhanRoot.name = "BaiZhan";

        baizhanRoot.transform.localPosition = new Vector3(0, 800, 0);

        baizhanRoot.transform.localScale = Vector3.one;

		MainCityUI.TryAddToObjectList (baizhanRoot);
		MainCityUI.TryRemoveFromObjectList(m_DestroyTarget);
        Destroy(m_DestroyTarget);

	}
    public void LoadHY_Map(ref WWW p_www, string p_path, Object p_object)
    {
    //    if (HY_Map)
    //    {
    //        return;
    //    }
        GameObject HY_Map = Instantiate(p_object) as GameObject;
     
        HY_Map.transform.localPosition = new Vector3(-200, 200, 0);

        HY_Map.transform.localScale = Vector3.one;

  
		Global.m_isOpenHuangYe = true;

		MainCityUI.TryAddToObjectList(HY_Map);
		HY_UIManager mWildnessManager = HY_Map.GetComponent<HY_UIManager>(); 
		mWildnessManager.init ();
    }
}
