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

	void Start () {

		IsActive = false;

		mLock.SetActive (true);

		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SKill_id);
		Lv.text = "Lv."+mMiBaoSkillTemp.lv.ToString();
		Icon.spriteName = mMiBaoSkillTemp.icon;
	}
	void Update () {
	
	}
	public void Init()
	{
		Lv.text = "Lv."+mSkillInfo.level.ToString ();

		mLock.SetActive (false);

	}
	public void Be_CHoosed()
	{
		//StartCoroutine (Changestatebtn());
	}
//	IEnumerator Changestatebtn()
//	{
//		yield return new WaitForSeconds (0.5f);
//		
//		this.gameObject.GetComponent<UIToggle>().value = !this.gameObject.GetComponent<UIToggle>().value;
//		
//
//	}
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
			if(FreshGuide.Instance().IsActive(100260)&& TaskData.Instance.m_TaskInfoDic[100260].progress >= 0)
			{
				//	Debug.Log("切换秘技)Z
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100260];
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[4]);
			}
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
