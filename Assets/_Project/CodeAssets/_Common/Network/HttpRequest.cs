//#define DEBUG_HTTP

//#define DEBUG_HTTP_PARAMS


#define CLOSE_CE_SHI_SERVER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HttpRequest : MonoBehaviour{

	#region Consts

	// NeiWang Develop server
	private const string SERVER_NEIWANG_PREFIX		= "http://192.168.3.80:8090/";

	// TiYan Public server
	private const string SERVER_TIYAN_PREFIX		= "http://203.195.230.100:9090/";


	#if CLOSE_CE_SHI_SERVER

	// Now is the same as NeiWang
	private const string SERVER_CESHI_PREFIX		= "http://192.168.3.80:8090/";
	#else
	// Ceshi develop test server
	private const string SERVER_CESHI_PREFIX		= "http://203.195.230.100:9091/";
	#endif


	/// Update Server
	public const string UPDATE_URL_NEIWANG			= "http://192.168.3.80:8070/wsRes/compare.jsp";

	public const string UPDATE_URL_TIYAN			= "http://203.195.230.100:9090/wsRes/compare.jsp";

	public const string UPDATE_URL_CESHI			= "http://203.195.230.100:8010/wsRes/compare.jsp";

		
	// develop register
	public const string REGISTER_URL				= "qxrouter/accountReg.jsp";

	// develop login
	public const string LOGIN_URL					= "qxrouter/accountLogin.jsp";

	// verify 3rd platform login 
	public const string THIRD_PLATFORM_LOGIN_URL	= "qxrouter/channel/checkLogin.jsp";

	// report unsupported device to server
	public const string REPORT_UNSUPPORT_DEVICE_URL			= "qxrouter/client/clientTooLow.jsp";

	/// 公告
	public const string NOTICE_URL 					= "qxrouter/sysNotice.jsp";	



	// report client info to server
	public const string OPERATION_SUPPORT_REPORT_ACTION_URL = "qxrouter/client/reportAction.jsp";


	#endregion



	#region Server Developer

	public const string SERVER_HUGE_PREFIX		= "http://192.168.0.176:8080/";

	#endregion



	#region Get Common Urls

	public static string GetPrefix(){
		string t_prefix = "";
		
		if( ConfigTool.GetServerType() == ConfigTool.ServerType.NeiWang ){
			t_prefix = SERVER_NEIWANG_PREFIX;
		}
		else if( ConfigTool.GetServerType() == ConfigTool.ServerType.TiYan ){
			t_prefix = SERVER_TIYAN_PREFIX;
		}
		else if( ConfigTool.GetServerType() == ConfigTool.ServerType.CeShi ){
			t_prefix = SERVER_CESHI_PREFIX;
		}
		else{
			Debug.LogError( "Error, Not Existed Server Type." );
		}
		
		return t_prefix;
	}

	public static string GetTiYanPrefix(){
		return SERVER_TIYAN_PREFIX;
	}

	public static string GetCeShiPrefix(){
		return SERVER_CESHI_PREFIX;
	}

	public static string GetNeiWangPrefix(){
		return SERVER_NEIWANG_PREFIX;
	}

	private static string m_cur_update_server_url = "";

	public static void SetUpdateUrl( string p_url ){
		m_cur_update_server_url = p_url;
	}

	public static string GetUpdateUrl(){
		return m_cur_update_server_url;
	}

	#endregion



	#region HttpRequest

    public static HttpRequest m_request;

    public delegate void HttpCallBack(string tempString);

    public static HttpRequest Instance(){
		if( m_request == null ){
			GameObject t_gameObject = UtilityTool.GetDontDestroyOnLoadGameObject();

			m_request = t_gameObject.AddComponent<HttpRequest>();
        }

        return m_request;
    }

	public void Connect( string p_url, Dictionary<string, string> p_params, HttpCallBack success_callback, HttpCallBack error_callback = null, List<EventDelegate> p_callback_list = null ){
		{
			bool p_log = false;
			
			#if DEBUG_HTTP
			p_log = true;
			#else
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_HTTP_STATUS ) ){
				p_log = true;
			}
			#endif
			
			if( p_log ){
				Debug.Log( Time.realtimeSinceStartup + "HttpRequest.Connect: " + p_url );
			}
		}

		// show sending
		{
			if( NetworkWaiting.m_instance_exist ){
				NetworkWaiting.Instance().ShowNetworkSending( p_url );
			}
			else{
//				Debug.Log( "NetworkWaiting not showing: " + NetworkWaiting.m_instance_exist );
			}
		}

        if( p_params == null ){
			StartCoroutine( Get( p_url, success_callback, error_callback, p_callback_list ) );
        }
        else{
			StartCoroutine( POST( p_url, p_params, success_callback, error_callback, p_callback_list ) );
        }
    }

	private IEnumerator Get( string p_url, HttpCallBack success_callback, HttpCallBack error_callback = null, List<EventDelegate> p_callback_list = null ){
		if( ConfigTool.IsEmulatingNetworkLatency() ){
			yield return new WaitForSeconds( ConfigTool.GetEmulatingNetworkLatency() );
		}

        WWW t_www = new WWW( p_url );

        yield return t_www;

		{
			bool p_log = false;
			
			#if DEBUG_HTTP
			p_log = true;
			#else
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_HTTP_STATUS ) ){
				p_log = true;
			}
			#endif
			
			if( p_log ){
				if( !string.IsNullOrEmpty( t_www.error ) ){
					Debug.Log( Time.realtimeSinceStartup + "WWW.error: " + t_www.error + " : " + p_url );
				}
				else{
					Debug.Log( Time.realtimeSinceStartup + "Get.Response: " + t_www.text );
				}
			}
		}

		// hide sending
		{
			if( NetworkWaiting.m_instance_exist ){
				NetworkWaiting.Instance().HideNeworkWaiting();
			}
		}

        if( !string.IsNullOrEmpty( t_www.error ) ){
			Debug.LogError( Time.realtimeSinceStartup + "WWW.error: " + t_www.error + " : " + p_url );

			if( error_callback != null ){
				error_callback( t_www.error + " : " + p_url  );
			}
        }
        else{
			if( success_callback != null ){
				success_callback( t_www.text );
            }
        }

		{
			EventDelegate.Execute( p_callback_list );
		}
    }

	private IEnumerator POST(string p_url, Dictionary<string, string> p_dic, HttpCallBack success_callback, HttpCallBack error_callback = null, List<EventDelegate> p_callback_list = null ){
		if( ConfigTool.IsEmulatingNetworkLatency() ){
			yield return new WaitForSeconds( ConfigTool.GetEmulatingNetworkLatency() );
		}

        WWWForm t_form = new WWWForm();

        if( p_dic != null ){
            foreach( KeyValuePair<string, string> p_pair in p_dic ){
				#if DEBUG_HTTP_PARAMS
				Debug.Log(  p_pair.Key + ": " + p_pair.Value );
				#endif

                t_form.AddField(p_pair.Key, p_pair.Value);
            }
        }
        WWW t_www = new WWW(p_url,t_form);

        yield return t_www;

		{
			bool p_log = false;
			
			#if DEBUG_HTTP
			p_log = true;
			#else
			if( ConfigTool.GetBool( ConfigTool.CONST_LOG_HTTP_STATUS ) ){
				p_log = true;
			}
			#endif
			
			if( p_log ){
				Debug.Log( Time.realtimeSinceStartup + "POST.Response: " + t_www.text );
				
				if( !string.IsNullOrEmpty( t_www.error ) ){
					Debug.Log( Time.realtimeSinceStartup + "WWW.error: " + t_www.error + " : " + p_url );
				}
			}
		}

		// hide sending
		{
			if( NetworkWaiting.m_instance_exist ){
				NetworkWaiting.Instance().HideNeworkWaiting();
			}
		}

        if( !string.IsNullOrEmpty( t_www.error ) ){
			Debug.LogError( Time.realtimeSinceStartup + "WWW.Error: " + t_www.error + " : " + p_url );

			if( error_callback != null ){
				error_callback( t_www.error + " : " + p_url  );
			}
        }
        else{
			if( success_callback != null ){
				success_callback( t_www.text );
            }
        }

		{
			EventDelegate.Execute( p_callback_list );
		}
    }

	#endregion



	#region Utilities



	#endregion
}
