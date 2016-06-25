using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

/// 全局类 保存君主属性 所有有关君主的数据从此类取
public class JunZhuData : MonoBehaviour, SocketProcessor
{
	public static int m_iChenghaoID;

    private static JunZhuData m_junzhu_data_instance;

    public JunZhuInfoRet m_junzhuInfo;

    public PveMiBaoZhanLi m_PveMiBaoZhanLi;

    public struct JunZhuUpgradeInfo
    {
        public int gongji;
        public int fangyu;
        public int shengming;
        public int tili;
    }

    public JunZhuUpgradeInfo m_junzhuSavedInfo;

    private bool ShowLevelUpLayer = false;
    //public JunZhuInfoRet m_junzhuSavedInfo;

    public bool m_RefreshCopperInfo = false;

    public int m_remainTime;

    public int m_CurrentLevel = 0;

    public int m_tili;

    private int Max_tili = 999;

	public bool UI_IsOpen = false;
    public bool m_LevelUpInfoSave = false;
	public int  Hy_Bi;
    public static JunZhuData Instance()
    {
        if (m_junzhu_data_instance == null)
        {
            GameObject t_GameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

            m_junzhu_data_instance = t_GameObject.AddComponent<JunZhuData>();
        }

        return m_junzhu_data_instance;
    }

    #region Mono

    void Awake()
    {
        SocketTool.RegisterMessageProcessor(this);
    }

	void OnDestroy(){
		SocketTool.UnRegisterMessageProcessor(this);

		m_junzhu_data_instance = null;
	}

    void Start()
    {
        m_junzhuInfo = new JunZhuInfoRet();
        //        SocketHelper.SendQXMessage(ProtoIndexes.C_PVE_ZHANLI);
        UI_IsOpen = false;
//        GetTiLi();
    }

    void Update()
    {
        if (CityGlobalData.m_showLevelupEnable)
        {
            CityGlobalData.m_showLevelupEnable = false;
            if (ShowLevelUpLayer && CityGlobalData.m_isBattleField_V4_2D)
            {
                ShowLevelUpLayer = false;
				ClientMain.addPopUP(20, 0, "", null);
            }
        }
        else
        {
            if (ShowLevelUpLayer && !CityGlobalData.m_isBattleField_V4_2D 
                && JunZhuLevelUpManagerment.m_JunZhuLevelUp == null 
                && TaskSignalInfoShow.m_TaskSignal == null)
            {
                ShowLevelUpLayer = false;
				ClientMain.addPopUP(20, 0, "", null);
            }
        }
    }

    #endregion

    public bool Refresh_JunZhuData(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.JunZhuInfoRet:
                    MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
                    QiXiongSerializer t_qx = new QiXiongSerializer();
                    JunZhuInfoRet tempInfo = new JunZhuInfoRet();
                    t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

                    SetInfo(tempInfo);
                    return true;
            }
        }

        return false;
    }

    public void RequestJunZhuInfo(JunZhuInfoRet tempZhuInfoRet)
    {
      //  m_junzhuInfo = tempZhuInfoRet;
        SetInfo(tempZhuInfoRet);
    }

    /// Obtain JunZhu Info
    public static void RequestJunZhuInfo()
    {
        JunZhuInfoReq tempInfo = new JunZhuInfoReq();

        MemoryStream tempStream = new MemoryStream();

        QiXiongSerializer tempSer = new QiXiongSerializer();

        tempSer.Serialize(tempStream, tempInfo);

        byte[] t_protof;

        t_protof = tempStream.ToArray();

        SocketTool.Instance().SendSocketMessage(ProtoIndexes.JunZhuInfoReq, ref t_protof);

        //		Debug.Log ("JunZhuInfoReq:" + ProtoIndexes.JunZhuInfoReq);
    }

    #region Proto Process

	public ErrorMessage errorResp;

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
				case ProtoIndexes.weiWang:
				{
//					Debug.Log ("ProtoIndexes.weiWang:" + ProtoIndexes.weiWang);
					ErrorMessage errorMsg = new ErrorMessage();
					errorMsg = QXComData.ReceiveQxProtoMessage (p_message,errorMsg) as ErrorMessage;
					
					if (errorMsg != null)
					{
//						Debug.Log ("errorMsg.errorCode:" + errorMsg.errorCode);
						errorResp = errorMsg;

						if (SportData.Instance != null)
						{
							if (SportData.Instance.m_isOpenSport)
							{
							SportPage.m_instance.RefreshWeiWang (errorResp.errorCode);
							}
						}
					}
					
					return true;
				}
				case ProtoIndexes.huangyeBi://荒野比
				{
					MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					ErrorMessage HuanbyeBi = new ErrorMessage();
					
					t_qx.Deserialize(t_stream, HuanbyeBi, HuanbyeBi.GetType());
					
					Hy_Bi = HuanbyeBi.errorCode;
					return true;
				}
                case ProtoIndexes.JunZhuInfoRet:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        JunZhuInfoRet tempInfo = new JunZhuInfoRet();

                        t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
                        SetInfo(tempInfo);
                        m_junzhuSavedInfo.gongji = tempInfo.gongJi;
                        m_junzhuSavedInfo.fangyu = tempInfo.fangYu;
                        m_junzhuSavedInfo.shengming = tempInfo.shengMing;
                        m_junzhuSavedInfo.tili = tempInfo.tili;
                        if (m_CurrentLevel == 0)
                        {
                            m_CurrentLevel = tempInfo.level;
                        }
                        else if (tempInfo.level > m_CurrentLevel)
                        {
                            m_LevelUpInfoSave = true;
                            ShowLevelUpLayer = true;
                           // CityGlobalData.m_isWhetherOpenLevelUp = false;
                            //ShowLevelUp();
                            m_CurrentLevel = tempInfo.level;
                            FunctionOpenTemp.GetLVAddOpenFunction(tempInfo.level);
                        }
                        CityGlobalData.m_JunZhuTouJinJieTag = true;

						if(Global.m_iCenterZhanli < JunZhuData.Instance().m_junzhuInfo.zhanLi)
						{
							if(Global.m_iCenterZhanli != 0)
							{
								Global.m_iCenterZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
								ClientMain.m_UIAddZhanliManager.createText(JunZhuData.Instance().m_junzhuInfo.zhanLi - Global.m_iCenterZhanli);
							}
						}

						Global.m_NewChenghao = new List<int>();
						string saveString;
						saveString = PlayerPrefs.GetString( ConstInGame.CONST_NEW_CHENGHAO + m_junzhuInfo.id );
						if(!string.IsNullOrEmpty(saveString))
						{
							while(saveString.IndexOf(",") != -1)
							{
								Global.m_NewChenghao.Add(int.Parse(Global.NextCutting(ref saveString)));
							}
							MainCityUI.SetRedAlert(500015, true);
						}
						if(Global.m_iPZhanli == 0)
						{
							Global.m_iPZhanli = m_junzhuInfo.zhanLi;
						}
						else if(Global.m_iPZhanli < m_junzhuInfo.zhanLi)
						{
							Global.m_iAddZhanli = m_junzhuInfo.zhanLi - Global.m_iPZhanli;
							if(Global.m_isAddZhanli)
							{
								Global.m_iPChangeZhanli = Global.m_iPZhanli;
								ClientMain.addPopUP(70, 1, "", null);
							}
						}
						Global.m_iPZhanli = m_junzhuInfo.zhanLi;
                        if(PlayerNameManager.m_ObjSelfName != null)
                         PlayerNameManager.UpdateSelfName();

                        if (NationManagerment.m_Nation != null)
                        {
                            NationManagerment.m_Nation.RequestData();
                        }
                        return true;
                    }
                case ProtoIndexes.S_ADD_TILI_INTERVAL:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        TimeWorkerResponse tempResponse = new TimeWorkerResponse();

                        t_qx.Deserialize(t_stream, tempResponse, tempResponse.GetType());

                        RefreshTili(tempResponse);

                        return true;
                    }
                case ProtoIndexes.S_BUY_TIMES_INFO: //请求购买信息 BuyTongbiResp
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        BuyTimesInfo BuyTimespInfo = new BuyTimesInfo();

                        t_qx.Deserialize(t_stream, BuyTimespInfo, BuyTimespInfo.GetType());

                        Buy_TimespInfo = BuyTimespInfo;
                        // Debug.Log("Buy_TimespInfo tiLiHuaFei  = " + Buy_TimespInfo.tongBiHuaFei);
                        // Debug.Log("Buy_TimespInfo tiLiHuaFei  = " + Buy_TimespInfo.tongBiHuoDe);
                        InitBuyUI();
                        return true;
                    }
				case ProtoIndexes.S_BUY_TongBi_Data://购买金币一级界面信息
				{
					MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					QiXiongSerializer t_qx = new QiXiongSerializer();
					BuyTongbiDataResp BuyTongbiData = new BuyTongbiDataResp();
					t_qx.Deserialize(t_stream, BuyTongbiData, BuyTongbiData.GetType());
					m_BuyTongBiData = BuyTongbiData;
					// Debug.Log("Buy_TimespInfo tiLiHuaFei  = " + Buy_TimespInfo.tongBiHuaFei);
					// Debug.Log("Buy_TimespInfo tiLiHuaFei  = " + Buy_TimespInfo.tongBiHuoDe);
					InitBuyUI();
				}
					break;
                case ProtoIndexes.S_BUY_MiBaoPoint: //请求tongbi购买信息 BuyTongbiResp
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        BuyMibaoPointResp mBuyMibaoPointResp = new BuyMibaoPointResp();

                        t_qx.Deserialize(t_stream, mBuyMibaoPointResp, mBuyMibaoPointResp.GetType());

                        InitBuyPointUI(mBuyMibaoPointResp);

                        return true;
                    }

                default: return false;
            }
        }
        return false;
    }

    #endregion



    void LackYBLoadBack(int i)
    {
        if (i == 2)
        {
//            Debug.Log("跳转到充值！");

         //   MainCityUI.ClearObjectList();
            EquipSuoData.TopUpLayerTip();
            //            QXTanBaoData.Instance().CheckFreeTanBao();
        }
    }

    void InitBuyPointUI(BuyMibaoPointResp inf)
    {
        if (inf.result == 0)
        {
            SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
        }
        else if (inf.result == 1)
        {
           // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
			Global.CreateFunctionIcon (101);
            //Debug.Log("购买失败");
        }
        else if (inf.result == 2)
        {
			Global.CreateFunctionIcon (1901);
            //Debug.Log("VIP等级不足");
           // Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), BuyPointFail);
        }
    }
    void BuyPointFail(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
        UIBox uibox = m_Box.GetComponent<UIBox>();

        string titleStr = "购买失败";

		string Gstr1 ="" ;//LanguageTemplate.GetText(LanguageTemplate.Text.VIP_LEVEL_NOT_ENOUGH);

	
		Gstr1 = "V特权等级不足，V特权等级提升到"+VipFuncOpenTemplate.GetNeedLevelByKey(17).ToString()+"级即可开启购买点数功能。\n\r参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";
	
        string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		uibox.setBox(titleStr, MyColorData.getColorString(1, Gstr1), null,
                     null, strbtn2, null, null, null, null);

    }


    void LoadTongBiBack(ref WWW p_www, string p_path, Object p_object)
    {

        GameObject TonGBi = Instantiate(p_object) as GameObject;

        TonGBi.transform.localPosition = new Vector3(0, -1000, 0);

        TonGBi.transform.localScale = Vector3.one;
        BuyTongBiUI mBuyTongBiUI = TonGBi.GetComponent<BuyTongBiUI>();
        mBuyTongBiUI.M_BuyTimespInfo = Buy_TimespInfo;
        mBuyTongBiUI.M_BuyTongBiInfo = m_Buy_TBpInfo;
        mBuyTongBiUI.Init();
    }
    BuyTongbiResp m_Buy_TBpInfo;

    public bool IsBuyTiLi = false;

    public bool IsBuyTongBi = false;

    public bool IsBuyPoint = false;

	public bool m_isQiangxingOpen = false;
	BuyTongbiDataResp m_BuyTongBiData;
    BuyTimesInfo Buy_TimespInfo;

	void LoadBuyTiLiMaxBack(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject tempObj = GameObject.Instantiate(p_object) as GameObject;
		MainCityUI.TryAddToObjectList(tempObj);
		TreasureCityUI.TryAddToObjectList(tempObj);
	}

    void LoadBuyTiLiBack(ref WWW p_www, string p_path, Object p_object)
    {
		Debug.Log ("iLi.........");
		//string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_1) + LanguageTemplate.GetText(LanguageTemplate.Text.BUY_6);
		string str2 = "\r\n" + LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR1);
		string str3 = Buy_TimespInfo.tiLiHuaFei.ToString();
		string str4 = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_2);
		string str5 = Buy_TimespInfo.tiLiHuoDe.ToString();
		string str6 = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_6) + "？" + "\r\n";
		string str7 = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_4);
		string str8 = Buy_TimespInfo.tiLi.ToString();
		string str9 = LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR3);
		GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
		UIBox uibox = m_Box.GetComponent<UIBox>();
		
		string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
//		int v = JunZhuData.Instance ().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey (17) ? VipFuncOpenTemplate.GetNeedLevelByKey (17) : 0;
		uibox.setBox(str1, str2 + str3 + str4 + str5 + str6 + str7 + str8 + str9, null, null, strbtn1, strbtn2, BuyTiLi, null, null, null,false,true,true,false,100,0,0);

		MainCityUI.TryAddToObjectList(uibox.gameObject);
		TreasureCityUI.TryAddToObjectList(uibox.gameObject);
	}

    void BuyPoint(int i)
    {
		if (i == 2)
		{
			// Debug.Log("发送购买体力的请求");
			if(JunZhuData.Instance ().m_junzhuInfo.vipLv < VipFuncOpenTemplate.GetNeedLevelByKey (17))
			{
				Global.CreateFunctionIcon (1901);
				return;
			}
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < Buy_TimespInfo.mibaoHuaFei)
			{
				Global.CreateFunctionIcon (101);
				// Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
				return;
			}
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_MiBaoPoint,(ProtoIndexes.S_BUY_MiBaoPoint).ToString());
		}
    }

    void BuyTiLi(int i)
    {
		if (i == 2)
		{
			//Debug.Log("发送购买体力的请求");
			
			if (JunZhuData.Instance().m_junzhuInfo.tili >= Max_tili)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadmoreTiliBack_3);
				return;
			}
			
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < Buy_TimespInfo.tiLiHuaFei)
			{
				Global.CreateFunctionIcon (101);
				//Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);
				return;
			}
			if (Buy_TimespInfo.tiLi <= 0)
			{
				Global.CreateFunctionIcon (1901);
				// Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_3);
				return;
			}
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TiLi);
		}
    }

    public void BuyTiliAndTongBi(bool tili, bool tongBi, bool Pot, bool qiangxing = false)
    {
		m_isQiangxingOpen = qiangxing;
        if (tili)
        {
            IsBuyTiLi = true;
			IsBuyTongBi = false;
			IsBuyPoint = false;
        }
        if (tongBi)
        {
			IsBuyTiLi = false;
			IsBuyTongBi = true;
			IsBuyPoint = false;
        }

        if (Pot)
        {
			IsBuyTiLi = false;
			IsBuyTongBi = false;
			IsBuyPoint = true;
        }
		if(tongBi)
		{
			Debug.Log(ProtoIndexes.C_BUY_TongBi_Data);
			Global.ScendNull(ProtoIndexes.C_BUY_TongBi_Data);
		}
		else
		{
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TIMES_REQ,(ProtoIndexes.S_BUY_TIMES_INFO).ToString());
		}
		if (UIYindao.m_UIYindao.m_isOpenYindao) {

			yindaoid = UIYindao.m_UIYindao.m_iCurId;
		} 
		else 
		{
			yindaoid = 0;
		}

    }
	int yindaoid = 0 ;
    void InitBuyUI()
    {
//		Debug.Log ("IsBuyTongBi = "+IsBuyTongBi);
//		Debug.Log ("IsBuyTiLi = "+IsBuyTiLi);
//		Debug.Log ("IsBuyPoint = "+IsBuyPoint);
//		if(UI_IsOpen )
//		{
//			return;
//		}
//		UI_IsOpen = true;
        if (IsBuyTongBi)
        {
            m_RefreshCopperInfo = true;
			IsBuyTongBi = false;

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.UI_PANEL_BUYMONEY), LoadBuyTongBiBack);
        }
        else if (IsBuyTiLi)
        {
			if(m_isQiangxingOpen)
			{
				Debug.Log("========1");
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyTiLiBack);
			}
			else
			{
				if (Buy_TimespInfo.tiLi <= 0)
				{
					Debug.Log("========2");
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyTongBiNoTimesBack);
					
					return;
				}
				IsBuyTiLi = false;
				if(JunZhuData.Instance().m_junzhuInfo.tili >= JunZhuData.Instance().m_junzhuInfo.tiLiMax && JunZhuData.Instance().m_junzhuInfo.level > Global.TILILVMAX
				   &&(Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN))
				{
					Global.CreateFunctionIcon(201);
				}
				else
				{
					Debug.Log("========3");
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyTiLiBack);
				}
			}
        }

		else if (IsBuyPoint)
        {
            m_RefreshCopperInfo = true;

            IsBuyPoint = false;

            Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyPoint_Back);
        }
		else
		{
			//UI_IsOpen = false;
		}
		CityGlobalData.m_isRightGuide = true;
    }

   public  void LoadBuyTongBiNoTimesBack(ref WWW p_www, string p_path, Object p_object)
    {
		string str3 = "";
        string title = "";
		Global.CreateFunctionIcon (1901);
//        if (IsBuyTongBi)
//        {
//            IsBuyTongBi = false;
//
//            title = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_1) + LanguageTemplate.GetText(LanguageTemplate.Text.BUY_5);
//			if(m_junzhuInfo.vipLv < 7)
//			{
//				str3 = "V特权等级不足，V特权等级提升到" + (m_junzhuInfo.vipLv + 1).ToString()+"级即可购买更多铜币。\n\r参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";
//			}
//			else
//			{
//				str3 = "对不起，您今日的购买次数已经用尽。";
//			}
//
//        }
//        else if (IsBuyTiLi)
//        {
//			if(m_junzhuInfo.vipLv < 7)
//			{
//				str3 = "V特权等级不足，V特权等级提升到" + (m_junzhuInfo.vipLv + 1).ToString()+"级即可购买更多体力。\n\r参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";
//			}
//			else
//			{
//				str3 = "对不起，您今日的购买次数已经用尽。";
//			}
//
//            IsBuyTiLi = false;
//            title = LanguageTemplate.GetText(LanguageTemplate.Text.BUY_1) + LanguageTemplate.GetText(LanguageTemplate.Text.BUY_6);
//        }
//        string str22 = LanguageTemplate.GetText(LanguageTemplate.Text.YOU_XIA_14);
//
//        string str33 = LanguageTemplate.GetText(LanguageTemplate.Text.YOU_XIA_15);
//        string[] s = str22.Split('*');
//
//    
//        string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
//
//        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
//        UIBox uibox = m_Box.GetComponent<UIBox>();
//
//		uibox.setBox(title, str3,null , null, strbtn, null, ChangeState, null, null,null,false,false);
    }


    void LoadBuyPoint_Back(ref WWW p_www, string p_path, Object p_object)
    {
//		Debug.Log ("弹出购买点数的UI");
        string str1 = "购买点数";//LanguageTemplate.GetText(LanguageTemplate.Text.BUY_1) + LanguageTemplate.GetText(LanguageTemplate.Text.BUY_5);
		string str2 = "\r\n" + "你的升级点数不足了" + "\r\n" + "是否花费" + Buy_TimespInfo.mibaoHuaFei.ToString() + "元宝购买"+Buy_TimespInfo.mibaoHuoDe.ToString()+"点数？";//LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_CONFIRM_DUIHUAN_USE_WEIWANG_ASKSTR1);
       
		//string str3 = ;//Buy_TimespInfo.tongBiHuaFei.ToString();
        //string str4 = ;//LanguageTemplate.GetText(LanguageTemplate.Text.BUY_2);
        //string str5 = ;//Buy_TimespInfo.tongBiHuoDe.ToString();
        //string str6 = ;//LanguageTemplate.GetText(LanguageTemplate.Text.BUY_5) + "？";
        //string str7 = ;//LanguageTemplate.GetText(LanguageTemplate.Text.BUY_4);
        //string str8 = ;//Buy_TimespInfo.tongBi.ToString();
        //string str9 = ;//LanguageTemplate.GetText(LanguageTemplate.Text.BAIZHAN_ADDNUM_ASKSTR3);
        
		string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
        string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
       
		UIBox uibox = m_Box.GetComponent<UIBox>();
		int v =  VipFuncOpenTemplate.GetNeedLevelByKey (17);
		uibox.setBox(str1, str2,  null, null, strbtn1, strbtn2, BuyPoint, null, null, null,false,true,true,false,100,0,v);

    }

    void LoadBuyTongBiBack(ref WWW p_www, string p_path, Object p_object)
    {
		GameObject tempObj = GameObject.Instantiate(p_object) as GameObject;
		UIBuyMoneyPanel tempPanel = tempObj.GetComponent<UIBuyMoneyPanel>();
		tempPanel.YIndao_id = yindaoid;
		tempPanel.setData(m_BuyTongBiData);
		MainCityUI.TryAddToObjectList(tempObj);
		TreasureCityUI.TryAddToObjectList(tempObj);
	}
    void LoadmoreTiliBack_3(ref WWW p_www, string p_path, Object p_object)
    {

        string str1 = "体力已满";

        string str2 = " 您的体力已满，无法购买更多... ";// LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_RECHARGE);

        string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YUANBAO_LACK_TITLE);

        string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);

        string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
        UIBox uibox = m_Box.GetComponent<UIBox>();

		uibox.setBox(str1, str2, null, null, strbtn, null, null, null, null,null,false,false);
    }
    public void LoadBack_2(ref WWW p_www, string p_path, Object p_object)
    {

        //string str1 = "元宝不足";
        string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_RECHARGE);

        string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YUANBAO_LACK_TITLE);

        string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);

        string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
        UIBox uibox = m_Box.GetComponent<UIBox>();

		uibox.setBox(titleStr, str2, null, null, CancleBtn, strbtn, LackYBLoadBack, null, null, null,false,false);
    }
    int MaxVIPLevel = 10;

	public void LoadBack_3(ref WWW p_www, string p_path, Object p_object)
    {
		Debug.Log ("Buywhta");
        string str1 = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);

        string str22 = LanguageTemplate.GetText(LanguageTemplate.Text.YOU_XIA_14);

        string str33 = LanguageTemplate.GetText(LanguageTemplate.Text.YOU_XIA_15);

        string[] s = str22.Split('*');

        string str3 = "";
		if(m_junzhuInfo.vipLv < 7)
		{
			str3 = "V特权等级不足，V特权等级提升到" + (m_junzhuInfo.vipLv + 1).ToString()+"级即可购买更多体力。\n\r参与【签到】即可每天提升一级【V特权】等级，最多可提升至V特权7级。";
		}
		else
		{
			str3 = "对不起，您今日的购买次数已经用尽。";
		}

        string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

        GameObject m_Box = GameObject.Instantiate(p_object) as GameObject;
        UIBox uibox = m_Box.GetComponent<UIBox>();

		uibox.setBox(str1, str3, null, null, strbtn, null, null, null, null,null,false,false);
    }

    void GetTiLi() //获得体力
    {
        //        Debug.Log("GetTiLi");

        TimeWorkerRequest tempRequest = new TimeWorkerRequest();

        tempRequest.type = 1;

        MemoryStream m_stream = new MemoryStream();

        QiXiongSerializer t_qx = new QiXiongSerializer();

        t_qx.Serialize(m_stream, tempRequest);


        byte[] t_byte;

        t_byte = m_stream.ToArray();

        SocketTool.Instance().SendSocketMessage(
            ProtoIndexes.C_ADD_TILI_INTERVAL,
            ref t_byte,
            false);
    }

    void RefreshTili(TimeWorkerResponse tempReponse)
    {
        m_tili = tempReponse.value;
        m_remainTime = tempReponse.time;

        StopCoroutine("CountDownTime");

		StartCoroutine("CountDownTime");
    }

    IEnumerator CountDownTime() //体力倒计时
    {
        while (m_remainTime >= 0)
        {
            if (m_remainTime > 0)
            {
                m_remainTime -= 1;
            }
            else
            {
                StopCoroutine("CountDownTime");

                GetTiLi();

                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

	/** Get seconds left when ti li recovered fully.
	 * 
	 * Note:
	 * 1.if value <= 0, means already full, or can't get the value.
	 */
	public static int GetTimeOfCoverTiLi()
	{
		if( JunZhuData.Instance() == null ){
			return -1;
		}

		if( JunZhuData.Instance().m_junzhuInfo == null ){
			return -1;
		}

		int CurTili = JunZhuData.Instance().m_junzhuInfo.tili;
		if( CurTili < JunZhuData.Instance().m_junzhuInfo.tiLiMax ){
			int mTime = (int) ( JunZhuData.Instance().m_junzhuInfo.tiLiMax - CurTili ) * 360;
			
			return mTime;
		}

		return -1;
	}

    public bool ShowLevelUp(string data)
    {
        if (m_junzhuInfo.level >= 5)
        {
//            QXTanBaoData.Instance().CheckFreeTanBao();
        }

        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.JUN_ZHU_UPGRADE_LAYER),
                                ResourcesLoadCallBack);
        return true;
    }

    public void ResourcesLoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject levelUp = Instantiate(p_object) as GameObject;

        levelUp.transform.localPosition = new Vector3(0, 10000, 0);

        levelUp.transform.localScale = Vector3.one;

    }
	void ChangeState(int i)
	{
		//UI_IsOpen = false;
	}


    public void SetInfo(JunZhuInfoRet info)
    {
        if (info != null)
        {
            m_junzhuInfo = info;
        }
    }
}