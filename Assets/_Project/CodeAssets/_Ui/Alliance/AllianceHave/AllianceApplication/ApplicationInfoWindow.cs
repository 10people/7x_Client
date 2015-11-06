using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class ApplicationInfoWindow : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UILabel nameLabel;//名字
	
	public UILabel levelLabel;//等级
	
	public UILabel junXianLabel;//军衔

	public UILabel rankLabel;//排名

	private int allianceId;//联盟id 
	private long applicateId;//申请入盟人员id

	//获得申请者信息
	public void GetApplicationInfo (ApplicantInfo tempInfo,AllianceHaveResp myAllianceInfo)
	{
		allianceId = myAllianceInfo.id;
		applicateId = tempInfo.junzhuId;

		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;

		nameLabel.text = tempInfo.name;

		levelLabel.text = tempInfo.level.ToString () + "级";

		int junXianId = tempInfo.junXian;//军衔id

		BaiZhanTemplate baizhanTemp = BaiZhanTemplate.getBaiZhanTemplateById (junXianId);
		
		junXianLabel.text = NameIdTemplate.GetName_By_NameId (baizhanTemp.templateName);

		rankLabel.text = tempInfo.rank.ToString ();
	}

	//拒绝入盟按钮
	public void RefuseBtn ()
	{
		RefuseApply refuseReq = new RefuseApply ();
		
		refuseReq.id = allianceId;
		refuseReq.junzhuId = applicateId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,refuseReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.REFUSE_APPLY,ref t_protof,"30126");
		Debug.Log ("拒绝请求");
		DestroyWin ();
	}

	//同意入盟按钮
	public void AgreeBtn ()
	{
		AgreeApply agreeReq = new AgreeApply ();
		
		agreeReq.id = allianceId;
		agreeReq.junzhuId = applicateId;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,agreeReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.AGREE_APPLY,ref t_protof,"30128");
		Debug.Log ("同意请求");
		DestroyWin ();
	}

	void OnClick ()
	{
		DestroyWin ();
		Debug.Log ("关闭页面");
	}

	void DestroyWin ()
	{
		AllianceApplicationData.Instance ().DestroySelect ();
		AllianceApplicationData.Instance ().MakeZheZhao (false);
		Destroy (this.gameObject);
	}
}
