//#define DEBUG_REQUEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class AccountRequest : MonoBehaviour {

	public static AccountRequest account;

	private JSONNode dengLuNode;

	public UIInput userName;//用户名

	public UIInput password;//密码输入

	public GameObject loginObj;
	public GameObject lastLoginObj;

	public GameObject tipWindow;

	private string msg;

	public UIToggle toggleBtn;

	private bool isRemember;

	/// <summary>
	/// 公告
	/// </summary>
	
	public GameObject noticeObj;

	public GameObject selectUrlObj;

	public GameObject inputObj;
	
	public EventHandler zheZhaoHandler;
	
	private float time = 0.4f;

	void Awake ()
	{
		account = this;
	}

	void Start () 
	{
		userName.GetComponent<EventHandler> ().m_handler += TransUp;
		password.GetComponent<EventHandler> ().m_handler += TransUp;
		zheZhaoHandler.m_handler += ZheZhaoHandlerCallBack;

		#if DEBUG_REQUEST
		Debug.Log( Time.realtimeSinceStartup + " AccountRequest.Start()" );
		#endif

//		switch (ConfigTool.m_server_type_enum)
//		{
//		case ConfigTool.ServerType.TengXun:
//
//			Debug.Log ("WhichInternet:腾讯");
//
//			break;
//
//		case ConfigTool.ServerType.NeiWang:
//
//			Debug.Log ("WhichInternet:内网");
//
//			break;
//		}

		// report when battle field is ready
		{
			OperationSupport.ReportClientAction( OperationSupport.ClientAction.RESOURCES_LOAD_DONE );
		}

		// fetching announcement
		{
			NoticeDataRequest ();
		}

		if(!string.IsNullOrEmpty (PlayerPrefs.GetString ("UserNameAndPassWord")))
		{
			string[] loginInfo = PlayerPrefs.GetString ("UserNameAndPassWord").Split (';');
			userName.value = loginInfo[0]; 
			password.value = loginInfo[1]; 

			isRemember = true;
			toggleBtn.startsActive = true;
		}
		else
		{
			toggleBtn.startsActive = true;
			isRemember = true;
		}

		selectUrlObj.SetActive (Prepare_Bundle_Config.ShowServerSelector ());
	}

	void OnDestroy(){
		account = null;
	}

	void NoticeDataRequest ()
	{
		#if DEBUG_REQUEST
		Debug.Log("HttpRequest.GetNoticeUrl ()HttpRequest.GetNoticeUrl () ::" + HttpRequest.GetNoticeUrl());

		Debug.Log ( Time.realtimeSinceStartup + "NoticeDataRequest()" );
		#endif

		HttpRequest.Instance ().Connect ( HttpRequest.GetPrefix() + HttpRequest.NOTICE_URL,
		                                  null, 
		                                  NoticeSuccess,
		                                  NoticeFail,
		                                  UtilityTool.GetEventDelegateList ( ClientMain.LoadSoundTemplate ) );
	}
	
	void NoticeSuccess (string tempResponse)
	{
		#if DEBUG_REQUEST
		Debug.Log ( Time.realtimeSinceStartup + "NoticeRes:" + tempResponse);
		#endif

		GameObject notice = (GameObject)Instantiate (noticeObj);

		notice.SetActive (true);
		notice.name = "Notice";

		notice.transform.transform.parent = loginObj.transform.parent;
		notice.transform.localPosition = Vector3.zero;
		notice.transform.localScale = Vector3.one;

		NoticeManager noticeMan = notice.GetComponent<NoticeManager> ();
		noticeMan.GetNoticeStr (tempResponse);
	}
	
	void NoticeFail (string tempResponse) 
	{
        CreateReConnectWindow();

		Debug.Log( Time.realtimeSinceStartup + "AccountRequest.NoticeFail: " + tempResponse );
	}

	#region Register

	/// <summary>
	/// 注册账号
	/// </summary>
	public void LoginRequestSend ()
	{
		if (SysparaTemplate.CompareSyeParaWord (userName.value) || SysparaTemplate.CompareSyeParaWord (password.value))
		{
			msg = "有奇怪的文字混进来了\n再仔细推敲一下吧...";

			ShowFailTipWindow (msg);
		}

		else
		{
			if (string.IsNullOrEmpty (userName.value) || string.IsNullOrEmpty (password.value) ||
			    userName.value == "请输入账号名" || password.value == "请输入密码")
			{
				msg = "账号或密码不能为空！";

				ShowFailTipWindow (msg);
			}
			
			else
			{
				Dictionary<string,string> tempUrl = new Dictionary<string,string>();
				
				tempUrl.Add ( "name", userName.value );
				
				tempUrl.Add ( "pwd",password.value );

				tempUrl.Add ( "CustomDeviceId", QualityTool.GetDeviceInfo() );

				AddClientInfo( tempUrl );

				#if DEBUG_REQUEST
//				DebugRequestParams( tempUrl );
				#endif
				
				HttpRequest.Instance ().Connect ( CityGlobalData.RigisterURL, 
				                                 tempUrl, 
				                                 LoginRequestSuccess, 
				                                 LoginRequestFail );
			}
		}
	}
	
	void LoginRequestSuccess (string tempResponse) 
	{
		if (isRemember)
		{
			if (string.IsNullOrEmpty (PlayerPrefs.GetString ("UserNameAndPassWord")))
			{
				PlayerPrefs.SetString ("UserNameAndPassWord", userName.value + ";" + password.value);
			}
			else 
			{
				PlayerPrefs.DeleteKey ("UserNameAndPassWord");
				
				PlayerPrefs.SetString ("UserNameAndPassWord", userName.value + ";" + password.value);	
			}
		}
		else
		{
			PlayerPrefs.DeleteKey ("UserNameAndPassWord");
		}

		PlayerPrefs.Save();

//		Debug.Log ("数据返回:" + tempResponse);
		
		JSONNode zhuCeNode = JSON.Parse (tempResponse);

		msg = zhuCeNode ["msg"].Value;

		ShowFailTipWindow (msg);

//		switch (zhuCeNode ["code"].AsInt)
//		{
//		case 0:
//
////			Debug.Log ("注册失败，该用户名已被注册！");
//
//			ShowFailTipWindow (2);
//
//			break;
//
//		case 1:
//
////			Debug.Log ("注册成功，请登录游戏！");
//
//			ShowFailTipWindow (4);
//
//			break;
//		}
	}
	
	void LoginRequestFail (string tempResponse) 
	{
		Debug.LogError( "LoginRequestFail: " + tempResponse );
		ShowFailTipWindow (tempResponse + "\n请选择其他服务器！");
	}

	#endregion



	#region Login

	/// <summary>
	/// 登陆
	/// </summary>
	public void DengLuRequestSend () 
	{
		if (SysparaTemplate.CompareSyeParaWord (userName.value) || SysparaTemplate.CompareSyeParaWord (password.value))
		{
			msg = "有奇怪的文字混进来了\n再仔细推敲一下吧...";

			ShowFailTipWindow (msg);
		}

		else
		{
			if (string.IsNullOrEmpty (userName.value) || string.IsNullOrEmpty (password.value) ||
			    userName.value == "请输入账号名" || password.value == "请输入密码")
			{
				msg = "账号或密码不能为空！";

				ShowFailTipWindow (msg);
			}
			
			else
			{
				Dictionary<string,string> tempUrl = new Dictionary<string,string>();
				
				tempUrl.Add("name",userName.value);
				
				tempUrl.Add("pwd", password.value);

				tempUrl.Add ( "CustomDeviceId", QualityTool.GetDeviceInfo() );

				AddClientInfo( tempUrl );

				#if DEBUG_REQUEST
//				DebugRequestParams( tempUrl );
				#endif

				// add uuid
				{
					OperationSupport.AppendHttpParamUUID( tempUrl );
				}
				
				HttpRequest.Instance ().Connect ( CityGlobalData.LoginURL,
				                                 tempUrl,
				                                 DengLuRequestSuccess, 
				                                 DengLuRequestFail );
			}
		}
	}

	public void DengLuRequestSuccess( string tempResponse ){
		if (string.IsNullOrEmpty (tempResponse)) {
			Debug.LogWarning( "DengLuRequestSuccess.Response.Null." );

			return;
		}

		if (isRemember)
		{
			if( string.IsNullOrEmpty (PlayerPrefs.GetString ("UserNameAndPassWord")))
			{
				PlayerPrefs.SetString ("UserNameAndPassWord", userName.value + ";" + password.value);
			}
			else 
			{
				PlayerPrefs.DeleteKey( "UserNameAndPassWord" );
				
				PlayerPrefs.SetString ( "UserNameAndPassWord", userName.value + ";" + password.value);	
			}
		}
		else
		{
			PlayerPrefs.DeleteKey( "UserNameAndPassWord" );
		}

		PlayerPrefs.Save();

//		Debug.Log ( "DengLuRequestSuccess: " + tempResponse );

		dengLuNode = JSON.Parse (tempResponse);

		int code = dengLuNode ["code"].AsInt; //登陆是否成功 code

		msg = dengLuNode ["msg"].Value; //反馈信息
		
		//登陆判定
		if( code == 0 )
		{
//			Debug.Log( "Login.Response.Code: " + code + " : " + msg );

			ShowFailTipWindow (msg);
		}

		else if (code == 1)
		{	
//			Debug.Log ("登陆成功！");

			loginObj.SetActive (false);
			lastLoginObj.SetActive (true);

			LastLogin last = lastLoginObj.GetComponent<LastLogin> ();
			last.loginNode = dengLuNode;
			last.CreateLastLogin ();
		}
	}
	
	void DengLuRequestFail (string tempResponse) 
	{
		// TODO, PopOut Window
		Debug.LogError ( "DengLuRequestFail: " + tempResponse );
		ShowFailTipWindow ("\n\n请选择其他服务器！");
	}

	#endregion



	#region Client Info

	private void DebugRequestParams( Dictionary<string, string> p_dict ){
		foreach( KeyValuePair<string, string> p_kv in p_dict ){
			Debug.Log( p_kv.Key + ": " + p_kv.Value );
		}
	}

	public static void AddClientInfo( Dictionary<string, string> p_dict ){
		#if DEBUG_REQUEST
		Debug.Log ( "AddClientInfo: " + Time.realtimeSinceStartup );
		#endif

		p_dict.Add( "ClientVersion", VersionTool.GetSmallVersion() );

		p_dict.Add( "SystemSoftware", SystemInfo.operatingSystem );

		p_dict.Add( "SystemHardware", SystemInfo.deviceModel );

		// TODO
		p_dict.Add( "TelecomOper", "" );

		p_dict.Add( "Network", "" );

		p_dict.Add( "ScreenWidth", Screen.width + "" );

		p_dict.Add( "ScreenHight", Screen.height + "" );

		p_dict.Add( "Density", Screen.dpi + "" );

		if (ThirdPlatform.IsThirdPlatform ()) {
			p_dict.Add ("RegChannel", ThirdPlatform.GetPlatformTag ());
		}
		else {
			p_dict.Add ( "RegChannel", "Default" );
		}

		p_dict.Add( "CpuHardware", SystemInfo.processorType + ".x" + SystemInfo.processorCount );

		p_dict.Add( "Memory", SystemInfo.systemMemorySize + "" );

		p_dict.Add( "GLRender", SystemInfo.graphicsShaderLevel + "" );

		p_dict.Add( "GLVersion", "" );

		p_dict.Add( "DeviceId", SystemInfo.deviceName );
	}

	#endregion

	//显示区状态
	public void ShowServeState (UILabel tempStrLabel,int state)
	{
		switch (state)
		{
		case 1:
			
			tempStrLabel.text = "[00ff00]新区[-]";
			
			break;
			
		case 2:
			
			tempStrLabel.text = "[00ff00]流畅[-]";
			
			break;
			
		case 3:
			
			tempStrLabel.text = "[dc0600]爆满[-]";
			
			break;
			
		case 4:
			
			tempStrLabel.text = "[dc0600]维护[-]";
			
			break;

		default:break;
		}
	}

	//上次登陆按钮状态
	public void ShowLastLoginBtn (GameObject btnObj,UILabel btn1Label,UILabel btn2Label)
	{
		int isNewPlayer = dengLuNode ["isLogined"].AsInt;//新老用户 1.登陆过 2.未登陆过
		switch (isNewPlayer)
		{
		case 1:

			btnObj.SetActive (true);
			
			JSONNode lastLogin = dengLuNode ["lastLogin"];

			btn1Label.text = "上次登录" + lastLogin ["id"].AsInt + "区";

			break;

		case 2:

			btnObj.SetActive (false);

			btn2Label.text = "上次登录" + " " + "区";

			break;
		}
	}

	public void ShowFailTipWindow (string msg)
	{
		GameObject tipWin = (GameObject)Instantiate (tipWindow);

		tipWin.SetActive (true);
		tipWin.transform.parent = this.transform;
		tipWin.transform.localPosition = Vector3.zero;
		tipWin.transform.localScale = Vector3.one;

		LoginTipWindow loginTipWin = tipWin.GetComponent<LoginTipWindow> ();
		loginTipWin.ScaleAnim ();
		loginTipWin.ShowTip (msg);
	}

	public void ToggleValueChange ()
	{
		if (isRemember)
		{
			isRemember = false;
			Debug.Log ("close");
		}
		else
		{
			isRemember = true;
			Debug.Log ("open");
		}
	}

     void CreateReConnectWindow()
    {
        Global.CreateBox(LanguageTemplate.GetText(LanguageTemplate.Text.LOST_CONNECTION_1),
                        LanguageTemplate.GetText(LanguageTemplate.Text.LOST_CONNECTION_2),
                        "",
                        null,
                         LanguageTemplate.GetText(LanguageTemplate.Text.LOST_CONNECTION_3),
                        null,
                        ReLoginClickCallback,
		                 null,
		                 null,
		                 null,
		                 false,
		                 false,
		                 false );
    }

     void ReLoginClickCallback(int p_i)
    {
//        Debug.Log("ReLoginClickCallback( " + p_i + " )");

        if (SocketTool.WillReconnect())
        {
            if (UIYindao.m_UIYindao != null && UIYindao.m_UIYindao.m_isOpenYindao)
            {
                UIYindao.m_UIYindao.CloseUI();
            }

            //			StartCoroutine( DelayedEnterLogin() );

			{
				SceneManager.RequestEnterLogin();
			}
        }
    }

	/// <summary>
	/// 用户名,密码字符长度限制
	/// </summary>
	void TextLimit ()
	{
		userName.value = NewSelectRole.TextLengthLimit (NewSelectRole.StrLimitType.USER_NAME,userName.value);
		password.value = NewSelectRole.TextLengthLimit (NewSelectRole.StrLimitType.USER_NAME,password.value);
	}

	//输入框上移
	void TransUp (GameObject tempObj)
	{
		zheZhaoHandler.gameObject.SetActive (true);
		zheZhaoHandler.GetComponent<UISprite> ().alpha = 0.1f;

		Hashtable up = new Hashtable ();
		
		up.Add ("time",time);
		up.Add ("easetype",iTween.EaseType.easeOutQuart);

		#if UNITY_EDITOR
		up.Add ("position",new Vector3(0,-55,0));
		#else
		up.Add ("position",new Vector3(0,270,0));
		#endif

		up.Add ("islocal",true);
		
		iTween.MoveTo (inputObj,up);
	}
	
	//输入框下移
	public void TransDown ()
	{
		zheZhaoHandler.gameObject.SetActive (false);

		TextLimit ();

		Hashtable down = new Hashtable ();
		
		down.Add ("time",time);
		down.Add ("easetype",iTween.EaseType.easeOutQuart);
		down.Add ("position",new Vector3(0,-55,0));
		down.Add ("islocal",true);
		
		iTween.MoveTo (inputObj,down);
	}

	void ZheZhaoHandlerCallBack (GameObject tempObj)
	{
		TransDown ();
	}
}
