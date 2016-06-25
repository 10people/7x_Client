using UnityEngine;
using System.Collections;

public class TreasureCityRoot : GeneralInstance<TreasureCityRoot> {

	public GameObject treasureCityUI;

	new void Awake()
	{
		base.Awake ();
	}

	// Use this for initialization
	void Start()
	{
		UIYindao.m_UIYindao.CloseUI ();
		ClientMain.m_sound_manager.chagneBGSound (1001);
		LoadTreasureCityUI ();
	}

	public void LoadTreasureCityUI ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TREASURE_CITY_UI),
		                        LoadTreasureCityUICallBack);
	}
	
	void LoadTreasureCityUICallBack (ref WWW p_www, string p_path, Object p_object)
	{
		treasureCityUI = GameObject.Instantiate (p_object) as GameObject;

		// create UI2DTool and set MainCity UI
		{
			UI2DTool.Instance.AddTopUI( treasureCityUI );
		}
		LoadModelParent ();
	}

	void LoadModelParent ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.MODEL_PARENT),
		                        ResourceLoadModelCallback);
		
	}
	private void ResourceLoadModelCallback(ref WWW p_www, string p_path, Object p_object)
	{
		Debug.Log ("p_object:" + p_object);
		Debug.Log ("TreasureCityPlayer.m_instance:" + TreasureCityPlayer.m_instance);
		TreasureCityPlayer.m_instance.CreatePlayerModel (p_object);
		LoadSelfName();
	}

	void LoadSelfName ()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.MAINCITY_PLAYER_NAME),
		                         LoadSelfNameCallback);
	}

	public void LoadSelfNameCallback(ref WWW p_www, string p_path, Object p_object)
	{
		PlayerSelfNameManagerment.ShowSelfeName (p_object);
	}

	new void OnDestroy()
	{
		base.OnDestroy ();
	}
}
