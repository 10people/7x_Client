using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceApplicationData : MonoBehaviour,SocketProcessor {

	private static AllianceApplicationData applicateData;

	public static AllianceApplicationData Instance ()
	{
		if (!applicateData)
		{
			applicateData = (AllianceApplicationData)GameObject.FindObjectOfType (typeof(AllianceApplicationData));
		}

		return applicateData;
	}

	private AllianceHaveResp m_allianceInfo;//联盟信息

	private List<ApplicantInfo> applicateList = new List<ApplicantInfo>();//申请入盟的成员list

	public GameObject applicateManagerObj;

	public GameObject zheZhao;

	public GameObject selectObj;

	private RefuseApplyResp refuseApplyResp;
	private AgreeApplyResp agreeApplyResp;
	
	private string titleStr;
	private string str1;
	private string str2;
	private string cancelStr;
	private string confirmStr;

	private string backName;

	private bool isNewMember = false;//是否同意新员工加入联盟

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);
	}

	//获得自己联盟信息
	public void GetAllianceInfo (AllianceHaveResp allianceInfo)
	{
		m_allianceInfo = allianceInfo;
		Debug.Log ("联盟id：" + allianceInfo.id);
		ApplicateAllianceReq (allianceInfo);
	}

	//查看入盟申请成员请求
	void ApplicateAllianceReq (AllianceHaveResp tempInfo)
	{
		LookApplicants applicateReq = new LookApplicants ();

		applicateReq.id = tempInfo.id;

		MemoryStream t_stream = new MemoryStream ();

		QiXiongSerializer t_serializer = new QiXiongSerializer ();

		t_serializer.Serialize (t_stream,applicateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.LOOK_APPLICANTS,ref t_protof,"30124");
		Debug.Log ("ApplicateReq" + ProtoIndexes.LOOK_APPLICANTS);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.LOOK_APPLICANTS_RESP://查看申请入盟成员请求返回
			{
				Debug.Log ("ApplicateResp" + ProtoIndexes.LOOK_APPLICANTS_RESP);
				MemoryStream application_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer application_qx = new QiXiongSerializer();
				
				LookApplicantsResp applicateResp = new LookApplicantsResp();
				
				application_qx.Deserialize(application_stream, applicateResp, applicateResp.GetType());
				
				if (applicateResp != null)
				{
					Debug.Log ("申请者信息：" + applicateResp.applicanInfo);

					ApplicateManager applicate = applicateManagerObj.GetComponent<ApplicateManager>();

					if (applicateResp.applicanInfo != null)
					{
						applicateList = applicateResp.applicanInfo;
						Debug.Log ("申请者个数：" + applicateList.Count);

						applicate.GetApplicationInfo (applicateResp.applicanInfo,m_allianceInfo);
					}
					else
					{
						applicateList.Clear ();
						applicate.GetApplicationInfo (applicateList,m_allianceInfo);
						applicate.ClearItems ();

						Debug.Log ("没有申请者");
					}
				}

				return true;
			}
			
			case ProtoIndexes.REFUSE_APPLY_RESP://拒绝入盟申请请求返回
			{
				Debug.Log ("拒绝返回：" + ProtoIndexes.REFUSE_APPLY_RESP);
				MemoryStream refuse_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer refuse_qx = new QiXiongSerializer();
				
				RefuseApplyResp refuseResp = new RefuseApplyResp();
				
				refuse_qx.Deserialize(refuse_stream, refuseResp, refuseResp.GetType());
				
				if (refuseResp != null)
				{
					refuseApplyResp = refuseResp;

					if (refuseApplyResp.result == 0)
					{
//						ApplicateAllianceReq (m_allianceInfo);

						RefreshItemsList (refuseApplyResp.junzhuId);
					}

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        RefuseResourceLoadCallback );
				}
				
				return true;
			}

			case ProtoIndexes.AGREE_APPLY_RESP://同意入盟申请请求返回
			{
				Debug.Log ("同意返回：" + ProtoIndexes.AGREE_APPLY_RESP);
				MemoryStream agree_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer agree_qx = new QiXiongSerializer();
				
				AgreeApplyResp agreeResp = new AgreeApplyResp();
				
				agree_qx.Deserialize(agree_stream, agreeResp, agreeResp.GetType());
				
				if (agreeResp != null)
				{
					agreeApplyResp = agreeResp;

					if (agreeResp.result == 0)
					{
//						ApplicateAllianceReq (m_allianceInfo);
						Debug.Log ("applicateList.Count：" + applicateList.Count);
						RefreshItemsList (agreeResp.memberInfo.junzhuId);

						isNewMember = true;
					}

					else if (agreeResp.result == 3)
					{
						RefreshItemsList (agreeResp.junzhuId);
					}

					Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
					                        AgreeResourceLoadCallback );
				}
				
				return true;
			}

			default:return false;
			}
		}

		return false;
	}

	public void RefuseResourceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		titleStr = "拒绝申请";

		switch (refuseApplyResp.result)
		{

		case 0:
			
			Debug.Log ("Refuse Success!");

			str2 = "您已拒绝" + backName + "的入盟申请！";

			break;
			
		case 1:
			
			Debug.Log ("Refuse Fail!");

			str1 = "拒绝失败！";
			str2 = "您没有对该申请的拒绝权限！";

			break;
		}

		uibox.setBox(titleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,confirmStr,null,null);
	}

	public void AgreeResourceLoadCallback ( ref WWW p_www, string p_path,  Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		titleStr = "同意申请";

		int resultType = agreeApplyResp.result;
		if (resultType == 0)
		{
			Debug.Log ("Agree Success!");

			str2 = "您已同意" + backName + "的入盟申请！";

			uibox.setBox(titleStr, null, MyColorData.getColorString (1,str2), 
			             null,confirmStr,null,null);
		}

		else
		{
			Debug.Log ("Agree Fail!");

			str1 = "同意入盟申请失败！";

			if (resultType == 1)
			{
				Debug.Log ("没有权限");
				str2 = "您没有对该申请的同意权限！";
			}

			else if (resultType == 2)
			{
				Debug.Log ("联盟人数已满");
				str2 = "联盟人数已满！";
			}
			else if (resultType == 3)
			{
				Debug.Log ("取消申请或加入其他联盟");
				
				str1 = "同意入盟申请失败！";
				
				str2 = backName + "已取消申请或加入其他联盟！";
			}
			uibox.setBox(titleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
			             null,confirmStr,null,null);
		}
	}

	//刷新申请者列表
	void RefreshItemsList (long junZhuId)
	{
		for (int i = 0;i < applicateList.Count;i ++)
		{
			if (junZhuId == applicateList[i].junzhuId)
			{
				backName = applicateList[i].name;
				applicateList.Remove (applicateList[i]);
			}
		}
		Debug.Log ("applicateListCount:" + applicateList.Count);

		ApplicateManager applicate = applicateManagerObj.GetComponent<ApplicateManager>();
		applicate.GetApplicationInfo (applicateList,m_allianceInfo);
	}

	//控制遮罩的显隐
	public void MakeZheZhao (bool flag)
	{
		if (flag)
		{
			zheZhao.SetActive (true);
		}
		
		else 
		{
			zheZhao.SetActive (false);
		}
	}

	//克隆被选择的对勾
	public void Select (GameObject parentObj, Vector3 pos)
	{
		GameObject select = (GameObject)Instantiate (selectObj);
		
		select.SetActive (true);
		select.name = "SelectMe";
		
		select.transform.parent = parentObj.transform;
		
		select.transform.localPosition = pos;
		
		select.transform.localScale = Vector3.one;
	}
	
	//销毁item上的对勾
	public void DestroySelect ()
	{
		if (GameObject.Find ("SelectMe") != null)
		{
			Destroy (GameObject.Find ("SelectMe"));
		}
	}

	//返回
	public void Back ()
	{
		if (isNewMember)
		{
			AllianceData.Instance.RequestData ();
		}

		Destroy (this.gameObject);
	}
	
	//关闭
	public void CloseAll ()
	{
		if (isNewMember)
		{
			AllianceData.Instance.RequestData ();
		}

		Destroy (GameObject.Find ("My_Union(Clone)"));
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
