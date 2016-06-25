using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AppMember : MonoBehaviour {

	public ApplicantInfo mApplicantInfo;

	public GameObject AgreeBtnobj;

	public GameObject DisAgreeBtnobj;

	public UILabel Agree_orDis;

	public int  IsAgree;// 1 2 3 拒绝 同意  失败

	public int allianceId;

	public UILabel nameLabel;//名字
	
	public UILabel levelLabel;//等级
	
	public UILabel junXianLabel;//军衔

	public UILabel ZhanliLabel;//排名

	public AllianceHaveResp m_tAllianceHaveResp;

	public void init()
	{
		if(m_tAllianceHaveResp.identity == 1||m_tAllianceHaveResp.identity == 2)
		{
			AgreeBtnobj.SetActive(true);
			
			DisAgreeBtnobj.SetActive(true);
			
			Agree_orDis.gameObject.SetActive(false);

		}
		
		else
		{
			AgreeBtnobj.SetActive(false);
			
			DisAgreeBtnobj.SetActive(false);
			
			Agree_orDis.gameObject.SetActive(false);
			
		}
		nameLabel.text = mApplicantInfo.name;

//		BaiZhanTemplate mBaiZhanTemplate = BaiZhanTemplate.getBaiZhanTemplateById (mApplicantInfo.junXian);

		string mJX = QXComData.GetJunXianName (mApplicantInfo.junXian);

//		string mJX = NameIdTemplate.GetName_By_NameId (mBaiZhanTemplate.funDesc);

		junXianLabel.text = mJX;

		levelLabel.text = mApplicantInfo.level.ToString();

		ZhanliLabel.text = mApplicantInfo.zhanLi.ToString(); // 战力后台为添加
	}
	public void GetBackData()
	{
		AgreeBtnobj.SetActive(false);

		DisAgreeBtnobj.SetActive(false);

		Agree_orDis.gameObject.SetActive(true);

		if(IsAgree == 1)
		{
			Agree_orDis.text  = "已拒绝";
		}
		if(IsAgree == 2)
		{
			Agree_orDis.text = "已通过";
		}
		if(IsAgree == 3)
		{
			Agree_orDis.text = "审核失败";
		}

	}
	public void AgreeBtn()
	{
		AgreeApply agreeReq = new AgreeApply ();
		
		agreeReq.id = allianceId;
		agreeReq.junzhuId = mApplicantInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,agreeReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.AGREE_APPLY,ref t_protof,"30128");
		//Debug.Log ("同意请求");
	}

	public void DisAgreeBtn()
	{
		RefuseApply refuseReq = new RefuseApply ();
		
		refuseReq.id = allianceId;
		refuseReq.junzhuId = mApplicantInfo.junzhuId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,refuseReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.REFUSE_APPLY,ref t_protof,"30126");
		//Debug.Log ("拒绝请求");
	}
}
