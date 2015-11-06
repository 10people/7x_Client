using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KuangMaiInfo : MonoBehaviour {

	public UILabel costYbLabel;//显示需要元宝数label
	
	public UISprite daZheSprite;//打折sprite
	
	private int type;//探宝类型
	
	public GameObject btnBg;//置灰按钮
	
	public GameObject btn;//探宝按钮
	
	public GameObject ybObj;//显示元宝和打折数的obj
	
	public UILabel levelLabel;//显示开启等级的label
	
	//获得矿脉信息
	public void GetKmInfo (ExploreMineInfo tempKm)
	{
		type = tempKm.type;

//		if (JunZhuData.Instance ().m_junzhuInfo.level > 0)
		if (FunctionOpenTemp.GetWhetherContainID (1102))
		{
			ybObj.SetActive (true);
			levelLabel.gameObject.SetActive (false);
			btn.SetActive (true);
			btnBg.SetActive (false);
			
			costYbLabel.text = tempKm.cost.ToString ();
			
			switch (tempKm.discount)
			{
			case 9:
				
				daZheSprite.spriteName = "9zhe";
				
				break;
			}
		}
		
		else
		{
			ybObj.SetActive (false);
			levelLabel.gameObject.SetActive (true);
			btn.SetActive (false);
			btnBg.SetActive (true);
			string str = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1102));
//			string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_OPEL_NEED_LEVEL);
			levelLabel.text = str;
		}
	}
	
	//矿脉领取请求
	public void KuangMaiReq ()
	{
		if (!TanBaoManager.tbManager.IsClick)
		{
			TanBaoManager.tbManager.IsClick = true;
			TanBaoManager.tbManager.SetActiveZheZhao (true);
			
			ExploreReq tbBuy = new ExploreReq();
			
			tbBuy.type = type;
			
			tbBuy.isBuy = true;
			
			MemoryStream t_stream = new MemoryStream ();
			
			QiXiongSerializer t_serializer = new QiXiongSerializer ();
			
			t_serializer.Serialize (t_stream,tbBuy);
			
			byte[] t_protof = t_stream.ToArray ();
			
			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.EXPLORE_REQ,ref t_protof,"30006|30005");
			//		Debug.Log ("tbBuyReq:" + ProtoIndexes.EXPLORE_REQ);
		}
	}
}
