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

    public bool NoAlliance = false;
    // public int m_AllianceAppliedCount = 0;
    public int m_AllianceCreatePrice = 0;

    public bool m_InstantiateNoAlliance = false;

	public bool IsAllianceUP = false;

    private bool _isNoAllianceKey = false;
 
    private int index_RemNum = 0;

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

    private bool _isAllianceNotExist;


    public AllianceHaveResp g_UnionInfo;

    public bool isNewLeader;//是否提示新盟主即位
    private bool Isdismissed = false;

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

    void Start()
    {
        isNewLeader = true;
//        RequestData();
    }

    //void Update()
    //{
    //    if (Isdismissed)
    //    {
    //        Isdismissed = false;
    //        if (GameObject.Find("My_Union(Clone)") != null)
    //        {
    //            Destroy(GameObject.Find("My_Union(Clone)"));
    //        }
           
    //    }
    //}

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
                                NoAlliance = true;
                            }
                            else
                            {
                                RefreshAllianceInfo(allianceNoRes);
                            }
                            _isNoAllianceKey = true;
                            m_InstantiateNoAlliance = true;

                            //Clear tenement data.
                            TenementData.Instance.m_AllianceCityTenementDic.Clear();
                            TenementData.Instance.m_AllianceCityTenementExp = new HouseExpInfo();
                        }

                        if (MainCityUI.m_MainCityUI != null)
                        {
                            MainCityUI.m_MainCityUI.m_MainCityUILT.RefreshAllianceInfo();
                        }

                        return true;
                    }

                case ProtoIndexes.ALLIANCE_HAVE_RESP://返回联盟信息， 给有联盟的玩家返回此条信息
                    {
                        MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        AllianceHaveResp allianceHaveRes = new AllianceHaveResp();

                        t_qx.Deserialize(t_tream, allianceHaveRes, allianceHaveRes.GetType());

                        //                Debug.Log("_isNoAllianceKey_isNoAllianceKey_isNoAllianceKey_isNoAllianceKey_isNoAllianceKey " + m_InstantiateNoAlliance);

                        //				if (_isNoAllianceKey)
                        //                {
                        //                    _isNoAllianceKey = false;
                        //                    SceneManager.EnterAllianceCity();
                        //                }

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

                            foreach (var item in AllianceAddFuncBtn)
                            {
                                if (!FunctionOpenTemp.m_EnableFuncIDList.Contains(item))
                                {
                                    FunctionOpenTemp.m_EnableFuncIDList.Add(item);
                                }
                            }
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
                            if (!CityGlobalData.m_isJieBiaoScene && BattleControlor.Instance() == null)
                            {
                                CityGlobalData.m_isAllianceScene = false;
                                CityGlobalData.m_isMainScene = true;
                                SceneManager.EnterMainCity();
                            }
                        }

                        return true;
                    }

                case ProtoIndexes.ALLIANCE_HAVE_NEW_APPLYER://主界面联盟按钮提示
                    {
                        Debug.Log("联盟动态：" + ProtoIndexes.ALLIANCE_HAVE_NEW_APPLYER);
                        Debug.Log("联盟index：" + 104);
                        MainCityUIRB.SetRedAlert(104, true);
                        return true;
                    }

                case ProtoIndexes.ALLIANCE_ALLOW_NOTIFY://加入联盟被批准通知
                    {
                        if (!CityGlobalData.m_isJieBiaoScene && BattleControlor.Instance() == null)
                        {
                            _isNoAllianceKey = false;
                            CityGlobalData.m_isMainScene = false;
                           // CityGlobalData.m_isAllianceScene = true;
                            SceneManager.EnterAllianceCity();
                        }
                        return true;
                    }
                case ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY://主界面联盟按钮提示
                    {
                        Debug.Log("联盟升级：" + ProtoIndexes.ALLIANCE_LEVEL_UP_NOTIFY);

                        IsAllianceUP = true;

                        return true;
                    }

                case ProtoIndexes.ALLIANCE_DISMISS_NOTIFY://联盟被解散成功
                    {
                        JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
               
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

                            SceneManager.EnterMainCity();
                        }
                        return true;
                        break;
                    }
                default: return false;
            }
        }
        return false;
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
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR1);
		string str2 = g_UnionInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_JIESAN_SUCCESS_STR2);
		string jieSanTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		uibox.setBox(jieSanTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2),null,confirmStr,null,DisAllianceSuccessBack);
		
	}
	void DisAllianceSuccessBack (int i)
	{
        GameObject uirot = GameObject.Find("_My_Union(Clone)");
		
		if(uirot)
		{
			MainCityUI.TryRemoveFromObjectList(uirot);
			Destroy(uirot);
		}
		//JunZhuData.Instance ().m_junzhuInfo.lianMengId = 0;
		CityGlobalData.m_isAllianceScene = false;
		CityGlobalData.m_isMainScene = true;
		SceneManager.EnterMainCity();
	}
    public void RequestData()
    {
//        Debug.Log("30102");

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
    }

    public void RefreshAllianceInfo(AllianceNonResp tempAllianceInfo)
    {
//        Debug.Log("tempAllianceInfotempAllianceInfo ::" + tempAllianceInfo.alincInfo.Count);

        for (int i = 0; i < tempAllianceInfo.alincInfo.Count; i++)
        {
//            Debug.Log(" tempAllianceInfo.alincInfo[i].name tempAllianceInfo.alincInfo[i].name" + tempAllianceInfo.alincInfo[i].name);
          
            //if (tempAllianceInfo.alincInfo[i].isApplied)
            //{
            //    m_AllianceAppliedCount++;
            //}
            m_AllianceInfoDic.Add(tempAllianceInfo.alincInfo[i].id, tempAllianceInfo.alincInfo[i]);
        }

        NoAlliance = true;
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
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LOOK_APPLICANTS,ref t_protof,"30124");

//		Debug.Log ("ApplicateReq" + ProtoIndexes.LOOK_APPLICANTS);
	}
    void OnDestroy()
    {
        SocketTool.UnRegisterMessageProcessor(this);
    }
}
