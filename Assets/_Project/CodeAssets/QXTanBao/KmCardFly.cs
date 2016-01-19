using UnityEngine;
using System.Collections;

public class KmCardFly : MonoBehaviour {
	
	private float time = 1;
	
	private float speed = 2.0f;
	
	private float ranPosX;
	private float ranPosY;
	private float minX = -5;
	private float maxX = -2;
	private float minY = -5;
	private float maxY = -2;
	
	//cardItem移动
	public void CardTrans () 
	{
		LittleFly ();
	}

	//卡片漂浮效果,向随机一个位置漂浮
	void LittleFly ()
	{
		ranPosX = Random.Range (minX,maxX);
		ranPosY = Random.Range (minY,maxY);

		Vector3 posTo = new Vector3 (ranPosX,ranPosY,0);
		
		Hashtable fly = new Hashtable ();
		fly.Add ("speed",speed);
		fly.Add ("easetype",iTween.EaseType.easeInOutSine);
		fly.Add ("position",posTo);
		fly.Add ("islocal", true);
		fly.Add ("oncomplete", "FlyEnd");
		fly.Add ("oncompletetarget", gameObject);
		iTween.MoveTo (this.gameObject,fly);
	}
	
	//卡片从原位置向某个位置漂浮结束时,向原来的位置漂浮回去
	void FlyEnd ()
	{
		Hashtable fly = new Hashtable ();
		fly.Add ("speed",speed);
		fly.Add ("easetype",iTween.EaseType.easeInOutSine);
		fly.Add ("position",Vector3.zero);
		fly.Add ("islocal", true);
		fly.Add ("oncomplete", "LittleFly");
		fly.Add ("oncompletetarget", gameObject);
		iTween.MoveTo (this.gameObject,fly);
	}
}
