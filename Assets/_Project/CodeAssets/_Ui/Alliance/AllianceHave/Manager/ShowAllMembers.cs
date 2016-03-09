using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;
public class ShowAllMembers : MonoBehaviour {

	public MemberInfo mMemberInfo;
	
	public UILabel m_name;
	
	public UILabel m_Level;
	
	public UILabel m_Zhiwei;
	
	public UILabel m_Contrbiution;

	public UILabel Curr_Contrbiution;//本月贡献

	public UILabel  DownTime;

	public UILabel ZhanLi;

	public UILabel GongJin;

	GetDragData mGetDragData;
	void Start () {
	
		mGetDragData = GetComponent<GetDragData>();
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}
	

	void Update () {
	
	}
	public void init()
	{
		FriendData.Instance.RequestData ();

		m_name.text = mMemberInfo.name;
		
		m_Level.text = mMemberInfo.level.ToString ();

		if(mMemberInfo.identity == 0)
		{
			m_Zhiwei.text = "盟员";
		}
		if(mMemberInfo.identity == 1)
		{
			m_Zhiwei.text = "副盟主";
		}
		if(mMemberInfo.identity == 2)
		{
			m_Zhiwei.text = "盟主";
		}
		m_Contrbiution.text = mMemberInfo.contribution.ToString ();

		Curr_Contrbiution.text = mMemberInfo.curMonthGongXian.ToString ();

		ShowDownTime ();

		ZhanLi.text = mMemberInfo.zhanLi.ToString ();
		if(mMemberInfo.gongJin < 0)
		{
			mMemberInfo.gongJin = 0;
		}
		GongJin.text = mMemberInfo.gongJin.ToString ();
	}

	void ShowDownTime()
	{
		//Debug.Log ("mMemberInfo.offlineTime = " +mMemberInfo.offlineTime);
		if(mMemberInfo.offlineTime <= 0)
		{
			DownTime.text = "在线";

			return;
		}
		if(mMemberInfo.offlineTime > (60*60*24))
		{
			int i = (int)(mMemberInfo.offlineTime)/(60*60*24);

			DownTime.text = "离线"+i+"天";
		}
		else if(mMemberInfo.offlineTime < (60*60*24)&&mMemberInfo.offlineTime > (60*60))
		{
			int i = (int)(mMemberInfo.offlineTime)/(60*60);
			//Debug.Log ("mMemberInfo.i = " +i);
			DownTime.text = "离线"+i+"小时";
		}
		else if(mMemberInfo.offlineTime < 60*60&&mMemberInfo.offlineTime > 60)
		{
			int i = (int)(mMemberInfo.offlineTime)/(60);
			
			DownTime.text = "离线"+i+"分钟";
		}else if(mMemberInfo.offlineTime < 60)
		{
			DownTime.text = "离线1分钟";
		}

	}
	GameObject m_tempObject;
	void OnClick()
	{
		//Show float buttons.
//		foreach(GameObject n in _MyAllianceManager.Instance().m_BtnList)
//		{
//			Destroy(n);
//		}
//		_MyAllianceManager.Instance().m_BtnList.Clear ();

		if(m_tempObject)
		{
			Destroy(m_tempObject);
		}else
		{
			foreach(GameObject btn in MembersManager.mInstance.BtnsList)
			{
				Destroy(btn);
			}
			MembersManager.mInstance.BtnsList.Clear();

			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.FLOAT_BUTTON), FloatButtonLoadCallBack);
		}

	}
	public GameObject FloatButtonsRoot;

	void FloatButtonLoadCallBack(ref WWW p_www, string p_path, Object p_object)
	{
		m_tempObject = (GameObject)Instantiate(p_object);

		MembersManager.mInstance.BtnsList.Add (m_tempObject);

		FloatButtonsController = m_tempObject.GetComponent<FloatButtonsController>();
		
		List<FloatButtonsController.ButtonInfo> tempList = new List<FloatButtonsController.ButtonInfo>();

		tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "查看信息", m_VoidDelegate = GetInfo });

		if(mMemberInfo.junzhuId != JunZhuData.Instance().m_junzhuInfo.id)
		{
			string ids = mMemberInfo.junzhuId.ToString();

			if(!FriendData.Friendsid.Contains (ids))
			{
				if (!FriendData.Instance.FriendIDList.Contains(mMemberInfo.junzhuId))
				{
					tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "加为好友", m_VoidDelegate = AddFriend });
				}
			}

			//tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "复制名字", m_VoidDelegate = CopyMemberName });

		}
	
		if(NewAlliancemanager.Instance().m_allianceHaveRes.identity == 2)
		{
			if(mMemberInfo.identity == 0)
			{

				tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "开除", m_VoidDelegate = GetOutAlliance });

				tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "升职", m_VoidDelegate = ShengZhiMemberInfo });
			}
			if(mMemberInfo.identity == 1)
			{
				tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "转让联盟", m_VoidDelegate = ZhuanRangAlliance });

				tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "降职", m_VoidDelegate = JiangZhiMemberInfo });

				tempList.Add(new FloatButtonsController.ButtonInfo() { m_LabelStr = "开除", m_VoidDelegate = GetOutAlliance });
			}
		}

		FloatButtonsController.Initialize(tempList, true);

		float m_x = 0;

		float m_y = 0;

		m_y = this.transform.localPosition.y + this.transform.parent.parent.localPosition.y;
		//Debug.Log ("m_y = "+m_y);
		if(tempList.Count > 3)
		{
			if(m_y < -80)
			{
				m_y += (tempList.Count-3)*20;
			}
		}

		Vector3 m_V = new Vector3 (m_x,m_y,0);

		m_tempObject.transform.parent = FloatButtonsRoot.transform;

		m_tempObject.transform.localPosition = m_V;

		m_tempObject.transform.localScale = Vector3.one;
		//TransformHelper.ActiveWithStandardize(FloatButtonsRoot.transform, m_tempObject.transform);
		//m_ModuleController.ScrollToEndIfClose();
	}

	public  void GetInfo()
	{
		JunZhuInfoSpecifyReq mJunZhuInfoSpecifyReq = new JunZhuInfoSpecifyReq ();

		mJunZhuInfoSpecifyReq.junzhuId = mMemberInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer q_serializer = new QiXiongSerializer ();
		
		q_serializer.Serialize (t_stream,mJunZhuInfoSpecifyReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.JUNZHU_INFO_SPECIFY_REQ,ref t_protof,"23068");
		DestroyFloatButtons();

		//Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.KING_DETAIL_WINDOW), KingDetailLoadCallBack);
	}
	public  void AddFriend()
	{
		FriendOperationLayerManagerment.AddFriends((int)mMemberInfo.junzhuId);

		SocketTool.Instance().SendSocketMessage (ProtoIndexes.C_GET_FRIEND_IDS);

		DestroyFloatButtons();
	}
	public  void TouPiao()
	{
		//FriendOperationLayerManagerment.AddFriends((int)mMemberInfo.junzhuId);
		//DestroyFloatButtons();
	}
//	void KingDetailLoadCallBack(ref WWW p_www, string p_path, Object p_object)
//	{
//		var temp = Instantiate(p_object) as GameObject;
//		var info = temp.GetComponent<KingDetailInfo>();
//		
//		var tempKingInfo = new KingDetailInfo.KingDetailData()
//		{
//			RoleID = mMemberInfo.roleId,
//			Attack = 0,
//			AllianceName = "",
//			BattleValue = mMemberInfo.zhanLi,
//			Junxian = mMemberInfo.junXian.ToString(),
//			KingName = mMemberInfo.name,
//			Level = mMemberInfo.level,
//			Money = mMemberInfo.gongJin,
//			Life = 0,
//			Protect = 0
//		};
//		if(mMemberInfo.isVoted == 0)
//		{
//			var tempConfigList = new List<KingDetailButtonController.KingDetailButtonConfig>()
//			{
//				new KingDetailButtonController.KingDetailButtonConfig() {m_ButtonStr = "投TA一票", m_ButtonClick = TouPiao},
//				
//			};
//			info.SetThis(tempKingInfo, tempConfigList);
//		}
//
//		if(mMemberInfo.isVoted == 1)
//		{
//			info.m_VotedSprite.gameObject.SetActive(true);
//			info.m_VotedSprite.spriteName = "Apply";
//		}
//		if(mMemberInfo.isVoted == 2)
//		{
//			info.m_VotedSprite.gameObject.SetActive(true);
//			info.m_VotedSprite.spriteName = "GiveUp";
//		}
//		//info.m_KingDetailEquipInfo.m_BagItemDic = m_JunZhuInfo.equip.items.Where(item => item.buWei > 0).ToDictionary(item => base.TransferBuwei(item.buWei));
//		
//		temp.SetActive(true);
//	}
	public void DestroyFloatButtons()
	{
		if (FloatButtonsController != null)
		{
			Destroy(FloatButtonsController.gameObject);
			FloatButtonsController = null;
		}
	}
	public FloatButtonsController FloatButtonsController;
	public void ShowBtnList()
	{
		if(NewAlliancemanager.Instance().m_allianceHaveRes.identity == 2)
		{

		}
		if(NewAlliancemanager.Instance().m_allianceHaveRes.identity == 1)
		{
			
		}
		if(NewAlliancemanager.Instance().m_allianceHaveRes.identity == 0)
		{
			
		}
	}
	public string CopyName;

	public void CopyMemberName()
	{
		CopyName = mMemberInfo.name;
		DestroyFloatButtons();
	}
	public void JiangZhiMemberInfo()
	{
		UpTitle down = new UpTitle ();
		
		down.id = NewAlliancemanager.Instance().m_allianceHaveRes.id;
		down.junzhuId = mMemberInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer q_serializer = new QiXiongSerializer ();
		
		q_serializer.Serialize (t_stream,down);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.DOWN_TITLE,ref t_protof,"30122");
		DestroyFloatButtons();
	}
	public void ShengZhiMemberInfo()
	{
		UpTitle up = new UpTitle ();
		
		up.id = NewAlliancemanager.Instance().m_allianceHaveRes.id;
		up.junzhuId = mMemberInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer q_serializer = new QiXiongSerializer ();
		
		q_serializer.Serialize (t_stream,up);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.UP_TITLE,ref t_protof,"30120");
		DestroyFloatButtons();

	}
	private string confirmStr;
	private string cancelStr;
	public void ZhuanRangAlliance()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ExitLoadCallback );

	}
	//z转让联盟
	public void ExitLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_DES1);
		string str2 = str1+"\n\r"+mMemberInfo.name+"\r\n"+LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_DES2);
		
		string exitTitle = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_EXIT_TITLE);
		
		uibox.setBox(exitTitle, MyColorData.getColorString (1,str2), null,
		             null,cancelStr,confirmStr,ComfirmZhuanRangAlliance);
	}
	void ComfirmZhuanRangAlliance(int i)
	{
		if( i == 2)
		{
			TransferAlliance transReq = new TransferAlliance ();
			
			transReq.id = NewAlliancemanager.Instance().m_allianceHaveRes.id;
			transReq.junzhuId = mMemberInfo.junzhuId;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize (t_stream,transReq);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.TRANSFER_ALLIANCE,ref t_protof,"30138");
		}

	}
	public void GetOutAlliance()
	{
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ResourceLoadCallback );
		DestroyFloatButtons();
	}
	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object ){
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent <UIBox> ();
		
		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_ASKSTR1);
		string str2 = mMemberInfo.name + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_ASKSTR2);
		
		string warringTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_TITLE);
		
		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
		
		uibox.setBox(warringTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,cancelStr,confirmStr,KaiChuWarring);
	}
	
	//提醒是否开除
	void KaiChuWarring (int i)
	{
		if (i == 2)
		{
			FireMember fire = new FireMember ();
			
			fire.id = NewAlliancemanager.Instance().m_allianceHaveRes.id;
			fire.junzhuId = mMemberInfo.junzhuId;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer q_serializer = new QiXiongSerializer ();
			
			q_serializer.Serialize (t_stream,fire);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.FIRE_MEMBER,ref t_protof,"30118");
		}
	}
}
