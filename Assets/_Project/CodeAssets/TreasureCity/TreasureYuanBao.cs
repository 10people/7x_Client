using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureYuanBao : MonoBehaviour {

	private int rewardYbNum;

	public GameObject body;

	private Vector3 targetPos;
	
	private Vector3 startPos;
	
	private bool flying;

	private GameObject jinBiFeiChuObj;
	private GameObject jinBiFeiObj;

	public void refreshdata (Vector3 _targetPos,int tempRewardNum)
	{
		rewardYbNum = tempRewardNum;

		targetPos = _targetPos;
		startPos = transform.position;
		
		flying = false;

		Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(200000), LoadJinBiFeiChu);
	}

	void LoadJinBiFeiChu(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		jinBiFeiChuObj = Instantiate(p_object) as GameObject;
		jinBiFeiChuObj.transform.parent = this.transform;
		jinBiFeiChuObj.transform.localPosition = Vector3.zero;
		jinBiFeiChuObj.transform.localScale = Vector3.one;

		SoundPlayEff sound = jinBiFeiChuObj.AddComponent<SoundPlayEff> ();
		sound.PlaySound(310102 + "");

		StartCoroutine (actionStart());
	}

	IEnumerator actionStart()
	{
		float delay = Random.value * .5f;
		
		yield return new WaitForSeconds (delay);
		
		float dropTime = .7f;
//		float dropTime = 0f;
		float dropHeight = 0f;
		
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
			"time", dropTime + 0.1f,
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
		
		//		targetY = targetY < targetPos.y ? targetPos.y : targetY;
		
		targetY = targetY > targetPos.y ? targetPos.y : targetY;
		
		transform.position = new Vector3 (transform.position.x, targetY, transform.position.z);
	}
	
	void OnActionFinish()
	{
		Destroy (jinBiFeiChuObj);
		Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(600247), LoadJinBiFei);

	}

	void LoadJinBiFei(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		jinBiFeiObj = Instantiate(p_object) as GameObject;
		jinBiFeiObj.transform.parent = this.transform;
		jinBiFeiObj.transform.localPosition = Vector3.zero;
		jinBiFeiObj.transform.localScale = Vector3.one;
		
		flying = true;
	}

	void Update ()
	{
		if (flying == false) return;
		
		Vector3 targetP = TreasureCityPlayer.m_instance.m_playerObj.transform.position + new Vector3(0, 1.5f, 0);
		
		if(Vector3.Distance(transform.position, targetP) < .5f)
		{
			flying = false;

			SoundPlayEff sound = jinBiFeiObj.AddComponent<SoundPlayEff> ();
			sound.PlaySound(310200 + "");

			StartCoroutine (DestroyDropAI());
		}
		else
		{
			Vector3 forward = (targetP - transform.position).normalized;
			
			Vector3 step = forward * 20 * Time.deltaTime;
			
			transform.position += step;
		}
	}

	IEnumerator DestroyDropAI()
	{
		yield return new WaitForSeconds (0.2f);
		
		List<RewardData> tempList = new List<RewardData>();
		tempList.Add (new RewardData(900002,rewardYbNum));
		GeneralRewardManager.Instance ().CreateReward (tempList);
		
		DestroyObject( gameObject );
	}
}
