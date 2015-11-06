using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class CountDayActivity : MonoBehaviour,SocketProcessor {

	public UIGrid m_grid;

	public UIButton m_getAwardButton;

	List<GameObject> m_awardObject = new List<GameObject>();



	void Start()
	{
		SocketTool.UnRegisterMessageProcessor(GetActivityData.m_activityData);

		EventDelegate.Add(m_getAwardButton.onClick,GetAward);
	}


	void OnEnable()
	{
		SocketTool.RegisterMessageProcessor(this);

		RefreshLayer();
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

					GetActivityData.m_activityData.m_awardInfo = tempAwardInfo;
					
					RefreshLayer();

				}return true;
			default: return false;
			}
		}
		return false;
	}

	void GetAward()
	{
		GetDailyAward tempAward = new GetDailyAward();
		tempAward.awardId = CityGlobalData.m_countAwardID;
		
		MemoryStream t_tream = new MemoryStream();
		QiXiongSerializer t_tx = new QiXiongSerializer();
		t_tx.Serialize(t_tream,tempAward);
		
		byte[] t_bytes;
		t_bytes = t_tream.ToArray();
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_get_daily_award,ref t_bytes);
	}

    int index_Name = 0;
    DailyAwardArr rewardarr;
	void RefreshLayer()
	{
		int tempCount = GetActivityData.m_activityData.m_awardInfo.loginAward.Count;

		Debug.Log("累计登陆奖励：" + tempCount);

		List<DailyAwardArr> tempAward = GetActivityData.m_activityData.m_awardInfo.loginAward;

		for(int i = 0;i < m_awardObject.Count;i ++)
		{
			m_awardObject[i].GetComponent<ActivityAwardItem>().InitWith(tempAward[i]);
		}

		if(GetActivityData.m_activityData.m_awardInfo.dailyAward.yiLing > 0)
		{
			m_getAwardButton.GetComponent<BoxCollider>().enabled = false;

			m_getAwardButton.GetComponentInChildren<UISprite>().spriteName = "button_alGetAward";
		}else
		{
			m_getAwardButton.GetComponent<BoxCollider>().enabled = true;
			
			m_getAwardButton.GetComponentInChildren<UISprite>().spriteName = "button_GetAward";
		}

		for(int i = 0;i < tempCount - m_awardObject.Count;i ++)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath(Res2DTemplate.Res.DAILY_REWARD_ITEM),
                                        LoadCallback);
            index_Name = i + 1;
	 
            rewardarr = tempAward[i];
		}
		m_grid.Reposition();
	}
    public void LoadCallback(ref WWW p_www, string p_path, Object p_object)
    {
        GameObject tempObject = (GameObject)Instantiate(p_object);
        tempObject.name = index_Name.ToString();

        tempObject.transform.parent = m_grid.transform;

        tempObject.transform.localScale = Vector3.one;

        tempObject.transform.localPosition = Vector3.zero;

        tempObject.GetComponent<ActivityAwardItem>().InitWith(rewardarr);

        m_awardObject.Add(tempObject);
    }

	void OnDisable()
	{
		SocketTool.UnRegisterMessageProcessor(this);

		SocketTool.RegisterMessageProcessor(GetActivityData.m_activityData);
	}
}
