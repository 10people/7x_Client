


using UnityEngine;
using System.Collections;



public interface UIWindowEventListener{
	
	void OnWindowEvent( int p_ui_id, UIWindowTool.WindowEvent p_ui_event );

	/** Note:
	 * 1.If UI was on top again, then will be called.
	 * 2.First Time the UI is open, will not be called.
	 * 3.Id will be send, because logic may under a big class.
	 */ 
	void OnTopAgain( int p_ui_id );
}
