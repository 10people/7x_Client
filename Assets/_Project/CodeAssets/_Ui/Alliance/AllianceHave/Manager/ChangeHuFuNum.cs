using UnityEngine;
using System.Collections;

public class ChangeHuFuNum : MonoBehaviour {

	bool ispress = false;
	private int m_iNum = 0;
	public GameObject ReCruit_root;
	
	public int BtnType = 0;//按钮类型 1为等级左减 2 为等级右加 3 为军衔左减 4为军衔右加
	
	MyAllianceInfo mMyAllianceInfo;
	void Start () {

		mMyAllianceInfo = ReCruit_root.GetComponent<MyAllianceInfo>();
	}
	
	// Update is called once per frame
	void Update () {
	

		if(ispress)
		{
			m_iNum ++;
			if(m_iNum > 10)
			{
				StartCoroutine(Re_CHangeStart()); 
			}
		}

	}
	void OnPress()
	{
		ispress = !ispress;
		if (ispress)
		{
			StartCoroutine(Re_CHangeStart()); 
		}
		m_iNum = 0;
		
	}
	IEnumerator Re_CHangeStart()
	{
		yield return new WaitForSeconds (0.0f);
		if (BtnType == 1) {
			if (mMyAllianceInfo.HufuNum > 1) {
				mMyAllianceInfo.HufuNum -= 1;
				Debug.Log("ispress111    "+BtnType);
				if (mMyAllianceInfo.HufuNum == 1) 
				{
				   ispress = false;
							

				}
			}
		}
				
		if (BtnType == 2) {

			if (mMyAllianceInfo.HufuNum < mMyAllianceInfo.Maxhufunum) 
			{
				mMyAllianceInfo.HufuNum += 1;
				Debug.Log("ispress222    "+BtnType);
				if (mMyAllianceInfo.HufuNum == mMyAllianceInfo.Maxhufunum) 
				{
					ispress = false;
				}		
			}
		}
		mMyAllianceInfo.initHufuData ();
	}

}
