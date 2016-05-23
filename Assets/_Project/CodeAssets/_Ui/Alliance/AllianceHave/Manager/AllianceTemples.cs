using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class AllianceTemples : MonoBehaviour ,SocketProcessor {

	public ExploreResp g_mExploreResp;

	public UILabel mGongXian;

	public UILabel mRuls;

	public UILabel mAddProperty1;

	public UILabel mAddProperty2;

	public UILabel JiBaiCostPreTime;//每一次花费

	public UILabel RemainTimes;//剩余次数

	private int CostPreTime;

	private int AllTime; //剩余总次数

	public List<JiBaiAward> mJiBaiList = new List<JiBaiAward>();

	public List<Award> OneKeyAward = new List<Award>();

	public GameObject JiBaoBtn;

	public GameObject OneKeyJiBaoBtn;

	public GameObject DengDaiYaoJiang;

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}

	void Start () {
	
		if(FreshGuide.Instance().IsActive(400020)&& TaskData.Instance.m_TaskInfoDic[400020].progress >= 0)
		{
			//			Debug.Log("去寺庙祭拜");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
		}
	}
	
	public void Init()
	{
		JiBaoBtn.GetComponent<UIButton>().enabled = true;
		OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = true;
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_INFO);
	}
	void Update () {

		mGongXian.text = NewAlliancemanager.Instance().m_allianceHaveRes.contribution.ToString ();
	}
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_LM_CHOU_JIANG_INFO://宗庙返回
			{
				Debug.Log("宗庙返回");
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				ExploreResp mExploreResp = new ExploreResp();
				
				application_qx.Deserialize(application_stream, mExploreResp, mExploreResp.GetType());

				g_mExploreResp = mExploreResp;
				if(mExploreResp == null)
				{
					//Debug.Log("mExploreResp = null");
				}
				InitData(mExploreResp);
				return true;
			}
			case ProtoIndexes.S_LM_CHOU_JIANG://祭拜返回
			{
				//Debug.Log("宗庙抽奖返回");
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				ErrorMessage aErrorMessage = new ErrorMessage();
				
				application_qx.Deserialize(application_stream, aErrorMessage, aErrorMessage.GetType());
				UIYindao.m_UIYindao.CloseUI();
				//Debug.Log("aErrorMessage.errorCode = "+aErrorMessage.errorCode);
				ChouJiangDataBack(aErrorMessage);
				Init();
                return true;
			}
			default:return false;
			}
		}
		
		return false;
	}
	List<int > Mark = new List<int> ();
	public void ChouJiangDataBack(ErrorMessage a_ErrorMessage)
	{

		//Debug.Log("a_ErrorMessage.errorDesc = "+a_ErrorMessage.errorDesc);
		string []s = a_ErrorMessage.errorDesc.Split('#');
		Mark.Clear ();
		OneKeyAward.Clear ();
		for (int i = 0; i < s.Length; i++)
		{
//			Debug.Log("s[i] = "+s[i]);
			Mark.Add(int.Parse(s[i]));
		}

		if(JiBaoTime) // 祭拜
		{
			StopCoroutine("ShowEff");
			StartCoroutine("ShowEff");
		}else
		{
			for (int i = 0; i < Mark.Count; i++)
			{

				mJiBaiList[Mark[i]].GetDataByChouJiang();
				OneKeyAward.Add(mJiBaiList[Mark[i]].mAwardinfo);

			}
			StartCoroutine (OpenAwardUI());
		}
//		NewAlliancemanager.Instance().Refreshtification ();
	}
	public GameObject mBoxEffect;
	int MaxNumber = 0;
	int startcount = 0;
	IEnumerator ShowEff()
	{
		bool IsStop = false;
		startcount = 0;
		int index = Random.Range (2,4);
		MaxNumber = index*10;
		float WaitingTime = 0.5f;
		mBoxEffect.SetActive (true);
		Debug.Log("MaxNumber = "+MaxNumber);
//		Debug.Log("mMark.count = "+mMark.Count);
		Debug.Log("mMark[0] = "+Mark[0]);
		int mStarcount = 0;
		while(startcount <= MaxNumber+Mark[0])
		{
			if(startcount < 10)
			{
				mJiBaiList[startcount].ChangeBoxPostion(mBoxEffect);
			}
			else
			{
			    mStarcount = startcount%10;
				mJiBaiList[mStarcount].ChangeBoxPostion(mBoxEffect);
			}

			if(startcount < 5)
			{
				if(WaitingTime > 0.04f)
				{
					WaitingTime -= 0.08f;
				}

			}
			else if(startcount >= 5 && startcount < MaxNumber+Mark[0] -5)
			{
				WaitingTime = 0.04f;
			}
			if(startcount >= MaxNumber+Mark[0] -5)
			{
				WaitingTime += 0.08f;
			}
			if(startcount == MaxNumber+Mark[0])
			{
				mBoxEffect.SetActive (false);
				DengDaiYaoJiang.SetActive(false);
				mJiBaiList[mStarcount].GetDataByChouJiang();
				OneKeyAward.Add(mJiBaiList[Mark[0]].mAwardinfo);
				StartCoroutine ("OpenAwardUI");
				//StartCoroutine("ShowAnimation");
				StopCoroutine("ShowEff");
			}
			startcount ++;
			yield return new WaitForSeconds (WaitingTime);
		}


	}
	IEnumerator OpenAwardUI()
	{
		UIYindao.m_UIYindao.CloseUI();
		float t = 1f;
		yield return new WaitForSeconds (t);
		JiBaoBtn.GetComponent<UIButton>().enabled = true;
		OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = true;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAO_DANG_DONE ),OpenLockLoadBack);
	}
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
        DengDaiYaoJiang.SetActive(false);
		GameObject tempObject = ( GameObject )Instantiate( p_object );

		tempObject.transform.parent = this.transform.parent;

		tempObject.transform.localPosition = Vector3.zero;

		tempObject.transform.localScale  = Vector3.one;

		GetJiBaiAward mGetJiBaiAward = tempObject.GetComponent<GetJiBaiAward>();

		mGetJiBaiAward.m_OneKeyAward = OneKeyAward;
       JiBaoBtn.GetComponent<Collider>().enabled = true;
       OneKeyJiBaoBtn.GetComponent<Collider>().enabled = true;
       mGetJiBaiAward.Init ();
	}
	public void InitData(ExploreResp m_ExploreResp)
	{

		int addTime = NewAlliancemanager.Instance().ZongmiaoLev;

		if(g_mExploreResp.info == null)
		{
			//Debug.Log("g_mExploreResp.info = null");
		}

		JiBaiCostPreTime.text = g_mExploreResp.info.money.ToString () + "贡献/次";
		CostPreTime = g_mExploreResp.info.money;
		mAddProperty1.text = LanguageTemplate.GetText(LanguageTemplate.Text.RULL2)+addTime.ToString();
		if(addTime >= 10)
		{
			mAddProperty2.gameObject.SetActive(false);
		}
		else
		{
			mAddProperty2.text = LanguageTemplate.GetText(LanguageTemplate.Text.RULL3)+(addTime+1).ToString();
		}
		string Rul = LanguageTemplate.GetText(LanguageTemplate.Text.RULL1);
		string []s = Rul.Split(':');
		string mRul = "";
		for (int i = 0; i < s.Length; i++)
		{
			mRul += s[i]+"\n\r";
			
		}
		mRuls.text = mRul;	

		for (int i = 0; i < m_ExploreResp.awardsList.Count; i++)
		{
			//Debug.Log ("_ExploreResp.awardsList[i] = "+m_ExploreResp.awardsList[i].itemNumber);
			mJiBaiList[i].mAwardinfo =  m_ExploreResp.awardsList[i];
			mJiBaiList[i].Init();
		}
		ShowTime ();
	}
	public void ShowTime()
	{
		Debug.Log ("g_mExploreResp.info.remainFreeCount = "+g_mExploreResp.info.remainFreeCount);
		if(g_mExploreResp.info.remainFreeCount <= 0)
		{
			int ChouJiangid1 = 600900;
			int ChouJiangid2 = 600905;
			PushAndNotificationHelper.SetRedSpotNotification(ChouJiangid1,false);
			PushAndNotificationHelper.SetRedSpotNotification(ChouJiangid2,false);
		}
		NewAlliancemanager.Instance().Refreshtification ();
		RemainTimes.text = "剩余次数："+g_mExploreResp.info.remainFreeCount.ToString () + "/" + g_mExploreResp.info.cd.ToString ();
		AllTime = g_mExploreResp.info.remainFreeCount;
		mGongXian.text = NewAlliancemanager.Instance().m_allianceHaveRes.contribution.ToString ();
		if(AllTime <= 0)
		{
			JiBaoBtn.SetActive(false);
			OneKeyJiBaoBtn.SetActive(false);
		}
		else
		{
			JiBaoBtn.SetActive(true);
			OneKeyJiBaoBtn.SetActive(true);
		}
	}
	private bool JiBaoTime;
	public void JIBai()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}
		if(CostPreTime <= NewAlliancemanager.Instance().m_allianceHaveRes.contribution && AllTime > 0)
		{
			g_mExploreResp.info.remainFreeCount-=1;
			NewAlliancemanager.Instance().m_allianceHaveRes.contribution -= CostPreTime;
			JiBaoTime = true;
			UIYindao.m_UIYindao.CloseUI();
			JiBaoBtn.GetComponent<UIButton>().enabled = false;
            JiBaoBtn.GetComponent<Collider>().enabled = false;
            OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = false;
			DengDaiYaoJiang.SetActive(true);
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_1);
			ShowTime();
		}
		else
		{
			Global.CreateFunctionIcon(701);
		}

	}
	void LackOfBuildLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"贡献值不足，无法进行祭拜！" ;//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null);
	}
	public void OneKeyJIBai()
	{
//		Debug.Log ("CostPreTime*AllTime = "+CostPreTime*AllTime);
//		Debug.Log ("AllTime = "+AllTime);
//		Debug.Log ("CostPreTime = "+CostPreTime);
		if(CostPreTime*AllTime <= NewAlliancemanager.Instance().m_allianceHaveRes.contribution)
		{
			JiBaoTime = false;
			NewAlliancemanager.Instance().m_allianceHaveRes.contribution -= CostPreTime*AllTime;
			g_mExploreResp.info.remainFreeCount-=AllTime;
			JiBaoBtn.GetComponent<UIButton>().enabled = false;
			OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = false;
            OneKeyJiBaoBtn.GetComponent<Collider>().enabled = false;

            DengDaiYaoJiang.SetActive(true);
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_N);
			ShowTime();
		}
		else
		{
			Global.CreateFunctionIcon(701);
		}
	}
	public void BuyTiLi()
	{
		
	}
	public void BuyTongBi()
	{
		
	}
	public void BuyYuanBao()
	{
		
	}
	public void Close()
	{
		NewAlliancemanager.Instance().BackToThis (this.gameObject);
	}
}
