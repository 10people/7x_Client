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

	[HideInInspector]public string ChapterName;

	public UILabel ChapterNameGB;

	public GameObject Art;

	public void init()
	{
		//ChaptersGB.text = Chapters;
		List<int > zhangjieid = PassLevelBtn.Instance().Sec_idlist;
		Art.SetActive(false);
		//Debug.Log("Chapternumber = "+Chapternumber);
		if(zhangjieid != null )
		{
			foreach(int id in zhangjieid)
			{
			//	Debug.Log("id = "+id);
				if(id == Chapternumber)
				{
					Art.SetActive(true);
					break;
				}
			}
		}
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
	  //  Debug.Log ("Chapternumber"+Chapternumber);
		MapData.sendmapMessage (Chapternumber);
		//MapData.mapinstance.deletelvfun();
	    MapData.mapinstance.IsCloseGuid = false;
	    Destroy (ParTrans,0.2f);
	}
}
