using UnityEngine;
using System.Collections;

public class FlyBirdController : MonoBehaviour {

	public static FlyBirdController birdController;

	public enum BirdState
	{
		LAND,
		STAND,
		FLY,
	}

	public Animator birdAnimator;

	void Awake ()
	{
		birdController = this;
	}

	/// <summary>
	/// Sets the state of the bird.
	/// </summary>
	/// <param name="tempState">Temp state.</param>
	public void SetBirdState (BirdState tempState)
	{
		switch (tempState)
		{
		case BirdState.LAND:

			birdAnimator.SetBool ("Land",true);

			break;
		case BirdState.STAND:
			break;
		case BirdState.FLY:

			birdAnimator.SetBool ("Fly",true);

			break;
		default:
			break;
		}
	}

	public void EnterCity ()
	{
//		Debug.Log ("EnterCity");
		EnterGame.enterGame.EnterGameWaitForAnimationEnd ();
	}

	void OnDestroy ()
	{
		birdController = null;
	}
}
