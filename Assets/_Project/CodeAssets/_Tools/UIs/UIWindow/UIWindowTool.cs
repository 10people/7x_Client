//#define DEBUG_WINDOW_TOOL



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/** 
 * @author:		Zhang YuGu
 * @Date: 		2016.4.12
 * @since:		Unity 5.1.3
 * Function:	Help to manage 2d UI's events, all managed window must use UIWindowEventTrigger.
 * 
 * Notes:
 * 1.For cur requirement and usage, all ui events will be here, now only contains Open and Close.
 * 2.Events will be automatically triggered with UIWindowEvents or Handled by hand.
 * 3.All UI info will be here, both Function UI and PopUp UI.
 * 4.Events can be listened by UIWindowEventListener.
 * 5.Add UIWindowEventTrigger to your window, if want to be auto managed.
 */ 
public class UIWindowTool : Singleton<UIWindowTool> {

	public enum WindowEvent{
		UI_OPEN = 1,
		UI_CLOSE = 2,
	}

	private static List<UIWindowEventListener> m_window_event_listeners_list = new List<UIWindowEventListener>();

	// 0 is Bottom, N-1 is Top.
	private static List<WindowState> m_window_state_list = new List<WindowState>();



	#region Mono

	void Start() {
	
	}
	
	void Update() {
		{
			UpdateStates();
		}

		{
			UpdateListeners();
		}

		{
			UpdateLog();
		}
	}

	#endregion



	#region Use

	/// Get Current Top UI.
	public static int GetCurrentUIId(){
		if( m_window_state_list.Count == 0 ){
			return UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID;
		}

		{
			WindowState t_state = m_window_state_list[ m_window_state_list.Count - 1 ];

			return t_state.GetUIId();
		}
	}

	public static void OnWindowEvent( UIWindowEventTrigger p_target, int p_ui_id, WindowEvent p_ui_event ){
		{
			ProcessWindowEvent( p_target, p_ui_id, p_ui_event );
		}

		{
			OnTriggerEventListeners( p_ui_id, p_ui_event );
		}

		{
			UpdateGuideStatus();
		}
	}

	public static void RegisterWindowEventListener( UIWindowEventListener p_listener ){
		if( p_listener == null ){
			return;
		}

		if( m_window_event_listeners_list.Contains( p_listener ) ){
			return;
		}

		m_window_event_listeners_list.Add( p_listener );
	}

	public static void UnRegisterWindowEventListener( UIWindowEventListener p_listener ){
		if( p_listener == null ){
			return;
		}

		if( !m_window_event_listeners_list.Contains( p_listener ) ){
			return;
		}

		m_window_event_listeners_list.Remove( p_listener );
	}

	#endregion



	#region Update

	private static void UpdateStates(){
//		for( int i = m_window_state_list.Count - 1; i >= 0; i-- ){
//			WindowState t_state = m_window_state_list[ i ];
//		}
	}

	private static void UpdateListeners(){
		for( int i = m_window_event_listeners_list.Count - 1; i >= 0; i-- ){
			UIWindowEventListener t_listener = m_window_event_listeners_list[ i ];

			if( t_listener == null ){
				m_window_event_listeners_list.Remove( t_listener );
			}
		}
	}

	private static void OnTriggerEventListeners( int p_ui_id, WindowEvent p_ui_event ){
		for( int i = 0; i < m_window_event_listeners_list.Count; i++ ){
			UIWindowEventListener t_listener = m_window_event_listeners_list[ i ];

			if( t_listener != null ){
				t_listener.OnWindowEvent( p_ui_id, p_ui_event );
			}
		}
	}

	#endregion



	#region Process

	public static void UpdateGuideStatus(){
		if( UIYindao.m_UIYindao == null ){
			return;
		}

		if( !UIYindao.m_UIYindao.m_isOpenYindao ){
			return;
		}

		if( GuideInfoTemplate.GetWindowId_By_GuideId( UIYindao.m_UIYindao.m_iCurId ) == UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID ){
			#if DEBUG_WINDOW_TOOL
			Debug.Log( "Top Window is Null." );
			#endif

			return;
		}

		#if DEBUG_WINDOW_TOOL
		Debug.Log ( "Cur.Guide.Id = "+UIYindao.m_UIYindao.m_iCurId);
		Debug.Log ( "Target.Window.Id = "+GuideInfoTemplate.GetWindowId_By_GuideId( UIYindao.m_UIYindao.m_iCurId) );
		Debug.Log ( "Cur.Window.Id = "+GetTopOpenedWindow().GetUIId());
		#endif

		if( GuideInfoTemplate.GetWindowId_By_GuideId( UIYindao.m_UIYindao.m_iCurId ) != GetTopOpenedWindowId() ){
			#if DEBUG_WINDOW_TOOL
			Debug.Log( "Guide and Window not the same, Close Guide Now." );
			#endif

			UIYindao.m_UIYindao.CloseUI();
		}
	}

	public static void ProcessWindowEvent( UIWindowEventTrigger p_target, int p_ui_id, WindowEvent p_ui_event ){
		if( p_ui_event == WindowEvent.UI_OPEN ){
			OnProcessOpen( p_target, p_ui_id, p_ui_event );
		}
		else if( p_ui_event == WindowEvent.UI_CLOSE ){
			OnProcessClose( p_target, p_ui_id, p_ui_event );
		}
		else{
			Debug.LogError( "Event Not Defined." );
		}
	}

	private static void OnProcessOpen( UIWindowEventTrigger p_target, int p_ui_id, WindowEvent p_ui_event ){
		WindowState t_state = GetWindowState( p_ui_id );

		if( t_state == null ){
			#if DEBUG_WINDOW_TOOL
			Debug.Log( "Open New Window: " + p_ui_id );
			#endif

			t_state = new WindowState( p_target, p_ui_id, p_ui_event );

			m_window_state_list.Add( t_state );
		}
		else{
			#if DEBUG_WINDOW_TOOL
			Debug.Log( "Open Already Existed Window: " + p_ui_id );
			#endif

			m_window_state_list.Remove( t_state );

			m_window_state_list.Add( t_state );

			TryNotifyOnTopAgain();
		}
	}

	private static void OnProcessClose( UIWindowEventTrigger p_target, int p_ui_id, WindowEvent p_ui_event ){
		#if DEBUG_WINDOW_TOOL
		Debug.Log( "Close Window: " + p_ui_id );
		#endif

		WindowState t_top_state = GetTopOpenedWindow();

		WindowState t_state = GetWindowState( p_ui_id );

		if( t_state == null ){
			#if UNITY_EDITOR
			Debug.LogError( "UI Not Exist: " + p_ui_id + " - " + p_ui_event );
			#endif

			return;
		}

		{
			m_window_state_list.Remove( t_state );
		}

		if( t_top_state == t_state ){
			TryNotifyOnTopAgain();
		}
	}

	private static void TryNotifyOnTopAgain(){
		#if DEBUG_WINDOW_TOOL
		Debug.Log( "TryNotifyOnTopAgain()" );
		#endif

		WindowState t_state = GetTopOpenedWindow();

		if( t_state == null ){
			#if DEBUG_WINDOW_TOOL
			Debug.Log( "Top Window doesn't exist." );
			#endif

			return;
		}

		{
			t_state.NotifyOnTopAgain( t_state.GetUIId() );
		}

		{
			for( int i = 0; i < m_window_event_listeners_list.Count; i++ ){
				UIWindowEventListener t_listener = m_window_event_listeners_list[ i ];

				if( t_listener != null ){
					#if DEBUG_WINDOW_TOOL
					Debug.Log( "OnTopAgain( " + t_listener + " )" );
					#endif

					t_listener.OnTopAgain( t_state.GetUIId() );
				}
			}
		}
	}

	#endregion



	#region Clear

	public static void Clear(){
		{
			m_window_state_list.Clear();
		}

		{
			m_window_event_listeners_list.Clear();
		}
	}

	#endregion



	#region Log

	public bool m_log_info = false;

	private void UpdateLog(){
		if( !m_log_info ){
			return;
		}

		m_log_info = false;

		Debug.Log( "------ UIWindowTool ------" );

		for( int i = m_window_state_list.Count - 1; i >= 0; i-- ){
			WindowState t_state = m_window_state_list[ i ];

			if( t_state != null ){
				t_state.Log( i + "" );
			}
			else{
				Debug.Log( i + ": is null." );
			}
		}
	}

	#endregion



	#region Utilities

	public int GetWindowCount(){
		return m_window_state_list.Count;
	}

	public WindowState GetWindow( int p_index ){
		if( p_index < 0 || p_index >= m_window_state_list.Count ){
			Debug.LogError( "Error, out of bound." );

			return null;
		}

		return m_window_state_list[ p_index ];
	}

	private static WindowState GetWindowState( int p_ui_id ){
		for( int i = m_window_state_list.Count - 1; i >= 0; i-- ){
			WindowState t_state = m_window_state_list[ i ];

			if( t_state != null ){
				if( t_state.GetUIId() == p_ui_id ){
					return t_state;
				}
			}
		}

		return null;
	}

	private static WindowState GetTopOpenedWindow(){
		for( int i = m_window_state_list.Count - 1; i >= 0; i-- ){
			WindowState t_state = m_window_state_list[ i ];

			if( t_state != null ){
				if( t_state.IsUIOpen() ){
					return t_state;
				}
			}
		}

		return null;
	}

	private static int GetTopOpenedWindowId(){
		WindowState t_state = GetTopOpenedWindow();

		if( t_state == null ){
			return UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID;
		}

		return t_state.GetUIId();
	}

	#endregion



	/// Opened Window.
	public class WindowState{
		private UIWindowEventTrigger m_trigger;

		private WindowEvent m_window_event;

		public WindowState( UIWindowEventTrigger p_target, int p_ui_id, WindowEvent p_window_event ){
			m_trigger = p_target;

			m_window_event = p_window_event;
		}

		#region Use

		public bool Exist(){
			return m_trigger != null;
		}

		public UIWindowEventTrigger GetTrigger(){
			return m_trigger;
		}

		public int GetUIId(){
			if( m_trigger == null ){
				Debug.LogError( "Error, Trigger not exist." );

				return UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID;
			}

			return m_trigger.m_ui_id;
		}

		public bool IsUIOpen(){
			if( m_window_event == WindowEvent.UI_OPEN ){
				return true;
			}

			return false;
		}

		public WindowEvent GetWindowEvent(){
			return m_window_event;
		}

		public void NotifyOnTopAgain( int p_ui_id ){
			if( m_trigger == null ){
				#if DEBUG_WINDOW_TOOL
				Debug.Log( "Top Window's trigger doesn't exist." );
				#endif

				return;
			}

			#if DEBUG_WINDOW_TOOL
			Debug.Log( "WindowState.Trigger.NotifyOnTopAgain( " + p_ui_id + " )" );
			#endif

			m_trigger.OnTopAgain( p_ui_id );
		}

		#endregion



		#region Utilities

		public void Log( string p_prefix ){
			if( m_trigger == null ){
				Debug.Log( p_prefix + ": trigger is null --- " + m_window_event );	

				return;
			}
			else{
				Debug.Log( p_prefix + ": " + m_trigger.m_ui_id + " --- " + m_window_event );
			}
		}

		#endregion
	}



}
