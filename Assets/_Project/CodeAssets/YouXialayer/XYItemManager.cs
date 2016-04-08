using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class XYItemManager : MonoBehaviour, SocketProcessor{

	public UIGrid mGid;

	public GameObject Item;

	public UIScrollView mScorview;
	public List <YXItem> YXItemList = new List<YXItem>();

	public static XYItemManager mXYItemManager;

	public static XYItemManager initance()
	{
		if(mXYItemManager == null )
		{
			mXYItemManager = (XYItemManager)GameObject.FindObjectOfType(typeof(XYItemManager));
		}
		return mXYItemManager;
	}

	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YOUXIA_INFO_RESP:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaInfoResp tempInfo = new YouXiaInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				InitUI(tempInfo);
				
				return true;
			}

			default: return false;
			}
		}
		return false;
	}
	void InitUI(YouXiaInfoResp mYouXiaInfoResp)
	{
		foreach(YXItem m_YXItem in YXItemList)
		{
			Destroy(m_YXItem.gameObject);
		}
		YXItemList.Clear ();
		//Debug.Log ("mYouXiaInfoResp.youXiaInfos.Count = "+mYouXiaInfoResp.youXiaInfos.Count);
		for (int i = 0; i < mYouXiaInfoResp.youXiaInfos.Count-1; i++) {
			//Debug.Log (" mYouXiaInfoResp.youXiaInfos.id = "+ mYouXiaInfoResp.youXiaInfos[i].id);
		

			for (int j = i+1; j < mYouXiaInfoResp.youXiaInfos.Count; j++)
			{
				YouXiaOpenTimeTemplate mYouxiaOpen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id(mYouXiaInfoResp.youXiaInfos[i].id);
				YouXiaOpenTimeTemplate m_YouxiaOpen = YouXiaOpenTimeTemplate.getYouXiaOpenTimeTemplateby_Id(mYouXiaInfoResp.youXiaInfos[j].id);
				if(mYouxiaOpen.openLevel > m_YouxiaOpen.openLevel)
				{
					YouXiaInfo mYouXiainfo = mYouXiaInfoResp.youXiaInfos[i];
					mYouXiaInfoResp.youXiaInfos[i] = mYouXiaInfoResp.youXiaInfos[j];
					mYouXiaInfoResp.youXiaInfos[j] = mYouXiainfo;
				}
			}

		}
		for(int i = 0 ; i < mYouXiaInfoResp.youXiaInfos.Count; i++)
		{
			GameObject m_UI = Instantiate(Item) as GameObject;
			
			m_UI.SetActive (true);
			
			m_UI.transform.parent = Item.transform.parent;
			
			m_UI.transform.localScale = Vector3.one;
			
			//m_UI.transform.localPosition = new Vector3(-300+Pos_Dis*i,0,0);
			
			YXItem mYXItem = m_UI.GetComponent<YXItem>();
			
			mYXItem.mYouXiaInfo = mYouXiaInfoResp.youXiaInfos[i];

//			Debug.Log("mYouXiaInfoResp.youXiaInfos[i].zuheId = "+mYouXiaInfoResp.youXiaInfos[i].zuheId);
//			Debug.Log("mYouXiaInfoResp.youXiaInfos[i].id = "+mYouXiaInfoResp.youXiaInfos[i].id);
			mYXItem.Init();
			
			YXItemList.Add(mYXItem);
		}
		mGid.repositionNow = true;
//		if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
//		{
//		//	Debug.Log("进入试练二阶界面1");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
//			mScorview.enabled = false;
//			return;
//
//		}
		bool idshow_alert = false;
		foreach(YXItem mXY in YXItemList)
		{
			if(mXY.Art.gameObject.activeInHierarchy)
			{
				idshow_alert = true;
				break;
			}
		}
		if(!idshow_alert)
		{
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
	}
	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
}
