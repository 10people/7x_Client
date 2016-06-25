using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LieFuManagerment : MonoBehaviour ,SocketProcessor {

	public UICamera mCamera;
	public List<UIEventListener> BtnList = new List<UIEventListener>(); 
	
	public string ButnName;

	public LieFuActionInfoResp mLieFuActionInfo;

	public GameObject LieFuItem;

	public UIGrid mgrid;

	public List<LieFuItem> LieFuItemList = new List<LieFuItem>(); 

	public LieFuActionResp m_LieFuAction;

	public delegate void CheckYindao();
	
	private CheckYindao mCheckYindao;

	private bool FirstLieFu = false;
	void Awake()
	{
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, YinDaoManager );
		}
		SocketTool.RegisterMessageProcessor(this);
		AddEventListener ();
		
	}
	public void AddEventListener()
	{
		BtnList.ForEach (item => SetBtnMoth(item));
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
//		if(YindaoIsopen)return;
		switch(mbutton.name)
		{
		case "Button_1":
			LieFuItemList[0].LieFuBtn();
			break;
		case "Button_2":
			LieFuItemList[1].LieFuBtn();
			break;
		case "Button_3":
			LieFuItemList[2].LieFuBtn();
			break;
		case "Button_4":
			LieFuItemList[3].LieFuBtn();
			break;
		case "Sprite":
			Close();
			break;
		default:
			break;
		}
	}

	void Start () 
	{
		YindaoIsopen = false;
		FirstLieFu = true;
	}
	void Update()
	{
		if(!YindaoIsopen)
		{
			YinDaoManager ();
		}

	}
	public void Init(CheckYindao m_CheckYindao = null)
	{
		if(m_CheckYindao != null)
		{
			mCheckYindao = m_CheckYindao;
		}

		SocketTool.Instance().SendSocketMessage(ProtoIndexes.LieFu_Action_Info_Req); // 猎符请求,
	}
	private bool YindaoIsopen;
	void YinDaoManager()
	{
		if(FirstLieFu)
		{
			if(FreshGuide.Instance().IsActive(200060)&& TaskData.Instance.m_TaskInfoDic[200060].progress >= 0)
			{
				Debug.Log("引导猎符按钮");

				YindaoIsopen = true;
				FirstLieFu = false;
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200060];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
				return;
				//StartCoroutine("SetBtnEnble");
			}
		}
		if(FreshGuide.Instance().IsActive(200065)&& TaskData.Instance.m_TaskInfoDic[200065].progress >= 0)
		{
			Debug.Log("第二次引导猎符按钮");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200065];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
			//StartCoroutine("SetBtnEnble");
		}
		else
		{
			if(UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
		}
	}
//	IEnumerator SetBtnEnble()
//	{
//		yield return new WaitForSeconds (0.5f);
//		YindaoIsopen = false;
//	}
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LieFu_Action_Info_Resp: //猎符首页返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LieFuActionInfoResp mLieFuActionInfoResp = new LieFuActionInfoResp();
				
				t_qx.Deserialize(t_stream, mLieFuActionInfoResp, mLieFuActionInfoResp.GetType());

				mLieFuActionInfo = mLieFuActionInfoResp;
		         
				InitUI();

				return true;
			}
			case ProtoIndexes.LieFu_Action_Resp: //猎符操作返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LieFuActionResp mLieFuActionResp = new LieFuActionResp();
				
				t_qx.Deserialize(t_stream, mLieFuActionResp, mLieFuActionResp.GetType());

				m_LieFuAction = mLieFuActionResp;
				Debug.Log("//猎符操作返回"+mLieFuActionResp.result);
				UIYindao.m_UIYindao.CloseUI();
				if(mLieFuActionResp.result == 0)
				{
					for(int i = 0; i < LieFuItemList.Count; i ++)
					{
						if(mLieFuActionResp.type == LieFuItemList[i].mLieFuActionInfo.type)
						{
							LieFuItemList[i].mLieFuActionInfo.state = mLieFuActionResp.typeState;
							LieFuItemList[i].Init();
						}
						if(mLieFuActionResp.nextType == LieFuItemList[i].mLieFuActionInfo.type)
						{
							LieFuItemList[i].mLieFuActionInfo.state = mLieFuActionResp.nextTypeState;
							LieFuItemList[i].Init();
						}
					}
					GetLieFuAward();

					Init();
					if(NewFuWenPage.Instance().RongHeUIisOpen)
					{
						FuWenInfoShow.Instance().Init();
					}

				}
				else if(mLieFuActionResp.result == 1)
				{
					Global.CreateFunctionIcon (501);
				}
				YindaoIsopen = false;
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
	void InitUI()
	{
		if(mLieFuActionInfo == null)
		{
			Debug.Log("mLieFuActionInfo == null");
			return;
		}
		if(LieFuItemList.Count > 0)
		{
			for(int i = 0; i < LieFuItemList.Count; i++)
			{
				LieFuItemList[i].mLieFuActionInfo = mLieFuActionInfo.lieFuActionInfo[i];
				LieFuItemList[i].Init();
			}

		}
		else
		{
			for(int i = 0; i < mLieFuActionInfo.lieFuActionInfo.Count; i++)
			{
				GameObject mLieFuItem = Instantiate(LieFuItem) as GameObject ;
				mLieFuItem.SetActive(true);
				mLieFuItem.transform.parent = LieFuItem.transform.parent;
				mLieFuItem.transform.localScale = Vector3.one;
				LieFuItem m_LieFuItem = mLieFuItem.GetComponent<LieFuItem>();
				m_LieFuItem.mLieFuActionInfo = mLieFuActionInfo.lieFuActionInfo[i];
				m_LieFuItem.Init();
				LieFuItemList.Add(m_LieFuItem);
				if(!BtnList.Contains(m_LieFuItem.mEventListener))
				{
					BtnList.Add(m_LieFuItem.mEventListener);
				}
			}
			AddEventListener ();
			mgrid.repositionNow = true;
		}

	}
	GameObject ShowFuwen;
	void GetLieFuAward()
	{
//		if(ShowFuwen)
//		{
//			Destroy(ShowFuwen);
//		}
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GETFUWEN),LoadShowFuwenCallback);
	}
	void LoadShowFuwenCallback(ref WWW p_www,string p_path, Object p_object)
	{
		
		ShowFuwen = Instantiate(p_object )as GameObject;
		
		ShowFuwen.transform.localScale = Vector3.one;
		
		ShowFuwen.transform.localPosition = new Vector3 (-100,-100,0);
		
		FuWenShow mFuWenShow = ShowFuwen.GetComponent<FuWenShow>();

		mFuWenShow.m_LieFuActionInfo = m_LieFuAction;

		mFuWenShow.Init (null,null,YinDaoManager,mCamera);
	}
	public void Close()
	{
		if(mCheckYindao != null)
		{
			mCheckYindao();
			mCheckYindao = null;
		}
		QXComData.SendQxProtoMessage (ProtoIndexes.C_LOAD_FUWEN_IN_BAG); // 
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
