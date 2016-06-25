using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PassLevelAward : MonoBehaviour , SocketProcessor{

	public static PassLevelAward mStatePassLevelAward;
	public UILabel PassLevel_id;
	public string level;
	public int LingQu; // 0为不可领取 1为可领取 2为已经领取
	public GameObject LingQuButn;
	public GameObject LingQuButnEnable;
	public UIGrid AwardRoot;
	public UILabel LingquLabel;

	public int ZhangJieBigId;
	public  Section m_Mapinfo; 
	int yinid = -1;
	/// <summary>
	/// The type. 1 pve界面 2为主界面
	/// </summary>
	public int type;
	private bool mdataback;
	private bool CanLingQu;

	public UIWindowEventTrigger mUIWidowEventTriger;
	void Awake()
	{
		mStatePassLevelAward = this;
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDisable()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	

	void Update () {
	
		if(!FreshGuide.Instance().IsActive(100404))
		{
			if (UIYindao.m_UIYindao.m_isOpenYindao)
			{
				UIYindao.m_UIYindao.CloseUI();
			}
		}
		if(!GeneralRewardManager.Instance().IsExitReward() && mdataback)
		{
			ClientMain.closePopUp();
		}
	}
	private void YinDaoControl()
	{
		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			yinid = UIYindao.m_UIYindao.m_iCurId;
			Debug.Log("关闭引导 id = "+yinid);
			UIYindao.m_UIYindao.CloseUI();
		}
	}
	public void Init(int zhangjieid = -1)
	{
		mdataback = false;
		CanLingQu = false;
		FunctionWindowsCreateManagerment.m_IsSaoDangNow = true;
		if(zhangjieid == -1)
		{
			m_Mapinfo = MapData.mapinstance.myMapinfo;
			ZhangJieBigId = m_Mapinfo.s_section;
			type = 1;
	     
			if(FreshGuide.Instance().IsActive(100404)&& TaskData.Instance.m_TaskInfoDic[100404].progress >= 0)
			{
				mUIWidowEventTriger = GetComponent<UIWindowEventTrigger>();
				mUIWidowEventTriger.enabled = false;
			}
			else
			{
				if(UIYindao.m_UIYindao.m_isOpenYindao)
				{
					UIYindao.m_UIYindao.CloseUI();
				}
			}
		}
		else
		{
			YinDaoControl();
			ZhangJieBigId = zhangjieid;
			type = 2;
		}

		PassLevel_id.text  = "打通 "+ZhangJieBigId.ToString()+"-"+PveTempTemplate.GetLevelCount_By_Chapter_Id(ZhangJieBigId).ToString()+" 关卡";

		GetPassZhangJieAwardReq mGetPassZhangJieAwardReq = new GetPassZhangJieAwardReq ();
		
		mGetPassZhangJieAwardReq.zhangJieId = ZhangJieBigId;
		
		MemoryStream exitStream = new MemoryStream ();
		
		QiXiongSerializer exitQx = new QiXiongSerializer ();
		
		exitQx.Serialize (exitStream, mGetPassZhangJieAwardReq);
		
		byte[] t_protof = exitStream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.has_get_zhangJie_award_req, ref t_protof,"24153");

		ShowAward ();
	
	}
	List<int > itemislist = new  List<int>();

	List<int > Numlist = new  List<int>();

	public void ShowAward()
	{
		PveAwardTemplate mPveAwardTemplate = PveAwardTemplate.getAwardTemp_By_AwardId (ZhangJieBigId);
		string[] ss = mPveAwardTemplate.awardId.Split('#');
		itemislist.Clear ();
		for (int j = 0; j < ss.Length; j++)
		{
			string[] award = ss[j].Split(':');

			itemislist.Add(int.Parse(award[1]));

			Numlist.Add(int.Parse(award[2]));
		}
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.ICON_SAMPLE),
		                        ResLoadedSimple);

	}
	void ResLoadedSimple(ref WWW p_www, string p_path, UnityEngine.Object p_object)
	{
		for (int i = 0; i < itemislist.Count; i++)
		{
			if (AwardRoot != null)
			{
				GameObject tempObject = (GameObject)Instantiate(p_object);
				
				tempObject.transform.parent = AwardRoot.transform;
				tempObject.transform.localPosition = Vector3.zero;
				var iconSpriteName = "";
				IconSampleManager iconSampleManager = tempObject.GetComponent<IconSampleManager>();
				
				CommonItemTemplate mItemTemp = CommonItemTemplate.getCommonItemTemplateById(itemislist[i]);
				
				iconSpriteName = mItemTemp.icon.ToString();
				
				iconSampleManager.SetIconType(IconSampleManager.IconType.item);
				
				NameIdTemplate mNameIdTemplate = NameIdTemplate.getNameIdTemplateByNameId(mItemTemp.nameId);
				
				string mdesc = DescIdTemplate.GetDescriptionById(mItemTemp.descId);
				
				var popTitle = mNameIdTemplate.Name;
				
				var popDesc = mdesc;
				
				iconSampleManager.SetIconByID(mItemTemp.id, Numlist[i].ToString(), 10);
				iconSampleManager.SetIconPopText(mItemTemp.id, popTitle, popDesc, 1);
				tempObject.transform.localScale = Vector3.one * 0.45f;
				//iconSampleManager.SetAwardNumber(Numlist[i]);
			}
		
		}

		AwardRoot.repositionNow = true;
	}
	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.has_get_passZhangJie_award_req:// 请求通章节奖励
			{
				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ErrorMessage mErrorMessage = new ErrorMessage();
				t_qx.Deserialize(t_tream, mErrorMessage, mErrorMessage.GetType());
				InitData(mErrorMessage);;
				return true;
			}
				break;
			case ProtoIndexes.get_passZhangJie_award_req:// l领取通章节奖励
			{
				MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				ErrorMessage mErrorMessage = new ErrorMessage();
				t_qx.Deserialize(t_tream, mErrorMessage, mErrorMessage.GetType());

				if(type == 1)
				{
					PassLevelBtn.Instance().IsOPenEffect = false;
					PassLevelBtn.Instance().CloseEffect ();
				}

				InitGetAwardData(mErrorMessage);

				SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_NOT_GET_AWART_ZHANGJIE_REQ);
			
				return true;
			}
				break;
			}
		}
		return false;
	}
	void InitData(ErrorMessage mErrorMess)
	{
		LingQu = mErrorMess.errorCode;
//		Debug.Log ("LingQu = "+LingQu);
		if(LingQu == 2) //没通关
		{
			LingQuButn.SetActive(false);
			LingQuButnEnable.SetActive(true);
		}
		if(LingQu == 4)//可领取
		{
			LingQuButn.SetActive(true);
			LingQuButnEnable.SetActive(false);
			CanLingQu = true;
		}
		if(LingQu == 3)//已经领取
		{
			LingQuButnEnable.SetActive(true);
			LingQuButn.SetActive(false);
			LingquLabel.text = "已经领取";
		}
	}
	void InitGetAwardData(ErrorMessage mErrorMess)
	{
		//Debug.Log ("mErrorMess.errorCode = "+mErrorMess.errorCode);
		if(mErrorMess.errorCode == 0)//领取成功
		{
			LingQuButn.SetActive(false);
			LingQuButnEnable.SetActive(true);
			LingquLabel.text = "已经领取";
			ShowAwardByEffect();
		}
		else
		{
			//Debug.Log ("领取失败———————— ");
		}
		CanLingQu = false;
	}
	void  ShowAwardByEffect()
	{
		List<RewardData> tempDataList = new List<RewardData> ();
		for(int i = 0 ; i  < itemislist.Count ; i ++)
		{
			RewardData data = new RewardData ( itemislist[i], Numlist[i]); 
			tempDataList.Add(data);

		}
		GeneralRewardManager.Instance().CreateReward (tempDataList); 
		mdataback = true;
		if(type == 2)
		{
//			if (yinid > 0)
//			{ 
//				UIYindao.m_UIYindao.setOpenYindao(yinid);
//				yinid = -1;
//			}

			Close();
		}
	}
	public void LingQuBtn()
	{
		ClientMain.addPopUP (80, 0, "", null);
	}
	public void ComLingQU()
	{
		GetPassZhangJieAwardReq mGetPassZhangJieAwardReq = new GetPassZhangJieAwardReq ();
		
		mGetPassZhangJieAwardReq.zhangJieId = ZhangJieBigId;
		
		MemoryStream exitStream = new MemoryStream ();
		
		QiXiongSerializer exitQx = new QiXiongSerializer ();
		
		exitQx.Serialize (exitStream, mGetPassZhangJieAwardReq);
		
		byte[] t_protof = exitStream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.get_zhangJie_award_req, ref t_protof,"24155");

	}

	public void Close()
	{
		if(CanLingQu)
		{
			return;
		}
		FunctionWindowsCreateManagerment.m_IsSaoDangNow = false;
		MapData.mapinstance.ShowYinDao = false;
		PassLevelBtn.Instance().OPenEffect ();
			
		MapData.mapinstance.ShowYinDao = true;
	
		Destroy (this.gameObject);
	}
}
