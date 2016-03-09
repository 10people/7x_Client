using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EnterGame : MonoBehaviour,SocketProcessor {

	public static EnterGame enterGame;

    public static HighestUI s_HighestUI;

	private int state;
	public int ServerState//区状态
	{
		set{state = value;}
	}

	void Awake ()
	{
		enterGame = this;
		SocketTool.RegisterMessageProcessor (this);
	}

	//进入游戏请求
	public void EnterGameReq ()
	{
		if (state == 4)
		{
//			Debug.Log ("维护");

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
			                        FailLoadBack);

			return;
		}

//		{
//			Debug.Log ( "EnterGameReq: " + Time.realtimeSinceStartup );
//			
//			Debug.Log ( "SocketTool.WillReconnect: " + SocketTool.WillReconnect () );
//		}

		if ( SocketTool.WillReconnect() ){
			SocketTool.Instance().Connect();

			LoginReq loginReq = new LoginReq ();

			if( ThirdPlatform.IsMyAppAndroidPlatform() ){
				loginReq.name = ThirdPlatform.GetMyAppToken();

//				Debug.Log( "Socket.LoginReq.name: " + loginReq.name );
			}
			else if( ThirdPlatform.IsPPPLatform() ){
				loginReq.name = ThirdPlatform.GetPPToken();
			}
			else if( ThirdPlatform.IsXYPlatform() ){
				loginReq.name = ThirdPlatform.GetXYToken();
			}
			else if( ThirdPlatform.IsTongBuPlatform() ){
				loginReq.name = ThirdPlatform.GetTongBuSession();
			}
			else if( ThirdPlatform.IsI4Platform() ){
				loginReq.name = ThirdPlatform.GetI4Token();
			}
			else if( ThirdPlatform.IsKuaiYongPlatform() ){
				loginReq.name = ThirdPlatform.GetKuaiYongToken();
			}
			else if( ThirdPlatform.IsHaiMaPlatform() ){
				loginReq.name = ThirdPlatform.GetHaiMaToken();
			}
			else if( ThirdPlatform.IsIApplePlatform() ){
				loginReq.name = ThirdPlatform.GetIAppleToken();
			}
			else if( ThirdPlatform.IsIToolsPlatform() ){
				loginReq.name = ThirdPlatform.GetIToolsToken();
			}
			else{
				loginReq.name = AccountRequest.account.userName.text;
			}
			
			MemoryStream t_tream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize ( t_tream, loginReq );
			
			byte[] t_bytes = t_tream.ToArray();

//			Debug.Log( "Final Socket.LoginReq.name: " + loginReq.name );
			
			SocketTool.Instance().SendSocketMessage ( ProtoIndexes.LOGIN_ACCOUNT, ref t_bytes ,"23104");
			
			{
				UtilityTool.Instance.ManualGamePause();
			}
        }
		else{
			SocketTool.LogSocketToolInfo();
		}

        //Initialize Highest UI.
	    if (s_HighestUI == null)
	    {
	        Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HIGHEST_UI), HighestUILoadCallback);
	    }
    }

    private void HighestUILoadCallback(ref WWW p_www, string p_path, UnityEngine.Object p_object)
    {
        var highestUI = Instantiate(p_object) as GameObject;
        s_HighestUI = highestUI.GetComponent<HighestUI>();
        DontDestroyOnLoad(highestUI);

        HighestUI.IsExist = true;
    }

    public bool OnProcessSocketMessage(QXBuffer p_message)
    {
        if (p_message != null)
        {
            switch (p_message.m_protocol_index)
            {
                case ProtoIndexes.LOGIN_ACCOUNT_RET:
                    {
                        MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);

                        LoginRet loginRet = new LoginRet();

                        QiXiongSerializer t_qx = new QiXiongSerializer();

                        t_qx.Deserialize(t_stream, loginRet, loginRet.GetType());

                        if (loginRet != null)
                        {
                            CityGlobalData.m_SeverTime = loginRet.serTime;
                            CityGlobalData.m_king_model_Id = loginRet.roleId;

							{
								// 2008-05-01 21:34:42
//								Debug.Log( "ServerTime: " + loginRet.serverTime );

								TimeHelper.SetServerDateTime( loginRet.serverTime );

//								Debug.Log( "RegisterTime: " + loginRet.accountRegisterTime );

								PlayerInfoCache.SetRegisterTime( loginRet.accountRegisterTime );
							}

                            //CityGlobalData.m_EnterCityPosition = new Vector3(loginRet.x,
                            //                                                   loginRet.y,
                            //                                                   loginRet.z);
                  
                            string value = loginRet.x.ToString() + ":" + loginRet.y.ToString() + ":" + loginRet.z.ToString();
                            PlayerPrefs.SetString("IsCurrentJunZhuPos", value);
                            if (!ConfigTool.GetBool(ConfigTool.CONST_OPEN_ALLTHE_FUNCTION))
                            {
								FunctionOpenTemp.AssignFunctionIds( loginRet.openFunctionID );

                                //		                for (int i = 0; i < FunctionOpenTemp.m_EnableFuncIDList.Count; i++)
                                //		                {
                                //		                    Debug.Log(" FunctionOpenTemp.m_EnableFuncIDList[i] :" + FunctionOpenTemp.m_EnableFuncIDList[i]);
                                //		                }
                                //						Debug.Log( "Login response:\ncode: " + loginRet.code + "\nmessage:" + loginRet.msg );
                            }


                            switch (loginRet.code)
                            {
                                case 1:

                                    //Debug.Log ("登陆成功，已有角色！");

                                    CityGlobalData.countryId = loginRet.guoJiaId;
                                    SceneManager.EnterMainCity();

                                    NGUIDebug.ClearLogs();

                                    break;

                                case 2:

                                    //						Debug.Log ("登陆成功，未创建角色！");
                                    //					CityGlobalData.m_nextSceneName = ConstInGame.CONST_SCENE_NAME_CREATE_ROLE;

                                    CityGlobalData.countryId = loginRet.guoJiaId;

                                    //						Debug.LogError( "Never Use This." );
                                    //
                                    //						Application.LoadLevel ( ConstInGame.CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE );

                                    //SceneManager.EnterCreateRole();

                                    					EnterBattleField.EnterBattlePve( 0, 1, LevelType.LEVEL_NORMAL );
                                    //	
                                    //					NGUIDebug.ClearLogs ();

                                    break;

                                case 3:

//                                    Debug.Log("登录失败");
//
                                    Debug.Log("失败原因：" + loginRet.msg);

									Global.CreateBox( "登录失败",
						                 "失败原因：" + loginRet.msg,
						                 null,
						                 null, 
						                 "确定", 
						                 null,
						                 AccPwdErrorClickCallback,
						                 null,
						                 null,
						                 null,
						                 false,
						                 false,
						                 true );
                                    break;

                                case 100:

                                    //						Debug.Log ("登陆成功，有联盟！");

                                    CityGlobalData.countryId = loginRet.guoJiaId;

                                    //						EquipsOfBody.Instance();
                                    //if (FunctionWindowsCreateManagerment.IsCurrentJunZhuScene() == 2)
                                    //{
                                        CityGlobalData.m_isAllianceTenentsScene = true;
                                        CityGlobalData.m_iAllianceTenentsSceneNum = FunctionWindowsCreateManagerment.IsFenChengNum();
                                    //SceneManager.EnterAllianceCityTenentsCityOne();
                                    SceneManager.EnterMainCity();
                                    //}
                                    //else
                                    //{
                                    //    SceneManager.EnterAllianceCity();
                                    //}


                                    break;

                                default: break;
                            }
                        }

                        return true;
                    }

                default: return false;
            }
        }

        return false;
    }

	public static void AccPwdErrorClickCallback( int p_i ){
//		Debug.Log("ReLoginClickCallback( " + p_i + " )");
		
		{
//			SceneManager.CleanGuideAndDialog();
			
			SceneManager.RequestEnterLogin();
		}
	}

	private void FailLoadBack(ref WWW p_www, string p_path, Object p_object)
	{
		UIBox uibox = (Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "服务器维护";
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		string textStr = "您所选的服务器正在关闭维护中...\n请稍后再次尝试，或选择其他服务器。";

		uibox.setBox(
			titleStr,
			"\n" + MyColorData.getColorString (1,textStr),
			null,
			null,
			confirmStr,

			null,
			OnServerMaintenance );
	}

	void OnServerMaintenance(int i ){
//		Debug.Log( "OnServerMaintenance( " + i + " )" );

		SceneManager.EnterLogin();
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
