﻿using UnityEngine;
using System.Collections;

public class BattleDroppenLayer : MonoBehaviour 
{
	public UILabel labelNum;


	private int num;


	public void init()
	{
		num = 0;

		labelNum.text = num + "";
	}

	public void addItem(int itemCount = 1)
	{
		num += itemCount;

		labelNum.text = num + "";
	}

}
