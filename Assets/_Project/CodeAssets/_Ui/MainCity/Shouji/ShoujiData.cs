using UnityEngine;
using System.Collections;

public class ShoujiData 
{
	public int m_iID;
	public int m_iType;
	public int m_iCurNum;
	public int m_iMaxNum;
	public string m_sDrawString;

	public ShoujiData(int id, int type, int curNum, int maxNum, string drawString)
	{
		m_iID = id;
		m_iType = type;
		m_iCurNum = curNum;
		m_iMaxNum = maxNum;
		m_sDrawString = drawString;
	}

	public void setCurNum(int num)
	{
		m_iCurNum = num;
	}
}
