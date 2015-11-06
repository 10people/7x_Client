using UnityEngine;
using System.Collections;

public class ShowCrossDisWin : MonoBehaviour {

	GameObject pveObj;

	void Start (){
		pveObj = GameObject.Find ("PveUI_cq(Clone)");
	}

	void OnClick(){

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_LEVEL_GRADE ),
		                        ResourceLoadCallback );
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject tempOjbect = Instantiate( p_object ) as GameObject;
		
		tempOjbect.transform.parent = pveObj.transform;
		
		tempOjbect.transform.localScale = Vector3.one;
		
		tempOjbect.transform.localPosition = Vector3.zero;
		
		Debug .Log (" 弹出通关评价的UI、。。。。。。。。。。。。。" +tempOjbect.name);
	}
}
