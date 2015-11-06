using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComCardFly : MonoBehaviour {

	public GameObject cardObj;

	private int cardNum = 10;

	private List<Vector3> posList = new List<Vector3> ();

	private List<GameObject> cardList = new List<GameObject>();

	void Start ()
	{
		CreatePos ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Q))
		{
			if (cardList.Count < cardNum)
			{
				CreateCard ();
			}
		}
	}

	void CreatePos ()
	{
		for (int i = 0;i < cardNum;i ++)
		{
			Vector3 t;
			if (i < (cardNum/2))
			{
				t = new Vector3(-275 + i* 130,100,0); 
			}
			else
			{
				t = new Vector3(-275+ (i - 5) * 130,-100,0);
			}
//			Debug.Log (t);
			posList.Add (t);
		}
	}

	void CreateCard ()
	{
		GameObject card = (GameObject)Instantiate (cardObj);
		card.SetActive (true);
		card.transform.parent = this.transform;
		card.transform.localPosition = cardObj.transform.localPosition;
		card.transform.localScale = Vector3.zero;

		Vector3 pos = posList[cardList.Count];
		Hashtable move = new Hashtable ();
		move.Add ("position",pos);
		move.Add ("time",1);
		move.Add ("islocal",true);
		iTween.MoveTo (card,move);

		Hashtable scale = new Hashtable ();
		scale.Add ("scale",Vector3.one);
		scale.Add ("easetype",iTween.EaseType.easeOutQuart);
		scale.Add ("time",1);
		iTween.ScaleTo (card,scale);

		iTween.RotateTo (card,Vector3.zero,1);

		cardList.Add (card);
	}
}
