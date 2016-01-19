using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HeroAI : BaseAI
{
	public override void Start()
	{
		base.Start();

		updataAttackRange();
	}

	public override void OnDestroy(){
		base.OnDestroy();
	}
}
