using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class PawnshopBagItem : MonoBehaviour
{
    [HideInInspector]
    public PawnshopLayerSell PawnshopLayerSellControllor;
    [HideInInspector]
    public IconSampleManager IconSampleManager;
    [HideInInspector]
    public BagItem BagItem;
    [HideInInspector]
    public int sellNum;

    private bool isAdding = false;

    public void RefreshData(BagItem item)
    {
        BagItem = item;

        //adding = false;
        sellNum = 0;

		var itemTemp = ItemTemp.getItemTempById(BagItem.itemId);

        if (itemTemp == null)
        {
			Debug.LogError("Can't set icon sample cause item: " + BagItem.itemId + " not found");
            return;
        }

        var fgSpriteName = !string.IsNullOrEmpty(itemTemp.icon) ? itemTemp.icon : "";
        var qualityFrameSpriteName = itemTemp.color != 0
            ? IconSampleManager.QualityPrefix + (itemTemp.color - 1)
            : "";

        IconSampleManager.SetIconType(IconSampleManager.IconType.item);
        IconSampleManager.SetIconBasic(30, fgSpriteName, "x" + BagItem.cnt, qualityFrameSpriteName);
        IconSampleManager.SetIconBasicDelegate(true, true, AddCount, AddCountContinue, StopAddCountContinue);
        IconSampleManager.SetIconButtonDelegate(null, null, DelNum);
        IconSampleManager.SetIconEffect(true,true);
		//IconSampleManager.SetIconPopText (BagItem.itemId);
    }

    public void DelNum(GameObject go)
    {
        sellNum--;

        sellNum = sellNum < 0 ? 0 : sellNum;

        IconSampleManager.SubButton.SetActive(sellNum > 0);

        if (sellNum > 0)
        {
            IconSampleManager.RightButtomCornorLabel.text = sellNum + "/" + BagItem.cnt;
        }
        else
        {
            IconSampleManager.RightButtomCornorLabel.text = "x" + BagItem.cnt;
        }

        UpdataSelNum();
    }


    private void AddCountContinue(GameObject go)
    {
        isAdding = true;
        StartCoroutine(DoAddCountContinue());
    }

    private void StopAddCountContinue(GameObject go)
    {
        //Coroutine stop automatically.
        isAdding = false;
    }

    private const float AddGapTime = 0.2f;

    private IEnumerator DoAddCountContinue()
    {
        while (isAdding)
        {
            yield return new WaitForSeconds(AddGapTime);
            yield return new WaitForEndOfFrame();
            AddCount(gameObject);
        }
    }

    private void AddCount(GameObject go)
    {
        if (sellNum == BagItem.cnt)
        {
            return;
        }

        sellNum = sellNum > BagItem.cnt ? BagItem.cnt : sellNum + 1;

        IconSampleManager.SubButton.SetActive(true);
        IconSampleManager.RightButtomCornorLabel.text = sellNum + "/" + BagItem.cnt;

        UpdataSelNum();
    }

    private void UpdataSelNum()
    {
        PawnshopLayerSellControllor.UpdateItemList();
    }
}
