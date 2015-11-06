using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShowAllMibaoSkill : MonoBehaviour {


	public UISprite L_SkillIcon;
	
	public UISprite R_SkillIcon;
	
	public UILabel L_ZuheName;

	public UILabel R_ZuheName;
	
	public UILabel L_SkillName;

	public UILabel R_SkillName;
	
	public UILabel L_Skillinstruction;

	public UILabel R_Skillinstruction;
	
	public UILabel L_instruction;

	public UILabel R_instruction;
	
	public UILabel L_SHUXIng1;
	public UILabel L_shuxing1;
	
	public UILabel R_SHUXIng1;
	public UILabel R_shuxing1;

	public UILabel L_SHUXIng2;
	public UILabel L_shuxing2;

	public UILabel R_SHUXIng2;
	public UILabel R_shuxing2;

	public UILabel L_SHUXIng3;
	public UILabel L_shuxing3;
	
	public UILabel R_SHUXIng3;
	public UILabel R_shuxing3;

	public UILabel L_SHUXIng4;
	public UILabel L_shuxing4;
	
	public UILabel R_SHUXIng4;
	public UILabel R_shuxing4;
	
	public int SkillId;

	void Start () {
	
	}
	

	void Update () {
	
	}

	public void Init()
	{
		MiBaoSkillTemp mMibaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SkillId);
		
		L_SkillIcon.spriteName = mMibaoskill.icon.ToString ();
		
		R_SkillIcon.spriteName = mMibaoskill.icon.ToString ();

		NameIdTemplate mName = NameIdTemplate.getNameIdTemplateByNameId (mMibaoskill.nameId);
		
		DescIdTemplate mDes = DescIdTemplate.getDescIdTemplateByNameId (mMibaoskill.zuheDesc);
		
		SkillTemplate mskilltemp = SkillTemplate.getSkillTemplateById (mMibaoskill.skill);
		
		NameIdTemplate Skill_Name = NameIdTemplate.getNameIdTemplateByNameId (mskilltemp.skillName);

		L_ZuheName.text = mName.Name+"("+"2/3"+")"; //大名字

		R_ZuheName.text = mName.Name+"("+"3/3"+")"; //大名字
		
		L_SkillName.text = Skill_Name.Name; //xx小名字

		R_SkillName.text = Skill_Name.Name; //xx小名字

		Init1 ();
		Init2 ();
	}

	void Init1()
	{
		MiBaoSkillTemp mMibaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SkillId);

		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (mMibaoskill.zuhe,2);

		DescIdTemplate DeliSkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.SkillDetail);
		Debug.Log ("mSkill.shuxingDesc2  = "+mSkill.shuxingDesc);
		DescIdTemplate SkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.shuxingDesc);

		L_Skillinstruction.text = SkillDes.description;

		L_instruction.text = DeliSkillDes.description;
		if(mSkill.desc1 != 0)
		{
			DescIdTemplate SHUXIng1Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc1);
			
			L_SHUXIng1.text = SHUXIng1Des.description;
			
			L_shuxing1.text = mSkill.value1;
		}
		
		if(mSkill.desc2 != 0)
		{
			DescIdTemplate SHUXIng2Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc2);
			
			L_SHUXIng2.text = SHUXIng2Des.description;
			
			L_shuxing2.text = mSkill.value2;
		}
		if(mSkill.desc3 != 0)
		{
			DescIdTemplate SHUXIng3Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc3);
			
			L_SHUXIng3.text = SHUXIng3Des.description;
			
			L_shuxing3.text = mSkill.value3;
		}
		if(mSkill.desc4 != 0)
		{
			DescIdTemplate SHUXIng4Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc4);
			
			L_SHUXIng4.text = SHUXIng4Des.description;
			
			L_shuxing4.text = mSkill.value4;
		}
	}
	void Init2()
	{
		MiBaoSkillTemp mMibaoskill = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SkillId);
		
		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (mMibaoskill.zuhe,3);

		DescIdTemplate DeliSkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.SkillDetail);

		Debug.Log ("mSkill.shuxingDesc2  = "+mSkill.shuxingDesc);
		DescIdTemplate SkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.shuxingDesc);

		R_Skillinstruction.text = SkillDes.description;

		R_instruction.text = DeliSkillDes.description;

		if(mSkill.desc1 != 0)
		{
			DescIdTemplate SHUXIng1Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc1);
			
			R_SHUXIng1.text = SHUXIng1Des.description;
			
			R_shuxing1.text = mSkill.value1;
		}
		
		if(mSkill.desc2 != 0)
		{
			DescIdTemplate SHUXIng2Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc2);
			
			R_SHUXIng2.text = SHUXIng2Des.description;
			
			R_shuxing2.text = mSkill.value2;
		}
		if(mSkill.desc3 != 0)
		{
			DescIdTemplate SHUXIng3Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc3);
			
			R_SHUXIng3.text = SHUXIng3Des.description;
			
			R_shuxing3.text = mSkill.value3;
		}
		if(mSkill.desc4 != 0)
		{
			DescIdTemplate SHUXIng4Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc4);
			
			R_SHUXIng4.text = SHUXIng4Des.description;
			
			R_shuxing4.text = mSkill.value4;
		}
	}

}
