using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using qxmobile.protobuf;

public class HouseSecretCardShower : MonoBehaviour
{
    [HideInInspector]
    public HouseBasic m_House;

    public List<UISprite> StarList;
    public UITexture Image;
    public UILabel Name;
    public UILabel Desc;
    public UIEventListener m_Listener;

    public void SetSecretCard(MibaoInfo tempInfo)
    {
        SetStar(tempInfo.star);
        var tempXmlTemp = MiBaoXmlTemp.templates.FirstOrDefault(TreasureItem => TreasureItem.id == tempInfo.miBaoId);

		Name.text = NameIdTemplate.GetTemplates().FirstOrDefault(NameIDItem => NameIDItem.nameId == tempXmlTemp.nameId).Name;
        
		{
			Desc.text =
				DescIdTemplate.GetTemplates().FirstOrDefault(DescIDItem => DescIDItem.descId == tempXmlTemp.descId).description;
		}
    }

    /// <summary>
    /// Set secret card stars active or deactive.
    /// </summary>
    /// <param name="num">active num</param>
    public void SetStar(int num)
    {
        if (num < 0 || num > StarList.Count)
        {
            Debug.LogError("Not correct num:" + num + ", abort setting star.");
            return;
        }

        for (int i = 0; i < StarList.Count; i++)
        {
            StarList[i].gameObject.SetActive(i < num);
        }
    }

    private void OnCloseClick(GameObject go)
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        m_Listener.onClick = OnCloseClick;

        HouseModelController.TryAddToHouseDimmer(gameObject);
    }

    void OnDisable()
    {
        m_Listener.onClick = null;

        HouseModelController.TryRemoveFromHouseDimmer(gameObject);
    }
}
