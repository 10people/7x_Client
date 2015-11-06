using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class HuangYeFog : MonoBehaviour {

//	//public FogInfo mFoinfo;//迷雾信息
//
//	public UILabel JianSheData;
//
//	[HideInInspector]public int JianShezhi;
//
//	public int m_status;
//
//	public int IconNumber;
//
//	public UILabel ShowneedLianmengLv;
//
//	private HuangYeFogTemplete m_HuangYeFogTemplete;
//
//	public GameObject m_FogParent;
//
//	private UISprite[] m_listSpriteFog;
//
//	private Color m_EndColor = Color.white;
//
//	private bool m_isChangColor = false;
//
//	private int m_iCurNum = 0;
//
//	private int m_iMaxNum = 8;
//
//	private float m_fChanger;
//
//	private float m_fChangeg;
//
//	private float m_fChangeb;
//
//	private float m_fChangea;
//
//	[HideInInspector] public bool IsActive = false;
//	void Start () {
//	
//	}
//	
//
//	void Update () 
//	{
//		if(m_isChangColor)
//		{
//			m_iCurNum ++;
//
//			if(m_iCurNum == m_iMaxNum)
//			{
//				m_isChangColor = false;
//				for(int i = 0; i < m_listSpriteFog.Length; i ++)
//				{
//					m_listSpriteFog[i].color = m_EndColor;
//				}
//			}
//			else
//			{
//				Color tempColor = new Color(m_listSpriteFog[0].color.r + m_fChanger, m_listSpriteFog[0].color.g + m_fChangeg, m_listSpriteFog[0].color.b + m_fChangeb, m_listSpriteFog[0].color.a + m_fChangea);
//				for(int i = 0; i < m_listSpriteFog.Length; i ++)
//				{
//					m_listSpriteFog[i].color = tempColor;
//				}
//			}
//		}
//	}
//
//	public void Startinit(HuangYeFogTemplete huangYeFogTemplete, GameObject FogParent)
//	{
//		m_FogParent = FogParent;
//
//		if(FogParent != null)
//		{
//			m_FogParent.SetActive(true);
//			m_listSpriteFog = m_FogParent.GetComponentsInChildren<UISprite>();
//			m_FogParent.SetActive(false);
//		}
//
//
//		BoxCollider mbox = this.gameObject.GetComponent<BoxCollider>();
//
//		JianSheData.gameObject.SetActive(false);
//
//		mbox.enabled = false;
//
//		m_HuangYeFogTemplete = huangYeFogTemplete;
//
//		int LianMengLevel = AllianceData.Instance.g_UnionInfo.level; // 联盟等级
//
////		if((m_HuangYeFogTemplete.needLv - LianMengLevel) == 1)
////		{
////			ShowneedLianmengLv.gameObject.SetActive(true);
////
////			ShowneedLianmengLv.text = "需要联盟等级达到：" +m_HuangYeFogTemplete.needLv.ToString()+"级";
////		}
//	}
//	public void init(OpenHuangYeResp m_Huangye)
//	{
//		m_status = mFoinfo.status;
//		if(mFoinfo.status == 1)
//		{
//
//		}
//		else
//		{
//			if(m_Huangye.treasureBattleMaxLevel >= m_HuangYeFogTemplete.needMaxLv)
//			{
//				m_FogParent.SetActive(true);
//				
//				BoxCollider mbox = this.gameObject.GetComponent<BoxCollider>();
//				
//				mbox.enabled = true;
//				
//				JianSheData.gameObject.SetActive(true);
//				
//				JianSheData.text = JianShezhi.ToString()+ LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_7);
//			}
//		}
//	}
//
//	public void setFogColor(Color end)
//	{
//		m_isChangColor = true;
//		m_EndColor = end;
//		m_iCurNum = 0;
//
//		m_fChanger = ((m_EndColor.r - m_listSpriteFog[0].color.r) / m_iMaxNum);
//		m_fChangeg = ((m_EndColor.g - m_listSpriteFog[0].color.g) / m_iMaxNum);
//		m_fChangeb = ((m_EndColor.b - m_listSpriteFog[0].color.b) / m_iMaxNum);
//		m_fChangea = ((m_EndColor.a - m_listSpriteFog[0].color.a) / m_iMaxNum);
//		Debug.Log(m_listSpriteFog[0].color.a);
//		Debug.Log(m_EndColor.a);
//		Debug.Log(m_fChangea);
//	}
}