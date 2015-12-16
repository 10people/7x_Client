using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MiBaoSkillTEm : MonoBehaviour {

	public GameObject Lock;

	public UISprite SkillIcon;

	public UILabel SkillName;

	public UILabel MiBaoNum;

	public bool isChoosed;

	//public MibaoGroup mMiBaoGroup;

	public int ZUHE_id;

	public int SkillId;

	public UISprite Skillbg;

	private int chooseType;
	public int SetChooseType
	{
		set{chooseType = value;}
	}

	void Start () {
	
	}
	

	void Update () {
	
	}

 
	public void BoolBeChoose()
	{
		
		if(isChoosed)
		{
//			Debug.Log("被选中的id = "+ZUHE_id);
//
//			UIToggle mUItoggle = this.GetComponent<UIToggle>();
//
//			mUItoggle.value = !mUItoggle.value;
			StartCoroutine(bechosed());
		}

	}

	IEnumerator bechosed()
	{

		yield return new WaitForSeconds (0.1f);

		UIToggle mUItoggle = this.GetComponent<UIToggle>();
		
		mUItoggle.value = !mUItoggle.value;
	}

	public void Init()
	{

		//ZUHE_id = mMiBaoGroup.zuheId;

		//SkillId = mMiBaoGroup.skillId;

		//Skillbg.spriteName = ZUHE_id.ToString();     //缺少资源等美术给资源后可以去掉注释

		BoolBeChoose ();

		int pinzhi = 0;

//		for(int i = 0 ; i < mMiBaoGroup.mibaoInfo.Count; i++)
//		{
//			if(mMiBaoGroup.mibaoInfo[i].level > 0 &&!mMiBaoGroup.mibaoInfo[i].isLock)
//			{
//				pinzhi++;
//			}
//
//		}

		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SkillId);

//		Debug.Log ("SkillId = "+SkillId);
//
//		Debug.Log ("mSkill.nameId = "+mSkill.nameId);

		NameIdTemplate mName = NameIdTemplate.getNameIdTemplateByNameId (mSkill.nameId);

		SkillName.text = mName.Name; //大名字
//		if(mMiBaoGroup.hasActive == 1)
//		{
//			Lock.SetActive(false);
//		}
//		else
//		{
//			Lock.SetActive(true);
//			
//			MiBaoNum.text = "(未解锁)";
//			
//			SkillIcon.spriteName = "";
//		}
		if(pinzhi >= 2)
		{

			MiBaoNum.text = "("+pinzhi.ToString()+"/3)";

			SkillIcon.spriteName = mSkill.icon.ToString ();

		}

		if(FreshGuide.Instance().IsActive(100230)&& TaskData.Instance.m_TaskInfoDic[100230].progress >= 0 && chooseType == (int)(CityGlobalData.MibaoSkillType.PveSend))
		{
			this.GetComponent<UIDragScrollView> ().enabled = false;
		}
	}

	public void ChoosedSkill()
	{
//		Debug.Log ("ZUHE_id  = " +ZUHE_id);

		ChangeMiBaoSkill.Instance ().newZuHe_id = ZUHE_id;

		ChangeMiBaoSkill.Instance ().ReFreshLeftData (ZUHE_id);

		if(FreshGuide.Instance().IsActive(100230)&& TaskData.Instance.m_TaskInfoDic[100230].progress >= 0 && chooseType == (int)(CityGlobalData.MibaoSkillType.PveSend))
		{
//			Debug.Log("保存一个秘宝技能");

			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100230];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
		}
	}

}
