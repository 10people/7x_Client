using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class NewFuWenPage : MonoBehaviour ,SocketListener {

	public GameObject TopLeftManualAnchor;
	public GameObject TopRightManualAnchor;

	public List<GameObject> ActiveBtns;

	[HideInInspector]public bool RongHeUIisOpen;

	public UIScrollView mScrollView;

	public UIScrollBar mUIScrollBar;

	public UILabel JiaPianNumber;

	public GameObject FirstPage;

	public GameObject SecondPage;

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public QueryFuwenResp mQueryFuwen;

	public FuwenInBagResp mFuwenInBag;

	public UILabel FuwenPage;

	public UILabel mOneKeyName;

	public GameObject mOneKeyNamObg;

	public static NewFuWenPage mNewFuWenPage;

	public List<FuWenLanWei> mFuWenLanWeiList = new List<FuWenLanWei>(); 

	public UILabel[] PropertyTemp;

	public UILabel  page1;

	public UILabel  page2;

	public UILabel  page3;

	public GameObject[] Locks;
	public GameObject OneKeyArelt;

	public bool SecondLieFu = false;

	public static NewFuWenPage Instance()
	{
		if (!mNewFuWenPage)
		{
			mNewFuWenPage = (NewFuWenPage)GameObject.FindObjectOfType (typeof(NewFuWenPage));
		}
		
		return mNewFuWenPage;
	}
	void Awake()
	{
		MainCityUI.setGlobalTitle(TopLeftManualAnchor, "符文", 0, 0);
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, YinDaoManager );
		}
		SocketTool.RegisterSocketListener(this);
		AddEventListener ();
		
	}
	public void AddEventListener()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		mNewFuWenPage = null;
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		Debug.Log ("mbutton.name = "+mbutton.name);
		switch(mbutton.name)
		{
		case "OnKeyIn":
		    OnKeyIn();
			break;
		case "OneKeyOff":
			OneKeyOff();
			break;
		case "OneKeyON":
			OneKeyOn();
			break;
		case "GoLieFu":
			GoTOLieFu();
			break;
		case "Button_2":
			ChangeJiaPian();
			break;
		case "Sprite (0)":
			XiangQian(0);
			break;
		case "Sprite (1)":
			XiangQian(1);
			break;
		case "Sprite (2)":
			XiangQian(2);
			break;
		case "Sprite (3)":
			XiangQian(3);
			break;
		case "Sprite (4)":
			XiangQian(4);
			break;
		case "Sprite (5)":
			XiangQian(5);
			break;
		case "Sprite (6)":
			XiangQian(6);
			break;
		case "Sprite (7)":
			XiangQian(7);
			break;

		case "FuYin_1":
			OPenFuYin(1);
			break;

		case "FuYin_2":
			OPenFuYin(2);
			break;

		case "FuYin_3":
			OPenFuYin(3);
			break;

		case "BackBtn":
			BackBtn();
			break;
		case "HelpBtn":
			HelpBtn();
			break;
//		case "MixBtn":
//			RongHe();
//			break;

		case "Button_0":
			Close();
			break;
		default:
			break;
		}
	}
	void Start () {
	
		//BackToFirst ();
		MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
//		Init ();
//		InitData();
		OnekeyXiangqiang = false;
		RongHeUIisOpen = false;

//		GetBagInfo ();
//		TopRightManualAnchor.transform.localPosition = new Vector3(480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY, 0);
//		TopLeftManualAnchor.transform.localPosition = new Vector3(-480 - ClientMain.m_iMoveX, 320 + ClientMain.m_iMoveY, 0);
		YinDaoManager ();
	}
	private bool YindaoIsopen;
	void YinDaoManager()
	{
		YindaoIsopen = false;
		if(FreshGuide.Instance().IsActive(200060)&& TaskData.Instance.m_TaskInfoDic[200060].progress >= 0)
		{
			Debug.Log("第一次引导去猎符");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200060];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			StartCoroutine("SetBtnEnble");
		}
		else if(FreshGuide.Instance().IsActive(200065)&& TaskData.Instance.m_TaskInfoDic[200065].progress >= 0)
		{
			Debug.Log("第二次引导去猎符");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200065];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			StartCoroutine("SetBtnEnble");
		}
		else if(FreshGuide.Instance().IsActive(100470)&& TaskData.Instance.m_TaskInfoDic[100470].progress >= 0)
		{
			Debug.Log("引导装备猎符空");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100470];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
			StartCoroutine("SetBtnEnble");

		}
		else
		{
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	IEnumerator SetBtnEnble()
	{
		yield return new WaitForSeconds (0.5f);
		YindaoIsopen = false;
	}
	int GetHuFuNum ()
	{
		int jiapian = 0;
		for (int i = 0;i < BagData.Instance().m_bagItemList.Count;i ++)
		{
			if (BagData.Instance().m_bagItemList[i].itemId == 910003 && BagData.Instance().m_bagItemList[i].cnt > 0)
			{
				jiapian += BagData.Instance().m_bagItemList[i].cnt;
			}
		}
		return jiapian;
	}
   public void BackToFirst()
	{
		OnekeyXiangqiang = false;
		RongHeUIisOpen = false;
		FuWenInfoShow.Instance().fuwensinBag.Clear ();
		FirstPage.SetActive (true);
		SecondPage.SetActive (false);
	}
	// Update is called once per frame
	void Update () {
	
		if(RongHeUIisOpen)
		{
			mOneKeyNamObg.name = "OnKeyIn";
			mOneKeyName.text = "一键放入";
		}
		else{
			mOneKeyNamObg.name = "OneKeyON";
			mOneKeyName.text = "一键穿戴";
		}
	}
	void SortPropertyList()
	{
		for(int i = 0 ; i < mQueryFuwen.attr.Count; i ++)
		{
			if(mQueryFuwen.attr[i].type == 2)
			{
				PropertyTemp[0].text = getmString(1)+mQueryFuwen.attr[i].wqSH.ToString();
				PropertyTemp[1].text = getmString(2)+mQueryFuwen.attr[i].wqJM.ToString();
				PropertyTemp[2].text = getmString(3)+mQueryFuwen.attr[i].wqBJ.ToString();
				PropertyTemp[3].text = getmString(4)+mQueryFuwen.attr[i].wqRX.ToString();
				PropertyTemp[4].text = getmString(5)+mQueryFuwen.attr[i].jnSH.ToString();
				PropertyTemp[5].text = getmString(6)+mQueryFuwen.attr[i].jnJM.ToString();
				PropertyTemp[6].text = getmString(7)+mQueryFuwen.attr[i].jnBJ.ToString();
				PropertyTemp[7].text = getmString(8)+mQueryFuwen.attr[i].jnRX.ToString();

			}
		}

	}
	string getmString(int index)
	{
		string mstr = "";
		switch(index)
		{
		case 1:
			mstr = "武器伤害加深+";
			break;
		case 2:
			mstr = "武器伤害抵抗+";
			break;
		case 3:
			mstr = "武器暴击加深+";
			break;
		case 4:
			mstr = "武器暴击抵抗+";
			break;
		case 5:
			mstr = "技能伤害加深+";
			break;
		case 6:
			mstr = "技能伤害抵抗+";
			break;
		case 7:
			mstr = "技能暴击加深+";
			break;
		case 8:
			mstr = "技能暴击抵抗+";
			break;
		default:
			break;
		}
		return mstr;
	}
	public void Init(int page = 1)
	{
	
		QueryFuwen  m_mQueryFuwen  = new QueryFuwen  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		m_mQueryFuwen.tab = page;
		MiBaoinfoer.Serialize (MiBaoinfoStream,m_mQueryFuwen);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_MAINPAGE_REQ,ref t_protof,ProtoIndexes.S_FUWEN_MAINPAGE_RES.ToString());

		InitPage ();

	}
	void InitPage()
	{
		
		int page1number = FuWenTabTemplate.getFuWenTabTemplateBytab (1).level;
		int page2number = FuWenTabTemplate.getFuWenTabTemplateBytab (2).level;
		int page3number = FuWenTabTemplate.getFuWenTabTemplateBytab (3).level;
		int junzhulevel = JunZhuData.Instance ().m_junzhuInfo.level;

		int JunZhuLevel = JunZhuData.Instance ().m_junzhuInfo.level;
		
		if(page1number > JunZhuLevel)
		{
			page1.text = page1number.ToString()+"级开启";
			Locks[0].SetActive(true);
		}
		else
		{
			page1.text = "符印一";
			Locks[0].SetActive(false);
		}
		if(page2number > JunZhuLevel)
		{
			Locks[1].SetActive(true);
			page2.text = page2number.ToString()+"级开启";
		}
		else
		{
			page2.text = "符印二";
			Locks[1].SetActive(false);
		}
		if(page3number > JunZhuLevel)
		{
			Locks[2].SetActive(true);
			page3.text = page3number.ToString()+"级开启";
		}
		else
		{
			page3.text = "符印三";
			Locks[2].SetActive(false);
		}
	}
	void setActiveFalse(int index)
	{
		ActiveBtns.ForEach (item =>DosetActiveFalse(item) );

		ActiveBtns [index].SetActive (true);
	}
	void DosetActiveFalse(GameObject mobg)
	{
		mobg.SetActive (false);
	}
	public void GetBagInfo()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_LOAD_FUWEN_IN_BAG,"8006"); // 
	}
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_FUWEN_MAINPAGE_RES: //FW首页返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				QueryFuwenResp mQueryFuwenResp  = new QueryFuwenResp();
				
				t_qx.Deserialize(t_stream, mQueryFuwenResp, mQueryFuwenResp.GetType());

				mQueryFuwen = mQueryFuwenResp;
		
				InitData();

				return true;
			}
			case ProtoIndexes.S_LOAD_FUWEN_IN_BAG_RESP: //FWbag返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenInBagResp mFuwenInBagResp  = new FuwenInBagResp();
				
				t_qx.Deserialize(t_stream, mFuwenInBagResp, mFuwenInBagResp.GetType());
				
				mFuwenInBag = mFuwenInBagResp;
				Debug.Log("Log_Times  = ");
				InitBags();
				
				return true;
			}
			case ProtoIndexes.S_FUWEN_OPERAT_RES: //符文镶嵌返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenResp mFuwenResp  = new FuwenResp();
				
				t_qx.Deserialize(t_stream, mFuwenResp, mFuwenResp.GetType());
	
				Debug.Log("0符文镶嵌或1拆卸成功mFuwenResp。result "+mFuwenResp.result);

				if(mFuwenResp.result == 0)
				{
					Init(mQueryFuwen.tab);
//					GetBagInfo();
				}
				else if(mFuwenResp.result == 1)
				{
					BackToFirst();
					Init(mQueryFuwen.tab);
//					GetBagInfo();
				}
				else 
				{
					Debug.Log("操作失败");
				}
				return true;
			}
			case ProtoIndexes.S_FUWEN_DUI_HUAN_RESP: //符文兑换请求返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenDuiHuanResp mFuwenDuiHuanResp  = new FuwenDuiHuanResp();
				
				t_qx.Deserialize(t_stream, mFuwenDuiHuanResp, mFuwenDuiHuanResp.GetType());
				
				if(mFuwenDuiHuanResp.result == 0)
				{
					string mStr = "兑换成功！";
					ClientMain.m_UITextManager.createText( mStr);
					GetBagInfo();
				}
				else
				{
					string mStr = "当前甲片数量不够，不足以进行兑换！";
					ClientMain.m_UITextManager.createText( mStr);
				}
				return true;
			}
			case ProtoIndexes.S_FUWEN_EQUIP_ALL_RESP: //一键穿戴返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenEquipAllResp mFuwenEquipAllResp  = new FuwenEquipAllResp();
				
				t_qx.Deserialize(t_stream, mFuwenEquipAllResp, mFuwenEquipAllResp.GetType());
				
				if(mFuwenEquipAllResp.result == 0)
				{
					string mStr = "一键穿戴成功！";
					ClientMain.m_UITextManager.createText( mStr);
					Init(mQueryFuwen.tab);
				}
				else if(mFuwenEquipAllResp.result == 1)
				{
					string mStr = "没有可穿戴的符文！";
					ClientMain.m_UITextManager.createText( mStr);
				}
				else if(mFuwenEquipAllResp.result == 2)
				{
					string mStr = "页签未解锁！";
					ClientMain.m_UITextManager.createText( mStr);
				}
				return true;
			}
			case ProtoIndexes.S_FUWEN_UNLOAD_ALL_RESP: //一键拆卸返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenUnloadAllResp mFuwenUnloadAllResp  = new FuwenUnloadAllResp();
				
				t_qx.Deserialize(t_stream, mFuwenUnloadAllResp, mFuwenUnloadAllResp.GetType());
				
				if(mFuwenUnloadAllResp.result == 0)
				{
					string mStr = "一键拆卸成功！";
					ClientMain.m_UITextManager.createText( mStr);
	
					Init(mQueryFuwen.tab);
				}
				else if(mFuwenUnloadAllResp.result == 1)
				{
					string mStr = "失败，没有可拆卸的符文！";
					ClientMain.m_UITextManager.createText( mStr);
				}
				else if(mFuwenUnloadAllResp.result == 2)
				{
					string mStr = "页签未解锁！";
					ClientMain.m_UITextManager.createText( mStr);
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
	private bool IsOpenlock;
	void InitData()
	{
		if(mQueryFuwen == null)
		{
			mQueryFuwen = MiBaoGlobleData.Instance().mueryFuwen;
		}
		Debug.Log ("Global.m_sPanelWantRun = "+Global.m_sPanelWantRun);
		if(Global.m_sPanelWantRun != null || Global.m_sPanelWantRun != "")
		{
			if(Global.m_sPanelWantRun == "LieFu")
			{
				GoTOLieFu();
				Global.m_sPanelWantRun = null;
			}
		}
		GetBagInfo();
		if(RongHeUIisOpen)
		{
			foreach(FuwenLanwei mlanwei in mQueryFuwen.lanwei)
			{
				if(mlanwei.lanweiId == FuWenInfoShow.Instance().mFuWenlanwei.lanweiId)
				{
					FuWenInfoShow.Instance().mFuWenlanwei = mlanwei;
					FuWenInfoShow.Instance().Init();
				}
			}
			
		}
		JiaPianNumber.text = GetHuFuNum ().ToString();
		Debug.Log ("mQueryFuwen.result = "+mQueryFuwen.result);
		if(mQueryFuwen.result == 0)
		{
			IsOpenlock = true;
			string FuwenPagelabel = "";
			switch(mQueryFuwen.tab)
			{
			case 1:
				FuwenPagelabel = "符印一";
				break;
			case 2:
				FuwenPagelabel = "符印二";
				break;
			case 3:
				FuwenPagelabel = "符印三";
				break;
			default:
				break;
			}
			FuwenPage.text = FuwenPagelabel;

		}
		else
		{
			IsOpenlock = false;
			Debug.Log("未解锁");
			string mStr = "未解锁";
			ClientMain.m_UITextManager.createText( mStr);
			return;
		}
		SortPropertyList ();
		setActiveFalse ( mQueryFuwen.tab -1 );
		InitPage ();
	}
	void CreateFuWenLanWei(bool bagisEmpty)
	{

		OneKeyArelt.SetActive(false);
		bool couldChanger = false;
		bool CouldLevelUp = false;
//		Debug.Log ("mmFuWenLanWeiList.Count = "+mFuWenLanWeiList.Count);
		for(int i = 0 ; i < mQueryFuwen.lanwei.Count; i++)
		{
//			Debug.Log ("mQueryFuwen.lanwei[i].itemId = "+mQueryFuwen.lanwei[i].itemId);
			if(i < mFuWenLanWeiList.Count)
			{
				mFuWenLanWeiList[i].mFuwenLanwei = mQueryFuwen.lanwei[i];
				mFuWenLanWeiList[i].showChangeTips = false;
				if(!bagisEmpty)
				{
					for(int j = 0 ; j < mFuwenInBag.fuwenList.Count ; j ++)
					{
						if(mQueryFuwen.lanwei[i].itemId != 0)
						{
							FuWenTemplate mfwbg1 = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenInBag.fuwenList[j].itemId);
							FuWenTemplate mfwlanweo2 = FuWenTemplate.GetFuWenTemplateByFuWenId(mQueryFuwen.lanwei[i].itemId);
							if(mfwlanweo2.inlayColor == mfwbg1.inlayColor )
							{
								if(mfwlanweo2.color == mfwbg1.color)
								{
									if(mfwlanweo2.fuwenLevel < mfwbg1.fuwenLevel)
									{
										OneKeyArelt.SetActive(true);
										mFuWenLanWeiList[i].showChangeTips = true;
										couldChanger  = true;
										break;
									}
								}
								else
								{
									if(mfwlanweo2.color < mfwbg1.color)
									{
										OneKeyArelt.SetActive(true);
										mFuWenLanWeiList[i].showChangeTips = true;
										couldChanger  = true;
										break;
									}
								}
			
							}
							if(mQueryFuwen.lanwei[i].flag)
							{
								CouldLevelUp = true;
							}
						}
						else if(mQueryFuwen.lanwei[i].itemId == 0)
						{
							if(mQueryFuwen.lanwei[i].flag)
							{
								OneKeyArelt.SetActive(true);
								couldChanger = true;
							}
						}
					}
				}
			
				mFuWenLanWeiList[i].init();
			}
		}
		if(mQueryFuwen.lanwei[0].itemId != 0)
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
		}
		// 符文的三个引导开关都在这控制
//		Debug.Log ("CouldLevelUp = "+CouldLevelUp);
//		Debug.Log ("OneKeyArelt.activeInHierarchy = "+OneKeyArelt.activeInHierarchy);
//		Debug.Log ("couldChanger = "+couldChanger);
//
//		Debug.Log ("500016 = "+PushAndNotificationHelper.IsShowRedSpotNotification(500016));
//		Debug.Log ("500014 = "+PushAndNotificationHelper.IsShowRedSpotNotification(500014));
//		Debug.Log ("500017 = "+PushAndNotificationHelper.IsShowRedSpotNotification(500017));
//		Debug.Log ("500010 = "+PushAndNotificationHelper.IsShowRedSpotNotification(500010));
		if(!CouldLevelUp)
		{
			int levelUp = 500016;
			PushAndNotificationHelper.SetRedSpotNotification(levelUp,false);
		}
		else
		{
			int levelUp = 500016;
			PushAndNotificationHelper.SetRedSpotNotification(levelUp,true);
		}
		if(!OneKeyArelt.activeInHierarchy)
		{
			int Dressid = 500014;
			PushAndNotificationHelper.SetRedSpotNotification(Dressid,false);
		}
		if(!couldChanger)
		{
			int CanChanege = 500017;
			PushAndNotificationHelper.SetRedSpotNotification(CanChanege,false);
		}
	}
	private List<GameObject> fuWenBagItemList = new List<GameObject> ();
	public GameObject BagFuWenItem;
	public UIGrid mGrid;
	public void InitBags() //初始化背包的符文
	{
		JiaPianNumber.text = GetHuFuNum ().ToString();

		OnekeyXiangqiang = false;
		if(mFuwenInBag.fuwenList == null)
		{

			Debug.Log("mFuwenInBag.fuwenList == null");
			foreach(GameObject ma_bagItem in fuWenBagItemList)
			{
				Destroy(ma_bagItem);

			}
			fuWenBagItemList.Clear();
			CreateBags (16);
			CreateFuWenLanWei (true);
			return;
		}
		else
		{
			int x = (mFuwenInBag.fuwenList.Count)%4;
			int y = 16 - x;
	
			CreateBags (mFuwenInBag.fuwenList.Count +y);
			CreateFuWenLanWei (false);
		}

		// 背包符文排序

		SortFuwens (!RongHeUIisOpen);
//
//		mScrollView.UpdateScrollbars (true);

	}
	public void SortFuwens(bool firstpage)
	{
		if (!firstpage) {
			for (int i = 0; i < mFuwenInBag.fuwenList.Count -1; i++) {
				
				for (int j = i+1; j < mFuwenInBag.fuwenList.Count; j++) {

					FuWenTemplate mtemp = FuWenTemplate.GetFuWenTemplateByFuWenId (mFuwenInBag.fuwenList [i].itemId);
					FuWenTemplate mtempNext = FuWenTemplate.GetFuWenTemplateByFuWenId (mFuwenInBag.fuwenList [j].itemId);

					if (mtemp.color > mtempNext.color) 
					{
						var mfuweninbag = mFuwenInBag.fuwenList [i];
						
						mFuwenInBag.fuwenList [i] = mFuwenInBag.fuwenList [j];
						
						mFuwenInBag.fuwenList [j] = mfuweninbag;

					} else if (mtemp.color == mtempNext.color) 
					{
						if (mtemp.fuwenLevel > mtempNext.fuwenLevel) 
						{
							var mfuweninbag = mFuwenInBag.fuwenList [i];
							
							mFuwenInBag.fuwenList [i] = mFuwenInBag.fuwenList [j];
							
							mFuwenInBag.fuwenList [j] = mfuweninbag;

						} else if (mtemp.fuwenLevel == mtempNext.fuwenLevel) 
						{
							if(mFuwenInBag.fuwenList[i].exp > mFuwenInBag.fuwenList[j].exp)
							{
								var mfuweninbag = mFuwenInBag.fuwenList [i];
								
								mFuwenInBag.fuwenList [i] = mFuwenInBag.fuwenList [j];
								
								mFuwenInBag.fuwenList [j] = mfuweninbag;
							}else if(mFuwenInBag.fuwenList[i].exp == mFuwenInBag.fuwenList[j].exp)
							{
								if(mFuwenInBag.fuwenList [i].cnt > mFuwenInBag.fuwenList [j].cnt)
								{
									var mfuweninbag = mFuwenInBag.fuwenList [i];
									
									mFuwenInBag.fuwenList [i] = mFuwenInBag.fuwenList [j];
									
									mFuwenInBag.fuwenList [j] = mfuweninbag;
								}
							}
						}
					}
					
				}
			}
	

		} else {
			for (int i = 0; i < mFuwenInBag.fuwenList.Count -1; i++) {
				
				for(int j = i+1; j < mFuwenInBag.fuwenList.Count; j++)
				{
					FuWenTemplate mtemp = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenInBag.fuwenList[i].itemId);
					FuWenTemplate mtempNext = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenInBag.fuwenList[j].itemId);
					if(mtemp.color < mtempNext.color)
					{
						var mfuweninbag = mFuwenInBag.fuwenList[i];
						
						mFuwenInBag.fuwenList[i] = mFuwenInBag.fuwenList[j];
						
						mFuwenInBag.fuwenList[j] = mfuweninbag;
					}
					else if(mtemp.color == mtempNext.color)
					{
						if(mtemp.fuwenLevel < mtempNext.fuwenLevel)
						{
							var mfuweninbag = mFuwenInBag.fuwenList[i];
							
							mFuwenInBag.fuwenList[i] = mFuwenInBag.fuwenList[j];
							
							mFuwenInBag.fuwenList[j] = mfuweninbag;
						}
						else if(mtemp.fuwenLevel == mtempNext.fuwenLevel)
						{
							if(mFuwenInBag.fuwenList[i].exp < mFuwenInBag.fuwenList[j].exp)
							{
								var mfuweninbag = mFuwenInBag.fuwenList[i];
								
								mFuwenInBag.fuwenList[i] = mFuwenInBag.fuwenList[j];
								
								mFuwenInBag.fuwenList[j] = mfuweninbag;
							}
							else if(mFuwenInBag.fuwenList[i].exp == mFuwenInBag.fuwenList[j].exp)
							{
								if(mFuwenInBag.fuwenList [i].cnt < mFuwenInBag.fuwenList [j].cnt)
								{
									var mfuweninbag = mFuwenInBag.fuwenList [i];
									
									mFuwenInBag.fuwenList [i] = mFuwenInBag.fuwenList [j];
									
									mFuwenInBag.fuwenList [j] = mfuweninbag;
								}
							}
						}
					}
					
				}
			}

		}
		fuWenBagItemList = QXComData.CreateGameObjectList (BagFuWenItem, mGrid, mFuwenInBag.fuwenList.Count, fuWenBagItemList);
		for (int i = 0; i < fuWenBagItemList.Count; i++) {
			bagItem m_bagItem = fuWenBagItemList [i].GetComponent<bagItem> ();
			
			m_bagItem.mfwbg = mFuwenInBag.fuwenList [i];
			
			m_bagItem.init ();
		}
		mScrollView.UpdatePosition ();
	}
	public void OneKeyOff() // 一键拆卸
	{
		if(!IsOpenlock)return;
		int FuwenNumber = 0;
		for(int i = 0 ; i < mQueryFuwen.lanwei.Count; i++)
		{
			if(mQueryFuwen.lanwei[i].itemId == 0)
			{
				FuwenNumber += 1;
			}
		}
		if(FuwenNumber == 8)
		{
			string mData = "无可拆卸的符文!";
			ClientMain.m_UITextManager.createText( mData);
			return;
		}
		FuwenUnloadAll  mFuwenUnloadAll  = new FuwenUnloadAll  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		mFuwenUnloadAll.tab = mQueryFuwen.tab;
		
		MiBaoinfoer.Serialize (MiBaoinfoStream,mFuwenUnloadAll);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_UNLOAD_ALL,ref t_protof,ProtoIndexes.S_FUWEN_UNLOAD_ALL_RESP.ToString());
	}
	public void OneKeyOn() // 一键穿戴
	{
		if(!IsOpenlock)return;
		int FuwenNumber = 0;
		for(int i = 0 ; i < mQueryFuwen.lanwei.Count; i++)
		{
			if(mQueryFuwen.lanwei[i].itemId == 0)
			{
				FuwenNumber += 1;
			}
		}
//		if(FuwenNumber == 0)
//		{
//			string mData = "符文栏已满!";
//			ClientMain.m_UITextManager.createText( mData);
//			return;
//		}

		FuwenEquipAll  mFuwenEquipAll  = new FuwenEquipAll  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		mFuwenEquipAll.tab = mQueryFuwen.tab;

		MiBaoinfoer.Serialize (MiBaoinfoStream,mFuwenEquipAll);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_EQUIP_ALL,ref t_protof,ProtoIndexes.S_FUWEN_EQUIP_ALL_RESP.ToString());

	}
	public void OnKeyIn()// 一键放入
	{
		if(!IsOpenlock)return;

		FuWenTemplate mFUwen = FuWenTemplate.GetFuWenTemplateByFuWenId (FuWenInfoShow.Instance().mFuWenlanwei.itemId);
		if(mFUwen.fuwenLevel >= mFUwen.levelMax)
		{
			ClientMain.m_UITextManager.createText("符文等级已达最高，不能再进行熔合了！");
			return;
		}
//		Debug.Log ("mFuwenInBag.fuwenList.Count = " +mFuwenInBag.fuwenList.Count);
		if(mFuwenInBag.fuwenList == null || mFuwenInBag.fuwenList.Count <= 0)
		{
			string mData = "无任何可放入的符文！";
			ClientMain.m_UITextManager.createText(mData);
			return;
		}
		if(fuWenBagItemList[0].GetComponent<bagItem>().isOneKey)
		{
			return;
		}
		StartCoroutine ("AllXIangQian");
	}
	public bool OnekeyXiangqiang = false;
	IEnumerator AllXIangQian()
	{
//		FuWenTemplate FuwenInfo = FuWenTemplate.GetFuWenTemplateByFuWenId(FuWenInfoShow.Instance().mCurrFuWenlanwei.itemId);

//		Debug.Log ("FuWenInfoShow.Instance().mCurrFuWenlanwei.exp = "+FuWenInfoShow.Instance().mCurrFuWenlanwei.exp);
//
//		Debug.Log ("FuwenInfo.lvlupExp = "+FuWenInfoShow.Instance().CurrLevelUpExp);

		if(FuWenInfoShow.Instance ().fuwensinBag.Count > 0 && !OnekeyXiangqiang)
		{
			SecondPage.GetComponent<FuWenInfoShow>().Init ();
			for(int i = 0; i < fuWenBagItemList.Count; i++)
			{
				bagItem m_bagItem = fuWenBagItemList[i].GetComponent<bagItem>();
				
				m_bagItem.init();
			}
		}
		FuWenInfoShow.Instance().LanweiExp = 0;
		if(!OnekeyXiangqiang)
		{
			for(int i = 0; i < fuWenBagItemList.Count; i++)
			{
				yield return new WaitForSeconds(0.01f);
				bagItem m_bagItem = fuWenBagItemList[i].GetComponent<bagItem>();
//
//				Debug.Log ("FuWenInfoShow.Instance().LanweiExp = "+FuWenInfoShow.Instance().LanweiExp);
//
//				Debug.Log ("FuWenInfoShow.Instance().CurrLevelUpExp = "+FuWenInfoShow.Instance().CurrLevelUpExp);

				if(FuWenInfoShow.Instance().LanweiExp >= FuWenInfoShow.Instance().CurrLevelUpExp)
				{
					break;
				}
				else
				{
					m_bagItem.isOneKey = true;
					m_bagItem.Choose();
					//Debug.Log ("mCurrFuWenlanwei.exp = "+FuWenInfoShow.Instance().mCurrFuWenlanwei.exp);
				}
			}
			OnekeyXiangqiang = true;
		}

	}
	public void GoTOLieFu() // 猎符
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LIEFU ),
		                        						                        NewFuWenObjLoadBack );
	}
	GameObject NewfuWenObj;
	void NewFuWenObjLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		Debug.Log ("去猎符");
		if(NewfuWenObj == null)
		{
			NewfuWenObj = GameObject.Instantiate( p_object ) as GameObject;
			LieFuManagerment mNewfuWenObj = NewfuWenObj.GetComponent<LieFuManagerment>();
			mNewfuWenObj.Init (YinDaoManager);
			MainCityUI.TryAddToObjectList(NewfuWenObj,true,false);
		} 
	}
	public void ChangeJiaPian()//兑换甲片
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.JIAPIANCASH ),
		                        ChangeJIaPian );
	}
	GameObject ChangeUI;
	void ChangeJIaPian ( ref WWW p_www, string p_path, Object p_object )
	{
		Debug.Log ("去兑换");
		if(ChangeUI == null)
		{
			ChangeUI = GameObject.Instantiate( p_object ) as GameObject;
		} 
	}
	public void BackBtn()//返回
	{
		BackToFirst ();
		InitBags ( );
	
	}
//	public void ChaiJie() // 拆卸
//	{
//
//	}
//	public void RongHe() //融合
//	{
//
//	}
	int lanweiInDex;
	public void XiangQian(int index) //镶嵌
	{
		if(!IsOpenlock)return;
//		Debug.Log ("mQueryFuwen.lanwei [index].itemId = "+mQueryFuwen.lanwei [index].itemId);
		lanweiInDex = index;
		 if(mQueryFuwen.lanwei [index].itemId == 0)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.FUWENCHUANDAI ),
			                        FuWenDress );
		}
		else
		{

			if(mFuWenLanWeiList[index].showChangeTips)
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.FUWENCHUANDAI ),
				                        FuWenChange );
			}else
			{

				FirstPage.SetActive (false);
				
				SecondPage.SetActive (true);

				RongHeUIisOpen = true;

				InitBags();

				FuWenInfoShow mFuWenInfoShow = SecondPage.GetComponent<FuWenInfoShow>();
				
				mFuWenInfoShow.FuWenItem_Id = mQueryFuwen.lanwei [index].itemId;

				mFuWenInfoShow.mFuWenlanwei = mQueryFuwen.lanwei [index];

				mFuWenInfoShow.Init ();
			}
		}
	}
	GameObject Fwchuandai;
	void FuWenDress ( ref WWW p_www, string p_path, Object p_object )
	{
		Debug.Log ("符文穿戴");
		if(Fwchuandai == null)
		{
			Fwchuandai = GameObject.Instantiate( p_object ) as GameObject;

			FuWenDress mFuWenDress = Fwchuandai.GetComponent<FuWenDress>();

			mFuWenDress.mFuwenLanwei = mQueryFuwen.lanwei[lanweiInDex];

			mFuWenDress.mFuwenInBagResp = mFuwenInBag;

			mFuWenDress.tab = mQueryFuwen.tab;

			mFuWenDress.XiangQian_TiHuan = true;

			mFuWenDress.Init();
			//MainCityUI.TryAddToObjectList(NewfuWenObj,false);
		} 
	}
	GameObject FwChange;
	void FuWenChange ( ref WWW p_www, string p_path, Object p_object )
	{
		Debug.Log ("符文替换");
		if(FwChange == null)
		{
			FwChange = GameObject.Instantiate( p_object ) as GameObject;

			FwChange.name = "FuWenChange";

			FuWenDress mFuWenDress = FwChange.GetComponent<FuWenDress>();
			
			mFuWenDress.mFuwenLanwei = mQueryFuwen.lanwei[lanweiInDex];
			
			mFuWenDress.mFuwenInBagResp = mFuwenInBag;
			
			mFuWenDress.tab = mQueryFuwen.tab;

			mFuWenDress.XiangQian_TiHuan = false;

			mFuWenDress.Init();
			//MainCityUI.TryAddToObjectList(NewfuWenObj,false);
		} 
	}

	public void ChaiJieFuWenOprection() // 符文拆解
	{
		OperateFuwenReq  mOperateFuwenReq  = new OperateFuwenReq  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		mOperateFuwenReq.tab = mQueryFuwen.tab;
		mOperateFuwenReq.action = 6;
		mOperateFuwenReq.lanweiId = mQueryFuwen.lanwei[lanweiInDex].lanweiId;
		MiBaoinfoer.Serialize (MiBaoinfoStream,mOperateFuwenReq);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_OPERAT_REQ,ref t_protof,ProtoIndexes.S_FUWEN_OPERAT_RES.ToString());
	}

	public void OPenFuYin(int index) //符文翻页
	{
		int openlevel = FuWenTabTemplate.getFuWenTabTemplateBytab (index).level;
		int junzhulevel = JunZhuData.Instance ().m_junzhuInfo.level;
		if(openlevel > junzhulevel)
		{
			string mstr = openlevel.ToString()+ "级后开启";

			ClientMain.m_UITextManager.createText(mstr);

			return;
		}
		if(index == mQueryFuwen.tab)
		{
			return;
		}
		if(RongHeUIisOpen)
		{
			BackBtn();
		}
		Init (index);
	}
	List<GameObject> Bags = new List<GameObject>();

	public GameObject bagstemp;

	public UIGrid  mgrid;
	private int bagnuber;// 临时变量
	void CreateBags(int bagNumber)
	{
		if(bagNumber > bagnuber)
		{
			for(int i = bagnuber ;i < bagNumber; i ++)
			{
				GameObject mBag = Instantiate(bagstemp) as GameObject;
				mBag.SetActive(true);
				mBag.transform.parent = bagstemp.transform.parent;
				mBag.transform.localScale = Vector3.one;
				Bags.Add(mBag);
			}
			bagnuber = bagNumber;
			mgrid.repositionNow = true;
		}
//		mScrollView.UpdatePosition ();
	}
	public  string GetFuWenProperty(int index)
	{
		string mstr = "";
		switch(index)
		{
		case 1:
			mstr = "攻击";
			break;
		case 2:
			mstr = "防御";
			break;
		case 3:
			mstr = "生命";
			break;
		case 4:
			mstr = "武器伤害加深";
			break;
		case 5:
			mstr = "武器伤害抵抗";
			break;
		case 6:
			mstr = "武器暴击加深";
			break;
		case 7:
			mstr = "武器暴击抵抗";
			break;
		case 8:
			mstr = "技能伤害加深";
			break;
		case 9:
			mstr = "技能伤害抵抗";
			break;
		case 10:
			mstr = "技能暴击加深";
			break;
		case 11:
			mstr = "技能暴击抵抗";
			break;
		default:
			break;
		}
		return mstr;
	}
	void HelpBtn()
	{
		GeneralControl.Instance.LoadRulesPrefab (LanguageTemplate.GetText (LanguageTemplate.Text.LIEFUDESC));
	}
	void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
