using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class PassLevelAward : MonoBehaviour , SocketProcessor{


	public UILabel PassLevel_id;
	public string level;
	public int LingQu; // 0为不可领取 1为可领取 2为已经领取
	public GameObject LingQuButn;

	public UIGrid AwardRoot;
	public UILabel LingquLabel;

	public int ZhangJieBigId;
	public  Section m_Mapinfo; 
	void Awake()
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	void OnDisable()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start () {
	
	}
	

	void Update () {
	
	}
	public void Init()
	{
		m_Mapinfo = MapData.mapinstance.myMapinfo;
		ZhangJieBigId = m_Mapinfo.s_section;
		PassLevel_id.text  = "打通"+m_Mapinfo.s_section.ToString()+"-"+m_Mapinfo.s_allLevel.Count.ToString()+"关卡";

		GetPassZhangJieAwardReq mGetPassZhangJieAwardReq = new GetPassZhangJieAwardReq ();
		
		mGetPassZhangJieAwardReq.zhangJieId = ZhangJieBigId;
		
		MemoryStream exitStream = new MemoryStream ();
		
		QiXiongSerializer exitQx = new QiXiongSerializer ();
		
		exitQx.Serialize (exitStream, mGetPassZhangJieAwardReq);
		
		byte[] t_protof = exitStream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.has_get_zhangJie_award_req, ref t_protof);

		ShowAward ();
		if(FreshGuide.Instance().IsActive(100140)&& TaskData.Instance.m_TaskInfoDic[100140].progress >= 0)
		{
			Debug.Log("指向宝箱点击领奖");
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100140];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
		}
	}
	List<int > itemislist = new  List<int>();

	List<int > Numlist = new  List<int>();

	public void ShowAward()
	{
		PveAwardTemplate mPveAwardTemplate = PveAwardTemplate.getAwardTemp_By_AwardId (m_Mapinfo.s_section);
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
				InitGetAwardData(mErrorMessage);
				if(MapData.mapinstance.myMapinfo.s_section == 1) //第一章节领奖引导
				{
					MapData.mapinstance.BackToCity();
				}
				SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_NOT_GET_AWART_ZHANGJIE_REQ);
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
		Debug.Log ("LingQu = "+LingQu);
		if(LingQu == 2) //没通关
		{
			LingQuButn.SetActive(false);
		}
		if(LingQu == 4)//可领取
		{
			LingQuButn.SetActive(true);
		}
		if(LingQu == 3)//已经领取
		{
			LingQuButn.SetActive(false);
			LingquLabel.text = "已经领取";
		}
	}
	void InitGetAwardData(ErrorMessage mErrorMess)
	{
		Debug.Log ("mErrorMess.errorCode = "+mErrorMess.errorCode);
		if(mErrorMess.errorCode == 0)//领取成功
		{
			LingQuButn.SetActive(false);
			LingquLabel.text = "已经领取";
			StartCoroutine(ShowAwardByEffect());
		}
		else
		{
			Debug.Log ("领取失败———————— ");
		}
	}
	IEnumerator ShowAwardByEffect()
	{
		for(int i = 0 ; i  < itemislist.Count ; i ++)
		{
			yield return new WaitForSeconds(0.5f);
			RewardData data = new RewardData ( itemislist[i], Numlist[i]); 
			GeneralRewardManager.Instance ().CreateReward (data); 
		}
	}
	public void LingQuBtn()
	{
		GetPassZhangJieAwardReq mGetPassZhangJieAwardReq = new GetPassZhangJieAwardReq ();
		
		mGetPassZhangJieAwardReq.zhangJieId = ZhangJieBigId;
		
		MemoryStream exitStream = new MemoryStream ();
		
		QiXiongSerializer exitQx = new QiXiongSerializer ();
		
		exitQx.Serialize (exitStream, mGetPassZhangJieAwardReq);
		
		byte[] t_protof = exitStream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.get_zhangJie_award_req, ref t_protof);
	
	}
	public void Close()
	{
		PassLevelBtn.Instance ().OPenEffect ();
		MapData.mapinstance.OpenEffect();
		Destroy (this.gameObject);
	}
}
