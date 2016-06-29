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

	public GameObject Item;
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
		// reigster trigger delegate
		{
			UIWindowEventTrigger.SetOnTopAgainDelegate( gameObject, ShowSL_Yindao );
		}
	}
	public	void ShowSL_Yindao()
	{
		if(FreshGuide.Instance().IsActive(200020)&& TaskData.Instance.m_TaskInfoDic[200020].progress >= 0)
		{
			Debug.Log("试练界面千重楼首页引导");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200020];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]);
		}
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ, ProtoIndexes.S_YOUXIA_INFO_RESP + "");
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

				EveryDayShowTime.Instance.m_isLoad1 = true;
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
		for(int i = 0 ; i < mYouXiaInfoResp.youXiaInfos.Count+1; i++)
		{
			GameObject m_UI = Instantiate(Item) as GameObject;
			
			m_UI.SetActive (true);
			
			m_UI.transform.parent = Item.transform.parent;
			
			m_UI.transform.localScale = Vector3.one;

			YXItem mYXItem = m_UI.GetComponent<YXItem>();
			if(i < mYouXiaInfoResp.youXiaInfos.Count)
			{
				m_UI.transform.localPosition = new Vector3(100 * (i+1),0,0);
				mYXItem.mYouXiaInfo = mYouXiaInfoResp.youXiaInfos[i];

				mYXItem.Init();
				
				YXItemList.Add(mYXItem);
			}
			else
			{				
				m_UI.transform.localPosition = new Vector3(0,0,0);
				mYXItem.InitQianChongLouBtn();
			}

		}
//		if(FreshGuide.Instance().IsActive(100315)&& TaskData.Instance.m_TaskInfoDic[100315].progress >= 0)
//		{
//		//	Debug.Log("进入试练二阶界面1");
//			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100315];
//			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[1]);
//			mScorview.enabled = false;
//			return;
//
//		}
		int index = 0;
		Debug.Log ("Global.m_sPanelWantRun = "+Global.m_sPanelWantRun);
		if(Global.m_sPanelWantRun != null || Global.m_sPanelWantRun != "")
		{
			switch(Global.m_sPanelWantRun)
			{
			case "SL_JiaoMiePanJun":
				index = 3;
				break;
			case "SL_XiJieQuanGui":
				index = 1;
				break;
			case "SL_WanBiGuiZhao":
				index = 4;
				break;
			case "SI_TaoFaShanZei":
				index = 2;
				break;
			case "SI_ZongHengLiuHe":
				index = 5;
				break;
			case "QianChongLou":
				index = 101;
				break;
			default:
				break;
			}
			Global.m_sPanelWantRun = "";
		}
		bool idshow_alert = false;

		Debug.Log ("index = "+index);

		foreach(YXItem mXY in YXItemList)
		{
			if(mXY.mYouXiaInfo.id == index)
			{
				mXY.Enter();
			}
			else if(index == 101)
			{
				mXY.isQianchonglou = true;
				mXY.Enter();
				index = 0;
			}
			if(mXY.Art.gameObject.activeInHierarchy)
			{
				idshow_alert = true;
			}
		}
		if(!idshow_alert)
		{
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}
		ShowSL_Yindao ();
	}
	public void Close()
	{
		MainCityUI.TryRemoveFromObjectList(this.gameObject);
		Destroy (this.gameObject);
	}
	public void EnterQianChongLou()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.LIEFU),LoadResourceCallback);
	}
	void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{

		GameObject tempOjbect = Instantiate(p_object )as GameObject;

		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = new Vector3 (100,100,0);

	}
}
