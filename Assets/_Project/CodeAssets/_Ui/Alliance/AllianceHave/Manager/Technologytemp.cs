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

	public UISprite Icon;

	public UILabel CurrInstrudction;

	public UILabel NextInstrudction;

	public UILabel CostBuilds;

	public int  Alliance_Builds;

	public UILabel level;

	public GameObject ResearchBtn;

	public GameObject Green_ResearchBtn;

	public int Keji_id;

	public int Keji_type;

	public int Keji_level;

	public int MaxKeji_level = 10;

	public int ShuYuanLevel;

	public int Identity;
	LianMengKeJiTemplate mLianMengKeJiTemplate;
	void Start () {
	
	}
	

	void Update () {
		Alliance_Builds = NewAlliancemanager.Instance ().m_allianceHaveRes.build;
	}
	public void Init()
	{
		Identity = NewAlliancemanager.Instance ().m_allianceHaveRes.identity;
		ShuYuanLevel = NewAlliancemanager.Instance ().KejiLev;
//		Debug.Log ("Identity = "+Identity);
//		Debug.Log ("ShuYuanLevel = "+ShuYuanLevel);
		if(Identity == 0)
		{
			Green_ResearchBtn.SetActive(false);
			ResearchBtn.SetActive(false);
		}
		else
		{
			Green_ResearchBtn.SetActive(true);
			if(Keji_level >= ShuYuanLevel )
			{
				ResearchBtn.SetActive(false);
			}
			else
			{
				ResearchBtn.SetActive(true);
			}
		}
		level.text = "Lv " + Keji_level.ToString ();

		 mLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,Keji_level);
//		Debug.Log ("Keji_type = "+Keji_type);
//		Debug.Log ("Keji_level = "+Keji_level);
//		Debug.Log ("mLianMengKeJiTemplate.desc = "+mLianMengKeJiTemplate.desc);
		CurrInstrudction.text = mLianMengKeJiTemplate.desc;
		Icon.spriteName = mLianMengKeJiTemplate.Icon.ToString ();
		if(Keji_level < MaxKeji_level)
		{
			LianMengKeJiTemplate NextLianMengKeJiTemplate = LianMengKeJiTemplate.GetLianMengKeJiTemplate_by_Type_And_Level (Keji_type,Keji_level+1);
			NextInstrudction.text = NextLianMengKeJiTemplate.desc;
		}
		else
		{
			NextInstrudction.gameObject.SetActive(false);
		}
		CostBuilds.text = "消耗建设值："+mLianMengKeJiTemplate.lvUpValue.ToString ();
	}
	public void Research()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpComform);
	}
	void UpComform(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"确定将"+mLianMengKeJiTemplate.name+"升级吗？" +"\r\n"+"\r\n"+"需要消耗联盟建设值："+mLianMengKeJiTemplate.lvUpValue.ToString();//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,CancleBtn,confirmStr,UpCom,null,null);
	}
	void UpCom(int i)
	{
		if (i == 2) {
			if (mLianMengKeJiTemplate.lvUpValue > Alliance_Builds) 
			{
				Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LackOfBuildLoadBack);
			}
			else
			{
				NewAlliancemanager.Instance ().m_allianceHaveRes.build = NewAlliancemanager.Instance ().m_allianceHaveRes.build -mLianMengKeJiTemplate.lvUpValue;
				NewAlliancemanager.Instance ().ShowJianSheZhi();
				ErrorMessage KeJiUp = new ErrorMessage ();
				
				MemoryStream MiBaoStream = new MemoryStream ();
				
				QiXiongSerializer MiBaoer = new QiXiongSerializer ();
				Debug.Log ("bulids  = "+NewAlliancemanager.Instance ().m_allianceHaveRes.build);
				KeJiUp.errorCode = Keji_type;
				TechnologyManager.Instance().CurKejiType = Keji_type;
				//NewAlliancemanager.Instance ().Up_id = id;
				
				MiBaoer.Serialize (MiBaoStream,KeJiUp);
				
				byte[] t_protof;
				t_protof = MiBaoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LMKJ_UP,ref t_protof);
			}
		}
	}
	void LackOfBuildLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"建设值不足，无法研究该科技" ;//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null);
	}
}
