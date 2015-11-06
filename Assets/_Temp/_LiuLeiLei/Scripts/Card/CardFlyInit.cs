using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardFlyInit : MonoBehaviour {
	
	public GameObject cardBtn;
	private int cardNum;

	void Start ()
	{
		cardNum = 10;
		InItCard ();
	}

	void InItCard ()
	{
		StartCoroutine (FlyOneByOne ());
	}

	IEnumerator FlyOneByOne ()
	{
		List<Vector3> posList = new List<Vector3> ();

		for (int i = 0;i < cardNum;i ++)
		{
			Vector3 t;
			if (i < (cardNum/2))
			{
				t = new Vector3(-300 + i* 150,100,0); 
			}
			else
			{
				t = new Vector3(-300 + (i - 5) * 150,-100,0);
			}

			posList.Add (t);
		}

		List<GameObject> objList = new List<GameObject> ();

		for (int i = 0;i < cardNum;i ++)
		{
			GameObject card = (GameObject)Instantiate (cardBtn);

			card.SetActive (true);

			card.transform.parent = cardBtn.transform.parent;
			card.transform.localPosition = posList[i];
			card.transform.localScale = Vector3.one;

			objList.Add (card);

			CardButon cardButton = objList[i].GetComponent<CardButon> ();
			cardButton.CardId = i;
			cardButton.GetCardClick (CardClickBack);

			yield return new WaitForSeconds (0.1f);
		}
	}

	void CardClickBack (GameObject go)
	{
		CardButon cardButton = go.GetComponent<CardButon> ();
		if (cardButton.CardId == 5 || cardButton.CardId == 7)
		{
			Debug.Log ("cao!");
		}
	}
}
