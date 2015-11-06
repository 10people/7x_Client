using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireBtn : MonoBehaviour {
	public GameObject supObj;
	public GameObject nextObj;

	public static FireBtn m_FireBtn;
	UILabel m_lable;
	UISprite m_Sprite;


	void OnClick()
	{
	
		if(UIYindao.m_UIYindao.m_isOpenYindao)
		{
			UIYindao.m_UIYindao.CloseUI();
			
		}
		    supObj.SetActive (false);
	    	nextObj.SetActive (true);

	    	
	}




}
