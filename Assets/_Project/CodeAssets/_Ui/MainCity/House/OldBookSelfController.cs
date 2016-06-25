using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;
using Object = UnityEngine.Object;

/// <summary>
/// old book list controller in house sys.
/// </summary>
public class OldBookSelfController : MonoBehaviour
{
    public UISprite m_Effectsprite;
    public UISprite m_RedAlertSprite;

    public OldBookWindow m_OldBookWindow;

    public List<GameObject> IconSampleParentList;

    [HideInInspector]
    public List<IconSampleManager> IconSampleManagerList = new List<IconSampleManager>();
    [HideInInspector]
    public GameObject IconSamplePrefab;

    public UILabel OldBookInfoLabel;
    public UISprite BoxSprite;
    public UIEventListener BoxListener;

    public NGUILongPress LongPressController;

    public UISprite OldBookLogoSprite;

    [HideInInspector]
    public int WholeBookItemId;
    [HideInInspector]
    public List<int> NumList = new List<int>();
    [HideInInspector]
    public List<int> BagItemIdList = new List<int>();

    private List<BagItem> bagItemList = new List<BagItem>();

    //box sprite light and dark sprite name
    private const string LightBoxStr = "TreasureBox";
    private const string DarkBoxStr = "TreasureBox_D";

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
        //ergodic book fragment icon list.
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

            //set book fragment data.
            TransformHelper.ActiveWithStandardize(IconSampleParentList[i].transform, iconSampleObject.transform);
            var itemManager = iconSampleObject.GetComponent<OldBookItemManager>() ??
                              iconSampleObject.AddComponent<OldBookItemManager>();
            itemManager.BoxId = 0;
            itemManager.ItemId = BagItemIdList[i];

            //set book fragment click delegate.
            var m_IconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
            m_IconSampleManager.SetIconByID(IconSampleManager.IconType.item, BagItemIdList[i], (NumList[i] > 0 ? NumList[i].ToString() : (ColorTool.Color_Red_c40000 + NumList[i] + "[-]")) + "/1", 5);
            if (NumList[i] > 0)
            {
                m_IconSampleManager.SetIconBasicDelegate(false, true, OnOldBookSelfClick);
            }
            else
            {
                m_IconSampleManager.SetIconBasicDelegate();
            }

            IconSampleManagerList.Add(m_IconSampleManager);
        }

        SetBox();
    }

    /// <summary>
    /// add fragment to exchange box
    /// </summary>
    /// <param name="go"></param>
    private void OnOldBookSelfClick(GameObject go)
    {
        var itemManager = go.GetComponent<OldBookItemManager>();

        setHuanWu temp = new setHuanWu
        {
            code = 10,
            boxIdx = 0,
            itemId = itemManager.ItemId
        };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_HUAN_WU_OPER);
    }

    public void SetBox()
    {
        BoxSprite.spriteName = NumList.Contains(0) ? DarkBoxStr : LightBoxStr;
        //		UI3DEffectTool.ClearUIFx(BoxSprite.gameObject);
        m_Effectsprite.gameObject.SetActive(false);

        //			UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,BoxSprite.gameObject,EffectIdTemplate.GetPathByeffectId(100179));
        m_Effectsprite.gameObject.SetActive(!NumList.Contains(0));
        m_RedAlertSprite.gameObject.SetActive(!NumList.Contains(0));

        // 特效在此添加100179  
    }

    private void ActiveTips(GameObject go)
    {
        ShowTip.showTip(WholeBookItemId);
    }

    private void DeactiveTips(GameObject go)
    {
        ShowTip.close();
    }

    /// <summary>
    /// combine old book
    /// </summary>
    /// <param name="go"></param>
    void OnBoxClick(GameObject go)
    {
        //can't combine cause not enough fragment.
        if (NumList.Contains(0))
        {
            Global.CreateFunctionIcon(1401);
            return;
        }

        //combine book.
        if (BagItemIdList[0].ToString().Length < 3)
        {
            Debug.LogError("Bag item id not correct:" + BagItemIdList[0]);
            return;
        }

        m_OldBookWindow.m_CombineBagItemList = bagItemList;

        combineIndex = int.Parse(BagItemIdList[0].ToString().Substring(2, 1));
        ExCanJuanJiangLi temp = new ExCanJuanJiangLi
        {
            code = combineIndex
        };
        SocketHelper.SendQXMessage(temp, ProtoIndexes.C_EX_CAN_JUAN_JIANG_LI);
    }

    /// <summary>
    /// combine book fail for not enough fragment
    /// </summary>
    /// <param name="www"></param>
    /// <param name="path"></param>
    /// <param name="loadedObject"></param>
    [Obsolete]
    private void OnCombineFailLoadCallBack(ref WWW www, string path, Object loadedObject)
    {
        UIBox uibox = (Instantiate(loadedObject) as GameObject).GetComponent<UIBox>();
        uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;
        uibox.setBox("缺少残简",
            LanguageTemplate.GetText(LanguageTemplate.Text.OLD_BOOK_NOT_ENOUGH1), null,
            bagItemList,
            LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM), null,
            null, null, null, null, true, true, false);
        //Set smaller font size to reveal icon.
        uibox.m_labelDis1.fontSize = 23;
    }

    private int combineIndex;

    void OnEnable()
    {
        BoxListener.onClick = OnBoxClick;
    }

    void OnDisable()
    {
        BoxListener.onClick = null;
    }

    void Start()
    {
        //Calc combine succeed show bag item.
        int awardType = int.Parse(BagItemIdList[0].ToString().Substring(2, 1)) + 10;
        var jiangLiItem = JiangLiTemplate.templates.Where(item => item.awardType == awardType).FirstOrDefault();
        var splitedBagItem = jiangLiItem.item.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
        bagItemList.Clear();
        foreach (var bagItem in splitedBagItem)
        {
            var splitedBagItemVar = bagItem.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            bagItemList.Add(new BagItem() { itemId = int.Parse(splitedBagItemVar[1]), cnt = int.Parse(splitedBagItemVar[2]) });
        }

        //Init long press tips.
        LongPressController.LongTriggerType = NGUILongPress.TriggerType.Press;
        LongPressController.OnLongPress = ActiveTips;
        LongPressController.OnLongPressFinish = DeactiveTips;
    }
}
