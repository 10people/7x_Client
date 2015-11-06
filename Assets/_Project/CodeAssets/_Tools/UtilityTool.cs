//#define DEBUG_BOX

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using qxmobile.protobuf;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2013.8.1
 * @since:		Unity 4.5.3
 * Function:	Utility functions.
 * 
 * Notes:
 * None.
 */
public class UtilityTool : Singleton<UtilityTool>{

    public static GameObject m_dont_destroy_on_load_gb = null;

    private static GameObject m_temp_gbs_root_gb = null;


    #region Mono

    void Awake()
    {
        // application config
        {
            Application.runInBackground = true;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        ConfigTool temp = ConfigTool.Instance;
    }

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
		{
			ComponentHelper.GlobalClassUpdate();
		}
    }

    void LateUpdate()
    {

    }

    void OnGUI()
    {

    }

    void OnApplicationFocus(bool p_focused)
    {
        //		Debug.Log( "UtilityTool.OnApplicationFocus( " + p_focused + " )" );


    }

    void OnApplicationPause(bool p_pause)
    {
        //		Debug.Log( "UtilityTool.OnApplicationPause( " + p_pause + " )" );

        if ( p_pause ){
			// clean
			{
				UtilityTool.Instance.DelayedUnloadUnusedAssets();
			}

        }

        // send message
        {
            if (SocketTool.IsConnected())
            {
                if (p_pause)
                {
                    //					Debug.Log( "Socket Send Game Pause." );

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.GAME_PAUSE, "");
                }
                else
                {
                    //					Debug.Log( "Socket Send Game Continue." );

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.GAME_CONTINUE, "");
                }
            }
        }
    }

    void OnDestroy()
    {
        
    }

    #endregion



    #region Game Pause

    private bool m_is_game_paused = false;

    public void ManualGamePause()
    {
        m_is_game_paused = !m_is_game_paused;

        OnApplicationPause(m_is_game_paused);
    }

    #endregion


	#region Text

	

    private static Object m_cached_box_obj;

    public static void LoadBox()
    {
        //		Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CachedBoxCallback );

        // Updated 2015.7.4, Bundle Load Use.
        Global.ResourcesDotLoad("New/Box", CachedBoxCallback);
    }

    private static void CachedBoxCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        m_cached_box_obj = p_object;
    }

	public GameObject CreateBox(string p_title, string p_des_1, string p_des_2, List<BagItem> p_bag_item, string p_btn_name_1, string p_btn_name_2, UIBox.onclick p_click_delegate, UIBox.OnBoxCreated p_on_create = null, UIFont uifontButton1 = null, UIFont uifontButton2 = null, bool isShowBagItemNumBelow = false, bool isSetDepth = true, bool isBagItemTop = true)
    {
		bool t_debug_box = false;

		if ( ConfigTool.GetBool (ConfigTool.CONST_LOG_DIALOG_BOX) ) {
			t_debug_box = true;
		}

		#if DEBUG_BOX
		t_debug_box = true;
		#endif

		if( t_debug_box ){
            Debug.Log("------ CreateBox ------");

            Debug.Log("p_title: " + p_title);

            Debug.Log("p_des_1: " + p_des_1);

            Debug.Log("p_des_2: " + p_des_2);

			if( p_bag_item != null ){
            	Debug.Log("p_bag_item: " + p_bag_item + " - count: " + p_bag_item.Count);
			}

            Debug.Log("p_btn_name_1: " + p_btn_name_1);

            Debug.Log("p_btn_name_2: " + p_btn_name_2);

            Debug.Log("p_click_delegate: " + p_click_delegate);

            Debug.Log("p_on_create: " + p_on_create);

			Debug.Log( "isShowBagItemNumBelow: " + isShowBagItemNumBelow );

			Debug.Log( "isSetDepth: " + isSetDepth );

			Debug.Log( "isBagItemTop: " + isBagItemTop );

            Debug.Log("------ CreateBox ------");
        }

        return ExecLoadBox(p_title,
                       p_des_1,
                       p_des_2,
                       p_bag_item,
                       p_btn_name_1,
                       p_btn_name_2,
                       p_click_delegate,
                       p_on_create,
	                   uifontButton1,
	                   uifontButton2,
	                   isShowBagItemNumBelow,
	                   isSetDepth,
	                   isBagItemTop);
    }

    GameObject ExecLoadBox(string tile, string dis1, string dis2, List<BagItem> bagItem, string buttonname1, string buttonname2, UIBox.onclick onClcik, UIBox.OnBoxCreated p_on_create, UIFont uifontButton1 = null, UIFont uifontButton2 = null, bool isShowBagItemNumBelow = false, bool isSetDepth = true, bool isBagItemTop = true)
    {
        if (m_cached_box_obj == null)
        {
            Debug.LogError("Error, No Cached box.");

            LoadBox();

            return null;
        }

        GameObject t_gb = GameObject.Instantiate(m_cached_box_obj) as GameObject;

        if (p_on_create != null)
        {
            p_on_create(t_gb);
        }

        UIBox uibox = (t_gb).GetComponent<UIBox>();

		uibox.setBox(tile, dis1, dis2, bagItem, buttonname1, buttonname2, onClcik, null,
		             uifontButton1,
		             uifontButton2,
		             isShowBagItemNumBelow,
		             isSetDepth,
		             isBagItemTop);

		return t_gb;
    }

    #endregion



    #region Resources Load

	public void Init(){

	}

    public void ExecResourcesLoad(string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list, bool p_open_simulate)
    {
        StartCoroutine(ResoucesDotLoad(p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate));
    }

    private IEnumerator ResoucesDotLoad(string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list, bool p_open_simulate)
    {


        yield return null;
    }

    #endregion



    #region Dont Destroy On Load

    private static string CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME = "_Dont_Destroy_On_Load";

    private static string CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME = "_Temps_GB_Root";

	public static string GetDontDestroyGameObjectName(){
		return CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;
	}

    public static GameObject GetDontDestroyOnLoadGameObject()
    {
        if (m_dont_destroy_on_load_gb == null)
        {
            m_dont_destroy_on_load_gb = new GameObject();

            m_dont_destroy_on_load_gb.name = CONST_DONT_DESTROY_ON_LOAD_GAME_OBJECT_NAME;

            DontDestroyOnLoad(m_dont_destroy_on_load_gb);
        }

        return m_dont_destroy_on_load_gb;
    }

    public static GameObject GetTempGameObjectsRootGameObject()
    {
        if (m_temp_gbs_root_gb == null)
        {
            m_temp_gbs_root_gb = new GameObject();

            m_temp_gbs_root_gb.name = CONST_TEMP_GAMEOBJECTS_ROOT_GAME_OBJECT_NAME;

            DontDestroyOnLoad(m_temp_gbs_root_gb);
        }

        return m_temp_gbs_root_gb;
    }

    public static void ClearDontDestroyGameObject()
    {
        //		Debug.Log( "UtilityTool.ClearDontDestroyGameObject()" );

        if (m_dont_destroy_on_load_gb != null)
        {

            MonoBehaviour[] t_items = m_dont_destroy_on_load_gb.GetComponents<MonoBehaviour>();

            for (int i = 0; i < t_items.Length; i++)
            {
                if (t_items[i].GetType() == typeof(UtilityTool))
                {
                    //					Debug.Log( "Skip UtilityTool." );

                    continue;
                }

                if (t_items[i].GetType() == typeof(ConfigTool))
                {
                    //					Debug.Log( "Skip ConfigTool." );

                    continue;
                }

                if (t_items[i].GetType() == typeof(Bundle_Loader))
                {
                    //					Debug.Log( "Skip Bundle_Loader." );

                    //					Bundle_Loader.CleanData();

                    continue;
                }

                if (t_items[i].GetType() == typeof(ThirdPlatform))
                {
                    continue;
                }


                t_items[i].enabled = false;

                Destroy(t_items[i]);
            }

            //			m_dont_destroy_on_load_gb.SetActive( false );
            //
            //			Destroy( m_dont_destroy_on_load_gb );
            //
            //			m_dont_destroy_on_load_gb = null;
        }
    }

    #endregion



    #region Resources Load

    /**
	 * Get List<EventDelegate>, if p_callback = null, then list.size = 0.
	 * if not null, return the p_callback conained list.
	 */
    public static List<EventDelegate> GetEventDelegateList(EventDelegate.Callback p_callback)
    {
        List<EventDelegate> t_list = new List<EventDelegate>();

        EventDelegate.Add(t_list, p_callback);

        return t_list;
    }

    public void ExecLoadCallback(Bundle_Loader.LoadResourceDone p_delegate, UnityEngine.Object t_object, string p_resource, List<EventDelegate> p_callback_list)
    {
        WWW t_www = null;

        if (p_delegate != null)
        {
            //				Debug.Log( "Exec Callback." );

            p_delegate(ref t_www, p_resource, t_object);
        }
        else
        {
            //				Debug.Log( "Callback Null." );
        }

        {
            EventDelegate.Execute(p_callback_list);
        }

        {
            Global.ResoucesLoaded(p_resource, t_object);
        }
    }

    #endregion



    #region Transform

    /// Set localPosition and localRotation to Zero.
    public static void ResetLocalPosAndLocalRot(GameObject p_gb)
    {
        if (p_gb == null)
        {
            return;
        }

        p_gb.transform.localPosition = Vector3.zero;

        p_gb.transform.localRotation = Quaternion.identity;
    }

    /// Set localPosition and localRotation and localScale to Zero.
    public static void ResetLocalPosAndLocalRotAndLocalScale(GameObject p_gb)
    {
        if (p_gb == null)
        {
            return;
        }

        ResetLocalPosAndLocalRot(p_gb);

        p_gb.transform.localScale = Vector3.one;
    }

    public static Vector3 GetLocalPositionInUIRoot(GameObject p_ngui_gb)
    {
        if (p_ngui_gb == null)
        {
            Debug.LogError("Error, ngui gb = null.");

            return Vector3.zero;
        }

        Vector3 t_local_pos = p_ngui_gb.transform.localPosition;

        //		Debug.Log( p_ngui_gb.name + ": " + p_ngui_gb.transform.localPosition + " - " + p_ngui_gb.transform.position );

        Transform t_parent = p_ngui_gb.transform.parent;

        while (t_parent != null)
        {
            if (t_parent.gameObject.GetComponent<UIRoot>() != null)
            {
                break;
            }

            t_local_pos = t_local_pos + t_parent.localPosition;

            //			Debug.Log( t_parent.name + ": " + t_parent.localPosition );

            t_parent = t_parent.parent;

            if (t_parent == null)
            {
                Debug.LogError("Error, UIRoot Not Founded.");

                return Vector3.zero;
            }
        }

        return t_local_pos;
    }

    public static Vector3 GetLocalScaleInUIRoot(GameObject p_ngui_gb)
    {
        if (p_ngui_gb == null)
        {
            Debug.LogError("Error, ngui gb = null.");

            return Vector3.one;
        }

        Vector3 t_local_scale = p_ngui_gb.transform.localScale;

        //		Debug.Log( p_ngui_gb.name + ": " + p_ngui_gb.transform.localPosition + " - " + p_ngui_gb.transform.position );

        Transform t_parent = p_ngui_gb.transform.parent;

        while (t_parent != null)
        {
            if (t_parent.gameObject.GetComponent<UIRoot>() != null)
            {
                break;
            }

            t_local_scale.x = t_local_scale.x * t_parent.localScale.x;
            t_local_scale.y = t_local_scale.y * t_parent.localScale.y;
            t_local_scale.z = t_local_scale.z * t_parent.localScale.z;

            //			Debug.Log( t_parent.name + ": " + t_parent.localPosition );

            t_parent = t_parent.parent;

            if (t_parent == null)
            {
                Debug.LogError("Error, UIRoot Not Founded.");

                return Vector3.one;
            }
        }

        return t_local_scale;
    }

    public static void CopyTransform(GameObject p_source, GameObject p_destination)
    {
        if (p_source == null)
        {
            Debug.LogError("CopyTransform.Source = null");

            return;
        }

        if (p_destination == null)
        {
            Debug.LogError("CopyTransform.Des = null");

            return;
        }

        p_destination.transform.localPosition = p_source.transform.localPosition;

        p_destination.transform.localScale = p_source.transform.localScale;

        p_destination.transform.localRotation = p_source.transform.localRotation;

    }

    /// <summary>
    /// Ergodic parent's all children
    /// </summary>
    /// <param name="parent">parent</param>
    /// <returns>all children</returns>
    public static List<Transform> ErgodicChilds(Transform parent)
    {
        List<Transform> returnTransforms = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            returnTransforms.Add(parent.GetChild(i));
        }

        foreach (var item in returnTransforms)
        {
            returnTransforms = returnTransforms.Concat(ErgodicChilds(item)).ToList();
        }

        return returnTransforms;
    }

    /// <summary>
    /// Ergodic child's all parents
    /// </summary>
    /// <param name="child">child</param>
    /// <returns>all parents</returns>
    public static List<Transform> ErgodicParents(Transform child)
    {
        if (child == null)
        {
            return null;
        }

        List<Transform> returnTransforms = new List<Transform>();
        Transform targetTransform = child.parent;
        while (targetTransform != null)
        {
            returnTransforms.Add(targetTransform);
            targetTransform = targetTransform.parent;
        }

        return returnTransforms;
    }

    /// <summary>
    /// Find the first child transform with special name. 
    /// </summary>
    /// <param name="parent">The parent tranfrom of the child which will be found.</param>
    /// <param name="objName">The name of the child transfrom.</param>
    /// <returns>The transfrom to be found, null if not found.</returns>
    public static Transform FindChild(Transform parent, string objName)
    {
        if (parent.name == objName)
        {
            return parent;
        }
        return (from Transform item in parent select FindChild(item, objName)).FirstOrDefault(child => child != null);
    }

    /// <summary>
    /// Find the first parent transform with special name. 
    /// </summary>
    /// <param name="child">The child tranfrom of the parent which will be found.</param>
    /// <param name="objName">The name of the child transfrom.</param>
    /// <returns>The transfrom to be found, null if not found.</returns>
    public static Transform FindParent(Transform child, string objName)
    {
        if (child == null)
        {
            return null;
        }
        return child.name == objName ? child : FindParent(child.parent.transform, objName);
    }

    /// <summary>
    /// Get the first parent specific component, for unity elder version in used, don't use GameObject.GetComponentInParent().
    /// </summary>
    /// <typeparam name="T">generic variable which inherited from monobehaviour</typeparam>
    /// <param name="child">The child tranfrom.</param>
    /// <returns>The component to be found, null if not found.</returns>
    public static T GetComponentInParent<T>(Transform child) where T : MonoBehaviour
    {
        if (child == null)
        {
            return null;
        }
        return child.GetComponent<T>() ?? GetComponentInParent<T>(child.parent.transform);
    }

    /// <summary>
    /// Set parent's child num to specific num, standardize automaticlly.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="prefabObject">child prefab</param>
    /// <param name="num">specific num</param>
    public static void AddOrDelItem(Transform parentTransform, GameObject prefabObject, int num)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:" + num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount != num)
            {
                var child = parentTransform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
        }
        else if (parentTransform.childCount < num)
        {
            while (parentTransform.childCount != num)
            {
                var child = Instantiate(prefabObject) as GameObject;

                if (child == null)
                {
                    Debug.LogError("Fail to instantiate prefab, abort.");
                    return;
                }

                ActiveWithStandardize(parentTransform, child.transform);
            }
        }
    }

    /// <summary>
    /// Set parent's child num to specific num, using pool manager, standardize automaticlly.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="num">specific num</param>
    /// <param name="poolList">pool list</param>
    /// <param name="poolPrefabKey">which pool prefab to use</param>
    public static void AddOrDelItemUsingPool(Transform parentTransform, int num, PoolManagerListController poolList, string poolPrefabKey)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:" + num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount != num)
            {
                var child = parentTransform.GetChild(0);
                child.parent = null;
                poolList.ReturnItem(poolPrefabKey, child.gameObject);
            }
        }
        else if (parentTransform.childCount < num)
        {
            while (parentTransform.childCount != num)
            {
                var child = poolList.TakeItem(poolPrefabKey);

                if (child == null)
                {
                    Debug.LogError("Fail to instantiate prefab, abort.");
                    return;
                }

                ActiveWithStandardize(parentTransform, child.transform);
            }
        }
    }

    /// <summary>
    /// Set default transform and active.
    /// </summary>
    /// <param name="parent">parent transform</param>
    /// <param name="targetChild">transform standardized</param>
    public static void ActiveWithStandardize(Transform parent, Transform targetChild)
    {
        targetChild.transform.parent = parent;
        targetChild.transform.localPosition = Vector3.zero;
        targetChild.transform.localEulerAngles = Vector3.zero;
        targetChild.transform.localScale = Vector3.one;
        targetChild.gameObject.SetActive(true);
    }

    #endregion



    #region GameObject

    public static void RemoveAllChildrenDeeply(GameObject p_gb, bool p_remove_self = false)
    {
        if (p_gb == null)
        {
            Debug.LogWarning("Error in RemoveAllChildrenDeeply, p_gb = null.");

            return;
        }

        int t_child_count = p_gb.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gb.transform.GetChild(i);

                RemoveAllChildrenDeeply(t_child.gameObject, true);
            }

            if (p_remove_self)
            {
                p_gb.SetActive(false);

                Destroy(p_gb);
            }
        }
    }

    #endregion



    #region Format

    /** Params:
     * p_float:		origin float
     * p_precision:	precision count
 	 *
     * Example:
     * FloatPrecision( 0.123456f, 2 ) -> 0.12
     */
    public static float FloatPrecision(float p_float, int p_precision)
    {
        if (p_precision < 0)
        {
            return p_float;
        }

        int t_count = 1;

        for (int i = 0; i < p_precision; i++)
        {
            t_count *= 10;
        }

        int t_time_int = (int)(p_float * t_count);

        float t_time = t_time_int * 1.0f / t_count;

        return t_time;
    }

    /// <summary>
    /// Get string bytes.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetBytesNumOfString(string str)
    {
        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(str);
        int n = 0;
        for (int i = 0; i < bytes.GetLength(0); i++)
        {
            if (i % 2 == 0)
            {
                n++;
            }
            else
            {
                if (bytes[i] > 0)
                {
                    n++;
                }
            }
        }
        return n;
    }

    /// <summary>
    /// Get string before index by bytes.
    /// </summary>
    /// <param name="origStr">original string</param>
    /// <param name="Index">get bytes before index</param>
    /// <returns>cutted string</returns>
    public static string GetSubStringWithByteIndex(string origStr, int Index)
    {
        if (string.IsNullOrEmpty(origStr) || Index < 0)
        {
            return null;
        }
        int bytesCount = System.Text.Encoding.GetEncoding("utf-8").GetByteCount(origStr);
        if (bytesCount > Index)
        {
            int readyLength = 0;
            for (int i = 0; i < origStr.Length; i++)
            {
                var byteLength = System.Text.Encoding.GetEncoding("utf-8").GetByteCount(new char[] { origStr[i] });
                readyLength += byteLength;
                if (readyLength == Index)
                {
                    origStr = origStr.Substring(0, i + 1);
                    break;
                }
                else if (readyLength > Index)
                {
                    origStr = origStr.Substring(0, i);
                    break;
                }
            }
        }
        return origStr;
    }
    #endregion



    #region Platform Path

    /// Params:
    /// p_path: file://E:/WorkSpace_External/DynastyMobile_2014/Assets/StreamingAssets/Android/Resources/Data/BattleField/BattleFlags/BattleFlags_-22f14f9d
    /// 
    /// return: BattleFlags_-22f14f9d
    public static string GetFileNameFromPath(string p_path)
    {
        int t_index = p_path.LastIndexOf('/');

        if (t_index < 0)
        {
            return p_path;
        }
        else
        {
            return p_path.Substring(t_index + 1);
        }
    }

    /** Params:
     * 1.p_relative_path:		"StreamingAssets/UIResources/MemoryTrace/MemoryTrace";
     */
    public static string GetFullPath_WithRelativePath(string p_res_relative_path)
    {
        // check first '/'
        if (!p_res_relative_path.StartsWith("/"))
        {
            p_res_relative_path = "/" + p_res_relative_path;
        }

        while (p_res_relative_path.StartsWith("/Assets"))
        {
            p_res_relative_path = RemovePrefix(p_res_relative_path, "/Assets");
        }

        return Application.dataPath + p_res_relative_path;
    }

    /* Params:
     * 1.p_res_relative_path: 	"Unfiled";
     * 
     * Return:			"Assets/StreamingAssets/PlatformX/Unfiled";
     */
    public static string GetConfigRelativePath_WithRelativePath(string p_config_relative_path_name)
    {
        string t_config_prefix = Application.streamingAssetsPath;

        // Add platform prefix
        {
            string t_platform = "/";

            t_platform = t_platform + UtilityTool.GetPlatformTag();

            t_config_prefix = t_config_prefix + t_platform;
        }

        return t_config_prefix + "/" + p_config_relative_path_name;
    }

    /* Return:	iOS/Android/Windows
     */
    public static string GetPlatformTag()
    {
        RuntimePlatform t_runtime_platform = Application.platform;

        string t_platform = "";

        if (t_runtime_platform == RuntimePlatform.WindowsEditor)
        {
            t_platform = t_platform + GetAndroidTag();
        }
        else if (t_runtime_platform == RuntimePlatform.Android)
        {
            t_platform = t_platform + GetAndroidTag();
        }
        else if (t_runtime_platform == RuntimePlatform.OSXEditor)
        {
            t_platform = t_platform + GetiOSTag();
        }
        else if (t_runtime_platform == RuntimePlatform.IPhonePlayer)
        {
            t_platform = t_platform + GetiOSTag();
        }
        else if (t_runtime_platform == RuntimePlatform.WindowsPlayer)
        {
            t_platform = t_platform + GetWindowsTag();
        }
        else
        {
            Debug.LogError("TargetPlatform Error: " + t_runtime_platform);
        }

        return t_platform;
    }

    /// "Android"
    public static string GetAndroidTag()
    {
        return "Android";
    }

    /// "iOS"
    public static string GetiOSTag()
    {
        return "iOS";
    }

    public static string GetWindowsTag()
    {
        return "Windows";
    }

    /// Desc:
    /// Asset Path for WWW.Load.
    ///
    /// Params:
    /// p_bundle_key:	"_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab";
    /// 
    /// return:
    /// OS.dataPath/Platform/_Project/ArtAssets/UIs/_CommonAtlas/Atlases/Atlas_Dict/fnt_big_button_prefab
    public static string GeStreamingAssetWWWPath(string p_res_asset_path)
    {
        {
            p_res_asset_path = UtilityTool.RemovePrefix(p_res_asset_path, "/");

            p_res_asset_path = "/" + p_res_asset_path;
        }

#if UNITY_EDITOR
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return "file://" + Application.streamingAssetsPath + p_res_asset_path;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return "file://" + Application.dataPath + "/StreamingAssets" + p_res_asset_path;
        }
#else
		if (Application.platform == RuntimePlatform.WindowsPlayer ){
			return "file://" + Application.streamingAssetsPath + p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.Android ){
			// Android
			return "jar:file://" + Application.dataPath + "!/assets" + p_res_asset_path;
		}
		else if( Application.platform == RuntimePlatform.IPhonePlayer ){
			// iOS 
			return "file://" + Application.dataPath + "/Raw" + p_res_asset_path;
		}
		
#endif

        return null;
    }

    #endregion



    #region File

    /** Params:
	* p_file_name: Local_File.bin
	*/
    public static string GetPersistentFilePath(string p_file_name)
    {
        string t_local_file_name = Application.persistentDataPath + "/" + p_file_name;

        return t_local_file_name;
    }

    /** Params:
    * p_file_name: Local_File.bin
    */
    public static System.IO.FileStream GetPersistentFileStream(string p_file_name)
    {
        string t_local_file_name = GetPersistentFilePath(p_file_name);

        System.IO.FileStream t_stream = new System.IO.FileStream(t_local_file_name,
                                                                 System.IO.FileMode.OpenOrCreate);

#if UNITY_IPHONE
		UnityEngine.iOS.Device.SetNoBackupFlag( t_local_file_name );
#endif

        return t_stream;
    }

    /// Params:
    /// p_file_name: Local_File.bin
    public static void DeletePersistentFileStream(string p_file_name)
    {
		string t_local_file_name = GetPersistentFilePath ( p_file_name );

		if (System.IO.File.Exists( t_local_file_name ) ) {
			System.IO.File.Delete( t_local_file_name );
		}
    }

    public static string ReadString(System.IO.FileStream p_stream)
    {
        byte[] t_bytes = new byte[p_stream.Length];

        p_stream.Read(t_bytes, 0, (int)p_stream.Length);

        string t_str = Encoding.UTF8.GetString(t_bytes);

        return t_str;
    }

    public static void WriteString(System.IO.FileStream p_stream, string p_data)
    {
        byte[] t_bytes = Encoding.UTF8.GetBytes(p_data);

        p_stream.Write(t_bytes, 0, t_bytes.Length);
    }

    /// Params:
    /// p_path: Application.dataPath + "/Resources/_Data/Config/Test/action.txt"
    public static void OutputFile(string p_path, string p_text)
    {
        FileStream t_file_stream = null;

        if (File.Exists(p_path))
        {
            t_file_stream = new FileStream(p_path, FileMode.Truncate);
        }
        else
        {
            t_file_stream = new FileStream(p_path, FileMode.Create);
        }

        StreamWriter t_stream_writer = new StreamWriter(
            t_file_stream,
            Encoding.Default);

        t_stream_writer.Write(p_text);

        t_stream_writer.Close();

        t_file_stream.Close();
    }

    #endregion



    #region Load

    public static void LoadStringStringDict(Dictionary<string, string> p_dict, TextAsset p_text, char p_splitter)
    {
        string[] t_lines = p_text.text.Split('\n');

        foreach (string t_line in t_lines)
        {
            string[] t_pair = t_line.Split(p_splitter);

            if (t_pair.Length == 2)
            {
                if (!p_dict.ContainsKey(t_pair[0].Trim()))
                {
                    p_dict.Add(t_pair[0].Trim(), t_pair[1].Trim());
                }
            }
            else
            {
                //				Debug.LogWarning( "Parse Error: " + t_line );
            }
        }
    }

    #endregion



    #region GUI

    /** Desc:
	 * Get GUI Rect with index and params[ 6 ].
	 * 
	 * Params:
	 * p_offset_x: offset x
	 * p_offset_y: offset y
	 * p_size_x: size x
	 * p_size_y: size y
	 * p_delta_x: delta x
	 * p_delta_y: delta y
	 */
    public static Rect GetGUIRect(int p_index, float[] p_params)
    {
        return GetGUIRect(p_index,
                          p_params[0], p_params[1],
                          p_params[2], p_params[3],
                          p_params[4], p_params[5]);
    }

    /** Params:
     * p_index: item index
     * p_offset_x: offset x
     * p_offset_y: offset y
     * p_size_x: size x
     * p_size_y: size y
     * p_delta_x: delta x
     * p_delta_y: delta y
     */
    public static Rect GetGUIRect(int p_index,
                                  float p_offset_x, float p_offset_y,
                                  float p_size_x, float p_size_y,
                                  float p_delta_x, float p_delta_y)
    {
        return new Rect(p_offset_x + p_index * p_delta_x, p_offset_y + p_index * p_delta_y,
                        p_size_x, p_size_y);
    }

    #endregion



    #region NGUI

    /// instantiate p_prefab and add it to p_parent's child.
    /// 
    /// Notes:
    /// 1.will also change it's layer to p_parent's.
    public static GameObject AddChild(GameObject p_parent, GameObject p_prefab)
    {
        GameObject t_gb = (GameObject)Instantiate(p_prefab);

        if (t_gb != null && p_parent != null)
        {
            Transform t = t_gb.transform;

            t.parent = p_parent.transform;

            t.localPosition = Vector3.zero;

            t.localRotation = Quaternion.identity;

            t.localScale = Vector3.one;

            t_gb.layer = p_parent.layer;
        }

        return t_gb;
    }

    public static GameObject GetUIRoot(GameObject p_game_object)
    {
        if (p_game_object == null)
        {
            Debug.LogError("Error, ngui gb = null.");

            return null;
        }

        Transform t_parent = p_game_object.transform.parent;

        while (t_parent != null)
        {
            if (t_parent.gameObject.GetComponent<UIRoot>() != null)
            {
                break;
            }

            t_parent = t_parent.parent;

            if (t_parent == null)
            {
                Debug.LogError("Error, UIRoot Not Founded.");

                return null;
            }
        }

        return t_parent.gameObject;
    }

    public static void SetScrollBarValue(UIScrollView view,UIScrollBar bar,float var)
    {
        if (var > 1 || var < 0)
        {
            Debug.LogError("Error setting in scroll bar.");
            return;
        }

        view.UpdateScrollbars(true);
        bar.value = var;
    }

    /// <summary>
    /// Adapt widget in verticle scroll view.
    /// </summary>
    /// <param name="scrollView">scroll view</param>
    /// <param name="scrollBar">scroll bar</param>
    /// <param name="widget">widget</param>
    public static void AdaptWidgetInScrollView(UIScrollView scrollView,UIScrollBar scrollBar,UIWidget widget)
    {
        //adapt pop up buttons to scroll view.
        float widgetValue = scrollView.GetWidgetValueRelativeToScrollView(widget).y;
        if (widgetValue < 0 || widgetValue > 1)
        {
            scrollView.SetWidgetValueRelativeToScrollView(widget, 0);

            //clamp scroll bar value.
            //donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            float scrollValue = scrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) scrollBar.value = scrollBar.value == 1.0f ? 0.99f : 1.0f;
            if (scrollValue <= 0) scrollBar.value = scrollBar.value == 0f ? 0.01f : 0f;
        }
    }

    #endregion



	#region Math

    public class SegmentInFoldLine
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        public float Distance;
        public float PerviousPercentInTotal;
        public float Percent;
    }

    public static Vector2 GetPointFromFoldLine(float precent, List<Vector2> positionList)
    {
        List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
        if (temp == null)
        {
            return Vector2.zero;
        }

        return GetPointFromSegmentLine(precent, temp);
    }

    public static List<SegmentInFoldLine> GetSegmentListFromFoldLine(List<Vector2> positionList)
    {
        if (positionList == null || positionList.Count < 2)
        {
            Debug.LogError("Cannot get point cause position number less than 2.");
            return null;
        }

        List<SegmentInFoldLine> temp = new List<SegmentInFoldLine>();
        for (int i = 0; i < positionList.Count - 1; i++)
        {
            temp.Add(new SegmentInFoldLine()
            {
                StartPoint = positionList[i],
                EndPoint = positionList[i + 1],
            });
        }

        temp.ForEach(item => item.Distance = Vector2.Distance(item.StartPoint, item.EndPoint));
        float TotalDistance = temp.Select(item => item.Distance).Sum();
        temp.ForEach(item => item.Percent = item.Distance / TotalDistance);

        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].PerviousPercentInTotal = i > 0 ? (temp[i - 1].Percent + temp[i - 1].PerviousPercentInTotal) : 0;
        }

        return temp;
    }

    public static Vector2 GetPointFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
    {
        SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();

        //Debug.LogWarning(precent + "," + (precent - tempLine.PerviousPercentInTotal)/tempLine.Percent + "," + tempLine.StartPoint);
        return Vector2.Lerp(tempLine.StartPoint, tempLine.EndPoint, (precent - tempLine.PerviousPercentInTotal) / tempLine.Percent);
    }

    public static Vector2 GetDirectionFromFoldLine(float precent, List<Vector2> positionList)
    {
        List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
        if (temp == null)
        {
            return Vector2.zero;
        }

        return GetDirectionFromSegmentLine(precent, temp);
    }

    public static Vector2 GetDirectionFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
    {
        SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();
        return tempLine.EndPoint - tempLine.StartPoint;
    }

    #endregion



    #region Layer

    public static void SetGameObjectLayer(GameObject p_target_layer_gb, GameObject p_gameobject)
    {
		if( p_gameobject == null ){
			Debug.LogError( "p_gameobject = null." );

			return;
		}

		if(p_target_layer_gb == null ){
			Debug.LogError( "p_target_layer_gb = null." );

			return;
		}

        int t_child_count = p_gameobject.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gameobject.transform.GetChild(i);

                t_child.gameObject.layer = p_target_layer_gb.layer;

                SetGameObjectLayer(p_target_layer_gb, t_child.gameObject);
            }

            p_gameobject.layer = p_target_layer_gb.layer;
        }
    }

    #endregion



    #region Message

    /// <summary>
    /// Send QX Serialized message to server.
    /// </summary>
    /// <param name="value">message to send</param>
    /// <param name="protoIndex">message index</param>
    public static void SendQXMessage(object value, int protoIndex)
    {
        MemoryStream memStream = new MemoryStream();

        QiXiongSerializer qxSer = new QiXiongSerializer();
        qxSer.Serialize(memStream, value);
        byte[] t_protof = memStream.ToArray();

        SocketTool.Instance().SendSocketMessage((short)protoIndex, ref t_protof);
    }

    /// <summary>
    /// Send QX message index to server.
    /// </summary>
    /// <param name="protoIndex">message index</param>
    public static void SendQXMessage(int protoIndex)
    {
        SocketTool.Instance().SendSocketMessage((short)protoIndex);
    }

    /// <summary>
    /// Transfer source message to target object, used in receiving message from sever.
    /// </summary>
    /// <param name="targetObject">target object</param>
    /// <param name="p_message">source message</param>
    /// <returns>true if transfer succeed, false if not</returns>
    public static bool ReceiveQXMessage(ref object targetObject, QXBuffer p_message, int index)
    {
        if (p_message == null || p_message.m_protocol_index != index)
        {
            return false;
        }

        //Execute received msg.
        MemoryStream memStream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
        QiXiongSerializer qxSer = new QiXiongSerializer();
        qxSer.Deserialize(memStream, targetObject, targetObject.GetType());

        return true;
    }

    #endregion



	#region File Log

	private const string LOG_FILE_NAME = "Log";

	public static void DeleteLogFile(){
//		Debug.Log ( "DeleteLogFile( " + GetPersistentFilePath( LOG_FILE_NAME ) + " )" );

		DeletePersistentFileStream ( LOG_FILE_NAME );
	}

	public static void LogFile( string p_log, string p_stack, LogType p_type ){
		string t_log_string = p_type + ": " + p_log + 
			"\n" + p_stack + 
				"\n";
		
//		Debug.Log ( "LogFile( " + t_log_string + " )" );

		System.IO.FileStream t_stream = UtilityTool.GetPersistentFileStream( LOG_FILE_NAME );

		t_stream.Position = t_stream.Length;

		UtilityTool.WriteString( t_stream, t_log_string );
		
		t_stream.Close();
	}

	#endregion



    #region GC

    public void DelayedUnloadUnusedAssets(){
        StartCoroutine( Exec_DelayedUnloadUnusedAssets( 0.5f ) );
    }

    IEnumerator Exec_DelayedUnloadUnusedAssets( float p_delay ){
        yield return p_delay;

        UnloadUnusedAssets();
    }

    public static void UnloadUnusedAssets(){
//		Debug.Log( "UtilityTool.UnloadUnusedAssets()" );

        Resources.UnloadUnusedAssets();

        System.GC.Collect();
    }

    #endregion



    #region Collider

    /// Clear All Colliders under p_gb and its' children.
    public static void ClearColliders(GameObject p_gb)
    {
        if (p_gb == null)
        {
            Debug.LogWarning("Error in ClearColliders, p_gb = null.");

            return;
        }

        int t_child_count = p_gb.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gb.transform.GetChild(i);

                ClearColliders(t_child.gameObject);
            }

            {
                Collider2D[] t_colliders = p_gb.GetComponents<Collider2D>();

                for (int i = t_colliders.Length - 1; i >= 0; i--)
                {
                    Collider2D t_collider = t_colliders[i];

                    Destroy(t_collider);
                }
            }

            {
                Collider[] t_colliders = p_gb.GetComponents<Collider>();

                for (int i = t_colliders.Length - 1; i >= 0; i--)
                {
                    Collider t_collider = t_colliders[i];

                    Destroy(t_collider);
                }
            }
        }
    }

    /// Disable All Colliders under p_gb and its' children.
    public static void DisableColliders(GameObject p_gb)
    {
        if (p_gb == null)
        {
            Debug.LogWarning("Error in DisableColliders, p_gb = null.");

            return;
        }

        int t_child_count = p_gb.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gb.transform.GetChild(i);

                DisableColliders(t_child.gameObject);
            }

            {
                Collider2D[] t_colliders = p_gb.GetComponents<Collider2D>();

                for (int i = 0; i < t_colliders.Length; i++)
                {
                    Collider2D t_collider = t_colliders[i];

                    t_collider.enabled = false;
                }
            }

            {
                Collider[] t_colliders = p_gb.GetComponents<Collider>();

                for (int i = 0; i < t_colliders.Length; i++)
                {
                    Collider t_collider = t_colliders[i];

                    t_collider.enabled = false;
                }
            }

            {
                iTween.Stop(p_gb);
            }
        }
    }

    public static void StopITweens(GameObject p_gb)
    {
        if (p_gb == null)
        {
            Debug.LogWarning("Error in ClearColliders, p_gb = null.");

            return;
        }

        int t_child_count = p_gb.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gb.transform.GetChild(i);

                StopITweens(t_child.gameObject);
            }

            {
                iTween.Stop(p_gb);
            }
        }
    }

    /// Clears All Monos without NGUI under p_gb and its' children.
    public static void ClearMonosWithoutNGUI(GameObject p_gb)
    {
        if (p_gb == null)
        {
            Debug.LogWarning("Error in ClearMonosWithoutNGUI, p_gb = null.");

            return;
        }

        int t_child_count = p_gb.transform.childCount;

        {
            for (int i = 0; i < t_child_count; i++)
            {
                Transform t_child = p_gb.transform.GetChild(i);

                ClearMonosWithoutNGUI(t_child.gameObject);
            }

            Component[] t_monos = p_gb.GetComponentsInChildren(typeof(MonoBehaviour));

            for (int i = 0; i < t_monos.Length; i++)
            {
                MonoBehaviour t_mono = (MonoBehaviour)t_monos[i];

                if (t_mono is UIWidget ||
                       t_mono is UIAnchor ||
                       t_mono is UIWidgetContainer ||
                       t_mono is UIFont ||
                       t_mono is UIAtlas)
                {
                    continue;
                }
                else
                {
                    t_mono.enabled = false;

                    Destroy(t_mono);
                }
            }
        }
    }

    #endregion



    #region Common Code Error Utility

    private static string m_common_code_error_tag = "";

    private static string m_common_code_error_content = "";

    private static string m_common_code_error = "";

    public static Rect m_common_code_scroll_rect = new Rect(0, 110, 960, 600);

    public static Rect m_common_code_scroll_content_rect = new Rect(0, 0, 960, 640);

    public static Vector2 m_common_code_scroll_pos = Vector2.zero;

    public static void SetCommonCodeError(string p_tag, string p_content)
    {
        m_common_code_error = "Tag: " + p_tag + "\n" +
            "Content: " + p_content;

        {
            m_common_code_scroll_rect.width = Screen.width * 0.8f;

            m_common_code_scroll_rect.height = Screen.height * 0.5f;
        }

        {
            m_common_code_scroll_content_rect.width = Screen.width;

            m_common_code_scroll_content_rect.height = Screen.height;
        }
    }

    public static void ClearCommonCodeError()
    {
        m_common_code_error = "";
    }

    public static string GetCommonCodeError()
    {
        return m_common_code_error;
    }

    #endregion



    #region Config Value

    public static bool GetBool(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, bool p_default_value = false)
    {
        if (!p_dict.ContainsKey(p_key))
        {
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].m_bool;
    }

    public static int GetInt(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, int p_default_value = 0)
    {
        if (!p_dict.ContainsKey(p_key))
        {
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].m_int;
    }

    public static float GetFloat(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, float p_default_value = 0f)
    {
        if (!p_dict.ContainsKey(p_key))
        {
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].m_float;
    }

    public static string GetString(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, string p_default_value = "")
    {
        if (!p_dict.ContainsKey(p_key))
        {
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].m_string;
    }

    public static string ValueToString(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, string p_default_value = "")
    {
        if (!p_dict.ContainsKey(p_key))
        {
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].ValueToString();
    }

    #endregion



    #region FPS

    public static string GetFPSString()
    {
        if (ConfigTool.Instance == null)
        {
            return "";
        }

        if (ConfigTool.Instance.m_fps_counter == null)
        {
            return "";
        }

        return ConfigTool.Instance.m_fps_counter.GetFPSString();
    }

    public static float GetFPSFloat()
    {
        if (ConfigTool.Instance == null)
        {
            return 0;
        }

        if (ConfigTool.Instance.m_fps_counter == null)
        {
            return 0;
        }

        return ConfigTool.Instance.m_fps_counter.GetFPSFloat();
    }

    public static int GetFPSInt()
    {
        if (ConfigTool.Instance == null)
        {
            return 0;
        }

        if (ConfigTool.Instance.m_fps_counter == null)
        {
            return 0;
        }

        return ConfigTool.Instance.m_fps_counter.GetFPSInt();
    }

    #endregion



    #region Utilities

    public static string AddPrefix(string p_string, string p_prefix)
    {
        if (!p_string.StartsWith(p_prefix))
        {
            return p_prefix + p_string;
        }

        return p_string;
    }

    public static string RemovePrefix(string p_string, string p_prefix)
    {
        if (p_string.StartsWith(p_prefix))
        {
            p_string = p_string.Substring(p_prefix.Length);
        }

        return p_string;
    }

    public static string RemoveSurfix(string p_string, string p_surfix)
    {
        if (p_string.Contains(p_surfix))
        {
            int t_count = p_string.Length - p_surfix.Length;

            if (t_count == p_string.LastIndexOf(p_surfix))
            {
                p_string = p_string.Substring(0, t_count);
            }
            else
            {
                Debug.LogError("Error, " + p_string + " not end with " + p_surfix);
            }
        }

        return p_string;
    }

    /** Desc:
     * 
     * Params:
     * 1.p_surfixes:	.jpg#.prefab#.png;
     */
    public static bool IsEndWith(string p_file_name, string p_surfixes)
    {
        char[] t_splitters = { '#' };

        string[] t_surfixes = p_surfixes.Split(t_splitters);

        for (int i = 0; i < t_surfixes.Length; i++)
        {
            if (p_file_name.ToLowerInvariant().EndsWith(t_surfixes[i].ToLowerInvariant()))
            {
                return true;
            }
        }

        return false;
    }



    public static string GetStringTime(int p_sec)
    {
        int t_sec = p_sec % 60;

        int t_min = p_sec / 60;

        int t_hour = t_min / 60;

        t_min = t_min % 60;

        return t_hour + ":" + t_min + ":" + t_sec;
    }

    public static float GetRandom(float p_min_inc, float p_max_inc)
    {
        return (p_max_inc - p_min_inc) * Random.value + p_min_inc;
    }

    public static bool IsLevelLoaded( string p_scene_name ){
        return Application.loadedLevelName == p_scene_name;
    }

    public static void QuitGame(){
        Debug.Log("QuitGame()");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    public static string FullNumWithZeroDigit(int ori, int maxLength)
    {
        int length = ori.ToString().Length;
        int zeroToAdd = maxLength - length;

        if (zeroToAdd < 0)
        {
            Debug.LogError("Length error when full number.");
            return null;
        }

        string returnStr = "";

        while (zeroToAdd > 0)
        {
            returnStr += "0";
            zeroToAdd--;
        }
        returnStr += ori;

        return returnStr;
    }

    #endregion
}