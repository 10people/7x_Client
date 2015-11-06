using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class EquipInfoCharge 
{
    /// <summary>
    /// Can equipment upgrade or not.
    /// </summary>
    /// <param name="id">equipment id</param>
    /// <param name="level">equipment level</param>
    /// <returns>true if can upgrade</returns>
	public static bool CanImproveQuality(int id,int level)
	{
        //Ergodic equipment list.
        for (int i = 0; i < ZhuangBei.templates.Count; i++)
		{
            //Select specific equipment with enough upgrade level.
            if (ZhuangBei.templates[i].id == id && level >= ZhuangBei.templates[i].jinjieLv)
			{
                //Ergodic available material.
				foreach(KeyValuePair<long ,List<BagItem>> materialItem in BagData.Instance().m_playerCaiLiaoDic)
				{
					foreach (BagItem bagItem in materialItem.Value)
					{
                        //Select specific needed material.
					    if (bagItem.itemId == int.Parse(ZhuangBei.templates[i].jinjieItem))
					    {
                            //return true if material enough, or false.
					        return bagItem.cnt > int.Parse(ZhuangBei.templates[i].jinjieNum);
					    }
					}
				}
			}
		}
		return false;
	}

    /// <summary>
    /// Check equipment is in bag or not.
    /// </summary>
    /// <param name="buwei">pos id</param>
    /// <returns>true if equipment in bag</returns>
	public static bool IsEquipmentInBag(int buwei)
	{
		int tempBuwei = 0;
		switch(buwei)
		{
		case 3:tempBuwei = 1;
			break;//刀
		case 4:tempBuwei = 2;
			break;//枪
		case 5:tempBuwei = 3;
			break;//弓
		case 0:tempBuwei = 11;
			break;//头盔
		case 8:tempBuwei = 12;
			break;//肩膀
		case 1:tempBuwei = 13;
			break;//铠甲
		case 7:tempBuwei = 14;
			break;//手套
		case 2:tempBuwei = 15;
			break;//裤子
		case 6:tempBuwei = 16;
			break;
		default:
			break;
		}
		foreach(KeyValuePair<int,BagItem> item in BagData.Instance().m_playerEquipDic) 
		{
			if(item.Value.buWei == tempBuwei)
			{
				return true;
			}
		}
		return false;
	}
}
