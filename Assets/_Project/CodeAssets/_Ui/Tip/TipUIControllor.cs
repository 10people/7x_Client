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

	public TipJewelControllor jewelControllor;

	// fix bug in some device
	public UIEventListener m_close_listener = null;


	void Awake()
	{
		m_close_listener.onPress = OnNGUIPressed;
	}

	public void OnNGUIPressed( GameObject go, bool state )
	{
		Debug.Log( "TipUI.SpriteBg.OnNGUIPressed( " + go + ", " + state + " )" );

		if( !state )
		{
			ShowTip.close();

//			gameObject.SetActive( false );
//
//			Destroy( gameObject );
		}
	}

	public void refreshDataItem(int commonItemId)
	{
		closeAll ();

		CommonItemTemplate template = CommonItemTemplate.getCommonItemTemplateById (commonItemId);

		if(template.itemType == 2 || template.itemType == 7 || template.itemType == 8)//装备，宝石，符文
		{
			jewelControllor.gameObject.SetActive (true);
			
			jewelControllor.refreshData (commonItemId);
		}
		else
		{
			itemControllor.gameObject.SetActive (true);

			itemControllor.refreshData (commonItemId);
		}
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

		jewelControllor.gameObject.SetActive (false);
	}

	void Update()
	{
		if(UICamera.touchCount <= 0)
		{
			ShowTip.close();

			return;
		}
	}

}
