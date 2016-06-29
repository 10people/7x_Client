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

	private bool m_isCanRequest = true;//可否请求数据

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
			CWBrandItem cwBrand = m_brandList[i].GetComponent<CWBrandItem> ();
			cwBrand.InItBrandItem (tempResp.grandList[i]);
		}

		//Reset scroll view.
		m_sc.UpdateScrollbars(true);
		m_sb.value = m_isRefreshToTop ? 0.0f : 1.0f;
		m_sb.ForceUpdate();
		m_sc.UpdatePosition();

		m_des.text = m_brandList.Count > 0 ? "" : "战报记录为空";

		m_brandObj.transform.parent.GetComponent<ItemTopCol> ().enabled = m_brandList.Count < 7 ? true : false;

		m_sc.enabled = m_brandList.Count < 7 ? false : true;
		m_sb.gameObject.SetActive (m_brandList.Count < 7 ? false : true);

		m_rules.text = m_brandList.Count >= 7 ? QXComData.yellow + LanguageTemplate.GetText (LanguageTemplate.Text.JUN_CHENG_ZHAN_9) + "[-]" : "";
	}

	void Update ()
	{
		float temp = m_sc.GetSingleScrollViewValue();
		
		if (temp == -100) return;
		
		//Reset can slide request.
		if (temp > -0.1f && temp < 1.1f)
		{
			m_isCanRequest = true;
		}
		
		if (!m_isCanRequest) return;

		if (CityWarData.Instance.M_BrandPage == 1)
		{
			if (m_brandList.Count >= 20)
			{
				if (temp > 1.25f)
				{
					m_isRefreshToTop = true;
					
					CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage + 1);
					
					m_isCanRequest = false;
					return;
				}
			}
		}
		else if (CityWarData.Instance.M_BrandPage > 1)
		{
			if (m_brandList.Count >= 20)
			{
				if (temp > 1.25f)
				{
					m_isRefreshToTop = true;
					
					CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage + 1);
					
					m_isCanRequest = false;
					return;
				}
				
				if (temp < -0.25f)
				{
					m_isRefreshToTop = false;
					
					CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage - 1);
					
					m_isCanRequest = false;
					return;
				}
			}
			else
			{
				if (temp < -0.25f)
				{
					m_isRefreshToTop = false;
					
					CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage - 1);
					
					m_isCanRequest = false;
					return;
				}
			}
		}

//		if (temp > 1.25f)
//		{
//			m_isRefreshToTop = true;
//
//			CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage + 1);
//
//			m_isCanRequest = false;
//			return;
//		}
//		if (temp < -0.25f)
//		{
//			m_isRefreshToTop = false;
//			
//			CityWarData.Instance.BrandReq (CityWarData.Instance.M_BrandPage - 1);
//
//			m_isCanRequest = false;
//			return;
//		}
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
