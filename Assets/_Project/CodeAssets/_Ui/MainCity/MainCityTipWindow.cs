using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using qxmobile.protobuf;

public class MainCityTipWindow : MYNGUIPanel , SocketListener
{
    public ScaleEffectController m_ScaleEffectController;
	public List<UISprite> m_pageButton = new List<UISprite>();//左边按钮
	public List<UILabel> m_pageButtonLabel = new List<UILabel>();//左边按钮文字
	private int[] m_openFunctionID = new int[]{-1,12,6,500010,-1};
	private bool[] m_isOpenFunction = new bool[]{false, false, false, false, false};
	private int m_iPageIndex = -1;
	private int m_iWantPageIndex = 0;
	public List<List<UISprite>> m_ListIcon = new List<List<UISprite>>();//每页的图标
	public List<UISprite> m_ListIcon0 = new List<UISprite>();
	public List<UISprite> m_ListIcon1 = new List<UISprite>();
	public List<UISprite> m_ListIcon2 = new List<UISprite>();
	public List<UISprite> m_ListIcon3 = new List<UISprite>();
	public List<UISprite> m_ListRect = new List<UISprite>();

	public List<List<UILabel>> m_ListLabel = new List<List<UILabel>>();//进度label
	public List<UILabel> m_ListLabel0 = new List<UILabel>();
	public List<UILabel> m_ListLabel1 = new List<UILabel>();
	public List<UILabel> m_ListLabel2 = new List<UILabel>();
	public List<UILabel> m_ListLabel3 = new List<UILabel>();

	private UpAction_S_getData0 data0;
	private UpAction_S_getData1 data1;
	private UpAction_S_getData2 data2;
	private UpAction_S_getData3 data3;

	public List<List<GameObject>> m_ListPagesObj = new List<List<GameObject>>();//进度label
	public List<GameObject> m_ListPages0 = new List<GameObject>();
	public List<GameObject> m_ListPages1 = new List<GameObject>();
	public List<GameObject> m_ListPages2 = new List<GameObject>();
	public List<GameObject> m_ListPages3 = new List<GameObject>();
	public List<GameObject> m_ListPages4 = new List<GameObject>();

	public List<List<UILabel>> m_ListDis = new List<List<UILabel>>();//进度label
	public List<UILabel> m_ListDis0 = new List<UILabel>();
	public List<UILabel> m_ListDis1 = new List<UILabel>();
	public List<UILabel> m_ListDis2 = new List<UILabel>();
	public List<UILabel> m_ListDis3 = new List<UILabel>();

	public List<GameObject> m_PageObj = new List<GameObject>();

	public List<GameObject> m_CloseObj = new List<GameObject>();

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
		for(int i = 0; i < m_openFunctionID.Length; i ++)
		{
			if(m_openFunctionID[i] != -1 && !FunctionOpenTemp.IsHaveID(m_openFunctionID[i]))
			{
				m_isOpenFunction[i] = false;
				m_pageButton[i].color = Color.black;
				m_pageButtonLabel[i].GetComponent<UILabelType>().setType(100);
			}
			else
			{
				m_isOpenFunction[i] = true;
//				UIButton temp = m_pageButton[i].gameObject.AddComponent<UIButton>();
//				temp.tweenTarget = m_pageButton[i].gameObject;
				m_pageButton[i].color = Color.white;
			}
		}
		m_ListIcon.Add(m_ListIcon0);
		m_ListIcon.Add(m_ListIcon1);
		m_ListIcon.Add(m_ListIcon2);
		m_ListIcon.Add(m_ListIcon3);
		m_ListLabel.Add(m_ListLabel0);
		m_ListLabel.Add(m_ListLabel1);
		m_ListLabel.Add(m_ListLabel2);
		m_ListLabel.Add(m_ListLabel3);
		m_ListPagesObj.Add(m_ListPages0);
		m_ListPagesObj.Add(m_ListPages1);
		m_ListPagesObj.Add(m_ListPages2);
		m_ListPagesObj.Add(m_ListPages3);
		m_ListPagesObj.Add(m_ListPages4);
		m_ListDis.Add(m_ListDis0);
		m_ListDis.Add(m_ListDis1);
		m_ListDis.Add(m_ListDis2);
		m_ListDis.Add(m_ListDis3);
//		OnSocketEvent(null);
		if(Global.m_sBianqiangClick != null && Global.m_sBianqiangClick != "")
		{
			m_iWantPageIndex = int.Parse(Global.m_sBianqiangClick.Substring(4,1));
			Global.m_sBianqiangClick = "";
//			Global.m_sBianqiangClick = "Page" + m_iPageIndex;
		}
		Global.ScendID(ProtoIndexes.C_GET_UPACTION_DATA, m_iWantPageIndex);
    }

	void DoCloseWindow()
	{
		Destroy(gameObject);
	}

	public bool OnSocketEvent(QXBuffer p_message)
	{
		if (p_message != null)
		{
			switch (p_message.m_protocol_index)
			{
			case ProtoIndexes.S_UPACTION_DATA_0:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data0 = new UpAction_S_getData0();
				
				t_qx.Deserialize(t_stream, data0, data0.GetType());
				changePage();
				break;
			}
			case ProtoIndexes.S_UPACTION_DATA_1:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data1 = new UpAction_S_getData1();
				
				t_qx.Deserialize(t_stream, data1, data1.GetType());
				changePage();
				break;
			}
			case ProtoIndexes.S_UPACTION_DATA_2:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data2 = new UpAction_S_getData2();
				
				t_qx.Deserialize(t_stream, data2, data2.GetType());
				changePage();
				break;
			}
			case ProtoIndexes.S_UPACTION_DATA_3:
			{
				MemoryStream t_stream = new MemoryStream(p_message.m_protocol_message, 0, p_message.position);
				
				QiXiongSerializer t_qx = new QiXiongSerializer();
				
				data3 = new UpAction_S_getData3();
				
				t_qx.Deserialize(t_stream, data3, data3.GetType());
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
		if(m_iPageIndex != -1)
		{
			m_PageObj[m_iPageIndex].SetActive(false);
		}
		m_iPageIndex = m_iWantPageIndex;
		m_PageObj[m_iPageIndex].SetActive(true);
		for(int i = 0; i < m_pageButton.Count; i ++)
		{
			if(m_isOpenFunction[i])
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
		}
		switch(m_iPageIndex)
		{
		case 0:
			if(data0.tianfuId == null || data0.tianfuId.Count == 0)
			{
				m_ListDis[m_iPageIndex][0].gameObject.SetActive(false);
			}
			for(int i = 0; i < 3; i ++)
			{
				if(i < data0.tianfuId.Count)
				{
					m_ListIcon[m_iPageIndex][i].gameObject.SetActive(true);
					m_ListIcon[m_iPageIndex][i].spriteName = data0.tianfuId[i]+"";
				}
				else
				{
					m_ListIcon[m_iPageIndex][i].gameObject.SetActive(false);
				}
			}
			m_ListLabel[m_iPageIndex][0].text = data0.curHeroLevel + "级/" + data0.maxHeroLevel + "级";
			m_ListLabel[m_iPageIndex][1].text = data0.curTanfuLevel + "/" + data0.maxTanfuLevel;
			if(!FunctionOpenTemp.IsHaveID(500000))
			{
				m_ListPagesObj[m_iPageIndex][1].gameObject.SetActive(false);
				m_CloseObj[0].SetActive(true);
			}
			break;
		case 1:
			for(int i = 0; i < 3; i ++)
			{
				if(i == 1)
				{
					m_ListLabel[m_iPageIndex][i].text = data1.pageData[i].curLevel + "/" + data1.pageData[i].maxLevel;
				}
				else
				{
					m_ListLabel[m_iPageIndex][i].text = ((int)((float)((float)data1.pageData[i].curLevel / (float)data1.pageData[i].maxLevel) * 100)) + "%";
				}
				if(data1.pageData[i].zhuangbeiData == null || data1.pageData[i].zhuangbeiData.Count == 0)
				{
					m_ListDis[m_iPageIndex][i].gameObject.SetActive(false);
				}
				for(int q = 0; q < 3; q ++)
				{
					if(data1.pageData[i].zhuangbeiData != null && q < data1.pageData[i].zhuangbeiData.Count)
					{
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(true);
						m_ListIcon[m_iPageIndex][i*3+q].spriteName = data1.pageData[i].zhuangbeiData[q].id + "";
					}
					else
					{
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(false);
					}
				}
			}
			if(!FunctionOpenTemp.IsHaveID(1211))
			{
				m_ListPagesObj[m_iPageIndex][1].gameObject.SetActive(false);
				m_CloseObj[1].SetActive(true);
			}
			if(!FunctionOpenTemp.IsHaveID(1210))
			{
				m_ListPagesObj[m_iPageIndex][2].gameObject.SetActive(false);
				m_CloseObj[2].SetActive(true);
			}
			break;
		case 2:
			m_ListLabel[m_iPageIndex][0].text = ((int)((float)((float)data2.pageData[0].curLevel / (float)data2.pageData[0].maxLevel) * 100)) + "%";
			m_ListLabel[m_iPageIndex][1].text = data2.pageData[1].curLevel + "/" + data2.pageData[1].maxLevel + "";
//				
			for(int i = 0; i < 2; i ++)
			{
				if(data2.pageData[i].mibaoDataId == null || data2.pageData[i].mibaoDataId.Count == 0)
				{
					m_ListDis[m_iPageIndex][i].gameObject.SetActive(false);
				}
				for(int q = 0; q < 3; q ++)
				{
					if(data2.pageData[i].mibaoDataId != null && q < data2.pageData[i].mibaoDataId.Count)
					{
						m_ListRect[i*3+q].spriteName = MiBaoGlobleData.Instance().getStart(data2.pageData[i].mibaoDataId[q]);
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(true);
						m_ListIcon[m_iPageIndex][i*3+q].spriteName = data2.pageData[i].mibaoDataId[q] + "";
					}
					else
					{
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(false);
					}
				}
			}
			break;
		case 3:
			m_ListLabel[m_iPageIndex][0].text = ((int)((float)((float)data3.pageData[0].curLevel / (float)data3.pageData[0].maxLevel) * 100)) + "%";
			m_ListLabel[m_iPageIndex][1].text = ((int)((float)((float)data3.pageData[1].curLevel / (float)data3.pageData[1].maxLevel) * 100)) + "%";
//			m_ListLabel[m_iPageIndex][0].text = data3.pageData[0].curLevel + "/" + data3.pageData[0].maxLevel;
//			m_ListLabel[m_iPageIndex][1].text = data3.pageData[1].curLevel + "/" + data3.pageData[1].maxLevel;
			//				
			for(int i = 0; i < 2; i ++)
			{
				if(data3.pageData[i].fuwenDataId == null || data3.pageData[i].fuwenDataId.Count == 0)
				{
					m_ListDis[m_iPageIndex][i].gameObject.SetActive(false);
				}
				for(int q = 0; q < 3; q ++)
				{

					if(data3.pageData[i].fuwenDataId != null && q < data3.pageData[i].fuwenDataId.Count)
					{
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(true);
						m_ListIcon[m_iPageIndex][i*3+q].spriteName = data3.pageData[i].fuwenDataId[q] + "";
					}
					else
					{
						m_ListIcon[m_iPageIndex][i*3+q].gameObject.SetActive(false);
					}
				}
			}
			break;
		}
	}

	public override void MYClick(GameObject ui)
	{
		Debug.Log(ui.name);
		if(ui.name.IndexOf("Close") != -1)
		{
			MainCityUI.TryRemoveFromObjectList(gameObject);
			m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
			m_ScaleEffectController.OnCloseWindowClick();
		}
		else if(ui.name.IndexOf("tili") != -1)
		{
			JunZhuData.Instance ().BuyTiliAndTongBi (true,false,false);
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
			case 3:
				if(data3 != null)
				{
					changePage();
					return;
				}
				break;
			case 4:
				changePage();
				break;
			}
			if(m_iWantPageIndex == 4)
			{
//				m_iPageIndex = m_iWantPageIndex;
//				changePage();
			}
			else if(m_iWantPageIndex != m_iPageIndex && m_isOpenFunction[m_iWantPageIndex])
			{
				Global.ScendID(ProtoIndexes.C_GET_UPACTION_DATA, m_iWantPageIndex);
			}
			else
			{
				m_iWantPageIndex = m_iPageIndex;
			}
		}
		else if(ui.name.IndexOf("icon") != -1)
		{
			int tempIndex = int.Parse(ui.name.Substring(4,1));
			switch(m_iPageIndex)
			{
			case 0:
//				if(data0.tianfuId[tempIndex])
				Global.m_sMainCityWantOpenPanel = 200;
				Global.m_sPanelWantRun = "Skill2," + data0.tianfuId[tempIndex];
				MainCityUI.TryRemoveFromObjectList(gameObject);
				m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
				m_ScaleEffectController.OnCloseWindowClick();
				Global.m_isShowBianqiang = true;
				Global.m_sBianqiangClick = "Page" + m_iPageIndex;
				break;
			case 1:
				if(data1.pageData[tempIndex / 3].zhuangbeiData[tempIndex % 3].type == 0)
				{

					if(tempIndex / 3 == 0)
					{
						Global.m_sPanelWantRun = "1,";
						Global.m_sMainCityWantOpenPanel = 12;
					}
					else if(tempIndex / 3 == 1)
					{
						Global.m_sPanelWantRun = "jinjie,0,";
						Global.m_sMainCityWantOpenPanel = 200;
					}
					else 
					{
						Global.m_sPanelWantRun = "2,";
						Global.m_sMainCityWantOpenPanel = 12;
					}
					Global.m_sPanelWantRun += data1.pageData[tempIndex / 3].zhuangbeiData[tempIndex % 3].id;
					MainCityUI.TryRemoveFromObjectList(gameObject);
					m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
					m_ScaleEffectController.OnCloseWindowClick();

					Global.m_isShowBianqiang = true;
					Global.m_sBianqiangClick = "Page" + m_iPageIndex;
				}
				else
				{
					Global.CreateBox("获取途径", data1.pageData[tempIndex / 3].zhuangbeiData[tempIndex % 3].text, null, null, "确定", null, null, null);
				}
				break;
			case 2:
				Global.m_sMainCityWantOpenPanel = 6;
				Global.m_sPanelWantRun = data2.pageData[tempIndex / 3].mibaoDataId[tempIndex % 3] + "";
				MainCityUI.TryRemoveFromObjectList(gameObject);
				m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
				m_ScaleEffectController.OnCloseWindowClick();
				Global.m_isShowBianqiang = true;
				Global.m_sBianqiangClick = "Page" + m_iPageIndex;
				break;
			case 3:
				Global.m_sMainCityWantOpenPanel = 200;
				Global.m_sPanelWantRun = "Skill3," + data3.pageData[tempIndex / 3].fuwenDataId[tempIndex % 3];
				MainCityUI.TryRemoveFromObjectList(gameObject);
				m_ScaleEffectController.CloseCompleteDelegate = DoCloseWindow;
				m_ScaleEffectController.OnCloseWindowClick();
				Global.m_isShowBianqiang = true;
				Global.m_sBianqiangClick = "Page" + m_iPageIndex;
				break;
			}
			Debug.Log("Global.m_sPanelWantRun="+Global.m_sPanelWantRun);
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
