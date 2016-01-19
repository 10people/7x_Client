using UnityEngine;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace com.tencent.bugly.unity3d
{
	// We dont use the LogType enum in Unity as the numerical order doesnt suit our purposes
	/// <summary>
	/// Log severity. 
	/// { Log, Info, Warning, Error, Assert, Exception }
	/// </summary>
	public enum LogSeverity
	{
		Log,
		Info,
		Warning,
		Error,
		Assert,
		Exception
	}
    
	public static class LogSeverityExtension
	{
		public static string DisplayName (this LogSeverity severity)
		{
			if (LogSeverity.Log == severity) {
				return "Debug";
			} else if (LogSeverity.Info == severity) {
				return "Info";
			} else if (LogSeverity.Warning == severity) {
				return "Warn";
			} else if (LogSeverity.Error == severity) {
				return "Error";
			} else if (LogSeverity.Assert == severity) {
				return "Assert";
			} else if (LogSeverity.Exception == severity) {
				return "Error";
			} else {
				return "UNKNOWN";
			}
		}
	}
    
	/// <summary>
	/// Log callback delegate.
	/// </summary>
	public delegate void LogCallbackDelegate (string log, string stackTrace, LogType type, bool report);
    
	/// <summary>
	/// Exception handler.
	/// </summary>
	public abstract class ExceptionHandler
	{

		/// <summary>
		/// Reports the exception.
		/// </summary>
		/// <param name="errorClass">Error class.</param>
		/// <param name="errorMessage">Error message.</param>
		/// <param name="callStack">Call stack.</param>
		/// <param name="autoExit">If set to <c>true</c> auto exit.</param>
		public abstract void ReportException (string errorClass, string errorMessage, string callStack, bool autoExit);
		
		/// <summary>
		/// Prints the log.
		/// </summary>
		/// <param name="msg">Message.</param>
		public abstract void PrintLog (LogSeverity level, string msg);
        
		private bool _logCallbackRegister = false;
		
		private LogSeverity _logLevel = LogSeverity.Exception;
		private event LogCallbackDelegate _logCallbackDelegate;

		private bool _autoExit = false;
        private bool _reportOnce = false;
        
		/// <summary>
		/// Raises the exception caught event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnExceptionCaught (System.Exception e, string message)
		{
			OnExceptionHandle (e, false, message);  
		}
        
		public void UnregisterExceptionHandler ()
		{
			_logCallbackRegister = false;
			_logCallbackDelegate = null;
            
			Application.RegisterLogCallback (null);
		}
        
		public void SetAutoQuitApplication (bool autoExit)
		{
			_autoExit = autoExit;
		}
		
		public bool IsAutoQuitApplication ()
		{
			return _autoExit;
		}
		
		public void RegisterExceptionHandler (LogSeverity level, LogCallbackDelegate callback)
		{
			_logLevel = level;
            
			if (callback != null) {
				_logCallbackDelegate += callback;
			}
            
			if (!_logCallbackRegister) {
				_logCallbackRegister = true;
                
				System.AppDomain.CurrentDomain.UnhandledException += OnUncaughtExceptionHandler;
				Application.RegisterLogCallback (OnLogCallback);
                
				PrintLog (LogSeverity.Log, "Register log callback");
			}
		}
        
		private void OnUncaughtExceptionHandler (object sender, System.UnhandledExceptionEventArgs args)
		{
			if (args == null || args.ExceptionObject == null) {
				return;
			}
            
			if (args.ExceptionObject.GetType () != typeof(System.Exception)) {
				return;
			}

			OnExceptionHandle ((System.Exception)args.ExceptionObject, true, null);
		}
        
		private void OnLogCallback (string log, string stackTrace, LogType type)
		{             
			LogSeverity severity = LogSeverity.Exception;
            
			switch (type) {
			case LogType.Assert:
				severity = LogSeverity.Assert;
				break;
			case LogType.Error:
				severity = LogSeverity.Error;
				break;
			case LogType.Exception:
				severity = LogSeverity.Exception;
				break;
			case LogType.Log:
				severity = LogSeverity.Log;
				break;
			case LogType.Warning:
				severity = LogSeverity.Warning;
				break;
			default:
				break;
			}

            bool report = false;
            
            if (severity >= _logLevel && !_reportOnce) {                
				string errorClass, errorMessage = null;
                
				// 提取错误类型和信息
				//Regex errorInfo = new Regex (@"^(?<errorClass>\S+):\s*(?<message>.*)");
                Match match = new Regex (@"^(?<errorClass>\S+):\s*(?<message>.*)").Match (log);
                
				if (match.Success) {
					errorClass = match.Groups ["errorClass"].Value;
					errorMessage = match.Groups ["message"].Value.Trim ();
				} else {
					errorClass = log;
				}
                
				if (stackTrace != null) {
					try {
						string[] stacks = stackTrace.Split ('\n');
						if (stacks != null && stacks.Length > 0) {
                            
							StringBuilder sb = new StringBuilder ("");
							foreach (string s in stacks) {
								if (s.Trim().Length == 0 || s.StartsWith("System.Collections.Generic.")) {
									continue;
								}
									
								int start = s.ToLower ().IndexOf ("(at");
								int end = s.ToLower ().IndexOf ("/assets/");
                                
								if (start > 0 && end > 0) {
									sb.AppendFormat ("{0}(at {1}\n", s.Substring (0, start), s.Substring (end));
								} else {
									if (s.Trim().Length > 0) {
										sb.AppendFormat("{0}\n", s);
									}
								}
							}
							if (sb.Length > 0) {
								stackTrace = sb.ToString ();
							}
						}
					} catch (System.Exception e) {
						PrintLog (LogSeverity.Warning, string.Format("Occur exception: {0}", e.ToString ()));
					}
				}
                
				// 上报c#的异常堆栈
				ReportException (errorClass, errorMessage, stackTrace, IsAutoQuitApplication ());
                
				report = true;
                
                _reportOnce = IsAutoQuitApplication ();
			}

			if (_logCallbackDelegate != null) {
				_logCallbackDelegate (log, stackTrace, type, report);
			}
		}
        
		private void OnExceptionHandle (System.Exception e, bool needQuit, string message)
		{
			if (e == null)
				return;
            
			StackTrace stackTrace = new StackTrace (e, true);
            
			int count = stackTrace.FrameCount;
            
			StringBuilder callStack = new StringBuilder ("");
			StackFrame frame = null;
			string fileName = null;
			for (int i = 0; i < count; i++) {
				frame = stackTrace.GetFrame (i);
				callStack.AppendFormat ("{0}.{1}", frame.GetMethod ().DeclaringType.Name, frame.GetMethod ().Name);
                
				ParameterInfo[] param = frame.GetMethod ().GetParameters ();
				if (param == null || param.Length == 0) {
					callStack.Append ("()");
				} else {
					callStack.Append ("(");
                    
					int pcount = param.Length;
                    
					ParameterInfo pinfo = null;
					for (int p = 0; p < pcount; p++) {
						pinfo = param [p];
						callStack.AppendFormat ("{0} {1}", pinfo.ParameterType.FullName, pinfo.Name);
                        
						if (p != pcount - 1) {
							callStack.Append (", ");
						}
					}
					pinfo = null;
                    
					callStack.Append (")");
				}
                
				fileName = frame.GetFileName ();
               
				if (fileName != null) {
					fileName = fileName.Replace ('\\', '/');
                    
					int loc = fileName.ToLower ().IndexOf ("/assets/");
					if (loc < 0) {
						loc = fileName.ToLower ().IndexOf ("assets/");
					}
                    
					if (loc > 0) {
						fileName = fileName.Substring (loc);
					}
				} else {
					fileName = "unknown";
				}
				callStack.AppendFormat (" (at {0}:{1})", fileName, frame.GetFileLineNumber ());
                
				if (i != count - 1) {
					callStack.AppendLine ();
				}
				fileName = null;
			}
            
			frame = null;
            
            bool report = false;
            
            if (!needQuit || !_reportOnce) {
            	ReportException (e.GetType ().Name, (message != null ? string.Format("{0}\nMessage:{1}", e.Message, message) : e.Message), callStack.ToString (), needQuit && IsAutoQuitApplication ());
            	
                _reportOnce = needQuit && IsAutoQuitApplication ();
            }
            
            if (_logCallbackDelegate != null) {
                _logCallbackDelegate (e.GetType ().Name, callStack.ToString (), needQuit ? LogType.Exception : LogType.Warning, report);
			}
		}
        
	}
}