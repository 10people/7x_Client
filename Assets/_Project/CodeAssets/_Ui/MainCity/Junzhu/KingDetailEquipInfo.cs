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
        m_IconList.ForEach(item => item.GetComponent<EventHandler>().m_click_handler += ShowEquipOfBody);
    }

    void OnDestroy()
    {
        m_IconList.ForEach(item => item.GetComponent<EventHandler>().m_click_handler -= ShowEquipOfBody);
    }

    void Start()
    {
        if (m_BagItemDic != null)
        {
            ShowEquipInfo();
        }
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

                var color = CommonItemTemplate.getCommonItemTemplateById(m_BagItemDic[i].itemId).color - 1;
                var sprite = m_QualityList[i].GetComponent<UISprite>();
                if (color > 0)
                {
                    sprite.spriteName = IconSampleManager.QualityPrefix + color;
                    //if (IconSampleManager.FreeQualityFrameSpriteName.Contains(color))
                    //{
                    //    sprite.SetDimensions(IconSampleManager.FreeQualityFrameLength, IconSampleManager.FreeQualityFrameLength);
                    //}
                    //else if (IconSampleManager.FrameQualityFrameSpriteName.Contains(color))
                    //{
                    //    sprite.SetDimensions(IconSampleManager.FrameQualityFrameLength, IconSampleManager.FrameQualityFrameLength);
                    //}
                    //else if (IconSampleManager.MibaoPieceQualityFrameSpriteName.Contains(color))
                    //{
                    //    sprite.SetDimensions(IconSampleManager.MibaoPieceQualityFrameLength, IconSampleManager.MibaoPieceQualityFrameLength);
                    //}
                    sprite.gameObject.SetActive(true);
                }
                else
                {
                    sprite.gameObject.SetActive(false);
                }
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
                            break;
                        }
                    }

                    m_ShowedDetailEquipInfo.GetEquipInfo(id, buwei);
                    break;
                }
            }
        }

        m_ShowedDetailEquipInfo.gameObject.SetActive(true);
    }
}