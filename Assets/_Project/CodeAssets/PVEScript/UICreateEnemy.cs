//#define USING_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICreateEnemy : MonoBehaviour
{
    [HideInInspector]
    public GameObject IconSamplePrefab;
	
    int distance = 97;//敌人头像距离
    int countDistance = 250;//偏移量

	List<LegendNpcTemplate> mLegendNpcTemplateList = new List<LegendNpcTemplate>();
	List<NpcTemplate> mNpcTemplate = new List<NpcTemplate>();
    void Start()
    {
       
    }

    public void InItEnemyList(int levelID)
    {
		mLegendNpcTemplateList.Clear();
		mNpcTemplate.Clear();
//		Debug.Log ("levelID = "+levelID);
		if(!CityGlobalData.PT_Or_CQ)
		{
			LegendPveTemplate L_pvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(levelID);
			
			int npcid = L_pvetemp.npcId;

			mLegendNpcTemplateList = LegendNpcTemplate.GetLegendNpcTemplates_By_npcid(npcid);

			for (int i = 0; i < mLegendNpcTemplateList.Count-1; i ++)
			{
				for(int j = i+1; j < mLegendNpcTemplateList.Count; )
				{
					if(mLegendNpcTemplateList[i].modelId == mLegendNpcTemplateList[j].modelId)
					{
						if(mLegendNpcTemplateList[i].type < mLegendNpcTemplateList[j].type)
						{
							mLegendNpcTemplateList[i] = mLegendNpcTemplateList[j];
						}
						mLegendNpcTemplateList.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			for (int i = 0; i < mLegendNpcTemplateList.Count-1; i ++)
			{
				for(int j = i+1 ; j < mLegendNpcTemplateList.Count; j++)
				{
					if(mLegendNpcTemplateList[i].type < mLegendNpcTemplateList[j].type)
					{
						LegendNpcTemplate mLegendNpc = mLegendNpcTemplateList[i];
						mLegendNpcTemplateList[i] = mLegendNpcTemplateList[j];
						mLegendNpcTemplateList[j] = mLegendNpc ;
					}
				}
			}
		}
		else
		{
			PveTempTemplate pvetemp = PveTempTemplate.GetPveTemplate_By_id(levelID);
			
			int npcid = pvetemp.npcId;

			mNpcTemplate = NpcTemplate.GetNpcTemplates_By_npcid(npcid);

			for (int i = 0; i < mNpcTemplate.Count-1; i ++)
			{
				for(int j = i+1; j < mNpcTemplate.Count; )
				{
					if(mNpcTemplate[i].modelId == mNpcTemplate[j].modelId)
					{
						if(mNpcTemplate[i].type < mNpcTemplate[j].type)
						{
							mNpcTemplate[i] = mNpcTemplate[j];
						}
						mNpcTemplate.RemoveAt(j);
					}
					else{
						j ++;
					}
				}
				
			}
			for (int i = 0; i < mNpcTemplate.Count-1; i ++)
			{
				for(int j = i+1 ; j < mNpcTemplate.Count; j++)
				{
					if(mNpcTemplate[i].type < mNpcTemplate[j].type)
					{
						NpcTemplate m_mNpcTemplate = mNpcTemplate[i];
						mNpcTemplate[i] = mNpcTemplate[j];
						mNpcTemplate[j] = m_mNpcTemplate ;
					}
				}
			}
			#if USING_DEBUG
			foreach(NpcTemplate mlengend in mNpcTemplate)
			{
				Debug.Log("mlengend.modelId = "+mlengend.modelId);
				Debug.Log("mlengend.type = "+mlengend.type);
			}
			#endif
		}
       
        getEnemyData();
    }

    void getEnemyData()
    {
//		List<LegendNpcTemplate> mLegendNpcTemplateList = new List<LegendNpcTemplate>();
//		List<NpcTemplate> mNpcTemplate = new List<NpcTemplate>();
		if (CityGlobalData.PT_Or_CQ) {

			if (IconSamplePrefab == null)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreatePuTong_Enemys);
			}
			else
			{
				WWW temp = null;
				OnCreatePuTong_Enemys(ref temp, null, IconSamplePrefab);
			}
		} 
		else 
		{
			if (IconSamplePrefab == null)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnCreateCQ_Enemys);
			}
			else
			{
				WWW temp = null;
				OnCreateCQ_Enemys(ref temp, null, IconSamplePrefab);
			}
		}
       
    }

    private void OnCreatePuTong_Enemys(ref WWW p_www, string p_path, Object p_object)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = p_object as GameObject;
        }
//		Debug.Log("mNpcTemplate.Count = "+mNpcTemplate.Count);

		int count = mNpcTemplate.Count;
		if(count > 4)
		{
			count =  4;
		}
		for (int n = 0; n < count; n++)
        {
            GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
		
			iconSampleObject.SetActive (true);
            iconSampleObject.transform.parent = transform;
			int allenemy = mNpcTemplate.Count;
			if(allenemy > 4)
			{
				allenemy = 4;
			}

			iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
            var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
			NpcTemplate m_NpcTemplate = NpcTemplate.GetNpcTemplate_By_id(mNpcTemplate[n].id);
			
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(m_NpcTemplate.EnemyName);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + m_NpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(m_NpcTemplate.desc).description;
			
			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";
//			Debug.Log("m_NpcTemplate.type = "+m_NpcTemplate.type);
			if(m_NpcTemplate.type == 4)
			{
//				Debug.Log("boss");
				rightButtomSpriteName = "boss";

			}
			if(m_NpcTemplate.type == 5)
			{
				rightButtomSpriteName = "JunZhu";
			}
			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(3, m_NpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			if(m_NpcTemplate.type == 4 || m_NpcTemplate.type == 5)
			{
				iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
				iconSampleManager.ShowBOssName(Enemy_Namestr.Name);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -23, 0);
				iconSampleObject.transform.localScale = new Vector3(0.8f,0.8f,1);
			}
        }
    }

	private void OnCreateCQ_Enemys(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		int count = mLegendNpcTemplateList.Count;
		if(count > 4)
		{
			count = 4;
		}
		for (int n = 0; n < count; n++)
		{
			GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
			
			iconSampleObject.SetActive (true);
			iconSampleObject.transform.parent = transform;

			int allenemy = mLegendNpcTemplateList.Count;
			if(allenemy > 4)
			{
				allenemy = 4;
			}

			iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -20, 0);
			var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
			
			LegendNpcTemplate m_LegendNpcTemplate= LegendNpcTemplate.GetLegendNpcTemplate_By_id(mLegendNpcTemplateList[n].id);

			string leftTopSpriteName = null;
			var rightButtomSpriteName = "";

			if(m_LegendNpcTemplate.type == 4)
			{
				rightButtomSpriteName = "boss";

			}
			if(m_LegendNpcTemplate.type == 5)
			{
				rightButtomSpriteName = "JunZhu";
			}
	
			NameIdTemplate Enemy_Namestr = NameIdTemplate.getNameIdTemplateByNameId(m_LegendNpcTemplate.EnemyName);
			var popTextTitle = Enemy_Namestr.Name + " " + "LV" + m_LegendNpcTemplate.level.ToString();
			var popTextDesc = DescIdTemplate.getDescIdTemplateByNameId(m_LegendNpcTemplate.desc).description;
//		
//			Debug.Log("mLegendNpcTemplateList[n].id = "+mLegendNpcTemplateList[n].id);
//			Debug.Log("m_LegendNpcTemplate.icon = "+m_LegendNpcTemplate.icon);

			iconSampleManager.SetIconType(IconSampleManager.IconType.pveHeroAtlas);
			iconSampleManager.SetIconBasic(3, m_LegendNpcTemplate.icon.ToString());
			iconSampleManager.SetIconPopText(0, popTextTitle, popTextDesc, 1);
			iconSampleManager.SetIconDecoSprite(leftTopSpriteName, rightButtomSpriteName);
			if(m_LegendNpcTemplate.type == 4 || m_LegendNpcTemplate.type == 5)
			{
				iconSampleManager.ShowBOssName(Enemy_Namestr.Name);
				iconSampleObject.transform.localScale = new Vector3(0.9f,0.9f,1);
			}
			else
			{
				iconSampleObject.transform.localPosition = new Vector3((allenemy - n) * distance - countDistance, -23, 0);
				iconSampleObject.transform.localScale = new Vector3(0.8f,0.8f,1);
			}
		}
	}

}
