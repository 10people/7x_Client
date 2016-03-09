//#define DEBUG_OPERATION_ACTION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OperationSupport {

	#region Report Client Action

	public enum ClientAction{
		LAUNCH_GAME,
		RESOURCES_LOAD_DONE,
		CREATE_ROLE,
		ENTER_GAME
	}

	public static void ReportClientAction( ClientAction p_client_action ){
		#if DEBUG_OPERATION_ACTION
		Debug.Log( "ReportClientAction( " + p_client_action + " )" );
		#endif

		Dictionary<string,string> p_params = new Dictionary<string,string>();

		{
			// add act param
			{
				string t_server_act_value = "";
				
				switch( p_client_action ){
				case ClientAction.LAUNCH_GAME:
					t_server_act_value = "LaunchGame";
					break;
					
				case ClientAction.RESOURCES_LOAD_DONE:
					t_server_act_value = "ResourcesLoadDone";
					break;
					
				case ClientAction.CREATE_ROLE:
					t_server_act_value = "CreateRole";
					break;
					
				case ClientAction.ENTER_GAME:
					t_server_act_value = "EnterGame";
					break;
				}
				
				p_params.Add ( "act" , t_server_act_value );
			}

			// add uuid param
			AppendHttpParamUUID( p_params );
			
			HttpRequest.Instance().Connect ( NetworkHelper.GetPrefix() + NetworkHelper.OPERATION_SUPPORT_REPORT_ACTION_URL, 
			                                 p_params, 
			                                 ReportSuccess, 
			                                 ReportFail );
		}
	}

	private static void ReportSuccess (string p_resp ){
//		Debug.Log( "OperationSupport.ReportClientAction（ " + p_resp + " )" );
	}
	
	private static void ReportFail (string p_resp ){
//		Debug.Log( "OperationSupport.ReportClientAction（ " + p_resp + " )" );
	}

	#endregion



	#region Helpers

	/// Append uuid param to http params.
	/// 
	/// ( key, value ): ( "UUID", SystemInfo.deviceUniqueIdentifier )
	public static void AppendHttpParamUUID( Dictionary<string, string> p_param_dict ){
		if ( p_param_dict == null ) {
			Debug.LogError( "Error, param dict is null." );

			return;
		}

		p_param_dict.Add ( "UUID" , SystemInfo.deviceUniqueIdentifier );
	}

	#endregion

}
