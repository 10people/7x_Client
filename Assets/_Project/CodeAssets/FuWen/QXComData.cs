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
	public static string yellow = "[eac102]";

	public static Color lightColor = new Color (1,0.8f,0.54f);

	public static int maxVipLevel = 7;

	public static int SportClearCdVipLevel = VipFuncOpenTemplate.GetNeedLevelByKey (9);//竞技可清除冷却的vip等级

	/// <summary>
	/// Players the icon.
	/// </summary>
	/// <returns>The icon.</returns>
	/// <param name="roleId">Role identifier.</param>
	public static string PlayerIcon (int roleId)
	{
		return "PlayerIcon" + roleId;
	}

	public static void LoadYuanBaoInfo (GameObject obj)
	{
		MainCityUI.setGlobalBelongings (obj, -30, 0);
	}

	public static void LoadTitleObj (GameObject obj,string title)
	{
		MainCityUI.setGlobalTitle (obj,title,0,0);
	}

	public static string AllianceName (string tempName)
	{
		return tempName.Equals ("***") || tempName.Equals ("") ? "无联盟" : "<" + tempName + ">";
	}

	#endregion

	#region JunXian

	private static readonly Dictionary<int,string[]> m_junXianDic = new Dictionary<int, string[]> ()
	{
		{1,new string[]{"小卒"}},
		{2,new string[]{"步兵"}},
		{3,new string[]{"骑士"}},
		{4,new string[]{"禁卫"}},
		{5,new string[]{"校尉"}},
		{6,new string[]{"先锋"}},
		{7,new string[]{"将军"}},
		{8,new string[]{"元帅"}},
		{9,new string[]{"诸侯"}},
	};

	public static string GetJunXianName (int jiBie)
	{
		return m_junXianDic [jiBie] [0];
	}

	#endregion

	#region JunZhuInfo

	/// <summary>
	/// Juns the zhu info.
	/// </summary>
	/// <returns>The zhu info.</returns>
	public static JunZhuInfoRet JunZhuInfo ()
	{
		var junZhuInfo = JunZhuData.Instance ().m_junzhuInfo;

		if (junZhuInfo != null)
		{
			return junZhuInfo;
		}

		Debug.LogError ("JunZhuInfo is null!");
		return null;
	}

	public static AllianceHaveResp AllianceInfo ()
	{
		var allianceInfo = AllianceData.Instance.g_UnionInfo;

		if (allianceInfo != null)
		{
			return allianceInfo;
		}

		Debug.LogError ("AllianceInfo is null");
		return null;
	}

	#endregion

	#region EffectColor

	private static readonly Dictionary<int,int> colorDic = new Dictionary<int, int>()//0-null | 1-0白 | 2.3-1绿 | 4.5.6-2蓝 | 7.8.9-3紫 | 10.11-橙
	{
		{0,-1},
		{1,0},
		{2,1},{3,1},
		{4,2},{5,2},{6,2},
		{7,3},{8,3},{9,3},
		{10,4},{11,4},{12,4},
		{21,0},{22,1},{23,2},{24,3},{25,4}
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

	private static readonly int[] differentColor = new int[]{3,5,6,8,9,11,12};
	public static bool IsDifferent (int color)
	{
		foreach (int id in differentColor)
		{
			if (id == color)
			{
				return true;
			}
		}
		return false;
	}

	private static readonly Dictionary<int,int> horseColorDic = new Dictionary<int, int>()
	{
		{1,0},{2,1},{3,3},{4,6},{5,9}
	};

	public static int HorsePinZhiId (int id)
	{
		return horseColorDic [id];
	}

	#endregion

	#region Nation

	private static readonly Dictionary<int,string[]> nationDic = new Dictionary<int, string[]>()
	{
		{0,new string[]{"周","nation_0"}},
		{1,new string[]{"齐","nation_1"}},
		{2,new string[]{"楚","nation_2"}},
		{3,new string[]{"燕","nation_3"}},
		{4,new string[]{"韩","nation_4"}},
		{5,new string[]{"赵","nation_5"}},
		{6,new string[]{"魏","nation_6"}},
		{7,new string[]{"秦","nation_7"}}
	};

	/// <summary>
	/// Gets the name of the nation.
	/// </summary>
	/// <returns>The nation name.</returns>
	/// <param name="nationId">Nation identifier.</param>
	public static string GetNationName (int nationId)
	{
		return nationDic [nationId][0];
	}

	/// <summary>
	/// Gets the name of the nation sprite.
	/// </summary>
	/// <returns>The nation sprite name.</returns>
	/// <param name="nationId">Nation identifier.</param>
	public static string GetNationSpriteName (int nationId)
	{
		return nationId >= 0 ? nationDic [nationId][1] : "";
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
		CAN_USE_PROP = 101,//可使用道具
		GENERAL_BAOXIANG = 102,//普通包厢
		GAOJI_BAOXIANG = 103,//精致宝箱
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

	#region LoadFunctionRoot

//	public static GameObject LoadRoot (GameObject rootObj,string path)
//	{
//		if (rootObj != null)
//		{
//			return rootObj;
//		}
//		else
//		{
//			return CreatRoot ();
//		}
//	}
//
//	private static GameObject CreatRoot ( ref WWW p_www, string p_path, UnityEngine.Object p_object )
//	{
//		GameObject tempRoot = GameObject.Instantiate (p_object);
//	}

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
		
		SocketTool.Instance().SendSocketMessage ((short)(protoA),ref t_protof,protoB);
	}

	/// <summary>
	/// Sends the qx proto message.
	/// </summary>
	/// <param name="protoA">C_ProtoIndex</param>
	/// <param name="protoB">S_Protoindex</param>
	public static void SendQxProtoMessage (int protoA ,string protoB = null)
	{	
		SocketTool.Instance().SendSocketMessage ((short)(protoA),protoB);
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

	#region MiBaoSkillLevel
	public static int GetMiBaoSkillLevel (int tempSkillId)
	{
		var skillList = MiBaoGlobleData.Instance().G_MiBaoInfo.skillList;
		foreach (SkillInfo skill in skillList)
		{
			if (skill.activeZuheId == tempSkillId)
			{
				return skill.level;
			}
		}

		return 0;
	}

	public static bool CanSelectMiBaoSkill ()
	{
		var skillList = MiBaoGlobleData.Instance().G_MiBaoInfo.skillList;
		return skillList == null ? false : true;
	}

	#endregion

	#region ShowBtnEffect
	public static void ShowChangeSkillEffect (bool tempOpen,GameObject tempSkillBtn,int tempEffectId)
	{
		UI3DEffectTool.ClearUIFx (tempSkillBtn);
		if (tempOpen)
		{
			if (MiBaoGlobleData.Instance().GetMiBaoskillOpen ())
			{
				UI3DEffectTool.ShowTopLayerEffect ( UI3DEffectTool.UIType.FunctionUI_1,tempSkillBtn,
				                                               EffectIdTemplate.GetPathByeffectId(tempEffectId));
			}
			
			BoxCollider btnBox = tempSkillBtn.GetComponent<BoxCollider> ();
			if (btnBox != null)
			{
				btnBox.enabled = MiBaoGlobleData.Instance().GetMiBaoskillOpen ();
			}
			
			UISprite btnSprite = tempSkillBtn.GetComponent<UISprite> ();	 
			if (btnSprite != null)
			{
				btnSprite.color = MiBaoGlobleData.Instance().GetMiBaoskillOpen () ? Color.white : Color.black;
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
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao(ZhuXianTemp.getTemplateById(tempTaskId).m_listYindaoShuju[tempState]);
			}

			break;
		case YinDaoStateControl.FINISHED_TASK_YINDAO:

			if(FreshGuide.Instance().IsActive(tempTaskId) && TaskData.Instance.m_TaskInfoDic[tempTaskId].progress < 0)
			{
				//ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[tempTaskId];
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao(ZhuXianTemp.getTemplateById(tempTaskId).m_listYindaoShuju[tempState]);
			}

			break;
		default:
			CityGlobalData.m_isRightGuide = true;
			break;
		}
//		Debug.Log ("yindoaend");
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

	/// <summary>
	/// Checks the open task.
	/// </summary>
	/// <returns><c>true</c>, if open task was checked, <c>false</c> otherwise.</returns>
	/// <param name="tempTaskId">Temp task identifier.</param>
	public static bool CheckOpenTask (int tempTaskId)
	{
		if (FreshGuide.Instance ().IsActive (tempTaskId))
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

	public static int ConvertDateTimeInt (System.DateTime time)
	{
		System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
		return (int)(time - startTime).TotalSeconds;
	}

	#endregion

	#region CreateBox
	public static GameObject CreateBox (int tempColorId,string tempText,bool isOneBtn,UIBox.onclick onClick,bool isFunction = false)
	{
		return isOneBtn ? UtilityTool.Instance.CreateBox (titleStr,
                                               			  MyColorData.getColorString (tempColorId,"\n" + tempText),null,null,
                                               			  confirmStr,null,
		                                      			  onClick,
		                                                  null,
		                                                  null,
		                                                  null,
		                                                  false,
		                                                  true,
		                                                  true,
		                                                  isFunction) 
						: UtilityTool.Instance.CreateBox (titleStr,
			                                  			  MyColorData.getColorString (tempColorId,"\n" + tempText),null,null,
		                                       			  cancelStr,confirmStr,
			                                   			  onClick,
			                                  			  null,
			                                 			  null,
			                                			  null,
			                                			  false,
			                                			  true,
			                                			  true,
			                                			  isFunction);
	}

	public static GameObject CreateBoxDiy (string tempText,bool isOneBtn,UIBox.onclick onClick,bool isFunction = false)
	{
		return isOneBtn ? UtilityTool.Instance.CreateBox (titleStr,
                                               			  "\n" + tempText,null,null,
                                               			  confirmStr,null,
		                                                  onClick,
		                                                  null,
		                                                  null,
		                                                  null,
		                                                  false,
		                                                  true,
		                                                  true,
		                                                  isFunction) 
						: UtilityTool.Instance.CreateBox (titleStr,
			                                   			  "\n" + tempText,null,null,
			                                   			  cancelStr,confirmStr,
                                  			  		      onClick,
						                                  null,
						                                  null,
						                                  null,
						                                  false,
						                                  true,
						                                  true,
						                                  isFunction);
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

			UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		case EffectPos.MID:

			UI3DEffectTool.ShowMidLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		case EffectPos.BOTTOM:

			UI3DEffectTool.ShowBottomLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,
			                                               obj,
			                                               EffectIdTemplate.GetPathByeffectId(effectId));

			break;
		default:
			break;
		}
	}
	
	public static void ClearEffect (GameObject obj)
	{
		UI3DEffectTool.ClearUIFx (obj);
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
		JIFEN = 6,//积分
	}
	/// <summary>
	/// 0-spriteName 1-name 2-size 3-LanguageTempId
	/// </summary>
	private static readonly Dictionary<MoneyType,string[]> moneyDic = new Dictionary<MoneyType, string[]>()
	{
		{MoneyType.WEIWANG,new string[]{"weiwangIcon","威望","60,60","30003"}},
		{MoneyType.GONGXUN,new string[]{"GongXun","功勋","60,63","30005"}},
		{MoneyType.HUANGYE,new string[]{"HuangYe","荒野币","70,70","30006"}},
		{MoneyType.GONGXIAN,new string[]{"GongXian","贡献","60,60","30004"}},
		{MoneyType.YUANBAO,new string[]{"YB_big","元宝","55,40","30001"}},
		{MoneyType.TONGBI,new string[]{"coinicon","铜币","50,50","30002"}},
		{MoneyType.JIFEN,new string[]{"gongjin","积分","50,50","30007"}}
	};

	/// <summary>
	/// Moneies the sprite.
	/// </summary>
	/// <returns>The sprite.</returns>
	/// <param name="tempType">Temp type.</param>
	public static UISprite MoneySprite (MoneyType tempType,UISprite tempSprite,float tempScaleTimes = 1)
	{
//		tempSprite.transform.localRotation = new Quaternion (0,0,tempType == MoneyType.YUANBAO ? 15 : 0,0);
		tempSprite.spriteName = moneyDic [tempType] [0];
		string[] scale = moneyDic [tempType] [2].Split (',');
		tempSprite.SetDimensions ((int)(float.Parse (scale[0]) * tempScaleTimes),(int)(float.Parse (scale[1]) * tempScaleTimes));
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

	/// <summary>
	/// Moneies the dic string.
	/// </summary>
	/// <returns>The dic string.</returns>
	/// <param name="tempType">Temp type.</param>
	/// <param name="tempIndex">Temp index.</param>
	public static string MoneyDicStr (MoneyType tempType,int tempIndex)
	{
		if (tempIndex > moneyDic[tempType].Length - 1)
		{
			Debug.LogError ("Money Str return null!");
		}
		return tempIndex > moneyDic[tempType].Length - 1 ? null : moneyDic[tempType][tempIndex];
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

	public static List<GameObject> CreateGameObjectList (GameObject tempObj,int tempResCount,List<GameObject> tempObjList)
	{
		int tempCount = tempResCount - tempObjList.Count;
		if (tempCount > 0)
		{
			for (int i = 0;i < tempCount;i ++)
			{
				GameObject obj = GameObject.Instantiate (tempObj);
				
				obj.SetActive (true);
				obj.transform.parent = tempObj.transform.parent;
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
		
		return tempObjList;
	}

	#endregion

	#region SetWidgetValueRelativeToScrollView

	public static void SetWidget (UIScrollView sc,UIScrollBar sb,GameObject obj)
	{
		UIWidget widget = obj.GetComponent<UIWidget>();
		
		float widgetValue = sc.GetWidgetValueRelativeToScrollView (widget).y;
		if (widgetValue < 0 || widgetValue > 1)
		{
			sc.SetWidgetValueRelativeToScrollView(widget, 0);
			
			//clamp scroll bar value.
			//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
			//set 0.99 and 0.01 cause same bar value not taken in execute.
			float scrollValue = sc.GetSingleScrollViewValue();
			if (scrollValue >= 1) sb.value = 0.99f;
			if (scrollValue <= 0) sb.value = 0.01f;
		}
	}

	#endregion

	#region UIScrollBar Value

	public static void InItScrollBarValue (UIScrollBar scrollBar,int value)
	{
		scrollBar.barSize = (float)value/100;
	}

	#endregion

	#region Vip

	public delegate void VipDelegate (int i);

	private static VipDelegate m_vipDelegate;

	public static void TurnToVip (string tempStr,VipDelegate tempDelegate = null)
	{
		m_vipDelegate = tempDelegate;
		CreateBoxDiy (tempStr,false,VipCallBack);
	}

	private static void VipCallBack (int i)
	{
		if (m_vipDelegate != null)
		{
			m_vipDelegate (i);
		}

		if (i == 2)
		{
			EquipSuoData.TopUpLayerTip();
		}
	}

	#endregion

	#region TextLimit
	public enum StrLimitType
	{
		USER_NAME,
		CREATE_ROLE_NAME,
	}
	private StrLimitType limitType;
	/// <summary>
	/// 字符串长度限制
	/// </summary>
	/// <returns>The refresh.</returns>
	/// <param name="limitType">Limit type.</param>
	/// <param name="str">String.</param>
	public static string TextLengthLimit (StrLimitType limitType,string str)
	{
		int limitCount = 0;
		
		string lastStr = "";
		
		List<char> textStrList = new List<char> ();
		
		for (int i = 0;i < str.Length;i ++)
		{
			switch (limitType)
			{
			case StrLimitType.CREATE_ROLE_NAME:
				
				limitCount = GetCreateRoleNameLimit();
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 0x4e00 && str[i] <= 0x9fa5))
				{
					textStrList.Add (str[i]);
				}
				break;
				
			case StrLimitType.USER_NAME:
				
				limitCount = 8;
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z'))
				{
					textStrList.Add (str[i]);
				}
				break;
			default:
				break;
			}
			
		}
		
		//		Debug.Log ("LimitCount:" + limitCount);
		
		for (int i = 0;i < textStrList.Count;i ++)
		{
			if (i < limitCount)
			{
				lastStr += textStrList[i];
			}
		}
		
		return lastStr;
	}

	public static int GetCreateRoleNameLimit ()
	{
		return 7;
	}
	
	public static int GetAvailableTextLength( StrLimitType limitType,string str ){
		int limitCount = 0;
		
		string lastStr = "";
		
		List<char> textStrList = new List<char> ();
		
		for (int i = 0;i < str.Length;i ++)
		{
			switch (limitType)
			{
			case StrLimitType.CREATE_ROLE_NAME:
				
				limitCount = GetCreateRoleNameLimit ();
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z') || (str[i] >= 0x4e00 && str[i] <= 0x9fa5))
				{
					textStrList.Add (str[i]);
				}
				break;
				
			case StrLimitType.USER_NAME:
				
				limitCount = 8;
				
				if ((str[i] >= 'A' && str[i] <= 'Z') || (str[i] >= '0' && str[i] <= '9') ||
				    (str[i] >= 'a' && str[i] <= 'z'))
				{
					textStrList.Add (str[i]);
				}
				break;
			default:
				break;
			}
			
		}
		
		return textStrList.Count;
	}
	#endregion
}

#region ShopGoodInfo
/// <summary>
/// Shop good info.
/// </summary>
public class ShopGoodInfo
{
	public int xmlId;//xmlId
	public int itemId;//兑换物品id
	public int itemType;//物品类型
	public int site;//物品位置
	public string itemName;//兑换物品名字
	public int itemNum;//兑换的数量
	public int needMoney;//需要的钱币数量
	public int countBuyTime;//剩余购买次数
    public bool isRestrictBuy;
	public QXComData.MoneyType moneyType;//币种
}

/// <summary>
/// Shop sell good info.
/// </summary>
public class ShopSellGoodInfo
{
	public int sellType;
	public int itemId;
	public long dbId;
	public QXComData.XmlType itemType;
	public int itemNum;
}
#endregion

#region BiaoJu
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
#endregion

#region YongBingInfo

public class YongBingInfo
{
	public int yongBingId;//id
	public int yongBingHp;//hp

	public int profession = 0;//佣兵职业
	public int iconId = 0;//佣兵头像id
	public int level = 0;//等级
	public int nameId = 0;//名字id
	public int desId = 0;//描述
}

#endregion

#region ChatInfo

public class ChatMessage
{
	public enum SendState
	{
		SENDING,
		SEND_FAIL,
		SEND_SUCCESS
	}
	public SendState sendState;
	public ChatPct chatPct;
}

#endregion
