using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class BiaoJuData : Singleton<BiaoJuData>,SocketProcessor {

	private YabiaoMainInfoResp biaoJuMainResp;

	private GameObject biaoJuPrefab;

	private string textStr;

	private int upHorseTargetLevel;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// Opens the biao ju.
	/// </summary>
	public void OpenBiaoJu ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_INFO_REQ,ProtoIndexes.S_YABIAO_INFO_RESP.ToString ());
		Debug.Log ("镖局首页请求：" + ProtoIndexes.C_YABIAO_INFO_REQ);
	}

	/// <summary>
	/// Opens the biao ju horse.
	/// </summary>
	public void OpenBiaoJuHorse ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_MENU_REQ,ProtoIndexes.S_YABIAO_MENU_RESP.ToString ());
		Debug.Log ("镖局马场请求：" + ProtoIndexes.C_YABIAO_MENU_REQ);
	}

	/// <summary>
	/// Ups the horse req.
	/// </summary>
	/// <param name="targetType">Target type.</param>
	public void UpHorseReq (int targetType)
	{
		upHorseTargetLevel = targetType;
		HorseType upHorse = new HorseType ();
		upHorse.horseType = targetType;
		QXComData.SendQxProtoMessage (upHorse,ProtoIndexes.C_SETHORSE_REQ,ProtoIndexes.S_SETHORSE_RESP.ToString ());
		Debug.Log ("设置马请求：" + ProtoIndexes.C_SETHORSE_REQ);
	}

	/// <summary>
	/// Buies the horse property req.
	/// </summary>
	/// <param name="tempPropId">Temp property identifier.</param>
	public void BuyHorsePropReq (int tempPropId)
	{
		HorsePropReq horsePropReq = new HorsePropReq ();
		horsePropReq.propType = tempPropId;
		QXComData.SendQxProtoMessage (horsePropReq,ProtoIndexes.C_BUYHORSEPROP_REQ,ProtoIndexes.S_BUYHORSEBUFF_RESP.ToString ());
		Debug.Log ("购买马具请求：" + ProtoIndexes.C_BUYHORSEPROP_REQ);
	}

	/// <summary>
	/// Biaos the ju record req.
	/// </summary>
	public void BiaoJuRecordReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_HISTORY_RSQ,ProtoIndexes.S_YABIAO_HISTORY_RESP.ToString ());
		Debug.Log ("劫镖记录请求：" + ProtoIndexes.C_YABIAO_HISTORY_RSQ);
	}

	/// <summary>
	/// Begins the yun biao req.
	/// </summary>
	public void BeginYunBiaoReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_REQ,ProtoIndexes.S_YABIAO_RESP.ToString ());

        SocketHelper.SendQXMessage(ProtoIndexes.C_GETMABIANTYPE_REQ);

        YaBiaoMoreInfoReq temp2 = new YaBiaoMoreInfoReq
        {
            type = 1
        };
        SocketHelper.SendQXMessage(temp2, ProtoIndexes.C_YABIAO_MOREINFO_RSQ);

        Debug.Log ("开始运镖请求：" + ProtoIndexes.C_YABIAO_REQ);
	}

	/// <summary>
	/// Enemies the page req.
	/// </summary>
	public void EnemyPageReq ()
	{
		QXComData.SendQxProtoMessage (ProtoIndexes.C_YABIAO_ENEMY_RSQ,ProtoIndexes.S_YABIAO_ENEMY_RESP.ToString ());
		Debug.Log ("仇人列表请求：" + ProtoIndexes.C_YABIAO_ENEMY_RSQ);
	}

	/// <summary>
	/// Buies the yun biao time req.
	/// </summary>
	public void BuyYunBiaoTimeReq ()
	{
		BuyCountsReq yBTimesReq = new BuyCountsReq();
		yBTimesReq.type = 10;
		CityGlobalData.SetYunBiaoBuyType = yBTimesReq.type;
		
		QXComData.SendQxProtoMessage (yBTimesReq,ProtoIndexes.C_YABIAO_BUY_RSQ,ProtoIndexes.S_YABIAO_BUY_RESP.ToString ());
		Debug.Log ("购买押镖次数请求：" + ProtoIndexes.C_YABIAO_BUY_RSQ);
	}
	
	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YABIAO_INFO_RESP:
			{
				Debug.Log ("镖局首页返回：" + ProtoIndexes.S_YABIAO_INFO_RESP);
				YabiaoMainInfoResp biaoJuResp = new YabiaoMainInfoResp();
				biaoJuResp = QXComData.ReceiveQxProtoMessage (p_message,biaoJuResp) as YabiaoMainInfoResp;

				if (biaoJuResp != null)
				{
					Debug.Log ("押镖活动是否开启：" + biaoJuResp.isOpen);
					Debug.Log ("还剩运镖次数：" + biaoJuResp.yaBiaoCiShu);
					Debug.Log ("已经购买次数：" + biaoJuResp.buyCiShu);
					Debug.Log ("新仇人：" + biaoJuResp.isNew4Enemy);
					Debug.Log ("新记录：" + biaoJuResp.isNew4History);

					biaoJuMainResp = biaoJuResp;

					if (biaoJuResp.isOpen)
					{
						if (biaoJuPrefab == null)
						{
							Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.YUNBIAO_MAIN_PAGE),
							                        BiaoJuLoadCallBack);
						}
						else
						{
							biaoJuPrefab.SetActive (true);
							
							InItBiaoJuPage ();
						}
					}
					else
					{
						textStr = "运镖活动未开启！";
						QXComData.CreateBox (1,textStr,true,null);
					}
				}

				return true;
			}
			case ProtoIndexes.S_YABIAO_MENU_RESP://选马页面信息返回
			{
				Debug.Log ("镖局马场返回：" + ProtoIndexes.S_YABIAO_MENU_RESP);
				YabiaoMenuResp horseRes = new YabiaoMenuResp();	
				horseRes = QXComData.ReceiveQxProtoMessage (p_message,horseRes) as YabiaoMenuResp;

				if (horseRes != null)
				{
					Debug.Log ("马匹类型:" + horseRes.horse);
					Debug.Log ("是否随机过马:" + horseRes.isNewHorse);

					BiaoJuPage.bjPage.GetHorseResp (horseRes);
				}
				return true;
			}
			case ProtoIndexes.S_SETHORSE_RESP:
			{
				Debug.Log ("设置马匹返回：" + ProtoIndexes.S_SETHORSE_RESP);
				SetHorseResult setHorseRes = new SetHorseResult();	
				setHorseRes = QXComData.ReceiveQxProtoMessage (p_message,setHorseRes) as SetHorseResult;
				
				if (setHorseRes != null)
				{
					Debug.Log ("setHorseRes.result:" + setHorseRes.result);//10:成功 20:马匹达到顶级 30:君主押镖信息不存在  40:元宝不足
					switch (setHorseRes.result)
					{
					case 10:

						BiaoJuPage.bjPage.RefreshHorsePage (upHorseTargetLevel);

						break;
					case 20:

						textStr = "已经是最高等级马！";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					case 30:

						textStr = "君主押镖信息不存在！";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					case 40://元宝不足

						BiaoJuPage.bjPage.LackYuanbao ();

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.S_BUYHORSEBUFF_RESP :
			{
				Debug.Log ("购买马具返回：" + ProtoIndexes.S_BUYHORSEBUFF_RESP );
				HorsePropResp horsePropRes = new HorsePropResp();	
				horsePropRes = QXComData.ReceiveQxProtoMessage (p_message,horsePropRes) as HorsePropResp;
				
				if (horsePropRes != null)
				{
					Debug.Log ("horsePropRes.res:" + horsePropRes.res);
					switch (horsePropRes.res)
					{
					case 10:

						BiaoJuPage.bjPage.InItHorseProp (horsePropRes.prop);
						BiaoJuPage.bjPage.OpenHorsePropWindow (gameObject);

						break;
					case 20:

						//购买失败
						textStr = "购买失败！";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					case 30:

						//元宝不足
						BiaoJuPage.bjPage.LackYuanbao ();

						break;
					default:
						break;
					}
				}

				return true;
			}
			case ProtoIndexes.S_YABIAO_HISTORY_RESP:
			{
				Debug.Log ("劫镖记录返回：" + ProtoIndexes.S_YABIAO_HISTORY_RESP );
				YBHistoryResp recordRes = new YBHistoryResp();
				recordRes = QXComData.ReceiveQxProtoMessage (p_message,recordRes) as YBHistoryResp;

				if (recordRes != null)
				{
					if (recordRes.historyList == null)
					{
						recordRes.historyList = new List<YBHistory>();
					}
				}

				BiaoJuPage.bjPage.InItRecordPage (recordRes);

				return true;
			}
			case ProtoIndexes.S_YABIAO_RESP:
			{
				Debug.Log ("开始运镖请求返回：" + ProtoIndexes.S_YABIAO_HISTORY_RESP );
				YabiaoResult yunBiaoRes = new YabiaoResult();
				yunBiaoRes = QXComData.ReceiveQxProtoMessage (p_message,yunBiaoRes) as YabiaoResult;

				if (yunBiaoRes != null)
				{
					switch (yunBiaoRes.result)
					{
					case 10:
						
						Debug.Log ("YunBiao Success!");
						BiaoJuPage.bjPage.CloseBiaoJu ();

						break;
					case 20:
						
						Debug.Log ("YunBiao Fail!");
						textStr = "运镖失败...";
						QXComData.CreateBox (1,textStr,true,null);
						
						break;
					case 30:

						Debug.Log ("已进入押镖");
						textStr = "正在运镖...";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					case 40:
						
						//运镖次数用完
						textStr = "运镖次数已用完...";
						QXComData.CreateBox (1,textStr,true,null);

						break;
					default:
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.S_YABIAO_ENEMY_RESP:
			{
				Debug.Log ("仇人列表返回：" + ProtoIndexes.S_YABIAO_ENEMY_RESP);
				EnemiesResp enemyPageResp = new EnemiesResp();
				enemyPageResp = QXComData.ReceiveQxProtoMessage (p_message,enemyPageResp) as EnemiesResp;
				
				if (enemyPageResp != null)
				{
					if (enemyPageResp.enemyList == null)
					{
						enemyPageResp.enemyList = new List<EnemiesInfo>();
					}

					BiaoJuPage.bjPage.InItEnemyPage (enemyPageResp);
				}
				
				return true;
			}
			case ProtoIndexes.S_YABIAO_BUY_RESP://购买运镖次数返回
			{
				Debug.Log ("购买运镖次数返回：" + ProtoIndexes.S_YABIAO_BUY_RESP);
				BuyCountsResp ybBuyTimesRes = new BuyCountsResp();
				ybBuyTimesRes = QXComData.ReceiveQxProtoMessage (p_message,ybBuyTimesRes) as BuyCountsResp;

				if (ybBuyTimesRes != null)
				{
					if (CityGlobalData.GetYunBiaoBuyType == 10)
					{
						switch (ybBuyTimesRes.result)
						{
						case 10:

							Debug.Log ("剩余运镖次数：" + ybBuyTimesRes.leftYBTimes);
							Debug.Log ("已经购买运镖回数：" + ybBuyTimesRes.usedYBVip);
							
							biaoJuMainResp.yaBiaoCiShu = ybBuyTimesRes.leftYBTimes;
							biaoJuMainResp.buyCiShu = ybBuyTimesRes.usedYBVip;
							
							InItBiaoJuPage ();

							break;
						case 20:

							//元宝不足
							BiaoJuPage.bjPage.LackYuanbao ();

							break;
						case 30:

							//今日购买次数已用完
							textStr = "今日购买次数已用尽...";
							QXComData.CreateBox (1,textStr,true,null);

							break;
						default:
							break;
						}
					}
				}
				
				return true;
			}
			}
		}
		return false;
	}

	public void BiaoJuLoadCallBack (ref WWW p_www, string p_path, Object p_object)
	{
		biaoJuPrefab = (GameObject)Instantiate(p_object);

		InItBiaoJuPage ();
	}

	void InItBiaoJuPage ()
	{
		if (!MainCityUI.IsExitInObjectList (biaoJuPrefab))
		{
			MainCityUI.TryAddToObjectList(biaoJuPrefab);
		}

		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}
		BiaoJuPage.bjPage.GetBiaoJuResp (biaoJuMainResp);
	}

	/// <summary>
	/// Turns to vip.
	/// </summary>
	public void TurnToVip ()
	{
		EquipSuoData.TopUpLayerTip (biaoJuPrefab,true);
	}

	void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
