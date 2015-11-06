//debug duplicate set, may find possible error.
//#define DEBUG_DUPLICATE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Icon sample handler, only for item icon now.
/// </summary>
public class IconSampleManager : MonoBehaviour
{
    private bool isSetted = false;

    public static int TransferOldQualityID(int oldID)
    {
        switch (oldID)
        {
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 4;
            case 4:
                return 7;
            case 5:
                return 10;
            default:
                Debug.LogError("Error old quality id.");
                return -1;
        }
    }

    #region Icon Components

    /// <summary>
    /// button effect component
    /// </summary>
    public UIButton ButtonEffect;
    /// <summary>
    /// button effect component
    /// </summary>
    public UISprite FirstWinSpr;
    /// <summary>
    /// button scale effect component
    /// </summary>
    public UIButtonScale ButtonScaleEffect;

    public UISprite BgSprite;

    /// <summary>
    /// icon sprite
    /// </summary>
    public UISprite FgSprite;

    /// <summary>
    /// quality frame sprite
    /// </summary>
    public UISprite QualityFrameSprite;

    /// <summary>
    /// frame to show icon selected or not
    /// </summary>
    public UISprite SelectFrameSprite;

    /// <summary>
    /// icon description label
    /// </summary>
    public UILabel RightButtomCornorLabel;

    public GameObject AddButton;
    public GameObject UpgradeButton;
    public GameObject SubButton;

    /// <summary>
    /// icon done label, control active and set text manually.
    /// </summary>
    public UILabel DoneLabel;

    /// <summary>
    /// Pop frame object, automaticlly setted by SetIcon method, DO NOT call setactive directly.
    /// </summary>
    public UISprite PopFrameSprite;

    /// <summary>
    /// left top cornor sprite.
    /// </summary>
    public UISprite LeftTopCornorSprite;

    /// <summary>
    /// left top cornor sprite.
    /// </summary>
    public UISprite RightButtomCornorSprite;

    /// <summary>
    /// Right top cornor sprite.
    /// </summary>
    public UISprite RightTopCornorSprite;

    /// <summary>
    /// Buttom sprite.
    /// </summary>
    public UISprite ButtomSprite;

    /// <summary>
    /// Middle sprite
    /// </summary>
    public UISprite MiddleSprite;

    /// <summary>
    /// Progress bar in the buttom.
    /// </summary>
    public UIProgressBar ButtomProgressBar;

    /// <summary>
    /// pop frame label, set this by SetIcon method.
    /// </summary>
    public UILabel PopTextLabel;

    public UISprite DimmerSprite;

    /// <summary>
    /// ngui normal, long, longfinish press contianer
    /// </summary>
    public NGUILongPress NguiLongPress;

	public UILabel AwardNum;//number of awards
    #endregion

    #region Used atlas

    public UIAtlas EquipAtlas;
    public UIAtlas ComAtlas_0;

    /// <summary>
    /// MiBao atlas
    /// </summary>
    public UIAtlas MibaoLitterAtlas;

    /// <summary>
    /// MiBao fragment atlas
    /// </summary>
    public UIAtlas MibaoSuiPianAtlas;
    public UIAtlas EnemyIcon;
    public UIAtlas CarriageAtlas;

    /// <summary>
    /// Collect scrap sprites to a atlas.
    /// </summary>
    public UIAtlas IconDecoAtlas;

    /// <summary>
    /// The fu wen atlas.
    /// </summary>
    public UIAtlas FuWenAtlas;

    #endregion

    #region EventDelegates

    /// <summary>
    /// Add icon long press finish event.
    /// </summary>
    private UIEventListener.VoidDelegate OnLongPressFinish
    {
        get { return NguiLongPress.OnLongPressFinish; }
        set { NguiLongPress.OnLongPressFinish = value; }
    }

    /// <summary>
    /// Add icon long press event.
    /// </summary>
    private UIEventListener.VoidDelegate OnLongPress
    {
        get { return NguiLongPress.OnLongPress; }
        set { NguiLongPress.OnLongPress = value; }
    }

    /// <summary>
    /// Add icon normal press event.
    /// </summary>
    private UIEventListener.VoidDelegate OnNormalPress
    {
        get { return NguiLongPress.OnNormalPress; }
        set { NguiLongPress.OnNormalPress = value; }
    }

    /// <summary>
    /// Add button event.
    /// </summary>
    public UIEventListener AddLis;

    /// <summary>
    /// Sub button event.
    /// </summary>
    public UIEventListener SubLis;

    /// <summary>
    /// Upgrade button event.
    /// </summary>
    public UIEventListener UpgradeLis;

    #endregion

    [HideInInspector]
    public enum IconType
    {
        null_type,
        item,
        equipment,
        livingWild,
        pveHeroAtlas,
        pveItemAtlas,
        oldMiBao,
        exchangeBox,
        mainCityAtlas,
        MiBao,
        MiBaoSuiPian,
        Carriage,
        HuangyeMonster,
        FuWen,
    }
    private IconType m_type;

    public const string QualityPrefix = "pinzhi";
    private int popTextMode;

    private const string YellowSelectedSpriteName = "CheckBox";

    /// <summary>
    /// Set icon by id, replace SetIconType and SetIconBasic.
    /// </summary>
    /// <param name="id">icon id</param>
    /// <param name="labelText">icon label text</param>
    /// <param name="basicDepth">basic depth set for every widget in icon</param>
    /// <param name="isShowDimmer">is show shield dimmer</param>
    public void SetIconByID(int id, string labelText = "", int basicDepth = 0, bool isShowDimmer = false)
    {
        int typeID = CommonItemTemplate.getCommonItemTemplateById(id).itemType;
        IconType type = new IconType();

        switch (typeID)
        {
            case 2:
                type = IconType.equipment;
                break;
            case 3:
            case 6:
            case 9:
                type = IconType.item;
                break;
            case 4:
                type = IconType.MiBao;
                break;
            case 5:
                type = IconType.MiBaoSuiPian;
                break;
            case 7:
            case 8:
                type = IconType.FuWen;
                break;
            case 0:
                type = IconType.item;
                break;
            default:
                Debug.LogError("Not defined icon type, check or use SetIconType instead.");
                break;
        }

        SetIconByID(type, id, labelText, basicDepth, isShowDimmer);
    }

    /// <summary>
    /// Set icon by id, require specific iconType, replace SetIconType and SetIconBasic.
    /// </summary>
    /// <param name="type">specific icon type</param>
    /// <param name="id">icon id</param>
    /// <param name="labelText">icon label text</param>
    /// <param name="basicDepth">basic depth set for every widget in icon</param>
    /// <param name="isShowDimmer">is show shield dimmer</param>
    [Obsolete("Do not use anymore, use SetIconByID without IconType")]
    public void SetIconByID(IconType type, int id, string labelText = "", int basicDepth = 0, bool isShowDimmer = false)
    {

#if DEBUG_DUPLICATE
        if (isSetted)
        {
            Debug.LogError("Set icon duplicate! Cannot call SetIconBasic and SetIconByID at the same time.");
        }
        isSetted = true;
#endif

        m_type = type;
        string fgSpriteName = "";
        string qualityFrameSpriteName = "";
        switch (m_type)
        {
            case IconType.null_type:
                break;
            case IconType.item:
                {
                    //Set atlas.
                    FgSprite.atlas = EquipAtlas;

                    //Set fgSprite and quality frame.
                    var itemTemp = ItemTemp.getItemTempById(id);
                    if (itemTemp == null)
                    {
                        Debug.LogError("Can't set icon sample cause item:" + id + " not found");
                        return;
                    }

                    fgSpriteName = !string.IsNullOrEmpty(itemTemp.icon) ? itemTemp.icon : "";
                    qualityFrameSpriteName = itemTemp.color != 0
                       ? QualityPrefix + (itemTemp.color - 1)
                       : "";
                    break;
                }
            case IconType.exchangeBox:
                {
                    FgSprite.atlas = EquipAtlas;

                    //Set select frame.
                    SelectFrameSprite.spriteName = YellowSelectedSpriteName;
                    SelectFrameSprite.width = SelectFrameSprite.height = 135;

                    //Set fgSprite and quality frame.
                    var itemTemp = ItemTemp.getItemTempById(id);
                    if (itemTemp == null)
                    {
                        Debug.LogError("Can't set icon sample cause item:" + id + " not found");
                        return;
                    }

                    fgSpriteName = !string.IsNullOrEmpty(itemTemp.icon) ? itemTemp.icon : "";
                    qualityFrameSpriteName = itemTemp.color != 0
                       ? QualityPrefix + (itemTemp.color - 1)
                       : "";
                    break;
                }
            case IconType.equipment:
                //Set atlas.
                FgSprite.atlas = EquipAtlas;

                //Set fgSprite and quality frame.
                var equipmentTemp = ZhuangBei.getZhuangBeiById(id);
                if (equipmentTemp == null)
                {
                    Debug.LogError("Can't set icon sample cause equipment:" + id + " not found");
                    return;
                }

                fgSpriteName = !string.IsNullOrEmpty(equipmentTemp.icon) ? equipmentTemp.icon : "";
                qualityFrameSpriteName = equipmentTemp.color != 0
                    ? QualityPrefix + (equipmentTemp.color - 1)
                    : "";
                break;
            case IconType.MiBao:
                {
                    FgSprite.atlas = MibaoLitterAtlas;
                    fgSpriteName = id.ToString();
                    CommonItemTemplate temp = CommonItemTemplate.getCommonItemTemplateById(id);
                    if (temp != null)
                    {
                        qualityFrameSpriteName = temp.color > 0
                            ? QualityPrefix + temp.color
                            : "";
                    }
                    break;
                }
            case IconType.MiBaoSuiPian:
                {
                    FgSprite.atlas = MibaoSuiPianAtlas;

                    fgSpriteName = id.ToString();

                    CommonItemTemplate temp = CommonItemTemplate.getCommonItemTemplateById(id);
                    if (temp != null)
                    {
                        qualityFrameSpriteName = temp.color > 0
                            ? QualityPrefix + temp.color
                            : "";
                    }
                    break;
                }
            case IconType.FuWen:
                {
                    FgSprite.atlas = FuWenAtlas;

                    //Set fgSprite and quality frame.
                    var temp = CommonItemTemplate.getCommonItemTemplateById(id);
                    if (temp == null)
                    {
                        Debug.LogError("Can't set icon sample cause fu shi:" + id + " not found");
                        return;
                    }

                    fgSpriteName = id.ToString();
                    qualityFrameSpriteName = temp.color != 0 ? QualityPrefix + (temp.color - 1) : "";
                    break;
                }
            default:
                Debug.LogError("Not defined icon type, check or use SetIconType instead.");
                break;
        }

        //Set transform to default.
        gameObject.SetActive(true);
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;

        //Set basic depth.
        if (basicDepth != 0)
        {
            var widgets = gameObject.GetComponentsInChildren<UIWidget>(true);
            foreach (var widget in widgets)
            {
                widget.depth += basicDepth;
            }
        }

        //Set fgSprite.
        if (!string.IsNullOrEmpty(fgSpriteName))
        {
            FgSprite.gameObject.SetActive(true);
            FgSprite.spriteName = fgSpriteName;
        }
        else
        {
            FgSprite.gameObject.SetActive(false);
        }

        //Set quality frame.
        if (!string.IsNullOrEmpty(qualityFrameSpriteName))
        {
            QualityFrameSprite.gameObject.SetActive(true);
            QualityFrameSprite.spriteName = qualityFrameSpriteName;
        }
        else
        {
            QualityFrameSprite.gameObject.SetActive(false);
        }

        //Set label text.
        RightButtomCornorLabel.text = labelText;
        RightButtomCornorLabel.gameObject.SetActive(!string.IsNullOrEmpty(labelText));

        //Set dimmer.
        DimmerSprite.gameObject.SetActive(isShowDimmer);
    }

	public void SetAwardNumber(int num)
	{
		if(num > 0)
		{
			AwardNum.gameObject.SetActive(true);

			AwardNum.text = "x"+num.ToString();
		}

	}

    /// <summary>
    /// Can not call this with SetIconByID at same time.
    /// </summary>
    /// <param name="type">icon type</param>    
    public void SetIconType(IconType type)
    {
        m_type = type;

        switch (m_type)
        {
            case IconType.null_type:
                break;

            case IconType.item:
            case IconType.equipment:
                //Set atlas.
                FgSprite.atlas = EquipAtlas;
                break;

            case IconType.exchangeBox:
                FgSprite.atlas = EquipAtlas;
                //Set select frame.
                SelectFrameSprite.spriteName = YellowSelectedSpriteName;
                SelectFrameSprite.width = SelectFrameSprite.height = 135;
                break;

            case IconType.pveHeroAtlas:
                //Set atlas.
                FgSprite.atlas = EnemyIcon;
                RightButtomCornorSprite.atlas = IconDecoAtlas;
                LeftTopCornorSprite.atlas = IconDecoAtlas;
                break;

            case IconType.oldMiBao:
                //Set atlas.
                FgSprite.atlas = MibaoLitterAtlas;

                ButtomSprite.atlas = IconDecoAtlas;
                ButtomSprite.width = ButtomSprite.height = 20;

                RightButtomCornorSprite.atlas = ComAtlas_0;
                RightButtomCornorSprite.SetDimensions(50, 50);
                RightButtomCornorSprite.transform.localPosition = new Vector3(16, -16, 0);

                BgSprite.atlas = IconDecoAtlas;
                BgSprite.spriteName = "MiBaodikuang";

                break;

            case IconType.mainCityAtlas:
                FgSprite.atlas = IconDecoAtlas;
                break;

            case IconType.MiBao:
                FgSprite.atlas = MibaoLitterAtlas;
                break;

            case IconType.MiBaoSuiPian:
                FgSprite.atlas = MibaoSuiPianAtlas;
                break;

            case IconType.Carriage:
                FgSprite.atlas = CarriageAtlas;
                LeftTopCornorSprite.atlas = IconDecoAtlas;
                LeftTopCornorSprite.width = LeftTopCornorSprite.height = 70;
                LeftTopCornorSprite.transform.localPosition = new Vector3(-25, 25, 0);

                //[FIX] add new icon sample with buttom darken sprite.
                //ButtomSprite.atlas = ComAtlas_0;
                //ButtomSprite.spriteName = "mask_darken";
                //ButtomSprite.transform.localPosition = new Vector3(0, -37, 0);
                //ButtomSprite.SetDimensions(96, 22);
                break;
            case IconType.HuangyeMonster:
                MiddleSprite.atlas = ComAtlas_0;
                break;
            case IconType.FuWen:
                FgSprite.atlas = FuWenAtlas;
                break;

            default:
                Debug.LogError("Not defined icon type.");
                break;
        }
    }

    /// <summary>
    /// Can not call this with SetIconByID at same time.
    /// </summary>
    /// <param name="fgSpriteName">FGSprite name</param>
    /// <param name="labelText">icon label text</param>
    /// <param name="basicDepth">basic depth set for every widget in icon</param>
    /// <param name="qualityFrameSpriteName">quality frame sprite name</param>
    /// <param name="isShowDimmer">is show shield dimmer</param>
    public void SetIconBasic(int basicDepth = 0,
        string fgSpriteName = "", string labelText = "", string qualityFrameSpriteName = "",
        bool isShowDimmer = false)
    {
#if DEBUG_DUPLICATE
        if (isSetted)
        {
            Debug.LogError("Set icon duplicate! Cannot call SetIconBasic and SetIconByID at the same time.");
        }
        isSetted = true;
#endif

        m_iconName = fgSpriteName;//@author LiuChang

        //Set transform to default.
        gameObject.SetActive(true);
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;

        //Set basic depth.
        if (basicDepth != 0)
        {
            var widgets = gameObject.GetComponentsInChildren<UIWidget>(true);
            foreach (var widget in widgets)
            {
                widget.depth += basicDepth;
            }
        }

        //Set fgSprite.
        if (!string.IsNullOrEmpty(fgSpriteName))
        {
            FgSprite.gameObject.SetActive(true);
            FgSprite.spriteName = fgSpriteName;
        }
        else
        {
            FgSprite.gameObject.SetActive(false);
        }

        //Set label text.
        RightButtomCornorLabel.text = labelText;
        RightButtomCornorLabel.gameObject.SetActive(!string.IsNullOrEmpty(labelText));

        //Set quality frame.
        if (!string.IsNullOrEmpty(qualityFrameSpriteName))
        {
            QualityFrameSprite.gameObject.SetActive(true);
            QualityFrameSprite.spriteName = qualityFrameSpriteName;
        }
        else
        {
            QualityFrameSprite.gameObject.SetActive(false);
        }

        //Set dimmer.
        DimmerSprite.gameObject.SetActive(isShowDimmer);
    }

    /// <summary>
    /// Set icon sample cornor deco sprite.
    /// </summary>
    /// <param name="leftTopCornorSpriteName">left top cornor sprite name</param>
    /// <param name="rightButtomCornorSpriteName">right buttom cornor sprite name</param>
    /// <param name="buttomSpriteName">bottom sprite name</param>
    /// <param name="rightTopCornorSpriteName">right top cornor sprite name</param>
    public void SetIconDecoSprite(string leftTopCornorSpriteName = "", string rightButtomCornorSpriteName = "",
        string buttomSpriteName = "",
        string rightTopCornorSpriteName = "")
    {
        //Set decorate sprite.
        LeftTopCornorSprite.spriteName = leftTopCornorSpriteName;
        if (!string.IsNullOrEmpty(leftTopCornorSpriteName))
        {
            LeftTopCornorSprite.gameObject.SetActive(true);
        }

        RightButtomCornorSprite.spriteName = rightButtomCornorSpriteName;
        if (!string.IsNullOrEmpty(rightButtomCornorSpriteName))
        {
            RightButtomCornorSprite.gameObject.SetActive(true);
        }

        ButtomSprite.spriteName = buttomSpriteName;
        if (!string.IsNullOrEmpty(buttomSpriteName))
        {
            ButtomSprite.gameObject.SetActive(true);
        }

        RightTopCornorSprite.spriteName = rightTopCornorSpriteName;
        if (!string.IsNullOrEmpty(rightTopCornorSpriteName))
        {
            RightTopCornorSprite.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Set icon sample normal click and longpress events.
    /// </summary>
    /// <param name="normalDelegate">normal click icon delegate</param>
    /// <param name="isLongPress">enable long press</param>
    /// <param name="isPressType">long press press mode or release mode</param>
    /// <param name="longPressFinishDelegate">long press finish delegate</param>
    /// <param name="longPressDelegate">long press delegate</param>
    public void SetIconBasicDelegate(bool isLongPress = false, bool isPressType = true,
        UIEventListener.VoidDelegate normalDelegate = null, UIEventListener.VoidDelegate longPressDelegate = null, UIEventListener.VoidDelegate longPressFinishDelegate = null)
    {
        //Set press lis.
        NguiLongPress.NormalPressTriggerWhenLongPress = !isLongPress;
        NguiLongPress.LongTriggerType = isPressType ? NGUILongPress.TriggerType.Press : NGUILongPress.TriggerType.Release;
        OnNormalPress = normalDelegate;
        OnLongPress = longPressDelegate;
        OnLongPressFinish = longPressFinishDelegate;
    }

    /// <summary>
    /// Set icon sample button normal click events, use SetIconButtonLongPress if you wanna use longpress at the same time.
    /// </summary>
    /// <param name="upgradeDelegate">upgrade button delegate</param>
    /// <param name="subDelegate">sub button delegate</param>
    /// <param name="addDelegate">add button delegate</param>
    public void SetIconButtonDelegate(UIEventListener.VoidDelegate upgradeDelegate = null,
        UIEventListener.VoidDelegate addDelegate = null, UIEventListener.VoidDelegate subDelegate = null)
    {
        //Set button lis.
        UpgradeLis.onClick = upgradeDelegate;
        AddLis.onClick = addDelegate;
        SubLis.onClick = subDelegate;

        if (upgradeDelegate == null)
        {
            UpgradeButton.SetActive(false);
        }
        if (addDelegate == null)
        {
            AddButton.SetActive(false);
        }
        if (subDelegate == null)
        {
            SubButton.SetActive(false);
        }
    }

    /// <summary>
    /// Set icon specific button normal click and long press events.
    /// </summary>
    /// <param name="key">specific button key</param>
    /// <param name="normalDelegate">normal click icon delegate</param>
    /// <param name="isPressType">long press press mode or release mode</param>
    /// <param name="longPressFinishDelegate">long press finish delegate</param>
    /// <param name="longPressDelegate">long press delegate</param>
    /// <returns>setting succeed or not</returns>
    public bool SetIconButtonLongPress(string key, bool isPressType = true,
        UIEventListener.VoidDelegate normalDelegate = null, UIEventListener.VoidDelegate longPressDelegate = null,
        UIEventListener.VoidDelegate longPressFinishDelegate = null)
    {
        GameObject tempGameObject;
        switch (key)
        {
            case "upgrade":
                {
                    tempGameObject = UpgradeButton;
                    break;
                }
            case "add":
                {
                    tempGameObject = AddButton;
                    break;
                }
            case "sub":
                {
                    tempGameObject = SubButton;
                    break;
                }
            default:
                {
                    Debug.LogError("Error key:" + key + " in setting icon sample button longpress.");
                    return false;
                }
        }
        NGUILongPress tempLongPress = tempGameObject.GetComponent<NGUILongPress>() ??
                                      tempGameObject.AddComponent<NGUILongPress>();

        //Set press lis.
        tempLongPress.LongTriggerType = isPressType ? NGUILongPress.TriggerType.Press : NGUILongPress.TriggerType.Release;
        tempLongPress.OnNormalPress = normalDelegate;
        tempLongPress.OnLongPress = longPressDelegate;
        tempLongPress.OnLongPressFinish = longPressFinishDelegate;

        return true;
    }

    /// <summary>
    /// Set progress bar value in the buttom.
    /// </summary>
    /// <param name="value">value to be setted.</param>
    public void SetProgressBar(float value)
    {
        ButtomProgressBar.value = value;
        ButtomProgressBar.gameObject.SetActive(true);
    }

    /// <summary>
    /// Open button effect and button scale effect or not.
    /// </summary>
    /// <param name="isButtonEffectEnable">open button effect</param>
    /// <param name="isButtonScaleEffectEnable">open button scale effect</param>
    public void SetIconEffect(bool isButtonEffectEnable = false, bool isButtonScaleEffectEnable = false)
    {
        //Set effect enable or not.
        ButtonEffect.enabled = isButtonEffectEnable;
        ButtonScaleEffect.enabled = isButtonScaleEffectEnable;
    }

    private int m_commonId;//@author LiuChang

    private string m_iconName;//@author LiuChang

    private string m_enemyName;//@author LiuChang

    private string m_enemyDesc;//@author LiuChang

    /// <summary>
    /// Set icon sample pop up text.
    /// </summary>
    /// <param name="commonId">id to set</param>
    /// <param name="popTextTitle">pop frame text title</param>
    /// <param name="popTextDesc">pop frame text desc</param>
    /// <param name="popMode">[Obsolete]0: left top at center of button, 1: above the button</param>
    /// <param name="offsetVec3">[Obsolete]pop frame offset</param>
    /// <returns>pop up gameobject</returns>
    public GameObject SetIconPopText(int commonId = 0, string popTextTitle = "", string popTextDesc = "", int popMode = 0, Vector3 offsetVec3 = new Vector3())
    {
        m_commonId = commonId;//@author LiuChang

        m_enemyName = popTextTitle;//@author LiuChang

        m_enemyDesc = popTextDesc;//@author LiuChang

        //Set pop text.
        PopTextLabel.text = popTextTitle + "\n \n" + popTextDesc;
        //if (!string.IsNullOrEmpty(popTextTitle) || !string.IsNullOrEmpty(popTextDesc))//@author LiuChang
        {
            NguiLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
            OnLongPress += ActivePopFrame;
            OnLongPressFinish += DeActivePopFrame;

            popTextMode = popMode;
        }

        return PopFrameSprite.gameObject;
    }

    private const int OutterGap = 20;
    private const float screenFixedHeight = 640f;
    private const float screenFixedWidth = 960f;

    private float screenAutoSize
    {
        get { return Math.Max(screenFixedWidth / Screen.width, screenFixedHeight / Screen.height); }
    }

    private float screenHeightInUIRootPos
    {
        get
        {
            return Screen.height * screenAutoSize;
        }
    }

    private float screenWidthInUIRootPos
    {
        get
        {
            return Screen.width * screenAutoSize;
        }
    }

    /// <summary>
    /// Active pop frame and set pos to adapt screen, may have little gap in offSetPosToUIRoot.
    /// </summary>
    /// <param name="go"></param>
    private void ActivePopFrame(GameObject go)
    {
        if (m_commonId != 0) ShowTip.showTip(m_commonId);//@author LiuChang

        else ShowTip.showTipEnemy(m_iconName, m_enemyName, m_enemyDesc);//@author LiuChang

        //StartCoroutine(DoActivePopFrame());
    }

    private Camera originalCamera;
    private Camera highestCamera;

    //popMode, 0: left top at center of button, 1: above the button.
    [Obsolete("Replaced with LiuChang's Tip")]
    private IEnumerator DoActivePopFrame()
    {
        if (!HighestUI.IsExist)
        {
            Debug.LogError("Cancel show pop frame cause highest ui layer not exist.");
            yield break;
        }

        //Calc tranfer local position.
        Vector3 borderOriginalLocalPos;
        switch (popTextMode)
        {
            case 0:
                borderOriginalLocalPos = Vector3.zero;
                break;
            case 1:
                borderOriginalLocalPos = new Vector3(0, BgSprite.height / 2.0f, 0);
                break;
            default:
                Debug.LogError("Not correct popTextMode:" + popTextMode);
                yield break;
        }
        var borderOriginalWorldPos = transform.TransformPoint(borderOriginalLocalPos);
        var screenPos = (originalCamera ?? (originalCamera = GetComponentInParent<Camera>())).WorldToScreenPoint(borderOriginalWorldPos);
        var borderHighestWorldPos = (highestCamera ?? (highestCamera = HighestUI.Instance.GetComponentInChildren<Camera>())).ScreenToWorldPoint(screenPos);
        var borderHighestLocalPos = HighestUI.Instance.PanelParent.transform.InverseTransformPoint(borderHighestWorldPos);

        //Change parent.
        PopFrameSprite.transform.parent = HighestUI.Instance.PanelParent.transform;
        HighestUI.Instance.childs.Add(PopFrameSprite.gameObject);
        PopFrameSprite.transform.localScale = Vector3.one;

        //Rerender after changing parent, get correct pop frame sprite width and height.
        PopFrameSprite.gameObject.SetActive(false);
        PopFrameSprite.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        //Set pop frame width and height with correct label's.
        PopFrameSprite.width = PopTextLabel.width + 2 * OutterGap;
        PopFrameSprite.height = PopTextLabel.height + 2 * OutterGap;

        switch (popTextMode)
        {
            case 0:
                PopFrameSprite.transform.localPosition = borderHighestLocalPos + new Vector3(PopFrameSprite.width / 2.0f, -PopFrameSprite.height / 2.0f, 0);
                break;
            case 1:
                PopFrameSprite.transform.localPosition = borderHighestLocalPos + new Vector3(0, PopFrameSprite.height / 2.0f, 0);
                break;
            default:
                Debug.LogError("Not correct popTextMode:" + popTextMode);
                yield break;
        }

        //Adapt pop frame pos y to screen.
        if (PopFrameSprite.transform.localPosition.y - PopFrameSprite.height / 2.0f < -screenHeightInUIRootPos / 2.0f)
        {
            var nowPos = PopFrameSprite.transform.localPosition;
            PopFrameSprite.transform.localPosition = new Vector3(nowPos.x, -screenHeightInUIRootPos / 2.0f + PopFrameSprite.height / 2.0f, 0);
        }
        if (PopFrameSprite.transform.localPosition.y + PopFrameSprite.height / 2.0f > screenHeightInUIRootPos / 2.0f)
        {
            var nowPos = PopFrameSprite.transform.localPosition;
            PopFrameSprite.transform.localPosition = new Vector3(nowPos.x, screenHeightInUIRootPos / 2.0f - PopFrameSprite.height / 2.0f, 0);
        }

        //Adapt pop frame pos x to screen.
        if (PopFrameSprite.transform.localPosition.x + PopFrameSprite.width / 2.0f > screenWidthInUIRootPos / 2.0f)
        {
            var nowPos = PopFrameSprite.transform.localPosition;
            PopFrameSprite.transform.localPosition = new Vector3(screenWidthInUIRootPos / 2.0f - PopFrameSprite.width / 2.0f, nowPos.y, 0);
        }
        if (PopFrameSprite.transform.localPosition.x - PopFrameSprite.width / 2.0f < -screenWidthInUIRootPos / 2.0f)
        {
            var nowPos = PopFrameSprite.transform.localPosition;
            PopFrameSprite.transform.localPosition = new Vector3(-screenWidthInUIRootPos / 2.0f + PopFrameSprite.width / 2.0f, nowPos.y, 0);
        }
    }

    #region Old Active pop frame, maybe used in future.

    ///// <summary>
    ///// Active pop frame and set pos to adapt screen, may have little gap in offSetPosToUIRoot.
    ///// </summary>
    ///// <param name="go"></param>
    //private void ActivePopFrame(GameObject go)
    //{
    //    StartCoroutine(DoActivePopFrame());
    //}

    ////popMode, 0: left top at center of button, 1: above the button.
    //private IEnumerator DoActivePopFrame()
    //{
    //    if (!HighestUI.IsExist)
    //    {
    //        Debug.LogError("Cancel show pop frame cause highest ui layer not exist.");
    //        yield break;
    //    }

    //    //Move to icon to calc, refresh for a frame to get correct label width and height.
    //    PopFrameSprite.transform.parent = transform;
    //    PopFrameSprite.gameObject.SetActive(false);
    //    PopFrameSprite.gameObject.SetActive(true);
    //    yield return new WaitForEndOfFrame();

    //    //Set pop frame width and height with correct label's.
    //    PopFrameSprite.width = PopTextLabel.width + 2 * OutterGap;
    //    PopFrameSprite.height = PopTextLabel.height + 2 * OutterGap;

    //    //Try set pop frame pos.
    //    Vector3 pos = new Vector3();
    //    switch (popTextMode)
    //    {
    //        case 0: pos = new Vector3(PopFrameSprite.width / 2.0f, -PopFrameSprite.height / 2.0f, 0);
    //            break;
    //        case 1: pos = new Vector3(0, PopFrameSprite.height / 2.0f + BgSprite.height / 2.0f, 0);
    //            break;
    //        default:
    //            Debug.LogError("Not correct popTextMode:" + popTextMode);
    //            break;
    //    }
    //    PopFrameSprite.transform.localPosition = pos + OffsetPos;

    //    //Get transfered 2 UIRoot.
    //    UIRoot highestUIRoot = HighestUI.Instance.Root;
    //    UIRoot iconUIRoot = UtilityTool.GetComponentInParent<UIRoot>(transform);
    //    if (highestUIRoot == null || iconUIRoot == null)
    //    {
    //        Debug.LogError("Can't find uiroot, abort setting adapt screen.");
    //        yield break;
    //    }

    //    //Move to highest hierarchy to render, do not use Camera.WorldToScreen cause can not get camera.....

    //    //Change parent.
    //    PopFrameSprite.transform.parent = HighestUI.Instance.PanelParent.transform;
    //    HighestUI.Instance.childs.Add(PopFrameSprite.gameObject);

    //    //Set pos to make it look like at the same pos in screen.
    //    Vector3 offSetPosToUIRoot = PopFrameSprite.transform.position - iconUIRoot.transform.position;
    //    PopFrameSprite.transform.position += highestUIRoot.transform.position - iconUIRoot.transform.position - offSetPosToUIRoot * (iconUIRoot.transform.localScale.x - highestUIRoot.transform.localScale.x);

    //    //Calc offSetPos between pop frame and zero pos camera when pop frame's localposition == zero, UIRoot scale considered in new UIRoot.
    //    offSetPosToUIRoot -= offSetPosToUIRoot *
    //                         (iconUIRoot.transform.localScale.x - highestUIRoot.transform.localScale.x);
    //    offSetPosToUIRoot = new Vector3(offSetPosToUIRoot.x / highestUIRoot.transform.localScale.x, offSetPosToUIRoot.y / highestUIRoot.transform.localScale.y, offSetPosToUIRoot.z / highestUIRoot.transform.localScale.z) - PopFrameSprite.transform.localPosition;

    //    //Adapt pop frame pos y to screen.
    //    if (PopFrameSprite.transform.localPosition.y + offSetPosToUIRoot.y - PopFrameSprite.height / 2.0f < -screenHeightInUIRootPos / 2.0f)
    //    {
    //        var nowPos = PopFrameSprite.transform.localPosition;
    //        PopFrameSprite.transform.localPosition = new Vector3(nowPos.x,
    //            -screenHeightInUIRootPos / 2.0f + PopFrameSprite.height / 2.0f - offSetPosToUIRoot.y,
    //            0);
    //    }
    //    if (PopFrameSprite.transform.localPosition.y + offSetPosToUIRoot.y + PopFrameSprite.height / 2.0f > screenHeightInUIRootPos / 2.0f)
    //    {
    //        var nowPos = PopFrameSprite.transform.localPosition;
    //        PopFrameSprite.transform.localPosition = new Vector3(nowPos.x,
    //            screenHeightInUIRootPos / 2.0f - PopFrameSprite.height / 2.0f - offSetPosToUIRoot.y,
    //            0);
    //    }

    //    //Adapt pop frame pos x to screen.
    //    if (PopFrameSprite.transform.localPosition.x + offSetPosToUIRoot.x + PopFrameSprite.width / 2.0f > screenWidthInUIRootPos / 2.0f)
    //    {
    //        var nowPos = PopFrameSprite.transform.localPosition;
    //        PopFrameSprite.transform.localPosition = new Vector3(screenWidthInUIRootPos / 2.0f - PopFrameSprite.width / 2.0f - offSetPosToUIRoot.x,
    //            nowPos.y,
    //            0);
    //    }
    //    if (PopFrameSprite.transform.localPosition.x + offSetPosToUIRoot.x - PopFrameSprite.width / 2.0f < -screenWidthInUIRootPos / 2.0f)
    //    {
    //        var nowPos = PopFrameSprite.transform.localPosition;
    //        PopFrameSprite.transform.localPosition = new Vector3(-screenWidthInUIRootPos / 2.0f + PopFrameSprite.width / 2.0f - offSetPosToUIRoot.x,
    //            nowPos.y,
    //            0);
    //    }

    //    PopFrameSprite.gameObject.SetActive(false);
    //    PopFrameSprite.gameObject.SetActive(true);
    //    yield return new WaitForEndOfFrame();
    //}

    #endregion

    /// <summary>
    /// Deactive pop frame.
    /// </summary>
    /// <param name="go"></param>
    private void DeActivePopFrame(GameObject go)
    {
        //@author LiuChang

        ShowTip.close();

        //        PopFrameSprite.gameObject.SetActive(false);
        //        PopFrameSprite.transform.parent = transform;
    }

    void OnDestroy()
    {
        if (PopFrameSprite.transform.parent != transform)
        {
            PopFrameSprite.transform.parent = null;
            Destroy(PopFrameSprite.gameObject);
        }
    }
}
