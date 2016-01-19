#if UNITY_EDITOR_WIN ||UNITY_ANDROID

using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace com.tencent.bugly.unity3d
{   
	/// <summary>
	/// Interface of Bugly SDK.
	/// </summary>
	public class Bugly
	{

		/// <summary>
		/// Plugins the version.
		/// </summary>
		/// <returns>The version.</returns>
		public static string PluginVersion () {
			return "1.2.0";
		}
        
		/// <summary>
		/// Enables the log.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public static void EnableLog (bool enable)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.EnableLog(enable);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                PrintWarning("This method was deprecated");
				#endif
			}
		}
        
		/// <summary>
		/// Inits the SDK with the app ID.
		/// </summary>
		/// <param name="appId">App identifier.</param>
		public static void InitSDK (string appId)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.InstallWithAppId(appId);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                PrintWarning("This method was deprecated");
				#endif
			}
		}
        
		/// <summary>
		/// Sets the auto quit application.
		/// </summary>
		/// <param name="autoExit">If set to <c>true</c> auto exit.</param>
		public static void SetAutoQuitApplication (bool autoExit)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
				ios.Bugly.SetAutoQuitApplication(autoExit);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
				android.Bugly.SetAutoQuitApplication(autoExit);
				#endif
			}
		}
        
        /// <summary>
        /// Determines if is auto quit application.
        /// </summary>
        /// <returns><c>true</c> if is auto quit application; otherwise, <c>false</c>.</returns>
		public static bool IsAutoQuitApplication ()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
				return ios.Bugly.IsAutoQuitApplication();
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
				return android.Bugly.IsAutoQuitApplication();
				#endif
			}
			return false;
		}
        
		/// <summary>
		/// Registers the c# exception handler by set Application.RegisterLogCallback
		/// </summary>
		/// <param name="level">Level.</param>
		/// <param name="callback">Callback.</param>
		public static void RegisterExceptionHandler (LogSeverity level, LogCallbackDelegate callback)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.RegisterExceptionHandler(level, callback);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                android.Bugly.RegisterExceptionHandler(level, callback);
				#endif
			}
		}

		/// <summary>
		/// Handles the exception.
		/// </summary>
		/// <param name="e">E.</param>
		public static void ReportException (System.Exception e) {
			ReportException(e, null);
		}
        
		/// <summary>
		/// Handles the exception.
		/// </summary>
		/// <param name="e">E.</param>
		public static void ReportException (System.Exception e, string message)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.OnExceptionCaught(e, message);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                android.Bugly.OnExceptionCaught(e, message);
				#endif
			}
		}
        
		/// <summary>
		/// Sets the user identifier.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		public static void SetUserId (string userId)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.SetUserId(userId);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                android.Bugly.SetUserId (userId);   
				#endif
			}
		}
        
		/// <summary>
		/// Sets the app version.
		/// </summary>
		/// <param name="version">Version.</param>
		public static void SetAppVersion (string version)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.SetBundleVersion(version);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                PrintWarning("This method was deprecated");
				#endif
			}
		}
		/// <summary>
		/// Sets the channel.
		/// </summary>
		/// <param name="channel">Channel.</param>
		public static void SetChannel (string channel)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.SetChannel(channel);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                PrintWarning("This method was deprecated");
				#endif
			}
		}
        
		public static void PrintLog (string format, params object[] args)
		{
			PrintLog (LogSeverity.Log, string.Format (format, args));
		}
        
		public static void PrintInfo (string format, params object[] args)
		{
			PrintLog (LogSeverity.Info, string.Format (format, args));
		}
        
		public static void PrintWarning (string format, params object[] args)
		{
			PrintLog (LogSeverity.Warning, string.Format (format, args));
		}
        
		public static void PrintError (string format, params object[] args)
		{
			PrintLog (LogSeverity.Error, string.Format (format, args));
		}
        
		private static void PrintLog (LogSeverity level, string msg)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				#if UNITY_IPHONE
                ios.Bugly.PrintLog(level, msg);
				#endif
			} else if (Application.platform == RuntimePlatform.Android) {
				#if UNITY_ANDROID
                android.Bugly.PrintLog (level, msg);
				#endif
			}
		}

        #if UNITY_ANDROID
        public static void SetReportDelayTime(string delay){
            if (Application.platform == RuntimePlatform.Android) {
                long delayTime = 0;
                try {
                    if (delay != null) {
                        delay = delay.Trim();
                        if (delay.Length > 0) {
                            delayTime = Convert.ToInt64 (delay);
                        }
                    }
                } catch(Exception e) {
                    PrintLog(string.Format("Fail to set report delay time cause by {0}", e.ToString()));
                    delayTime = 0;
                }
                android.Bugly.SetDelayReportTime (delayTime);
            }
        }
        #endif
        
        #if UNITY_IPHONE
        public static void SetUserData(string key, string value){
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ios.Bugly.SetUserData(key, value);
            }
        }
        
        public static void SetDeviceId(string deviceId){
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ios.Bugly.SetDeviceId(deviceId);
            }
        }

		public static void SetBundleId(string bundleId){
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				ios.Bugly.SetBundleId(bundleId);
			}
		}
        
        public static void EnableCrashAndSymbolicateInProcess(bool merged, bool symbolicate){
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                ios.Bugly.EnableCrashMergeUploadAndSymbolicateInProcess(merged, symbolicate);
            }
        }
        #endif
	}
   
}

#endif