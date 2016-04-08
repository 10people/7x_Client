//#define DEBUG_GC

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
    void Start(){

    }

    void FixedUpdate(){

    }

    // Update is called once per frame
    void Update(){
		{
			ComponentHelper.GlobalClassUpdate();

			ObjectHelper.OnUpdate();

			SocketHelper.UpdateErrorBoxes();

//			SoundHelper.OnUpdate();
		}

		#if UNITY_ANDROID
		if( Input.GetKeyDown( KeyCode.Escape ) ){
			Debug.Log( "Android KeyCode.Escape()" );
		
			OnQuitTips();
		}
		#endif

		#if UNITY_STANDALONE || UNITY_EDITOR
		if( Input.GetKeyDown( KeyCode.Escape ) ){
//			Debug.Log( "StandAlone KeyCode.Escape()" );
			
			OnQuitTips();
		}
		#endif

    }

    void LateUpdate(){

    }

    void OnGUI(){

    }

    void OnApplicationFocus( bool p_focused ){
//		Debug.Log( "UtilityTool.OnApplicationFocus( " + p_focused + " )" );
    }

    void OnApplicationPause(bool p_pause){
//		Debug.Log( "UtilityTool.OnApplicationPause( " + p_pause + " )" );

        if ( p_pause ){
			// clean
//			{
//				UtilityTool.Instance.DelayedUnloadUnusedAssets();
//			}

        }

        // send message
        {
            if (SocketTool.IsConnected()){
                if (p_pause){
                    //					Debug.Log( "Socket Send Game Pause." );

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.GAME_PAUSE, "");
                }
                else{
                    //					Debug.Log( "Socket Send Game Continue." );

                    SocketTool.Instance().SendSocketMessage(ProtoIndexes.GAME_CONTINUE, "");
                }
            }
        }
    }

	void OnLevelWasLoaded(int level) {
//		Debug.Log( "OnLevelWasLoaded( " + level + " )" );

//		Debug.Log( "Application.Info( " + Application.loadedLevel + ", " + Application.loadedLevelName + " )" );

//		{
//			LoadingHelper.ConfigBloomAndLight();
//		}
	}

    void OnDestroy(){
		base.OnDestroy();
	}
	
	#endregion



    #region Game Pause

    private bool m_is_game_paused = false;

    public void ManualGamePause(){
        m_is_game_paused = !m_is_game_paused;

        OnApplicationPause(m_is_game_paused);
    }

    #endregion


	#region Text

	

    private static Object m_cached_box_obj;

    public static void LoadBox(){
        //		Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CachedBoxCallback );

//		Debug.Log( "UtilityTool.LoadBox()" );

        // Updated 2015.7.4, Bundle Load Use.
        Global.ResourcesDotLoad( "New/Box", CachedBoxCallback );
    }

    private static void CachedBoxCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
//		Debug.Log( "UtilityTool.CachedBoxCallback( " + p_path + " - " + p_object + " )" );

        m_cached_box_obj = p_object;
    }

	public GameObject CreateBox( 
			string p_title, 
			string p_des_1, string p_des_2,
			List<BagItem> p_bag_item, 
			string p_btn_name_1, string p_btn_name_2, 
			UIBox.onclick p_click_delegate, UIBox.OnBoxCreated p_on_create = null,
			UIFont uifontButton1 = null, UIFont uifontButton2 = null, 
			bool isShowBagItemNumBelow = false, 
			bool isSetDepth = true, bool isBagItemTop = true, bool isFunction = false ){
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

			Debug.Log( "isBagItemTop: " + isFunction );

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
	                   isBagItemTop,
		               isFunction);
    }

	GameObject ExecLoadBox(string tile, string dis1, string dis2, List<BagItem> bagItem, string buttonname1, string buttonname2, UIBox.onclick onClcik, UIBox.OnBoxCreated p_on_create, UIFont uifontButton1 = null, UIFont uifontButton2 = null, bool isShowBagItemNumBelow = false, bool isSetDepth = true, bool isBagItemTop = true, bool isFunction = false ){
        if ( m_cached_box_obj == null ){
            Debug.LogError("Error, No Cached box.");

            LoadBox();

            return null;
        }

        GameObject t_gb = GameObject.Instantiate(m_cached_box_obj) as GameObject;

        if (p_on_create != null){
            p_on_create(t_gb);
        }

        UIBox uibox = (t_gb).GetComponent<UIBox>();

		uibox.setBox(tile, dis1, dis2, bagItem, buttonname1, buttonname2, onClcik, null,
		             uifontButton1,
		             uifontButton2,
		             isShowBagItemNumBelow,
		             isSetDepth,
		             isBagItemTop,
		             isFunction);

		return t_gb;
    }

    #endregion



    #region Resources Load

	public void Init(){

	}

    public void ExecResourcesLoad(string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list, bool p_open_simulate){
        StartCoroutine(ResoucesDotLoad(p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate));
    }

    private IEnumerator ResoucesDotLoad(string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list, bool p_open_simulate){


        yield return null;
    }

    #endregion



    #region Resources Load

    /**
	 * Get List<EventDelegate>, if p_callback = null, then list.size = 0.
	 * if not null, return the p_callback conained list.
	 */
    public static List<EventDelegate> GetEventDelegateList(EventDelegate.Callback p_callback){
        List<EventDelegate> t_list = new List<EventDelegate>();

        EventDelegate.Add(t_list, p_callback);

        return t_list;
    }

    public void ExecLoadCallback(Bundle_Loader.LoadResourceDone p_delegate, UnityEngine.Object t_object, string p_resource, List<EventDelegate> p_callback_list){
        WWW t_www = null;

        if (p_delegate != null){
            //				Debug.Log( "Exec Callback." );

            p_delegate(ref t_www, p_resource, t_object);
        }
        else{
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



    #region Format

    /// <summary>
    /// Get string bytes.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    [Obsolete("Finally, i abandon this cause getting real width in chinese/ english mixed string in uilabel is really complex, donot use this any more.")]
    public static float GetBytesNumOfString(string str){
        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(str);
        float n = 0;
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

    #endregion



    #region Dictionary

	public static void UpdateStringStringDictKeyValue( Dictionary<string, string> p_dict, string p_key, string p_value ){
		if( p_dict == null ){
			return;
		}

		if( !p_dict.ContainsKey( p_key ) ){
			p_dict.Add( p_key, p_value );
		}
		else{
			p_dict[ p_key ] = p_value;
		}
	}

	public static void LoadStringStringDict( Dictionary<string, string> p_dict, TextAsset p_text, char p_splitter ){
		LoadStringStringDict( p_dict, p_text.text, p_splitter );
	}

    public static void LoadStringStringDict( Dictionary<string, string> p_dict, string p_text, char p_splitter ){
		if( p_dict == null ){
			return;
		}

		if( string.IsNullOrEmpty( p_text ) ){
			return;
		}

        string[] t_lines = p_text.Split('\n');

		if( t_lines.Length <= 0 ){
			return;
		}

        foreach ( string t_line in t_lines ){
            string[] t_pair = t_line.Split( p_splitter );

            if( t_pair.Length == 2 ){
                if( !p_dict.ContainsKey( t_pair[0].Trim() ) ){
                    p_dict.Add( t_pair[0].Trim(), t_pair[1].Trim() );
                }
            }
            else{
                //				Debug.LogWarning( "Parse Error: " + t_line );
            }
        }
    }

    #endregion



    #region GC

	private static float m_last_gc_time = 0.0f;

	public static void CheckGCTimer(){
//		#if DEBUG_GC
//		Debug.Log( "CheckGCTimer( " + Time.realtimeSinceStartup + " - " + m_last_gc_time + " )" );
//		#endif

		float t_duration = ConfigTool.GetFloat( ConfigTool.CONST_MAINCITY_UI_GC, 180.0f );

		if( Time.realtimeSinceStartup - m_last_gc_time > t_duration ){
			#if DEBUG_GC
			Debug.Log( "GC Time Up: " + Time.realtimeSinceStartup + " - " + m_last_gc_time );
			#endif

			UnloadUnusedAssets();
		}
//		else{
//			#if DEBUG_GC
//			Debug.Log( "Waiting For GC Count Down: " +
//				( Time.realtimeSinceStartup - m_last_gc_time ) + " / " + t_duration + "   " + 
//				Time.realtimeSinceStartup + " - " + m_last_gc_time );
//			#endif
//		}
	}

	public static void UnloadUnusedAssets( bool p_clean_anim = false ){
		#if DEBUG_GC
		Debug.Log( "UtilityTool.UnloadUnusedAssets()" );
		#endif

		{
			m_last_gc_time = Time.realtimeSinceStartup;
		}

		Resources.UnloadUnusedAssets();

		ComponentHelper.UnloadUseless( p_clean_anim );
		
		System.GC.Collect();
	}

    public void DelayedUnloadUnusedAssets(){
		#if DEBUG_GC
		Debug.Log( "UtilityTool.DelayedUnloadUnusedAssets()" );
		#endif

        StartCoroutine( Exec_DelayedUnloadUnusedAssets( 0.5f ) );
    }

    IEnumerator Exec_DelayedUnloadUnusedAssets( float p_delay ){
        yield return p_delay;

        UnloadUnusedAssets();
    }

    #endregion



    #region Config Value

    public static bool GetBool(Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, bool p_default_value = false)
    {
        if( !p_dict.ContainsKey( p_key ) ){
//			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[ p_key ].m_bool;
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

    public static float GetFloat( Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, float p_default_value = 0f ){
        if ( !p_dict.ContainsKey( p_key ) ){
            //			Debug.LogError( "Key Not Contained: " + p_key );

            return p_default_value;
        }

        return p_dict[p_key].m_float;
    }

    public static string GetString( Dictionary<string, ConfigTool.ConfigValue> p_dict, string p_key, string p_default_value = "" ){
        if ( !p_dict.ContainsKey( p_key ) ){
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



	#region Android

	private GameObject m_android_quit_tips_gb = null;

	private void OnQuitTips(){
		if( m_android_quit_tips_gb != null ){
//			Debug.Log( "Tips still there." );

			return;
		}

		m_android_quit_tips_gb = Global.CreateBox( LanguageTemplate.GetText( LanguageTemplate.Text.EXIT_GAME_TIPS_TITLE ), 
						LanguageTemplate.GetText( LanguageTemplate.Text.EXIT_GAME_TIPS ), 
						"",
						null,
						LanguageTemplate.GetText( LanguageTemplate.Text.CONFIRM ), 
						LanguageTemplate.GetText( LanguageTemplate.Text.CANCEL ), 
						OnConfirmQuit,
						null,
						null,
						null,
						false,
						false,
						false );
	}

	public void OnConfirmQuit( int p_int ){
//		Debug.Log( "OnConfirmQuit( " + p_int + " )" );

		if( p_int == 1 ){
			QuitGame();
		}
	}

	#endregion


	#region Coroutine Box

	/** Desc:
	 * Coroutine box will not be destroyed when loading.
	 * 
	 */
	public static void StartCorutineBox(
					string tile, 
					string dis1, 
					string dis2, 
					List<BagItem> bagItem, 
					string buttonname1,

					string buttonname2, 
					UIBox.onclick onClcik, 
					UIBox.OnBoxCreated p_on_create = null, 
					UIFont uifontButton1 = null, 
					UIFont uifontButton2 = null, 

					bool isShowBagItemNumBelow = false, 
					bool isSetDepth = true, 
					bool isBagItemTop = true,
					bool isFunction = false ){
		UtilityTool.Instance.StartCoroutine(
			UtilityTool.Instance.CoroutineBox( tile,
				dis1,
				dis2,
				bagItem,
				buttonname1,

				buttonname2, 
				onClcik, 
				p_on_create, 
				uifontButton1, 
				uifontButton2, 

				isShowBagItemNumBelow, 
				isSetDepth, 
				isBagItemTop,
				isFunction ) );
	}

	private IEnumerator CoroutineBox(
					string tile, 
					string dis1, 
					string dis2, 
					List<BagItem> bagItem, 
					string buttonname1,

					string buttonname2, 
					UIBox.onclick onClcik, 
					UIBox.OnBoxCreated p_on_create = null, 
					UIFont uifontButton1 = null, 
					UIFont uifontButton2 = null, 

					bool isShowBagItemNumBelow = false, 
					bool isSetDepth = true, 
					bool isBagItemTop = true,
					bool isFunction = false ){
		yield return new WaitForEndOfFrame();

		GameObject t_gb = Global.CreateBox( 
			tile,
			dis1,
			dis2,
			bagItem,
			buttonname1,

			buttonname2, 
			onClcik, 
			p_on_create, 
			uifontButton1, 
			uifontButton2, 

			isShowBagItemNumBelow, 
			isSetDepth, 
			isBagItemTop,
			isFunction );
		
		GameObjectHelper.DontDestroyGameObject( t_gb );
	}

	#endregion


    #region Utilities

    public static string GetStringTime(int p_sec){
        int t_sec = p_sec % 60;

        int t_min = p_sec / 60;

        int t_hour = t_min / 60;

        t_min = t_min % 60;

        return t_hour + ":" + t_min + ":" + t_sec;
    }

    public static float GetRandom(float p_min_inc, float p_max_inc){
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

    public static string FullNumWithZeroDigit(int ori, int maxLength){
        int length = ori.ToString().Length;
        int zeroToAdd = maxLength - length;

        if (zeroToAdd < 0){
            Debug.LogError("Length error when full number.");
            return null;
        }

        string returnStr = "";

        while (zeroToAdd > 0){
            returnStr += "0";
            zeroToAdd--;
        }
        returnStr += ori;

        return returnStr;
    }

    private const float SCREEN_OFFSET = 0.2f;

    public static bool IsInScreen(Vector3 p_pos,Camera p_camera)
    {
        Vector3 t_vec = p_camera.WorldToViewportPoint(p_pos);

        if (t_vec.x < -SCREEN_OFFSET || t_vec.x > 1 + SCREEN_OFFSET)
        {
            return false;
        }

        if (t_vec.y < -SCREEN_OFFSET || t_vec.y > 1 + SCREEN_OFFSET)
        {
            return false;
        }

        return true;
    }

    #endregion
}