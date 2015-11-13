using UnityEngine;
using System.Collections;

public class GUIHelper {

	#region GUI
	
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
}
