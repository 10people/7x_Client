using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class SaoDangBtn : MonoBehaviour,SocketProcessor {

	int needpower;
	int ExistingPower = 0;
	public int SaoDangTimes;
	private int SaodangId;
	//public GameObject supgame;
	public static SaoDangBtn saodanginfo;
	public PveSaoDangRet saodinfo;
	public Dictionary<string,PveStarAwardItem> pverewardItem = new Dictionary<string,PveStarAwardItem>();
	public List<PveStarAwardItem> pveW_item = new List<PveStarAwardItem>();

	private int Vipgrade = 0;

	private int junZhuLevel;//君主等级

	public bool ISCanSaodang;


	void Awake()
	{ 
		saodanginfo = this;
	}

	void OnDestroy()
	{
		saodanginfo = null;
	}

	void Start () 
	{

		SaodangId = Pve_Level_Info.CurLev;
		//needpower = ShowTiLi.needTiLi*SaoDangTimes;
		//Debug.Log ("needpower"+needpower);
		//Debug.Log ("SaodangId"+SaodangId);
		ISCanSaodang = true;
	}

	void Update ()
	{
		junZhuLevel = JunZhuData.Instance().m_junzhuInfo.level;


	}

	public   bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_PVE_SAO_DANG:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
					
				PveSaoDangRet tempInfo = new PveSaoDangRet();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				saodinfo = tempInfo;

				//.Log("请求扫荡是数据返回了。。。");

				pveW_item.Clear();
			
				PveLevelUImaneger.mPveLevelUImaneger.sendLevelDrop(SaodangId);

				popRewardUI();

				SocketTool.UnRegisterMessageProcessor(this);
				//Debug.Log("扫荡接收完毕。。。。");
				return true;
			}
			default: return false;
			}
		}
		
		return false;
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate(p_object) as GameObject;

		GameObject obj = GameObject.Find ("Mapss");
		
		tempOjbect.transform.parent = obj.transform;

		tempOjbect.transform.localPosition = new Vector3 (0,0,0);

		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		
		SaoDangManeger mSaoDangManeger = tempOjbect.GetComponent<SaoDangManeger>();

		mSaoDangManeger.m_PveSaoDangRet = saodinfo;

		mSaoDangManeger.SaodangType = 1;

		mSaoDangManeger.Init ();
	}

	void popRewardUI()
	{
		PveLevelUImaneger.mPveLevelUImaneger.CloseEffect ();

		PveLevelUImaneger.mPveLevelUImaneger.IsSaodang = true;

		Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_SAODANG_LEVEL ),LoadResourceCallback);
	}


	void SendSaoDangInfo(int id,int howTimes)
	{

		PveSaoDangReq saodanginfo = new PveSaoDangReq ();

		MemoryStream saodangstream = new MemoryStream ();

		QiXiongSerializer saodangSer = new QiXiongSerializer ();

		int i = 1;

		if(PveLevelUImaneger.mPveLevelUImaneger.Lv_Info.type == 2)
		{
			i = -1;
		}
		saodanginfo.guanQiaId = id*i;

		saodanginfo.times = howTimes;

		//Debug.Log ("saodanginfo.times = " +saodanginfo.times);

		saodangSer.Serialize (saodangstream, saodanginfo);

		byte[] t_protof;

		t_protof = saodangstream.ToArray();

		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_PVE_SAO_DANG,ref t_protof);

	}

	public void LoadResourceCallback1(ref WWW p_www,string p_path, Object p_object)
	{
		//Debug.Log ("needpower33333" +needpower);
		GameObject tempOjbect = Instantiate(p_object) as GameObject;

		GameObject obj = GameObject.Find ("Mapss");
		
		tempOjbect.transform.parent = obj.transform;

		tempOjbect.transform.localPosition = Vector3.zero;

		tempOjbect.transform.localScale = new Vector3 (1,1,1);
	}
	//UIFont mtitleFont; //标题字体
	//UIFont mbtn1Font; //按钮1字体
	//UIFont mbtn2Font;
	void getTili(int i)
	{
		if(i == 2)
		{
			JunZhuData.Instance().BuyTiliAndTongBi(true,false,false);
		}
	}
	void LockTiLiLoadBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str = LanguageTemplate.GetText (LanguageTemplate.Text.TITITLE);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str),null,CancleBtn,confirmStr,getTili);
	}

	void LoadRenWuBack(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.PVE_RESET_BTN_BOX_TITLE);

		string Contain1 = LanguageTemplate.GetText(LanguageTemplate.Text.RENWU_LIMIT);
		
		string Contain2 = Contain1+"\r\n"+"\r\n"+ZhuXianTemp.GeTaskTitleById (FunctionOpenTemp.GetTemplateById(3000010).m_iDoneMissionID)+"\r\n"+"\r\n"+"后开启该功能";

		//string Contain3 = LanguageTemplate.GetText(LanguageTemplate.Text.FINGHT_CONDITON);

		string Comfirm = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		uibox.setBox(title,Contain2, null,null,Comfirm,null,null,null,null);
	}
	void OnClick()
	{
		if(!ISCanSaodang)
		{
			return;
		}
		ISCanSaodang = false;
		Vipgrade = JunZhuData.Instance().m_junzhuInfo.vipLv;

		ExistingPower = JunZhuData.Instance().m_junzhuInfo.tili;


		//Debug.Log ("FunctionOpenTemp.GetWhetherContainID(3000010) = " +FunctionOpenTemp.GetWhetherContainID(3000010));

		if(!FunctionOpenTemp.GetWhetherContainID(3000010))
		{
		
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LoadRenWuBack);
			ISCanSaodang = true;
			return;
		}

		if (junZhuLevel >= FunctionOpenTemp.GetTemplateById(3000010).Level)
		{
			SaodangId = Pve_Level_Info.CurLev;
			//Debug.Log("SaoDangTimes = " +SaoDangTimes);
			needpower = PveLevelUImaneger.GuanqiaReq.tili*SaoDangTimes;
			//Debug.Log ("needpower" +needpower);
			if(needpower > ExistingPower)//体力不够
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LockTiLiLoadBack);
				ISCanSaodang = true;
				//Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_NO_TI_LI ),LoadResourceCallback1);
			}
			else
			{
				int viplv = VipFuncOpenTemplate.GetNeedLevelByKey(13);

				if(SaoDangTimes>1&&Vipgrade<viplv)
				{
					Global.ResourcesDotLoad (Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_CANT_SAO_DANG_REMAIN ),LoadResourceCallback1);
					ISCanSaodang = true;
				}else{
					//Debug.Log ("发送扫荡请求。。。");
					SocketTool.RegisterMessageProcessor(this);

					SendSaoDangInfo (SaodangId,SaoDangTimes);
				}
			}
		}

		else
		{
			PveLevelUImaneger.mPveLevelUImaneger.zheZhao.SetActive (true);

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),LevelTips);
			ISCanSaodang = true;
		}
	}

	public void LevelTips(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string Tilte = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string str = LanguageTemplate.GetText (LanguageTemplate.Text.JUNZHU_LV)+FunctionOpenTemp.GetTemplateById(3000010).Level.ToString()+LanguageTemplate.GetText (LanguageTemplate.Text.SAODANGGUANQIA);

		string Btn = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);

		uibox.setBox(Tilte,null,MyColorData.getColorString (1,str),null,Btn,null,FalseZheZhao);
	}

	void FalseZheZhao (int i)
	{
		PveLevelUImaneger.mPveLevelUImaneger.zheZhao.SetActive (false);
	}
}
