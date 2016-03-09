using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainCityOpenFunction : MonoBehaviour 
{
	public UISprite m_spriteIcon;
	public UILabel m_labelDes;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setData(int id)
	{

	}

	public void upShow()
	{
		var temp = FunctionUnlock.templates.Where(item => !FunctionOpenTemp.m_EnableFuncIDList.Contains(item.id));

//		Debug.Log(temp.First().id);

		if (temp.Any())
		{
			gameObject.SetActive(true);
			m_spriteIcon.spriteName = "Function_" + temp.First().id;
			m_labelDes.text = temp.First().des2;
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
