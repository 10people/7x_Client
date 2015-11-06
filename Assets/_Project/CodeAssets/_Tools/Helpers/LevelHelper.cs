#define DEBUG_LEVEL_HELPER

using UnityEngine;
using System.Collections;

public class LevelHelper {
	
	#region Scene Checkers
	
	/// <summary>
	/// Determine if is in battle scene.
	/// </summary>
	public static bool IsInBattleScene(){
		bool t_battle_field = Application.loadedLevelName.StartsWith( ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_PREFIX );
		
		return t_battle_field;
	}
	
	#endregion
	
	
	
	#region Scene Names
	
	/// <summary>
	/// Get the battle scene name by identifier.
	/// </summary>
	public static string GetBattleSceneNameById( string p_battle_field_id ){
		return ConstInGame.CONST_SCENE_NAME_BATTLE_FIELD_PREFIX + p_battle_field_id;
	}
	
	#endregion



//	#region Director Load Load
//
//	#endregion
}
