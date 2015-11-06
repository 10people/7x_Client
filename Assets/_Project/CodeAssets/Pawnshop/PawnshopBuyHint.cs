using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class PawnshopBuyHint : MonoBehaviour
{
    public PawnshopUIControllor controllor;

    public UILabel labelNum;

    public UILabel labelCost;

    public UILabel labelName;

    public UILabel labelDesc;

    public GameObject spriteRMB;

    public GameObject spriteDKP;

	public GameObject spriteCoin;


    [HideInInspector] public GoodsInfo item;

	[HideInInspector] public int from;


	private static GameObject m_iconSamplePrefab;

	private IconSampleManager m_iconSampleManager;

    private readonly Vector3 iconSamplePos = new Vector3(-191.0f, 62.7f, 0);

	private int itemType = 0;
	
	private int itemId = 0;
	
	private int itemNum = 0;
	
	private int needNum = 0;
	
	private int needType = 0;


    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_iconSamplePrefab == null)
        {
            m_iconSamplePrefab = p_object as GameObject;
        }

		if(m_iconSampleManager != null)
		{
			DestroyObject (m_iconSampleManager.gameObject);
			
			m_iconSampleManager = null;
		}

        GameObject iconSampleObject = Instantiate(m_iconSamplePrefab) as GameObject;
		iconSampleObject.transform.parent = transform;
		m_iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
		
		m_iconSampleManager.SetIconByID(itemId, "x" + itemNum,211);
        m_iconSampleManager.transform.localPosition = iconSamplePos;
        m_iconSampleManager.SetIconPopText (itemId);
    }

    public void RefreshData(GoodsInfo _item, int _from)
    {
        item = _item;

		from = _from;

		if(from == 1)
		{
			DangPuTemplate dangPuTemplate = DangPuTemplate.getDangpuTemplateById(item.itemId);

			itemType = dangPuTemplate.itemType;

			itemId = dangPuTemplate.itemId;

			itemNum = dangPuTemplate.itemNum;

			needNum = dangPuTemplate.needNum;

			needType = dangPuTemplate.needType;
		}
		else
		{
			DangpuItemCommonTemplate dangpuCommonTemplate = DangpuItemCommonTemplate.getDangpuItemCommonById(item.itemId);

			itemType = dangpuCommonTemplate.itemType;

			itemId = dangpuCommonTemplate.itemId;

			itemNum = dangpuCommonTemplate.itemNum;
			
			needNum = dangpuCommonTemplate.needNum;
			
			needType = dangpuCommonTemplate.needType;
		}

		CommonItemTemplate template = CommonItemTemplate.getCommonItemTemplateById (itemId);

        labelNum.text = itemNum + "";

        labelCost.text = needNum + "";

		labelName.text = NameIdTemplate.getNameIdTemplateByNameId(template.nameId).Name;

        labelDesc.text = DescIdTemplate.getDescIdTemplateByNameId(template.descId).description;

		spriteCoin.SetActive (needType == 0);

        spriteRMB.SetActive(needType == 1);

        spriteDKP.SetActive(needType == 2);

		if (m_iconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
		}
		else
		{
			WWW temp = null;
			
			OnIconSampleLoadCallBack(ref temp, null, m_iconSamplePrefab);
		}
    }

    public void Buy()
    {
//		if (FreshGuide.Instance().IsActive(100120) && TaskData.Instance.m_TaskInfoDic[100120].progress >= 0)
//		{
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
//
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
//		}

		if (controllor.buyingItem != null) return;

		if(needType == 0)//coin
		{
			if(needNum > JunZhuData.Instance().m_junzhuInfo.jinBi)
			{
				JunZhuData.Instance ().BuyTiliAndTongBi (false , true,false );
//				controllor.showCoinErrorHint();

				return;
			}
		}
		else if(needType == 1)//rmb
		{
			if(needNum > JunZhuData.Instance().m_junzhuInfo.yuanBao)
			{
				controllor.showRmbErrorHint();

				return;
			}
		}
		else if(needType == 2)//dkp
		{
			if(AllianceData.Instance.g_UnionInfo == null || needNum > AllianceData.Instance.g_UnionInfo.contribution)
			{
				controllor.showDkpErrorHint();

				return;
			}
		}

		controllor.sendBuyItem(item, from);

        CloseLayer();
    }

    public void CloseLayer()
    {
		DestroyObject (m_iconSampleManager.gameObject);

        gameObject.SetActive(false);
    }

}
