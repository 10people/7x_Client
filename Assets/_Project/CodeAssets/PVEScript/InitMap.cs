using UnityEngine;
using System.Collections;

public class InitMap : MonoBehaviour {

	public UITexture mapTextrue;

	[HideInInspector]public int map_num;

	public void LoadmapBack(ref WWW p_www,string p_path,UnityEngine.Object p_object)
	{
		mapTextrue.mainTexture = (Texture)p_object;
	}
	public void init()
	{
		//Debug.Log ("map_num  = " +map_num);
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.MAPPIC)+map_num.ToString(),LoadmapBack);
	}
}
