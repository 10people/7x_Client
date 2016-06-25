using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using qxmobile.protobuf;

/// <summary>
/// self exchange box, used in old book sys in house sys.
/// </summary>
public class ExchangeSelfController : MonoBehaviour
{
    public OldBookWindow m_OldBookWindow;

    public List<GameObject> IconSampleParentList;

    [HideInInspector]
    public List<IconSampleManager> IconSampleManagerList = new List<IconSampleManager>();
    [HideInInspector]
    public GameObject IconSamplePrefab;
	private bool EffectisOpen;
    [HideInInspector]
    public List<OldBookWindow.ExchangeInfo> ExchangeSelfInfoList = new List<OldBookWindow.ExchangeInfo>();

    public void RefreshExchangeBoxSelf()
    {
        if (IconSamplePrefab != null)
        {
            WWW temp = null;
            OnIconSampleLoadCallBack(ref temp, "", IconSamplePrefab);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
    }

    private void OnIconSampleLoadCallBack(ref WWW www, string temp, Object loadedObject)
    {
        if (IconSamplePrefab == null)
        {
            IconSamplePrefab = loadedObject as GameObject;
        }
		EffectisOpen = false;
        IconSampleManagerList.Clear();
        List<OldBookWindow.ExchangeInfo> sortedList =
            ExchangeSelfInfoList.Where(item => item.itemId != 0)
                .Concat(ExchangeSelfInfoList.Where(item => item.itemId == 0))
                .ToList();

        //ergodic box items.
        for (int i = 0; i < IconSampleParentList.Count; i++)
        {
            GameObject iconSampleObject;
            if (IconSampleParentList[i].transform.childCount != 0)
            {
                iconSampleObject = IconSampleParentList[i].transform.GetChild(0).gameObject;
            }
            else
            {
                iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
            }

            //set each box manager.
            TransformHelper.ActiveWithStandardize(IconSampleParentList[i].transform, iconSampleObject.transform);
            var itemManager = iconSampleObject.GetComponent<OldBookItemManager>() ??
                              iconSampleObject.AddComponent<OldBookItemManager>();
            itemManager.BoxId = sortedList[i].boxId;
            itemManager.ItemId = sortedList[i].itemId;

            //instance icon and set
            var m_IconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		

            if (sortedList[i].itemId != 0)
            {
                m_IconSampleManager.SetIconByID(IconSampleManager.IconType.exchangeBox, sortedList[i].itemId, "", 5);
                m_IconSampleManager.SetIconBasicDelegate(false, true, OnExchangeSelfClick);
            }
            else
            {
                m_IconSampleManager.SetIconType(IconSampleManager.IconType.null_type);
                m_IconSampleManager.SetIconBasic(5);
                m_IconSampleManager.SetIconBasicDelegate();
            }
			m_IconSampleManager.m_Position_y = i;

			if(OldBookWindow.Itemids.Contains(sortedList[i].itemId)&&OldBookWindow.Itemids.Contains(sortedList[i].boxId)&& !EffectisOpen)
			{
				if( m_OldBookWindow.myPosition == m_IconSampleManager.m_Position_y)
				{
					m_IconSampleManager.m2DAnimation_Continuly.gameObject.SetActive(true);
					m_IconSampleManager.m2DAnimation_Continuly.Reset();
					m_IconSampleManager.m_Itemid = sortedList[i].itemId;
					m_IconSampleManager.m_Itemid = sortedList[i].boxId;
					m_IconSampleManager.AnimationIsOpen = true;
					EffectisOpen = true;
				}
			}
            IconSampleManagerList.Add(m_IconSampleManager);
        }
    }

    /// <summary>
    /// on box item click
    /// </summary>
    /// <param name="go"></param>
    private void OnExchangeSelfClick(GameObject go)
    {
        var iconSampleManager = go.GetComponent<IconSampleManager>();
        var itemManager = go.GetComponent<OldBookItemManager>();

        switch (m_OldBookWindow.OldBookMode)
        {
            //add to or remove from old book.
            case OldBookWindow.Mode.OldBookSelf:
                setHuanWu temp = new setHuanWu
                {
                    code = 20,
                    boxIdx = itemManager.BoxId,
                    itemId = itemManager.ItemId
                };
                SocketHelper.SendQXMessage(temp, ProtoIndexes.C_HUAN_WU_OPER);
                break;
            //add to or remove from exchange items.
            case OldBookWindow.Mode.ExchangeBoxOther:
		
//			    Debug.Log ("myself");
                bool isActiveBefore = iconSampleManager.SelectFrameSprite.gameObject.activeSelf;

				iconSampleManager.m2DAnimation.gameObject.SetActive (!isActiveBefore);
				iconSampleManager.m2DAnimation.loop = false;
			    iconSampleManager.m2DAnimation.Reset();
                IconSampleManagerList.ForEach(manager => manager.SelectFrameSprite.gameObject.SetActive(false));
			    m_OldBookWindow.myPosition = iconSampleManager.m_Position_y;
                iconSampleManager.SelectFrameSprite.gameObject.SetActive(!isActiveBefore);

                if (iconSampleManager.SelectFrameSprite.gameObject.activeSelf)
                {
                    m_OldBookWindow.MyToExchange = new OldBookWindow.ToExchange
                    {
                        boxId = itemManager.BoxId,
                        itemId = itemManager.ItemId
                    };
                }
                else
                {
                    m_OldBookWindow.MyToExchange = null;
                }

                //exchange check
                m_OldBookWindow.CheckExchange();
                break;
        }
    }
}
