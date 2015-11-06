using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlagGroup : MonoBehaviour 
{
	public int groupId;

	public int maxActive;

	public float delay;


	[HideInInspector] public List<BattleFlag> listFlags = new List<BattleFlag> ();


	private List<BattleFlag> listFadeIn = new List<BattleFlag> ();

	private float timeCount = 0;


	public void FadeIn(BattleFlag bf)
	{
		if(listFadeIn.Contains (bf) == false) listFadeIn.Add (bf);
	}

	private void Update ()
	{
		if (listFadeIn.Count == 0) 
		{
			timeCount = 0;

			return;
		}

		int activeCount = 0;

		foreach(BattleFlag bf in listFlags)
		{
			if(bf.node == null) continue;

			if(bf.node.gameObject.activeSelf == true && bf.node.isAlive == true) activeCount ++; 
		}

		if (activeCount >= maxActive) return;

		timeCount += Time.deltaTime;

		if (timeCount < delay) return;

		timeCount = 0;

		BattleFlag nextFlag = null;

		foreach(BattleFlag bf in listFadeIn)
		{
			if(bf.node == null) continue;

			nextFlag = bf;

			break;
		}

		if (nextFlag == null) return;

		nextFlag.node.fadeIn();

		listFadeIn.Remove (nextFlag);
	}

}
