using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceKuangMaiInfo : MonoBehaviour {

	public UILabel costYbLabel;//显示需要元宝数label

	public GameObject allianceKMBtn;

	public UISprite daZheSprite;//打折sprite
	
	private int type;//探宝类型
	
	//获得联盟矿脉信息
	public void GetAllianceKmInfo (ExploreMineInfo tempAllianceKm)
	{
		type = tempAllianceKm.type;
		//		Debug.Log ("联盟矿脉：" + tempAllianceKm.type);

		if (FunctionOpenTemp.GetWhetherContainID (1102))
		{
			allianceKMBtn.SetActive (true);
			daZheSprite.gameObject.SetActive (true);
			costYbLabel.text = tempAllianceKm.cost.ToString ();
			
			switch (tempAllianceKm.discount)
			{
			case 9:
				
				daZheSprite.spriteName = "9zhe";
				
				break;
			}
		}
		
		else
		{
			allianceKMBtn.SetActive (false);
			daZheSprite.gameObject.SetActive (false);
			costYbLabel.text = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1102));
		}
	}
	
	//联盟矿脉领取请求
	public void AllianceKmReq ()
	{
		if (!TanBaoManager.tbManager.IsClick)
		{
			TanBaoManager.tbManager.IsClick = true;

			switch (QXTanBaoData.Instance ().tanBaoResp.hasGuild)
			{
			case false:
				
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        ReqFailBack);
				
				break;
				
			case true:
				
				AllianceKnReqSend ();
				
				break;
				
			default:break;
			}
		}
	}

	void AllianceKnReqSend ()
	{
		TanBaoManager.tbManager.SetActiveZheZhao (true);
		
		ExploreReq tbBuy = new ExploreReq();
		
		tbBuy.type = type;
		tbBuy.isBuy = true;
		
		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,tbBuy);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.EXPLORE_REQ,ref t_protof,"30006|30005");
		Debug.Log ("tbBuyReq:" + ProtoIndexes.EXPLORE_REQ);
	}

	void ReqFailBack (ref WWW p_www,string p_path, Object p_object)
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();
		
		string titleStr = "提示";
		string str = "您需要创建或加入一个联盟后才能使用联盟矿脉的探宝功能！";
		
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirm,null,
		             ReqBack);
	}

	void ReqBack (int i)
	{
		TanBaoManager.tbManager.IsClick = false;
	}
}
