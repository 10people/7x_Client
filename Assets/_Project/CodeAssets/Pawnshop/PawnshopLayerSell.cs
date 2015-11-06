using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qxmobile.protobuf;

public class PawnshopLayerSell : MonoBehaviour
{
    public PawnshopUIControllor controllor;

    public UILabel label_1;//请选择要典当的物品

    public UILabel label_2;//本次典当可获得100元宝

    public UILabel label_3;//999元宝

	public UILabel label_4;//没有任何物品时候的典当

	public UILabel label_5;//没有任何物品时候的典当

    public GameObject sellable;//有物品可以典当

    public GameObject sellunable;//一无所有


    [HideInInspector] public List<long> sellBagId = new List<long>();

    [HideInInspector] public List<int> sellNum = new List<int>();


	private static GameObject m_iconSamplePrefab;

    private List<BagItem> tempList = new List<BagItem>();

    private List<PawnshopBagItem> m_pawnshopBagItemList = new List<PawnshopBagItem>();

    private readonly Vector3 iconSamplePos = new Vector3(0, -200, 0);

	private const float width = 110f;

    private float left
    {
        get { return -54.0f; }
    }


    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_iconSamplePrefab == null)
        {
            m_iconSamplePrefab = p_object as GameObject;
        }
//		count = tempList.Count;
        for (int i = 0; i < tempList.Count; i++)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;

            iconSampleObject.transform.parent = transform;

			iconSampleObject.transform.localPosition = iconSamplePos + new Vector3(left + width * i, 0, 0);

            IconSampleManager iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();

			PawnshopBagItem pawnshopBagItem = iconSampleObject.GetComponent<PawnshopBagItem>() ?? iconSampleObject.AddComponent<PawnshopBagItem>();

            pawnshopBagItem.IconSampleManager = iconSampleManager;

			pawnshopBagItem.PawnshopLayerSellControllor = this;

			pawnshopBagItem.RefreshData(tempList[i]);

            m_pawnshopBagItemList.Add(pawnshopBagItem);
        }
		m_isCodeEnd = true;
    }
	public int count = 0;
	public bool m_isCodeEnd = false;

	private int num = 0;
    public void refreshData()
    {
        //Clear sell list objects.
        foreach (PawnshopBagItem item in m_pawnshopBagItemList)
        {
            item.IconSampleManager.transform.parent = null;

            Destroy(item.IconSampleManager.gameObject);
        }

        m_pawnshopBagItemList.Clear();
        tempList.Clear();
        sellBagId.Clear();
        sellNum.Clear();
        foreach (BagItem bi in BagData.Instance().m_bagItemList)
        {
            if (bi.itemType == 21)//玉玦
            {
                tempList.Add(bi);
            }
        }
		count = tempList.Count;
        if (tempList.Count == 0)
        {
            sellable.SetActive(false);

            sellunable.SetActive(true);

			label_4.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TRANS_83);

			label_5.text = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_TRANS_93);
            return;
        }
        sellable.SetActive(true);

        sellunable.SetActive(false);

        label_1.gameObject.SetActive(true);

        label_2.gameObject.SetActive(false);

        label_3.gameObject.SetActive(false);
		if (m_iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleLoadCallBack(ref temp, null, m_iconSamplePrefab);
		}
		//        if (m_iconSamplePrefab == null)
		//        {
		//            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
//        }
//        else
//        {
//            WWW temp = null;
//            OnIconSampleLoadCallBack(ref temp, null, m_iconSamplePrefab);
//        }
    }

    public void UpdateItemList()
    {
        sellBagId.Clear();

        sellNum.Clear();

        label_1.gameObject.SetActive(false);

        label_2.gameObject.SetActive(false);

        label_3.gameObject.SetActive(false);

        int num = 0;

        foreach (PawnshopBagItem item in m_pawnshopBagItemList)
        {
            if (item.sellNum == 0) continue;

            sellBagId.Add(item.BagItem.dbId);

            sellNum.Add(item.sellNum);

            label_1.gameObject.SetActive(false);

            label_2.gameObject.SetActive(true);

            label_3.gameObject.SetActive(false);

            ItemTemp template = ItemTemp.getItemTempById(item.BagItem.itemId);

            num += template.sellNum * item.sellNum;
        }

        label_2.text = "" + num;

        label_3.text = "" + num;
    }

    public void OnSell()
    {
        foreach (PawnshopBagItem item in m_pawnshopBagItemList)
        {
            item.BagItem.cnt -= item.sellNum;

            item.RefreshData(item.BagItem);

			item.IconSampleManager.SubButton.SetActive(false);
        }

        if(controllor.sendSellItem())
		{
			StartCoroutine(sellAction());
		}

//		refreshData();
    }

    IEnumerator sellAction()
    {
        label_1.gameObject.SetActive(false);

        label_2.gameObject.SetActive(false);

        label_3.gameObject.SetActive(true);

        bool f = true;

		label_3.transform.localPosition = new Vector3(label_3.transform.localPosition.x, -62, label_3.transform.localPosition.z);

        for (; f == true; )
        {
            yield return new WaitForEndOfFrame();

            label_3.transform.localPosition += new Vector3(0, 2f, 0);

            if (label_3.transform.localPosition.y > 0) f = false;
        }

        label_1.gameObject.SetActive(true);

        label_2.gameObject.SetActive(false);

        label_3.gameObject.SetActive(false);
    }

}
