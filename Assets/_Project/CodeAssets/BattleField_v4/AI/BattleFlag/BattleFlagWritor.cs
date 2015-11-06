using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BattleFlagWritor : MonoBehaviour
{

	public int chapterId;


	private List<BattleWinFlag> winFlags = new List<BattleWinFlag>();

	private List<BattleFlag> flags = new List<BattleFlag>();

	private List<BattleBuffFlag> buffFlags = new List<BattleBuffFlag> ();

	private List<BattleFlagGroup> groups = new List<BattleFlagGroup> ();

	private List<BattleCameraFlag> cameraFalgs = new List<BattleCameraFlag>();

	private List<BattleDramaFlag> dramaFalgs = new List<BattleDramaFlag>();


	private List<int> walls = new List<int>();


	void Start()
	{
		//checkout ();
	}

	public void checkout()
	{
		string path = Application.dataPath + "/Resources/_Data/BattleField/";

		{
			getBattleFlagGroup();

			writeBattleFlagGroup();
		}

		{
			getBattleFlag();

			writeBattleFlag();
		}

		{
			getBattleBuffFlag();

			writeBattleBuffFlag();
		}

		{
			getBattleCameraFlag();

			writeBattleCameraFlag(path);
		}

		{
			getBattleDramaFlag();

			writeBattleDramaFlag();
		}

		{
			getBattleWinFlag();

			writeBattleWin();
		}

		GameObject Changeobject = GameObject.Find ("ChangeXmlToJson");

		if(Changeobject != null)
		{
			Changeobject.SendMessage("writeNext");
		}
	}

	public void rewriteCameraFlag()
	{
		chapterId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		getBattleCameraFlag();
		
		writeBattleCameraFlag("C:\\7xHeros\\");
	}

	private void getBattleFlagGroup()
	{
		groups.Clear ();

		Component[] coms = GetComponentsInChildren(typeof(BattleFlagGroup));
		
		foreach(Component temp in coms)
		{
			BattleFlagGroup group = (BattleFlagGroup)temp;
			
			groups.Add(group);
		}
	}

	private void writeBattleFlagGroup()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/BattleFlags" + "//" + "Group_" + chapterId + ".xml";
		
		FileInfo t = new FileInfo(pa);
		
		StreamWriter sw = t.CreateText();
		
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		
		sw.WriteLine("<dataset>");
		
		foreach(BattleFlagGroup group in groups)
		{
			string str = "<BattleGroup";
			
			str += " groupId=\"" + group.groupId + "\"";
			
			str += " maxActive=\"" + group.maxActive + "\"";
			
			str += " delay=\"" + group.delay + "\"";
			
			str += " />";
			
			sw.WriteLine(str);
		}
		
		sw.WriteLine("</dataset>");
		
		sw.Close();
		
		sw.Dispose();
		
		Debug.Log ("Write Battle Group !  " + pa);
	}

	private void getBattleFlag()
	{
		flags.Clear();

		Component[] coms = GetComponentsInChildren(typeof(BattleFlag));

		foreach(Component temp in coms)
		{
			BattleFlag bf = (BattleFlag)temp;

			bf.refreshEye2eyeFlags();

			bf.refreshEnterFlags();

			bf.refreshAttackFlags();

			bf.refreshKillFlags();

			bf.refreshGroup();

			flags.Add(bf);
		}
	}

	private void writeBattleFlag()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/BattleFlags" + "//" + "Battle_" + chapterId + ".xml";

		FileInfo t = new FileInfo(pa);
		
		StreamWriter sw = t.CreateText();

		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

		sw.WriteLine("<dataset>");

		foreach(BattleFlag bf in flags)
		{
			string str = "<BattleFlag";

			str += " flagId=\"" + bf.flagId + "\"";

			str += " forwardFlagId=\"" + bf.forwardFlagId + "\"";

			str += " triggerMode=\"" + (int)bf.triggerMode + "\"";

			str += " x=\"" + bf.transform.position.x + "\"";

			str += " y=\"" + bf.transform.position.y + "\"";

			str += " z=\"" + bf.transform.position.z + "\"";

			str += " rx=\"" + bf.transform.eulerAngles.x + "\"";
			
			str += " ry=\"" + bf.transform.eulerAngles.y + "\"";
			
			str += " rz=\"" + bf.transform.eulerAngles.z + "\"";

			str += " cx=\"" + bf.transform.localScale.x + "\"";
			
			str += " cy=\"" + bf.transform.localScale.y + "\"";
			
			str += " cz=\"" + bf.transform.localScale.z + "\"";

			str += " triggerCount=\"" + bf.triggerCount + "\"";

			str += " triggerFunc=\"" + (int)bf.triggerFunc + "\"";

			str += " willRelive=\"" + (bf.willRelive ? 1 : 0) + "\"";

			str += " dieable=\"" + (bf.dieable ? 1 : 0) + "\"";

			str += " accountable=\"" + (bf.accountable ? 1 : 0) + "\"";

			str += " hideInDrama=\"" + (bf.hideInDrama ? 1 : 0) + "\"";

			str += " hideWithDramable=\"" + (bf.hideWithDramable ? 1 : 0) + "\"";

			str += " guideId=\"" + bf.guideId + "\"";

			str += " hintLabelId=\"" + bf.hintLabelId + "\"";

			str += " groupId=\"" + (bf.flagGroup == null ? 0 :  bf.flagGroup.groupId) + "\"";

			string strNodeSkill = "";

			if(bf.nodeSkillAble.Count == 0) strNodeSkill += "0";

			for(int i = 0; i < bf.nodeSkillAble.Count; i++)
			{
				int skillId = bf.nodeSkillAble[i];

				if(i != 0) strNodeSkill += ",";

				strNodeSkill += skillId;
			}

			str += " nodeSkill=\"" + strNodeSkill + "\"";

			if(bf.alarmGc == null)
			{
				str += " alarmPosition=\"0,0,0\"";
			}
			else
			{
				str += " alarmPosition=\"" 
					+ bf.alarmGc.transform.position.x + "," 
					+ bf.alarmGc.transform.position.y + "," 
					+ bf.alarmGc.transform.position.z + "\"";
			}

			string strPath = "";

			//Path
			for(int i = 0; i < bf.hoverPathGc.Count; i++)
			{
				GameObject pathGc = bf.hoverPathGc[i];

				if(i != 0) strPath += "|";

				strPath += pathGc.transform.position.x + "," + pathGc.transform.position.y + "," + pathGc.transform.position.z;
			}

			str += " hoverPath=\"" + strPath + "\"";

			//Eye2eye
			{
				string strEye2eye = " triggerFlagEye2eye=\"";
				
				if(bf.triggerFlagEye2eye.Count == 0) strEye2eye += "0";
				
				for(int ieye = 0; ieye < bf.triggerFlagEye2eye.Count; ieye++)
				{
					strEye2eye += bf.triggerFlagEye2eye[ieye].flagId + "";
					
					if(ieye != bf.triggerFlagEye2eye.Count - 1)
					{
						strEye2eye += ",";
					}
				}
				
				strEye2eye += "\"";
				
				str += strEye2eye;
			}

			//Enter
			{
				string strEnter = " triggerFlagEnter=\"";

				if(bf.triggerFlagEnter.Count == 0) strEnter += "0";

				for(int iEnter = 0; iEnter < bf.triggerFlagEnter.Count; iEnter++)
				{
					strEnter += bf.triggerFlagEnter[iEnter].flagId + "";

					if(iEnter != bf.triggerFlagEnter.Count - 1)
					{
						strEnter += ",";
					}
				}

				strEnter += "\"";

				str += strEnter;
			}

			//Attack
			{
				string strAttack = " triggerFlagAttack=\"";

				if(bf.triggerFlagAttack.Count == 0) strAttack += "0";

				for(int iAttack = 0; iAttack < bf.triggerFlagAttack.Count; iAttack++)
				{
					strAttack += bf.triggerFlagAttack[iAttack].flagId + "";
					
					if(iAttack != bf.triggerFlagAttack.Count - 1)
					{
						strAttack += ",";
					}
				}
				
				strAttack += "\"";
				
				str += strAttack;
			}

			//Kill
			{
				string strKill = " triggerFlagKill=\"";

				if(bf.triggerFlagKill.Count == 0) strKill += "0";

				for(int iKill = 0; iKill < bf.triggerFlagKill.Count; iKill++)
				{
					strKill += bf.triggerFlagKill[iKill].flagId + "";
					
					if(iKill != bf.triggerFlagKill.Count - 1)
					{
						strKill += ",";
					}
				}
				
				strKill += "\"";
				
				str += strKill;
			}

			//blood
			{
				string strBlood = " triggerFlagBlood=\"";

				if(bf.triggerFlagBlood.Count == 0 && bf.triggerFlagBloodInteger.Count == 0) strBlood += "0";

				for(int iBlood = 0; iBlood < bf.triggerFlagBlood.Count; iBlood++)
				{
					strBlood += bf.triggerFlagBlood[iBlood].x + "|" + bf.triggerFlagBlood[iBlood].y;

					if(iBlood != bf.triggerFlagBlood.Count - 1)
					{
						strBlood += ",";
					}
				}

				for(int iBlood = 0; iBlood < bf.triggerFlagBloodInteger.Count; iBlood++)
				{
					strBlood += bf.triggerFlagBloodInteger[iBlood].x + "|" + bf.triggerFlagBloodInteger[iBlood].y;
					
					if(iBlood != bf.triggerFlagBloodInteger.Count - 1)
					{
						strBlood += ",";
					}
				}

				strBlood += "\"";

				str += strBlood;
			}

			//distance
			{
				string strDistance = " triggerFlagDistance=\"";

				BattleDistanceFlag[] distanceFlags = bf.gameObject.GetComponents<BattleDistanceFlag>();

				if(distanceFlags.Length == 0 && bf.triggerFlagDistance.Count == 0) strDistance += "0";
				
				for(int iDistance = 0; iDistance < distanceFlags.Length; iDistance++)
				{
					BattleDistanceFlag bdf = distanceFlags[iDistance];

					for(int jDistance = 0; jDistance < bdf.triggerDistance.Count; jDistance++)
					{
						Vector2 vec = bdf.triggerDistance[jDistance];

						strDistance += vec.x + "^" + (int)vec.y;

						if(jDistance != bdf.triggerDistance.Count - 1)
						{
							strDistance += "_";
						}
					}

					strDistance += "|" + bdf.count + "|" + bdf.triggerFlag.flagId;

					if(iDistance != distanceFlags.Length - 1)
					{
						strDistance += ",";
					}
				}

				for(int iDistance = 0; iDistance <  bf.triggerFlagDistance.Count; iDistance++)
				{
					DistanceFlag bdf = bf.triggerFlagDistance[iDistance];
					
					for(int jDistance = 0; jDistance < bdf.triggerDistance.Count; jDistance++)
					{
						Vector2 vec = bdf.triggerDistance[jDistance];
						
						strDistance += vec.x + "^" + (int)vec.y;
						
						if(jDistance != bdf.triggerDistance.Count - 1)
						{
							strDistance += "_";
						}
					}
					
					strDistance += "|" + bdf.count + "|" + bdf.triggerFlag;
					
					if(iDistance != bf.triggerFlagDistance.Count - 1)
					{
						strDistance += ",";
					}
				}

				strDistance += "\"";

				str += strDistance;
			}

			str += " />";

			sw.WriteLine(str);


			if(bf.flagId > 1000 && bf.flagId < 2000)
			{
				walls.Add((int)bf.transform.localScale.x);
			}
		}

		sw.WriteLine("</dataset>");

		sw.Close();
		
		sw.Dispose();

		Debug.Log ("Write Battle Flag !  " + pa);
	}

	private void getBattleBuffFlag()
	{
		buffFlags.Clear();
		
		Component[] coms = GetComponentsInChildren(typeof(BattleBuffFlag));
		
		foreach(Component temp in coms)
		{
			BattleBuffFlag bf = (BattleBuffFlag)temp;

			buffFlags.Add(bf);
		}
	}
	
	private void writeBattleBuffFlag()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/BattleFlags" + "//" + "Buff_" + chapterId + ".xml";
		
		FileInfo t = new FileInfo(pa);
		
		StreamWriter sw = t.CreateText();
		
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		
		sw.WriteLine("<dataset>");
		
		foreach(BattleBuffFlag bf in buffFlags)
		{
			string str = "<BattleBuffFlag";
			
			str += " flagId=\"" + bf.flagId + "\"";
			
			str += " x=\"" + bf.transform.position.x + "\"";
			
			str += " y=\"" + bf.transform.position.y + "\"";
			
			str += " z=\"" + bf.transform.position.z + "\"";
			
			str += " rx=\"" + bf.transform.eulerAngles.x + "\"";
			
			str += " ry=\"" + bf.transform.eulerAngles.y + "\"";
			
			str += " rz=\"" + bf.transform.eulerAngles.z + "\"";
			
			str += " cx=\"" + bf.transform.localScale.x + "\"";
			
			str += " cy=\"" + bf.transform.localScale.y + "\"";
			
			str += " cz=\"" + bf.transform.localScale.z + "\"";
			
			str += " refreshTime=\"" + bf.refreshTime + "\"";
			
			str += " />";
			
			sw.WriteLine(str);
		}
		
		sw.WriteLine("</dataset>");
		
		sw.Close();
		
		sw.Dispose();
		
		Debug.Log ("Write Battle Buff Flag !  " + pa);
	}

	private void getBattleCameraFlag()
	{
		cameraFalgs.Clear();

		Component[] coms = GetComponentsInChildren(typeof(BattleCameraFlag));

		foreach(Component temp in coms)
		{
			BattleCameraFlag bf = (BattleCameraFlag)temp;
			
			cameraFalgs.Add(bf);
		}
	}

	private void writeBattleCameraFlag(string _path)
	{
		string pa = _path + "BattleFlags" + "//" + "Camera_" + chapterId + ".xml";

		FileInfo t = new FileInfo(pa);

		StreamWriter sw = t.CreateText();

		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

		sw.WriteLine("<dataset>");

		foreach(BattleCameraFlag flag in cameraFalgs)
		{
			string str = "<BattleCameraFlag";

			str += " flagId=\"" + flag.flagId + "\"";

			str += " radius=\"" + flag.radius + "\"";

			str += " x=\"" + flag.transform.position.x + "\"";

			str += " y=\"" + flag.transform.position.y + "\"";

			str += " z=\"" + flag.transform.position.z + "\"";

			str += " px=\"" + flag.cameraPosition.x + "\"";
			
			str += " py=\"" + flag.cameraPosition.y + "\"";
			
			str += " pz=\"" + flag.cameraPosition.z + "\"";

			str += " rx=\"" + flag.cameraRotation.x + "\"";

			str += " ry=\"" + flag.cameraRotation.y + "\"";

			str += " rz=\"" + flag.cameraRotation.z + "\"";

			str += " ex=\"" + flag.camera4Param.x + "\"";

			str += " ey=\"" + flag.camera4Param.y + "\"";

			str += " ez=\"" + flag.camera4Param.z + "\"";

			str += " ew=\"" + flag.camera4Param.w + "\"";

			str += " killMin=\"" + flag.killMin + "\"";

			str += " killMax=\"" + flag.killMax + "\"";

			str += " />";

			sw.WriteLine(str);
		}

		sw.WriteLine("</dataset>");

		sw.Close();

		sw.Dispose();

		Debug.Log ("Write Battle Camera Flag !  " + pa);
	}

	private void getBattleDramaFlag()
	{
		dramaFalgs.Clear();
		
		Component[] coms = GetComponentsInChildren(typeof(BattleDramaFlag));
		
		foreach(Component temp in coms)
		{
			BattleDramaFlag bf = (BattleDramaFlag)temp;
			
			dramaFalgs.Add(bf);
		}
	}

	private void getBattleWinFlag()
	{
		winFlags.Clear ();

		BattleWinFlag[] flags = GetComponents<BattleWinFlag>();

		if (flags.Length == 0) Debug.LogError ("ERROR: There is no WinFlag !");

		foreach(BattleWinFlag bf in flags)
		{
			winFlags.Add(bf);
		}
	}

	private void writeBattleDramaFlag()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/BattleFlags" + "//" + "Drama_" + chapterId + ".xml";
		
		FileInfo t = new FileInfo(pa);
		
		StreamWriter sw = t.CreateText();
		
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		
		sw.WriteLine("<dataset>");
		
		foreach(BattleDramaFlag flag in dramaFalgs)
		{
			BoxCollider bc = (BoxCollider)flag.gameObject.GetComponent("BoxCollider");

			string str = "<BattleDramaFlag";
			
			str += " flagId=\"" + flag.flagId + "\"";
			
			str += " eventId=\"" + flag.eventId + "\"";
			
			str += " x=\"" + flag.transform.position.x + "\"";
			
			str += " y=\"" + flag.transform.position.y + "\"";
			
			str += " z=\"" + flag.transform.position.z + "\"";
			
			str += " cx=\"" + bc.size.x + "\"";
			
			str += " cy=\"" + bc.size.y + "\"";
			
			str += " cz=\"" + bc.size.z + "\"";

			str += " rx=\"" + flag.transform.localEulerAngles.x + "\"";

			str += " ry=\"" + flag.transform.localEulerAngles.y + "\"";

			str += " rz=\"" + flag.transform.localEulerAngles.z + "\"";

			str += " />";
			
			sw.WriteLine(str);
		}
		
		sw.WriteLine("</dataset>");
		
		sw.Close();
		
		sw.Dispose();

		Debug.Log ("Write Battle Drama Flag !  " + pa);
	}

	private void writeBattleWin()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/BattleFlags" + "//" + "Win_" + chapterId + ".xml";

		FileInfo t = new FileInfo(pa);

		StreamWriter sw = t.CreateText();

		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

		sw.WriteLine("<dataset>");

		foreach(BattleWinFlag flag in winFlags)
		{
			string str = "<BattleWin";

			str += " id=\"" + flag.winId + "\"";

			str += " winType=\"" + (int)flag.winType + "\"";

			str += " killNum=\"" + flag.killNum + "\"";

			float dx = flag.destinationObject == null ? 0 : flag.destinationObject.transform.position.x;

			float dy = flag.destinationObject == null ? 0 : flag.destinationObject.transform.position.y;

			float dz = flag.destinationObject == null ? 0 : flag.destinationObject.transform.position.z;

			str += " destinationx=\"" + dx + "\"";

			str += " destinationy=\"" + dy + "\"";

			str += " destinationz=\"" + dz + "\"";

			str += " destinationRadius=\"" + flag.destinationRadius + "\"";

			str += " showOnUI=\"" + (flag.showOnUI ? 1 : 0) + "\"";

			str += " />";

			sw.WriteLine(str);
		}

		sw.WriteLine("</dataset>");

		sw.Close();

		sw.Dispose();

		Debug.Log ("Write Battle Win Done !  " + pa);
	}

	public void writeWall()
	{
		string pa = Application.dataPath + "/Resources/_Data/BattleField/wall.xml";
		
		FileInfo t = new FileInfo(pa);
		
		StreamWriter sw = t.CreateText();
		
		sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
		
		sw.WriteLine("<dataset>");

		string str_1 = "1-5";

		string str_2 = "6-10";

		string str_3 = "11-20";

		string str_4 = "21-30";

		string str_5 = "30";

		List<int> int_1 = new List<int> ();

		List<int> int_2 = new List<int> ();

		List<int> int_3 = new List<int> ();

		List<int> int_4 = new List<int> ();

		List<int> int_5 = new List<int> ();

		foreach(int bf in walls)
		{
			string str = "<BattleFlag";
			
			str += " cx=\"" + bf + "\"";

			if(bf <= 5)
			{
				str += " cx=\"" + str_1 + "\"";

				int_1.Add(bf);
			}
			else if(bf <= 10)
			{
				str += " cx=\"" + str_2 + "\"";
				
				int_2.Add(bf);
			}
			else if(bf <= 20)
			{
				str += " cx=\"" + str_3 + "\"";
				
				int_3.Add(bf);
			}
			else if(bf <= 30)
			{
				str += " cx=\"" + str_4 + "\"";
				
				int_4.Add(bf);
			}
			else
			{
				str += " cx=\"" + str_5 + "\"";
				
				int_5.Add(bf);
			}

			str += " />";

			sw.WriteLine(str);
		}

		sw.WriteLine("</dataset>");

		sw.WriteLine("\nTOTAL:  ");

		writeList (int_1, str_1, sw);

		writeList (int_2, str_2, sw);

		writeList (int_3, str_3, sw);

		writeList (int_4, str_4, sw);

		writeList (int_5, str_5, sw);

		sw.Close();
		
		sw.Dispose();
		
		Debug.Log ("Write Battle Flag !  " + pa);
	}

	private void writeList(List<int> list, string str, StreamWriter sw)
	{
		string stri = "\n" + str + "( " + list.Count + " ) : ";
		
		foreach(int i in list)
		{
			stri += i + ", ";
		}

		sw.WriteLine(stri);
	}

}
