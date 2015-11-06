using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class EveryDayActivity : MonoBehaviour,SocketProcessor {

	public List<UITexture> m_spriteList = new List<UITexture>();

	public List<UILabel> m_labelList = new List<UILabel>();

	public UIButton m_getAward;
	

	void Start()
	{
		SocketTool.UnRegisterMessageProcessor(GetActivityData.m_activityData);

		EventDelegate.Add(m_getAward.onClick,GetAward);
	}

	void OnEnable()
	{
		SocketTool.RegisterMessageProcessor(this);

		RefreshLayer();
	}

    int index = 0;
	void RefreshLayer()
	{
		int tempCount = GetActivityData.m_activityData.m_awardInfo.dailyAward.items.Count;

		for(int i = 0;i < tempCount; i++){
			int tempIconId = GetActivityData.m_activityData.m_awardInfo.dailyAward.items[i].awardIconId;
            index = i;
			Debug.Log("奖励物品id：" + tempIconId);
            Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.EQUIP_ICON_PREFIX) + tempIconId, 
			                        LoadCallback);
		

			m_labelList[i].text = GetActivityData.m_activityData.m_awardInfo.dailyAward.items[i].awardName + "\nX" + GetActivityData.m_activityData.m_awardInfo.dailyAward.items[i].cnt;
		}

		for(int i = tempCount;i < m_spriteList.Count;i ++){
			m_spriteList[i].gameObject.SetActive(false);
		}

		if(GetActivityData.m_activityData.m_awardInfo.dailyAward.yiLing > 0){
			m_getAward.GetComponentInChildren<UISprite>().spriteName = "button_alGetAward";
			
			m_getAward.GetComponent<BoxCollider>().enabled = false;
			
		}
		else{
			m_getAward.GetComponentInChildren<UISprite>().spriteName = "button_GetAward";
			
			m_getAward.GetComponent<BoxCollider>().enabled = true;
		}
	}

    public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
		m_spriteList[index].mainTexture = (Texture)p_object;
    }

	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		Debug.Log("领取成功。。。");

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

				GetActivityData.m_activityData.m_awardInfo = tempAwardInfo;

				RefreshLayer();
				if( UIYindao.m_UIYindao.m_isOpenYindao ){
					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
				}

			}return true;
			default: return false;
			}
		}
		return false;
	}


	void GetAward()
	{
		GetDailyAward tempAward = new GetDailyAward();
		tempAward.awardId = CityGlobalData.m_everyDaywardId;

		MemoryStream t_tream = new MemoryStream();
		QiXiongSerializer t_tx = new QiXiongSerializer();
		t_tx.Serialize(t_tream,tempAward);

		byte[] t_bytes;
		t_bytes = t_tream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_get_daily_award,ref t_bytes);
	}


	void OnDisable()
	{
		SocketTool.UnRegisterMessageProcessor(this);

		SocketTool.RegisterMessageProcessor(GetActivityData.m_activityData);
	}
}
