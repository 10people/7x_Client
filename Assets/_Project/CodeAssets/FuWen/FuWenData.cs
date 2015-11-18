using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class FuWenData : Singleton<FuWenData>,SocketProcessor {

	public enum FuWenOperateType
	{
		LOCK = 1,//锁定
		UNLOCK = 2,//解锁
		GENERAL_HECHENG = 3,//普通合成
		YIJIAN_HECHENG = 4,//一键合成
		EQUIP_FUWEN = 5,//装备符文
		REMOVE_FUWEN = 6,//卸下符文
	}
	private FuWenOperateType fuWenOperateType = FuWenOperateType.LOCK;

	public QueryFuwenResp fuWenDataResp;//符文首页信息

	private string failMsg;//失败原因
	private int operateItemId;//符石操作id
	private int heChengLanWeiId;//合成符石所在栏位id

	private GameObject fuWenObj;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// 符文信息请求
	/// </summary>
	public void FuWenDataReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_FUWEN_MAINPAGE_REQ,"8002");

//		Debug.Log ("符文首页请求:" + ProtoIndexes.C_FUWEN_MAINPAGE_REQ);
	}

	/// <summary>
	/// 符石操作请求 // 1-锁定，2-解锁,3-普通合成，4-一键合成,5-装备符文，6-卸下符文
	/// </summary>
	/// <param name="tempType">符石操作类型</param>
	/// <param name="tempFuWen">符石itemid：1,2,3,4,5</param>
	/// <param name="tempLanWeiId">符石lanWeiId：3,5,6</param>
	public void FuWenOperateReq (FuWenOperateType tempType,int tempItemId,int tempLanWeiId)
	{
		fuWenOperateType = tempType;
		operateItemId = tempItemId;
		heChengLanWeiId = tempLanWeiId;

		OperateFuwenReq operateFuWenReq = new OperateFuwenReq ();

		operateFuWenReq.type = (int)tempType;
		Debug.Log ("operateFuWenReq.type:" + operateFuWenReq.type);
		if (operateFuWenReq.type == 5 || operateFuWenReq.type == 6 || operateFuWenReq.type == 3)
		{
			operateFuWenReq.lanweiId = tempLanWeiId;

			if (operateFuWenReq.type != 6)
			{
				operateFuWenReq.itemId = tempItemId;
			}
		}
		else
		{
			operateFuWenReq.itemId = tempItemId;
		}

		QXComData.SendQxProtoMessage (operateFuWenReq,ProtoIndexes.C_FUWEN_OPERAT_REQ,"8004");
		Debug.Log ("符文操作请求:" + ProtoIndexes.C_FUWEN_OPERAT_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_FUWEN_MAINPAGE_RES://符文首页信息返回
			{
				QueryFuwenResp fuWenData = new QueryFuwenResp ();
				fuWenData = QXComData.ReceiveQxProtoMessage (p_message,fuWenData) as QueryFuwenResp;

				if (fuWenData != null)
				{
					if (fuWenData.lanwei == null)
					{
						fuWenData.lanwei = new List<FuwenLanwei>();
					}
					if (fuWenData.attr == null)
					{
						fuWenData.attr = new List<JunzhuAttr>();
					}
					if (fuWenData.fuwens == null)
					{
						fuWenData.fuwens = new List<Fuwen>();
					}

//					Debug.Log ("战力：" + fuWenData.zhanli);
//					Debug.Log ("符文页：" + fuWenData.lanwei.Count);
//					Debug.Log ("属性：" + fuWenData.attr.Count);
//					Debug.Log ("背包符文：" + fuWenData.fuwens.Count);

//					for (int i = 0;i < fuWenData.lanwei.Count;i ++)
//					{
//						Debug.Log ("符文栏位：" + fuWenData.lanwei[i].lanweiId + "||" + fuWenData.lanwei[i].itemId);
//					}
//
//					for (int i = 0;i < fuWenData.fuwens.Count;i ++)
//					{
//						Debug.Log ("符文：" + fuWenData.fuwens[i].itemId);
//					}

					for (int i = 0;i < fuWenData.lanwei.Count;i ++)
					{
						if (fuWenData.lanwei[i].itemId > 0)
						{
							FuShiRedTips (true);
							break;
						}
					}

					fuWenDataResp = fuWenData;

					for (int i = 0;i < fuWenData.lanwei.Count;i ++)
					{
						if (fuWenData.lanwei[i].flag)
						{
							//激活红点提示
							FuShiRedTips (true);
							break;
						}
						else
						{
							//关闭红点提示
							FuShiRedTips (false);
						}
					}

//					GameObject fuWenObj = GameObject.Find ("FuWen(Clone)");
					if (Global.m_isOpenFuWen)
					{
						if (fuWenObj == null)
						{
							Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.FUWEN ),
							                        FuWenObjLoadBack );
						}
						else
						{
							fuWenObj.SetActive (true);
							FuWenMainPage fuWenMainPage = fuWenObj.GetComponent<FuWenMainPage> ();
							fuWenMainPage.InItFuWenPage (fuWenDataResp);
						}
					}
				}

				return true;
			}

			case ProtoIndexes.S_FUWEN_OPERAT_RES:
			{
				FuwenResp fuWenOperate = new FuwenResp ();
				fuWenOperate = QXComData.ReceiveQxProtoMessage (p_message,fuWenOperate) as FuwenResp;

				if (fuWenOperate != null)
				{
					Debug.Log ("操作结果：" + fuWenOperate.result + "//0-成功 1-失败");
					Debug.Log ("战力：" + fuWenOperate.zhanli);

//					GameObject fuWenObj = GameObject.Find ("FuWen(Clone)");

					if (fuWenObj != null)
					{
						FuWenMainPage fuWenMainPage = fuWenObj.GetComponent<FuWenMainPage> ();

						switch (fuWenOperate.result)
						{
						case 0:

							switch (fuWenOperateType)
							{	
							case FuWenOperateType.GENERAL_HECHENG:
								
								fuWenMainPage.CurHeChengItemId = 0;
								fuWenMainPage.SuccessTips (fuWenOperateType);
								fuWenMainPage.ShowHeChengFuShi (operateItemId,heChengLanWeiId == 0 ? true : false);
								if (heChengLanWeiId == 0)
								{
									fuWenMainPage.FxController (FuWenMixBtn.FxType.OPEN);
								}
								else
								{
									fuWenMainPage.FxController (FuWenMixBtn.FxType.CLEAR);
								}
								break;
								
							case FuWenOperateType.YIJIAN_HECHENG:
								
								fuWenMainPage.CurHeChengItemId = 0;
								fuWenMainPage.SuccessTips (fuWenOperateType);
								fuWenMainPage.ShowHeChengFuShi (operateItemId,true);
								fuWenMainPage.FxController (FuWenMixBtn.FxType.OPEN);

								break;
								
							case FuWenOperateType.EQUIP_FUWEN:
								
								fuWenMainPage.CurXiangQianId = 0;
								fuWenMainPage.SuccessTips (fuWenOperateType);

								break;
								
							case FuWenOperateType.REMOVE_FUWEN:
								
								fuWenMainPage.CurXiangQianId = 0;
								if (operateItemId == fuWenMainPage.CurHeChengItemId)
								{
									fuWenMainPage.CurHeChengItemId = 0;
								}

								break;
								
							default:
								break;
							}

							FuWenDataReq ();
							
							break;
							
						case 1:
							
							Debug.Log ("失败原因：" + fuWenOperate.reason);
							failMsg = fuWenOperate.reason;
							
							switch (fuWenOperateType)
							{	
							case FuWenOperateType.GENERAL_HECHENG:
								
								fuWenMainPage.CurHeChengItemId = 0;
								fuWenMainPage.ShowMixBtns ();

								break;
								
							case FuWenOperateType.YIJIAN_HECHENG:
								
								fuWenMainPage.CurHeChengItemId = 0;
								fuWenMainPage.ShowMixBtns ();

								break;
							}
							fuWenMainPage.FxController (FuWenMixBtn.FxType.CLEAR);
							Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
							                        FailLoadBack);
							break;
						}
					}
				}

				return true;
			}
			case ProtoIndexes.S_FUWEN_TIPS://符文红点推送
			{
				Debug.Log ("符石红点提示");

				FuShiRedTips (true);

				return true;
			}
			}
		}

		return false;
	}

	private void FailLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = "提示";
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr,"\n\n" + MyColorData.getColorString (1,failMsg),null,null,confirmStr,null,FailBack);
	}
	void FailBack (int i)
	{
//		GameObject fuWenObj = GameObject.Find ("FuWen(Clone)");
		
		if (fuWenObj != null)
		{
			FuWenMainPage fuWenMainPage = fuWenObj.GetComponent<FuWenMainPage> ();
			fuWenMainPage.EffectPanel (false);
			fuWenMainPage.IsBtnClick = false;
		}
	}

	/// <summary>
	/// 开启符文
	/// </summary>
	public void OpenFuWen ()
	{
		if(FunctionOpenTemp.IsHaveID(500010))
		{
			Global.m_isOpenFuWen = true;
			FuWenDataReq ();
		}
		else
		{
			ZhuXianTemp zhuXianTemp = ZhuXianTemp.getTemplateById (FunctionOpenTemp.GetTemplateById (500010).m_iDoneMissionID);
			ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"请先[dc0600]" + zhuXianTemp.desc + "[-]"));
		}
	}
	
	void FuWenObjLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		fuWenObj = GameObject.Instantiate( p_object ) as GameObject;
		FuWenMainPage fuWenMainPage = fuWenObj.GetComponent<FuWenMainPage> ();
		fuWenMainPage.InItFuWenPage (fuWenDataResp);
	}
	
	/// <summary>
	/// 符石红点提示
	/// </summary>
	void FuShiRedTips (bool isRed)
	{
		if (FunctionOpenTemp.IsHaveID(500010))
		{
			Global.m_isFuWen = isRed;
			if (!isRed)
			{
				if(!(EquipsOfBody.Instance().EquipUnWear() || 
				     EquipsOfBody.Instance().EquipReplace() || 
				     Global.m_isNewChenghao || 
				     Global.m_isTianfuUpCan || BagData.AllUpgrade()))
				{
					MainCityUIRB.SetRedAlert(200, false);
				}
			}
			else
			{
				MainCityUIRB.SetRedAlert(200, true);
			}
		}
	}
	
	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
