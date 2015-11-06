using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FunctionButtonManager : MonoBehaviour, IComparable<FunctionButtonManager>
{
    #region xml data

    public void SetData(FunctionOpenTemp template)
    {
        m_FuncTemplate = template;

        m_index = template.m_iID;
        if (m_index < 0)
        {
            m_ButtonSprite.spriteName = "NULL";
        }
        else
        {
            //Set main sprite.
            if (!MainCityUIRB.ButtonSpriteNameTransferDic.ContainsKey(m_index))
            {
                Debug.LogError("ButtonSpriteNameTransferDic donot contain key:" + m_index);
                return;
            }
            m_ButtonSprite.spriteName = MainCityUIRB.ButtonSpriteNameTransferDic[m_index];
        }

        m_button_name = template.key;
        m_RankIndex = template.rank;
        m_Type = (MainCityUIRB.ButtonType)template.type;
    }

    public int m_index;
    private FunctionOpenTemp m_FuncTemplate;
    private string m_button_name;
    private int m_RankIndex;
    private MainCityUIRB.ButtonType m_Type;

    #endregion

    /// <summary>
    /// Locked item cannot be enabled.
    /// </summary>
    public static List<int> s_LockedList = new List<int>();

    public UISprite m_ButtonSprite;
    public GameObject RedAlertObject;

    /// <summary>
    /// Lock sprite
    /// </summary>
    public GameObject DenableObject;

    /// <summary>
    /// Button click component.
    /// </summary>
    public UIButton m_UiButton;

    /// <summary>
    /// Disable all buttons click manually.
    /// </summary>
    public static bool CanClick
    {
        get { return !ClientMain.m_isNewOpenFunction; }
    }

    /// <summary>
    /// Is function button enabled or not.
    /// </summary>
    public bool IsEnabled;

    /// <summary>
    /// Is red alert object showed or particle effect showed.
    /// </summary>
    [HideInInspector]
    public bool IsAlertShowed;

    public void EnableButton()
    {
        IsEnabled = true;
        m_UiButton.enabled = true;
        m_ButtonSprite.color = Color.white;
        DenableObject.SetActive(false);
    }

    public void DenableButton()
    {
        //Cancel show sprite when null button.
        if (m_index < 0) return;

        IsEnabled = false;
        m_UiButton.enabled = false;
        m_ButtonSprite.color = Color.grey;
        DenableObject.SetActive(true);
        HideRedAlert();
    }

    /// <summary>
    /// Show red alert object.
    /// </summary>
    public void ShowRedAlert()
    {
        //Cancel show sprite when null button.
        if (m_index < 0) return;

        //cancel show locked button
        if (s_LockedList.Contains(m_index)) return;

        if (!m_UiButton.isEnabled)
        {
            Debug.LogWarning("============Cancel show redalert in button:" + m_index + " cause button not enabled.");
            return;
        }

        IsAlertShowed = true;
        RedAlertObject.SetActive(true);
    }

    public void HideRedAlert()
    {
        IsAlertShowed = false;
        RedAlertObject.SetActive(false);
    }

    public delegate void OnFuncBTNClick(int index);

    public OnFuncBTNClick m_OnFuncBtnClick;

    [HideInInspector]
    public bool IsShowEffect;

    public void ShowEffect()
    {
        if (IsShowEffect)
        {
            //[FIX]open effect here
            //UI3DEffectTool.Instance().ShowTopLayerEffect(gameObject, EffectTemplate.GetEffectPath(EffectTemplate.Effects.UI_FUNCTION_OPEN));
        }
    }

    void OnDisable()
    {
        if (IsShowEffect)
        {
            //[FIX]open effect here
            //UI3DEffectTool.Instance().ClearUIFx(gameObject);
        }
    }

    private readonly Vector2 popLabelDistance = new Vector2(0, 25);
    private const float popLabelDuration = 2.0f;
    private const float popLabelProtectDuration = 1.5f;

    private void OnClick()
    {
        //Disable all buttons click manually.
        if (!CanClick) return;

        if (!IsEnabled)
        {
            //get not open warning tips.
            string str = m_FuncTemplate.m_sNotOpenTips;

            if (!string.IsNullOrEmpty(str) && str != "-1")
            {
                ClientMain.m_UITextManager.createText(str);
            }

            return;
        }

        IsShowEffect = false;
        //[FIX]open effect here
        //UI3DEffectTool.Instance().ClearUIFx(gameObject);

        if (m_OnFuncBtnClick != null)
        {
            m_OnFuncBtnClick(m_index);
        }
    }

    /// <summary>
    /// protection for pop up icon not open info.
    /// </summary>
    private bool canShowPopLabel = true;

    /// <summary>
    /// for buttons compare, not used in this version.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(FunctionButtonManager other)
    {
        if (other == null)
        {
            return 1;
        }
        else
        {
            return m_RankIndex.CompareTo(other.m_RankIndex);
        }
    }
}
