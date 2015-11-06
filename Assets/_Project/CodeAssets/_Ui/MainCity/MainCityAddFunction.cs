using UnityEngine;
using System.Collections;

public class MainCityAddFunction : MYNGUIPanel 
{
	private int m_iNum;
	public int m_id;
	
	public enum AddFunctionSatatae
	{
		Def		= 0,
		Eff 	= 1,
		Click	= 2,
	}

	private AddFunctionSatatae m_AddFunctionSatatae = AddFunctionSatatae.Eff;

	public UILabel m_UILabel;
	public UILabel m_UILabelDes;
	public GameObject m_objEff;
	public GameObject m_objEff1;
	public GameObject m_objThis;
	public UISprite m_UISpriteButton;

	// Use this for initialization
	void Start ()
	{

	}

	public void set(string data)
	{
		m_id = int.Parse(data);
		m_iNum = 0;
		m_AddFunctionSatatae = AddFunctionSatatae.Eff;
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff, EffectTemplate.getEffectTemplateByEffectId( 100178 ).path);
		string spriteName = m_id + "";
//		if(spriteName.IndexOf("Circle") != -1)
//		{
			UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff1, EffectTemplate.getEffectTemplateByEffectId( 100107 ).path);
//		}
//		else
//		{
//			UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.MainUI_0, m_objEff1, EffectTemplate.getEffectTemplateByEffectId( 100106 ).path);
//		}
		m_UISpriteButton.spriteName = m_id + "";

//		Debug.Log(FunctionUnlock.getGroudById(m_id));
//		Debug.Log(FunctionUnlock.getGroudById(m_id).desID);
//		Debug.Log(DescIdTemplate.GetDescriptionById(FunctionUnlock.getGroudById(m_id).desID));

		m_UILabelDes.text = DescIdTemplate.GetDescriptionById(FunctionUnlock.getGroudById(m_id).desID);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(m_AddFunctionSatatae)
		{
		case AddFunctionSatatae.Eff:
			m_iNum ++;
			if(m_iNum <= 15)
			{
				m_UISpriteButton.gameObject.transform.localScale = new Vector3(m_iNum * 0.1f, m_iNum * 0.1f, m_iNum * 0.1f);
			}
			else if(m_iNum <= 20)
			{
				m_UISpriteButton.gameObject.transform.localScale = new Vector3( 1 + (20 - m_iNum) * 0.1f, 1 + (20 - m_iNum) * 0.1f, 1 + (20 - m_iNum) * 0.1f);
			}
			else
			{
				m_AddFunctionSatatae = AddFunctionSatatae.Click;
			}
			break;
		case AddFunctionSatatae.Def:
			break;
		}
	}


	public override void MYClick(GameObject ui)
	{
		if(m_AddFunctionSatatae == AddFunctionSatatae.Click)
		{
			MainCityUI.TryRemoveFromObjectList(MainCityUI.m_MainCityUI.m_AddFunction);
			Destroy(m_objThis);
			ClientMain.closePopUp();
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

}