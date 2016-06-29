using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class PrepareForTreasureCity : MonoBehaviour {

	private int m_loadNum = 0;
	private GameObject m_treasureCityUI;

	void Awake()
	{
		Prepare_For_TreasureCity ();
	}

	void Prepare_For_TreasureCity ()
	{
		LoadingHelper.InitSectionInfo(StaticLoading.m_loading_sections,"TreasureUI",100,1);
		
		EnterNextScene.DirectLoadLevel ();
		StartCoroutine (LoadTreasureCityScene ());
	}
	
	private IEnumerator LoadTreasureCityScene ()
	{
		while (!UtilityTool.IsLevelLoaded (SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.TREASURE_CITY)))
		{
			yield return new WaitForEndOfFrame ();
		}
		
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath(Res2DTemplate.Res.TREASURE_CITY_UI), OnTCitySceneLoaded);
	}

	void OnTCitySceneLoaded (ref WWW p_www, string p_path, Object p_object)
	{
		m_treasureCityUI = GameObject.Instantiate (p_object) as GameObject;
		
		// create UI2DTool and set MainCity UI
		{
			UI2DTool.Instance.AddTopUI ( m_treasureCityUI );
		}
		
		if (UtilityTool.IsLevelLoaded(SceneTemplate.GetScenePath(SceneTemplate.SceneEnum.TREASURE_CITY)))
		{
			LoadingHelper.ItemLoaded(StaticLoading.m_loading_sections, "TreasureUI", "TreasureUI");
		}

		m_loadNum ++;
	}

	void Update()
	{
		if (m_loadNum >= 1)
		{
			TCityLoadDone ();
		}
	}

	void TCityLoadDone ()
	{
		if (TreasureCityRoot.m_instance != null)
		{
			TreasureCityRoot.m_instance.treasureCityUI = m_treasureCityUI;
			TreasureCityRoot.m_instance.LoadModelParent ();
		}
		EnterNextScene.Instance ().DestroyUI();
		Destroy(this);
	}
}
