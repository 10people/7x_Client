using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class KuangDongInfo : MonoBehaviour {

//	public GameObject kd_btn;//矿洞btn
//	
//	public GameObject kdbtn_bg;//矿洞不可领取时显示的btn
//	
//	public UILabel getTimesLabel;//领取次数lable
//	
//	public UILabel countTimeLabel;//剩余时间label
//	
//	private int type;//探宝类型
//	
//	private int cdTime;//cd时间
//
//	public GameObject cdTimeBg;
//	
//	//获得矿洞信息
//	public void GetKdInfo (ExploreMineInfo tempKd)
//	{
//		type = tempKd.type;
//		//		Debug.Log ("探宝type：" + tempKd.type);
//		//可否领取
//		if (tempKd.isCanGet) 
//		{
//			kdbtn_bg.SetActive (false);
//			
//			kd_btn.SetActive (true);
//			
//			string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_FREE);
//			
//			getTimesLabel.text = str;
//			
//			countTimeLabel.text = "";
//
//			cdTimeBg.SetActive (false);
//		}
//		
//		else {
//			
//			kdbtn_bg.SetActive (true);
//			
//			kd_btn.SetActive (false);
//			
//			if (tempKd.gotTimes > 0 && tempKd.gotTimes < 5) {
//				
//				if (cdTime == 0)
//				{
//					cdTime = tempKd.remainingTime;
//					
//					StartCoroutine (KuangDongCd ());
//				}
//				
//				string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_TODAY);
//				getTimesLabel.text = str + tempKd.gotTimes + "/" + tempKd.totalTimes;
//			}
//			
//			if (tempKd.gotTimes == 5) {
//				
//				string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_TODAY_FREENUM_USEEND);
//				getTimesLabel.text = str;
//				
//				countTimeLabel.text = "";
//			}
//		}
//	}
//	
//	IEnumerator KuangDongCd () {
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
//			string str = LanguageTemplate.GetText (LanguageTemplate.Text.TANBAO_WAITDES);
//			
//			countTimeLabel.text = hourStr + ":" + minuteStr + ":" + secondStr + str;
//			cdTimeBg.SetActive (true);
//			
//			if (cdTime == 0) {
//				
//				QXTanBaoData.Instance ().TBInfoReq ();
//			}
//			
//			yield return new WaitForSeconds(1);
//		}
//	}
//	
//	//矿洞领取请求
//	public void KuangDongReq ()
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
//			MemoryStream t_stream = new MemoryStream ();
//			
//			QiXiongSerializer t_serializer = new QiXiongSerializer ();
//			
//			t_serializer.Serialize (t_stream,tbBuy);
//			
//			byte[] t_protof = t_stream.ToArray ();
//			
//			SocketTool.Instance ().SendSocketMessage (ProtoIndexes.EXPLORE_REQ,ref t_protof,"30006|30005");
//		}
//	}
}
