using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class GetAllianceData : MonoBehaviour , SocketListener{

	private static GetAllianceData m_instance;

//	public static GetAllianceData m_GetAllianceData;

	public static GetAllianceData Instance()
	{
		if (m_instance == null)
		{
			GameObject t_gameObject = GameObjectHelper.GetDontDestroyOnLoadGameObject();;
			
			m_instance = t_gameObject.AddComponent<GetAllianceData>();
		}
		
		return m_instance;
	}

	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
		
	}

	void Start()
	{
	
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);

		m_instance = null;
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
				
			case ProtoIndexes.LOOK_APPLICANTS_RESP://查看申请入盟成员请求返回
			{
//				Debug.Log ("查看申请入盟成员请求返回" + ProtoIndexes.LOOK_APPLICANTS_RESP);

				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				LookApplicantsResp applicateResp = new LookApplicantsResp();
				
				application_qx.Deserialize(application_stream, applicateResp, applicateResp.GetType());

				FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (104);
				
				int index = functionTemp.m_iID;
				if(AllianceData.Instance.g_UnionInfo.identity == 2 ||AllianceData.Instance.g_UnionInfo.identity == 1 )
				{
					if (applicateResp != null)
					{

						if (applicateResp.applicanInfo != null)
						{
							Debug.Log ("申请者信息：" + applicateResp.applicanInfo.Count);

							if(applicateResp.applicanInfo.Count > 0)
							{
								MainCityUI.SetRedAlert(index,true);

								return true;
							}
						}
					}
				}
			
				MainCityUI.SetRedAlert(index,false);
				return true;
			}
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}

}
