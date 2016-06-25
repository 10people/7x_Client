using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EquipGrowthMaterialUseManagerment  
{
    public struct MaterialInfo
    {
        public long dbid;
        public int materialId;
        public int type;
        public string icon;
        public string count;
        public bool isSelected;
        public int quality;
        public int buwei;
        public bool isTouchControl;
        public int materialEXP;
        //inlay use
        public int inlayColor;
        public int expCurr;
        public int expNeed;
        public int MaxLevel;
        public int levelCurr;
        public int attribute;
    };

	public static List<MaterialInfo> listMaterials = new List<MaterialInfo>();

	public static List<long> listTouchedId = new List<long>();
 

    public static Dictionary<int, int> listCurrentAddExp = new Dictionary<int, int>();
    public static int CurrentExpIndex = 0; 
    public  static bool m_IsSurpassLimited = false;

    public static int m_TotalAddExp = 0;
    public static bool m_IsTotalSurpassLimited;
   public static int m_RealCurrentAddExp = 0;


    public static bool strengthenIsOn = false;
    public static bool materialItemTouched = false;

    public struct TopuchInfo
    {
        public long _dbid;
        public int _itemid;
    };
    public static TopuchInfo m_MaterialId;

    public static int m_EuipId = 0;

    public static bool m_IsStrengthen = false;
    public static bool materialItemReduce = false;

    public static bool touchIsEnable = true;
    public static bool touchReduceIsEnable = true;


    public static int equipLevel;
    public static int equipAdvanceid;
    public static int Levelsaved;

    public static int m_GemLevel;
    public static int m_GemLevelsaved;
    public static void AddUseMaterials(TopuchInfo materialId)//添加要强化的Id
	{
	  for(int i = 0;i < listMaterials.Count;i++)
	  {
		if(listMaterials[i].materialId == materialId._itemid 
                && listMaterials[i].dbid == materialId._dbid)
		{	 
		 listTouchedId.Add(listMaterials[i].dbid);
		 break;
		}
	  }
	}

    

	public static void ReduceUseMaterials(TopuchInfo material)//删除Id
	{
		//for(int i = 0;i < listMaterials.Count;i++)
		//{
		//	if(listMaterials[i].materialId == material._itemid
  //              && listMaterials[i].dbid == material._dbid)
		//	{
   			  RemoveItem(material._dbid);
  //              return;
		//	}
		//}
	}
	private static void RemoveItem(long id)
	{
     int size = listTouchedId.Count;
      for (int i = 0;i < size; i++)
	  {
        if (listTouchedId[i] == id)
		{
           listTouchedId.RemoveAt(i);
           return;
		}
	  }
	}

    //public static void QuicklyStrengthen(int equipId,int curr,int max,int type)//一键强化ID统计
    //{
    //    listTouchedId.Clear();
    //    int maxExpSum = curr;//
    //   // Debug.Log("listMaterials.CountlistMaterials.Count ::" + listMaterials.Count);
    //    for (int i = 0; i < listMaterials.Count; i++)
    //    {
    //        int _count = int.Parse(listMaterials[i].count);
    //            for (int j = 0; j < _count; j++)
    //            {
    //                if (maxExpSum < ExpXxmlTemp.GetNeedMaxExpByExpId(ZhuangBei.GetExpIdBy_EquipId(m_EuipId)))
    //                {
    //                    //if (ExpXxmlTemp.GetMaxLevelByAddExp(ZhuangBei.GetExpIdBy_EquipId(equipId), maxExpSum, EquipGrowthMaterialUseManagerment.Levelsaved) < JunZhuData.Instance().m_junzhuInfo.level)
    //                    //{
    //                        maxExpSum += listMaterials[i].materialEXP;
    //                        listTouchedId.Add(listMaterials[i].dbid);
    //                    //}
    //                    //else
    //                    //{
    //                    //  //  Debug.Log("listTouchedId.Count listTouchedId.Count ::" + listTouchedId.Count);
    //                    //    return;
    //                    //}
    //                }
    //                //}
    //                //else if (type == 1 && listMaterials[i].type == 1)
    //                //{
    //                //    if (maxExpSum < ExpXxmlTemp.GetNeedMaxExpByExpId(ZhuangBei.GetExpIdBy_EquipId(m_EuipId)))
    //                //    {
    //                //        maxExpSum += listMaterials[i].materialEXP;
    //                //        listTouchedId.Add(listMaterials[i].dbid);
    //                //    }
    //                //}
    //                //else
    //                //{
    //                //    if (maxExpSum < ExpXxmlTemp.GetNeedMaxExpByExpId(ZhuangBei.GetExpIdBy_EquipId(m_EuipId)))
    //                //    {
    //                //        maxExpSum += listMaterials[i].materialEXP;
    //                //        listTouchedId.Add(listMaterials[i].dbid);
    //                //    }
    //                //}

     
    //            }
    //        }
    //        //else
    //        //{
    //        //    if (maxExpSum < ExpXxmlTemp.GetNeedMaxExpByExpId(ZhuangBei.GetExpIdBy_EquipId(m_EuipId)))
    //        //    {
    //        //        listTouchedId.Add(listMaterials[i].dbid);
    //        //    }
    //        //}
     
    //}

}
