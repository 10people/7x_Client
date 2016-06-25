//#define RESOURCE_PATH

#define BUNDLE_PATH


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class Global
{
	public static bool m_isOpenPVP = false;//pve

    public static bool m_isOpenBaiZhan = false;//pvp
	public static bool m_isSportDataInItEnd = true;//pvpdata init end

	public static bool m_isOpenHuangYe = false;//huangye

	public static int m_iOpenFunctionIndex = -1;
	public static bool m_isAddZhanli = true;
	public static bool m_isZhanli = false;

	public static int m_iScreenID = 0;

	public static int m_iScreenNoID = 0;

	public static bool m_isOpenJiaoxue = true;

	public static int m_sMainCityWantOpenPanel = -1;
	public static string m_sPanelWantRun = "";
	public static bool m_isShowBianqiang = false;
	public static string m_sBianqiangClick = "";
	public static List<int> m_NewChenghao = new List<int>();
	public static int m_iPChangeZhanli = 0;
	public static int m_iPZhanli = 0;
	public static int m_iAddZhanli = 0;
	public static int m_iCenterZhanli = 0;

	public static bool m_isOpenFuWen = false;//fu wen

	public static bool m_isOpenShop = false;//shang pu

	public static bool m_isOpenPlunder = false;//lue duo

	public static bool m_isCityWarOpen = false;//citywar

	public static bool m_isChangeRoleOpen = false;//切换形象

	public static List<TongzhiData> m_listAllTheData = new List<TongzhiData>();
	public static List<TongzhiData> m_listMainCityData = new List<TongzhiData>();
	public static List<TongzhiData> m_listJiebiaoData = new List<TongzhiData>();
	public static List<TongzhiData> m_listShiLianData = new List<TongzhiData> ();
	public static List<TongzhiData> m_listJunchengData = new List<TongzhiData> ();

	public const int TILILVMAX = 14;

	public static int getBili(int w, float curNum, float maxNum)
	{
		int W = (int)((curNum / maxNum) * w);
		if(W > w)
		{
			W = w;
		}
		return W;
	}

	public static int[] getNum(int num){
        int curNum = 1;
        
		int wei = 1;
        
		while( ( curNum *= 10 ) <= num ){
            wei++;
        }
        curNum /= 10;
        int[] returnNum = new int[wei];
        for (int i = 0; i < wei; i++)
        {
            returnNum[i] = num / curNum;
            num %= curNum;
            curNum /= 10;
        }
        return returnNum;
    }

    public static float getQuadrant(int bPosX, int bPosY, float ePosX, float ePosY)
    {
        if (bPosX > ePosX)
        {
            if (bPosY > ePosY)
            {
                return 270;
            }
            else
            {
                return 180;
            }
        }
        else
        {
            if (bPosY > ePosY)
            {
                return 0;
            }
            else
            {
                return 90;
            }
        }
    }

    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }

    public static string getTimeString(int time)
    {
        if (time < 0)
        {
            return "00:00:00";
        }
        else
        {
            int H = (int)(time / 3600);
            time = time % 3600;
            int M = (int)(time / 60);
            int S = time % 60;
            string returnstring = "";
            if (H < 10)
            {
                returnstring += ("0" + H + ":");
            }
            else
            {
                returnstring += (H + ":");
            }
            if (M < 10)
            {
                returnstring += ("0" + M + ":");
            }
            else
            {
                returnstring += (M + ":");
            }
            if (S < 10)
            {
                returnstring += ("0" + S);
            }
            else
            {
                returnstring += (S);
            }
            return returnstring;
        }
    }

    public static int getRandom(int max)
    {
        return UnityEngine.Random.Range(0, max);
    }
	
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

		GameObjectHelper.SetGameObjectLayer( obj, newLayer );

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }

            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static Transform FindTransform(string name, GameObject Bone)
    {
        if (null == Bone)
        {
            return null;
        }
        if (Bone.name == name)
        {
            return Bone.transform;
        }

        var transforms = Bone.GetComponentsInChildren<Transform>();

        foreach (Transform t in transforms)
        {
            if (t.name == name) return t;
        }

        return null;
    }

    public static List<GameObject> AddSkinnedMeshTo(GameObject obj, Transform root)
    {
        return AddSkinnedMeshTo(obj, root, true);
    }

    public static List<GameObject> AddSkinnedMeshTo(GameObject obj, Transform root, bool hideFromObj)
    {
        List<GameObject> result = new List<GameObject>();

        SkinnedMeshRenderer[] BonedObjects = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in BonedObjects)
        {
            result.Add(ProcessBonedObject(smr, root));
        }

        obj.SetActive(false);

        return result;
    }

    private static GameObject ProcessBonedObject(SkinnedMeshRenderer ThisRenderer, Transform root)
    {
        GameObject newObject = new GameObject(ThisRenderer.gameObject.name);
        newObject.transform.parent = root;

        SkinnedMeshRenderer NewRenderer = newObject.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;

        Transform[] MyBones = new Transform[ThisRenderer.bones.Length];

        for (int i = 0; i < ThisRenderer.bones.Length; i++)
        {
            MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, root);
        }

        NewRenderer.bones = MyBones;
        NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
        NewRenderer.materials = ThisRenderer.materials;
		GameObjectHelper.SetGameObjectLayer( NewRenderer.gameObject, root.gameObject.layer );

        return newObject;
    }

    private static Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        Transform ReturnObj;

        if (ThisGObj.name == ThisName)
        {
            return ThisGObj.transform;
        }

        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);

            if (ReturnObj != null)
            {
                return ReturnObj;
            }
        }

        return null;
    }

    public static string getString(string indexof, string data)
    {
        return data.Substring(data.IndexOf(indexof) + 1, data.Length - (data.IndexOf(indexof) + 1));
    }

    public static GameObject CreateHeroModel(GameObject hero, int worldx, int worldy, int x, int y, int width, int height)
    {
        return CreateHeroModel(hero, worldx, worldy, x, y, width, height, 0.7f);
    }

    public static GameObject CreateHeroModel(GameObject hero, int worldx, int worldy, int x, int y, int width, int height, float localScale)
    {
        Global.SetLayerRecursively(hero, LayerMask.NameToLayer("Default"));
        hero = UnityEngine.Object.Instantiate(hero) as GameObject;
        hero.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        hero.transform.position = new Vector3(0, -1, 5f);
        GameObject tempG = new GameObject("HeroClipCamera", typeof(Camera));
        Camera m_OrthCamera = tempG.GetComponent<Camera>();
        hero.transform.parent = m_OrthCamera.transform;
        m_OrthCamera.orthographic = true;
        m_OrthCamera.orthographicSize = 1;
        m_OrthCamera.transform.position = new Vector3((float)worldx, (float)worldy, 0f);

        m_OrthCamera.nearClipPlane = 0;
        m_OrthCamera.farClipPlane = 100;
        m_OrthCamera.clearFlags = CameraClearFlags.Depth;
//        float scaleInitX = (float)((x - (Screen.width / 2)) * MYNGUIManager.m_fScale + (Screen.width / 2)) / Screen.width;
//        float scaleInitY = (float)((y - (Screen.height / 2)) * MYNGUIManager.m_fScale + (Screen.height / 2)) / Screen.height;
//        width = (int)(width * MYNGUIManager.m_fScale);
//        height = (int)(height * MYNGUIManager.m_fScale);
//        m_OrthCamera.camera.rect = new Rect(scaleInitX, 1f - (((float)height / Screen.height) + scaleInitY), ((float)width / Screen.width), ((float)height / Screen.height));
//        hero.transform.localScale = new Vector3(localScale, localScale, localScale);
        return tempG;
    }

	/****
	 * @tile 标题
	 * @dis1 上介绍
	 * @dis2 下介绍
	 * @bagItem 物品
	 * @buttonname1 按钮1的名字
	 * @buttonname2 按钮2的名字
	 * @onClcik 点击后的回调
	 * 
	 * isSetDepth: true ---> Cam.Depth = 45
	 * isSetDepth: false ---> Cam.Depth = 100
	 */
	public static GameObject CreateBox( string tile, 
		string dis1, string dis2, 
		List<BagItem> bagItem, string buttonname1, string buttonname2,
		UIBox.onclick onClcik, UIBox.OnBoxCreated p_on_create = null, 
		UIFont uifontButton1 = null, UIFont uifontButton2 = null, 
		bool isShowBagItemNumBelow = false, 
		bool isSetDepth = true, bool isBagItemTop = true,
		bool isFunction = false,
		int p_window_id = UIWindowEventTrigger.DEFAULT_POP_OUT_WINDOW_ID,
	    int vip0 = 0, int vip1 = 0){
		return UtilityTool.Instance.CreateBox(
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
			isFunction,
			p_window_id,
			vip0,
			vip1);
	}

	public static GameObject CreateFunctionIcon(int id)
	{
		GameObject temp = GameObject.Instantiate(UtilityTool.m_cached_functionJump_obj) as GameObject;

		UIFunctionJump functionJump = (temp).GetComponent<UIFunctionJump>();

		functionJump.setDate(id);

		MainCityUI.TryAddToObjectList(temp);

		return temp;
	}

	public static GameObject GetObj(ref GameObject obj, string name)
	{
		Transform[] tempOBJS = obj.GetComponentsInChildren<Transform>();
		for(int i = 0; i < tempOBJS.Length; i ++)
		{
			if(tempOBJS[i].name == name)
			{
				return tempOBJS[i].gameObject;
			}
		}
		return null;
	}

	public static Color getStringColor(string data)
	{
		float r;
		float g;
		float b;
		int colorNum = getStringNum(data, 16);
		r = ((colorNum & 255 << 16) >> 16) / 255f;
		g = ((colorNum & 255 << 8) >> 8) / 255f;
		b = (colorNum & 255) / 255f;
		return new Color(r,g,b);
	}

	public static int getStringNum(string data, int jinzhi)
	{
		switch(jinzhi)
		{
		case 2:
			jinzhi = 1;
			break;
		case 8:
			jinzhi = 3;
			break;
		case 16:
			jinzhi = 4;
			break;
		}
		int tempNum = 1;
		int tempAll = 0;
		for(int i = data.Length - 1; i >= 0; i --)
		{
			tempAll += (getCharNum(data.Substring(i, 1)) * tempNum);
			tempNum = (tempNum << jinzhi);
		}
		return tempAll;
	}

	public static int getCharNum(string data)
	{
		switch(data)
		{
		case "0":
			return 0;
		case "1":
			return 1;
		case "2":
			return 2;
		case "3":
			return 3;
		case "4":
			return 4;
		case "5":
			return 5;
		case "6":
			return 6;
		case "7":
			return 7;
		case "8":
			return 8;
		case "9":
			return 9;
		case "a":
			return 10;
		case "b":
			return 11;
		case "c":
			return 12;
		case "d":
			return 13;
		case "e":
			return 14;
		case "f":
			return 15;
		}
		return 0;
	}

	public static int getZhanli(JunZhuInfoRet junzhuinfo)
	{
		return junzhuinfo.zhanLi;
	}

	/// <summary>
	/// 获得点与旋转矩形碰撞
	/// </summary>
	/// <returns><c>true</c>, if coll rect was set, <c>false</c> otherwise.</returns>
	/// <param name="p0">碰撞点</param>
	/// <param name="p1">4个中分点1</param>
	/// <param name="p2">4个中分点2</param>
	/// <param name="p3">4个中分点3</param>
	/// <param name="p4">4个中分点4</param>
	public static bool getCollRect(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
	{
		float scalex = (float)(Screen.width / 960f);
		float scaley = (float)(Screen.height / 640f);
		float x0 = p0.x;
		float y0 = p0.z;
		float x1 = p1.x;
		float y1 = p1.z;
		float x2 = p2.x;
		float y2 = p2.z;
		
		float a = y2 - y1;
		float b = x1 - x2;
		float c = y1*x2 - x1*y2;
		float d1 = Mathf.Abs((a * x0) + (b * y0) + c) / (float)Math.Sqrt((float)Math.Pow(a, 2) + (float)Math.Pow(b, 2));

		x1 = p3.x;
		y1 = p3.z;
		x2 = p4.x;
		y2 = p4.z;
		
		a = y2 - y1;
		b = x1 - x2;
		c = y1*x2 - x1*y2;
		float d2 = Mathf.Abs((a * x0) + (b * y0) + c) / (float)Math.Sqrt((float)Math.Pow(a, 2) + (float)Math.Pow(b, 2));
		return (d1 <= Vector3.Distance(p3, p4) / 2 && d2 <= Vector3.Distance(p1, p2) / 2);
	}

    /// <summary>
    /// Set UIGrid's child item to specific num with child prefab.
    /// </summary>
    /// <param name="grid">UIGrid</param>
    /// <param name="count">specific items' num</param>
    /// <param name="itemPrefab">grid's child prefab</param>
    public static void SetGridItems(UIGrid grid, int count, GameObject itemPrefab)
    {
        while (grid.transform.childCount > count)
        {
            var child = grid.transform.GetChild(0);
            child.parent = null;
            UnityEngine.Object.Destroy(child.gameObject);
        }

        while (grid.transform.childCount < count)
        {
            NGUITools.AddChild(grid.gameObject, itemPrefab);
        }
    }

	#region Bundle Utilities

	private static float m_total_download_time = 0;
	
	public static void ResetTotalDownloadTime(){
		m_total_download_time = 0;
	}
	
	public static float GetTotalDownloadTime(){
		return m_total_download_time;
	}
	
	/// Updates the total download time with p_delta.
	public static void UpdateTotalDownloadTime( float p_delta ){
		m_total_download_time = m_total_download_time + p_delta;
	}

	public static string NextCutting(ref string data, string fuhao = ",")
	{
		string tempReturnString;
		if(data.IndexOf(fuhao) == -1)
		{
			tempReturnString = data.Substring(0, data.Length);
			data = "";
			return tempReturnString;
		}
		tempReturnString = data.Substring(0, data.IndexOf(fuhao));
		data = data.Substring(data.IndexOf(fuhao) + 1, data.Length - (data.IndexOf(fuhao) + 1));
		return tempReturnString;
	}

	//public static void YuanbaoBuzu()
	//{
	//	Global.CreateBox("元宝不足",
	//	                 "是否购买元宝",
	//	                 null, null, "确定", "取消", YuanbaoClick);
	//}

	//public static void YuanbaoClick(int i)
	//{
	//	Debug.Log(i);
	//	if(i == 1)
	//	{
	//		TopUpLoadManagerment.LoadPrefab(true);
	//	}
	//}

	#endregion



	#region Load Resource

	public delegate void LoadResourceCallback();

	public static bool m_is_loading_from_bundle = false;

	public static void ResourcesDotLoad( string p_resource_path, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list = null, bool p_open_simulate = false ){
		ResourcesDotLoad( p_resource_path, null, p_delegate, p_callback_list , p_open_simulate );
	}

	public static void ResourcesDotLoad( string p_resource_path, System.Type p_type, Bundle_Loader.LoadResourceDone p_delegate, List<EventDelegate> p_callback_list = null, bool p_open_simulate = false ){
//		Debug.Log( "ResourcesDotLoad( " + p_resource_path + " )" );

#if RESOURCE_PATH
//		UtilityTool.Instance.ExecResourcesLoad( p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate );

		Bundle_Loader.LoadAssetFromResources( p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate );
		return;
#elif BUNDLE_PATH
		BundleHelper.LoadAsset( p_resource_path, p_type, p_delegate, p_callback_list );

		/*
		TimeHelper.ResetTaggedTime( TimeHelper.CONST_TIME_INFO_NOT_FOUND_IN_BUNDLE );

		if( !Bundle_Loader.LoadAssetFromBundle( p_resource_path, p_type, p_delegate, p_callback_list ) ){
//			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_BUNDLE_DOWNLOADING, false ) ){
//				Debug.LogError( "Resources not Contained: " + p_resource_path );
//			}

			TimeHelper.UpdateTimeInfo( TimeHelper.CONST_TIME_INFO_NOT_FOUND_IN_BUNDLE );
			
//			UtilityTool.Instance.ExecResourcesLoad( p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate );

			Bundle_Loader.LoadAssetFromResources( p_resource_path, p_type, p_delegate, p_callback_list, p_open_simulate );
		}
		*/
		
		return;
#else
		Debug.LogError( "Error, Path Not Defined." );
#endif
	}

	#endregion



	#region Resources.Load's Watchers

	private class ResourcesDotLoadWatcher{
		private Dictionary<string, int> m_to_load_res_dict = new Dictionary<string, int>();

		private List<EventDelegate> m_call_back_list;

		private float m_watcher_init_time 	= 0.0f;

		private float m_waited_time			= 0.0f;

		private const float WATCHER_WARNING_TIME	= 10.0f;

		public ResourcesDotLoadWatcher( List<string> p_to_load_res_list, List<EventDelegate> p_callback_list ){
			ConvertToDict( p_to_load_res_list );

			m_call_back_list = p_callback_list;

			m_watcher_init_time = Time.realtimeSinceStartup;
		}

		private void ConvertToDict( List<string> p_to_load_res_list ){
			foreach( string p_res_path in p_to_load_res_list ){
				if( m_to_load_res_dict.ContainsKey( p_res_path ) ){
					m_to_load_res_dict[ p_res_path ] = m_to_load_res_dict[ p_res_path ] + 1;
				}
				else{
					m_to_load_res_dict.Add( p_res_path, 1 );
				}
			}
		}

		private void ExecuteCallback(){
			EventDelegate.Execute( m_call_back_list );
		}

		/// Return:
		/// true, if watch jobs done.
		/// false, if not.
		public bool ResLoaded( string p_res_path ){
			if( m_to_load_res_dict.ContainsKey( p_res_path ) ){
				m_to_load_res_dict[ p_res_path ] = m_to_load_res_dict[ p_res_path ] - 1;

				if( m_to_load_res_dict[ p_res_path ] == 0 ){
					m_to_load_res_dict.Remove( p_res_path );
				}
			}

			if( m_to_load_res_dict.Count == 0 ){
				ExecuteCallback();

				return true;
			}

			{
				m_waited_time = Time.realtimeSinceStartup - m_watcher_init_time;
				
				if( m_waited_time > WATCHER_WARNING_TIME ){
					Debug.LogError( "Error, Load Watcher is Waiting Too Long: " + m_waited_time );

					foreach( KeyValuePair<string, int> t_pair in m_to_load_res_dict ){
						Debug.Log( t_pair.Key + " - " + t_pair.Value );
					}
				}
			}

			return false;
		}
	}

	private static List<ResourcesDotLoadWatcher> m_resources_dot_load_watchers_list = new List<ResourcesDotLoadWatcher>();

	/// Desc:
	/// p_callback_list will be called when all p_res_to_load_list assets were loaded.
	/// 
	/// Notes:
	/// 1.MUST BE THE SAME PATH AS ResourceDotLoad's ASSET_PATH param.
	/// 2.if you have only 1 callback, just use UtilityTool.GetEventDelegateList( p_callback ) to get a list.
	public static void SetResourcesDotLoadWatchers( List<string> p_res_to_load_list, List<EventDelegate> p_callback_list = null ){
		if( p_res_to_load_list.Count <= 0 ){
			Debug.LogError( "Error, No Item To Be Watched." );

			return;
		}

		if( p_callback_list.Count <= 0 ){
			Debug.LogError( "Error, No Callback Setted." );

			return;
		}

		ResourcesDotLoadWatcher t_watcher = new ResourcesDotLoadWatcher( p_res_to_load_list, p_callback_list );

		m_resources_dot_load_watchers_list.Add( t_watcher );
	}

	public static void ResoucesLoaded( string p_res_path, UnityEngine.Object p_res_object ){
		if( string.IsNullOrEmpty( p_res_path ) ){
			return;
		}

//		Debug.Log( "ResoucesLoaded( " + p_res_path + " - " + p_res_object.name + " )" );

		for( int i = m_resources_dot_load_watchers_list.Count - 1; i >= 0; i-- ){
			ResourcesDotLoadWatcher t_watcher = m_resources_dot_load_watchers_list[ i ];

			if( t_watcher.ResLoaded( p_res_path ) ){
				m_resources_dot_load_watchers_list.Remove( t_watcher );
			}
		}
	}

	#endregion



	#region Load Level

	public static void LoadLevel( string p_level_name, Bundle_Loader.LoadResourceDone p_delegate ){
//		Debug.Log( "Global.LoadLevel( " + p_level_name + " )" );
		
		#if RESOURCE_PATH
		
		ExecLevelLoad( p_level_name, p_delegate );
		
		return;
		#elif BUNDLE_PATH
		BundleHelper.LoadLevelAsset( p_level_name, p_delegate );

//		if( !Bundle_Loader.LoadLevelBundle( p_level_name, p_delegate ) ){
//			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_BUNDLE_DOWNLOADING, true ) ){
//				Debug.LogError( "Level not Contained: " + p_level_name );
//			}
//
//			ExecLevelLoad( p_level_name, p_delegate );
//		}
//		
		return;
		#else
		Debug.LogError( "Error, Path Not Defined." );
		#endif
	}

	private static void ExecLevelLoad( string p_level_name, Bundle_Loader.LoadResourceDone p_delegate ){
		UtilityTool.Instance.ExecLoadCallback( p_delegate, null, p_level_name, null );
	}

	public static GameObject getEffAtkWaring(GameObject obj)
	{
		GameObject returnTemp = GameObject.Instantiate(obj) as GameObject;

		SoundPlayEff spe = returnTemp.AddComponent<SoundPlayEff>();

		spe.PlaySound("410000");

//		AudioSource temp;
//		temp = returnTemp.GetComponentInChildren<AudioSource>();
//		if(temp != null)
//		{
//			temp.volume = ClientMain.m_sound_manager.m_fMaxEffVolume;
//		}
		return returnTemp;
	}

	public static void ScendNull(short sendID, int waitid = -1)
	{
		SocketTool.Instance().SendSocketMessage(sendID, waitid + "");
	}

	public static void ScendID(short sendID, int id, int waitid = -1)
	{
		TalentUpLevelReq req = new TalentUpLevelReq();
		
		req.pointId = id;
		
		MemoryStream tempStream = new MemoryStream();
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Serialize(tempStream, req);
		
		byte[] t_protof;
		
		t_protof = tempStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(sendID, ref t_protof, waitid + "");
	}

	public static void upDataTongzhiData(List<TongzhiData> listData)
	{
//		Debug.Log("m_listMainCityData.cout="+m_listMainCityData.Count);

		if(listData != null)
		{
			for(int i = 0; i < listData.Count; i ++)
			{
				bool tempBool = false;
				if(getTongzhiData(m_listAllTheData, listData[i].m_SuBaoMSG.subaoId) != null)
				{
					tempBool = true;
				}
				if(!tempBool)
				{
					m_listAllTheData.Add(listData[i]);
				}
			}
		}
		m_listJiebiaoData = new List<TongzhiData>();
		m_listMainCityData = new List<TongzhiData>();
		m_listShiLianData = new List<TongzhiData> ();
		m_listJunchengData = new List<TongzhiData> ();
		for(int i = 0; i < m_listAllTheData.Count; i ++)
		{
//			int tempState = m_listAllTheData[i].m_iSceneType;
			if((m_listAllTheData[i].m_ReceiveSceneType & 1) != 0)
			{
				m_listJiebiaoData.Add(m_listAllTheData[i]);
			}
			if((m_listAllTheData[i].m_ReceiveSceneType & 2) != 0)
			{
				m_listMainCityData.Add(m_listAllTheData[i]);
			}
			if((m_listAllTheData[i].m_ReceiveSceneType & 4) != 0)
			{
				m_listShiLianData.Add(m_listAllTheData[i]);
			}
			if((m_listAllTheData[i].m_ReceiveSceneType & 8) != 0)
			{
				m_listJunchengData.Add(m_listAllTheData[i]);
			}
		}
//		Debug.Log("m_listMainCityData.cout="+m_listMainCityData.Count);
	}

	public static TongzhiData getTongzhiData(List<TongzhiData> listData, long id)
	{
		for(int i = 0; i < listData.Count; i ++)
		{
			if(listData[i].m_SuBaoMSG.subaoId == id)
			{
				return listData[i];
			}
		}
		return null;
	}

	#endregion

}