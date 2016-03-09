//#define DEBUG_HTTP

//#define DEBUG_HTTP_PARAMS



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class HttpRequest : MonoBehaviour{

	#region HttpRequest

    public static HttpRequest m_request;

    public delegate void HttpCallBack(string tempString);

    public static HttpRequest Instance(){
		if( m_request == null ){
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();

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
				Debug.Log( Time.realtimeSinceStartup + " HttpRequest.Connect: " + p_url );
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
					Debug.Log( Time.realtimeSinceStartup + " WWW.error: " + t_www.error + " : " + p_url );
				}
				else{
					Debug.Log( Time.realtimeSinceStartup + " Get.Response: " + t_www.text );
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

			{
				OnHttpError();
			}

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
				Debug.Log( Time.realtimeSinceStartup + " POST.Response: " + t_www.text );
				
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
			Debug.LogError( "WWW.Error: " + t_www.error + " : " + p_url );

			{
				OnHttpError();
			}

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

	private static void OnHttpError(){
//		#if DEBUG_HTTP
//		Debug.Log( "OnHttpError( Restore Default Server )" );

		PlayerPrefs.DeleteKey( "选服" );

		PlayerPrefs.Save();
//		#endif
	}

	#endregion
}
