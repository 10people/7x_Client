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
	public FunctionButtonManager m_FunctionButtonManager;
	private FunctionOpenTemp m_FunctionOpenTemp;

	// Use this for initialization
	void Start ()
	{

	}

	public void set(string data)
	{
		m_id = int.Parse(data);
		m_FunctionOpenTemp = FunctionOpenTemp.GetTemplateById(m_id);
		m_FunctionButtonManager.SetData(m_FunctionOpenTemp);
		m_iNum = 0;
		m_AddFunctionSatatae = AddFunctionSatatae.Eff;
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff, EffectTemplate.getEffectTemplateByEffectId( 100178 ).path);
		string spriteName = m_id + "";

		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff1, EffectTemplate.getEffectTemplateByEffectId( 100107 ).path);

		m_UILabelDes.text = m_FunctionOpenTemp.Des;
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
				m_FunctionButtonManager.gameObject.transform.localScale = new Vector3(m_iNum * 0.1f, m_iNum * 0.1f, m_iNum * 0.1f);
			}
			else if(m_iNum <= 20)
			{
				m_FunctionButtonManager.gameObject.transform.localScale = new Vector3( 1 + (20 - m_iNum) * 0.1f, 1 + (20 - m_iNum) * 0.1f, 1 + (20 - m_iNum) * 0.1f);
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
			MainCityUI.m_MainCityUI.AddButton(m_FunctionButtonManager, m_FunctionOpenTemp);
			Destroy(m_objThis);
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