using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneGuideManager : MonoBehaviour, IUIRootAutoActivator
{
	public UIPanel m_scene_panel;

	public UILabel m_lb_title;

	public float m_show_time = 1.5f;
	
	public float m_fade_time = .5f;

	public UILabelType labelType;


	private static SceneGuideManager m_instance = null;

	private EventTriggerTemplate curTemplate;


	public static SceneGuideManager Instance(){
		if( m_instance == null ){
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.SCENE_GUIDE ), ResourceLoadCallback );
		}
		
		return m_instance;
	}

	public static void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		GameObject t_gb = (GameObject)GameObject.Instantiate( p_object );
		
		if( t_gb == null ){
			Debug.LogError( "Instantiate to null." );
			
			return;
		}
		
		DontDestroyOnLoad( t_gb );
	}

	#region Mono

	void Awake(){
		m_instance = this;

		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
	}

	void OnDestroy(){
		m_instance = null;

		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}
	}


	#endregion



	#region Scene Guide

	public void ShowSceneGuide( int eventTriggerId, string desc = null )
	{
		curTemplate = EventTriggerTemplate.getEventTriggerTemplateById (eventTriggerId);

		labelType.m_iType = curTemplate.dictType;

		labelType.init ();

		m_lb_title.gameObject.SetActive (true);

		if(curTemplate.descId != 0)
		{
			m_lb_title.text = DescIdTemplate.GetDescriptionById(curTemplate.descId);
		}
		else
		{
			if(desc == null || desc.Length == 0)
			{
				Debug.LogError("MISSING TIGGEREVENT DESC !");

				return;
			}

			m_lb_title.text = desc;
		}

		iTween.StopByName ("FadeInTips");

		iTween.StopByName ("FadeOutTips");

		EnableSceneTips( true );
	}

	private void EnableSceneTips( bool p_show )
	{
		m_scene_panel.gameObject.SetActive( p_show );

		if( p_show ) FadeSceneTips();
	}

	private void FadeSceneTips(){
		iTween.ValueTo( gameObject, iTween.Hash( 
		   "name", "FadeInTips",
		   "from", 0f,
		   "to", 1f,
		   "delay", 0,
		   "time", m_fade_time,
		   "easetype", iTween.EaseType.linear,
		   "onupdate", "OnSceneTipsFade" ) );
		
		iTween.ValueTo( gameObject, iTween.Hash(
			"name", "FadeOutTips",
			"from", 1f,
			"to", 0f,
			"delay", m_fade_time + m_show_time,
			"time", m_fade_time,
			"easetype", iTween.EaseType.linear,
			"onupdate", "OnSceneTipsFade",
			"oncomplete", "OnSceneTipsHide" ) );
	}

	public void OnSceneTipsFade( float p_value )
	{
		m_scene_panel.alpha = p_value;
	}

	public void OnSceneTipsHide()
	{
		EnableSceneTips( false );
	}

	#endregion



	#region IUIRootAutoActivator

	public bool IsNGUIVisible(){
		if( m_scene_panel == null ){
			return false;
		}

		return m_scene_panel.alpha > 0 ? true : false;
	}

	#endregion

}
