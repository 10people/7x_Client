using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class NewAlliancemanager : MonoBehaviour, SocketListener
{
	public GameObject JunChengBattleAlert;

    public bool mYinDaoisOpen = false;
    public GameObject RecruteBtn;
    private bool HaveappyMembers;
    private string titleStr;
    private string str2;
    private string jieSanTitleStr;
    private string closeTitleStr;
    private string cancelStr;
    private string confirmStr;

    public AllianceHaveResp m_allianceHaveRes;

	private UpgradeLevelInfoResp m_UpgradeLevelInfoResp;

    public JianZhuList m_JianZhu;


    public UILabel AllianceName;

    public UILabel OnlineNum;

    public UILabel Builds;

    public UILabel Label_LeaderBtn;

	public UISprite AllianceCountryChange;
	public GameObject AllianceCountryChangebtn;

    public GameObject[] mBUILDS;

    public GameObject DeiInfoMation;

    public GameObject JuanXianUI;

    public GameObject Appbtn;
	public UISprite AppbtnSprite;
    public GameObject AppbtnAlert;
    public GameObject NoticeBox;

    ExitAllianceResp mexitResp;

    public GameObject All_KeZhan;
    public GameObject All_FirstUI;
    public GameObject All_ReadRoom;
    public GameObject All_Temples;
    public GameObject All_Apply;

    public GameObject NeedScoleUI;

    public List<AllBuildsTmp> AllBuildsTmp_List = new List<AllBuildsTmp>();

    public static NewAlliancemanager m_NewAlliancemanager;

    public List<UIEventListener> BtnList = new List<UIEventListener>();

    [HideInInspector]
    public int KejiLev;
    [HideInInspector]
    public int KezhanLev;
    [HideInInspector]
    public int TutengLev;
    [HideInInspector]
    public int ZongmiaoLev;
    [HideInInspector]
    public int ShangPuLev;
    public MyAllianceInfo mMyAllianceInfo;
    public int Up_id;

    public GameObject KeZhan;
    public GameObject Temples;
    public GameObject Apply;
    public GameObject XiaoWu;
    public GameObject HyQs;
    public GameObject ReadRoom;
    public GameObject BuildUpVecotry;
	public GameObject mindYouMenbers;
    public static NewAlliancemanager Instance()
    {
        if (!m_NewAlliancemanager)
        {
            m_NewAlliancemanager = (NewAlliancemanager)GameObject.FindObjectOfType(typeof(NewAlliancemanager));
        }
        return m_NewAlliancemanager;
    }

    void Awake()
    {
        BtnList.ForEach(item => SetBtnMoth(item));
        SocketTool.RegisterSocketListener(this);
    }
    void Start()
    {
//        AllianceData.Instance.RequestData();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ,ProtoIndexes.ALLIANCE_HAVE_RESP.ToString());

        jieSanTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIACNE_JIESAN_TITLE);
        closeTitleStr = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_CLOSE_RECRUIT_TITLE);
        confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        GetAllianceBuildsMessege();
        //Init ();
    }
    void SetBtnMoth(UIEventListener mUIEventListener)
    {
        mUIEventListener.onClick = BtnManagerMent;
    }
    public void BtnManagerMent(GameObject mbutton)
    {
//        Debug.Log("mbutton.name = " + mbutton.name);
        switch (mbutton.name)
        {
            case "LookIfo": // 查看信息
                CheckInfo();
                break;
            case "BattleBtn": // 郡城战
                EnterBattleBtn();
              
                break;
            case "ApplyInfo": // 申请信息  只要盟主和副盟主能1 2
                ApplyInfo();
                break;
            case "RecurteBtm": //招募信息  所有成员都能点击
                ReCruitSetting();
                break;
            case "closebtn (1)": //关闭按钮
                Close();
                break;
            case "Help": //帮助按钮
                Help();
                break;
            case "GetPath": //显示获得建设值的途径
                GetPathOfBuilds();
                break;
            case "UPLevelAndAddSpBtn": //升级和加速按钮
                AllianceUpBtn();
                break;
            case "Leaders": //查看消耗行情
                LookUpLevelNeedBuildsBtn();
                break;

            default:
                break;
        }
    }
    void EnterBattleBtn()
    {
		CityWarData.Instance.OpenCityWar(CityWarData.CityWarType.NORMAL_CITY);
    }
	void GetPathOfBuilds()
	{
		Titie = "建设值获取途径";
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_LEADER_SETTINGS ),
		                        LeaderSettingsLoadCallback );
	}
	string Titie;
	GameObject LederSet;
	public void LeaderSettingsLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		if(LederSet == null)
		{
			LederSet = Instantiate( p_object ) as GameObject;
			
			LederSet.transform.localScale = Vector3.one;
			LederSet.transform.localPosition = new Vector3 (500,200,0);
			LianmengMuBiaomanager mLianmengMuBiaomanager = LederSet.GetComponent<LianmengMuBiaomanager>();
			//mLianmengMuBiaomanager.Lianmeng_Alliance = m_Alliance;
			
			mLianmengMuBiaomanager.Init(Titie);
			MainCityUI.TryAddToObjectList(LederSet,false);
		}
//		if(LederSet == null)
//		{
//			LederSet = Instantiate( p_object ) as GameObject;
//			
//			LederSet.transform.localScale = Vector3.one;
//			LederSet.transform.localPosition = new Vector3 (500,200,0);
//			ChooseCountry mChooseCountry = LederSet.GetComponent<ChooseCountry>();
//			//mLianmengMuBiaomanager.Lianmeng_Alliance = m_Alliance;
//			//int x = (int)(mChooseCountry.ChooseType.Choose);
//			mChooseCountry.Init(2);
//			MainCityUI.TryAddToObjectList(LederSet,false);
//		}
		
	}


    void AllianceUpBtn() // 联盟升级 或者加速
    {
		int CostJianSheZhi = LianMengTemplate.GetLianMengTemplate_by_lv(m_allianceHaveRes.level).cost;
		bool UpLevel = ShengJi;
		if(UpLevel)
		{
			if(CostJianSheZhi <= m_allianceHaveRes.build)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpAllianceLevelremind);
			}
			else
			{
				Titie = "建设值不足";
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_LEADER_SETTINGS ),
				                        LeaderSettingsLoadCallback );
			}
		}
		else
		{
			if(m_allianceHaveRes.speedUpRemainTimes <= 0)
			{
				ClientMain.m_UITextManager.createText("加速次数不足，已无法加速！");
			}
			else
			{
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_UPINFO_SPEEDUP,ProtoIndexes.S_ALLIANCE_UPINFO_SPEEDUP.ToString());
			}

		}

    }

	void UpAllianceLevelremind(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "联盟升级";
		int needbuild =  LianMengTemplate.GetLianMengTemplate_by_lv(m_allianceHaveRes.level).cost;
		int time = LianMengTemplate.GetLianMengTemplate_by_lv (m_allianceHaveRes.level).lvlupTime ;
		string str = "联盟等级"+m_allianceHaveRes.level.ToString()+"级升"+(m_allianceHaveRes.level+1).ToString()+"级"+"\r\n"+"需要消耗建设值："+needbuild.ToString()+"\r\n"+"需要消耗时间："+CalculatorTime(time).ToString();
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null ,null,CancleBtn,confirmStr,SendMassegeforUpLevel,null,null,null);
	}

	void SendMassegeforUpLevel(int i)
	{
		if(i == 2)
		{
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_UPGRADE_LEVEL);
		}
	}
	GameObject mUIBox;
	void UpAllianceLevelAspeedremind(ref WWW p_www,string p_path, Object p_object)
	{
		if(mUIBox == null)
		{
			mUIBox = (GameObject.Instantiate(p_object) as GameObject);
			UIBox uibox = mUIBox.GetComponent<UIBox>();
			string titleStr = "联盟加速";

			string mT = getmTime(m_UpgradeLevelInfoResp.mTime);
			int mCos = m_UpgradeLevelInfoResp.mBuild;
			string str = "加速可以缩短"+mT+"升级时间"+"\n\r"+"消耗建设值："+mCos.ToString()+"\n\r"+"剩余加速次数："+m_allianceHaveRes.speedUpRemainTimes.ToString();
			
			string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
			
			string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null ,null,CancleBtn,confirmStr,SendMassegeforSpeedLevel,null,null,null);
		}
	    
	}
	string getmTime(int m_t)
	{
		string st = "";
		int f = m_t % 3600;
		if(f == 0)
		{
			int result  =  m_t/ 3600;
			st = result.ToString()+"小时";
		}else{
			int result1  =  m_t/ 3600;
			int result2  =  m_t% 3600;
			int result3 =  result2/ 60;
			st = result1.ToString()+"小时"+result3.ToString()+"分";
		}
		return st;
	}
	void SendMassegeforSpeedLevel(int i)
	{
		if(i == 2)
		{
			Debug.Log("m_UpgradeLevelInfoResp.mBuild = "+m_UpgradeLevelInfoResp.mBuild);
			Debug.Log("m_allianceHaveRes.build = "+m_allianceHaveRes.build);
			if(m_UpgradeLevelInfoResp.mBuild > m_allianceHaveRes.build)
			{
				Titie = "建设值不足";
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_LEADER_SETTINGS ),
				                        LeaderSettingsLoadCallback );
			}
			else
			{
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_ALLIANCE_UPGRADELEVEL_SPEEDUP,ProtoIndexes.S_ALLIANCE_UPGRADELEVEL_SPEEDUP.ToString());
			}
		}
	}
	void AddSpeedComfirm(ref WWW p_www,string p_path, Object p_object)
	{
		if(mUIBox == null)
		{
			mUIBox = (GameObject.Instantiate(p_object) as GameObject);
			UIBox uibox = mUIBox.GetComponent<UIBox>();
			string titleStr = "建设值不足";

			string str = "建设值不足";
			
			string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
			
			string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null ,null,confirmStr,null,null,null,null);
		}
		
	}
    void LookUpLevelNeedBuildsBtn()
    {


    }
    //查看入盟申请成员请求
    void ApplicateAllianceReq(AllianceHaveResp tempInfo)
    {
        LookApplicants applicateReq = new LookApplicants();

        applicateReq.id = tempInfo.id;

        MemoryStream t_stream = new MemoryStream();

        QiXiongSerializer t_serializer = new QiXiongSerializer();

        t_serializer.Serialize(t_stream, applicateReq);

        byte[] t_protof = t_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.LOOK_APPLICANTS, ref t_protof, "30124");
        Debug.Log("ApplicateReq" + ProtoIndexes.LOOK_APPLICANTS);
    }
    void OnDestroy()
    {
        SocketTool.UnRegisterSocketListener(this);

        m_NewAlliancemanager = null;
    }
	string CalculatorTime(int mTime)
	{
		string mstr = "";
		Debug.Log ("mTime = "+mTime);
		if(mTime < 24)
		{
			mstr = mTime.ToString()+"小时";
		}

		else{

			int d = mTime/24;
			int h = mTime%24;
			
			mstr = d.ToString()+"天"+h.ToString()+"小时";
		}
		return mstr;
	}
    void Update()
    {
        //Shownotice ();
    }

    public List<Collider> mainColliders = new List<Collider>();

    /// <summary>
    /// Shows the alliance GUID.联盟引导
    /// </summary>
    public void ShowAllianceGuid()
    {
		return;
        SomeUIis_OPen = false;
        UIYindao.m_UIYindao.CloseUI();
        //Debug.Log("联盟引导");
        if (FreshGuide.Instance().IsActive(400010) && TaskData.Instance.m_TaskInfoDic[400010].progress >= 0)
        {
            //			Debug.Log("去小屋领经验");
            mYinDaoisOpen = true;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400010];
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
            StartCoroutine("SetmButtonEnabel");
        }
        else if (FreshGuide.Instance().IsActive(400020) && TaskData.Instance.m_TaskInfoDic[400020].progress >= 0)
        {
            //			Debug.Log("去寺庙祭拜");
            mYinDaoisOpen = true;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400020];
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
            StartCoroutine("SetmButtonEnabel");
        }
        else if (FreshGuide.Instance().IsActive(400030) && TaskData.Instance.m_TaskInfoDic[400030].progress >= 0)
        {
            //			Debug.Log("去图腾祭拜");
            mYinDaoisOpen = true;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400030];
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
            StartCoroutine("SetmButtonEnabel");
        }
        else if (FreshGuide.Instance().IsActive(400040) && TaskData.Instance.m_TaskInfoDic[400040].progress >= 0)
        {
            //			Debug.Log("去商店购买东西");
            mYinDaoisOpen = true;
            ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[400040];
            UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[0]);
            StartCoroutine("SetmButtonEnabel");
        }

        //Open all colliders.
        mainColliders.ForEach(item => item.enabled = true);
    }
    IEnumerator SetmButtonEnabel()
    {
        yield return new WaitForSeconds(1f);
        mYinDaoisOpen = false;
    }
    public bool SomeUIis_OPen = false;
    public void GetAllianceBuildsMessege()
    {
        //CityGlobalData.m_isRightGuide = true;
        if (!SomeUIis_OPen)
        {
            ShowAllianceGuid();
        }
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_JIAN_ZHU_INFO,ProtoIndexes.S_JIAN_ZHU_INFO.ToString());
        MainCityUI.setGlobalBelongings(this.gameObject, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY - 5);
    }
	private int mLianmengLevel ;
    public bool OnSocketEvent(QXBuffer p_message)
    {

        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {

                case ProtoIndexes.ALLIANCE_HAVE_RESP://返回联盟信息， 给有联盟的玩家返回此条信息
                {
                    MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    AllianceHaveResp allianceHaveRes = new AllianceHaveResp();

                    t_qx.Deserialize(t_tream, allianceHaveRes, allianceHaveRes.GetType());

                    //Debug.Log ("监听到联盟信息返回了");
				    mLianmengLevel = AllianceData.Instance.g_UnionInfo.level;
                    m_allianceHaveRes = allianceHaveRes;
					if(mLianmengLevel != m_allianceHaveRes.level)
					{
						mLianmengLevel = m_allianceHaveRes.level;
						SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_JIAN_ZHU_INFO);
					}
				    InitAlliance();

                    return true;
                }
                case ProtoIndexes.S_JIAN_ZHU_INFO:
                {
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    JianZhuList mJianZhuList = new JianZhuList();

                    t_qx.Deserialize(t_stream, mJianZhuList, mJianZhuList.GetType());

                    m_JianZhu = mJianZhuList;

                    //				Debug.Log("请求建筑返回");

                    InitBuilds(mJianZhuList);
                    if (Global.m_sPanelWantRun != null && Global.m_sPanelWantRun != "")
                    {
                        if (Global.m_sPanelWantRun == "AllianceTuteng")
                        {
                            WorshipLayerManagerment.m_bulidingLevel = TutengLev;
                            Enter_MoBai();

                            Global.m_sPanelWantRun = "";
                        }
                        if (Global.m_sPanelWantRun == "AllianceHuangye")
                        {
                            ENterHY();

                            Global.m_sPanelWantRun = "";
                        }
                    }
                    return true;
                }
                case ProtoIndexes.S_JIAN_ZHU_UP:
                {
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer t_qx = new QiXiongSerializer();

                    ErrorMessage BuildUpback = new ErrorMessage();

                    t_qx.Deserialize(t_stream, BuildUpback, BuildUpback.GetType());

                    Debug.Log("BuildUpback   ");

                    if (BuildUpback.errorCode == 0)
                    {
                        for (int i = 0; i < AllBuildsTmp_List.Count; i++)
                        {
                            if (Up_id == AllBuildsTmp_List[i].id)
                            {

                                //							Debug.Log ("Up_id：" + Up_id);
                                AllBuildsTmp_List[i].lv += 1;
                                AllBuildsTmp_List[i].Init();

                                int effectid = 100180;
                                BuildUpVecotry.SetActive(true);
                                BuildUpVecotry.transform.localPosition = AllBuildsTmp_List[i].gameObject.transform.localPosition;
                                UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, BuildUpVecotry, EffectIdTemplate.GetPathByeffectId(effectid));
                                StartCoroutine(closeEffect());

                                HYInterface hyInterface = AllBuildsTmp_List[i].GetComponent<HYInterface>();
                                if (hyInterface != null)
                                {
                                    hyInterface.Init();
                                }
                            }
                        }

                    }
                    else
                    {
					    string mstrtext = "";
						switch(BuildUpback.errorCode)
						{
						case 1:
							mstrtext = "只有盟主才能升级建筑！";
							break;
						case 10:
							break;
						case 20:
							mstrtext = "建筑等级已达最高！";
							break;
						case 30:
						    mstrtext = "联盟等级不足，请先提升联盟等级！";
							break;
						case 40:
							mstrtext = "建设值不足！";
							break;
						case 50:
							break;
						case 60:
							mstrtext = "联盟等级不足，请先提升联盟等级！";
							break;
					    default:
						    break;
						}
					    ClientMain.m_UITextManager.createText(mstrtext);
                    }
                    return true;
                }
                case ProtoIndexes.ALLIANCE_FIRE_NOTIFY://被联盟开除通知
                {
                    if (UIYindao.m_UIYindao.m_isOpenYindao)
                    {
                        UIYindao.m_UIYindao.CloseUI();
                    }

                    MainCityUI.TryRemoveFromObjectList(this.gameObject);
                    Destroy(this.gameObject);
                    return true;
                }

                case ProtoIndexes.EXIT_ALLIANCE_RESP://退出联盟返回
                {
                    MemoryStream exit_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer exit_qx = new QiXiongSerializer();

                    ExitAllianceResp exitResp = new ExitAllianceResp();

                    exit_qx.Deserialize(exit_stream, exitResp, exitResp.GetType());
                    Debug.Log("退出联盟2");
                    if (exitResp != null)
                    {
                        mexitResp = exitResp;

                        if (exitResp.code == 0)
                        {
                            CityGlobalData.m_isMainScene = true;

                            //去掉商铺联盟相关红点
						CloseAllRedPoints();
                        }

                        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                                ExitAllianceLoadCallback);

                    }
                    return true;
                }
                case ProtoIndexes.LOOK_APPLICANTS_RESP://查看申请入盟成员请求返回
                {
                    //	Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
                    MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                    QiXiongSerializer application_qx = new QiXiongSerializer();

                    LookApplicantsResp applicateResp = new LookApplicantsResp();

                    application_qx.Deserialize(application_stream, applicateResp, applicateResp.GetType());
//                    Debug.Log("==============3");
                    if (applicateResp != null)
                    {
//                        Debug.Log("==============2");
                        if (applicateResp.applicanInfo == null || applicateResp.applicanInfo.Count == 0)
                        {
                            HaveappyMembers = false;
                        }
                        else
                        {
                            HaveappyMembers = true;
                        }
                        AppbtnAlert.SetActive(HaveappyMembers);
                        MainCityUI.SetRedAlert(410000, HaveappyMembers);
                    }
                    else
                    {
//                        Debug.Log("==============1");
                    }
                    return true;
                }
                case ProtoIndexes.RED_NOTICE_INFO://监听红点事件
                {
                    ErrorMessage t_error_body = new ErrorMessage();

                    ProtoHelper.DeserializeProto(t_error_body, p_message);

                    for (int i = 0; i < AllBuildsTmp_List.Count; i++)
                    {
                        HYInterface hyInterface = AllBuildsTmp_List[i].GetComponent<HYInterface>();
                        if (hyInterface != null)
                        {
                            hyInterface.Init();
                        }
                    }
                    return true;
                }
			   case ProtoIndexes.S_ALLIANCE_UPINFO_SPEEDUP://盟加速信息返回
				{
					MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer application_qx = new QiXiongSerializer();
					
					UpgradeLevelInfoResp mUpgradeLevelInfoResp = new UpgradeLevelInfoResp();
								
					application_qx.Deserialize(application_stream, mUpgradeLevelInfoResp, mUpgradeLevelInfoResp.GetType());

				    m_UpgradeLevelInfoResp = mUpgradeLevelInfoResp;
				    Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpAllianceLevelAspeedremind);
					
					return true;
				}
			    case ProtoIndexes.S_ALLIANCE_UPGRADELEVEL_SPEEDUP://加速结果返回
				{
					MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer application_qx = new QiXiongSerializer();
					
					UpgradeLevelSpeedUpResp mUpgradeLevelSpeedUpResp = new UpgradeLevelSpeedUpResp();
					
					application_qx.Deserialize(application_stream, mUpgradeLevelSpeedUpResp, mUpgradeLevelSpeedUpResp.GetType());
				   if(mUpgradeLevelSpeedUpResp.result == 0)
					{
					int effectid = 100180;
					BuildUpVecotry.SetActive(true);
					BuildUpVecotry.transform.localPosition = Vector3.zero;
					BuildUpVecotry.GetComponent<UISprite>().spriteName = "AddSpeed";
					UI3DEffectTool.ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,BuildUpVecotry,EffectIdTemplate.GetPathByeffectId(effectid));
					StartCoroutine( "closeEffect");
					m_allianceHaveRes.upgradeRemainTime = mUpgradeLevelSpeedUpResp.remainTime;
				    Debug.Log("mUpgradeLevelSpeedUpResp.remainTime = "+mUpgradeLevelSpeedUpResp.remainTime);

				    SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ);
						
				    }else if(mUpgradeLevelSpeedUpResp.result == 1)
					{
						string mstr1 = "建设值不足！";
						ClientMain.m_UITextManager.createText(mstr1);
				    }else if(mUpgradeLevelSpeedUpResp.result == 2)
					{
						string mstr2 = "只有盟主才能升级联盟！";
						ClientMain.m_UITextManager.createText(mstr2);
					}
				    else if(mUpgradeLevelSpeedUpResp.result == 3)
					{
					    string mstr2 = "联盟未处于升级状态中！";
						ClientMain.m_UITextManager.createText(mstr2);
					}
				    else if(mUpgradeLevelSpeedUpResp.result == 3)
					{
						string mstr2 = "请先加入一个联盟！";
						ClientMain.m_UITextManager.createText(mstr2);
					} 
					    Debug.Log("加速结果返回3");

					return true;
				}
			case ProtoIndexes.DISMISS_ALLIANCE_OK://l联盟解散返回
			{
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				DismissAllianceResp mDismissAllianceResp = new DismissAllianceResp();
				
				application_qx.Deserialize(application_stream, mDismissAllianceResp, mDismissAllianceResp.GetType());
				if(mDismissAllianceResp.result == 1)
				{
					string mstr2 = "当前正在进行联盟战，无法解散联盟！";
					ClientMain.m_UITextManager.createText(mstr2);
					
				}
				Debug.Log("mDismissAllianceResp.result = "+mDismissAllianceResp.result);
				
				return true;
			}
                default: return false;
            }
        }
        return false;
    
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
	[HideInInspector]public UpgradeLevelSpeedUpResp m_UpgradeLevelSpeedUpResp;
    IEnumerator closeEffect()
    {
        yield return new WaitForSeconds(1f);
        BuildUpVecotry.SetActive(false);
    }
	[HideInInspector]public  int Online = 0;
    public void InitAlliance()
    {
         Online = 0;
        if (m_allianceHaveRes.memberInfo != null)
        {
            for (int i = 0; i < m_allianceHaveRes.memberInfo.Count; i++)
            {
                if (m_allianceHaveRes.memberInfo[i].offlineTime < 0)//之前为显示在线人数 后改为显示目前人数和最多能容纳人数
                {
                    Online += 1;
                }
            }

        }
        OnlineNum.text = "在线人数：" + Online.ToString();
        AllianceName.text = m_allianceHaveRes.name + "(Lv." + m_allianceHaveRes.level.ToString() + ")";

		if(m_allianceHaveRes.identity != 2)
		{
			AllianceCountryChangebtn.GetComponent<BoxCollider>().enabled = false;
			AllianceCountryChange.color = new Color (0,0,0,255);
		}
        ShowJianSheZhi();
        ShowBtn();
		InitUpLevelInfo ();
		ShowJunChengZhanAlert ();
		if(m_JianZhu != null)
		{
			InitBuilds(m_JianZhu);
		}
    }

	public void ShowJunChengZhanAlert()
	{
		//NewAlliancemanager.Instance ().ShowJunChengZhanAlert ();

		int JunCheng1 = 300500 ;
		int JunCheng2 = 310410 ;
		int JunCheng3 = 310420 ;
		bool IsShow1 = PushAndNotificationHelper.IsShowRedSpotNotification(JunCheng1);
		bool IsShow2 = PushAndNotificationHelper.IsShowRedSpotNotification(JunCheng2);
		bool IsShow3 = PushAndNotificationHelper.IsShowRedSpotNotification(JunCheng3);

//		Debug.Log ("IsShow1:" + IsShow1);
//		Debug.Log ("IsShow2:" + IsShow2);
//		Debug.Log ("IsShow3:" + IsShow3);

		if(IsShow1||IsShow2||IsShow3)
		{
			JunChengBattleAlert.SetActive (true);
		}
		else
		{
			JunChengBattleAlert.SetActive (false);
		}
	}

    public void ShowJianSheZhi()
    {
        Builds.text = m_allianceHaveRes.build.ToString();
    }
    public void InitBuilds(JianZhuList mJianZhu)
    {
        AllBuildsTmp_List.Clear();

        for (int i = 0; i < mJianZhu.list.Count; i++)
        {
//            Debug.Log("mJianZhu.list.lv = " + mJianZhu.list[i].lv);

            AllBuildsTmp mAllBuildsTmp = mBUILDS[i].GetComponent<AllBuildsTmp>();

            mAllBuildsTmp.lv = mJianZhu.list[i].lv;

            mAllBuildsTmp.id = i + 1;

            AllBuildsTmp_List.Add(mAllBuildsTmp);

            mAllBuildsTmp.Init();
        }
        for (int i = 0; i < 2; i++)
        {
            AllBuildsTmp mAllBuildsTmp = mBUILDS[i + 5].GetComponent<AllBuildsTmp>();

            mAllBuildsTmp.id = i + 6;

            mAllBuildsTmp.Init();
        }
        Refreshtification();
    }
    public void Refreshtification()
    {
        HYInterface mHYInterface1 = KeZhan.GetComponent<HYInterface>();
        mHYInterface1.Init();
        HYInterface mHYInterface4 = Temples.GetComponent<HYInterface>();
        mHYInterface4.Init();
        HYInterface mHYInterface5 = Apply.GetComponent<HYInterface>();
        mHYInterface5.Init();
        HYInterface mHYInterface6 = XiaoWu.GetComponent<HYInterface>();
        mHYInterface6.Init();
        HYInterface mHYInterface7 = HyQs.GetComponent<HYInterface>();
        mHYInterface7.Init();
        HYInterface mHYInterface8 = ReadRoom.GetComponent<HYInterface>();
        mHYInterface8.Init();
    }
    public void ExitAllianceLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();

        string exitTitleStr = "退出联盟";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_DES1);

        if (mexitResp.code == 0)
        {
            string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_DES1)+"\n";
            string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_DES2);
//            uibox.YinDaoControl(Del);
			uibox.setBox(exitTitleStr, MyColorData.getColorString(1, str1+str2), null, null, confirmStr, null, null);
            Del();
        }
        else
        {
            //Debug.Log("退出失败");

            string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_FAIL)+"\n";
            string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_FAIL_REASON);
			uibox.setBox(exitTitleStr, MyColorData.getColorString(1, str1+str2), null, null, confirmStr, null, null);
        }
    }
    void Del()
    {
        DeletUI_i(1);
    }
    public void DeletUI_i(int i)//Quite。
    {
        AllianceData.Instance.IsAllianceNotExist = true;
        QXChatUIBox.chatUIBox.SetSituationState();
        JunZhuData.Instance().m_junzhuInfo.lianMengId = 0;
        AllianceData.Instance.RequestData();
        //SceneManager.EnterMainCity();
        MainCityUI.TryRemoveFromObjectList(this.gameObject);
        Destroy(this.gameObject);
    }
    GameObject OpenRecruit;
    public void ReCruitSetting()
    {
        if (OpenRecruit)
        {
            return;
        }
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ALLIANCE_RECRUIT),
                                RecruitLoadCallback);
    }

    //联盟招募异步加载回调
    public void RecruitLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        OpenRecruit = Instantiate(p_object) as GameObject;
//        OpenRecruit.transform.parent = this.transform; ;
        OpenRecruit.transform.localScale = Vector3.one;
        OpenRecruit.transform.localPosition = new Vector3 (1555,100,0);
        ReCruit mReCruit = OpenRecruit.GetComponent<ReCruit>();
        //mReCruit.Z_UnionInfo = m_tempInfo;
        mReCruit.initLevel();
        mReCruit.ChangeNum();

        mReCruit.init(HSowMapAndBuilds);
        All_FirstUI.SetActive(false);
    }
    void HSowMapAndBuilds()
    {
        All_FirstUI.SetActive(true);
    }
    void ShowBtn()
    {
        //Debug.Log ("m_allianceHaveRes.status = "+m_allianceHaveRes.status );
        JuanXianUI.SetActive(false);
		AppbtnSprite.color = new Color (0,0,0,255);
        if (m_allianceHaveRes.identity == 2)
        {
            Label_LeaderBtn.text = "解散联盟";
//            RecruteBtn.SetActive(true);
            //			JuanXianUI.SetActive(true);
			AppbtnSprite.color = new Color (255,255,255,255);
            Appbtn.SetActive(true);
            int mEvent_id = 410000; // 联盟申请
            if (PushAndNotificationHelper.IsShowRedSpotNotification(mEvent_id) && HaveappyMembers)
            {
                AppbtnAlert.SetActive(true);
            }
            else
            {
                AppbtnAlert.SetActive(false);
            }
            NoticeBox.GetComponent<BoxCollider>().enabled = true;
        }
        else if (m_allianceHaveRes.identity == 1)
        {
			AppbtnSprite.color = new Color (255,255,255,255);

            Label_LeaderBtn.text = "退出联盟";
            //JuanXianUI.SetActive(false);
            Appbtn.SetActive(true);
            int mEvent_id = 410000;
            if (PushAndNotificationHelper.IsShowRedSpotNotification(mEvent_id))
            {
                AppbtnAlert.SetActive(true);
            }
            else
            {
                AppbtnAlert.SetActive(false);
            }
            NoticeBox.GetComponent<BoxCollider>().enabled = true;

        }
        else
        {
            //			JuanXianUI.SetActive(false);
            //
            //			JuanXianUI.transform.localPosition = new Vector3(0,-290,0);
            Appbtn.GetComponent<BoxCollider>().enabled = false;
			AppbtnSprite.color = new Color (0,0,0,255);
            Label_LeaderBtn.text = "退出联盟";
            NoticeBox.GetComponent<BoxCollider>().enabled = false;
        }

        mMyAllianceInfo.m_Alliance = m_allianceHaveRes;

        mMyAllianceInfo.Init();
    }
    public void BuyTiLi()
    {
        JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
    }
    public void BuyTongBi()
    {
        JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
    }
    public void BuyYuanBao()
    {
        MainCityUI.ClearObjectList();
        EquipSuoData.TopUpLayerTip();
    }

    public void Help()
    {
        GeneralControl.Instance.LoadRulesPrefab(LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCEINSTRCDUTION));
    }
    public void CheckInfo()
    {
        DeiInfoMation.transform.localScale = Vector3.one;
    }
    public void CloseCheckInfo()
    {
        DeiInfoMation.transform.localScale = new Vector3(0, 0, 1);
    }

    public void ApplyInfo() //打开申请列表
    {
        All_Apply.SetActive(true);
        All_FirstUI.SetActive(false);
        //Buttons.transform.localPosition = new Vector3(50,298,0);
        ApplyManager mApplyManager = All_Apply.GetComponent<ApplyManager>();

        mApplyManager.m_tempInfo = m_allianceHaveRes;

        mApplyManager.Init();

        //AppbtnAlert.SetActive(false);
    }

	public void AllianceChangerCountry() //转国
	{
		MiBaoGlobleData.Instance ().LoadCountryUI (2);
	}
    public void QuitAlliance()
    {

        if (m_allianceHaveRes.identity == 2) // 盟主
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    AllianceTransTipsLoadCallback1);
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    ExitLoadCallback);
        }
    }
    //解散联盟提示异步加载回调
    public void AllianceTransTipsLoadCallback1(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();

        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR1);
//        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR2);

		uibox.setBox(jieSanTitleStr, MyColorData.getColorString(1, str1), null, null, cancelStr, confirmStr, DisAlliance);
    }

    //退出联盟提示框异步加载回调
    public void ExitLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();

        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR1);
        //string str2 =  str1 + "\n\r" + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_ASKSTR2);

        string exitTitle = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_EXIT_TITLE);

		uibox.setBox(exitTitle, MyColorData.getColorString(1, str1), null,
                     null, cancelStr, confirmStr, ExitAllianceReq);
    }
    //发送退出联盟的请求
    void ExitAllianceReq(int i)
    {
        if (i == 2)
        {
            ExitAlliance exitReq = new ExitAlliance();

            exitReq.id = m_allianceHaveRes.id;

            MemoryStream exitStream = new MemoryStream();

            QiXiongSerializer exitQx = new QiXiongSerializer();

            exitQx.Serialize(exitStream, exitReq);

            byte[] t_protof = exitStream.ToArray();

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.EXIT_ALLIANCE, ref t_protof, "30114");
        }
    }

    void DisAlliance(int i)
    {
        if (i == 2)
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
                                    AllianceTransTipsLoadCallback2);
        }
    }
    //解散联盟再次提示异步加载回调
    public void AllianceTransTipsLoadCallback2(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject boxObj = Instantiate(p_object) as GameObject;

        UIBox uibox = boxObj.GetComponent<UIBox>();

        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR3)+"\n";
        string str2 = m_allianceHaveRes.name + LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_CONFIRM_JIESAN_ASKSTR4);

        string sanSiStr = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_JIESAN_SANSI);
        string jieSanStr = LanguageTemplate.GetText(LanguageTemplate.Text.ALLIANCE_JIESAN);

		uibox.setBox(jieSanTitleStr, MyColorData.getColorString(1, str1+str2), null, null, sanSiStr, jieSanStr, DisAllianceReq);
    }
    //发送解散联盟请求
    void DisAllianceReq(int i)
    {
        if (i == 2)
        {
            DismissAlliance disAllianceReq = new DismissAlliance();

            disAllianceReq.id = m_allianceHaveRes.id;

            MemoryStream dis_stream = new MemoryStream();

            QiXiongSerializer disQx = new QiXiongSerializer();

            disQx.Serialize(dis_stream, disAllianceReq);

            byte[] t_protof = dis_stream.ToArray(); ;

            SocketTool.Instance().SendSocketMessage(ProtoIndexes.DISMISS_ALLIANCE, ref t_protof, "30132");
            //Debug.Log ("jiesanReq:" + ProtoIndexes.DISMISS_ALLIANCE);
        }
    }
    public void ENterOtherUI(int m_id)
    {
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        //Debug.Log ("m_id = " +m_id );
        //Buttons.transform.localPosition = new Vector3(50,298,0);
        if (SomeUIis_OPen) { return; }
        SomeUIis_OPen = true;
        if (mYinDaoisOpen)
        {
            return;
        }
        switch (m_id)
        {
            case 1:
                All_KeZhan.SetActive(true);
                AllianceKeZhanManager mAllianceKeZhanManager = All_KeZhan.GetComponent<AllianceKeZhanManager>();
                mAllianceKeZhanManager.m_AllianceHaveRes = m_allianceHaveRes;
                mAllianceKeZhanManager.Init();
                break;
            case 2:
                All_ReadRoom.SetActive(true);
                TechnologyManager mTechnologyManager = All_ReadRoom.GetComponent<TechnologyManager>();
                mTechnologyManager.Init();
                break;
            case 3:
                // 膜拜
                break;
            case 4:
                //商铺
                break;
            case 5:
                All_Temples.SetActive(true);
                AllianceTemples mAllianceTemples = All_Temples.GetComponent<AllianceTemples>();
                mAllianceTemples.Init();
                break;
            default:
                break;
        }
        All_FirstUI.SetActive(false);
    }
    public void ENterHY()
    {
        if (mYinDaoisOpen)
        {
            return;
        }
        if (m_allianceHaveRes.level >= 2)
        {
            MiBaoGlobleData.Instance().OpenHYMap_UI();
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CantEnterOpenLockLoadBack);
        }
    }

    void CantEnterOpenLockLoadBack(ref WWW p_www, string p_path, Object p_object)
    {
        SomeUIis_OPen = true;
        UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

        string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);

        string str2 = "";

        string str1 = "\r\n" + "联盟等级到达2级才能进入荒野求生！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

        string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        string confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        uibox.YinDaoControl(ShowAllianceGuid);
        uibox.setBox(titleStr, MyColorData.getColorString(1, str1 + str2), null, null, confirmStr, null, null, null, null);
    }
    [HideInInspector]
    public OldBookWindow m_OldBookWindow;
    public void ENterAllianceHorse()
    {
        if (mYinDaoisOpen)
        {
            return;
        }
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        //Buttons.transform.localPosition = new Vector3(50,298,0);
        All_FirstUI.SetActive(false);
        if (m_OldBookWindow != null)
        {
            m_OldBookWindow.gameObject.SetActive(true);
            m_OldBookWindow.OldBookMode = OldBookWindow.Mode.OldBookSelf;
			OldBookWindow.Itemids.Clear ();
            m_OldBookWindow.RefreshUI();
        }
        else
        {
            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.OLD_BOOK_WINDOW), OnBookLoadCallBack);
        }
        SomeUIis_OPen = true;
    }
    void OnBookLoadCallBack(ref WWW www, string path, object loadedObject)
    {
        var tempObject = Instantiate(loadedObject as GameObject) as GameObject;
        m_OldBookWindow = tempObject.GetComponent<OldBookWindow>();
        m_OldBookWindow.IsSelfHouse = true;
		m_OldBookWindow.gameObject.SetActive(true);
		m_OldBookWindow.OldBookMode = OldBookWindow.Mode.OldBookSelf;
		OldBookWindow.Itemids.Clear ();
		m_OldBookWindow.RefreshUI();

        TransformHelper.ActiveWithStandardize(transform, m_OldBookWindow.transform);

       // ENterAllianceHorse();
    }
    public void EnterShop()
    {
        if (mYinDaoisOpen)
        {
            return;
        }
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        ShopData.Instance.OpenShop(ShopData.ShopType.GONGXIAN);
    }

    public void Enter_MoBai() // 进入膜拜界面  直接复制TutengLev膜拜等级
    {
        if (mYinDaoisOpen)
        {
            return;
        }
        if (UIYindao.m_UIYindao.m_isOpenYindao)
        {
            UIYindao.m_UIYindao.CloseUI();
        }
        WorshipLayerManagerment.m_bulidingLevel = TutengLev;
        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.WORSHIP_MAIN_LAYER),
                                     WorshipLayerLoadCallback);
        SomeUIis_OPen = true;
    }
    private static void WorshipLayerLoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        MainCityUI.TryAddToObjectList(tempObject);
    }
    public void BackToThis(GameObject m_game)
    {
        //Buttons.transform.localPosition = new Vector3(0,298,0);
		Debug.Log ("back to first");
        SomeUIis_OPen = false;
//        ShowAllianceGuid();
		All_FirstUI.SetActive(true);
        m_game.SetActive(false);
		//All_FirstUI.GetComponent<MyAllianceInfo> ().ShowAllianceGuid ();

    }

    public void Close()
    {
        MainCityUI.TryRemoveFromObjectList(this.gameObject);
        Destroy(this.gameObject);
    }
	int MaxLevel = 10;
	public void InitUpLevelInfo()
	{
		if(m_allianceHaveRes.level >=  MaxLevel)
		{
			Upmind.text = "联盟等级已达最大";
			DownTime.text = "";
			LevelUpButton.enabled = false;
			LevelUpButtonSprite.color = new Color (0,0,0,255);
			StopCoroutine("CountTime");
			return;
		}
		if(m_allianceHaveRes.identity != 2)
		{
			LevelUpButton.enabled = false;
			LevelUpButtonSprite.color = new Color (0,0,0,255);
			if(m_allianceHaveRes.upgradeRemainTime <= 0)
			{
				ShengJi = true;
				mindYouMenbers.SetActive(true);
				LianMengTemplate mLianmeng = LianMengTemplate.GetLianMengTemplate_by_lv(m_allianceHaveRes.level);
				Upmind.text = "升级需建设值"+mLianmeng.cost.ToString();
				Upmind.color = Color.red;
				UPBtnName.text = "升级";
				if(mLianmeng.cost <=  m_allianceHaveRes.build)
				{
					DownTime.text = "建设值已满，此乃我盟逆转危局之良机盟主再不升级更待何时！";
				}
				else{
					DownTime.text = "建设值尚且不足，请我盟兄弟齐心协力建设，壮大我盟！";
				}

				for(int i = 0 ; i < m_allianceHaveRes.memberInfo.Count; i++)
				{
					if(m_allianceHaveRes.memberInfo[i].identity == 2)
					{
						if(m_allianceHaveRes.memberInfo[i].offlineTime >(60*24*60))
						{
							Upmind.text = "盟主24小时未上线";
							DownTime.text = "盟员可以转盟或提醒盟主上线";
							break;
						}
					}
				}
				StopCoroutine("CountTime");
			}
			else
			{
				ShengJi = false;
				mindYouMenbers.SetActive(false);
				Upmind.text = "联盟升级中...";
				Upmind.color = Color.green;
				StopCoroutine("CountTime");
				StartCoroutine("CountTime");
			}
		
		}
		else
		{
			LevelUpButton.enabled  = true;
			LevelUpButtonSprite.color = new Color (255,255,255,255);
			if(m_allianceHaveRes.upgradeRemainTime <= 0)
			{
				LianMengTemplate mLianmeng = LianMengTemplate.GetLianMengTemplate_by_lv(m_allianceHaveRes.level);
				Upmind.text = "升级需建设值"+mLianmeng.cost.ToString();
				Upmind.color = Color.red;
				if(mLianmeng.cost <=  m_allianceHaveRes.build)
				{
					DownTime.text = "建设值已满，此乃我盟逆转危局之良机盟主再不升级更待何时！";
				}
				else{
					DownTime.text = "建设值尚且不足，请我盟兄弟齐心协力建设，壮大我盟！";
				}
				ShengJi = true;
				StopCoroutine("CountTime");
				mindYouMenbers.SetActive(true);
				UPBtnName.text = "升级";
			}
			else
			{
				mindYouMenbers.SetActive(false);
				UPBtnName.text = "加速";
				Upmind.text = "联盟升级中...";
				Upmind.color = Color.green;
				DownTime.text = "";
				ShengJi = false;
				StopCoroutine("CountTime");
				StartCoroutine("CountTime");
			}
		}
	}
	public UILabel UPBtnName;
	public UILabel Upmind;
	public UILabel DownTime;
	public UISprite LevelUpButtonSprite;
	public BoxCollider LevelUpButton;

	private bool ShengJi;


	IEnumerator CountTime()
	{
		int m_Time = m_allianceHaveRes.upgradeRemainTime;

//		Debug.Log("m_Time = " + m_Time);

		while(m_Time > 0)
		{
			m_Time -=1;
			
			int D = (int)(m_Time/(60*60*24));
			int H = (int)((m_Time%(60*60*24))/(60*60));
			int M = (int)(((m_Time%(60*60*24))%(60*60))/60);
			int S = (int)(m_Time%60);

//			Debug.Log("Day = "+D);
			string mday = "";
			if(D > 0)
			{
				mday = D.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.DAY);
			}
			DownTime.text = "剩余时间："+mday+H.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.HOUR)+M.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.MINUTE)+S.ToString()+LanguageTemplate.GetText(LanguageTemplate.Text.SECOND);
			yield return new WaitForSeconds (1.0f);
		}
		if(m_Time <= 0)
		{
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.ALLIANCE_INFO_REQ); // 刷新联系信息
		}
	}
}
