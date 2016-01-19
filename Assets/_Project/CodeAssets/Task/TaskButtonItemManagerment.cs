using UnityEngine;
using System.Collections;

public class TaskButtonItemManagerment : MonoBehaviour {

    public UISprite m_SpriteIcon;
    public delegate void OnClick_Touch(int type);
    OnClick_Touch CallBackTouch;
    private int _TypeNum = 0;
    void Start ()
    {
	
	}

    void OnClick()
    {
        if (CallBackTouch != null)
        {
            CallBackTouch(_TypeNum);
        }

    }

    public void ShowInfo(TaskLayerManager.TaskType info, OnClick_Touch callback)
    {
        _TypeNum = info._type;
        m_SpriteIcon.spriteName = info._Icon;
        CallBackTouch = callback;
    }
	
 
}
