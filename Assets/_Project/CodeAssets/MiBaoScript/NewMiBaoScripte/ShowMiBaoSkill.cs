using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ShowMiBaoSkill : MonoBehaviour,SocketProcessor {

	public UISprite SkillIcon1;

	public UISprite SkillIcon2;

	public UILabel ZuheName;

	public UILabel SkillName;

	public UILabel Skillinstruction;

	public UILabel instruction;

	public UILabel SHUXIng1;

	public UILabel SHUXIng2;

	public UILabel SHUXIng3;

	public UILabel SHUXIng4;

	public UILabel SHUXIng_1;
	
	public UILabel SHUXIng_2;
	
	public UILabel SHUXIng_3;
	
	public UILabel SHUXIng_4;


	public UILabel name_Num;

	public UILabel All_instruction;


	public MibaoInfoResp ShowMiBaoGroupTemp;

	public MibaoGroup ShowMiBaoGroup;

	public List<UISprite> mMBTempt = new List<UISprite>();

	public GameObject SkillFIX;

	public GameObject ActiveBtn;

	public GameObject JinJiebtn;

	private bool CanActive = false;

	private bool CanJinjie = false;

	public GameObject Arts;

	public UILabel LockAndJinJie;
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}

	void Start () {
	
	}


	void Update () {
	
	}
	public int SkillId;
	public void Init()
	{
		CanActive = false;
		
	    CanJinjie = false;

		int MiBaoNum = 0;
		 
		for(int i = 0 ; i < ShowMiBaoGroup.mibaoInfo.Count; i ++)
		{
			MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(ShowMiBaoGroup.mibaoInfo[i].miBaoId);

			mMBTempt[i].spriteName = mMiBaoXmlTemp.icon.ToString();

			if(ShowMiBaoGroup.mibaoInfo[i].level > 0&& !ShowMiBaoGroup.mibaoInfo[i].isLock )
			{
				MiBaoNum += 1;
				mMBTempt[i].color =  Color.white;
			}
			else
			{
				mMBTempt[i].color =  new Color(0,0,0,255);
			}
		}
		Debug.Log ("SkillId = "+SkillId);

		MiBaoSkillTemp mSkill = MiBaoSkillTemp.getMiBaoSkillTempBy_pz_id (MiBaoNum,SkillId);
		
		NameIdTemplate mName = NameIdTemplate.getNameIdTemplateByNameId (mSkill.nameId);
		
		DescIdTemplate mDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.zuheDesc);

		SkillTemplate mskilltemp = SkillTemplate.getSkillTemplateById (mSkill.skill);

		NameIdTemplate Skill_Name = NameIdTemplate.getNameIdTemplateByNameId (mskilltemp.skillName);

		SkillIcon1.spriteName = mSkill.icon.ToString ();
		
		SkillIcon2.spriteName = mSkill.icon.ToString ();

		ZuheName.text = mName.Name; //大名字

		SkillName.text = Skill_Name.Name; //xx小名字

		DescIdTemplate SkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.shuxingDesc);

		Skillinstruction.text = SkillDes.description;

		Debug.Log ("mSkill.SkillDetail = " +mSkill.SkillDetail);

		DescIdTemplate DeliSkillDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.SkillDetail);

		instruction.text = DeliSkillDes.description;


		if(mSkill.desc1 != 0)
		{
			DescIdTemplate SHUXIng1Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc1);
			
			SHUXIng1.text = SHUXIng1Des.description;

			SHUXIng_1.text = mSkill.value1;
		}

		if(mSkill.desc2 != 0)
		{
			DescIdTemplate SHUXIng2Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc2);
			
			SHUXIng2.text = SHUXIng2Des.description;
			
			SHUXIng_2.text = mSkill.value2;
		}
		if(mSkill.desc3 != 0)
		{
			DescIdTemplate SHUXIng3Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc3);
			
			SHUXIng3.text = SHUXIng3Des.description;
			
			SHUXIng_3.text = mSkill.value3;
		}
		if(mSkill.desc4 != 0)
		{
			DescIdTemplate SHUXIng4Des = DescIdTemplate.getDescIdTemplateByNameId (mSkill.desc4);
			
			SHUXIng4.text = SHUXIng4Des.description;
			
			SHUXIng_4.text = mSkill.value4;
		}

		DescIdTemplate All_instructionDes = DescIdTemplate.getDescIdTemplateByNameId (mSkill.SkillSummary);

		All_instruction.text = All_instructionDes.description;

		int number = 0;

		if (ShowMiBaoGroup.hasActive == 1)
		{
			if(ShowMiBaoGroup.hasJinjie == 0)
			{
				number = 2;
			}
			else
			{
				number = 3;
			}
		}

		name_Num.text = mName.Name+"("+number.ToString()+"/"+"3"+")"; //大名字

		int effectId = 100160 ;

		if(ShowMiBaoGroup.hasActive == 0)
		{
			ActiveBtn.SetActive(true);

			JinJiebtn.SetActive(false);
			if(MiBaoNum >= 2)
			{

				CanActive = true;

				LockAndJinJie.gameObject.SetActive(true);

				LockAndJinJie.text = "解锁";

				PushAndNotificationHelper.SetRedSpotNotification (610, true);

				Arts.SetActive(true);

				StartCoroutine("BtnShake");

				if(FreshGuide.Instance().IsActive(100220)&& TaskData.Instance.m_TaskInfoDic[100220].progress>= 0)
				{
					
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100220];
					
					Debug.Log("秘宝技能激活2");
					
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[2]); //选择秘宝

				}
			}
			else
			{
				Arts.SetActive(false);
			}
		}
		else
		{
			ActiveBtn.SetActive(false);

			if(ShowMiBaoGroup.hasJinjie == 0)
			{
				JinJiebtn.SetActive(true);

				if(MiBaoNum >= 3)
				{
					CanJinjie = true;

					LockAndJinJie.gameObject.SetActive(true);

					LockAndJinJie.text = "进阶";

					Arts.SetActive(true);

					StartCoroutine("BtnMove");
				}
				else
				{
					Arts.SetActive(false);
					JinJiebtn.SetActive(false);
					LockAndJinJie.gameObject.SetActive(false);
				}
			}
			else
			{
				LockAndJinJie.gameObject.SetActive(false);
				Arts.SetActive(false);
				JinJiebtn.SetActive(false);
			}
		}

		//特效加载

		//UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.FunctionUI_1,SkillFIX,
		                                              // EffectIdTemplate.GetPathByeffectId(effectId));
	}
	IEnumerator BtnShake()
	{

		while(ShowMiBaoGroup.hasActive == 0)
		{
			yield return new WaitForSeconds (1f);
			
			iTween.ShakePosition(ActiveBtn,new Vector3(0.02f,0.001f,0),1);
			//iTween.ShakeRotation(ActiveBtn,new Vector3(0,0,10f),1);
		}

	}
	int Dir = -1;
	float Speed = 0.03f;
	IEnumerator BtnMove()
	{
		Vector3 starpos = new Vector3 (0,-11,0);;

		while(ShowMiBaoGroup.hasJinjie == 0)
		{

			Vector3 endPos  = starpos+ new Vector3(0,20,0);

			//Debug.Log("move.....le1  =  " +endPos);

			TweenPosition.Begin(JinJiebtn,1f,endPos);

			yield return new WaitForSeconds (1.0f);

			JinJiebtn.transform.localPosition = endPos - new Vector3(0,20,0);

			//Debug.Log("move.....le2  =  " +JinJiebtn.transform.localPosition);
		}
		
	}
	int Activetype ;

	public void OpenLock()
	{
		Activetype = 1;

		Debug.Log ("CanActive = "+CanActive);
		if(CanActive)
		{
			if(FreshGuide.Instance().IsActive(100220)&& TaskData.Instance.m_TaskInfoDic[100220].progress>= 0)
			{
				
				ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100220];
				
				Debug.Log("秘宝技能激活3");
				
				UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]); //选择秘宝

			}
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),MiBaoUpStarLoadBack);
			return;
		}
		else{

			ShowOpenCondition();
		}


	}
	public void jinJie()
	{
		Activetype = 2;

		if(CanJinjie)
		{

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),MiBaoUpStarLoadBack);

			return;
		}
		else
		{
			ShowOpenCondition();
		}


	}
	public void ShowAllInfo()
	{
		MiBaoManager.Instance ().CurrSkill_id = SkillId;;
		
		MiBaoManager.Instance ().SortUI ("MiBaoSkillZuheInfo");

	}

	void MiBaoUpStarLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "";
		string str = "";
		if(Activetype == 1)
		{
			titleStr = "激活";
			str ="\r\n"+"确定激活此技能吗？";
		}
		else{

			titleStr = "进阶";
			str = "\r\n"+"确定进阶此技能吗？";
		}
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancel = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancel,confirm,SendStarUpInfo);
				

	}
	void SendStarUpInfo(int i)
	{
		if( i == 2)
		{
			MiBaoDealSkillReq mMiBaoDealSkillReq = new MiBaoDealSkillReq ();
			
			MemoryStream miBaoStream = new MemoryStream ();
			
			QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
			
			mMiBaoDealSkillReq.zuheId = ShowMiBaoGroup.zuheId;
			
			mMiBaoDealSkillReq.activeOrJinjie = Activetype;
			
			MiBaoSer.Serialize (miBaoStream,mMiBaoDealSkillReq);
			byte[] t_protof;
			t_protof = miBaoStream.ToArray();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.MIBAO_DEAL_SKILL_REQ,ref t_protof);
		}
	}

	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.MIBAO_DEAL_SKILL_RESP://m秘宝激活或者进阶返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MiBaoDealSkillResp mMiBaoDealSkillResp = new MiBaoDealSkillResp();
				
				t_qx.Deserialize(t_stream, mMiBaoDealSkillResp, mMiBaoDealSkillResp.GetType());

				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
				switch(mMiBaoDealSkillResp.message)
				{
				case 10:

					Debug.Log("激活失败");
					break;
				case 11:

					ShowMiBaoGroup.hasActive = 1;
					PushAndNotificationHelper.SetRedSpotNotification (610, false);
					Debug.Log("激活成功");
					Init();
					break;

				case 20:

					Debug.Log("进阶失败");
					break;

				case 21:
					Debug.Log("激活成功");
					ShowMiBaoGroup.hasJinjie = 1;
					SkillId = mMiBaoDealSkillResp.skillId;
					Init();
					break;

				default:
					break;

				}
				UI3DEffectTool.Instance ().ShowTopLayerEffect (UI3DEffectTool.UIType.PopUI_2,SkillTemp,EffectIdTemplate.GetPathByeffectId(100178));
				return true;
			}
				
			default: return false;
			}
			
		}
		
		
		return false;
	}
	public GameObject SkillTemp;
	public void ShowOpenCondition()
	{
		
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),OpenLockLoadBack);
		
	}
	
	void OpenLockLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		string str1 = "";
		string str2 = "";
		if(Activetype == 1)
		{
			str2 = "解锁两个或两个以上秘宝！";

			str1 = "\r\n"+"解锁此技能需要达成以下条件："+"\r\n"+"\r\n"+str2;//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

		}
		else
		{
			str2 = "解锁三个秘宝！";

			str1 = "\r\n"+"进阶此技能需要达成以下条件："+"\r\n"+"\r\n"+str2;//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

		}
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null,null
		             );
	}
}
