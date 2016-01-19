using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tencent.bugly.unity3d.android
{
#if UNITY_ANDROID
	public static class Bugly
	{
		/// <summary>
		/// Enables the log.
		/// </summary>
		/// <param name="enable">If set to <c>true</c> enable.</param>
		public static void EnableLog (bool enable)
		{
			BuglyAgent.GetInstance ().EnableLog (enable);
		}
        
		public static void SetDelayReportTime (long delay)
		{
			BuglyAgent.GetInstance ().SetDelayReportTime (delay);
		}
        
		/// <summary>
		/// Sets the channel.
		/// </summary>
		/// <param name="channel">Channel.</param>
		public static void SetChannel (string channel)
		{
			BuglyAgent.GetInstance ().SetChannel (channel);
		}
        
		/// <summary>
		/// Sets the version.
		/// </summary>
		/// <param name="version">Version.</param>
		public static void SetVersion (string version)
		{
			BuglyAgent.GetInstance ().SetVersion (version);
		}
        
		/// <summary>
		/// Sets the auto quit application.
		/// </summary>
		/// <param name="autoExit">If set to <c>true</c> auto exit.</param>
		public static void SetAutoQuitApplication (bool autoExit)
		{
			BuglyAgent.GetInstance ().SetAutoQuitApplication (autoExit);
		}
		
		public static bool IsAutoQuitApplication ()
		{
			return BuglyAgent.GetInstance ().IsAutoQuitApplication ();
		}
        
		/// <summary>
		/// Inits the with app identifier.
		/// </summary>
		/// <param name="appId">App identifier.</param>
		public static void InitWithAppId (string appId)
		{   
			BuglyAgent.GetInstance ().InitWithAppId (appId);
		}
        
		/// <summary>
		/// Sets the user identifier.
		/// </summary>
		/// <param name="userId">User identifier.</param>
		public static void SetUserId (string userId)
		{
			BuglyAgent.GetInstance ().SetUserId (userId);
		}
        
		/// <summary>
		/// Raises the exception caught event.
		/// </summary>
		/// <param name="e">E.</param>
		/// <param name="message">Message attached the exception</param>
		public static void OnExceptionCaught (System.Exception e, string message)
		{
			BuglyAgent.GetInstance ().OnExceptionCaught (e, message);
		}
        
		/// <summary>
		/// Unregisters the exception handler.
		/// </summary>
		public static void UnregisterExceptionHandler ()
		{
			BuglyAgent.GetInstance ().UnregisterExceptionHandler ();
		}
        
		/// <summary>
		/// Registers the exception handler.
		/// </summary>
		/// <param name="level">Level.</param>
		/// <param name="callback">Callback.</param>
		public static void RegisterExceptionHandler (LogSeverity level, LogCallbackDelegate callback)
		{
			BuglyAgent.GetInstance ().RegisterExceptionHandler (level, callback);
		}
        
		public static void PrintLog (LogSeverity level, string msg)
		{
			BuglyAgent.GetInstance ().PrintLog (level, msg);
		}
        
		private sealed class BuglyAgent : ExceptionHandler
		{
			public static readonly BuglyAgent instance = new BuglyAgent ();
            
			public static BuglyAgent GetInstance ()
			{
				return instance;
			}
            
			private AndroidJavaObject _bugly;
			private bool _logEnable;
			private long _configDelay;
			private string _configChannel;
			private string _configVersion;
			private string _configUser;
            
			private BuglyAgent ()
			{
				AndroidJavaClass javaBugly = new AndroidJavaClass ("com.tencent.bugly.unity.UnityAgent");
				_bugly = javaBugly.CallStatic<AndroidJavaObject> ("getInstance");
				_logEnable = false;
			}
            
			public void EnableLog (bool enable)
			{
				_logEnable = enable;
			}
            
			public void SetDelayReportTime (long delay)
			{
				_configDelay = delay;
			}
            
			public void SetChannel (string channel)
			{
				_configChannel = channel;
			}
            
			public void SetVersion (string version)
			{
				_configVersion = version;
			}
            
			public void InitWithAppId (string appId)
			{
				PrintLog(LogSeverity.Warning, "This method was deprecated");
			}
            
			public void SetUserId (string userId)
			{
				_configUser = userId;
				_bugly.Call ("setUserId", userId);
			}
            
			public override void ReportException (string errorClass, string errorMessage, string callStack, bool autoExit)
			{
				_bugly.Call ("traceException", errorClass, errorMessage, callStack, autoExit);
			}
            
			public override void PrintLog (LogSeverity level, string msg)
			{
				if (!_logEnable) {
					if (level < LogSeverity.Info) {
						return;
					}
				}
				_bugly.Call ("printLog", string.Format ("Unity-[{0}]: {1}", level.DisplayName (), msg));
			}
		}
	}
#endif
}