using UnityEngine;
using System.Collections;

public class GetEnemyType : MonoBehaviour {

	private int CurrentEnemynumber;//敌人序列中的第几个的名字
	private int enemytype = 0;

	void Start () 
	{

		getenemy_Type (CurrentEnemynumber);
		this.transform.localPosition = new Vector3 (-40,40,0);
	}

	void getenemy_Type (int n)
	{
		//enemytype = GetPveTempID.GuanqiaReq.enemies[n].type;
		UISprite Enemytype = gameObject.GetComponent<UISprite> ();
		if(enemytype == 11)
		{
			Enemytype.spriteName = "";//盾
		}
		if(enemytype == 12)
		{
			Enemytype.spriteName = "";//车
		}
		if(enemytype == 13)
		{
			Enemytype.spriteName = "";//弓
		}
		if(enemytype == 14)
		{
			Enemytype.spriteName = "";//骑
		}
		if(enemytype == 21)
		{
			Enemytype.spriteName = "";//枪兵
		}
		if(enemytype == 22)
		{
			Enemytype.spriteName = "";//盾兵
		}
		if(enemytype == 23)
		{
			Enemytype.spriteName = "";//弓兵
		}
		if(enemytype >= 30&& enemytype <= 40)
		{
			Enemytype.spriteName = "";//boss
		}
	}
}
