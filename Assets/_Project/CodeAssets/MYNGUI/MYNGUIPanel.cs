using UnityEngine;
using System;
using System.Collections;

public abstract class MYNGUIPanel : MonoBehaviour
{
//	public int iUiIndexPanel = -1;
//	public bool m_isUp = false;
//	public bool m_isOpen = false;
	public float m_TimeP = 0;
	public abstract void MYClick(GameObject ui);
	public abstract void MYMouseOver(GameObject ui);
	public abstract void MYMouseOut(GameObject ui);
	public abstract void MYPress(bool isPress, GameObject ui);
	public abstract void MYelease(GameObject ui);
	public abstract void MYoubleClick(GameObject ui);
	public abstract void MYonInput(GameObject ui, string c);
	public abstract void MYondrag(Vector2 delta);
//	public abstract void setActive(bool isActive);
//	public abstract void upData();
//	public abstract void uiRemove();
//	public abstract void uiDelete();




//	public override void MYClick(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYMouseOver(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYMouseOut(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYPress(bool isPress, GameObject ui)
//	{
//		
//	}
//	
//	public override void MYelease(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYondrag(Vector2 delta)
//	{
//		
//	}
//	
//	public override void MYoubleClick(GameObject ui)
//	{
//		
//	}
//	
//	public override void MYonInput(GameObject ui, string c)
//	{
//		
//	}

}