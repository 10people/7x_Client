using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LeaderItem : MonoBehaviour {

	public UISprite headIcon;//头像icon
	
	public UISprite zhiWei;//职位
	
	public UILabel levelLabel;//等级
	
	public UILabel nameLabel;//名字
	
	private MemberInfo thisMemberInfo;//成员详情
	
	private AllianceHaveResp m_allianceInfo;//联盟信息

	private string leaderName;//副盟主名字

	//获取副盟主item信息
	public void ShowLeaderItemInfo (MemberInfo tempInfo,AllianceHaveResp allianceInfo)
	{
		thisMemberInfo = tempInfo;
		m_allianceInfo = allianceInfo;
		
		zhiWei.gameObject.SetActive (true);
		zhiWei.spriteName = "fumenzhu";
		
		levelLabel.text = tempInfo.level.ToString ();

		headIcon.spriteName = "PlayerIcon" + tempInfo.roleId;

		leaderName = tempInfo.name;
		nameLabel.text = tempInfo.name;
	}

	void OnClick ()
	{
		Vector3 localPos = new Vector3 (40f,-60f,0);
		TransAlliance.trans.Select (this.gameObject,localPos);

		TransAlliance.trans.MakeZheZhao (true);

		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
		                        ResourceLoadCallback );
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path,  Object p_object ){
		GameObject boxObj = Instantiate( p_object ) as GameObject;
		
		UIBox uibox = boxObj.GetComponent<UIBox> ();

		string str1 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_DES1) + leaderName + "?";
		string str2 = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_DES2);

		string transTitleStr = LanguageTemplate.GetText (LanguageTemplate.Text.ALLIANCE_TRANS_TITLE);

		string confirmStr = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		string cancelStr = LanguageTemplate.GetText (LanguageTemplate.Text.CANCEL);

		uibox.setBox(transTitleStr, MyColorData.getColorString (1,str1),MyColorData.getColorString (1,str2), 
		             null,cancelStr,confirmStr,TransformAlliance);
	}

	void TransformAlliance (int i)
	{
		TransAlliance.trans.MakeZheZhao (false);

		if (i == 1)
		{
			//Debug.Log ("不转让了");
			TransAlliance.trans.DestroySelect ();
		}

		if (i == 2)
		{
			//Debug.Log ("确定转让");
			TransAllianceReq ();
		}
	}

	//转让请求
	void TransAllianceReq ()
	{
		TransferAlliance transReq = new TransferAlliance ();

		transReq.id = m_allianceInfo.id;
		transReq.junzhuId = thisMemberInfo.junzhuId;

		MemoryStream t_stream = new MemoryStream ();

		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,transReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance().SendSocketMessage (ProtoIndexes.TRANSFER_ALLIANCE,ref t_protof,"30138");
	}
}
