using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class chooseChapterBtn : MonoBehaviour {

	[HideInInspector]public int Chapternumber;

	public GameObject NoShowBtn;

	public GameObject ParTrans;
	//[HideInInspector]public string Chapters;
	[HideInInspector]public string ChapterName;
	//public UILabel ChaptersGB;
	public UILabel ChapterNameGB;
//	public List<Level> AllLvs = new List<Level> ();
//	public List<Level> PassLvs = new List<Level> ();
	public void init()
	{
		//ChaptersGB.text = Chapters;
		ChapterNameGB.text = ChapterName;
	}

	void OnClick()
	{
		CityGlobalData.PveLevel_UI_is_OPen = false;
		MapData.mapinstance.Lv.Clear();
		MapData.mapinstance.JYLvs = 0;
		MapData.mapinstance.Starnums = 0;
		choosemap.nextMap = Chapternumber;
		choosemap.UpAndDownbtn = false;
	    MapData.mapinstance.CurrChapter = Chapternumber;
	    Debug.Log ("Chapternumber"+Chapternumber);
		MapData.sendmapMessage (Chapternumber);
		//MapData.mapinstance.deletelvfun();
	    MapData.mapinstance.IsCloseGuid = false;
	    Destroy (ParTrans,0.2f);
	}
}
