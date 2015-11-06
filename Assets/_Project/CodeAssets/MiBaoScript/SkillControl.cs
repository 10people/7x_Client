using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class SkillControl : MonoBehaviour {

//	public GameObject mZHPiece;
//	public GameObject SkillGroup;
//	public GameObject MibaoTempp;
	public GameObject SkillGroupBox;

	[HideInInspector]public List<MibaoInfo> mMibaoInfo_ZH = new List<MibaoInfo> ();

	GameObject mSkillGroup;
	GameObject mMiBao;
	GameObject mNoPiece;

	Vector3 skillPos1;
//	Vector3 endposition ;

	[HideInInspector]public  Vector3 Starposition ;
	[HideInInspector]public  int mID ;

	[HideInInspector]public  int MinPZ ;
	[HideInInspector]public  int ZH_ID ;

	[HideInInspector]public MibaoInfo  M_mibaoinfo ;  //	技能暴击减免
	void Start () {
	
//		endposition = new Vector3 (0,200,0);

	}
	

	void Update () {
	
		if(mSkillGroup||mMiBao)
		{
			SkillGroupBox.SetActive(true);
		}
		else{
			SkillGroupBox.SetActive(false);
		}

	}
	public void mClose()
	{


	}

	void LoadShowpathBack(ref WWW p_www,string p_path, Object p_object)
	{
		mNoPiece = Instantiate ( p_object )as GameObject;
		
		mNoPiece.SetActive (true);
		
		mNoPiece.transform.parent = this.transform;
		
		mNoPiece.transform.localScale = new Vector3 (1,1,1);
		
		mNoPiece.transform.localPosition = new Vector3 (0,-46,0);
		
		MadeMiBao mMadeMiBao = mNoPiece.GetComponent<MadeMiBao> ();
		
		mMadeMiBao.myZuHe_mibao = M_mibaoinfo;
		
		mMadeMiBao.Init ();
		if (FreshGuide.Instance().IsActive(300210) && TaskData.Instance.m_TaskInfoDic[300210].progress >= 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[300210];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}
	}

	void LoadShowpathBack22(ref WWW p_www,string p_path, Object p_object)
	{
		mNoPiece = Instantiate ( p_object )as GameObject;
		
		mNoPiece.SetActive (true);
		
		mNoPiece.transform.parent = this.transform;
		
		mNoPiece.transform.localScale = new Vector3 (1,1,1);

		mNoPiece.transform.localPosition = new Vector3 (0,-46,0);
		
		NoEnughPiece mNoEnughPiece = mNoPiece.GetComponent<NoEnughPiece> ();
		mNoEnughPiece.my_mibao = M_mibaoinfo;
		mNoEnughPiece.Init ();
	}

	public 	void ShowPicePath()
	{
		if(mNoPiece)
		{
			return;
		} // 判断是弹出合成UI还是弹出 获取路劲
		//Debug.Log ("M_mibaoinfo.tempId = "+M_mibaoinfo.tempId);
		MiBaoSuipianXMltemp mMiBaoSuipianXMltemp = MiBaoSuipianXMltemp.getMiBaoSuipianXMltempBytempid (M_mibaoinfo.tempId);
		if(M_mibaoinfo.suiPianNum >= mMiBaoSuipianXMltemp.hechengNum )//假如 碎片数多余合成需要的个数 弹出合成UI
		{

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_START_MADE ),LoadShowpathBack);

		}else{ //不足时弹出获取路径。。。。。。。。。

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_NOT_ENOUGH_PIECE ),LoadShowpathBack22);


		}
	
	}
	
	public void Init()
	{
		
		createSkillgroup (Starposition,1);
	}

	void LoadcreateSkillgroupBack(ref WWW p_www,string p_path, Object p_object)
	{
		mSkillGroup = Instantiate (p_object) as GameObject;
		
		mSkillGroup.SetActive (true);
		
		mSkillGroup.transform.parent = this.transform;
		
		mSkillGroup.transform.localPosition = skillPos1;
		
		mSkillGroup.transform.localScale = new Vector3 (0,0,0);
		
		SkillGroup m_SkillGroup = mSkillGroup.GetComponent<SkillGroup>();

		m_SkillGroup.MbInfo_ZH = mMibaoInfo_ZH;
		
	//	Debug.Log ("mMibaoInfo_ZH"+mMibaoInfo_ZH.Count);
		m_SkillGroup.zuhe_id = ZH_ID;
		m_SkillGroup.minpinzhi = MinPZ;
		m_SkillGroup.M_mibao = M_mibaoinfo;
		m_SkillGroup.Startposition = skillPos1;
		m_SkillGroup.init ();
	}

	void createSkillgroup(Vector3 skillPos,int a)
	{
		if(mSkillGroup)
		{
			return;
		}
		skillPos1 = skillPos;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_SKILL_GROUP ),LoadcreateSkillgroupBack);

	}

	void LoadcreateMiBaoBack(ref WWW p_www,string p_path, Object p_object)
	{
		mMiBao = Instantiate ( p_object )as GameObject;
		
		mMiBao.SetActive (true);
		
		mMiBao.transform.parent = this.transform;
		
		mMiBao.transform.localPosition = skillPos1;
		
		mMiBao.transform.localScale = new Vector3 (0,0,0);
		
		MiBaoCard mMiBaoCard = mMiBao.GetComponent<MiBaoCard>();
		
		mMiBaoCard.My_mibaoinfo = M_mibaoinfo;

		mMiBaoCard.Startposition = skillPos1;
		
		mMiBaoCard.init ();

	}


    public 	void createMiBao(Vector3 MiBaoPos,int a)
	{
		if(mMiBao)
		{
			return;
		}
		skillPos1 = MiBaoPos;
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_MI_BAO ),LoadcreateMiBaoBack);

	}
}
