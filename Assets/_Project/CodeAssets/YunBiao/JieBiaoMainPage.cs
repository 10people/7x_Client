using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class JieBiaoMainPage : MonoBehaviour,SocketProcessor {

	public static JieBiaoMainPage jieBiaoMain;

	/// <summary>
	/// 劫镖首页信息返回
	/// </summary>
	public YabiaoInfoResp jieHuoMainRes;
	private List<YabiaoJunZhuInfo> allJunZhuList = new List<YabiaoJunZhuInfo> ();

	/// <summary>
	/// 君主信息模块
	/// </summary>
	public UISprite headIcon;//君主头像
	public UISprite horseIcon;//马车拖箱
	public UISprite pinZhi;//品质
	public UISprite country;//国家
	public UILabel levelLabel;//等级
	public UILabel junZhuName;//名字
	public UILabel allianceName;//联盟
	public UILabel hpLabel;//血量
	public UILabel zhanLi;//战力

	/// <summary>
	/// 运镖状态模块
	/// </summary>
	public UILabel stateLabel;//运镖状态
	public UILabel protectTime;//保护时间
	private int cdTime;//保护cd
	public GameObject jinDuObj;//包含进度显示的obj
	public UILabel jinDuLabel;//运镖进度
	public UILabel awardLabel;//运镖完成后的奖励
	public UIScrollBar jinDuBar;//显示进度条
	public UIScrollBar hpBar;//血量进度条
	public GameObject changeSkillBtn;//切换技能按钮
	public UILabel addLabel;//数值加成

	/// <summary>
	/// 运镖房间信息模块
	/// </summary>
	public GameObject roomGrid;
	private List<GameObject> roomItemList = new List<GameObject> ();//房间itemList
	public GameObject roomItemObj;//房间item
	public UILabel noRoomDesLabel1;//无运镖房间时的描述
	public UILabel noRoomDesLabel2;
	public GameObject selectBoxObj;//选择框

	public GameObject yaBiaoTeamListObj;//押镖君主item列表 

	private int selectRoomId;//查看的当前运镖房间id

	[HideInInspector]public List<string> roomNameList = new List<string> ();//运镖房间名字

	/// <summary>
	/// 选择的劫镖房间信息
	/// </summary>
	private YabiaoRoomInfo selectRoomInfo;

	private bool isRefresh;//是否可以刷新页面

	private List<long> junZhuIdList = new List<long>();//押镖君主id

	public ScaleEffectController m_ScaleEffectController;

	private bool isOpenChangeMiBao = false;//是否打开选择技能页面
	public bool IsOpenChangeMiBao
	{
		set{isOpenChangeMiBao = value;}
		get{return isOpenChangeMiBao;}
	}

	void Awake ()
	{
		jieBiaoMain = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		selectRoomId = 0;

		JieBiaoMainPageInfoReq ();
	}

	/// <summary>
	/// 劫镖首页信息请求
	/// </summary>
	public void JieBiaoMainPageInfoReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_JIEBIAO_INFO_REQ,"3410");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_JIEBIAO_INFO_RESP://劫镖信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YabiaoInfoResp jieBiaoInfoRes = new YabiaoInfoResp();
				
				t_qx.Deserialize(t_stream, jieBiaoInfoRes, jieBiaoInfoRes.GetType());
				
				if (jieBiaoInfoRes != null)
				{
					if (jieBiaoInfoRes.roomList == null)
					{
						jieBiaoInfoRes.roomList = new List<YabiaoRoomInfo>();
					}
					else
					{
						for (int i = 0;i < jieBiaoInfoRes.roomList.Count;i ++)
						{
//							Debug.Log ("房间id：" + (i + 1) + jieBiaoInfoRes.roomList[i].roomId);

							if (jieBiaoInfoRes.roomList[i].ybjzList == null)
							{
								jieBiaoInfoRes.roomList[i].ybjzList = new List<YabiaoJunZhuInfo>();
							}

							List<YabiaoJunZhuInfo> ybJunZhuInfoList = jieBiaoInfoRes.roomList[i].ybjzList;
//							for (int j = 0;j < ybJunZhuInfoList.Count;j ++)
//							{
////								Debug.Log ("君主id：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].junZhuId);
////								Debug.Log ("君主name：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].junZhuName);
////								Debug.Log ("战力：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].zhanLi);
////								Debug.Log ("血量：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].hp);
////								Debug.Log ("最高血量：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].maxHp);
////								Debug.Log ("收益：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].worth);
////								Debug.Log ("最大收益：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].maxWorth);
////								Debug.Log ("状态：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].state);
////								Debug.Log ("进度：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].usedTime);
////								Debug.Log ("总进度：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].totalTime);
////								Debug.Log ("联盟name：" + (i + 1) + (j + 1) + ":" + ybJunZhuInfoList[j].lianMengName);
//							}
						}
					}

					jieHuoMainRes = jieBiaoInfoRes;

					InItYunBiaoState ();
					InItRoomList ();

					isRefresh = true;

                    if (TimeHelper.Instance.IsTimeCalcKeyExist("RefreshJieBiao"))
					{
                        TimeHelper.Instance.RemoveFromTimeCalc("RefreshJieBiao");
					}
                    TimeHelper.Instance.AddOneDelegateToTimeCalc("RefreshJieBiao", 10);
				}
				
				return true;
			}
			}
		}

		return false;
	}

	void Update ()
	{
        if (TimeHelper.Instance.IsTimeCalcKeyExist("RefreshJieBiao") && isRefresh)
		{
            if (TimeHelper.Instance.IsCalcTimeOver("RefreshJieBiao") && isRefresh)
			{
				JieBiaoMainPageInfoReq ();
				
				isRefresh = false;
			}
		}
	}

	/// <summary>
	/// 初始化君主信息和运镖状态
	/// </summary>
	void InItYunBiaoState ()
	{
//		Debug.Log ("初始化君主信息和运镖状态");
//		Debug.Log ("roomNum:" + jieHuoMainRes.roomList.Count);
		var junZhuInfo = JunZhuData.Instance ().m_junzhuInfo;

		country.spriteName = "nation_" + junZhuInfo.guoJiaId;
		junZhuName.text = junZhuInfo.name;
		levelLabel.text = "Lv" + junZhuInfo.level.ToString ();

		string nameStr = "";
		if (!AllianceData.Instance.IsAllianceNotExist)
		{
			nameStr = "<" + AllianceData.Instance.g_UnionInfo.name + ">";
		}
		else
		{
			nameStr = "无联盟";
		}

		allianceName.text = nameStr;

		allJunZhuList.Clear ();
		for (int i = 0;i < jieHuoMainRes.roomList.Count;i ++)
		{
			List<YabiaoJunZhuInfo> tempInfoList = jieHuoMainRes.roomList[i].ybjzList;
			
			for (int j = 0;j < tempInfoList.Count;j ++)
			{
				allJunZhuList.Add (tempInfoList[j]);
			}
		}

		junZhuIdList.Clear ();
		if (allJunZhuList.Count > 0)
		{
			for (int j = 0;j < allJunZhuList.Count;j ++)
			{
				//					Debug.Log ("junZhuId:" + allJunZhuList[j].junZhuId);
				junZhuIdList.Add (allJunZhuList[j].junZhuId);
				
				if (junZhuIdList.Contains (junZhuInfo.id))
				{
					//						Debug.Log ("包含id");
					jinDuObj.SetActive (true);
					
					headIcon.spriteName = "";//不显示
					horseIcon.spriteName = "horseIcon" + allJunZhuList[j].horseType;
					pinZhi.spriteName = "pinzhi" + (allJunZhuList[j].horseType - 1);
					
					junZhuName.transform.parent.transform.localPosition = new Vector3(-30,25,0);
					
					zhanLi.text = junZhuInfo.zhanLi.ToString ();
					
					int hpNum = (int)((allJunZhuList[j].hp / (float)allJunZhuList[j].maxHp) * 100);
					hpLabel.text = allJunZhuList[j].hp + "/" + allJunZhuList[j].maxHp;
					YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (hpBar,hpNum);

					addLabel.text = MyColorData.getColorString (6, "+" + allJunZhuList[j].huDun) + "%";
					//						Debug.Log ("cdTime :" + allJunZhuList[j].baohuCD);
					//保护时间显示
					if (cdTime == 0)
					{
						cdTime = allJunZhuList[j].baohuCD;
						
						StartCoroutine (ProtectTimeCd ());
					}
					
					string stateStr = "";
					switch (allJunZhuList[j].state)
					{
					case 10:
						protectTime.transform.parent.transform.localPosition = new Vector3(135,35,0);
						stateStr = "进行中";
						break;
						
					case 20:
						protectTime.transform.parent.transform.localPosition = new Vector3(135,35,0);
						stateStr = "应战中";
						break;
						
					case 30:
						protectTime.transform.parent.transform.localPosition = new Vector3(135,50,0);
						stateStr = "保护期";
						break;
					default:
						break;
					}
					
					stateLabel.text = stateStr;
					
					int jinDu = (int)((allJunZhuList[j].usedTime / (float)allJunZhuList[j].totalTime) * 100);
					//						Debug.Log ("jinDu:" + jinDu);
					jinDuLabel.text = "进度" + jinDu.ToString () + "%";
					
					YunBiaoMainPage.yunBiaoMainData.InItScrollBarValue (jinDuBar,jinDu);
					
					awardLabel.text = allJunZhuList[j].worth.ToString ();

					break;
				}
				
				else
				{
					//						Debug.Log ("不包含id");
					headIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
					horseIcon.spriteName = "";//不显示
					pinZhi.spriteName = "";//不显示
					jinDuObj.SetActive (false);
					junZhuName.transform.parent.transform.localPosition = new Vector3(0,10,0);
					protectTime.transform.parent.transform.localPosition = new Vector3(135,35,0);
					stateLabel.text = "您没有在运镖";
				}
			}

			//检测是否有可选技能
			ShowChangeSkillEffect (true);
		}
		else
		{
			headIcon.spriteName = "PlayerIcon" + CityGlobalData.m_king_model_Id;
			horseIcon.spriteName = "";//不显示
			pinZhi.spriteName = "";//不显示
			jinDuObj.SetActive (false);
			junZhuName.transform.parent.transform.localPosition = new Vector3(0,10,0);
			protectTime.transform.parent.transform.localPosition = new Vector3(135,35,0);
			stateLabel.text = "您没有在运镖";
		}
	}

	IEnumerator ProtectTimeCd ()
	{
		while (cdTime > 0) {
			
			cdTime --;

			if (cdTime != 0)
			{
				protectTime.text = "保护期" + cdTime.ToString () + "秒";
			}

			else
			{
				protectTime.text = "";
				JieBiaoMainPageInfoReq ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	/// <summary>
	/// 初始化房间信息
	/// </summary>
	void InItRoomList ()
	{
		foreach (GameObject obj in roomItemList)
		{
			Destroy (obj);
		}
		roomItemList.Clear ();

		YunBiaoMainPage.yunBiaoMainData.MoveBackToTop (roomGrid,jieHuoMainRes.roomList.Count,3);

		if (allJunZhuList.Count > 0)
		{
			for (int i = 0;i < jieHuoMainRes.roomList.Count;i ++)
			{
				if (jieHuoMainRes.roomList[i].ybjzList.Count > 0)
				{
					GameObject roomItem = (GameObject)Instantiate (roomItemObj);
					
					roomItem.SetActive (true);
					roomItem.transform.parent = roomGrid.transform;
					roomItem.transform.localPosition = Vector3.zero;
					roomItem.transform.localScale = Vector3.one;
					
					roomItemList.Add (roomItem);
				}
			}
			
			roomGrid.GetComponent<UIGrid> ().repositionNow = true;
			
			for (int i = 0;i < roomItemList.Count;i ++)
			{
				if (selectRoomId > roomItemList.Count)
				{
					selectRoomId = roomItemList.Count;
				}
				roomItemList[i].GetComponent<RoomItem> ().GetRoomItemInfo (jieHuoMainRes.roomList[i],selectRoomId,i);
			}
			
			noRoomDesLabel1.text = "";
			noRoomDesLabel2.text = "";
		}
		else
		{
			foreach (GameObject obj in roomItemList)
			{
				Destroy (obj);
			}
			roomItemList.Clear ();

			noRoomDesLabel1.text = "当前没有运镖车辆";
			noRoomDesLabel2.text = "当前没有运镖车辆";
			
			yaBiaoTeamListObj.SetActive (false);
		}
	}

	/// <summary>
	/// 克隆选择框
	/// </summary>
	/// <param name="obj">Object.</param>
	public void InstantSelectBox (GameObject obj)
	{
		GameObject selectBox = GameObject.Find ("SelectBoxObj") as GameObject;

		if (selectBox != null)
		{
			Destroy (selectBox);
		}

		selectBox = (GameObject)Instantiate (selectBoxObj);

		selectBox.SetActive (true);

		selectBox.name = "SelectBoxObj";
		selectBox.transform.parent = obj.transform;
		selectBox.transform.localPosition = Vector3.zero;
		selectBox.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// 开始劫镖按钮
	/// </summary>
	public void BeginJieBiaoBtn ()
	{
        //跳转到劫镖场景
    
        CarriageSceneManager.Instance.EnterCarriage (selectRoomId);
        CityGlobalData.m_isJieBiaoScene = true;
    }

	/// <summary>
	/// 获得选中的房间信息
	/// </summary>
	public void GetRoomInfo (YabiaoRoomInfo tempInfo)
	{
		selectRoomInfo = tempInfo;

		selectRoomId = tempInfo.roomId;
	}

	/// <summary>
	/// 切换技能
	/// </summary>
	public void ChangeSkillBtn ()
	{
		if(!MiBaoGlobleData.Instance ().GetEnterChangeMiBaoSkill_Oder ())
		{
			return;
		}
		ShowChangeSkillEffect (false);
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.PVP_CHOOSE_MI_BAO), LoadBack);
		isOpenChangeMiBao = true;
	}
	void LoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject mChoose_MiBao = Instantiate(p_object) as GameObject;
		
		mChoose_MiBao.SetActive(true);
		
		mChoose_MiBao.transform.parent = this.transform.parent;
		
		mChoose_MiBao.transform.localPosition = Vector3.zero;
		
		mChoose_MiBao.transform.localScale = Vector3.one;
		
		ChangeMiBaoSkill mChangeMiBaoSkill = mChoose_MiBao.GetComponent<ChangeMiBaoSkill>();
		
		mChangeMiBaoSkill.Init ((int)(CityGlobalData.MibaoSkillType.YaBiao_Fangshou),jieHuoMainRes.fangyuZuHeId);
	}
	/// <summary>
	/// 返回上一界面按钮
	/// </summary>
	public void BackBtn ()
	{
        if (TimeHelper.Instance.IsTimeCalcKeyExist("RefreshJieBiao"))
		{
            TimeHelper.Instance.RemoveFromTimeCalc("RefreshJieBiao");
		}
		YunBiaoData.Instance.YunBiaoInfoReq ();
		Destroy (this.gameObject);
	}

	/// <summary>
	/// 是否显示切换技能按钮特效
	/// </summary>
	public void ShowChangeSkillEffect (bool isOpen)
	{
		UI3DEffectTool.Instance ().ClearUIFx (changeSkillBtn);

		if (isOpen)
		{
			var mibaoGroupList = MiBaoGlobleData.Instance().G_MiBaoInfo.mibaoGroup;
			
			List<int> zuHeIdList = new List<int> ();
			
			for (int i = 0;i < mibaoGroupList.Count;i ++)
			{
				zuHeIdList.Add (mibaoGroupList[i].zuheId);
			}

			if (!zuHeIdList.Contains (jieHuoMainRes.fangyuZuHeId))
			{
				if (MiBaoGlobleData.Instance ().GetMiBaoskillOpen () && !isOpenChangeMiBao)
				{
					UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,changeSkillBtn,
					                                               EffectIdTemplate.GetPathByeffectId(110006));
				}
			}
		}
	}

	/// <summary>
	/// 关闭镖局按钮
	/// </summary>
	public void CloseBtn ()
	{
        if (TimeHelper.Instance.IsTimeCalcKeyExist("RefreshJieBiao"))
		{
            TimeHelper.Instance.RemoveFromTimeCalc("RefreshJieBiao");
		}
		m_ScaleEffectController.OnCloseWindowClick();
		YunBiaoMainPage.yunBiaoMainData.DestroyRoot ();
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
