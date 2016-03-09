using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIFuli : MYNGUIPanel , SocketListener
{
	public FuliPageData m_FuliPageData;
	public List<FuliPageData> m_listFuliPageData = new List<FuliPageData>();
	private FuLiHuoDongResp m_FuLiHuoDongResp;
	// Use this for initialization
	void Awake()
	{
		SocketTool.RegisterSocketListener(this);	
	}
	
	void Start () 
	{
		Global.ScendNull(ProtoIndexes.C_FULIINFO_REQ);
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_FULIINFO_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				FuLiHuoDongResp tempInfo = new FuLiHuoDongResp();
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());
				m_FuLiHuoDongResp = tempInfo;
				int b_X = (tempInfo.xianshi.Count - 1) * (-115);
				FuliPageData tempPageData;
				bool tempFirst = false;
				if(m_listFuliPageData.Count == 0)
				{
					tempFirst = true;
				}
				for(int i = 0; i < tempInfo.xianshi.Count; i ++)
				{
					if(tempFirst)
					{
						tempPageData = GameObject.Instantiate(m_FuliPageData.gameObject).GetComponent<FuliPageData>();
						m_listFuliPageData.Add(tempPageData);
					}
					m_listFuliPageData[i].gameObject.transform.parent = m_FuliPageData.gameObject.transform.parent;
					m_listFuliPageData[i].gameObject.transform.localScale = Vector3.one;
					m_listFuliPageData[i].gameObject.transform.localPosition = new Vector3(b_X, 23, 0);
					m_listFuliPageData[i].gameObject.name = "Item" + i;
					b_X += 230;
					m_listFuliPageData[i].m_listObjRed.SetActive(tempInfo.xianshi[i].isCanGet);
					m_listFuliPageData[i].m_SpriteBG.spriteName = "fuliImage" + tempInfo.xianshi[i].typeId;
					m_listFuliPageData[i].m_labelFunctionName.text = "[b]" + FunctionOpenTemp.GetTemplateById(tempInfo.xianshi[i].typeId).Des + "[-]";
					if(tempInfo.xianshi[i].isCanGet)
					{
						m_listFuliPageData[i].m_listLingquLabel.text = MyColorData.getColorString(4, "点击领取");
						m_listFuliPageData[i].m_listObjIcon.gameObject.SetActive(true);
						m_listFuliPageData[i].m_listJiangliLabel.gameObject.SetActive(true);
						m_listFuliPageData[i].m_listJiangliLabel.text = MyColorData.getColorString(2, tempInfo.xianshi[i].content);

						m_listFuliPageData[i].m_listTimeLabel.gameObject.SetActive(false);
					}
					else
					{
						m_listFuliPageData[i].m_listLingquLabel.text = MyColorData.getColorString(2, "下次领取时间");
						m_listFuliPageData[i].m_listTimeLabel.gameObject.SetActive(true);
						m_listFuliPageData[i].m_listTimeLabel.text = MyColorData.getColorString(5, tempInfo.xianshi[i].content);

						m_listFuliPageData[i].m_listObjIcon.gameObject.SetActive(false);
						m_listFuliPageData[i].m_listJiangliLabel.gameObject.SetActive(false);
					}
					switch(tempInfo.xianshi[i].typeId)
					{
					case 1390:
						m_listFuliPageData[i].m_listObjIcon.spriteName = "YB_big";
						break;
					case 1391:
						m_listFuliPageData[i].m_listObjIcon.spriteName = "Energy";
						break;
					case 1392:
						m_listFuliPageData[i].m_listObjIcon.spriteName = "YB_big";
						break;
					}
					m_listFuliPageData[i].gameObject.SetActive(true);
				}
				break;
			}
			case ProtoIndexes.S_FULIINFOAWARD_RESP:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				QiXiongSerializer t_qx = new QiXiongSerializer();
				FuLiHuoDongAwardResp tempInfo = new FuLiHuoDongAwardResp();
				t_qx.Deserialize(t_stream, tempInfo, tempInfo.GetType());

				if(tempInfo.result != "0")
				{
					RewardData dataList;
					string tempData = tempInfo.result;
					Global.NextCutting(ref tempData, ":");
					dataList = new RewardData(int.Parse(Global.NextCutting(ref tempData, ":")), int.Parse(Global.NextCutting(ref tempData, ":")));
					GeneralRewardManager.Instance().CreateReward (dataList);
				}

//				for (int m = 0;m < tempEmail.goodsList.Count;m ++)
//				{
//					RewardData data = new RewardData(tempEmail.goodsList[m].id,tempEmail.goodsList[m].count);
//					dataList.Add (data);
//				}
				break;
			}
			default: return false;
			}
			
		}else
		{
			Debug.Log ("p_message == null");
		}
		
		return false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public override void MYClick(GameObject ui)
	{
		if(ui.name.IndexOf("Close") != -1)
		{
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);
		}
		else if(ui.name.IndexOf("Item0") != -1)
		{
			Global.ScendID(ProtoIndexes.C_FULIINFOAWARD_REQ, m_FuLiHuoDongResp.xianshi[0].typeId);
		}
		else if(ui.name.IndexOf("Item1") != -1)
		{
			Global.ScendID(ProtoIndexes.C_FULIINFOAWARD_REQ, m_FuLiHuoDongResp.xianshi[1].typeId);
		}
		else if(ui.name.IndexOf("Item2") != -1)
		{
			Global.ScendID(ProtoIndexes.C_FULIINFOAWARD_REQ, m_FuLiHuoDongResp.xianshi[2].typeId);
		}
	}
	
	public override void MYMouseOver(GameObject ui)
	{
	}
	
	public override void MYMouseOut(GameObject ui)
	{
	}
	
	public override void MYPress(bool isPress, GameObject ui)
	{
	}
	
	public override void MYelease(GameObject ui)
	{
	}
	
	public override void MYondrag(Vector2 delta)
	{
		
	}
	
	public override void MYoubleClick(GameObject ui)
	{
	}
	
	public override void MYonInput(GameObject ui, string c)
	{
	}
}
