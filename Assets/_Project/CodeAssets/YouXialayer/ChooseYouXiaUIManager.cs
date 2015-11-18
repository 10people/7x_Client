using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChooseYouXiaUIManager : MonoBehaviour,SocketProcessor {

	[HideInInspector]

	public int BigId;

	public GameObject[] YouXiaItemTemp;

	public UILabel ToDayCountTimes;

	public int HavatTimes;

	public UILabel mName;

	public YouXiaInfo mYouXia_Info;

	public GameObject Buybtn;

	public GameObject Buybtnbg;

	public static ChooseYouXiaUIManager mChooseYouXiaUIManager;
	void Awake()
	{ 
		mChooseYouXiaUIManager = this;

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
	public void Init()
	{
	

		List<YouxiaPveTemplate> mYouxiaPveTemplateList = YouxiaPveTemplate.getYouXiaPveTemplateListBy_BigId (BigId);

		Debug.Log ("mYouxiaPveTemplateList.Count = "+mYouxiaPveTemplateList.Count);
		Debug.Log ("YouXiaItemTemp.Length = "+YouXiaItemTemp.Length);
		Debug.Log ("mYouXia_Info.remainTimes = "+mYouXia_Info.remainTimes);
		YouxiaPveTemplate myouxia = YouxiaPveTemplate.getYouXiaPveTemplateBy_BigId (BigId);
		
		mName.text = NameIdTemplate.GetName_By_NameId (myouxia.bigName);
		
		ToDayCountTimes.text = mYouXia_Info.remainTimes.ToString ();

		for(int i = 0 ; i < mYouxiaPveTemplateList.Count; i++)
		{
			YouXiaItem mYouXiaItem = YouXiaItemTemp[i].GetComponent<YouXiaItem>();

			mYouXiaItem.L_id = mYouxiaPveTemplateList[i].id;

			mYouXiaItem.YouXiadifficulty = i+1;

			//mYouXiaItem.CountTime = HavatTimes;

			mYouXiaItem.bigid = mYouxiaPveTemplateList[i].bigId;

			mYouXiaItem.m_item_name = mName.text;

			mYouXiaItem.mYou_XiaInfo = mYouXia_Info;

			mYouXiaItem.Init();
		}

		ShowBuyBtn ();
	}
	void ShowBuyBtn()
	{
	
		VipTemplate mVip = VipTemplate.GetVipInfoByLevel (JunZhuData.Instance().m_junzhuInfo.vipLv);
		
		if(mVip.YouxiaTimes <= 0)
		{
			Buybtn.SetActive(false);
			
			Buybtnbg.SetActive(false);
		}
		else
		{
			if(mYouXia_Info.remainTimes > 0)
			{
				Buybtn.SetActive(true);
			}
			else
			{
				Buybtn.SetActive(false);
				
				Buybtnbg.SetActive(true);
			}
		}
	}
	public  bool OnProcessSocketMessage( QXBuffer p_message )
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index){
				
			case ProtoIndexes.S_YOUXIA_TIMES_INFO_RESP:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaTimesInfoResp tempInfo = new YouXiaTimesInfoResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				M_tempInfo = tempInfo;

				BuyTimesInfoBack(tempInfo);

				Debug.Log("购买游侠信息次数返回");

				return true;
			}
			case ProtoIndexes.S_YOUXIA_TIMES_BUY_RESP:
			{
				
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YouXiaTimesBuyResp tempInfo = new YouXiaTimesBuyResp();
				
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				
				Debug.Log("确认购买游侠信息返回");

				if(tempInfo.result == 0)
				{
					ComfireBuyTimesInfoBack(tempInfo);
				}
				else
				{
					Debug.LogError("购买失败了");
				}

				return true;
			}	
			default: return false;
			}
			
		}
		
		return false;
	}


	private void BuyTimesInfoBack(YouXiaTimesInfoResp m_tempInfo)
	{
		if(m_tempInfo.remainBuyTimes > 0)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBuyTimesInfoBack);
		}
		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), HaveNoTimesReMainBack);
		}
	}

	YouXiaTimesInfoResp M_tempInfo;

	void LoadBuyTimesInfoBack(ref WWW p_www, string p_path, Object p_object)
	{
		string str1 =  LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string str2 = "\r\n"+"您是否要用"+M_tempInfo.cost.ToString()+"元宝"+"\r\n"+"\r\n"+"购买"+M_tempInfo.getTimes.ToString()+"次？";

		string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);

		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		uibox.setBox(str1, str2 , null, null, strbtn1, strbtn2, BuYComfire, null, null, null);
	}
	void BuYComfire(int i)
	{
		if (i == 2)
		{
			// Debug.Log("发送购买金币的请求");
			if (JunZhuData.Instance().m_junzhuInfo.yuanBao < M_tempInfo.cost)
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LoadBack_2);

				return;
			}
			YouXiaTimesBuyReq mYouXiaTimesInfoReq = new YouXiaTimesBuyReq ();
			
			MemoryStream YouXiaTimesInfoReqStream = new MemoryStream ();
			
			QiXiongSerializer YouXiaTimesInfoReqer = new QiXiongSerializer ();
			
			mYouXiaTimesInfoReq.type = BigId;
			
			YouXiaTimesInfoReqer.Serialize (YouXiaTimesInfoReqStream,mYouXiaTimesInfoReq);
			
			byte[] t_protof;
			
			t_protof = YouXiaTimesInfoReqStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(
				ProtoIndexes.C_YOUXIA_TIMES_BUY_REQ, 
				ref t_protof,
				true,
				ProtoIndexes.S_YOUXIA_TIMES_BUY_RESP );
		}
	}
	void LoadBack_2(ref WWW p_www, string p_path, Object p_object)
	{
		
		//string str1 = "元宝不足";
		string str2 = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_IS_RECHARGE);
		
		string titleStr = LanguageTemplate.GetText(LanguageTemplate.Text.YUANBAO_LACK_TITLE);
		
		string CancleBtn = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
		
		string strbtn = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		uibox.setBox(titleStr, null, str2, null, CancleBtn, strbtn,LackYBLoadBack, null, null, null);
	}
	void HaveNoTimesReMainBack(ref WWW p_www, string p_path, Object p_object)
	{
		string title = LanguageTemplate.GetText(LanguageTemplate.Text.CHAT_UIBOX_INFO);

		string str2 = "\r\n"+"今日购买次数已经用完了"+"\r\n"+"\r\n"+"提升Vip等级可以增加购买次数";//LanguageTemplate.GetText(LanguageTemplate.Text.FINGHT_CONDITON);

		//string strbtn1 = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);

		string strbtn2 = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);

		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		uibox.setBox(title,str2, null, null, strbtn2,  null, null, null, null);
	}
	private void ComfireBuyTimesInfoBack(YouXiaTimesBuyResp m_tempInfo)
	{
		ToDayCountTimes.text = m_tempInfo.ramainTimes.ToString ();

		foreach(SelectYouXiaEntertype mSelectYouXiaEntertype in EnterYouXiaBattle.GlobleEnterYouXiaBattle.SelectYouXiaEntertypeList)
		{
			if(mSelectYouXiaEntertype.mYouXiaInfo.id == BigId)
			{
				mSelectYouXiaEntertype.mYouXiaInfo.remainTimes = m_tempInfo.ramainTimes;

				mSelectYouXiaEntertype.ShouTimes();
			}
		
		}
		mYouXia_Info.remainTimes = m_tempInfo.ramainTimes;

		ShowBuyBtn ();
	}
	public void BuyTimes()
	{
		YouXiaTimesInfoReq mYouXiaTimesInfoReq = new YouXiaTimesInfoReq ();
		
		MemoryStream YouXiaTimesInfoReqStream = new MemoryStream ();
		
		QiXiongSerializer YouXiaTimesInfoReqer = new QiXiongSerializer ();
		
		mYouXiaTimesInfoReq.type = BigId;
		
		YouXiaTimesInfoReqer.Serialize (YouXiaTimesInfoReqStream,mYouXiaTimesInfoReq);
		
		byte[] t_protof;
		
		t_protof = YouXiaTimesInfoReqStream.ToArray();
		
		SocketTool.Instance().SendSocketMessage(
			ProtoIndexes.C_YOUXIA_TIMES_INFO_REQ, 
			ref t_protof,
			true,
			ProtoIndexes.S_YOUXIA_TIMES_INFO_RESP );

	}
	void LackYBLoadBack (int i)
	{
		if (i == 2)
		{
			Debug.Log ("跳转到充值！");

			MainCityUI.ClearObjectList();
			TopUpLoadManagerment.m_instance.LoadPrefab(true);
			QXTanBaoData.Instance ().CheckFreeTanBao ();
		}
	}
	public void Back()
	{
		EnterYouXiaBattle.GlobleEnterYouXiaBattle.ShowOrClose ();
		Destroy (this.gameObject);
	}

	public void Close()
	{
		GameObject g = GameObject.Find ("Enter_YouXiaBattle(Clone)");
		
		MainCityUI.TryRemoveFromObjectList(g);
		
		Destroy (g);
	}
}
