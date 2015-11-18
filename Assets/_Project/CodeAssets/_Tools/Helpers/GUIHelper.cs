using UnityEngine;
using System.Collections;

public class GUIHelper {

	#region GUI Rect
	
	/** Desc:
	 * Get GUI Rect with index and params[ 6 ].
	 * 
	 * Params:
	 * p_offset_x: offset x
	 * p_offset_y: offset y
	 * p_size_x: size x
	 * p_size_y: size y
	 * p_delta_x: delta x
	 * p_delta_y: delta y
	 */
	public static Rect GetGUIRect( int p_index, float[] p_params ){
		return GetGUIRect(p_index,
		                  p_params[0], p_params[1],
		                  p_params[2], p_params[3],
		                  p_params[4], p_params[5]);
	}
	
	/** Params:
     * p_index: item index
     * p_offset_x: offset x
     * p_offset_y: offset y
     * p_size_x: size x
     * p_size_y: size y
     * p_delta_x: delta x
     * p_delta_y: delta y
     */
	public static Rect GetGUIRect( int p_index,
	                              float p_offset_x, float p_offset_y,
	                              float p_size_x, float p_size_y,
	                              float p_delta_x, float p_delta_y ){
		return new Rect(p_offset_x + p_index * p_delta_x, p_offset_y + p_index * p_delta_y,
		                p_size_x, p_size_y);
	}
	
	#endregion



	#region GUI JoyStick

	public enum GUIJoyStickType{
		Left,
		Right,
	}

	public enum GUIJoyStickButton{
		Up,
		Down,
		Left,
		Right,
	}

	public static Rect GetJoyStickRect( GUIJoyStickType p_type, GUIJoyStickButton p_button ){
		Rect t_rect = new Rect( 0, 0, ScreenTool.GetWidth( 0.075f ), ScreenTool.GetHeight( 0.15f ) );

		if( p_button == GUIJoyStickButton.Up ||
		   p_button == GUIJoyStickButton.Down ){
			t_rect.x = ScreenTool.GetX( 0.25f ) - t_rect.width / 2;
		}
		else if( p_button == GUIJoyStickButton.Left ){
			t_rect.x = ScreenTool.GetX( 0.25f ) - t_rect.width;
		}
		else{
			t_rect.x = ScreenTool.GetX( 0.25f );
		}

		if( p_type == GUIJoyStickType.Right ){
			t_rect.x = t_rect.x + ScreenTool.GetWidth( 0.5f );
		}

		if( p_button == GUIJoyStickButton.Left ||
		   p_button == GUIJoyStickButton.Right ){
			t_rect.y = ScreenTool.GetY( 0.5f );
		}
		else if( p_button == GUIJoyStickButton.Up ){
			t_rect.y = ScreenTool.GetY( 0.5f ) - t_rect.height;
		}
		else{
			t_rect.y = ScreenTool.GetY( 0.5f ) + t_rect.height;
		}

		return t_rect;
	}

	#endregion



	#region Common Utilities

	public static GUIStyle m_gui_lb_style;
	
	public static GUIStyle m_gui_btn_style;
	
	public static GUIStyle m_gui_text_field_style;

	
	public static float[] m_btn_rect_params = new float[ 6 ];
	
	public static float[] m_lb_rect_params = new float[ 6 ];
	
	public static float[] m_slider_rect_params = new float[ 6 ];
	
	public static float[] m_toggle_rect_params = new float[ 6 ];


	#endregion
}
