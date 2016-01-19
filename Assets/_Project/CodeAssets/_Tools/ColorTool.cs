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

	// 196 0 0
	public const string Color_Red_c40000 = "[c40000]";

	// 0 255 0
	public const string Color_Green_00ff00 = "[00ff00]";

	// 1 107 197
    public const string Color_Blue_016bc5 = "[016bc5]";

    // 0 0 0
    public const string Color_Black_000000 = "[000000]";

    //255 255 255
    public const string Color_White_ffffff = "[ffffff]";

	#region Mono

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion



	#region Utilities

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
