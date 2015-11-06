using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class SkillGroup : MonoBehaviour {

	Vector3 endposition;
	Vector3 scale;
	[HideInInspector]public int Skillnum;
	[HideInInspector]public Vector3 Startposition;

	[HideInInspector]public MibaoInfo  M_mibao ;  //	技能暴击减免

	[HideInInspector]public int   minpinzhi ;  //	技能暴击减免
	[HideInInspector]public int  zuhe_id ;  //	技能暴击减免

	int activenum = 0;

	public UILabel SkillIntruduce;
	public UILabel MiBaoName;
	public UILabel SkillShuXing;

	public UILabel SkillDengji;

	public UISprite Skill;
	public UISprite mibao1;
	public UISprite mibao2;
	public UISprite mibao3;

	public UISprite mibaopinzhi1;
	public UISprite mibaopinzhi2;
	public UISprite mibaopinzhi3;

	[HideInInspector]public List<MibaoInfo> MbInfo_ZH = new List<MibaoInfo> ();
	[HideInInspector]public List<UISprite> mibaoicon = new List<UISprite> ();

	[HideInInspector]public List<UISprite> mibaopinzhi = new List<UISprite> ();

	float time = 0.5f;
	void Start () {
	


		endposition = new Vector3 (0,-48,0);
		scale = new Vector3 (1f,1f,1);
		startmove ();


	}

	void startmove()
	{
		iTween.MoveTo(this.gameObject, iTween.Hash("position", endposition, "time",time,"islocal",true));
		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale,"time",time));
	}

	void Update () {
	
	}

	public void init()
	{
		//dubiao
		mibaoicon.Add (mibao1);
		mibaoicon.Add (mibao2);
		mibaoicon.Add (mibao3);

		mibaopinzhi.Add (mibaopinzhi1);
		mibaopinzhi.Add (mibaopinzhi2);
		mibaopinzhi.Add (mibaopinzhi3);
		//ExpXxmlTemp mExpXxmlTemp = ExpXxmlTemp.getExpXxmlTemp_By_expId (mMiBaoXmlTemp.expId,My_mibaoinfo.level);//假设等级为1


		//Debug.Log ("zuhe_id+zuhe_id"+zuhe_id);
		//Debug.Log ("zuhe_id+minpinzhi"+minpinzhi);
		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi (zuhe_id,minpinzhi);

		SkillTemplate mSkillTemplate = SkillTemplate.getSkillTemplateById (mMiBaoSkillTemp.skill);

	//	string MshuxingDesc = mMiBaoSkillTemp.shuxingDesc;


		//Debug.Log ("MshuxingDesc+MshuxingDesc"+MshuxingDesc);


		char[] separator = new char[] { '#' };
		//string[] s = MshuxingDesc.Split (separator);
	
//		string m_s = "";
//		for(int j = 0; j < s.Length; j++ )
//		{
//			m_s += s[j]+"\r\n";
//		}
//
//		SkillShuXing.text = m_s;//属性介绍。。。。
//


		
		string sSkillIntruduce = DescIdTemplate.GetDescriptionById(mSkillTemplate.funDesc);
		string[] sSkiIntruduce = sSkillIntruduce.Split (separator);
		string m_skill = "";
		for(int j = 0; j < sSkiIntruduce.Length; j++ )
		{
			m_skill += sSkiIntruduce[j]+"\r\n";
		}

		SkillIntruduce.text = m_skill;//读取技能介绍表。、。。一个秘宝组的技能介绍

		//Debug.Log ("aba" +SkillIntruduce.text);

		MiBaoName.text = NameIdTemplate.GetName_By_NameId (mSkillTemplate.skillName);

		for(int i = 0; i < MbInfo_ZH.Count; i++)
		{
			MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(MbInfo_ZH[i].miBaoId);
			mibaoicon[i].spriteName = mMiBaoXmlTemp.icon.ToString(); // 读表  暂未实现
			mibaopinzhi[i].spriteName = "pinzhi"+(mMiBaoXmlTemp.pinzhi-1).ToString();
			if(MbInfo_ZH[i].level == 0)
			{
				mibaoicon[i].color = Color.gray;
			}
			else{
				activenum ++;
			}
		}
		//Skill.spriteName = "";//icon读表 表里暂时么有找到
		if(activenum < 3)
		{
			//没有全部激活了  技能显示
			Skill.color = Color.gray;
			SkillDengji.text = " 技能尚未激活";
		}
		else{
			SkillDengji.text = minpinzhi.ToString();
		}
		//char[] separator = new char[] { '/r/n' };
		//Regex reg = new Regex(@"\r\n");string[] s = reg.split(.....);


		//GetSkillCondition.text.Split('\n');
	}
	public void mClose()
	{
		GameObject supGb = GameObject.Find ("Secret(Clone)");
		if(supGb)
		{
			TaskData.Instance.m_DestroyMiBao = false;
			MainCityUI.TryRemoveFromObjectList(supGb);
			Destroy(supGb);
		}
	}
	public void Back()
	{

		scale = new Vector3 (0,0,0);
		iTween.MoveTo(this.gameObject, iTween.Hash("position", Startposition, "time",time,"islocal",true));
		iTween.ScaleTo (this.gameObject,iTween.Hash("scale",scale,"time",time));
		Destroy (this.gameObject,time);
	}


}
