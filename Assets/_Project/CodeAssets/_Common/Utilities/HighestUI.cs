using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cannot access this if IsExist is false.
/// </summary>
public class HighestUI : Singleton<HighestUI>, IUIRootAutoActivator
{
    public static bool IsExist;

    public UIRoot Root;

    /// <summary>
    /// Must add to childs after move to parent panel.
    /// </summary>
    public GameObject PanelParent;

    public UIAnchor m_TopAnchor;
    public UIAnchor m_TopLeftAnchor;
    public UIAnchor m_TopRightAnchor;
    public UIAnchor m_ButtomAnchor;
    public UIAnchor m_ButtomLeftAnchor;
    public UIAnchor m_ButtomRightAnchor;
    public UIAnchor m_CenterAnchor;
    public UIAnchor m_LeftAnchor;
    public UIAnchor m_RightAnchor;

    public BroadCast m_BroadCast;

    /// <summary>
    /// Must add all object under this to childs.
    /// </summary>
    public List<GameObject> childs = new List<GameObject>();

    void OnLevelWasLoaded(int level)
    {
        for (int i = 0; i < childs.Count; i++)
        {
            Destroy(childs[i]);
            childs[i] = null;
        }
    }

    void Awake(){
        DontDestroyOnLoad(gameObject);

		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
    }

	void OnDestroy(){
		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}

		{
			base.OnDestroy();
		}
	}


	#region IUIRootAutoActivator
	
	public bool IsNGUIVisible(){
		return m_BroadCast.IsInBroadCast;
	}
	
	#endregion
}
