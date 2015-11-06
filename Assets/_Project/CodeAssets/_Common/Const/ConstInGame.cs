using UnityEngine;
using System.Collections;

public class ConstInGame : MonoBehaviour {

	// ------------------------- PlayerPrefs -----------------------------------------

	public const string CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_VERSION_DEFAULT	= "";

	// small bundle version
	public const string CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_SMALL_VERSION	= "CachedBundleSmallVersion";

	// big bundle version
	public const string CONST_PLAYER_PREFS_KEY_CACHED_BUNDLE_BIG_VRESION	= "CachedBundleBigVersion";

	public const string CONST_ENCRYPT_RJDL_KEY	= "MVIDMIG3F";

	public const string CONST_ENCRYPT_RJDL_IV	= "IMVYWMGU1";

	// First Time To Play Video
	public const string CONST_FIRST_TIME_TO_PLAY_VIDEO = "FirstTimePlayVideo";

	public const string CONST_EXTRACT_BUNBLES_KEY		= "ExtractAllBundles";


	// ------------------------- PlayerPrefs -----------------------------------------



	// ------------------------- Scene Name -----------------------------------------

	#region Main Scene

	public const string CONST_SCENE_NAME_BUNDLE			= "Game_Scene_Bundle_Loader";

	public const string CONST_SCENE_NAME_LOGIN			= "Game_Scene_Login";

	public const string CONST_SCENE_NAME_CREATE_ROLE	= "Game_Scene_Create_Role";

	public const string CONST_SCENE_NAME_HOUSE			= "Game_Scene_House";

	public const string CONST_SCENE_NAME_CARRIAGE       = "Game_Scene_Carriage";

	public const string CONST_SCENE_NAME_ALLIANCE_BATTLE       = "Game_Scene_AllianceBattle";

	public const string CONST_SCENE_NAME_MAIN_CITY		= "Game_Scene_MainCity";

    public const string CONST_SCENE_NAME_MAIN_CITY_YEWAN = "Game_Scene_MainCity_YeWan";

    public const string CONST_SCENE_NAME_LOADING___FOR_COMMON_SCENE	= "Game_Scene_Loading";


	public const string CONST_SCENE_NAME_ALLIANCE_CITY = "Game_Scene_AllianceCity";

	public const string CONST_SCENE_NAME_ALLIANCE_CITY_YE_WAN = "Game_Scene_AllianceCity_YeWan";


	public const string CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_ONE = "Game_Scene_AllianceTenentsCity_One";

    public const string CONST_SCENE_NAME_ALLIANCE_CITY_TENENTS_CITY_YEWAN = "Game_Scene_AllianceTenentsCity_One_YeWan";
	#endregion



	#region Battle Field

	/// Every Battle Scene MUST Start With "BattleField_V4_".
	public const string CONST_SCENE_NAME_BATTLE_FIELD_PREFIX		= "BattleField_V4_";

	#endregion



	#region Debug
	
	public const string CONST_SCENE_VERSION_CHECK_AND_PRE_LOAD				= "Debug_Bundle_Loader";

	#endregion


	#region Battle Field Replay
	
	public const string CONST_SCENE_NAME_BATTLE_FIELD_REPLAY		= "BattleReplay_V4";
	
	#endregion
	

	// -------------------------- Scene Name ------------------------------------------



	// ------------------------- Persistent Data File -----------------------------------------
	
	public const string CONST_PERSISTENT_DATA_BUNDLE_LIST	= "BundleList";

	public const string CONST_NEW_CHENGHAO = "NewChenghao";
	
	// ------------------------- Persistent Data File -----------------------------------------
}
