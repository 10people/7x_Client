using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceFightData : Singleton<AllianceFightData>,SocketProcessor {

	public RequestFightInfoResp allianceFightResp;

	private int applyResult;//报名结果

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	//联盟战信息请求
	public void AllianceFightDataReq ()
	{
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.ALLIANCE_FIGHT_INFO_REQ,"4202");
		Debug.Log ("联盟战信息请求：" + ProtoIndexes.ALLIANCE_FIGHT_INFO_REQ);
	}

	//联盟战报名请求
	public void AllianceFightApplyReq ()
	{
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.ALLIANCE_FIGHT_APPLY,"4204");
		Debug.Log ("联盟战报名请求：" + ProtoIndexes.ALLIANCE_FIGHT_INFO_REQ);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ALLIANCE_FIGHT_INFO_RESP://联盟战信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				RequestFightInfoResp allianceFightData = new RequestFightInfoResp();
				
				t_qx.Deserialize(t_stream, allianceFightData, allianceFightData.GetType());

				if (allianceFightData != null)
				{
					Debug.Log ("联盟战状态：" + allianceFightData.state + "//赛程，0-无，1-32强，2-16强，3-8强，4-4强，5-半决赛，6-三四名比赛，7-决赛，8-报名");
					Debug.Log ("是否可报名：" + allianceFightData.isCanApply  + "//true-可以，false-不可以");
					Debug.Log ("是否有资格参赛：" + allianceFightData.isCanFight + "//true-有资格，false-没有资格");
					Debug.Log ("是否报名：" + allianceFightData.isApply + "//false-未报名，true-已经报名");
					Debug.Log ("距离报名结束时间：" + allianceFightData.applyRemaintime  + "//单位-秒");
					Debug.Log ("比赛状态：" + allianceFightData.fightState  + "//0-未开始，1-正在进行中，2-已经结束");
					Debug.Log ("比赛开启时间：" + allianceFightData.startTime);

					if (allianceFightData.matchInfos == null)
					{
						allianceFightData.matchInfos = new List<FightMatchInfo>();
					}

					allianceFightResp = allianceFightData;

					GameObject allianceFightObj = GameObject.Find ("AllianceFightMainPageObj");
					if (allianceFightObj != null)
					{
						AllianceFightMainPage allianceMain = allianceFightObj.GetComponent<AllianceFightMainPage> ();
						allianceMain.GetAllianceFightResp (allianceFightData);
					}
				}

				return true;
			}
			case ProtoIndexes.ALLIANCE_FIGHT_APPLY_RESP://联盟战报名返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ApplyFightResp fightApplyResp = new ApplyFightResp();
				
				t_qx.Deserialize(t_stream, fightApplyResp, fightApplyResp.GetType());

				if (fightApplyResp != null)
				{
					Debug.Log ("报名结果：" + fightApplyResp.result +
					           "//0-成功，1-不是联盟成员，2-没有报名权限，" +
					           "3-找不到所在联盟，4-联盟等级不足，5-联盟成员数不足，" +
					           "6-联盟建设值不足，7-现在不是报名时间");

					applyResult = fightApplyResp.result;

					Global.CreateBox ("报名结果",ResultStr (fightApplyResp.result),null,null,"确定",null,null,null);

					if (fightApplyResp.result == 0)
					{
						AllianceFightDataReq ();
					}
				}

				return true;
			}
			}
		}
		return false;
	}

	//打开联盟战首页
	public void OpenAllianceFightMainPage ()
	{
//		ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"[dc0600]暂未开启，敬请期待[-]"));
//		return;
		if (JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"请先加入一个联盟"));
		}
		else
		{
			if (AllianceData.Instance.g_UnionInfo.level >= 5)
			{
				Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.ALLIANCE_BATTLE_WINDOW ),
				                        AllianceFightLoadBack );
			}
			else
			{
				ClientMain.m_UITextManager.createText(MyColorData.getColorString (1,"请先将联盟等级升到[dc0600]5[-]级"));
			}
		}
	}
	
	void AllianceFightLoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		GameObject allianceFight = GameObject.Instantiate( p_object ) as GameObject;

		allianceFight.name = "AllianceFightMainPageObj";

		if (UIYindao.m_UIYindao.m_isOpenYindao)
		{
			CityGlobalData.m_isRightGuide = true;
		}
	}

	void AllianceFightApplyCallBack (int i)
	{

	}

	/// <summary>
	/// 报名结果描述 0-成功，1-不是联盟成员，2-没有报名权限，3-找不到所在联盟，4-联盟等级不足，5-联盟成员数不足，6-联盟建设值不足，7-现在不是报名时间
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="i">The index.</param>
	string ResultStr (int i)
	{
		string result = "";
		switch (i)
		{
		case 0:
			result = "\n\n报名成功！";
			break;
		case 1:
			result = "\n报名失败！\n不是联盟成员！";
			break;
		case 2:
			result = "\n报名失败！\n没有报名权限！";
			break;
		case 3:
			result = "\n报名失败！\n找不到所在联盟！";
			break;
		case 4:
			result = "\n报名失败！\n联盟等级不足！";
			break;
		case 5:
			result = "\n报名失败！\n联盟成员数不足！";
			break;
		case 6:
			result = "\n报名失败！\n联盟建设值不足！";
			break;
		case 7:
			result = "\n报名失败！\n现在不是报名时间！";
			break;
		default:
			break;
		}
		return MyColorData.getColorString (1,result);
	}

	public void OnDestroy (){
		SocketTool.UnRegisterMessageProcessor (this);

		base.OnDestroy();
	}
}
