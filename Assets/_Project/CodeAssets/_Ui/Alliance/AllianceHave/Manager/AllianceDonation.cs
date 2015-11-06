using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class AllianceDonation : MonoBehaviour,SocketProcessor {

	public AllianceHaveResp tempAllianceResp;//联盟信息

	public UILabel desLabel1;

	public UILabel desLabel2;

	public GameObject boxObj;
	public GameObject flyNumObj;

	private int donateHuFuNum;
	private int huFuNum;

	private float m_fScale = 0.1f;

	void Awake ()
	{
		SocketTool.RegisterMessageProcessor (this);
	}

	void Start ()
	{
		BoxAnimation ();
		GetHuFuNum ();
	}

	void GetHuFuNum ()
	{
		for (int i = 0;i < BagData.Instance().m_bagItemList.Count;i ++)
		{
			if (BagData.Instance().m_bagItemList[i].itemId == 910000 && BagData.Instance().m_bagItemList[i].cnt > 0)
			{
				huFuNum += BagData.Instance().m_bagItemList[i].cnt;
			}
		}
	}

	void BoxAnimation ()
	{
		ItemTemp itemTemp = ItemTemp.getItemTempById (910000);
		int effectId = itemTemp.effectId;

		desLabel1.text = "捐献1个虎符，您可以获得" + effectId + "贡献值，联盟可获得" + effectId + "建设值";

		Hashtable boxScale = new Hashtable ();
		boxScale.Add ("time",0.3f);
		boxScale.Add ("easetype",iTween.EaseType.easeOutQuart);
		boxScale.Add ("islocal",true);
		boxScale.Add ("scale",Vector3.one);
		iTween.ScaleTo (boxObj,boxScale);
	}

	void Update ()
	{
//		for (int i = 0;i < BagData.Instance().m_bagItemList.Count;i ++)
//		{
//			if (BagData.Instance().m_bagItemList[i].itemId == 910000 && BagData.Instance().m_bagItemList[i].cnt > 0)
//			{
//				huFuNum = BagData.Instance().m_bagItemList[i].cnt;
//				Debug.Log ("hufuNum:" + huFuNum);
//			}
//			
//			else
//			{
//				huFuNum = 0;
//			}
//		}

		desLabel2.text = "您现在拥有" + huFuNum + "个虎符";
	}

	//捐献1个虎符按钮
	public void Donation1Btn ()
	{
		if (huFuNum > 0)
		{
			DonationReq (1);
		}

		else
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ResourceLoadCallback2 );
		}
	}

	//捐献10个虎符按钮
	public void Donation10Btn ()
	{
		if (huFuNum >= 10)
		{
			DonationReq (10);
		}
		
		else
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
			                        ResourceLoadCallback2 );
		}
	}

	//取消按钮
	public void CancelBtn ()
	{
		Destroy (this.gameObject);
	}

	//捐献请求
	void DonationReq (int num)
	{
		DonateHuFu donateReq = new DonateHuFu ();

		donateReq.count = num;
		Debug.Log ("count:" + donateReq.count);
		donateHuFuNum = num;

		MemoryStream t_stream = new MemoryStream ();
		
		QiXiongSerializer t_serializer = new QiXiongSerializer ();
		
		t_serializer.Serialize (t_stream,donateReq);
		
		byte[] t_protof = t_stream.ToArray ();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.ALLIANCE_HUFU_DONATE,ref t_protof,"30150");
		Debug.Log ("DonationReq:" + ProtoIndexes.C_HYRESOURCE_CHANGE);
	}

	public bool OnProcessSocketMessage (QXBuffer p_message) {
		
		if (p_message != null) 
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.ALLIANCE_HUFU_DONATE_RESP:

				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				DonateHuFuResp donateResp = new DonateHuFuResp();
				
				t_qx.Deserialize(t_stream, donateResp, donateResp.GetType());

				if (donateResp != null)
				{
					Debug.Log ("捐献结果：" + donateResp.result);
					Debug.Log ("捐献贡献：" + donateResp.gongxian);
					Debug.Log ("捐献建设：" + donateResp.build);

					if (donateResp.result == 0)
					{
//						AllianceData.Instance.RequestData ();

						MyAllianceMain.m_allianceMain.g_UnionInfo.build += donateResp.build;
						MyAllianceMain.m_allianceMain.g_UnionInfo.contribution += donateResp.gongxian;
						MyAllianceMain.m_allianceMain.InItMyAlliance ();

						if (donateHuFuNum == 1)
						{
							huFuNum -= 1;
							CreateFlyNum (new Vector3(0,-75,0),1,donateResp.gongxian,donateResp.build);
						}

						else if (donateHuFuNum == 10)
						{
							huFuNum -= 10;
							CreateFlyNum (new Vector3(200,-75,0),10,donateResp.gongxian,donateResp.build);
						}
					}

					else if (donateResp.result == 1)
					{
						Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.GLOBAL_DIALOG_BOX ),
						                        ResourceLoadCallback2 );
					}
				}

				return true;

			default:return false;
			}
		}

		return false;
	}
			
	//捐献结果显示
	void CreateFlyNum (Vector3 pos,int disHuFuNum,int addGongXianNum,int addBuildNum)
	{
		GameObject flyNum = (GameObject)Instantiate (flyNumObj);
		
		flyNum.SetActive (true);
		flyNum.transform.parent = flyNumObj.transform.parent;
		flyNum.transform.localPosition = pos;
		flyNum.transform.localScale = flyNumObj.transform.localScale;
		
		DonationFlyNum donationFlyNum = flyNum.GetComponent<DonationFlyNum> ();

		if (disHuFuNum == 1)
		{
			donationFlyNum.position = new Vector3(0,-25,0);
		}
		else if (disHuFuNum == 10)
		{
			donationFlyNum.position = new Vector3(200,-25,0);
		}

		donationFlyNum.disHuFu.text = "-" + disHuFuNum + "虎符";
		donationFlyNum.addGongXian.text = "+" + addGongXianNum + "贡献值";
		donationFlyNum.addContribution.text = "+" + addBuildNum + "建设值";
	}

	public void ResourceLoadCallback2 ( ref WWW p_www, string p_path, Object p_object )
	{
		UIBox uibox = (GameObject.Instantiate(p_object) as GameObject).GetComponent<UIBox>();

		string titleStr = "捐献失败";

		string str = "您的虎符不足！";

		string confirmStr = "确定";

		uibox.setBox(titleStr,null, MyColorData.getColorString (1,str), 
		             null,confirmStr,null,null);
	}

	void OnDestroy ()
	{
		SocketTool.UnRegisterMessageProcessor (this);
	}
}
