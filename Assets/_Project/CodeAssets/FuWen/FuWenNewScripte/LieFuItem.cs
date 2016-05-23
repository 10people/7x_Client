using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class LieFuItem : MonoBehaviour {

	public UILabel ItemName;
	public UISprite ItemPinZhi;
	public UISprite Icon;
	public UILabel PreCost;
	public UIEventListener mEventListener;

	public LieFuActionInfo mLieFuActionInfo ;

	public GameObject HitEnabel;

	public BoxCollider mColider;
	void Start () {
	

	}
	

	void Update () {
	
	}
	public void Init()
	{
		mColider.gameObject.name = "Button_"+mLieFuActionInfo.type;
		mColider.enabled = false;

		LieFuTemplate mLieFuTemp = LieFuTemplate.getLieFuTemplateBy_Id (mLieFuActionInfo.type);
		ItemName.text = mLieFuTemp.Name;
		Icon.spriteName = mLieFuTemp.IconId.ToString ();
		if(mLieFuActionInfo.cost <= 0)
		{
			PreCost.text = "首次免费";
		}else
		{
			PreCost.text = mLieFuActionInfo.cost.ToString ()+"铜币";
		}

		Color p_color = Color.red;
		switch(mLieFuActionInfo.type)
		{
		case 1:
			ItemPinZhi.spriteName = "pinzhi1";
			MathHelper.ParseHexString( "10dc00", out p_color, Color.red );
			PreCost.color = p_color;
			break;
		case 2:
			ItemPinZhi.spriteName = "pinzhi3";
			MathHelper.ParseHexString( "00cbe9", out p_color, Color.red );
			PreCost.color = p_color;
			break;
		case 3:
			ItemPinZhi.spriteName = "pinzhi6";
			MathHelper.ParseHexString( "d001e8", out p_color, Color.red );
			PreCost.color = p_color;
			break;
		case 4:
			ItemPinZhi.spriteName = "pinzhi9";
			MathHelper.ParseHexString( "eb4200", out p_color, Color.red );
			PreCost.color = p_color;
			break;
		default:
			break;
		}
		if(mLieFuActionInfo.state == 0)
		{
			Icon.color = Color.gray;
			mColider.enabled = false;
			HitEnabel.SetActive(true);
		}
		else
		{
			Icon.color = Color.white;
			HitEnabel.SetActive(false);
			mColider.enabled = true;
		}
	}
	public void LieFuBtn()
	{
//		Debug.Log ("mLieFuActionInfo.state = "+mLieFuActionInfo.state);
//		Debug.Log ("mLieFuActionInfo.type = "+mLieFuActionInfo.type);
		if(JunZhuData.Instance().m_junzhuInfo.jinBi < mLieFuActionInfo.cost )
		{
			Global.CreateFunctionIcon (501);

			return;
		}
		if(mLieFuActionInfo.state != 0)
		{
			if(mLieFuActionInfo.type != 1)
			{
				mColider.enabled = false;
				HitEnabel.SetActive(true);
			}

			LieFuActionReq  mLieFuActionReq  = new LieFuActionReq  ();
			MemoryStream MiBaoinfoStream = new MemoryStream ();
			QiXiongSerializer MiBaoinfoer = new QiXiongSerializer ();
			
			mLieFuActionReq.type = mLieFuActionInfo.type;
			MiBaoinfoer.Serialize (MiBaoinfoStream,mLieFuActionReq);
			
			byte[] t_protof;
			t_protof = MiBaoinfoStream.ToArray();
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.LieFu_Action_req,ref t_protof);
		}
	
	}
}
