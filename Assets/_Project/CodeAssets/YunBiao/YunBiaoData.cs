using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class YunBiaoData : Singleton<YunBiaoData>,SocketProcessor {

	public YabiaoMainInfoResp yunBiaoRes;

	public bool isNewEnemy;//是否有新仇人
	public bool isNewRecord;//是否有新记录

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	/// <summary>
	/// 运镖首页信息请求
	/// </summary>
	public void YunBiaoInfoReq ()
	{
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_YABIAO_INFO_REQ,"3402");
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_YABIAO_INFO_RESP://运镖首页信息返回
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				YabiaoMainInfoResp yunBiaoInfoRes = new YabiaoMainInfoResp();
				
				t_qx.Deserialize(t_stream, yunBiaoInfoRes, yunBiaoInfoRes.GetType());
				
				if (yunBiaoInfoRes != null)
				{
//					Debug.Log ("还剩运镖次数：" + yunBiaoInfoRes.yaBiaoCiShu);
//					Debug.Log ("还剩截镖次数：" + yunBiaoInfoRes.jieBiaoCiShu);
//					Debug.Log ("已经购买次数：" + yunBiaoInfoRes.buyCiShu);
//					Debug.Log ("新仇人：" + yunBiaoInfoRes.isNew4Enemy);
//					Debug.Log ("新记录：" + yunBiaoInfoRes.isNew4History);

					yunBiaoRes = yunBiaoInfoRes;

					isNewEnemy = yunBiaoInfoRes.isNew4Enemy;
					isNewRecord = yunBiaoInfoRes.isNew4History;

					CheckNewRecordOrEnemy (isNewEnemy,isNewRecord);

					GameObject yunBiaoObj = GameObject.Find ("YunBiaoMainPage");
					
					if (yunBiaoObj != null)
					{
						YunBiaoMainPage.yunBiaoMainData.InItYunBiaoMainPage (yunBiaoInfoRes);
						YunBiaoMainPage.yunBiaoMainData.CheckNewEnemyOrRecord (isNewEnemy,isNewRecord);
					}
				}
				
				return true;
			}

			case ProtoIndexes.S_PUSH_YBRECORD_RESP://运镖信息提示（历史，仇人）
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				isNew4RecordResp yunBiaoTips = new isNew4RecordResp();
				
				t_qx.Deserialize(t_stream, yunBiaoTips, yunBiaoTips.GetType());
				
				if (yunBiaoTips != null)
				{
					Debug.Log ("新仇人：" + yunBiaoTips.isNew4Enemy);
					Debug.Log ("新记录：" + yunBiaoTips.isNew4History);

					isNewEnemy = yunBiaoTips.isNew4Enemy;
					isNewRecord = yunBiaoTips.isNew4History;

					CheckNewRecordOrEnemy (isNewEnemy,isNewRecord);

					GameObject yunBiaoObj = GameObject.Find ("YunBiaoMainPage");

					if (yunBiaoObj != null)
					{
						YunBiaoMainPage.yunBiaoMainData.CheckNewEnemyOrRecord (isNewEnemy,isNewRecord);
					}
				}

				return true;
			}
			}
		}

		return false;
	}
 	
	/// <summary>
	/// 检测是否有新的历史或仇人
	/// </summary>
	public void CheckNewRecordOrEnemy (bool tempNewEnemy,bool tempNewRecord)
	{
		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (310);
		int index = functionTemp.m_iID;

		if (tempNewEnemy || tempNewRecord)
		{
			//收纳按钮提示
			MainCityUIRB.SetRedAlert (index,true);
		}
		else
		{
			//取消收纳按钮提示
			MainCityUIRB.SetRedAlert (index,false);
		}
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
