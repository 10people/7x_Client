using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KuangJingInfo : MonoBehaviour {

//	public UILabel freeLabel; //显示免费的Label
//	
//	public GameObject ybLabelObj; //显示元宝数及元宝图标的obj
//	
//	public UILabel ybLabel;//显示元宝数的label
//	
//	public UILabel countTimeLabel; //CD时间文本框
//	
//	private bool kjGet;//矿井可否免费领取
//	
//	private int type;//探宝类型
//	
//	private int cdTime;//cd时间
//	
//	public GameObject btnBg;//置灰按钮
//	
//	public GameObject btn;//探宝按钮
//
//	public GameObject cdTimeBg;
//
//	public GameObject tipObj;
//	
//	//获得矿井信息
//	public void GetKjInfo (ExploreMineInfo tempKj)
//	{
//		kjGet = tempKj.isCanGet;
//		
//		type = tempKj.type;
//
////		if (JunZhuData.Instance ().m_junzhuInfo.level > 0)
//		if (FunctionOpenTemp.GetWhetherContainID (1102))
//		{
//			btnBg.SetActive (false);
//			btn.SetActive (true);
//
//			tipObj.SetActive (tempKj.isCanGet);
//
//			if (tempKj.isCanGet) 
//			{
//				string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_FREE);
//				
//				freeLabel.text = str;
//				
//				ybLabelObj.SetActive (false);
//				
//				countTimeLabel.text = "";
//				cdTimeBg.SetActive (false);
//			}
//			
//			else {
//				
//				freeLabel.text = "";
//				
//				ybLabel.text = tempKj.cost.ToString ();
//				
//				ybLabelObj.SetActive (true);
//				
//				if (cdTime == 0)
//				{
//					cdTime = tempKj.remainingTime;
//					
//					StartCoroutine (KuangJingCd ());
//				}
//			}
//		}
//		
//		else
//		{
//			btnBg.SetActive (true);
//			btn.SetActive (false);
//			string str = ZhuXianTemp.GeTaskTitleById(FunctionOpenTemp.GetMissionIdById(1102));
////			string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_OPEL_NEED_LEVEL);
//			freeLabel.text = str;
//			ybLabelObj.SetActive (false);
//		}
//	}
//	
//	IEnumerator KuangJingCd () {
//
//		string hourStr = "";
//		string minuteStr = "";
//		string secondStr = "";
//
//		while (cdTime > 0) {
//			
//			cdTime --;
//			
//			int hour = cdTime/3600;
//			if (hour < 10)
//			{
//				hourStr = "0" + hour;
//			}
//			else
//			{
//				hourStr = hour.ToString ();
//			}
//
//			int minute = (cdTime/60)%60;
//			if (minute < 10)
//			{
//				minuteStr = "0" + minute;
//			}
//			else
//			{
//				minuteStr = minute.ToString ();
//			}
//
//			int second = cdTime%60;
//			if (second < 10)
//			{
//				secondStr = "0" + second;
//			}
//			else
//			{
//				secondStr = second.ToString ();
//			}
//
//			//			string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_WAITDES);
//			string str = "后免费";
//			countTimeLabel.text = hourStr + ":" + minuteStr + ":" + secondStr + str;
//			cdTimeBg.SetActive (true);
//			if (cdTime == 0) {
//				
//				QXTanBaoData.Instance ().TBInfoReq ();
//			}
//			
//			yield return new WaitForSeconds(1);
//		}
//	}
//	
//	//矿井领取请求
//	public void KuangJingReq ()
//	{
//		if (!TanBaoManager.tbManager.IsClick)
//		{
//			TanBaoManager.tbManager.IsClick = true;
//			TanBaoManager.tbManager.SetActiveZheZhao (true);
//			
//			ExploreReq tbBuy = new ExploreReq();
//			
//			tbBuy.type = type;
//			
//			if (kjGet)
//			{
//				tbBuy.isBuy = false;
//			}
//			
//			else
//			{
//				tbBuy.isBuy = true;
//			}
//			
//			MemoryStream t_stream = new MemoryStream ();
//			
//			QiXiongSerializer t_serializer = new QiXiongSerializer ();
//			
//			t_serializer.Serialize (t_stream,tbBuy);
//			
//			byte[] t_protof = t_stream.ToArray ();
//			
//			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.EXPLORE_REQ,ref t_protof,"30006|30005");
//			Debug.Log ("tbBuyReq:" + ProtoIndexes.EXPLORE_REQ);
//		}
//	}
}
