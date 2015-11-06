using UnityEngine;
using System.Collections;

public class AddLvLeftBtn : MonoBehaviour {

	bool ispress = false;
	private int m_iNum = 0;
	public GameObject ReCruit_root;

	public int BtnType;//按钮类型 1为等级左减 2 为等级右加 3 为军衔左减 4为军衔右加

	ReCruit mReCruit_root;
	void Start () {
	
	    mReCruit_root =ReCruit_root.GetComponent<ReCruit>();
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
		if(BtnType == 1)
		{
			if(mReCruit_root.mlv > mReCruit_root.mlv_min)
			{
				mReCruit_root.mlv -= 1;
				if(mReCruit_root.mlv == mReCruit_root.mlv_min)
				{
					ispress = false;
				}

			}
		}
		if(BtnType == 2)
		{
			if(mReCruit_root.mlv < mReCruit_root.mlv_Max)
			{
				mReCruit_root.mlv += 1;
				if(mReCruit_root.mlv == 100)
				{
					ispress = false;
				}
				
			}
		}
		if(BtnType == 3)
		{
			if(mReCruit_root.mjunxian > mReCruit_root.mjunxian_min)
			{
				mReCruit_root.mjunxian -= 1;
				if(mReCruit_root.mjunxian == 1)
				{
					ispress = false;
				}
				
			}
		}
		if(BtnType == 4)
		{
			if(mReCruit_root.mjunxian < mReCruit_root.mjunxian_Max)
			{
				mReCruit_root.mjunxian += 1;
				if(mReCruit_root.mjunxian == 8)
				{
					ispress = false;
				}
				
			}
		}
		mReCruit_root.init ();
	}
}
