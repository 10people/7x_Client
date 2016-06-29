using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ChooseCountry : MonoBehaviour,SocketProcessor {

	public List<UIEventListener> BtnList = new List<UIEventListener>(); 

	public List<CountryTemp> CountryTempList = new List<CountryTemp>(); 

	public GameObject m_CounrtyTemp;

	public UIGrid mUIGrid;
	/// <summary>
	/// 1 为选择国家 2为联盟转国 
	/// </summary>
	[HideInInspector]public int Type;//1 为选择国家 2为联盟转国 

	public GameObject CountryDesObj;

	public UILabel Country_Name;

	public UILabel Count_Name;

	public UILabel Des;

	public UILabel BtnNam;

	private string cancelStr;
	private string confirmStr;

	public UILabel ZhuanGuoCost;
	public enum ChooseType{

		Create = 1,
		Choose = 2,
	}
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
		BtnList.ForEach (item => SetBtnMoth(item));
		
	}
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void SetBtnMoth(UIEventListener mUIEventListener)
	{
		mUIEventListener.onClick = BtnManagerMent;
	}

	public void BtnManagerMent(GameObject mbutton)
	{
		int id = int.Parse(mbutton.name.Substring(7, mbutton.name.Length - 7));
		if(id<8&&id >0)
		{
			ChooseContryBtn(id);
		}
		else
		{
			switch(mbutton.name)
			{
			case "Button_0":
				Close();
				break;
			case "Button_11":
				SendData();
				break;
			case "Button_2":
				
				break;
			case "Button_3":
				
				break;
			default:
				break;
			}
		}
	
	}
	void Start () {

		confirmStr = LanguageTemplate.GetText(LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText(LanguageTemplate.Text.CANCEL);
	}
	private int NeedHufu;
	public void Init(int mType)
	{

		ZhuanGuoCost.text = "x"+CanshuTemplate.GetValueByKey("CHANGE_COUNTRY_COST").ToString();

		NeedHufu = (int)CanshuTemplate.GetValueByKey ("CHANGE_COUNTRY_COST");
		Type = mType;
		int CountryCount = 7;
		for (int i = 0; i < CountryCount; i++)
		{
			GameObject mCty = Instantiate(m_CounrtyTemp) as GameObject ;
			mCty.SetActive(true);
			mCty.transform.parent = m_CounrtyTemp.transform.parent;
			mCty.transform.localScale = Vector3.one;
			mCty.transform.localPosition = CountryZuoBiaoTemplate.GetZuobiao_by_id(i+1);
			CountryTemp mCountryTemp = mCty.GetComponent<CountryTemp>();
			mCountryTemp.beChoosed.gameObject.SetActive(false);
			mCountryTemp.CountryIcon = i+1;
			mCountryTemp.name = "Button_"+(i+1).ToString();
			if(mCountryTemp.CountryIcon == AllianceData.Instance.g_UnionInfo.country)
			{
				mCountryTemp.myself.SetActive(true);
			}
			else
			{
				mCountryTemp.myself.SetActive(false);
			}
			mCountryTemp.Init();
			CountryTempList.Add(mCountryTemp);
			UIEventListener mUIEventListener = mCty.GetComponent<UIEventListener>();
			BtnList.Add(mUIEventListener);
		}
		BtnList.ForEach (item => SetBtnMoth(item));
		//mUIGrid.repositionNow = true;
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index){
			case ProtoIndexes.S_LM_CHANGE_COUNTRY_REQP://联盟转国请求返回
			{
				MemoryStream noticeResp_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer noticeResp_qx = new QiXiongSerializer();
				
				ChangeAllianceCountryResp mChangeAllianceCountryResp = new ChangeAllianceCountryResp();
				
				noticeResp_qx.Deserialize (noticeResp_stream, mChangeAllianceCountryResp, mChangeAllianceCountryResp.GetType());

				Debug.Log("转国请求返回了result= "+mChangeAllianceCountryResp.result);

				if(mChangeAllianceCountryResp.result == 0)
				{
					AllianceData.Instance.RequestData();
					string mdata = "转国成功！";
					ClientMain.m_UITextManager.createText(mdata);
					Close();
				}
				else if(mChangeAllianceCountryResp.result == 1)
				{
					string mdata = "国家选择错误！";
					ClientMain.m_UITextManager.createText(mdata);
				}
				else if(mChangeAllianceCountryResp.result == 2)
				{
					string mdata = "权限不足！";
					ClientMain.m_UITextManager.createText(mdata);
				}
				else if(mChangeAllianceCountryResp.result == 3)
				{
					string mdata = "没有联盟！";
					ClientMain.m_UITextManager.createText(mdata);
				}
				else if(mChangeAllianceCountryResp.result == 4)
				{
					Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), LockOFPices);
//					string mdata = "虎符不足！";
//					ClientMain.m_UITextManager.createText(mdata);
				}
				else if(mChangeAllianceCountryResp.result < 0)
				{
					mDataTime = -mChangeAllianceCountryResp.result;
					Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), ZhuanGuoCDTime);
				}
				return true;
			}
		
			default: return false;
			}
			
		}
		
		return false;
	}
	string GetCountryName(int countryid,int type = 1)
	{
		string m_Name = "";

		switch(countryid)
		{
		case 1:
			m_Name = "齐国";
			break;
		case 2:
			m_Name = "楚国";
			break;
		case 3:
			m_Name = "燕国";
			break;
		case 4:
			m_Name = "韩国";
			break;
		case 5:
			m_Name = "赵国";
			break;
		case 6:
			m_Name = "魏国";
			break;
		case 7:
			m_Name = "秦国";
			break;
		default:
			break;
		}
		if(type == 2)
		{
			m_Name = m_Name.Substring(0,1);
			//Debug.Log("m_Name = "+m_Name);
		}
		return m_Name;
	}
	private string m_TimeLabel;


	void SendData()
	{
		if(Type == 1)
		{
			// 联盟创建时选择国家在这个对接 
		}
		else
		{
			if(CurrCountryID == AllianceData.Instance.g_UnionInfo.country)
			{
				Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.GLOBAL_DIALOG_BOX), CanZhuanGuo);
			}
			else
			{
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX),
				                        AllianceTransTipsLoadCallback1);
			}
		}
	}
	public void AllianceTransTipsLoadCallback1(ref WWW p_www, string p_path, Object p_object)
	{
		int needHuFu = NeedHufu; // 需要的护符数

		GameObject boxObj = Instantiate(p_object) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox>();
		string jieSanTitleStr = "提示";
		string str1 = "\n"+LanguageTemplate.GetText (LanguageTemplate.Text.ZHUANGUOCOST)+"x"+needHuFu.ToString()+"。\n"+LanguageTemplate.GetText (LanguageTemplate.Text.ZHUANCOMFORIE);//"联盟转国需要花费虎符"+needHuFu.ToString()+"\n"+ "确认转国吗？";

		uibox.setBox(jieSanTitleStr, str1, null, null, cancelStr, confirmStr, ChangeCountry);
	}

	void ChangeCountry(int i)
	{
		if(i == 2)
		{
			int needHuFu = 0; // 需要的护符数
			int mHuFu = AllianceData.Instance.g_UnionInfo.hufuNum;

			ChangeAllianceCountry mChangeAllianceCountry = new ChangeAllianceCountry ();
			MemoryStream MiBaoinfoStream = new MemoryStream ();
			QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
			
			mChangeAllianceCountry.country = CurrCountryID;
			MiBaoinfoer.Serialize (MiBaoinfoStream,mChangeAllianceCountry);
			
			byte[] t_protof;
			t_protof = MiBaoinfoStream.ToArray();
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_LM_CHANGE_COUNTRY,ref t_protof,ProtoIndexes.S_LM_CHANGE_COUNTRY_REQP.ToString());
		}
	}

	void LockOFPices(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "虎符不足，无法进行转国操作！"+"\n\r"+"联盟虎符通过成员进行军政建设来获得。";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, str1,null,null,confirmStr,null,null);
	}
	private UIBox uibox;
	void ZhuanGuoCDTime(ref WWW p_www,string p_path, Object p_object)
	{
		uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "成功转国后，24小时内不能再次进行转国！"+"\n";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);

		string str2 = "还需等待";

		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, str1+str2,null,null,confirmStr,null,null);
		uibox.mCountTime (mDataTime);
	}
	private int mDataTime;

	void CanZhuanGuo(ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = LanguageTemplate.GetText (LanguageTemplate.Text.CHAT_UIBOX_INFO);
		
		string str1 = "不能和自己的国家进行转国操作！";//LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_92);
		
		string CancleBtn = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr, str1,null,null,confirmStr,null,null);
	}

	private int CurrCountryID;
	public UISprite CountryIcon;
	void ChooseContryBtn(int Countryid){

		if(Countryid == AllianceData.Instance.g_UnionInfo.country)
		{
			return;
		}

		CountryIcon.spriteName = "flag_"+Countryid.ToString();

		CurrCountryID = Countryid;

		CountryDesObj.SetActive (true);

		//Country_Name.text = GetCountryName (Countryid);
		Country_Name.text = MyColorData.getColorString(CountryZuoBiaoTemplate.GeTempByid(Countryid).color
		                                               , GetCountryName(Countryid));
		Count_Name.text = MyColorData.getColorString(CountryZuoBiaoTemplate.GeTempByid(Countryid).color
		                                             , GetCountryName(Countryid,2));
		Des.text = CountryZuoBiaoTemplate.GetDesco_by_id (Countryid);

		if(Type == 1)
		{
			BtnNam.text = "下一步";
		}
		else
		{
			BtnNam.text = "确定";
		}
//		Debug.Log ("Countryid = "+Countryid);
//		Debug.Log ("CountryTempList.count = "+CountryTempList.Count);
		CountryTempList.ForEach (item => SetCountryTempListMoth(item));
		CountryTempList [Countryid-1].beChoosed.gameObject.SetActive (true);
	}

	void SetCountryTempListMoth(CountryTemp mCount)
	{
		mCount.beChoosed.gameObject.SetActive (false); 
	}

	void Close()
	{
		Destroy (this.gameObject);
	}
}
