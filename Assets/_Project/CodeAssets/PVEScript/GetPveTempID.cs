using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GetPveTempID : MonoBehaviour {

	public GameObject comLevelObj;//鏅?氬叧鍗?bj
	public GameObject comLevelObj_cross;//宸查氬叧鏅?氬叧鍗?bj
	public GameObject specialLevelObj;//绮捐嫳鎴栦紶濂囧叧鍗?bj
	
	public List<UISprite> starSpriteList = new List<UISprite>();//鏄熸槦spriteList
	
	public UILabel Lv_name;
	public UILabel Lv_name1;
	public UILabel Lv_name2;
	
	public int getstarjiangli;//鏄熺骇濂栧姳 111-110-100
	public int sendStars;
	public int Stars;//鏄熸暟
	
	private int needjunzhuLevel;//褰撳墠鑻遍泟绛夌骇
	[HideInInspector]public int NeedJunZhuLv;//闇瑕佺殑鍚涗富绛夌骇
	
	[HideInInspector]public int currentID;//涓存椂绔犺妭鍙橀噺
	public static int CurLev;

	
	GameObject tempOjbect_PVEUI;//鎺夎惤鏄剧ず椤甸潰
	
	public static string showlevelName;//鍏冲崱鍚嶅瓧
	[HideInInspector]public string Lvname;
	
	private float mTime;
	
	[HideInInspector]public bool Is_Pass;//鏄?惁杩囧叧
	
	[HideInInspector]public int LvType;//鍏冲崱绫诲瀷
	
	private List<UISprite> stars = new List<UISprite>();
	public UISprite spriteStar;
	
	public GameObject spriteDiBiao;
	public GameObject M_Line;
	
	//[HideInInspector]public Level mLevel;//鍏冲崱淇℃伅
	
	[HideInInspector]public int showState;//鏄剧ず鐘舵?	
	[HideInInspector]public bool ShowJIantou;
	[HideInInspector]public bool ShowLin;
	[HideInInspector]public Vector2 Zuob1;
	[HideInInspector]public Vector2 Zuob2;
	
	[HideInInspector]public Level litter_Lv;//浼犲?鍏冲崱淇℃伅
	
	[HideInInspector]public int Lv_Pingjia;
	
	public bool Startsendedmasg;

	float M_Distance;

	public UISprite Cru_Level;

	void Awake ()
	{

	}

	void Start ()
	{
		Startsendedmasg = true;
	}
	public void Init ()
	{  
		needjunzhuLevel = JunZhuData.Instance().m_junzhuInfo.level;

		currentID = litter_Lv.guanQiaId;

		NeedJunZhuLv = litter_Lv.s_level;

		LvType = litter_Lv.type;

		Is_Pass = litter_Lv.s_pass;

		//sendStars = litter_Lv.s_starNum;

	//	Stars = litter_Lv.s_starNum;//鏄熸暟

		if (!CityGlobalData.PT_Or_CQ)
		{
			Startinit_CQLv ();
		}

		else
		{
			StartInit ();
		}
	}
	
	void Startinit_CQLv ()
	{	
		comLevelObj.SetActive (false);
		comLevelObj_cross.SetActive (false);
		specialLevelObj.SetActive (true);

		if(litter_Lv.chuanQiPass)
		{
			Lv_Pingjia = litter_Lv.pingJia;

			for (int i = 0;i < starSpriteList.Count;i ++)
			{
				if (i < Lv_Pingjia)
				{
					starSpriteList[i].spriteName = "BigStar";
					starSpriteList[i].gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
				}
			}
		}

		UIButton childs = (UIButton)GetComponentInChildren(typeof(UIButton));
		childs.gameObject.SetActive(true);

		if (showState == 0)
		{
			childs.gameObject.GetComponent<Collider>().enabled = false;
		}
		else if (showState == 2)
		{
			childs.gameObject.GetComponent<Collider>().enabled = true;
		}

		Lv_name.text = Lvname;
		Lv_name1.text = Lvname;
		Lv_name2.text = Lvname;

		M_Distance = Vector2.Distance (Zuob1,Zuob2);

		if(ShowJIantou)
		{

		}

		if(ShowLin)
		{

		}
	}

	void StartInit()
	{
		if (!Is_Pass)
		{
			if (LvType == 0)
			{
				//Debug.Log ("Lv_LvType 11111111 ");
				if(showState == 0)
				{
					comLevelObj.SetActive (true);
				}else{
					comLevelObj.SetActive (false);
				}

				comLevelObj_cross.SetActive (false);
				specialLevelObj.SetActive (false);
			}
			
			else if (LvType == 1)
			{
				//Debug.Log ("Lv_LvType 2222222 ");
				comLevelObj.SetActive (false);
				comLevelObj_cross.SetActive (false);
				if(showState == 0)
				{
					specialLevelObj.SetActive (true);
				}
				else{
					specialLevelObj.SetActive (false);
				}

			}
		}
		
		else if(Is_Pass)
		{
			if (LvType == 0)
			{
//				Debug.Log ("Lv_LvType 33333333334 ");

				comLevelObj.SetActive (false);
				comLevelObj_cross.SetActive (true);
				specialLevelObj.SetActive (false);
			}
			
			else if (LvType == 1||LvType == 2)
			{
//				Debug.Log ("Lv_LvType 44444 ");

				comLevelObj.SetActive (false);
				comLevelObj_cross.SetActive (false);
				specialLevelObj.SetActive (true);

				for (int i = 0;i < starSpriteList.Count;i ++)
				{
					if (i < Stars)
					{
						starSpriteList[i].spriteName = "BigStar";
						starSpriteList[i].gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
					}
				}
			}
		}
		Lv_name.text = Lvname;
		Lv_name1.text = Lvname;
		Lv_name2.text = Lvname;

		M_Distance = Vector2.Distance (Zuob1,Zuob2);
		if(ShowJIantou)
		{
		//	CreateDiBiaoLine ();
		}
		if(ShowLin)
		{
			//Debug.Log ("M_Distance"+M_Distance);
			//ShowLine();
		}
	}
	



	public void ChooseChaper()
	{
		createpveui(Startsendedmasg);
	}

	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_LIMIT);
		string Contain2 = NeedJunZhuLv.ToString ();;
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		uibox.setBox(title,Contain1, Contain2,null,Comfirm,null,null,null,null);
	}
	int RenWuId;
	void createpveui(bool issended)
	{
		if (issended)
		{
			Startsendedmasg = false;
			if(CityGlobalData.PT_Or_CQ)
			{
				if(MapData.mapinstance.Lv.ContainsKey(currentID))
				{
					showlevelName = Lvname;

					CurLev = currentID;
					StartCoroutine(ChangerDataState());

					RenWuId = litter_Lv.renWuId;
				
					//Debug.Log("RenWuId = " +RenWuId);

					if(RenWuId <= 0)
					{
						if(needjunzhuLevel >= NeedJunZhuLv ){//
							
							if(JunZhuData.Instance().m_junzhuInfo.zhanLi > PveTempTemplate.GetPveTemplate_By_id(CurLev).PowerLimit)
							{
								ShowUIbaseBackData ();
							}
							else{
								Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadPowerUpBack);
							}
						}
						else{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        LoadResourceCallback );
						}

					}
					else{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
					}
				}
			}
			else{
				if(MapData.mapinstance.CQLv.Contains(litter_Lv))
				{
					showlevelName = Lvname;

//					Debug.Log("currentID"+currentID);

					CurLev = litter_Lv.guanQiaId;
					StartCoroutine(ChangerDataState());

					
					RenWuId = litter_Lv.renWuId;
					if(RenWuId <= 0)
					{
						if(needjunzhuLevel >= NeedJunZhuLv ){//
							
							if(JunZhuData.Instance().m_junzhuInfo.zhanLi > LegendPveTemplate.GetlegendPveTemplate_By_id(CurLev).PowerLimit)
							{
								ShowUIbaseBackData ();
							}
							else{
								Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadPowerUpBack);
							}
						}
						else{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        LoadResourceCallback );
						}
						
					}
					else{
						Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
					}
				}
			}

		}			
			
	}
	void LoadRenWuBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
	
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);
		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT);

		string Contain2 = ZhuXianTemp.GeTaskTitleById (RenWuId);
		//
	//	Debug.Log (" RenWuId = " +RenWuId);

		//.Log (" RenWuId = " +Contain2);

		string Contain3 = LanguageTemplate.GetText(LanguageTemplate.Text.FINGHT_CONDITON);
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		uibox.setBox(title,Contain1, Contain2,null,Comfirm,null,null,null,null);
	}
	void LoadPowerUpBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		int lv = 0;
		if(CityGlobalData.PT_Or_CQ)
		{
			lv = PveTempTemplate.GetPveTemplate_By_id (CurLev).PowerLimit;
		}
		else{
			
			lv = LegendPveTemplate.GetlegendPveTemplate_By_id (CurLev).PowerLimit;
		}
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);

		string Contain1 =  LanguageTemplate.GetText(LanguageTemplate.Text.POWER_LIMIT);

		string Contain2 = lv.ToString ();
		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		uibox.setBox(title,Contain1, Contain2,null,Comfirm,null,null,null,null);
	}
	IEnumerator ChangerDataState()
	{
		yield return new WaitForSeconds (1.0f);

		Startsendedmasg = true;

//		Debug.Log ("Startsendedmasg"+Startsendedmasg);
	}
	public void loadback(ref WWW p_www,string p_path, Object p_object)
	{

		tempOjbect_PVEUI = Instantiate(p_object)as GameObject;
		
		GameObject mtran = GameObject.Find ("Mapss");
		
		tempOjbect_PVEUI.transform.parent = mtran.transform;
		
		tempOjbect_PVEUI.transform.localPosition = new Vector3(0,0,0);
		
		tempOjbect_PVEUI.transform.localScale = new Vector3 (1,1,1);
		
		PveLevelUImaneger mPveLevelUImaneger = tempOjbect_PVEUI.GetComponent<PveLevelUImaneger>();
		mPveLevelUImaneger.Lv_Info = litter_Lv;
		mPveLevelUImaneger.Create_No_Disdroy = true;

		mPveLevelUImaneger.m_guidnotes = MapData.mapinstance.GuidLevel;
		mPveLevelUImaneger.init ();

	}
	void ShowUIbaseBackData()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_UI),loadback);

	}

	
}
