using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;


public class ConfromBuyTili : MonoBehaviour,SocketProcessor {
	public UILabel shouTiLiHuafei;
	public UILabel ShowTimes;
	//public UILabel ShowgetTili;
	public GameObject sup;
	BuyTimesInfo buyInfo;
//	int Yuanbao;
	void Awake()
	{ 
		SocketTool.RegisterMessageProcessor(this);

	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
	void Start()
	{
//		Yuanbao = JunZhuData.Instance ().m_junzhuInfo.yuanBao;
		JunZhuData.Instance().IsBuyTiLi = true;
		SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TIMES_REQ);
	}
	public bool OnProcessSocketMessage(QXBuffer p_message){
		
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
//			case ProtoIndexes.S_BUY_TIMES_INFO:
//			{
//				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
//				
//				QiXiongSerializer t_qx = new QiXiongSerializer();
//				
//				BuyTimesInfo BuyTimespInfo = new BuyTimesInfo();
//				
//				t_qx.Deserialize(t_stream, BuyTimespInfo, BuyTimespInfo.GetType());
//				buyInfo = BuyTimespInfo;
//				Debug.Log ("huafei......."+BuyTimespInfo.tiLiHuaFei);
//				//ShowgetTili.text = BuyTimespInfo.tiLiHuoDe.ToString();//显示数据到UI
//				ShowTimes.text =  BuyTimespInfo.tiLi.ToString();//购买体力时的花费 元宝数
//				//shouTiLiHuafei.text = "您是否要用"+BuyTimespInfo.tiLiHuaFei.ToString()+"元宝来购买"+ BuyTimespInfo.tiLiHuoDe.ToString()+"体力";
//				return true;
//			}
			default: return false;
			}
		}
		else{
			//Debug.Log("返回数据为空");
		}
		return false;
	}

	void OnClick()
	{
		MapData.mapinstance.IsCloseGuid = false;
		ConformBuyTiliMessage ();
		Destroy (sup);
	}
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		GameObject tempOjbect = Instantiate (p_object)as GameObject;
		GameObject obj = GameObject.Find ("Map(Clone)");
		tempOjbect.transform.parent = obj.transform;
		tempOjbect.transform.localScale = new Vector3 (1,1,1);
		tempOjbect.transform.localPosition = new Vector3 (1,1,1);
	}
	void ConformBuyTiliMessage()//确认购买体力
	{
		//Debug.Log ("huafei......."+buyInfo.tiLiHuaFei);
		//Debug.Log ("Yuanbao......."+JunZhuData.Instance ().m_junzhuInfo.yuanBao);
		if(buyInfo.tiLiHuaFei <= JunZhuData.Instance ().m_junzhuInfo.yuanBao)
		{
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_BUY_TiLi);
		}
		else
		{
			//显示元宝不足，弹出购买元宝菜单
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.PVE_NOT_ENOUGH_YUAN_BAO ),
			                        LoadResourceCallback );

		}
		//PD.C_BUY_TiLi
	}
}
