using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceData : Singleton<AllianceData>, SocketProcessor
{
    public Dictionary<int, NonAllianceInfo> m_AllianceInfoDic = new Dictionary<int, NonAllianceInfo>();
 
    public int m_AllianceCreatePrice = 0;

    public bool m_InstantiateNoAlliance = false;

	public bool IsAllianceUP = false;

    public bool m_InviteGetData = false ;

    public bool m_Invite_List_Load = false;
    public InviteList m_InviteInfo;
    private bool _isNoAllianceKey = false;
 
    private int index_RemNum = 0;
    private immediatelyJoinResp _AllianceQuickApply = null;
    /// <summary>
    /// 判断是否存在联盟
    /// </summary>
    public bool IsAllianceNotExist
    {
        get { return _isAllianceNotExist; }
        set
        {
            _isAllianceNotExist = value;
        }
    }

    private bool _isAllianceNotExist = true;


    public AllianceHaveResp g_UnionInfo;

    public bool isNewLeader;//是否提示新盟主即位
    private bool Isdismissed = false;

	public int  Hy_Bi;
    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        isNewLeader = true;
        RequestAllianceData();

        //        RequestData();
    }

    void Update()
    {
		if(Hy_Bi !=JunZhuData.Instance ().Hy_Bi)
		{
			Hy_Bi = JunZhuData.Instance ().Hy_Bi;
		}

//        if (Isdismissed)
//        {
//            Isdismissed = false;
//            if (GameObject.Find("My_Union(Clone)") != null)
//            {
//                Destroy(GameObject.Find("My_Union(Clone)"));
//            }
//
//        }
    }

        private static readonly List<int> AllianceAddFuncBtn = new List<int>() { 2000, 2001, 2002 };

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.ALLIANCE_NON_RESP:
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        AllianceNonResp allianceNoRes = new AllianceNonResp();

                        t_qx.Deserialize(t_tream, allianceNoRes, allianceNoRes.GetType());

                        if (allianceNoRes != null)//返回联盟信息， 给没有联盟的玩家返回此条信息
                        {
                            m_AllianceCreatePrice = allianceNoRes.needYuanbao;

                            CityGlobalData.m_AllianceIsHave = false;
                            m_AllianceInfoDic.Clear();
                            IsAllianceNotExist = true;

                            if (allianceNoRes.alincInfo == null)
                            {
                                
                            }
                            else
                            {
                                RefreshAllianceInfo(allianceNoRes);
                            }
                            _isNoAllianceKey = true;
                            if (AllianceLayerManagerment.m_No_AllianceLayer)
                            {
                                m_InstantiateNoAlliance = true;
                            }

                            //Clear tenement data.
                            TenementData.Instance.m_AllianceCityTenementDic.Clear();
                            TenementData.Instance.m_AllianceCityTenementExp = new HouseExpInfo();
                        }

                        if (MainCityUI.m_MainCityUI != null)
                        {
                            MainCityUI.m_MainCityUI.m_MainCityUILT.RefreshAllianceInfo();
                        }
						
						//抢宝箱
						if (TreasureCityUI.m_instance != null)
						{
							TreasureCityUI.m_instance.tCityUITL.RefreshAllianceInfo ();
						}
					
                        if (PlayerNameManager.m_ObjSelfName != null)
                        {
                            PlayerNameManager.UpdateSelfName();
                        }
                        MainCityUI.SetSuperRed(104, true);
                        RequestYaoQing();
                        return true;
                    }

                case ProtoIndexes.ALLIANCE_HAVE_RESP://返回联盟信息， 给有联盟的玩家返回此条信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        AllianceHaveResp allianceHaveRes = new AllianceHaveResp();

                        t_qx.Deserialize(t_tream, allianceHaveRes, allianceHaveRes.GetType());

                        if (allianceHaveRes != null)
                        {
                            //					Debug.Log("接收有联盟信息！");

                            g_UnionInfo = allianceHaveRes;

                            for (int i = 0; i < allianceHaveRes.memberInfo.Count; i++)
                            {
                                if (allianceHaveRes.memberInfo[i].identity == 2)
                                {
                                    //Debug.Log("SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS");
                                    AllianceEffigyManagerment.m_EffigyRoleId = allianceHaveRes.memberInfo[i].roleId;
                                    AllianceEffigyManagerment.m_EffigyInfoGet = true;
                                    break;
                                }
                            }
                            IsAllianceNotExist = false;

                            CityGlobalData.m_AllianceIsHave = true;

                            //                            foreach (var item in AllianceAddFuncBtn)
                            //                            {
                            //                                if (!FunctionOpenTemp.m_EnableFuncIDList.Contains(item))
                            //                                {
                            //									Debug.Log("=============1");
                            //									Debug.Log(item);
                            //                                    FunctionOpenTemp.m_EnableFuncIDList.Add(item);
                            //                                }
                            //                            }
                            ApplicateAllianceReq(g_UnionInfo);
                            //					GameObject allianceObj = GameObject.Find("My_Union(Clone)");
                            //					if (allianceObj != null)
                            //					{
                            //						Debug.Log("更新联盟信息！");
                            //
                            //						MyAllianceMain myAllianceMain = allianceObj.GetComponent<MyAllianceMain>();
                            //						myAllianceMain.g_UnionInfo = allianceHaveRes;
                            //						myAllianceMain.InItMyAlliance();
                            //						
                            //						AllMembersData memberData = allianceObj.GetComponent<AllMembersData>();
                            //						memberData.GetAllianceInfo(allianceHaveRes);
                            //					}

                            //					Debug.Log ("联盟名字：" + allianceHaveRes.name);
                            //					Debug.Log ("联盟id：" + allianceHaveRes.id);
                            //					Debug.Log ("联盟等级：" + allianceHaveRes.level);
                            //					Debug.Log ("当前经验：" + allianceHaveRes.exp);
                            //					Debug.Log ("升级经验：" + allianceHaveRes.needExp);
                            //					Debug.Log ("建设：" + allianceHaveRes.build);
                            //					Debug.Log ("成员数量：" + allianceHaveRes.members);
                            //					Debug.Log ("最大成员数量：" + allianceHaveRes.memberMax);
                            //					Debug.Log ("贡献：" + allianceHaveRes.contribution);
                            //					Debug.Log ("公告：" + allianceHaveRes.notice);
                            //					Debug.Log ("身份：" + allianceHaveRes.identity);
                            //					Debug.Log ("是否开启招募：" + allianceHaveRes.isAllow);
                            //					Debug.Log ("盟主名字：" + allianceHaveRes.mengzhuName);
                        }

                        if (MainCityUI.m_MainCityUI != null)
                        {
                            MainCityUI.m_MainCityUI.m_MainCityUILT.RefreshAllianceInfo();
                        }

						//抢宝箱
						if (TreasureCityUI.m_instance != null)
						{
							TreasureCityUI.m_instance.tCityUITL.RefreshAllianceInfo ();
						}

                        if (PlayerNameManager.m_ObjSelfName != null)
                        {
                            PlayerNameManager.UpdateSelfName();
                        }
                        MainCityUI.SetSuperRed(104, false);
                        MainCityUI.SetButtonNum(104, 0);
                        return true;
                    }

                case ProtoIndexes.ALLIANCE_FIRE_NOTIFY://被联盟开除通知
                    {
                        CityGlobalData.m_AllianceIsHave = false;
                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
                        if (g_UnionInfo.identity != 2)
                        {
                            RequestData();

                            // m_AllianceAppliedCount = 0;
                            g_UnionInfo = null;
                            Isdismissed = true;
                            JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;//new add
                            IsAllianceNotExist = true;
							MainCityUI.SetSuperRed(104, true);
                            //if (!CityGlobalData.m_isJieBiaoScene && BattleControlor.Instance() == null)
                            //{
                            //    CityGlobalData.m_isAllianceScene = false;
                            //    CityGlobalData.m_isMainScene = true;
                            //    SceneManager.EnterMainCity();
                            //}
                        }

                        return true;
                    }

                case ProtoIndexes.ALLIANCE_HAVE_NEW_APPLYER://主界面联盟按钮提示
                    {
                        //                        Debug.Log("联盟动态：" + ProtoIndexes.ALLIANCE_HAVE_NEW_APPLYER);
                        //                        Debug.Log("联盟index：" + 104);
                        MainCityUI.SetRedAlert(104, true);
                        return true;
                    }

                case ProtoIndexes.ALLIANCE_ALLOW_NOTIFY://加入联盟被批准通知
                    {
                        if (!CityGlobalData.m_isJieBiaoScene && BattleControlor.Instance() == null)
                        {
                            _isNoAllianceKey = false;
                            CityGlobalData.m_isMainScene = false;
                            // CityGlobalData.m_isAllianceScene = true;
                            // SceneManager.EnterAllianceCity();
                            if (Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_MAINCITY))
                            {
                                JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                                GameObject obj = new GameObject();
                                obj.name = "MainCityUIButton_104";
                                MainCityUI.m_MainCityUI.MYClick(obj);
                            }
                            else
                            {
								RequestData ();
                                ClientMain.m_UITextManager.createText("您的申请已被批准！您已成功加入联盟！");
                            }
                            JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;//new add
                        }
                        return true;
                    }
                case ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY://主界面联盟按钮提示
                    {
                        //                        Debug.Log("联盟升级：" + ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY);

                        IsAllianceUP = true;

                        return true;
                    }

                case ProtoIndexes.ALLIANCE_DISMISS_NOTIFY://联盟被解散成功
                    {
                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
						{
							//去掉商铺联盟相关红点
//							PushAndNotificationHelper.SetRedSpotNotification (600700,false);//贡献商铺
//							PushAndNotificationHelper.SetRedSpotNotification (903,false);//荒野商店
					        CloseAllRedPoints();
						}
						
						if(MainCityUI.m_MainCityUI.m_WindowObjectList.Count>=1)
						{
							DisAllianceSuccessBack(0);
						}
                        if (!CityGlobalData.m_isJieBiaoScene && BattleControlor.Instance() == null)
                        {
                            for (int i = 0; i < g_UnionInfo.memberInfo.Count; i++)
                            {
                                if (g_UnionInfo.memberInfo[i].identity == 2)
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                        DisAllianceLoadCallback);
                                    return true;
                                }
                            }

                            //SceneManager.EnterMainCity();
                        }
                        return true;
                        break;
                    }
                case ProtoIndexes.S_JIAN_ZHU_INFO:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        JianZhuList mJianZhuList = new JianZhuList();

                        t_qx.Deserialize(t_stream, mJianZhuList, mJianZhuList.GetType());

                        //				Debug.Log("请求建筑返回11");

                        return true;
                    }
                case ProtoIndexes.S_JIAN_ZHU_UP:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        ErrorMessage BuildUpback = new ErrorMessage();

                        t_qx.Deserialize(t_stream, BuildUpback, BuildUpback.GetType());

                        //				Debug.Log("BuildUpback main - -   ");

                        return true;
                    }
                case ProtoIndexes.S_ALLIANCE_INVITE_RESP: /** 同意联盟邀请 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        immediatelyJoinResp AllianceQuickApply = new immediatelyJoinResp();
                        t_qx.Deserialize(t_tream, AllianceQuickApply, AllianceQuickApply.GetType());
                        _AllianceQuickApply = AllianceQuickApply;
                        switch ((AllianceConnectRespEnum)AllianceQuickApply.code)
                        {
                            case AllianceConnectRespEnum.E_ALLIANCE_ZERO:
                                {
                                    if (Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_MAINCITY))
                                    {
                                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 1;
                                        GameObject obj = new GameObject();
                                        obj.name = "MainCityUIButton_104";
                                        MainCityUI.m_MainCityUI.MYClick(obj);
                                    }
                                    else
                                    {
                                        ClientMain.m_UITextManager.createText("联盟加入成功！");
										//十连副本刷新
										if (Application.loadedLevelName.Equals(ConstInGame.CONST_SCENE_NAME_TREASURECITY))
										{
											RequestData ();
										}
                                    }
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_ONE:
                                {
                                    ClientMain.m_UITextManager.createText("失败：联盟需要审批！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_TWO:
                                {

                                    ClientMain.m_UITextManager.createText("未能加入联盟！该联盟已经解散！");

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_THREE:
                                {
                                    ClientMain.m_UITextManager.createText("玩家申请的联盟数量已经满了！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FOUR:
                                {
                                    ClientMain.m_UITextManager.createText("失败，联盟未开启招募！");

                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_FIVE:
                                {
                                    ClientMain.m_UITextManager.createText("失败，该联盟人数已经满员！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SIX:
                                {
                                    ClientMain.m_UITextManager.createText("失败君主等级不满足！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_SEVEN:
                                {
                                    ClientMain.m_UITextManager.createText("失败，军衔等级不满足！");
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_EIGHT:
                                {
                                    Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_SWITCH_COUNTRY),
                                            ResLoaded);
                                }
                                break;
                            case AllianceConnectRespEnum.E_ALLIANCE_NINE:
                                {
                                    ClientMain.m_UITextManager.createText("间隔时间不到！");
                                }
                                break;


                            default:
                                break;
                        }

                        return true;
                    }
                case ProtoIndexes.S_ALLIANCE_INVITE_LIST: /** 邀请的联盟列表 **/
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        InviteList invitetem = new InviteList();
                   
                        t_qx.Deserialize(t_tream, invitetem, invitetem.GetType());
                        m_InviteInfo = invitetem;
                        if (AllianceLayerManagerment.m_No_AllianceLayer != null)
                        {
                            m_Invite_List_Load = true;
                        }
                        else
                        {
                            if (IsAllianceNotExist)
                            {
                                //if (invitetem.inviteInfo != null)
                                //{
                                //    PushAndNotificationHelper.IsShowRedSpotNotification(104);
                                //}
                                //else
                                //{
                                //    PushAndNotificationHelper.SetRedSpotNotification(104, false);
                                //}
                            }
                            
                        }

                        if (invitetem.inviteInfo != null && IsAllianceNotExist)
                        {
                            MainCityUI.SetButtonNum(104, invitetem.inviteInfo.Count);
                        }
                        else
                        {
                            MainCityUI.SetButtonNum(104, 0);
                        }
                       
                        return true;
                    }
                //case ProtoIndexes.S_ALLIANCE_INVITE_REFUSE: /** 拒绝联盟邀请返回 **/
                //    {
                //        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                //        QiXiongSerializer t_qx = new QiXiongSerializer();

                //        RefuseInvite invitetem = new RefuseInvite();
                //        t_qx.Deserialize(t_tream, invitetem, invitetem.GetType());

                //        return true;
                //    }
                case ProtoIndexes.S_ALLIANCE_INVITE:// 邀请某人加入联盟请求返回
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        AllianceInviteResp inviteResp = new AllianceInviteResp();

                        t_qx.Deserialize(t_stream, inviteResp, inviteResp.GetType());
                        m_InviteGetData = true;
                        switch (inviteResp.result)
                        {
                            case 0:
                                {
                                    ClientMain.m_UITextManager.createText("邀请成功！");
                                }
                                break;
                            case 1:
                                {
                                    ClientMain.m_UITextManager.createText("君主不存在！");
                                }
                                break;
                            case 2:
                                {
                                    ClientMain.m_UITextManager.createText("联盟人数已满！");
                                }
                                break;
                            case 3:
                                {
                                    ClientMain.m_UITextManager.createText("此玩家尚未开启联盟功能，无法邀请入盟！");
                                }
                                break;
                            case 4:
                                {
                                    ClientMain.m_UITextManager.createText("该玩家已经是其他联盟成员,无法邀请入盟。");
                                }
                                break;
                            case 5:
                                {
                                    ClientMain.m_UITextManager.createText("你没有权限邀请别人加入联盟！");
                                }
                                break;
                            case 6:
                                {
                                    ClientMain.m_UITextManager.createText("该玩家已是我盟的一员！");
                                }
                                break;
                            case 7:
                                {
                                    ClientMain.m_UITextManager.createText("对方未开启联盟！");
                                }
                                break;
                        }
                        return true;
                    }
			
                default: return false;
            }

        }
        return false;
    }
    void ResLoaded(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
      GameObject tempObject = (GameObject)Instantiate(p_object);
      tempObject.GetComponent<AllianceSwitchCountryManagerment>().ShowInfo(_AllianceQuickApply.guojiaId, _AllianceQuickApply.lmId);
      MainCityUI.TryAddToObjectList(tempObject);
      UIYindao.m_UIYindao.CloseUI();
    }

    public void DisAllianceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		//PlayerModelController.m_playerModelController.m_isCanUpdatePosition = false;
		//Vector3 vec_pos = new Vector3(-1.0f, 0.4f, -63.0f);
		//PlayerModelController.m_playerModelController.UploadPlayerPosition(vec_pos);
		CityGlobalData.m_isMainScene = true;
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR1)+"\n";
		string str2 = g_UnionInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR2);

		string jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		uibox.setBox(jieSanTitleStr, MyColorData.getColorString (1,str1+str2), null,null,confirmStr,null,DisAllianceSuccessBack);
		
	}
	void DisAllianceSuccessBack (int i)
    {
        AllianceData.Instance.IsAllianceNotExist = true;
		MainCityUI.SetSuperRed(104, true);
        QXChatUIBox.chatUIBox.SetSituationState();
        GameObject uirot = GameObject.Find("New_My_Union(Clone)");
		
		if(uirot)
		{
			MainCityUI.TryRemoveFromObjectList(uirot);
			Destroy(uirot);
		}
		 JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
		CityGlobalData.m_isAllianceScene = false;
		CityGlobalData.m_isMainScene = true;

		//SceneManager.EnterMainCity();
	}
    public void RequestData()
    {
         SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
    }

    public void RequestCurrentData()
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ, true, p_receiving_wait_proto_index: ProtoIndexes.ALLIANCE_NON_RESP);
    }
    void CloseAllRedPoints()
	{
		PushAndNotificationHelper.SetRedSpotNotification(600700, false);//贡献商铺
		PushAndNotificationHelper.SetRedSpotNotification(903, false);//荒野商店
		PushAndNotificationHelper.SetRedSpotNotification(500050, false);//XiaoWuid
		PushAndNotificationHelper.SetRedSpotNotification(400000, false);//Mobai
		PushAndNotificationHelper.SetRedSpotNotification(400017, false);//fangcan
		PushAndNotificationHelper.SetRedSpotNotification(600900, false);//ChouJiangid1
		PushAndNotificationHelper.SetRedSpotNotification(600905, false);//ChouJiangid2
		PushAndNotificationHelper.SetRedSpotNotification(300200, false);//HYid
		PushAndNotificationHelper.SetRedSpotNotification(600500, false);//Evet
		PushAndNotificationHelper.SetRedSpotNotification(600600, false);//Readroom
		PushAndNotificationHelper.SetRedSpotNotification(600800, false);//是否显示经验
		PushAndNotificationHelper.SetRedSpotNotification(500020, false);//国家
		PushAndNotificationHelper.SetRedSpotNotification(500022, false);//国家
	}
    public void RefreshAllianceInfo(AllianceNonResp tempAllianceInfo)
    {
        for (int i = 0; i < tempAllianceInfo.alincInfo.Count; i++)
        {
            m_AllianceInfoDic.Add(tempAllianceInfo.alincInfo[i].id, tempAllianceInfo.alincInfo[i]);
        }

    }
	//查看入盟申请成员请求
	void ApplicateAllianceReq (AllianceHaveResp tempInfo)
	{
		if(tempInfo.identity == 0)
		{
			return;
		}
		LookApplicants applicateReq = new LookApplicants ();
		
		applicateReq.id = tempInfo.id;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,applicateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.LOOK_APPLICANTS,ref t_protof);
	}

	void OnDestroy(){
        SocketTool.UnRegisterMessageProcessor(this);

		base.OnDestroy();
	}

    public void RequestAppliactionAlliance(int id)//申请加入联盟
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        ApplyAlliance allianceApplication = new ApplyAlliance();
        allianceApplication.id = id;
        t_qx.Serialize(t_tream, allianceApplication);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.APPLY_ALLIANCE, ref t_protof);
    }

    public void RequestJoinAlliance(int id)//立即加入联盟
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        CancelJoinAlliance allianceApplyCancel = new CancelJoinAlliance();
        allianceApplyCancel.id = id;

        t_qx.Serialize(t_tream, allianceApplyCancel);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.IMMEDIATELY_JOIN, ref t_protof);
    }

    public void RequestYaoQing()//联盟邀请列表
    {
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE_LIST);
    }
    public void RequestRefuseInvite(int id)//拒绝联盟邀请
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        RefuseInvite refuse = new RefuseInvite();
        refuse.id = id;
        t_qx.Serialize(t_tream, refuse);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE_REFUSE, ref t_protof);
    }
    public int m_agree_id = 0;
    public void RequestAgreeInvite(int id)//同意联盟邀请
    {
        MemoryStream t_tream = new MemoryStream();
        QiXiongSerializer t_qx = new QiXiongSerializer();
        AgreeInvite agree = new AgreeInvite();
        agree.id = id;
        m_agree_id = id;
        t_qx.Serialize(t_tream, agree);
        byte[] t_protof;
        t_protof = t_tream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE_AGREE, ref t_protof);
    }

    public void RequestAllianceData()
    {

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
    }
    public void RequestAllianceInvite(long _junzhuid)////联盟邀请
    {
        AllianceInvite green = new AllianceInvite();
        green.junzhuId = _junzhuid;
        MemoryStream t_stream = new MemoryStream();
        QiXiongSerializer q_serializer = new QiXiongSerializer();
        q_serializer.Serialize(t_stream, green);
        byte[] t_protof = t_stream.ToArray();
        SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_INVITE, ref t_protof);
    }

}
