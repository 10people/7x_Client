using UnityEngine;
using System.Collections;

public class Eventitem : MonoBehaviour {

	public string EventData;
	public UILabel m_Time;
	public UILabel m_Event;

	void Start () {
	
	}

	void Update () {
	
	}
	public void init()
	{

		char[] w = {'#'};

		//Debug.Log ("EventData = " +EventData);

		string[] s = EventData.Split (w);
		for (int i = 0; i < s.Length; i++) 
		{
			m_Time.text = s[0];
			
			m_Event.text = SortString(s[1]);
		}

	}
	string SortString(string mStr)
	{
		string NewStr = mStr;
		for (int i = 0; i < mStr.Length-1; i++) {
			char ame = mStr [i];
			char q = mStr [i + 1];
			if (ame == '掠' && q == '夺') {
				string s = mStr;
				s = s.Replace ('掠', '征');
				s = s.Replace ('夺', '战');
				NewStr = s;
			} 
		}
		for (int i = 0; i < mStr.Length-4; i++) {
			char ame = mStr [i];
			char q1 = mStr [i + 1];
			char q2 = mStr [i + 2];
			char q3 = mStr [i + 3];
			if (ame == '洗' && q1 == '劫'&& q2 == '权'&& q3 == '贵') {
				string s = mStr;
				s = s.Replace ('洗', '木');
				s = s.Replace ('劫', '桶');
				s = s.Replace ('权', '挑');
				s = s.Replace ('贵', '战');
				NewStr = s;
			}
		}
	 
		return NewStr;
	}
}
