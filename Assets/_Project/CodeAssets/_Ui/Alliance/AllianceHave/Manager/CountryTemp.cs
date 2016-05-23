using UnityEngine;
using System.Collections;

public class CountryTemp : MonoBehaviour {

	public int CountryIcon;

	public UISprite m_CountryIcon;

	public UISprite beChoosed;

	public GameObject ButtonName;

	public GameObject myself;

	public UISprite m_CountryIconMyself;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void Init()
	{
		m_CountryIcon.spriteName = "nation_"+CountryIcon.ToString();
		if(myself.activeInHierarchy)
		{
			m_CountryIconMyself.gameObject.SetActive(true);
			m_CountryIconMyself.spriteName = "nation_"+CountryIcon.ToString();
		}
		else
		{
			m_CountryIconMyself.gameObject.SetActive(false);
		}
	}
}
