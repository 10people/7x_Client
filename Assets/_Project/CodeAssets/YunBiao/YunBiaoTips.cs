using UnityEngine;
using System.Collections;

public class YunBiaoTips : MonoBehaviour {

	public UILabel label1;
	
	public UILabel label2;

	private bool isAlpha;
	public bool SetAlpha
	{
		set{isAlpha = value;}
	}

	public enum ShowType
	{
		help,
		horse,
		daoJu
	}

	public void SetTips (string str,ShowType type)
	{
		switch (type)
		{
		case ShowType.help://协助者

			label1.gameObject.SetActive (true);
			label2.gameObject.SetActive (false);

			label1.text = str;

			break;

		case ShowType.horse://马

			label1.gameObject.SetActive (false);
			label2.gameObject.SetActive (true);
		
			label2.text = str;

			break;

		case ShowType.daoJu://道具

			break;
		}
	}

	void Update ()
	{
		if (isAlpha)
		{
			this.GetComponent<UIWidget> ().alpha = 1;
		}
		else
		{
			this.GetComponent<UIWidget> ().alpha -= 0.05f;
			if (this.GetComponent<UIWidget> ().alpha <= 0)
			{
				this.GetComponent<UIWidget> ().alpha = 0;
			}
		}
	}
}
