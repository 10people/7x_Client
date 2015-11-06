using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;

public class BaiZhanChooseMiBao : MonoBehaviour, SocketProcessor {

	[HideInInspector]public bool BackFangShou_UI;// 返回界面判断 true为返回防守阵形 false为返回挑战秘宝上阵的界面；
	public	MibaoInfoResp m_MiBaoInfo;//秘宝信息
	public	GameObject m_MiBaoTemp;//秘宝信息
	public	Transform MiBaoTempRoot;//秘宝的父对象

	public UILabel SkillDisActive;
	public UILabel Skill_Active;

	public static List<MiBaotemp> Active_MiBao = new List<MiBaotemp>(); 
	public static bool Skill_isActive;//判断技能是否被激活
	public static int  ZuHe_id;//技能组合ID
	public static int  min_PinZhi;//技能最低品质
	public static List <long> MiBaolist = new List<long> ();//向后台发送选中的秘宝 保存
	//j君主信息。。。。。。。。。。。。。。。。。。。。。。。。
	public UILabel m_UILbaelXiangxiZhanli;
	public UILabel m_UILbaelXiangxiATC;
	public UILabel m_UILbaelXiangxiDEF;
	public UILabel m_UILbaelXiangxiHP;
	public UILabel m_UILbaelXiangxiWQSHJS;
	public UILabel m_UILbaelXiangxiWQBJL;
	public UILabel m_UILbaelXiangxiWQBJJS;
	public UILabel m_UILbaelXiangxiJNSHJS;
	public UILabel m_UILbaelXiangxiJNBJL;
	public UILabel m_UILbaelXiangxiJNBJJS;
	public UILabel m_UILbaelXiangxiWQSHDK;
	public UILabel m_UILbaelXiangxiWQBJDK;
	public UILabel m_UILbaelXiangxiJNSHDK;
	public UILabel m_UILbaelXiangxiJNBJDK;


	[HideInInspector]public BaiZhanInfoResp my_HeroInfo;//我的君主的信息
//	public List<MiBaoBriefInfo> my_MiBaoInfo = new List<MiBaoBriefInfo> ();
	
//	public List<MiBaoBriefInfo> Enemy_MiBaoIfon = new List<MiBaoBriefInfo> ();
	public static BaiZhanChooseMiBao mBaiZhanChooseMiBao;

	public JunZhuInfoRet tempInfo1;

	private bool isReqSaveMiaBao = false;

	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);

	}
	void Start () {

	
	}
	void Update()
	{
		FreshSkill ();

	}
	public void FreshSkill()//检测技能是否被激活
	{
		if(!Skill_isActive)
		{
			SkillDisActive.gameObject.SetActive(true);
			Skill_Active.gameObject.SetActive(false);
		}
		else{
			SkillDisActive.gameObject.SetActive(false);
			Skill_Active.gameObject.SetActive(true);
			MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempByZuHe_Pinzhi(ZuHe_id,min_PinZhi);
			SkillTemplate mSkillTemplate = SkillTemplate.getSkillTemplateById(mMiBaoSkillTemp.skill);
			string mDescIdTemplate = DescIdTemplate.GetDescriptionById(mSkillTemplate.funDesc);

			char[] separator = new char[] { '#' };
			string[] s = mDescIdTemplate.Split (separator);
			
			string m_s = "";
//			for(int j = 0; j < s.Length; j++ )
//			{
//				m_s += s[j]+"\r\n                    ";
//			}
			m_s = s[0];
		

			Skill_Active.text = m_s;
		}
	}
	public void Request_MiBaoInfo()
	{
		mBaiZhanChooseMiBao = this;
		Debug.Log ("发送秘宝请求协议");
		m_MiBaoInfo = MiBaoGlobleData.Instance().G_MiBaoInfo;
		tempInfo1 = (JunZhuInfoRet)JunZhuData.Instance ().m_junzhuInfo.Public_MemberwiseClone();
		InitMiBao_Data(MiBaoGlobleData.Instance().G_MiBaoInfo);

		//SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ,"29604");

		Request_JunZhuInfo (tempInfo1);
	}
	public  void Request_JunZhuInfo(JunZhuInfoRet tempInfo)
	{
		JunZhuInfoRet M_tempInfo = JunZhuData.Instance ().m_junzhuInfo;

		m_UILbaelXiangxiZhanli.text =  MyColorData.getColorString(10, M_tempInfo.zhanLi); 
	
		m_UILbaelXiangxiATC.text = MyColorData.getColorString(10, M_tempInfo.gongJi); 
		m_UILbaelXiangxiDEF.text = MyColorData.getColorString(10, M_tempInfo.fangYu); 
		m_UILbaelXiangxiHP.text = MyColorData.getColorString(10, M_tempInfo.shengMing); 

		m_UILbaelXiangxiWQSHJS.text = "[005fff]" + tempInfo.wqSH.ToString ();
		m_UILbaelXiangxiWQBJL.text = "[005fff]" + tempInfo.wqJM.ToString ();
		m_UILbaelXiangxiWQBJJS.text = "[005fff]" + tempInfo.wqBJ.ToString ();
		m_UILbaelXiangxiJNSHJS.text = "[9200ff]" + tempInfo.wqRX.ToString ();
		m_UILbaelXiangxiJNBJL.text = "[9200ff]" + tempInfo.wqBJL.ToString ()+"%";
		
		m_UILbaelXiangxiJNBJJS.text = "[9200ff]" + tempInfo.jnSH.ToString ();
		m_UILbaelXiangxiWQSHDK.text = "[005fff]" + tempInfo.jnJM.ToString ();
		m_UILbaelXiangxiWQBJDK.text = "[005fff]" + tempInfo.jnBJ.ToString ();
		m_UILbaelXiangxiJNSHDK.text = "[9200ff]" + tempInfo.jnRX.ToString ();
		m_UILbaelXiangxiJNBJDK.text = "[9200ff]" + tempInfo.jnBJL.ToString ()+"%";

	
//		Debug.Log("M_tempInfo.zhanLi="+M_tempInfo.zhanLi);
//		Debug.Log("tempInfo.zhanLi ="+tempInfo.zhanLi);
		if(tempInfo.zhanLi > M_tempInfo.zhanLi)
		{
			m_UILbaelXiangxiZhanli.text += MyColorData.getColorString(4, ("+" + (tempInfo.zhanLi - M_tempInfo.zhanLi)));
		}
		if(tempInfo.gongJi > M_tempInfo.gongJi)
		{
			m_UILbaelXiangxiATC.text += MyColorData.getColorString(4, ("+" + (tempInfo.gongJi - M_tempInfo.gongJi)));

		}
		if(tempInfo.fangYu > M_tempInfo.fangYu)
		{
			m_UILbaelXiangxiDEF.text += MyColorData.getColorString(4, ("+" + (tempInfo.fangYu - M_tempInfo.fangYu)));
		}
		if(tempInfo.shengMing > M_tempInfo.shengMing)
		{
			m_UILbaelXiangxiHP.text += MyColorData.getColorString(4, ("+" + (tempInfo.shengMing - M_tempInfo.shengMing)));
		}

	}
	public int m_BattleType; // 1为荒野资源点  2为百战 3为PVE 4为藏宝点

	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
//			case ProtoIndexes.S_MIBAO_INFO_RESP:
//			{
//				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				MibaoInfoResp MiBaoInfo = new MibaoInfoResp();
//				
//				t_qx.Deserialize(t_stream, MiBaoInfo, MiBaoInfo.GetType());
//				Debug.Log ("秘宝协议返回。。");
//				m_MiBaoInfo = MiBaoInfo;
//				if(m_MiBaoInfo.mibaoInfo != null)
//				{
//					InitMiBao_Data(m_MiBaoInfo);
//				}
//				else{
//					
//					Debug.Log("m_MiBaoInfo.mibaiInfo == null" );
//					return  false;
//				}
//
//				
//				return true;
//			}
			case ProtoIndexes.S_MIBAO_SELECT_RESP: //      318 还没修改协议 
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				MibaoSelectResp Sava_MiBao = new MibaoSelectResp();
				
				t_qx.Deserialize(t_stream, Sava_MiBao, Sava_MiBao.GetType());

				Debug.Log ("MiBao Save Resq Back = "+Sava_MiBao.success);
				if(Sava_MiBao.success == 1)//保存成功
				{
					if(m_BattleType == 2)
					{
						isReqSaveMiaBao = true;
						BaiZhanData.Instance().BaiZhanReq();//pvp
					}
//					if(m_BattleType == 1)
//					{
//						HYResoureEnemy.Instance().SendMessage_HYResoures();//资源点
//					}
//					if(m_BattleType == 3)
//					{
//						//pve
//						PveLevelUImaneger.mPveLevelUImaneger.sendLevelDrop(PveLevelUImaneger.mPveLevelUImaneger.Lv_Info.guanQiaId);
//						MapData.mapinstance.GuidLevel = -2;
//						MapData.mapinstance.ShowPVEGuid();
//					}
					if(m_BattleType == 4)
					{
						HYRetearceEnemy.Instance().Init();//资源点
					}
					Destroy(this.gameObject);
				}
				else{//保存失败
					return false;
				}
				return true;
			}
				
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	public GameObject IsBeChoosed_Root;

//	public List<MiBaoBriefInfo> myMiBaoInfo = new List<MiBaoBriefInfo>();
	public List<long> my_RESMiBaodbid = new List<long>();


	void InitMiBao_Data(MibaoInfoResp m_miBao)//初始化密保表
	{
//
//		BeChoose_miBao_Bg m_BeChoose_miBao_Bg = IsBeChoosed_Root.GetComponent<BeChoose_miBao_Bg>();
//		m_BeChoose_miBao_Bg.AddMiBaoPosition ();
//		MiBaolist.Clear ();
//		Active_MiBao.Clear ();
//		List<long> MiBaodbid = new List<long> ();
//		MiBaodbid.Clear();
//		for(int i = 0; i < m_miBao.mibaoInfo.Count; i++)
//		{
//			if(m_miBao.mibaoInfo[i].level > 0)//如果秘宝是激活状态 显示在选择框内；
//			{
//				GameObject mMiBao = Instantiate(m_MiBaoTemp) as GameObject;
//				mMiBao.SetActive(true);
//				mMiBao.transform.parent = MiBaoTempRoot;
//
//				mMiBao.transform.localPosition = m_MiBaoTemp.transform.localPosition;
//				mMiBao.transform.localScale = m_MiBaoTemp.transform.localScale;
//
//				MiBaotemp mMiBaotemp = mMiBao.GetComponent<MiBaotemp>();
//				mMiBaotemp.mMiBaoinfo = m_miBao.mibaoInfo[i];
//				mMiBaotemp.init();
//				Active_MiBao.Add(mMiBaotemp);
//			}
//			//Debug.Log("m_BattleType = "+m_BattleType);
//			if(m_BattleType == 1)
//			{
//				foreach(int mymibao in my_RESMiBaodbid)
//				{
//
//					//MiBaoXmlTemp mmiBaoTemp =  MiBaoXmlTemp.getMiBaoXmlTempById(mymibao.miBaoId);
//					if(mymibao > 0&&m_miBao.mibaoInfo[i].dbId == mymibao)
//					{
//						Debug.Log("mymibaodbid = "+mymibao);
//						Debug.Log("mymibaoid = "+m_miBao.mibaoInfo[i].miBaoId);
//						MiBaodbid.Add(m_miBao.mibaoInfo[i].dbId);
//						
//						m_BeChoose_miBao_Bg.m_MiBaoinfo = m_miBao.mibaoInfo[i];
//						m_BeChoose_miBao_Bg.Instan_MiBao (Vector3.zero,true);
//					}
//				}
//			}
//			if(m_BattleType == 2)
//			{
//				foreach(MiBaoBriefInfo mymibao in myMiBaoInfo)
//				{
//					//MiBaoXmlTemp mmiBaoTemp =  MiBaoXmlTemp.getMiBaoXmlTempById(mymibao.miBaoId);
//					if(m_miBao.mibaoInfo[i].miBaoId == mymibao.miBaoId)
//					{
//						MiBaodbid.Add(m_miBao.mibaoInfo[i].dbId);
//
//						m_BeChoose_miBao_Bg.m_MiBaoinfo = m_miBao.mibaoInfo[i];
//						m_BeChoose_miBao_Bg.Instan_MiBao (Vector3.zero,true);
//					}
//				}
//
//			}
//			if(m_BattleType == 3)
//			{
//				MiBaodbid = PveLevelUImaneger.GuanqiaReq.mibaoIds;
//
//				foreach(int mb_bdid in MiBaodbid)
//				{
//					//Debug.Log("mb_bdid = "+mb_bdid);
//					if(mb_bdid > 0&&m_miBao.mibaoInfo[i].dbId == mb_bdid)
//					{
//
//						m_BeChoose_miBao_Bg.m_MiBaoinfo = m_miBao.mibaoInfo[i];
//						bool IsSave = true;
//						m_BeChoose_miBao_Bg.Instan_MiBao (Vector3.zero,IsSave);
//					}
//				}
//			}
//			if(m_BattleType == 4)
//			{
//				foreach(int mymibao in my_RESMiBaodbid)
//				{
//					
//					//MiBaoXmlTemp mmiBaoTemp =  MiBaoXmlTemp.getMiBaoXmlTempById(mymibao.miBaoId);
//					if(mymibao > 0&&m_miBao.mibaoInfo[i].dbId == mymibao)
//					{
//						Debug.Log("mymibao = "+mymibao);
//						MiBaodbid.Add(m_miBao.mibaoInfo[i].dbId);
//						
//						m_BeChoose_miBao_Bg.m_MiBaoinfo = m_miBao.mibaoInfo[i];
//						m_BeChoose_miBao_Bg.Instan_MiBao (Vector3.zero,true);
//					}
//				}
//			}
//		}
//		MiBaoTempRoot.GetComponent<UIGrid> ().repositionNow = true;//秘宝重新排序
//
//		if (Active_MiBao.Count > 6)
//		{
//			MiBaoTempRoot.GetComponent<ItemTopCol> ().enabled = false;
//		}
//
//		else 
//		{
//			MiBaoTempRoot.GetComponent<ItemTopCol> ().enabled = true;
//		}

	}
	public void Delet_Btn()
	{
		//删除UI
//		Destroy (this.gameObject);
		BaiZhanData.Instance ().CloseBaiZhan ();
	}
	public void Back_Btn()
	{
		if (m_BattleType == 2)
		{
			if (MiBaolist.Count == 0)
			{
				if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
				{
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
				}
			}
			else
			{
				if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
				{
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
					if (isReqSaveMiaBao)
					{
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[8]);
					}
					else
					{
						UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
					}
				}
			}
		}
		Destroy (this.gameObject);
	}

	public void SavaMiBao_Btn()
	{
		//发送秘宝保存请求。。。。
//		SaveMiBaoReq i_d = new SaveMiBaoReq ();


		MibaoSelect Mibaoid = new MibaoSelect ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
		//Debug.Log ("MiBao Save Resq m_BattleType = "+m_BattleType);
		if(m_BattleType == 1)
		{
			Mibaoid.type = 4; // 1Pve  2 PVP  3 TreS  4 Res
		}
		if(m_BattleType == 2)
		{
			Mibaoid.type = 2; //PVP 

			if (MiBaolist.Count == 0)
			{
				if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
				{
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[5]);
				}
			}
			else
			{
				if(FreshGuide.Instance().IsActive(100180) && TaskData.Instance.m_TaskInfoDic[100180].progress >= 0)
				{
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100180];
					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[8]);
				}
			}
		}
		if(m_BattleType == 3)
		{
			Mibaoid.type = 1;// 1Pve

		}
		if(m_BattleType == 4)
		{
			Mibaoid.type = 3;// TreS
			
		}
		//Debug.Log ("MiBaolist.Count = "+MiBaolist.Count);
		if(MiBaolist.Count < 3)
		{
			for(int j = MiBaolist.Count; j < 3; j++ )
			{
				MiBaolist.Add(-1);
			}
		}
		foreach(int m_id in MiBaolist)
		{
			Debug.Log("Save _m_id = "+m_id);
		}
		Mibaoid.mibaoIds = MiBaolist;
		MiBaoSer.Serialize (miBaoStream,Mibaoid);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT,ref t_protof);

	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
