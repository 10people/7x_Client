using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GetActivityData : MonoBehaviour,SocketProcessor 
{
	public static GetActivityData m_activityData;

	public DailyAwardInfo m_awardInfo;


	void Awake()
	{
		m_activityData = this;
	}

	void OnEnable()
	{
		SocketTool.RegisterMessageProcessor(this);
	}

	void OnClick()
	{
		GetData();
	}

	public void GetData()
	{
		SocketTool.Instance().SendSocketMessage( ProtoIndexes.C_get_daily_award_info );
	}


	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				case ProtoIndexes.S_daily_award_info:
				{
					MemoryStream t_tream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
					
					QiXiongSerializer t_qx = new QiXiongSerializer();
					
					DailyAwardInfo tempAwardInfo = new DailyAwardInfo();
					
					t_qx.Deserialize(t_tream,tempAwardInfo,tempAwardInfo.GetType());

					m_awardInfo = tempAwardInfo;

				if(m_awardInfo.dailyAward.items == null)
				{
					Debug.Log("m_awardInfo = null");
				}

					foreach(DailyAward tempDail in m_awardInfo.dailyAward.items)
				{
					Debug.Log("装备ID：" + tempDail.awardIconId);
				}

					EnterGameActivityDataLayer();

				}return true;
			default: return false;
			}
		}
		return false;
	}

	void EnterGameActivityDataLayer(){
		if(UIYindao.m_UIYindao.m_isOpenYindao){
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
		}
 
        Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.GAME_ACTIVITY) , 
		                        LoadCallBack);
			 
		
	}

    public void  LoadCallBack(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = Instantiate(p_object) as GameObject;
        tempObject.name = "GameActivity";
    }
	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
