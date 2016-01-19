//#define MYAPP_ANDROID_PLATFORM



//#define PP_PLATFORM

//#define XY_PLATFORM

//#define TONGBU_PLATFORM

//#define I4_PLATFORM

//#define KUAIYONG_PLATFORM

//#define HAIMA_PLATFORM

//#define I_APPLE_PLATFORM

//#define ITOOLS_PLATFORM



//#define DEBUG_THIRD_PLATFORM

//#define DEBUG_HUGE_LOGIN



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.IO;

#if MYAPP_ANDROID_PLATFORM
using Msdk;
using LitJson;
#endif

using SimpleJSON;

using qxmobile;
using qxmobile.protobuf;

public class ThirdPlatform : MonoBehaviour {

	public enum PlatformType{
		None,
		MyApp_Android_Platform,

		PP_Platform,
		XY_Platform,
		TongBu_Platform,
		I4_Platform,
		KuaiYong_Platform,
		HaiMa_Platform,
		I_Apple_Platform,
		ITools_Platform,
	}
	
	#if MYAPP_ANDROID_PLATFORM
	private PlatformType m_platform_type = PlatformType.MyApp_Android_Platform;
	#elif PP_PLATFORM
	private PlatformType m_platform_type = PlatformType.PP_Platform;
	#elif XY_PLATFORM
	private PlatformType m_platform_type = PlatformType.XY_Platform;
	#elif TONGBU_PLATFORM
	private PlatformType m_platform_type = PlatformType.TongBu_Platform;
	#elif I4_PLATFORM
	private PlatformType m_platform_type = PlatformType.I4_Platform;
	#elif KUAIYONG_PLATFORM
	private PlatformType m_platform_type = PlatformType.KuaiYong_Platform;
	#elif HAIMA_PLATFORM
	private PlatformType m_platform_type = PlatformType.HaiMa_Platform;
	#elif I_APPLE_PLATFORM
	private PlatformType m_platform_type = PlatformType.I_Apple_Platform;
	#elif ITOOLS_PLATFORM
	private PlatformType m_platform_type = PlatformType.ITools_Platform;
	#else
	private PlatformType m_platform_type = PlatformType.None;
	#endif


	private static ThirdPlatform m_instance = null;

	public static ThirdPlatform Instance(){
		if( m_instance == null ){
			Debug.LogError( "Error, ThirdPlatform.m_instance = null." );
		}

		return m_instance;
	}

	#region Mono

	void Awake(){
//		Debug.Log( "ThirdPlatform.Awake()" );

		m_instance = this;

		SetPlatformStatus( PlatformStatus.LogOut );
	}
	
	void Start () {
//		Debug.Log( "ThirdPlatform.Start()" );

		if( IsMyAppAndroidPlatform() ){
			#if MYAPP_ANDROID_PLATFORM
			Debug.Log( "WGPlatform.Init()" );

			WGPlatform.Instance.Init ();
			#endif
		}
		else if ( IsPPPLatform () ) {
			#if PP_PLATFORM
			Bonjour.initSDK( 6469,"34deda633a232a335d363ef4520b770e", UtilityTool.GetDontDestroyGameObjectName() );
			
			Bonjour.startPPSDK();
			#endif
		} 
		else if ( IsI4Platform () ) {
			#if I4_PLATFORM
			Bonjour.pxAsInit();
			#endif
		}
		else if( IsHaiMaPlatform() ) {
			#if HAIMA_PLATFORM
			Bonjour.Public_XHPaySetUnityReceiver( this.transform.name );

			Bonjour.ZHPayInit();

			Bonjour.ZHPaySetSupportOrientation( Bonjour.ZHPayOrientation.ZHPayOrientationLandscape );
			#endif
		}
	}

	void Update(){
		ProcessToken ();
	}

	private float[] m_btn_rect_params = new float[ 6 ];
	
//	void OnGUI(){
//		#if DEBUG_THIRD_PLATFORM
//		OnGUIThird();
//		#endif
//	}

//	void OnGUIThird(){
//		{
//			m_btn_rect_params[ 0 ] = Screen.width * 0.8f;
//			
//			m_btn_rect_params[ 1 ] = Screen.height * 0.1f;
//			
//			m_btn_rect_params[ 2 ] = Screen.width * 0.1f;
//			
//			m_btn_rect_params[ 3 ] = Screen.height * 0.08f;
//			
//			m_btn_rect_params[ 4 ] = 0;
//			
//			m_btn_rect_params[ 5 ] = Screen.height * 0.1f;
//		}
//		
//		int t_button_index = 0;
//		
//		Rect t_rect = GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params );
//
//		if( GUI.Button( t_rect, "Show Center" ) ){
//			ThirdPlatform.ShowSDKCenter();
//		}
//		
//		if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "Log Out" ) ){
//			ThirdPlatform.LogOut();
//		}
//		
//		if( GUI.Button( GUIHelper.GetGUIRect( t_button_index++, m_btn_rect_params ), "Start Bundle Update" ) ){
//			ThirdPlatform.ThirdPlatformLoginSuccess();
//		}
//	}

	void OnDestroy(){
		m_instance = null;
	}

	#endregion



	#region Game

	public const string POPUP_TIPS_TITLE							= "平台登陆";

	/// happened in 2 places, bundle updating and relogin.
	public static void ThirdPlatformLoginSuccess(){
		Debug.Log( "ThirdPlatform.ThirdPlatformLoginSuccess()" );


		if ( PrepareBundles.GetBundleUpdateState () < PrepareBundles.UpdateState.CHECKING_UPDATE_INFO ) {
			if( PrepareBundles.Instance() == null ){
				Debug.LogError( "Prepare_Bundle_Config.instance == null, Error, in Wrong Place." );
				
				return;
			}

			PrepareBundles.Instance ().UpdateServerSelected ( null );
		}
		else if ( PrepareBundles.GetBundleUpdateState () == PrepareBundles.UpdateState.PREPARE_START_GAME ) {
			PrepareBundles.BundleUpdateDone ();
		}
		else {
			Debug.LogError( "ThirdPlatformLoginSuccess in wrong state: " + PrepareBundles.GetBundleUpdateState () );

			ThirdPlatform.Instance().SetPlatformStatus( PlatformStatus.LogOut );
		}
		
		#if TONGBU_PLATFORM
		Bonjour.showSDKToolBar();
		#endif
	}

	public static void StartGame(){
		Debug.Log( "ThirdPlatform.StartGame()" );

		PrepareBundles.StartGame();
	}

	public void LogOutCallback( bool p_check_sdk ){
		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "LogOutCallback()" );
		#endif

		SetPlatformStatus( PlatformStatus.LogOut );

		if( p_check_sdk ){
			// kuaiyong sdk login workflow error, should check manually
			if( ThirdPlatform.IsKuaiYongPlatform() ){
				CheckLoginToShowSDK();
			}

		}

		if ( PrepareBundles.Instance () != null ) {
			Debug.Log( "Still In Version Check, Skip ReLogin Ops." );
			
			return;
		}
		
		SceneManager.EnterLogin();
	}

	#endregion
	
	
	
	#region Game UI
	
	private static void ShowErrorBox( string p_error_string, UIBox.onclick p_click ){
		Global.CreateBox( PrepareBundleHelper.POPUP_TIPS_TITLE,
		                 p_error_string,
		                 "",
		                 null,
		                 PrepareBundleHelper.BUTTON_TXT_OK, 
		                 null, 
		                 p_click,
		                 null,
		                 null,
		                 null,
		                 false,
		                 false,
		                 false );
	}
	
	public void BoxDoNone( int p_int ){
		Debug.Log( "BoxDoNone( " + p_int + " )" );

		CheckLoginToShowSDK ();
	}
	
	public void BoxDoCheckLoginToShowSDK( int p_int ){
		Debug.Log( "BoxDoCheckLoginToShowSDK( " + p_int + " )" );
		
		CheckLoginToShowSDK();
	}
	
	#endregion



	#region Network

	string m_login_info = "";

	public string GetLoginInfo(){
		return m_login_info;
	}

	public void UploadToken(){
//		qxrouter/channel/checkLogin.jsp?channel=XY&sid=80a5fe53d3540300005a17e308a4b1fb

		#if DEBUG_HUGE_LOGIN
		string t_url = HttpRequest.SERVER_HUGE_PREFIX + "qxrouter/channel/checkLogin.jsp";
		#else
		string t_url = NetworkHelper.GetPrefix() + NetworkHelper.THIRD_PLATFORM_LOGIN_URL;
		#endif

		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "UploadToken( " + t_url + "   " + m_platform_type + " )" );
		#endif

		{
			Dictionary< string,string > t_request_params = new Dictionary<string,string>();
			
			{
				// add 3rd platform info
				if( IsPPPLatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetPPToken() );
				}
				else if( IsMyAppAndroidPlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetMyAppToken() );
				}
				else if( IsXYPlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetXYToken() );

					t_request_params.Add( "uid", GetXYUid() );
				}
				else if( IsTongBuPlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetTongBuSession() );
				}
				else if( IsI4Platform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetI4Token() );
				}
				else if( IsKuaiYongPlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetKuaiYongToken() );
				}
				else if( IsHaiMaPlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetHaiMaToken() );

					#if HAIMA_PLATFORM
					t_request_params.Add( "accessid", Bonjour.ZHPayGetUserId() );
					#endif
				}
				else if( IsIApplePlatform() ){
					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetIAppleToken() );

					t_request_params.Add( "user_id", GetIAppleUserId() );
				}
				else if( IsIToolsPlatform() ){
					#if DEBUG_THIRD_PLATFORM
					Debug.Log( "GetIToolsToken: " + GetIToolsToken() );
					#endif

					t_request_params.Add( "channel", GetPlatformTag() );
					
					t_request_params.Add( "sid", GetIToolsToken() );
				}
				else{
					Debug.LogError( "Error, Wrong Position." );
				}

				// add uuid
				{
					OperationSupport.AppendHttpParamUUID( t_request_params );
				}

				// report client detail infomation
				{
					AccountRequest.AddClientInfo( t_request_params );
				}
			}
			
			HttpRequest.Instance().Connect( t_url, 
			                               t_request_params, 
			                               TokenUploadSuccessCallback, 
			                               TokenUploadFailCallback ); 
		}
	}

	public void TokenUploadSuccessCallback( string p_response ){
		Debug.Log( "TokenUploadSuccessCallback()" );

		m_login_info = p_response;

		JSONNode t_node = JSON.Parse( p_response );

		#if DEBUG_THIRD_PLATFORM
		Debug.Log( t_node.ToString( "" ) );
		#endif

		int t_code = t_node[ "code" ].AsInt;

		string t_msg = t_node[ "msg" ].Value;

		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "t_code: " + t_code );

		Debug.Log( "t_msg: " + t_msg );
		#endif

		if( t_code == 1 ){
			SetPlatformStatus( PlatformStatus.LogIn );

			StartGame();
		}
		else if( t_code == -2 ){
			#if DEBUG_THIRD_PLATFORM
			Debug.Log( "TokenUploadSuccessCallback: " + t_code );
			#endif

			SetPlatformStatus( PlatformStatus.LogOut );

			ShowErrorBox( "登录失败.", BoxDoNone );
		}
		else{
			#if DEBUG_THIRD_PLATFORM
			Debug.Log( "TokenUploadSuccessCallback: " + t_code );
			#endif

			SetPlatformStatus( PlatformStatus.LogOut );

			ShowErrorBox( "其他失败.", BoxDoNone );
		}
	}
	
	public void TokenUploadFailCallback( string p_response ){
		Debug.LogError ( "TokenUploadFailCallback( " + p_response + " )" );

//		ShowErrorBox( p_response, BoxDoNone );

		ShowErrorBox( p_response, TokenLoadFailExec );
	}

	public void TokenLoadFailExec( int p_int ){
		Debug.Log( "TokenLoadFailExec( " + p_int + " )" );
		
		BoxDoNone( p_int );

		ThirdPlatform.Instance().UploadToken();
	}

	#endregion



	#region Platform Status

	public enum PlatformStatus{
		LogOut,

		// only HaiMa use this, start from sdk show to game server verify done.
		Verifying,	

		LogIn,
	}

	private PlatformStatus m_platform_status = PlatformStatus.LogOut;

	public PlatformStatus GetPlatformStatus(){
		return m_platform_status;
	}

	public void SetPlatformStatus( PlatformStatus p_status ){
		#if DEBUG_THIRD_PLATFORM
		Debug.Log( "SetPlatformStatus( " + p_status + " )" );
		#endif

		m_platform_status = p_status;
	}

	#endregion



	#region Platform
	
	public static void CheckLoginToShowSDK(){
		Debug.Log( "ThirdPlatform.CheckLoginToShowSDK()" );

		if (m_instance == null) {
			Debug.Log( "ThirdPlatform not initialized." );

			return;
		}

		if ( Instance ().GetPlatformStatus() == PlatformStatus.LogOut ) {
			Debug.Log( "Ready for next login turn( " + Instance().GetPlatformStatus() + " )" );

			ShowSDKCenter ();
		}
		else {
			Debug.Log( "Previous Login turn not done: " + Instance().GetPlatformStatus() );
		}
	}
	
	public static void ShowSDKCenter(){
		Debug.Log( "ThirdPlatform.ShowSDKCenter()" );

		#if HAIMA_PLATFORM
		ThirdPlatform.Instance().SetPlatformStatus( PlatformStatus.Verifying );
		#endif

		Bonjour.showSDKCenter();
	}

	public static void LogOut(){
		Debug.Log( Time.realtimeSinceStartup + " ThirdPlatform.LogOut()" );

		if ( m_instance != null ){
			Instance().SetPlatformStatus( PlatformStatus.LogOut );
		}
		else{
			Debug.LogError( "Error, instance = null." );
		}

		Bonjour.logout();
	}
	
	#endregion



	#region MyApp Platform
	private string m_myapp_login_state;
	
	private string m_myapp_login_message;
	
	public const string PLATFORM_MY_APP_ANDROID_TAG 		= "MyApp";
	
	private Msdk.LoginRet m_my_app_login_ret	= null;
	
	//TODO GAME 回调实现，游戏可以参考下面代码
	
	/// <summary>
	///  登陆回调
	/// </summary>
	/// <param name="jsonRet">Json ret.</param>
	void OnLoginNotify( string jsonRet ){
		Debug.Log( "OnLoginNotify=" + jsonRet );
		
		#if MYAPP_ANDROID_PLATFORM
		
		Msdk.LoginRet ret = Msdk.LoginRet.ParseJson( jsonRet );
		
		{
			m_my_app_login_ret = ret;
		}
		
		if ( ret == null ) {
			
			m_myapp_login_state = "登陆失败";
			
			m_myapp_login_message = " ";
			
			return;
		}
		
		Debug.Log ( "MyApp.OnLoginNotify( " + ret + " )" );
		
		/*
		 *  loginRet.platform表示当前的授权平台, 值类型为ePlatform, 可能值为ePlatform_QQ, ePlatform_Weixin
	 	 *     loginRet.flag值表示返回状态, 可能值(eFlag枚举)如下：
		 *       eFlag_Succ: 返回成功, 游戏接收到此flag以后直接读取LoginRet结构体中的票据进行游戏授权流程.
		 *       eFlag_QQ_NoAcessToken: 手Q授权失败, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
		 *       eFlag_QQ_UserCancel: 用户在授权过程中
		 *       eFlag_QQ_LoginFail: 手Q授权失败, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
		 *       eFlag_QQ_NetworkErr: 手Q授权过程中出现网络错误, 游戏接收到此flag以后引导用户去重新授权(重试)即可.
		 *     loginRet.token是一个List<TokenRet>, 其中存放的TokenRet有type和value, 通过遍历Vector判断type来读取需要的票据. type(TokenType)类型定义如下:
		 *       eToken_QQ_Access,
		 *       eToken_QQ_Pay,
		 *       eToken_WX_Access,
		 *       eToken_WX_Refresh
		 */
		switch ( ret.flag ){
		case eFlag.eFlag_Succ:				
			// 登陆成功, 可以读取各种票据				
			int platform= ret.platform;
			
			if(ePlatform.ePlatform_Weixin == platform){
				m_myapp_login_state = "微信登陆成功";
			}
			else if(ePlatform.ePlatform_QQ == platform){
				m_myapp_login_state = "QQ登陆成功";
			}
			else if(ePlatform.ePlatform_QQHall == platform){
				m_myapp_login_state = "大厅登陆成功";
			}
			
			m_myapp_login_message = ret.ToString();
			
			{
				ThirdPlatformLoginSuccess();
			}
			
			break;
			
		case eFlag.eFlag_WX_RefreshTokenSucc:
			Debug.Log( "微信票据刷新成功." );
			break;
			
		case eFlag.eFlag_WX_AccessTokenExpired:
			Debug.Log( "微信票据过期，重刷新token." );
			
			WGPlatform.Instance.WGRefreshWXToken();
			break;
			
			
			// 游戏逻辑，对登陆失败情况分别进行处理
		case eFlag.eFlag_Local_Invalid:
			// 自动登录失败, 需要重新授权, 包含本地票据过期, 刷新失败登所有错误
			m_myapp_login_state = "自动登陆失败";
			m_myapp_login_message = ret.desc;
			break;
			
		case eFlag.eFlag_WX_UserCancel:
		case eFlag.eFlag_WX_NotInstall:
		case eFlag.eFlag_WX_NotSupportApi:
		case eFlag.eFlag_WX_LoginFail:
			
		default:
			m_myapp_login_state = "登陆失败";
			m_myapp_login_message = ret.desc;
			
			{
				Debug.Log( "LoginFail.flag: " + ret.flag );
				
				LogOutCallback ( true );
				
				CheckLoginToShowSDK();
			}
			
			break;
		}
		
		Debug.Log( "MyApp.Login.State: " + m_myapp_login_state );
		
		Debug.Log( "MyApp.Login.Message: " + m_myapp_login_message );
		
		#endif
	}
	
	public static string GetMyAppToken(){
		#if MYAPP_ANDROID_PLATFORM
		return Instance().m_my_app_login_ret.GetAccessToken();
		#else
		return "";
		#endif
	}
	
	public void AndroidCallString( string p_param ){
		Debug.Log( "AndroidCallString( " + p_param + " )" );
	}
	
	#endregion



	#region PP Platform

	public const string PLATFORM_PP_TAG = "PP";

	private string m_pp_token = "";

	public static string GetPPToken(){
		return Instance().m_pp_token;
	}

	void U3D_ppDylibLoadSucceedCallBack(){
		Debug.Log( Time.realtimeSinceStartup + "ThirdPlatform.U3D_ppDylibLoadSucceedCallBack()" );

		#if PP_PLATFORM
		Bonjour.login();
		#endif
	}
	
	//paramNoti:[30s_token]
	void U3D_ppLoginSuccessCallBack( string paramNoti ){
		Debug.Log( Time.realtimeSinceStartup + "ThirdPlatform.U3D_ppLoginSuccessCallBack( " + paramNoti + " )" );

		#if PP_PLATFORM
		Bonjour.tokenVerifyCallBack( true );
		#endif

		m_pp_token = paramNoti;

		{
			ThirdPlatformLoginSuccess();
		}
	}
	
	void U3D_ppLogOffCallBack(){
		Debug.Log( Time.realtimeSinceStartup + "ThirdPlatform.U3D_ppLogOffCallBack()" );

		LogOutCallback ( true );
	}
	
	//paramNoti:[Common.PPPayResultCode]
	void U3D_ppPayResultCallBack( string paramNoti ){
		if(int.Parse(paramNoti) == (int)Common.PPPayResultCode.PPPayResultCodeSucceed){//pay success
			Debug.Log( "ThirdPlatform.U3D_ppPayResultCallBack() " + paramNoti + "pay success" );
		}else{
			Debug.Log( "ThirdPlatform..U3D_ppPayResultCallBack() " + paramNoti + "pay failer" );
		}
	}
	
	void U3D_ppCenterDidShowCallBack(){	
		Debug.Log( Time.realtimeSinceStartup + "ThirdPlatform.U3D_ppCenterDidShowCallBack()" );

	}

	void U3D_ppCenterDidCloseCallBack(){
		Debug.Log( Time.realtimeSinceStartup + "ThirdPlatform.U3D_ppCenterDidCloseCallBack()" );

		CheckLoginToShowSDK();
	}

	#endregion
		
	

	#region XY Platform

	public const string PLATFORM_XY_TAG = "XY";

	private string m_xy_token = "";

	private string m_xy_uid = "";

	public static string GetXYToken(){
		return Instance().m_xy_token;
	}

	public static string GetXYUid(){
		return Instance ().m_xy_uid;
	}

	public void XYSetToken( string p_msg ){
		Debug.Log ( "ThirdPlatform.XYSetToken( " + p_msg + " )" );

		m_xy_token = p_msg;
	}

	public void XYSetUid( string p_msg ){
		Debug.Log ( "ThirdPlatform.XYSetUid( " + p_msg + " )" );

		m_xy_uid = p_msg;
	}

	public void XYLoginNotice(){
		Debug.Log ( "ThirdPlatform.XYLoginNotice( " + " )" );

		{
			ThirdPlatformLoginSuccess();
		}
	}

	public void XYLogoutFinish(){
		Debug.Log ( "ThirdPlatform.XYLogoutFinish( " + " )" );

		LogOutCallback ( true );
	}

	public void XYLeavePlatform(){
		Debug.Log ( "ThirdPlatform.XYLeavePlatform( " + " )" );

		CheckLoginToShowSDK();
	}

	public void XYGuestTurnOfficial(){
		Debug.Log ( "ThirdPlatform.XYGuestTurnOfficial( " + " )" );
	}

	#endregion



	#region TongBu Platform

	public const string PLATFORM_TONGBU_TAG = "TongBu";

	private string m_tongbu_session = "";

	public static string GetTongBuSession(){
		return Instance().m_tongbu_session;
	}
	
	public void TongBuSetSession( string p_msg ){
		Debug.Log ( "ThirdPlatform.TongBuSetSession( " + p_msg + " )" );
		
		m_tongbu_session = p_msg;

		{
			ThirdPlatformLoginSuccess();
		}
	}

	public void TongBuLogoutFinish(){
		Debug.Log ( "ThirdPlatform.TongBuLogoutFinish( " + " )" );

		LogOutCallback ( true );
	}
	
	public void TongBuLeavePlatform(){
		Debug.Log ( "ThirdPlatform.TongBuLeavePlatform( " + " )" );

		CheckLoginToShowSDK();
	}
	
	public void TongBuGuestTurnOfficial(){
		Debug.Log ( "ThirdPlatform.TongBuGuestTurnOfficial( " + " )" );
	}
	
	#endregion



	#region I4 Platform

	public const string PLATFORM_AISI_TAG = "AiSi";
	
	private string m_i4_token = "";

	private string m_i4_id = "";


	public static string GetI4Token(){
		return Instance().m_i4_token;
	}

	public static string GetI4Id(){
		return Instance ().m_i4_id;
	}

	public void I4SetId( string p_msg ){
		Debug.Log ( "ThirdPlatform.I4SetId( " + p_msg + " )" );

		m_i4_id = p_msg;
	}
	
	public void I4SetToken( string p_msg ){
		Debug.Log ( "ThirdPlatform.I4SetToken( " + p_msg + " )" );
		
		m_i4_token = p_msg;
		
		{
			ThirdPlatformLoginSuccess();
		}
	}
	
	public void I4LogoutFinish(){
		Debug.Log ( "ThirdPlatform.I4LogoutFinish( " + " )" );
		
		LogOutCallback (true);
	}

	public void I4ClosePageView(){
		Debug.Log ( "ThirdPlatform.I4ClosePageView( " + " )" );
		
		CheckLoginToShowSDK();
	}

	public void I4CloseLoginView(){
		Debug.Log ( "ThirdPlatform.I4CloseLoginView( " + " )" );
		
		CheckLoginToShowSDK();
	}

	#endregion



	#region KuaiYong Platform

	public const string PLATFORM_KUAIYONG_TAG = "KuaiYong";

	private string m_kuaiyong_token = "";
	
	public static string GetKuaiYongToken(){
		return Instance().m_kuaiyong_token;
	}
	
	public void KuaiYongSetToken( string p_msg ){
		Debug.Log ( "ThirdPlatform.KuaiYongSetToken( " + p_msg + " )" );
		
		m_kuaiyong_token = p_msg;
		
		{
			ThirdPlatformLoginSuccess();
		}
	}

//	public void KuaiYongSubAccountLogoutFinish(){
//		Debug.Log ( "ThirdPlatform.KuaiYongSubAccountLogoutFinish( " + " )" );
//
//		KuaiYongLogout ( false );
//	}
	
	public void KuaiYongLogoutFinish(){
		Debug.Log ( "ThirdPlatform.KuaiYongLogoutFinish( " + " )" );
		
		LogOutCallback ( true );
	}

//	private void KuaiYongLogout( bool p_check_sdk ){
//		LogOutCallback ( p_check_sdk );
//	}
	
	public void KuaiYongLeavePlatform(){
		Debug.Log ( "ThirdPlatform.KuaiYongLeavePlatform( " + " )" );
		
		CheckLoginToShowSDK();
	}

	#endregion



	#region HaiMa Platform
	
	public const string PLATFORM_HAIMA_TAG = "HaiMa";
	
	private string m_haima_token = "";
	
	public static string GetHaiMaToken(){
		return Instance().m_haima_token;
	}

	public void ZHPayLoginSuccess( string p_msg ){
		Debug.Log ( "ThirdPlatform.HaiMaSetToken( " + p_msg + " )" );

		Debug.Log( "Should see this or other." );

		// 回调：账号登陆成功
		Dictionary<string,string> userInfo = Bonjour.SplitStringToDic( p_msg );
		string userId = userInfo["userId"];                         //用户ID，唯一标识
		string validateToken = userInfo["validateToken"];   //登陆验证token，可用于服务器登录有效性验证，详见文档
		string userName = userInfo["userName"];              //用户名，1.3.0版将弃用，首次接入的开发者不要使用，老开发者可继续使用
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：账号登陆成功ID:"+userId, SendMessageOptions.RequireReceiver);

		m_haima_token = validateToken;
		
		{
			ThirdPlatformLoginSuccess();
		}
	}

	
	void ZHPayLoginCancel()
	{
		// TODO: 回调：登录取消 
		//		TestObj.SendMessage("ShowResult","ZHPay回调：登录取消",SendMessageOptions.RequireReceiver);
		Debug.Log ( "ThirdPlatform.ZHPayLoginCancel()" );

		Debug.Log( "Should see this or other." );

		// prepare for next login turn
		SetPlatformStatus( PlatformStatus.LogOut );

		CheckLoginToShowSDK();
	}


	private void ZHPayDidLogout(){
		Debug.Log ( "ThirdPlatform.ZHPayDidLogout()" );

		#if HAIMA_PLATFORM
		if( ThirdPlatform.Instance().GetPlatformStatus() == PlatformStatus.Verifying ){
			Debug.Log( "Previous verifying not done, return." );

			return;
		}
		#endif

		LogOutCallback ( true );
	}
	
	public void ZHPayViewOut(){
		Debug.Log ( "ThirdPlatform.ZHPayViewOut()" );
		
		CheckLoginToShowSDK();
	}

//	void ZHPayDidLogout()
//	{
//		// 回调：账号注销成功，此时需要将游戏退出，并切换到登录前画面
//		TestObj.SendMessage("ShowResult","ZHPay回调：注销成功",SendMessageOptions.RequireReceiver);
//	}
	
	void ZHPayViewIn()
	{
		// 回调：SDK界面出现
//		TestObj.SendMessage("ShowResult","ZHPay回调：SDK界面出现",SendMessageOptions.RequireReceiver);
	}
	
//	void ZHPayViewOut()
//	{
//		// 回调：SDK界面退出
//		TestObj.SendMessage("ShowResult","ZHPay回调：SDK界面退出",SendMessageOptions.RequireReceiver);
//	}
	
	void ZHPayCheckUpdateFinish(string result)
	{
		//回调：检查更新回调
		Dictionary<string,string> resultInfo = Bonjour.SplitStringToDic( result );
		bool isSuccess = (resultInfo["isSuccess"]=="1");
		bool needUpdate = (resultInfo["needUpdate"]=="1");
		bool isForce = (resultInfo["isForce"]=="1");
		/*在此可添加您的处理*/
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：检查更新完毕，是否请求成功:" + isSuccess + " 是否需要更新:" + needUpdate + " 是否强制更新:" + isForce ,SendMessageOptions.RequireReceiver);
	}
	
	void ZHPayResultSuccessWithOrder(string result)
	{
		// 回调：订单支付成功
		Dictionary<string,string> orderInfo = Bonjour.SplitStringToDic(result);
		string orderId = orderInfo["orderId"];
		string productName = orderInfo["productName"];
		string productDescription = orderInfo["gameName"];
		string productPrice = orderInfo["productPrice"];
		string userParam = orderInfo["userParam"];
		/*在此可处理您的订单*/
		
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：订单支付成功："+result,SendMessageOptions.RequireReceiver);
	}
	
	void ZHPayResultFailedWithOrder(string result)
	{
		// 回调：订单支付失败
		Dictionary<string,string> orderResult = Bonjour.SplitStringToDic(result);
		string errorCode = orderResult["errorCode"];
		string orderId = orderResult["orderId"];
		string productName = orderResult["productName"];
		string productDescription = orderResult["gameName"];
		string productPrice = orderResult["productPrice"];
		string userParam = orderResult["userParam"];
		// errorCode："0":余额不足  "1":订单创建错误  "2":重复提交订单,请更换订单号  "3":网络不通畅（可能已购买成功但客户端已超时,建议去自己服务器进行订单查询）  "4":服务器错误  "5":其它错误
		/*在此可处理您的订单*/
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：订单支付失败："+result,SendMessageOptions.RequireReceiver);
	}
	
	void ZHPayResultCancelWithOrder(string result)
	{
		// 回调：用户中途取消支付
		Dictionary<string,string> orderInfo = Bonjour.SplitStringToDic(result);
		string orderId = orderInfo["orderId"];
		string productName = orderInfo["productName"];
		string productDescription = orderInfo["gameName"];
		string productPrice = orderInfo["productPrice"];
		string userParam = orderInfo["userParam"];
		/*在此可处理您的订单*/
		
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：用户中途取消支付："+result,SendMessageOptions.RequireReceiver);
	}
	
	void ZHCheckOrderFinishedWithOrder(string result)
	{
		// 回调：查询订单完毕
		Dictionary<string,string> orderResult = Bonjour.SplitStringToDic(result);
		string status = orderResult["status"];
		string orderId = orderResult["orderId"];
		string money = orderResult["money"];
		// status：0:待支付  1:已支付  2:过期失效（未曾支付） 3: 订单不存在（或未完成支付流程）  4:支付失败
		/*在此可处理您的订单*/
		
//		TestObj.SendMessage("ShowResult","ZHPay回调：查询订单完毕："+result,SendMessageOptions.RequireReceiver);
	}
	
	void ZHCheckOrderDidFailed(string orderId)
	{
		// 回调：查询订单失败
//		TestObj.SendMessage("ShowResult","ZHPay回调：查询订单失败，订单号："+orderId,SendMessageOptions.RequireReceiver);
	}


	#endregion



	#region I Apple Platform
	
	public const string PLATFORM_I_APPLE_TAG = "IApple";
	
	private string m_i_apple_token = "";

	private string m_i_apple_user_id = "";
	
	public static string GetIAppleToken(){
		return Instance().m_i_apple_token;
	}

	public static string GetIAppleUserId(){
		return Instance ().m_i_apple_user_id;
	}

	public void IAppleSetUserId( string p_msg ){
		Debug.Log ( "ThirdPlatform.IAppleSetUserId( " + p_msg + " )" );
		
		m_i_apple_user_id = p_msg;
	}
	
	public void IAppleSetToken( string p_msg ){
		Debug.Log ( "ThirdPlatform.IAppleSetToken( " + p_msg + " )" );
		
		m_i_apple_token = p_msg;
		
		{
			if( string.IsNullOrEmpty( m_i_apple_user_id ) ){
				Debug.LogError( "Error, user id not setted." );
			}

			ThirdPlatformLoginSuccess();
		}
	}

	public void IAppleLeavePlatform(){
		Debug.Log ( "ThirdPlatform.IAppleLeavePlatform( " + " )" );
		
		CheckLoginToShowSDK();
	}

	public void IAppleLogoutFinish(){
		Debug.Log ( "ThirdPlatform.IAppleLogoutFinish( " + " )" );
		
		LogOutCallback ( true );
	}
	
//	private void IAppleLogout( bool p_check_sdk ){
//		LogOutCallback ( p_check_sdk );
//	}
	
	#endregion



	#region ITools Platform
	
	public const string PLATFORM_ITOOLS_TAG = "iTools";
	
	private string m_itools_token = "";
	
	public static string GetIToolsToken(){
		return Instance().m_itools_token;
	}
	
	public void IToolsSetToken( string p_msg ){
		Debug.Log ( "ThirdPlatform.IToolsSetToken( " + p_msg + " )" );
		
		m_itools_token = p_msg;
		
		{
			ThirdPlatformLoginSuccess();
		}
	}
	
	//	public void KuaiYongSubAccountLogoutFinish(){
	//		Debug.Log ( "ThirdPlatform.KuaiYongSubAccountLogoutFinish( " + " )" );
	//
	//		KuaiYongLogout ( false );
	//	}
	
	public void IToolsLogoutFinish(){
		Debug.Log ( "ThirdPlatform.IToolsLogoutFinish( " + " )" );
		
		LogOutCallback ( true );
	}
	
//	private void IToolsLogout( bool p_check_sdk ){
//		LogOutCallback ( p_check_sdk );
//	}
	
	public void IToolsLeavePlatform(){
		Debug.Log ( "ThirdPlatform.IToolsLeavePlatform( " + " )" );
		
		CheckLoginToShowSDK();
	}
	
	#endregion


	
	#region XG
	
	private static string m_xg_token = "";
	
	private static bool m_xg_token_sended = false;
	
	private void ProcessToken(){
		if ( m_xg_token_sended ) {
			//			Debug.Log( "Already Sended." );
			
			return;
		}
		
		if ( !SocketTool.IsConnected () ) {
			//			Debug.Log( "Waiting For Socket Connection." );
			
			return;
		}
		
		if ( !IsXGTokeGetted ()) {
			//			Debug.Log( "Waiting For XG Token." );
			
			return;
		}
		
		{
			ErrorMessage t_error_message = new ErrorMessage();
			
			t_error_message.errorDesc = m_xg_token;
			
			t_error_message.cmd = 0;
			
			t_error_message.errorCode = 0;
			
			SocketHelper.SendQXMessage( t_error_message, ProtoIndexes.C_XG_TOKEN );
			
			Debug.Log( "Upload XG Token: " + t_error_message.errorCode + 
			          "   Desc: " + t_error_message.errorDesc +
			          "   Client: " + t_error_message.cmd );
		}
		
		{
			m_xg_token_sended = true;
		}
	}
	
	public static bool IsXGTokeGetted(){
		if (string.IsNullOrEmpty (m_xg_token)) {
			return false;
		}
		
		return true;
	}
	
	public static string GetXGToken(){
		return m_xg_token;
	}
	
	public void XGSetToken( string p_token ){
		Debug.Log ( "ThirdPlatform.XGSetToken( " + p_token + " )" );
		
		m_xg_token = p_token;
		
		m_xg_token_sended = false;
	}
	
	#endregion
	
	
	
	#region Platform Type
	
	public static PlatformType GetPlatformType(){
		#if MYAPP_ANDROID_PLATFORM
		return PlatformType.MyApp_Android_Platform;
		#elif PP_PLATFORM
		return PlatformType.PP_Platform;
		#elif XY_PLATFORM
		return PlatformType.XY_Platform;
		#elif TONGBU_PLATFORM
		return PlatformType.TongBu_Platform;
		#elif I4_PLATFORM
		return PlatformType.I4_Platform;
		#elif KUAIYONG_PLATFORM
		return PlatformType.KuaiYong_Platform;
		#elif HAIMA_PLATFORM
		return PlatformType.HaiMa_Platform;
		#elif I_APPLE_PLATFORM
		return PlatformType.I_Apple_Platform;
		#elif ITOOLS_PLATFORM
		return PlatformType.ITools_Platform;
		#else
		return PlatformType.None;
		#endif
	}
	
	public static string GetPlatformTag(){
		#if MYAPP_ANDROID_PLATFORM
		return PLATFORM_MY_APP_ANDROID_TAG;
		#elif PP_PLATFORM
		return PLATFORM_PP_TAG;
		#elif XY_PLATFORM
		return PLATFORM_XY_TAG;
		#elif TONGBU_PLATFORM
		return PLATFORM_TONGBU_TAG;
		#elif I4_PLATFORM
		return PLATFORM_AISI_TAG;
		#elif KUAIYONG_PLATFORM
		return PLATFORM_KUAIYONG_TAG;
		#elif HAIMA_PLATFORM
		return PLATFORM_HAIMA_TAG;
		#elif I_APPLE_PLATFORM
		return PLATFORM_I_APPLE_TAG;
		#elif ITOOLS_PLATFORM
		return PLATFORM_ITOOLS_TAG;
		#else
		return "Default";
		#endif
	}
	
	public static string GetPlatformSession(){
		if ( IsPPPLatform() ){
			return GetPPToken();
		} 
		else if( IsMyAppAndroidPlatform() ){
			return GetMyAppToken();
		}
		else if ( IsXYPlatform() ) {
			return GetXYToken();
		} 
		else if ( IsTongBuPlatform() ) {
			return GetTongBuSession();
		}
		else if ( IsI4Platform()) {
			return GetI4Token();
		} 
		else if ( IsKuaiYongPlatform() ) {
			return GetKuaiYongToken();
		} 
		else if ( IsHaiMaPlatform() ) {
			return GetHaiMaToken();
		}
		else if ( IsIApplePlatform() ){
			return GetIAppleToken();
		}
		else if( IsIToolsPlatform() ){
			return GetIToolsToken();
		}
		else{
			return "Default";
		}
	}
	
	public static bool IsThirdPlatform(){
		return !( GetPlarformType () == PlatformType.None );
	}
	
	public static bool IsMyAppAndroidPlatform(){
		return GetPlarformType() == PlatformType.MyApp_Android_Platform;
	}
	
	public static bool IsPPPLatform(){
		return GetPlarformType() == PlatformType.PP_Platform;
	}
	
	public static bool IsXYPlatform(){
		return GetPlarformType() == PlatformType.XY_Platform;
	}
	
	public static bool IsTongBuPlatform(){
		return GetPlarformType() == PlatformType.TongBu_Platform;
	}
	
	public static bool IsI4Platform(){
		return GetPlarformType() == PlatformType.I4_Platform;
	}
	
	public static bool IsKuaiYongPlatform(){
		return GetPlarformType () == PlatformType.KuaiYong_Platform;
	}
	
	public static bool IsHaiMaPlatform(){
		return GetPlatformType () == PlatformType.HaiMa_Platform;
	}
	
	public static bool IsIApplePlatform(){
		return GetPlarformType () == PlatformType.I_Apple_Platform;
	}
	
	public static bool IsIToolsPlatform(){
		return GetPlarformType () == PlatformType.ITools_Platform;
	}
	
	public static PlatformType GetPlarformType(){
		if( m_instance == null ){
			return PlatformType.None;
		}
		
		return Instance().m_platform_type;
	}
	
	#endregion
	
	
	

}
