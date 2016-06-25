using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class FuWenDress : MonoBehaviour ,SocketListener {

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public  FuwenInBagResp mFuwenInBagResp;

	public  FuwenLanwei mFuwenLanwei;

	private List<GameObject> fuWenBagItemList = new List<GameObject> ();

	public GameObject BagFuWenItem;

	public UIGrid mGrid;

	public int tab;

	public int minlayColor;

	public FuwenResp m_FuwenResp;

	List<FuwenInBag> currfuwenList = new List<FuwenInBag> ();

	public UILabel TitleName;

	public bool XiangQian_TiHuan = true;// true为镶嵌 false 替换

	public delegate void CheckYindao();
	
	private CheckYindao mCheckYindao;

	public UILabel mMind;

	void Awake()
	{
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
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}
	public void BtnManagerMent(GameObject mbutton)
	{
		Debug.Log ("mbutton.name = "+mbutton.name);

		int id = int.Parse(mbutton.name.Substring(7, mbutton.name.Length - 7));

		Debug.Log ("id.id = "+id);

		if(id < 1000)
		{
			DressOprection(id);
		}
		else
		{
			Close();
		}
	}
	void Start () {
	
	}
	private bool YindaoIsopen;
	void YinDaoManager()
	{
		YindaoIsopen = false;
		if(FreshGuide.Instance().IsActive(100470)&& TaskData.Instance.m_TaskInfoDic[100470].progress >= 0)
		{
			Debug.Log("装上符文");
			YindaoIsopen = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100470];
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

	public void Init(CheckYindao m_CheckYindao = null)
	{
		//Debug.Log ("mFuwenLanwei.lanweiId = "+mFuwenLanwei.lanweiId);
		mMind.gameObject.SetActive(false);
		if(m_CheckYindao != null)
		{
			mCheckYindao = m_CheckYindao;
		}
		if(XiangQian_TiHuan)
		{
			TitleName.text = "穿 戴";
			YinDaoManager();
		}else
		{
			TitleName.text = "替 换";
		}

		FuWenOpenTemplate mOPenFunction = FuWenOpenTemplate.GetFuWenOpenTemplateBy_By_Id (mFuwenLanwei.lanweiId);

		currfuwenList.Clear ();
		if(mFuwenInBagResp == null || mFuwenInBagResp.fuwenList == null)
		{
			Debug.Log("背包为空");
			mMind.gameObject.SetActive(true);
			return;
		}

		for(int i = 0; i < mFuwenInBagResp.fuwenList.Count; i++)
		{
			FuWenTemplate mfw = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenInBagResp.fuwenList[i].itemId);

			if(mfw.inlayColor == mOPenFunction.inlayColor)
			{
				//Debug.Log("mFuwenInBagResp.fuwenList[i].itemId = "+mFuwenInBagResp.fuwenList[i].itemId);
				if(XiangQian_TiHuan)
				{
					currfuwenList.Add(mFuwenInBagResp.fuwenList[i]);
				}
				else
				{
					FuWenTemplate LanweiFuWen = FuWenTemplate.GetFuWenTemplateByFuWenId(mFuwenLanwei.itemId);
					if(mfw.color > LanweiFuWen.color)
					{
						currfuwenList.Add(mFuwenInBagResp.fuwenList[i]);
					}
					else if(mfw.color == LanweiFuWen.color)
					{
						if(mfw.fuwenLevel > LanweiFuWen.fuwenLevel)
						{
							currfuwenList.Add(mFuwenInBagResp.fuwenList[i]);
						}
					}
				}
			}
		}


		mMind.gameObject.SetActive(currfuwenList.Count <= 0);

		fuWenBagItemList = QXComData.CreateGameObjectList (BagFuWenItem,mGrid,currfuwenList.Count,fuWenBagItemList);
		for(int i = 0; i < fuWenBagItemList.Count; i++)
		{
			DressItem m_DressItem = fuWenBagItemList[i].GetComponent<DressItem>();
			
			m_DressItem.mFuwenInBag = currfuwenList[i];

			BtnList.Add(m_DressItem.mListener);

			m_DressItem.ButtonName.name = "Button_"+i.ToString();

			m_DressItem.XiangQian = XiangQian_TiHuan;

			m_DressItem.Init();

		}
		AddEventListener ();
	}
	void DressOprection(int index)
	{
		OperateFuwenReq  mOperateFuwenReq  = new OperateFuwenReq  ();
		MemoryStream MiBaoinfoStream = new MemoryStream ();
		QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
		
		mOperateFuwenReq.tab = tab;

		mOperateFuwenReq.action = 5;

		mOperateFuwenReq.lanweiId = mFuwenLanwei.lanweiId;

		mOperateFuwenReq.bagId = currfuwenList[index].bagId;

		MiBaoinfoer.Serialize (MiBaoinfoStream,mOperateFuwenReq);
		
		byte[] t_protof;
		t_protof = MiBaoinfoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_FUWEN_OPERAT_REQ,ref t_protof,ProtoIndexes.S_FUWEN_OPERAT_RES.ToString());
	}
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_FUWEN_OPERAT_RES: //符文镶嵌返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				FuwenResp mFuwenResp  = new FuwenResp();
				
				t_qx.Deserialize(t_stream, mFuwenResp, mFuwenResp.GetType());
				Debug.Log ("符文镶嵌返回 ");
				Debug.Log ("符文替换返回 ");
				m_FuwenResp = mFuwenResp;
				if(m_FuwenResp.result != 0)
				{
					ClientMain.m_UITextManager.createText("只能更换更换品质的符文！");
					Debug.Log("m_FuwenResp.reason = "+m_FuwenResp.reason);
				}
				else
				{
					InitData();
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
	void InitData()
	{
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
		}

		NewFuWenPage.Instance ().Init (NewFuWenPage.Instance ().mQueryFuwen.tab);
//
//		NewFuWenPage.Instance ().GetBagInfo ();

		Close ();
	}
	public void Close()
	{
		if(mCheckYindao != null)
		{
			mCheckYindao();
			mCheckYindao = null;
		}
		Destroy (this.gameObject);
	}
}









