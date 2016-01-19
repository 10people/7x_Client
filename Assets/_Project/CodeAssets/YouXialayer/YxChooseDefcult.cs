using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class YxChooseDefcult : MonoBehaviour {
	[HideInInspector]
	
	public int BigId;
	public UILabel mTiLi;
	
	public UILabel mTongBi;
	
	public UILabel mYuanBao;

	public GameObject Item;

	public UILabel YxName;
	public UIGrid mGid;
	public YouXiaInfo mYouXia_Info;
	public List <YouXiaItem> YouXiaItemmList = new List<YouXiaItem>();
	public static YxChooseDefcult mmmYxChooseDefcult;
	public UIScrollView mScorview;
	void Awake()
	{
		mmmYxChooseDefcult = this;
	}
	void Start () {
	
	}
	

	void Update () {
	
		mTiLi.text = JunZhuData.Instance ().m_junzhuInfo.tili.ToString();
		
		mTongBi.text = JunZhuData.Instance ().m_junzhuInfo.jinBi.ToString();
		
		mYuanBao.text = JunZhuData.Instance ().m_junzhuInfo.yuanBao.ToString();
	}
	public void Init()
	{
		if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
		{
			Debug.Log("进入试练二阶界面222");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			mScorview.enabled = false;
		}
		List<YouxiaPveTemplate> mYouxiaPveTemplateList = YouxiaPveTemplate.getYouXiaPveTemplateListBy_BigId (BigId);
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (BigId);
		YxName.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);
		foreach(YouXiaItem m_YXItem in YouXiaItemmList)
		{
			Destroy(m_YXItem.gameObject);
		}
		YouXiaItemmList.Clear ();

		for(int i = 0 ; i < mYouxiaPveTemplateList.Count; i++)
		{
			GameObject m_UI = Instantiate(Item) as GameObject;
			
			m_UI.SetActive (true);
			
			m_UI.transform.parent = Item.transform.parent;
			
			m_UI.transform.localScale = Vector3.one;
			
			//m_UI.transform.localPosition = new Vector3(-300+Pos_Dis*i,0,0);
			
			YouXiaItem mYXItem = m_UI.GetComponent<YouXiaItem>();
			
			mYXItem.L_id = mYouxiaPveTemplateList[i].id;
			
			mYXItem.YouXiadifficulty = i+1;
			
			//mYouXiaItem.CountTime = HavatTimes;
			mYXItem.mYou_XiaInfo = mYouXia_Info;
			mYXItem.bigid = mYouxiaPveTemplateList[i].bigId;
			mYXItem.Init();
			YouXiaItemmList.Add(mYXItem);
		}
		mGid.repositionNow = true;
	}
	public void BuyTiLi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
	}
	public void BuyTongBi()
	{
		JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
	}
	public void BuyYuanBao()
	{
		MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
    }

	public void Help()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.SHILIANINSTRCDUTION));
	}
	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
