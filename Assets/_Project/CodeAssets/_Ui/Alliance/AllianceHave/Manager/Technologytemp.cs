using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class Technologytemp : MonoBehaviour {

	public GameObject mAlert;

	public UILabel REmind;

	public UISprite Icon;

	public UILabel CurrInstrudction;

	public UILabel NextInstrudction;

	public UILabel CostBuilds;

	public int  Alliance_Builds;

	public UILabel level;

	public GameObject ResearchBtn;

	public GameObject Green_ResearchBtn;

	public UILabel Green_ResearchBtnLabel;

	public GameObject mActiveBtn;

	public int Keji_id;

	public int Keji_type;

	public int Keji_level;

	public int MaxKeji_level = 10;

	public int ShuYuanLevel;

	public int Identity;

	public UILabel KeJiName;

	LianMengKeJiTemplate mLianMengKeJiTemplate;

	public int Keji_Index;

	public int JiHuoLv;

	public GameObject BtnList;

	public UILabel NoOpen;
	void Start () {
	
	}
	

	void Update () {
		Alliance_Builds = NewAlliancemanager.Instance().m_allianceHaveRes.build;
	}
	public void Init()
	{
		Alliance_Builds = NewAlliancemanager.Instance().m_allianceHaveRes.build;
		Identity = NewAlliancemanager.Instance().m_allianceHaveRes.identity;
		ShuYuanLevel = NewAlliancemanager.Instance().KejiLev;
		LianMengKeJiTemplate m_LianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,0);
		int OPenLv = m_LianMengKeJiTemplate.shuYuanlvNeeded;
		if(ShuYuanLevel < OPenLv)
		{
			BtnList.SetActive(false);
			NoOpen.text = "书院"+OPenLv.ToString()+"级开放";
		}
		else
		{
			BtnList.SetActive(true);
			NoOpen.text = "";
		}
//		Debug.Log("Identity = "+Identity);
		if(Identity == 0) // 盟员 版
		{

//
//			Debug.Log("Keji_type = "+Keji_type);
//			Debug.Log("JiHuoLv = "+JiHuoLv);
//			Debug.Log("Keji_level = "+Keji_level);
			if(JiHuoLv > MaxKeji_level)
			{
				JiHuoLv = MaxKeji_level;
			}
			if(Keji_level > MaxKeji_level)
			{
				Keji_level = MaxKeji_level;
			}
			level.text = "Lv " + JiHuoLv.ToString ();
			mLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,JiHuoLv);
			if(JiHuoLv < MaxKeji_level)
			{
				LianMengKeJiTemplate NextLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,JiHuoLv+1);
				NextInstrudction.text = NextLianMengKeJiTemplate.desc;
			}
			else
			{
				NextInstrudction.gameObject.SetActive(false);
			}
			CostBuilds.gameObject.SetActive(false);
			ResearchBtn.SetActive(false);
			if(JiHuoLv < Keji_level)
			{
				Green_ResearchBtn.SetActive(false);
				REmind.gameObject.SetActive(false);
				mActiveBtn.SetActive(true);
				mAlert.SetActive(true);
			}
			else
			{
				Green_ResearchBtn.SetActive(true);
				Green_ResearchBtnLabel.text = "激 活";
				mActiveBtn.SetActive(false);
				REmind.gameObject.SetActive(true);
				if(JiHuoLv >= MaxKeji_level )
				{
					REmind.gameObject.SetActive(false);
					Green_ResearchBtnLabel.text = "已到最高";
				}
				mAlert.SetActive(false);
			}
		

		}
		else // 盟主或者副盟主版
		{
			REmind.gameObject.SetActive(false);
			level.text = "Lv " + Keji_level.ToString ();

			mLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,Keji_level);
			if(Keji_level < MaxKeji_level)
			{
				LianMengKeJiTemplate NextLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,Keji_level+1);
				NextInstrudction.text = NextLianMengKeJiTemplate.desc;
			}
			else
			{
				NextInstrudction.gameObject.SetActive(false);
			}
			CostBuilds.gameObject.SetActive(true);
    		CostBuilds.text = "消耗建设值："+mLianMengKeJiTemplate.lvUpValue.ToString ();
			mActiveBtn.SetActive(false);
			if(Keji_level >= MaxKeji_level)
			{
				Green_ResearchBtn.SetActive(true);
				Green_ResearchBtnLabel.text = "已到最高";
				ResearchBtn.SetActive(false);
				mAlert.SetActive(false);
			}
			else
			{
				Green_ResearchBtn.SetActive(false);
				ResearchBtn.SetActive(true);
//				Debug.Log("mLianMengKeJiTemplate.lvUpValue = "+mLianMengKeJiTemplate.lvUpValue);
//				Debug.Log("Alliance_Buildse = "+Alliance_Builds);
				if (mLianMengKeJiTemplate.lvUpValue <= Alliance_Builds &&Keji_level < ShuYuanLevel) 
				{
					mAlert.SetActive(true);
				}
				else
				{
					mAlert.SetActive(false);
				}

			}	
		}

		KeJiName.text = mLianMengKeJiTemplate.name;
		
		CurrInstrudction.text = mLianMengKeJiTemplate.desc;
		Icon.spriteName = mLianMengKeJiTemplate.Icon.ToString ();
	}
	public void Research()
	{
		if(Keji_level>= ShuYuanLevel)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),DOnt_UpComform);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpComform);
		}
	}
	public void ActiveNow()
	{
		JiHuoLMKJReq KeJiUp = new JiHuoLMKJReq ();
		
		MemoryStream MiBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoer = new QiXiongSerializer ();
	
		KeJiUp.keJiType = Keji_type;
		TechnologyManager.Instance().CurKejiType = Keji_type;
		//NewAlliancemanager.Instance().Up_id = id;
		TechnologyManager.Instance().JianZhu_InDex = Keji_Index;
		MiBaoer.Serialize (MiBaoStream,KeJiUp);
		
		byte[] t_protof;
		t_protof = MiBaoStream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKEJI_JIHUO,ref t_protof);

	}
	void DOnt_UpComform(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"科技等级不可超过书院等级,请先升级书院。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null);
	}
	void UpComform(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "确定将"+mLianMengKeJiTemplate.name+"升级吗？" +"\r\n"+"需要消耗联盟建设值："+mLianMengKeJiTemplate.lvUpValue.ToString();//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,CancleBtn,confirmStr,UpCom,null,null);
	}
	string Titie;
	void UpCom(int i)
	{
		if (i == 2) {
			if (mLianMengKeJiTemplate.lvUpValue > Alliance_Builds) 
			{
				Titie  = "建设值不足";
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_LEADER_SETTINGS ),
				                        LeaderSettingsLoadCallback );
				//Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LackOfBuildLoadBack);
			}
			else
			{
				NewAlliancemanager.Instance().m_allianceHaveRes.build = NewAlliancemanager.Instance().m_allianceHaveRes.build -mLianMengKeJiTemplate.lvUpValue;
				NewAlliancemanager.Instance().ShowJianSheZhi();
				ErrorMessage KeJiUp = new ErrorMessage ();
				
				MemoryStream MiBaoStream = new MemoryStream ();
				
				QiXiongSerializer MiBaoer = new QiXiongSerializer ();
			//	Debug.Log ("bulids  = "+NewAlliancemanager.Instance().m_allianceHaveRes.build);
				KeJiUp.errorCode = Keji_type;
				TechnologyManager.Instance().CurKejiType = Keji_type;
				//NewAlliancemanager.Instance().Up_id = id;
				TechnologyManager.Instance().JianZhu_InDex = Keji_Index;
				MiBaoer.Serialize (MiBaoStream,KeJiUp);
				
				byte[] t_protof;
				t_protof = MiBaoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKJ_UP,ref t_protof);
			}
		}
	}
	GameObject LederSet;
	public void LeaderSettingsLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		if(LederSet == null)
		{
			LederSet = Instantiate( p_object ) as GameObject;
			
			LederSet.transform.localScale = Vector3.one;
			LederSet.transform.localPosition = new Vector3 (500,200,0);
			LianmengMuBiaomanager mLianmengMuBiaomanager = LederSet.GetComponent<LianmengMuBiaomanager>();
			//mLianmengMuBiaomanager.Lianmeng_Alliance = m_Alliance;
			
			mLianmengMuBiaomanager.Init(Titie);
			MainCityUI.TryAddToObjectList(LederSet,false);
		}
		
	}
}
