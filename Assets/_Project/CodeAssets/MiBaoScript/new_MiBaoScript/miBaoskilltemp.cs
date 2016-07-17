using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class miBaoskilltemp : MonoBehaviour {

	public GameObject mLock;

	public UISprite Icon;

	public UILabel Lv;

	public SkillInfo mSkillInfo;

	public int SKill_id;

	public bool IsActive;

	public bool beChoosed;
	public GameObject NewAddMiBaoShangZhen;

	public GameObject beChoosedSprite;

	void Start () {


	}

	public void Init()
	{
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SKill_id);
		Lv.text = "Lv."+mMiBaoSkillTemp.lv.ToString();
		Icon.spriteName = mMiBaoSkillTemp.icon;
		Lv.text = "Lv."+mSkillInfo.level.ToString ();
		IsActive = true;
		mLock.SetActive (false);
//		Debug.Log ("SKill_id  = " +SKill_id);
	}

	public void IsUnLock()
	{
		IsActive = false;
		
		mLock.SetActive (true);
		
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SKill_id);
		Lv.text = "Lv."+mMiBaoSkillTemp.lv.ToString();
		Icon.spriteName = mMiBaoSkillTemp.icon;
	}

	public void ShowDeilInfo()
	{
		if(NewMiBaoSkill.Instance().COmeMiBaoUI)
		{
			if(NewMiBaoSkill.Instance().SaveId == SKill_id)
			{
				return;
			}
			NewMiBaoSkill.Instance().ShowBeChoosed_MiBao (SKill_id,IsActive);
		}
		else
		{
			if(NewMiBaoSkill.Instance().SaveId == SKill_id)
			{
				return;
			}
			NewMiBaoSkill.Instance().ShowBeChoosed_MiBao (SKill_id,IsActive);
			if(IsActive)
			{
				NewAddMiBaoShangZhen.SetActive(true);
				NewAddMiBaoShangZhen.transform.localPosition = this.gameObject.transform.localPosition;
			}
		}

	}
}
