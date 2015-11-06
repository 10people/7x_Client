using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class BeChoose_miBao_Bg : MonoBehaviour {

	public MibaoInfo m_MiBaoinfo;
	public GameObject m_BeChoose_MiBao;

	public List<MibaoInfo> bechooseMiBao = new List<MibaoInfo>();//被选择的秘宝

	[HideInInspector]public Vector3 position1 ;
	[HideInInspector]public Vector3 position2 ;
	[HideInInspector]public Vector3 position3 ;
	public List<Vector3> poses = new List<Vector3>();
	public List<GameObject> MiBaochoose = new List<GameObject>();

	int pinzhi = 5;
	int [] zuhe_id = new int[3];
	void Start () {


	}
	public void AddMiBaoPosition()
	{
		position1 = new Vector3(-130,0,0);
		position2 = new Vector3(0,0,0);
		position3 = new Vector3(130,0,0);
		poses.Clear ();
		poses.Add (position1);
		poses.Add (position2);
		poses.Add (position3);
		//Debug.Log("poses[i] = "+poses[0]);

	}
	public void SortMiBao()
	{
		if(MiBaochoose.Count == 0)return;
		for(int i = 0; i < MiBaochoose.Count; i++)
		{
			
			TweenPosition.Begin(MiBaochoose[i], 0.2f, poses[i]);
		}
		
	}

	public void SortSavaMiBao()
	{
		if(MiBaochoose.Count == 0)return;
		for(int i = 0; i < MiBaochoose.Count; i++)
		{
			//Debug.Log("poses[i] = "+poses[i]);
			MiBaochoose[i].transform.localPosition =  poses[i];
		}
		
	}

	void Update () {
	

		if(MiBaochoose.Count == 3)
		{

			for(int i = 0; i < bechooseMiBao.Count; i++)
			{
				MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById(bechooseMiBao[i].miBaoId);
				if(mMiBaoXmlTemp.pinzhi < pinzhi)
				{
					pinzhi = mMiBaoXmlTemp.pinzhi;

				}
				zuhe_id[i] = mMiBaoXmlTemp.zuheId;
			}
			if(zuhe_id[0] == zuhe_id[1]&&zuhe_id[1] == zuhe_id[2])
			{
				BaiZhanChooseMiBao.Skill_isActive = true;
				BaiZhanChooseMiBao.ZuHe_id = zuhe_id[0];
				BaiZhanChooseMiBao.min_PinZhi = pinzhi;

			}
			else{
				BaiZhanChooseMiBao.Skill_isActive = false;
			}

		}
		else{
			BaiZhanChooseMiBao.Skill_isActive = false;
		}
	}
	public void Instan_MiBao(Vector3 pos, bool IsSavaMiBao)
	{
//		if(MiBaochoose.Count < 3)
//		{
//			GameObject mMiBao = Instantiate (m_BeChoose_MiBao) as GameObject;
//			mMiBao.SetActive(true);
//			mMiBao.transform.parent = m_BeChoose_MiBao.transform.parent;
//			mMiBao.transform.localPosition = pos;
//			mMiBao.transform.localScale = m_BeChoose_MiBao.transform.localScale;
//			MiBaochoose.Add(mMiBao);
//
//			beChoosedMiBaoTemp mbeChoosedMiBaoTemp = mMiBao.GetComponent<beChoosedMiBaoTemp>();
//			mbeChoosedMiBaoTemp.mMiBaoinfo = m_MiBaoinfo;
//	
//			JunZhuInfoRet Junzu_Data = BaiZhanChooseMiBao.mBaiZhanChooseMiBao.tempInfo1;
//
//			Debug.Log(Global.getZhanli( Junzu_Data));
//			Debug.Log(Global.getZhanli( JunZhuData.Instance ().m_junzhuInfo));
//
//			Junzu_Data.shengMing += m_MiBaoinfo.shengMing;
//			Junzu_Data.gongJi += m_MiBaoinfo.gongJi;
//			Junzu_Data.fangYu += m_MiBaoinfo.fangYu;
//			Junzu_Data.wqSH += m_MiBaoinfo.wqSH;
//			Junzu_Data.wqJM += m_MiBaoinfo.wqJM;
//			Junzu_Data.wqBJ += m_MiBaoinfo.wqBJ;
//			Junzu_Data.wqRX += m_MiBaoinfo.wqRX;
//			Junzu_Data.jnSH += m_MiBaoinfo.jnSH;
//			Junzu_Data.jnJM += m_MiBaoinfo.jnJM;
//			Junzu_Data.jnBJ += m_MiBaoinfo.jnBJ;
//			Junzu_Data.jnRX += m_MiBaoinfo.jnRX;
//
//			Junzu_Data.zhanLi = Global.getZhanli( Junzu_Data);
//
//			BaiZhanChooseMiBao.mBaiZhanChooseMiBao.Request_JunZhuInfo(Junzu_Data);
//
//			mbeChoosedMiBaoTemp.init();
//			//Debug.Log("IsSavaMiBao = "+IsSavaMiBao);
//			if(IsSavaMiBao)
//			{
//				SortSavaMiBao();
//			}else
//			{
//				//Debug.Log("MiBaochoose.Count = "+MiBaochoose.Count);
//				float dis = Vector3.Distance(MiBaochoose[MiBaochoose.Count - 1].transform.localPosition,poses[MiBaochoose.Count - 1]);
//				float m_v = 2000.0f;
//				float mTime = dis/m_v;
//				TweenPosition.Begin(MiBaochoose[MiBaochoose.Count - 1], mTime, poses[MiBaochoose.Count - 1]);
//				if (UIYindao.m_UIYindao.m_isOpenYindao && GetPveTempID.CurLev == 100205)
//				{
//					MapData.mapinstance.GuidLevel = -1;
//					MapData.mapinstance.ShowPVEGuid();
//				}
//			}
//
//			//Debug.Log("m_MiBaoinfo.miBaoId = "+m_MiBaoinfo.miBaoId);
//			MiBaoXmlTemp mMiBaoXmlTemp = MiBaoXmlTemp.getMiBaoXmlTempById (m_MiBaoinfo.miBaoId);
//			//Debug.Log("mMiBaoXmlTemp.zuheid = "+mMiBaoXmlTemp.zuheId);
//			bechooseMiBao.Add(m_MiBaoinfo);
//
//			BaiZhanChooseMiBao.MiBaolist.Add(m_MiBaoinfo.dbId);
//			//Debug.Log("m_MiBaoinfo.dbId = "+m_MiBaoinfo.dbId);
//		}
//		else
//		{
//			// Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.MI_BAO_REMIND_MI_BAO ),Loadback);
//			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),Loadback);
//		}
	}
	void Loadback(ref WWW p_www,string p_path, Object p_object)
	{
		

		string Title = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string Str1 = LanguageTemplate.GetText (LanguageTemplate.Text.HUANGYE_20);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		uibox.setBox(Title,null, Str1,null,confirmStr,null,null,null,null);

	}
}
