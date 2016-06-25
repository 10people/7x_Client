using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UITiliMax : MYNGUIPanel
{
	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Button") != -1)
		{
			int index = int.Parse(ui.name.Substring(6, 1));
			switch(index)
			{
			case 1:
				Global.m_sMainCityWantOpenPanel = 300000;
				break;
			case 2:
				Global.m_sMainCityWantOpenPanel = 200;
				break;
			case 3:
				Global.m_sMainCityWantOpenPanel = 6;
				break;
			}
		}
		DoCloseWindow();
	}

	void DoCloseWindow()
	{
		MainCityUI.TryRemoveFromObjectList(gameObject);
		TreasureCityUI.TryRemoveFromObjectList(gameObject);
		Destroy(gameObject);
	}
	
	public override void MYMouseOver(GameObject ui)
	{
		
	}
	
	public override void MYMouseOut(GameObject ui)
	{
		
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
		
	}
	
	public override void MYelease(GameObject ui)
	{
		
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
}