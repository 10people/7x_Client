using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TipUIControllor : MonoBehaviour
{
	public TipItemControllor itemControllor;

	public TipEnemyControllor enemyContorllor;


	public void refreshDataItem(int commonItemId)
	{
		closeAll ();

		itemControllor.gameObject.SetActive (true);

		itemControllor.refreshData (commonItemId);
	}

	public void refreshDataEnemy(string iconName, string enemyName, string enemyDesc)
	{
		closeAll ();

		enemyContorllor.gameObject.SetActive (true);

		enemyContorllor.refreshData (iconName, enemyName, enemyDesc);
	}

	private void closeAll()
	{
		itemControllor.gameObject.SetActive (false);

		enemyContorllor.gameObject.SetActive (false);
	}

	void Update()
	{
		if(UICamera.touchCount == 0)
		{
			ShowTip.close();

			return;
		}
	}

}
