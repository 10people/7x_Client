using UnityEngine;
using System.Collections;

public class UIFuli : MYNGUIPanel
{
	public WarItem m_qiandaoItem;
	public WarItem m_chengjiuItem;
	// Use this for initialization
	void Start () 
	{
		m_qiandaoItem.redObj.SetActive(FunctionOpenTemp.GetTemplateById(140).m_show_red_alert);
		Debug.Log(m_qiandaoItem.redObj.activeSelf);
		if(m_qiandaoItem.redObj.activeSelf)
		{
			m_qiandaoItem.numLabel.text = MyColorData.getColorString(4, "今日可签到");
		}
		else
		{
			m_qiandaoItem.numLabel.text = MyColorData.getColorString(2, "今日可签到");
		}
		m_chengjiuItem.redObj.SetActive(FunctionOpenTemp.GetTemplateById(144).m_show_red_alert);
		if(m_chengjiuItem.redObj.activeSelf)
		{
			m_chengjiuItem.numLabel.text = MyColorData.getColorString(4, "可领奖");
		}
		else
		{
			m_chengjiuItem.numLabel.text = MyColorData.getColorString(2, "达成目标即可领奖");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_qiandaoItem.redObj.SetActive(FunctionOpenTemp.GetTemplateById(140).m_show_red_alert);
		m_chengjiuItem.redObj.SetActive(FunctionOpenTemp.GetTemplateById(144).m_show_red_alert);
		if(m_qiandaoItem.redObj.activeSelf)
		{
			m_qiandaoItem.numLabel.text = MyColorData.getColorString(4, "今日可签到");
		}
		else
		{
			m_qiandaoItem.numLabel.text = MyColorData.getColorString(2, "今日可签到");
		}
		if(m_chengjiuItem.redObj.activeSelf)
		{
			m_chengjiuItem.numLabel.text = MyColorData.getColorString(4, "可领奖");
		}
		else
		{
			m_chengjiuItem.numLabel.text = MyColorData.getColorString(2, "达成目标即可领奖");
		}
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Close") != -1)
		{
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);
		}
		else if(ui.name.IndexOf("Item0") != -1)
		{
            UIYindao.m_UIYindao.CloseUI();
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.SIGNAL_LAYER),
			                        MainCityUI.m_MainCityUI.AddUIPanel);

		}
		else if(ui.name.IndexOf("Item1") != -1)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ACTIVITY_LAYER),
			                        MainCityUI.m_MainCityUI.AddUIPanel);
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
