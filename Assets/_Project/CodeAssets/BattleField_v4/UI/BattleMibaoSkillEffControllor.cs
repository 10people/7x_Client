using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMibaoSkillEffControllor : MonoBehaviour 
{
	public List<UISprite> spriteframes;

	public UISprite spriteBack;

	public UISprite spriteLabel_1;

	public UISprite spriteLabel_2;

	public GameObject colliderObject;


	private int skillNameId;


	private static BattleMibaoSkillEffControllor _instance;

	private static float label1StartTime = .2f;

	private static float label2StartTime = .8f;

	private static float label1ActionTime = .15f;

	private static float label2ActionTime = .15f;

	private static float hideActionTime = .5f;

	private static float endTime = 1.5f;

	private static iTween.EaseType labelEase = iTween.EaseType.easeOutElastic;

	private static Vector3 labelStartScale = new Vector3(5f, 5f, 1f);

	private static Vector3 labelEndScale = new Vector3 (1f, 1f, 1f);

//	private float tempTimeScale = 1;

	private static float minAlpha = .1f;


	public static BattleMibaoSkillEffControllor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }


	void Start () 
	{
		hide();
	}

	public static void showSkillEff(int _skillNameId)
	{
		if(_instance.gameObject.activeSelf == false) _instance._showSkillEff (_skillNameId);
	}

	private void _showSkillEff(int _skillNameId)
	{
		BattleUIControlor.Instance ().attackJoystick.reset ();

		if(_skillNameId == 650001)//羲皇青龙诀 根据模型id播语音
		{
			if(BattleControlor.Instance().getKing().modelId == 1002)//豪杰
			{
				//811860

				ClientMain.Instance().m_SoundPlayEff.PlaySound("811860");
			}
			else if(BattleControlor.Instance().getKing().modelId == 1003)//儒雅
			{
				//811880

				ClientMain.Instance().m_SoundPlayEff.PlaySound("811880");
			}
			else if(BattleControlor.Instance().getKing().modelId == 1004)//蔷薇
			{
				//811870

				ClientMain.Instance().m_SoundPlayEff.PlaySound("811870");
			}
			else if(BattleControlor.Instance().getKing().modelId == 1005)//萝莉
			{
				//811890

				ClientMain.Instance().m_SoundPlayEff.PlaySound("811890");
			}
		}
		else if(_skillNameId == 650007)//蚩尤地煞诀
		{
			if(BattleControlor.Instance().getKing().modelId == 1003 
			   && CityGlobalData.m_battleType == EnterBattleField.BattleType.Type_GuoGuan
			   && CityGlobalData.m_tempSection == 0
			   && CityGlobalData.m_tempLevel == 1)//仅儒雅在第0章第一关播放语音
			{
				//811920

				ClientMain.Instance().m_SoundPlayEff.PlaySound("811920");
			}
		}

//		tempTimeScale = Time.timeScale;

		hide ();

//		UIBackgroundEffect t_ef = EffectTool.SetUIBackgroundEffect( Camera.main.gameObject, true );

//		BattleUIControlor.Instance ().layerFight.SetActive (false);

//		Time.timeScale = 0;

		gameObject.SetActive (true);

//		colliderObject.SetActive (true);

		skillNameId = _skillNameId;
		
		foreach(UISprite sprite in spriteframes)
		{
			sprite.spriteName = skillNameId + "_frame";
		}
		
		spriteBack.spriteName = skillNameId + "_back";
		
		spriteLabel_1.spriteName = skillNameId + "_1";
		
		spriteLabel_2.spriteName = skillNameId + "_2";

		_showSkillEffFrameAction();

		_showSkillEffBackAction();

		_showSkillEffLabel1Action();

		_showSkillEffLabel2Action();

		_endAction();
	}

	private void _showSkillEffFrameAction()
	{
		foreach(UISprite sprite in spriteframes)
		{
			sprite.gameObject.SetActive(true);

			sprite.alpha = minAlpha;
		}

		foreach(UISprite sprite in spriteframes)
		{
			iTween.ValueTo(sprite.gameObject, iTween.Hash(
				"from", minAlpha,
				"to", 1f,
				"time", hideActionTime,
				"onupdate", "iSetAlpha"
				));
		}
	}

	private void _showSkillEffBackAction()
	{
		spriteBack.gameObject.SetActive (true);

		spriteBack.alpha = minAlpha;

		iTween.ValueTo(spriteBack.gameObject, iTween.Hash(
			"from", minAlpha,
			"to", 1f,
			"time", hideActionTime,
			"onupdate", "iSetAlpha"
			));
	}

	private void _showSkillEffLabel1Action()
	{
		spriteLabel_1.transform.localScale = labelStartScale;

		spriteLabel_1.alpha = 1f;

		iTween.ValueTo(gameObject, iTween.Hash(
			"delay", label1StartTime,
			"from", labelStartScale.x,
			"to", labelEndScale.x,
			"time", label1ActionTime,
			"easetype", labelEase,
			"onupdate", "_showSkillEffLabel1Update",
			"onstart", "_showSkillEffLabel1Start"
			));
	}

	private void _showSkillEffLabel1Start()
	{
		spriteLabel_1.gameObject.SetActive (true);
	}

	private void _showSkillEffLabel1Update(float _scale)
	{
		spriteLabel_1.transform.localScale = new Vector3(_scale, _scale, 1f);
	}

	private void _showSkillEffLabel2Action()
	{
		spriteLabel_2.transform.localScale = labelStartScale;

		spriteLabel_2.alpha = 1f;

		iTween.ValueTo(gameObject, iTween.Hash(
			"delay", label2StartTime,
			"from", labelStartScale.x,
			"to", labelEndScale.x,
			"time", label2ActionTime,
			"easetype", labelEase,
			"onupdate", "_showSkillEffLabel2Update",
			"onstart", "_showSkillEffLabel2Start"
			));
	}

	private void _showSkillEffLabel2Start()
	{
		spriteLabel_2.gameObject.SetActive (true);
	}

	private void _showSkillEffLabel2Update(float _scale)
	{
		spriteLabel_2.transform.localScale = new Vector3(_scale, _scale, 1f);
	}

	private void _endAction()
	{
		foreach(UISprite sprite in spriteframes)
		{
			iTween.ValueTo(sprite.gameObject, iTween.Hash(
				"delay", endTime,
				"from", 1f,
				"to", minAlpha,
				"time", hideActionTime,
				"onupdate", "iSetAlpha"
				));
		}

		iTween.ValueTo(spriteBack.gameObject, iTween.Hash(
			"delay", endTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"onupdate", "iSetAlpha"
			));

		iTween.ValueTo(spriteLabel_1.gameObject, iTween.Hash(
			"delay", endTime - label1StartTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"onupdate", "iSetAlpha"
			));

		iTween.ValueTo(spriteLabel_2.gameObject, iTween.Hash(
			"delay", endTime - label2StartTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"onupdate", "iSetAlpha",
			"oncomplete", "startHide",
			"oncompletetarget", gameObject
			));
	}

	private void startHide()
	{
		StartCoroutine (startHideAction());
	}

	private IEnumerator startHideAction()
	{
		yield return new WaitForEndOfFrame ();

		hide ();
	}

	private void hide()
	{
		skillNameId = 0;

//		UIBackgroundEffect t_ef = EffectTool.SetUIBackgroundEffect( Camera.main.gameObject, false );

//		BattleUIControlor.Instance ().layerFight.SetActive (true);

		foreach(UISprite sprite in spriteframes)
		{
			sprite.gameObject.SetActive (false);
		}
		
		spriteBack.gameObject.SetActive (false);
		
		spriteLabel_1.gameObject.SetActive (false);

		spriteLabel_1.alpha = 1f;

		spriteLabel_2.gameObject.SetActive (false);

		spriteLabel_2.alpha = 1f;

		gameObject.SetActive (false);

//		colliderObject.SetActive (false);

//		Time.timeScale = tempTimeScale;
	}

}
