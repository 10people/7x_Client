using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnterYouXiaBattle : MonoBehaviour,SocketProcessor
{

    public ScaleEffectController m_ScaleEffectController;

	public GameObject YouXiaEnterUITemp;

	private float Pos_Dis = 300;

	public List <SelectYouXiaEntertype> SelectYouXiaEntertypeList = new List<SelectYouXiaEntertype>();

	public static EnterYouXiaBattle GlobleEnterYouXiaBattle;
	[HideInInspector]public int m_Time;
	void Awake()
	{ 
		GlobleEnterYouXiaBattle = this;

		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_YOUXIA_INFO_REQ);
	}
	
	public  bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index){

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
		foreach(SelectYouXiaEntertype m_SelectYouXiaEntertype in SelectYouXiaEntertypeList)
		{
			Destroy(m_SelectYouXiaEntertype.gameObject);
		}
		SelectYouXiaEntertypeList.Clear ();

		 m_Time = 0;
		for(int i = 0 ; i < mYouXiaInfoResp.youXiaInfos.Count; i++)
		{
			GameObject m_UI = Instantiate(YouXiaEnterUITemp) as GameObject;

			m_UI.SetActive (true);

			m_UI.transform.parent = YouXiaEnterUITemp.transform.parent;

			m_UI.transform.localScale = Vector3.one;

			m_UI.transform.localPosition = new Vector3(-300+Pos_Dis*i,0,0);

			SelectYouXiaEntertype mSelectYouXiaEntertype = m_UI.GetComponent<SelectYouXiaEntertype>();

			mSelectYouXiaEntertype.mYouXiaInfo = mYouXiaInfoResp.youXiaInfos[i];

			mSelectYouXiaEntertype.Init();

			SelectYouXiaEntertypeList.Add(mSelectYouXiaEntertype);

			if(mYouXiaInfoResp.youXiaInfos[i].open &&mYouXiaInfoResp.youXiaInfos[i].remainColdTime <=0 )
			{
				m_Time += mYouXiaInfoResp.youXiaInfos[i].remainTimes;
			}

		}
		if(m_Time == 0)
		{
			PushAndNotificationHelper.SetRedSpotNotification( 305, false );
		}

//		PushAndNotificationHelper.IsShowRedSpotNotification( 305 )
	}
	public void Closed()
	{
	    m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
        m_ScaleEffectController.OnCloseWindowClick();
	}

    void DoCloseWindow()
    {
        MainCityUI.TryRemoveFromObjectList(gameObject);
        Destroy(this.gameObject);
    }
	public GameObject NeedCloseObg;
	public void ShowOrClose()
	{
		Debug.Log ("NeedCloseObg = " +NeedCloseObg);
		if(NeedCloseObg == null)
		{
			return;
		}
		if(NeedCloseObg.activeInHierarchy)
		{
			NeedCloseObg.SetActive(false);
		}
		else
		{
			NeedCloseObg.SetActive(true);
		}
	}
	public GameObject SecendNeedCloseObg;

	public void SecondShowOrClose()
	{
		Debug.Log ("SecendNeedCloseObg = " +SecendNeedCloseObg);
		if(SecendNeedCloseObg == null)
		{
			return;
		}
		if(SecendNeedCloseObg.activeInHierarchy)
		{
			SecendNeedCloseObg.SetActive(false);
		}
		else
		{
			SecendNeedCloseObg.SetActive(true);
		}
	}
	public GameObject ThirdNeedCloseObg;
	
	public void ThirdShowOrClose()
	{
		Debug.Log ("ThirdNeedCloseObg = " +ThirdNeedCloseObg);
		if(ThirdNeedCloseObg == null)
		{
			return;
		}
		if(ThirdNeedCloseObg.activeInHierarchy)
		{
			ThirdNeedCloseObg.SetActive(false);
		}
		else
		{
			ThirdNeedCloseObg.SetActive(true);
		}
	}
}
