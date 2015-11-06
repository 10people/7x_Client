using UnityEngine;
using System.Collections;

public class EnterLueDuo : MonoBehaviour {


	void OnClick ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LUEDUO ),
		                        LueDuoObjLoadBack );
	}

	void LueDuoObjLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject LueDuoObj = GameObject.Instantiate( p_object ) as GameObject;
		LueDuoObj.name = "LueDuo";
		
		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}
	}
}
