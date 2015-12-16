using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXComData {

	#region ComVariable 

	public static string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);//确定按钮
	public static string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);//取消按钮
	public static string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);//提示

	public static int maxVipLevel = 10;

	public static void LoadYuanBaoInfo (GameObject obj)
	{
		MainCityUI.m_MainCityUI.setGlobalBelongings (obj, 420, 314);
	}

	#endregion

	#region EffectColor

	private static readonly Dictionary<int,int> colorDic = new Dictionary<int, int>()//0-null | 1-0白 | 2.3-1绿 | 4.5.6-2蓝 | 7.8.9-3紫 | 10.11-橙
	{
		{0,-1},{1,0},{2,1},{3,1},{4,2},{5,2},{6,2},{7,3},{8,3},{9,3},{10,4},{11,4}
	};

	/// <summary>
	/// Gets the color of the effect.
	/// </summary>
	/// <returns>The effect color.</returns>
	/// <param name="xmlColorId">Xml color identifier.</param>
	public static int GetEffectColorByXmlColorId (int xmlColorId)
	{
		return colorDic [xmlColorId];
	}


	#endregion

	#region XmlTemplateType

	public enum XmlType
	{
		COMMON = 0,//普通道具
		EQUIP = 2,//装备
		YUJUE = 3,//玉珏
		MIBAO = 4,//秘宝
		MIBAO_PIECE = 5,//秘宝碎片
		ADVANCED_MATERIALS = 6,//进阶材料
		BAOSHI = 7,//宝石
		FUSHI = 8,//符石
		STRENGTH_MATERIALS = 9,//强化材料
	}

	/// <summary>
	/// Gets the xml type by item identifier.
	/// </summary>
	/// <returns>The xml type by item identifier.</returns>
	/// <param name="itemId">Item identifier.</param>
	public static XmlType GetXmlTypeByItemId (int itemId)
	{
		return (XmlType)Enum.ToObject (typeof (XmlType),CommonItemTemplate.GetCommonItemTemplateTypeById (itemId));
	}

	#endregion

	#region SendProtoMessage And ReceiveProtoMessage
	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (object value, int protoA ,string protoB = null)
	{
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,value);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),ref t_protof,protoB);
	}

	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (int protoA ,string protoB = null)
	{	
		SocketTool.Instance ().SendSocketMessage ((short)(protoA),protoB);
	}

	/// <summary>
	/// Receives the qx proto message.
	/// </summary>
	/// <returns>The qx proto message.</returns>
	/// <param name="p_message">P_message.</param>
	/// <param name="value">Value.</param>
	public static object ReceiveQxProtoMessage (QXBuffer p_message,object value)
	{
		MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
		
		QiXiongSerializer t_qx = new QiXiongSerializer();
		
		t_qx.Deserialize(t_stream, value, value.GetType());

		return value;
	}
	#endregion

	#region ShowBtnEffect
	public static void ShowChangeSkillEffect (bool tempOpen,GameObject tempSkillBtn,int tempEffectId)
	{
		UI3DEffectTool.Instance ().ClearUIFx (tempSkillBtn);
		if (tempOpen)
		{
			if (MiBaoGlobleData.Instance ().GetMiBaoskillOpen ())
			{
				UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,tempSkillBtn,
				                                               EffectIdTemplate.GetPathByeffectId(tempEffectId));
			}
			
			BoxCollider btnBox = tempSkillBtn.GetComponent<BoxCollider> ();
			if (btnBox != null)
			{
				btnBox.enabled = MiBaoGlobleData.Instance ().GetMiBaoskillOpen ();
			}
			
			UISprite btnSprite = tempSkillBtn.GetComponent<UISprite> ();	 
			if (btnSprite != null)
			{
				btnSprite.color = MiBaoGlobleData.Instance ().GetMiBaoskillOpen () ? Color.white : Color.black;
			}
		}
	}
	#endregion

	#region YinDaoControl
	public enum YinDaoStateControl
	{
		UN_FINISHED_TASK_YINDAO,//任务未完成时的引导步骤
		FINISHED_TASK_YINDAO,//任务完成时的引导步骤
	}
	public static void YinDaoStateController (YinDaoStateControl tempYinDao,int tempTaskId,int tempState)
	{
		switch (tempYinDao)
		{
		case YinDaoStateControl.UN_FINISHED_TASK_YINDAO:

			if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress >= 0)
			{
				//ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[tempTaskId];
				UIYindao.m_UIYindao.setOpenYindao(ZhuXianTemp.getTemplateById(tempTaskId).m_listYindaoShuju[tempState]);
			}

			break;
		case YinDaoStateControl.FINISHED_TASK_YINDAO:

			if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress < 0)
			{
				//ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[tempTaskId];
				UIYindao.m_UIYindao.setOpenYindao(ZhuXianTemp.getTemplateById(tempTaskId).m_listYindaoShuju[tempState]);
			}

			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Checks the state of the yin DAO open.
	/// </summary>
	/// <returns><c>true</c>, if yin DAO open state was checked, <c>false</c> otherwise.</returns>
	/// <param name="tempTaskId">Temp task identifier.</param>
	public static bool CheckYinDaoOpenState (int tempTaskId)
	{
		if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress >= 0)
		{
			return true;
		}
		return false;
	}
	
	#endregion

	#region TimeStyle 00:00:00
	public static string TimeFormat (int tempTime)
	{
		string hourStr = "";
		string minuteStr = "";
		string secondStr = "";

		int hour = tempTime / 3600;
		int minute = (tempTime / 60) % 60;
		int second = tempTime % 60;
		//			Debug.Log (hour + ":" + minute + ":" + second);
		if (hour < 10)
		{
			hourStr = "0" + hour;
		}
		else
		{
			hourStr = hour.ToString ();
		}
		
		if (minute < 10)
		{
			minuteStr = "0" + minute;
		}
		else
		{
			minuteStr = minute.ToString ();
		}
		
		if (second < 10) 
		{
			secondStr = "0" + second;
		} 
		else 
		{
			secondStr = second.ToString ();
		}
		
		return hourStr + " : " + minuteStr + "：" + secondStr;
	}

	public static string UTCToTimeString(long time, string format)
	{
		long lTime = time * 10000;
		
		DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		
		TimeSpan toNow = new TimeSpan(lTime);
		
		DateTime dtResult = dtStart.Add(toNow);
		
		// "yyyy-MM-dd HH:mm:ss"
		return dtResult.ToString(format);
	}

	#endregion

	#region CreateBox
	public static GameObject CreateBox (int tempColorId,string tempText,bool isOneBtn,UIBox.onclick onClcik)
	{
		return isOneBtn ? UtilityTool.Instance.CreateBox (titleStr,
                                               MyColorData.getColorString (tempColorId,"\n\n" + tempText),null,null,
                                               confirmStr,null,
		                                       onClcik) 
						: UtilityTool.Instance.CreateBox (titleStr,
		                                       MyColorData.getColorString (tempColorId,"\n\n" + tempText),null,null,
		                                       cancelStr,confirmStr,
		                                       onClcik);
	}
	#endregion

	#region Instance Obj Effect
	public enum EffectPos
	{
		TOP,
		MID,
		BOTTOM,
	}
	public static void InstanceEffect (EffectPos pos,GameObject obj,int effectId)
	{
		switch (pos)
		{
		case EffectPos.TOP:

			UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		case EffectPos.MID:

			UI3DEffectTool.Instance ().ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		case EffectPos.BOTTOM:

			UI3DEffectTool.Instance ().ShowBottomLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		default:
			break;
		}
	}
	
	public static void ClearEffect (GameObject obj)
	{
		UI3DEffectTool.Instance ().ClearUIFx (obj);
	}
	#endregion

	#region MoneyType
	public enum MoneyType
	{
		WEIWANG =3,//威望
		GONGXUN = 5,//功勋
		HUANGYE = 4,//荒野
		GONGXIAN = 2,//贡献
		YUANBAO = 1,//元宝
		TONGBI = 0,//铜币
	}
	private static readonly Dictionary<MoneyType,string[]> moneyDic = new Dictionary<MoneyType, string[]>()
	{
		{MoneyType.WEIWANG,new string[]{"weiwangIcon","威望","60,60"}},
		{MoneyType.GONGXUN,new string[]{"GongXun","功勋","60,63"}},
		{MoneyType.HUANGYE,new string[]{"HuangYe","荒野币","70,70"}},
		{MoneyType.GONGXIAN,new string[]{"GongXian","贡献","60,60"}},
		{MoneyType.YUANBAO,new string[]{"YB_big","元宝","55,40"}},
		{MoneyType.TONGBI,new string[]{"coinicon","铜币","50,50"}}
	};

	/// <summary>
	/// Moneies the sprite.
	/// </summary>
	/// <returns>The sprite.</returns>
	/// <param name="tempType">Temp type.</param>
	public static UISprite MoneySprite (MoneyType tempType,UISprite tempSprite)
	{
		tempSprite.transform.localScale = Vector3.one;
//		tempSprite.transform.localRotation = new Quaternion (0,0,tempType == MoneyType.YUANBAO ? 15 : 0,0);
		tempSprite.spriteName = moneyDic [tempType] [0];
		string[] scale = moneyDic [tempType] [2].Split (',');
		tempSprite.SetDimensions (int.Parse (scale[0]),int.Parse (scale[1]));
		return tempSprite;
	}
	/// <summary>
	/// Moneies the name.
	/// </summary>
	/// <returns>The name.</returns>
	/// <param name="tempType">Temp type.</param>
	public static string MoneyName (MoneyType tempType)
	{
		return moneyDic [tempType] [1];
	}
	#endregion

	#region CreateScrollView GameObjectList

	public static List<GameObject> CreateGameObjectList (GameObject tempObj,UIGrid tempGrid,int tempResCount,List<GameObject> tempObjList)
	{
		int tempCount = tempResCount - tempObjList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (tempObj);

				obj.SetActive (true);
				obj.transform.parent = tempGrid.transform;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = Vector3.one;

				tempObjList.Add (obj);
			}
		}
		else
		{
			for (int i = 0;i < Mathf.Abs (tempCount);i ++)
			{
				GameObject.Destroy (tempObjList[tempObjList.Count - 1]);
				tempObjList.RemoveAt (tempObjList.Count - 1);
			}
		}

		tempGrid.repositionNow = true;

		return tempObjList;
	}

	#endregion
}

/// <summary>
/// Shop good info.
/// </summary>
public class ShopGoodInfo
{
	public int xmlId;//xmlId
	public int itemId;//兑换物品id
	public int itemType;//物品类型
	public string itemName;//兑换物品名字
	public int itemNum;//兑换的数量
	public int needMoney;//需要的钱币数量
	public QXComData.MoneyType moneyType;//币种
	
	public static ShopGoodInfo CreateShopGood (int tempXmlId,int tempId,int tempItemType,string tempName,int tempMoney,QXComData.MoneyType tempType,int tempNum)
	{
		ShopGoodInfo tempGood = new ShopGoodInfo ();
		tempGood.xmlId = tempXmlId;
		tempGood.itemId = tempId;
		tempGood.itemType = tempItemType;
		tempGood.itemName = tempName;
		tempGood.needMoney = tempMoney;
		tempGood.moneyType = tempType;
		tempGood.itemNum = tempNum;
		
		return tempGood;
	}
}

/// <summary>
/// Biao ju horse info.
/// </summary>
public class BiaoJuHorseInfo
{
	public int horseId;
	public int horseItemId;
	public string horseName;
	public int shouYi;
	public int upNeedMoney;
	public int needVipLevel;
}

/// <summary>
/// Horse property info.
/// </summary>
public class HorsePropInfo
{
	public int id;
	public string name;
	public int iconId;
	public int colorId;
	public string desc;
	public int cost;
	public bool isBuy;

	public static HorsePropInfo CreateHorseProp (int tempId,string tempName,int tempIconId,int tempColorId,string tempDesc,int tempCost,bool tempIsBuy)
	{
		HorsePropInfo tempInfo = new HorsePropInfo ();
		tempInfo.id = tempId;
		tempInfo.name = tempName;
		tempInfo.iconId = tempIconId;
		tempInfo.colorId = tempColorId;
		tempInfo.desc = tempDesc;
		tempInfo.cost = tempCost;
		tempInfo.isBuy = tempIsBuy;

		return tempInfo;
	}

	public static HorsePropInfo CreateHorseProp (int tempId,bool tempIsBuy)
	{
		MaJuTemplate maJuTemp = MaJuTemplate.GetMaJuTemplateById (tempId);
		HorsePropInfo propInfo = HorsePropInfo.CreateHorseProp (maJuTemp.id,
		                                                        NameIdTemplate.GetName_By_NameId (maJuTemp.nameId),
		                                                        maJuTemp.iconId,
		                                                        maJuTemp.colorId,
		                                                        DescIdTemplate.GetDescriptionById (maJuTemp.descId),
		                                                        maJuTemp.priceId,
		                                                        tempIsBuy);

		return propInfo;
	}
}
