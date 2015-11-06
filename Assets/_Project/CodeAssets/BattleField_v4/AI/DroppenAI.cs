using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using qxmobile.protobuf;

public class DroppenAI : MonoBehaviour 
{
	public GameObject body;


	private Vector3 targetPos;

	private Vector3 startPos;

	private bool flying;

	private DroppenItem item;


	public void refreshdata(Vector3 _targetPos, DroppenItem _item)
	{
		targetPos = _targetPos;

		item = _item;

		startPos = transform.position;

		flying = false;

		GameObject effectTemple;

		BattleEffectControllor.Instance ().GetEffectDict ().TryGetValue(
			EffectIdTemplate.getEffectTemplateByEffectId(200001).path, 
			out effectTemple);

		body.SetActive (true);

		StartCoroutine (actionStart());
	}

	IEnumerator actionStart()
	{
		float delay = Random.value * .5f;

		yield return new WaitForSeconds (delay);

		float dropTime = 1.5f;
		
		float dropHeight = 1f;

		BattleEffectControllor.Instance ().PlayEffect (200000, startPos, (targetPos - startPos).normalized);
		
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0,
			"to", 1,
			"time", dropTime,
			"easeType", iTween.EaseType.linear,
			"onupdate", "updateX"
			));
		
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0,
			"to", dropHeight,
			"time", dropTime / 2,
			"easeType", iTween.EaseType.easeOutCirc,
			"onupdate", "updateY"
			));
		
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", dropHeight,
			"to", 0,
			"delay", dropTime / 2,
			"time", dropTime / 2,
			"easeType", iTween.EaseType.easeInCirc,
			"onupdate", "updateY"
			));
		
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0,
			"to", 1,
			"time", dropTime + 1f,
			"easeType", iTween.EaseType.linear,
			"onupdate", "OnActionUpdate",
			"oncomplete", "OnActionFinish"
			));
	}

	public void updateX(float x)
	{
		Vector3 tempPos = startPos + (targetPos - startPos) * x;

		transform.position = new Vector3 (tempPos.x, transform.position.y, tempPos.z);
	}

	public void updateY(float y)
	{
		float targetY = y + targetPos.y;

		targetY = targetY < targetPos.y ? targetPos.y : targetY;

		transform.position = new Vector3 (transform.position.x, targetY, transform.position.z);
	}

	public void OnActionUpdate()
	{

	}

	public void OnActionFinish()
	{
//		SoundPlayEff sound = gameObject.AddComponent<SoundPlayEff> ();
//		
//		sound.PlaySound(1060);

		flying = true;

		body.SetActive (false);

		BattleEffectControllor.Instance ().PlayEffect (200001, gameObject, 999);
	}

	public void Update ()
	{
		if (flying == false) return;

		Vector3 targetP = BattleControlor.Instance ().getKing ().transform.position + new Vector3(0, 1.5f, 0);

		if(Vector3.Distance(transform.position, targetP) < .5f)
		{
			flying = false;

			SoundPlayEff sound = gameObject.AddComponent<SoundPlayEff> ();

			sound.PlaySound(310200 + "");

			body.SetActive(false);

			BloodLabelControllor.Instance().showDroppenAwardEx(BattleControlor.Instance ().getKing (), item);

			StartCoroutine(des());
		}
		else
		{
			Vector3 forward = (targetP - transform.position).normalized;
			
			Vector3 step = forward * 10 * Time.deltaTime;

			transform.position += step;
		}
	}

	IEnumerator des()
	{
		yield return new WaitForSeconds(.5f);

		DestroyObject (gameObject);
	}

}
