using UnityEngine;
using System.Collections;

public class UIFunctionJump : MYNGUIPanel 
{
	public UILabel m_LabelTile;
	public UILabel m_LabelDes;
	public UISprite[] m_SpriteIcon;
	public UISprite[] m_SpriteIconRed;
	public FunctionLinkTemplate m_FunctionLinkTemplate;

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
			if(!FunctionOpenTemp.IsHaveID(m_FunctionLinkTemplate.functionID[index]))
			{
				ClientMain.m_UITextManager.createText(FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[index]).m_sNotOpenTips);
			}
			else
			{
				if((Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_MAINCITY_YEWAN || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY || Application.loadedLevelName == ConstInGame.CONST_SCENE_NAME_ALLIANCECITY_YEWAN))
				{
					if(FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[index]).GetParent_menu_id() == 1420)
					{
						Global.m_sMainCityWantOpenPanel = m_FunctionLinkTemplate.functionID[index];
						Global.m_sPanelWantRun = m_FunctionLinkTemplate.functionSprite[index];
						if(Global.m_sPanelWantRun == "null")
						{
							Global.m_sPanelWantRun = null;
						}
					}
					else
					{
						Global.m_sMainCityWantOpenPanel = FunctionOpenTemp.GetTemplateById(m_FunctionLinkTemplate.functionID[index]).GetParent_menu_id();
						Global.m_sPanelWantRun = m_FunctionLinkTemplate.functionSprite[index];
						if(Global.m_sPanelWantRun == "null")
						{
							Global.m_sPanelWantRun = null;
						}
					}
				}
				else
				{
					if(m_FunctionLinkTemplate.functionID[index] == 1300)
					{
						RechargeData.Instance.RechargeDataReq ();
					}
					else
					{
						ClientMain.m_UITextManager.createText("返回主城后才可进入本功能");
					}
				}
			}
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
