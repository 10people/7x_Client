using UnityEngine;
using System.Collections;

public class EnterYunBiao : MonoBehaviour {

	void OnClick ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YUNBIAO_MAIN_PAGE ),
		                        JieBiaoMainPageLoadBack );
	}

	void JieBiaoMainPageLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject jieHuoMainPageObj = GameObject.Instantiate( p_object ) as GameObject;
	}
}
