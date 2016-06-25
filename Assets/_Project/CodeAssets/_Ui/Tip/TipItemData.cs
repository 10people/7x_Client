using UnityEngine;
using System.Collections;

//	@author LiuChang
//
//	TipItemData tipItemData;
//
//	tipItemData = TipItemData.createTipItemData ();
//
//	tipItemData = TipItemData.createTipItemData ().setTouchPosition(TipItemData.ScreenPosition.LEFT);
//
//	tipItemData = TipItemData.createTipItemData (TipItemData.ScreenPosition.LEFT);

public class TipItemData : ScriptableObject 
{
	public enum ScreenPosition
	{
		DEFAULT,
		LEFT,
		RIGHT,
	}

	private ScreenPosition t_touchedPositon; 

	public ScreenPosition touchedPositon
	{
		get
		{
			return t_touchedPositon;
		}
	}

	private int t_exp;

	public int exp
	{
		get
		{
			return t_exp;
		}
	}


	private TipItemData()
	{
		t_touchedPositon = ScreenPosition.DEFAULT;
	}

	public static TipItemData createTipItemData()
	{
		return new TipItemData ();
	}

	public static TipItemData createTipItemData(ScreenPosition _touchedPositon)
	{
		return createTipItemData ().setTouchPosition (_touchedPositon);
	}

	public static TipItemData createTipItemData(int _exp)
	{
		return createTipItemData ().setExp (_exp);
	}

	public static TipItemData createTipItemData(ScreenPosition _touchedPositon, int _exp)
	{
		return createTipItemData ().setTouchPosition (_touchedPositon).setExp (_exp);
	}

	public TipItemData setTouchPosition(ScreenPosition _touchedPositon)
	{
		t_touchedPositon = _touchedPositon;

		return this;
	}

	public TipItemData setExp(int _exp)
	{
		t_exp = _exp;

		return this;
	}

}
