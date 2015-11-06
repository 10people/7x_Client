using UnityEngine;
using System.Collections;

public class EnterRankList : MonoBehaviour {

	void OnClick ()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.RANK_LIST ),
		                        RankListLoadBack );
	}

	public void RankListLoadBack( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject rankObj = (GameObject)Instantiate( p_object );
		
		rankObj.name = "RankList";
	}
}
