using UnityEngine;
using System.Collections;

public class UILabelForLanguage : MonoBehaviour 
{
	public int m_LanguageId;
	// Use this for initialization
	void Start () 
	{
		UILabel tempLabel = gameObject.GetComponent<UILabel>();
		if(tempLabel != null)
		{
			tempLabel.text = LanguageTemplate.GetText(m_LanguageId);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
