using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GeneralVip : GeneralInstance<GeneralVip> {

	void Awake ()
	{
		base.Awake ();
	}

	public delegate void ShowVipDelegate ();
	private ShowVipDelegate M_ShowVipDelegate;

	private GameObject M_CameraObj;

	public GameObject m_showVipObj;
	public UISprite m_vipSprite1;
	public UISprite m_vipSprite2;
	public GameObject  m_effect;
	public UILabel m_vDesLabel;
	public List<Animator> m_vipAnimaList  = new List<Animator>();
	
	public void ShowVip (int m_curVip,GameObject tempCamera = null,ShowVipDelegate tempDelegate = null)
	{
		GeneralRewardManager.Instance ().M_OtherExit = true;
		M_ShowVipDelegate = tempDelegate;
		M_CameraObj = tempCamera;
		if (M_CameraObj != null)
		{
			EffectTool.SetUIBackgroundEffect (M_CameraObj,true);
		}

		m_showVipObj.SetActive (true);
		m_vipSprite1.spriteName = "0_" + m_curVip;
		m_vipSprite2.spriteName = "0_" + m_curVip;
		
		for (int i = 0;i < m_vipAnimaList.Count;i ++)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(226 + i), VipAnimationLoadBack);
		}
		
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_effect, EffectIdTemplate.GetPathByeffectId(100108), null);
		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_effect, EffectIdTemplate.GetPathByeffectId(620215), null);
		
		m_vDesLabel.text = "[dbba8f]点击任意处继续[-]";
	}
	
	private void VipAnimationLoadBack(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		int id = 0;
		if (p_path == Res2DTemplate.GetResPath(226))
		{
			id = 0;
		}
		else if (p_path == Res2DTemplate.GetResPath(227))
		{
			id = 1;
		}
		else if (p_path == Res2DTemplate.GetResPath(228))
		{
			id = 2;
		}
		
		RuntimeAnimatorController anim = (RuntimeAnimatorController)p_object;
		m_vipAnimaList[id].runtimeAnimatorController = anim;
		m_vipAnimaList[id].enabled = true;
	}

	public override void MYClick (GameObject ui)
	{
		switch (ui.name)
		{
		case "ShowVipClose":
			m_showVipObj.SetActive (false);
			GeneralRewardManager.Instance ().M_OtherExit = false;
			UI3DEffectTool.ClearUIFx (m_effect);

			if (M_ShowVipDelegate != null)
			{
				M_ShowVipDelegate ();
				if (M_CameraObj != null)
				{
					EffectTool.SetUIBackgroundEffect (M_CameraObj,false);
					M_CameraObj = null;
				}
			}

			break;
		}
	}

	void OnDestroy ()
	{
		base.OnDestroy ();
	}
}
