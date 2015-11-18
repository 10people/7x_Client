using UnityEngine;
using System.Collections;

public class ShowTip : ScriptableObject
{

	private static int m_commonItemId;

	private static string m_iconName;

	private static string m_enemyName;

	private static string m_enemyDesc;


	private static GameObject tipObject;


	public static void showTipEnemy(string iconName, string enemyName, string enemyDesc)
	{
		m_iconName = iconName;

		m_enemyName = enemyName;

		m_enemyDesc = enemyDesc;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TIP), LoadResultResCallbackEnemy);
	}

	private static void LoadResultResCallbackEnemy(ref WWW p_www, string p_path, Object p_object)
	{
		if (tipObject != null) return;

		tipObject = (GameObject)Instantiate(p_object);
		
		tipObject.transform.localPosition = new Vector3 (5000, 0, 0);
		
		tipObject.transform.localScale = new Vector3 (1, 1, 1);
		
		tipObject.transform.localEulerAngles = Vector3.zero;
		
		tipObject.SetActive (true);
		
		TipUIControllor tipControllor = tipObject.GetComponent<TipUIControllor>();
		
		tipControllor.refreshDataEnemy (m_iconName, m_enemyName, m_enemyDesc);
	}

	public static void showTip(int commonItemId)
	{
//		Debug.Log ("Show Tips With Id " + commonItemId);

		bool flag = CommonItemTemplate.haveCommonItemTemplateById (commonItemId);

		if (flag == false) return;

		m_commonItemId = commonItemId;

		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.TIP), LoadResultResCallbackItem);
	}

	private static void LoadResultResCallbackItem(ref WWW p_www, string p_path, Object p_object)
	{
		if (tipObject != null) return;
		
		tipObject = (GameObject)Instantiate(p_object);
		
		tipObject.transform.localPosition = new Vector3 (5000, 0, 0);
		
		tipObject.transform.localScale = new Vector3 (1, 1, 1);
		
		tipObject.transform.localEulerAngles = Vector3.zero;
		
		tipObject.SetActive (true);
		
		TipUIControllor tipControllor = tipObject.GetComponent<TipUIControllor>();
		
		tipControllor.refreshDataItem (m_commonItemId);
	}

	public static void close()
	{
		if (tipObject == null) return;

		DestroyObject (tipObject);

		tipObject = null;
	}

}
