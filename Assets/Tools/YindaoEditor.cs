using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class YindaoEditor : MonoBehaviour 
{
	public static YindaoEditor YINDAOEDITOR;

	public GameObject m_ObjBox;

	public UIYindao m_UIYindao;

	public static bool m_open_guide_editor = false;

	private int m_iSelectType = -1;

	private int m_iSelectYindaoIndex = -1;

	private YindaoTemp.YindaoElenemt m_curYindaoElenemt = new YindaoTemp.YindaoElenemt();
	
//	private bool[] m_isOpenElement = new bool[]{false, false, false};

	private bool m_isMouse = false;

	private bool m_isMouseDown = false;

	private Vector3 m_PosB;

	private Vector3 m_PosE;

	private int m_isMouseNum = 0;

	private string m_sId= "";

	private string m_sOpenDialogID = "";

	private string m_sYinDaoDialogID = "";

	private string m_sEffID = "";

	private string[] m_sPosShow = new string[]{ "＼", "↑", "／", "←", "〇", "→", "／", "↓", "＼" };

	private int m_iGUIMoveX = 0;
//	private List<YindaoElenemt> m_listYindaoElenemt;

	public static YindaoEditor Instance(){
//		if(YINDAOEDITOR == null)
//		{
//			YINDAOEDITOR = this;
//		}

		return YINDAOEDITOR;
	}

	// Use this for initialization
	void Start (){
		YINDAOEDITOR = this;

		m_open_guide_editor = ConfigTool.GetBool( ConfigTool.CONST_OPEN_GUIDE_EDITOR );

		if(m_open_guide_editor)
		{
			m_ObjBox.SetActive(true);
		}
	}

	private int getYindaoIndex(int id)
	{
		for(int i = 0; i < YindaoTemp.m_listYindaoElenemt.Count; i ++)
		{
			if(YindaoTemp.m_listYindaoElenemt[i].m_ID == id)
			{
				return i;
			}
		}
		return -1;
	}

	private void openEditorYindao()
	{
		m_iSelectType = -1;
		if(m_iSelectYindaoIndex != -1)
		{
			YindaoTemp.m_listYindaoElenemt[m_iSelectYindaoIndex] = m_curYindaoElenemt;
		}
		int tempID = int.Parse(m_sId);
		m_iSelectYindaoIndex = getYindaoIndex(tempID);
		if(m_iSelectYindaoIndex == -1)
		{
			m_UIYindao.CloseUI();
			m_curYindaoElenemt = new YindaoTemp.YindaoElenemt();
			
			m_curYindaoElenemt.m_ID = tempID;
			m_iSelectYindaoIndex = YindaoTemp.m_listYindaoElenemt.Count;
			YindaoTemp.m_listYindaoElenemt.Add(m_curYindaoElenemt);
			m_UIYindao.setOpenYindao(m_curYindaoElenemt);
		}
		else
		{
			m_UIYindao.CloseUI();
			m_curYindaoElenemt = YindaoTemp.m_listYindaoElenemt[m_iSelectYindaoIndex];
			m_UIYindao.setOpenYindao(tempID);
		}
		if(m_open_guide_editor)
		{
			m_ObjBox.SetActive(true);
		}
	}

	public void setCurYindao(int id)
	{
		m_iSelectType = -1;
		m_sId = id + "";
		int tempID = int.Parse(m_sId);
		m_iSelectYindaoIndex = getYindaoIndex(tempID);
		m_curYindaoElenemt = YindaoTemp.m_listYindaoElenemt[m_iSelectYindaoIndex];
	}

	void OnGUI()
	{
		if( !m_open_guide_editor ){
			return;
		}
		if(GUI.Button(new Rect(Screen.width / 2 - 50,590,50,25), "左"))
		{
			m_iGUIMoveX -= 50;
		}
		else if(GUI.Button(new Rect(Screen.width /2 + 50,590,50,25), "右"))
		{
			m_iGUIMoveX += 50;
		}
		else if(GUI.Button(new Rect(Screen.width - 175 + m_iGUIMoveX,0,50,25), "开启"))
		{
			openEditorYindao();
		}
		else if(GUI.Button(new Rect(130 + m_iGUIMoveX,0,50,25), "取消"))
		{
			m_iSelectType = -1;
		}
		else if(GUI.Button(new Rect(Screen.width - 125 + m_iGUIMoveX,0,50,25), "关闭"))
		{
			m_UIYindao.CloseUI();
			if(m_open_guide_editor)
			{
				m_ObjBox.SetActive(false);
				m_iSelectType = -1;
			}
		}
		else if(m_ObjBox.activeSelf && GUI.Button(new Rect(Screen.width - 75 + m_iGUIMoveX,25,75,25), "关闭Box"))
		{
			m_ObjBox.SetActive(false);
			m_iSelectType = -1;
		}
		else if(!m_ObjBox.activeSelf && GUI.Button(new Rect(Screen.width - 75 + m_iGUIMoveX,25,75,25), "开启Box"))
		{
			m_ObjBox.SetActive(true);
		}
		else if(GUI.Button(new Rect(Screen.width - 75 + m_iGUIMoveX,0,75,25), "保存文档"))
		{
			if(m_iSelectYindaoIndex != -1)
			{
				YindaoTemp.m_listYindaoElenemt[m_iSelectYindaoIndex] = m_curYindaoElenemt;
			}
			string tempDocumentsPath = Application.dataPath +  "/Resources/_Data/Design/";
			if (!Directory.Exists(tempDocumentsPath))
			{
				Directory.CreateDirectory(tempDocumentsPath);
			}
			FileStream temp = File.Create(tempDocumentsPath + "/YinDaoText.txt");
			StreamWriter sw = new StreamWriter(temp);
			sw.Write("任务步骤=" + YindaoTemp.m_listYindaoElenemt.Count + "\r\n");
			for(int i = 0; i < YindaoTemp.m_listYindaoElenemt.Count; i ++)
			{
				sw.Write("id=" + YindaoTemp.m_listYindaoElenemt[i].m_ID + "\r\n");
				int tempNum = 0;
				if(YindaoTemp.m_listYindaoElenemt[i].m_Click != null)
				{
					tempNum ++;
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_ImageMove != null)
				{
					tempNum ++;
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_Label != null)
				{
					tempNum ++;
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_Eff != null)
				{
					tempNum ++;
				}

				sw.Write("当前任务显示=" + tempNum + "\r\n");

				if(YindaoTemp.m_listYindaoElenemt[i].m_Click != null)
				{
					sw.Write("click\r\n");
					sw.Write("type=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.type + "\r\n");
					sw.Write("pos=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.pos + "\r\n");
					sw.Write("x=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.x + "\r\n");
					sw.Write("y=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.y + "\r\n");
					sw.Write("w=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.w + "\r\n");
					sw.Write("h=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.h + "\r\n");
					sw.Write("isColl=" + YindaoTemp.m_listYindaoElenemt[i].m_Click.isColl + "\r\n");
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_ImageMove != null)
				{
					sw.Write("imagemove\r\n");
					sw.Write("name=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.name + "\r\n");
					sw.Write("pos=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.pos + "\r\n");
					sw.Write("Angle=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.Angle + "\r\n");
					sw.Write("desID=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.desID + "\r\n");
					sw.Write("x=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.x + "\r\n");
					sw.Write("y=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.y + "\r\n");
					sw.Write("type=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.type + "\r\n");
					sw.Write("speedx=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.speedx + "\r\n");
					sw.Write("speedy=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.speedy + "\r\n");
					sw.Write("moveend=" + YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.moveend + "\r\n");
					if(YindaoTemp.m_listYindaoElenemt[i].m_ID == 603)
					{
						Debug.Log(i);
						Debug.Log(YindaoTemp.m_listYindaoElenemt[i].m_ImageMove.x);
					}
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_Label != null)
				{
					sw.Write("label\r\n");
					sw.Write("str=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.str + "\r\n");
					sw.Write("pos=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.pos + "\r\n");
					sw.Write("dialogid=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.dialogid + "\r\n");
					sw.Write("x=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.x + "\r\n");
					sw.Write("y=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.y + "\r\n");
					sw.Write("w=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.w + "\r\n");
					sw.Write("color=" + YindaoTemp.m_listYindaoElenemt[i].m_Label.color + "\r\n");
				}
				if(YindaoTemp.m_listYindaoElenemt[i].m_Eff != null)
				{
					sw.Write("Eff\r\n");
					sw.Write("id=" + YindaoTemp.m_listYindaoElenemt[i].m_Eff.id + "\r\n");
					sw.Write("pos=" + YindaoTemp.m_listYindaoElenemt[i].m_Eff.pos + "\r\n");
					sw.Write("x=" + YindaoTemp.m_listYindaoElenemt[i].m_Eff.x + "\r\n");
					sw.Write("y=" + YindaoTemp.m_listYindaoElenemt[i].m_Eff.y + "\r\n");
				}
			}
			sw.Close();
			temp.Close();
		}

		GUI.Box(new Rect(0 + m_iGUIMoveX,0,50,20),"click");
		GUI.Box(new Rect(0 + m_iGUIMoveX,20,50,20),"image");
		GUI.Box(new Rect(0 + m_iGUIMoveX,40,50,20),"label");
		GUI.Box(new Rect(0 + m_iGUIMoveX,60,50,20),"eff");


		if(m_curYindaoElenemt.m_Click == null)
		{
			if(GUI.Button(new Rect(50 + m_iGUIMoveX,0,30,20),"add") && m_iSelectYindaoIndex != -1)
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_Click = new Click();
				m_curYindaoElenemt.m_Click.type = 0;
				m_curYindaoElenemt.m_Click.pos = "C";
				m_curYindaoElenemt.m_Click.isColl = 1;
				m_curYindaoElenemt.m_Click.x = 480;
				m_curYindaoElenemt.m_Click.y = 320;
				m_curYindaoElenemt.m_Click.w = 100;
				m_curYindaoElenemt.m_Click.h = 100;
				m_iSelectType = 0;
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
		}
		if(m_curYindaoElenemt.m_ImageMove == null)
		{
			if(GUI.Button(new Rect(50 + m_iGUIMoveX,20,30,20),"add") && m_iSelectYindaoIndex != -1)
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_ImageMove = new ImageMove();
				m_curYindaoElenemt.m_ImageMove.name = "jiantou";
				m_curYindaoElenemt.m_ImageMove.pos = "C";
				m_curYindaoElenemt.m_ImageMove.Angle = 0;
				m_curYindaoElenemt.m_ImageMove.x = 480;
				m_curYindaoElenemt.m_ImageMove.y = 300;
				m_curYindaoElenemt.m_ImageMove.type = 0;
				m_curYindaoElenemt.m_ImageMove.speedx = 0;
				m_curYindaoElenemt.m_ImageMove.speedy = 2;
				m_curYindaoElenemt.m_ImageMove.moveend = 10;
				m_iSelectType = 1;
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
		}
		if(m_curYindaoElenemt.m_Label == null)
		{
			if(GUI.Button(new Rect(50 + m_iGUIMoveX,40,30,20),"add") && m_iSelectYindaoIndex != -1)
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_Label = new Label();

				m_curYindaoElenemt.m_Label.str = "XXXXXXXXXXX";
				m_curYindaoElenemt.m_Label.pos = "C";
				m_curYindaoElenemt.m_Label.dialogid = 100000;
				m_curYindaoElenemt.m_Label.x = 0;
				m_curYindaoElenemt.m_Label.y = 0;
				m_curYindaoElenemt.m_Label.w = 0;
				m_curYindaoElenemt.m_Label.color = "[ffffff]";
				m_iSelectType = 2;
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
				m_curYindaoElenemt.m_Label.y = 640;
				m_sOpenDialogID = "100000";
			}
		}
		if(m_curYindaoElenemt.m_Eff == null)
		{
			if(GUI.Button(new Rect(50 + m_iGUIMoveX,60,30,20),"add") && m_iSelectYindaoIndex != -1)
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_Eff = new Eff();
				
				m_curYindaoElenemt.m_Eff.id = 100000;
				m_curYindaoElenemt.m_Eff.pos = "C";
				m_curYindaoElenemt.m_Eff.x = 480;
				m_curYindaoElenemt.m_Eff.y = 300;
				m_iSelectType = 3;
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
				m_sEffID = "100000";
			}
		}
		for(int i = 0; i < 4; i ++)
		{
			if(GUI.Button(new Rect(80 + m_iGUIMoveX,i * 20 ,20,20), m_iSelectType == i ? "●" : ""))
			{
				if(!m_ObjBox.activeSelf)
				{
					return;
				}
				switch(i)
				{
				case 0:
					if(m_curYindaoElenemt.m_Click == null)
					{
						return;
					}
					break;
				case 1:
					if(m_curYindaoElenemt.m_ImageMove == null)
					{
						return;
					}
					break;
				case 2:
					if(m_curYindaoElenemt.m_Label == null)
					{
						return;
					}
					else
					{
						m_sOpenDialogID = "" + m_curYindaoElenemt.m_Label.dialogid;
					}
					break;
				case 3:
					if(m_curYindaoElenemt.m_Eff == null)
					{
						return;
					}
					else
					{
						m_sEffID = "" + m_curYindaoElenemt.m_Eff.id;
					}
					break;
				}
				m_iSelectType = i;
			}
			else if(GUI.Button(new Rect(100 + m_iGUIMoveX,i * 20 ,30,20), "X"))
			{
				m_iSelectType = -1;
				m_UIYindao.CloseUI();
				switch(i)
				{
				case 0:
					m_curYindaoElenemt.m_Click = null;
					break;
				case 1:
					m_curYindaoElenemt.m_ImageMove = null;
					break;
				case 2:
					m_curYindaoElenemt.m_Label = null;
					break;
				}
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
		}
		m_sId = GUI.TextArea(new Rect(Screen.width / 2 - 50 + m_iGUIMoveX,0,100,20),m_sId);
		switch(m_iSelectType)
		{
		case 0:
			for(int i = 0; i < 9; i ++)
			{
				if(i != getPosIndex(m_curYindaoElenemt.m_Click.pos))
				{
					if(GUI.Button(new Rect(((i % 3) * 30) + m_iGUIMoveX, Screen.height - 90 + ((int)(i / 3)) * 30,30,30), m_sPosShow[i]))
					{
						m_curYindaoElenemt.m_Click.pos = getPosString(i);
						Debug.Log(m_curYindaoElenemt.m_Click.pos);
					}
				}
			}
			if(!m_isMouse)
			{
				if(GUI.Button(new Rect(Screen.width - 50 + m_iGUIMoveX,50,50,50), "鼠标框"))
				{
					m_isMouse = true;
				}
			}
			if(Input.GetMouseButton (0))
			{
				if(m_isMouse)
				{
					if(m_isMouseDown)
					{
						m_PosE = Input.mousePosition;
						if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
						{
							if(m_PosB.x != m_PosE.x && m_PosB.y != m_PosE.y)
							{
								m_UIYindao.CloseUI();
								int w = Mathf.Abs((int)m_PosB.x - (int)m_PosE.x) * 2;
								int h = Mathf.Abs((int)m_PosB.y - (int)m_PosE.y) * 2;
								int x = (int)m_PosB.x;
								int y = (int)m_PosB.y;
								y = 640 - y;
								m_curYindaoElenemt.m_Click.x = x;
								m_curYindaoElenemt.m_Click.y = y;
								m_curYindaoElenemt.m_Click.w = w;
								m_curYindaoElenemt.m_Click.h = h;
								m_UIYindao.setOpenYindao(m_curYindaoElenemt);
							}
						}
					}
					else
					{
						m_isMouseDown = true;
						m_PosB = Input.mousePosition;
					}
				}
				else
				{
					if(m_isMouseDown)
					{
						m_PosE = Input.mousePosition;
						if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
						{
							m_UIYindao.CloseUI();
							m_curYindaoElenemt.m_Click.x += (int)(m_PosE.x - m_PosB.x);
							m_curYindaoElenemt.m_Click.y -= (int)(m_PosE.y - m_PosB.y);
							m_UIYindao.setOpenYindao(m_curYindaoElenemt);
							m_PosB = m_PosE;
						}
					}
					else
					{
						m_isMouseDown = true;
						m_PosB = Input.mousePosition;
					}
				}
			}
			else if(!Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					if(m_isMouse)
					{
						m_UIYindao.CloseUI();
						int w = Mathf.Abs((int)m_PosB.x - (int)m_PosE.x) * 2;
						int h = Mathf.Abs((int)m_PosB.y - (int)m_PosE.y) * 2;
						int x = (int)m_PosB.x;
						int y = (int)m_PosB.y;
						y = 640 - y;
						m_curYindaoElenemt.m_Click.x = x;
						m_curYindaoElenemt.m_Click.y = y;
						m_curYindaoElenemt.m_Click.w = w;
						m_curYindaoElenemt.m_Click.h = h;
						m_UIYindao.setOpenYindao(m_curYindaoElenemt);
						m_isMouse = false;
						m_isMouseDown = false;
					}
					else
					{
						m_isMouseDown = false;
					}
				}
			}
			break;
		case 1:
			for(int i = 0; i < 9; i ++)
			{
				if(i != getPosIndex(m_curYindaoElenemt.m_ImageMove.pos))
				{
					if(GUI.Button(new Rect(((i % 3) * 30), Screen.height - 90 + ((int)(i / 3)) * 30,30,30), m_sPosShow[i]))
					{
						m_curYindaoElenemt.m_ImageMove.pos = getPosString(i);
						Debug.Log(m_curYindaoElenemt.m_Click.pos);
					}
				}
			}

			if(Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					m_PosE = Input.mousePosition;
					if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
					{
						m_UIYindao.CloseUI();
						m_curYindaoElenemt.m_ImageMove.x += (int)(m_PosE.x - m_PosB.x);
//						if(YindaoTemp.m_listYindaoElenemt[i].m_ID == 603)
//						{
//							Debug.Log(m_iSelectYindaoIndex);
//							Debug.Log("m_curYindaoElenemt.m_ImageMove.x=" + m_curYindaoElenemt.m_ImageMove.x);
//						}
						m_curYindaoElenemt.m_ImageMove.y -= (int)(m_PosE.y - m_PosB.y);
						m_UIYindao.setOpenYindao(m_curYindaoElenemt);
						m_PosB = m_PosE;
					}
				}
				else
				{
					m_isMouseDown = true;
					m_PosB = Input.mousePosition;
				}
			}
			else if(!Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					m_isMouseDown = false;
				}
			}
			break;
		case 2:
			if(!m_isMouse)
			{
				if(GUI.Button(new Rect(Screen.width - 50 + m_iGUIMoveX,50,50,50), "鼠标框"))
				{
					m_isMouse = true;
				}
			}
			if(Input.GetMouseButton (0))
			{
				if(m_isMouse)
				{
					if(m_isMouseDown)
					{
						m_PosE = Input.mousePosition;
						if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
						{
							if(m_PosB.x != m_PosE.x && m_PosB.y != m_PosE.y)
							{
								m_UIYindao.CloseUI();
								int w = Mathf.Abs((int)m_PosB.x - (int)m_PosE.x) + 80;
								Debug.Log(w);
								if(w + 60 > 960)
								{
									w = 0;
								}
								Debug.Log("zuizhong="+w);
								int x = (int)m_PosB.x;
								int y = (int)m_PosB.y;
								y = 640 - y;
								m_curYindaoElenemt.m_Label.x = x;
								m_curYindaoElenemt.m_Label.y = y;
								m_curYindaoElenemt.m_Label.w = w;
								m_UIYindao.setOpenYindao(m_curYindaoElenemt);
							}
						}
					}
					else
					{
						m_isMouseDown = true;
						m_PosB = Input.mousePosition;
					}
				}
				else
				{
					if(m_isMouseDown)
					{
						m_PosE = Input.mousePosition;
						if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
						{
							m_UIYindao.CloseUI();
							m_curYindaoElenemt.m_Label.x += (int)(m_PosE.x - m_PosB.x);
							m_curYindaoElenemt.m_Label.y -= (int)(m_PosE.y - m_PosB.y);
							Debug.Log(m_curYindaoElenemt.m_Label.w);
							if(m_curYindaoElenemt.m_Label.x < 0)
							{
								m_curYindaoElenemt.m_Label.x = 0;
							}
							if(m_curYindaoElenemt.m_Label.y >= 640)
							{
								m_curYindaoElenemt.m_Label.y = 640;
							}
							if(m_curYindaoElenemt.m_Label.w == 0)
							{
								m_curYindaoElenemt.m_Label.w = 900;
							}
							if(m_curYindaoElenemt.m_Label.x + m_curYindaoElenemt.m_Label.w + 60 > 960)
							{
								m_curYindaoElenemt.m_Label.x = 960 - (m_curYindaoElenemt.m_Label.w + 60);
							}
							if(m_curYindaoElenemt.m_Label.w == 900)
							{
								m_curYindaoElenemt.m_Label.w = 0;
							}
							m_UIYindao.setOpenYindao(m_curYindaoElenemt);
							m_PosB = m_PosE;
						}
					}
					else
					{
						m_isMouseDown = true;
						m_PosB = Input.mousePosition;
					}
				}
			}
			else if(!Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					if(m_isMouse)
					{
						m_UIYindao.CloseUI();
						int w = Mathf.Abs((int)m_PosB.x - (int)m_PosE.x) + 80;
						if(w + 60 > 960)
						{
							w = 0;
						}
						int x = (int)m_PosB.x;
						int y = (int)m_PosB.y;
						y = 640 - y;
						m_curYindaoElenemt.m_Label.x = x;
						m_curYindaoElenemt.m_Label.y = y;
						m_curYindaoElenemt.m_Label.w = w;
						m_UIYindao.setOpenYindao(m_curYindaoElenemt);
						m_isMouse = false;
						m_isMouseDown = false;
					}
					else
					{
						m_isMouseDown = false;
					}
				}
			}
			break;
		case 3:
			for(int i = 0; i < 9; i ++)
			{
				if(i != getPosIndex(m_curYindaoElenemt.m_Eff.pos))
				{
					if(GUI.Button(new Rect(((i % 3) * 30) + m_iGUIMoveX, Screen.height - 90 + ((int)(i / 3)) * 30,30,30), m_sPosShow[i]))
					{
						m_curYindaoElenemt.m_Eff.pos = getPosString(i);
						Debug.Log(m_curYindaoElenemt.m_Click.pos);
					}
				}
			}
			
			if(Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					m_PosE = Input.mousePosition;
					if((m_isMouseNum = ++m_isMouseNum % 10) == 0)
					{
						m_UIYindao.CloseUI();
						m_curYindaoElenemt.m_Eff.x += (int)(m_PosE.x - m_PosB.x);
						//						if(YindaoTemp.m_listYindaoElenemt[i].m_ID == 603)
						//						{
						//							Debug.Log(m_iSelectYindaoIndex);
						//							Debug.Log("m_curYindaoElenemt.m_ImageMove.x=" + m_curYindaoElenemt.m_ImageMove.x);
						//						}
						m_curYindaoElenemt.m_Eff.y -= (int)(m_PosE.y - m_PosB.y);
						m_UIYindao.setOpenYindao(m_curYindaoElenemt);
						m_PosB = m_PosE;
					}
				}
				else
				{
					m_isMouseDown = true;
					m_PosB = Input.mousePosition;
				}
			}
			else if(!Input.GetMouseButton (0))
			{
				if(m_isMouseDown)
				{
					m_isMouseDown = false;
				}
			}
			break;
		}
		switch(m_iSelectType)
		{
		case 0:
			GUI.Box(new Rect(0 + m_iGUIMoveX,80 ,25,20), "○");
			GUI.Box(new Rect(50 + m_iGUIMoveX,80 ,25,20), "□");

			GUI.Label(new Rect(0 + m_iGUIMoveX,80 ,100,20), "isColl");
			if(GUI.Button(new Rect(25 + m_iGUIMoveX,80 ,25,20), m_curYindaoElenemt.m_Click.type == 0 ? "●" : ""))
			{
				m_curYindaoElenemt.m_Click.type = 0;
				m_UIYindao.CloseUI();
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			else if(GUI.Button(new Rect(75 + m_iGUIMoveX,80 ,25,20), m_curYindaoElenemt.m_Click.type == 1 ? "●" : ""))
			{
				m_curYindaoElenemt.m_Click.type = 1;
				m_UIYindao.CloseUI();
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			else if(GUI.Button(new Rect(75 + m_iGUIMoveX,100 ,25,20), m_curYindaoElenemt.m_Click.isColl == 0 ? "●" : "○"))
			{
				m_curYindaoElenemt.m_Click.isColl = ++m_curYindaoElenemt.m_Click.isColl % 2;
				m_UIYindao.CloseUI();
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			break;
		case 1:
			for(int i = 0; i < 9; i += 2)
			{
				if((i == 0 && m_curYindaoElenemt.m_ImageMove.Angle == 3)
				   ||(i == 2 && m_curYindaoElenemt.m_ImageMove.Angle == 1)
				   ||(i == 6 && m_curYindaoElenemt.m_ImageMove.Angle == 2)
				   ||(i == 8 && m_curYindaoElenemt.m_ImageMove.Angle == 0)
				   || i == 4
				   )
				{
					Debug.Log("continue=" + i);
					continue;
				}
				if(GUI.Button(new Rect(((i % 3) * 30) + m_iGUIMoveX, 300 + ((int)(i / 3)) * 30,30,30), m_sPosShow[i]))
				{
					m_UIYindao.CloseUI();
					switch(i)
					{
					case 0:
						m_curYindaoElenemt.m_ImageMove.Angle = 3;
						m_curYindaoElenemt.m_ImageMove.type = 0;
						m_curYindaoElenemt.m_ImageMove.speedx = 0;
						m_curYindaoElenemt.m_ImageMove.speedy = 2;
						break;
					case 2:
						m_curYindaoElenemt.m_ImageMove.Angle = 1;
						m_curYindaoElenemt.m_ImageMove.type = 0;
						m_curYindaoElenemt.m_ImageMove.speedx = 2;
						m_curYindaoElenemt.m_ImageMove.speedy = 0;
						break;
					case 6:
						m_curYindaoElenemt.m_ImageMove.Angle = 2;
						m_curYindaoElenemt.m_ImageMove.type = 0;
						m_curYindaoElenemt.m_ImageMove.speedx = -2;
						m_curYindaoElenemt.m_ImageMove.speedy = 0;
						break;
					case 8:
						m_curYindaoElenemt.m_ImageMove.Angle = 0;
						m_curYindaoElenemt.m_ImageMove.type = 0;
						m_curYindaoElenemt.m_ImageMove.speedx = 0;
						m_curYindaoElenemt.m_ImageMove.speedy = -2;
						break;
					}
					m_UIYindao.setOpenYindao(m_curYindaoElenemt);
				}
			}
			m_sYinDaoDialogID = GUI.TextArea(new Rect(0 + m_iGUIMoveX,400,100,20),m_sYinDaoDialogID);
			if(GUI.Button(new Rect(100 + m_iGUIMoveX,400 ,50,20), "确定"))
			{
				m_curYindaoElenemt.m_ImageMove.desID = int.Parse(m_sYinDaoDialogID);
				m_UIYindao.CloseUI();
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			break;
		case 2:
			m_sOpenDialogID = GUI.TextArea(new Rect(0 + m_iGUIMoveX,80,100,20),m_sOpenDialogID);
			if(GUI.Button(new Rect(100 + m_iGUIMoveX, 80 ,30,20), ""))
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_Label.dialogid = int.Parse(m_sOpenDialogID);
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			break;
		case 3:
			m_sEffID = GUI.TextArea(new Rect(0 + m_iGUIMoveX,80,100,20),m_sEffID);
			if(GUI.Button(new Rect(100 + m_iGUIMoveX, 80 ,30,20), ""))
			{
				m_UIYindao.CloseUI();
				m_curYindaoElenemt.m_Eff.id = int.Parse(m_sEffID);
				m_UIYindao.setOpenYindao(m_curYindaoElenemt);
			}
			break;
		}
	}

	public int getPosIndex(string pos)
	{
		int tempIndex = 4;
		if(pos.IndexOf("l") != -1 || pos.IndexOf("L") != -1 )
		{
			tempIndex -= 1;
		}
		else if(pos.IndexOf("r") != -1 || pos.IndexOf("R") != -1 )
		{
			tempIndex += 1;
		}
		if(pos.IndexOf("t") != -1 || pos.IndexOf("T") != -1 )
		{
			tempIndex -= 3;
		}
		else if(pos.IndexOf("b") != -1 || pos.IndexOf("B") != -1 )
		{
			tempIndex += 3;
		}
		return tempIndex;
	}

	public string getPosString(int index)
	{
		switch(index)
		{
		case 0:
			return "LT";
		case 1:
			return "T";
		case 2:
			return "RT";
		case 3:
			return "L";
		case 4:
			return "C";
		case 5:
			return "R";
		case 6:
			return "LB";
		case 7:
			return "B";
		case 8:
			return "RB";
		}
		return "";
	}
}
