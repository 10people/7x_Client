using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LeaderSetting : MonoBehaviour, SocketProcessor {

	public AllianceHaveResp m_UnionInfo;
	public LookMembersResp membersResp;

	public GameObject Open_ZhaoMu;//开启招募按钮
	public GameObject Close_ZhaoMu;//关闭招募按钮

	private string jieSanTitleStr;
	private string closeTitleStr;
	private string confirmStr;
	private string cancelStr;

	GameObject OpenRecruit;

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);
	}

	void Start ()
	{
		jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		closeTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_TITLE);
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}

	//获得联盟信息
	public void InItSetting ()
	{
		//Debug.Log ("isOpen:" + m_UnionInfo.isAllow);
		int isOpen = m_UnionInfo.isAllow;// 0为关闭  1为开启

		if(isOpen == 0)
		{
			Open_ZhaoMu.SetActive(true);
			Close_ZhaoMu.SetActive(false);
		}

		else
		{
			Open_ZhaoMu.SetActive(false);
			Close_ZhaoMu.SetActive(true);
		}
	}

	//入盟申请
	public void ApplyAlliance ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_APPLY ),
		                        AllianceApplyLoadCallback );
	}
	
	//入盟申请异步加载回调
	public void AllianceApplyLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject applyObj = Instantiate( p_object ) as GameObject;
		
		applyObj.name = "ApplyAlliance";
		applyObj.transform.parent = this.gameObject.transform.parent;
		applyObj.transform.localPosition = Vector3.zero;
		applyObj.transform.localScale = Vector3.one;
		
		AllianceApplicationData apply = applyObj.GetComponent<AllianceApplicationData> ();
		apply.GetAllianceInfo (m_UnionInfo);
	}

	//转让联盟
	public void TransflateUnion()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_TRANS ),
		                        AllianceTransLoadCallback );
	}

	//转让联盟异步加载回调
	public void AllianceTransLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject transAlliance = Instantiate( p_object ) as GameObject;
		
		transAlliance.name = "TransAlliance";
		transAlliance.transform.parent = this.gameObject.transform.parent;
		transAlliance.transform.localPosition = Vector3.zero;
		transAlliance.transform.localScale = Vector3.one;
		
		TransAlliance trans = transAlliance.GetComponent<TransAlliance> ();
		trans.GetOwnLianMeng (m_UnionInfo,membersResp);
	}

	//解散联盟
	public void DisMissUnion()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        AllianceTransTipsLoadCallback1 );
	}

	//解散联盟提示异步加载回调
	public void AllianceTransTipsLoadCallback1 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR2);

		uibox.setBox(jieSanTitleStr,MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,cancelStr,confirmStr,DisAlliance);
	}

	void DisAlliance (int i)
	{
		if(i == 2)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        AllianceTransTipsLoadCallback2 );
		}
	}

	//解散联盟再次提示异步加载回调
	public void AllianceTransTipsLoadCallback2 ( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR3);
		string str2 = m_UnionInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR4);

		string sanSiStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SANSI);
		string jieSanStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN);

		uibox.setBox(jieSanTitleStr,MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,sanSiStr,jieSanStr,DisAllianceReq);
	}

	//发送解散联盟请求
	void DisAllianceReq (int i)
	{
		if(i == 2)
		{
			DismissAlliance disAllianceReq = new DismissAlliance ();
			
			disAllianceReq.id = m_UnionInfo.id;
			
			MemoryStream dis_stream = new MemoryStream ();
			
			QiXiongSerializer disQx = new QiXiongSerializer ();
			
			disQx.Serialize (dis_stream,disAllianceReq);
			
			byte[] t_protof = dis_stream.ToArray();;
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.DISMISS_ALLIANCE,ref t_protof,"30132");
			//Debug.Log ("jiesanReq:" + ProtoIndexes.DISMISS_ALLIANCE);
		}
	}

	//开启招募
	public void OpenZhaomu ()
	{
		if(OpenRecruit)
		{
			return;
		}

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_RECRUIT ),
		                        RecruitLoadCallback );
	}

	//联盟招募异步加载回调
	public void RecruitLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		OpenRecruit = Instantiate( p_object ) as GameObject;
		OpenRecruit.transform.parent = this.transform.parent;
		OpenRecruit.transform.localScale = Vector3.one;
		OpenRecruit.transform.localPosition = Vector3.zero;
		ReCruit mReCruit = OpenRecruit.GetComponent<ReCruit>();
		//mReCruit.Z_UnionInfo = m_UnionInfo;
		mReCruit.init ();
	}

	//关闭招募
	public void CloseZhaomu ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        CloseRecruitLoadCallback );
	}

	//关闭招募提示异步加载回调
	public void CloseRecruitLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_ASKSTR1);
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_ASKSTR2);

		uibox.setBox(closeTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,cancelStr,confirmStr,SendCloseZm);
	}
	
	void SendCloseZm(int i)
	{
		if (i == 2)
		{
			CloseApply closeReq = new CloseApply ();
			
			closeReq.id = m_UnionInfo.id;
			
			MemoryStream closeStream = new MemoryStream ();
			
			QiXiongSerializer closeQx = new QiXiongSerializer ();
			
			closeQx.Serialize (closeStream, closeReq);
			
			byte[] t_protof = closeStream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.CLOSE_APPLY, ref t_protof, "30136");
		}
	}


	//接收Union求返回的数据
	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.DISMISS_ALLIANCE_OK://无法判断是否解散成功
			{
				Debug.Log ("jiesan:" + ProtoIndexes.DISMISS_ALLIANCE_OK);
				//去掉商铺联盟相关红点
				PushAndNotificationHelper.SetRedSpotNotification (600700,false);//贡献商铺
				PushAndNotificationHelper.SetRedSpotNotification (903,false);//荒野商店

				PushAndNotificationHelper.SetRedSpotNotification (410000,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (410010,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (600500,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (600600,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (600750,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (600850,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (300200,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (300300,false);//荒野商店
				PushAndNotificationHelper.SetRedSpotNotification (400018,false);//荒野商店

				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        DisAllianceLoadCallback );
		 		return true;
			}
				
			case ProtoIndexes.CLOSE_APPLY_OK://关闭招募信息返回
			{
			//	Debug.Log ("guanbi:" + ProtoIndexes.CLOSE_APPLY_OK);
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        CloseRecuritLoadCallback );

				SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);

				m_UnionInfo.isAllow = 0;

				InItSetting ();

				GameObject My_Union = GameObject.Find("My_Union(Clone)");
				if(My_Union)
				{
					MyAllianceMain mLeaderSetting = My_Union.GetComponent<MyAllianceMain>();
					mLeaderSetting.g_UnionInfo.isAllow = 0;
				}

				return true;
			}

			default: return false;
			}
		}
		
		return false;
	}

	//联盟解散成功异步加载回调
	public void DisAllianceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);

		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
        //PlayerModelController.m_playerModelController.m_isCanUpdatePosition = false;
        //Vector3 vec_pos = new Vector3(-1.0f, 0.4f, -63.0f);
        //PlayerModelController.m_playerModelController.UploadPlayerPosition(vec_pos);
        CityGlobalData.m_isMainScene = true;
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR1);
		string str2 = m_UnionInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR2);

		uibox.setBox(jieSanTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,DisAllianceSuccessBack);

		GameObject uirot = GameObject.Find("My_Union(Clone)");
		
		if(uirot)
		{
			//			if (Application.loadedLevelName != "MainCity")
			//			{
			//				SceneManager.EnterMainCity();
			//			}
			
			Destroy(uirot);
		}
	}

	void DisAllianceSuccessBack (int i)
	{
        AllianceData.Instance.IsAllianceNotExist = true;
        QXChatUIBox.chatUIBox.SetSituationState();
        //JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
        //CityGlobalData.m_isAllianceScene = false;
        //CityGlobalData.m_isMainScene = true;

        JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;//new add
        // SceneManager.EnterMainCity();
    }

	//关闭招募返回异步加载回调
	public void CloseRecuritLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{	
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_SUCCESS);
		uibox.setBox(closeTitleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null);
	}

	public void CloseUI()
	{
		Destroy (this.gameObject);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
