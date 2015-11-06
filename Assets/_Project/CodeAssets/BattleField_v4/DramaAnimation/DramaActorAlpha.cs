using UnityEngine;
using System.Collections;

public class DramaActorAlpha : DramaActor
{
	public UISprite m_SpriteBG;

	public float m_fEndAlpha;

	public float m_fNeedTime;
	
	void Start()
	{
		actorType = ACTOR_TYPE.ALPHABG;
	}

	protected override void funcStart()
	{
		m_SpriteBG = GameObject.Find("UISpirteAlphaBG").GetComponent<UISprite>();
	}

	protected override float func ()
	{
		base.func ();

		iTween.ValueTo(gameObject, iTween.Hash(
			"name", "move_" + 0,
			"from", m_SpriteBG.alpha,
			"to", m_fEndAlpha,
			"delay", 0,
			"time", m_fNeedTime,
			"easeType", iTween.EaseType.linear,
			"onupdate", "UpdataValue",
			"LoopType", "none"
			));
		
		return m_fNeedTime;
	}

	protected void UpdataValue(float f)
	{
		m_SpriteBG.alpha = f;
	}
	
	protected override bool funcDone ()
	{
		if(m_SpriteBG.alpha == m_fEndAlpha)
		{
			return true;
		}
		return false;
	}
	
}
