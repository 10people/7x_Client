using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KingDetailEquipInfo : MonoBehaviour
{
    public List<UISprite> m_IconList;
    public List<UISprite> m_QualityList;
    public KingDetailEquipDetailInfo m_ShowedDetailEquipInfo;

    public Dictionary<int, BagItem> m_BagItemDic = new Dictionary<int, BagItem>();

    void Awake()
    {
        m_IconList.ForEach(item => item.GetComponent<EventHandler>().m_handler += ShowEquipOfBody);
    }

    void OnDestroy()
    {
        m_IconList.ForEach(item=>item.GetComponent<EventHandler>().m_handler -= ShowEquipOfBody);
    }

    void Start()
    {
        if (m_BagItemDic != null)
        {
            ShowEquipInfo();
        }
    }

    void Update()
    {
        //if (EquipsOfBody.Instance().m_isRefrsehEquips)
        //{
        //    EquipsOfBody.Instance().m_isRefrsehEquips = false;

        //    if (m_BagItemDic != null)
        //    {
        //        ShowEquipInfo();
        //    }
        //}
    }

    void ShowEquipOfBody(GameObject tempObject) //显示玩家身上的装备信息
    {
        if (m_BagItemDic.ContainsKey(int.Parse(tempObject.name)))
        {
            if (m_BagItemDic[int.Parse(tempObject.name)].wuYi > 0 ||
                m_BagItemDic[int.Parse(tempObject.name)].tongShuai > 0 ||
                m_BagItemDic[int.Parse(tempObject.name)].mouLi > 0)
            {
                SendZhuangBeiInfo(m_BagItemDic[int.Parse(tempObject.name)].itemId, true, true,
                    int.Parse(tempObject.name));
            }
            else
            {
                SendZhuangBeiInfo(m_BagItemDic[int.Parse(tempObject.name)].itemId, true, false,
                    int.Parse(tempObject.name));
            }
        }
    }

    public void ShowEquipInfo()
    {
        for (int i = 0; i < m_IconList.Count; i++) //初始化玩家背包scrollview的item
        {
            if (m_BagItemDic.ContainsKey(i))
            {
                m_IconList[i].gameObject.SetActive(true);

                m_IconList[i].GetComponent<UISprite>().enabled = true;
                m_IconList[i].spriteName = ZhuangBei.getZhuangBeiById(m_BagItemDic[i].itemId).icon;
                m_QualityList[i].GetComponent<UISprite>().spriteName =
                    QualityIconSelected.SelectQuality(
                        ZhuangBei.GetColorByEquipID(
                            int.Parse(ZhuangBei.getZhuangBeiById(m_BagItemDic[i].itemId).icon)));
                m_QualityList[i].gameObject.SetActive(true);
            }
            else
            {
                m_QualityList[i].gameObject.SetActive(false);
            }
        }
    }

    private void SendZhuangBeiInfo(int id, bool isWear, bool isXilian, int buwei)
    {
        if (isWear)
        {
            for (int i = 0; i < ZhuangBei.templates.Count; i++)
            {
                if (ZhuangBei.templates[i].id == id)
                {
                    for (int j = 0; j < ItemTemp.templates.Count; j++)
                    {
                        if (ItemTemp.templates[j].id == int.Parse(ZhuangBei.templates[i].jinjieItem))
                        {
                            //                            itemIcon = ItemTemp.templates[j].icon;

                            break;
                        }
                    }

                    m_ShowedDetailEquipInfo.GetEquipInfo(id, m_BagItemDic[buwei].dbId, buwei, isWear, 2);
                    break;
                }
            }
        }

        m_ShowedDetailEquipInfo.gameObject.SetActive(true);
    }
}