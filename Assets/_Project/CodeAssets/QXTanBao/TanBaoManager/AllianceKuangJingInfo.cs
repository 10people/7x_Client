using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class AllianceKuangJingInfo : MonoBehaviour {

	public GameObject allianceKjObj;

	public GameObject gongXianIcon;
			
	public UILabel ybLabel;//显示元宝数的label
	
	private ExploreMineInfo tempKjInfo;

	//获得联盟矿井信息
	public void GetAllianceKjInfo (ExploreMineInfo tempAllianceKj)
	{
		tempKjInfo = tempAllianceKj;
		//		Debug.Log ("联盟矿井：" + tempAllianceKj.type);

		if (FunctionOpenTemp.GetWhetherContainID (1102))
		{
			allianceKjObj.SetActive (true);
			gongXianIcon.SetActive (true);
			ybLabel.text = tempAllianceKj.cost.ToString ();

		}
		
		else
		{
			allianceKjObj.SetActive (false);
			gongXianIcon.SetActive (false);
			ybLabel.text = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1102));
		}
	}
	
	//联盟矿井领取请求
	public void AllianceKjReq ()
	{
		if (!TanBaoManager.tbManager.IsClick)
		{
			TanBaoManager.tbManager.IsClick = true;

			switch (QXTanBaoData.Instance ().tanBaoResp.hasGuild)//0-not have 1- have
			{
			case false:
				
				Global.ResourcesDotLoad(Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
				                        ReqFailBack);
				
				break;
				
			case true:
				
				AllianceKjReqSend ();
				
				break;
			}
		}
	}

	void AllianceKjReqSend ()
	{
		TanBaoManager.tbManager.SetActiveZheZhao (true);
		
		ExploreReq tbBuy = new ExploreReq();
		
		tbBuy.type = tempKjInfo.type;
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
		string str = "您需要创建或加入一个联盟后才能使用联盟矿井的探宝功能！";
		
		string confirm = LanguageTemplate.GetText (LanguageTemplate.Text.CONFIRM);
		
		uibox.setBox(titleStr,MyColorData.getColorString (1,str), null,null,confirm,null,
		             ReqBack);
	}

	void ReqBack (int i)
	{
		TanBaoManager.tbManager.IsClick = false;
	}
}
