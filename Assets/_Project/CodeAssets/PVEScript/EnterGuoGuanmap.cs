using UnityEngine;
using System.Collections;

public class EnterGuoGuanmap : MonoBehaviour {

	public bool DataSended;
	public static EnterGuoGuanmap m_instance;
	public static GameObject tempObject;

	public int ShouldOpen_id = 0 ;

	public static EnterGuoGuanmap Instance()
	{
		if (m_instance == null)
		{
			GameObject t_gameObject = UtilityTool.GetDontDestroyOnLoadGameObject();;
			
			m_instance = t_gameObject.AddComponent<EnterGuoGuanmap>();
		}
		
		return m_instance;
	}
	void Awake(){
	

	}

	void OnClick(){
		EnterPveUI (-1);
	}

	private static int m_chosen_chapter = -1;

	public static void EnterPveUI( int p_chapter ){
		m_chosen_chapter = p_chapter;

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ),
				ResLoaded );
	}

	public static void ResLoaded( ref WWW p_www, string p_path, UnityEngine.Object p_object )
	{
		//Debug.Log ("加载pve页面 2222 " +Global.m_isOpenPVP );
		if (tempObject != null)
		{
			return;
		}
		tempObject = ( GameObject )Instantiate( p_object );
		MainCityUI.TryAddToObjectList(tempObject);
		tempObject.transform.position = new Vector3( 0,500,0 );
		Global.m_isOpenPVP = true;
		//Debug.Log ("Global.m_isOpenPVP= " +Global.m_isOpenPVP );
		MapData mMapinfo = tempObject.GetComponent<MapData>();

		mMapinfo.GuidLevel = 0;

		mMapinfo.startsendinfo( m_chosen_chapter );
	}
}
