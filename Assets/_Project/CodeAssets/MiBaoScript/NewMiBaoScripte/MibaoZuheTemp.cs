using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MibaoZuheTemp : MonoBehaviour {

	//public MibaoGroup mMiBaoGroup;

	[HideInInspector]public int Skill_id;

	public UISprite Skill_Icon;

	public UILabel SKillName;

	public UILabel SKill_Instruction;

	public GameObject MiBaoTep;

	public GameObject SkillLock;

	private float dis = 130;

	public UISprite Dragon;

	[HideInInspector]public int DragonNum;

	[HideInInspector]public int Zuhe;

	//[HideInInspector]public int Pinzhi;
	public GameObject Art;

	public UISprite Dikuang;
	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{
	
//		if(mMiBaoGroup.hasActive == 1)
//		{
//			SkillLock.SetActive(false);
//		}
//		else
//		{
//			SkillLock.SetActive(true);
//		}
//		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (Skill_id);
//		
//		NameIdTemplate mName = NameIdTemplate.getNameIdTemplateByNameId (mSkill.nameId);
//		
//		//DescIdTemplate mDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.zuheDesc);
//		
//		SKillName.text = mName.Name;
//		
//		Skill_Icon.spriteName = mSkill.icon.ToString();
//		
//		
//		char mchar  = '#';
//		
//		string[] s = mDes.description.Split(mchar);
//		
//		string des = "";
//		for(int m = 0 ; m < s.Length; m ++)
//		{
//			des += s[m]+"\r\n";
//		}
//		
//		SKill_Instruction.text = des;
//		Dragon.spriteName = DragonNum.ToString ();
//		Dikuang.spriteName = (DragonNum + 10).ToString ();
//		Dragon.SetDimensions ((int)(Dragon.width),(int)(Dragon.height));
//
//		CreateMiBaoTemp ();
	}

	void CreateMiBaoTemp()
	{
		int ActiveMiBaoNumber = 0;
//		for(int i = 0 ; i < mMiBaoGroup.mibaoInfo.Count; i ++)
//		{
//			GameObject mMiBaotep = Instantiate(MiBaoTep) as GameObject;
//
//			mMiBaotep.SetActive(true);
//
//			mMiBaotep.transform.parent = MiBaoTep.transform.parent;
//
//			mMiBaotep.transform.localPosition = new Vector3(-130+i*dis,0,0);
//
//			mMiBaotep.transform.localScale = Vector3.one;
//
//			MBTemp mMBTemp = mMiBaotep.GetComponent<MBTemp>();
//
//			mMBTemp.mMiBaoinfo = mMiBaoGroup.mibaoInfo[i];
//
//			MiBaoManager.Instance().mMBTempList.Add(mMBTemp);
//
//			mMBTemp.Init();
//
//			if(!mMiBaoGroup.mibaoInfo[i].isLock &&mMiBaoGroup.mibaoInfo[i].level > 0)
//			{
//				ActiveMiBaoNumber++;
//			}
//		}
		//Debug.Log ("ActiveMiBaoNumber = "+ActiveMiBaoNumber);

//		if(ActiveMiBaoNumber >= 2)
//		{
//			//Debug.Log("mMiBaoGroup.hasActive = "+mMiBaoGroup.hasActive);
//			if(mMiBaoGroup.hasActive == 0)
//			{
//				Art.SetActive(true);
//			}
//			else
//			{
//				if(ActiveMiBaoNumber >= 3)
//				{
//					if(mMiBaoGroup.hasJinjie == 0)
//					{
//						Art.SetActive(true);
//					}
//					else
//					{
//						Art.SetActive(false);
//					}
//				}
//				else
//				{
//					Art.SetActive(false);
//				}
//			}
//		}
//		else
//		{
//			Art.SetActive(false);
//		}
	}

	public void ShowSkillInfo()
	{
		//MiBaoManager.Instance ().MiBaoManager_mMiBaoGroup = mMiBaoGroup;

		MiBaoManager.Instance ().CurrSkill_id = Skill_id;

		MiBaoManager.Instance ().SortUI ("MiBaoSkillInfo");

	}
}
