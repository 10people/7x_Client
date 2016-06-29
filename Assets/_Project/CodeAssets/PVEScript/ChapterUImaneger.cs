using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ChapterUImaneger : MonoBehaviour {

	float my_locScolx;
	bool IsStart;
	public GameObject childgb;
	Vector3 startpos;
//	Vector3 endpos = new Vector3(0,0,0);
	public GameObject ChapterTemp;
	[HideInInspector]public  int CurrChaper;
	[HideInInspector]public  int AllChapers;
//	List <string>Chaptes = new List<string> ();
//	List <string>ChaptesName = new List<string> ();
	public GameObject ChildparentGB;

	Vector3 endposition;
	Vector3 scale1;
	Vector3 scale2;
	float time = 0.5f;
	public static ChapterUImaneger mChapterUImaneger;
	public static ChapterUImaneger Instance()
	{
		if (!mChapterUImaneger)
		{
			mChapterUImaneger = (ChapterUImaneger)GameObject.FindObjectOfType (typeof(ChapterUImaneger));
		}
		return mChapterUImaneger;
	}

	void Update()
	{
		if(GameObject.Find("NewPVEUI(Clone)"))
		{
			CloseUI();
		}
	}

	void Start () {


		endposition = new Vector3(0,0,0);

		scale1 = new Vector3 (1f,1f,1);
		scale2 = new Vector3 (0f,0f,0f);
		startpos = new Vector3(431,-265,0);
		childgb.transform.localPosition = startpos;
		PopUI ();
	}

	public void Init()
	{
		//getName_and_Chapers ();
		int j = 0;
	//	Debug.Log("AllChapers = "+AllChapers);
		for( int i = j; i<AllChapers ; i++)
		{
			//Debug.Log("看看章节名字    "+ChaptesName[i]);
			PveTempTemplate mPveTempTemplate = PveTempTemplate.GetPveTemplate_By_Chapter_Id(i+1);
			string Map_Name = NameIdTemplate.GetName_By_NameId(mPveTempTemplate.bigName);
			//ChaptesName.Add(Map_Name);
			GameObject chapterbtn = Instantiate(ChapterTemp)as GameObject;
			chapterbtn.SetActive(true);
			chapterbtn.transform.parent = ChapterTemp.transform.parent;
			chapterbtn.transform.localScale = ChapterTemp.transform.localScale;
			chooseChapterBtn mchooseChapterBtn = chapterbtn.GetComponent<chooseChapterBtn>();
			mchooseChapterBtn.ChapterName = Map_Name;
			//mchooseChapterBtn.Chapters = Chaptes[i];
			mchooseChapterBtn.Chapternumber = 1+i;
			if(CurrChaper == i+1)
			{
				UIToggle mtoggle = chapterbtn.GetComponent<UIToggle>();
				mtoggle.startsActive = true;
				//Debug.Log("看看章节名字    "+mtoggle.startsActive+i);
			}
			mchooseChapterBtn.init();
		}
		ChildparentGB.GetComponent<UIGrid> ().repositionNow = true;

		mFixUniform mmFixUniform = ChildparentGB.GetComponent<mFixUniform>();

		mmFixUniform.offset = new Vector3(-4,10,0);

		if(AllChapers > 6){

			mmFixUniform.enabled = false;
		}
	}



	void getName_and_Chapers()
	{
//		PveTempTemplate mPveTempTemplate = PveTempTemplate.GetPveTemplate_By_Chapter_Id(AllChapers);
//		string Map_Name = NameIdTemplate.GetName_By_NameId(mPveTempTemplate.bigName);


	}

	public void DeletFun()
	{
		CityGlobalData.PveLevel_UI_is_OPen = false;

		MapData.mapinstance.ShowYinDao = true;
		
		iTween.MoveTo(childgb, iTween.Hash("position", startpos, "time",time,"islocal",true));
		
		iTween.ScaleTo (childgb,iTween.Hash("scale",scale2,"time",time));
		
		//MapData.mapinstance.OpenEffect();
		if (CityGlobalData.PT_Or_CQ) {
			
			PassLevelBtn.Instance().OPenEffect ();
			
		}
		CloseUI ();
	}
	void CloseUI()
	{
		Destroy (this.gameObject,time);
	}
	void PopUI()
	{
		iTween.MoveTo(childgb, iTween.Hash("position", endposition, "time",time,"islocal",true));

		iTween.ScaleTo (childgb,iTween.Hash("scale",scale1,"time",time));
	}

}
