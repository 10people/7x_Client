//#define DEBUG_LONG_PRESS



using UnityEngine;



/// <summary>
/// Long press control for NGUI, OnLongPressFinish only for press type.
/// </summary>
public class NGUILongPress : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Only used for press type.
    /// </summary>
    public UIEventListener.VoidDelegate OnLongPressFinish;

    /// <summary>
    /// Long press trigger.
    /// </summary>
    public UIEventListener.VoidDelegate OnLongPress;

    /// <summary>
    /// Normal press trigger.
    /// </summary>
    public UIEventListener.VoidDelegate OnNormalPress;

    public enum TriggerType
    {
        Release,
        Press
    }
    [HideInInspector]
    public TriggerType LongTriggerType = TriggerType.Press;

    /// <summary>
    /// Usually, u should set this to false, donot modify unless u know what u r doing.
    /// </summary>
    [HideInInspector]
    public bool NormalPressTriggerWhenLongPress = false;

    #endregion

    #region Private Fields

    private bool dragged;
    private float lastPress = -1f;
    private bool isInPress;
    private Vector3 cachedClickPos;
    private const float MinDeviation = 0.01f;
    private const float LongClickDuration = 0.01f;

    #endregion



	#region Mono

	void Awake(){
		#if DEBUG_LONG_PRESS
		GameObjectHelper.LogGameObjectHierarchy( gameObject, "NGUILongPress.Awake()" );
		#endif
	}

	void OnDestroy(){
		#if DEBUG_LONG_PRESS
		GameObjectHelper.LogGameObjectHierarchy( gameObject, "NGUILongPress.OnDestroy()" );
		#endif
	}

	#endregion


    #region Private Methods

    private void OnPress(bool pressed)
    {
		#if DEBUG_LONG_PRESS
		GameObjectHelper.LogGameObjectHierarchy( gameObject, "NGUILongPress.OnPress( " + pressed + " )" );
		#endif

        if (pressed)
        {
            dragged = false;
            lastPress = Time.realtimeSinceStartup;
            isInPress = true;

			#if DEBUG_LONG_PRESS
			Debug.Log( "Refresh: " + lastPress );
			#endif

            cachedClickPos = Input.mousePosition;

//			#if UNITY_EDITOR
//			#elif UNITY_ANDROID || UNITY_IPHONE
//			cachedClickPos = DeviceHelper.GetFirstTouchPosition();
//			#endif

            cachedClickPos = Input.mousePosition;
            Invoke("CheckPressTypeLongPress", LongClickDuration);

        }
        else
        {
            isInPress = false;

			#if DEBUG_LONG_PRESS
			Debug.Log( "Cur Time Offset: " + ( Time.realtimeSinceStartup - lastPress ) );

			Debug.Log( "Duration: " + LongClickDuration );

			Debug.Log( "Cur.Time: " + Time.realtimeSinceStartup );

			Debug.Log( "Last Press: " + lastPress );
			#endif

            //If the press time is over long click duration and the object is not be dragged, trigger long press.
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration)
            {
				#if DEBUG_LONG_PRESS
				Debug.Log( "Long Press Found." );
				#endif

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

    private void OnDragStart()
    {
        dragged = true;

		Vector3 t_compare_pos = Input.mousePosition;

		float t_dis = Vector3.Distance(cachedClickPos, Input.mousePosition);

//		#if UNITY_EDITOR
//		#elif UNITY_ANDROID || UNITY_IPHONE
//		t_dis = Vector2.Distance( DeviceHelper.GetFirstTouchPosition(), new Vector2( cachedClickPos.x, cachedClickPos.y ) );
//		#endif

		if( NGUIHelper.HaveUIScrollViewInParent( gameObject ) ){
			if ( t_dis > MinDeviation
				&& Time.realtimeSinceStartup - lastPress > LongClickDuration)
			{
				CheckPressTypeLongPressFinish();
			}
		}
    }

    private void CheckReleaseTypeLongPress()
    {
        if (LongTriggerType == TriggerType.Release && !dragged && OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    private void CheckPressTypeLongPressFinish()
    {
        if (LongTriggerType == TriggerType.Press && OnLongPressFinish != null)
        {
            OnLongPressFinish(gameObject);
        }
    }

    private void CheckPressTypeLongPress()
    {
        if (LongTriggerType == TriggerType.Press && !dragged && isInPress && OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    #endregion
}
