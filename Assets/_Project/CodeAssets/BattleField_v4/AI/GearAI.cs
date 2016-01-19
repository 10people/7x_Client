using UnityEngine;
using System.Collections;

using qxmobile.protobuf;

public class GearAI : BaseAI
{

	public override void BaseUpdate ()
	{

	}

	public override void die(bool slowDown)
	{
		if (isAlive == false) return;

		//dropItem ();

		mAnim.SetTrigger (getAnimationName(AniType.ANI_Dead));
		
		dieActionDone ();
	}

}
