using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class QXTanBaoData : MonoBehaviour,SocketProcessor {

	private static QXTanBaoData tbData;

	public static QXTanBaoData Instance()
	{
		if ( tbData == null )
		{
			GameObject t_GameObject = UtilityTool.GetDontDestroyOnLoadGameObject();
			
			tbData = t_GameObject.AddComponent< QXTanBaoData >();
		}
		
		return tbData;
	}

	public ExploreInfoResp tanBaoResp;

	public int tb1CdTime;
	public int tb2CdTime;

	void Awake () 
	{	
		SocketTool.RegisterMessageProcessor (this);
	}
	
	//请求探宝信息
	public void TBInfoReq () 
	{	
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.EXPLORE_INFO_REQ,"30003");

//		Debug.Log ("tbReq:" + ProtoIndexes.EXPLORE_INFO_REQ);
	}
	
	//返回探宝信息
	public bool OnProcessSocketMessage (QXBuffer p_message) 
	{	
		if (p_message != null) 
		{	
			if (p_message.m_protocol_index == ProtoIndexes.EXPLORE_INFO_RESP) 
			{
//				Debug.Log ("tbRes:" + ProtoIndexes.EXPLORE_INFO_RESP);

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				ExploreInfoResp tanBaoInfo = new ExploreInfoResp();
				
				t_qx.Deserialize(t_stream, tanBaoInfo, tanBaoInfo.GetType());
				
				if (tanBaoInfo != null ) 
				{	
//					Debug.Log ("有无联盟:" + tanBaoInfo.hasGuild);
//					Debug.Log ("元宝数:" + tanBaoInfo.yuanBao);
//					Debug.Log ("贡献值:" + tanBaoInfo.gongXian);
//					Debug.Log ("铁:" + tanBaoInfo.tie);
//					Debug.Log ("铜:" + tanBaoInfo.tong);

					tanBaoResp = tanBaoInfo;

//					foreach (ExploreMineInfo emInfo in tanBaoInfo.mineRegionList) 
//					{
//						Debug.Log ("探宝类型:" + emInfo.type);
//						Debug.Log ("可否探宝:" + emInfo.isCanGet);
//						Debug.Log ("探宝剩余次数:" + emInfo.gotTimes);
//						Debug.Log ("探宝总次数:" + emInfo.totalTimes);
//						Debug.Log ("cd时间:" + emInfo.remainingTime);
//						Debug.Log ("花费元宝:" + emInfo.cost);
//						Debug.Log ("打折数:" + emInfo.discount);
//					}

					CheckFreeTanBao ();

					TanBaoCd ();

					GameObject tbObj = GameObject.Find ("QXTanBao");

					if (tbObj != null)
					{
						TanBaoManager tanBaoManager = tbObj.GetComponent<TanBaoManager> ();
						tanBaoManager.GetTanBaoInfo (tanBaoInfo);
					}
				}
				
				return true;
			}
		}
		return false;
	}

	public void TanBaoCd ()
	{
		for (int i = 0;i < tanBaoResp.mineRegionList.Count;i ++)
		{
			if (tanBaoResp.mineRegionList[i].type == 0)
			{
				if (tb1CdTime == 0)
				{
					tb1CdTime = tanBaoResp.mineRegionList[i].remainingTime;

					StartCoroutine (TanBaoCd (tanBaoResp.mineRegionList[i].type,tb1CdTime));
				}
			}

			else if (tanBaoResp.mineRegionList[i].type == 1)
			{
				if (tb2CdTime == 0)
				{
					tb2CdTime = tanBaoResp.mineRegionList[i].remainingTime;
					
					StartCoroutine (TanBaoCd (tanBaoResp.mineRegionList[i].type,tb2CdTime));
				}
			}
		}
	}

	IEnumerator TanBaoCd (int type,int cdTime)
	{
		while (cdTime > 0) 
		{	
			cdTime --;

			switch (type)
			{
			case 0:

				tb1CdTime = cdTime;

				break;

			case 1:

				tb2CdTime = cdTime;

				break;

			default:break;
			}

			if (cdTime == 0) 
			{	
				TBInfoReq ();
			}
			
			yield return new WaitForSeconds(1);
		}
	}

	public void CheckFreeTanBao ()
	{
		FunctionOpenTemp functionTemp = FunctionOpenTemp.GetTemplateById (11);
		int index = functionTemp.m_iID;

		foreach (ExploreMineInfo tempInfo in tanBaoResp.mineRegionList)
		{
			if (tempInfo.isCanGet)
			{
				if (tempInfo.type == 0)
				{
					MainCityUIRB.SetRedAlert (index,true);
				}
				else if (tempInfo.type == 1)
				{
					if (FunctionOpenTemp.GetWhetherContainID (1102))
					{
						MainCityUIRB.SetRedAlert (index,true);
					}
				}

				break;
			}
			else
			{
				MainCityUIRB.SetRedAlert (index,false);
			}
		}
	}

	void OnDestroy () 
	{	
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
