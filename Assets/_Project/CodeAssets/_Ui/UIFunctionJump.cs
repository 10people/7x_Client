using UnityEngine;
using System.Collections;

public class UIFunctionJump : MYNGUIPanel 
{
	public UILabel m_LabelTile;
	public UILabel m_LabelDes;
	public UISprite[] m_SpriteIcon;
	public UISprite[] m_SpriteIconRed;
	public FunctionLinkTemplate m_FunctionLinkTemplate;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void setDate(int id)
	{
		m_FunctionLinkTemplate = FunctionLinkTemplate.getGroudById(id);
		if(m_FunctionLinkTemplate.tile.IndexOf("[b]") == -1)
		{
			m_FunctionLinkTemplate.tile = "[b]" + m_FunctionLinkTemplate.tile + "[-]";
		}

		m_LabelTile.text = m_FunctionLinkTemplate.tile;
		m_LabelDes.text = m_FunctionLinkTemplate.des;
		int bx = (m_FunctionLinkTemplate.functionID.Length - 1) * -50;
		for(int i = 0; i < 4; i ++)
		{
			if(i < m_FunctionLinkTemplate.functionID.Length)
			{
				m_SpriteIcon[i].gameObject.SetActive(true);
				m_SpriteIcon[i].gameObject.name = "MainCityUIButton_" + i;
				m_SpriteIcon[i].spriteName = "Function_" + m_FunctionLinkTemplate.functionID[i];
				m_SpriteIcon[i].gameObject.transform.localPosition = new Vector3(bx + i * 100, -37, 0);
				m_SpriteIcon[i].SetDimensions(FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[i]).m_iImageW,FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[i]).m_iImageH);
				m_SpriteIconRed[i].gameObject.SetActive(FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[i]).m_show_red_alert);
			}
			else
			{
				m_SpriteIcon[i].gameObject.SetActive(false);
			}
		}
	}

	public override void MYClick(GameObject ui)
	{
		if (ui.name.IndexOf("MainCityUIButton_") != -1)
		{
			int index = int.Parse(ui.name.Substring(17, ui.name.Length - 17));
			Global.m_sMainCityWantOpenPanel = FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[index]).GetParent_menu_id();
			Global.m_sPanelWantRun = m_FunctionLinkTemplate.functionSprite[index];
			Debug.Log(Global.m_sPanelWantRun);
		}
		DoCloseWindow();
	}
	
	void DoCloseWindow()
	{
		MainCityUI.TryRemoveFromObjectList(gameObject);
		TreasureCityUI.TryRemoveFromObjectList(gameObject);
		Destroy(gameObject);
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
	public override void MYoubleClick(GameObject ui)
	{
		
	}
	public override void MYonInput(GameObject ui, string c)
	{
		
	}
	public override void MYondrag(Vector2 delta)
	{
		
	}
}
