using UnityEngine;
using System.Collections;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EquipAttribute : MonoBehaviour {

	public UISlider m_Slider;
	
	public UISprite m_arrow;

	public UILabel m_count;

	public enum AttributeType:int
	{
		gongji = 1,
		fangyu,
		shengming,
		tongshuai,
		wuli,
		mouli
	}

	public AttributeType m_attributeType;


    float CalculateNum(BagItem tempBagItem)
    {
        ZhuangBei tempBaseZhuangBei = ZhuangBei.getZhuangBeiById(tempBagItem.itemId); //基本数值
        int tempQianghuaId = int.Parse(ZhuangBei.getZhuangBeiById(tempBagItem.itemId).qianghuaId);
        QiangHuaTemplate template = QiangHuaTemplate.GetTemplateByItemId(tempQianghuaId, tempBagItem.qianghuaHighestLv); //获取攻击 防御 生命上限；

        int tempPingzhi = tempBagItem.pinZhi;
        int tempLevel = tempBagItem.qianghuaHighestLv;
        int tempBaseNum = 0;
        int tempCountNum = 0;

        switch (m_attributeType)
        {
            case AttributeType.gongji:
                {
                    tempBaseNum = int.Parse(tempBaseZhuangBei.gongji);
                    tempCountNum = tempBaseNum + ((template == null) ? (0) : (template.gongji));
                    break;
                }
            case AttributeType.fangyu:
                {
                    tempBaseNum = int.Parse(tempBaseZhuangBei.fangyu);
                    tempCountNum = tempBaseNum + ((template == null) ? (0) : (template.fangyu));
                    break;
                }
            case AttributeType.shengming:
                {
                    tempBaseNum = int.Parse(tempBaseZhuangBei.shengming);
                    tempCountNum = tempBaseNum + ((template == null) ? (0) : (template.shengming));
                    break;
                }
            case AttributeType.tongshuai:
                {
                    //tempBaseNum = int.Parse(tempBaseZhuangBei.tongli);
                    //if (tempPingzhi <= 1)
                    //{
                    //    tempPingzhi = 0;
                    //    tempLevel = 0;
                    //}
                    //tempCountNum = CityGlobalData.GetUpdrageData(tempPingzhi, tempLevel) + tempBaseNum;//统帅，武力，智谋
                    break;
                }
            case AttributeType.wuli:
                {
                    //tempBaseNum = int.Parse(tempBaseZhuangBei.wuli);
                    //if (tempPingzhi <= 1)
                    //{
                    //    tempPingzhi = 0;
                    //    tempLevel = 0;
                    //}
                    //tempCountNum = CityGlobalData.GetUpdrageData(tempPingzhi, tempLevel) + tempBaseNum;
                    break;
                }
            case AttributeType.mouli:
                {
                    //tempBaseNum = int.Parse(tempBaseZhuangBei.mouli);
                    //if (tempPingzhi < 3)
                    //{
                    //    tempPingzhi = 0;
                    //    tempLevel = 0;
                    //}
                    //tempCountNum = CityGlobalData.GetUpdrageData(tempPingzhi, tempLevel) + tempBaseNum;
                    break;
                }
            default: break;
        }
        return (tempCountNum == 0) ? 0.0f : tempBaseNum * 1.0f / tempCountNum;
    }

    public void InitWithItem(BagItem tempBagItem, int tempBagData, int tempCount)
    {
        string tempSpriteName = "";
        Debug.Log(CalculateNum(tempBagItem));
        m_Slider.value = CalculateNum(tempBagItem);
        m_count.text = tempCount.ToString();

        if (tempBagData == tempCount)
        {
            m_arrow.gameObject.SetActive(false);
            return;
        }
        else if (tempBagData > tempCount)
        {
            tempSpriteName = "arrow_down";
        }
        else
        {
            tempSpriteName = "arrow_up";
        }
        m_arrow.gameObject.SetActive(true);
        m_arrow.spriteName = tempSpriteName;
    }
}
