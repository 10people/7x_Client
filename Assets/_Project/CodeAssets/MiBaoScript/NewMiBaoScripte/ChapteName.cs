using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public class ChapteName : MonoBehaviour {


	public string Chapter_name;

	public UILabel Chaptername;

	public GameObject Box;

	public int ChapterNum;

	public int NodeNum;

	public int Levelid;

	public bool IsOPen;

	void Start () {
	
	}

	void Update () {
	
	}

	public void Init()
	{
//		LegendPveTemplate mlegendpve = LegendPveTemplate.GetlegendPveTemplate_By_id (Levelid);
//
//		List<LegendPveTemplate> m_list = LegendPveTemplate.GetlegendPveTemplate_By_bigName (mlegendpve.bigName);
//
//		for(int i = 0; i < m_list.Count; i++)
//		{
//			if(m_list[i].id == Levelid)
//			{
//				NodeNum = i+1;
//			}
//		}

	
		Chaptername.text = Chapter_name;

		if(IsOPen)
		{
			Box.SetActive(false);
		}
		else
		{
			Box.SetActive(true);
		}

	}

	public void EnterChapter()
	{
//		WindowBackShowController.SaveWindowInfo ("Secret(Clone)", Res2DTemplate.Res.UI_PANEL_SECRET);

		CityGlobalData.PT_Or_CQ = false;

		EnterGuoGuanmap.Instance().ShouldOpen_id = Levelid;

		EnterGuoGuanmap.EnterPveUI (ChapterNum);

//		GameObject mobg = GameObject.Find ("Secret(Clone)");

//		MainCityUI.TryRemoveFromObjectList(mobg);

		TaskData.Instance.m_DestroyMiBao = false;

//		Destroy(mobg) ;

	}
}
