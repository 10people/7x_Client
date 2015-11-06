using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class HYRewardBtnsCol : MonoBehaviour,SocketProcessor{

	public static HYRewardBtnsCol rewardBtnsCol;

	private RewardItemInfo rewardInfo;//获得的奖励物品信息
	private ApplyerInfo applicantItemInfo;//要分配给的玩家信息

	private ApplyRewardResp applyResp;//奖励申请返回信息
	private CancelApplyRewardResp cancelApplyResp;//取消奖励申请返回信息
	private GiveRewardResp giveRewardResp;//分配奖励返回信息

	private List<long> junZhuIdList = new List<long> ();

	private string rewardName;//当前申请奖励物品的名字
	private int itemId;

	[HideInInspector]public string lastRewardName;//上次申请物品名字

	private string titleStr;
	private string str;
	private string cancelStr;
	private string confirmStr;

	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);

		rewardBtnsCol = this;
	}

	public void GetRewardInfo (RewardItemInfo tempInfo)
	{
		rewardInfo = tempInfo;
	
		HuangYeAwardTemplete hyAwardTemp = HuangYeAwardTemplete.getHuangYeAwardTemplateBySiteId (tempInfo.site);
		itemId = hyAwardTemp.itemId;

		rewardName = NameIdTemplate.GetName_By_NameId (itemId);
	}

	//获得申请人信息
	public void GetApplicantInfo (ApplyerInfo tempInfo)
	{
		applicantItemInfo = tempInfo;
	}

	//申请奖励按钮
	public void ApplyBtn ()
	{
		Debug.Log ("ApplyBtn!");
		if (lastRewardName == "" || lastRewardName == null)
		{
			RewardApplyReq ();
		}

		else
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ApplyLoadBack);
		}
	}
	//Apply TipsWindow Load Back
	void ApplyLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		junZhuIdList.Clear ();
		foreach (ApplyerInfo applyer in rewardInfo.applyerInfo)
		{
			junZhuIdList.Add (applyer.junzhuId);
		}
		
		if (junZhuIdList.Contains (JunZhuData.Instance ().m_junzhuInfo.id))
		{
			Debug.Log ("Have Applied!");
			titleStr = "申请失败";
			
			str = "\n\n您已申请过" + rewardName + "！";
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,confirmStr,
			             null);
		}
		
		else
		{
			Debug.Log ("Not Apply!");
			titleStr = "申请提示";
			
			str = "\n您同一时间只能申请一件奖励，申请此件将会取消您申请以下奖励的队列\n" + lastRewardName + "\n是否要继续？";
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancelStr,confirmStr,
			             Apply);
		}
	}
	void Apply (int i)
	{
		if (i == 1)
		{
			Debug.Log ("Confirm Cancel Apply!");
		}
		
		else if (i == 2)
		{
			Debug.Log ("Confirm Apply!");
			RewardApplyReq ();
		}
	}
	
	//取消申请按钮
	public void CancelApplyBtn ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        CancelApplyLoadBack);
	}
	//Cancel Apply TipsWindow Load Back
	void CancelApplyLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		Debug.Log ("*****:" + rewardInfo.applyerInfo.Count);

		junZhuIdList.Clear ();
		foreach (ApplyerInfo applyer in rewardInfo.applyerInfo)
		{
			junZhuIdList.Add (applyer.junzhuId);
		}

		if (junZhuIdList.Contains (JunZhuData.Instance ().m_junzhuInfo.id))
		{
			Debug.Log ("Have Applied!");
			titleStr = "取消申请";
			
			str = "\n您是否要取消对以下奖励的申请？\n" + rewardName + "\n再次申请需要重新排队，是否要继续？";
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancelStr,confirmStr,
			             CancelApply);
		}

		else
		{
			Debug.Log ("Not Apply!");
			titleStr = "取消失败";
			
			str = "\n\n您还未申请过" + rewardName + ",无法取消申请！";
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,
			             null);
		}
	}
	void CancelApply (int i)
	{
		if (i == 1)
		{
			Debug.Log ("Close Cancel Apply!");
		}
		
		else if (i == 2)
		{
			if (rewardInfo.applyerInfo.Count > 0)
			{
				Debug.Log ("Confirm Cancel Apply!");
				CancelRewardApplyReq ();
			}
		}
	}

	//奖励分配
	public void DivideBtn ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        DivideLoadBack);
	}
	
	void DivideLoadBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		if (rewardInfo.nums > 0)
		{
			titleStr = "确认分配";
			str = "\n您是否要将以下奖励分配给以下盟员？\n" + rewardName + "\n" + applicantItemInfo.name;
			
			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,cancelStr,confirmStr,
			             DivideBack);
		}

		else
		{
			titleStr = "分配失败";
			str = "\n\n" + rewardName + "个数不足，无法分配！";

			uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,
			             null);
		}
	}
	
	void DivideBack (int i)
	{
		if (i == 2)
		{
			Divide ();
		}
	}

	//荒野奖励申请
	void RewardApplyReq ()
	{
		ApplyReward applyReq = new ApplyReward();
		
		applyReq.site = rewardInfo.site;
		
		MemoryStream t_stream = new MemoryStream();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,applyReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_APPLY_REWARD,ref t_protof,"30410");
		Debug.Log ("申请奖励:" + ProtoIndexes.C_APPLY_REWARD);
	}
	
	//荒野取消奖励申请
	void CancelRewardApplyReq ()
	{
		CancelApplyReward cancelApplyReq = new CancelApplyReward();
		
		cancelApplyReq.site = rewardInfo.site;
		
		MemoryStream t_stream = new MemoryStream();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,cancelApplyReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_CANCEL_APPLY_REWARD,ref t_protof,"30412");
		Debug.Log ("取消奖励申请:" + ProtoIndexes.C_CANCEL_APPLY_REWARD);
	}

	//荒野奖励分配
	void Divide ()
	{
		GiveReward giveReq = new GiveReward();
		
		giveReq.site = rewardInfo.site;
		giveReq.junzhuId = applicantItemInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,giveReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_GIVE_REWARD,ref t_protof,"30414");
		Debug.Log ("RewardApplyReq:" + ProtoIndexes.C_GIVE_REWARD);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_APPLY_REWARD://荒野奖励申请返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ApplyRewardResp rewardApplyResp = new ApplyRewardResp();
				
				t_qx.Deserialize(t_stream, rewardApplyResp, rewardApplyResp.GetType());
				
				if (rewardApplyResp != null)
				{
					switch (rewardApplyResp.result)
					{
					case 0:

//						Debug.Log ("申请成功!");
//						if (rewardApplyResp.curSite != null)
//						{
//							Debug.Log ("当前申请位置:" + rewardApplyResp.curSite);
//						}
//						if (rewardApplyResp.preSite != null)
//						{
//							Debug.Log ("前一个申请位置:" + rewardApplyResp.preSite);
//						}
//						if (rewardApplyResp.applyerInfo != null)
//						{
//							Debug.Log ("申请人id:" + rewardApplyResp.applyerInfo.junzhuId);
//						}

						applyResp = rewardApplyResp;

						HYRewardLibrary.hyReward.ApplySuccessRefresh (rewardApplyResp);

						break;
						
					case 1:
						
						Debug.Log ("申请失败!");
						
						break;
					}

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        ApplyAwardLoacBack);
				}
				
				return true;
			}
				
			case ProtoIndexes.S_CANCEL_APPLY_REWARD://荒野奖励取消申请返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				CancelApplyRewardResp cancelRewardApplyResp = new CancelApplyRewardResp();
				
				t_qx.Deserialize(t_stream, cancelRewardApplyResp, cancelRewardApplyResp.GetType());
				
				if (cancelRewardApplyResp != null)
				{
					switch (cancelRewardApplyResp.result)
					{
					case 0:

						Debug.Log ("取消申请成功!");
						if (cancelRewardApplyResp.site != null)
						{
							Debug.Log ("取消申请位置:" + cancelRewardApplyResp.site);
						}
						if (cancelRewardApplyResp.junzhuId != null)
						{
							Debug.Log ("取消申请id:" + cancelRewardApplyResp.junzhuId);
						}

						cancelApplyResp = cancelRewardApplyResp;

						HYRewardLibrary.hyReward.CancelApplySuccessRefresh (cancelRewardApplyResp);

						break;
						
					case 1:
						
						Debug.Log ("取消申请失败!");
						
						break;
					}

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        CancelApplyAwardLoacBack);
				}
				
				return true;
			}

			case ProtoIndexes.S_GIVE_REWARD:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				GiveRewardResp giveResp = new GiveRewardResp();
				
				t_qx.Deserialize(t_stream, giveResp, giveResp.GetType());
				
				if (giveResp != null)
				{
					giveRewardResp = giveResp;

					HYRewardLibrary.hyReward.DivideSuccessRefresh (giveRewardResp);

					Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        GiveAwardLoacBack);
				}
				
				return true;
			}

			default:return false;
			}
		}

		return false;
	}
	
	void ApplyAwardLoacBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		if (applyResp.result == 0)
		{
			string curRewardName = "";
			int curRow = 0;
			foreach (RewardItemInfo tempInfo in HYRewardLibrary.hyReward.m_rewardResp.itemInfo)
			{
				if (applyResp.curSite == tempInfo.site)
				{
					HuangYeAwardTemplete hyAwardTemp = HuangYeAwardTemplete.getHuangYeAwardTemplateBySiteId (tempInfo.site);
					curRewardName = NameIdTemplate.GetName_By_NameId (hyAwardTemp.itemId);

					curRow = tempInfo.applyerInfo.Count;
				}
			}

			titleStr = "申请成功";
			str = "\n您已成功加入申请以下奖励的队列\n" + curRewardName + "\n您目前排在第" + curRow + "位";
		}
		
		else if (applyResp.result == 1)
		{
			titleStr = "申请失败";
			
			str = "\n\n已申请过该奖励！";
		}
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,
		             null);
	}

	void CancelApplyAwardLoacBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		if (cancelApplyResp.result == 0)
		{
			titleStr = "取消申请";
			str = "\n\n您已成功取消该奖励的申请。";
		}
		
		else if (cancelApplyResp.result == 1)
		{
			titleStr = "取消失败";
			
			str = "\n\n您还未申请该奖励！";
		}
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,
		             null);
	}

	void GiveAwardLoacBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		if (giveRewardResp.result == 0)
		{
			Debug.Log ("分配成功");
			titleStr = "分配成功";
			str = "\n您已成功将以下奖励分配给以下盟员。\n" + rewardName + "\n" + applicantItemInfo.name;
		}
		
		else if (giveRewardResp.result == 1)
		{
			Debug.Log ("分配失败");
			titleStr = "分配失败";
			
			str = "\n\n系统已分配奖励!";
		}
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str),null,null,confirmStr,null,
		             null);
	}

	//详情按钮
	public void XiangQingBtn ()
	{
		Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        XiangQingLoacBack);
	}

	void XiangQingLoacBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		titleStr = "奖励详情";
		str = "\n" + rewardName + "\n" + DescIdTemplate.GetDescriptionById (itemId);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirmStr,null,
		             null);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
