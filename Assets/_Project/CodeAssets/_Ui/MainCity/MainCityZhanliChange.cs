using UnityEngine;
using System.Collections;

public class MainCityZhanliChange : MYNGUIPanel 
{
	public int m_iNum;
	public float m_iCurZhanli;
	public float m_iChangeNum;
	public int m_iWantToZhanli;
	public UILabel m_UILabelHeroZhanli;
	public UILabel m_UILabelUpZhanli;
	public GameObject m_objEff;

	public enum ZhanliAnimationSatatae
	{
		Def 	= 1,
		Eff 	= 2,
		Label	= 3,
	}

	public ZhanliAnimationSatatae m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Def;

	// Use this for initialization
	void Start () 
	{
		m_UILabelHeroZhanli.text = Global.m_iPZhanli.ToString();
		m_UILabelUpZhanli.text = Global.m_iAddZhanli + " ↑";

		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_objEff, EffectTemplate.getEffectTemplateByEffectId( 100191 ).path);
		m_iCurZhanli = Global.m_iPChangeZhanli;
		m_iChangeNum = (JunZhuData.Instance().m_junzhuInfo.zhanLi - m_iCurZhanli) / 20f;

		m_iWantToZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(m_ZhanliAnimationSatatae)
		{
		case ZhanliAnimationSatatae.Def:
			m_iNum ++;
			if(m_iNum == 10)
			{
				m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Label;
				m_iNum = 0;
			}
			break;
		case ZhanliAnimationSatatae.Eff:
			if(m_iWantToZhanli < JunZhuData.Instance().m_junzhuInfo.zhanLi)
			{
				m_UILabelUpZhanli.text = Global.m_iAddZhanli + " ↑";
				m_iChangeNum = (JunZhuData.Instance().m_junzhuInfo.zhanLi - m_iCurZhanli) / 20f;
				m_iWantToZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
				m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Label;
				m_iNum = 0;
			}
			m_iNum ++;
			if(m_iNum == 30)
			{
//				Debug.Log("===========1");
				ClientMain.closePopUp();
				Global.m_isZhanli = false;
				Global.m_iAddZhanli = 0;
				GameObject.Destroy(gameObject);
				Global.m_iPZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
			}

			break;
		case ZhanliAnimationSatatae.Label:
			if(m_iWantToZhanli < JunZhuData.Instance().m_junzhuInfo.zhanLi)
			{
				m_UILabelUpZhanli.text = Global.m_iAddZhanli + " ↑";
				m_iChangeNum = (JunZhuData.Instance().m_junzhuInfo.zhanLi - m_iCurZhanli) / 20f;
				m_iWantToZhanli = JunZhuData.Instance().m_junzhuInfo.zhanLi;
				m_iNum = 0;
			}
			m_iNum ++;
			m_iCurZhanli += m_iChangeNum;

			m_UILabelHeroZhanli.text = (int)m_iCurZhanli + "";
			if(m_iNum == 20)
			{
				m_iNum = 0;
				m_ZhanliAnimationSatatae = ZhanliAnimationSatatae.Eff;
				m_UILabelHeroZhanli.text = JunZhuData.Instance().m_junzhuInfo.zhanLi.ToString();
			}
			break;
		}
	}

	public override void MYClick(GameObject ui)
	{

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
