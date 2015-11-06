using UnityEngine;
using System.Collections;
using qxmobile.protobuf;

public class PawnshopBuyItem : MonoBehaviour
{
    public PawnshopUIControllor controllor;

    public UILabel labelCost;

    public UISprite spriteRMB;

    public UISprite spriteDKP;

	public UISprite spriteCoin;

    public GameObject sellout;

    public GoodsInfo good;
	

	private static GameObject m_iconSamplePrefab;

	private IconSampleManager m_iconSampleManager;

    private DangPuTemplate dangPuTemplate;

	private DangpuItemCommonTemplate dangPuCommonTemplate;
    
	private bool isSellout;

	private int from;


	public void RefreshData(GoodsInfo _good, DangpuItemCommonTemplate _template, int _from)
	{
		good = _good;
		
		from = _from;
		
		//template = DangPuTemplate.getDangpuTemplateById(good.itemId);
		
		dangPuCommonTemplate = _template;
		
		isSellout = good.isSoldOut;
		
		labelCost.text = "" + dangPuCommonTemplate.needNum;
		
		spriteCoin.gameObject.SetActive (dangPuCommonTemplate.needType == 0);
		
		spriteRMB.gameObject.SetActive(dangPuCommonTemplate.needType == 1);
		
		spriteDKP.gameObject.SetActive(dangPuCommonTemplate.needType == 2);
		
		sellout.SetActive(isSellout);
		
		if (m_iconSamplePrefab == null)
		{
			//			Debug.Log("=====================================1");
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
		}
		else
		{
			WWW temp = null;
			//			Debug.Log("=====================================2");
			OnIconSampleLoadCallBack(ref temp, null, m_iconSamplePrefab);
		}
	}

	public void RefreshData(GoodsInfo _good, DangPuTemplate _template, int _from)
    {
        good = _good;

		from = _from;

        //template = DangPuTemplate.getDangpuTemplateById(good.itemId);

		dangPuTemplate = _template;

        isSellout = good.isSoldOut;

		labelCost.text = "" + dangPuTemplate.needNum;

		spriteCoin.gameObject.SetActive (dangPuTemplate.needType == 0);
		
		spriteRMB.gameObject.SetActive(dangPuTemplate.needType == 1);

		spriteDKP.gameObject.SetActive(dangPuTemplate.needType == 2);

        sellout.SetActive(isSellout);

        if (m_iconSamplePrefab == null)
        {
//			Debug.Log("=====================================1");
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleLoadCallBack);
        }
        else
        {
            WWW temp = null;
//			Debug.Log("=====================================2");
            OnIconSampleLoadCallBack(ref temp, null, m_iconSamplePrefab);
        }
    }

    private readonly Vector3 iconSamplePos = new Vector3(-2, 18, 0);

    private void OnIconSampleLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        if (m_iconSamplePrefab == null)
        {
            m_iconSamplePrefab = p_object as GameObject;
        }

        if (m_iconSampleManager == null || m_iconSampleManager.gameObject == null)
        {
            GameObject iconSampleObject = Instantiate(p_object) as GameObject;
            iconSampleObject.transform.parent = transform;
            m_iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
        }

		int itemId = dangPuTemplate == null ? dangPuCommonTemplate.itemId : dangPuTemplate.itemId;
		int itemNum = dangPuTemplate == null ? dangPuCommonTemplate.itemNum : dangPuTemplate.itemNum;

		m_iconSampleManager.SetIconByID(itemId, "x" + itemNum,5);
        m_iconSampleManager.transform.localPosition = iconSamplePos;
        m_iconSampleManager.SetIconBasicDelegate(false,true,OnPawnBagItemClick);
		m_iconSampleManager.SetIconPopText (itemId);
    }

    public void OnPawnBagItemClick(GameObject go)
    {
        OnClick();
    }

    void OnClick()
    {
        if (isSellout == true)
        {
            return;
        }
    
		if (FreshGuide.Instance().IsActive(100290) && TaskData.Instance.m_TaskInfoDic[100290].progress >= 0)
		{
			CityGlobalData.m_isRightGuide = false;
			
			TaskData.Instance.m_iCurMissionIndex = 100290;
			
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
			
			tempTaskData.m_iCurIndex = 5;
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
		}

		controllor.showbuyHint(good, from);
    }

}
