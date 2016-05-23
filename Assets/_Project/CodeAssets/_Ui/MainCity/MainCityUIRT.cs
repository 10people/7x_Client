using UnityEngine;
using System.Collections;

/// <summary>
/// UI controller in main city ui right top.
/// </summary>
public class MainCityUIRT : MYNGUIPanel
{
    public GameObject PopupObject;

    public GameObject PopupBGObject;

	public UILabel m_MoneyNum;
	public UILabel m_yuanbaoNuM;
	public UILabel m_energyNuM;

	public GameObject m_TiliMaxTishi;
	public float m_Time;
    /// <summary>
    /// Task pop up label info
    /// </summary>
    public UILabel PopupLabel;

	void Start()
	{
//		MainCityUI.m_MainCityUI.setGlobalBelongings(gameObject, 0, 0);
	}

	void Update()
	{
		if(JunZhuData.Instance().m_junzhuInfo.tili >= JunZhuData.Instance().m_junzhuInfo.tiLiMax)
		{
			float tempTime = Time.time - m_Time;
			if(m_TiliMaxTishi.activeSelf)
			{
				if(tempTime > 10)
				{
					m_TiliMaxTishi.SetActive(false);
				}
			}
			else
			{
				if(tempTime > 120)
				{
					m_TiliMaxTishi.SetActive(false);
				}
			}
		}
	}

	public void RefreshJunZhuInfo()
	{
		//money info
		m_MoneyNum.text = JunZhuData.Instance().m_junzhuInfo.jinBi.ToString();
		if (JunZhuData.Instance().m_junzhuInfo.jinBi > 100000000)
		{
			m_MoneyNum.text = JunZhuData.Instance().m_junzhuInfo.jinBi / 10000 + "万";
		}
		
		//ingot info
		m_yuanbaoNuM.text = JunZhuData.Instance().m_junzhuInfo.yuanBao.ToString();
		if (JunZhuData.Instance().m_junzhuInfo.yuanBao > 100000000)
		{
			m_yuanbaoNuM.text = (JunZhuData.Instance().m_junzhuInfo.yuanBao / 10000) + "万";
		}
		
		//energy info
		string energyText = JunZhuData.Instance().m_junzhuInfo.tili > 10000 ? JunZhuData.Instance().m_junzhuInfo.tili / 10000 + "万" : JunZhuData.Instance().m_junzhuInfo.tili.ToString();
		string energyMaxText = JunZhuData.Instance().m_junzhuInfo.tiLiMax > 10000 ? JunZhuData.Instance().m_junzhuInfo.tiLiMax / 10000 + "万" : JunZhuData.Instance().m_junzhuInfo.tiLiMax.ToString();
		if(int.Parse(energyText) >= int.Parse(energyMaxText) && JunZhuData.Instance().m_junzhuInfo.level > Global.TILILVMAX)
		{
			m_energyNuM.text = MyColorData.getColorString(18, energyText + "/" + energyMaxText);
			m_TiliMaxTishi.SetActive(true);
			m_Time = Time.time;
		}
		else
		{
			m_energyNuM.text = energyText + "/" + energyMaxText;
			m_TiliMaxTishi.SetActive(false);
			m_Time = Time.time;
		}
	}

    public static void OutterShowPopupDetail(string text, float duration)
    {
        MainCityUI.m_MainCityUI.m_MainCityUIRT.ShowPopupDetail(text, duration);
    }

    private void ShowPopupDetail(string text, float duration)
    {
        StartCoroutine(DoShowPopupDetail(text, duration));
    }

    private IEnumerator DoShowPopupDetail(string text, float duration)
    {
        PopupObject.SetActive(false);

        PopupLabel.text = text;

        PopupObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        PopupObject.SetActive(false);
    }

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("RT_BuyMoney") != -1)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(false, true, false);
		}
		else if(ui.name.IndexOf("RT_BuyRecharge") != -1)
		{
			Global.CreateFunctionIcon(101);
		}
		else if(ui.name.IndexOf("RT_BuyEnergy") != -1)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true, false, false);
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
