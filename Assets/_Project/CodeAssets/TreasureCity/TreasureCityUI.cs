using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureCityUI : GeneralInstance<TreasureCityUI> {

	public TreasureCityUIBR tCityUIBR;//右下
	public TreasureCityUITop tCityUITop;//正上
	public TreasureCityUITL tCityUITL;//左上
	public TreasureCityUITR tCityUITR;//右上

	void Awake ()
	{
		base.Awake ();
	}

	void Start ()
	{
		TopLeftUI ();
	}

	void OnDestroy ()
	{
		base.OnDestroy ();
	}

	public void InItMainUI ()
	{

	}

	#region Main UI Controller

	public void BottomUI (bool active)
	{
		tCityUIBR.SetOpenBoxBtnState (active);
	}

	public void TopUI (ErrorMessage tempMsg)
	{
		tCityUITop.InItTopUI (tempMsg);
	}

	public void TopYBUI (ErrorMessage tempMsg)
	{
		tCityUITop.InItTopUIYB (tempMsg);
	}

	public void TopUIMsg (string msg)
	{
		tCityUITop.GetChatMsg (msg);
	}

	public void TopLeftUI ()
	{
		tCityUITL.RefreshJunZhuInfo ();
	}

	#endregion

	public static List<GameObject> m_WindowObjectList = new List<GameObject>();

	public static bool IsWindowsExist ()
	{
		if (TreasureCityUI.m_instance == null) return false;
		
		//Clear object list.
		for (int i = 0; i < TreasureCityUI.m_WindowObjectList.Count; i++)
		{
			if (TreasureCityUI.m_WindowObjectList[i] == null || !TreasureCityUI.m_WindowObjectList[i].activeInHierarchy)
			{
				TreasureCityUI.m_WindowObjectList.RemoveAt(i);
			}
		}
		
		return TreasureCityUI.m_WindowObjectList.Count > 0;
	}
	
	public static void TryAddToObjectList(GameObject go, bool p_add_to_2d_tool = true)
	{
		if (TreasureCityUI.m_instance != null)
		{
			TreasureCityUI.m_WindowObjectList.Add(go);
			
			// assume all param:go is functionUI's main page, if not will cause an error
			if (p_add_to_2d_tool)
			{
				UI2DTool.Instance.AddTopUI(go);
			}
		}
		else
		{
			Debug.LogWarning("Warning, TreasureCityUI not exist.");
		}
	}
	
	public static void TryRemoveFromObjectList(GameObject go)
	{
		if (TreasureCityUI.m_instance != null && TreasureCityUI.m_WindowObjectList.Contains(go))
		{
			TreasureCityUI.m_WindowObjectList.Remove(go);
		}
	}
}
