/// <summary>
/// Change With ThirdPlatform
/// </summary>



//#define XG_PUSH



//#define MYAPP_ANDROID_PLATFORM



//#define PP_PLATFORM

//#define XY_PLATFORM

//#define TONGBU_PLATFORM

//#define I4_PLATFORM

//#define KUAIYONG_PLATFORM

//#define HAIMA_PLATFORM

//#define I_APPLE_PLATFORM

//#define ITOOLS_PLATFORM



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



/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.8.1
 * @since:		Unity 5.1
 * Function:	Cross Platform Use.
 * 
 * Notes:
 * None.
 */ 
public class Bonjour{

	#region Default Calls

	#if !XG_PUSH
	public static void showSDKCenter(){

	}

	public static void logout(){

	}

	#endif

	#endregion



	#region XG

	#if UNITY_IOS && XG_PUSH
	[DllImport("__Internal")]
	private static extern void U3D_XG_ClearAllPush();
	#endif

	public static void ClearAllPush(){
//		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			#if UNITY_IOS && XG_PUSH
			U3D_XG_ClearAllPush();
			#endif
//		}
	}

	#if UNITY_IOS && XG_PUSH
	[DllImport("__Internal")]
	private static extern void U3D_XG_LocalPush( string p_push_key, 
	                                            int p_sec_since_now,
	                                             string p_push_content );
	#endif

	#if UNITY_ANDROID && XG_PUSH
	private static void U3D_Android_XG_LocalPush( string p_push_key,
	                                             int p_sec_since_now,
	                                             string p_push_content ){
		{
			Debug.Log( "U3D_ANDROID_XG_LocalPush()" );
			
			Debug.Log( "p_push_key: " + p_push_key );
			
			Debug.Log( "p_sec_since_now: " + p_sec_since_now );
			
			Debug.Log( "p_push_content: " + p_push_content );
		}

		using ( AndroidJavaClass t_cls = new AndroidJavaClass( "com.tencent.tmgp.qixiongwushuang.UnityPlayerActivity" ) ) { 
			if( t_cls == null ){
				Debug.LogError( "Error, AndroidJavaClass is null." );

				return;
			}

			t_cls.CallStatic( "OnLocalPush", p_push_key, p_sec_since_now, p_push_content );
		} 
	}
	#endif

	public static void LocalPush( string p_push_key,
	                             int p_sec_since_now,
	                             string p_push_content ){
//		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			#if UNITY_IOS && XG_PUSH
			U3D_XG_LocalPush( p_push_key, p_sec_since_now, p_push_content );
			#endif
//		}

		#if UNITY_ANDROID && XG_PUSH
		U3D_Android_XG_LocalPush( p_push_key, p_sec_since_now, p_push_content );
		#endif
	}

	#endregion



	#region Utilities

	public static Dictionary<string,string> SplitStringToDic(string result)
	{
		Dictionary<string,string> returnDic = new Dictionary<string, string>();
		if(result!=""){
			string[] pairs = result.Split(new string[] {"#&#and#&#"},System.StringSplitOptions.None);
			
			for (int i = 0; i < pairs.Length; i++)
			{
				string thisPair = pairs[i];
				string[] onePair = thisPair.Split(new string[] {"#=#equal#=#"},System.StringSplitOptions.None);
				if(onePair.Length==2)
				{
					returnDic.Add(onePair[0],onePair[1]);
				}
			}
		}
		return returnDic;
	}

	#endregion



	#if PP_PLATFORM

	#region PP

	/**
     *  Initialize ppsdk
     * 
     * 	@param paramAppId  appid
     * 	@param paramAppKey appKey
     */
	[DllImport("__Internal")]
     private static extern void U3D_initSDK (int paramAppId,
											string paramAppKey,
											string paramSendMsgNotiClass);
	
     public static void initSDK (int paramAppId,
								string paramAppKey,
								string paramSendMsgNotiClass){
		if (Application.platform == RuntimePlatform.IPhonePlayer ){
           U3D_initSDK(paramAppId,
						paramAppKey,
						paramSendMsgNotiClass);

		}
     }
	
	/**
     *  start ppsdk
     */
	[DllImport("__Internal")]
	private static extern void U3D_startPPSDK();
	public static void startPPSDK()
	{	
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
           U3D_startPPSDK();
		}
	}

	/**
     *  login ppsdk
     */
	[DllImport("__Internal")]
	private static extern void U3D_login();
	public static void login()
	{
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			U3D_login();
		}
	}
	
	/**
     *  setRechargeAmount (option default 10)
     */
	[DllImport("__Internal")]
     private static extern void U3D_setRechargeAmount(int rechargeAmount);
     public static void setRechargeAmount(int rechargeAmount)
     {
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
           U3D_setRechargeAmount(rechargeAmount);
		}
     }
	
	/**
     *  setBuoyPosition (option default x = 30,y = 30)
     */
	[DllImport("__Internal")]
     private static extern void U3D_setBuoyPosition(int x, int y);
     public static void setBuoyPosition(int x,int y)
     {
		if ( Application.platform == RuntimePlatform.IPhonePlayer  ){
           U3D_setBuoyPosition(x,y);
		}
     }
	
	/**
     *  setIsOpenNSlogData (option default NO)
     */
	[DllImport("__Internal")]
     private static extern void U3D_setIsOpenNSlogData(bool isOpenNSlogData);
     public static void setIsOpenNSlogData(bool isOpenNSlogData)
     {
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
           U3D_setIsOpenNSlogData(isOpenNSlogData);
		}
     }
	
	/**
     *  showSDKCenter (option)
     */
	[DllImport("__Internal")]
     private static extern void U3D_showSDKCenter();
     public static void showSDKCenter()
     {
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
           U3D_showSDKCenter();
		}
     }
	
	/**
     *  logout
     */
	[DllImport("__Internal")]
     private static extern void  U3D_logout();
     public static void logout()
     {
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
		  U3D_logout();
		}
     }
	
	
	/**
     *  get loginState 1:login 0:logoff
     */
	[DllImport("__Internal")]
     private static extern int  U3D_loginState();
     public static int loginState()
     {
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
		  return U3D_loginState();
		}
		
		return 0;
     }



	/**
	 * 兑换道具 
	 * @param price，商品的价格
	 * @param BillNo，商品的订单号。从游戏服务端自行获取，char（20），保证唯一
	 * @param BillTitle，商品的名称
	 * @param RoleId，角色ID，游戏
	 * @param ZoneId，分区ID，此ID要和开发者中心后台的分区ID对应，否则充值出现提示分区参数错误。
	 * 
	 */	
	[DllImport("__Internal")]
     private static extern void U3D_exchangeGoods(int paramPrice,
													string paramBillNo,
													string paramBillTitle,
                                          			string paramRoleId,
													int paramZoneId);
     public static void exchangeGoods(int paramPrice,
										string paramBillNo,
										string paramBillTitle,
                                        string paramRoleId,
										int paramZoneId)
     {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
			U3D_exchangeGoods(paramPrice,paramBillNo,paramBillTitle,paramRoleId,paramZoneId);
		}
     }
	
	/**
     *  tokenVerifyCallBack (required)
     */
	[DllImport("__Internal")]
     private static extern void U3D_tokenVerifyCallBack (bool paramIsSuccess);
	
     public static void tokenVerifyCallBack (bool paramIsSuccess)
     {
		if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
           U3D_tokenVerifyCallBack(paramIsSuccess);
		}
     }

	#endregion

	#endif




	#if XY_PLATFORM
	
	#region XY

	
	[DllImport("__Internal")]
	private static extern void XY_showSDKCenter();
	public static void showSDKCenter()
	{
		Debug.Log ( "ShowSDKCenter()" );

		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			XY_showSDKCenter();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void  XY_logout();
	public static void logout()
	{
		Debug.Log ( "logout()" );

		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			XY_logout();
		}
	}

	#endregion
	
	#endif



	#if TONGBU_PLATFORM
	
	#region TongBu
	
	
	[DllImport("__Internal")]
	private static extern void TongBu_showSDKCenter();
	public static void showSDKCenter()
	{
		Debug.Log ( "ShowSDKCenter()" );
		
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			TongBu_showSDKCenter();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void  TongBu_logout();
	public static void logout()
	{
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			TongBu_logout();
		}
	}

	[DllImport("__Internal")]
	private static extern void TongBu_showSDKToolBar();
	public static void showSDKToolBar()
	{
		Debug.Log ( "showSDKToolBar()" );
		
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			TongBu_showSDKToolBar();
		}
	}
	
	#endregion
	
	#endif



	#if I4_PLATFORM
	
	#region I4
	
	[DllImport("__Internal")]  
	private static extern void AsInit();
	//init
	public static void pxAsInit()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			Debug.Log( "Bonjour.pxAsInit()" );
			AsInit();
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsLogin();
	//Login
	public static void pxAsLogin ()  
	{  
		
		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			AsLogin(); 
		}  
	}  
	
	[DllImport("__Internal")]  
	private static extern void AsCenter();  
	//AsCenter
	public static void showSDKCenter()  
	{  
		Debug.Log ( "I4.showSDKCenter()" );

		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
//			AsCenter(); 

			AsLogin();
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsLogout();  
	//Logout
	public static void logout()  
	{  
		Debug.Log ( "I4.logout()" );

		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			AsLogout(); 
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsUserID();
	//IsLogined
	public static void pxAsUserID()  
	{  
		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			AsUserID();
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsUserName();
	//UserName
	public static void pxAsUserName()  
	{  
		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			AsUserName();
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsIsLog(); 
	//SetUpdateDebug
	public static void pxAsIsLog()  
	{  
		if (Application.platform == RuntimePlatform.IPhonePlayer)   
		{  
			AsIsLog();  
		}  
	}
	
	[DllImport("__Internal")]  
	private static extern void AsPayRMB(int amount, string paramBillNo,string paramBillTitle,string paramRoleId,int zoneID); 
	//Pay RMB
	public static void pxAsPayRMB(int amount, string paramBillNo,string paramBillTitle,string paramRoleId,int zoneID)
	{  
		if ( Application.platform == RuntimePlatform.IPhonePlayer )   
		{  
			AsPayRMB(amount,paramBillNo,paramBillTitle,paramRoleId,zoneID); 
		}  
	}
	
	#endregion
	
	#endif



	#if KUAIYONG_PLATFORM
	
	#region KuaiYong
	
	
	[DllImport("__Internal")]
	private static extern void KuaiYong_showSDKCenter();
	public static void showSDKCenter(){
		Debug.Log ( "ShowSDKCenter()" );
		
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			KuaiYong_showSDKCenter();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void  KuaiYong_logout();
	public static void logout(){
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			KuaiYong_logout();
		}
	}
	
	#endregion
	
	#endif



	#if HAIMA_PLATFORM

	#region Custom

	public static void Public_XHPaySetUnityReceiver( string gbName ){
		XHPaySetUnityReceiver (gbName);
	}

	public static void showSDKCenter(){
		Debug.Log ( "ShowSDKCenter()" );

		if( Application.platform == RuntimePlatform.IPhonePlayer ){
//			Debug.Log( "Show User Center or Start Login?" );

			XHPayStartLogin();

//			XHPayShowUserCenter();
		}
	}

	public static void logout(){
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			XHPayStartLogout();
		}
	}

	#endregion

	/*
	 *  所有TestObj.SendMessage的方法都可以删除
	 */
	public GameObject TestObj;
	
	
	private static string appId = "5dc8f55d7b7ea1d55510670db19b9abe";   // 设置您的APPID
	private static bool testUpdate = false;		 // 测试更新模式。为true时，则不论版本号，肯定提示更新
	private static int updateAlertType = 0;        // 当检查更新失败时，控制是否允许跳过强制更新； 
	//  0：不提示检查失败（直接跳过并进入游戏） 
	//  1：不允许跳过（alert只有一个"重新检查"按钮） 
	//  2：允许选择跳过更新（alert有两个按钮，一个“否”，一个“重新检查”）
	private static bool logOut = false;		 		 // 设置控制台打印日志：true为开启，fasle为关闭
	
	
	void Start() 
	{
//		Debug.Log( this.transform.name );
//
//		XHPaySetUnityReceiver( this.transform.name );	
	}
	

	
	#region ZHPaySDK - API部分 -
	
	public enum ZHPayOrientation
	{
		ZHPayOrientationPortrait = 1,    			  //竖向
		ZHPayOrientationLandscapeLeft,           //横向向左（Home键在左侧）
		ZHPayOrientationLandscapeRight,	     //横向向右（Home键在右侧）
		ZHPayOrientationPortraitUpsideDown,  //竖向倒置
		ZHPayOrientationLandscape,                //横向
		ZHPayOrientationAll,  			     			 //所有方向
		ZHPayOrientationAllButUpsideDown,    //除竖向倒置外的所有方向
	}	//程序支持的方向//
	
	
	public static void ZHPayInit()
	{
		//API：SDK初始化
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayInit(appId,testUpdate,updateAlertType);
			XHPaySetLogEnable(logOut);
		}
	}
	
	public static void ZHPaySetSupportOrientation(ZHPayOrientation orientation)
	{
		//TODO: API：设置程序支持方向
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPaySetSupportOrientation(orientation);
		}
	}
	
	public static void ZHPayStartLogin()
	{
		// API：开始登陆
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayStartLogin();
		}
	}
	
	public static void ZHPayStartLogout()
	{
		// API：注销
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayStartLogout();
		}
	}
	
	public static void ZHPaySwitchAccount(bool shouldLogout)
	{
		//TODO: API：切换账号, shouldLogout：是否注销当前用户
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPaySwitchAccount(shouldLogout);
		}
	}
	
	public static void ZHPayShowUserCenter()
	{
		// API：展示用户中心
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayShowUserCenter();
		}
	}
	
	public static bool ZHPayIsLogined()
	{
		//TODO: API：是否已登录
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayIsLogined();
		}
		return false;
	}
	
	public static string ZHPayGetAppId()
	{
		//API：获取初始化时的AppId
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayGetAppId();
		}
		return "";
	}
	
	public static string ZHPayGetSDKVersion()
	{
		//API：获取SDK版本号
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayGetSDKVersion();
		}
		return "";
	}
	
	public static string ZHPayGetUserName()
	{
		//API：获取已登录用户名
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayGetUserName();
		}
		return "";
	}
	
	public static string ZHPayGetUserId()
	{
		//API：获取已登录用户Id
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayGetUserId();
		}
		return "";
	}
	
	public static void ZHPayCheckUpdate()
	{
		//API：检查更新
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayCheckUpdate();
		}
	}
	
	public static void ZHPayCheckLoginStatus()
	{
		//API：用户异地登陆检查，建议在游戏重要节点检查
		//若检测到用户异地登陆，SDK会回调ZHPayDidLogout()方法，此时您需要将游戏退出；没有异地登陆时不会有任何回调。
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayCheckLoginStatus();
		}
	}
	
	public static bool ZHPayStartOrder(string orderId,string productName,string gameName,double productPrice,string userParam)
	{
		//API：订单支付（订单号，商品名，游戏名，价格，用户自定义参数） 前四个参数不可为空，价格单位为元，最多两位小数
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			return XHPayStartOrder(orderId, productName, gameName, productPrice,userParam);
		}
		return false;
	}
	
	public static void ZHPayCheckOrder(string orderId)
	{
		//API：订单状态查询（订单号）
		if (Application.platform != RuntimePlatform.OSXEditor)   
		{  
			XHPayCheckOrder(orderId);
		}
	}
	
	
	#endregion
	

	
	#region 用C对SDK-API进行封装，不用调用
	
	[DllImport("__Internal")]
	private static extern void XHPaySetUnityReceiver(string gbName);
	
	[DllImport("__Internal")]
	private static extern void XHPayInit(string appId,bool testUpdate,int alertType);
	
	[DllImport("__Internal")]
	private static extern void XHPaySetLogEnable(bool logEnable);
	
	[DllImport("__Internal")]
	private static extern void XHPaySetSupportOrientation(ZHPayOrientation orientation);
	
	[DllImport("__Internal")]
	private static extern void XHPayStartLogin();
	
	[DllImport("__Internal")]
	private static extern void XHPayStartLogout();
	
	[DllImport("__Internal")]
	private static extern void XHPaySwitchAccount(bool shouldLogout);
	
	[DllImport("__Internal")]
	private static extern void XHPayShowUserCenter();
	
	[DllImport("__Internal")]
	private static extern void XHPayCheckUpdate();
	
	[DllImport("__Internal")]
	private static extern void XHPayCheckLoginStatus();
	
	[DllImport("__Internal")]
	private static extern bool XHPayIsLogined();
	
	[DllImport("__Internal")]
	private static extern string XHPayGetAppId();
	
	[DllImport("__Internal")]
	private static extern string XHPayGetSDKVersion();
	
	[DllImport("__Internal")]
	private static extern string XHPayGetUserName();
	
	[DllImport("__Internal")]
	private static extern string XHPayGetUserId();
	
	[DllImport("__Internal")]
	private static extern bool XHPayStartOrder(string orderId,string productName,string gameName,double productPrice,string userParam);
	
	[DllImport("__Internal")]
	private static extern void XHPayCheckOrder(string orderId);
	
	#endregion
	#endif



	#if I_APPLE_PLATFORM
	
	#region I Apple
	
	
	[DllImport("__Internal")]
	private static extern void I_Apple_showSDKCenter();
	public static void showSDKCenter(){
		Debug.Log ( "ShowSDKCenter()" );
		
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			I_Apple_showSDKCenter();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void  I_Apple_logout();
	public static void logout(){
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			I_Apple_logout();
		}
	}
	
	#endregion
	
	#endif



	#if ITOOLS_PLATFORM
	
	#region ITools
	
	
	[DllImport("__Internal")]
	private static extern void ITools_showSDKCenter();
	public static void showSDKCenter(){
		Debug.Log ( "ShowSDKCenter()" );
		
		if( Application.platform == RuntimePlatform.IPhonePlayer ){
			ITools_showSDKCenter();
		}
	}
	
	[DllImport("__Internal")]
	private static extern void  ITools_logout();
	public static void logout(){
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.IPhonePlayer ){
			ITools_logout();
		}
	}
	
	#endregion
	
	#endif



	#if MYAPP_ANDROID_PLATFORM
	
	#region My App

	public static void showSDKCenter(){
		Debug.Log ( "MyApp.ShowSDKCenter(), do Nothing." );
		
//		if( Application.platform == RuntimePlatform.Android ){
//			WGPlatform.Instance.WGLogin( ePlatform.ePlatform_None );
//		}
	}

	public static void logout(){
		Debug.Log ( "logout()" );
		
		if ( Application.platform == RuntimePlatform.Android ){
			WGPlatform.Instance.WGLogout();
		}
	}
	
	#endregion
	
	#endif

}
