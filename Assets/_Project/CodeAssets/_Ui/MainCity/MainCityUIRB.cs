using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

/// <summary>
/// 主城UI界面右下角功能开启按钮管理类
/// </summary>
public class MainCityUIRB : MYNGUIPanel
{
    public UILabel m_FuncNotOpenInfoLabel;
    public GameObject m_FuncNotOpenInfoObject;
	public static bool isOpen = true;
	public UISprite m_SpriteOpen;
	public GameObject m_ObjPropUse;
	public IconSampleManager m_IconSampleManager;
	public UILabel m_UILabelPropName;
	public UILabel m_UILabelPropUse;
	public List<int> m_listPropUseID = new List<int>();
	public List<int> m_listPropUseNum = new List<int>();

    public enum ButtonType
    {
        Fast,
        Navigation,
        Stick
    }

    private bool isShowBtnEffect;

    public bool IsInitialized = false;

	void Awake()
	{
		m_Panel = GetComponent<UIPanel>();
		if (UIYindao.m_UIYindao != null)
		{
			m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();
		}
	}

	void Update()
	{
		while(m_listStaticPropUseID.Count > 0)
		{
			setPropUse(m_listStaticPropUseID[0], m_listStaticPropUseNum[0]);
			m_listStaticPropUseID.RemoveAt(0);
			m_listStaticPropUseNum.RemoveAt(0);
		}
		while(m_listStaticPropDeleteID.Count > 0)
		{
			deleteProp(m_listStaticPropDeleteID[0]);
			m_listStaticPropDeleteID.RemoveAt(0);
		}
	}

    /// <summary>
    /// Add/Remove a button to/from button list.
    /// </summary>
    /// <param name="index"></param>
    private void AddButton(int index, bool isAdd = true)
    {
//        FunctionOpenTemp template = FunctionOpenTemp.GetTemplateById(index);
//        if (isAdd)
//        {
//            float scale = (template.type == 4 || template.type == 5) ? 0.75f : 1.0f;
//
//            var tempObject = Instantiate(ButtonPrefab) as GameObject;
//            tempObject.transform.name = "button" + index;
//            var tempButtonManager = tempObject.GetComponent<FunctionButtonManager>();
//            tempButtonManager.SetData(template);
////            tempButtonManager.m_OnFuncBtnClick = InnerButtonTrigger;
//
//            GetButtonListByType(template.type - 1).Add(tempButtonManager);
//
//            TransformHelper.ActiveWithStandardize(GetButtonParentByType(template.type - 1).transform, tempObject.transform);
//            tempObject.transform.localScale = Vector3.one * scale;
//        }
//        else
//        {
//            var tempList = m_totalButtonManagerlist.Where(item => item.m_index == index).ToList();
//
//            if (tempList != null && tempList.Count() == 1)
//            {
//                GetButtonListByType(template.type - 1).Remove(tempList[0]);
//                Destroy(tempList[0].gameObject);
//            }
//        }
    }
	
    private const float rankGap = 90;

    private const int fastColumnNum = 4;
    private const int navigationColumnNum = 2;


    /// <summary>
    /// Execute on gohome btn click
    /// </summary>
    



    private void LoadYouXiaBack(ref WWW www, string path, Object loadedObject)
    {
        GameObject tempObject = Instantiate(loadedObject) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);

        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            CityGlobalData.m_isRightGuide = true;
        }

    }

    public delegate void MainCityUIRBDelegate();
    public static MainCityUIRBDelegate m_MainCityUIRBDelegate;

    [HideInInspector]
    public UIPanel m_Panel;
    [HideInInspector]
    public UIPanel m_YinDaoPanel;
    public static bool IsCanClickButtons = true;

    public class RedAlertCommand
    {
        public int index;
        public bool IsShow;
        public bool m_manual_set;
    }
    public static List<RedAlertCommand> m_RedAlertCommandList = new List<RedAlertCommand>();

    private const float FadeInOutDuration = 1.0f;
    private bool isYinDaoShow;
    private float panelYinDaoFadeSpeed;

    /// <summary>
    /// Set MainCity UI panel.
    /// </summary>
    /// <param name="isShow"></param>
    public void SetPanel(bool isShow)
    {
//        iTween.ValueTo(gameObject, iTween.Hash(
//            "from", m_Panel.alpha,
//                    "to", isShow ? 1.0f : 0.0f,
//                    "time", FadeInOutDuration,
//                    "easetype", "linear",
//                    "onupdate", "UpdatePanelA"));
//
//        StopCoroutine("UpdateYinDaoPanelA");
//
//        if (UIYindao.m_UIYindao == null)
//        {
//            return;
//        }
//        m_YinDaoPanel = UIYindao.m_UIYindao.GetComponentInChildren<UIPanel>();
//
//        isYinDaoShow = isShow;
//        panelYinDaoFadeSpeed = Math.Abs(m_YinDaoPanel.alpha - (isShow ? 1.0f : 0.0f)) * (1 / FadeInOutDuration);
//        StartCoroutine("UpdateYinDaoPanelA");
    }

    private void UpdatePanelA(float a)
    {
        m_Panel.alpha = a;
    }

    private IEnumerator UpdateYinDaoPanelA()
    {
        while (m_YinDaoPanel.alpha >= 0 && m_YinDaoPanel.alpha <= 1)
        {
            if (isYinDaoShow)
            {
                m_YinDaoPanel.alpha += panelYinDaoFadeSpeed * Time.deltaTime;
            }
            else
            {
                m_YinDaoPanel.alpha -= panelYinDaoFadeSpeed * Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private static void ExecuteStoredRedAlertCommand()
    {
        m_MainCityUIRBDelegate -= ExecuteStoredRedAlertCommand;

        foreach (var item in m_RedAlertCommandList)
        {
            MainCityUI.SetRedAlert(item.index, item.IsShow, item.m_manual_set);
        }
        m_RedAlertCommandList.Clear();
    }

    /// <summary>
    /// Set main city ui RB button's red alert active or deactive.
    /// [WARNING] Note that calling of zaixian activity and xianshi activity's red alert will show particle effect.
    /// </summary>
    /// <param name="id">function id, reference in FunctionOpen.xml</param>
    /// <param name="isShow">active or deactive, true: active, false: deactive</param>
    /// <returns>set successfully or not, true: succeed, false: fail</returns>
    /// p_manual_set: if it's called by old red spot sys.

    public static bool LockRedAlert(int id, bool isRemove = false)
    {
        try
        {
            //Lock red alert state that cannot be setted.
            if (isRemove)
            {
                if (FunctionButtonManager.s_LockedList.Contains(id)) FunctionButtonManager.s_LockedList.Remove(id);
            }
            else
            {
                if (!FunctionButtonManager.s_LockedList.Contains(id)) FunctionButtonManager.s_LockedList.Add(id);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Got exception in LockRedAlert, ex:" + ex.Message + ", \nstackTrace:" + ex.StackTrace);
            return false;
        }
    }

    private const float MovingDuration = 0.5f;

    private bool isNoiTween = false;

    private void OnCloseEndDelegate()
    {
        MainCityUI.m_MainCityUI.setInit();
    }

    private void CheckActiveSlideRedAlert()
    {

    }
	
    public GameObject TimeCalcRoot;
    public UILabel TimeCalcLabel;

    /// <summary>
    /// Show time calculation in online activity.
    /// </summary>
    /// <param name="second">time measured by seconds</param>
    public static void ShowTimeCalc(int second)
    {
        if (MainCityUI.m_MainCityUI != null)
        {
            MainCityUI.m_MainCityUI.m_MainCityUIRB.DoShowTimeCalc(second);
        }
    }

    private int TotalTimeCalcSecond;

    void DoShowTimeCalc(int second)
    {
        TotalTimeCalcSecond = second;

//        var tempList = m_middleStickButtonManagerlist.Where(item => item.m_index == 15).ToList();
//        if (tempList == null || tempList.Count != 1)
//        {
//            return;
//        }

//        TimeCalcRoot.transform.position = tempList[0].transform.position;
//        TimeCalcRoot.transform.localScale = tempList[0].transform.localScale;
        TimeCalcRoot.gameObject.SetActive(true);

        if (TimeHelper.Instance.IsTimeCalcKeyExist("MainCityUIRBTimeCalc"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");
        }
        TimeHelper.Instance.AddEveryDelegateToTimeCalc("MainCityUIRBTimeCalc", second, RefreshTimeCalc);

        //hide red alert in zaixian activity
		MainCityUI.SetRedAlert(15, false);
    }

    void RefreshTimeCalc(int second)
    {
        if (TotalTimeCalcSecond - second > 0)
        {
            TimeCalcLabel.text = ColorTool.Color_Green_00ff00 + TimeHelper.SecondToClockTime(TotalTimeCalcSecond - second) + "[-]";
        }
        else
        {
			TimeCalcLabel.text = ColorTool.Color_Green_00ff00 + "领取[-]";
//            TimeCalcRoot.gameObject.SetActive(false);
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");

            //Show red alert in zaixian activity
            MainCityUI.SetRedAlert(15, true);
        }
    }

    private int PlayAddButtonID;
    private GameObject PlayAddButtonObject;
	public static List<int> m_listStaticPropUseID = new List<int>();
	public static List<int> m_listStaticPropUseNum = new List<int>();
	public static List<int> m_listStaticPropDeleteID = new List<int>();
	public static void setSavePropUse(int id, int num = 0)
	{
		m_listStaticPropUseID.Add(id);
		m_listStaticPropUseNum.Add(num);
	}
	public static void setDeletePropUse(int id)
	{
		m_listStaticPropDeleteID.Add(id);
	}
	public void setPropUse(int id, int num = 0)
	{
//		Debug.Log("setPropUse id=" + id);
		bool tempAdd = true;
		for(int i = 0; i < m_listPropUseID.Count; i ++)
		{
			if(m_listPropUseID.Contains(id))
			{
				tempAdd = false;
				break;
			}
		}
		if(tempAdd)
		{
			m_listPropUseID.Add(id);
			m_listPropUseNum.Add(num);
		}
		if(!m_ObjPropUse.activeSelf)
		{
			m_ObjPropUse.SetActive(true);
//			if(MainCityUIRB.isOpen)
//			{
				m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);
//			}
//			else
//			{
//				m_ObjPropUse.transform.localPosition = new Vector3(-190, 100, 0);
//			}
			if(num == 0)
			{
				m_IconSampleManager.SetIconByID(id);
			}
			else
			{
				m_IconSampleManager.SetIconByID(id, "x"+num);
			}
			m_IconSampleManager.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
			CommonItemTemplate tempCommon = CommonItemTemplate.getCommonItemTemplateById(id);
			m_UILabelPropName.text = NameIdTemplate.GetName_By_NameId (tempCommon.nameId);
			if(tempCommon.itemType == 2)
			{
				m_UILabelPropUse.text = "点击穿戴";
			}
			else
			{
				m_UILabelPropUse.text = "点击使用";
			}
		}
//
//		int typeID = .itemType;
//
//		NameIdTemplate.GetName_By_NameId (commonItemTemplate.nameId) + "x" + _item.num;
	}

	public void deleteProp(int id)
	{
		m_ObjPropUse.SetActive(false);
		for(int i = 0; i < m_listPropUseID.Count; i ++)
		{
			if(m_listPropUseID[i] == id)
			{
				m_listPropUseID.RemoveAt(i);
				m_listPropUseNum.RemoveAt(i);
			}
		}
		if(m_listPropUseID.Count > 0)
		{
			setPropUse(m_listPropUseID[0], m_listPropUseNum[0]);
		}
	}

    void OnDisable()
    {
        if (UtilityTool.m_ApplicationIsQuitting) return;

        if (TimeHelper.Instance.IsTimeCalcKeyExist("MainCityUIRBTimeCalc"))
        {
            TimeHelper.Instance.RemoveFromTimeCalc("MainCityUIRBTimeCalc");
        }
    }

	public void EndRB()
	{
//		if(!isOpen)
//		{
//			ClientMain.addPopUP(40, 2, "6", null);
//			ClientMain.addPopUP(40, 2, "12", null);
//			ClientMain.addPopUP(40, 2, "1210", null);
//			ClientMain.addPopUP(40, 2, "250", null);
//		}
		if(!MainCityUIRB.isOpen)
		{
			QXChatUIBox.chatUIBox.SetChatUIBoxPos(false);
		}
		if(m_ObjPropUse.activeSelf)
		{
			if(!MainCityUIRB.isOpen)
			{
				m_ObjPropUse.transform.localPosition = new Vector3(-190, 50, 0);

			}
//			else
//			{
//				m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);
//			}
		}
		MainCityUI.m_MainCityUI.setInit();
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("RB_OpenButton") != -1)
		{
			isOpen = !isOpen;
			if(isOpen)
			{
				m_SpriteOpen.spriteName = "Open";
				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 150, -(640 + ClientMain.m_iMoveY * 2 - 45), -75, 0);
				QXChatUIBox.chatUIBox.SetChatUIBoxPos(true);
				if(m_ObjPropUse.activeSelf)
				{
					m_ObjPropUse.transform.localPosition = new Vector3(-190, 150, 0);

//					if(MainCityUIRB.isOpen)
//					{
//						m_ObjPropUse.transform.localPosition = new Vector3(-190, 50, 0);
//					}
//					else
//					{
//
//					}
				}
				if(UIYindao.m_UIYindao.m_isOpenYindao && (UIYindao.m_UIYindao.m_iCurId == 603 || UIYindao.m_UIYindao.m_iCurId == 6030))
				{
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}
			}
			else
			{
				m_SpriteOpen.spriteName = "Close";
//				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 50 + 150, -(640 + ClientMain.m_iMoveY * 2 - 50), 100, 0);
				MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setBPos(960 + ClientMain.m_iMoveX * 2 - 50 + (MainCityUI.m_MainCityUI.m_MainCityListButton_RB.m_listFunctionButtonManager.Count * 75), -(640 + ClientMain.m_iMoveY * 2 - 45), -75, 0);
			}
			MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setPos();
			MainCityUI.m_MainCityUI.m_MainCityListButton_RB.setMove(EndRB);
		}
		else if(ui.name.IndexOf("RB_UseProp") != -1)
		{
			m_ObjPropUse.SetActive(false);
			UseProp(m_listPropUseID[0]);
			m_listPropUseID.RemoveAt(0);
			m_listPropUseNum.RemoveAt(0);
			if(m_listPropUseID.Count > 0)
			{
				setPropUse(m_listPropUseID[0], m_listPropUseNum[0]);
			}
		}
	}

	public void UseProp(int id)
	{
		List<BagItem> bagItemList = BagData.Instance().m_bagItemList;
		for(int i = 0; i < bagItemList.Count; i ++)
		{
			if (bagItemList[i].itemId == id)
			{
				Global.ScendID(ProtoIndexes.C_EquipAdd, bagItemList[i].bagIndex);
				break;
			}
		}
	}

	public override void MYMouseOver(GameObject ui)
	{
	}
	
	public override void MYMouseOut(GameObject ui)
	{
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
	}
	
	public override void MYelease(GameObject ui)
	{
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
	}
}