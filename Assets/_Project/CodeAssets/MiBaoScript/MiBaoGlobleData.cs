using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class MiBaoGlobleData : MonoBehaviour ,SocketProcessor
{
	private static MiBaoGlobleData m_instance;
	public MibaoInfoResp G_MiBaoInfo;//返回秘宝信息
	public bool MiBaoISNull = true;//判断秘宝是否为空

	public bool MiBaoUp = false;//判断秘宝是否已经升级

	private bool CanLevelUp;
	private bool changeState;
	private int m_Money;

	private int JUNZHULevel;

	private int JunZhuJinBi;

	private int Point;

	public static MiBaoGlobleData Instance()
	{
		if (m_instance == null)
		{
			GameObject t_gameObject = UtilityTool.GetDontDestroyOnLoadGameObject();;
			
			m_instance = t_gameObject.AddComponent<MiBaoGlobleData>();
		}
		
		return m_instance;
	}



	void Awake()
	{
		changeState = true;
		SocketTool.RegisterMessageProcessor(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start()
	{
	
		SendMiBaoIfoMessage();
	}
	//发送秘宝请求
	public static void SendMiBaoIfoMessage()
	{
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
	}
	void Update()
	{
//		{
//			TimeHelper.SignetTime();
//
//			int t_count = 0;
//
//			for( int i = 0; i < 100; i++ ){
//				t_count += i;
//			}
//		}
//
//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "After 100+." );
//
//			TimeHelper.SignetTime();
//		}

		if(CityGlobalData.PveLevel_UI_is_OPen &&changeState)
		{
			changeState = false;
			StopCoroutine("ChangePveOpenDate");
			StartCoroutine("ChangePveOpenDate");
		}
		if(MiBaoDataBack)
		{
			if(JUNZHULevel != JunZhuData.Instance ().m_junzhuInfo.level || JunZhuJinBi != JunZhuData.Instance ().m_junzhuInfo.jinBi ||Point != G_MiBaoInfo.levelPoint)
			{
				JUNZHULevel = JunZhuData.Instance ().m_junzhuInfo.level;
				
				JunZhuJinBi = JunZhuData.Instance ().m_junzhuInfo.jinBi;
				if(G_MiBaoInfo.levelPoint != null)
				{
					Point = G_MiBaoInfo.levelPoint;
				}
				
				ShowMiBaoCanLevelUp ();
			}
		}
//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "After Coroutines." );
//
//			TimeHelper.SignetTime();
//		}

//		ShowMiBaoCanLevelUp ();
	}
	IEnumerator ChangePveOpenDate()
	{
//		Debug.Log ("ChangePveOpenDate ");
		yield return new WaitForSeconds (2.0f);
		changeState = true;
		CityGlobalData.PveLevel_UI_is_OPen = false;
	}
	bool MiBaoDataBack = false;
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_MIBAO_INFO_RESP://秘宝信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
				
				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());
				
				G_MiBaoInfo = MiBaoInfo;

//				Debug.Log("G_MiBaoInfo.count = " +G_MiBaoInfo.mibaoGroup.Count);

		  		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (6);
		        CanLevelUp = false;
				MiBaoDataBack = true;
				int index = functionTemp.m_iID;
				int ActiveMiBaonum = 0;
				for(int i = 0 ; i < MiBaoInfo.mibaoGroup.Count; i ++)
				{
					ActiveMiBaonum = 0;
					for(int j = 0 ; j < MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j ++)
					{
						if(MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level > 0&&!MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
						{
							if(MiBaoInfo.mibaoGroup[i].mibaoInfo[j].suiPianNum >= MiBaoInfo.mibaoGroup[i].mibaoInfo[j].needSuipianNum && MiBaoInfo.mibaoGroup[i].mibaoInfo[j].star < 5)
							{
								// Removed By YuGu, red alert auto updated by PushHelper.
//								MainCityUIRB.SetRedAlert(index,true);

//								Debug.Log("Up  star");

								PushAndNotificationHelper.SetRedSpotNotification (602, true);
								return true;
								//break;
							}
							ActiveMiBaonum += 1;

						}
						else 
						{
							MiBaoSuipianXMltemp mMiBaosuipian = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid(MiBaoInfo.mibaoGroup[i].mibaoInfo[j].tempId);

							if(MiBaoInfo.mibaoGroup[i].mibaoInfo[j].suiPianNum >= mMiBaosuipian.hechengNum&&!MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock )
							{
								Debug.Log("Hecheng");
								PushAndNotificationHelper.SetRedSpotNotification (605, true);

								// Removed By YuGu, red alert auto updated by PushHelper.
//								MainCityUIRB.SetRedAlert(index,true);

								return true;
								//break;
							}
			
						}

						if(ActiveMiBaonum >= 2&&MiBaoInfo.mibaoGroup[i].hasActive ==0)
						{
							Debug.Log("Up  star2");
							PushAndNotificationHelper.SetRedSpotNotification (610, true);
						}
						if(ActiveMiBaonum > 2&&MiBaoInfo.mibaoGroup[i].hasActive ==1)
						{

							if(MiBaoInfo.mibaoGroup[i].hasJinjie == 0)
							{
								Debug.Log("Up  star3");
								PushAndNotificationHelper.SetRedSpotNotification (610, true);
							}

						}
//
//						int lv = MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level;
//						if(lv == 0)
//						{
//							lv = 1;
//						}
//						MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(MiBaoInfo.mibaoGroup[i].mibaoInfo[j].miBaoId);
//						
//						ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId, lv);
//						
//						if(G_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
//						   lv < JunZhuData.Instance().m_junzhuInfo.level&&lv < 100&&MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level >=1 &&!MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
//						{
//
//							CanLevelUp = true;// 有升级点数可以升级秘宝
//						}

					}

				}

			//	CantUpMiBao();
				// Removed By YuGu, red alert auto updated by PushHelper.
//				MainCityUIRB.SetRedAlert(index,false);

				return true;
			}
				
			default: return false;
			}
			
		}
		
		else
		{
			Debug.Log("p_message == null");
		}
		
		return false;
	}

	public void ShowMiBaoCanLevelUp()
	{
		CanLevelUp = false;

		if( G_MiBaoInfo ==null || G_MiBaoInfo.mibaoGroup == null || G_MiBaoInfo.mibaoGroup.Count == 0)
		{
			return;
		}
		for (int i = 0; i < G_MiBaoInfo.mibaoGroup.Count; i ++) {
		
			for(int j = 0 ; j < G_MiBaoInfo.mibaoGroup[i].mibaoInfo.Count; j ++)
			{
				int lv = G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level;
				if(lv == 0)
				{
					lv = 1;
				}
				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].miBaoId);
				
				ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId, lv);

				if(G_MiBaoInfo.levelPoint > 0 && JunZhuData.Instance().m_junzhuInfo.jinBi >= mExpXxmlTemp.needExp &&
				   lv < JunZhuData.Instance().m_junzhuInfo.level&&lv < 100&&G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].level >=1 &&!G_MiBaoInfo.mibaoGroup[i].mibaoInfo[j].isLock)
				{
					
					CanLevelUp = true;// 有升级点数可以升级秘宝
					break;
				}
			}
		}

		{
			TimeHelper.LogDeltaTimeSinceSignet( "After 2 for." );
			
			TimeHelper.SignetTime();
		}

		CantUpMiBao ();

//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "After CantUpMiBao." );
//			
//			TimeHelper.SignetTime();
//		}
	}
	void  CantUpMiBao()
	{
		// TODO
		// 秘宝升级全部完成后调用

	//Debug.Log ("CanLevelUp = " +CanLevelUp);

		if(!CanLevelUp)
		{
			if(PushAndNotificationHelper.IsShowRedSpotNotification(600))
			{
				PushAndNotificationHelper.SetRedSpotNotification (600, false);
			}
		}
		else
		{
			if(!PushAndNotificationHelper.IsShowRedSpotNotification(600))
			{
				PushAndNotificationHelper.SetRedSpotNotification (600, true);
			}
		}
	}
	public bool GetMiBaoskillOpen()
	{
	
		for (int i = 0; i < G_MiBaoInfo.mibaoGroup.Count; i ++)
		{
			if(G_MiBaoInfo.mibaoGroup[i].hasActive ==1)
			{
				return true;
			}
		}
		return false;
	}
	public bool GetEnterChangeMiBaoSkill_Oder()
	{

		if(!GetMiBaoskillOpen ())
		{
			//Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),CantOpenSkillUI);
		
			return false;
		}
		return true;
	}
	void CantOpenSkillUI(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "\r\n"+"对不起，您当前没有可使用的秘技，"+"\r\n"+"激活同属一组的两个秘宝可以激活对应的秘技。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,null,null,null);
	}

	public MibaoInfo getMibao(int id)
	{
		for(int q = 0; q < MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup.Count; q ++)
		{
			for(int p = 0; p < MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo.Count; p++)
			{
				if(MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo[p].miBaoId == id)
				{
					return MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup[q].mibaoInfo[p];
				}
			}
		}
		return null;
	}

	public string getStart(int id)
	{
		string returnStart;
		MibaoInfo temp = getMibao(id);
		if(temp == null)
		{
			return "";
		}
		switch(temp.star)
		{
		case 1:
			returnStart = "pinzhi3";
			break;
		case 2:
			returnStart = "pinzhi6";
			break;
		case 3:
			returnStart = "pinzhi9";
			break;
		case 4:
			returnStart = "pinzhi9";
			break;
		case 5:
			returnStart = "pinzhi9";
			break;
		default:
			returnStart = "";
			break;
		}
		return returnStart;
	}
}
