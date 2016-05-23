using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXSetChannel : GeneralInstance<QXSetChannel> {

	public List<GameObject> m_btnList = new List<GameObject>();

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItSetChannel ()
	{

	}
	
	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "WorldBtn":
			break;
		case "AllianceBtn":
			break;
		case "PrivateBtn":
			break;
		case "WifiBtn":
			break;
		case "ZheZhao":
			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
