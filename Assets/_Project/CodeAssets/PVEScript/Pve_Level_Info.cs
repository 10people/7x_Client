using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
 
public class Pve_Level_Info : MonoBehaviour {

	//传奇关卡信息

	[HideInInspector]public Level litter_Lv;

	[HideInInspector]public Vector2 Zuob1;

	[HideInInspector]public Vector2 Zuob2;
	
	public GameObject Active_YuanDian_Sprite; 

	public GameObject DisActive_YuanDian_Sprite; 

	public GameObject JianTou_Sprite;

	public List<UISprite> stars = new List<UISprite>();

	public GameObject spriteStar_UIroot;

	public UISprite m_WinType;// 胜利类型

	private float M_Distance;

	public UILabel Lv_name;

	public GameObject PT_Level;

	public GameObject BoxBtn;

	public GameObject JY_CQ_Level;

	public GameObject Cur_levelTops;

	public GameObject JYAndCQDisPass;

	public GameObject PT_DisPass;

	[HideInInspector]public bool Lv_IsOpen = false;

	[HideInInspector]public int  LineType;

	private int needjunzhuLevel;//当前英雄等级

	public static int CurLev;

	public bool Startsendmasg = true;

	public static Pve_Level_Info G_Pve_Level_Info;

	public bool IsRotation = false;

	public UILabel mLanguage;

	public UILabel mLanguageLabel;

	public UILabel OpebSkillLanguage;

	public UISprite mBossIcon;
	public UISprite mBossIconbg;
	public UISprite mBossIconbg1;
	public enum m_Level_Type 
	{ 
		Putong = 0, 

		JingYing, 

		ChuanQi 

	} 

	public enum m_Level_Line 
	{ 
		ActiveYuan = 0, 
		
		Jiantou, 
		
		DisYuan 
		
	} 

	void Start () {
	
		G_Pve_Level_Info = this;

		Startsendmasg = true;
	}
	

	void Update () {
		if(litter_Lv.renWuId  > 0)
		{
			//Debug.Log("litter_Lv.renWuId = " +litter_Lv.renWuId);
			if(FreshGuide.Instance().IsActive(litter_Lv.renWuId) &&TaskData.Instance.m_TaskInfoDic[litter_Lv.renWuId].progress < 0)
			{
				litter_Lv.renWuId = -1;
			}
		}
		if(mLanguage.gameObject.activeInHierarchy)
		{
			if(PlayLanguage)
			{
				Vector3 tempScale = mLanguage.gameObject.transform.localScale;
				float addNum = 0.05f;
				if (tempScale == Vector3.one)
				{
					//tempScale = Vector3.zero;
					PlayLanguage = false;
					addNum = 0.05f;
//					StopCoroutine ("ChangeTimeTolanguage");
//					StartCoroutine ("ChangeTimeTolanguage");
					ChangeTimeTolanguage ();
				}
				if (tempScale.x < 1&&PlayLanguage)
				{
//					if (tempScale.x >= 0.95f)
//					{
//						addNum = 0.0005f;
//					}
					
					tempScale.x += addNum;
					tempScale.y += addNum;
					tempScale.z += addNum;
				}
				mLanguage.gameObject.transform.localScale = tempScale;
			}

		}
	
	}
	int STarTime;
	bool PlayLanguage = false;
	void  StopTime()
	{
		STarTime = Random.Range (5,20);
		mLanguage.gameObject.transform.localScale = Vector3.zero;
		Invoke ("PoP_QiPao",STarTime);
	}
	void  ChangeTimeTolanguage()
	{
		Invoke ("StopTime",3f);
	}
	private void PoP_QiPao()
	{
		PlayLanguage = true;
	}
	public void Init()
	{
		StopTime ();
		PlayLanguage = false;
		if(mLanguage.gameObject.activeInHierarchy)
		{
			mLanguage.gameObject.transform.localScale = Vector3.zero;
		}
		needjunzhuLevel = JunZhuData.Instance().m_junzhuInfo.level;

		int levelState = litter_Lv.type;  

		// 0 普通  1 精英  2  传奇
		if(levelState == (int)m_Level_Type.Putong)
		{
			JY_CQ_Level.SetActive(false);

			PT_Level.SetActive(true);

			if(litter_Lv.s_pass)
			{
				OpebSkillLanguage.gameObject.SetActive(false);
			}
			else
			{
				PveTempTemplate mPveTemp = PveTempTemplate.GetPveTemplate_By_id (litter_Lv.guanQiaId);
				if(mPveTemp.OPenSkillLabel != "")
				{
					OpebSkillLanguage.gameObject.SetActive(true);
					OpebSkillLanguage.text = mPveTemp.OPenSkillLabel;
				}
				else{
					OpebSkillLanguage.gameObject.SetActive(false);
				}

			}
			if(litter_Lv.guanQiaId%10 == 1)
			{

				if(litter_Lv.s_pass)
				{
					PT_Level.GetComponent<Collider>().enabled = false;

					for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId +1)
						{
							if(!MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
							{
								LineType = (int)m_Level_Line.Jiantou;
							
								break;
							}
						}

						LineType = (int)m_Level_Line.ActiveYuan;
					}
				}
				else
				{
					PT_Level.GetComponent<Collider>().enabled = true;

					LineType = (int)m_Level_Line.DisYuan;

					Lv_IsOpen = true;
				}
			}
			else
			{
				if(litter_Lv.s_pass)
				{

					PT_Level.GetComponent<Collider>().enabled = false;

					for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId +1)
						{
							if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
							{
								LineType = (int)m_Level_Line.ActiveYuan;

								break;
							}
						}

						LineType = (int)m_Level_Line.Jiantou;
					}

					//Debug.Log("LineType  = "+LineType);
				}
				else
				{
					LineType = (int)m_Level_Line.DisYuan;

					for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId -1)
						{
							if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
							{
								PT_Level.GetComponent<Collider>().enabled = true;

								Lv_IsOpen = true;

								break;
							}
						}

						PT_DisPass.SetActive(true);

						PT_Level.GetComponent<Collider>().enabled = false;
					}

				}
			}
			if(litter_Lv.s_pass)
			{
				PT_DisPass.SetActive(true);
				PT_DisPass.GetComponent<UISprite>().color = new Color(255,255,255,255);
			}
			if(Lv_IsOpen)
			{
				PT_DisPass.SetActive(false);
				int effectId =  100171;
				MapData.mapinstance.ShowEffectLevelid = litter_Lv.guanQiaId;
				MapData.mapinstance.OpenEffect();
			}

		}
		////////////////////////////
	
		if(levelState == (int)m_Level_Type.JingYing)
		{
			PveTempTemplate mPveTemp = PveTempTemplate.GetPveTemplate_By_id (litter_Lv.guanQiaId);
		
			List<NpcTemplate> mNpcTemplate = new List<NpcTemplate>();
			mNpcTemplate = NpcTemplate.GetNpcTemplates_By_npcid(mPveTemp.npcId);
			foreach(NpcTemplate mNpc in mNpcTemplate)
			{
				if(mNpc.type == 4)
				{
					mBossIcon.spriteName = mNpc.icon.ToString();
					break;
				}
			}

			if(mPveTemp.BigIcon == 1)
			{
				mBossIconbg.spriteName = "BossLevel";
				mBossIconbg.SetDimensions(105,105);
				mBossIcon.gameObject.transform.localPosition = new Vector3(-1.5f,2.9f,0);

				mBossIconbg1.spriteName = "BossLevel";
				mBossIconbg1.SetDimensions(105,105);
			}
			else
			{
				mBossIconbg.spriteName = "JingYinLevel";
				mBossIcon.gameObject.transform.localPosition = new Vector3(-0.5f,-3f,0);
			}
			PT_Level.SetActive(false);
			
			JY_CQ_Level.SetActive(true);
			mLanguage.gameObject.SetActive(true);
			if(!litter_Lv.s_pass)
			{
				if(mPveTemp.bubble == "" || mPveTemp.bubble ==null)
				{
					mLanguage.gameObject.SetActive(true);
				}
				else
				{
					mLanguage.gameObject.SetActive(true);
					mLanguageLabel.text = mPveTemp.bubble;
				}
				mBossIcon.color = new Color(0,0,0,255);
			}
			else
			{
				mBossIcon.color = new Color(255,255,255,255);
				mLanguage.gameObject.SetActive(false);
			}
			if(litter_Lv.guanQiaId%10 != 1)
			{
				if(litter_Lv.s_pass)
				{
					JY_CQ_Level.GetComponent<Collider>().enabled = true;

					for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId +1)
						{
							if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
							{
								LineType = (int)m_Level_Line.ActiveYuan;
					
								break;
							}
						}
						LineType = (int)m_Level_Line.Jiantou;
					}				

				}
				else{

					for(int n = 0; n<MapData.mapinstance.myMapinfo.s_allLevel.Count; n++)
					{
						if(MapData.mapinstance.myMapinfo.s_allLevel[n].guanQiaId == litter_Lv.guanQiaId -1)
						{
							if(MapData.mapinstance.myMapinfo.s_allLevel[n].s_pass)
							{
								JY_CQ_Level.GetComponent<Collider>().enabled = true;

								Lv_IsOpen = true;

								JYAndCQDisPass.SetActive(false);

								break;
							}
						}

						JYAndCQDisPass.SetActive(true);

						JY_CQ_Level.GetComponent<Collider>().enabled = false;
					}

					LineType = (int)m_Level_Line.DisYuan;

				}
			}
			if(Lv_IsOpen)
			{
				mBossIcon.color = new Color(255,255,255,255);
			}
			if(litter_Lv.s_pass)
			{
				ShowStar();
				
				Show_Win_Type(true);
			}
			ShowBox (true);
		}
		if(levelState == (int)m_Level_Type.ChuanQi)
		{
			PT_Level.SetActive(false);
			
			JY_CQ_Level.SetActive(true);

			LegendPveTemplate L_pvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(litter_Lv.guanQiaId);
			List<LegendNpcTemplate> mLegendNpcTemplateList = new List<LegendNpcTemplate>();
			mLegendNpcTemplateList = LegendNpcTemplate.GetLegendNpcTemplates_By_npcid(L_pvetemp.npcId);
			mLanguage.gameObject.SetActive(true);
			if(!litter_Lv.chuanQiPass)
			{
				mBossIcon.color = new Color(0,0,0,255);
				if(L_pvetemp.bubble == "" || L_pvetemp.bubble ==null)
				{
					mLanguage.gameObject.SetActive(true);
				}
				else
				{
					mLanguage.gameObject.SetActive(true);
					mLanguageLabel.text = L_pvetemp.bubble;
				}
			}
			else
			{
				mBossIcon.color = new Color(255,255,255,255);
				mLanguage.gameObject.SetActive(false);
			}
			foreach(LegendNpcTemplate mNpc in mLegendNpcTemplateList)
			{
				if(mNpc.type == 4)
				{
					mBossIcon.spriteName = mNpc.icon.ToString();
					break;
				}
			}
			
			if(L_pvetemp.BigIcon == 1)
			{
				mBossIconbg.spriteName = "BossLevel";
				mBossIconbg.SetDimensions(105,105);
				mBossIcon.gameObject.transform.localPosition = new Vector3(-1.5f,2.9f,0);
				
				mBossIconbg1.spriteName = "BossLevel";
				mBossIconbg1.SetDimensions(105,105);
			}
			else
			{
				mBossIconbg.spriteName = "JingYinLevel";
				mBossIcon.gameObject.transform.localPosition = new Vector3(-0.5f,-3f,0);
			}

			int Lvpos = 0;

			for(int i = 0; i<MapData.mapinstance.CQLv.Count; i++)
			{
				if(litter_Lv == MapData.mapinstance.CQLv[i])
				{
					Lvpos = i;
				}
			}
			if(Lvpos == 0)
			{
				if(!litter_Lv.chuanQiPass)
				{
					//JY_CQ_Level.collider.enabled = true;
					LineType = (int)m_Level_Line.DisYuan;

					Lv_IsOpen = true;

					JYAndCQDisPass.SetActive(false);
				}else
				{
					if(MapData.mapinstance.CQLv[Lvpos+1].chuanQiPass)
					{
						LineType = (int)m_Level_Line.ActiveYuan;
					}
					else
					{
						LineType = (int)m_Level_Line.Jiantou;
					}
				}
			}
			else
			{
				if(Lvpos == MapData.mapinstance.CQLv.Count - 1)
				{
					LineType = -1;

					if(MapData.mapinstance.CQLv[Lvpos-1].chuanQiPass)
					{
						if(!litter_Lv.chuanQiPass)
						{
							Lv_IsOpen = true;
						}
					}else
					{
						JY_CQ_Level.GetComponent<Collider>().enabled = false;

						JYAndCQDisPass.SetActive(true);

					}
				}
				else
				{
					if(litter_Lv.chuanQiPass)
					{
						if(MapData.mapinstance.CQLv[Lvpos+1].chuanQiPass)
						{
							LineType = (int)m_Level_Line.ActiveYuan;
						}
						else
						{
							LineType = (int)m_Level_Line.Jiantou;
						}
					}
					else
					{
						LineType = (int)m_Level_Line.DisYuan;

						if(MapData.mapinstance.CQLv[Lvpos-1].chuanQiPass)
						{
							Lv_IsOpen = true;
						}
						else
						{
							JYAndCQDisPass.SetActive(true);

							JY_CQ_Level.GetComponent<Collider>().enabled = false;
						}
					}
				}

			}

			if(litter_Lv.chuanQiPass)
			{
				ShowStar();
				Show_Win_Type(false);
			}
			if(Lv_IsOpen)
			{
				mBossIcon.color = new Color(255,255,255,255);
			}
			ShowBox (false);
		}

		if(Lv_IsOpen)
		{
//			Debug.Log("EnterGuoGuanmap.Instance().ShouldOpen_id  = " +EnterGuoGuanmap.Instance().ShouldOpen_id);

			//Cur_levelTops.SetActive(true);
//
//			MapData.mapinstance.ShowEffectLevelid = litter_Lv.guanQiaId;
//
//			if(EnterGuoGuanmap.Instance().ShouldOpen_id != 1)
//			{
//				MapData.mapinstance.OpenEffect();
//			}

			//UI3DEffectTool.ClearUIFx (ChangeMiBaobtn);
		}

		ShowLevel ();

		DrawLine ();

		if(litter_Lv.guanQiaId == EnterGuoGuanmap.Instance().ShouldOpen_id)
		{   
			EnterGuoGuanmap.Instance().ShouldOpen_id = 1;

			Startsendmasg = true;
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
			POPLevelInfo();
		}
	}
	public UISprite Boxsprite;
	public void ShowBox(bool jingyin)
	{
		//Debug.Log ("jingyin = "+jingyin);
		if(jingyin)
		{
			if(!litter_Lv.s_pass)
			{
				BoxBtn.GetComponent<BoxCollider>().enabled = false;
				
				BoxBtn.SetActive (false);
				
			}
			else
			{
				int passFinishnum = 0;
				
				int Getrawardnum = 0;
				for(int j = 0 ; j < litter_Lv.starInfo.Count; j++)
				{
					if(litter_Lv.starInfo[j].finished)
					{
						passFinishnum += 1;
						if(litter_Lv.starInfo[j].getRewardState)
						{
							Getrawardnum +=1;
						}
					}
				}
				if(Getrawardnum >= passFinishnum)
				{
					BoxBtn.SetActive (false);
					return;
				}
				if(passFinishnum == litter_Lv.starInfo.Count)
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.SetActive (false);
					}
					else
					{
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}else
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
						
						BoxBtn.SetActive (true);
						
						BoxBtn.gameObject.transform.localScale = new Vector3(0.7f,0.7f,1);
						
						Boxsprite.color = new Color(0,0,0,255);
					}
					else
					{
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}
			}
		}
		else
		{
			if(!litter_Lv.chuanQiPass)
			{
				BoxBtn.GetComponent<BoxCollider>().enabled = false;
				
				BoxBtn.SetActive (false);
				
			}
			else
			{
				int passFinishnum = 0;
				
				int Getrawardnum = 0;
				for(int j = 0 ; j < litter_Lv.cqStarInfo.Count; j++)
				{
					if(litter_Lv.cqStarInfo[j].finished)
					{
						passFinishnum += 1;
						if(litter_Lv.cqStarInfo[j].getRewardState)
						{
							Getrawardnum +=1;
						}
					}
				}
				if(Getrawardnum >= passFinishnum)
				{
					BoxBtn.SetActive (false);
					return;
				}
				if(passFinishnum == litter_Lv.cqStarInfo.Count)
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.SetActive (false);
					}
					else
					{
						//Debug.Log("aaaa");
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}else
				{
					if(Getrawardnum == passFinishnum)
					{
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
						//Debug.Log("bbbbbbb");
						BoxBtn.SetActive (true);
						
						BoxBtn.gameObject.transform.localScale = new Vector3(0.7f,0.7f,1);
						
						Boxsprite.color = new Color(0,0,0,255);
					}
					else
					{
						//Debug.Log("ccccc");
						BoxBtn.SetActive (true);
						BoxBtn.GetComponent<BoxCollider>().enabled = true;
					}
				}
			}
		}

	}

	void ShowStar()
	{
		if(!litter_Lv.s_pass)
		{
			spriteStar_UIroot.SetActive (false);

			return;
		}

		spriteStar_UIroot.SetActive (true);

		int starnum = 0;//litter_Lv.starNum;

		//Debug.Log ("starnum"+starnum);
		for(int j = 0 ; j < litter_Lv.starInfo.Count; j++)
		{
			if(litter_Lv.starInfo[j].finished)
			{
				starnum += 1;
			}
		}
		for(int i = 0; i < starnum; i++)
		{
			stars[i].spriteName = "BigStar";
			
			stars[i].gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
		}


	}

	void ShowLevel()
	{
		PveTempTemplate m_item = PveTempTemplate.GetPveTemplate_By_id (litter_Lv.guanQiaId);

		string M_Name = NameIdTemplate.GetName_By_NameId(m_item.smaName);

		Lv_name.text = M_Name;

	}

	void DrawLine()
	{
		if (Zuob2.x == 0 && Zuob2.y == 0)
						return;

		M_Distance = Vector2.Distance (Zuob1,Zuob2);

		int Dibiaonum = (int)(M_Distance / 40);

		//Debug.Log ("1LineType = "+LineType);

		switch(LineType)
		{
		case (int)m_Level_Line.ActiveYuan:

			ShowActiveYuan(Dibiaonum);

			break;

		case (int)m_Level_Line.Jiantou:

			ShowJianTou(Dibiaonum);

			break;

		case (int)m_Level_Line.DisYuan:

			//ShowDisYuan(Dibiaonum);

			break;

		default:
			break;

		}

	}

	void ShowActiveYuan(int mDibiaonum)
	{
		GameObject LineObject = (GameObject)Instantiate(Active_YuanDian_Sprite);
		
		LineObject.SetActive(true);
		
		LineObject.transform.parent = this.transform.parent;
		
		LineObject.transform.localScale = Active_YuanDian_Sprite.transform.localScale;
		
		float DisX = (Zuob2.x-Zuob1.x)/2;
		
		float DisY = (Zuob2.y - Zuob1.y)/2;
		
		LineObject.transform.localPosition = new Vector3(Zuob1.x+DisX,Zuob1.y+DisY,0);

		float angles;
		
		if(Zuob2.y-Zuob1.y > 0)
		{
			if(Zuob2.x-Zuob1.x < 0)
			{
				angles = Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),LineObject.transform.up);
				
			}
			else{
				angles = -Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),LineObject.transform.up);
			}
			
		}else{
			if(Zuob2.x-Zuob1.x < 0)
			{
				angles = 180f-Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),LineObject.transform.up);
			}
			else{
				angles = 180f+Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),LineObject.transform.up);
			}
		}
		LineObject.transform.eulerAngles = new Vector3(0,0,angles);

		Transform mTr = LineObject.transform.FindChild ("Sprite");

		UISprite mSp = mTr.GetComponent<UISprite>();
		//Debug.Log ("M_Distance = "+M_Distance);
		if(!IsRotation)
		{
			mSp.gameObject.transform.Rotate ( 0,-180,0);

			mSp.gameObject.transform.localPosition = new Vector3(mSp.gameObject.transform.localPosition.x - 12,mSp.gameObject.transform.localPosition.y,0);
		}
		else
		{
			mSp.gameObject.transform.localPosition = new Vector3(mSp.gameObject.transform.localPosition.x + 12,mSp.gameObject.transform.localPosition.y,0);
		}

		mSp.SetDimensions (50, (int)(M_Distance-100) );


	}
	void ShowDisYuan(int mDibiaonum)
	{
		if(M_Distance<120)
		{
			GameObject LineObject = (GameObject)Instantiate(DisActive_YuanDian_Sprite);
			
			LineObject.SetActive(true);
			
			LineObject.transform.parent = this.transform.parent;
			
			LineObject.transform.localScale = DisActive_YuanDian_Sprite.transform.localScale;
			
			float DisX = (Zuob2.x-Zuob1.x)/2;
			
			float DisY = (Zuob2.y - Zuob1.y)/2;
			
			LineObject.transform.localPosition = new Vector3(Zuob1.x+DisX,Zuob1.y+DisY,0);
			
		}
		else
		{
			for(int i = 2; i < mDibiaonum; i++)
			{
				GameObject LineObject = (GameObject)Instantiate(DisActive_YuanDian_Sprite);
				
				LineObject.SetActive(true);
				
				LineObject.transform.parent = this.transform.parent;
				
				LineObject.transform.localScale = DisActive_YuanDian_Sprite.transform.localScale;
				
				float DisX = (Zuob2.x-Zuob1.x)/(mDibiaonum+1);
				
				float DisY = (Zuob2.y - Zuob1.y)/(mDibiaonum+1);
				
				LineObject.transform.localPosition = new Vector3(Zuob1.x+DisX*i,Zuob1.y+DisY*i,0);
			}
		}
	}
	void ShowJianTou(int mDibiaonum)
	{
	
		if(M_Distance<120)
		{
			GameObject Jian_tou = (GameObject)Instantiate(JianTou_Sprite);

			Jian_tou.SetActive(true);

			Jian_tou.transform.parent = this.transform.parent;

			Jian_tou.transform.localScale = JianTou_Sprite.transform.localScale;

			float DisX = (Zuob2.x-Zuob1.x)/2;

			float DisY = (Zuob2.y - Zuob1.y)/2;

			Jian_tou.transform.localPosition = new Vector3(Zuob1.x+DisX,Zuob1.y+DisY,0);

			float angles;
			float n_Time = 0;
			if(Zuob2.y-Zuob1.y > 0)
			{
				if(Zuob2.x-Zuob1.x < 0)
				{
					angles = Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
					
				}
				else{
					angles = -Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
				}
				
			}else{
				if(Zuob2.x-Zuob1.x < 0)
				{
					angles = 180f-Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
				}
				else{
					angles = 180f+Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
				}
			}
			Jian_tou.transform.eulerAngles = new Vector3(0,0,angles);

			JianTouMove mJianTouMove = Jian_tou.GetComponent<JianTouMove>();

			mJianTouMove.mTime = n_Time;
		}
		else
		{
			float n_Time = 0;
			for(int i = 2; i < mDibiaonum; i++)
			{
				n_Time += 0.2f;
				float DisX = (Zuob2.x-Zuob1.x)/(mDibiaonum+1);

				float DisY = (Zuob2.y - Zuob1.y)/(mDibiaonum+1);
				
				GameObject Jian_tou = (GameObject)Instantiate(JianTou_Sprite.gameObject);

				Jian_tou.SetActive(true);

				Jian_tou.transform.parent = this.transform.parent;

				Jian_tou.transform.localScale = JianTou_Sprite.transform.localScale;

				Jian_tou.transform.localPosition = new Vector3(Zuob1.x+DisX*i,Zuob1.y+DisY*i,0);

				float angles;

				if(Zuob2.y-Zuob1.y > 0)
				{
					if(Zuob2.x-Zuob1.x < 0)
					{
						angles = Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
						
					}
					else{
						angles = -Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
					}
					
				}else{
					if(Zuob2.x-Zuob1.x < 0)
					{
						angles = 180f-Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
						
					}
					else{
						angles = 180f+ Vector3.Angle(new Vector3(Mathf.Abs(Zuob2.x-Zuob1.x),Mathf.Abs(Zuob2.y-Zuob1.y),0),Jian_tou.transform.up);
					}
					
				}

				Jian_tou.transform.eulerAngles = new Vector3(0,0,angles);

				JianTouMove mJianTouMove = Jian_tou.GetComponent<JianTouMove>();
				
				mJianTouMove.mTime = n_Time;
			}
		}
	}

	public void GetAwardBtn()
	{
		// 领取奖励按钮 宝箱
		//UI3DEffectTool.ClearUIFx (BoxBtn);
		MapData.mapinstance.CloseEffect();
		if(CityGlobalData.PT_Or_CQ)
		{
			PassLevelBtn.Instance().CloseEffect ();
		}

		MapData.mapinstance.ClosewPVEGuid ();

		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_GRADE_REWARD),LoadResourceCallback2);
	}

	public void LoadResourceCallback2(ref WWW p_www,string p_path, Object p_object)
	{
////		Debug.Log ("0000000000 0-0");
//
		GameObject tempOjbect = Instantiate(p_object)as GameObject;

		tempOjbect.transform.parent = GameObject.Find ("Mapss").transform;

		tempOjbect.transform.localScale = Vector3.one;

		tempOjbect.transform.localPosition = Vector3.zero;
		
		PveStarAwardpanel mPveStarAwardpanel = tempOjbect.GetComponent<PveStarAwardpanel>();

		mPveStarAwardpanel.M_Level = litter_Lv;

		mPveStarAwardpanel.Init ();
	}
	public void POPLevelInfo()
	{
		createpveui(Startsendmasg);
	}
	void Show_Win_Type(bool pongtong)
	{
		int WinType = 0;

		if(pongtong)
		{
		    WinType = litter_Lv.win_Level;
		}
		else{
			WinType = litter_Lv.pingJia;
		}
	
		m_WinType.gameObject.SetActive (true);

		switch(WinType)
		{
		case 1:
			m_WinType.spriteName = "xiansheng";
			break;
		case 2:
			m_WinType.spriteName = "xiaosheng";
			break;
		case 3:
			m_WinType.spriteName = "wansheng";
			break;
		default:
			break;

		}

	}
	int RenWuId;

	void createpveui(bool issended)
	{
//		Debug.Log("Startsendmasg = " +Startsendmasg);

		if (issended)
		{
			UIYindao.m_UIYindao.CloseUI ();
			Startsendmasg = false;
			if(CityGlobalData.PT_Or_CQ)
			{
				if(MapData.mapinstance.Lv.ContainsKey(litter_Lv.guanQiaId))
				{

					CurLev = litter_Lv.guanQiaId;

					StartCoroutine(ChangerDataState());
					
					RenWuId = litter_Lv.renWuId;
					
//					Debug.Log("RenWuId = " +RenWuId);
					
					if(RenWuId <= 0) // <=
					{
						if(needjunzhuLevel >= litter_Lv.s_level ){//
							
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
					CurLev = litter_Lv.guanQiaId;

					StartCoroutine(ChangerDataState());
					
					
					RenWuId = litter_Lv.renWuId;
					if(RenWuId <= 0)
					{
						if(needjunzhuLevel >= litter_Lv.s_level ){//
							
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
	void ShowUIbaseBackData()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_UI),loadback);
		
	}
	IEnumerator ChangerDataState()
	{
		yield return new WaitForSeconds (1.0f);
		
		Startsendmasg = true;
	}
	void LoadRenWuBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);

		string Contain2 = ZhuXianTemp.GeTaskTitleById (RenWuId);

		string Contain1 = "\r\n"+LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT)+"\r\n"+"\r\n"+Contain2;

		string Contain3 = LanguageTemplate.GetText(LanguageTemplate.Text.FINGHT_CONDITON);

		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		uibox.setBox(title,Contain1, null,null,Comfirm,null,null,null,null);
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
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);

		string Contain1 = "\n\r"+LanguageTemplate.GetText(LanguageTemplate.Text.LEVEL_LIMIT)+litter_Lv.s_level.ToString ()+LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_UP_LEVEL)+"\n\r"+"\n\r"+LanguageTemplate.GetText(LanguageTemplate.Text.GET_EXP);

		//string Contain2 = litter_Lv.s_level.ToString ();

		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		uibox.setBox(title,Contain1, null,null,Comfirm,null,null,null,null);
	}

	GameObject tempOjbect_PVEUI;//掉落显示页面

	public void loadback(ref WWW p_www,string p_path, Object p_object)
	{
		if(CityGlobalData.PveLevel_UI_is_OPen)
		{
			return;
		}
		tempOjbect_PVEUI = Instantiate (p_object)as GameObject;
		CityGlobalData.PveLevel_UI_is_OPen = true;
		MainCityUI.TryAddToObjectList (tempOjbect_PVEUI);
		MapData.mapinstance.CloseEffect();
		if(CityGlobalData.PT_Or_CQ)
		{
			PassLevelBtn.Instance().CloseEffect ();
		}
		MapData.mapinstance.ClosewPVEGuid ();
		tempOjbect_PVEUI.transform.localPosition = new Vector3(0,400,0);
		
		tempOjbect_PVEUI.transform.localScale = new Vector3 (1,1,1);
		
		NewPVEUIManager mNewPVEUIManager = tempOjbect_PVEUI.GetComponent<NewPVEUIManager>();

		mNewPVEUIManager.mLevel = litter_Lv;
		mNewPVEUIManager.Init ();
	}
}
