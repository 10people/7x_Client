using UnityEngine;
using System.Collections;

public class XiaoWuInrerface : MonoBehaviour {
	public int QIPao_id;

	int STarTime;
	bool PlayLanguage = false;

	public GameObject mLanguage;
	public UILabel mLanguageLabel;
	void  StopTime()
	{
		STarTime = Random.Range (10,40);
		mLanguage.transform.localScale = Vector3.zero;
		Invoke ("PoP_QiPao",STarTime);
	}
	void  ChangeTimeTolanguage()
	{
		Invoke ("StopTime",3f);
	}
	private void PoP_QiPao()
	{
		PlayLanguage = true;
	}
	void Start () {
	
		Init ();
	}

	void Update () {
	
		if(PlayLanguage)
		{
			Vector3 tempScale = mLanguage.transform.localScale;
			float addNum = 0.05f;
			if (tempScale == Vector3.one)
			{

				PlayLanguage = false;
				addNum = 0.05f;

				ChangeTimeTolanguage ();
			}
			if (tempScale.x < 1&&PlayLanguage)
			{
				tempScale.x += addNum;
				tempScale.y += addNum;
				tempScale.z += addNum;
			}
			mLanguage.transform.localScale = tempScale;
		}
	}
	public void Init()
	{
		LMBubbleTemplate mLMb = LMBubbleTemplate.getLMBubbleTemplateBy_id (QIPao_id);
		mLanguageLabel.text = mLMb.triggerFunc;
		StopTime ();
		PlayLanguage = false;
	}
}
