using UnityEngine;
using System.Collections;

public class NGUIHelper {

	#region NGUI
	
	public static GameObject GetUIRoot( GameObject p_game_object ){
		if (p_game_object == null)
		{
			Debug.LogError("Error, ngui gb = null.");
			
			return null;
		}
		
		Transform t_parent = p_game_object.transform.parent;
		
		while (t_parent != null)
		{
			if (t_parent.gameObject.GetComponent<UIRoot>() != null)
			{
				break;
			}
			
			t_parent = t_parent.parent;
			
			if (t_parent == null)
			{
				Debug.LogError("Error, UIRoot Not Founded.");
				
				return null;
			}
		}
		
		return t_parent.gameObject;
	}
	
	public static void SetScrollBarValue( UIScrollView view,UIScrollBar bar,float var ){
		if (var > 1 || var < 0)
		{
			Debug.LogError("Error setting in scroll bar.");
			return;
		}
		
		view.UpdateScrollbars(true);
		bar.value = var;
	}
	
	/// <summary>
	/// Adapt widget in verticle scroll view.
	/// </summary>
	/// <param name="scrollView">scroll view</param>
	/// <param name="scrollBar">scroll bar</param>
	/// <param name="widget">widget</param>
	public static void AdaptWidgetInScrollView( UIScrollView scrollView,UIScrollBar scrollBar,UIWidget widget ){
		//adapt pop up buttons to scroll view.
		float widgetValue = scrollView.GetWidgetValueRelativeToScrollView(widget).y;
		if (widgetValue < 0 || widgetValue > 1)
		{
			scrollView.SetWidgetValueRelativeToScrollView(widget, 0);
			
			//clamp scroll bar value.
			//donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
			//set 0.99 and 0.01 cause same bar value not taken in execute.
			float scrollValue = scrollView.GetSingleScrollViewValue();
			if (scrollValue >= 1) scrollBar.value = scrollBar.value == 1.0f ? 0.99f : 1.0f;
			if (scrollValue <= 0) scrollBar.value = scrollBar.value == 0f ? 0.01f : 0f;
		}
	}
	
	#endregion
}
