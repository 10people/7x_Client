using UnityEngine;
using System.Collections;

public class PopSaodangUI : MonoBehaviour {



	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
		
		GameObject obj = GameObject.Find ("Map(Clone)");
		tempOjbect.transform.parent = obj.transform;
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
	}

	void OnClick()
	{
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_SAO_DANG),LoadResourceCallback);

	}
}
