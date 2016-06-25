using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class QianChongLouManagerMent : MonoBehaviour,SocketProcessor {

	public UISprite VipN;

	public UILabel EXpAdd;

	public GameObject m_PlayerModel;

	public GameObject m_PlayerParent;

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	List<GameObject> AwardList = new List<GameObject>(); 

	public UILabel HistoryNumber;

	public GameObject AwardRoot;

	public GameObject Awardtemp;

	public UILabel LayerName;

	public MainInfoResp mMainInfo;

	public UILabel RecZhanLi;

	public UILabel mZhanLi;

	public GameObject OnStrongBtn;

	[HideInInspector]public ChongLouSaoDangResp mQCL_S_DInfo;

	public static QianChongLouManagerMent mQianChongLouManagerMent;

	private bool YindaoIsopen;

	private int mJunZhuZhanli;

	public GameObject mSaoDangBtn;

	public UISprite mSaodangSprite;

	public static QianChongLouManagerMent Instance()
	{
		if (!mQianChongLouManagerMent)
		{
			mQianChongLouManagerMent = (QianChongLouManagerMent)GameObject.FindObjectOfType (typeof(QianChongLouManagerMent));
		}
		
		return mQianChongLouManagerMent;
	}
	
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
		BtnList.ForEach (item => SetBtnMoth(item));
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, YinDaoManager );
		}
	}
	void OnDestroy()
	{
		mQianChongLouManagerMent = null;
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	void Start () {
	
		initStart ();
	}
	void Update()
	{
		if(JunZhuData.Instance().m_junzhuInfo.zhanLi > mJunZhuZhanli && mMainInfo != null)
		{
			mJunZhuZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
			ShowmZhanLiInfo();
		}
	}
	void initStart()
	{
		CanSaoDang = true;

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.CHONG_LOU_INFO_REQ,ProtoIndexes.CHONG_LOU_INFO_RESP.ToString()); // 重楼主界面请求,
		YinDaoManager ();
	}
	void YinDaoManager()
	{
		YindaoIsopen = false;
		if(FreshGuide.Instance().IsActive(200020)&& TaskData.Instance.m_TaskInfoDic[200020].progress >= 0)
		{
			Debug.Log("千重楼首页引导");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			StartCoroutine("SetBtnEnble");
		}else
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	IEnumerator SetBtnEnble()
	{
		yield return new WaitForSeconds (0.5f);
		YindaoIsopen = false;
	}

	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.CHONG_LOU_INFO_RESP: //千重楼返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MainInfoResp mMainInfoResp = new MainInfoResp();
				
				t_qx.Deserialize(t_stream, mMainInfoResp, mMainInfoResp.GetType());

				mMainInfo = mMainInfoResp;

				InitData();

				return true;
			}
			case ProtoIndexes.CHONG_LOU_SAO_DANG_RESP: //千重楼扫荡返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ChongLouSaoDangResp mChongLouSaoDangResp = new ChongLouSaoDangResp();
				
				t_qx.Deserialize(t_stream, mChongLouSaoDangResp, mChongLouSaoDangResp.GetType());

				Debug.Log ("扫荡返回了 result = "+mChongLouSaoDangResp.result);
				CanSaoDang = true;
				if(mChongLouSaoDangResp.result == 0)
				{
					initStart();
					
					mQCL_S_DInfo = mChongLouSaoDangResp;
					
					SangDangDataBack(mChongLouSaoDangResp);
				}
				else if(mChongLouSaoDangResp.result == 1)
				{
					string data = "[c40000]今日扫荡次数已用完！[-]";
					ClientMain.m_UITextManager.createText( data);
				}
				else if(mChongLouSaoDangResp.result == 2)
				{
					string data = "[c40000]今日已无可扫荡关卡！[-]";
					ClientMain.m_UITextManager.createText( data);
				}
				else if(mChongLouSaoDangResp.result == 3)
				{
					string data = "[c40000]通过后可扫荡！[-]";
					ClientMain.m_UITextManager.createText( data);
				}
				else{
					Debug.Log ("扫荡失败了");
				}
				return true;
			}
			default: return false;
			}
			
		}
		else
		{
		    Debug.Log ("p_message == null");
		}
//		
		return false;
	}

	public void BtnManagerMent(GameObject mbutton)
	{
		if(YindaoIsopen)
		{
			return;
		}
		switch(mbutton.name)
		{
		case "Button_0":
			Close();
			break;
		case "Button_1":
			SaoDangBtn();
			break;
		case "Button_2":
			ShowAward();
			break;
		case "Button_3":
			BattleBtn();
			break;
		case "Button_4":
			ToOnStronger();
			break;
		case "Button_5":
			HelpBtn();
			break;
		default:
			break;
		}
	}
	void HelpBtn()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.QIANCHONGLOURoles));
	}
	void ToOnStronger()
	{
		MainCityUILT.ShowMainTipWindow();
	}
	public GameObject EnterBtn;
	public UISprite m_EnterBtnSprite;
	public void InitData()
	{
		int viplevel = 5;
		VipN.spriteName = "v" + viplevel.ToString ();
		VipTemplate mvtemp = VipTemplate.GetVipInfoByLevel (viplevel);

		EXpAdd.text = "(额外获得"+((mvtemp.ExpAdd - 1)*100 ).ToString()+"%经验加成)";

		Debug.Log("mMainInfo.currentLv = "+mMainInfo.currentLv);
		InitAward ();
		if(mMainInfo == null)
		{
			Debug.Log("mMainInfo == null");
			return;
		}
		LayerName.text = "第"+mMainInfo.currentLv.ToString()+"层";
		HistoryNumber.text = mMainInfo.historyHighestLv.ToString();
		LoadPlayer ();

		if (mMainInfo.currentLv > 70) {
			EnterBtn.GetComponent<UIButton>().enabled = false;
			m_EnterBtnSprite.color = Color.gray;
		}
		if(mMainInfo.currentLv > mMainInfo.historyHighestLv)
		{
			mSaoDangBtn.GetComponent<UIButton>().enabled = false;
			mSaodangSprite.color = Color.gray;
		}
	}
	void LoadPlayer()
	{
		if(m_PlayerModel)
		{
			Destroy(m_PlayerModel);
		}

		ChonglouPveTemplate mChonglouPve = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (mMainInfo.currentLv);
		int modelid = ChongLouNpcTemplate.Get_QCL_NpcModel_By_npcid (mChonglouPve.npcId);

		Global.ResourcesDotLoad(ModelTemplate.GetResPathByModelId(modelid),
		                        LoadCallback );
		ShowmZhanLiInfo ();
	}
	public void ShowmZhanLiInfo()
	{
		ChonglouPveTemplate mChonglouPve = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (mMainInfo.currentLv);
		
		RecZhanLi.text = mChonglouPve.recZhanli.ToString ();
		
		if(JunZhuData.Instance().m_junzhuInfo.zhanLi < mChonglouPve.recZhanli )
		{
			OnStrongBtn.SetActive(true);
			mZhanLi.text = MyColorData.getColorString(5, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
		else
		{
			OnStrongBtn.SetActive(false);
			mZhanLi.text = MyColorData.getColorString(4, JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString ());
		}
	}
	public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		m_PlayerModel = Instantiate(p_object) as GameObject;
		
		m_PlayerModel.SetActive( true );
		
		m_PlayerModel.transform.parent = m_PlayerParent.transform;
		m_PlayerModel.name = p_object.name;

		GameObjectHelper.SetGameObjectLayerRecursive( m_PlayerModel, m_PlayerModel.transform.parent.gameObject.layer );
		
		m_PlayerModel.transform.localPosition = new Vector3(0, 0, 0);
		m_PlayerModel.GetComponent<NavMeshAgent>().enabled = false;

		BaseAI mBaseAI = m_PlayerModel.GetComponent<BaseAI>();
		if(mBaseAI != null )
		{
			Destroy(mBaseAI);
			mBaseAI.enabled = false;
		}
		m_PlayerModel.AddComponent <DramaStorySimulation>();	
		//m_PlayerModel.GetComponent<Animator>().Play("zhuchengidle");
		
		m_PlayerModel.transform.localScale = new Vector3(140,140,140);

		m_PlayerModel.transform.Rotate (0, 200, 0);// = new Vector3 (0,40,0);

//		m_PlayerModel.transform.localRotation = new Quaternion(0,20,0,0);
//		m_PlayerModel.transform.rotation = new Quaternion(0,163,0,0);
		
		//		m_PlayerModel.GetComponent<PlayerWeaponManagerment>().ShowWeapon(3);
	}
	List<int> First_t_items = new List<int>();
	List<int> t_items = new List<int>();
	[HideInInspector]
	public GameObject IconSamplePrefab;

	float m_Dis = 45;

	void InitAward()
	{
		foreach(GameObject maward in m_AwardIcon)
		{
			Destroy(maward);
		}
		m_AwardIcon.Clear ();

		First_t_items.Clear ();

		t_items.Clear ();

		foreach(GameObject mgame in AwardList)
		{
			Destroy(mgame);
		}
		AwardList.Clear();

		ChonglouPveTemplate mCL = ChonglouPveTemplate.Get_QCL_PVETemplate_By_Layer (mMainInfo.currentLv);

		string m_Award = mCL.awardId;

		char[] t_items_delimiter = { ',' };
		
		char[] t_item_id_delimiter = { '=' };
		
		string[] t_item_strings = m_Award.Split(t_items_delimiter);
		
		for (int i = 0; i < t_item_strings.Length; i++)
		{
			string t_item = t_item_strings[i];
			
			string[] t_finals = t_item.Split(t_item_id_delimiter);
			
			if(t_finals[0] != "" && !t_items.Contains(int.Parse(t_finals[0])))
			{
				t_items.Add(int.Parse(t_finals[0]));
			}
		}

		if(mMainInfo.currentLv>mMainInfo.historyHighestLv)
		{
			string[] First_item_strings = mCL.firstAwardID.Split(t_items_delimiter);
			
			for (int i = 0; i < First_item_strings.Length; i++)
			{
				string t_item = First_item_strings[i];
				
				string[] t_finals = t_item.Split(t_item_id_delimiter);
				
				if(t_finals[0] != "" && !First_t_items.Contains(int.Parse(t_finals[0])))
				{
					First_t_items.Add(int.Parse(t_finals[0]));
				}
			}
		}

		numPara = t_items.Count + First_t_items.Count;
//		Debug.Log ("numPara = "+numPara);
//		for(int i = 0; i < AwardNumber; i++)
//		{
//			GameObject mAward = Instantiate(Awardtemp)as GameObject; 
//			mAward.SetActive(true);
//			mAward.transform.parent = AwardRoot.transform;
//			mAward.transform.localScale = Vector3.one;
//			mAward.transform.localPosition = new Vector3 (i*m_X ,0,0);
//			AwardList.Add(mAward);
//		}
		if (IconSamplePrefab == null)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE), OnIconSampleCallBack);
		}
		else
		{
			WWW temp = null;
			OnIconSampleCallBack(ref temp, null, IconSamplePrefab);
		}

	}
	List <GameObject> m_AwardIcon = new List<GameObject> (); 

	int numPara;
	private void OnIconSampleCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		if (IconSamplePrefab == null)
		{
			IconSamplePrefab = p_object as GameObject;
		}
		List<AwardTemp> mAwardTemp = new List<AwardTemp> ();
		for (int n = 0; n < numPara; n++)
		{
			if(n < First_t_items.Count)
			{
				mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(First_t_items[n]);
			}
			else
			{
				mAwardTemp = AwardTemp.getAwardTempList_By_AwardId(t_items[n - First_t_items.Count]);
			}

			for (int i = 0; i < mAwardTemp.Count; i++)
			{
				if(mAwardTemp[i].weight != 0)
				{
					GameObject iconSampleObject = Instantiate(IconSamplePrefab) as GameObject;
					
					m_AwardIcon.Add(iconSampleObject);
					
					iconSampleObject.SetActive(true);
					
					iconSampleObject.transform.parent = AwardRoot.transform;
					
					iconSampleObject.transform.localPosition = new Vector3((n) * m_Dis, 0, 0);
					
					//FirstAwardPos = iconSampleObject.transform.localPosition;
					
					var iconSampleManager = iconSampleObject.GetComponent<IconSampleManager>();
					
					var iconSpriteName = "";
					
					CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(mAwardTemp[i].itemId);
					
					iconSpriteName = mItemTemp.icon.ToString();
					
					iconSampleManager.SetIconType(IconSampleManager.IconType.item);
					
					NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mAwardTemp[i].itemId);
					
					string mdesc = DescIdTemplate.GetDescriptionById(mAwardTemp[i].itemId);
					
					var popTitle = mNameIdTemplate.Name;
					
					var popDesc = mdesc;
					
					iconSampleManager.SetIconByID(mItemTemp.id, mAwardTemp[i].itemNum.ToString(), 3);
					iconSampleManager.SetIconPopText(mAwardTemp[i].itemId, popTitle, popDesc, 1);
					iconSampleObject.transform.localScale = new Vector3(0.4f,0.4f,1);
					if(n < First_t_items.Count)
					iconSampleManager.FirstWinSpr.gameObject.SetActive(true);
				}
				
			}
		}
		
	}
	GameObject EnterBattelUI;
	void BattleBtn() // 挑战
	{
		if(mMainInfo.currentLv > 70)
		{
			ClientMain.m_UITextManager.createText("此版本只可挑战到70层，敬请期待正式版！");
			return;
		}
		if(EnterBattelUI)
		{
			return;
		}
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.QCLLAYERINFO),LoadEnterUICallback);
	}
	void LoadEnterUICallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		EnterBattelUI = Instantiate(p_object )as GameObject;
		
		EnterBattelUI.transform.localScale = Vector3.one;
		
		EnterBattelUI.transform.localPosition = new Vector3 (-100,100,0);
		
		QCLLayerInfo mQCLLayerInfo = EnterBattelUI.GetComponent<QCLLayerInfo>();

		mQCLLayerInfo.First_Awars = First_t_items;

		mQCLLayerInfo.Awards = t_items;

		mQCLLayerInfo.Init (mMainInfo.zuheSkill,mMainInfo.currentLv,YinDaoManager);
	}
	bool CanSaoDang = true;
	void SaoDangBtn() // 扫荡
	{	 
		if(mMainInfo.historyHighestLv < 1)
		{
			string data = "[c40000]通过一次[-]后才能扫荡！";
			ClientMain.m_UITextManager.createText( data);
			return;
		}
		if(CanSaoDang)
		{
			CanSaoDang = false;
			Debug.Log("发送扫荡请求！");
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.CHONG_LOU_SAO_DANG_REQ); // 重楼扫荡请求,
		}

	}
	void SangDangDataBack(ChongLouSaoDangResp mSangdang)
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.QCLLAYERSAODANG),LoadResourceCallback);
	}
	void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		GameObject tempOjbect = Instantiate(p_object )as GameObject;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = new Vector3 (100,100,0);

		QCLLayerSaoDang mQCLLayerSaoDang = tempOjbect.GetComponent<QCLLayerSaoDang>();

		mQCLLayerSaoDang.mQCL_saodangInfo = mQCL_S_DInfo;

		mQCLLayerSaoDang.Init (mMainInfo.historyHighestLv,mMainInfo.currentLv);
	}
	GameObject AwardsScrollview ;
	void ShowAward()// 商品预览
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.QCLPREVIEW),LoadAwardsResourceCallback);
	}

	void LoadAwardsResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		if(AwardsScrollview)
		{
			return;
		}
		AwardsScrollview = Instantiate(p_object )as GameObject;
		
		AwardsScrollview.transform.localScale = Vector3.one;
		
		AwardsScrollview.transform.localPosition = new Vector3 (100,-100,0);
		
		QCLPreViewManager mQCLPreViewManager = AwardsScrollview.GetComponent<QCLPreViewManager>();
		
		mQCLPreViewManager.Init ();
	}
	void Close() //关闭
	{
		CityGlobalData.QCLISOPen = false;
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
