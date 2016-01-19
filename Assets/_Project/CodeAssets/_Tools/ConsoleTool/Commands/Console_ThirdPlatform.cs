//#define DEBUG_MSDK

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


using Msdk;
using LitJson;

using SimpleJSON;

using qxmobile;
using qxmobile.protobuf;

public class Console_ThirdPlatform {
	
	#region MSDK
	
	public static void OnInitMSDK( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnInitMSDK() " );

		WGPlatform.Instance.Init ();
		#endif
	}

	public static void OnMSDKLoginQQ( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnMSDKLoginQQ() " );
		
		Debug.Log ("点击登录QQ");
		
		WGPlatform.Instance.WGLoginQQ ();
		#endif
	}

	public static void OnMSDKLoginWX( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnMSDKLoginWX() " );
		
		Debug.Log ("点击登录微信");
		
		WGPlatform.Instance.WGLoginWX ();
		#endif
	}

	public static void OnMSDKLoginGuest( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnMSDKLoginGuest() " );
		
		Debug.Log ("点击游客登录");
		
		WGPlatform.Instance.WGLogin ( Msdk.ePlatform.ePlatform_Guest );
		#endif
	}
	
	public static void OnMSDKAutoLogin( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnMSDKAutoLogin() " );
		
		Debug.Log ("自动登录");		
		
		WGPlatform.Instance.WGLoginWithLocalInfo();
		#endif
	}
	
	public static void OnMSDKLogOut( string[] p_params ){
		#if DEBUG_MSDK
		Debug.Log( "OnMSDKLogOut() " );
		
		Debug.Log ("登出");
		
		WGPlatform.Instance.WGLogout();
		#endif
	}
	
	#endregion


}