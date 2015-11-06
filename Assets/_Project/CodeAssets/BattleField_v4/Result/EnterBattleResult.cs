using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnterBattleResult : ScriptableObject
{

	private static BattleResultControllor resultControllor;

	private static BattleControlor.BattleResult result;

	private static List<Enums.Currency> currencies;

	private static List<int> nums;

	private static int battleTime_second;

	private static int totalTime_second;

	private static BattleResultControllor.CloseCallback closeCallback;


	public static void showBattleResult(BattleControlor.BattleResult t_result, List<Enums.Currency> t_currencies, List<int> t_nums,
	                                    int t_battleTime_second, int t_totalTime_second, BattleResultControllor.CloseCallback t_closeCallback = null)
	{
		result = t_result;

		currencies = t_currencies;

		nums = t_nums;

		battleTime_second = t_battleTime_second;

		totalTime_second = t_totalTime_second;

		closeCallback = t_closeCallback;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.BATTLE_RESULT), LoadResultResCallback);
	}

	private static void LoadResultResCallback(ref WWW p_www, string p_path, Object p_object)
	{
		GameObject resultObject = (GameObject)Instantiate(p_object);
		
		resultObject.transform.localPosition = new Vector3 (5000, 0, 0);
		
		resultObject.transform.localScale = new Vector3 (1, 1, 1);
		
		resultObject.transform.localEulerAngles = Vector3.zero;

		resultObject.SetActive (true);
		
		resultControllor = resultObject.GetComponent<BattleResultControllor>();

		resultControllor.showResultGeneral (result, currencies, nums, battleTime_second, totalTime_second, closeCallback);
	}

}
