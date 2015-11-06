using UnityEngine;
using System.Collections;

public class ExchangeChat : MonoBehaviour {

	public UILabel chatLabel;

	void Start () 
	{
		ShowChat (0);
	}

	public void ShowChat (int type) 
	{
		switch (type)
		{
		case 0:

			chatLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.BAI_ZHAN_9);

			break;

		case 1:

			chatLabel.text = LanguageTemplate.GetText (LanguageTemplate.Text.BAI_ZHAN_13);

			break;

		default:break;
		}

		StartCoroutine (DisChatBox ());
	}

	IEnumerator DisChatBox ()
	{
		yield return new WaitForSeconds (5f);
		this.gameObject.SetActive (false);
	}
}
