using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using qxmobile.protobuf;

public class MainCityYunYing : MYNGUIPanel , SocketListener
{
	public ScaleEffectController m_ScaleEffectController;
	public List<UISprite> m_pageButton = new List<UISprite>();//左边按钮
	public List<UILabel> m_pageButtonLabel = new List<UILabel>();//左边按钮文字
	private bool[] m_isOpenFunction = new bool[]{false, false, false};
	private int m_iPageIndex = -1;
	private int m_iWantPageIndex = 0;
	
	public UILabel m_page0Dis0;
	public UILabel m_page0Dis1;
	public UILabel m_page1Dis0;
	public UILabel m_page2Dis0;
	public UILabel m_page3Dis0;
	public UILabel m_curLv;

	private ErrorMessage data0;
	private ExploreResp data1;
	private ErrorMessage data2;
	
	public List<GameObject> m_PageObj = new List<GameObject>();
	private short[] m_ScendID = new short[]{ProtoIndexes.GET_QQ_INFO, ProtoIndexes.GET_LV_INFO ,ProtoIndexes.GET_REQ_INFO};
	public MainCityYunYingData m_MainCityYunYingData;
	public List<MainCityYunYingData> m_listMainCityYunYingData = new List<MainCityYunYingData>();

	public GameObject m_Red;
//	public GameObject m_MonetParentObj;
//	public GameObject m_objTitle;

	public GameObject m_CopyPanel;
	
	void Awake()
	{
		SocketTool.RegisterSocketListener(this);
	}
	
	void OnDestroy()
	{
		SocketTool.UnRegisterSocketListener(this);
	}
	
	void Start()
	{
//		MainCityUI.setGlobalTitle(m_objTitle, "开服活动", 0, 0);
//		MainCityUI.setGlobalBelongings(m_MonetParentObj, 480 + ClientMain.m_iMoveX - 30, 320 + ClientMain.m_iMoveY);

		//		OnSocketEvent(null);
//		if(Global.m_sBianqiangClick != null && Global.m_sBianqiangClick != "")
//		{
//			m_iWantPageIndex = int.Parse(Global.m_sBianqiangClick.Substring(4,1));
//			Global.m_sBianqiangClick = "";
//			//			Global.m_sBianqiangClick = "Page" + m_iPageIndex;
//		}
		Global.ScendNull(ProtoIndexes.GET_QQ_INFO);
	}
	
	void DoCloseWindow()
	{
		Destroy(gameObject);
	}

	void Update()
	{
		m_Red.SetActive(FunctionOpenTemp.GetTemplateById(1430).m_show_red_alert);
	}
	
	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.GET_QQ_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data0 = new ErrorMessage();
				
				t_qx.Deserialize(t_stream, data0, data0.GetType());
				changePage();
				break;
			}
			case ProtoIndexes.GET_LV_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data1 = new ExploreResp();
				
				t_qx.Deserialize(t_stream, data1, data1.GetType());
				changePage();
				break;
			}
			case ProtoIndexes.GET_REQ_INFO:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data2 = new ErrorMessage();
				
				t_qx.Deserialize(t_stream, data2, data2.GetType());
				changePage();
				break;
			}
			default: return false;
			}
		}
		else
		{
			
		}
		
		return false;
	}
	
	public void changePage()
	{
		if(m_iPageIndex != m_iWantPageIndex)
		{
			if(m_iPageIndex != -1)
			{
				m_PageObj[m_iPageIndex].SetActive(false);
			}
			m_iPageIndex = m_iWantPageIndex;
			m_PageObj[m_iPageIndex].SetActive(true);
		}
		for(int i = 0; i < m_pageButton.Count; i ++)
		{
			if(i == m_iPageIndex)
			{
				m_pageButton[i].color = Color.white;
				m_pageButtonLabel[i].GetComponent<UILabelType>().setType(2);
			}
			else
			{
				m_pageButton[i].color = Color.grey;
				m_pageButtonLabel[i].GetComponent<UILabelType>().setType(101);
			}
		}
		switch(m_iPageIndex)
		{
		case 0:
			Debug.Log(data0.errorDesc);
			string[] tempData = data0.errorDesc.Split(new string[]{"#=#=#"}, System.StringSplitOptions.None);
			m_page0Dis0.text = tempData[0];
			m_page0Dis1.text = "官方QQ群：[ff0000]" + tempData[1] + "[-]";
			m_page3Dis0.text = "官方QQ群：" + tempData[1];
			DeviceHelper.SetClipBoard(tempData[1]);
			break;
		case 1:
			int bx = -83;
			MainCityUI.SetRedAlert(1430 , false);
			m_MainCityYunYingData.gameObject.SetActive(false);
			for(int i = 0; i < data1.awardsList.Count; i ++)
			{
				if(m_listMainCityYunYingData.Count <= i)
				{
					GameObject temp = GameObject.Instantiate(m_MainCityYunYingData.gameObject) as GameObject;
					temp.SetActive(true);
					temp.transform.parent = m_MainCityYunYingData.gameObject.transform.parent;
					temp.transform.localScale = Vector3.one;
					temp.transform.localPosition = new Vector3(bx + i * 173, 68, 0);
					MainCityYunYingData tempYunYingData = temp.GetComponent<MainCityYunYingData>();
					m_listMainCityYunYingData.Add(tempYunYingData);

					m_listMainCityYunYingData[i].m_IconSampleManager.SetIconByID(data1.awardsList[i].itemId, "x" + data1.awardsList[i].itemNumber);
//					m_listMainCityYunYingData[i].m_IconSampleManager.seta(data1.awardsList[i].itemNumber);
					m_listMainCityYunYingData[i].m_IconSampleManager.SetIconPopText(data1.awardsList[i].itemId);

				}
				if(data1.awardsList[i].pieceNumber == 2)
				{
					m_listMainCityYunYingData[i].m_objButton.SetActive(true);
					MainCityUI.SetRedAlert(1430 , true);
				}
				else
				{
					m_listMainCityYunYingData[i].m_objButton.SetActive(false);
				}
				if(data1.awardsList[i].pieceNumber == 1)
				{
					m_listMainCityYunYingData[i].m_objGray.SetActive(true);
				}
				else
				{
					m_listMainCityYunYingData[i].m_objGray.SetActive(false);
				}
				if(data1.awardsList[i].pieceNumber == 3)
				{
					m_listMainCityYunYingData[i].m_labelLingqu.gameObject.SetActive(true);
				}
				else
				{
					m_listMainCityYunYingData[i].m_labelLingqu.gameObject.SetActive(false);
				}
				m_listMainCityYunYingData[i].m_objButton.name = "lingjiangButton" + i;
				m_listMainCityYunYingData[i].m_labelNeedLV.text = "等级" + data1.awardsList[i].miBaoStar + "级";
				m_curLv.text = "当前等级" + JunZhuData.Instance().m_junzhuInfo.level + "级";

			}
			break;
		case 2:
			m_page2Dis0.text = data2.errorDesc;
			break;
		}
	}
	
	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("Close") != -1)
		{
			MainCityUI.TryRemoveFromObjectList(gameObject);
			DoCloseWindow();
		}
		else if(ui.name.IndexOf("quedingCopy") != -1)
		{
			m_CopyPanel.SetActive(false);
		}
		else if(ui.name.IndexOf("Copy") != -1)
		{
			m_CopyPanel.SetActive(true);
		}
		else if(ui.name.IndexOf("Page") != -1)
		{
			int tempIndex = int.Parse(ui.name.Substring(4, 1));
			if(m_iWantPageIndex != tempIndex && m_iWantPageIndex == m_iPageIndex)
			{
				m_iWantPageIndex = tempIndex;
			}
			switch(m_iWantPageIndex)
			{
			case 0:
				if(data0 != null)
				{
					changePage();
					return;
				}
				break;
			case 1:
				if(data1 != null)
				{
					changePage();
					return;
				}
				break;
			case 2:
				if(data2 != null)
				{
					changePage();
					return;
				}
				break;
			}
			if(m_iWantPageIndex != m_iPageIndex)
			{
				Global.ScendNull(m_ScendID[m_iWantPageIndex]);
			}
			else
			{
				m_iWantPageIndex = m_iPageIndex;
			}
		}
		else if(ui.name.IndexOf("lingjiangButton") != -1)
		{
			int tempIndex = int.Parse(ui.name.Substring(15, ui.name.Length - 15));

			ErrorMessage req = new ErrorMessage();
			
			req.errorCode = data1.awardsList[tempIndex].miBaoStar;
			
			MemoryStream tempStream = new MemoryStream();
			
			QiXiongSerializer t_qx = new QiXiongSerializer();
			
			t_qx.Serialize(tempStream, req);
			
			byte[] t_protof;
			
			t_protof = tempStream.ToArray();
			
			SocketTool.Instance().SendSocketMessage(ProtoIndexes.GET_LV_REWARD, ref t_protof);

			Global.ScendNull(ProtoIndexes.GET_LV_INFO);
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
