using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMibaoSkillEffControllor : MonoBehaviour 
{
	public List<UISprite> spriteframes;

	public UISprite spriteBack;

	public UISprite spriteLabel_1;

	public UISprite spriteLabel_2;


	private int skillNameId;

	private BattleMibaoEffTemplate template;


	private static BattleMibaoSkillEffControllor _instance;

	private static float label1ActionTime = .15f;

	private static float label2ActionTime = .15f;

	private static float hideActionTime = .5f;

	private static iTween.EaseType labelEase = iTween.EaseType.easeOutElastic;

	private static Vector3 labelStartScale = new Vector3(5f, 5f, 1f);

	private static Vector3 labelEndScale = new Vector3 (1f, 1f, 1f);


	public static BattleMibaoSkillEffControllor Instance() { return _instance; }
	
	private void Awake() { _instance = this; }


	void Start () 
	{
		hide();
	}

	public static void showSkillEff(int _skillNameId)
	{
		_instance._showSkillEff (_skillNameId);
	}

	private void _showSkillEff(int _skillNameId)
	{
		hide ();

		gameObject.SetActive (true);

		skillNameId = _skillNameId;
		
		template = BattleMibaoEffTemplate.getMibaoEffectTemplateByNameId (skillNameId);

		foreach(UISprite sprite in spriteframes)
		{
			sprite.spriteName = skillNameId + "_frame";
		}
		
		spriteBack.spriteName = skillNameId + "_back";
		
		spriteLabel_1.spriteName = skillNameId + "_1";
		
		spriteLabel_2.spriteName = skillNameId + "_2";

		StartCoroutine (_showSkillEffFrameAction());

		StartCoroutine (_showSkillEffBackAction());

		StartCoroutine (_showSkillEffLabel1Action());

		StartCoroutine (_showSkillEffLabel2Action());

		StartCoroutine (_endAction());
	}

	private IEnumerator _showSkillEffFrameAction()
	{
		foreach(UISprite sprite in spriteframes)
		{
			sprite.gameObject.SetActive(true);

			sprite.alpha = 0;
		}

		yield return new WaitForSeconds (template.frameStartTime);

		foreach(UISprite sprite in spriteframes)
		{
			TweenAlpha.Begin(sprite.gameObject, template.frameActionTime, 1f);
		}
	}

	private IEnumerator _showSkillEffBackAction()
	{
		spriteBack.gameObject.SetActive (true);

		spriteBack.alpha = 0;

		yield return new WaitForSeconds (template.backStartTime);

		TweenAlpha.Begin(spriteBack.gameObject, template.backActionTime, 1f);
	}

	private IEnumerator _showSkillEffLabel1Action()
	{
		spriteLabel_1.transform.localScale = labelStartScale;

		spriteLabel_1.alpha = 1f;

		yield return new WaitForSeconds (template.label1StartTime);

		spriteLabel_1.gameObject.SetActive (true);

		iTween.ScaleTo (spriteLabel_1.gameObject, iTween.Hash(
			"scale", labelEndScale,
			"time", label1ActionTime,
			"easetype", labelEase
			));
	}

	private IEnumerator _showSkillEffLabel2Action()
	{
		spriteLabel_2.transform.localScale = labelStartScale;

		spriteLabel_2.alpha = 1f;

		yield return new WaitForSeconds (template.label2StartTime);
		
		spriteLabel_2.gameObject.SetActive (true);
		
		iTween.ScaleTo (spriteLabel_2.gameObject, iTween.Hash(
			"scale", labelEndScale,
			"time", label2ActionTime,
			"easetype", labelEase
			));
	}

	private IEnumerator _shkeAction()
	{
		yield return new WaitForSeconds (template.shakeTime);

		BattleControlor.Instance().getKing ().gameCamera.Shake (KingCamera.ShakeType.Cri);
	}

	private IEnumerator _endAction()
	{
		yield return new WaitForSeconds (template.endTime);

		foreach(UISprite sprite in spriteframes)
		{
			TweenAlpha.Begin(sprite.gameObject, hideActionTime, 0f);
		}

		TweenAlpha.Begin(spriteBack.gameObject, hideActionTime, 0f);

		TweenAlpha.Begin(spriteLabel_1.gameObject, hideActionTime, 0f);

		TweenAlpha.Begin(spriteLabel_2.gameObject, hideActionTime, 0f);

		yield return new WaitForSeconds (hideActionTime);

		hide ();
	}

	private void hide()
	{
		skillNameId = 0;

		foreach(UISprite sprite in spriteframes)
		{
			sprite.gameObject.SetActive (false);
		}
		
		spriteBack.gameObject.SetActive (false);
		
		spriteLabel_1.gameObject.SetActive (false);
		
		spriteLabel_2.gameObject.SetActive (false);

		gameObject.SetActive (false);
	}

}
