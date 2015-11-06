//#define DEBUG_RED_SPOT

//#define DEBUG_RED_SPOT_FETCH

//#define DEBUG_RED_SPOT_TIME_COUNT

//#define DEBUG_LOCAL_PUSH

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

using qxmobile.protobuf;

/** 
 * @author:		Zhang YuGu
 * @Date: 		2015.9.16
 * @since:		Unity 5.1.2
 * Function:	Global access to manage ios push and android notifications, also help manage game's red spot notification.
 * 
 * Notes:
 * 1.red spot notification data is stored in FunctionOpenTemp temporarily.
 */ 
public class PushAndNotificationHelper : MonoBehaviour, SocketProcessor{


	#region Mono

	void Awake(){
		{
			SocketTool.RegisterMessageProcessor( this );
		}
	}

	void Start(){
		// time clock for red spot notification fetch
		{
			StartCoroutine( FetchRedSpotNotification() );
		}
	}

	void Update(){
		if ( !SocketTool.IsConnected() ) {
			return;
		}

		// update count down
		{
			for( int i = m_red_spot_count_down_list.Count - 1; i >= 0; i-- ){
				RedSpotCountDown t_red_spot = m_red_spot_count_down_list[ i ];

				t_red_spot.Update();
			}
		}
	}

	void OnApplicationPause( bool p_pause ){
//		Debug.Log( "OnApplicationPause( " + p_pause + " )" );
		
		if ( p_pause ){
			PushAndNotificationHelper.UpdateLocalPush();
		}
		else{
			PushAndNotificationHelper.ClearAllPush();
		}
	}

	void OnDestroy(){
		{
			SocketTool.UnRegisterMessageProcessor( this );
		}
	}

	#endregion



	#region red spot notification

	public enum CountDownType{
		SetLocalRedSpot,
		FetchServerRedSpot,
	}

	private class RedSpotCountDown{
		private float m_count_down_total = 0.0f;

		private float m_count_down_remain = 0.0f;

		private int m_red_spot_id = -1;

		private CountDownType m_count_down_type = CountDownType.SetLocalRedSpot;

		public RedSpotCountDown( CountDownType p_type, int p_red_spot_id, float p_count_down_sec ){
			m_count_down_type = p_type;

			m_count_down_total = p_count_down_sec;

			m_count_down_remain = p_count_down_sec;

			m_red_spot_id = p_red_spot_id;
		}

		// update time
		public void Update(){
			m_count_down_remain = m_count_down_remain - Time.deltaTime;

			if ( IsDone () ) {
				OnDone();
			}
		}

		private void OnDone(){
			#if DEBUG_RED_SPOT_TIME_COUNT
			Debug.Log( "RedSpotCountDown( OnDone: " + m_red_spot_id + " )" );
			#endif

			switch (m_count_down_type) {
			case CountDownType.FetchServerRedSpot:
				OnFetchServerRedSpot();
				break;

			case CountDownType.SetLocalRedSpot:
				SetRedSpotNotification( m_red_spot_id, true );
				
				m_red_spot_count_down_list.Remove( this );
				break;
			}

			#if DEBUG_RED_SPOT_TIME_COUNT
			Debug.Log( "RedSpotCountDown( count: " + m_red_spot_count_down_list.Count + " )" );
			#endif
		}

		// check if is Done
		private bool IsDone(){
			return m_count_down_remain <= 0.0f;
		}
	}

	private static List<RedSpotCountDown> m_red_spot_count_down_list = new List<RedSpotCountDown>();

	/// <summary>
	/// Adds the count down red spot.
	/// </summary>
	/// 
	/// Type.SetLocalRedSpot, means when time out, local red spot will be lit.
	/// Type.FetchServerRedSpot, means when time out, connect server to check all red spot status.
	public static void AddCountDownRedSpot( CountDownType p_type, int p_function_id, float p_count_down_sec ){
		#if DEBUG_RED_SPOT_TIME_COUNT
		Debug.Log( "AddCountDownRedSpot( " + p_type + ", " + p_function_id + ", " + p_count_down_sec + " )" );
		#endif

		if( p_count_down_sec < 0 ){
			Debug.LogError( "Error for sec: " + p_count_down_sec );

			return;
		}

		RedSpotCountDown t_red_spot = new RedSpotCountDown( p_type, p_function_id, p_count_down_sec );

		m_red_spot_count_down_list.Add( t_red_spot );
	}

	public static void ClearCountDownRedSpot(){
		m_red_spot_count_down_list.Clear ();
	}

	/// <summary>
	/// Every 120s fetch red spot status automatically.
	/// </summary>
	/// <returns>The red spot notification.</returns>
	IEnumerator FetchRedSpotNotification(){
		while( true ){
			yield return new WaitForSeconds( 120.0f );

			OnFetchServerRedSpot();
		}
	}

	private static void OnFetchServerRedSpot(){
		if( SocketTool.Instance() != null ){
			if( SocketTool.IsConnected() ){
				#if DEBUG_RED_SPOT_FETCH
				Debug.Log( "SocketTool.IsConnected()()" );
				#endif

				TimeWorkerRequest tempRequest = new TimeWorkerRequest();
				
				tempRequest.type = 1;
				
				MemoryStream m_stream = new MemoryStream();
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				t_qx.Serialize(m_stream, tempRequest);
				
				
				byte[] t_byte;
				
				t_byte = m_stream.ToArray();
				
				SocketTool.Instance().SendSocketMessage(
					ProtoIndexes.C_ADD_TILI_INTERVAL,
					ref t_byte,
					false);
			}
		}
	}

	/// Manual Check if it could be manual changed.
	public static bool IsManualSetRedNotificationOK( int p_function_open_id, bool p_is_show ){
		if ( FunctionOpenTemp.IsRedSpotDataOpen( p_function_open_id ) ) {
			Debug.LogError( "Function is using red spot data, never change it manually: " + p_function_open_id + ", " + p_is_show );

			return false;
		}

		return true;
	}

	/// Set red spot status with function_open_id in FunctionOpenTemp.xml.
	/// 
	/// Note:
	/// 1.Only support new red spot notification with data.
	public static void SetRedSpotNotification( int p_function_open_id, bool p_is_show ){
//		{
//			TimeHelper.SignetTime();
//		}

		if ( !FunctionOpenTemp.IsRedSpotDataOpen( p_function_open_id ) ) {
			Debug.LogError( "SetRedSpotNotification Error, function not using red spot data: " + p_function_open_id );

			return;
		}

		FunctionOpenTemp.SetRedSpotNotification ( p_function_open_id, p_is_show );

//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "FunctionOpen.Set" );
//			
//			TimeHelper.SignetTime();
//		}

		FunctionOpenTemp.UpdateRedSpotDataHierarchy ();

//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "FunctionOpen.UpdateData" );
//			
//			TimeHelper.SignetTime();
//		}

		UpdateMainMenusNewRedSpot ();

//		{
//			TimeHelper.LogDeltaTimeSinceSignet( "FunctionOpen.UpdateMainMenu" );
//			
//			TimeHelper.SignetTime();
//		}
	}

	/// Check if new red spot notification with data is showing.
	/// 
	/// Note:
	/// 1.Only Support new red spot notification with data.
	public static bool IsShowRedSpotNotification( int p_function_open_id ){
		if ( !FunctionOpenTemp.IsRedSpotDataOpen( p_function_open_id ) ) {
			Debug.LogError( "IsShowRedSpotNotification Error, function not using red spot data: " + p_function_open_id );
			
			return false;
		}

		return FunctionOpenTemp.IsShowRedSpotNotification ( p_function_open_id );
	}

	/// Clear all red spot data.
	public static void ClearAllRedSpotNotification(){
		#if DEBUG_RED_SPOT
		Debug.Log( "ClearAllRedSpotNotification()" );
		#endif

		{
			FunctionOpenTemp.ClearAllRedSpotNotification ();
		}

		{
			ClearCountDownRedSpot();
		}
	}

	public static void LogRedSpotNotification(){
		FunctionOpenTemp.LogUseRedSpotData ();	
	}

	public static void UpdateMainMenusNewRedSpot(){
//		#if DEBUG_RED_SPOT
//		Debug.Log( "UpdateMainMenusNewRedSpot()" );
//		#endif

		List<FunctionOpenTemp> t_templates = FunctionOpenTemp.templates;

		for ( int i = 0; i < t_templates.Count; i++ ) {
			FunctionOpenTemp t_target = t_templates[ i ];

			if( !t_target.m_use_red_push_data ){
				continue;
			}

			// all new red spot are under MainCityUIRB
			MainCityUIRB.SetRedAlert( t_target.m_iID, t_target.m_show_red_alert, false );
		}
	}

	#endregion



	#region local push and notification

	public static void UpdateLocalPush(){
		Debug.Log ( "UpdateLocalPush()" );

		if ( !SocketTool.IsConnected () ) {
			Debug.Log( "Network not established, skip local push." );

			return;
		}

		// if network not connect, junzhudata.instance will cause an error, send buffer
		PushAndNotificationHelper.LocalTiliPush( JunZhuData.GetTimeOfCoverTiLi(), LanguageTemplate.GetText( LanguageTemplate.Text.STRENGTH_SUFFICIENT ) );
	}

	public static void ClearAllPush(){
		Debug.Log( "ClearAllPush()" );

		Bonjour.ClearAllPush();
	}

	private enum LocalPushType{
		TILI_OVERFLOW = 0,
	};

	private static string[] LocalPushKeys = {
		"Tili_OverFlow",
	};

	public static void LocalTiliPush( int p_push_sec_since_now, string p_push_content ){
		if ( p_push_sec_since_now <= 0 ) {
			#if DEBUG_LOCAL_PUSH
			Debug.Log( "Need not to push." );
			#endif

			return;
		}

		LocalPush( LocalPushKeys [ (int)LocalPushType.TILI_OVERFLOW ], 
		          p_push_sec_since_now,
		          p_push_content );
	}

	private static void LocalPush( string p_key, int p_push_sec_since_now, string p_push_content ){
		Debug.Log( "local push: " + p_key + ", " +
		                p_push_sec_since_now + ", " +
		                p_push_content );

		Bonjour.LocalPush ( p_key, p_push_sec_since_now, p_push_content );
	}

	#endregion



	#region Network processor

	
	public bool OnProcessSocketMessage( QXBuffer p_message ){
		if( p_message.m_protocol_index == ProtoIndexes.RED_NOTICE_INFO ){
			ErrorMessage t_error_body = new ErrorMessage();

			t_error_body = (ErrorMessage)ProtoHelper.DeserializeProto( t_error_body, p_message );

			#if DEBUG_RED_SPOT
			Debug.Log( "Server Active Red Spot: " + t_error_body.errorCode );
			#endif

			if( t_error_body.errorCode > 0 ){
				SetRedSpotNotification( t_error_body.errorCode, true );
			}
			else{
				SetRedSpotNotification( -t_error_body.errorCode, false );
			}

			if(t_error_body.errorCode == 1000001)
			{
				CityGlobalData.AllianceEventNotice = t_error_body.errorCode;
			}
			if(t_error_body.errorCode == 1000002)
			{
				CityGlobalData.AllianceApplyNotice = t_error_body.errorCode;
			}
			return true;
		}
				
		return false;
	}

	#endregion
}
