//#define DEBUG_SHOW_TIP

using UnityEngine;
using System.Collections;

public class ShowTip : ScriptableObject
{

	public static TipItemData tipItemData;


	private static int m_commonItemId;

	private static string m_iconName;

	private static string m_enemyName;

	private static string m_enemyDesc;

	private static GameObject tipObject;


	public static void showTipEnemy(string iconName, string enemyName, string enemyDesc, TipItemData _tipItemData = null){
		#if DEBUG_SHOW_TIP
		Debug.Log( "showTipEnemy( " + iconName + ", " + enemyName + ", " + enemyDesc + " )" );
		#endif

		tipItemData = _tipItemData;

		m_iconName = iconName;

		m_enemyName = enemyName;

		m_enemyDesc = enemyDesc;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TIP), LoadResultResCallbackEnemy);
	}

	private static void LoadResultResCallbackEnemy(ref WWW p_www, string p_path, Object p_object){
		#if DEBUG_SHOW_TIP
		Debug.Log( "LoadResultResCallbackEnemy( " + p_www + ", " + p_path + ", " + p_object + " )" );
		#endif

		if (tipObject != null) {
			#if DEBUG_SHOW_TIP
			LogInfo( "tipObject != null, return." );
			#endif

			CameraHelper.ShowCameraInfo( tipObject );

			return;
		}

		tipObject = (GameObject)Instantiate(p_object);
		
		tipObject.transform.localPosition = new Vector3 (5000, 0, 0);
		
		tipObject.transform.localScale = new Vector3 (1, 1, 1);
		
		tipObject.transform.localEulerAngles = Vector3.zero;
		
		tipObject.SetActive (true);
		
		TipUIControllor tipControllor = tipObject.GetComponent<TipUIControllor>();

		try
		{
			tipControllor.refreshDataEnemy (m_iconName, m_enemyName, m_enemyDesc);
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);

			close();
		}
	}

	public static void showTip(int commonItemId, TipItemData _tipItemData = null){
		#if DEBUG_SHOW_TIP
		Debug.Log( "showTip( " + commonItemId + " )" );
		#endif

        //Debug.Log("=========Show Tips With Id: " + commonItemId);

		tipItemData = _tipItemData;

        bool flag = CommonItemTemplate.haveCommonItemTemplateById (commonItemId);

		if (flag == false){
			#if DEBUG_SHOW_TIP
			LogInfo( "flag == false, return." );
			#endif

			CameraHelper.ShowCameraInfo( tipObject );

			return;
		}

		m_commonItemId = commonItemId;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TIP), LoadResultResCallbackItem);
	}

	private static void LoadResultResCallbackItem(ref WWW p_www, string p_path, Object p_object){
		#if DEBUG_SHOW_TIP
		Debug.Log( "LoadResultResCallbackItem( " + p_www + ", " + p_path + ", " + p_object + " )" );
		#endif

        //Debug.Log("=========== load tip prefab.");

		if (tipObject != null){
			#if DEBUG_SHOW_TIP
			LogInfo( "tipObject != null, return." );
			#endif

			CameraHelper.ShowCameraInfo( tipObject );

			return;
		}
		
		tipObject = (GameObject)Instantiate(p_object);
		
		tipObject.transform.localPosition = new Vector3 (5000, 0, 0);
		
		tipObject.transform.localScale = new Vector3 (1, 1, 1);
		
		tipObject.transform.localEulerAngles = Vector3.zero;
		
		tipObject.SetActive (true);
		
		TipUIControllor tipControllor = tipObject.GetComponent<TipUIControllor>();

		try
		{
			tipControllor.refreshDataItem (m_commonItemId);
		}
		catch(System.Exception e)
		{
			Debug.LogError(e);

			close();
		}
	}

	public static void close(){
		#if DEBUG_SHOW_TIP
		Debug.Log( "close()" );
		#endif

		if (tipObject == null){
			#if DEBUG_SHOW_TIP
			LogInfo( "tipObject == null, return." );
			#endif

			return;
		}

		DestroyObject (tipObject);

		tipObject = null;

		#if DEBUG_SHOW_TIP
		LogInfo( "close()" );
		#endif
	}

	private static void LogInfo( string p_prefix ){
		if( !string.IsNullOrEmpty( p_prefix ) ){
			Debug.Log( "----------------------------- Log: " + p_prefix + " --------------------------" );
		}

		if( tipObject == null ){
			Debug.Log( "tipObject is null: " + tipObject );	
		}
		else{
			GameObjectHelper.LogGameObjectHierarchy( tipObject );
		}
	}
}
