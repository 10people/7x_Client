using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class choosemap : MonoBehaviour {
	
	public static int nextMap ;
	public  int mycurMap ;
	GameObject CurMap;
	GameObject NextMap;
	Vector3 vect0 = new Vector3(0,0,0);
	Vector3 vectRight = new Vector3(960,0,0);
	Vector3 vectLeft = new Vector3(-960,0,0);
	public GameObject Lev;
	public static bool UpAndDownbtn;
	public GameObject MapRoot;
	private int my_mapnum;
	void Start () {

		UpAndDownbtn = true;

	//	AddCurMap (mycurMap);
	
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		CurMap = Instantiate(p_object ) as GameObject;
		
		CurMap.transform.parent = MapRoot.transform;
		
		CurMap.transform.localScale = new Vector3 (1,1,1);
		
		CurMap.transform.localPosition = new Vector3 (0,0,0);

		InitMap minitMap = CurMap.GetComponent<InitMap>();

		minitMap.map_num = my_mapnum;

		minitMap.init ();
		
		if(CityGlobalData.PT_Or_CQ )
		{
			CreateLvs(CurMap);
		}
		else
		{
			Create_Cq_Lvs(CurMap);
		}
	}
	public void LoadResourceCallback1(ref WWW p_www,string p_path, Object p_object)
	{
		NextMap = Instantiate(p_object ) as GameObject;
		
		NextMap.transform.parent = CurMap.transform.parent = MapRoot.transform;;
		
		NextMap.transform.localScale = new Vector3 (1,1,1);
		
		NextMap.transform.localPosition = new Vector3 (0,0,0);

		InitMap minitMap = NextMap.GetComponent<InitMap>();

		minitMap.map_num = my_mapnum;

		minitMap.init ();
		
		if(CityGlobalData.PT_Or_CQ){
			CreateLvs(NextMap);
		}
		else{
			Create_Cq_Lvs(NextMap);
		}
		
		Destroy(CurMap,0.3f);
		
		CurMap = NextMap;
	}
   public void AddCurMap(int Chapter)
	{
		if(CurMap)
		{
			Destroy(CurMap);
		}
		my_mapnum = Chapter;

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ) + Chapter.ToString(),
		                         LoadResourceCallback);

		mycurMap = Chapter;
	}

	IEnumerator AddNextMap(int Chapter){

		if( !UpAndDownbtn ){

			yield return new WaitForSeconds (0.3f);
		}
		my_mapnum = Chapter;

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ) + Chapter.ToString(),
		                        LoadResourceCallback1);

		//NextMap.transform.localPosition = new Vector3 (0,0,0);
	}



	public void loadback1(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject Cloud = Instantiate( p_object ) as GameObject;

		Cloud.transform.parent = this.transform;

		Cloud.transform.localScale = new Vector3(1,1,1);

		Cloud.transform.localPosition = new Vector3(1,1,1);

	}


	public void loadback2(ref WWW p_www,string p_path, Object p_object)
	{
		NextMap = Instantiate( p_object ) as GameObject;
		
		NextMap.transform.parent = CurMap.transform.parent = MapRoot.transform;;
		
		NextMap.transform.localScale = new Vector3 (1,1,1);
		
		NextMap.transform.localPosition = vectRight;

		InitMap minitMap = NextMap.GetComponent<InitMap>();

		minitMap.map_num = my_mapnum;

		minitMap.init ();

		if(CityGlobalData.PT_Or_CQ){
			CreateLvs(NextMap);
		}
		else{
			Create_Cq_Lvs(NextMap);
		}
		
		TweenPosition.Begin(CurMap,0.4f,vectLeft);
		
		TweenPosition.Begin(NextMap,0.4f,vect0);
		
		Destroy(CurMap,0.5f);
		
		CurMap = NextMap;

	}
	public void loadback3(ref WWW p_www,string p_path, Object p_object)
	{
		NextMap = Instantiate( p_object ) as GameObject;

		NextMap.transform.parent = CurMap.transform.parent = MapRoot.transform;;

		NextMap.transform.localScale = new Vector3 (1,1,1);

		NextMap.transform.localPosition = vectLeft;

		InitMap minitMap = NextMap.GetComponent<InitMap>();

		minitMap.map_num = my_mapnum;

		minitMap.init ();

		if(CityGlobalData.PT_Or_CQ)
		{
			CreateLvs(NextMap);
		}
		else{
			Create_Cq_Lvs(NextMap);
		}
		TweenPosition.Begin(CurMap,0.4f,vectRight);

		TweenPosition.Begin(NextMap,0.4f,vect0);

		Destroy(CurMap,0.5f);

		CurMap = NextMap;
	}
	public  void sortmap(int Nextmap){
      
		if(!UpAndDownbtn){
			//AddNextMap(Nextmap);
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_CLOUD ),loadback1);

			StartCoroutine(AddNextMap(Nextmap));
		}
		else
		{
			my_mapnum = Nextmap;

			if(mycurMap < Nextmap)
			{
//				Debug.Log("向右边移动");

				Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ) + Nextmap.ToString(),loadback2);

			}
			else if(mycurMap > Nextmap)
			{
//				Debug.Log("向左边移动");
				Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_MAP_PREFIX ) + Nextmap.ToString(),loadback3);
			}

		}

		mycurMap = Nextmap;
	}
	void Create_Cq_Lvs(GameObject UIroot)//创建传奇关卡的小关卡
	{
		MapData.mapinstance.Pve_Level_InfoList.Clear ();

		for(int n = 0; n<MapData.mapinstance.CQLv.Count; n++)
		{
			Vector2 ZuoBiao1 = PVEZuoBiaoTemplate.GetCoordinate(MapData.mapinstance.myMapinfo.s_section,MapData.mapinstance.CQLv[n].guanQiaId);

			GameObject LVL = Instantiate(Lev)as GameObject;

			LVL.SetActive(true);

			LVL.name = "CQ_Levels" + n;

			LVL.transform.parent = UIroot.transform;

			LVL.transform.localScale =new Vector3 (1,1,1);

			LVL.transform.localPosition =new Vector3 (ZuoBiao1.x, ZuoBiao1.y,0);

			Pve_Level_Info LvChild = LVL.GetComponent<Pve_Level_Info>();

			LvChild.litter_Lv = MapData.mapinstance.CQLv[n];

			LvChild.litter_Lv.type = 2;
					
			if (n < MapData.mapinstance.CQLv.Count-1)
			{
				Vector2 ZuoBiao2 = PVEZuoBiaoTemplate.GetCoordinate(MapData.mapinstance.myMapinfo.s_section,MapData.mapinstance.CQLv[n+1].guanQiaId);

				LvChild.Zuob1 =ZuoBiao1;

				LvChild.Zuob2 =ZuoBiao2;
			}
		
			MapData.mapinstance.Pve_Level_InfoList.Add(LvChild);

			LvChild.Init();

		}
		Global.m_isOpenPVP = false;
		MapData.mapinstance.nowCurrChapter = MapData.mapinstance.CurrChapter;
	}
	void CreateLvs(GameObject UIroot)//创建普通关卡的小关卡
	{
		MapData.mapinstance.Pve_Level_InfoList.Clear ();

		for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
		{
			Vector2 ZuoBiao1 = PVEZuoBiaoTemplate.GetCoordinate(MapData.mapinstance.myMapinfo.s_section,MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId);
			
			GameObject lvl = Instantiate(Lev)as GameObject;

			lvl.SetActive(true);

			lvl.name = "Levels" + n;

			lvl.transform.parent = UIroot.transform;

			lvl.transform.localScale =new Vector3 (1,1,1);

			lvl.transform.localPosition =new Vector3 (ZuoBiao1.x, ZuoBiao1.y,0);


			Pve_Level_Info LvChild = lvl.GetComponent<Pve_Level_Info>();

			LvChild.litter_Lv = MapData.mapinstance.myMapinfo.s_allLevel[n];
		
			int DIR = PVEZuoBiaoTemplate.GetDir_by(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId);
			if(DIR == 0)
			{
				LvChild.IsRotation = false;
			}
			else
			{
				LvChild.IsRotation = true;
			}

			if (n < MapData.mapinstance.myMapinfo.s_allLevel.Count-1)
			{
				Vector2 ZuoBiao2 = PVEZuoBiaoTemplate.GetCoordinate(MapData.mapinstance.myMapinfo.s_section,MapData.mapinstance.myMapinfo.s_allLevel[n+1].guanQiaId);

				LvChild.Zuob1 =ZuoBiao1;

				LvChild.Zuob2 =ZuoBiao2;
			
			}
			MapData.mapinstance.Pve_Level_InfoList.Add(LvChild);

			LvChild.Init();

		}
		Global.m_isOpenPVP = false;
		MapData.mapinstance.nowCurrChapter = MapData.mapinstance.CurrChapter;
	}

}
