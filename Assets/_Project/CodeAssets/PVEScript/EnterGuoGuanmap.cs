using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EnterGuoGuanmap : MonoBehaviour {

	public bool DataSended;
	public static EnterGuoGuanmap m_instance;
	public static GameObject tempObject;

	public int ShouldOpen_id = 0 ;

	public static EnterGuoGuanmap Instance()
	{
		if (m_instance == null)
		{
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();;
			
			m_instance = t_gameObject.AddComponent<EnterGuoGuanmap>();
		}
		
		return m_instance;
	}
	void Awake(){
		if (Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "") {

		}

	}

	void OnDestroy(){
		m_instance = null;

		tempObject = null;
	}

	void OnClick(){
		EnterPveUI (-1);
	}

	private static int m_chosen_chapter = -1;

	public static void EnterPveUI( int p_chapter ){

		m_chosen_chapter = p_chapter;
		if(MainCityUI.m_MainCityUI != null)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ),
						                        ResLoaded );
		}

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

		
		MapData mMapinfo = tempObject.GetComponent<MapData>();

		mMapinfo.GuidLevel = 0;
		Debug.Log ("Global.m_sPanelWantRun " +Global.m_sPanelWantRun );
		if(Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
		{
			if(Global.NextCutting(ref Global.m_sPanelWantRun, "#") == "chuanqi")
			{
				int x = -1;
				bool success = int.TryParse(Global.m_sPanelWantRun,out x);

				if(x != 0)
				{
					CityGlobalData.PT_Or_CQ = false;

					mMapinfo.startsendinfo( x );
					
					Global.m_sPanelWantRun = "";
					return;
				}
				else
				{
					Global.m_sPanelWantRun = "chuanqi";
				}
			}
		}
		mMapinfo.startsendinfo( m_chosen_chapter );
	}
	
}
