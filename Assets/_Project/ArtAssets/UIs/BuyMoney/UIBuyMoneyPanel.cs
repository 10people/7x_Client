using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class UIBuyMoneyPanel : MYNGUIPanel , SocketProcessor
{
	public UIBuyMoneyItemData m_UIBuyMoneyItemData;
	public List<UIBuyMoneyItemData> m_listItemData;
	public BuyTongbiDataResp m_BuyTongbiData;
	public GameObject m_objShowGetMoney;

	public UILabel m_labelDes;
	public UILabel m_labelUseYuanbao;
	public UILabel m_labelGetMoney;

	public GameObject m_objShowGetLianxuMoney;
	public UILabel m_labelLianxuDes;
	public UILabel m_labelLianxuUseYuanbao;
	public UILabel m_labelLianxuGetMoney;

	public UILabel m_labelAnimationBaoji;
	public UILabel m_labelAnimationMoney;

	public List<TongbiResp> m_listAnimationData = new List<TongbiResp>();
	public int m_iAnimationNum = 0;
	public int m_iAnimationState = 0;
	public bool m_isAnimation = false;
	public int YIndao_id = 0;
	// Use this for initialization
	void Start () 
	{
		SocketTool.RegisterMessageProcessor(this);
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(m_isAnimation)
		{
			float tempScale = 0;
			switch(m_iAnimationState)
			{
			case 0:
				m_iAnimationNum ++;
				tempScale = 2f + 0.2f * m_iAnimationNum;
				m_labelAnimationBaoji.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				m_labelAnimationMoney.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				if(m_iAnimationNum == 1)
				{
					m_iAnimationNum = 0;
					m_iAnimationState = 1;
				}
				break;
			case 1:
				m_iAnimationNum ++;
				tempScale = 2f - 0.2f * m_iAnimationNum;
				m_labelAnimationBaoji.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				m_labelAnimationMoney.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				m_labelAnimationBaoji.gameObject.transform.localPosition = new Vector3(m_labelAnimationBaoji.gameObject.transform.localPosition.x, m_labelAnimationBaoji.gameObject.transform.localPosition.y - 1, m_labelAnimationBaoji.gameObject.transform.localPosition.z);
				m_labelAnimationMoney.gameObject.transform.localPosition = new Vector3(m_labelAnimationMoney.gameObject.transform.localPosition.x, m_labelAnimationMoney.gameObject.transform.localPosition.y - 1, m_labelAnimationMoney.gameObject.transform.localPosition.z);
				if(m_iAnimationNum == 6)
				{
					m_iAnimationNum = 0;
					m_iAnimationState = 2;
				}
				break;
			case 2:
				m_iAnimationNum ++;
				tempScale = m_labelAnimationBaoji.gameObject.transform.localScale.x + 0.2f * m_iAnimationNum;
				m_labelAnimationBaoji.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				m_labelAnimationMoney.gameObject.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
				if(m_iAnimationNum == 1)
				{
					m_iAnimationNum = 0;
					m_iAnimationState = 3;
				}
				break;
			case 3:
				m_iAnimationNum ++;
				if(m_iAnimationNum == 10)
				{
					m_iAnimationNum = 0;
					m_iAnimationState = 4;
				}
				break;
			case 4:
				m_iAnimationNum ++;
				m_labelAnimationBaoji.gameObject.transform.localPosition = new Vector3(m_labelAnimationBaoji.gameObject.transform.localPosition.x, m_labelAnimationBaoji.gameObject.transform.localPosition.y + 15, m_labelAnimationBaoji.gameObject.transform.localPosition.z);
				m_labelAnimationMoney.gameObject.transform.localPosition = new Vector3(m_labelAnimationMoney.gameObject.transform.localPosition.x, m_labelAnimationMoney.gameObject.transform.localPosition.y + 15, m_labelAnimationMoney.gameObject.transform.localPosition.z);
				m_labelAnimationBaoji.alpha = 1 - m_iAnimationNum * 0.2f;
				m_labelAnimationMoney.alpha = 1 - m_iAnimationNum * 0.2f;
				if(m_iAnimationNum == 5)
				{
					m_labelAnimationBaoji.gameObject.transform.localScale = new Vector3(2, 2, 2);
					m_labelAnimationMoney.gameObject.transform.localScale = new Vector3(2, 2, 2);
					m_labelAnimationBaoji.gameObject.transform.localPosition = new Vector3(0, 80, 0);
					m_labelAnimationMoney.gameObject.transform.localPosition = new Vector3(0, 14, 0);
					m_labelAnimationBaoji.alpha = 1f;
					m_labelAnimationMoney.alpha = 1f;
					m_labelAnimationBaoji.gameObject.SetActive(false);
					m_labelAnimationMoney.gameObject.SetActive(false);
					m_listAnimationData.RemoveAt(0);
					m_iAnimationNum = 0;
					m_iAnimationState = 0;
					m_isAnimation = false;
				}
				break;
			}
		}
		else
		{
			if(m_listAnimationData.Count > 0)
			{
				m_isAnimation = true;
				if(m_listAnimationData[0].baoji > 1)
				{
					m_labelAnimationBaoji.gameObject.SetActive(true);
					m_labelAnimationBaoji.text = "暴击x" + m_listAnimationData[0].baoji;
				}
				m_labelAnimationMoney.gameObject.SetActive(true);
				m_labelAnimationMoney.text = "获得铜币" + (m_listAnimationData[0].baoji * m_listAnimationData[0].shumu);
			}
		}
	}

	public bool OnProcessSocketMessage(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_BUY_TongBi_Data://购买金币一级界面信息
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				BuyTongbiDataResp BuyTongbiData = new BuyTongbiDataResp();
				
				t_qx.Deserialize(t_stream, BuyTongbiData, BuyTongbiData.GetType());
				
				setData(BuyTongbiData);
			}
				break;
			case ProtoIndexes.S_BUY_TongBi_LiXu: //请求tongbi购买信息 BuyTongbiResp
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				LianXuBuyTongbiResp BuyTongBiInfo = new LianXuBuyTongbiResp();
				
				t_qx.Deserialize(t_stream, BuyTongBiInfo, BuyTongBiInfo.GetType());

				InitBuyTongBiUI(BuyTongBiInfo);
				// SocketTool.Instance().SendSocketMessage(ProtoIndexes.C_MIBAO_INFO_REQ);
				return true;
			}
			default: return false;
			}
		}
		return false;
	}

	public void setData(BuyTongbiDataResp tongbiData)
	{
		m_BuyTongbiData = tongbiData;
		m_labelUseYuanbao.text = tongbiData.cost.ToString();
		m_labelGetMoney.text = tongbiData.getTonbi.ToString();
		string tempString = MyColorData.getColorString(1, "用少量元宝换取大量铜币\r\n(今日可用");
		tempString += MyColorData.getColorString(2, m_BuyTongbiData.nowCount + "/" + m_BuyTongbiData.maxCount);
		tempString += MyColorData.getColorString(1, ")");
		m_labelDes.text = tempString;
	}

	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("Close") != -1)
		{
			Debug.Log("YIndao_id = "+YIndao_id);
//			MainCityUI.OpenGui();
			if(YIndao_id != 0)
			{
				CityGlobalData.m_isRightGuide = false;
				UIYindao.m_UIYindao.setOpenYindao(YIndao_id);
				YIndao_id = 0;
			}
			GameObject.Destroy(gameObject);
			MainCityUI.TryRemoveFromObjectList(gameObject);
			TreasureCityUI.TryRemoveFromObjectList(gameObject);
			JunZhuData.Instance().UI_IsOpen = false;
		}
		else if(ui.name.IndexOf("ButtonLianxu1") != -1)
		{
			m_objShowGetLianxuMoney.SetActive(false);
		}
		else if(ui.name.IndexOf("ButtonLianxu2") != -1)
		{
			Global.ScendNull(ProtoIndexes.C_BUY_TongBi_LiXu);
		}
		else if(ui.name.IndexOf("Button1") != -1)
		{
			Global.ScendNull(ProtoIndexes.C_BUY_TongBi);
		}
		else if(ui.name.IndexOf("Button2") != -1)
		{
			if(m_BuyTongbiData.nowCount >= m_BuyTongbiData.maxCount)
			{
				Global.ScendNull(ProtoIndexes.C_BUY_TongBi);
			}
			else
			{
				m_objShowGetLianxuMoney.SetActive(true);
				m_labelLianxuDes.text = "连续换取" + m_BuyTongbiData.lixuCount + "次铜币";
				m_labelLianxuUseYuanbao.text = (m_BuyTongbiData.cost * m_BuyTongbiData.lixuCount).ToString();
				m_labelLianxuGetMoney.text = (m_BuyTongbiData.getTonbi * m_BuyTongbiData.lixuCount).ToString();
			}
//			m_BuyTongbiData.
//			public UILabel m_labelLianxuDes;
//			public UILabel m_labelLianxuUseYuanbao;
//			public UILabel m_labelLianxuGetMoney;
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

	void InitBuyTongBiUI(LianXuBuyTongbiResp inf)
	{
		// Debug.Log("请求购买铜币返回" + inf.baoji);
		if (inf.result == 0)
		{
			m_objShowGetLianxuMoney.SetActive(false);
			m_objShowGetMoney.SetActive(true);
			m_UIBuyMoneyItemData.gameObject.SetActive(true);
			for(int i = 0; i < inf.tongbi.Count; i ++)
			{
				UIBuyMoneyItemData temp = GameObject.Instantiate(m_UIBuyMoneyItemData.gameObject).GetComponent<UIBuyMoneyItemData>();
				temp.transform.parent = m_UIBuyMoneyItemData.transform.parent;
				temp.transform.localPosition = new Vector3(0, 0, 0);
				temp.transform.localScale = Vector3.one;
				m_listItemData.Add(temp);
				if(inf.tongbi[i].baoji <= 1)
				{
					temp.m_labelBaoji.gameObject.SetActive(false);
				}
				else
				{
					temp.m_labelBaoji.text = "暴击x" + inf.tongbi[i].baoji;
				}
				temp.m_labelUseYuanbao.text = inf.tongbi[i].cost.ToString();
				temp.m_labelGetMoney.text = inf.tongbi[i].shumu.ToString();
				m_listAnimationData.Add(inf.tongbi[i]);
			}
			m_UIBuyMoneyItemData.gameObject.SetActive(false);
			int y = -45;
			for(int i = m_listItemData.Count - 1; i >= 0; i--)
			{
				m_listItemData[i].transform.localPosition = new Vector3(0, y, 0);
				y -= 40;
			}
		}
		else if (inf.result == 1)
		{
			//购买失败
			JunZhuData.Instance().IsBuyTongBi = true;
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), JunZhuData.Instance().LoadBuyTongBiNoTimesBack);
		}
		else if (inf.result == 2)
		{
			Global.ResourcesDotLoad(Res2DTemplate.GetResPath(Res2DTemplate.Res.GLOBAL_DIALOG_BOX), JunZhuData.Instance().LoadBack_2);
		}
	}

	void OnDestroy()
	{
		SocketTool.UnRegisterMessageProcessor(this);
	}
}
