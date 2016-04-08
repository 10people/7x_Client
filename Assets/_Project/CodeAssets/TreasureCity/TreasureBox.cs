using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class TreasureBox : MonoBehaviour {
	
	public EnterScene enterScene; 

	private TreasureOpenBox tOpenBox;

	private GameObject lightEffect;

	private Animator animation;

	/// <summary>
	/// Ins it effect.
	/// </summary>
	public void InItEffect ()
	{
		Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(3000), InstanceEffect);
	}

	void InstanceEffect (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		GameObject instanceEffect = Instantiate(p_object) as GameObject;
		instanceEffect.transform.parent = this.transform;
		instanceEffect.transform.localPosition = Vector3.zero;
		instanceEffect.transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Destroies the box.
	/// </summary>
	public void DestroyBox (TreasureOpenBox tempOpenBox)
	{
		tOpenBox = tempOpenBox;

		animation = this.GetComponent<Animator> ();

		//产生爆开特效
//		GetComponent<BoxCollider> ().enabled = false;
//		TCityPlayerManager.m_instance.TargetBoxUID = -1;//更新player范围的宝箱
		
		if (tempOpenBox.isOpen && tempOpenBox.num > 0)
		{
			Debug.Log ("Destroy2");

			Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(600246), LightEffectLoadBack);
		}
		else
		{
			DestroyGameObj ();
		}
	}

	void LightEffectLoadBack (ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		lightEffect = Instantiate(p_object) as GameObject;
		lightEffect.transform.parent = this.transform.parent;
		lightEffect.transform.localPosition = this.transform.localPosition + new Vector3(0,0.5f,0);
		lightEffect.transform.localScale = Vector3.one;
		
		StartCoroutine ("BoxAnimate");
	}
	
	IEnumerator BoxAnimate ()
	{
		yield return new WaitForSeconds (1f);

		animation.enabled = true;
	}

	public void JinBiFeiChu ()
	{
		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (6903), ResourceLoadCallback );
	}

	public void AnimationFinished ()
	{
		Destroy (lightEffect);
		DestroyGameObj ();
	}

	private void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object )
	{
//		Debug.Log ("LigntEffect");
		GameObject droppenObject = Instantiate (p_object) as GameObject;
		
		droppenObject.transform.parent = this.transform.parent;
		droppenObject.transform.localPosition = this.transform.localPosition + new Vector3(0,0.5f,0);
		droppenObject.transform.localScale = Vector3.one;

//		Vector3 foward = new Vector3(Random.value, 0, Random.value).normalized;
		Vector3 targetPos = transform.position;

		TreasureYuanBao treasureYb = droppenObject.GetComponent<TreasureYuanBao> ();
		treasureYb.refreshdata (targetPos,tOpenBox.num);
	}

	void DestroyGameObj ()
	{
		TCityPlayerManager.m_instance.DestroyBoxName (tOpenBox.exitScene);

		TreasureCityUI.m_instance.BottomUI (false);
		Destroy (gameObject);
	}
}