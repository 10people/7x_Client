using UnityEngine;
using System.Collections;

public class LongPressManangerment : MonoBehaviour
{
    public UIEventListener.VoidDelegate OnLongPressFinish;
    public UIEventListener.VoidDelegate OnLongPress;
    public UIEventListener.VoidDelegate OnNormalPress;


    public UISprite m_SpriteIcon;
    public enum TriggerType
    {
        Release,
        Press
    }
    [HideInInspector]
    public TriggerType LongTriggerType = TriggerType.Press;
    [HideInInspector]
    public bool NormalPressTriggerWhenLongPress = false;


    private bool dragged;
    private float lastPress = -1f;
    private bool isInPress;
    private Vector3 cachedClickPos;
    private const float MinDeviation = 0.01f;
    private const float LongClickDuration = 0.2f;


    private void OnPress(bool pressed)
    {
        if (pressed)
        {
            dragged = false;
            lastPress = Time.realtimeSinceStartup;
            isInPress = true;
            cachedClickPos = Input.mousePosition;
            if (LongTriggerType == TriggerType.Press)
            {
                Invoke("CheckPressTypeLongPress", LongClickDuration);
            }
        }
        else
        {
            isInPress = false;
            //If the press time is over long click duration and the object is not be dragged, trigger long press.
            //if (Time.realtimeSinceStartup - lastPress > LongClickDuration)
            {
                CheckReleaseTypeLongPress();
                CheckPressTypeLongPressFinish();
            }
        }
    }

    private void OnClick()
    {
        isInPress = false;
        if (!NormalPressTriggerWhenLongPress)
        {
            if (Time.realtimeSinceStartup - lastPress < LongClickDuration)
            {
                CancelInvoke("CheckPressTypeLongPress");
                if (OnNormalPress != null)
                {
                    OnNormalPress(gameObject);
                }
            }
        }
        else
        {
            CancelInvoke("CheckPressTypeLongPress");
            if (OnNormalPress != null)
            {
                OnNormalPress(gameObject);
            }
        }
    }

    private void CheckReleaseTypeLongPress()
    {
        if (OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    private void CheckPressTypeLongPressFinish()
    {
       // if (LongTriggerType == TriggerType.Press && OnLongPressFinish != null)
        {
            OnLongPressFinish(gameObject);
        }
    }

    private void CheckPressTypeLongPress()
    {
        if (isInPress && OnLongPress != null)
        {
            OnLongPress(gameObject);
            transform.GetComponent<TipsManagerment>().SetIconPopText(NameIdTemplate.GetName_By_NameId(CommonItemTemplate.getCommonItemTemplateById(int.Parse(m_SpriteIcon.spriteName)).nameId), DescIdTemplate.GetDescriptionById(CommonItemTemplate.getCommonItemTemplateById(int.Parse(m_SpriteIcon.spriteName)).descId));
        }
    }

}