using UnityEngine;
using System.Collections;

public class BuyTiLiBtn : MonoBehaviour {

	public static int buyTimes;
		   
	void Start () {
	
		buyTimes = 3;//测试3次情况 

	}

	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object)as GameObject;
		GameObject obj = GameObject.Find ("Map(Clone)");
		
		tempOjbect.transform.parent = obj.transform;
		
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
	}
	void OnClick(){
		if(buyTimes > 0){

			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_BUY_TI_LI ),
			                        LoadResourceCallback);

		}
		else{
			//购买次数已用完
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_BUY_TI_LI_NO_CHANCE ),
			                        LoadResourceCallback);

			
		}
	}
}
