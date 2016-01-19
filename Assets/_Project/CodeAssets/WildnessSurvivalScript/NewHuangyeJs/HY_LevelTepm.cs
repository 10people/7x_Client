using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
public class HY_LevelTepm : MonoBehaviour {

	public HuangYeTreasure mHuangYeTreasure;

	public UILabel Lv_Name;

	public UILabel BloodNum;


	[HideInInspector]public int State;

	public UISprite Lv_icon;

	public GameObject Blood;

	public UISlider mUISlider;

	string CancleBtn;

	string confirmStr;

	public GameObject CanActive;
	public GameObject Resttingbg;
	public GameObject DisActive;
	public AllianceHaveResp m_Allance;

	void Start () {
	
		 CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		 confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
	}
	

	void Update () {

	}
	public void Init()
	{
		m_Allance = AllianceData.Instance.g_UnionInfo;//获取联盟的信息

		if(mHuangYeTreasure.isActive == 1)
		{
			//可以被激活
			//Lv_icon.spriteName = "L_Com";
			DisActive.SetActive(false);
			Blood.SetActive(false);

			CanActive.SetActive(true);
			Resttingbg.SetActive(false);
		}
		if(mHuangYeTreasure.isActive == 0)
		{
			// 不可以被激活

			Blood.SetActive(false);
			DisActive.SetActive(true);
			CanActive.SetActive(false);	
			Resttingbg.SetActive(false);
		}
		if(mHuangYeTreasure.isActive == 2)
		{
			//已经激活过了
			Lv_icon.color =new Color(255,255,255,255);
			DisActive.SetActive(false);
			if(mHuangYeTreasure.isOpen == 0)
			{
				Blood.SetActive(false);
				CanActive.SetActive(false);
				Resttingbg.SetActive(true);
			}
			if(mHuangYeTreasure.isOpen == 1)
			{
				Blood.SetActive(true);

				CanActive.SetActive(false);	
				Resttingbg.SetActive(false);

				mUISlider.value = (float)( mHuangYeTreasure.jindu )/ (float)(100);

				BloodNum.text = mHuangYeTreasure.jindu.ToString()+"%";
			}
		}
		HuangYePveTemplate mHuangYePveTemplate = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);
		
		//string mHuangYeDesc = DescIdTemplate.GetDescriptionById (mHuangYePveTemplate.descId);
		
		string mName = NameIdTemplate.GetName_By_NameId (mHuangYePveTemplate.nameId);

		Lv_Name.text = mName;
	}
	public void OpenLevelUI() // 开启宝藏点
	{

		if(mHuangYeTreasure.isActive == 1)
		{
			if(m_Allance.identity == 0)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),CantOpenLoadBack);
			}
			else
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),IsOpenLoadBack);
			}
		}
		if(mHuangYeTreasure.isActive == 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),IsOpenFailLoadBack);
		}
		if(mHuangYeTreasure.isActive == 2)
		{
			if(mHuangYeTreasure.isOpen == 0)
			{
				if(m_Allance.identity == 0|| m_Allance.identity == 1)
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),CantRestingLoadBack);
				}
				else
				{
					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),ResettingLoadBack);
				}

			}
			if(mHuangYeTreasure.isOpen == 1)
			{
				BattleTreasure();
			}
		}


	}
    void BattleTreasure() // 攻击宝藏点
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.HYTREASURE_UI), LoadBack);
	}

	private GameObject ZhengRongUI;
	void LoadBack( ref WWW p_www,string p_path, Object p_object )
	{
		if(ZhengRongUI != null)
		{
			return;
		}

		ZhengRongUI = Instantiate(p_object) as GameObject;
		
		ZhengRongUI.transform.localPosition = new Vector3(0,0,0);
		
		ZhengRongUI.transform.localScale = new Vector3(1,1,1);
		
		HYRetearceEnemy m_HuangyeZhengRong = ZhengRongUI.GetComponent<HYRetearceEnemy>();
		
		m_HuangyeZhengRong.mHuangYeTreasure = mHuangYeTreasure;
		
		m_HuangyeZhengRong.Init();
		MainCityUI.TryAddToObjectList (ZhengRongUI);
		//HY_UIManager.Instance ().NeedCloseObg = ZhengRongUI;
		//Debug.Log ("HY_UIManager.Instance ().NeedCloseObg = " +HY_UIManager.Instance ().NeedCloseObg);
	}

	void ResettingLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "重置关卡";//LanguageTemplate.GetText (LanguageTemplate.Text.RESTTING_CQ_TITLE);

		HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);
		int OpenHuafei = mhuangye.openCost; // read Table

		string str = "是否需要花费"+OpenHuafei.ToString()+"联盟建设值来重置该宝藏点？";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,ComfirmRestingIdentity,null,null);
	}

	void IsOpenFailLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "必须完成之前的宝藏点才能激活该宝藏点！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void IsOpenLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "激活";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);
		int OpenHuafei = mhuangye.openCost; // read Table
		
		string str = "激活该宝藏点需要花费"+OpenHuafei.ToString()+"联盟建设值，是否需要激活？";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
	
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,SureIdentity,null,null);
	}

	void ComfirmRestingIdentity(int i)
	{
		if(i == 2)
		{
			HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);

			int OpenHuafei = mhuangye.openCost; // read Table

			if(m_Allance.build < OpenHuafei)
			{
				// 建设值不够了
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),RestingLackBuildOpenLoadBack);
			}
			else
			{
				SendResstingReqs();
			}
		}
	}

	void SureIdentity(int i)
	{

		if(i == 2)
		{
			HuangYePveTemplate mhuangye = HuangYePveTemplate.getHuangYePveTemplatee_byid (mHuangYeTreasure.fileId);
			int RestingOpenHuafei = mhuangye.openCost; // read Table

			if(m_Allance.build < RestingOpenHuafei)
			{
				// 建设值不够了

				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LackBuildOpenLoadBack);
			}
			else
			{
				SendActiveReqs();

			}

		}
	}

	void CantRestingLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "重置失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "只有盟主才能重置该宝藏点！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void CantOpenLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "激活失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "只有盟主或者副盟主才能激活该宝藏点！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void RestingLackBuildOpenLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "重置失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "联盟建设值不足了，无法重置！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void LackBuildOpenLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "激活失败";//LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = "联盟建设值不足了，无法激活！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,confirmStr,null,null,null,null);
	}

	void SendActiveReqs()
	{
		ActiveTreasureReq mActiveTreasureReq = new ActiveTreasureReq ();
		
		MemoryStream mOpen_HuangyeStream = new MemoryStream ();
		
		QiXiongSerializer Huangye_er = new QiXiongSerializer ();
		
		mActiveTreasureReq.idOfFile = mHuangYeTreasure.fileId;

		HY_UIManager.Instance().IsactiveID = mHuangYeTreasure.fileId;

		Huangye_er.Serialize (mOpen_HuangyeStream,mActiveTreasureReq);
		
		byte[] t_protof;
		
		t_protof = mOpen_HuangyeStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.ACTIVE_TREASURE_REQ,ref t_protof);

	}
	void SendResstingReqs()
	{
		OpenHuangYeTreasure mOpenHuangYeTreasure = new OpenHuangYeTreasure ();
		
		MemoryStream mOpen_HuangyeStream = new MemoryStream ();
		
		QiXiongSerializer Huangye_er = new QiXiongSerializer ();
		
		mOpenHuangYeTreasure.id = mHuangYeTreasure.id;
		
		HY_UIManager.Instance().IsactiveID = mHuangYeTreasure.fileId;
		
		Huangye_er.Serialize (mOpen_HuangyeStream,mOpenHuangYeTreasure);
		
		byte[] t_protof;
		
		t_protof = mOpen_HuangyeStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_OPEN_TREASURE,ref t_protof);
		
	}
}



















