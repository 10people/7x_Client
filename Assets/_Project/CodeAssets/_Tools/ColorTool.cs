using UnityEngine;
using System.Collections;

public class ColorTool : MonoBehaviour {
	// 255 248 217
	public const string Color_White_fff8d9 = "[fff8d9]";

	// 237 195 71
	public const string Color_Gold_edc347 = "[edc347]";

	// pop window tips
	// 255 177 42
	public const string Color_Gold_ffb12a = "[ffb12a]";

    //f4e002
    public const string Color_Yellow_f4e002 = "[f4e002]";

	// 196 0 0
	public const string Color_Red_c40000 = "[c40000]";

	// 255 0 0
	public const string Color_Red_FF0000 = "[ff0000]";

	// 0 255 0
	public const string Color_Green_00ff00 = "[00ff00]";

	// 32 219 2
	public const string Color_Green_20db02 = "[20db02]";

	// 1 107 197
    public const string Color_Blue_016bc5 = "[016bc5]";

    //1 221 240
    public const string Color_Blue_01edf0 = "[01edf0]";

    // 0 0 0
    public const string Color_Black_000000 = "[000000]";

    //255 255 255
    public const string Color_White_ffffff = "[ffffff]";

    //203 2 232
    public const string Color_Purple_cb02d8 = "[cb02d8]";

    //255 127 0
    public const string Color_Orange_ff7f00 = "[FF7F00]";

    #region Mono

	#endregion



	#region Utilities

    public static string GetColorString(string color, string originalStr)
    {
        if (string.IsNullOrEmpty(color))
        {
            Debug.LogError("Color is null");

            return null;
        }

        return "[" + color + "]" + originalStr + "[-]";
    }

	public static string GetQualityPrefix( int p_quality ){
		//Debug.Log( "UpdateItemName: " + p_quality + " - " + p_name );
		
		switch( p_quality ){
		case 0:
		case 1:
			return CityGlobalData.Color_Equip_White;
			
		case 2:
			return CityGlobalData.Color_Equip_Green;
			
		case 3:
			return CityGlobalData.Color_Equip_Blue;
			
		case 4:
			return CityGlobalData.Color_Equip_Purple;
			
		case 5:
			return CityGlobalData.Color_Equip_Orange;
			
		}

		Debug.LogError( "Error, Quality " + p_quality );
		
		return "[000000]";
	}

	public static string UpdateWithQuality( string p_str, int p_quality ){
		string t_prefix = GetQualityPrefix( p_quality );

		return t_prefix + p_str + "[-]";
	}

	#endregion
}
