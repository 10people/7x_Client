using UnityEngine;
using System.Collections;

public class BattleEnmeyMibaoShowContollor : MonoBehaviour 
{

	public UISprite spriteBack;

	public UISprite spriteLabel_1;

	public UISprite spriteLabel_2;


	private static BattleEnmeyMibaoShowContollor _instance;


	private int skillNameId;

	private static float minAlpha = .1f;

	private static float label1StartTime = .2f;
	
	private static float label2StartTime = .8f;
	
	private static float label1ActionTime = .15f;
	
	private static float label2ActionTime = .15f;
	
	private static float hideActionTime = .5f;
	
	private static float endTime = 1.5f;

	private static iTween.EaseType labelEase = iTween.EaseType.easeOutElastic;
	
	private static Vector3 labelStartScale = new Vector3(5f, 5f, 1f);
	
	private static Vector3 labelEndScale = new Vector3 (1f, 1f, 1f);

	
	public static BattleEnmeyMibaoShowContollor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }

	void Start () 
	{
		hide();
	}

	public static void show(int _skillNameId)
	{
		Instance ()._show (_skillNameId);
	}

	private void _show(int _skillNameId)
	{
		hide ();

		gameObject.SetActive (true);

		skillNameId = _skillNameId;

		spriteLabel_1.spriteName = skillNameId + "_1";
		
		spriteLabel_2.spriteName = skillNameId + "_2";

		_showSkillEffBackAction ();

		_showSkillEffLabel1Action ();

		_showSkillEffLabel2Action ();

		_endAction();
	}

	private void _showSkillEffBackAction()
	{
		spriteBack.gameObject.SetActive (true);
		
		spriteBack.alpha = minAlpha;
		
		iTween.ValueTo(spriteBack.gameObject, iTween.Hash(
			"from", minAlpha,
			"to", 1f,
			"time", hideActionTime,
			"ignoretimescale", true,
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
			"ignoretimescale", true,
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
			"ignoretimescale", true,
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
		iTween.ValueTo(spriteBack.gameObject, iTween.Hash(
			"delay", endTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"ignoretimescale", true,
			"onupdate", "iSetAlpha"
			));
		
		iTween.ValueTo(spriteLabel_1.gameObject, iTween.Hash(
			"delay", endTime - label1StartTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"ignoretimescale", true,
			"onupdate", "iSetAlpha"
			));
		
		iTween.ValueTo(spriteLabel_2.gameObject, iTween.Hash(
			"delay", endTime - label2StartTime,
			"from", 1f,
			"to", minAlpha,
			"time", hideActionTime,
			"ignoretimescale", true,
			"onupdate", "iSetAlpha",
			"oncomplete", "hide",
			"oncompletetarget", gameObject
			));
	}


	private void hide()
	{
		skillNameId = 0;
		
		UIBackgroundEffect t_ef = EffectTool.SetUIBackgroundEffect( Camera.main.gameObject, false );
		
		BattleUIControlor.Instance ().layerFight.SetActive (true);
		
		spriteBack.gameObject.SetActive (false);
		
		spriteLabel_1.gameObject.SetActive (false);
		
		spriteLabel_2.gameObject.SetActive (false);
		
		gameObject.SetActive (false);
	}
	
}
