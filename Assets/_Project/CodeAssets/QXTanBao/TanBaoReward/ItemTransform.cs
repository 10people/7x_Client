using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ItemTransform : MonoBehaviour {

//	public int tanBaoType;
//
//	public float time;
//
//	public GameObject transPoint1;//精铁飞向的位置设定
//	public GameObject transPoint2;//青铜飞向的位置设定
//	
//	public UISprite icon;
//
//	public GameObject singleObj;
//	public GameObject multipleObj;
//
//	public GameObject moneyObj;
//
//	void Start ()
//	{
//		WaitTrans ();
//	}
//
//	//移动
//	void WaitTrans ()
//	{
//		Hashtable move = new Hashtable ();
//		move.Add ("easeType",iTween.EaseType.easeInQuart);
//
//		if (tanBaoType == 0)
//		{
//			move.Add ("position",transPoint1.transform.position);
//		}
//		else
//		{
//			move.Add ("position",transPoint2.transform.position);
//		}
//
//		move.Add ("time", time);
//		move.Add ("onstart","MoveBegin");
//		move.Add ("onupdate","Moveing");
//		move.Add ("oncomplete","MoveEnd");
//		iTween.MoveTo (this.gameObject,move);
//		
//		Hashtable scale = new Hashtable ();
//		scale.Add ("time",time);
//		scale.Add ("easeType",iTween.EaseType.easeInQuart);
//		scale.Add ("scale",new Vector3(0.5f,0.5f,0.5f));
//		iTween.ScaleTo (this.gameObject,scale);
//	}
//
//	void MoveBegin ()
//	{
//
////		Global.ResourcesDotLoad( EffectTemplate.getEffectTemplateByEffectId(100140).path, 
////		                        ResourceLoadCallbackShadowTemple );
//	}
//	
//	public void ResourceLoadCallbackShadowTemple( ref WWW p_www, string p_path, UnityEngine.Object p_object )
//	{
//		GameObject effectObj = GameObject.Instantiate (p_object) as GameObject;
//		effectObj.transform.parent = this.transform;
//		effectObj.transform.localPosition = Vector3.zero;
//		effectObj.transform.localScale = Vector3.one;
//	}
//
//	void Moveing ()
//	{
////		this.transform.FindChild ("ItemEffectObj").gameObject.SetActive (true);
//		for (int i = 0;i < this.transform.childCount;i ++)
//		{
//			Transform t = this.transform.GetChild (i);
//			t.gameObject.SetActive (true);
//		}
//	}
//
//	void MoveEnd ()
//	{
//		MoneyManager money = moneyObj.GetComponent<MoneyManager> ();
//		money.GetJtOrQt (tanBaoType,1);
//
//		if (tanBaoType == 0 || tanBaoType == 1 || tanBaoType == 11)
//		{
//			SingleReward singleReward = singleObj.GetComponent<SingleReward> ();
//			singleReward.GetItemTransState (true);
//		}
//
//		else if (tanBaoType == 10 || tanBaoType == 12)
//		{
////			Debug.Log ("MoveEnd!");
//			MultipleReward multipReward = multipleObj.GetComponent<MultipleReward> ();
//			multipReward.GetItemNum (1);
//		}
//		
//		Destroy (this.gameObject);
//	}
}
