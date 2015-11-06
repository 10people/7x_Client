using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICreateEnemy : MonoBehaviour
{
    [HideInInspector]
    public GameObject IconSamplePrefab;

    List<int> GetPveEnemyId = new List<int>();

    List<int> soldires = new List<int>();
    List<int> heros = new List<int>();
    List<int> Bosses = new List<int>();
    List<int> Zhi_Ye = new List<int>();

    int EnemyNumBers = 0;//显示敌人数量
    int distance = 120;//敌人头像距离
    int countDistance = 360;//偏移量
	List <GameObject> mHerosIcon = new List<GameObject> (); 

    void Start()
    {
       
    }

    public void InItEnemyList(int leveType)
    {
		foreach(GameObject con in mHerosIcon)
		{
			Destroy(con);

		}
		mHerosIcon.Clear ();

		soldires.Clear();
		heros.Clear();
		Bosses.Clear();
		Zhi_Ye.Clear();
		if (leveType == 0)
        {
            EnemyNumBers = 6;
        }

        else if (leveType == 1 || leveType == 2)
        {
            EnemyNumBers = 4;
        }
		//Debug.Log ("GetPveTempID.CurLev = "+GetPveTempID.CurLev);
		if(!CityGlobalData.PT_Or_CQ)
		{
			LegendPveTemplate L_pvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(Pve_Level_Info.CurLev);
			
			int npcid = L_pvetemp.npcId;
		
			List<LegendNpcTemplate> mLegendNpcTemplateList = LegendNpcTemplate.GetLegendNpcTemplates_By_npcid(npcid);
			//Debug.Log("npcid t = " +npcid);
			foreach(LegendNpcTemplate mLegendNpcTemplate in mLegendNpcTemplateList)
			{
				int icn = int.Parse(mLegendNpcTemplate.icon);

				if(icn != 0 && mLegendNpcTemplate.type == 4&& !Bosses.Contains(mLegendNpcTemplate.EnemyId)) // boss
				{
					Bosses.Add(mLegendNpcTemplate.id);
				}
				if(icn != 0 && mLegendNpcTemplate.type == 3&& !heros.Contains(mLegendNpcTemplate.id)) // hero
				{				
						heros.Add(mLegendNpcTemplate.id);
				}
				if(icn != 0 && mLegendNpcTemplate.type == 2&& !soldires.Contains(mLegendNpcTemplate.id)) // Solider
				{
						soldires.Add(mLegendNpcTemplate.id);
				}
			}

			for (int i = 0; i < soldires.Count-1; i ++)
			{
				
				LegendNpcTemplate m_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (soldires [i]);
				
				for(int j = i+1; j < soldires.Count; )
				{
					LegendNpcTemplate j_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (soldires [j]);
					if(m_LegendNpcTemplate.profession == j_LegendNpcTemplate.profession)
					{
						
						soldires.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}

			for (int i = 0; i < heros.Count-1; i ++)
			{
				//Debug.Log("heros[i] = "+heros[i]);
				LegendNpcTemplate m_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (heros [i]);
				
				for(int j = i+1; j < heros.Count; )
				{
					LegendNpcTemplate j_LegendNpcTemplate = LegendNpcTemplate.GetLegendNpcTemplate_By_id (heros [j]);
					if(m_LegendNpcTemplate.profession == j_LegendNpcTemplate.profession)
					{
						
						heros.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			//GetPveEnemyId = LegendNpcTemplate.GetEnemyId_By_npcid(npcid);
		}
		else
		{
			PveTempTemplate pvetemp = PveTempTemplate.GetPveTemplate_By_id(Pve_Level_Info.CurLev);
			
			int npcid = pvetemp.npcId;
			//Debug.Log("npcid  = " +npcid);
			//Debug.Log("npcid = " +npcid);
			List<NpcTemplate> mNpcTemplateList = NpcTemplate.GetNpcTemplates_By_npcid(npcid);

			//Debug.Log("mNpcTemplateList.count = "+mNpcTemplateList.Count);

			foreach(NpcTemplate mNpcTemplate in mNpcTemplateList)
			{
				int icn = int.Parse(mNpcTemplate.icon);
				if(icn != 0&&mNpcTemplate.type == 4&& !Bosses.Contains(mNpcTemplate.id)) // boss
				{
					Bosses.Add(mNpcTemplate.id);
				}	
				if(icn != 0 && mNpcTemplate.type == 3&& !heros.Contains(mNpcTemplate.id)) // hero
				{
					heros.Add(mNpcTemplate.id);
				}
				if(icn != 0 && mNpcTemplate.type == 2&& !soldires.Contains(mNpcTemplate.id)) // Solider
				{
					   soldires.Add(mNpcTemplate.id);

				}
			}
			for (int i = 0; i < soldires.Count-1; i ++)
			{
				
				NpcTemplate m_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (soldires [i]);
				
				for(int j = i+1; j < soldires.Count; )
				{
					NpcTemplate j_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (soldires [j]);
					if(m_NpcTemplate.profession == j_NpcTemplate.profession)
					{
						
						soldires.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			
			
			for (int i = 0; i < heros.Count-1; i ++)
			{
				//Debug.Log("heros[i] = "+heros[i]);
				NpcTemplate m_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (heros [i]);
				
				for(int j = i+1; j < heros.Count; )
				{
					NpcTemplate j_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id (heros [j]);
					if(m_NpcTemplate.profession == j_NpcTemplate.profession)
					{
						heros.Remove(heros [j]);
					}else
					{
						j ++;
					}
				}
				
			}

		}

       
        getEnemyData();
    }

    void getEnemyData()
    {
        //List<string> EyName = new List<string>(GetPveTempID.NewEnemy.Keys);

        int bossnum = Bosses.Count;
        int heronum = heros.Count;
        int solder = soldires.Count;

		//Debug.Log ("boss个数：" + bossnum);
		//Debug.Log ("hero个数：" + heronum);
		//Debug.Log ("soldier个数：" + solder);

        if (bossnum > 0)//BOSS个数不为0
        {
            if (bossnum < EnemyNumBers)//boss不大于大于6个
            {
                if (heronum > 0)//w武将个数大于0
                {
                    if (heronum + bossnum < EnemyNumBers)//w武将和bpss的总个数小于6
                    {
                        if (solder > 0)
                        {
                            if (heronum + bossnum + solder > EnemyNumBers)
                            {
                                createBoss(bossnum);
                                createHeros(heronum);
                                createSoliders(EnemyNumBers - (bossnum + heronum));
                            }
                            else
                            {
                                createBoss(bossnum);
                                createHeros(heronum);
                                createSoliders(solder);
                            }
                        }
                        else
                        {
                            createBoss(bossnum);
                            createHeros(heronum);
                        }
                    }
                    else
                    {//boss和武将的和大于6了 只创建6个
                        createBoss(bossnum);
                        createHeros(EnemyNumBers - bossnum);
                    }
                }
                else
                {
                    //ww武将为0
                    if (solder > 0)
                    {
                        if (solder + bossnum > EnemyNumBers)
                        {
                            createBoss(bossnum);
                            createSoliders(EnemyNumBers - bossnum);
                        }
                        else
                        {
                            createBoss(bossnum);
                            createSoliders(solder);
                        }
                    }
                    else
                    {
                        createBoss(bossnum);
                    }
                }
            }
            else
            {
                //boss的个数大于6
                createBoss(EnemyNumBers);
            }
        }
        else
        {
            //假如boss的个数为0000000000
            if (heronum > 0)//w武将个数大于0
            {
                if (heronum < EnemyNumBers)//w武将和bpss的总个数小于6
                {
                    if (solder > 0)
                    {
                        if (heronum + solder <= EnemyNumBers)
                        {
                            createHeros(heronum);
                            createSoliders(solder);
                        }
                        else
                        {
                            createHeros(heronum);
                            createSoliders(EnemyNumBers - heronum);
                        }
                    }
                    else
                    {
                        createHeros(heronum);
                    }
                }
                else
                {
                    createHeros(EnemyNumBers);
                }
            }
            else
            {
                if (solder > EnemyNumBers)
                {
                    createSoliders(EnemyNumBers);
                }
                else
                {
                    createSoliders(solder);
                }
            }
        }
        //this.gameObject.GetComponent<UIGrid>().repositionNow = true;
    }

    private void OnCreateBossCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        for (int n = 0; n < createBossPara; n++)
        {

            GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			iconSampleObject.SetActive (true);
            iconSampleObject.transform.parent = transform;
            var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            if (allenemy >= EnemyNumBers)
            {
                iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - n) * distance - countDistance, 0, 0);
            }
            else
            {
                iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, 0, 0);
            }

			if(!CityGlobalData.PT_Or_CQ)
			{
				//EnemyNameid = LegendNpcTemplate.GetEnemyNameId_By_EnemyId(Bosses[n]);

				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(Bosses[n]);

				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;

				string leftTopSpriteName = null;

				var rightButtomSpriteName = "boss";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{

				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(Bosses[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;

				var rightButtomSpriteName = "boss";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);

			}
    
        }
    }

    private int createBossPara;

    private int allenemy
    {
        get { return Bosses.Count + heros.Count + soldires.Count; }
    }

    void createBoss(int i)
    {
        createBossPara = i;

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateBossCallBack);
        }
        else
        {
            WWW temp = null;
            OnCreateBossCallBack(ref temp, null, IconSamplePrefab);
        }
    }

    private void OnCreateHeroCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        for (int n = 0; n < createHeroPara; n++)
        {
            GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);
			iconSampleObject.SetActive (true);
            iconSampleObject.transform.parent = transform;
            var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            if (allenemy >= EnemyNumBers)
            {
                iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - Bosses.Count - n) * distance - countDistance, 0, 0);
            }
            else
            {
                iconSampleObject.transform.localPosition = new Vector3((allenemy - Bosses.Count - n) * distance - countDistance, 0, 0);
            }

			if(!CityGlobalData.PT_Or_CQ)
			{

				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(heros[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{
				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(heros[n]);

				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
				
			}
            
        }
    }

    private int createHeroPara;

    void createHeros(int i)
    {
        createHeroPara = i;

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateHeroCallBack);
        }
        else
        {
            WWW temp = null;
            OnCreateHeroCallBack(ref temp, null, IconSamplePrefab);
        }
    }

    private void OnCreateSoldierCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }

        for (int n = 0; n < createSoldierPara; n++)
        {
            GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			mHerosIcon.Add(iconSampleObject);

			iconSampleObject.SetActive (true);
            iconSampleObject.transform.parent = transform;
            var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

            if (allenemy >= EnemyNumBers)
            {
                iconSampleObject.transform.localPosition = new Vector3((EnemyNumBers - (Bosses.Count + heros.Count)
                                                                   - n) * distance - countDistance, 0, 0);
            }
            else
            {
                iconSampleObject.transform.localPosition = new Vector3((allenemy - (Bosses.Count + heros.Count)
                                                                    - n) * distance - countDistance, 0, 0);
            }
            iconSampleObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

			int EnemyNameid = 0;
			
			if(!CityGlobalData.PT_Or_CQ)
			{
				LegendNpcTemplate mLeGendNpcTemp = LegendNpcTemplate.GetLegendNpcTemplate_By_id(soldires[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mLeGendNpcTemp.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mLeGendNpcTemp.level.ToString();
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mLeGendNpcTemp.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mLeGendNpcTemp.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			}
			else{

				NpcTemplate mNpcTemplate = NpcTemplate.GetNpcTemplate_By_id(soldires[n]);
				
				NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(mNpcTemplate.EnemyName);
				var popTextTitle = Enemy_Namestr.Name + " " + "LV" + mNpcTemplate.level.ToString();;
				var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(mNpcTemplate.desc).description;
				
				string leftTopSpriteName = null;
				var rightButtomSpriteName = "";
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
				iconSampleManager.SetIconBasic(10, mNpcTemplate.icon.ToString());
				iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
				iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
				
			}
           
        }
    }

    private int createSoldierPara;

    void createSoliders(int i)
    {
        createSoldierPara = i;

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateSoldierCallBack);
        }
        else
        {
            WWW temp = null;
            OnCreateSoldierCallBack(ref temp, null, IconSamplePrefab);
        }
    }
}
