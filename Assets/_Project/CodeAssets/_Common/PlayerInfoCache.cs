using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerInfoCache {

	#region Register Info

	private static DateTime m_register_server_time;

	// 2008-05-01 07:34:42
	public static void SetRegisterTime( string p_server_time ){
		if( string.IsNullOrEmpty( p_server_time ) ){
//			Debug.Log( "Please Check Register Time: " + p_server_time );

//			Debug.Log( "Server Time Null, return local time." );

			return;
		}

		m_register_server_time = DateTime.Parse( p_server_time );
	}

	public static string GetRegisterTimeString( string p_format = TimeHelper.DEFAULT_DATETIME_FORMAT ){
		if( m_register_server_time == null ){
			return "";
		}

		return m_register_server_time.ToString( p_format );
	}

	#endregion
}