using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class SelectHorseMainPage : MonoBehaviour,SocketProcessor {
	
	public GameObject horseItemObj;

	public UILabel awardLabel;//收益

	private List<GameObject> horseObjList = new List<GameObject> ();
	/// <summary>
	/// 选马首页信息
	/// </summary>
	private YabiaoMenuResp horseRes;
	/// <summary>
	/// 请求运镖结果返回
	/// </summary>
	private YabiaoResult resultRes;
	/// <summary>
	/// 规则说明
	/// </summary>
	public UILabel rulesLabel;
	private List<string> ruleList = new List<string> ();
	/// <summary>
	/// 协助盟友item
	/// </summary>
	public GameObject helpItemObj;
	private List<GameObject> helpItemList = new List<GameObject> ();
	public List<Vector3> posList = new List<Vector3> ();
	/// <summary>
	/// 协助运镖返回
	/// </summary>
	private AskYaBiaoHelpResp yaBiaoHelpResp;

	public GameObject zheZhao;

	private int backType;//升级马返回类型
	private string titleStr;
	private string str;
	private string cancel;
	private string confirm;

	/// <summary>
	/// 获得被拒绝的协助者id
	/// </summary>
	private static long refuseId;
	public static long GetRefuseId
	{
		set{refuseId = value;}
//		get{return refuseId;}
	}
	/// <summary>
	/// 获得被拒绝的协助者名字
	/// </summary>
	private static string refuseName;
	public static string GetRefuseName
	{
		set{refuseName = value;}
	}
	/// <summary>
	/// 协助邀请剩余次数
	/// </summary>
	public UILabel helpCountTime;
	/// <summary>
	/// 选择框
	/// </summary>
	public GameObject selectBoxObj;

	public ScaleEffectController m_ScaleEffectController;

	private bool shining = false;//是否播放选择框闪烁动画
	int shiningCount = 0;//闪烁次数
	float alphaCount;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		CreateHorseItem ();

		alphaCount = selectBoxObj.GetComponent<UIWidget> ().alpha;

		cancel = YunBiaoMainPage.yunBiaoMainData.cancelStr;
		confirm = YunBiaoMainPage.yunBiaoMainData.confirmStr;
	}

	/// <summary>
	/// 创建马匹item
	/// </summary>
	void CreateHorseItem ()
	{
		for (int i = 0;i < 5;i ++)
		{
			GameObject horseItem = (GameObject)Instantiate (horseItemObj);

			horseItem.SetActive (true);
			horseItem.transform.parent = horseItemObj.transform.parent;
			horseItem.transform.localPosition = new Vector3 (187 * i,0,0);
			horseItem.transform.localScale = Vector3.one;

			horseObjList.Add (horseItem);

			HorseItem horse = horseItem.GetComponent<HorseItem> ();
			horse.GetHorseType (i + 1);
		}

		ruleList.Clear ();
		rulesLabel.text = "";
		ruleList.Add (LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_4));
		ruleList.Add (LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_5));
		ruleList.Add (LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_6));
		ruleList.Add (LanguageTemplate.GetText (LanguageTemplate.Text.YUN_BIAO_7));

		for (int i = 0;i < ruleList.Count;i ++)
		{
			if (i < ruleList.Count - 1)
			{
				rulesLabel.text += ruleList[i] + "\n";
			}
			else
			{
				rulesLabel.text += ruleList[i];
			}
		}
		awardLabel.text = YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (1).ToString ();
		HorseMainPageReq ();
	}

	/// <summary>
	/// 选马首页信息请求
	/// </summary>
	void HorseMainPageReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_MENU_REQ,"3404");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YABIAO_MENU_RESP://选马页面信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YabiaoMenuResp horseMainRes = new YabiaoMenuResp();
				
				t_qx.Deserialize(t_stream, horseMainRes, horseMainRes.GetType());

				if (horseMainRes != null)
				{
					Debug.Log ("马匹类型：" + horseMainRes.horse);

					if (horseMainRes.jz == null)
					{
						horseMainRes.jz = new List<XieZhuJunZhu>();
					}

					horseRes = horseMainRes;

					//判断是否应该随机马
					Debug.Log ("是否随机过马：" + horseMainRes.isNewHorse);
					HorseRandomAnimation (horseMainRes.isNewHorse,horseRes.horse);

					InItHelpFriends ();
				}

				return true;
			}

			case ProtoIndexes.S_SETHORSE_RESP://请求设置马匹返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				SetHorseResult setHorseRes = new SetHorseResult();
				
				t_qx.Deserialize(t_stream, setHorseRes, setHorseRes.GetType());

				if (setHorseRes != null)
				{
					Debug.Log ("setHorseRes:" + setHorseRes.result);

					backType = setHorseRes.result;

					if (setHorseRes.result == 40)
					{
						YunBiaoMainPage.yunBiaoMainData.LackYbBoxLoad ();
					}
					else
					{
						if (setHorseRes.result == 10)
						{
							if (horseRes.horse < 5)
							{
								horseRes.horse += 1;
								
								HorseRandomAnimation (false,horseRes.horse);
							}
						}

						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        HorseSetLoadBack );
					}
				}

				return true;
			}

			case ProtoIndexes.S_YABIAO_RESP://请求运镖返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YabiaoResult yunBiaoRes = new YabiaoResult();
				
				t_qx.Deserialize(t_stream, yunBiaoRes, yunBiaoRes.GetType());
				
				if (yunBiaoRes != null)
				{
					switch (yunBiaoRes.result)
					{
					case 10:
						
						Debug.Log ("YunBiao Success!");

						//跳转到劫镖页面
						YunBiaoMainPage.yunBiaoMainData.JieHuoBtn ();

						YunBiaoData.Instance.yunBiaoRes.yaBiaoCiShu -= 1;
						YunBiaoData.Instance.yunBiaoRes.state = 10;
						YunBiaoMainPage.yunBiaoMainData.InItYunBiaoMainPage (YunBiaoData.Instance.yunBiaoRes);

						BackBtn ();

						break;
						
					case 20:
						
						Debug.Log ("YunBiao Fail!");
						
						break;
					}
				}
				
				return true;
			}
			case ProtoIndexes.S_YABIAO_HELP_RESP ://请求协助押镖返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YaBiaoHelpResp sendHelpInfoResp = new YaBiaoHelpResp();
				
				t_qx.Deserialize(t_stream, sendHelpInfoResp, sendHelpInfoResp.GetType());

				if (sendHelpInfoResp != null)
				{
					switch (sendHelpInfoResp.code)
					{
					case 10:

						Debug.Log ("发送协助邀请成功");
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        				                        SendSuccessLoadBack );

						break;
					case 20:

						Debug.Log ("还没有联盟");
						//弹出无联盟提醒
						ShowWarrningInfo ("您还没有盟友，请先加入一个联盟");

						break;

					case 30:

						Debug.Log ("发送频率过快");
						//弹出时间过快提醒
						ShowWarrningInfo ("冷却中，请不要频繁发送协助邀请");

						break;
					}
				}

				return true;
			}
			case ProtoIndexes.S_ASK_YABIAO_HELP_RESP://答复请求协助运镖返回
			{
				Debug.Log ("答复请求协助运镖返回：" + ProtoIndexes.S_ASK_YABIAO_HELP_RESP);
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				AskYaBiaoHelpResp helpResp = new AskYaBiaoHelpResp();
				
				t_qx.Deserialize(t_stream, helpResp, helpResp.GetType());
			
				if (helpResp != null)
				{
					yaBiaoHelpResp = helpResp;
					switch (helpResp.code)
					{
					case 10:
						Debug.Log ("协助者名字：" + helpResp.jz.name);
						horseRes.jz.Add (helpResp.jz);//添加协助者信息到协助者信息列表里
						CreateHelpItem (helpResp.jz);//添加一个协助者item

						break;

					case 20:

						Debug.Log ("盟友拒绝邀请！");

						break;
					}
				}

				return true;
			}
			case ProtoIndexes.S_TICHU_YBHELP_RESP://拒绝协助返回
			{
				Debug.Log ("踢出一个协助者：" + ProtoIndexes.S_TICHU_YBHELP_RESP);
				//删除一个协助者，更新协助者列表
				RefreshHelpItemList ();

//				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
//				                        RefuseHelpLoadBack );

				return true;
			}
			}
		}

		return false;
	}

	/// <summary>
	/// 升级马返回
	/// </summary>
	/// <param name="p_www">P_www.</param>
	/// <param name="p_path">P_path.</param>
	/// <param name="p_object">P_object.</param>
	void HorseSetLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		titleStr = "提示";

		switch (backType)
		{
		case 10:
			str = "已将马升级到" + HorsePinZhi (horseRes.horse);
			break;

		case 20:
			str = "已是最高等级马！";
			break;

		case 30:
			str = "君主押镖信息不存在！";
			break;

		default:break;
		}
		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, confirm, null,
		             null);
	}

	/// <summary>
	/// 随机马的动画
	/// </summary>
	void HorseRandomAnimation (bool isRandom,int horseType)
	{
		if (isRandom)
		{
			//随机动画
			ZheZhaoControl (true);
		}
		else
		{
			MoveToPosition (horseType - 1);
		}
	}

	/// <summary>
	/// 随机动画时可否点击界面控制
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	void ZheZhaoControl (bool isActive)
	{
		zheZhao.SetActive (isActive);
		if (isActive)
		{
			TweenAlpha zheZhaoAlpha = zheZhao.GetComponent<TweenAlpha> ();
			zheZhaoAlpha.duration = 0.1f;
			zheZhao.GetComponent<UISprite> ().alpha = 0.05f;
			zheZhaoAlpha.from = 0.05f;
			zheZhaoAlpha.to = 1;
			EventDelegate.Add (zheZhaoAlpha.onFinished,AnimateBegin);
		}
	}
	
	void AnimateBegin ()
	{
		StartCoroutine ("HorseSelectAnimate");
	}

	private float animateTime = 0.01f;
	private int moveCount = 25;
	private int startCount;
	//选马动画
	IEnumerator HorseSelectAnimate ()
	{
		while (startCount < moveCount + horseRes.horse)
		{
//			Debug.Log ("animateTime:" + animateTime);
//			Debug.Log ("horseType:" + horseRes.horse);
//			Debug.Log ("startCount:" + startCount);

			MoveToPosition (startCount);
			startCount ++;

//			if (startCount < moveCount + horseRes.horse - 5)
//			{
//				if (animateTime > 0.05f)
//				{
//					if (animateTime > 0.3f)
//					{
//						animateTime -= 0.08f;
//					}
//					else 
//					{
//						animateTime -= 0.04f;
//					}
//				}
//				else
//				{
//					animateTime = 0.05f;
//				}
//			}
//			else
//			{
//				if (animateTime < 0.5f)
//				{
//					animateTime += 0.08f;
//				}
//				else
//				{
//					animateTime = 0.5f;
//				}
//			}

			if (startCount < moveCount + horseRes.horse - 5)
			{
				animateTime = 0.01f;
			}
			else
			{
				if (animateTime < 0.5f)
				{
					animateTime += 0.08f;
				}
				else
				{
					animateTime = 0.5f;
				}
			}

			if (startCount == moveCount + horseRes.horse)
			{
				//播放选择框闪烁动画
//				ZheZhaoControl (false);
				StartCoroutine ("Shining");
			}

			yield return new WaitForSeconds (animateTime);
		}
	}

	IEnumerator Shining ()
	{
		StopCoroutine ("HorseSelectAnimate");
		yield return new WaitForSeconds (1);
		shining = true;
	}

	/// <summary>
	/// 选择框移动到指定位置 
	/// </summary>
	void MoveToPosition (int i)
	{
		int indexNumber = 0;
		if (i > 4)
		{
			indexNumber = i % 5;
		}
		else
		{
			indexNumber = i;
		}
		selectBoxObj.transform.localPosition = new Vector3 (187 * indexNumber,-22,0);

		if (indexNumber < 1)
		{
			awardLabel.text = YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (1).ToString ();
		}
		else
		{
			awardLabel.text = YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (1) + "+" + "[00ff00]" + 
				(YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (indexNumber + 1) - 
				 YunBiaoMainPage.yunBiaoMainData.GetHorseAwardNum (1)) + "[-]";
		}
	}
	
	void Update ()
	{
		if (shining)
		{
			if (shiningCount < 3)
			{
				alphaCount -= Time.deltaTime * 15f;
				
				if(alphaCount <= -1 )
				{
					alphaCount = 1;
					shiningCount ++;
				}

				selectBoxObj.GetComponent<UIWidget> ().alpha = Mathf.Abs (alphaCount);
			}
			else
			{
				ZheZhaoControl (false);
				shining = false;
			}
		}
	}

	/// <summary>
	/// 升级马按钮
	/// </summary>
	/// <returns>The horse level button.</returns>
	public void UpHorseLevelBtn ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        UpHorseLoadBack );
	}
	void UpHorseLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		titleStr = "提示";

		if (horseRes.horse < 5)
		{
			CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (horseRes.horse);

			str = "确定消耗" + cartTemp.ShengjiCost + "元宝将马升级到" + HorsePinZhi (horseRes.horse + 1) + "？";
			uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
			             null, cancel,confirm,
			             UpHorseBack);
		}
		else
		{
			str = "已是最高等级马！";
			uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
			             null, confirm, null,
			             null);
		}
	}
	void UpHorseBack (int i)
	{
		if (i == 2)
		{
			CartTemplate cartTemp = CartTemplate.GetCartTemplateByType (horseRes.horse);

			if (JunZhuData.Instance ().m_junzhuInfo.yuanBao < cartTemp.ShengjiCost)
			{
				//元宝不足
				YunBiaoMainPage.yunBiaoMainData.LackYbBoxLoad ();
			}

			else
			{
				//发送升级马请求
				SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_SETHORSE_REQ,"3406");
			}
		}
	}

	/// <summary>
	/// 马的品质
	/// </summary>
	/// <returns>The back type.</returns>
	/// <param name="i">The index.</param>
	private string HorsePinZhi (int i)
	{
		Debug.Log ("品质：" + i);
		string s = "";
		switch (i)
		{
		case 1:
			s = "白色品质";
			break;

		case 2:
			s = "绿色品质";
			break;

		case 3:
			s = "蓝色品质";
			break;

		case 4:
			s = "紫色品质";
			break;

		case 5:
			s = "橙色品质";
			break;
		}
		Debug.Log ("颜色：" + s);
		return s;
	}

	/// <summary>
	/// 创建协助好友
	/// </summary>
	void InItHelpFriends ()
	{
		for (int i = 0;i < horseRes.jz.Count;i ++)
		{
			CreateHelpItem (horseRes.jz[i]);
		}
		Debug.Log ("horseRes.remainAskXZ:" + horseRes.remainAskXZ);
		helpCountTime.text = "剩余" + horseRes.remainAskXZ.ToString () + "次";
	}
	/// <summary>
	/// 创建协助者item
	/// </summary>
	void CreateHelpItem (XieZhuJunZhu tempJunZhu)
	{
		GameObject helpItem = (GameObject)Instantiate (helpItemObj);
		
		helpItem.SetActive (true);
		helpItem.transform.parent = helpItemObj.transform.parent;
		helpItem.transform.localPosition = posList[helpItemList.Count];
		helpItem.transform.localScale = Vector3.one;

		HelpItemInfo helpInfo = helpItem.GetComponent<HelpItemInfo> ();
		helpInfo.GetHelpInfo (tempJunZhu);

		helpItemList.Add (helpItem);
	}

	/// <summary>
	/// 协助运镖请求
	/// </summary>
	public void YunBiaoHelpReq ()
	{
		if (JunZhuData.Instance ().m_junzhuInfo.lianMengId <= 0)
		{
			//您还没有盟友，请先加入一个联盟
			Debug.Log ("联盟id：" + JunZhuData.Instance ().m_junzhuInfo.lianMengId);
			ShowWarrningInfo ("您还没有盟友，请先加入一个联盟");
		}
		else
		{
			if (horseRes.remainAskXZ <= 0)
			{
				//您的协助邀请剩余次数不足
				Debug.Log ("协助邀请剩余次数：" + horseRes.remainAskXZ);
				ShowWarrningInfo ("您的协助邀请剩余次数不足");
			}
			else
			{
                if (TimeHelper.Instance.IsTimeCalcKeyExist("HelpReq"))
				{
                    if (!TimeHelper.Instance.IsCalcTimeOver("HelpReq"))
					{
						//冷却中，请不要频繁发送协助邀请
						Debug.Log ("冷却中，请不要频繁发送协助邀请");
						ShowWarrningInfo ("冷却中，请不要频繁发送协助邀请");
					}
					else
					{
						SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_HELP_RSQ,"3430");
						Debug.Log ("协助邀请请求2：" + ProtoIndexes.C_YABIAO_HELP_RSQ);
                        TimeHelper.Instance.RemoveFromTimeCalc("HelpReq");
                        TimeHelper.Instance.AddOneDelegateToTimeCalc("HelpReq", 10);
					}
				}
				else
				{
					SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_HELP_RSQ,"3430");
					Debug.Log ("协助邀请请求1：" + ProtoIndexes.C_YABIAO_HELP_RSQ);
                    TimeHelper.Instance.AddOneDelegateToTimeCalc("HelpReq", 10);
				}
			}
		}
	}

	/// <summary>
	/// 刷新协助者itemList
	/// </summary>
	void RefreshHelpItemList ()
	{
		foreach (XieZhuJunZhu xiezhu in horseRes.jz)
		{
			Debug.Log ("xiezhu.name:" + xiezhu.name);
		}
		for (int i = 0;i < horseRes.jz.Count;i ++)
		{
			if (horseRes.jz[i].jzId == refuseId)
			{
				horseRes.jz.RemoveAt (i);
				Destroy (helpItemList[i]);
				helpItemList.RemoveAt (i);
			}
		}
		foreach (XieZhuJunZhu xiezhu in horseRes.jz)
		{
			Debug.Log ("xiezhu.name:" + xiezhu.name);
		}
		for (int i = 0;i < helpItemList.Count;i ++)
		{
			Hashtable move = new Hashtable();
			move.Add ("position",posList[i]);
			move.Add ("islocal",true);
			move.Add ("time",0.5f);
			iTween.MoveTo (helpItemList[i],move);
		}
	}

	/// <summary>
	/// 发送协助邀请成功返回
	/// </summary>
	void SendSuccessLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();
		
		titleStr = "提示";
		
		str = "\n\n您已成功发送协助邀请信息，\n请耐心等待您的盟友前来相助";
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, confirm,null,
		             null);
	}

	/// <summary>
	/// 拒绝协助者返回提示
	/// </summary>
	void RefuseHelpLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();

		titleStr = "提示";

		str = "您已拒绝" + refuseName + "的协助！";

		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, confirm,null,
		             null);
	}

	/// <summary>
	/// 警告信息弹出
	/// </summary>
	void ShowWarrningInfo (string str)
	{
		ClientMain.m_UITextManager.createText(MyColorData.getColorString (5,str));
	}
	
	/// <summary>
	/// 开始运镖按钮
	/// </summary>
	public void BeginYunBiaoBtn ()
	{
		if (YunBiaoData.Instance.yunBiaoRes.isOpen)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        YunBiaoAskLoadBack );
		}
		else
		{
			YunBiaoMainPage.yunBiaoMainData.YunBiaoNotOpen ();
		}
	}
	void YunBiaoAskLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate( p_object ) as GameObject).GetComponent<UIBox> ();
		
		titleStr = "提示";

		str = "\n\n确定消耗一个运镖次数吗？\n如果运镖成功，\n收益会通过邮件发送给您。";
		uibox.setBox(titleStr, MyColorData.getColorString (1,str), null,  
		             null, cancel, confirm,
		             YunBiaoAskBack);
	}
	void YunBiaoAskBack (int i)
	{
		if (i == 2)
		{
			//发送运镖请求
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_REQ,"3408");
		}
	}

	/// <summary>
	/// 返回上一界面
	/// </summary>
	public void BackBtn ()
	{
		Destroy (this.gameObject);
	}
	/// <summary>
	/// 关闭镖局
	/// </summary>
	public void CloseBtn ()
	{
		m_ScaleEffectController.OnCloseWindowClick();
		YunBiaoMainPage.yunBiaoMainData.DestroyRoot ();
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
