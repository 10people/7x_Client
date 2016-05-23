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



	#region Label

	public static Vector2 GetTextWidth( UILabel p_label, string p_text ){
		if( p_label == null ){
			Debug.Log( "UILabel is null." );

			return Vector2.zero;
		}

		p_label.UpdateNGUIText( 0, 1000000, 1000000 );

		Vector2 t_size = NGUIText.CalculatePrintedSize( p_text );

		return t_size;
	}

	#endregion



	#region UIScroll View

	public static bool HaveUIScrollViewInParent( GameObject p_gb ){
		if( p_gb == null ){
			Debug.Log( "GameObject is null." );

			return false;
		}

		UIScrollView t_scroll = p_gb.GetComponentInParent<UIScrollView>();

		if( t_scroll != null ){
			return true;
		}

		return false;
	}

	#endregion



//	#region Pivot
//
//	public static bool IsLeft (UIWidget.Pivot pivot)
//	{
//		return pivot == UIWidget.Pivot.Left ||
//			pivot == UIWidget.Pivot.TopLeft ||
//			pivot == UIWidget.Pivot.BottomLeft;
//	}
//
//	public static bool IsRight (UIWidget.Pivot pivot)
//	{
//		return pivot == UIWidget.Pivot.Right ||
//			pivot == UIWidget.Pivot.TopRight ||
//			pivot == UIWidget.Pivot.BottomRight;
//	}
//
//	public static bool IsTop (UIWidget.Pivot pivot)
//	{
//		return pivot == UIWidget.Pivot.Top ||
//			pivot == UIWidget.Pivot.TopLeft ||
//			pivot == UIWidget.Pivot.TopRight;
//	}
//
//	public static bool IsBottom (UIWidget.Pivot pivot)
//	{
//		return pivot == UIWidget.Pivot.Bottom ||
//			pivot == UIWidget.Pivot.BottomLeft ||
//			pivot == UIWidget.Pivot.BottomRight;
//	}
//
//	public static UIWidget.Pivot GetHorizontal (UIWidget.Pivot pivot)
//	{
//		if (IsLeft(pivot)) return UIWidget.Pivot.Left;
//		if (IsRight(pivot)) return UIWidget.Pivot.Right;
//		return UIWidget.Pivot.Center;
//	}
//
//	public static UIWidget.Pivot GetVertical (UIWidget.Pivot pivot)
//	{
//		if (IsTop(pivot)) return UIWidget.Pivot.Top;
//		if (IsBottom(pivot)) return UIWidget.Pivot.Bottom;
//		return UIWidget.Pivot.Center;
//	}
//
//	public static UIWidget.Pivot Combine (UIWidget.Pivot horizontal, UIWidget.Pivot vertical)
//	{
//		if (horizontal == UIWidget.Pivot.Left)
//		{
//			if (vertical == UIWidget.Pivot.Top) return UIWidget.Pivot.TopLeft;
//			if (vertical == UIWidget.Pivot.Bottom) return UIWidget.Pivot.BottomLeft;
//			return UIWidget.Pivot.Left;
//		}
//
//		if (horizontal == UIWidget.Pivot.Right)
//		{
//			if (vertical == UIWidget.Pivot.Top) return UIWidget.Pivot.TopRight;
//			if (vertical == UIWidget.Pivot.Bottom) return UIWidget.Pivot.BottomRight;
//			return UIWidget.Pivot.Right;
//		}
//		return vertical;
//	}
//
//	#endregion
}
