using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CityWarBrand : GeneralInstance<CityWarBrand> {

	public delegate void CityWarBrandDelegate ();
	public CityWarBrandDelegate M_CityWarBrandDelegate;

	public UIScrollView m_sc;
	public UIScrollBar m_sb;

	public UILabel m_des;
	public UILabel m_rules;

	public GameObject m_brandObj;
	private List<GameObject> m_brandList = new List<GameObject> ();

	private bool m_canTap = false;
	private bool m_isRefreshToTop = true;

	new void Awake ()
	{
		base.Awake ();
	}

	public void InItBrandPage (CityWarGrandResp tempResp)
	{
		m_canTap = true;
		if (tempResp.grandList.Count == 0)
		{
			if (CityWarData.Instance.M_BrandPage != 1)
			{
				return;
			}
		}
		m_brandList = QXComData.CreateGameObjectList (m_brandObj,tempResp.grandList.Count,m_brandList);

		for (int i = 0;i < m_brandList.Count;i ++)
		{
			m_brandList[i].transform.localPosition = new Vector3(0,-49 * i,0);
			m_sc.UpdateScrollbars (true);
			m_sb.value = m_isRefreshToTop ? 0 : 1;
			CWBrandItem cwBrand = m_brandList[i].GetComponent<CWBrandItem> ();
			cwBrand.InItBrandItem (tempResp.grandList[i]);
		}

		m_des.text = m_brandList.Count > 0 ? "" : ("战报记录为空");

		m_sc.enabled = m_brandList.Count < 8 ? false : true;
		m_sb.gameObject.SetActive (m_brandList.Count < 8 ? false : true);

		m_rules.text = QXComData.yellow + "历史战报最多保存" + 50 + "条[-]";
	}

	void Update ()
	{
		float temp = m_sc.GetSingleScrollViewValue();
		if (temp != -100) 
		{
			if (CityWarData.Instance.M_BrandPage == 1)
			{
				if (m_brandList.Count >= 20)
				{
					if (temp > 1.25f && m_canTap)
					{
						m_isRefreshToTop = true;
						m_canTap = false;
						CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage + 1);
					}
				}
			}
			else if (CityWarData.Instance.M_BrandPage > 1)
			{
				if (m_brandList.Count >= 20)
				{
					if (temp > 1.25f && m_canTap)
					{
						m_isRefreshToTop = true;
						m_canTap = false;
						CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage + 1);
					}
					else if (temp < -0.25f && m_canTap)
					{
						m_isRefreshToTop = false;
						m_canTap = false;
						CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage - 1);
					}
				}
				else
				{
					if ((m_brandList.Count < 5 ? temp > 1.0f : temp < -0.25f) && m_canTap)
					{
						m_isRefreshToTop = false;
						m_canTap = false;
						CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage - 1);
					}
				}
			}
		}
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ZheZhao":

			if (M_CityWarBrandDelegate != null)
			{
				M_CityWarBrandDelegate ();
			}

			break;
		default:
			break;
		}
	}

	new void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
