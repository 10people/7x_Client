using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BaiZhanExchange : MonoBehaviour,SocketProcessor {

	public static BaiZhanExchange exchange;

	public ExchageAwardResp exchangeInfo; 

	private int reqType;
	
//	private int refreshCdTime;//刷新物品cd时间
	[HideInInspector]public int hour;//时
	[HideInInspector]public int minute;//分
	[HideInInspector]public int second;//秒

	private string titleStr;
	private string str;
	private string confirmStr;
	private string cancelStr;

	void Awake () 
	{	
		exchange = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

//		DuiHuanReq (0);
	}

	//兑换请求
	public void DuiHuanReq (int type)
	{
		ExchageAwardReq exchangeReq = new ExchageAwardReq();
		
		exchangeReq.type = type;

		reqType = type;

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,exchangeReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.EXCHAGE_AWARD_REQ,ref t_protof,"27004");
//		Debug.Log ("baiZhanDuiHuanReq:" + ProtoIndexes.EXCHAGE_AWARD_REQ);
	}
	
	public bool OnProcessSocketMessage (QXBuffer p_message) 
	{	
		if (p_message != null) {
			
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.EXCHAGE_AWARD_RESP://兑换页面返回
//				Debug.Log ("baiZhanDuiHuanRes:" + ProtoIndexes.EXCHAGE_AWARD_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ExchageAwardResp duiHuanInfo = new ExchageAwardResp();
				
				t_qx.Deserialize(t_stream, duiHuanInfo, duiHuanInfo.GetType());
				
				if (duiHuanInfo != null)
				{
//					Debug.Log ("duiHuanInfo:" + duiHuanInfo);

//					if (refreshCdTime == 0)
//					{
//						refreshCdTime = duiHuanInfo.leftTime;
//						StartCoroutine (RefreshTimeCount ());
//					}

					if (duiHuanInfo.msg == 1)
					{
						Debug.Log ("刷新成功!");
						Debug.Log ("RefreshTime:" + duiHuanInfo.leftTime);
						Debug.Log ("WeiWang:" + duiHuanInfo.weiWang);
						Debug.Log ("List:" + duiHuanInfo.duiHuanList.Count);
						Debug.Log ("needYB:" + duiHuanInfo.needYB);

						exchangeInfo = duiHuanInfo;
						GeneralControl.Instance.LoadStorePrefab (GeneralControl.StoreType.PVP,"威望商店",duiHuanInfo.duiHuanList,duiHuanInfo.leftTime,
						                                         duiHuanInfo.weiWang,duiHuanInfo.needYB);
						if (reqType == 1)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ExchangeLoadBack1 );
						}

//						GameObject exchangeWin = GameObject.Find ("BaiZhanExchange");
//						if (exchangeWin != null)
//						{
////							ExchangeManager exchangeMan = exchangeWin.GetComponent<ExchangeManager> ();
////							exchangeMan.exchangeResp = exchangeInfo;
////							exchangeMan.InItExchangePage ();
//
//							if (reqType == 1)
//							{
//								Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
//								                        ExchangeLoadBack1 );
//							}
//						}
					}

					else
					{
						if (reqType == 1)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ExchangeLoadBack2 );
						}
					}
				}

				return true;
			}
		}
		
		return false;
	}

	//刷新成功
	void ExchangeLoadBack1 (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_REFRESH_SUCCESS_TITLE);
		
		str = "\n\n" + LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_REFRESH_SUCCESS);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null, null,confirmStr,null,null);
	}

	//威望不足
	void ExchangeLoadBack2 (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.WEIWANG_NOT_ENOUGH_TITLE);
		
		str = "\n\n" + LanguageTemplate.GetText (LanguageTemplate.Text.WEIWANG_NOT_ENOUGU);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,null);
	} 

	//刷新物品cd
//	IEnumerator RefreshTimeCount ()
//	{
//		while (refreshCdTime > 0) 
//		{	
//			refreshCdTime --;
//
//			hour = refreshCdTime / 3600;
//			
//			minute = (refreshCdTime /60 ) % 60;
//			
//			second = refreshCdTime % 60;
//
//			if (refreshCdTime == 0) 
//			{	
//				BaiZhanExchange.exchange.DuiHuanReq (0);
//			}
//			
//			yield return new WaitForSeconds(1);
//		}
//	}

	//重置按钮
	public void ResetBtn ()
	{
		if (BaiZhanMainPage.baiZhanMianPage.IsYinDaoStop)
		{
			return;
		}
		if (!BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent)
		{
			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = true;
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ResetCallback );
		}
	}
	void ResetCallback (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_TITLE);
		
		if (JunZhuData.Instance().m_junzhuInfo.vipLv >= BaiZhanMainPage.baiZhanMianPage.baiZhanResp.canCleanCDvipLev)
		{
			str = "\n" + LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_USE_YUANBAO_ASKSTR1) 
				+ BaiZhanMainPage.baiZhanMianPage.baiZhanResp.cdYuanBao + LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_USE_YUANBAO_ASKSTR2);
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,
			             null,cancelStr,confirmStr,ResetReq);
		}
		
		else
		{
			str = "\n\n达到VIP" + BaiZhanMainPage.baiZhanMianPage.baiZhanResp.canCleanCDvipLev + "级可清除冷却！";
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,
			             null,confirmStr,null,ResetBack);
		}
	}
	void ResetReq (int i)
	{
		BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
		if (i == 2)
		{
			ConfirmManager.confirm.ConfirmReq (1,null,0);
		}
	}
	void ResetBack (int i)
	{
		BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = false;
	}

	//兑换商店按钮
	public void HuiHuanBtn ()
	{
		if (BaiZhanMainPage.baiZhanMianPage.IsYinDaoStop)
		{
			return;
		}
		if (!BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent)
		{
			BaiZhanMainPage.baiZhanMianPage.IsOpenOpponent = true;
			BaiZhanMainPage.baiZhanMianPage.ShowChangeSkillEffect (false);
			DuiHuanReq (0);
			//		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_DUI_HUAN ),
			//		                        DuiHuanLoadCallback );
		}
	}

	public void DuiHuanLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject dhPanel = Instantiate( p_object ) as GameObject;
		
		dhPanel.SetActive (true);
		dhPanel.name = "BaiZhanExchange";
		
		dhPanel.transform.parent = this.transform;
		dhPanel.transform.localPosition = Vector3.zero;
		dhPanel.transform.localScale = Vector3.one;
		Debug.Log ("exchangeInfo:" + exchangeInfo);
		ExchangeManager exchangeMan = dhPanel.GetComponent<ExchangeManager> ();
		exchangeMan.exchangeResp = exchangeInfo;
		exchangeMan.InItExchangePage ();
	}

	void OnDestroy () {
		
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
