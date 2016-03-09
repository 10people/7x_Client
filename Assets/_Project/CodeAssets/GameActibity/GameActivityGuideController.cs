using UnityEngine;
using System.Collections;

public class GameActivityGuideController : MonoBehaviour 
{
	public GameObject GuideSprite;
	public GameObject masklayer;
	public GameObject MainController;
	void Start () 
	{

	}


	void Update()
	{
//		if(FreshGuide.Instance().IsActive ((FreshGuide.GuideState)6))
//		{
//			this.gameObject.SetActive(true);
//	
//		  if(MainController.GetComponent<GameActivityManager> ().isGuideMove) 
//		 {
//			MainController.GetComponent<GameActivityManager> ().isGuideMove = false;
//			masklayer.GetComponent<TweenPosition>().enabled = true;
//			GuideSprite.GetComponent<TweenPosition>().enabled = true;
//			EventDelegate.Add(GuideSprite.GetComponent<TweenPosition> ().onFinished,AlphaShow); 
//			//this.gameObject.SetActive (true);	
//		 }
//		}
//		else 
//		{
//			this.gameObject.SetActive(false);
//		}

	}


    void AlphaShow()
	{
		GuideSprite.GetComponent<TweenRotation>().enabled = true;
 	}
}
