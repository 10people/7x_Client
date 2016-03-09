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
	
	}
	
	public void Init()
	{
		if(FreshGuide.Instance().IsActive(400020)&& TaskData.Instance.m_TaskInfoDic[400020].progress >= 0)
		{
			//			Debug.Log("去寺庙祭拜");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
		}
		DengDaiYaoJiang.SetActive(false);
		JiBaoBtn.GetComponent<UIButton>().enabled = true;
		OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = true;
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_INFO);
	}
	void Update () {

	}
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_LM_CHOU_JIANG_INFO://宗庙返回
			{
				//Debug.Log("宗庙返回");
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
				return true;
			}
			default:return false;
			}
		}
		
		return false;
	}
	public void ChouJiangDataBack(ErrorMessage a_ErrorMessage)
	{
		List<int > Mark = new List<int> ();
		//Debug.Log("a_ErrorMessage.errorDesc = "+a_ErrorMessage.errorDesc);
		string []s = a_ErrorMessage.errorDesc.Split('#');
		Mark.Clear ();
		OneKeyAward.Clear ();
		for (int i = 0; i < s.Length; i++)
		{
			//Debug.Log("s[i] = "+s[i]);
			Mark.Add(int.Parse(s[i]));
		}

		if(JiBaoTime) // 祭拜
		{
			StartCoroutine(ShowEff(Mark,0));
		}else
		{
			for (int i = 0; i < mJiBaiList.Count; i++)
			{
				for (int j = 0; j < Mark.Count; j++)
				{
					if(i+1 == Mark[j])
					{
						mJiBaiList[i].GetDataByChouJiang();
						OneKeyAward.Add(mJiBaiList[i].mAwardinfo);
					}
				}
			}
			StartCoroutine (OpenAwardUI());
		}
		NewAlliancemanager.Instance().Refreshtification ();
	}
	IEnumerator ShowEff(List<int > mMark,int mTimes )
	{
		bool IsStop = false;
//		Debug.Log("mMark.count = "+mMark.Count);
//		Debug.Log("mMark[0] = "+mMark[0]);
		float WaitingTime = 0.2f;
		float AddTime = WaitingTime/(float)mTimes;
		for (int i = 0; i < mJiBaiList.Count; i++)
		{
			mTimes++;

			//Debug.Log("i = "+i);
			if(mTimes < 11)
			{

				yield return new WaitForSeconds (WaitingTime);
				mJiBaiList[i].CHoseShowAnimation();
			}
			else
			{
				//Debug.Log("mTimes = "+mTimes);

				yield return new WaitForSeconds (WaitingTime);

				for (int j = 0; j < mMark.Count; j++)
				{
					if(i == mMark[j])
					{
						IsStop = true;
						mJiBaiList[i].GetDataByChouJiang();
						OneKeyAward.Add(mJiBaiList[i].mAwardinfo);
						StartCoroutine (OpenAwardUI());
						break;
					}
				}
				if(IsStop)
				{break;}
				mJiBaiList[i].CHoseShowAnimation();
			}

		}
		if(JiBaoTime) // 祭拜
		{
			StartCoroutine(ShowEff(mMark,11));
			JiBaoTime = false;
		}

	}
	IEnumerator OpenAwardUI()
	{
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
		//Debug.Log ("g_mExploreResp.info.remainFreeCount = "+g_mExploreResp.info.remainFreeCount);
		if(g_mExploreResp.info.remainFreeCount <= 0)
		{
			int ChouJiangid = 600900;
			PushAndNotificationHelper.SetRedSpotNotification(ChouJiangid,false);
		}
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
		if(CostPreTime <= NewAlliancemanager.Instance().m_allianceHaveRes.contribution)
		{
			g_mExploreResp.info.remainFreeCount-=1;
			NewAlliancemanager.Instance().m_allianceHaveRes.contribution -= CostPreTime;
			JiBaoTime = true;

			JiBaoBtn.GetComponent<UIButton>().enabled = false;
			OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = false;
			DengDaiYaoJiang.SetActive(true);
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_1);
			ShowTime();
		}
		else
		{
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LackOfBuildLoadBack);
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
		if(CostPreTime*AllTime <= NewAlliancemanager.Instance().m_allianceHaveRes.contribution)
		{
			JiBaoTime = false;
			NewAlliancemanager.Instance().m_allianceHaveRes.contribution -= CostPreTime*AllTime;
			g_mExploreResp.info.remainFreeCount-=AllTime;
			JiBaoBtn.GetComponent<UIButton>().enabled = false;
			OneKeyJiBaoBtn.GetComponent<UIButton>().enabled = false;
			DengDaiYaoJiang.SetActive(true);
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_LM_CHOU_JIANG_N);
			ShowTime();
		}
		else
		{
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LackOfBuildLoadBack);
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
