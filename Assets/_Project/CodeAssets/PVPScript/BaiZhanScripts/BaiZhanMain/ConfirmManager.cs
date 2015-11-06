using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ConfirmManager : MonoBehaviour,SocketProcessor {

	public static ConfirmManager confirm;

	private ConfirmExecuteResp confirmResp;
	private int confirmType;//请求类型
	private DuiHuanInfo duiHuanInfo;//兑换物品的信息
	private int getWeiWang;//要领取的威望

	private string titleStr;
	private string str1;
	private string str2;
	private string confirmStr;
	private string cancelStr;
	
	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);

		confirm = this;
	}
	
	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}

	/// <summary>
	/// 百战中确定某件事发送请求
	// 0 元宝确定购买挑战次数
	// 1 元宝清除下次挑战的冷却时间，
	// 2 确定兑换（用威望兑换）
	// 4 确定领取生产奖励
	// 5 确定花费元宝更新玩家对手
	/// </summary>
	/// <param name="sendType">Send type.</param>
	public void ConfirmReq (int sendType,DuiHuanInfo tempInfo,int tempWeiWang)
	{
		getWeiWang = tempWeiWang;

		ConfirmExecuteReq confirmReq = new ConfirmExecuteReq();
		
		confirmReq.type = sendType;
		
		confirmType = confirmReq.type;

		if (tempInfo != null)
		{
			duiHuanInfo = tempInfo;
			confirmReq.info = tempInfo.id;
		}

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,confirmReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.CONFIRM_EXECUTE_REQ,ref t_protof,"27016");
		Debug.Log ("baiZhanConfirmReq:" + ProtoIndexes.CONFIRM_EXECUTE_REQ);
	}
	
	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null) {
			
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.CONFIRM_EXECUTE_RESP:
			{
				Debug.Log ("baiZhanConfirmRes:" + ProtoIndexes.CONFIRM_EXECUTE_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ConfirmExecuteResp confirmInfo = new ConfirmExecuteResp();
				
				t_qx.Deserialize(t_stream, confirmInfo, confirmInfo.GetType());
				
				if (confirmInfo != null)
				{
					confirmResp = confirmInfo;
//					Debug.Log ("confirmType：" + confirmType);
//					Debug.Log ("confirmInfo.success：" + confirmInfo.success);
					if (confirmInfo.success == 1)
					{
						switch (confirmType)
						{
						case 0:
							
							Debug.Log ("今日挑战剩余次数：");
							if (confirmInfo.leftTimes != null && confirmInfo.totalTimes != null)
							{
								Debug.Log ("剩余次数：" + confirmInfo.leftTimes);
								Debug.Log ("总次数：" + confirmInfo.totalTimes);
								BaiZhanData.Instance ().BaiZhanReq ();
//								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.pvpInfo.leftTimes = confirmInfo.leftTimes;
//								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.pvpInfo.totalTimes = confirmInfo.totalTimes;   
//								BaiZhanMainPage.baiZhanMianPage.ChallangeRules ();
							}

							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ResourceLoadCallback );

							break;
							
						case 1:

							Debug.Log ("清除冷却成功");
							Debug.Log ("confirmInfo:" + confirmInfo.nextCDYB);
							if (confirmInfo.nextCDYB != null)
							{
								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.cdYuanBao = confirmInfo.nextCDYB;
								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.pvpInfo.time = 0;
								BaiZhanMainPage.baiZhanMianPage.cdTime = 0;
								BaiZhanMainPage.baiZhanMianPage.ChallangeRules ();
							}

							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ResourceLoadCallback );

							break;
							
						case 2:

							Debug.Log ("兑换成功");

//							GameObject exchangeWin = GameObject.Find ("BaiZhanExchange");
//							if (exchangeWin != null)
//							{
//								ExchangeManager exchangeMan = exchangeWin.GetComponent<ExchangeManager> ();
//								exchangeMan.GoodsRefresh (duiHuanInfo);
//								exchangeMan.SendChatMsg ();
//							}

							if (FreshGuide.Instance().IsActive(100200) && TaskData.Instance.m_TaskInfoDic[100200].progress >= 0)
							{
								UIYindao.m_UIYindao.CloseUI();
								Debug.Log ("CloseYinDao100200");
							}

							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ResourceLoadCallback );

							break;
							
//						case 3:
//
//							Debug.Log ("确定可以刷新兑换列表");
//
//							break;
							
						case 4:

							Debug.Log ("领取生产奖励成功");

							BaiZhanMainPage.baiZhanMianPage.baiZhanResp.canGetweiWang = 0;
							BaiZhanMainPage.baiZhanMianPage.baiZhanResp.hasWeiWang += getWeiWang;
							BaiZhanMainPage.baiZhanMianPage.InItMyRank ();
							//关闭按钮特效
							UIYindao.m_UIYindao.setCloseUIEff ();
							BaiZhanMainPage.baiZhanMianPage.GetAwardNumFly (getWeiWang);

							break;
							
						case 5:

							Debug.Log ("换一批对手");

							if (confirmInfo.oppoList != null && confirmInfo.nextHuanYiPiYB != null)
							{
								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.huanYiPiYB = confirmInfo.nextHuanYiPiYB;
								BaiZhanMainPage.baiZhanMianPage.baiZhanResp.oppoList = confirmInfo.oppoList;
								BaiZhanMainPage.baiZhanMianPage.OpponentsInfo ();
							}

							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ResourceLoadCallback );

							break;

						case 6:

							if (BaiZhanUnExpected.unExpected.enterPlace == 1)
							{
								if (confirmInfo.oppoList != null && confirmInfo.junZhuRank != null)
								{
									BaiZhanMainPage.baiZhanMianPage.baiZhanResp.pvpInfo.rank = confirmInfo.junZhuRank;
									BaiZhanMainPage.baiZhanMianPage.baiZhanResp.oppoList = confirmInfo.oppoList;
									BaiZhanMainPage.baiZhanMianPage.InItMyRank ();
									BaiZhanMainPage.baiZhanMianPage.OpponentsInfo ();
								
								}
								Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
								                        ResourceLoadCallback );
							}
							else if (BaiZhanUnExpected.unExpected.enterPlace == 2)
							{
								GameObject tiaoZhanObj = GameObject.Find ("New_ZhengRong(Clone)");
								
								if (tiaoZhanObj)
								{
									Destroy (tiaoZhanObj);
								}
								
								BaiZhanData.Instance ().BaiZhanReq ();

								Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
								                        ResourceLoadCallback );
							}
							else if (BaiZhanUnExpected.unExpected.enterPlace == 0)
							{
								BaiZhanData.Instance ().BaiZhanReq ();
							}
							break;

						case 7:

							if (BaiZhanUnExpected.unExpected.enterPlace == 1)
							{
								if (confirmInfo.oppoList != null)
								{
									BaiZhanMainPage.baiZhanMianPage.baiZhanResp.oppoList = confirmInfo.oppoList;
									BaiZhanMainPage.baiZhanMianPage.OpponentsInfo ();
								}
							}
							else if (BaiZhanUnExpected.unExpected.enterPlace == 2)
							{
								GameObject tiaoZhanObj = GameObject.Find ("New_ZhengRong(Clone)");
								
								if (tiaoZhanObj)
								{
									Destroy (tiaoZhanObj);
								}
								
								BaiZhanData.Instance ().BaiZhanReq ();
							}

							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
							                        ResourceLoadCallback );

							break;

						default:break;
						}
					}

					else
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        ResourceLoadCallback );
					}
				}
				return true;
			}
			case ProtoIndexes.S_ADDRIEND_RESP:
			{
				//Refresh all friend data if receive friend added.
				FriendData.Instance.RequestData();
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), AddFriendSucceedCallBack);

				return true;
			}
			}
		}
		
		return false;
	}

	private string addFriendName;
	public string AddFriendName
	{
		set{addFriendName = value;}
	}

	private long addFriendId;
	public long AddFriendId
	{
		set{addFriendId = value;}
	}

	public void AddFriendSucceedCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		uibox.m_labelDis2.overflowMethod = UILabel.Overflow.ResizeHeight;

		titleStr = "添加成功";

		str1 = "\n\n您已成功添加" + addFriendName + "到好友列表！";

		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), null, 
		             null, confirmStr, null,AddFriendLockBack);
	}
	void AddFriendLockBack (int i)
	{
		BaiZhanMainPage.baiZhanMianPage.RefreshOpponentList (addFriendId);
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		switch (confirmResp.success)
		{
		case 0:
		{
			Debug.Log ("元宝不足");
			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.YUANBAO_LACK_TITLE);

			str1 = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_IS_RECHARGE);
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, cancelStr, confirmStr,LackYuanBao);
			
			break;
		}
		case 1:
		{
			if (confirmType == 0)//元宝确定购买挑战次数
			{
				Debug.Log ("购买挑战次数成功");
				string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_ADDNUM_TITLE);

				str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_TIAOZHAN_ADDNUM_DES);

				uibox.setBox (titleStr,null,MyColorData.getColorString (1,str1),
				              null,confirmStr,null,null);
			}
			
			if (confirmType == 1)//元宝清除下次挑战的冷却时间
			{
				Debug.Log ("清除冷却成功");
				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_TITLE);

				str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DISCD_SUCCESS);
				
				uibox.setBox (titleStr,null,MyColorData.getColorString (1,str1),
				              null,confirmStr,null,ClearSuccess);
			}
			
			else if (confirmType == 2)//确定兑换（用威望兑换）
			{
				Debug.Log ("兑换成功");
				titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DUIHUAN_TITLE);

				str1 = LanguageTemplate.GetText (LanguageTemplate.Text.BAIZHAN_DUIHUAN_SUCCESS);

				uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1),
				             null, confirmStr, null, DuiHuanSuccess);
			}

			else if (confirmType == 5)//确定更换对手
			{
				Debug.Log ("换对手成功");
				titleStr = "更换成功";
				
				str1 = "更换挑战对手成功！";
				
				uibox.setBox (titleStr,null,MyColorData.getColorString (1,str1),
				              null,confirmStr,null,null);
			}

			else if (confirmType == 6 || confirmType == 7)
			{
				Debug.Log ("刷新对手");
				titleStr = "刷新对手";
				
				str1 = "对手已刷新！";
				
				uibox.setBox (titleStr,null,MyColorData.getColorString (1,str1),
				              null,confirmStr,null,RefreshPlayerList);
			}

			break;
		}

		case 2:
		{
			Debug.Log ("购买失败");

			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.BUY_FAIL);

			str1 = LanguageTemplate.GetText (LanguageTemplate.Text.VIP_LEVEL_NOT_ENOUGH);
		
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, confirmStr, null, null);
			
			break;
		}	

		case 3:
		{	
			Debug.Log ("威望不足");

			titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.WEIWANG_NOT_ENOUGH_TITLE);

			str1 = LanguageTemplate.GetText (LanguageTemplate.Text.WEIWANG_NOT_ENOUGU);

			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, confirmStr, null, null);
			
			break;
		}

		case 4:
		{
			Debug.Log ("购买次数用尽");

			titleStr = "购买失败";
			
			str1 = "您今日购买次数已用尽！";
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, confirmStr, null, null);

			break;
		}

		case 5:
		{
			Debug.Log ("vip等级不足，无法清除冷却cd");
			
			titleStr = "清除失败";
			
			str1 = "vip等级不足，无法清除冷却cd！";
			
			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str1), 
			             null, confirmStr, null, null);
			
			break;
		}

		default:break;
		}
	}

	void LackYuanBao (int i)
	{
		if (i == 2)
		{
			//跳转到充值
			TopUpLoadManagerment.m_instance.LoadPrefab(false);
			BaiZhanMainPage.baiZhanMianPage.DestroyBaiZhanRoot ();
		}
	}

	void ClearSuccess (int i)
	{

	}

	void RefreshPlayerList (int i)
	{
		if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
		{
			UIYindao.m_UIYindao.m_isOpenYindao = true;
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}
	}

	void DuiHuanSuccess (int i)
	{
		BaiZhanExchange.exchange.DuiHuanReq (0);
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
