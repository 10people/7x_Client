using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCityWantOpenManager : MYNGUIPanel {
	public MainCityWantOpenData m_MainCityWantOpenData;
	public List<MainCityWantOpenData> m_listData = new List<MainCityWantOpenData>();
	public ScaleEffectController m_ScaleEffectController;
	// Use this for initialization
	void Start () 
	{
		int y = 186;
		for(int i = 0; i < FunctionUnlock.templates.Count; i ++)
		{
			if(!FunctionOpenTemp.IsHaveID(FunctionUnlock.templates[i].id))
			{
				MainCityWantOpenData temp = GameObject.Instantiate(m_MainCityWantOpenData.gameObject).GetComponent<MainCityWantOpenData>();
				temp.transform.parent = m_MainCityWantOpenData.transform.parent;
				temp.transform.localPosition = new Vector3(0, y, 0);
				temp.m_Des1.text = FunctionUnlock.templates[i].des1;
				temp.m_Des2.text = FunctionUnlock.templates[i].des2;
				temp.m_Icon.spriteName = FunctionUnlock.templates[i].id + "";
				temp.transform.localScale = Vector3.one;
				y -= 110;
			}
		}
		m_MainCityWantOpenData.gameObject.SetActive(false);

		m_ScaleEffectController.OpenCompleteDelegate += scaleOver;
		MainCityUI.TryAddToObjectList(gameObject);
		CityGlobalData.m_isRightGuide = true;
	}

	public void scaleOver()
	{

	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Close") != -1)
		{
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);
		}
	}

	public override void MYMouseOver(GameObject ui)
	{
	}
	
	public override void MYMouseOut(GameObject ui)
	{
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
	}
	
	public override void MYelease(GameObject ui)
	{
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
