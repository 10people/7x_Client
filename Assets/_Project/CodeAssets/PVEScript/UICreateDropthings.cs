using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UICreateDropthings : MonoBehaviour
{
    [HideInInspector]
    public GameObject IconSamplePrefab;

    private int awardNum;//掉落物品个数

	private int m_Leveltype;

	List <GameObject> m_AwardIcon = new List<GameObject> (); 

	List<int> First_t_items = new List<int>();

	private int FirstDropthin_Icon;

	public Level mLevl;

    public void GetAward(int levelType)
    {
		m_Leveltype = levelType;

		foreach(GameObject maward in m_AwardIcon)
		{
			Destroy(maward);
		}
		m_AwardIcon.Clear ();

        if (levelType == 0)
        {
            awardNum = 6;
        }
        else if (levelType == 1 || levelType == 2)
        {
            awardNum = 4;
        }

        List<int> t_items = new List<int>();
		First_t_items.Clear ();
		if(!CityGlobalData.PT_Or_CQ)
		{
			LegendPveTemplate Legendpvetemp = LegendPveTemplate.GetlegendPveTemplate_By_id(Pve_Level_Info.CurLev);
			char[] t_items_delimiter = { ',' };
			
			char[] t_item_id_delimiter = { '=' };
			
			string[] t_item_strings = Legendpvetemp.awardId.Split(t_items_delimiter);
			
			for (int i = 0; i < t_item_strings.Length; i++)
			{
				string t_item = t_item_strings[i];
				
				string[] t_finals = t_item.Split(t_item_id_delimiter);
				
				if(t_finals[0] != "" && !t_items.Contains(int.Parse(t_finals[0])))
				{
					t_items.Add(int.Parse(t_finals[0]));
				}
			}
			if(!mLevl.chuanQiPass)
			{
				if(Legendpvetemp.firstAwardId != null && Legendpvetemp.firstAwardId !="")
				{
					string[] First_t_item_strings = Legendpvetemp.firstAwardId.Split(t_item_id_delimiter);
					
					FirstDropthin_Icon = int.Parse(First_t_item_strings[0]);
					
					First_t_items.Add(FirstDropthin_Icon);
				}
			}
		}
		else
		{
			PveTempTemplate pvetemp = PveTempTemplate.GetPveTemplate_By_id(Pve_Level_Info.CurLev);
			//		Debug.Log ("pvetemp.awardId ：" +pvetemp.awardId);
			
			char[] t_items_delimiter = { ',' };
			
			char[] t_item_id_delimiter = { '=' };

			string[] t_item_strings = pvetemp.awardId.Split(t_items_delimiter);
			
			for (int i = 0; i < t_item_strings.Length; i++)
			{
				string t_item = t_item_strings[i];
				
				string[] t_finals = t_item.Split(t_item_id_delimiter);
				if(t_finals[0] != "")
				{
					t_items.Add(int.Parse(t_finals[0]));
				}

			}
			if(!mLevl.s_pass)
			{
				if(pvetemp.firstAwardId != null && pvetemp.firstAwardId !="")
				{
					string[] First_t_item_strings = pvetemp.firstAwardId.Split(t_item_id_delimiter);
					
					FirstDropthin_Icon = int.Parse(First_t_item_strings[0]);
					
					First_t_items.Add(FirstDropthin_Icon);
				}
			}
		}
        int initNum;
        if (awardNum >= t_items.Count)
        {
            initNum = t_items.Count;
        }
        else
        {
            initNum = awardNum;
        }

        numPara = initNum;
        itemsPara = t_items;

        if (IconSamplePrefab == null)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
        }
        else
        {
            WWW temp = null;
            OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
        }
    }

    private int numPara;
    private List<int> itemsPara;
	Vector3 FirstAwardPos = new Vector3 (0,0,0);

	float x = -240;
    private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
    {
		int pos = 0;

		int MaxPuTong_pos = 6;

		int  MaxJingYing_pos = 4;

		if(First_t_items.Count > 0)
		{
			MaxPuTong_pos = MaxPuTong_pos - First_t_items.Count;

			MaxJingYing_pos = MaxJingYing_pos - First_t_items.Count;
		}	
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		switch(mLevl.type)
		{
		case 0:
			if(!mLevl.s_pass)
			{
				if(First_t_items.Count > 0)
				{
					x = -120;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		case 1:
			if(!mLevl.s_pass)
			{
				if(First_t_items.Count > 0)
				{
					x = -120;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		case 2:
			if(!mLevl.chuanQiPass)
			{
				if(First_t_items.Count > 0)
				{
					x = -120;
					
					CreateFirsrAward ();
					//Debug.Log ("First_t_items[0] ! = "+First_t_items[0]);
				}
			}
			break;
		default:
			break;
		}

	
		for (int n = 0; n < numPara; n++)
        {
			//Debug.Log("itemsPara[n] = "+itemsPara[n]);
            List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(itemsPara[n]);

//			foreach(AwardTemp mmAwardTemp in mAwardTemp)
//			{
//				Debug.Log("mmAwardTemp.itemId = "+mmAwardTemp.itemId);
//			}

            for (int i = 0; i < mAwardTemp.Count; i++)
            {
				if(mAwardTemp[i].weight != 0)
				{
					pos += 1;
					
					if (m_Leveltype == 1 || m_Leveltype == 2)
					{
						if(pos > MaxJingYing_pos)
						{
							return;
						}
					}
					else
					{
						if(pos > MaxPuTong_pos)
						{
							return;
						}
					}
									
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;

					m_AwardIcon.Add(iconSampleObject);

					iconSampleObject.SetActive(true);

					iconSampleObject.transform.parent = transform;

					iconSampleObject.transform.localPosition = new Vector3(x + (pos-1) * 120, 1, 1);

					//FirstAwardPos = iconSampleObject.transform.localPosition;

					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

					var iconSpriteName = "";
			
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[i].itemId);

					iconSpriteName = mItemTemp.icon.ToString();

					iconSampleManager.SetIconType(IconSampleManager.IconType.item);

					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);

					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);

					var popTitle = mNameIdTemplate.Name;

					var popDesc = mdesc;

                    iconSampleManager.SetIconByID(mItemTemp.id, "", 10);
                    iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, popTitle, popDesc, 1);
				}
            }
        }
	
    }
	public void CreateFirsrAward()
	{			
//		Debug.Log ("Create FirstAward now !");

		GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
		
		m_AwardIcon.Add(iconSampleObject);
		
		iconSampleObject.SetActive(true);
		
		iconSampleObject.transform.parent = transform;
		
		iconSampleObject.transform.localPosition = new Vector3(-240, 0, 0);
		
		var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
		var iconSpriteName = "";

		List<AwardTemp> mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(FirstDropthin_Icon);

		CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[0].itemId);
		
		iconSpriteName = mItemTemp.icon.ToString();
		
		iconSampleManager.SetIconType(IconSampleManager.IconType.item);

		NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[0].itemId);
		
		string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[0].itemId);
		
		var popTitle = mNameIdTemplate.Name;
		
		var popDesc = mdesc;

        iconSampleManager.SetIconByID(mItemTemp.id, "", 15);
        iconSampleManager.SetIconPopText(mAwardTemp[0].itemId, popTitle, popDesc, 1);
		iconSampleManager.FirstWinSpr.gameObject.SetActive(true);
	}
}
