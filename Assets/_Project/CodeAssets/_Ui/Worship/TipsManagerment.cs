using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TipsManagerment : MonoBehaviour 
{
    public UISprite PopFrameSprite;
 
    public UILabel PopTextLabel;

    public LongPressManangerment NguiLongPress;
    public UISprite BgSprite;
    public int ShowType = 0;
    private int popTextMode;
    private Vector3 OffsetPos;

    private const int OutterGap = 20;
    private const float screenFixedHeight = 640f;
    private const float screenFixedWidth = 960f;

    private UIEventListener.VoidDelegate OnLongPressFinish
    {
        get { return NguiLongPress.OnLongPressFinish; }
        set { NguiLongPress.OnLongPressFinish = value; }
    }

    private UIEventListener.VoidDelegate OnLongPress
    {
        get { return NguiLongPress.OnLongPress; }
        set { NguiLongPress.OnLongPress = value; }
    }


    void Start()
    {
        OnLongPress += ActivePopFrame;
        OnLongPressFinish += DeActivePopFrame;
    }
    public GameObject SetIconPopText(string popTextTitle = "", string popTextDesc = "", int popMode = 0, Vector3 offsetVec3 = new Vector3())
    {
        //Set pop text.
        PopTextLabel.text = popTextTitle + "\n \n" + popTextDesc;
        if (!string.IsNullOrEmpty(popTextTitle) || !string.IsNullOrEmpty(popTextDesc))
        {
            NguiLongPress.LongTriggerType = LongPressManangerment.TriggerType.Press;
            OnLongPress += ActivePopFrame;
            OnLongPressFinish += DeActivePopFrame;

            popTextMode = popMode;

            if (offsetVec3 != Vector3.zero)
            {
                OffsetPos = offsetVec3;
            }
        }

        return PopFrameSprite.gameObject;
    }

    private float screenAutoSize
    {
        get { return Mathf.Max(screenFixedWidth / Screen.width, screenFixedHeight / Screen.height); }
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

    
    private void ActivePopFrame(GameObject go)
    {
        StartCoroutine(DoActivePopFrame());
    }



    private IEnumerator DoActivePopFrame()
    {
        PopFrameSprite.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();

        //Set pop frame width and height.
        PopFrameSprite.width = PopTextLabel.width + 2 * OutterGap;
        PopFrameSprite.height = PopTextLabel.height + 2 * OutterGap;

        //Try set pop frame pos.
        Vector3 pos = new Vector3();
        switch (popTextMode)
        {
            case 0: pos = new Vector3(PopFrameSprite.width / 2.0f, -PopFrameSprite.height / 2.0f, 0);
                break;
            case 1: pos = new Vector3(0, PopFrameSprite.height / 2.0f + BgSprite.height / 2.0f, 0);
                break;
            default:
                Debug.LogError("Not correct popTextMode:" + popTextMode);
                break;
        }
        PopFrameSprite.transform.localPosition = pos + OffsetPos;

        //UIRoot root = UtilityTool.GetComponentInParent<UIRoot>(PopFrameSprite.transform);
        //if (root == null)
        //{
        //    Debug.LogError("Can't find uiroot in parent, abort setting adapt screen.");
        //    yield break;
        //}

        //Vector3 offSetPosToUIRoot = PopFrameSprite.transform.position - root.transform.position;
        //offSetPosToUIRoot = new Vector3(offSetPosToUIRoot.x / root.transform.localScale.x, offSetPosToUIRoot.y / root.transform.localScale.y, offSetPosToUIRoot.z / root.transform.localScale.z);
        if (ShowType == 0)
        {
            PopFrameSprite.transform.localPosition = new Vector3(-300, 288, 0);
        }
        else
        {

            PopFrameSprite.transform.localPosition = new Vector3(0, 248, 0);
        }
        //Adapt pop frame pos y to screen.
        //if (PopFrameSprite.transform.localPosition.y + offSetPosToUIRoot.y - PopFrameSprite.height / 2.0f < -screenHeightInUIRootPos / 2.0f)
        //{
        //    var nowPos = PopFrameSprite.transform.localPosition;
        //    PopFrameSprite.transform.localPosition = new Vector3(nowPos.x,
        //        -screenHeightInUIRootPos / 2.0f + PopFrameSprite.height / 2.0f - offSetPosToUIRoot.y,
        //        0);
        //}
        //if (PopFrameSprite.transform.localPosition.y + offSetPosToUIRoot.y + PopFrameSprite.height / 2.0f > screenHeightInUIRootPos / 2.0f)
        //{
        //    var nowPos = PopFrameSprite.transform.localPosition;
        //    PopFrameSprite.transform.localPosition = new Vector3(nowPos.x,
        //        screenHeightInUIRootPos / 2.0f - PopFrameSprite.height / 2.0f - offSetPosToUIRoot.y,
        //        0);
        //}

        ////Adapt pop frame pos x to screen.
        //if (PopFrameSprite.transform.localPosition.x + offSetPosToUIRoot.x + PopFrameSprite.width / 2.0f > screenWidthInUIRootPos / 2.0f)
        //{
        //    var nowPos = PopFrameSprite.transform.localPosition;
        //    PopFrameSprite.transform.localPosition = new Vector3(screenWidthInUIRootPos / 2.0f - PopFrameSprite.width / 2.0f - offSetPosToUIRoot.x,
        //        nowPos.y,
        //        0);
        //}
        //if (PopFrameSprite.transform.localPosition.x + offSetPosToUIRoot.x - PopFrameSprite.width / 2.0f < -screenWidthInUIRootPos / 2.0f)
        //{
        //    var nowPos = PopFrameSprite.transform.localPosition;
        //    PopFrameSprite.transform.localPosition = new Vector3(-screenWidthInUIRootPos / 2.0f + PopFrameSprite.width / 2.0f - offSetPosToUIRoot.x,
        //        nowPos.y,
        //        0);
        //}
    }

    /// <summary>
    /// Deactive pop frame.
    /// </summary>
    /// <param name="go"></param>
    private void DeActivePopFrame(GameObject go)
    {
        PopFrameSprite.gameObject.SetActive(false);
    }
}
