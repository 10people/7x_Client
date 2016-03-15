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
	private GameObject lightEffect;

	public void InItTreasureBox (EnterScene tempEnterScene)
	{
		enterScene = tempEnterScene;
	}

	public void DestroyBox ()
	{
		//产生爆开特效
		this.GetComponent<BoxCollider> ().enabled = false;

		TreasureCityPlayer.m_instance.TargetBoxUID = -1;//更新player范围的宝箱

		if (TCityPlayerManager.m_instance.IsOpenBox)
		{
			Debug.Log ("Destroy2");
			TCityPlayerManager.m_instance.IsOpenBox = false;
			Global.ResourcesDotLoad(EffectIdTemplate.GetPathByeffectId(600246), TenementNPCBigHouse);
		}
		else
		{
			TreasureCityUI.m_instance.BottomUI (false);
			Destroy (gameObject);
		}
	}

	void TenementNPCBigHouse(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		lightEffect = Instantiate(p_object) as GameObject;
		lightEffect.transform.parent = this.transform.parent;
		lightEffect.transform.localPosition = Vector3.zero;
		lightEffect.transform.localScale = Vector3.one;

		StartCoroutine ("BoxAnimate");
	}

	IEnumerator BoxAnimate ()
	{
		yield return new WaitForSeconds (1);

		Global.ResourcesDotLoad ( PlayerInCityManager.GetModelResPathByRoleId (6903), ResourceLoadCallback );

//		RewardData data = new RewardData ();
//		GeneralRewardManager.Instance ().CreateReward ();
		TreasureCityUI.m_instance.BottomUI (false);
		Destroy (lightEffect);
		Destroy (gameObject);
	}

	private void ResourceLoadCallback(ref WWW p_www, string p_path, Object p_object )
	{
		GameObject obj = Instantiate (p_object) as GameObject;

		obj.transform.parent = this.transform.parent;
		obj.transform.localPosition = this.transform.localPosition;
		obj.transform.localScale = Vector3.one;
	}
}
