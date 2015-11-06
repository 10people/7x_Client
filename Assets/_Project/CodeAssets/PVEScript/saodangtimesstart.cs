using UnityEngine;
using System.Collections;

public class saodangtimesstart : MonoBehaviour {
	public GameObject supgame;
	private int CurTimes;
	UILabel lab;
	void Start () {
	
		lab = GetComponent<UILabel>();
		CurTimes = SaoDangBtn.saodanginfo.saodinfo.endTime;
	}
	
	public void loadback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate( p_object ) as GameObject;
		
		GameObject obj = GameObject.Find ("Map(Clone)");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		tempOjbect.transform.localPosition = new Vector3 (0,0,0);
		CurTimes = SaoDangBtn.saodanginfo.saodinfo.endTime;
		
	}
	public void loadback1(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate( p_object ) as GameObject;
		
		GameObject obj = GameObject.Find ("Map(Clone)");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		tempOjbect.transform.localPosition = new Vector3 (0,0,0);
		Destroy (supgame);
		
	}
	void Update () {
	

		lab.text = SaoDangBtn.saodanginfo.saodinfo.endTime.ToString () + "次";

		if( ( CurTimes + 1 ) == SaoDangBtn.saodanginfo.saodinfo.endTime ){

			Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),loadback);

		}

		if( SaoDangBtn.saodanginfo.saodinfo.endTime > SaoDangBtn.saodanginfo.saodinfo.allTime ){
			//扫荡完成
			Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAO_DANG_DONE ),loadback1);


		}
	}
}
