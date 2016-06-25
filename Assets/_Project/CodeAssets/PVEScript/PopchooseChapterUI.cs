using UnityEngine;
using System.Collections;

public class PopchooseChapterUI : MonoBehaviour {

	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object )as GameObject;
		tempOjbect.name = "ChooseZhangjie";
	}
	void OnClick()
	{

		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_CHOOSE_CHAPTER),LoadResourceCallback);
	
	}
}
