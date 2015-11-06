using UnityEngine;
using System.Collections;

public class MainCityZhanliChange : MYNGUIPanel 
{
	private int m_iNum;
	private float m_iCurZhanli;
	private float m_iWantChangeZhanli;
	private float m_iChangeNum;
	public UILabel m_UILabelZhanli;
	public GameObject m_objEff;
	public GameObject m_objEndAnimation;

	public enum ZhanliAnimationSatatae
	{
		Def 	= 1,
		Eff 	= 2,
		Label	= 3,
	}

	public ZhanliAnimationSatatae m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Def;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(m_ZhanliAnimationSatatae)
		{
		case ZhanliAnimationSatatae.Eff:
			m_iNum ++;
			if(m_iNum == 5)
			{
				m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Label;
				m_iWantChangeZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
				m_iCurZhanli = Global.m_iZhanli;
				m_iChangeNum = (m_iWantChangeZhanli - m_iCurZhanli) / 40f;
				m_iNum = 0;
			}
			break;
		case ZhanliAnimationSatatae.Label:
			m_iNum ++;
			m_iCurZhanli += m_iChangeNum;
			if(m_iNum == 40)
			{
				Global.m_iZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
				m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Def;
				Global.m_isZhanli = false;
				m_objEff.SetActive(false);
				m_objEndAnimation.SetActive(false);
				ClientMain.closePopUp();
			}
			else
			{
				Global.m_iZhanli = (int)m_iCurZhanli;
			}
			MainCityUI.m_MainCityUI.m_MainCityUILT.m_ZhanLiLabel.text = Global.m_iZhanli.ToString();
			break;
		}
	}

	public bool setAnimation(string data)
	{
		m_iNum = 0;
		m_objEff.SetActive(true);
		m_objEndAnimation.SetActive(true);

		m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Eff;
		UI3DEffectTool.Instance().ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff, EffectTemplate.getEffectTemplateByEffectId( 100176 ).path);
		return true;
	}

	public override void MYClick(GameObject ui)
	{
//		Global.m_iZhanli = 20000;
		m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Def;
		Global.m_isZhanli = false;
		m_objEff.SetActive(false);
		m_objEndAnimation.SetActive(false);

		Global.m_iZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
		MainCityUI.m_MainCityUI.m_MainCityUILT.m_ZhanLiLabel.text = Global.m_iZhanli.ToString();
		ClientMain.closePopUp();
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
