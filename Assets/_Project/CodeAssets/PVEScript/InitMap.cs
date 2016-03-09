using UnityEngine;
using System.Collections;

public class InitMap : MonoBehaviour {

	public UITexture mapTextrue;

	[HideInInspector]public int map_num;

	void OnDestroy(){
		mapTextrue = null;
	}

	public void LoadmapBack(ref WWW p_www,string p_path,UnityEngine.Object p_object)
	{
		mapTextrue.mainTexture = (Texture)p_object;
		mapTextrue.SetDimensions (959,570);
	}

	public void init()
	{
		//Debug.Log ("map_num  = " +map_num);
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.MAPPIC)+map_num.ToString(),LoadmapBack);
	}


}
