using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class MemberInfoBtns : MonoBehaviour {

	public EventHandler sureBtn;//确定按钮

	public EventHandler kaiChuBtn;//开除按钮

	public EventHandler shengZhiBtn;//升职按钮

	public EventHandler jiangZhiBtn;//降职按钮

	private int iAllianceId;//联盟id

	private long iMemberId;//成员id

	private string sMemberName;//成员名字

	private GameObject mainInfoObj;//包含信息窗口以及按钮的obj

	void Start ()
	{
		mainInfoObj = this.gameObject.transform.FindChild ("MainInfo").gameObject;

		kaiChuBtn.m_click_handler += KaiChu;
		shengZhiBtn.m_click_handler += ShengZhi;
		jiangZhiBtn.m_click_handler += JiangZhi;

		sureBtn.m_click_handler += SureBtnClick;
	}

	//确定按钮
	void SureBtnClick (GameObject tempObj)
	{
		DestroyThisObj ();
	}

	//获得id
	public void GetThisMemberInfo (MemberInfo m_info,AllianceHaveResp m_alliance)
	{
		iAllianceId = m_alliance.id;
		iMemberId = m_info.junzhuId;
		sMemberName = m_info.name;
	}

	//开除成员
	void KaiChu (GameObject tempObj)
	{
		mainInfoObj.SetActive (false);

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ResourceLoadCallback );

	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object ){
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent <UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_ASKSTR1);
		string str2 = sMemberName + LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_ASKSTR2);

		string warringTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_KAICHU_WARRING_TITLE);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		uibox.setBox(warringTitleStr, MyColorData.getColorString (1,str1), MyColorData.getColorString (1,str2), 
		             null,cancelStr,confirmStr,KaiChuWarring);
	}

	//提醒是否开除
	void KaiChuWarring (int i)
	{
		if (i == 2)
		{
			FireMember fire = new FireMember ();
			
			fire.id = iAllianceId;
			fire.junzhuId = iMemberId;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer q_serializer = new QiXiongSerializer ();
			
			q_serializer.Serialize (t_stream,fire);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance().SendSocketMessage (ProtoIndexes.FIRE_MEMBER,ref t_protof,"30118");
		}
		DestroyThisObj ();
	}

	//升职
	void ShengZhi (GameObject tempObj)
	{
		UpTitle up = new UpTitle ();

		up.id = iAllianceId;
		up.junzhuId = iMemberId;

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer q_serializer = new QiXiongSerializer ();
		
		q_serializer.Serialize (t_stream,up);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.UP_TITLE,ref t_protof,"30120");

		DestroyThisObj ();
	}

	//降职
	void JiangZhi (GameObject tempObj)
	{
		UpTitle down = new UpTitle ();

		down.id = iAllianceId;
		down.junzhuId = iMemberId;

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer q_serializer = new QiXiongSerializer ();
		
		q_serializer.Serialize (t_stream,down);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.DOWN_TITLE,ref t_protof,"30122");

		DestroyThisObj ();
	}

	void DestroyThisObj ()
	{
		AllianceMember.a_member.DestroySelect ();
		AllianceMember.a_member.MakeZheZhao (false);
		Destroy (this.gameObject);
	}
}
