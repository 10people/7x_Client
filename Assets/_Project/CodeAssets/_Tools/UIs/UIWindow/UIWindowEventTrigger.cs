//#define DEBUG_TRIGGER



using UnityEngine;
using System.Collections;



public class UIWindowEventTrigger : MonoBehaviour, UIWindowEventListener{

	public const int DEFAULT_UI_WINDOW_ID			= 0;

	public const int DEFAULT_POP_OUT_WINDOW_ID		= 100;

	/// Never Change it Manually in runtime, use Set
	public int m_ui_id = UIWindowEventTrigger.DEFAULT_UI_WINDOW_ID;

	/// true, fx will hide if window not the top
	/// false, fx will not hide if window is not top
	public bool m_fx_auto_hide = false;

	#region Mono

	void OnEnable(){
		UIWindowTool.OnWindowEvent( this, m_ui_id, UIWindowTool.WindowEvent.UI_OPEN );
	}

	void OnDisable(){
		UIWindowTool.OnWindowEvent( this, m_ui_id, UIWindowTool.WindowEvent.UI_CLOSE );
	}

	public void SetWindowId( int p_window_id ){
		m_ui_id = p_window_id;

		UIWindowTool.UpdateGuideStatus();
	}

	#endregion



	#region Callback

	private DelegateHelper.VoidDelegate m_on_top_delegate = null;

	/** Note:
	 * 1.Any time when onTop, this will be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public void SetOnTopDelegate( DelegateHelper.VoidDelegate p_delegate ){
		m_on_top_delegate = p_delegate;
	}

	/** Note:
	 * 1.Any time when onTop, this will be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public static void SetOnTopDelegate( GameObject p_trigger_gb, DelegateHelper.VoidDelegate p_delegate ){
		if( p_trigger_gb == null ){
			Debug.LogError( "Trigger GameObject is null." );

			return;
		}

		UIWindowEventTrigger t_trigger = p_trigger_gb.GetComponent<UIWindowEventTrigger>();

		if( t_trigger == null ){
			Debug.LogError( "Trigger not configged." );
		}
		else{
			t_trigger.SetOnTopDelegate( p_delegate );
		}
	}

	private DelegateHelper.VoidDelegate m_on_top_again_delegate = null;

	/** Note:
	 * 1.If UI was on top again, then will be called.
	 * 2.First Time the UI is open, will not be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public void SetOnTopAgainDelegate( DelegateHelper.VoidDelegate p_delegate ){
		m_on_top_again_delegate = p_delegate;
	}

	/** Note:
	 * 1.If UI was on top again, then will be called.
	 * 2.First Time the UI is open, will not be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public static void SetOnTopAgainDelegate( GameObject p_trigger_gb, DelegateHelper.VoidDelegate p_delegate ){
		if( p_trigger_gb == null ){
			Debug.LogError( "Trigger GameObject is null." );

			return;
		}

		UIWindowEventTrigger t_trigger = p_trigger_gb.GetComponent<UIWindowEventTrigger>();

		if( t_trigger == null ){
			Debug.LogError( "Trigger not configged." );
		}
		else{
			t_trigger.SetOnTopAgainDelegate( p_delegate );
		}
	}

	#endregion



	#region Window Event Listener

	public void OnWindowEvent( int p_ui_id, UIWindowTool.WindowEvent p_ui_event ){
		
	}

	/** Note:
	 * 1.If UI was on top again, then will be called.
	 * 2.First Time the UI is open, will not be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public void OnTopAgain( int p_ui_id ){
		if( m_ui_id == p_ui_id ){
			if( m_on_top_again_delegate != null ){
				#if DEBUG_TRIGGER
				Debug.Log( "OnTopAgain( " + m_on_top_again_delegate + " )" );
				#endif

				m_on_top_again_delegate();
			}
		}
	}

	/** Note:
	 * 1.Any time when onTop, this will be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	public void OnTop( int p_ui_id ){
		if( m_ui_id == p_ui_id ){
			if( m_on_top_delegate != null ){
				#if DEBUG_TRIGGER
				Debug.Log( "OnTopAgain( " + m_on_top_delegate + " )" );
				#endif

				m_on_top_delegate();
			}
		}
	}

	#endregion



	#region Utilities



	#endregion
}
