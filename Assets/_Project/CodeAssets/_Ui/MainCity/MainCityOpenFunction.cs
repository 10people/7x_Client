using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainCityOpenFunction : MonoBehaviour 
{
	public UISprite m_spriteIcon;
	public UILabel m_labelDes;

	public void setData(int id)
	{

	}

	public void upShow()
	{
		gameObject.SetActive(false);
		for(int i = 0; i < FunctionUnlock.templates.Count; i ++)
		{
			if(!FunctionOpenTemp.IsHaveID(FunctionUnlock.templates[i].id))
			{
				gameObject.SetActive(true);
				m_spriteIcon.spriteName = "Function_" + FunctionUnlock.templates[i].id;
				m_labelDes.text = FunctionUnlock.templates[i].des2;
				break;
			}
		}
	}
}
