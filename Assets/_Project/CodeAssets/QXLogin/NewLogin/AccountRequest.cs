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

	public GameObject loginObj1;
	public GameObject loginObj2;
	public GameObject lastLoginObj;

	public GameObject selectObj1;
	public GameObject selectObj2;
	public enum EnterType
	{
		LAST_LOGIN,
		SELECT_ONE,
		SELECT_TWO,
	}
	public EnterType enterType;

	private string textStr;

	public UIToggle toggleBtn;

	private bool isRemember;

	private readonly Dictionary<int,string> ServeStateDic = new Dictionary<int, string>()
	{
		{1,"[00ff00]新区[-]"},
		{2,"[00ff00]流畅[-]"},
		{3,"[dc0600]爆满[-]"},
		{4,"[dc0600]维护[-]"}
	};

	public GameObject zhezhaoObj;

	public GameObject backBtnObj;

	public List<EventHandler> loginHandlerList = new List<EventHandler> ();

	/// <summary>
	/// 公告
	/// </summary>
	public GameObject noticeObj;

	public GameObject selectUrlObj;

	public GameObject inputObj;
	
	private float time = 0.4f;

	void Awake ()
	{
		account = this;
	}

	void Start () 
	{
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

		if(!string.IsNullOrEmpty (GetStoredUserNameAndPassWord()))
		{
			string[] loginInfo = GetStoredUserNameAndPassWord().Split (';');
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

		selectUrlObj.SetActive ( PrepareBundleHelper.ShowServerSelector ());

		foreach (EventHandler handler in loginHandlerList)
		{
			handler.m_click_handler -= LoginHandlerClickBack;
			handler.m_click_handler += LoginHandlerClickBack;
		}
	}

	void LoginHandlerClickBack (GameObject obj)
	{
		switch (obj.name)
		{
		case "RegisterButton":

			LoginRequestSend ();

			break;
		case "LoginButton":

			DengLuRequestSend ();

			break;
		case "ZheZhao":

			TransDown ();

			break;
		case "NameInput":

			TransUp ();

			break;
		case "PasswordInput":

			TransUp ();

			break;
		case "Toggle":

			ToggleValueChange ();

			break;
		case "QQ":

			QQBtn ();

			break;
		case "WeiXin":

			WeiXinBtn ();

			break;
		case "BackBtn":

			{
				ThirdPlatform.LogOut();
			}

			backBtnObj.SetActive (false);

			switch (enterType)
			{
			case EnterType.LAST_LOGIN:

				lastLoginObj.SetActive (false);

				break;
			case EnterType.SELECT_ONE:

				selectObj1.SetActive (false);

				break;
			case EnterType.SELECT_TWO:

				selectObj2.SetActive (false);

				break;
			default:
				break;
			}

			loginObj1.SetActive (m_selectPlatform);
			loginObj2.SetActive (!m_selectPlatform);

			break;
		default:
			break;
		}
	}

	void OnDestroy(){
		account = null;
	}

	void NoticeDataRequest ()
	{
		#if DEBUG_REQUEST
		Debug.Log("HttpRequest.GetNoticeUrl ()HttpRequest.GetNoticeUrl() : " + "" );

		Debug.Log ( Time.realtimeSinceStartup + "NoticeDataRequest()" );
		#endif

		HttpRequest.Instance().Connect ( NetworkHelper.GetPrefix() + NetworkHelper.NOTICE_URL,
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

		notice.transform.transform.parent = loginObj2.transform.parent;
		notice.transform.localPosition = Vector3.zero;
		notice.transform.localScale = Vector3.one;

		NoticeManager noticeMan = notice.GetComponent<NoticeManager> ();
		noticeMan.GetNoticeStr (tempResponse);
	}
	
	public static void NoticeFail (string tempResponse) 
	{
//		Debug.Log( " AccountRequest.NoticeFail: " + tempResponse );

        CreateReConnectWindow();
	}

	#region Active LoginObj
	private bool m_selectPlatform;
	private UIWidget loginWidget;
	public void SetActiveLoginObj (bool selectPlatform)
	{
		m_selectPlatform = selectPlatform;
		loginObj1.SetActive (selectPlatform);
		loginObj2.SetActive (!selectPlatform);

		FlyBirdController.birdController.SetBirdState (FlyBirdController.BirdState.LAND);

		loginWidget = (selectPlatform ? loginObj1 : loginObj2).GetComponent<UIWidget> ();
		loginWidget.alpha = 0;
		StartCoroutine ("ShowLoginWindow");
	}

	IEnumerator ShowLoginWindow ()
	{
		while (loginWidget.alpha <= 1)
		{
			loginWidget.alpha += 0.05f;
			yield return new WaitForSeconds (0.02f);

			if (loginWidget.alpha >= 1)
			{
				loginWidget.alpha = 1;
				StopCoroutine ("ShowLoginWindow");
			}
		}
	}

	private UIWidget selectObjWidget;
	/// <summary>
	/// Dises the active login object.
	/// </summary>
	public void DisActiveLoginObj ()
	{
		switch (enterType)
		{
		case EnterType.LAST_LOGIN:
			selectObjWidget = lastLoginObj.GetComponent<UIWidget> ();
			break;
		case EnterType.SELECT_ONE:
			selectObjWidget = selectObj1.GetComponent<UIWidget> ();
			break;
		case EnterType.SELECT_TWO:
			selectObjWidget = selectObj2.GetComponent<UIWidget> ();
			break;
		default:
			break;
		}

		selectObjWidget.alpha = 1;
		backBtnObj.GetComponent<UIWidget> ().alpha = 1;

		FlyBirdController.birdController.SetBirdState (FlyBirdController.BirdState.FLY);

		StartCoroutine ("DisLoginWindow");
	}

	IEnumerator DisLoginWindow ()
	{
		while (selectObjWidget.alpha > 0)
		{
			selectObjWidget.alpha -= 0.05f;
			backBtnObj.GetComponent<UIWidget> ().alpha -= 0.05f;
			yield return new WaitForSeconds (0.02f);
			
			if (selectObjWidget.alpha <= 0)
			{
				selectObjWidget.alpha = 0;

				StopCoroutine ("DisLoginWindow");
			}
		}
	}

	#endregion

	#region Register

	/// <summary>
	/// QQs the button.
	/// </summary>
	void QQBtn ()
	{
		if( ThirdPlatform.IsMyAppAndroidPlatform() ){
			Debug.Log( "MyApp flow, Now Start QQ Login." );
			
			ThirdPlatform.ShowQQLogin();
			
			return;
		}
	}

	/// <summary>
	/// Weis the xin button.
	/// </summary>
	void WeiXinBtn ()
	{
		if( ThirdPlatform.IsMyAppAndroidPlatform() ){
			Debug.Log( "MyApp flow, Now Start WeiXin Login." );
			
			ThirdPlatform.ShowWXLogin();
			
			return;
		}
	}

	/// <summary>
	/// 注册账号
	/// </summary>
	void LoginRequestSend ()
	{
		QQBtn ();

//		if (SysparaTemplate.CompareSyeParaWord (userName.value) || SysparaTemplate.CompareSyeParaWord (password.value))
//		{
//			textStr = "有奇怪的文字混进来了\n再仔细推敲一下吧...";
//			QXComData.CreateBox (1,textStr,true,null);
//		}
//		else
//		{
//
//		}

		if (string.IsNullOrEmpty (userName.value) || string.IsNullOrEmpty (password.value) ||
		    userName.value == "请输入账号名" || password.value == "请输入密码")
		{
			textStr = "账号或密码不能为空！";
			QXComData.CreateBox (1,textStr,true,null);
		}
		
		else
		{
			Dictionary<string,string> tempUrl = new Dictionary<string,string>();
			
			tempUrl.Add ( "name", userName.value );
			
			tempUrl.Add ( "pwd",password.value );
			
			tempUrl.Add ( "CustomDeviceId", DeviceHelper.GetDeviceInfo() );
			
			AddClientInfo( tempUrl );
			
			#if DEBUG_REQUEST
			//				DebugRequestParams( tempUrl );
			#endif
			
			HttpRequest.Instance().Connect ( CityGlobalData.RigisterURL, 
			                                tempUrl, 
			                                LoginRequestSuccess, 
			                                LoginRequestFail );
		}
	}
	
	void LoginRequestSuccess (string tempResponse) 
	{
		if (isRemember)
		{
			if (string.IsNullOrEmpty (GetStoredUserNameAndPassWord()))
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

		textStr = zhuCeNode ["msg"].Value;
		QXComData.CreateBox (1,textStr,true,null);
	}
	
	void LoginRequestFail (string tempResponse) 
	{
		Debug.LogError( "LoginRequestFail: " + tempResponse );
		textStr = "请选择其他服务器！";
		QXComData.CreateBox (1,textStr,true,null);
	}

	#endregion

	#region Login

	/// <summary>
	/// 登陆
	/// </summary>
	void DengLuRequestSend () 
	{
		WeiXinBtn ();

//		if (SysparaTemplate.CompareSyeParaWord (userName.value) || SysparaTemplate.CompareSyeParaWord (password.value))
//		{
//			textStr = "有奇怪的文字混进来了\n\n再仔细推敲一下吧...";
//			QXComData.CreateBox (1,textStr,true,null);
//		}
//		else
//		{
//
//		}

		if (string.IsNullOrEmpty (userName.value) || string.IsNullOrEmpty (password.value) ||
		    userName.value == "请输入账号名" || password.value == "请输入密码")
		{
			textStr = "账号或密码不能为空！";
			QXComData.CreateBox (1,textStr,true,null);
		}
		
		else
		{
			Dictionary<string,string> tempUrl = new Dictionary<string,string>();
			
			tempUrl.Add("name",userName.value);
			
			tempUrl.Add("pwd", password.value);
			
			tempUrl.Add ( "CustomDeviceId", DeviceHelper.GetDeviceInfo() );
			
			AddClientInfo( tempUrl );
			
			#if DEBUG_REQUEST
			//				DebugRequestParams( tempUrl );
			#endif
			
			// add uuid
			{
				OperationSupport.AppendHttpParamUUID( tempUrl );
			}
			
			HttpRequest.Instance().Connect ( CityGlobalData.LoginURL,
			                                tempUrl,
			                                DengLuRequestSuccess, 
			                                DengLuRequestFail );
		}
	}

	public void DengLuRequestSuccess( string tempResponse ){
		if (string.IsNullOrEmpty (tempResponse)) {
			Debug.LogWarning( "DengLuRequestSuccess.Response.Null." );

			return;
		}

		if (isRemember)
		{
			if( string.IsNullOrEmpty (GetStoredUserNameAndPassWord()))
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

		textStr = dengLuNode ["msg"].Value; //反馈信息
		
		//登陆判定
		switch (code)
		{
		case 0:
//			Debug.Log( "AccountRequest Normal Fail." );

			QXComData.CreateBox (1,textStr,true,null);
			break;
		case 1:

			backBtnObj.SetActive (true);
			loginObj1.SetActive (false);
			loginObj2.SetActive (false);
			lastLoginObj.SetActive (true);
			
			LastLogin last = lastLoginObj.GetComponent<LastLogin> ();
			last.loginNode = dengLuNode;
			last.CreateLastLogin ();
			break;
		default:
			Debug.Log( "AccountRequest Other Fail." );
			break;
		}
	}
	
	void DengLuRequestFail (string tempResponse) 
	{
		// TODO, PopOut Window
		Debug.LogError ( "DengLuRequestFail: " + tempResponse );
		textStr = "请选择其他服务器！";
		QXComData.CreateBox (1,textStr,true,null);
	}

	#endregion



	#region Client Info

	private void DebugRequestParams( Dictionary<string, string> p_dict ){
		foreach( KeyValuePair<string, string> p_kv in p_dict ){
//			Debug.Log( p_kv.Key + ": " + p_kv.Value );
		}
	}

	public static void AddClientInfo( Dictionary<string, string> p_dict ){
		#if DEBUG_REQUEST
		Debug.Log ( "AddClientInfo: " + Time.realtimeSinceStartup );
		#endif

		p_dict.Add( "ClientVersion", VersionTool.GetClientVersionString() );

		p_dict.Add( "SystemSoftware", SystemInfo.operatingSystem );

		#if UNITY_EDITOR || UNITY_STANDALONE
		p_dict.Add( "SystemHardware", "" );
		#else
		p_dict.Add( "SystemHardware", SystemInfo.deviceModel + "|" + DeviceHelper.GetDeviceCompany() );
		#endif

//		Debug.Log( "SystemHardware: " + ( SystemInfo.deviceModel + "|" + DeviceHelper.GetDeviceCompany() ) );

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

		p_dict.Add( "DeviceId", SystemInfo.deviceUniqueIdentifier );
	}

	#endregion

	//显示区状态
	public void ShowServeState (UILabel tempStrLabel,int state)
	{
		tempStrLabel.text = ServeStateDic[state];
	}

	public int isNewPlayer;

	//上次登陆按钮状态
	public void ShowLastLoginBtn (UISprite btnSprite,UILabel btnLabel)
	{
		isNewPlayer = dengLuNode ["isLogined"].AsInt;//新老用户 1.登陆过 2.未登陆过
		Debug.Log ("isNewPlayer:" + isNewPlayer);
		btnSprite.color = Color.white;
		btnLabel.color = Color.white;

		switch (isNewPlayer)
		{
		case 1:

			btnSprite.color = Color.white;
			btnLabel.color = Color.white;
			
			JSONNode lastLogin = dengLuNode ["lastLogin"];

			btnLabel.text = "上次登录" + lastLogin ["id"].AsInt + "区";

			break;

		case 2:

//			btnSprite.color = Color.black;
//			btnLabel.color = Color.black;

			btnLabel.text = "选择分区";

			break;
		}
	}

	void ToggleValueChange ()
	{
		if (isRemember)
		{
			isRemember = false;
//			Debug.Log ("close");
		}
		else
		{
			isRemember = true;
//			Debug.Log ("open");
		}
	}

	public static void CreateReConnectWindow()
    {
//		Debug.Log( "AccountRequest.CreateReConnectWindow()" );

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
		                 false,
						true );
    }

     public static void ReLoginClickCallback(int p_i)
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
		userName.value = QXComData.TextLengthLimit (QXComData.StrLimitType.USER_NAME,userName.value);
		password.value = QXComData.TextLengthLimit (QXComData.StrLimitType.USER_NAME,password.value);
	}

	//输入框上移
	void TransUp ()
	{
		zhezhaoObj.SetActive (true);
		zhezhaoObj.GetComponent<UISprite> ().alpha = 0.1f;

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
	void TransDown ()
	{
		zhezhaoObj.SetActive (false);

		TextLimit ();

		Hashtable down = new Hashtable ();
		
		down.Add ("time",time);
		down.Add ("easetype",iTween.EaseType.easeOutQuart);
		down.Add ("position",new Vector3(0,-55,0));
		down.Add ("islocal",true);
		
		iTween.MoveTo (inputObj,down);
	}

	#region Utilities

	private static string GetStoredUserNameAndPassWord(){
		string  t_string = PlayerPrefs.GetString ("UserNameAndPassWord");

		return t_string;
	}

	#endregion
}
