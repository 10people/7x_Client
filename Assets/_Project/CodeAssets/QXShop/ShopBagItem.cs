using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShopBagItem : MonoBehaviour
{

    private ShopSellGoodInfo sellGoodInfo;

    private IconSampleManager iconSample;

    private int sellNum;
    private bool isAdding = false;
    private float addGrapTime = 0.2f;

    /// <summary>
    /// Gets the bag item info.
    /// </summary>
    /// <param name="tempBagItemInfo">Temp bag item info.</param>
    public void GetBagItemInfo(ShopSellGoodInfo tempInfo)
    {
        sellGoodInfo = tempInfo;
        iconSample = gameObject.GetComponent<IconSampleManager>();
        isAdding = false;
        sellNum = 0;

        iconSample.SubButton.SetActive(false);

        iconSample.SetIconByID(sellGoodInfo.itemId, "x" + sellGoodInfo.itemNum, 2);
        iconSample.SetIconBasicDelegate(true, true, AddCount, StartAddContinue, StopAddContinue);
        iconSample.SetIconButtonDelegate(null, null, DelateSellNum);
        iconSample.SetIconEffect(true, true);
    }

    /// <summary>
    /// Adds the count.
    /// </summary>
    /// <param name="obj">Object.</param>
    private void AddCount(GameObject obj)
    {
        if (sellNum < sellGoodInfo.itemNum)
        {
            sellNum = sellNum > sellGoodInfo.itemNum ? sellGoodInfo.itemNum : sellNum + 1;

            iconSample.SubButton.SetActive(true);
            iconSample.RightButtomCornorLabel.text = sellNum + "/" + sellGoodInfo.itemNum;

            ShopPage.shopPage.GetSellBagItem(sellGoodInfo, 1);
        }
    }

    /// <summary>
    /// Adds the count continue.
    /// </summary>
    /// <param name="go">Go.</param>
    private void StartAddContinue(GameObject obj)
    {
        isAdding = true;
        StartCoroutine(AddSellObjCoroutine());
    }
    /// <summary>
    /// Stops the add continue.
    /// </summary>
    /// <param name="obj">Object.</param>
    private void StopAddContinue(GameObject obj)
    {
        //Coroutine stop automatically.
        isAdding = false;
    }
    /// <summary>
    /// Delates the sell number.
    /// </summary>
    /// <param name="obj">Object.</param>
    private void DelateSellNum(GameObject obj)
    {
        if (sellNum > 0)
        {
            sellNum--;

            iconSample.SubButton.SetActive(sellNum > 0);
            iconSample.RightButtomCornorLabel.text = sellNum > 0 ? sellNum + "/" + sellGoodInfo.itemNum : "x" + sellGoodInfo.itemNum;

            ShopPage.shopPage.GetSellBagItem(sellGoodInfo, 2);
        }
    }

    IEnumerator AddSellObjCoroutine()
    {
        while (isAdding)
        {
            yield return new WaitForSeconds(addGrapTime);
            yield return new WaitForEndOfFrame();
            AddCount(gameObject);
        }
    }
}