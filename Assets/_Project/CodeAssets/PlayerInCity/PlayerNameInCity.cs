using UnityEngine;
using System.Collections;

public class PlayerNameInCity : ObjectName{
    public void SetMyselfName(Vector3 tempVec3) //设置玩家名字位置
    {
		//base.m_playerName.text = Mathf.RoundToInt(tempVec3.x * 100).ToString();
//		+ "," + Mathf.RoundToInt(tempVec3.y * 100).ToString() + "," + Mathf.RoundToInt(tempVec3.z * 100).ToString();
    }

	public void OnDestroy(){
//		Debug.Log( "PlayerNameInCity.OnDestroy()" );
	}
}
