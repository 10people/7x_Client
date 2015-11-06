using UnityEngine;
using System.Collections;

public class EnterBaiZhan : MonoBehaviour {
	public int boo;
	void OnClick ()
	{
		if(boo == 1)
		{
			if (GameObject.Find ("BaiZhan") == null)
			{
				BaiZhanWindow ();
			}
		}
		else{
			Debug.Log("AllianceData.Instance.Ex_Union"+AllianceData.Instance.IsAllianceNotExist);
            //if(AllianceData.Instance.Ex_Union == 1)
            //{
            //    Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_NOT_HAVE_ROOT ),
            //                            AllianceNotHaveLoadCallback );
            //}
            //if(AllianceData.Instance.Ex_Union == 2)
            //{
            //    Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_HAVE_ROOT ),
            //                            AllianceHaveLoadCallback );
            //}
            Global.ResourcesDotLoad("_UIs/MainCity/UISettingUp/Prefabs/SettingUpLayer",
                                          LoadCallback );
            
		}
	
	}

	public void AllianceNotHaveLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject tempObject = Instantiate( p_object ) as GameObject;
	}
    public void  LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
    }


	public void AllianceHaveLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject tempObject = Instantiate( p_object ) as GameObject;
	}

	void BaiZhanWindow ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVP_BAI_ZHAN ),
		                        BaiZhanLoadCallback );
	}

	public void BaiZhanLoadCallback( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject baizhanRoot = Instantiate( p_object ) as GameObject;
		
		baizhanRoot.SetActive (true);
		
		baizhanRoot.name = "BaiZhan";
		
		baizhanRoot.transform.localPosition = new Vector3 (0,800,0);
		
		baizhanRoot.transform.localScale = Vector3.one;
	}
}
