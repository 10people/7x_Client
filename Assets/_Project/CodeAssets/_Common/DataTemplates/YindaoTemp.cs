using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class YindaoTemp : XmlLoadManager
{
	public static string[] tempAllData;
	public static List<YindaoElenemt> m_listYindaoElenemt = new List<YindaoElenemt>();

//	public static List<string> m_ListMissionID = new List<string>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null )
	{
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "YinDaoText.txt"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){	
		{
			m_listYindaoElenemt.Clear();
		}

		TextAsset tempAsset;
		if(obj != null)
		{
			tempAsset = obj as TextAsset;

//			Debug.Log(tempAsset);

			tempAllData = tempAsset.text.Split('\r');
		}
		else
		{
			Debug.Log(www.text);

			tempAllData = www.text.Split('\r');
		}

//		Debug.Log(tempAllData.Length);

		tempfenxi();
	}

	public static void tempfenxi()
	{
		for(int i = 0; i < tempAllData.Length; i ++)
		{
			if(tempAllData[i].IndexOf("//") != -1)
			{
				tempAllData[i] = tempAllData[i].Substring(0, tempAllData[i].IndexOf("//"));
			}
		}
		int index = 1;

//		Debug.Log(tempAllData[0]);

		int num = int.Parse(Global.getString("=", tempAllData[0]));
		for(int i = 0; i < num; i ++)
		{
			try
			{
				YindaoElenemt tempElenemt = new YindaoElenemt();
//				m_listYindaoElenemt

				tempElenemt.m_ID = int.Parse(Global.getString("=", tempAllData[index++]));
				int elementNum = int.Parse(Global.getString("=", tempAllData[index++]));
				for (int q = 0; q < elementNum; q++)
				{
					string temp = tempAllData[index++];
					if (temp.IndexOf("click") != -1)
					{
						tempElenemt.m_Click = new Click();
						tempElenemt.m_Click.type = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Click.pos = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_Click.x = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Click.y = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Click.w = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Click.h = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Click.isColl = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
					}
					else if (temp.IndexOf("label") != -1)
					{
						tempElenemt.m_Label = new Label();
						tempElenemt.m_Label.str = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_Label.pos = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_Label.dialogid = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Label.x = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Label.y = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Label.w = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Label.color = Global.getString("=", YindaoTemp.tempAllData[index++]);
					}
					else if (temp.IndexOf("imagemove") != -1)
					{
						tempElenemt.m_ImageMove = new ImageMove();
						tempElenemt.m_ImageMove.name = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_ImageMove.pos = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_ImageMove.Angle = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_ImageMove.desID = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_ImageMove.x = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_ImageMove.y = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_ImageMove.type = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));

						tempElenemt.m_ImageMove.speedx = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_ImageMove.speedy = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						if(tempElenemt.m_ImageMove.Angle == 180)
						{
							tempElenemt.m_ImageMove.speedy = -2;
						}
						tempElenemt.m_ImageMove.moveend = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
					}
					else if (temp.IndexOf("Eff") != -1)
					{
						tempElenemt.m_Eff = new Eff();
						tempElenemt.m_Eff.id = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Eff.pos = Global.getString("=", YindaoTemp.tempAllData[index++]);
						tempElenemt.m_Eff.x = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
						tempElenemt.m_Eff.y = int.Parse(Global.getString("=", YindaoTemp.tempAllData[index++]));
					}
					else if (temp.IndexOf("image") != -1)
					{
						index += 5;
					}
				}
				tempElenemt.m_AddSound = false;
				m_listYindaoElenemt.Add(tempElenemt);
			}
			catch
			{
//				Debug.Log(m_ListMissionID[m_ListMissionID.Count - 1]);
			}
//			if(m_listYindaoElenemt[0] == null)
//			{
//				Debug.Log("==1");
//			}
		}
	}
	public struct YindaoElenemt
	{
		public int m_ID;
		public Click m_Click;
		public ImageMove m_ImageMove;
		public Label m_Label;
		public Eff m_Eff;
		public bool m_AddSound;
	}
}
public class Click
{
	public int type;
	public string pos;
	public int isColl;
	public int x;
	public int y;
	public int w;
	public int h;
}

public class ImageMove
{
	public string name;
	public string pos;
	public int Angle;
	public int desID;
	public int x;
	public int y;
	public int type;
	public int speedx;
	public int speedy;
	public int moveend;
}

public class Label
{
	public string str;
	public string pos;
	public int dialogid;
	public int x;
	public int y;
	public int w;
	public string color;
}

public class Eff
{
	public int id;
	public string pos;
	public int x;
	public int y;
	public GameObject eff;
}