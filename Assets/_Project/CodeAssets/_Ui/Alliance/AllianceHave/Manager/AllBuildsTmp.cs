using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllBuildsTmp : MonoBehaviour {

	public UILabel mName;

	public UISprite mIcon;

	public UILabel mLevel;

	public GameObject UpBtn;

	public GameObject GreenUpBtn;

	public int  id;

	public int  lv;

	public int Max_Lv = 10;

	public int Build;//拥有建设值

	public int AllianceLevel;//

	public UILabel OnBtnLabel;

	private int costbuild;

	public string BuildName;

	public int Identity ;
	void Start () {
	
	}

	void Update () {
	
	}
	public void Init()
	{
		Build = NewAlliancemanager.Instance ().m_allianceHaveRes.build;

		AllianceLevel = NewAlliancemanager.Instance ().m_allianceHaveRes.level;

		Identity = NewAlliancemanager.Instance ().m_allianceHaveRes.identity;

		GetBuldType ();
	}
	void GetBuldType()
	{
		//Debug.Log ("id =    "+id);
		if (Identity == 0) {
			
			UpBtn.SetActive (false);
			
			GreenUpBtn.SetActive (false);
		}
		else {
			
			GreenUpBtn.SetActive (true);
		}
		switch(id)
		{
		case 1:

			if(Max_Lv > lv)
			{
				LianMengKeZhanTemplate m_LianMengKeZhanTemplate = LianMengKeZhanTemplate.GetLianMengKeZhanTemplate_by_lev(lv+1);
				 
				costbuild = m_LianMengKeZhanTemplate.keZhan_lvUp_value;
				if( AllianceLevel >= m_LianMengKeZhanTemplate.alliance_lv_needed&&Identity != 0)
				{
					UpBtn.SetActive(true);
				}else
				{
					UpBtn.SetActive(false);
				}
			}
			else
			{
				UpBtn.SetActive(true);
			}
		
			BuildName = "联盟客栈";

			mName.text = "联盟客栈Lv."+lv.ToString();

			NewAlliancemanager.Instance ().KezhanLev = lv;
			break;
		case 2:

			if(Max_Lv > lv)
			{
				LianMengShuYuanTemplate m_LianMengShuYuanTemplate = LianMengShuYuanTemplate.GetLianMengShuYuanTemplate_by_shuYuanLevel(lv+1);
				costbuild = m_LianMengShuYuanTemplate.shuYuan_lvUp_value;
				if( AllianceLevel >= m_LianMengShuYuanTemplate.alliance_lv_needed&&Identity != 0)
				{
					UpBtn.SetActive(true);
				}else
				{
					UpBtn.SetActive(false);
				}
			}
			else
			{
				UpBtn.SetActive(true);
				UpBtn.GetComponent<BoxCollider>().enabled = false;
				OnBtnLabel.text = "已达最高";
			}
			BuildName = "联盟书院";
			mName.text = "联盟书院Lv."+lv.ToString();
			NewAlliancemanager.Instance ().KejiLev = lv;
			break;
		case 3:

			if(Max_Lv > lv)
			{
				LianMengTuTengTemplate m_LianMengTuTengTemplate = LianMengTuTengTemplate.getTuTengAwardByLevel(lv+1);
				costbuild = m_LianMengTuTengTemplate.tuTeng_lvUp_value;
				if( AllianceLevel >= m_LianMengTuTengTemplate.alliance_lv_needed&&Identity != 0)
				{
					UpBtn.SetActive(true);
				}else
				{
					UpBtn.SetActive(false);
				}
			}
			else
			{
				UpBtn.SetActive(true);
				UpBtn.GetComponent<BoxCollider>().enabled = false;
				OnBtnLabel.text = "已达最高";
			}
			BuildName = "联盟图腾";
			mName.text = "联盟图腾Lv."+lv.ToString();
			NewAlliancemanager.Instance ().TutengLev = lv;
			break;
		case 4:

			if(Max_Lv > lv)
			{
				LianMengShangPuTemplate m_LLianMengShangPuTemplate = LianMengShangPuTemplate.GetLianMengShangPuTemplate_by_lv(lv+1);
				costbuild = m_LLianMengShangPuTemplate.shangPu_lvUp_value;
				if( AllianceLevel >= m_LLianMengShangPuTemplate.alliance_lv_needed&&Identity != 0)
				{
					UpBtn.SetActive(true);
				}else
				{
					UpBtn.SetActive(false);
				}
			}
			else
			{
				UpBtn.SetActive(true);
				UpBtn.GetComponent<BoxCollider>().enabled = false;
				OnBtnLabel.text = "已达最高";
			}
			BuildName = "联盟商铺";
			mName.text = "联盟商铺Lv."+lv.ToString();
			NewAlliancemanager.Instance ().ShangPuLev = lv;
			break;
		case 5:
			if(Max_Lv > lv)
			{
				LianMengZongMiaoTemplate m_LianMengZongMiaoTemplate = LianMengZongMiaoTemplate.GetLianMengZongMiaoTemplate_by_lev(lv+1);
				costbuild = m_LianMengZongMiaoTemplate.zongMiao_lvUp_value;
				if( AllianceLevel >= m_LianMengZongMiaoTemplate.alliance_lv_needed&&Identity != 0)
				{
					UpBtn.SetActive(true);
				}else
				{
					UpBtn.SetActive(false);
				}
			}
			else
			{
				UpBtn.SetActive(true);
				UpBtn.GetComponent<BoxCollider>().enabled = false;
				OnBtnLabel.text = "已达最高";
			}
			BuildName = "联盟宗庙";
			mName.text = "联盟宗庙Lv."+lv.ToString();
			NewAlliancemanager.Instance ().ZongmiaoLev = lv;
			break;
		default:
			break;
		}
	
	}
	public void Enter()
	{
		NewAlliancemanager.Instance ().ENterOtherUI (id);
	}
	public void UpGreed()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),UpComform);
	}
	void UpComform(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"确定将"+BuildName+"升级吗？" +"\r\n"+"\r\n"+"需要消耗联盟建设值："+costbuild.ToString();//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,CancleBtn,confirmStr,UpCom,null,null);
	}
	void UpCom(int i)
	{
		if (i == 2) {
			if (Build < costbuild) 
			{
				Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LackOfBuildLoadBack);
			}
			else
			{
				NewAlliancemanager.Instance ().m_allianceHaveRes.build = NewAlliancemanager.Instance ().m_allianceHaveRes.build -costbuild;
				NewAlliancemanager.Instance ().ShowJianSheZhi();
				ErrorMessage BuildUp = new ErrorMessage ();
				
				MemoryStream MiBaoStream = new MemoryStream ();
				
				QiXiongSerializer MiBaoer = new QiXiongSerializer ();
				Debug.Log ("Build-id = "+id);
				BuildUp.errorCode = id;
		
				NewAlliancemanager.Instance ().Up_id = id;
		
				MiBaoer.Serialize (MiBaoStream,BuildUp);
				
				byte[] t_protof;
				t_protof = MiBaoStream.ToArray();
				SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_JIAN_ZHU_UP,ref t_protof);
			}
		}
	}
	void LackOfBuildLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "\r\n"+"建设值不足，无法升级该建筑" ;//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str1), null,null,confirmStr,null,null,null);
	}
}
