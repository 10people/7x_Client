using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardButon : MonoBehaviour {

	public delegate void CardClick (GameObject go);

	public CardClick cardClick;

	private int cardId;

	public int CardId
	{
		get{return cardId;}
		set{cardId = value;}
	}

	public void GetCardClick (CardClick tempClick)
	{
		cardClick = tempClick;

		this.gameObject.GetComponent<EventHandler> ().m_handler += OnCardClick;
	}

	void OnCardClick (GameObject obj)
	{
		if (cardClick != null)
		{
			cardClick (obj);
		}
	}
}
