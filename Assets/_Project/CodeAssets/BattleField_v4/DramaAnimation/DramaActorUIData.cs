using UnityEngine;
using System.Collections;

using SimpleJSON;

public class DramaActorUIData : MonoBehaviour
{
	public enum UIActionType
	{
		actionMove,
		actionSale,
		actionAlpha,
		actionAnimator,
	}

	public UIActionType actionType;

	public float waittingTime;

	public float actionTime;

	public float startValue;

	public float endValue;

	public Vector3 startVector3;

	public Vector3 endVector3;

	public string animatorPath;

	public iTween.EaseType easeType;


	[HideInInspector] public RuntimeAnimatorController runTimeAnimatorController;


	public JSONClass getJson()
	{
		JSONClass json = new JSONClass ();

		json ["actionType"].AsInt = (int)actionType;

		json ["waittingTime"].AsFloat = waittingTime;

		json ["actionTime"].AsFloat = actionTime;

		json ["startValue"].AsFloat = startValue;

		json ["endValue"].AsFloat = endValue;

		json ["startVector3x"].AsFloat = startVector3.x;

		json ["startVector3y"].AsFloat = startVector3.y;

		json ["startVector3z"].AsFloat = startVector3.z;

		json ["endVector3x"].AsFloat = endVector3.x;
		
		json ["endVector3y"].AsFloat = endVector3.y;
		
		json ["endVector3z"].AsFloat = endVector3.z;

		json ["animatorPath"] = animatorPath;

		json ["easeType"].AsInt = (int)easeType;

		return json;
	}

	public static DramaActorUIData getDaraByJson(JSONClass json, GameObject gc)
	{
		DramaActorUIData data = gc.AddComponent<DramaActorUIData> ();

		data.actionType = (UIActionType)json ["actionType"].AsInt;

		data.waittingTime = json ["waittingTime"].AsFloat;

		data.actionTime = json ["actionTime"].AsFloat;

		data.startValue = json ["startValue"].AsFloat;

		data.endValue = json ["endValue"].AsFloat;

		data.startVector3 = new Vector3 (json ["startVector3x"].AsFloat, json ["startVector3y"].AsFloat, json ["startVector3z"].AsFloat);

		data.endVector3 = new Vector3 (json ["endVector3x"].AsFloat, json ["endVector3y"].AsFloat, json ["endVector3z"].AsFloat);

		data.animatorPath = json ["animatorPath"];

		data.easeType = (iTween.EaseType)json ["easeType"].AsInt;

		return data;
	}

}
