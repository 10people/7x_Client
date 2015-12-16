using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoManager : MYNGUIPanel  , SocketListener
{

    public ScaleEffectController m_ScaleEffectController;

	public UIFont TitleFont;

	public UIFont BtnFont;

	public UIFont CantinFont;

	public UILabel PointNum;// 点数

	public UILabel CountTime; // 到时间

	public UILabel TongBiNum; //铜币数

	public UILabel ZhanLiNum; //战力

	public GameObject MiBao_ScrollView; //秘宝UI

	public GameObject MiBao_TempInfo; //秘宝信息

	public GameObject MiBao_ZhanLiInfo; //秘宝战力信息界面

	public GameObject MiBao_SkillInfo; //秘宝技能介绍

	public GameObject MiBao_SkillZuHeInfo; //组合技能介绍

	//public GameObject Lock_PieceUI; //缺少碎片界面\

	public GameObject mBackBtn;

	private int MaxPoint ; //最大点数 

	public string Curr_UIName;//记录当前显示的UI名字

	public MibaoInfoResp G_MiBaoInfo;

	public static MiBaoManager mMiBaoData;

	public List<GameObject> UIRoots = new List<GameObject>();

	//public MibaoGroup MiBaoManager_mMiBaoGroup;

	public int CurrSkill_id;

	public MibaoInfo MiBaoManager_mMiBaotempinfo;

	public List<MBTemp> mMBTempList = new List<MBTemp>();

	private int JunzhuZhaoli = 0;

	public NGUILongPress EnergyDetailLongPress;

	public static MiBaoManager Instance ()
	{
		if (!mMiBaoData)
		{
			mMiBaoData = (MiBaoManager)GameObject.FindObjectOfType (typeof(MiBaoManager));
		}
		
		return mMiBaoData;
	}


	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
		EnergyDetailLongPress.LongTriggerType = NGUILongPress.TriggerType.Press;
		EnergyDetailLongPress.NormalPressTriggerWhenLongPress = false;
		EnergyDetailLongPress.OnLongPressFinish = OnCloseDetail;
		EnergyDetailLongPress.OnLongPress = OnEnergyDetailClick;
		//Debug.Log ( "MiBaoManager.Awake()" );
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		//Debug.Log ( "MiBaoManager.OnDestroy()" );
	}

	void Start () 
	{

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);

		TaskData.Instance.m_DestroyMiBao = true;

		InitStart ();
	}

	public void OnEnergyDetailClick(GameObject go)//显示体力恢复提示
	{
		int commonItemId = 900001;
		
		ShowTip.showTip( commonItemId);
		//        RecoverToliCips.transform.localScale = new Vector3(1, 1, 1);
		//        Invoke("diseCoverTiLiClips", 1.5f);
	}
	private void OnCloseDetail(GameObject go)
	{
		ShowTip.close();
	}
	#region fulfil my ngui panel
	
	/// <summary>
	/// my click in my ngui panel
	/// </summary>
	/// <param name="ui"></param>
	public override void MYClick(GameObject ui)
	{
		
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	
	#endregion
	public void InitStart ()
	{

		//G_MiBaoInfo = MiBaoGlobleData.Instance ().G_MiBaoInfo;

		VipTemplate mVip = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance().m_junzhuInfo.vipLv);


		MaxPoint = mVip.MiBaoLimit;

		foreach(MBTemp mMBTemp in mMBTempList)
		{
			
			Destroy(mMBTemp.gameObject);
		}

		mMBTempList.Clear ();

		foreach(GameObject m_ui in UIRoots)
		{
			
			Destroy(m_ui);
		}
		
		UIRoots.Clear ();
		
		UIRoots.Add (MiBao_ScrollView);

		MiBao_ScrollView.SetActive (true);

		UIRoots.Add (MiBao_TempInfo);
		
		UIRoots.Add (MiBao_ZhanLiInfo);
		
		UIRoots.Add (MiBao_SkillInfo);
		
		UIRoots.Add (MiBao_SkillZuHeInfo);
		
		//UIRoots.Add (Lock_PieceUI);
		
		mBackBtn.SetActive (false);
		
		Curr_UIName = "MiBaoScrollView";
	
		//Init(G_MiBaoInfo);
		ZhanLiNum.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString();

		JunzhuZhaoli = JunZhuData.Instance ().m_junzhuInfo.zhanLi;
	}

	void Update () {
		
		TongBiNum .text = JunZhuData.Instance ().m_junzhuInfo.jinBi.ToString ();

	}

	public void ShowZhanLiAnmition()
	{
		StopCoroutine("Update_ZhanliAndLevel");
		
		StartCoroutine("Update_ZhanliAndLevel");
	}
	IEnumerator Update_ZhanliAndLevel()
	{
		yield return new WaitForSeconds(0.1f);

//		Debug.Log("JunZhuData.Instance ().m_junzhuInfo.zhanLi  = "+JunZhuData.Instance ().m_junzhuInfo.zhanLi);
//		
//		Debug.Log("JunzhuZhaoli  = "+JunzhuZhaoli);

		if(JunZhuData.Instance ().m_junzhuInfo.zhanLi > JunzhuZhaoli)
		{
			int m = (JunZhuData.Instance ().m_junzhuInfo.zhanLi - JunzhuZhaoli);
			
			float mTime = 0.01f;
			
			while(m > 0)
			{
				
				if(m > 10000)
				{
					m -= 10000;
					JunzhuZhaoli += 10000;
				}
				if(m > 1000)
				{
					m -= 1000;
					JunzhuZhaoli += 1000;
				}
				if(m > 100)
				{
					m -= 100;
					JunzhuZhaoli += 100;
				}
				if(m > 10)
				{
					m -= 10;
					JunzhuZhaoli += 10;
				}
				else
				{
					m -= 1;
					JunzhuZhaoli += 1;
				}
				
				ZhanLiNum.text = (JunzhuZhaoli).ToString();
				
				ZhanLiNum.gameObject.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
				
				yield return new WaitForSeconds(mTime);
			}
//			Debug.Log("MiBao Animation1");
			
			ZhanLiNum.gameObject.transform.localScale = Vector3.one;
		}
		else{
//			Debug.Log("MiBao Animation2");
			
			ZhanLiNum.text = JunZhuData.Instance ().m_junzhuInfo.zhanLi.ToString();
		}
	}
	public void SortUI(string UI_Name)
	{
//		Debug.Log ("UI_Name = " +UI_Name);

		foreach(GameObject m_ui in UIRoots)
		{
			if(m_ui.activeInHierarchy)
			{
				Curr_UIName = m_ui.name;
			}

			if(m_ui.name == UI_Name)
			{
				if(UI_Name == "MiBaoScrollView")
				{
					mBackBtn.SetActive (false);
				}
				else
				{
					mBackBtn.SetActive (true);
				}

				m_ui.SetActive(true);

				switch(UI_Name)
				{
				case "MiBaoSkillInfo":

					ShowMiBaoSkill mShowMiBaoSkill = m_ui.GetComponent<ShowMiBaoSkill>();
					
					mShowMiBaoSkill.ShowMiBaoGroupTemp = G_MiBaoInfo;

				//	mShowMiBaoSkill.ShowMiBaoGroup = MiBaoManager_mMiBaoGroup;

					mShowMiBaoSkill.SkillId = CurrSkill_id;

					mShowMiBaoSkill.Init();

					break;

				case "MiBaoScrollView":
					

					MiBaoScrollView mMiBaoScrollView = MiBao_ScrollView.GetComponent<MiBaoScrollView>();

					mMiBaoScrollView.my_MiBaoInfo = G_MiBaoInfo;

					mMiBaoScrollView.Init();
					
					break;

				case "MiBaoSkillZuheInfo":
					

					ShowAllMibaoSkill mShowAllMibaoSkill = m_ui.GetComponent<ShowAllMibaoSkill>();
					
					mShowAllMibaoSkill.SkillId = CurrSkill_id;
					
					mShowAllMibaoSkill.Init();
					break;

				case "MiBaoTempInfo":
					

					MiBaoDesInfo mMiBaoDesInfo = m_ui.GetComponent<MiBaoDesInfo>();
					
					mMiBaoDesInfo.ShowmMiBaoinfo = MiBaoManager_mMiBaotempinfo;
					
					mMiBaoDesInfo.Init();

					
					break;

				case "MiBaoZhanLiInfo":

					break;
				default:
					break;
				}
			}
			else
			{
				m_ui.SetActive(false);
			}
		}
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
		switch (p_message.m_protocol_index)
		{
			case ProtoIndexes.S_MIBAO_INFO_RESP:
			{
//				Debug.Log("秘宝数据返回了glob ");

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());

				G_MiBaoInfo = MiBaoInfo;

				//Debug.Log("m_MiBaoInfo.remainTime = " +G_MiBaoInfo.remainTime);
				//Debug.Log("m_MiBaoInfo.levelPoint = " +G_MiBaoInfo.levelPoint);

				Init(MiBaoInfo);

				return true;
	        }
			default: return false;
		}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}

	public void Init(MibaoInfoResp m_MiBaoInfo)
	{
	
		PointNum.text = m_MiBaoInfo.levelPoint.ToString ();

		t = m_MiBaoInfo.remainTime;

		if(m_MiBaoInfo.remainTime < 0)
		{
			//Debug.Log( "Stop 1" );

			StopCoroutine("showTime");

			CountTime.text = "("+"已满"+")";

		}else
		{
			//Debug.Log("m_MiBaoInfo.remainTime = " +m_MiBaoInfo.remainTime);

			//Debug.Log( "Stop Start 2" );

			StopCoroutine("showTime");

			StartCoroutine("showTime");
		}


		if(MiBao_ScrollView.activeInHierarchy)
		{

			MiBaoScrollView mMiBaoScrollView = MiBao_ScrollView.GetComponent<MiBaoScrollView>();

			mMiBaoScrollView.my_MiBaoInfo = G_MiBaoInfo;

			mMiBaoScrollView.Init();
		}
	}

	int t = 0 ;

	IEnumerator showTime()
	{
		//float t_id = Random.Range (-1000.0f, 1000.0f );

		while(t > 0)
		{
			yield return new WaitForSeconds (1.0f);
			t -= 1;

			int M = (int)(t/60);

			int S = (int)(t%60);

			string s = "";

			if( S < 10)
			{
				s = "0"+S.ToString();
			}
			else
			{
				s = S.ToString();
			}

			CountTime.text = "("+M.ToString()+":"+s+")";
	
			//Debug.Log( t_id + ": " + s );
		}

			G_MiBaoInfo.levelPoint += 1;

		   // Debug.Log ("G_MiBaoInfo.levelPoint = " +G_MiBaoInfo.levelPoint);

			PointNum.text = G_MiBaoInfo.levelPoint.ToString ();

			if(MaxPoint > G_MiBaoInfo.levelPoint)
			{
				t = 10*60;

		    	StopCoroutine("showTime");

			    StartCoroutine("showTime");

			}else{

			    CountTime.text = "("+"已满"+")";

			    //StopCoroutine("showTime");
	
			}
	}

	public void AddBtn ()
	{
		JunZhuData.Instance ().BuyTiliAndTongBi(false,true,false);

	}

	public void ShowZhanLiInfo ()
	{
		if(MiBao_ZhanLiInfo.activeInHierarchy)
		{
			SortUI ("MiBaoScrollView");

			return;
			//SortUI ("MiBaoScrollView");
		}

			SortUI ("MiBaoZhanLiInfo");
	
	}

	public void CloseBtn ()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
        MainCityUI.TryRemoveFromObjectList(gameObject);
     //   MainCityUI.TryRemoveFromObjectList(gameObject);
		TaskData.Instance.m_DestroyMiBao = false;
        Destroy(this.gameObject);
    }

	public void BackBtn ()
	{

		SortUI ("MiBaoScrollView");
	}
	public void ShowTips()
	{

		int commonItemId = 900001;

		ShowTip.showTip( commonItemId);
	}
}
